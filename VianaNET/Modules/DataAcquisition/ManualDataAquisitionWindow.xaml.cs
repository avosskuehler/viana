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
using WPFMediaKit.DirectShow.Controls;

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
    private List<Ellipse> visualDataPoints;
    private int indexOfVisualDataPointRingBuffer;
    private D3DRenderer videoSurfaceClone;
    private Point mouseDownLocation;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public ManualDataAquisitionWindow()
    {
      InitializeComponent();
      this.indexOfVisualDataPointRingBuffer = 0;
      CreateVisualDataPoints(7);

      this.CursorEllipse.Width = 2 * visualDataPointRadius;
      this.CursorEllipse.Height = 2 * visualDataPointRadius;

      this.videoSurfaceClone = Video.Instance.VideoElement.CloneD3DRenderer();
      this.videoSurfaceClone.MouseDown += new MouseButtonEventHandler(player_MouseDown);
      this.videoSurfaceClone.MouseMove += new MouseEventHandler(player_MouseMove);
      this.playerContainerGrid.Children.Insert(0, this.videoSurfaceClone);
      this.videoSurfaceClone.InvalidateVideoImage();
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
        double scaledX = e.GetPosition(this.videoSurfaceClone).X;
        double scaledY = e.GetPosition(this.videoSurfaceClone).Y;
        double factorX = Video.Instance.VideoElement.NaturalVideoWidth / this.videoSurfaceClone.UniformWidth;
        double factorY = Video.Instance.VideoElement.NaturalVideoHeight / this.videoSurfaceClone.UniformHeight;
        double spaceX = (this.videoSurfaceClone.ActualWidth - this.videoSurfaceClone.UniformWidth) / 2;
        double spaceY = (this.videoSurfaceClone.ActualHeight - this.videoSurfaceClone.UniformHeight) / 2;
        double originalX = factorX * (scaledX - spaceX);
        double originalY = Video.Instance.VideoElement.NaturalVideoHeight - factorY * (scaledY - spaceY);

        VideoData.Instance.AddPoint(new Point(originalX, originalY));

        Video.Instance.StepOneFrame(true);

        double canvasPosX = e.GetPosition(this.windowCanvas).X;
        double canvasPosY = e.GetPosition(this.windowCanvas).Y;

        if (this.visualDataPointRingBufferSize > 0)
        {
          Canvas.SetTop(
            this.visualDataPoints[this.indexOfVisualDataPointRingBuffer],
            canvasPosY - visualDataPointRadius);

          Canvas.SetLeft(
            this.visualDataPoints[this.indexOfVisualDataPointRingBuffer],
            canvasPosX - visualDataPointRadius);

          this.indexOfVisualDataPointRingBuffer++;
          if (this.indexOfVisualDataPointRingBuffer >= visualDataPointRingBufferSize)
          {
            this.indexOfVisualDataPointRingBuffer = 0;
          }
        }
      }
      else if (e.ChangedButton == MouseButton.Right)
      {
        Video.Instance.StepOneFrame(false);
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
      Video.Instance.StepOneFrame(false);
    }

    private void timelineSlider_TickUpClicked(object sender, EventArgs e)
    {
      Video.Instance.StepOneFrame(true);
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

      if (this.visualDataPoints != null)
      {
        foreach (Ellipse item in this.visualDataPoints)
        {
          if (this.windowCanvas.Children.Contains(item))
          {
            this.windowCanvas.Children.Remove(item);
          }
        }

        this.visualDataPoints.Clear();
      }

      this.indexOfVisualDataPointRingBuffer = 0;

      if (count > 0)
      {
        this.visualDataPoints = new List<Ellipse>(count);
        for (int i = 0; i < count; i++)
        {
          Ellipse visualDataPoint = new Ellipse();
          visualDataPoint.Width = 2 * visualDataPointRadius;
          visualDataPoint.Height = 2 * visualDataPointRadius;
          visualDataPoint.Stroke = Brushes.Red;
          this.windowCanvas.Children.Add(visualDataPoint);
          this.visualDataPoints.Add(visualDataPoint);
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
