// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Video.cs" company="Freie Universität Berlin">
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
//   The video.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Drawing;
  using System.IO;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using DirectShowLib;

  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   The video.
  /// </summary>
  public class Video : DependencyObject, INotifyPropertyChanged
  {
    #region Static Fields

    /// <summary>
    ///   The is data acquisition running property.
    /// </summary>
    public static readonly DependencyProperty IsDataAcquisitionRunningProperty =
      DependencyProperty.Register(
        "IsDataAcquisitionRunning", typeof(bool), typeof(Video), new FrameworkPropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    ///   The video source property.
    /// </summary>
    public static readonly DependencyProperty VideoSourceProperty = DependencyProperty.Register(
      "VideoSource", typeof(ImageSource), typeof(Video), new UIPropertyMetadata(null));

    /// <summary>
    ///   The has video property.
    /// </summary>
    public static readonly DependencyProperty HasVideoProperty = DependencyProperty.Register(
      "HasVideo", typeof(bool), typeof(Video), new UIPropertyMetadata(false));

    /// <summary>
    ///   The instance.
    /// </summary>
    private static Video instance;

    #endregion

    #region Fields

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

    #endregion

    #region Constructors and Destructors

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
      this.videoCaptureElement.VideoAvailable += this.videoCaptureElement_VideoAvailable;
      this.videoElement = this.videoPlayerElement;
      this.videoMode = VideoMode.None;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///   The video frame changed.
    /// </summary>
    public event EventHandler VideoFrameChanged;

    #endregion

    #region Public Properties

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
        return this.videoElement.MediaPositionFrameIndex;
      }
    }

    /// <summary>
    /// Gets or sets the time position in milliseconds, where
    /// the video time should be zero.
    /// </summary>
    public long TimeZeroPositionInMs { get; set; }

    /// <summary>
    ///   Gets the frame timestamp in ms.
    /// </summary>
    public long FrameTimestampInMs
    {
      get
      {
        return (long)(this.videoElement.MediaPositionInNanoSeconds * VideoBase.NanoSecsToMilliSecs) - this.TimeZeroPositionInMs;
      }

      // get { return this.videoElement.MediaPositionInMilliSeconds; }
    }

    /// <summary>
    ///   Gets the frame timestamp in ms starting with zero at the beginning of the video.
    /// </summary>
    public long FrameTimestampInMsWithoutOffest
    {
      get
      {
        return (long)(this.videoElement.MediaPositionInNanoSeconds * VideoBase.NanoSecsToMilliSecs);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether is data acquisition running.
    /// </summary>
    public bool IsDataAcquisitionRunning
    {
      get
      {
        return (bool)this.GetValue(IsDataAcquisitionRunningProperty);
      }

      set
      {
        this.SetValue(IsDataAcquisitionRunningProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether has video.
    /// </summary>
    public bool HasVideo
    {
      get
      {
        return (bool)this.GetValue(HasVideoProperty);
      }

      set
      {
        this.SetValue(HasVideoProperty, value);
      }
    }

    /// <summary>
    ///   Gets the video capturer element.
    /// </summary>
    public VideoCapturer VideoCapturerElement
    {
      get
      {
        return this.videoCaptureElement;
      }
    }

    /// <summary>
    ///   Gets the video element.
    /// </summary>
    public VideoBase VideoElement
    {
      get
      {
        return this.videoElement;
      }
    }

    /// <summary>
    ///   Gets or sets the video mode.
    /// </summary>
    public VideoMode VideoMode
    {
      get
      {
        return this.videoMode;
      }

      set
      {
        this.SetVideoMode(value);
      }
    }

    /// <summary>
    ///   Gets the video player element.
    /// </summary>
    public VideoPlayer VideoPlayerElement
    {
      get
      {
        return this.videoPlayerElement;
      }
    }

    /// <summary>
    ///   Gets or sets the video source.
    /// </summary>
    public ImageSource VideoSource
    {
      get
      {
        return (ImageSource)this.GetValue(VideoSourceProperty);
      }

      set
      {
        this.SetValue(VideoSourceProperty, value);
      }
    }

    /// <summary>
    /// Gets the video input devices.
    /// </summary>
    public ObservableCollection<DsDevice> VideoInputDevices
    {
      get
      {
        return new ObservableCollection<DsDevice>(DShowUtils.GetVideoInputDevices());
      }
    }

    /// <summary>
    /// Gets a valaue indicating whether there are video input devices
    /// available on the system
    /// </summary>
    public bool HasVideoInputDevices
    {
      get
      {
        return this.VideoInputDevices.Count > 0;
      }
    }

    #endregion

    #region Public Methods and Operators

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
      var visual = new DrawingVisual();
      DrawingContext dc = visual.RenderOpen();
      dc.DrawImage(
        this.videoElement.ImageSource,
        new Rect(0, 0, this.videoElement.NaturalVideoWidth, this.videoElement.NaturalVideoHeight));

      dc.Close();
      var rtp = new RenderTargetBitmap(
        (int)this.videoElement.NaturalVideoWidth,
        (int)this.videoElement.NaturalVideoHeight,
        96d,
        96d,
        PixelFormats.Default);
      rtp.Render(visual);

      using (var outStream = new MemoryStream())
      {
        var pnge = new PngBitmapEncoder();
        pnge.Frames.Add(BitmapFrame.Create(rtp));
        pnge.Save(outStream);
        returnBitmap = new Bitmap(outStream);

        // returnBitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
      }

      return returnBitmap;
    }

    // public void UpdateNativeBitmap()
    // {
    // switch (this.videoMode)
    // {
    // case VideoMode.File:
    // this.videoPlayerElement.UpdateNativeBitmap();
    // break;
    // case VideoMode.Capture:
    // break;
    // }
    // }

    /// <summary>
    /// The load movie.
    /// </summary>
    /// <param name="filename">
    /// The filename. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool LoadMovie(string filename)
    {
      bool success = true;
      switch (this.videoMode)
      {
        case VideoMode.File:
          success = this.videoPlayerElement.LoadMovie(filename);
          this.VideoSource = this.videoElement.ImageSource;
          break;
        case VideoMode.Capture:

          // Do not render, because the mapped view is already
          // populated with the video data
          break;
      }

      return success;
    }

    /// <summary>
    ///   The pause.
    /// </summary>
    public void Pause()
    {
      this.videoElement.Pause();
    }

    /// <summary>
    ///   The play.
    /// </summary>
    public void Play()
    {
      this.videoElement.Play();
    }

    /// <summary>
    ///   The refresh processing map.
    /// </summary>
    public void RefreshProcessingMap()
    {
      this.videoElement.RefreshProcessingMap();
    }

    /// <summary>
    ///   The revert.
    /// </summary>
    public void Revert()
    {
      this.videoElement.Revert();
    }

    /// <summary>
    /// The step one frame.
    /// </summary>
    /// <param name="forward">
    /// The forward. 
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

    #endregion

    #region Methods

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="obj">
    /// The obj. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as Video).OnPropertyChanged(args);
    }

    /// <summary>
    ///   The on video frame changed.
    /// </summary>
    private void OnVideoFrameChanged()
    {
      if (this.VideoFrameChanged != null)
      {
        this.VideoFrameChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// The set video mode.
    /// </summary>
    /// <param name="newVideoMode">
    /// The new video mode. 
    /// </param>
    private void SetVideoMode(VideoMode newVideoMode)
    {
      if (this.videoMode == newVideoMode)
      {
        return;
      }

      this.videoElement.VideoFrameChanged -= this.videoElement_VideoFrameChanged;
      this.videoMode = newVideoMode;

      switch (this.videoMode)
      {
        case VideoMode.File:
        case VideoMode.None:

          // this.videoCaptureElement.Stop();
          this.videoCaptureElement.Dispose();
          this.videoElement = this.videoPlayerElement;
          break;
        case VideoMode.Capture:
          List<DsDevice> videoDevices = DShowUtils.GetVideoInputDevices();
          if (videoDevices.Count > 0)
          {
            this.videoElement = this.videoCaptureElement;

            // this.videoCaptureElement.NewCamera(this.VideoCapturerElement.VideoCaptureDevice, 5, 320, 240);
            this.videoCaptureElement.NewCamera(this.VideoCapturerElement.VideoCaptureDevice, 0, 0, 0);
          }

          break;
      }

      this.videoElement.VideoFrameChanged += this.videoElement_VideoFrameChanged;
      this.VideoSource = this.videoElement.ImageSource;
    }

    /// <summary>
    /// The video capture element_ video available.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void videoCaptureElement_VideoAvailable(object sender, EventArgs e)
    {
      this.VideoSource = this.videoElement.ImageSource;
      this.videoElement.Play();
    }

    /// <summary>
    /// The video element_ video frame changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void videoElement_VideoFrameChanged(object sender, EventArgs e)
    {
      this.OnVideoFrameChanged();
    }

    #endregion
  }
}