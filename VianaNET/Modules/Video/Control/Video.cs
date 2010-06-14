using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFMediaKit.DirectShow.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Data;
using DirectShowLib;

namespace VianaNET
{
  public class Video : DependencyObject
  {
    private static Video instance;

    private VideoMode videoMode;
    private MediaElementBase videoElement;
    private VideoCapturer videoCaptureElement;
    private VideoPlayer videoPlayerElement;

    public VideoMode VideoMode
    {
      get { return this.videoMode; }
      set { this.SetVideoMode(value); }
    }

    public int FrameIndex
    {
      get
      {
        switch (this.videoMode)
        {
          case VideoMode.File:
            return this.videoPlayerElement.MediaPositionFrameIndex;
          case VideoMode.Capture:
            return VideoData.Instance.Count;
        }

        return 0;
      }

    }

    public long FrameTimestamp
    {
      get
      {
        switch (this.videoMode)
        {
          case VideoMode.File:
            return this.videoPlayerElement.MediaPositionTime;
          case VideoMode.Capture:
            return this.videoCaptureElement.CaptureTime;
        }

        return 0;
      }
    }

    public bool IsDataAcquisitionRunning
    {
      get { return (bool)GetValue(IsDataAcquisitionRunningProperty); }
      set { SetValue(IsDataAcquisitionRunningProperty, value); }
    }

    public static readonly DependencyProperty IsDataAcquisitionRunningProperty =
      DependencyProperty.Register(
      "IsDataAcquisitionRunning",
      typeof(bool),
      typeof(Video),
      new UIPropertyMetadata(false));

    public ImageSource VideoSource
    {
      get { return (ImageSource)GetValue(VideoSourceProperty); }
      set { SetValue(VideoSourceProperty, value); }
    }

    public static readonly DependencyProperty VideoSourceProperty =
      DependencyProperty.Register(
      "VideoSource",
      typeof(ImageSource),
      typeof(Video),
      new UIPropertyMetadata(null));

    public System.Drawing.Bitmap CurrentFrameBitmap
    {
      get { return (System.Drawing.Bitmap)GetValue(CurrentFrameBitmapProperty); }
      set { SetValue(CurrentFrameBitmapProperty, value); }
    }

    public static readonly DependencyProperty CurrentFrameBitmapProperty =
      DependencyProperty.Register(
      "CurrentFrameBitmap",
      typeof(System.Drawing.Bitmap),
      typeof(Video),
      new PropertyMetadata(null));

    private Video()
    {
      // Need to have the construction of the Video objects
      // in constructor, to get the databindings to their 
      // properties to work.
      this.videoPlayerElement = new VideoPlayer();
      this.videoCaptureElement = new VideoCapturer();

      this.videoElement = this.videoPlayerElement;
      this.videoMode = VideoMode.None;
    }

    public MediaElementBase VideoElement
    {
      get { return this.videoElement; }
    }

    public VideoPlayer VideoPlayerElement
    {
      get { return this.videoPlayerElement; }
    }

    public VideoCapturer VideoCapturerElement
    {
      get { return this.videoCaptureElement; }
    }

    private void SetVideoMode(VideoMode newVideoMode)
    {
      if (this.videoMode == newVideoMode)
      {
        return;
      }

      BindingOperations.ClearAllBindings(this);

      this.videoMode = newVideoMode;
      switch (this.videoMode)
      {
        case VideoMode.File:
          this.videoElement = this.videoPlayerElement;
          Binding bitmapBinding = new Binding();
          bitmapBinding.Source = this.videoPlayerElement;
          bitmapBinding.Path = new PropertyPath("CurrentFrameBitmap");
          BindingOperations.SetBinding(this, Video.CurrentFrameBitmapProperty, bitmapBinding);

          break;
        case VideoMode.Capture:
          this.videoElement = this.videoCaptureElement;
          break;
      }

      this.VideoSource = this.videoElement.D3DImage;
    }

    /// <summary>
    /// Gets the <see cref="Video"/> singleton.
    /// If the underlying instance is null, a instance will be created.
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

    public System.Drawing.Bitmap CreateBitmapFromCurrentImageSource()
    {
      if (this.videoElement.NaturalVideoWidth <= 0)
      {
        return null;
      }

      System.Drawing.Bitmap returnBitmap;
      DrawingVisual visual = new DrawingVisual();
      DrawingContext dc = visual.RenderOpen();
      dc.DrawImage(
        this.videoElement.D3DImage,
        new Rect(0, 0, this.videoElement.NaturalVideoWidth, this.videoElement.NaturalVideoHeight));

      dc.Close();
      RenderTargetBitmap rtp = new RenderTargetBitmap(
        this.videoElement.NaturalVideoWidth,
        this.videoElement.NaturalVideoHeight,
        96d,
        96d,
        PixelFormats.Default);
      rtp.Render(visual);

      using (MemoryStream outStream = new MemoryStream())
      {
        PngBitmapEncoder pnge = new PngBitmapEncoder();
        pnge.Frames.Add(BitmapFrame.Create(rtp));
        pnge.Save(outStream);
        returnBitmap = new System.Drawing.Bitmap(outStream);
        //returnBitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
      }

      return returnBitmap;
    }


    public void Play()
    {
      this.videoElement.Play();
    }

    public void Pause()
    {
      this.videoElement.Pause();
    }

    public void Stop()
    {
      this.videoElement.Stop();
    }

    public void Revert()
    {
      switch (this.videoMode)
      {
        case VideoMode.File:
          this.videoPlayerElement.Revert();
          break;
        case VideoMode.Capture:
          break;
      }

      this.UpdateNativeBitmap();
    }

    public void UpdateNativeBitmap()
    {
      switch (this.videoMode)
      {
        case VideoMode.File:
          this.videoPlayerElement.UpdateNativeBitmap();
          break;
        case VideoMode.Capture:
          break;
      }
    }
  }
}
