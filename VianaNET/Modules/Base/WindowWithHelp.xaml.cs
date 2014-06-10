// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowWithHelp.xaml.cs" company="Freie Universität Berlin">
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
//   The window with help.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VianaNET.Application;

namespace VianaNET.Modules.Base
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The window with help.
  /// </summary>
  public partial class WindowWithHelp : Window
  {
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IndexOfTrackedObject" />.
    /// </summary>
    public static readonly DependencyProperty IndexOfTrackedObjectProperty =
      DependencyProperty.Register(
        "IndexOfTrackedObject", typeof(int), typeof(WindowWithHelp), new FrameworkPropertyMetadata(1, OnPropertyChanged));

    #endregion

    #region Fields

    /// <summary>
    ///   The mouse down location.
    /// </summary>
    private Point mouseDownLocation;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="WindowWithHelp" /> class.
    /// </summary>
    public WindowWithHelp()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public Properties

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
    /// Handles the MouseLeftButtonDown event of the Container control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    protected virtual void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    /// <summary>
    /// Handles the MouseLeftButtonUp event of the Container control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    protected virtual void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    /// <summary>
    /// Handles the MouseMove event of the Container control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    protected virtual void Container_MouseMove(object sender, MouseEventArgs e)
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

    /// <summary>
    /// Mouse is the over control panel.
    /// </summary>
    /// <param name="isOver">if set to <c>true</c> is over.</param>
    protected virtual void MouseOverControlPanel(bool isOver)
    {
    }

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
      var window = obj as WindowWithHelp;

      // Reset index if appropriate
      if (window.IndexOfTrackedObject > Viana.Project.ProcessingData.NumberOfTrackedObjects)
      {
        window.IndexOfTrackedObject = 1;
      }
    }

    /// <summary>
    /// Handles the MouseEnter event of the ControlPanel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void ControlPanel_MouseEnter(object sender, MouseEventArgs e)
    {
      this.MouseOverControlPanel(true);
    }

    /// <summary>
    /// Handles the MouseLeave event of the ControlPanel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void ControlPanel_MouseLeave(object sender, MouseEventArgs e)
    {
      this.MouseOverControlPanel(false);
    }

    /// <summary>
    /// Drag window mouse down.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
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
      if (this.DescriptionMessage.Visibility == Visibility.Visible)
      {
        this.DescriptionMessage.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.DescriptionMessage.Visibility = Visibility.Visible;
      }
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the Window control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
     private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Escape)
      {
        e.Handled = true;
        this.Close();
      }
    }

     /// <summary>
     /// Handles the Click event of the btnDone control.
     /// </summary>
     /// <param name="sender">The source of the event.</param>
     /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void btnDone_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    #endregion
  }
}