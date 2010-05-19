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
  public partial class WindowWithHelp : Window
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

    public WindowWithHelp()
    {
      InitializeComponent();
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

    protected virtual void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

    }

    protected virtual void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {

    }

    protected virtual void Container_MouseMove(object sender, MouseEventArgs e)
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
