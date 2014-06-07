// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualDataAquisitionWindow.xaml.cs" company="Freie Universität Berlin">
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
//   Interaction logic for ReconstructionWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VianaNET.Application;

namespace VianaNET.Modules.DataAcquisition
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;
  using System.Windows.Threading;

  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   Interaction logic for ReconstructionWindow.xaml
  /// </summary>
  public partial class ManualDataAquisitionWindow : Window
  {
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="BrushOfCossHair" />.
    /// </summary>
    public static readonly DependencyProperty BrushOfCossHairProperty = DependencyProperty.Register(
      "BrushOfCossHair",
      typeof(SolidColorBrush),
      typeof(ManualDataAquisitionWindow),
      new FrameworkPropertyMetadata(Project.TrackObjectColors[0], OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IndexOfTrackedObject" />.
    /// </summary>
    public static readonly DependencyProperty IndexOfTrackedObjectProperty =
      DependencyProperty.Register(
        "IndexOfTrackedObject",
        typeof(int),
        typeof(ManualDataAquisitionWindow),
        new FrameworkPropertyMetadata(1, OnPropertyChanged));

    #endregion

    #region Fields

    /// <summary>
    ///   The timeslider update timer.
    /// </summary>
    private readonly DispatcherTimer timesliderUpdateTimer;

    /// <summary>
    ///   The index of visual data point ring buffer.
    /// </summary>
    private int indexOfVisualDataPointRingBuffer;

    /// <summary>
    ///   The is dragging.
    /// </summary>
    private bool isDragging;

    /// <summary>
    ///   The mouse down location.
    /// </summary>
    private Point mouseDownLocation;

    /// <summary>
    ///   The visual data point radius.
    /// </summary>
    private int visualDataPointRadius = 5;

    /// <summary>
    ///   The visual data point ring buffer size.
    /// </summary>
    private int visualDataPointRingBufferSize = 5;

    /// <summary>
    ///   The visual data points.
    /// </summary>
    private List<Ellipse>[] visualDataPoints;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ManualDataAquisitionWindow" /> class.
    /// </summary>
    public ManualDataAquisitionWindow()
    {
      this.InitializeComponent();

      this.ObjectIndexPanel.DataContext = this;
      this.WindowCanvas.DataContext = this;

      this.indexOfVisualDataPointRingBuffer = 0;
      this.CreateVisualDataPoints(7);
      this.SkipPointCount = 1;

      this.CursorEllipse.Width = 2 * this.visualDataPointRadius;
      this.CursorEllipse.Height = 2 * this.visualDataPointRadius;

      this.timesliderUpdateTimer = new DispatcherTimer();
      this.timesliderUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
      this.timesliderUpdateTimer.Tick += this.TimesliderUpdateTimerTick;

      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        this.FramePanel.Visibility = Visibility.Hidden;
        this.TimelineSlider.Visibility = Visibility.Hidden;
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the index of the currently tracked object
    /// </summary>
    public SolidColorBrush BrushOfCossHair
    {
      get
      {
        return (SolidColorBrush)this.GetValue(BrushOfCossHairProperty);
      }

      set
      {
        this.SetValue(BrushOfCossHairProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the index of the currently tracked object
    /// </summary>
    public int IndexOfTrackedObject
    {
      get
      {
        return (int)this.GetValue(IndexOfTrackedObjectProperty);
      }

      set
      {
        this.SetValue(IndexOfTrackedObjectProperty, value);
      }
    }

    public int SkipPointCount { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">
    /// The source of the event. This. 
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var window = obj as ManualDataAquisitionWindow;

      if (window == null)
      {
        return;
      }

      // Update crosshair brush
      if (args.Property == IndexOfTrackedObjectProperty)
      {
        // Reset index if appropriate
        if (window.IndexOfTrackedObject > Viana.Project.ProcessingData.NumberOfTrackedObjects)
        {
          window.IndexOfTrackedObject = 1;
        }

        window.BrushOfCossHair = new SolidColorBrush(Viana.Project.ProcessingData.TargetColor[window.IndexOfTrackedObject - 1]);
      }
    }

    /// <summary>
    /// Handles the Click event of the ButtonReady control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ButtonReadyClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the MouseEnter event of the ControlPanel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void ControlPanelMouseEnter(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(false);
    }

    /// <summary>
    /// Handles the MouseLeave event of the ControlPanel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void ControlPanelMouseLeave(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(true);
    }

    /// <summary>
    /// This method creates circles for the last few data points
    /// </summary>
    /// <param name="count">The number of points to be shown. </param>
    private void CreateVisualDataPoints(int count)
    {
      this.visualDataPointRingBufferSize = count;

      // Remove old visual data points for all tracked objects
      if (this.visualDataPoints != null)
      {
        for (int j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
        {
          foreach (Ellipse item in this.visualDataPoints[j])
          {
            if (this.WindowCanvas.Children.Contains(item))
            {
              this.WindowCanvas.Children.Remove(item);
            }
          }

          this.visualDataPoints[j].Clear();
        }
      }

      this.indexOfVisualDataPointRingBuffer = 0;

      // Create new visual data points if appropriate
      if (count > 0)
      {
        this.visualDataPoints = new List<Ellipse>[Viana.Project.ProcessingData.NumberOfTrackedObjects];
        for (int j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
        {
          this.visualDataPoints[j] = new List<Ellipse>(count);
          for (int i = 0; i < count; i++)
          {
            var visualDataPoint = new Ellipse();
            visualDataPoint.Width = 2 * this.visualDataPointRadius;
            visualDataPoint.Height = 2 * this.visualDataPointRadius;
            visualDataPoint.Stroke = new SolidColorBrush(Viana.Project.ProcessingData.TargetColor[j]);
            visualDataPoint.IsEnabled = false;
            visualDataPoint.IsHitTestVisible = false;

            this.WindowCanvas.Children.Add(visualDataPoint);
            this.visualDataPoints[j].Add(visualDataPoint);
          }
        }
      }
    }

    /// <summary>
    /// Drag window mouse down.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void DragWindowMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.mouseDownLocation = args.GetPosition(this.WindowCanvas);
      this.GridTop.CaptureMouse();
    }

    /// <summary>
    /// Drag window mouse up.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void DragWindowMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.GridTop.ReleaseMouseCapture();
    }

    /// <summary>
    /// Hides the window.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void HideWindow(object sender, RoutedEventArgs e)
    {
      this.ControlPanel.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void MinimizeWindow(object sender, MouseButtonEventArgs e)
    {
      if (this.HelpText.Visibility == Visibility.Visible)
      {
        this.HelpText.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.HelpText.Visibility = Visibility.Visible;
      }
    }

    /// <summary>
    /// Handles the ValueChanged event of the RecentPointCount control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Decimal}"/> instance containing the event data.</param>
    private void RecentPointCountValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      this.CreateVisualDataPoints((int)e.NewValue);
    }

    /// <summary>
    /// Show or hide cursor sharp.
    /// </summary>
    /// <param name="show">True, if cursor should be shown otherwise false.</param>
    private void ShowHideCursorSharp(bool show)
    {
      Visibility vis = show ? Visibility.Visible : Visibility.Collapsed;
      this.VerticalCursorLineBottom.Visibility = vis;
      this.VerticalCursorLineTop.Visibility = vis;
      this.HorizontalCursorLineLeft.Visibility = vis;
      this.HorizontalCursorLineRight.Visibility = vis;
      this.CursorEllipse.Visibility = vis;
    }

    /// <summary>
    ///   The step one frame backward.
    /// </summary>
    private void StepOneFrameBackward()
    {
      if (this.TimelineSlider.Value >= this.TimelineSlider.SelectionStart + this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepFrames(false, this.SkipPointCount);
      }
    }

    /// <summary>
    ///   The step one frame forward.
    /// </summary>
    private void StepOneFrameForward()
    {
      if (this.TimelineSlider.Value <= this.TimelineSlider.SelectionEnd - this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepFrames(true, this.SkipPointCount);
      }
    }

    /// <summary>
    /// Handles the Loaded event of the Window control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      this.HorizontalCursorLineLeft.X2 = this.WindowCanvas.ActualWidth;
      this.HorizontalCursorLineRight.X2 = this.WindowCanvas.ActualWidth;
      this.VerticalCursorLineTop.Y2 = this.WindowCanvas.ActualHeight;
      this.VerticalCursorLineBottom.Y2 = this.WindowCanvas.ActualHeight;

      this.timesliderUpdateTimer.Start();
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the Window control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    private void WindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Escape)
      {
        e.Handled = true;
        this.Close();
      }
      else if (e.Key == Key.Right)
      {
        e.Handled = true;
        this.StepOneFrameForward();
      }
      else if (e.Key == Key.Left)
      {
        e.Handled = true;
        this.StepOneFrameBackward();
      }
    }

    /// <summary>
    /// Handles the MouseDown event of the player control.
    /// This captures the click location and transforms to video coordinates
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void PlayerMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        double factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.VideoImage.ActualWidth;
        double factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.VideoImage.ActualHeight;
        double spaceX = 0; // (this.videoSurfaceClone.ActualWidth - this.videoSurfaceClone.UniformWidth) / 2;
        double spaceY = 0; // (this.videoSurfaceClone.ActualHeight - this.videoSurfaceClone.UniformHeight) / 2;
        double originalX = factorX * (scaledX - spaceX);
        double originalY = factorY * (scaledY - spaceY);

        Viana.Project.VideoData.AddPoint(this.IndexOfTrackedObject - 1, new Point(originalX, originalY));

        if (this.IndexOfTrackedObject == Viana.Project.ProcessingData.NumberOfTrackedObjects)
        {
          this.StepOneFrameForward();
        }

        double canvasPosX = e.GetPosition(this.WindowCanvas).X;
        double canvasPosY = e.GetPosition(this.WindowCanvas).Y;

        if (this.visualDataPointRingBufferSize > 0)
        {
          Canvas.SetTop(
            this.visualDataPoints[this.IndexOfTrackedObject - 1][this.indexOfVisualDataPointRingBuffer],
            canvasPosY - this.visualDataPointRadius);

          Canvas.SetLeft(
            this.visualDataPoints[this.IndexOfTrackedObject - 1][this.indexOfVisualDataPointRingBuffer],
            canvasPosX - this.visualDataPointRadius);

          this.indexOfVisualDataPointRingBuffer++;
          if (this.indexOfVisualDataPointRingBuffer >= this.visualDataPointRingBufferSize)
          {
            this.indexOfVisualDataPointRingBuffer = 0;
          }
        }

        this.IndexOfTrackedObject++;
      }
      else if (e.ChangedButton == MouseButton.Right)
      {
        this.StepOneFrameBackward();
        this.IndexOfTrackedObject = 1;
      }
    }

    /// <summary>
    /// Handles the MouseMove event of the player control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void PlayerMouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.WindowCanvas);

      this.HorizontalCursorLineLeft.Y1 = mouseMoveLocation.Y;
      this.HorizontalCursorLineLeft.Y2 = mouseMoveLocation.Y;
      this.HorizontalCursorLineLeft.X2 = mouseMoveLocation.X - this.visualDataPointRadius;
      this.HorizontalCursorLineRight.Y1 = mouseMoveLocation.Y;
      this.HorizontalCursorLineRight.Y2 = mouseMoveLocation.Y;
      this.HorizontalCursorLineRight.X1 = mouseMoveLocation.X + this.visualDataPointRadius;
      this.VerticalCursorLineTop.X1 = mouseMoveLocation.X;
      this.VerticalCursorLineTop.X2 = mouseMoveLocation.X;
      this.VerticalCursorLineTop.Y2 = mouseMoveLocation.Y - this.visualDataPointRadius;
      this.VerticalCursorLineBottom.X1 = mouseMoveLocation.X;
      this.VerticalCursorLineBottom.X2 = mouseMoveLocation.X;
      this.VerticalCursorLineBottom.Y1 = mouseMoveLocation.Y + this.visualDataPointRadius;
      Canvas.SetTop(this.CursorEllipse, mouseMoveLocation.Y - this.visualDataPointRadius);
      Canvas.SetLeft(this.CursorEllipse, mouseMoveLocation.X - this.visualDataPointRadius);
    }

    /// <summary>
    /// Handles the DragCompleted event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="DragCompletedEventArgs"/> instance containing the event data.</param>
    private void TimelineSliderDragCompleted(object sender, DragCompletedEventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds =
        (long)(this.TimelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
      this.isDragging = false;
    }

    /// <summary>
    /// Handles the DragStarted event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="DragStartedEventArgs"/> instance containing the event data.</param>
    private void TimelineSliderDragStarted(object sender, DragStartedEventArgs e)
    {
      this.isDragging = true;
    }

    /// <summary>
    /// Handles the TickDownClicked event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void TimelineSliderTickDownClicked(object sender, EventArgs e)
    {
      this.StepOneFrameBackward();
    }

    /// <summary>
    /// Handles the TickUpClicked event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void TimelineSliderTickUpClicked(object sender, EventArgs e)
    {
      this.StepOneFrameForward();
    }

    /// <summary>
    /// Handles the Tick event of the timesliderUpdateTimer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void TimesliderUpdateTimerTick(object sender, EventArgs e)
    {
      if (!this.isDragging && Video.Instance.VideoMode == VideoMode.File)
      {
        double preciseTime = Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds;
        this.TimelineSlider.Value = preciseTime * VideoBase.NanoSecsToMilliSecs;
        Video.Instance.VideoPlayerElement.UpdateFrameIndex();
      }
    }

    /// <summary>
    /// Handles the MouseMove event of the windowCanvas control
    /// by moving the control panel.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void WindowCanvasMouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.WindowCanvas);

      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.GridTop.IsMouseOver)
        {
          var currentLocation = new Point();
          currentLocation.X = Canvas.GetLeft(this.ControlPanel);
          currentLocation.Y = Canvas.GetTop(this.ControlPanel);

          Canvas.SetTop(this.ControlPanel, currentLocation.Y + mouseMoveLocation.Y - this.mouseDownLocation.Y);
          Canvas.SetLeft(this.ControlPanel, currentLocation.X + mouseMoveLocation.X - this.mouseDownLocation.X);
          this.mouseDownLocation = mouseMoveLocation;
        }
      }
    }

    #endregion

    /// <summary>
    /// Handles the OnValueChanged event of the SkipPointCount control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Decimal}"/> instance containing the event data.</param>
    private void SkipPointCount_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      this.SkipPointCount = (int)e.NewValue + 1;
    }
  }
}