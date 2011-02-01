using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace VianaNET
{
  /// <summary>
  /// Interaction logic for ReconstructionWindow.xaml
  /// </summary>
  public partial class ManualDataAquisitionWindow : Window
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

    private int visualDataPointRingBufferSize = 5;
    private int visualDataPointRadius = 5;
    private List<Ellipse>[] visualDataPoints;
    private int indexOfVisualDataPointRingBuffer;
    private Point mouseDownLocation;
    private DispatcherTimer timesliderUpdateTimer;
    bool isDragging = false;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public ManualDataAquisitionWindow()
    {
      InitializeComponent();

      this.ObjectIndexPanel.DataContext = this;
      this.windowCanvas.DataContext = this;

      this.indexOfVisualDataPointRingBuffer = 0;
      CreateVisualDataPoints(7);

      this.CursorEllipse.Width = 2 * visualDataPointRadius;
      this.CursorEllipse.Height = 2 * visualDataPointRadius;

      this.timesliderUpdateTimer = new DispatcherTimer();
      this.timesliderUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
      this.timesliderUpdateTimer.Tick += new EventHandler(timesliderUpdateTimer_Tick);

      //this.videoSurfaceClone = Video.Instance.VideoElement.CloneD3DRenderer();
      //this.videoSurfaceClone.MouseDown += new MouseButtonEventHandler(player_MouseDown);
      //this.videoSurfaceClone.MouseMove += new MouseEventHandler(player_MouseMove);
      //this.playerContainerGrid.Children.Insert(0, this.videoSurfaceClone);
      //this.videoSurfaceClone.InvalidateVideoImage();
      //VideoPlayer.Instance.AttachToControl(this.playerContainerGrid);
      //VideoPlayer.Instance.MouseDown += new MouseButtonEventHandler(player_MouseDown);
      //VideoPlayer.Instance.MouseMove += new MouseEventHandler(player_MouseMove);

      //this.timelineSlider.SelectionStart = 0;
      //this.timelineSlider.SelectionEnd = this.timelineSlider.Maximum;

      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        this.FramePanel.Visibility = Visibility.Hidden;
        this.timelineSlider.Visibility = Visibility.Hidden;
      }
    }

    ~ManualDataAquisitionWindow()
    {
      //// Unregister event listeners.
      //VideoPlayer.Instance.MouseDown -= new MouseButtonEventHandler(player_MouseDown);
      //VideoPlayer.Instance.MouseMove -= new MouseEventHandler(player_MouseMove);
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

    /// <summary>
    /// Gets or sets the index of the currently tracked object
    /// </summary>
    public int IndexOfTrackedObject
    {
      get { return (int)this.GetValue(IndexOfTrackedObjectProperty); }
      set { this.SetValue(IndexOfTrackedObjectProperty, value); }
    }

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the property <see cref="IndexOfTrackedObject"/>.
    /// </summary>
    public static readonly DependencyProperty IndexOfTrackedObjectProperty = DependencyProperty.Register(
      "IndexOfTrackedObject",
      typeof(int),
      typeof(ManualDataAquisitionWindow),
      new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnPropertyChanged)));

    /// <summary>
    /// Gets or sets the index of the currently tracked object
    /// </summary>
    public SolidColorBrush BrushOfCossHair
    {
      get { return (SolidColorBrush)this.GetValue(BrushOfCossHairProperty); }
      set { this.SetValue(BrushOfCossHairProperty, value); }
    }

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the property <see cref="BrushOfCossHair"/>.
    /// </summary>
    public static readonly DependencyProperty BrushOfCossHairProperty = DependencyProperty.Register(
      "BrushOfCossHair",
      typeof(SolidColorBrush),
      typeof(ManualDataAquisitionWindow),
      new FrameworkPropertyMetadata(Calibration.TrackObjectColors[0], new PropertyChangedCallback(OnPropertyChanged)));

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS
    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">The source of the event. This.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> with 
    /// the event data.</param>
    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      ManualDataAquisitionWindow window = obj as ManualDataAquisitionWindow;

      // Reset index if appropriate
      if (window.IndexOfTrackedObject > Calibration.Instance.NumberOfTrackedObjects)
      {
        window.IndexOfTrackedObject = 1;
      }

      // Update crosshair brush
      window.BrushOfCossHair = Calibration.TrackObjectColors[window.IndexOfTrackedObject-1];
    }

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.HorizontalCursorLineLeft.X2 = this.windowCanvas.ActualWidth;
      this.HorizontalCursorLineRight.X2 = this.windowCanvas.ActualWidth;
      this.VerticalCursorLineTop.Y2 = this.windowCanvas.ActualHeight;
      this.VerticalCursorLineBottom.Y2 = this.windowCanvas.ActualHeight;

      this.timesliderUpdateTimer.Start();
    }

    private void player_MouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.windowCanvas);

      this.HorizontalCursorLineLeft.Y1 = mouseMoveLocation.Y;
      this.HorizontalCursorLineLeft.Y2 = mouseMoveLocation.Y;
      this.HorizontalCursorLineLeft.X2 = mouseMoveLocation.X - visualDataPointRadius;
      this.HorizontalCursorLineRight.Y1 = mouseMoveLocation.Y;
      this.HorizontalCursorLineRight.Y2 = mouseMoveLocation.Y;
      this.HorizontalCursorLineRight.X1 = mouseMoveLocation.X + visualDataPointRadius;
      this.VerticalCursorLineTop.X1 = mouseMoveLocation.X;
      this.VerticalCursorLineTop.X2 = mouseMoveLocation.X;
      this.VerticalCursorLineTop.Y2 = mouseMoveLocation.Y - visualDataPointRadius;
      this.VerticalCursorLineBottom.X1 = mouseMoveLocation.X;
      this.VerticalCursorLineBottom.X2 = mouseMoveLocation.X;
      this.VerticalCursorLineBottom.Y1 = mouseMoveLocation.Y + visualDataPointRadius;
      Canvas.SetTop(this.CursorEllipse, mouseMoveLocation.Y - visualDataPointRadius);
      Canvas.SetLeft(this.CursorEllipse, mouseMoveLocation.X - visualDataPointRadius);
    }

    private void player_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        double factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.VideoImage.ActualWidth;
        double factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.VideoImage.ActualHeight;
        double spaceX = 0;//(this.videoSurfaceClone.ActualWidth - this.videoSurfaceClone.UniformWidth) / 2;
        double spaceY = 0;// (this.videoSurfaceClone.ActualHeight - this.videoSurfaceClone.UniformHeight) / 2;
        double originalX = factorX * (scaledX - spaceX);
        double originalY = factorY * (scaledY - spaceY);

        VideoData.Instance.AddPoint(this.IndexOfTrackedObject-1, new Point(originalX, originalY));

        if (this.IndexOfTrackedObject == Calibration.Instance.NumberOfTrackedObjects)
        {
          this.StepOneFrameForward();
        }

        double canvasPosX = e.GetPosition(this.windowCanvas).X;
        double canvasPosY = e.GetPosition(this.windowCanvas).Y;

        if (this.visualDataPointRingBufferSize > 0)
        {
          Canvas.SetTop(
            this.visualDataPoints[this.IndexOfTrackedObject-1][this.indexOfVisualDataPointRingBuffer],
            canvasPosY - visualDataPointRadius);

          Canvas.SetLeft(
            this.visualDataPoints[this.IndexOfTrackedObject-1][this.indexOfVisualDataPointRingBuffer],
            canvasPosX - visualDataPointRadius);

          this.indexOfVisualDataPointRingBuffer++;
          if (this.indexOfVisualDataPointRingBuffer >= visualDataPointRingBufferSize)
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

    private void windowCanvas_MouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = e.GetPosition(this.windowCanvas);

      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.GridTop.IsMouseOver)
        {
          Point currentLocation = new Point();
          currentLocation.X = Canvas.GetLeft(this.ControlPanel);
          currentLocation.Y = Canvas.GetTop(this.ControlPanel);

          Canvas.SetTop(this.ControlPanel, currentLocation.Y + mouseMoveLocation.Y - mouseDownLocation.Y);
          Canvas.SetLeft(this.ControlPanel, currentLocation.X + mouseMoveLocation.X - mouseDownLocation.X);
          mouseDownLocation = mouseMoveLocation;
        }
      }
    }

    private void timelineSlider_TickDownClicked(object sender, EventArgs e)
    {
      StepOneFrameBackward();
    }

    private void StepOneFrameBackward()
    {
      if (this.timelineSlider.Value >= this.timelineSlider.SelectionStart + this.timelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(false);
      }
    }

    private void timelineSlider_TickUpClicked(object sender, EventArgs e)
    {
      StepOneFrameForward();
    }

    private void StepOneFrameForward()
    {
      if (this.timelineSlider.Value <= this.timelineSlider.SelectionEnd - this.timelineSlider.TickFrequency)
      {
        Video.Instance.StepOneFrame(true);
      }
    }

    void timesliderUpdateTimer_Tick(object sender, EventArgs e)
    {
      if (!isDragging && Video.Instance.VideoMode == VideoMode.File)
      {
        double preciseTime = Video.Instance.VideoPlayerElement.MediaPositionInNanoSeconds;
        //double alignedTime = (int)(preciseTime / Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds) *
        // Video.Instance.VideoPlayerElement.FrameTimeIn100NanoSeconds;
        this.timelineSlider.Value = preciseTime * VideoBase.NanoSecsToMilliSecs;
        Video.Instance.VideoPlayerElement.UpdateFrameIndex();
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

    private void ControlPanel_MouseEnter(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(false);
    }

    private void HideWindow(object sender, RoutedEventArgs e)
    {
      this.ControlPanel.Visibility = Visibility.Collapsed;
    }

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

    private void DragWindowMouseDown(object sender, MouseButtonEventArgs args)
    {
      mouseDownLocation = args.GetPosition(this.windowCanvas);
      this.GridTop.CaptureMouse();
    }

    private void DragWindowMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.GridTop.ReleaseMouseCapture();
    }

    private void ControlPanel_MouseLeave(object sender, MouseEventArgs e)
    {
      this.ShowHideCursorSharp(true);
    }

    private void RecentPointCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      this.CreateVisualDataPoints((int)e.NewValue);
    }

    private void ButtonReady_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Escape)
      {
        e.Handled = true;
        this.Close();
      }
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

    private void CreateVisualDataPoints(int count)
    {
      this.visualDataPointRingBufferSize = count;

      // Remove old visual data points for all tracked objects
      if (this.visualDataPoints != null)
      {
        for (int j = 0; j < Calibration.Instance.NumberOfTrackedObjects; j++)
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
        this.visualDataPoints = new List<Ellipse>[Calibration.Instance.NumberOfTrackedObjects];
        for (int j = 0; j < Calibration.Instance.NumberOfTrackedObjects; j++)
        {
          this.visualDataPoints[j] = new List<Ellipse>(count);
          for (int i = 0; i < count; i++)
          {
            Ellipse visualDataPoint = new Ellipse();
            visualDataPoint.Width = 2 * visualDataPointRadius;
            visualDataPoint.Height = 2 * visualDataPointRadius;
            visualDataPoint.Stroke = Calibration.TrackObjectColors[j];
            visualDataPoint.IsEnabled = false;
            visualDataPoint.IsHitTestVisible = false;

            this.windowCanvas.Children.Add(visualDataPoint);
            this.visualDataPoints[j].Add(visualDataPoint);
          }
        }
      }
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    private void ShowHideCursorSharp(bool show)
    {
      Visibility vis = show ? Visibility.Visible : Visibility.Collapsed;
      this.VerticalCursorLineBottom.Visibility = vis;
      this.VerticalCursorLineTop.Visibility = vis;
      this.HorizontalCursorLineLeft.Visibility = vis;
      this.HorizontalCursorLineRight.Visibility = vis;
      this.CursorEllipse.Visibility = vis;
    }

    #endregion //HELPER
  }
}
