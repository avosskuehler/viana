using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Shapes;
using AvalonDock;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Data;
using System.IO;
using AForge.Imaging.Filters;
using AForge;
using System.Windows.Threading;
using System.Threading;

namespace VianaNET
{
  public partial class VideoWindow : DockableContent
  {
    private const int margin = 10;
    private const int timelineMouseCaptureMargin = 15;

    private Line currentLine;
    private Rectangle timelineSelectionRange;
    private Thumb timelineThumb;
    private Canvas timelineSelectionCanvas;
    bool isReady = true;

    public void LoadVideo()
    {
      if (!Video.Instance.VideoPlayerElement.LoadMovie(string.Empty))
      {
        return;
      }
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

      this.BlobsControl.Visibility = Visibility.Collapsed;

      if (this.LeftVideoPanel.Children.Count > 1)
      {
        Video.Instance.Stop();
        this.LeftVideoPanel.Children.RemoveAt(0);
      }

      this.LeftVideoPanel.Children.Insert(0, Video.Instance.VideoElement);

      // This bindings could not be set in XAML, because
      // when switching VideoElement, they get lost.
      Binding widthBinding = new Binding();
      Binding heightBinding = new Binding();

      switch (newVideoMode)
      {
        case VideoMode.File:
          // Unregister existing capture events
          Video.Instance.VideoCapturerElement.NewVideoSample -=
            new EventHandler<WPFMediaKit.DirectShow.MediaPlayers.VideoSampleArgs>(Instance_NewVideoSample);
          // Register to video open event
          Video.Instance.VideoPlayerElement.VideoFileOpened +=
            new EventHandler(VideoPlayer_VideoFileOpened);

          // Set Overlay height and width bindings
          widthBinding.Source = Video.Instance.VideoPlayerElement;
          heightBinding.Source = Video.Instance.VideoPlayerElement;

          // Update UI
          this.timelineSlider.Visibility = Visibility.Visible;
          this.btnRevert.Visibility = Visibility.Visible;
          break;
        case VideoMode.Capture:
          Video.Instance.VideoPlayerElement.VideoFileOpened -=
            new EventHandler(VideoPlayer_VideoFileOpened);

          Video.Instance.VideoCapturerElement.NewVideoSample +=
          new EventHandler<WPFMediaKit.DirectShow.MediaPlayers.VideoSampleArgs>(Instance_NewVideoSample);

          // Set Overlay height and width bindings sources
          widthBinding.Source = Video.Instance.VideoCapturerElement;
          heightBinding.Source = Video.Instance.VideoCapturerElement;

          this.timelineSlider.Visibility = Visibility.Collapsed;
          this.btnRevert.Visibility = Visibility.Collapsed;
          break;
      }

      // Set Overlay height and width bindings
      widthBinding.Path = new PropertyPath("UniformWidth");
      this.OverlayCanvas.SetBinding(Canvas.WidthProperty, widthBinding);
      heightBinding.Path = new PropertyPath("UniformHeight");
      this.OverlayCanvas.SetBinding(Canvas.HeightProperty, heightBinding);
    }

    private void Instance_NewVideoSample(object sender, WPFMediaKit.DirectShow.MediaPlayers.VideoSampleArgs e)
    {
      if (this.isReady)
      {
        this.isReady = false;

        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
        {
          Video.Instance.CurrentFrameBitmap = e.VideoFrame;
          //this.BlobsControl.NativeBitmap = e.VideoFrame;
          this.BlobsControl.UpdatedProcessedImage();
        }, null);

        this.isReady = true;
      }
    }

    public VideoWindow()
    {
      InitializeComponent();
      this.SetVideoMode(VideoMode.File);
    }

    void VideoPlayer_VideoFileOpened(object sender, EventArgs e)
    {
      //this.BlobsControl.UpdatedProcessedImage();
      this.BlobsControl.UpdateScale();
      this.timelineSlider.SelectionStart = 0;
      this.timelineSlider.SelectionEnd = this.timelineSlider.Maximum;
    }

    void timelineSlider_TickUpClicked(object sender, EventArgs e)
    {
      Video.Instance.StepOneFrame(true);
    }

    void timelineSlider_TickDownClicked(object sender, EventArgs e)
    {
      Video.Instance.StepOneFrame(false);
    }

    private void btnSeekPrevious_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.StepOneFrame(false);
    }

    private void btnSeekNext_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.StepOneFrame(true);
    }

    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Play();
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Pause();
    }

    private void btnRevert_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Revert();
    }

    private void btnPause_Click(object sender, RoutedEventArgs e)
    {
      Video.Instance.Pause();
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

    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = Video.Instance.VideoElement.UniformWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = Video.Instance.VideoElement.UniformHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return !double.IsNaN(scaleX);
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
        if (newX < 0 || newX > Video.Instance.VideoElement.UniformWidth)
        {
          return;
        }

        if (newY < 0 || newY > Video.Instance.VideoElement.UniformHeight)
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

    private void ResetOuterRegion()
    {
      CombinedGeometry geometry = this.OuterRegion.Data as CombinedGeometry;
      RectangleGeometry outerRectangleGeometry = geometry.Geometry1 as RectangleGeometry;
      outerRectangleGeometry.Rect = new Rect(0, 0, Video.Instance.VideoElement.UniformWidth, Video.Instance.VideoElement.UniformHeight);
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
        //if (this.BlobsControl.IsVisible)
        //{
        //  this.BlobsControl.UpdatedProcessedImage();
        //}
      }
    }

    public void StopAutomaticDataAquisition()
    {
      // Set acquisition mode
      Video.Instance.IsDataAcquisitionRunning = false;

      this.AutomaticAquisitionFinished();
    }

    public void RunAutomaticDataAquisition()
    {
      StatusBarContent.Instance.StatusLabel = Localization.Labels.StatusIsCalculating;
      Video.Instance.VideoElement.D3DImageChanged += new EventHandler(VideoPlayer_D3DImageChanged);

      // Set acquisition mode
      Video.Instance.IsDataAcquisitionRunning = true;

      if (Video.Instance.VideoMode == VideoMode.File)
      {
        // Go back to initial position
        Video.Instance.Revert();

        // Start refreshing first D3DImage, that will call the first
        // D3DImageChanged event which itself calls the next frame
        Video.Instance.VideoPlayerElement.InvalidateVideoImage();
      }
    }

    void VideoPlayer_D3DImageChanged(object sender, EventArgs e)
    {
      switch (Video.Instance.VideoMode)
      {
        case VideoMode.File:
          int frameCount = Video.Instance.VideoPlayerElement.FrameCount;
          int frameIndex = Video.Instance.VideoPlayerElement.MediaPositionFrameIndex;
          if (frameIndex < frameCount - 1)
          {
            StatusBarContent.Instance.ProgressBarValue = (double)frameIndex / (frameCount - 1) * 100;
            //this.BlobsControl.UpdatedProcessedImage();
            Video.Instance.StepOneFrame(true);
          }
          else
          {
            AutomaticAquisitionFinished();
          }
          break;
        case VideoMode.Capture:
          // Update marquee progress bar
          StatusBarContent.Instance.ProgressBarValue++;
          if (StatusBarContent.Instance.ProgressBarValue >= 100)
          {
            StatusBarContent.Instance.ProgressBarValue = 0;
          }
          break;
      }
    }

    private void AutomaticAquisitionFinished()
    {
      Video.Instance.VideoElement.D3DImageChanged -= new EventHandler(VideoPlayer_D3DImageChanged);

      Video.Instance.IsDataAcquisitionRunning = false;

      // Reset Statusbar
      StatusBarContent.Instance.StatusLabel = Localization.Labels.StatusBarReady;
      StatusBarContent.Instance.ProgressBarValue = 0;

      // Recalculate dependent data values
      VideoData.Instance.RefreshDistanceVelocityAcceleration();
    }

    private void OverlayCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      PlaceCalibration();
      PlaceClippingRegion();
    }

    private enum RangeSelectionThumb
    {
      None,
      Start,
      End,
    }

    private RangeSelectionThumb currentRangeSelectionThumb;

    private void timelineSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      CheckWhichRangeEndToChange(e.GetPosition(this.timelineSelectionRange));
      this.timelineSlider.CaptureMouse();
    }

    private void CheckWhichRangeEndToChange(Point mouseDownPosition)
    {
      if (this.timelineSlider.IsMouseOver && !this.timelineThumb.IsMouseOver)
      {
        if (mouseDownPosition.X > -timelineMouseCaptureMargin && mouseDownPosition.X < timelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.Start;
        }
        else if (mouseDownPosition.X > this.timelineSelectionRange.ActualWidth - timelineMouseCaptureMargin &&
          mouseDownPosition.X < this.timelineSelectionRange.ActualWidth + timelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.End;
        }
      }
    }

    private void timelineSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      UpdateSelectionRange(e.GetPosition(this.timelineSelectionCanvas));
      this.currentRangeSelectionThumb = RangeSelectionThumb.None;
      this.timelineSlider.ReleaseMouseCapture();
    }

    private void UpdateSelectionRange(Point selectionPosition)
    {
      if (this.timelineSlider.IsMouseOver && !this.timelineThumb.IsMouseOver)
      {
        double factor = this.timelineSlider.Maximum / this.timelineSelectionCanvas.ActualWidth;
        switch (currentRangeSelectionThumb)
        {
          case RangeSelectionThumb.None:
            break;
          case RangeSelectionThumb.Start:
            this.timelineSlider.SelectionStart = selectionPosition.X * factor;
            break;
          case RangeSelectionThumb.End:
            this.timelineSlider.SelectionEnd = selectionPosition.X * factor;
            break;
          default:
            break;
        }
      }
    }

    private void timelineSlider_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        UpdateSelectionRange(e.GetPosition(this.timelineSelectionCanvas));
      }
    }

    private void DockableContent_Loaded(object sender, RoutedEventArgs e)
    {
      this.timelineSelectionRange =
        this.timelineSlider.Template.FindName("PART_SelectionRange", this.timelineSlider) as Rectangle;
      this.timelineThumb =
        this.timelineSlider.Template.FindName("Thumb", this.timelineSlider) as Thumb;
      this.timelineSelectionCanvas =
        this.timelineSlider.Template.FindName("SelectionCanvas", this.timelineSlider) as Canvas;

    }
  }
}