using System;
using System.Globalization;
using System.IO;
using System.Windows;

using MediaInfoLib;
using Microsoft.Win32;

namespace VianaNET
{
  using System.Threading;
  using System.Windows.Interop;
  using System.Windows.Media.Imaging;

  using Emgu.CV;
  using Emgu.CV.CvEnum;
  using Emgu.CV.Structure;

  public class VideoPlayerCV : VideoBase
  {
    private string filename = string.Empty;
    private bool isFrameTimeCapable;

    public event EventHandler StepComplete;
    public event EventHandler FileComplete;

    // Create capture object 
    private Capture capture;
    //Image holder 
    private Image<Bgr, byte> image;

    public bool LoadMovie(string fileName)
    {
      try
      {
        if (!File.Exists(fileName))
        {
          var ofd = new OpenFileDialog
            {
              CheckFileExists = true,
              CheckPathExists = true,
              FilterIndex = 1,
              Filter = Localization.Labels.VideoFilesFilter,
              Title = Localization.Labels.LoadVideoFilesTitle
            };
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

        var videoHeader = new MediaInfo();
        videoHeader.Open(fileName);
        //string parameters=videoHeader.Option("Info_Parameters"); 
        string informString = videoHeader.Inform();
        ErrorLogger.WriteLine("###################### LOAD VIDEO ##########################");
        ErrorLogger.WriteLine(informString);
        var nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        string frameRateString = videoHeader.Get(StreamKind.Video, 0, "FrameRate");
        double framerate;
        if (double.TryParse(frameRateString, NumberStyles.AllowDecimalPoint, nfi, out framerate))
        {
          this.FrameTimeInNanoSeconds = (long)(10000000d / framerate);//Math.Round(1000d / framerate, 4);
        }

        videoHeader.Close();

        //Load avi 
        capture = new Capture(fileName);
        capture.ImageGrabbed += this.CaptureImageGrabbed;
        var fps = capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS);
        this.FrameCount = (int)capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_COUNT);
        //this.FrameCount =
        //  (int)(this.MediaDurationInMS / (this.FrameTimeInNanoSeconds * NanoSecsToMilliSecs));

        this.HasVideo = true;
        Video.Instance.ImageProcessing.InitializeImageFilters();
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

    void CaptureImageGrabbed(object sender, EventArgs e)
    {
      this.image = capture.RetrieveBgrFrame();
      this.ImageSource = loadBitmap(this.image.ToBitmap());
      Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
      {
        ((InteropBitmap)this.ImageSource).Invalidate();

        // Send new image to processing thread
        this.OnVideoFrameChanged();
      }, null);
    }

    public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
    {
      IntPtr ip = source.GetHbitmap();
      BitmapSource bs;
      try
      {
        bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
           IntPtr.Zero, Int32Rect.Empty,
           System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
      }
      finally
      {
      }

      return bs;
    }

    public override void Play()
    {
      capture.Start();
    }

    public override void Stop()
    {
      capture.Stop();
    }

    public override void Revert()
    {
      capture.Stop();
      capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
    }

    private void StepOneFrameForward()
    {
      capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, this.MediaPositionFrameIndex++);
    }

    private void StepOneFrameBackward()
    {
      capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, this.MediaPositionFrameIndex--);
    }

    private int StepFramesForward(int nFramesToStep)
    {
      capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, this.MediaPositionFrameIndex + nFramesToStep);
      return 0;
    }

    public void UpdateFrameIndex()
    {
      this.MediaPositionFrameIndex = (int)capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES);
    }


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

    public VideoPlayerCV()
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

    public override long MediaPositionInNanoSeconds
    {
      get
      {
        return 0;
      }

      set
      {
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
      typeof(VideoPlayerCV),
      new UIPropertyMetadata(default(double)));

    public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
  "SelectionStart",
  typeof(double),
  typeof(VideoPlayerCV));

    public double SelectionStart
    {
      get { return (double)GetValue(SelectionStartProperty); }
      set { SetValue(SelectionStartProperty, value); }
    }

    public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register(
      "SelectionEnd",
      typeof(double),
      typeof(VideoPlayerCV));

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
        this.StepOneFrameForward();
      }
      else
      {
        this.StepOneFrameBackward();
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
