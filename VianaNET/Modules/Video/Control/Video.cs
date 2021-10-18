// <copyright file="Video.cs" company="FU Berlin">
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2015 Dr. Adrian Voßkühler  
//   Licensed under GPL V3
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Drawing;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using SharpDX.MediaFoundation;
  using VianaNET.CustomStyles.Types;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Video.Dialogs;

  /// <summary>
  ///   This class is the base class for all video inputs.
  /// </summary>
  public class Video : DependencyObject, INotifyPropertyChanged
  {


    /// <summary>
    ///   Prevents a default instance of the <see cref="Video" /> class from being created.
    /// </summary>
    private Video()
    {
      // Need to have the construction of the Video objects
      // in constructor, to get the databindings to their 
      // properties to work.
      this.videoPlayerElement = new VideoPlayer();
      this.videoCaptureElement = new VideoCapturer();
      this.videoCaptureElement.VideoAvailable += this.VideoCaptureElementVideoAvailable;
      this.videoElement = this.videoPlayerElement;
      this.videoMode = VideoMode.None;
    }


    /// <summary>
    ///   The is data acquisition running property.
    /// </summary>
    public static readonly DependencyProperty IsDataAcquisitionRunningProperty =
      DependencyProperty.Register(
        "IsDataAcquisitionRunning",
        typeof(bool),
        typeof(Video),
        new FrameworkPropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    ///   The image source property.
    /// </summary>
    public static readonly DependencyProperty OriginalImageSourceProperty =
      DependencyProperty.Register(
        "OriginalImageSource",
        typeof(ImageSource),
        typeof(Video),
        new UIPropertyMetadata(null));

    /// <summary>
    ///   The color processed image source property.
    /// </summary>
    public static readonly DependencyProperty ColorProcessedImageSourceProperty =
      DependencyProperty.Register(
        "ColorProcessedImageSource",
        typeof(ImageSource),
        typeof(Video),
        new UIPropertyMetadata(null));

    /// <summary>
    ///   The motion processed image source property.
    /// </summary>
    public static readonly DependencyProperty MotionProcessedImageSourceProperty =
      DependencyProperty.Register(
        "MotionProcessedImageSource",
        typeof(ImageSource),
        typeof(Video),
        new UIPropertyMetadata(null));

    /// <summary>
    ///   The has video property.
    /// </summary>
    public static readonly DependencyProperty HasVideoProperty = DependencyProperty.Register(
      "HasVideo",
      typeof(bool),
      typeof(Video),
      new UIPropertyMetadata(false));

    /// <summary>
    ///   The instance.
    /// </summary>
    private static Video instance;

    /// <summary>
    ///   The video capture element.
    /// </summary>
    private readonly VideoCapturer videoCaptureElement;

    /// <summary>
    ///   The video player element.
    /// </summary>
    private readonly VideoPlayer videoPlayerElement;

    /// <summary>
    ///   The video element.
    /// </summary>
    private VideoBase videoElement;

    /// <summary>
    ///   The video mode.
    /// </summary>
    private VideoMode videoMode;

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///   The video frame changed.
    /// </summary>
    public event EventHandler VideoFrameChanged;





    /// <summary>
    ///   Gets the <see cref="Video" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static Video Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new Video();
        }

        // return the existing/new instance
        return instance;
      }
    }

    /// <summary>
    ///   Gets the frame index.
    /// </summary>
    public int FrameIndex
    {
      get
      {
        this.videoPlayerElement.UpdateFrameIndex();
        return this.videoElement.MediaPositionFrameIndex;
      }
    }

    public System.Windows.Size FrameSize { get; set; }

    public double FPS { get; set; }

    /// <summary>
    ///   Gets the frame timestamp in ms.
    /// </summary>
    public double FrameTimestampInMs => this.videoElement.MediaPositionInMS - App.Project.VideoData.TimeZeroPositionInMs;

    /// <summary>
    ///   Gets the frame timestamp in ms starting with zero at the beginning of the video.
    /// </summary>
    public double FrameTimestampInMsWithoutOffest => this.videoElement.MediaPositionInMS;

    /// <summary>
    ///   Gets or sets a value indicating whether is data acquisition running.
    /// </summary>
    public bool IsDataAcquisitionRunning
    {
      get => (bool)this.GetValue(IsDataAcquisitionRunningProperty);

      set => this.SetValue(IsDataAcquisitionRunningProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether has video.
    /// </summary>
    public bool HasVideo
    {
      get => (bool)this.GetValue(HasVideoProperty);

      set => this.SetValue(HasVideoProperty, value);
    }

    /// <summary>
    ///   Gets the video capturer element.
    /// </summary>
    public VideoCapturer VideoCapturerElement => this.videoCaptureElement;

    /// <summary>
    ///   Gets the video element.
    /// </summary>
    public VideoBase VideoElement => this.videoElement;

    /// <summary>
    ///   Gets or sets the video mode.
    /// </summary>
    public VideoMode VideoMode
    {
      get => this.videoMode;

      set => this.SetVideoMode(value);
    }

    public bool IsLiveCamera => this.videoMode == VideoMode.Capture;

    /// <summary>
    ///   Gets the video player element.
    /// </summary>
    public VideoPlayer VideoPlayerElement => this.videoPlayerElement;

    /// <summary>
    ///   Gets or sets the image source.
    /// </summary>
    public ImageSource OriginalImageSource
    {
      get => (ImageSource)this.GetValue(OriginalImageSourceProperty);

      set => this.SetValue(OriginalImageSourceProperty, value);
    }

    /// <summary>
    ///   Gets or sets the image source of the color processed video.
    /// </summary>
    public ImageSource ColorProcessedImageSource
    {
      get => (ImageSource)this.GetValue(ColorProcessedImageSourceProperty);

      set => this.SetValue(ColorProcessedImageSourceProperty, value);
    }

    /// <summary>
    ///   Gets or sets the image source of the motion processed video.
    /// </summary>
    public ImageSource MotionProcessedImageSource
    {
      get => (ImageSource)this.GetValue(MotionProcessedImageSourceProperty);

      set => this.SetValue(MotionProcessedImageSourceProperty, value);
    }

    /// <summary>
    /// Gets the video input devices of this computer using Microsoft Media Foundation
    /// </summary>
    public ObservableCollection<CameraDevice> VideoInputDevicesMSMF
    {
      get
      {
        {
          var cameras = new ObservableCollection<CameraDevice>();
          var attributes = new MediaAttributes(1);
          attributes.Set(CaptureDeviceAttributeKeys.SourceType.Guid, CaptureDeviceAttributeKeys.SourceTypeVideoCapture.Guid);
          var devices = MediaFactory.EnumDeviceSources(attributes);
          for (var i = 0; i < devices.Count(); i++)
          {
            var friendlyName = devices[i].Get(CaptureDeviceAttributeKeys.FriendlyName);
            cameras.Add(new CameraDevice(i, friendlyName));
          }

          return cameras;
        }
      }
    }

    /// <summary>
    ///   Gets a valaue indicating whether there are video input devices
    ///   available on the system
    /// </summary>
    public bool HasVideoInputDevices => this.VideoInputDevicesMSMF.Count > 0;

    /// <summary>
    ///   The cleanup.
    /// </summary>
    public void Cleanup()
    {
      this.videoCaptureElement.Dispose();
      this.videoPlayerElement.Dispose();
    }

    /// <summary>
    ///   The create bitmap from current image source.
    /// </summary>
    /// <returns> The <see cref="Bitmap" /> . </returns>
    public Bitmap CreateBitmapFromCurrentImageSource()
    {
      if (this.videoElement.NaturalVideoWidth <= 0)
      {
        return null;
      }

      Bitmap returnBitmap;
      DrawingVisual visual = new DrawingVisual();
      DrawingContext dc = visual.RenderOpen();
      dc.DrawImage(
        this.OriginalImageSource,
        new Rect(0, 0, this.videoElement.NaturalVideoWidth, this.videoElement.NaturalVideoHeight));

      dc.Close();
      RenderTargetBitmap rtp = new RenderTargetBitmap(
        (int)this.videoElement.NaturalVideoWidth,
        (int)this.videoElement.NaturalVideoHeight,
        96d,
        96d,
        PixelFormats.Default);
      rtp.Render(visual);

      using (MemoryStream outStream = new MemoryStream())
      {
        PngBitmapEncoder pnge = new PngBitmapEncoder();
        pnge.Frames.Add(BitmapFrame.Create(rtp));
        pnge.Save(outStream);
        returnBitmap = new Bitmap(outStream);

        // returnBitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
      }

      return returnBitmap;
    }

    /// <summary>
    ///   Loads the movie with the given filename
    /// </summary>
    /// <param name="filename">The filename with full path</param>
    /// <returns>True if successful, otherwise false.</returns>
    public bool LoadMovie(string filename)
    {
      bool success = true;
      switch (this.videoMode)
      {
        case VideoMode.File:
          this.videoPlayerElement.Dispose();
          success = this.videoPlayerElement.LoadMovie(filename);
          if (!success && File.Exists(filename))
          {
            this.videoPlayerElement.Dispose();
            return false;
          }

          StatusBarContent.Instance.VideoFilename = this.videoPlayerElement.VideoFilename;
          break;
        case VideoMode.Capture:

          // Do not render, because the mapped view is already
          // populated with the video data
          break;
      }

      return success;
    }

    /// <summary>
    ///   Pauses the video element.
    /// </summary>
    public void Pause()
    {
      this.videoElement.Pause();
    }

    /// <summary>
    ///   Starts playing the video.
    /// </summary>
    public void Play()
    {
      this.videoElement.Play();
    }

    /// <summary>
    ///   Refreshes the processing map.
    /// </summary>
    public void RefreshProcessingMap()
    {
      this.videoElement.RefreshProcessingMap();
    }

    /// <summary>
    ///   Reverts the video to start position.
    /// </summary>
    public void Revert()
    {
      this.videoElement.Revert();
    }

    /// <summary>
    ///   This method steps the video the given number of frames in the given direction
    /// </summary>
    /// <param name="forward">
    ///   True, if we should go forward in the video stream.
    ///   False to go backwards.
    /// </param>
    /// <param name="count">The number of frames to move</param>
    public void StepFrames(bool forward, int count)
    {
      switch (this.videoMode)
      {
        case VideoMode.File:
          this.videoPlayerElement.StepFrames(forward, count);
          break;
        case VideoMode.Capture:
          // Do nothing
          break;
      }
    }

    /// <summary>
    ///   This method steps the video one frame in the given direction
    /// </summary>
    /// <param name="forward">
    ///   True, if we should go forward in the video stream.
    ///   False to go backwards.
    /// </param>
    public void StepOneFrame(bool forward)
    {
      switch (this.videoMode)
      {
        case VideoMode.File:
          this.videoPlayerElement.StepOneFrame(forward);
          break;
        case VideoMode.Capture:

          // Do nothing
          break;
      }
    }

    /// <summary>
    ///   The stop.
    /// </summary>
    public void Stop()
    {
      this.videoElement.Stop();
    }

    /// <summary>
    ///   The on property changed.
    /// </summary>
    /// <param name="args">
    ///   The args.
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(args.Property.Name));
    }

    /// <summary>
    ///   Called when property changed.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is Video video)
      {
        video.OnPropertyChanged(args);
      }
    }

    /// <summary>
    ///   The on video frame changed.
    /// </summary>
    private void OnVideoFrameChanged()
    {
      this.VideoFrameChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///   The set video mode.
    /// </summary>
    /// <param name="newVideoMode">
    ///   The new video mode.
    /// </param>
    private void SetVideoMode(VideoMode newVideoMode)
    {
      if (this.videoMode == newVideoMode)
      {
        return;
      }

      this.videoElement.VideoFrameChanged -= this.VideoElementVideoFrameChanged;
      this.videoMode = newVideoMode;

      switch (this.videoMode)
      {
        case VideoMode.File:
        case VideoMode.None:
          this.videoCaptureElement.Dispose();
          this.videoElement = this.videoPlayerElement;
          break;
        case VideoMode.Capture:
          if (this.VideoInputDevicesMSMF.Any())
          {
            this.videoElement = this.videoCaptureElement;
            this.videoCaptureElement.NewCamera(Video.Instance.VideoCapturerElement.VideoCaptureDevice.Index);
            StatusBarContent.Instance.VideoFilename = "Live Video";
          }

          break;
      }

      this.videoElement.VideoFrameChanged += this.VideoElementVideoFrameChanged;
    }

    /// <summary>
    ///   Handles the VideoAvailable event of the videoCaptureElement control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void VideoCaptureElementVideoAvailable(object sender, EventArgs e)
    {
      this.videoElement.Play();
    }

    /// <summary>
    ///   Handles the VideoFrameChanged event of the videoElement control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void VideoElementVideoFrameChanged(object sender, EventArgs e)
    {
      this.OnVideoFrameChanged();
    }
  }
}