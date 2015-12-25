// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlayer.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2012 Dr. Adrian Voßkühler  
//   ------------------------------------------------------------------------
//   This program is free software; you can redistribute it and/or modify it 
//   under the terms of the GNU General Public License as published by the 
//   Free Software Foundation; either version 2 of the License, or 
//   (at your option) any later version.
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License 
//   along with this program; if not, write to the Free Software Foundation, 
//   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//   ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   The video player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using DirectShowLib;

using VianaNET.Logging;

namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Windows;
  using System.Windows.Forms;
  using System.Windows.Forms.VisualStyles;
  using System.Windows.Threading;

  using DirectShowLib;

  using MediaInfoNET;

  using VianaNET.Application;
  using VianaNET.Logging;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Video.Dialogs;
  using VianaNET.Resources;

  using Vlc.DotNet.Core;
  using Vlc.DotNet.Forms;

  using File = System.IO.File;
  using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

  /// <summary>
  ///   The video player.
  /// </summary>
  public class VideoPlayer : VideoBase
  {
    #region Constants

    /// <summary>
    ///   The volume full.
    /// </summary>
    private const int VolumeFull = 0;

    /// <summary>
    ///   The volume silence.
    /// </summary>
    private const int VolumeSilence = -10000;

    /// <summary>
    ///   The wm graph notify.
    /// </summary>
    private const int WMGraphNotify = 0x0400 + 13;

    #endregion

    #region Static Fields

    /// <summary>
    ///   The media duration in ms property.
    /// </summary>
    public static readonly DependencyProperty MediaDurationInMSProperty =
      DependencyProperty.Register(
        "MediaDurationInMS", typeof(double), typeof(VideoPlayer), new UIPropertyMetadata(default(double)));

    #endregion

    #region Fields

    /// <summary>
    ///   The time format.
    /// </summary>
    private readonly Guid timeFormat = TimeFormat.MediaTime;

    /// <summary>
    ///   The basic audio.
    /// </summary>
    private IBasicAudio basicAudio;

    /// <summary>
    ///   The basic video.
    /// </summary>
    private IBasicVideo basicVideo;

    /// <summary>
    ///   The event thread.
    /// </summary>
    private Thread eventThread;

    /// <summary>
    ///   The frame step.
    /// </summary>
    private IVideoFrameStep frameStep;

    ///// <summary>
    /////   The is frame time capable.
    ///// </summary>
    //private bool isFrameTimeCapable;

    /// <summary>
    ///   The media event.
    /// </summary>
    private IMediaEvent mediaEvent;

    /// <summary>
    ///   The media position.
    /// </summary>
    private IMediaPosition mediaPosition;

    /// <summary>
    ///   The media seeking.
    /// </summary>
    private IMediaSeeking mediaSeeking;

    /// <summary>
    ///   The should exit event loop.
    /// </summary>
    private volatile bool shouldExitEventLoop;

    /// <summary>
    ///   The video window.
    /// </summary>
    private IVideoWindow videoWindow;

    #endregion

    #region Public Events

    /// <summary>
    ///   The file complete.
    /// </summary>
    public event EventHandler FileComplete;

    /// <summary>
    ///   The step complete.
    /// </summary>
    public event EventHandler StepComplete;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the filename of the video file
    /// </summary>
    public string VideoFilename { get; set; }

    /// <summary>
    ///   Gets or sets the media duration in ms.
    /// </summary>
    public double MediaDurationInMS
    {
      get
      {
        return (double)this.GetValue(MediaDurationInMSProperty);
      }

      set
      {
        this.SetValue(MediaDurationInMSProperty, value);
      }
    }

    ///// <summary>
    /////   Gets the video duration in ms, which is the media duration
    /////   times the framerate factor.
    ///// </summary>
    //public double VideoDurationInMs
    //{
    //  get
    //  {
    //    return this.MediaDurationInMS * Viana.Project.VideoData.FramerateFactor;
    //  }
    //}

    /// <summary>
    ///   Gets or sets the media position in nano seconds.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public override long MediaPositionInNanoSeconds
    {
      get
      {
        if (this.mediaSeeking == null)
        {
          return 0;
        }

        long currentPosition;
        //long stopPosition;
        int hr = this.mediaSeeking.GetCurrentPosition(out currentPosition);
        //hr = this.mediaSeeking.GetPositions(out currentPosition, out stopPosition);
        DsError.ThrowExceptionForHR(hr);

        //long milliSeconds = currentPosition;
        //if (this.timeFormat == TimeFormat.Frame)
        //{
        //  return (long)(currentPosition * this.FrameTimeInNanoSeconds);
        //}
        //else
        //{
        return (long)(currentPosition * Viana.Project.VideoData.FramerateFactor);
        //}
      }

      set
      {
        if (this.mediaSeeking == null)
        {
          return;
        }

        long currentPosition = (long)(value / Viana.Project.VideoData.FramerateFactor);
        if (currentPosition < 0)
        {
          currentPosition = 0;
        }

        int hr = this.mediaSeeking.SetPositions(
          new DsLong(currentPosition),
          AMSeekingSeekingFlags.AbsolutePositioning,
          null,
          AMSeekingSeekingFlags.NoPositioning);
        DsError.ThrowExceptionForHR(hr);

        this.UpdateFrameIndex();
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The dispose.
    /// </summary>
    public override void Dispose()
    {
      this.ReleaseEventThread();

      //// Release and zero DirectShow interfaces
      if (this.mediaSeeking != null)
      {
        this.mediaSeeking = null;
      }

      if (this.mediaPosition != null)
      {
        this.mediaPosition = null;
      }

      if (this.mediaControl != null)
      {
        this.mediaControl = null;
      }

      if (this.basicAudio != null)
      {
        this.basicAudio = null;
      }

      if (this.basicVideo != null)
      {
        this.basicVideo = null;
      }

      if (this.mediaEvent != null)
      {
        this.mediaEvent = null;
      }

      if (this.videoWindow != null)
      {
        this.videoWindow = null;
      }

      if (this.frameStep != null)
      {
        this.frameStep = null;
      }

      // Release DirectShow interfaces
      base.Dispose();

      // Clear file name to allow selection of new file with open dialog
      this.VideoFilename = string.Empty;
    }

    // this.openFileDialog1.Filter = @"Video Files (*.avi; *.qt; *.mov; *.mpg; *.mpeg; *.m1v)|*.avi; *.qt; *.mov; *.mpg; *.mpeg; *.m1v|Audio files (*.wav; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd)|*.wav; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd|MIDI Files (*.mid, *.midi, *.rmi)|*.mid; *.midi; *.rmi|Image Files (*.jpg, *.bmp, *.gif, *.tga)|*.jpg; *.bmp; *.gif; *.tga|All Files (*.*)|*.*";

    /// <summary>
    /// The load movie.
    /// </summary>
    /// <param name="fileName">
    /// The file name. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool LoadMovie(string fileName)
    {
      this.ReleaseEventThread();
    Start:
      try
      {
        if (!File.Exists(fileName))
        {
          if (fileName != string.Empty)
          {
            var messageTitle = Labels.AskVideoNotFoundMessageTitle;
            messageTitle = messageTitle.Replace("%1", Path.GetFileName(fileName));
            messageTitle = messageTitle.Replace("%2", Path.GetDirectoryName(fileName));

            var dlg = new VianaDialog(Labels.AskVideoNotFoundTitle, messageTitle, Labels.AskVideoNotFoundMessage, false);
            if (!dlg.ShowDialog().GetValueOrDefault(false))
            {
              return false;
            }
          }

          var ofd = new OpenFileDialog();
          ofd.CheckFileExists = true;
          ofd.CheckPathExists = true;
          ofd.FilterIndex = 1;
          ofd.Filter = Labels.VideoFilesFilter;
          ofd.Title = Labels.LoadVideoFilesTitle;
          if (ofd.ShowDialog().Value)
          {
            fileName = ofd.FileName;
          }
          else
          {
            return false;
          }
        }

        this.VideoFilename = fileName;

        // Reset status variables
        this.CurrentState = PlayState.Stopped;

        // Read out video properties
        var aviFile = new MediaFile(this.VideoFilename);
        this.FrameTimeInNanoSeconds = (long)(10000000d / aviFile.Video[0].FrameRate) + 1;
        this.MediaDurationInMS = aviFile.General.DurationMillis;
        this.FrameCount = aviFile.FrameCount;
        Viana.Project.VideoData.FramerateFactor = 1;
        try
        {
          this.BuildGraph();
        }
        catch (Exception)
        {
          // Store filename
          //var file = this.VideoFilename;
          //this.Dispose();
          //this.ReRenderVideoFile(file);
          //fileName = Viana.Project.VideoFile;
          //goto Start;
          return false;
        }

        Viana.Project.VideoFile = this.VideoFilename;
        Video.Instance.HasVideo = true;
        Viana.Project.ProcessingData.InitializeImageFilters();
        this.Revert();
        this.OnVideoAvailable();
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
        this.Dispose();
      }

      return true;
    }


    /// <summary>
    ///   The revert.
    /// </summary>
    public override void Revert()
    {
      base.Revert();

      int hr = 0;
      var zeroPosition = new DsLong((long)(Viana.Project.VideoData.SelectionStart / NanoSecsToMilliSecs));

      // if (zeroPosition <= 0)
      // {
      // zeroPosition = 1000;
      // }
      if (zeroPosition < 0)
      {
        zeroPosition = 0;
      }

      if ((this.mediaControl == null) || (this.mediaSeeking == null))
      {
        return;
      }

      // Seek to the beginning
      hr = this.mediaSeeking.SetPositions(
        zeroPosition, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
      if (hr != 0)
      {
        ErrorLogger.WriteLine("Error while revert video. Message: " + DsError.GetErrorText(hr));
      }

      // Display the first frame to indicate the reset condition
      if (this.mediaControl.Pause() >= 0)
      {
        this.CurrentState = PlayState.Paused;
      }

      this.UpdateFrameIndex();
    }

    /// <summary>
    /// This method steps the video the given number of frames in the given direction
    /// </summary>
    /// <param name="forward">True, if we should go forward in the video stream. 
    /// False to go backwards. </param>
    /// <param name="count">The number of frames to move</param>
    public void StepFrames(bool forward, int count)
    {
      if (forward)
      {
        //if (!this.isFrameTimeCapable || Video.Instance.IsDataAcquisitionRunning)
        //{
        //  this.StepFrames(count);
        //}
        //else
        //{
        this.MediaPositionInNanoSeconds += this.FrameTimeInNanoSeconds * count;
        //}
      }
      else
      {
        //if (this.timeFormat == TimeFormat.Frame)
        //{
        //  this.MediaPositionFrameIndex = this.MediaPositionFrameIndex - count;
        //}
        //else
        //{
        this.MediaPositionInNanoSeconds -= this.FrameTimeInNanoSeconds * count;
        //}
      }

      //// Throw event
      //if (this.StepComplete != null)
      //{
      //  this.StepComplete(this, EventArgs.Empty);
      //}
    }

    /// <summary>
    /// This method steps the video one frame in the given direction
    /// </summary>
    /// <param name="forward">True, if we should go forward in the video stream. 
    /// False to go backwards. </param>
    public void StepOneFrame(bool forward)
    {
      if (forward)
      {
        if (Video.Instance.IsDataAcquisitionRunning)
        {
          this.StepFrames(1);
        }

        //if (!this.isFrameTimeCapable)// || Video.Instance.IsDataAcquisitionRunning)
        //{
        //  this.StepFrames(1);
        //}
        else
        {
          this.MediaPositionInNanoSeconds += this.FrameTimeInNanoSeconds;
        }
      }
      else
      {
        //if (this.timeFormat == TimeFormat.Frame)
        //{
        //  this.MediaPositionFrameIndex--;
        //}
        //else
        //{
        this.MediaPositionInNanoSeconds -= this.FrameTimeInNanoSeconds;
        //}
      }

      //// Throw event
      //if (this.StepComplete != null)
      //{
      //  this.StepComplete(this, EventArgs.Empty);
      //}

    }

    /// <summary>
    ///   The stop.
    /// </summary>
    public override void Stop()
    {
      if (this.mediaControl == null)
      {
        return;
      }

      if (this.mediaControl.Pause() >= 0)
      {
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Normal, (SendOrPostCallback)delegate { this.CurrentState = PlayState.Paused; }, null);
      }
    }

    /// <summary>
    ///   The update frame index.
    /// </summary>
    public void UpdateFrameIndex()
    {
      // long currentTime = this.MediaPositionInNanoSeconds;
      double index = this.MediaPositionInNanoSeconds / (double)this.FrameTimeInNanoSeconds;
      this.MediaPositionFrameIndex = (int)Math.Round(index);
    }

    #endregion

    #region Methods

    /// <summary>
    ///   The build graph.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void BuildGraph()
    {
      if (this.VideoFilename == string.Empty)
      {
        return;
      }

      this.filterGraph = (IFilterGraph2)new FilterGraph();

      // #if DEBUG
      this.rotEntry = new DsROTEntry(this.filterGraph);

      // #endif

      // IFileSourceFilter urlSourceFilter = new URLReader() as IFileSourceFilter;
      // IBaseFilter sourceFilter = urlSourceFilter as IBaseFilter;

      // string fileURL = string.Concat(@"file:///", this.filename.Replace("\\","/"));
      // hr = urlSourceFilter.Load(fileURL, null);
      // DsError.ThrowExceptionForHR(hr);
      // this.filterGraph.AddFilter(sourceFilter, "URL Source");
      IBaseFilter sourceFilter;
      this.filterGraph.AddSourceFilter(this.VideoFilename, "File Source", out sourceFilter);

      // Create the SampleGrabber interface
      this.sampleGrabber = (ISampleGrabber)new SampleGrabber();
      var baseGrabFlt = (IBaseFilter)this.sampleGrabber;

      this.ConfigureSampleGrabber(this.sampleGrabber);

      // Add the frame grabber to the graph
      int hr = this.filterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
      if (hr != 0)
      {
        ErrorLogger.WriteLine(
          "Error in m_graphBuilder.AddFilter(). Could not add filter. Message: " + DsError.GetErrorText(hr));
      }

      IPin sampleGrabberIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);
      IPin sampleGrabberOut = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
      IPin sourceOut;

      // Iterate through source output pins, to find video output pin to be connected to
      // the sample grabber
      int i = 0;
      do
      {
        sourceOut = DsFindPin.ByDirection(sourceFilter, PinDirection.Output, i);
        if (sourceOut == null)
        {
          throw new ArgumentOutOfRangeException("Found no compatible video source output pin");
        }

        hr = this.filterGraph.Connect(sourceOut, sampleGrabberIn);
        i++;
      }
      while (hr < 0);

      DsError.ThrowExceptionForHR(hr);
      hr = this.filterGraph.Render(sampleGrabberOut);
      DsError.ThrowExceptionForHR(hr);

      //// Have the graph builder construct its the appropriate graph automatically
      // hr = this.graphBuilder.RenderFile(filename, null);
      // DsError.ThrowExceptionForHR(hr);

      // QueryInterface for DirectShow interfaces
      this.mediaControl = (IMediaControl)this.filterGraph;

      // this.mediaEventEx = (IMediaEventEx)this.graphBuilder;
      this.mediaSeeking = (IMediaSeeking)this.filterGraph;
      this.mediaPosition = (IMediaPosition)this.filterGraph;
      this.mediaEvent = (IMediaEvent)this.filterGraph;

      //hr = this.mediaSeeking.IsFormatSupported(TimeFormat.Frame);
      //if (hr != 0)
      //{
      //  this.isFrameTimeCapable = false;
      //}
      //else
      //{
      //  this.isFrameTimeCapable = true;

      // string text = DsError.GetErrorText(hr);
      // hr = this.mediaSeeking.SetTimeFormat(TimeFormat.Frame);
      // text = DsError.GetErrorText(hr);
      //}

      // hr = this.mediaSeeking.GetTimeFormat(out this.timeFormat);
      // DsError.ThrowExceptionForHR(hr);

      // Query for video interfaces, which may not be relevant for audio files
      this.videoWindow = this.filterGraph as IVideoWindow;
      hr = this.videoWindow.put_AutoShow(OABool.False);
      DsError.ThrowExceptionForHR(hr);

      this.basicVideo = this.filterGraph as IBasicVideo;

      // Query for audio interfaces, which may not be relevant for video-only files
      this.basicAudio = this.filterGraph as IBasicAudio;

      //// Have the graph signal event via window callbacks for performance
      // hr = this.mediaEventEx.SetNotifyWindow(this.Handle, WMGraphNotify, IntPtr.Zero);
      // DsError.ThrowExceptionForHR(hr);

      // Get the event handle the graph will use to signal
      // when events occur
      IntPtr hEvent;
      hr = this.mediaEvent.GetEventHandle(out hEvent);
      DsError.ThrowExceptionForHR(hr);

      // Reset event loop exit flag
      this.shouldExitEventLoop = false;

      // Create a new thread to wait for events
      this.eventThread = new Thread(this.EventWait);
      this.eventThread.Name = "Media Event Thread";
      this.eventThread.Start();

      this.GetFrameStepInterface();

      // Update the SampleGrabber.
      this.SaveSizeInfo(this.sampleGrabber);
    }

    /// <summary>
    ///   The event wait.
    /// </summary>
    private void EventWait()
    {
      // Returned when GetEvent is called but there are no events
      const int E_ABORT = unchecked((int)0x80004004);

      int hr;
      IntPtr p1, p2;
      EventCode ec;

      // Console.WriteLine("LoopStarted");
      do
      {
        // If we are shutting down
        if (this.shouldExitEventLoop)
        {
          break;
        }

        // Make sure that we don't access the media event interface
        // after it has already been released.
        if (this.mediaEvent == null)
        {
          return;
        }

        // Avoid contention for m_State
        lock (this)
        {
          // If we are not shutting down
          if (!this.shouldExitEventLoop)
          {
            // Read the event
            for (hr = this.mediaEvent.GetEvent(out ec, out p1, out p2, 100);
                 hr >= 0;
                 hr = this.mediaEvent.GetEvent(out ec, out p1, out p2, 100))
            {
              // Console.WriteLine("InLoop");
              //// Write the event name to the debug window
              // Debug.WriteLine(ec.ToString());

              // If the clip is finished playing
              if (ec == EventCode.Complete)
              {
                // Call Stop() to set state
                this.Stop();

                // Throw event
                if (this.FileComplete != null)
                {
                  this.FileComplete(this, EventArgs.Empty);
                }
              }
              else if (ec == EventCode.StepComplete)
              {
                // Throw event
                if (this.StepComplete != null)
                {
                  this.StepComplete(this, EventArgs.Empty);
                }
              }

              // Release any resources the message allocated
              hr = this.mediaEvent.FreeEventParams(ec, p1, p2);
              DsError.ThrowExceptionForHR(hr);
            }

            // If the error that exited the loop wasn't due to running out of events
            if (hr != E_ABORT)
            {
              DsError.ThrowExceptionForHR(hr);
            }
          }
          else
          {
            // We are shutting down
            break;
          }
        }
      }
      while (true);

      // Console.WriteLine("LoopExited");
    }


    /// <summary>
    /// The get frame step interface.
    /// Some video renderers support stepping media frame by frame with the
    /// IVideoFrameStep interface.  See the interface documentation for more
    /// details on frame stepping.
    /// </summary>
    /// <returns> True, if frame step interface is available, otherwise false </returns>
    private bool GetFrameStepInterface()
    {
      int hr = 0;

      IVideoFrameStep frameStepTest = null;

      // Get the frame step interface, if supported
      frameStepTest = (IVideoFrameStep)this.filterGraph;

      // Check if this decoder can step
      hr = frameStepTest.CanStep(0, null);
      if (hr == 0)
      {
        this.frameStep = frameStepTest;
        return true;
      }

      this.frameStep = null;
      return false;
    }

    ///// <summary>
    /////   The read video properties.
    ///// </summary>
    ///// <returns> The <see cref="int" /> . </returns>
    //private int ReadVideoProperties()
    //{
    //  int hr = 0;

    //  if (this.mediaSeeking == null)
    //  {
    //    return 0;
    //  }

    //  long mediaduration;
    //  hr = this.mediaSeeking.GetDuration(out mediaduration);
    //  DsError.ThrowExceptionForHR(hr);
    //  this.MediaDurationInMS = mediaduration * NanoSecsToMilliSecs;

    //  return hr;
    //}

    /// <summary>
    ///   The release event thread.
    /// </summary>
    private void ReleaseEventThread()
    {
      // Shut down event loop
      this.shouldExitEventLoop = true;

      // Wait for shutdown
      if (this.eventThread != null)
      {
        this.eventThread.Join(500);
      }
    }

    /// <summary>
    /// The step frames.
    /// </summary>
    /// <param name="numberOfFramesToStep">
    /// The n frames to step. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    private int StepFrames(int numberOfFramesToStep)
    {
      //Console.WriteLine("StepFrames: #" + Video.Instance.FrameIndex);
      int hr = 0;

      // If the Frame Stepping interface exists, use it to step frames
      if (this.frameStep != null)
      {
        // The renderer may not support frame stepping for more than one
        // frame at a time, so check for support.  S_OK indicates that the
        // renderer can step nFramesToStep successfully.
        hr = this.frameStep.CanStep(numberOfFramesToStep, null);
        if (hr == 0)
        {
          // The graph must be paused for frame stepping to work
          if (this.CurrentState != PlayState.Paused)
          {
            this.Pause();
          }

          // Step the requested number of frames, if supported
          hr = this.frameStep.Step(numberOfFramesToStep, null);
        }
      }

      this.UpdateFrameIndex();

      return hr;
    }

    #endregion
  }
}