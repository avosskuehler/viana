// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasureLengthWindow.xaml.cs" company="Freie Universität Berlin">
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
  using VianaNET.CustomStyles.Types;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   Interaction logic for ModifyDataWindow.xaml
  /// </summary>
  public partial class MeasureLengthWindow
  {
    /// <summary>
    ///   Mausbewegungen ignorieren, da über Control panel
    /// </summary>
    private bool ignoreMouse;

    /// <summary>
    ///   The mouse down location.
    /// </summary>
    private Point mouseDownLocation;

    /// <summary>
    ///   The start point.
    /// </summary>
    private Point startPoint;

    /// <summary>
    ///   The end point.
    /// </summary>
    private Point endPoint;

    /// <summary>
    ///   The start point is set.
    /// </summary>
    private bool startPointIsSet;


    /// <summary>
    ///   Initializes a new instance of the <see cref="MeasureLengthWindow" /> class.
    /// </summary>
    public MeasureLengthWindow()
    {
      this.InitializeComponent();
      this.WindowCanvas.DataContext = this;
      this.TimelineSlider.Value = Video.Instance.VideoElement.MediaPositionInMS;
    }





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

      double scaledX = e.GetPosition(this.VideoImage).X;
      double scaledY = e.GetPosition(this.VideoImage).Y;
      double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
      double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
      double originalX = factorX * scaledX;
      double originalY = factorY * scaledY;

      if (!this.startPointIsSet)
      {
        this.startPoint = new Point(originalX, originalY);
        this.ruler.X1 = scaledX;
        this.ruler.Y1 = scaledY;
        this.ruler.X2 = scaledX;
        this.ruler.Y2 = scaledY;
        this.ruler.Visibility = Visibility.Visible;
        this.LengthLabelBorder.Visibility = Visibility.Visible;
        this.startPointIsSet = true;
      }
      else
      {
        this.endPoint = new Point(originalX, originalY);

        this.UpdateLengthLabel();

        this.startPointIsSet = false;
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

      if (this.startPointIsSet)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        this.ruler.X2 = scaledX;
        this.ruler.Y2 = scaledY;
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
        double originalX = factorX * scaledX;
        double originalY = factorY * scaledY;
        this.endPoint = new Point(originalX, originalY);

        this.UpdateLengthLabel();
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
        Video.Instance.StepFrames(false, App.Project.VideoData.UseEveryNthPoint);
      }

      var posFrames = Video.Instance.VideoElement.OpenCVObject.Get(OpenCvSharp.VideoCaptureProperties.PosFrames);
      this.TimelineSlider.Value = (posFrames - 1) * Video.Instance.VideoElement.FrameTimeInMS;
    }

    /// <summary>
    ///   The step frames forward.
    /// </summary>
    private void StepFramesForward()
    {
      if (this.TimelineSlider.Value <= this.TimelineSlider.SelectionEnd - this.TimelineSlider.TickFrequency)
      {
        Video.Instance.StepFrames(true, App.Project.VideoData.UseEveryNthPoint);
      }

      var posFrames = Video.Instance.VideoElement.OpenCVObject.Get(OpenCvSharp.VideoCaptureProperties.PosFrames);
      this.TimelineSlider.Value = (posFrames - 1) * Video.Instance.VideoElement.FrameTimeInMS;
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
      Video.Instance.VideoPlayerElement.MediaPositionInMS = this.TimelineSlider.Value;
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
    ///   Updates the length label by calculating the length of the current line.
    /// </summary>
    private void UpdateLengthLabel()
    {
      // Abstand berechnen
      double distance =
        Math.Sqrt(Math.Pow(this.endPoint.Y - this.startPoint.Y, 2) + Math.Pow(this.endPoint.X - this.startPoint.X, 2));
      double result = distance * App.Project.CalibrationData.ScalePixelToUnit;

      ((Label)this.LengthLabelBorder.Child).Content = result.ToString("N2") + " "
                                                      + App.Project.CalibrationData.LengthUnit;
      double centerLineX = (this.ruler.X1 + this.ruler.X2) / 2;
      double centerLineY = (this.ruler.Y1 + this.ruler.Y2) / 2;

      Canvas.SetLeft(this.LengthLabelBorder, centerLineX - this.LengthLabelBorder.ActualWidth / 2);
      Canvas.SetTop(this.LengthLabelBorder, centerLineY - this.LengthLabelBorder.ActualHeight / 2);
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
          Point currentLocation = new Point
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


  }
}