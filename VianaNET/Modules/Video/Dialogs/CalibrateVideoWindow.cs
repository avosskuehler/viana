// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalibrateVideoWindow.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   The calibrate video window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace VianaNET.Modules.Video.Dialogs
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Shapes;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Base;

  /// <summary>
  ///   The calibrate video window.
  /// </summary>
  public class CalibrateVideoWindow : WindowWithHelp
  {


    /// <summary>
    ///   The origin path.
    /// </summary>
    private readonly Path originPath;

    /// <summary>
    ///   The ruler.
    /// </summary>
    private readonly Line ruler;

    /// <summary>
    ///   The end point.
    /// </summary>
    private Point endPoint;

    /// <summary>
    ///   The origin is set.
    /// </summary>
    private bool originIsSet;

    /// <summary>
    ///   The start point.
    /// </summary>
    private Point startPoint;

    /// <summary>
    ///   The start point is set.
    /// </summary>
    private bool startPointIsSet;

    /// <summary>
    /// Mausbewegungen ignorieren, da über Control panel
    /// </summary>
    private bool ignoreMouse;





    /// <summary>
    ///   Initializes a new instance of the <see cref="CalibrateVideoWindow" /> class.
    /// </summary>
    public CalibrateVideoWindow()
    {
      this.Title = VianaNET.Localization.Labels.CalibrateVideoWindowTitle;
      this.LabelTitle.Content = VianaNET.Localization.Labels.CalibrateWindowHelpControlTitle;
      this.DescriptionTitle.Content = VianaNET.Localization.Labels.CalibrateWindowSpecifyOriginHeader;
      this.DescriptionMessage.Text = VianaNET.Localization.Labels.CalibrateWindowSpecifyOriginDescription;
      this.originPath = (Path)this.Resources["OriginPath"];
      this.windowCanvas.Children.Add(this.originPath);
      this.ruler = (Line)this.Resources["Ruler"];
      this.windowCanvas.Children.Add(this.ruler);
      this.originIsSet = false;
    }





    /// <summary>
    /// Mouse is the over control panel.
    /// </summary>
    /// <param name="isOver">if set to <c>true</c> is over.</param>
    protected override void MouseOverControlPanel(bool isOver)
    {
      base.MouseOverControlPanel(isOver);
      this.ignoreMouse = isOver;
    }

    /// <summary>
    /// Handles the MouseLeftButtonDown event of the Container control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
    protected override void ContainerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      base.ContainerMouseLeftButtonDown(sender, e);
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

      if (!this.originIsSet)
      {
        App.Project.CalibrationData.OriginInPixel = new Point(originalX, originalY);
        this.originIsSet = true;
        this.originPath.Visibility = Visibility.Visible;
        Canvas.SetLeft(this.originPath, scaledX - this.originPath.ActualWidth / 2);
        Canvas.SetTop(this.originPath, scaledY - this.originPath.ActualHeight / 2);

        this.DescriptionTitle.Content = VianaNET.Localization.Labels.CalibrateWindowSpecifyLengthHeader;
        this.DescriptionMessage.Text = VianaNET.Localization.Labels.CalibrateWindowSpecifyLengthDescription;
      }
      else if (!this.startPointIsSet)
      {
        this.startPoint = new Point(originalX, originalY);
        this.ruler.X1 = scaledX;
        this.ruler.Y1 = scaledY;
        this.ruler.X2 = scaledX;
        this.ruler.Y2 = scaledY;
        this.ruler.Visibility = Visibility.Visible;
        this.startPointIsSet = true;
      }
      else if (this.startPointIsSet)
      {
        this.endPoint = new Point(originalX, originalY);

        // Sicher gehen, dass Messpunkt nicht zu dicht liegen
        double distance = Math.Sqrt(Math.Pow(this.endPoint.Y - this.startPoint.Y, 2) + Math.Pow(this.endPoint.X - this.startPoint.X, 2));
        if (distance < 5)
        {
          VianaDialog info = new VianaDialog(
            VianaNET.Localization.Labels.CalibrationLengthToShortTitle,
            VianaNET.Localization.Labels.CalibrationLengthToShortDescription,
            VianaNET.Localization.Labels.CalibrationLengthToShortMessage,
            true);
          info.ShowDialog();
          this.startPointIsSet = false;
          this.ruler.Visibility = Visibility.Hidden;
          return;
        }

        LengthDialog lengthDialog = new LengthDialog();

        if (lengthDialog.ShowDialog().GetValueOrDefault(false))
        {
          // Save ruler points to Settings
          App.Project.CalibrationData.RulerEndPointInPixel = this.endPoint;
          App.Project.CalibrationData.RulerStartPointInPixel = this.startPoint;

          Vector lengthVector = new Vector();
          lengthVector = Vector.Add(
            lengthVector,
            new Vector(App.Project.CalibrationData.RulerStartPointInPixel.X, App.Project.CalibrationData.RulerStartPointInPixel.Y));
          lengthVector.Negate();
          lengthVector = Vector.Add(
            lengthVector,
            new Vector(App.Project.CalibrationData.RulerEndPointInPixel.X, App.Project.CalibrationData.RulerEndPointInPixel.Y));
          double length = lengthVector.Length;
          App.Project.CalibrationData.ScalePixelToUnit = App.Project.CalibrationData.RulerValueInRulerUnits / length;
          App.Project.CalibrationData.IsVideoCalibrated = true;
          this.DialogResult = true;
        }

        this.Close();
      }
    }

    /// <summary>
    /// Handles the MouseMove event of the Container control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    protected override void ContainerMouseMove(object sender, MouseEventArgs e)
    {
      base.ContainerMouseMove(sender, e);
      if (this.ignoreMouse)
      {
        return;
      }

      if (this.originIsSet && this.startPointIsSet)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        this.ruler.X2 = scaledX;
        this.ruler.Y2 = scaledY;
      }
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown" /> attached event reaches an element 
    /// in its route that is derived from this class. Implement this method to add class handling for this event.
    /// Resets the calibration on F10.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      if (e.SystemKey == Key.F10)
      {
        // Reset calibration
        App.Project.CalibrationData.OriginInPixel = new Point();
        App.Project.CalibrationData.RulerEndPointInPixel = new Point();
        App.Project.CalibrationData.RulerStartPointInPixel = new Point();
        App.Project.CalibrationData.ScalePixelToUnit = 1;
        App.Project.CalibrationData.IsVideoCalibrated = false;

        this.DialogResult = true;

        this.Close();
      }
    }


  }
}