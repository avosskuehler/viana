// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifyDataWindow.xaml.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.DataAcquisition
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Threading;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   Interaction logic for ModifyDataWindow.xaml
  /// </summary>
  public partial class ModifyDataWindow
  {
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="BrushOfCossHair" />.
    /// </summary>
    public static readonly DependencyProperty BrushOfCossHairProperty = DependencyProperty.Register(
      "BrushOfCossHair",
      typeof(SolidColorBrush),
      typeof(ModifyDataWindow),
      new FrameworkPropertyMetadata(ProcessingData.TrackObjectColors[0], OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IndexOfTrackedObject" />.
    /// </summary>
    public static readonly DependencyProperty IndexOfTrackedObjectProperty =
      DependencyProperty.Register(
        "IndexOfTrackedObject",
        typeof(int),
        typeof(ModifyDataWindow),
        new FrameworkPropertyMetadata(1, OnPropertyChanged));

    #endregion

    #region Fields

    /// <summary>
    ///   The timeslider update timer.
    /// </summary>
    private readonly DispatcherTimer timesliderUpdateTimer;

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

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyDataWindow"/> class. 
    ///   Initializes a new instance of the <see cref="ManualDataAquisitionWindow"/> class.
    /// </summary>
    public ModifyDataWindow()
    {
      this.InitializeComponent();

      this.ObjectIndexPanel.DataContext = this;
      this.WindowCanvas.DataContext = this;

      this.VisualDataPoint.Stroke = Brushes.Green; //ProcessingData.TrackObjectColors[0];

      this.CursorEllipse.Width = 2 * this.visualDataPointRadius;
      this.CursorEllipse.Height = 2 * this.visualDataPointRadius;

      this.timesliderUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
      this.timesliderUpdateTimer.Tick += this.TimesliderUpdateTimerTick;

      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        this.FramePanel.Visibility = Visibility.Hidden;
        this.TimelineSlider.Visibility = Visibility.Hidden;
      }

      this.ShowHideCursorSharp(true);
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

    #endregion

    #region Methods

    /// <summary>
    /// Moves to given frame index.
    /// </summary>
    /// <param name="frameIndex">Index of the frame.</param>
    public void MoveToFrame(int frameIndex)
    {
      var newMediaPosition = Video.Instance.VideoPlayerElement.FrameTimeInNanoSeconds * frameIndex;
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds = newMediaPosition;
      this.TimelineSlider.Value = newMediaPosition * VideoBase.NanoSecsToMilliSecs;
      Video.Instance.VideoPlayerElement.UpdateFrameIndex();
      this.UpdateDataPointLocation();
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="obj">
    /// The source of the event. This.
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data.
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var window = obj as ModifyDataWindow;

      if (window == null)
      {
        return;
      }

      // Reset index if appropriate
      if (window.IndexOfTrackedObject > Viana.Project.ProcessingData.NumberOfTrackedObjects)
      {
        window.IndexOfTrackedObject = 1;
      }

      // Update crosshair brush
      window.BrushOfCossHair = ProcessingData.TrackObjectColors[window.IndexOfTrackedObject - 1];
    }

    /// <summary>
    /// Handles the Click event of the ButtonReady control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ButtonReadyClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the MouseEnter event of the ControlPanel control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void ControlPanelMouseEnter(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(false);
    }

    /// <summary>
    /// Handles the MouseLeave event of the ControlPanel control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void ControlPanelMouseLeave(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(true);
    }

    /// <summary>
    /// Drag window mouse down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void DragWindowMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.mouseDownLocation = args.GetPosition(this.WindowCanvas);
      this.GridTop.CaptureMouse();
    }

    /// <summary>
    /// Drag window mouse up.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void DragWindowMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.GridTop.ReleaseMouseCapture();
    }

    /// <summary>
    /// Hides the window.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void HideWindow(object sender, RoutedEventArgs e)
    {
      this.ControlPanel.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void MinimizeWindow(object sender, MouseButtonEventArgs e)
    {
      this.HelpText.Visibility = this.HelpText.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Handles the MouseDown event of the player control.
    ///   This captures the click location and transforms to video coordinates
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void PlayerMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        double factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.VideoImage.ActualWidth;
        double factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.VideoImage.ActualHeight;
        double originalX = factorX * scaledX;
        double originalY = factorY * scaledY;

        Viana.Project.VideoData.UpdatePoint(
          Video.Instance.FrameIndex,
          this.IndexOfTrackedObject - 1,
          new Point(originalX, originalY));

        double canvasPosX = e.GetPosition(this.WindowCanvas).X;
        double canvasPosY = e.GetPosition(this.WindowCanvas).Y;

        Canvas.SetTop(this.VisualDataPoint, canvasPosY - this.visualDataPointRadius);
        Canvas.SetLeft(this.VisualDataPoint, canvasPosX - this.visualDataPointRadius);

        this.IndexOfTrackedObject++;
      }
    }

    /// <summary>
    /// Handles the MouseMove event of the player control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
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
    /// Show or hide cursor sharp.
    /// </summary>
    /// <param name="show">
    /// True, if cursor should be shown otherwise false.
    /// </param>
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
    ///   The step frames backward.
    /// </summary>
    private void StepFramesBackward()
    {
      if (this.TimelineSlider.Value >= this.TimelineSlider.SelectionStart + this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepFrames(false, Viana.Project.VideoData.UseEveryNthPoint);
        this.UpdateDataPointLocation();
      }
    }

    /// <summary>
    ///   The step frames forward.
    /// </summary>
    private void StepFramesForward()
    {
      if (this.TimelineSlider.Value <= this.TimelineSlider.SelectionEnd - this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepFrames(true, Viana.Project.VideoData.UseEveryNthPoint);
        this.UpdateDataPointLocation();
      }
    }

    /// <summary>
    /// Handles the DragCompleted event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="DragCompletedEventArgs"/> instance containing the event data.
    /// </param>
    private void TimelineSliderDragCompleted(object sender, DragCompletedEventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds =
        (long)(this.TimelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
      this.isDragging = false;
      this.UpdateDataPointLocation();
    }

    private void UpdateDataPointLocation()
    {
      var sample = Viana.Project.VideoData.Samples.GetSampleByFrameindex(Video.Instance.FrameIndex);
      if (sample != null)
      {
        this.VisualDataPoint.Visibility = Visibility.Visible;
        double factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.VideoImage.ActualWidth;
        double factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.VideoImage.ActualHeight;

        double scaledX = sample.Object[this.IndexOfTrackedObject - 1].PixelX / factorX;
        double scaledY = sample.Object[this.IndexOfTrackedObject - 1].PixelY / factorY;

        var screenLocation = this.VideoImage.TranslatePoint(new Point(scaledX, scaledY), this.WindowCanvas);

        Canvas.SetTop(this.VisualDataPoint, screenLocation.Y - this.visualDataPointRadius);
        Canvas.SetLeft(this.VisualDataPoint, screenLocation.X - this.visualDataPointRadius);
      }
      else
      {
        this.VisualDataPoint.Visibility = Visibility.Collapsed;
      }
    }

    /// <summary>
    /// Handles the DragStarted event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="DragStartedEventArgs"/> instance containing the event data.
    /// </param>
    private void TimelineSliderDragStarted(object sender, DragStartedEventArgs e)
    {
      this.isDragging = true;
    }

    /// <summary>
    /// Handles the TickDownClicked event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="EventArgs"/> instance containing the event data.
    /// </param>
    private void TimelineSliderTickDownClicked(object sender, EventArgs e)
    {
      this.StepFramesBackward();
    }

    /// <summary>
    /// Handles the TickUpClicked event of the timelineSlider control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="EventArgs"/> instance containing the event data.
    /// </param>
    private void TimelineSliderTickUpClicked(object sender, EventArgs e)
    {
      this.StepFramesForward();
    }

    /// <summary>
    /// Handles the Tick event of the timesliderUpdateTimer control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="EventArgs"/> instance containing the event data.
    /// </param>
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
    ///   by moving the control panel.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void WindowCanvasMouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.WindowCanvas);

      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.GridTop.IsMouseOver)
        {
          var currentLocation = new Point
                                  {
                                    X = Canvas.GetLeft(this.ControlPanel),
                                    Y = Canvas.GetTop(this.ControlPanel)
                                  };

          Canvas.SetTop(this.ControlPanel, currentLocation.Y + mouseMoveLocation.Y - this.mouseDownLocation.Y);
          Canvas.SetLeft(this.ControlPanel, currentLocation.X + mouseMoveLocation.X - this.mouseDownLocation.X);
          this.mouseDownLocation = mouseMoveLocation;
        }
      }
    }

    /// <summary>
    /// Handles the Loaded event of the Window control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      this.HorizontalCursorLineLeft.X2 = this.WindowCanvas.ActualWidth;
      this.HorizontalCursorLineRight.X2 = this.WindowCanvas.ActualWidth;
      this.VerticalCursorLineTop.Y2 = this.WindowCanvas.ActualHeight;
      this.VerticalCursorLineBottom.Y2 = this.WindowCanvas.ActualHeight;

      this.MoveToFrame(Video.Instance.FrameIndex);
      this.timesliderUpdateTimer.Start();
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the Window control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="KeyEventArgs"/> instance containing the event data.
    /// </param>
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
        this.StepFramesForward();
      }
      else if (e.Key == Key.Left)
      {
        e.Handled = true;
        this.StepFramesBackward();
      }
    }

    #endregion
  }
}