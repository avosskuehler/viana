using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AvalonDock;
using VianaNETShaderEffectLibrary;
using System.Threading;

namespace VianaNET
{
  public partial class VideoWindow : DockableContent
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS

    private const int margin = 10;

    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private Line currentLine;
    bool isDragging = false;
    private bool cancelCalculation;

    private DispatcherTimer timesliderUpdateTimer;
    //private ThresholdEffect thresholdEffect;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public VideoWindow()
    {
      InitializeComponent();
      this.SetVideoMode(VideoMode.File);

      this.timesliderUpdateTimer = new DispatcherTimer();
      this.timesliderUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
      this.timesliderUpdateTimer.Tick += new EventHandler(timesliderUpdateTimer_Tick);

      //this.thresholdEffect = new ThresholdEffect();

      RenderOptions.SetBitmapScalingMode(this.VideoImage, BitmapScalingMode.LowQuality);

      Calibration.Instance.PropertyChanged +=
        new System.ComponentModel.PropertyChangedEventHandler(CalibrationPropertyChanged);

      Video.Instance.VideoFrameChanged +=
        new EventHandler(OnVideoFrameChanged);

      Video.Instance.VideoPlayerElement.StepComplete +=
        new EventHandler(VideoPlayerElement_StepComplete);
      Video.Instance.VideoPlayerElement.FileComplete +=
        new EventHandler(VideoPlayerElement_FileComplete);

      Video.Instance.ImageProcessing.PropertyChanged +=
        new System.ComponentModel.PropertyChangedEventHandler(ImageProcessing_PropertyChanged);

      this.timelineSlider.SelectionEndReached +=
        new EventHandler(timelineSlider_SelectionEndReached);
      this.timelineSlider.SelectionAndValueChanged +=
        new EventHandler(timelineSlider_SelectionAndValueChanged);
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
    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void LoadVideo(string fileNameToAnalyse)
    {
      if (!Video.Instance.LoadMovie(fileNameToAnalyse))
      {
        return;
      }

      this.timesliderUpdateTimer.Start();
      //this.VideoImage.Source = Video.Instance.VideoSource;
    }

    public void SetVideoMode(VideoMode newVideoMode)
    {
      if (Video.Instance.VideoMode == newVideoMode)
      {
        // No change in video mode, so nothing to to.
        return;
      }

      Video.Instance.VideoMode = newVideoMode;
      Calibration.Instance.Reset();
      VideoData.Instance.Reset();
      ShowCalibration(false);
      ShowClipRegion(false);

      switch (newVideoMode)
      {
        case VideoMode.File:
          // Update UI
          this.timelineSlider.Visibility = Visibility.Visible;
          this.btnRevert.Visibility = Visibility.Visible;
          this.btnRecord.Visibility = Visibility.Collapsed;
          break;
        case VideoMode.Capture:
          this.timelineSlider.Visibility = Visibility.Collapsed;
          this.btnRevert.Visibility = Visibility.Collapsed;
          this.btnRecord.Visibility = Visibility.Visible;
          break;
      }
    }

    public void UpdateCalibration()
    {
      ShowOrHideCalibration(Visibility.Visible);
      PlaceCalibration();
    }

    public void UpdateClippingRegion()
    {
      ShowOrHideClipRegion(Visibility.Visible);
      PlaceClippingRegion();
    }

    public void ShowCalibration(bool show)
    {
      if (Calibration.Instance.IsVideoCalibrated)
      {
        ShowOrHideCalibration(show ? Visibility.Visible : Visibility.Collapsed);
      }
    }

    public void ShowClipRegion(bool show)
    {
      if (Calibration.Instance.HasClipRegion)
      {
        ShowOrHideClipRegion(show ? Visibility.Visible : Visibility.Collapsed);
      }
    }

    public void StopAutomaticDataAquisition()
    {
      switch (Video.Instance.VideoMode)
      {
        case VideoMode.File:
          this.cancelCalculation = true;
          break;
        case VideoMode.Capture:
          AutomaticAquisitionFinished();
          break;
      }
    }

    private int automaticDataAquisitionTotalFrameCount;
    private int automaticDataAquisitionCurrentFrameCount;

    public void RunAutomaticDataAquisition()
    {
      StatusBarContent.Instance.StatusLabel = Localization.Labels.StatusIsCalculating;

      VideoData.Instance.Reset();

      // Set acquisition mode
      Video.Instance.IsDataAcquisitionRunning = true;

      if (Video.Instance.VideoMode == VideoMode.File)
      {
        // Go back to initial position
        Video.Instance.Revert();
        this.automaticDataAquisitionCurrentFrameCount = 0;
        this.automaticDataAquisitionTotalFrameCount = (int)((this.timelineSlider.SelectionEnd - this.timelineSlider.SelectionStart) / (this.timelineSlider.FrameTimeInNanoSeconds * VideoBase.NanoSecsToMilliSecs));

        Video.Instance.StepOneFrame(true);
      }
    }

    void VideoPlayerElement_FileComplete(object sender, EventArgs e)
    {
      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
        if (Video.Instance.IsDataAcquisitionRunning)
        {
          AutomaticAquisitionFinished();
        }
      });
    }

    void VideoPlayerElement_StepComplete(object sender, EventArgs e)
    {
      Dispatcher.Invoke((ThreadStart)delegate
      {
        if (Video.Instance.IsDataAcquisitionRunning)
        {
          automaticDataAquisitionCurrentFrameCount++;
          StatusBarContent.Instance.ProgressBarValue =
            (double)automaticDataAquisitionCurrentFrameCount /
            (automaticDataAquisitionTotalFrameCount - 1) * 100;

          if (automaticDataAquisitionCurrentFrameCount == automaticDataAquisitionTotalFrameCount - 1
            || this.cancelCalculation)
          {
            AutomaticAquisitionFinished();
            return;
          }

          Video.Instance.StepOneFrame(true);
        }
      });
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

    void ImageProcessing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "CurrentBlobCenter")
      {
        double scaleX;
        double scaleY;

        if (GetScales(out scaleX, out scaleY))
        {
          Point? blobCenter = Video.Instance.ImageProcessing.CurrentBlobCenter;
          if (blobCenter.HasValue)
          {
            this.BlobHorizontalLine.Visibility = Visibility.Visible;
            this.BlobVerticalLine.Visibility = Visibility.Visible;
            this.BlobHorizontalLine.Y1 = blobCenter.Value.Y * scaleY;
            this.BlobHorizontalLine.Y2 = blobCenter.Value.Y * scaleY;
            this.BlobVerticalLine.X1 = blobCenter.Value.X * scaleX;
            this.BlobVerticalLine.X2 = blobCenter.Value.X * scaleX;
          }
          else
          {
            this.BlobHorizontalLine.Visibility = Visibility.Collapsed;
            this.BlobVerticalLine.Visibility = Visibility.Collapsed;
          }
        }
      }
      else if (e.PropertyName == "TargetColor" ||
        e.PropertyName == "ColorThreshold")
      {
        //thresholdEffect.Threshold = Video.Instance.ImageProcessing.ColorThreshold;
        //thresholdEffect.TargetColor = Video.Instance.ImageProcessing.TargetColor;
        //thresholdEffect.BlankColor = Colors.Black;
      }
      else if (e.PropertyName == "IsTargetColorSet")
      {
        if (Video.Instance.ImageProcessing.IsTargetColorSet)
        {
          this.BlobVerticalLine.Visibility = Visibility.Visible;
          this.BlobHorizontalLine.Visibility = Visibility.Visible;
        }
      }
    }

    void CalibrationPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
    }

    void OnVideoFrameChanged(object sender, EventArgs e)
    {
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        Video.Instance.ImageProcessing.ProcessImage();
      }
      else
      {
        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
          Video.Instance.ImageProcessing.ProcessImage();
        });
      }
    }

    void timelineSlider_SelectionEndReached(object sender, EventArgs e)
    {
      Video.Instance.Pause();
    }

    void timelineSlider_SelectionAndValueChanged(object sender, EventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds =
        (long)(timelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
    }

    void timesliderUpdateTimer_Tick(object sender, EventArgs e)
    {
      if (!isDragging && Video.Instance.VideoMode == VideoMode.File)
      {
        double preciseTime = Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds;
        //double alignedTime = (int)(preciseTime / Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds) *
        // Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds;
        this.timelineSlider.Value = preciseTime * VideoBase.NanoSecsToMilliSecs;
      }
    }

    private void timelineSlider_DragStarted(object sender, DragStartedEventArgs e)
    {
      isDragging = true;
    }

    private void timelineSlider_DragDelta(object sender, DragDeltaEventArgs e)
    {
      //     Video.Instance.VideoPlayerElement.MediaPositionInMS = (long)timelineSlider.Value;
    }

    private void timelineSlider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds = (long)(timelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
      isDragging = false;
    }

    void VideoPlayer_VideoFileOpened(object sender, EventArgs e)
    {
      //this.BlobsControl.UpdatedProcessedImage();
      //this.BlobsControl.UpdateScale();
      //this.timelineSlider.SelectionStart = 0;
      //this.timelineSlider.SelectionEnd = this.timelineSlider.Maximum;
    }

    void timelineSlider_TickUpClicked(object sender, EventArgs e)
    {
      if (this.timelineSlider.Value <= this.timelineSlider.SelectionEnd - this.timelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(true);
      }
    }

    void timelineSlider_TickDownClicked(object sender, EventArgs e)
    {
      if (this.timelineSlider.Value >= this.timelineSlider.SelectionStart + this.timelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(false);
      }
    }

    private void btnRecord_Click(object sender, RoutedEventArgs e)
    {
      //Video.Instance.Record();
    }

    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Play();
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Stop();
    }

    private void btnRevert_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Revert();
      this.timelineSlider.Value = this.timelineSlider.SelectionStart;
    }

    private void btnPause_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Pause();
    }

    private void TopLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
    }

    private void Line_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
    }

    private void LeftLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }

    private void BottomLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
    }

    private void RightLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }


    private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.currentLine = sender as Line;
      Mouse.Capture(this.currentLine);
    }

    private void Line_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && this.currentLine != null)
      {
        double newX = e.GetPosition(this.OverlayCanvas).X;
        double newY = e.GetPosition(this.OverlayCanvas).Y;
        if (newX < 0 || newX > this.VideoImage.ActualWidth)
        {
          return;
        }

        if (newY < 0 || newY > this.VideoImage.ActualHeight)
        {
          return;
        }

        switch (this.currentLine.Name)
        {
          case "TopLine":
            if (newY + margin < this.BottomLine.Y1)
            {
              this.currentLine.Y1 = newY;
              this.currentLine.Y2 = newY;
              this.LeftLine.Y1 = newY;
              this.RightLine.Y1 = newY;
            }
            break;
          case "BottomLine":
            if (newY > this.TopLine.Y1 + margin)
            {
              this.currentLine.Y1 = newY;
              this.currentLine.Y2 = newY;
              this.LeftLine.Y2 = newY;
              this.RightLine.Y2 = newY;
            }
            break;
          case "LeftLine":
            if (newX + margin < this.RightLine.X1)
            {
              this.currentLine.X1 = newX;
              this.currentLine.X2 = newX;
              this.TopLine.X1 = newX;
              this.BottomLine.X1 = newX;
            }
            break;
          case "RightLine":
            if (newX > this.LeftLine.X1 + margin)
            {
              this.currentLine.X1 = newX;
              this.currentLine.X2 = newX;
              this.TopLine.X2 = newX;
              this.BottomLine.X2 = newX;
            }
            break;
        }

        this.ResetOuterRegion();
      }
    }

    private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    private void OverlayCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      PlaceCalibration();
      PlaceClippingRegion();
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

    private void PlaceClippingRegion()
    {
      double scaleX;
      double scaleY;

      if (!GetScales(out scaleX, out scaleY))
      {
        return;
      }

      if (this.OuterRegion.IsVisible && Calibration.Instance.ClipRegion != Rect.Empty)
      {
        this.TopLine.X1 = Calibration.Instance.ClipRegion.Left * scaleX;
        this.TopLine.X2 = Calibration.Instance.ClipRegion.Right * scaleX;
        this.TopLine.Y1 = Calibration.Instance.ClipRegion.Top * scaleY;
        this.TopLine.Y2 = Calibration.Instance.ClipRegion.Top * scaleY;
        this.BottomLine.X1 = Calibration.Instance.ClipRegion.Left * scaleX;
        this.BottomLine.X2 = Calibration.Instance.ClipRegion.Right * scaleX;
        this.BottomLine.Y1 = Calibration.Instance.ClipRegion.Bottom * scaleY;
        this.BottomLine.Y2 = Calibration.Instance.ClipRegion.Bottom * scaleY;
        this.LeftLine.X1 = Calibration.Instance.ClipRegion.Left * scaleX;
        this.LeftLine.X2 = Calibration.Instance.ClipRegion.Left * scaleX;
        this.LeftLine.Y1 = Calibration.Instance.ClipRegion.Top * scaleY;
        this.LeftLine.Y2 = Calibration.Instance.ClipRegion.Bottom * scaleY;
        this.RightLine.X1 = Calibration.Instance.ClipRegion.Right * scaleX;
        this.RightLine.X2 = Calibration.Instance.ClipRegion.Right * scaleX;
        this.RightLine.Y1 = Calibration.Instance.ClipRegion.Top * scaleY;
        this.RightLine.Y2 = Calibration.Instance.ClipRegion.Bottom * scaleY;
        this.ResetOuterRegion();
      }
    }

    private void PlaceCalibration()
    {
      double scaleX;
      double scaleY;

      if (GetScales(out scaleX, out scaleY))
      {
        Canvas.SetLeft(this.OriginPath, Calibration.Instance.OriginInPixel.X * scaleX - this.OriginPath.ActualWidth / 2);
        Canvas.SetTop(this.OriginPath, Calibration.Instance.OriginInPixel.Y * scaleY - this.OriginPath.ActualHeight / 2);
        this.RulerLine.X1 = Calibration.Instance.RulerStartPointInPixel.X * scaleX;
        this.RulerLine.Y1 = Calibration.Instance.RulerStartPointInPixel.Y * scaleY;
        this.RulerLine.X2 = Calibration.Instance.RulerEndPointInPixel.X * scaleX;
        this.RulerLine.Y2 = Calibration.Instance.RulerEndPointInPixel.Y * scaleY;
        double centerLineX = (this.RulerLine.X1 + this.RulerLine.X2) / 2;
        double centerLineY = (this.RulerLine.Y1 + this.RulerLine.Y2) / 2;

        Canvas.SetLeft(this.RulerLabelBorder, centerLineX - this.RulerLabelBorder.ActualWidth / 2);
        Canvas.SetTop(this.RulerLabelBorder, centerLineY - this.RulerLabelBorder.ActualHeight / 2);
      }
    }

    private void ResetOuterRegion()
    {
      CombinedGeometry geometry = this.OuterRegion.Data as CombinedGeometry;
      RectangleGeometry outerRectangleGeometry = geometry.Geometry1 as RectangleGeometry;
      outerRectangleGeometry.Rect = new Rect(0, 0, VideoImage.ActualWidth, VideoImage.ActualHeight);
      RectangleGeometry innerRectangleGeometry = geometry.Geometry2 as RectangleGeometry;
      Rect innerRect = new Rect(new Point(this.LeftLine.X1, this.TopLine.Y1), new Point(this.RightLine.X1, this.BottomLine.Y1));
      innerRectangleGeometry.Rect = innerRect;

      double scaleX;
      double scaleY;
      if (this.GetScales(out scaleX, out scaleY))
      {
        Rect clipRect = innerRect;
        clipRect.Scale(1 / scaleX, 1 / scaleY);
        Calibration.Instance.ClipRegion = clipRect;
      }
    }
    private void AutomaticAquisitionFinished()
    {
      Video.Instance.IsDataAcquisitionRunning = false;
      this.cancelCalculation = false;
      //if (Video.Instance.VideoMode == VideoMode.File)
      //{
      //  Video.Instance.VideoPlayerElement.StepComplete -=
      //    new EventHandler(VideoPlayerElement_StepComplete);
      //  Video.Instance.VideoPlayerElement.FileComplete -=
      //    new EventHandler(VideoPlayerElement_FileComplete);
      //}

      // Reset Statusbar
      StatusBarContent.Instance.StatusLabel = Localization.Labels.StatusBarReady;
      StatusBarContent.Instance.ProgressBarValue = 0;

      // Recalculate dependent data values
      VideoData.Instance.RefreshDistanceVelocityAcceleration();
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    private bool GetScales(out double scaleX, out double scaleY)
    {
      //double sourceRatio = Video.Instance.VideoElement.NaturalVideoWidth / Video.Instance.VideoElement.NaturalVideoHeight;
      //double destinationRatio = this.VideoImage.ActualWidth / this.VideoImage.ActualHeight;

      //double uniformWidth = this.VideoImage.ActualHeight /
      //  Video.Instance.VideoElement.NaturalVideoHeight *
      //  Video.Instance.VideoElement.NaturalVideoWidth;
      //double uniformHeight = this.VideoImage.ActualHeight;

      //if (sourceRatio < destinationRatio)
      //{
      //  uniformWidth = this.VideoImage.ActualWidth;
      //  uniformHeight = this.VideoImage.ActualWidth /
      //  Video.Instance.VideoElement.NaturalVideoWidth *
      //  Video.Instance.VideoElement.NaturalVideoHeight;
      //}

      scaleX = this.VideoImage.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.VideoImage.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return (!double.IsInfinity(scaleX) && !double.IsNaN(scaleX));
    }

    private void ShowOrHideCalibration(Visibility visibility)
    {
      this.OriginPath.Visibility = visibility;
      this.RulerLine.Visibility = visibility;
      this.RulerLabelBorder.Visibility = visibility;
    }

    private void ShowOrHideClipRegion(Visibility visibility)
    {
      this.TopLine.Visibility = visibility;
      this.BottomLine.Visibility = visibility;
      this.LeftLine.Visibility = visibility;
      this.RightLine.Visibility = visibility;
      this.OuterRegion.Visibility = visibility;
    }

    #endregion //HELPER

  }
}