
using WPFMediaKit.DirectShow.Controls;
using Microsoft.Win32;
using MediaInfoLib;
using System.Globalization;
using System;
using System.Windows.Controls;
using System.Windows;
using WPFMediaKit.DirectShow.MediaPlayers;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace VianaNET
{
  public class VideoPlayer : MediaUriElement
  {
    //    //Timeline.DesiredFrameRateProperty.OverrideMetadata(  typeof(Timeline),    new FrameworkPropertyMetadata { DefaultValue = 20 }   );

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
      this.BeginInit();
      this.MediaOpened += new RoutedEventHandler(VideoPlayer_MediaOpened);
      this.MediaEnded += new RoutedEventHandler(VideoPlayer_MediaEnded);
      this.VideoRenderer = VideoRendererType.VideoMixingRenderer9;
      this.PreferedPositionFormat = MediaPositionFormat.MediaTime;
      this.LoadedBehavior = WPFMediaKit.DirectShow.MediaPlayers.MediaState.Pause;
      this.VideoImage.SizeChanged += new SizeChangedEventHandler(VideoImage_SizeChanged);
      this.EndInit();
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS

    public event EventHandler VideoFileOpened;

    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    public new double UniformHeight
    {
      get { return (double)GetValue(UniformHeightProperty); }
      set { SetValue(UniformHeightProperty, value); }
    }

    public static readonly DependencyProperty UniformHeightProperty =
      DependencyProperty.Register(
      "UniformHeight",
      typeof(double),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(double)));

    public new double UniformWidth
    {
      get { return (double)GetValue(UniformWidthProperty); }
      set { SetValue(UniformWidthProperty, value); }
    }

    public static readonly DependencyProperty UniformWidthProperty =
      DependencyProperty.Register(
      "UniformWidth",
      typeof(double),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(double)));

    /// <summary>
    /// Time between frames in ms units.
    /// </summary>
    public double FrameTime
    {
      get { return (double)GetValue(FrameTimeProperty); }
      set { SetValue(FrameTimeProperty, value); }
    }

    public static readonly DependencyProperty FrameTimeProperty =
      DependencyProperty.Register(
      "FrameTime",
      typeof(double),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(double)));

    public int MediaPositionFrameIndex
    {
      get { return (int)GetValue(MediaPositionFrameIndexProperty); }
      set { SetValue(MediaPositionFrameIndexProperty, value); }
    }

    public static readonly DependencyProperty MediaPositionFrameIndexProperty =
      DependencyProperty.Register(
      "MediaPositionFrameIndex",
      typeof(int),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(int)));

    public long MediaPositionTime
    {
      get { return (long)GetValue(MediaPositionTimeProperty); }
      set { SetValue(MediaPositionTimeProperty, value); }
    }

    public static readonly DependencyProperty MediaPositionTimeProperty =
      DependencyProperty.Register(
      "MediaPositionTime",
      typeof(long),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(long)));

    public int FrameCount
    {
      get { return (int)GetValue(FrameCountProperty); }
      set { SetValue(FrameCountProperty, value); }
    }

    public static readonly DependencyProperty FrameCountProperty =
      DependencyProperty.Register(
      "FrameCount",
      typeof(int),
      typeof(VideoPlayer),
      new UIPropertyMetadata(default(int)));

    //public ImageSource VideoSource
    //{
    //  get { return (ImageSource)GetValue(VideoSourceProperty); }
    //  set { SetValue(VideoSourceProperty, value); }
    //}

    //public static readonly DependencyProperty VideoSourceProperty =
    //  DependencyProperty.Register(
    //  "VideoSource",
    //  typeof(ImageSource),
    //  typeof(VideoPlayer),
    //  new UIPropertyMetadata(null));

    public System.Drawing.Bitmap CurrentFrameBitmap
    {
      get { return (System.Drawing.Bitmap)GetValue(CurrentFrameBitmapProperty); }
      set { SetValue(CurrentFrameBitmapProperty, value); }
    }

    public static readonly DependencyProperty CurrentFrameBitmapProperty =
      DependencyProperty.Register(
      "CurrentFrameBitmap",
      typeof(System.Drawing.Bitmap),
      typeof(VideoPlayer), new PropertyMetadata(null));

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

      this.Source = new Uri(fileName);
      return true;
    }

    public void StepOneFrame(bool forward)
    {
      if (this.IsPlaying)
      {
        this.Pause();
      }

      switch (this.CurrentPositionFormat)
      {
        case MediaPositionFormat.MediaTime:
          if (forward)
          {
            this.MediaPosition += (long)(this.FrameTime * 10000);
          }
          else
          {
            this.MediaPosition -= (long)(this.FrameTime * 10000);
          }
          break;
        case MediaPositionFormat.Frame:
          if (forward)
          {
            this.MediaPosition++;
          }
          else
          {
            this.MediaPosition--;
          }
          break;
        case MediaPositionFormat.Byte:
          break;
        case MediaPositionFormat.Field:
          break;
        case MediaPositionFormat.Sample:
          if (forward)
          {
            this.MediaPosition++;
          }
          else
          {
            this.MediaPosition--;
          }
          break;
        case MediaPositionFormat.None:
          break;
        default:
          break;
      }

      if (this.MediaPosition > this.MediaDuration)
      {
        this.MediaPosition = this.MediaDuration;
      }
      else if (this.MediaPosition < 0)
      {
        this.MediaPosition = 0;
      }
    }

    public void Revert()
    {
      this.MediaPosition = 0;
      this.Pause();
    }

    public void UpdateNativeBitmap()
    {
      if (!this.IsPlaying && this.NaturalVideoHeight > 0)
      {
        //this.CurrentFrameBitmap = CreateBitmapFromCurrentImageSource();
      }
    }

    public System.Drawing.Bitmap CreateBitmapFromCurrentImageSource()
    {
      if (this.NaturalVideoWidth <= 0)
      {
        return null;
      }

      System.Drawing.Bitmap returnBitmap;
      DrawingVisual visual = new DrawingVisual();
      DrawingContext dc = visual.RenderOpen();
      dc.DrawImage(
        this.D3DImage,
        new Rect(0, 0, this.NaturalVideoWidth, this.NaturalVideoHeight));

      dc.Close();
      RenderTargetBitmap rtp = new RenderTargetBitmap(
        this.NaturalVideoWidth,
        this.NaturalVideoHeight,
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

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES

    protected override void OnUnloadedOverride()
    {
      // Don´t unload when just hiding surface...
      //base.OnUnloadedOverride();
    }

    protected override void OnMediaPositionChanged(DependencyPropertyChangedEventArgs e)
    {
      //if (this.MediaPosition > (long)Math.Round(this.SelectionEnd))
      //{
      //  this.MediaPosition = (long)Math.Round(this.SelectionEnd);
      //  return;
      //}
      //else if (this.MediaPosition < (long)Math.Round(this.SelectionStart))
      //{
      //  this.MediaPosition = (long)Math.Round(this.SelectionStart);
      //  return;
      //}

      base.OnMediaPositionChanged(e);

      switch (this.CurrentPositionFormat)
      {
        case MediaPositionFormat.MediaTime:
          this.MediaPositionFrameIndex = (int)Math.Round(this.MediaPosition / this.FrameTime / 10000);
          this.MediaPositionTime = this.MediaPosition / 10000;
          break;
        case MediaPositionFormat.Frame:
          this.MediaPositionFrameIndex = (int)this.MediaPosition;
          this.MediaPositionTime = (long)(this.MediaPosition * this.FrameTime);
          break;
        case MediaPositionFormat.Byte:
          break;
        case MediaPositionFormat.Field:
          break;
        case MediaPositionFormat.Sample:
          this.MediaPositionFrameIndex = (int)this.MediaPosition;
          this.MediaPositionTime = (long)(this.MediaPosition * this.FrameTime);
          break;
        case MediaPositionFormat.None:
          break;
        default:
          break;
      }

      UpdateNativeBitmap();
    }

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    void VideoImage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.UniformWidth = this.VideoImage.ActualWidth;
      this.UniformHeight = this.VideoImage.ActualHeight;
    }

    void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
    {
      this.Pause();
    }

    void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
    {
      switch (this.CurrentPositionFormat)
      {
        case MediaPositionFormat.MediaTime:
          this.FrameCount = (int)(this.MediaDuration / this.FrameTime / 10000);
          break;
        case MediaPositionFormat.Frame:
          this.FrameCount = (int)(this.MediaDuration);
          break;
        case MediaPositionFormat.Byte:
          break;
        case MediaPositionFormat.Field:
          break;
        case MediaPositionFormat.Sample:
          this.FrameCount = (int)(this.MediaDuration);
          break;
      }

      this.OnVideoFileOpened();
      UpdateNativeBitmap();
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

    private void OnVideoFileOpened()
    {
      if (this.VideoFileOpened != null)
      {
        this.VideoFileOpened(this, EventArgs.Empty);
      }
    }

    #endregion //HELPER
  }
}
