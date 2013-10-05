// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The video window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VianaNET.Application;

namespace VianaNET.Modules.Video
{
  using System;
  using System.ComponentModel;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;
  using System.Windows.Threading;

  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Localization;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Dialogs;

  /// <summary>
  ///   The video window.
  /// </summary>
  public partial class VideoWindow
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constants

    /// <summary>
    ///   The margin.
    /// </summary>
    private const int DefaultMargin = 10;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Fields

    /// <summary>
    ///   The timeslider update timer.
    /// </summary>
    private readonly DispatcherTimer timesliderUpdateTimer;

    /// <summary>
    ///   The automatic data aquisition current frame count.
    /// </summary>
    private int automaticDataAquisitionCurrentFrameCount;

    /// <summary>
    ///   The automatic data aquisition total frame count.
    /// </summary>
    private int automaticDataAquisitionTotalFrameCount;

    /// <summary>
    ///   The blob horizontal lines.
    /// </summary>
    private Line[] blobHorizontalLines;

    /// <summary>
    ///   The blob vertical lines.
    /// </summary>
    private Line[] blobVerticalLines;

    /// <summary>
    ///   The cancel calculation.
    /// </summary>
    private bool cancelCalculation;

    /// <summary>
    ///   The current line.
    /// </summary>
    private Line currentLine;

    /// <summary>
    ///   The is dragging.
    /// </summary>
    private bool isDragging;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="VideoWindow" /> class.
    /// </summary>
    public VideoWindow()
    {
      this.InitializeComponent();
      this.SetVideoMode(VideoMode.File);
      this.CreateCrossHairLines();
      this.timesliderUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
      this.timesliderUpdateTimer.Tick += this.TimesliderUpdateTimerTick;

      // this.thresholdEffect = new ThresholdEffect();
      RenderOptions.SetBitmapScalingMode(this.VideoImage, BitmapScalingMode.LowQuality);

      VianaNetApplication.Project.CalibrationData.PropertyChanged += this.CalibrationPropertyChanged;

      Video.Instance.VideoFrameChanged += this.OnVideoFrameChanged;

      Video.Instance.VideoPlayerElement.StepComplete += this.VideoPlayerElementStepComplete;
      Video.Instance.VideoPlayerElement.FileComplete += this.VideoPlayerElementFileComplete;

      VianaNetApplication.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      VianaNetApplication.Project.ProcessingData.FrameProcessed += this.ProcessingDataFrameProcessed;
      this.TimelineSlider.SelectionEndReached += this.TimelineSliderSelectionEndReached;
      this.TimelineSlider.SelectionAndValueChanged += this.TimelineSliderSelectionAndValueChanged;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Methods and Operators

    /// <summary>
    /// The load video.
    /// </summary>
    /// <param name="fileNameToAnalyse">
    /// The file name to analyse. 
    /// </param>
    public void LoadVideo(string fileNameToAnalyse)
    {
      if (!Video.Instance.LoadMovie(fileNameToAnalyse))
      {
        return;
      }

      this.timesliderUpdateTimer.Start();

      // this.VideoImage.Source = Video.Instance.VideoSource;
    }

    /// <summary>
    ///   The run automatic data aquisition.
    /// </summary>
    public void RunAutomaticDataAquisition()
    {
      StatusBarContent.Instance.StatusLabel = Labels.StatusIsCalculating;

      VianaNetApplication.Project.VideoData.Reset();

      // Set acquisition mode
      Video.Instance.IsDataAcquisitionRunning = true;

      if (Video.Instance.VideoMode == VideoMode.File)
      {
        // Go back to initial position
        Video.Instance.Revert();
        this.automaticDataAquisitionCurrentFrameCount = 0;
        this.automaticDataAquisitionTotalFrameCount =
          (int)
          ((this.TimelineSlider.SelectionEnd - this.TimelineSlider.SelectionStart)
           / (this.TimelineSlider.FrameTimeInNanoSeconds * VideoBase.NanoSecsToMilliSecs));

        Video.Instance.StepOneFrame(true);
      }
    }

    /// <summary>
    /// The set video mode.
    /// </summary>
    /// <param name="newVideoMode">
    /// The new video mode. 
    /// </param>
    public void SetVideoMode(VideoMode newVideoMode)
    {
      // Reset UI
      VianaNetApplication.Project.CalibrationData.Reset();
      VianaNetApplication.Project.VideoData.Reset();
      VianaNetApplication.Project.ProcessingData.Reset();
      this.BlobsControl.UpdateDataPoints();
      this.TimelineSlider.ResetSelection();
      this.CreateCrossHairLines();

      this.ShowOrHideCalibration(Visibility.Hidden);
      this.ShowOrHideClipRegion(Visibility.Hidden);

      if (Video.Instance.VideoMode == newVideoMode)
      {
        // No change in video mode, so nothing else to do.
        return;
      }

      Video.Instance.VideoMode = newVideoMode;

      switch (newVideoMode)
      {
        case VideoMode.File:

          // Update UI
          this.TimelineSlider.Visibility = Visibility.Visible;
          this.BtnRecord.Visibility = Visibility.Collapsed;
          this.SelectionPanel.Visibility = Visibility.Visible;
          this.BtnSeekPrevious.Visibility = Visibility.Visible;
          this.BtnSeekNext.Visibility = Visibility.Visible;
          break;
        case VideoMode.Capture:
          this.TimelineSlider.Visibility = Visibility.Collapsed;
          this.BtnRecord.Visibility = Visibility.Visible;
          this.SelectionPanel.Visibility = Visibility.Collapsed;
          this.BtnSeekPrevious.Visibility = Visibility.Collapsed;
          this.BtnSeekNext.Visibility = Visibility.Collapsed;
          break;
      }
    }

    /// <summary>
    /// The show calibration.
    /// </summary>
    /// <param name="show">
    /// The show. 
    /// </param>
    public void ShowCalibration(bool show)
    {
      if (VianaNetApplication.Project.CalibrationData.IsVideoCalibrated)
      {
        this.ShowOrHideCalibration(show ? Visibility.Visible : Visibility.Collapsed);
      }
    }

    /// <summary>
    /// The show clip region.
    /// </summary>
    /// <param name="show">
    /// The show. 
    /// </param>
    public void ShowClipRegion(bool show)
    {
      if (VianaNetApplication.Project.CalibrationData.HasClipRegion)
      {
        this.ShowOrHideClipRegion(show ? Visibility.Visible : Visibility.Collapsed);
      }
    }

    /// <summary>
    ///   The stop automatic data aquisition.
    /// </summary>
    public void StopAutomaticDataAquisition()
    {
      switch (Video.Instance.VideoMode)
      {
        case VideoMode.File:
          this.cancelCalculation = true;
          this.AutomaticAquisitionFinished();
          break;
        case VideoMode.Capture:
          this.AutomaticAquisitionFinished();
          break;
      }
    }

    /// <summary>
    ///   The update calibration.
    /// </summary>
    public void UpdateCalibration()
    {
      this.ShowOrHideCalibration(Visibility.Visible);
      this.PlaceCalibration();
    }

    /// <summary>
    ///   The update clipping region.
    /// </summary>
    public void UpdateClippingRegion()
    {
      this.ShowOrHideClipRegion(Visibility.Visible);
      this.PlaceClippingRegion();
    }

    #endregion

    #region Methods

    /// <summary>
    ///   The automatic aquisition finished.
    /// </summary>
    private void AutomaticAquisitionFinished()
    {
      Video.Instance.IsDataAcquisitionRunning = false;
      this.cancelCalculation = false;

      // if (Video.Instance.VideoMode == VideoMode.File)
      // {
      // Video.Instance.VideoPlayerElement.StepComplete -=
      // new EventHandler(VideoPlayerElement_StepComplete);
      // Video.Instance.VideoPlayerElement.FileComplete -=
      // new EventHandler(VideoPlayerElement_FileComplete);
      // }

      // Reset Statusbar
      StatusBarContent.Instance.StatusLabel = Labels.StatusBarReady;
      StatusBarContent.Instance.ProgressBarValue = 0;

      // Recalculate dependent data values
      VianaNetApplication.Project.VideoData.RefreshDistanceVelocityAcceleration();
    }

    /// <summary>
    /// The bottom line_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void BottomLineMouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
    }

    /// <summary>
    /// The calibration property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    /// <summary>
    ///   The create cross hair lines.
    /// </summary>
    private void CreateCrossHairLines()
    {
      if (this.blobHorizontalLines != null)
      {
        foreach (Line item in this.blobHorizontalLines)
        {
          this.OverlayCanvas.Children.Remove(item);
        }
      }

      if (this.blobVerticalLines != null)
      {
        foreach (Line item in this.blobVerticalLines)
        {
          this.OverlayCanvas.Children.Remove(item);
        }
      }

      this.blobHorizontalLines = new Line[VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects];
      this.blobVerticalLines = new Line[VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects];

      for (int i = 0; i < VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        var widthBinding = new Binding("ActualWidth") { ElementName = "VideoImage" };
        var newHorizontalLine = new Line
          {
            Visibility = Visibility.Hidden,
            Stroke = ProcessingData.TrackObjectColors[i],
            StrokeThickness = 2,
            X1 = 0,
            X2 = 0,
            Y1 = 0,
            Y2 = 0
          };
        newHorizontalLine.SetBinding(Line.X2Property, widthBinding);
        this.blobHorizontalLines[i] = newHorizontalLine;
        this.OverlayCanvas.Children.Add(newHorizontalLine);

        var heightBinding = new Binding("ActualHeight") { ElementName = "VideoImage" };
        var newVerticalLine = new Line
          {
            Visibility = Visibility.Hidden,
            Stroke = ProcessingData.TrackObjectColors[i],
            StrokeThickness = 2,
            X1 = 0,
            X2 = 0,
            Y1 = 0,
            Y2 = 0
          };
        newVerticalLine.SetBinding(Line.Y2Property, heightBinding);
        this.blobVerticalLines[i] = newVerticalLine;
        this.OverlayCanvas.Children.Add(newVerticalLine);
      }
    }

    /// <summary>
    /// The get scales.
    /// </summary>
    /// <param name="scaleX">
    /// The scale x. 
    /// </param>
    /// <param name="scaleY">
    /// The scale y. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    private bool GetScales(out double scaleX, out double scaleY)
    {
      // double sourceRatio = Video.Instance.VideoElement.NaturalVideoWidth / Video.Instance.VideoElement.NaturalVideoHeight;
      // double destinationRatio = this.VideoImage.ActualWidth / this.VideoImage.ActualHeight;

      // double uniformWidth = this.VideoImage.ActualHeight /
      // Video.Instance.VideoElement.NaturalVideoHeight *
      // Video.Instance.VideoElement.NaturalVideoWidth;
      // double uniformHeight = this.VideoImage.ActualHeight;

      // if (sourceRatio < destinationRatio)
      // {
      // uniformWidth = this.VideoImage.ActualWidth;
      // uniformHeight = this.VideoImage.ActualWidth /
      // Video.Instance.VideoElement.NaturalVideoWidth *
      // Video.Instance.VideoElement.NaturalVideoHeight;
      // }
      scaleX = this.VideoImage.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.VideoImage.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return !double.IsInfinity(scaleX) && !double.IsNaN(scaleX);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The image processing_ frame processed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ProcessingDataFrameProcessed(object sender, EventArgs e)
    {
      double scaleX;
      double scaleY;

      if (this.GetScales(out scaleX, out scaleY))
      {
        for (int i = 0; i < VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          Point? blobCenter = VianaNetApplication.Project.ProcessingData.CurrentBlobCenter[i];
          if (blobCenter.HasValue)
          {
            this.blobHorizontalLines[i].Visibility = Visibility.Visible;
            this.blobVerticalLines[i].Visibility = Visibility.Visible;
            this.blobHorizontalLines[i].Y1 = blobCenter.Value.Y * scaleY;
            this.blobHorizontalLines[i].Y2 = blobCenter.Value.Y * scaleY;
            this.blobVerticalLines[i].X1 = blobCenter.Value.X * scaleX;
            this.blobVerticalLines[i].X2 = blobCenter.Value.X * scaleX;
          }
          else
          {
            this.blobHorizontalLines[i].Visibility = Visibility.Collapsed;
            this.blobVerticalLines[i].Visibility = Visibility.Collapsed;
          }
        }
      }
    }

    /// <summary>
    /// The image processing_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ProcessingDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "TargetColor")
      {
        this.UpdateCrossHairColors();
      }
      else if (e.PropertyName == "IsTargetColorSet")
      {
        if (VianaNetApplication.Project.ProcessingData.IsTargetColorSet)
        {
          this.UpdateCrossHairColors();

          for (int i = 0; i < VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects; i++)
          {
            this.blobVerticalLines[i].Visibility = Visibility.Visible;
            this.blobHorizontalLines[i].Visibility = Visibility.Visible;
          }
        }
      }
      else if (e.PropertyName == "NumberOfTrackedObjects")
      {
        this.CreateCrossHairLines();
      }
    }

    /// <summary>
    /// The left line_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LeftLineMouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }

    /// <summary>
    /// The line_ mouse leave.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LineMouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
    }

    /// <summary>
    /// The line_ mouse left button down.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LineMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.currentLine = sender as Line;
      Mouse.Capture(this.currentLine);
    }

    /// <summary>
    /// The line_ mouse left button up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LineMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    /// <summary>
    /// The line_ mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LineMouseMove(object sender, MouseEventArgs e)
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
            if (newY + DefaultMargin < this.BottomLine.Y1)
            {
              this.currentLine.Y1 = newY;
              this.currentLine.Y2 = newY;
              this.LeftLine.Y1 = newY;
              this.RightLine.Y1 = newY;
            }

            break;
          case "BottomLine":
            if (newY > this.TopLine.Y1 + DefaultMargin)
            {
              this.currentLine.Y1 = newY;
              this.currentLine.Y2 = newY;
              this.LeftLine.Y2 = newY;
              this.RightLine.Y2 = newY;
            }

            break;
          case "LeftLine":
            if (newX + DefaultMargin < this.RightLine.X1)
            {
              this.currentLine.X1 = newX;
              this.currentLine.X2 = newX;
              this.TopLine.X1 = newX;
              this.BottomLine.X1 = newX;
            }

            break;
          case "RightLine":
            if (newX > this.LeftLine.X1 + DefaultMargin)
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

    /// <summary>
    /// The on video frame changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void OnVideoFrameChanged(object sender, EventArgs e)
    {
      this.ProcessImage();
    }

    /// <summary>
    /// Updates the image
    /// </summary>
    private void ProcessImage()
    {
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        VianaNetApplication.Project.ProcessingData.ProcessImage();
      }
      else
      {
        this.Dispatcher.BeginInvoke((ThreadStart)(() => VianaNetApplication.Project.ProcessingData.ProcessImage()));
      }
    }

    /// <summary>
    /// The overlay canvas_ size changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void OverlayCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.PlaceCalibration();
      this.PlaceClippingRegion();
      this.ProcessImage();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   The place calibration.
    /// </summary>
    private void PlaceCalibration()
    {
      double scaleX;
      double scaleY;

      if (this.GetScales(out scaleX, out scaleY))
      {
        Canvas.SetLeft(this.OriginPath, VianaNetApplication.Project.CalibrationData.OriginInPixel.X * scaleX - this.OriginPath.ActualWidth / 2);
        Canvas.SetTop(this.OriginPath, VianaNetApplication.Project.CalibrationData.OriginInPixel.Y * scaleY - this.OriginPath.ActualHeight / 2);
        this.RulerLine.X1 = VianaNetApplication.Project.CalibrationData.RulerStartPointInPixel.X * scaleX;
        this.RulerLine.Y1 = VianaNetApplication.Project.CalibrationData.RulerStartPointInPixel.Y * scaleY;
        this.RulerLine.X2 = VianaNetApplication.Project.CalibrationData.RulerEndPointInPixel.X * scaleX;
        this.RulerLine.Y2 = VianaNetApplication.Project.CalibrationData.RulerEndPointInPixel.Y * scaleY;
        double centerLineX = (this.RulerLine.X1 + this.RulerLine.X2) / 2;
        double centerLineY = (this.RulerLine.Y1 + this.RulerLine.Y2) / 2;

        Canvas.SetLeft(this.RulerLabelBorder, centerLineX - this.RulerLabelBorder.ActualWidth / 2);
        Canvas.SetTop(this.RulerLabelBorder, centerLineY - this.RulerLabelBorder.ActualHeight / 2);
      }
    }

    /// <summary>
    ///   The place clipping region.
    /// </summary>
    private void PlaceClippingRegion()
    {
      double scaleX;
      double scaleY;

      if (!this.GetScales(out scaleX, out scaleY))
      {
        return;
      }

      if (this.OuterRegion.IsVisible && VianaNetApplication.Project.CalibrationData.ClipRegion != Rect.Empty)
      {
        this.TopLine.X1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Left * scaleX;
        this.TopLine.X2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Right * scaleX;
        this.TopLine.Y1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Top * scaleY;
        this.TopLine.Y2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Top * scaleY;
        this.BottomLine.X1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Left * scaleX;
        this.BottomLine.X2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Right * scaleX;
        this.BottomLine.Y1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Bottom * scaleY;
        this.BottomLine.Y2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Bottom * scaleY;
        this.LeftLine.X1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Left * scaleX;
        this.LeftLine.X2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Left * scaleX;
        this.LeftLine.Y1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Top * scaleY;
        this.LeftLine.Y2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Bottom * scaleY;
        this.RightLine.X1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Right * scaleX;
        this.RightLine.X2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Right * scaleX;
        this.RightLine.Y1 = VianaNetApplication.Project.CalibrationData.ClipRegion.Top * scaleY;
        this.RightLine.Y2 = VianaNetApplication.Project.CalibrationData.ClipRegion.Bottom * scaleY;
        this.ResetOuterRegion();
      }
    }

    /// <summary>
    ///   The reset outer region.
    /// </summary>
    private void ResetOuterRegion()
    {
      var geometry = this.OuterRegion.Data as CombinedGeometry;
      var outerRectangleGeometry = geometry.Geometry1 as RectangleGeometry;
      outerRectangleGeometry.Rect = new Rect(0, 0, this.VideoImage.ActualWidth, this.VideoImage.ActualHeight);
      var innerRectangleGeometry = geometry.Geometry2 as RectangleGeometry;
      var innerRect = new Rect(
        new Point(this.LeftLine.X1, this.TopLine.Y1), new Point(this.RightLine.X1, this.BottomLine.Y1));
      innerRectangleGeometry.Rect = innerRect;

      double scaleX;
      double scaleY;
      if (this.GetScales(out scaleX, out scaleY))
      {
        Rect clipRect = innerRect;
        clipRect.Scale(1 / scaleX, 1 / scaleY);
        VianaNetApplication.Project.CalibrationData.ClipRegion = clipRect;
      }
    }

    /// <summary>
    /// The right line_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RightLineMouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }

    /// <summary>
    /// The show or hide calibration.
    /// </summary>
    /// <param name="visibility">
    /// The visibility. 
    /// </param>
    private void ShowOrHideCalibration(Visibility visibility)
    {
      this.OriginPath.Visibility = visibility;
      this.RulerLine.Visibility = visibility;
      this.RulerLabelBorder.Visibility = visibility;
    }

    /// <summary>
    /// The show or hide clip region.
    /// </summary>
    /// <param name="visibility">
    /// The visibility. 
    /// </param>
    private void ShowOrHideClipRegion(Visibility visibility)
    {
      this.TopLine.Visibility = visibility;
      this.BottomLine.Visibility = visibility;
      this.LeftLine.Visibility = visibility;
      this.RightLine.Visibility = visibility;
      this.OuterRegion.Visibility = visibility;
    }

    /// <summary>
    /// The top line_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TopLineMouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
    }

    /// <summary>
    ///   The update cross hair colors.
    /// </summary>
    private void UpdateCrossHairColors()
    {
      for (int i = 0; i < VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.blobHorizontalLines[i].Stroke = ProcessingData.TrackObjectColors[i];
        this.blobVerticalLines[i].Stroke = ProcessingData.TrackObjectColors[i];
      }
    }

    /// <summary>
    /// The video player element_ file complete.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void VideoPlayerElementFileComplete(object sender, EventArgs e)
    {
      this.Dispatcher.BeginInvoke(
        (ThreadStart)delegate
          {
            if (Video.Instance.IsDataAcquisitionRunning)
            {
              this.AutomaticAquisitionFinished();
            }
          });
    }

    /// <summary>
    /// The video player element_ step complete.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void VideoPlayerElementStepComplete(object sender, EventArgs e)
    {
      // Do Events
      this.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));

      // Run next sample
      this.Dispatcher.Invoke(
        (ThreadStart)delegate
          {
            if (Video.Instance.IsDataAcquisitionRunning)
            {
              this.automaticDataAquisitionCurrentFrameCount++;
              StatusBarContent.Instance.ProgressBarValue = (double)this.automaticDataAquisitionCurrentFrameCount
                                                           / (this.automaticDataAquisitionTotalFrameCount - 1) * 100;

              if (this.automaticDataAquisitionCurrentFrameCount == this.automaticDataAquisitionTotalFrameCount - 1
                  || this.cancelCalculation)
              {
                this.AutomaticAquisitionFinished();
                return;
              }

              Video.Instance.StepOneFrame(true);
            }
          });
    }

    /// <summary>
    /// The video player_ video file opened.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void VideoPlayerVideoFileOpened(object sender, EventArgs e)
    {
      // this.BlobsControl.UpdatedProcessedImage();
      // this.BlobsControl.UpdateScale();
      // this.timelineSlider.SelectionStart = 0;
      // this.timelineSlider.SelectionEnd = this.timelineSlider.Maximum;
    }

    /// <summary>
    /// The btn record_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void BtnRecordClick(object sender, RoutedEventArgs e)
    {
      bool wasCapturing = false;
      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        wasCapturing = true;
        this.SetVideoMode(VideoMode.None);
      }

      var saveVideoDialog = new SaveVideoDialog();
      if (saveVideoDialog.ShowDialog().GetValueOrDefault(false))
      {
        this.SetVideoMode(VideoMode.File);
        this.LoadVideo(saveVideoDialog.LastRecordedVideoFile);
      }
      else if (wasCapturing)
      {
        this.SetVideoMode(VideoMode.Capture);
      }
    }

    private bool isPlaying;

    /// <summary>
    /// The btn start_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void BtnPlayClick(object sender, RoutedEventArgs e)
    {
      if (this.isPlaying)
      {
        this.BtnPlayImage.Source = VianaNetApplication.GetImageSource("Start16.png");
        Video.Instance.Pause();
      }
      else
      {
        this.BtnPlayImage.Source = VianaNetApplication.GetImageSource("Pause16.png");
        Video.Instance.Play();
      }

      this.isPlaying = !this.isPlaying;
    }

    /// <summary>
    /// The btn stop_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void BtnStopClick(object sender, RoutedEventArgs e)
    {
      Video.Instance.Revert();
      this.TimelineSlider.Value = this.TimelineSlider.SelectionStart;
      this.isPlaying = false;
      this.BtnPlayImage.Source = VianaNetApplication.GetImageSource("Start16.png");
    }

    private void BtnSeekNextClick(object sender, RoutedEventArgs e)
    {
      this.StepForward();
    }

    private void BtnSeekPreviousClick(object sender, RoutedEventArgs e)
    {
      this.StepBackward();
    }

    /// <summary>
    /// The timeline slider_ drag completed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderDragCompleted(object sender, DragCompletedEventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds =
        (long)(this.TimelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
      this.isDragging = false;
    }

    /// <summary>
    /// The timeline slider_ drag delta.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderDragDelta(object sender, DragDeltaEventArgs e)
    {
      // Video.Instance.VideoPlayerElement.MediaPositionInMS = (long)timelineSlider.Value;
    }

    /// <summary>
    /// The timeline slider_ drag started.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderDragStarted(object sender, DragStartedEventArgs e)
    {
      this.isDragging = true;
    }

    /// <summary>
    /// The timeline slider_ selection and value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderSelectionAndValueChanged(object sender, EventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds =
        (long)(this.TimelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
    }

    /// <summary>
    /// The timeline slider_ selection end reached.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderSelectionEndReached(object sender, EventArgs e)
    {
      Video.Instance.Pause();
      this.BtnPlayImage.Source = VianaNetApplication.GetImageSource("Start16.png");
      this.isPlaying = false;
    }

    /// <summary>
    /// The timeline slider_ tick down clicked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderTickDownClicked(object sender, EventArgs e)
    {
      this.StepBackward();
    }

    /// <summary>
    /// The timeline slider_ tick up clicked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimelineSliderTickUpClicked(object sender, EventArgs e)
    {
      this.StepForward();
    }

    /// <summary>
    /// The timeslider update timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TimesliderUpdateTimerTick(object sender, EventArgs e)
    {
      if (!this.isDragging && Video.Instance.VideoMode == VideoMode.File)
      {
        double preciseTime = Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds;

        // double alignedTime = (int)(preciseTime / Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds) *
        // Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds;
        this.TimelineSlider.Value = preciseTime * VideoBase.NanoSecsToMilliSecs;
      }
    }

    /// <summary>
    /// Steps the video one frame backward, if available
    /// </summary>
    private void StepBackward()
    {
      if (this.TimelineSlider.Value >= this.TimelineSlider.SelectionStart + this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(false);
      }
    }

    /// <summary>
    /// Steps the video one frame forwared, if available
    /// </summary>
    private void StepForward()
    {
      if (this.TimelineSlider.Value <= this.TimelineSlider.SelectionEnd - this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(true);
      }
    }
    #endregion

    private void BtnSetCutoutLeftClick(object sender, RoutedEventArgs e)
    {
      this.TimelineSlider.SelectionStart = this.TimelineSlider.Value;
      this.TimelineSlider.UpdateSelectionTimes();
    }

    private void BtnSetCutoutRightClick(object sender, RoutedEventArgs e)
    {
      this.TimelineSlider.SelectionEnd = this.TimelineSlider.Value;
      this.TimelineSlider.UpdateSelectionTimes();
    }

    private void BtnSetZeroTimeClick(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.VideoData.TimeZeroPositionInMs = Video.Instance.FrameTimestampInMsWithoutOffest;
    }
  }
}