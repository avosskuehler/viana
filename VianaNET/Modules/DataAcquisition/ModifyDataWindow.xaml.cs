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
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Threading;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Controls;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Resources;

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
    /// Indicates the dragging of the time line thumb
    /// </summary>
    private bool isDraggingTimeLineThumb;

    /// <summary>
    /// Stores the dragging state of the (max) three objects
    /// </summary>
    private bool[] isDraggingCursor;

    /// <summary>
    /// Stores the arrow pointers
    /// </summary>
    private ArrowPointer[] arrowPointers;

    /// <summary>
    ///   The mouse down location.
    /// </summary>
    private Point mouseDownLocation;

    /// <summary>
    ///   The visual data point radius.
    /// </summary>
    private int visualDataPointRadius = 8;

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

      var arrowCount = Viana.Project.ProcessingData.NumberOfTrackedObjects;
      this.isDraggingCursor = new bool[arrowCount];
      this.arrowPointers = new ArrowPointer[arrowCount];
      for (int i = 0; i < arrowCount; i++)
      {
        var pointer = new ArrowPointer();
        pointer.Name = "Object" + (i + 1).ToString(CultureInfo.InvariantCulture);
        pointer.Stroke = ProcessingData.TrackObjectColors[i];
        pointer.Fill = Brushes.Transparent;
        pointer.Length = 50;
        pointer.CenterSpace = 5;
        pointer.HeadHeight = 6;
        pointer.HeadWidth = 15;
        pointer.Cursor = Cursors.Hand;
        pointer.MouseLeftButtonDown += this.ArrowShapeMouseLeftButtonDown;
        pointer.MouseMove += this.CursorEllipse_OnMouseMove;
        pointer.MouseLeftButtonUp += this.CursorEllipse_OnMouseLeftButtonUp;
        this.arrowPointers[i] = pointer;
        this.WindowCanvas.Children.Add(pointer);
      }

      this.SetVisibilityOfArrowCursor(true);

      this.CursorEllipse.Width = 2 * this.visualDataPointRadius;
      this.CursorEllipse.Height = 2 * this.visualDataPointRadius;

      this.timesliderUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
      this.timesliderUpdateTimer.Tick += this.TimesliderUpdateTimerTick;

      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        this.FramePanel.Visibility = Visibility.Hidden;
        this.TimelineSlider.Visibility = Visibility.Hidden;
      }

      this.SetVisibilityOfSharpCursor(false);
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
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
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
      this.SetVisibilityOfSharpCursor(false);
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
      //this.SetVisibilityOfSharpCursor(true);
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
    /// This captures the click location and transforms to video coordinates
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void PlayerMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        var scaledX = e.GetPosition(this.VideoImage).X;
        var scaledY = e.GetPosition(this.VideoImage).Y;
        var factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.VideoImage.ActualWidth;
        var factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.VideoImage.ActualHeight;
        var originalX = factorX * scaledX;
        var originalY = factorY * scaledY;

        Viana.Project.VideoData.AddPoint(this.IndexOfTrackedObject - 1, new Point(originalX, originalY));

        if (this.IndexOfTrackedObject == Viana.Project.ProcessingData.NumberOfTrackedObjects)
        {
          this.SetVisibilityOfSharpCursor(false);
        }

        this.IndexOfTrackedObject++;
        this.UpdateDataPointLocation();
      }
      else if (e.ChangedButton == MouseButton.Right)
      {
        this.StepFramesBackward();
        this.IndexOfTrackedObject = 1;
      }
    }

    private void ArrowShapeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var arrowShape = sender as ArrowPointer;
      if (arrowShape == null)
      {
        return;
      }

      var index = arrowShape.Name.Replace("Object", string.Empty);
      this.IndexOfTrackedObject = int.Parse(index);

      this.Cursor = Cursors.Hand;

      for (var i = 0; i < this.isDraggingCursor.Length; i++)
      {
        this.isDraggingCursor[i] = false;
      }

      this.isDraggingCursor[this.IndexOfTrackedObject - 1] = true;

      // Retrieve the coordinate of the mouse position.
      Point pt = e.GetPosition((UIElement)sender);
      this.MoveSharpCursorToScreenLocation(pt);
      this.SetVisibilityOfSharpCursor(true);
      this.MoveArrowCursorToScreenLocation(this.IndexOfTrackedObject, pt);
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
      var mouseMoveLocation = e.GetPosition(this.WindowCanvas);
      this.MoveSharpCursorToScreenLocation(mouseMoveLocation);

      if (Mouse.LeftButton == MouseButtonState.Pressed && this.Cursor == Cursors.Hand)
      {
        this.MoveArrowCursorToScreenLocation(this.IndexOfTrackedObject, mouseMoveLocation);
      }
    }

    private void CursorEllipse_OnMouseMove(object sender, MouseEventArgs e)
    {
      if (Mouse.LeftButton == MouseButtonState.Pressed && this.Cursor == Cursors.Hand)
      {
        var mouseMoveLocation = e.GetPosition(this.WindowCanvas);
        this.MoveSharpCursorToScreenLocation(mouseMoveLocation);
        this.MoveArrowCursorToScreenLocation(this.IndexOfTrackedObject, mouseMoveLocation);
      }
    }

    private void CursorEllipse_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.Cursor == Cursors.Hand)
      {
        this.SetVisibilityOfSharpCursor(false);
        this.Cursor = Cursors.Arrow;

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

      }
    }

    private void MoveSharpCursorToScreenLocation(Point newLocation)
    {
      this.HorizontalCursorLineLeft.Y1 = newLocation.Y;
      this.HorizontalCursorLineLeft.Y2 = newLocation.Y;
      this.HorizontalCursorLineLeft.X2 = newLocation.X - this.visualDataPointRadius;
      this.HorizontalCursorLineRight.Y1 = newLocation.Y;
      this.HorizontalCursorLineRight.Y2 = newLocation.Y;
      this.HorizontalCursorLineRight.X1 = newLocation.X + this.visualDataPointRadius;
      this.VerticalCursorLineTop.X1 = newLocation.X;
      this.VerticalCursorLineTop.X2 = newLocation.X;
      this.VerticalCursorLineTop.Y2 = newLocation.Y - this.visualDataPointRadius;
      this.VerticalCursorLineBottom.X1 = newLocation.X;
      this.VerticalCursorLineBottom.X2 = newLocation.X;
      this.VerticalCursorLineBottom.Y1 = newLocation.Y + this.visualDataPointRadius;
      Canvas.SetTop(this.CursorEllipse, newLocation.Y - this.visualDataPointRadius);
      Canvas.SetLeft(this.CursorEllipse, newLocation.X - this.visualDataPointRadius);

      // Update crosshair brush
      this.BrushOfCossHair = ProcessingData.TrackObjectColors[this.IndexOfTrackedObject - 1];
    }

    /// <summary>
    /// Show or hide cursor sharp.
    /// </summary>
    /// <param name="show">
    /// True, if cursor should be shown otherwise false.
    /// </param>
    private void SetVisibilityOfSharpCursor(bool show)
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
      this.isDraggingTimeLineThumb = false;
      this.UpdateDataPointLocation();
    }

    private void UpdateDataPointLocation()
    {
      this.ObjectIndexTextBox.Text = Labels.ModifyDataWindowTrackItemNumberHeader;
      var sample = Viana.Project.VideoData.Samples.GetSampleByFrameindex(Video.Instance.FrameIndex);
      var isSet = false;
      if (sample != null)
      {
        this.SetVisibilityOfArrowCursor(true);

        var factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.VideoImage.ActualWidth;
        var factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.VideoImage.ActualHeight;

        for (int i = 1; i <= Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          if (sample.Object[i - 1] != null)
          {
            var scaledX = sample.Object[i - 1].PixelX / factorX;
            var scaledY = sample.Object[i - 1].PixelY / factorY;

            var screenLocation = this.VideoImage.TranslatePoint(new Point(scaledX, scaledY), this.WindowCanvas);

            this.MoveArrowCursorToScreenLocation(i, screenLocation);
          }
          else
          {
            this.HideArrowPointer(i);
            this.SetVisibilityOfSharpCursor(true);
            this.ObjectIndexTextBox.Text = Labels.ManualDataAcquisitionTrackItemNumberHeader;
            if (!isSet)
            {
              this.IndexOfTrackedObject = i;
              isSet = true;
            }
          }
        }
      }
      else
      {
        this.IndexOfTrackedObject = 1;
        this.SetVisibilityOfArrowCursor(false);
        this.SetVisibilityOfSharpCursor(true);
        this.ObjectIndexTextBox.Text = Labels.ManualDataAcquisitionTrackItemNumberHeader;
      }
    }

    private void HideArrowPointer(int i)
    {
      this.arrowPointers[i - 1].Visibility = Visibility.Collapsed;
    }

    private void SetVisibilityOfArrowCursor(bool shouldShow)
    {
      if (shouldShow)
      {
        for (int i = 0; i < this.arrowPointers.Length; i++)
        {
          var pointer = this.arrowPointers[i];
          pointer.Visibility = i < Viana.Project.ProcessingData.NumberOfTrackedObjects ? Visibility.Visible : Visibility.Hidden;
        }
      }
      else
      {
        foreach (var arrowPointer in this.arrowPointers)
        {
          arrowPointer.Visibility = Visibility.Hidden;
        }
      }
    }

    private void MoveArrowCursorToScreenLocation(int objectIndex, Point screenLocation)
    {
      this.arrowPointers[objectIndex - 1].X = screenLocation.X;
      this.arrowPointers[objectIndex - 1].Y = screenLocation.Y;
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
      this.isDraggingTimeLineThumb = true;
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
      if (!this.isDraggingTimeLineThumb && Video.Instance.VideoMode == VideoMode.File)
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