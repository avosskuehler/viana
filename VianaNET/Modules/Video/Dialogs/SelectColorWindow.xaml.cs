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

namespace VianaNET
{
  public partial class SelectColorWindow : Window
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

    private Point mouseDownLocation;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public SelectColorWindow()
    {
      InitializeComponent();
      this.ObjectIndexPanel.DataContext = this;
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
      typeof(SelectColorWindow),
      new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnPropertyChanged)));

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

    protected void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ControlPanel.IsMouseOver)
      {
        return;
      }

      try
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
        double originalX = factorX * scaledX;
        double originalY = factorY * scaledY;
        Int32Rect rect = new Int32Rect((int)originalX, (int)originalY, 1, 1);

        System.Drawing.Bitmap frame = Video.Instance.CreateBitmapFromCurrentImageSource();
        if (frame == null)
        {
          throw new ArgumentNullException("Native Bitmap is null.");
        }

        System.Drawing.Color color = frame.GetPixel((int)originalX, (int)originalY);
        Color selectedColor = Color.FromArgb(255, color.R, color.G, color.B);
        Video.Instance.ImageProcessing.TargetColor[this.IndexOfTrackedObject - 1] = selectedColor;
        ImageProcessing.TrackObjectColors[this.IndexOfTrackedObject - 1] = new SolidColorBrush(selectedColor);
      }
      catch (Exception)
      {
        VianaDialog error = new VianaDialog(
          "Error",
          "No Color selected",
          "Could not detect the color at the given position");
        error.ShowDialog();
      }

      if (this.IndexOfTrackedObject == Video.Instance.ImageProcessing.NumberOfTrackedObjects)
      {
        this.DialogResult = true;
        this.Close();
      }

      this.IndexOfTrackedObject++;
      this.ObjectIndexLabel.Content = this.IndexOfTrackedObject.ToString();
    }

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

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
      SelectColorWindow window = obj as SelectColorWindow;

      // Reset index if appropriate
      if (window.IndexOfTrackedObject > Video.Instance.ImageProcessing.NumberOfTrackedObjects)
      {
        window.IndexOfTrackedObject = 1;
      }
    }

    protected void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

    }

    protected void Container_MouseMove(object sender, MouseEventArgs e)
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

    private void ControlPanel_MouseEnter(object sender, MouseEventArgs e)
    {
      this.MouseOverControlPanel(true);
    }

    private void HideWindow(object sender, RoutedEventArgs e)
    {
      this.ControlPanel.Visibility = Visibility.Collapsed;
    }

    private void MinimizeWindow(object sender, MouseButtonEventArgs e)
    {
      if (this.DescriptionMessage.Visibility == Visibility.Visible)
      {
        this.DescriptionMessage.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.DescriptionMessage.Visibility = Visibility.Visible;
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
      this.MouseOverControlPanel(false);
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Escape)
      {
        e.Handled = true;
        this.Close();
      }
    }

    private void btnDone_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
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
    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    protected virtual void MouseOverControlPanel(bool isOver)
    {
    }

    #endregion //HELPER

  }
}
