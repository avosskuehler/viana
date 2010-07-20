using Microsoft.Win32;
using MediaInfoLib;
using System.Globalization;
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;

namespace VianaNET
{
  public class VideoPlayer : VideoBase
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private MediaPlayer mediaPlayer;
    private VideoDrawing videoDrawing;
    private DrawingImage drawingImage;
    private DrawingVisual drawingVisual;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public VideoPlayer()
    {
      this.mediaPlayer = new MediaPlayer();
      this.mediaPlayer.ScrubbingEnabled = true;
      this.videoDrawing = new VideoDrawing();
      this.videoDrawing.Player = this.mediaPlayer;
      this.drawingImage = new DrawingImage(this.videoDrawing);
      this.drawingVisual = new DrawingVisual();
      //this.drawingImage.Freeze();
      this.videoDrawing.Rect = new Rect(0, 0, 100, 100);
      this.ImageSource = drawingImage;
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

    public RenderTargetBitmap RenderTargetBitmap { get; set; }

    //public double UniformHeight
    //{
    //  get { return (double)GetValue(UniformHeightProperty); }
    //  set { SetValue(UniformHeightProperty, value); }
    //}

    //public static readonly DependencyProperty UniformHeightProperty =
    //  DependencyProperty.Register(
    //  "UniformHeight",
    //  typeof(double),
    //  typeof(VideoPlayer),
    //  new UIPropertyMetadata(default(double)));

    //public double UniformWidth
    //{
    //  get { return (double)GetValue(UniformWidthProperty); }
    //  set { SetValue(UniformWidthProperty, value); }
    //}

    //public static readonly DependencyProperty UniformWidthProperty =
    //  DependencyProperty.Register(
    //  "UniformWidth",
    //  typeof(double),
    //  typeof(VideoPlayer),
    //  new UIPropertyMetadata(default(double)));

    public override long MediaPositionInMS
    {
      get
      {
        return (long)this.mediaPlayer.Position.TotalMilliseconds;
      }

      set
      {
        this.mediaPlayer.Position = new TimeSpan(0, 0, 0, 0, (int)value);
      }
    }

    public double MediaDuration
    {
      get { return (double)GetValue(MediaDurationProperty); }
      set { SetValue(MediaDurationProperty, value); }
    }

    public static readonly DependencyProperty MediaDurationProperty =
      DependencyProperty.Register(
      "MediaDuration",
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

    public bool LoadMovie(string fileName)
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
        this.FrameTime = 1000d / framerate;//Math.Round(1000d / framerate, 4);
      }

      ////string frameCountString = videoHeader.Get(StreamKind.Video, 0, "FrameCount");
      ////int framecount;
      ////if (int.TryParse(frameCountString, out framecount))
      ////{
      ////  this.FrameCount = framecount;
      ////}

      //string durationString = videoHeader.Get(StreamKind.Video, 0, "Duration");
      //double duration;
      //if (double.TryParse(durationString, NumberStyles.AllowDecimalPoint, nfi, out duration))
      //{
      //  this.Duration = duration;
      //}

      //string codec = videoHeader.Get(StreamKind.Video, 0, "Format");

      videoHeader.Close();

      this.mediaPlayer.Open(new Uri(fileName));
      this.mediaPlayer.Pause();

      while (!this.mediaPlayer.NaturalDuration.HasTimeSpan)
      {
        Thread.Sleep(200);
      }

      this.MediaDuration = this.mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
      this.NaturalVideoHeight = this.mediaPlayer.NaturalVideoHeight;
      this.NaturalVideoWidth = this.mediaPlayer.NaturalVideoWidth;
      this.videoDrawing.Rect = new Rect(0, 0, NaturalVideoWidth, NaturalVideoHeight);

      this.CreateMemoryMapping(4);

      this.RenderTargetBitmap = new RenderTargetBitmap(
  (int)this.NaturalVideoWidth,
  (int)this.NaturalVideoHeight,
  96d,
  96d,
  PixelFormats.Default);
      this.RenderTargetBitmap.Changed += new EventHandler(renderTargetBitmap_Changed);

      this.HasVideo = true;
      this.OnVideoAvailable();
      return true;
    }

    private void renderTargetBitmap_Changed(object sender, EventArgs e)
    {
      this.RenderTargetBitmap.CopyPixels(new Int32Rect(0, 0, (int)this.NaturalVideoWidth, (int)this.NaturalVideoHeight), this.Map, this.bufferLength, this.Stride);

      // Encoding the RenderBitmapTarget as a PNG file.
      PngBitmapEncoder png = new PngBitmapEncoder();
      png.Frames.Add(BitmapFrame.Create(this.RenderTargetBitmap));
      using (Stream stm = File.Create(@"c:\Dumps\RTB.png"))
      {
        png.Save(stm);
      }
      this.OnVideoFrameChanged();
    }

    public void StepOneFrame(bool forward)
    {
      //    RenderTargetBitmap rtp = new RenderTargetBitmap(
      //(int)this.NaturalVideoWidth,
      //(int)this.NaturalVideoHeight,
      //96d,
      //96d,
      //PixelFormats.Default);
      //    MediaElement me = new MediaElement();
      //    rtp.Render(me);
      //    rtp.CopyPixels
    }

    public override void Play()
    {
      this.mediaPlayer.Play();
      base.Play();
    }

    public override void Pause()
    {
      this.mediaPlayer.Pause();
    }

    public override void Stop()
    {
      this.mediaPlayer.Stop();
      base.Stop();
    }

    public override void Revert()
    {
      this.mediaPlayer.Pause();
      TimeSpan ts = new TimeSpan(0, 0, 0, 0, 0);
      this.mediaPlayer.Position = ts;
    }

    //public void UpdateNativeBitmap()
    //{
    //  if (!this.IsPlaying && this.NaturalVideoHeight > 0)
    //  {
    //    //this.CurrentFrameBitmap = CreateBitmapFromCurrentImageSource();
    //  }
    //}

    //public System.Drawing.Bitmap CreateBitmapFromCurrentImageSource()
    //{
    //  if (this.NaturalVideoWidth <= 0)
    //  {
    //    return null;
    //  }

    //  System.Drawing.Bitmap returnBitmap;
    //  DrawingVisual visual = new DrawingVisual();
    //  DrawingContext dc = visual.RenderOpen();
    //  dc.DrawImage(
    //    this.D3DImage,
    //    new Rect(0, 0, this.NaturalVideoWidth, this.NaturalVideoHeight));

    //  dc.Close();
    //  RenderTargetBitmap rtp = new RenderTargetBitmap(
    //    this.NaturalVideoWidth,
    //    this.NaturalVideoHeight,
    //    96d,
    //    96d,
    //    PixelFormats.Default);
    //  rtp.Render(visual);

    //  using (MemoryStream outStream = new MemoryStream())
    //  {
    //    PngBitmapEncoder pnge = new PngBitmapEncoder();
    //    pnge.Frames.Add(BitmapFrame.Create(rtp));
    //    pnge.Save(outStream);
    //    returnBitmap = new System.Drawing.Bitmap(outStream);
    //    //returnBitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
    //  }

    //  return returnBitmap;
    //}

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

    void VideoImage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      //this.UniformWidth = this.mediaPlayer.ActualWidth;
      //this.UniformHeight = this.VideoImage.ActualHeight;
    }

    void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
    {
      this.Pause();
    }

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

    internal void RenderVideo()
    {
      DrawingContext drawingContext = drawingVisual.RenderOpen();

      using (drawingContext)
      {
        drawingContext.DrawVideo(this.mediaPlayer, new Rect(0, 0, this.NaturalVideoWidth,this.NaturalVideoHeight));
      }

      this.RenderTargetBitmap.Render(drawingVisual);
    }
  }
}
