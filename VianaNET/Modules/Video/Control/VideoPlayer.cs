using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaInfoLib;
using Microsoft.Win32;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Interop;

namespace VianaNET
{

  public class VideoPlayer : VideoBase
  {
    private const int WMGraphNotify = 0x0400 + 13;
    private const int VolumeFull = 0;
    private const int VolumeSilence = -10000;

    //private IMediaEventEx mediaEventEx = null;
    private IVideoWindow videoWindow;
    private IBasicAudio basicAudio;
    private IBasicVideo basicVideo;
    private IMediaSeeking mediaSeeking;
    private IMediaPosition mediaPosition;
    private IVideoFrameStep frameStep;
    private IMediaEvent mediaEvent;

    private Guid timeFormat = TimeFormat.MediaTime;
    private string filename = string.Empty;

    private volatile bool shouldExitEventLoop;

    // Event used by Media Event thread
    private ManualResetEvent manualResetEvent;
    private bool isFrameTimeCapable;

    public event EventHandler StepComplete;
    public event EventHandler FileComplete;
    // 
    //this.openFileDialog1.Filter = @"Video Files (*.avi; *.qt; *.mov; *.mpg; *.mpeg; *.m1v)|*.avi; *.qt; *.mov; *.mpg; *.mpeg; *.m1v|Audio files (*.wav; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd)|*.wav; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd|MIDI Files (*.mid, *.midi, *.rmi)|*.mid; *.midi; *.rmi|Image Files (*.jpg, *.bmp, *.gif, *.tga)|*.jpg; *.bmp; *.gif; *.tga|All Files (*.*)|*.*";

    public bool LoadMovie(string fileName)
    {
      try
      {
        if (!File.Exists(fileName))
        {
          OpenFileDialog ofd = new OpenFileDialog();
          ofd.CheckFileExists = true;
          ofd.CheckPathExists = true;
          ofd.FilterIndex = 1;
          ofd.Filter = Localization.Labels.VideoFilesFilter;
          ofd.Title = Localization.Labels.LoadVideoFilesTitle;
          if (ofd.ShowDialog().Value)
          {
            fileName = ofd.FileName;
          }
          else
          {
            return false;
          }
        }

        this.filename = fileName;

        // Reset status variables
        this.CurrentState = PlayState.Stopped;

        // Add video file to recent files list
        RecentFiles.Instance.Add(fileName);

        MediaInfo videoHeader = new MediaInfo();
        videoHeader.Open(fileName);

        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        string frameRateString = videoHeader.Get(StreamKind.Video, 0, "FrameRate");
        double framerate;
        if (double.TryParse(frameRateString, NumberStyles.AllowDecimalPoint, nfi, out framerate))
        {
          this.FrameTimeInNanoSeconds = (long)(10000000d / framerate);//Math.Round(1000d / framerate, 4);
        }

        videoHeader.Close();

        this.BuildGraph();
        this.ReadVideoProperties();

        this.FrameCount =
          (int)(this.MediaDurationInMS / (this.FrameTimeInNanoSeconds * NanoSecsToMilliSecs));

        this.HasVideo = true;
        Video.Instance.ImageProcessing.InitializeImageFilters();
        this.Revert();
        this.OnVideoAvailable();
      }
      catch (Exception ex)
      {
        this.Dispose();
        ErrorLogger.ProcessException(ex, false);
      }

      return true;
    }

    private void BuildGraph()
    {
      int hr = 0;

      if (this.filename == string.Empty)
        return;

      this.filterGraph = (IFilterGraph2)new FilterGraph();

#if DEBUG
      this.rotEntry = new DsROTEntry(this.filterGraph);
#endif

      //IFileSourceFilter urlSourceFilter = new URLReader() as IFileSourceFilter;
      //IBaseFilter sourceFilter = urlSourceFilter as IBaseFilter;

      //string fileURL = string.Concat(@"file:///", this.filename.Replace("\\","/"));
      //hr = urlSourceFilter.Load(fileURL, null);
      //DsError.ThrowExceptionForHR(hr);
      //this.filterGraph.AddFilter(sourceFilter, "URL Source");

      IBaseFilter sourceFilter;
      this.filterGraph.AddSourceFilter(this.filename, "File Source", out sourceFilter);

      IPin sourceOut = DsFindPin.ByDirection(sourceFilter, PinDirection.Output, 0);

      // Create the SampleGrabber interface
      this.sampleGrabber = (ISampleGrabber)new SampleGrabber();
      IBaseFilter baseGrabFlt = (IBaseFilter)this.sampleGrabber;

      this.ConfigureSampleGrabber(this.sampleGrabber);

      // Add the frame grabber to the graph
      hr = this.filterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
      if (hr != 0)
      {
        ErrorLogger.WriteLine("Error in m_graphBuilder.AddFilter(). Could not add filter. Message: " + DsError.GetErrorText(hr));
      }

      IPin sampleGrabberIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);
      IPin sampleGrabberOut = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);

      hr = this.filterGraph.Connect(sourceOut, sampleGrabberIn);
      DsError.ThrowExceptionForHR(hr);
      hr = this.filterGraph.Render(sampleGrabberOut);
      DsError.ThrowExceptionForHR(hr);

      //// Have the graph builder construct its the appropriate graph automatically
      //hr = this.graphBuilder.RenderFile(filename, null);
      //DsError.ThrowExceptionForHR(hr);

      // QueryInterface for DirectShow interfaces
      this.mediaControl = (IMediaControl)this.filterGraph;
      //this.mediaEventEx = (IMediaEventEx)this.graphBuilder;
      this.mediaSeeking = (IMediaSeeking)this.filterGraph;
      this.mediaPosition = (IMediaPosition)this.filterGraph;
      this.mediaEvent = (IMediaEvent)this.filterGraph;

      hr = this.mediaSeeking.IsFormatSupported(TimeFormat.Frame);
      if (hr != 0)
      {
        this.isFrameTimeCapable = false;
      }
      else
      {
        this.isFrameTimeCapable = true;
        //string text = DsError.GetErrorText(hr);
        //hr = this.mediaSeeking.SetTimeFormat(TimeFormat.Frame);
        //text = DsError.GetErrorText(hr);
      }

      //hr = this.mediaSeeking.GetTimeFormat(out this.timeFormat);
      //DsError.ThrowExceptionForHR(hr);

      // Query for video interfaces, which may not be relevant for audio files
      this.videoWindow = this.filterGraph as IVideoWindow;
      hr = this.videoWindow.put_AutoShow(OABool.False);
      DsError.ThrowExceptionForHR(hr);

      this.basicVideo = this.filterGraph as IBasicVideo;

      // Query for audio interfaces, which may not be relevant for video-only files
      this.basicAudio = this.filterGraph as IBasicAudio;

      //// Have the graph signal event via window callbacks for performance
      //hr = this.mediaEventEx.SetNotifyWindow(this.Handle, WMGraphNotify, IntPtr.Zero);
      //DsError.ThrowExceptionForHR(hr);

      // Get the event handle the graph will use to signal
      // when events occur
      IntPtr hEvent;
      hr = this.mediaEvent.GetEventHandle(out hEvent);
      DsError.ThrowExceptionForHR(hr);

      // Wrap the graph event with a ManualResetEvent
      manualResetEvent = new ManualResetEvent(false);
      manualResetEvent.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(hEvent, true);

      // Create a new thread to wait for events
      Thread t = new Thread(new ThreadStart(this.EventWait));
      t.Name = "Media Event Thread";
      t.Start();

      GetFrameStepInterface();

      // Update the SampleGrabber.
      this.SaveSizeInfo(this.sampleGrabber);
    }

    private int ReadVideoProperties()
    {
      int hr = 0;

      if (this.mediaSeeking == null)
        return 0;

      long mediaduration;
      hr = this.mediaSeeking.GetDuration(out mediaduration);
      DsError.ThrowExceptionForHR(hr);
      this.MediaDurationInMS = mediaduration * NanoSecsToMilliSecs;

      return hr;
    }

    //
    // Some video renderers support stepping media frame by frame with the
    // IVideoFrameStep interface.  See the interface documentation for more
    // details on frame stepping.
    //
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
      else
      {
        this.frameStep = null;
        return false;
      }
    }

    public override void Dispose()
    {
      // Shut down event loop
      this.shouldExitEventLoop = true;

      try
      {
        // Release the thread (if the thread was started)
        if (manualResetEvent != null)
        {
          manualResetEvent.Set();
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
      finally
      {
        manualResetEvent = null;
      }

      // Release DirectShow interfaces
      base.Dispose();

      lock (this)
      {

        //// Release and zero DirectShow interfaces
        if (this.mediaSeeking != null)
          this.mediaSeeking = null;
        if (this.mediaPosition != null)
          this.mediaPosition = null;
        if (this.mediaControl != null)
          this.mediaControl = null;
        if (this.basicAudio != null)
          this.basicAudio = null;
        if (this.basicVideo != null)
          this.basicVideo = null;
        //if (this.videoWindow != null)
        //  this.videoWindow = null;
        if (this.frameStep != null)
          this.frameStep = null;

        // Clear file name to allow selection of new file with open dialog
        this.filename = string.Empty;
      }
    }

    public override void Stop()
    {
      if (this.mediaControl == null)
        return;

      if (this.mediaControl.Pause() >= 0)
      {
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (SendOrPostCallback)delegate
        {
          this.CurrentState = PlayState.Paused;
        }, null);
      }
    }

    public override void Revert()
    {
      base.Revert();

      int hr = 0;
      DsLong zeroPosition = new DsLong((long)(this.SelectionStart / NanoSecsToMilliSecs));
      if (zeroPosition <= 0)
      {
        zeroPosition = 1000;
      }

      if ((this.mediaControl == null) || (this.mediaSeeking == null))
        return;

      // Seek to the beginning
      hr = this.mediaSeeking.SetPositions(
        zeroPosition,
        AMSeekingSeekingFlags.AbsolutePositioning,
        null,
        AMSeekingSeekingFlags.NoPositioning);
      if (hr != 0)
      {
        ErrorLogger.WriteLine("Error while revert video. Message: " + DsError.GetErrorText(hr));
      }

      // Display the first frame to indicate the reset condition
      if (this.mediaControl.Pause() >= 0)
      {
        this.CurrentState = PlayState.Paused;
      }

      UpdateFrameIndex();
    }

    private int StepOneFrame()
    {
      int hr = 0;

      // If the Frame Stepping interface exists, use it to step one frame
      if (this.frameStep != null)
      {
        // The graph must be paused for frame stepping to work
        if (this.CurrentState != PlayState.Paused)
        {
          Pause();
        }

        // Step the requested number of frames, if supported
        hr = this.frameStep.Step(1, null);
      }

      return hr;
    }

    private int StepFrames(int nFramesToStep)
    {
      int hr = 0;

      // If the Frame Stepping interface exists, use it to step frames
      if (this.frameStep != null)
      {
        // The renderer may not support frame stepping for more than one
        // frame at a time, so check for support.  S_OK indicates that the
        // renderer can step nFramesToStep successfully.
        hr = this.frameStep.CanStep(nFramesToStep, null);
        if (hr == 0)
        {
          // The graph must be paused for frame stepping to work
          if (this.CurrentState != PlayState.Paused)
          {
            Pause();
          }

          // Step the requested number of frames, if supported
          hr = this.frameStep.Step(nFramesToStep, null);
        }
      }

      UpdateFrameIndex();

      return hr;
    }

    public void UpdateFrameIndex()
    {
      //long currentTime = this.MediaPositionInNanoSeconds;
      this.MediaPositionFrameIndex = (int)(this.MediaPositionInNanoSeconds / (double)this.FrameTimeInNanoSeconds);
    }

    // Wait for events to happen.  This approach uses waiting on an event handle.
    // The nice thing about doing it this way is that you aren't in the windows message
    // loop, and don't have to worry about re-entrency or taking too long.  Plus, being
    // in a class as we are, we don't have access to the message loop.
    // Alternately, you can receive your events as windows messages.  See
    // IMediaEventEx.SetNotifyWindow.
    private void EventWait()
    {
      // Returned when GetEvent is called but there are no events
      const int E_ABORT = unchecked((int)0x80004004);

      int hr;
      IntPtr p1, p2;
      EventCode ec;

      do
      {
        // Wait for an event
        this.manualResetEvent.WaitOne(-1, true);
        // Make sure that we don't access the media event interface
        // after it has already been released.
        if (this.mediaEvent == null)
          return;

        // Avoid contention for m_State
        lock (this)
        {
          // If we are not shutting down
          if (!this.shouldExitEventLoop)
          {
            // Read the event
            for (
                hr = this.mediaEvent.GetEvent(out ec, out p1, out p2, 0);
                hr >= 0;
                hr = this.mediaEvent.GetEvent(out ec, out p1, out p2, 0)
                )
            {
              // Write the event name to the debug window
              Debug.WriteLine(ec.ToString());

              // If the clip is finished playing
              if (ec == EventCode.Complete)
              {
                // Call Stop() to set state
                Stop();

                // Throw event
                if (FileComplete != null)
                {
                  FileComplete(this, EventArgs.Empty);
                }
              }
              else if (ec == EventCode.StepComplete)
              {
                // Throw event
                if (StepComplete != null)
                {
                  StepComplete(this, EventArgs.Empty);
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
      } while (true);
    }

    //private void HandleGraphEvent()
    //{
    //  int hr = 0;
    //  EventCode evCode;
    //  IntPtr evParam1, evParam2;

    //  // Make sure that we don't access the media event interface
    //  // after it has already been released.
    //  if (this.mediaEventEx == null)
    //    return;

    //  // Process all queued events
    //  while (this.mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
    //  {
    //    // Free memory associated with callback, since we're not using it
    //    hr = this.mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);

    //    // If this is the end of the clip, reset to beginning
    //    if (evCode == EventCode.Complete)
    //    {
    //      DsLong pos = new DsLong(0);
    //      // Reset to first frame of movie
    //      hr = this.mediaSeeking.SetPositions(pos, AMSeekingSeekingFlags.AbsolutePositioning,
    //        null, AMSeekingSeekingFlags.NoPositioning);
    //      if (hr < 0)
    //      {
    //        // Some custom filters (like the Windows CE MIDI filter)
    //        // may not implement seeking interfaces (IMediaSeeking)
    //        // to allow seeking to the start.  In that case, just stop
    //        // and restart for the same effect.  This should not be
    //        // necessary in most cases.
    //        hr = this.mediaControl.Stop();
    //        hr = this.mediaControl.Run();
    //      }
    //    }
    //  }
    //}

    /*
     * WinForm Related methods
     */

    //protected override void WndProc(ref Message m)
    //{
    //  switch (m.Msg)
    //  {
    //    case WMGraphNotify:
    //      {
    //        HandleGraphEvent();
    //        break;
    //      }
    //  }

    //  // Pass this message to the video window for notification of system changes
    //  if (this.videoWindow != null)
    //    this.videoWindow.NotifyOwnerMessage(m.HWnd, m.Msg, m.WParam, m.LParam);

    //  base.WndProc(ref m);
    //}

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS
    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public VideoPlayer()
    {
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    //public override long MediaPositionFrameIndex
    //{
    //  get
    //  {
    //    if (this.mediaSeeking == null)
    //    {
    //      throw new ArgumentNullException("MediaSeekingInterface not implemented");
    //    }

    //    long currentPosition;
    //    long stopPosition;
    //    int hr = this.mediaSeeking.GetPositions(out currentPosition, out stopPosition);
    //    DsError.ThrowExceptionForHR(hr);

    //    long nanoSeconds = currentPosition;
    //    if (this.timeFormat == TimeFormat.Frame)
    //    {
    //      return (long)(currentPosition);
    //    }
    //    else
    //    {
    //      return (long)(nanoSeconds * NanoSecsToMilliSecs / this.FrameTime);
    //    }
    //  }

    //  set
    //  {
    //    if (this.mediaSeeking == null)
    //    {
    //      throw new ArgumentNullException("MediaSeekingInterface not implemented");
    //    }

    //    long newPositionInFrames = value;
    //    if (this.timeFormat == TimeFormat.Frame)
    //    {
    //      int hr = this.mediaSeeking.SetPositions(
    //         new DsLong(newPositionInFrames),
    //         AMSeekingSeekingFlags.AbsolutePositioning,
    //         null,
    //         AMSeekingSeekingFlags.NoPositioning);
    //      DsError.ThrowExceptionForHR(hr);
    //    }
    //  }
    //}

    //public long MediaPositionInDSTime
    //{
    //  get { return (long)GetValue(MediaPositionInDSTimeProperty); }
    //  set { SetValue(MediaPositionInDSTimeProperty, value); }
    //}

    //public static readonly DependencyProperty MediaPositionInDSTimeProperty =
    //  DependencyProperty.Register(
    //  "MediaPositionInDSTime",
    //  typeof(long),
    //  typeof(VideoPlayer),
    //  new UIPropertyMetadata(default(long)));

    //public long MediaPositionInMS
    //{
    //  get
    //  {
    //    return (long)(this.MediaPositionInNanoSeconds * NanoSecsToMilliSecs);
    //  }

    //  set
    //  {
    //    this.MediaPositionInNanoSeconds = (long)(value / NanoSecsToMilliSecs);
    //  }
    //}

    public override long MediaPositionInNanoSeconds
    {
      get
      {
        if (this.mediaSeeking == null)
        {
          ErrorLogger.WriteLine("MediaSeekingInterface not implemented");
          return 0;
        }

        long currentPosition;
        long stopPosition;
        int hr = this.mediaSeeking.GetPositions(out currentPosition, out stopPosition);
        DsError.ThrowExceptionForHR(hr);

        long milliSeconds = currentPosition;
        if (this.timeFormat == TimeFormat.Frame)
        {
          return (long)(currentPosition * this.FrameTimeInNanoSeconds);
        }
        else
        {
          return currentPosition;
        }
      }

      set
      {
        if (this.mediaSeeking == null)
        {
          throw new ArgumentNullException("MediaSeekingInterface not implemented");
        }

        long currentPosition = value;
        if (currentPosition < 0)
        {
          currentPosition = 1000;
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

    public double MediaDurationInMS
    {
      get { return (double)GetValue(MediaDurationInMSProperty); }
      set { SetValue(MediaDurationInMSProperty, value); }
    }

    public static readonly DependencyProperty MediaDurationInMSProperty =
      DependencyProperty.Register(
      "MediaDurationInMS",
      typeof(double),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(double)));

    public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
  "SelectionStart",
  typeof(double),
  typeof(MediaSlider));

    public double SelectionStart
    {
      get { return (double)GetValue(SelectionStartProperty); }
      set { SetValue(SelectionStartProperty, value); }
    }

    public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register(
      "SelectionEnd",
      typeof(double),
      typeof(MediaSlider));

    public double SelectionEnd
    {
      get { return (double)GetValue(SelectionEndProperty); }
      set { SetValue(SelectionEndProperty, value); }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void StepOneFrame(bool forward)
    {
      if (forward)
      {
        if (!this.isFrameTimeCapable || Video.Instance.IsDataAcquisitionRunning)
        {
          this.StepFrames(1);
        }
        else
        {
          this.MediaPositionInNanoSeconds += this.FrameTimeInNanoSeconds;
        }
        //this.MediaPositionInNanoSeconds =
        //  (long)(this.MediaPositionInMilliSeconds / NanoSecsToMilliSecs + this.FrameTimeInNanoSeconds);
      }
      else
      {
        if (this.timeFormat == TimeFormat.Frame)
        {
          this.MediaPositionFrameIndex--;
        }
        else
        {
          this.MediaPositionInNanoSeconds -= this.FrameTimeInNanoSeconds;
          //this.MediaPositionInNanoSeconds =
          //  (long)(this.MediaPositionInMilliSeconds / NanoSecsToMilliSecs - this.FrameTimeInNanoSeconds);
        }
      }
    }


    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER
    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS
    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
