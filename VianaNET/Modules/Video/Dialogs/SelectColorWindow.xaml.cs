// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectColorWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The select color window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.Video.Dialogs
{
  using System;
  using System.Drawing;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Video.Control;

  using Color = System.Drawing.Color;
  using Point = System.Windows.Point;

  /// <summary>
  ///   The select color window.
  /// </summary>
  public partial class SelectColorWindow : Window
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IndexOfTrackedObject" />.
    /// </summary>
    public static readonly DependencyProperty IndexOfTrackedObjectProperty =
      DependencyProperty.Register(
        "IndexOfTrackedObject",
        typeof(int),
        typeof(SelectColorWindow),
        new FrameworkPropertyMetadata(1, OnPropertyChanged));

    #endregion

    #region Fields

    /// <summary>
    ///   The mouse down location.
    /// </summary>
    private Point mouseDownLocation;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SelectColorWindow" /> class.
    /// </summary>
    public SelectColorWindow()
    {
      this.InitializeComponent();
      this.ObjectIndexPanel.DataContext = this;
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
    public int IndexOfTrackedObject
    {
      get => (int)this.GetValue(IndexOfTrackedObjectProperty);

      set => this.SetValue(IndexOfTrackedObjectProperty, value);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The container_ mouse left button down.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    protected void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    /// <summary>
    /// The container_ mouse left button up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
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

        Bitmap frame = Video.Instance.CreateBitmapFromCurrentImageSource();
        if (frame == null)
        {
          throw new ArgumentNullException("Native Bitmap is null.");
        }

        Color color = frame.GetPixel((int)originalX, (int)originalY);
        System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(255, color.R, color.G, color.B);
        App.Project.ProcessingData.TargetColor[this.IndexOfTrackedObject - 1] = selectedColor;
        //Project.TrackObjectColors[this.IndexOfTrackedObject - 1] = new SolidColorBrush(selectedColor);
      }
      catch (Exception)
      {
        VianaDialog error = new VianaDialog("Error", "No Color selected", "Could not detect the color at the given position", true);
        error.ShowDialog();
      }

      if (this.IndexOfTrackedObject == App.Project.ProcessingData.NumberOfTrackedObjects)
      {
        this.DialogResult = true;
        this.Close();
      }

      this.IndexOfTrackedObject++;
      this.ObjectIndexLabel.Content = this.IndexOfTrackedObject.ToString();
    }

    /// <summary>
    /// The container_ mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
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

          Canvas.SetTop(this.ControlPanel, currentLocation.Y + mouseMoveLocation.Y - this.mouseDownLocation.Y);
          Canvas.SetLeft(this.ControlPanel, currentLocation.X + mouseMoveLocation.X - this.mouseDownLocation.X);
          this.mouseDownLocation = mouseMoveLocation;
        }
      }
    }

    /// <summary>
    /// The mouse over control panel.
    /// </summary>
    /// <param name="isOver">
    /// The is over. 
    /// </param>
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
      SelectColorWindow window = obj as SelectColorWindow;

      // Reset index if appropriate
      if (window.IndexOfTrackedObject > App.Project.ProcessingData.NumberOfTrackedObjects)
      {
        window.IndexOfTrackedObject = 1;
      }
    }

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
      this.MouseOverControlPanel(true);
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
      this.MouseOverControlPanel(false);
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
    /// The btn done_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void btnDone_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}