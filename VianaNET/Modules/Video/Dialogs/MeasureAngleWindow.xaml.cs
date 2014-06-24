// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasureAngleWindow.xaml.cs" company="Freie Universität Berlin">
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
namespace VianaNET.Modules.Video.Dialogs
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Threading;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   Interaction logic for MeasureAngleWindow.xaml
  /// </summary>
  public partial class MeasureAngleWindow
  {
    #region Fields

    /// <summary>
    ///   The timeslider update timer.
    /// </summary>
    private readonly DispatcherTimer timesliderUpdateTimer;

    /// <summary>
    ///   Mausbewegungen ignorieren, da über Control panel
    /// </summary>
    private bool ignoreMouse;

    /// <summary>
    ///   Indicates the dragging of the time line thumb
    /// </summary>
    private bool isDraggingTimeLineThumb;

    /// <summary>
    ///   The mouse down location.
    /// </summary>
    private Point mouseDownLocation;

    /// <summary>
    ///   The start point is set.
    /// </summary>
    private bool startPointIsSet;

    /// <summary>
    ///   The start point is set.
    /// </summary>
    private bool centerPointIsSet;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MeasureAngleWindow" /> class.
    /// </summary>
    public MeasureAngleWindow()
    {
      this.InitializeComponent();
      this.WindowCanvas.DataContext = this;
      this.timesliderUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
      this.timesliderUpdateTimer.Tick += this.TimesliderUpdateTimerTick;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the MouseLeftButtonDown event of the Container control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    protected void ContainerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.ignoreMouse)
      {
        return;
      }

      var location = e.GetPosition(this.VideoImage);

      if (!this.startPointIsSet)
      {
        this.Line2.Visibility = Visibility.Hidden;
        this.AngleLabelBorder.Visibility = Visibility.Hidden;
        this.Arc.Visibility = Visibility.Hidden;
        this.Line1.X1 = location.X;
        this.Line1.Y1 = location.Y;
        this.Line1.X2 = location.X;
        this.Line1.Y2 = location.Y;
        this.Line1.Visibility = Visibility.Visible;
        this.startPointIsSet = true;
      }
      else if (this.startPointIsSet && !this.centerPointIsSet)
      {
        this.Line1.X2 = location.X;
        this.Line1.Y2 = location.Y;
        this.Line2.X1 = location.X;
        this.Line2.Y1 = location.Y;
        this.Arc.Center = location;
        this.centerPointIsSet = true;
      }
      else
      {
        this.Line2.X2 = location.X;
        this.Line2.Y2 = location.Y;
        this.UpdateAngleLabel();
        this.startPointIsSet = false;
        this.centerPointIsSet = false;
      }
    }

    /// <summary>
    /// Handles the MouseMove event of the Container control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    protected void ContainerMouseMove(object sender, MouseEventArgs e)
    {
      if (this.ignoreMouse)
      {
        return;
      }

      var scaledX = e.GetPosition(this.VideoImage).X;
      var scaledY = e.GetPosition(this.VideoImage).Y;

      if (this.startPointIsSet && !this.centerPointIsSet)
      {
        this.Line1.X2 = scaledX;
        this.Line1.Y2 = scaledY;
      }
      else if (this.startPointIsSet && this.centerPointIsSet)
      {
        this.Line2.Visibility = Visibility.Visible;
        this.AngleLabelBorder.Visibility = Visibility.Visible;
        this.Arc.Visibility = Visibility.Visible;
        this.Line2.X2 = scaledX;
        this.Line2.Y2 = scaledY;
        this.UpdateAngleLabel();
      }
    }

    /// <summary>
    /// The mouse is over control panel method.
    /// </summary>
    /// <param name="isOver">
    /// The is over.
    /// </param>
    protected void MouseOverControlPanel(bool isOver)
    {
      this.ignoreMouse = isOver;
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
      this.HelpText.Visibility = this.HelpText.Visibility == Visibility.Visible
                                   ? Visibility.Collapsed
                                   : Visibility.Visible;
    }

    /// <summary>
    ///   The step frames backward.
    /// </summary>
    private void StepFramesBackward()
    {
      if (this.TimelineSlider.Value >= this.TimelineSlider.SelectionStart + this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepFrames(false, Viana.Project.VideoData.UseEveryNthPoint);
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
    ///   Updates the angle label by calculating the angle between the two lines.
    /// </summary>
    private void UpdateAngleLabel()
    {
      // Winkel berechnen
      var vecHor = new Vector(1, 0);
      var vec1 = new Vector(this.Line1.X1 - this.Line1.X2, this.Line1.Y1 - this.Line1.Y2);
      var vec2 = new Vector(this.Line2.X2 - this.Line2.X1, this.Line2.Y2 - this.Line2.Y1);
      var startAngle = Vector.AngleBetween(vecHor, vec1);
      var endAngle = Vector.AngleBetween(vecHor, vec2);
      this.Arc.EndAngle = endAngle;
      this.Arc.StartAngle = startAngle;

      double angle = Vector.AngleBetween(vec1, vec2);
      if (angle < 0)
      {
        angle = 360 + angle;
      }

      ((Label)this.AngleLabelBorder.Child).Content = angle.ToString("N2") + " °";
      Canvas.SetLeft(this.AngleLabelBorder, this.Line1.X2 - this.AngleLabelBorder.ActualWidth / 2);
      Canvas.SetTop(this.AngleLabelBorder, this.Line1.Y2 - this.AngleLabelBorder.ActualHeight / 2);
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