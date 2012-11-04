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
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="BrushOfCossHair" />.
    /// </summary>
    public static readonly DependencyProperty BrushOfCossHairProperty = DependencyProperty.Register(
      "BrushOfCossHair", 
      typeof(SolidColorBrush), 
      typeof(ManualDataAquisitionWindow), 
      new FrameworkPropertyMetadata(ImageProcessing.TrackObjectColors[0], OnPropertyChanged));

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

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ManualDataAquisitionWindow" /> class.
    /// </summary>
    public ManualDataAquisitionWindow()
    {
      this.InitializeComponent();

      this.ObjectIndexPanel.DataContext = this;
      this.windowCanvas.DataContext = this;

      this.indexOfVisualDataPointRingBuffer = 0;
      this.CreateVisualDataPoints(7);

      this.CursorEllipse.Width = 2 * this.visualDataPointRadius;
      this.CursorEllipse.Height = 2 * this.visualDataPointRadius;

      this.timesliderUpdateTimer = new DispatcherTimer();
      this.timesliderUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
      this.timesliderUpdateTimer.Tick += this.timesliderUpdateTimer_Tick;

      // this.videoSurfaceClone = Video.Instance.VideoElement.CloneD3DRenderer();
      // this.videoSurfaceClone.MouseDown += new MouseButtonEventHandler(player_MouseDown);
      // this.videoSurfaceClone.MouseMove += new MouseEventHandler(player_MouseMove);
      // this.playerContainerGrid.Children.Insert(0, this.videoSurfaceClone);
      // this.videoSurfaceClone.InvalidateVideoImage();
      // VideoPlayer.Instance.AttachToControl(this.playerContainerGrid);
      // VideoPlayer.Instance.MouseDown += new MouseButtonEventHandler(player_MouseDown);
      // VideoPlayer.Instance.MouseMove += new MouseEventHandler(player_MouseMove);

      // this.timelineSlider.SelectionStart = 0;
      // this.timelineSlider.SelectionEnd = this.timelineSlider.Maximum;
      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        this.FramePanel.Visibility = Visibility.Hidden;
        this.timelineSlider.Visibility = Visibility.Hidden;
      }
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="ManualDataAquisitionWindow" /> class.
    /// </summary>
    ~ManualDataAquisitionWindow()
    {
      //// Unregister event listeners.
      // VideoPlayer.Instance.MouseDown -= new MouseButtonEventHandler(player_MouseDown);
      // VideoPlayer.Instance.MouseMove -= new MouseEventHandler(player_MouseMove);
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
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

      // Reset index if appropriate
      if (window.IndexOfTrackedObject > Video.Instance.ImageProcessing.NumberOfTrackedObjects)
      {
        window.IndexOfTrackedObject = 1;
      }

      // Update crosshair brush
      window.BrushOfCossHair = ImageProcessing.TrackObjectColors[window.IndexOfTrackedObject - 1];
    }

    /// <summary>
    /// The button ready_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonReady_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The control panel_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ControlPanel_MouseEnter(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(false);
    }

    /// <summary>
    /// The control panel_ mouse leave.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ControlPanel_MouseLeave(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(true);
    }

    /// <summary>
    /// The create visual data points.
    /// </summary>
    /// <param name="count">
    /// The count. 
    /// </param>
    private void CreateVisualDataPoints(int count)
    {
      this.visualDataPointRingBufferSize = count;

      // Remove old visual data points for all tracked objects
      if (this.visualDataPoints != null)
      {
        for (int j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
        {
          foreach (Ellipse item in this.visualDataPoints[j])
          {
            if (this.windowCanvas.Children.Contains(item))
            {
              this.windowCanvas.Children.Remove(item);
            }
          }

          this.visualDataPoints[j].Clear();
        }
      }

      this.indexOfVisualDataPointRingBuffer = 0;

      // Create new visual data points if appropriate
      if (count > 0)
      {
        this.visualDataPoints = new List<Ellipse>[Video.Instance.ImageProcessing.NumberOfTrackedObjects];
        for (int j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
        {
          this.visualDataPoints[j] = new List<Ellipse>(count);
          for (int i = 0; i < count; i++)
          {
            var visualDataPoint = new Ellipse();
            visualDataPoint.Width = 2 * this.visualDataPointRadius;
            visualDataPoint.Height = 2 * this.visualDataPointRadius;
            visualDataPoint.Stroke = ImageProcessing.TrackObjectColors[j];
            visualDataPoint.IsEnabled = false;
            visualDataPoint.IsHitTestVisible = false;

            this.windowCanvas.Children.Add(visualDataPoint);
            this.visualDataPoints[j].Add(visualDataPoint);
          }
        }
      }
    }

    /// <summary>
    /// The drag window mouse down.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private void DragWindowMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.mouseDownLocation = args.GetPosition(this.windowCanvas);
      this.GridTop.CaptureMouse();
    }

    /// <summary>
    /// The drag window mouse up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DragWindowMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.GridTop.ReleaseMouseCapture();
    }

    /// <summary>
    /// The hide window.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void HideWindow(object sender, RoutedEventArgs e)
    {
      this.ControlPanel.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// The minimize window.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
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
    /// The recent point count_ value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RecentPointCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      this.CreateVisualDataPoints((int)e.NewValue);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The show hide cursor sharp.
    /// </summary>
    /// <param name="show">
    /// The show. 
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
    ///   The step one frame backward.
    /// </summary>
    private void StepOneFrameBackward()
    {
      if (this.timelineSlider.Value >= this.timelineSlider.SelectionStart + this.timelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(false);
      }
    }

    /// <summary>
    ///   The step one frame forward.
    /// </summary>
    private void StepOneFrameForward()
    {
      if (this.timelineSlider.Value <= this.timelineSlider.SelectionEnd - this.timelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(true);
      }
    }

    /// <summary>
    /// The window_ loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.HorizontalCursorLineLeft.X2 = this.windowCanvas.ActualWidth;
      this.HorizontalCursorLineRight.X2 = this.windowCanvas.ActualWidth;
      this.VerticalCursorLineTop.Y2 = this.windowCanvas.ActualHeight;
      this.VerticalCursorLineBottom.Y2 = this.windowCanvas.ActualHeight;

      this.timesliderUpdateTimer.Start();
    }

    /// <summary>
    /// The window_ preview key down.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Escape)
      {
        e.Handled = true;
        this.Close();
      }
    }

    /// <summary>
    /// The player_ mouse down.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void player_MouseDown(object sender, MouseButtonEventArgs e)
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

        VideoData.Instance.AddPoint(this.IndexOfTrackedObject - 1, new Point(originalX, originalY));

        if (this.IndexOfTrackedObject == Video.Instance.ImageProcessing.NumberOfTrackedObjects)
        {
          this.StepOneFrameForward();
        }

        double canvasPosX = e.GetPosition(this.windowCanvas).X;
        double canvasPosY = e.GetPosition(this.windowCanvas).Y;

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
    /// The player_ mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void player_MouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.windowCanvas);

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
    /// The timeline slider_ drag completed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void timelineSlider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds =
        (long)(this.timelineSlider.Value / VideoBase.NanoSecsToMilliSecs);
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
    private void timelineSlider_DragDelta(object sender, DragDeltaEventArgs e)
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
    private void timelineSlider_DragStarted(object sender, DragStartedEventArgs e)
    {
      this.isDragging = true;
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
    private void timelineSlider_TickDownClicked(object sender, EventArgs e)
    {
      this.StepOneFrameBackward();
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
    private void timelineSlider_TickUpClicked(object sender, EventArgs e)
    {
      this.StepOneFrameForward();
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
    private void timesliderUpdateTimer_Tick(object sender, EventArgs e)
    {
      if (!this.isDragging && Video.Instance.VideoMode == VideoMode.File)
      {
        double preciseTime = Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds;

        // double alignedTime = (int)(preciseTime / Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds) *
        // Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds;
        this.timelineSlider.Value = preciseTime * VideoBase.NanoSecsToMilliSecs;
        Video.Instance.VideoPlayerElement.UpdateFrameIndex();
      }
    }

    /// <summary>
    /// The window canvas_ mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void windowCanvas_MouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.windowCanvas);

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
  }
}