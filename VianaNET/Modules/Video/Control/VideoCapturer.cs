// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoCapturer.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Windows;

  using DirectShowLib;
  using OpenCvSharp;
  using OpenCvSharp.WpfExtensions;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Logging;

  /// <summary>
  ///   This is the main class for the DirectShow interop.
  ///   It creates a graph that pushes video frames from a Video Input Device
  ///   through the filter chain to a SampleGrabber, from which the
  ///   frames can be catched and send into the processing tree of
  ///   the application.
  /// </summary>
  public class VideoCapturer : VideoBase
  {
    #region Static Fields

    /// <summary>
    ///   The video capture device property.
    /// </summary>
    public static readonly DependencyProperty VideoCaptureDeviceProperty =
      DependencyProperty.Register(
        "VideoCaptureDevice",
        typeof(DsDevice),
        typeof(VideoCapturer),
        new PropertyMetadata(OnVideoCaptureDevicePropertyChanged));

    #endregion

    public VideoCapturer()
    {
      this.bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
      this.bkgWorker.DoWork += this.Worker_DoWork;

      this.opencvCapture = new VideoCapture();

    }

    /// <summary>
    /// Finalizes an instance of the <see cref="VideoCapturer"/> class.
    /// </summary>
    ~VideoCapturer()
    {

      if (this.VideoDeviceFilter != null)
      {
        Marshal.ReleaseComObject(this.VideoDeviceFilter);
        this.VideoDeviceFilter = null;
      }

      if (this.bkgWorker != null)
      {
        this.bkgWorker.CancelAsync();
      }

      if (this.opencvCapture != null)
      {
        this.opencvCapture.Dispose();
      }

    }

    #region Fields

    /// <summary>
    ///   Saves the framerate of the video stream
    /// </summary>
    private double fps;

    /// <summary>
    ///   The frame timer.
    /// </summary>
    private Stopwatch frameTimer;

    /// <summary>
    /// The OpenCV VideoCapture Device Control
    /// </summary>
    private readonly VideoCapture opencvCapture;

    private readonly BackgroundWorker bkgWorker;


    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the framerate of the video stream.
    /// </summary>
    public double FPS => this.fps;

    /// <summary>
    ///   Gets a value indicating whether this capturer is in the PlayState.Running state.
    /// </summary>
    public bool IsRunning => this.CurrentState == PlayState.Running;

    /// <summary>
    ///   Gets or sets the video capture device.
    /// </summary>
    public DsDevice VideoCaptureDevice
    {
      get => (DsDevice)this.GetValue(VideoCaptureDeviceProperty);

      set => this.SetValue(VideoCaptureDeviceProperty, value);
    }

    /// <summary>
    ///   Gets the selected video device
    /// </summary>
    public IBaseFilter VideoDeviceFilter { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Shut down capture.
    ///   This is used to release all resources needed by the capture graph.
    /// </summary>
    public override void Dispose()
    {
      this.Stop();
      base.Dispose();
    }

    /// <summary>
    /// This method creates a new graph for the given capture device and
    ///   properties.
    /// </summary>
    /// <param name="frameRate">
    /// The framerate to use.
    /// </param>
    /// <param name="width">
    /// The width to use.
    /// </param>
    /// <param name="height">
    /// The height to use.
    /// </param>
    public void NewCamera()
    {
      this.Dispose();

      this.frameCounter = 0;
      this.frameTimer = new Stopwatch();

      try
      {

        if (!this.opencvCapture.Open(0, OpenCvSharp.VideoCaptureAPIs.ANY))
        {
          this.opencvCapture.Dispose();
          return;
        }

        Video.Instance.VideoElement.SaveSizeInfo(this.opencvCapture);
        this.fps = this.opencvCapture.Fps;

        if (!this.bkgWorker.IsBusy)
        {
          this.bkgWorker.RunWorkerAsync();
        }
      }
      catch
      {
        this.Dispose();
        ErrorLogger.WriteLine("Error in Camera.Capture(), Could not initialize graphs");
        Video.Instance.HasVideo = false;
        return;
      }

      App.Project.ProcessingData.InitializeImageFilters();

      this.Play();
      Video.Instance.HasVideo = true;
      this.OnVideoAvailable();
    }

    /// <summary>
    ///   The play.
    /// </summary>
    public override void Play()
    {
      base.Play();
      if (!this.bkgWorker.IsBusy)
      {
        this.bkgWorker.RunWorkerAsync();
      }

      this.frameTimer.Start();
    }

    /// <summary>
    ///   The revert.
    /// </summary>
    public override void Revert()
    {
      base.Revert();
      this.frameTimer.Reset();
    }

    /// <summary>
    ///   The show property page of video device.
    /// </summary>
    public void ShowPropertyPageOfVideoDevice()
    {
      if (this.VideoDeviceFilter != null)
      {
        DShowUtils.DisplayPropertyPage(IntPtr.Zero, this.VideoDeviceFilter);
      }
    }

    /// <summary>
    ///   The stop.
    /// </summary>
    public override void Stop()
    {
      this.bkgWorker.CancelAsync();
      if (this.frameTimer != null)
      {
        this.frameTimer.Stop();
      }

      base.Stop();
    }

    /// <summary>
    /// Resets the frame timing, sets frame counter to zero.
    /// </summary>
    public void ResetFrameTiming()
    {
      this.frameTimer.Restart();
      this.frameCounter = 0;
    }

    #endregion

    #region Methods

    /// <summary>
    ///   The on video frame changed.
    /// </summary>
    protected override void OnVideoFrameChanged()
    {
      this.UpdateFrameNumberAndMediatime();
      base.OnVideoFrameChanged();
    }


    /// <summary>
    /// The on video capture device property changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    private static void OnVideoCaptureDevicePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      if (args.NewValue as DsDevice == null)
      {
        return;
      }

      //if (Video.Instance.VideoMode != VideoMode.Capture)
      //{
      //  return;
      //}

      if (obj is VideoCapturer videoCapturer)
      {
        DsDevice device = args.NewValue as DsDevice;
        if (videoCapturer.VideoDeviceFilter != null)
        {
          Marshal.ReleaseComObject(videoCapturer.VideoDeviceFilter);
        }

        videoCapturer.VideoDeviceFilter = DShowUtils.CreateFilter(FilterCategory.VideoInputDevice, device.Name);
        if (Video.Instance.VideoMode == VideoMode.Capture)
        {
          videoCapturer.NewCamera();
        }
      }
    }

    /// <summary>
    ///   The update frame number and mediatime.
    /// </summary>
    private void UpdateFrameNumberAndMediatime()
    {
      this.MediaPositionFrameIndex = this.frameCounter;
      this.MediaPositionInNanoSeconds = this.frameTimer.ElapsedMilliseconds * 10000;
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = (BackgroundWorker)sender;
      while (!worker.CancellationPending)
      {
        using (Mat frameMat = this.opencvCapture.RetrieveMat())
        {
          // Must create and use WriteableBitmap in the same thread(UI Thread).
          this.Dispatcher.Invoke(() =>
          {
            System.Windows.Media.Imaging.WriteableBitmap newFrame = frameMat.ToWriteableBitmap();
            Video.Instance.OriginalImageSource = newFrame;
            Video.Instance.VideoElement.NewFrameCallback(newFrame);
          });
        }

        Thread.Sleep(30);
      }
    }

    #endregion
  }
}