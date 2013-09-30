// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalibrateVideoWindow.cs" company="Freie Universität Berlin">
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
//   The calibrate video window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VianaNET.Application;

namespace VianaNET.Modules.Video.Dialogs
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Shapes;

  using VianaNET.Data;
  using VianaNET.Localization;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Base;

  /// <summary>
  ///   The calibrate video window.
  /// </summary>
  public class CalibrateVideoWindow : WindowWithHelp
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Fields

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

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="CalibrateVideoWindow" /> class.
    /// </summary>
    public CalibrateVideoWindow()
    {
      this.Title = Labels.CalibrateVideoWindowTitle;
      this.LabelTitle.Content = Labels.CalibrateWindowHelpControlTitle;
      this.DescriptionTitle.Content = Labels.CalibrateWindowSpecifyOriginHeader;
      this.DescriptionMessage.Text = Labels.CalibrateWindowSpecifyOriginDescription;
      this.originPath = (Path)this.Resources["OriginPath"];
      this.windowCanvas.Children.Add(this.originPath);
      this.ruler = (Line)this.Resources["Ruler"];
      this.windowCanvas.Children.Add(this.ruler);
      this.originIsSet = false;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    protected override void MouseOverControlPanel(bool isOver)
    {
      base.MouseOverControlPanel(isOver);
      this.ignoreMouse = isOver;
    }

    protected override void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      base.Container_MouseLeftButtonDown(sender, e);
      if (ignoreMouse)
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
        VianaNetApplication.Project.CalibrationData.OriginInPixel = new Point(originalX, originalY);
        this.originIsSet = true;
        this.originPath.Visibility = Visibility.Visible;
        Canvas.SetLeft(this.originPath, scaledX - this.originPath.ActualWidth / 2);
        Canvas.SetTop(this.originPath, scaledY - this.originPath.ActualHeight / 2);

        this.DescriptionTitle.Content = Labels.CalibrateWindowSpecifyLengthHeader;
        this.DescriptionMessage.Text = Labels.CalibrateWindowSpecifyLengthDescription;
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
        double distance = Math.Sqrt(Math.Pow(endPoint.Y - startPoint.Y, 2) + Math.Pow(endPoint.X - startPoint.X, 2));
        if (distance < 5)
        {
          var info = new VianaDialog(
            Labels.CalibrationLengthToShortTitle,
            Labels.CalibrationLengthToShortDescription,
            Labels.CalibrationLengthToShortMessage,
            true);
          info.ShowDialog();
          this.startPointIsSet = false;
          this.ruler.Visibility = Visibility.Hidden;
          return;
        }

        var lengthDialog = new LengthDialog();

        if (lengthDialog.ShowDialog().GetValueOrDefault(false))
        {
          // Save ruler points to Settings
          VianaNetApplication.Project.CalibrationData.RulerEndPointInPixel = this.endPoint;
          VianaNetApplication.Project.CalibrationData.RulerStartPointInPixel = this.startPoint;

          var lengthVector = new Vector();
          lengthVector = Vector.Add(
            lengthVector,
            new Vector(VianaNetApplication.Project.CalibrationData.RulerStartPointInPixel.X, VianaNetApplication.Project.CalibrationData.RulerStartPointInPixel.Y));
          lengthVector.Negate();
          lengthVector = Vector.Add(
            lengthVector,
            new Vector(VianaNetApplication.Project.CalibrationData.RulerEndPointInPixel.X, VianaNetApplication.Project.CalibrationData.RulerEndPointInPixel.Y));
          double length = lengthVector.Length;
          VianaNetApplication.Project.CalibrationData.ScalePixelToUnit = VianaNetApplication.Project.CalibrationData.RulerValueInRulerUnits / length;
          VianaNetApplication.Project.CalibrationData.IsVideoCalibrated = true;
          this.DialogResult = true;
        }

        this.Close();
      }
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
    protected override void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      base.Container_MouseLeftButtonUp(sender, e);
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
    protected override void Container_MouseMove(object sender, MouseEventArgs e)
    {
      base.Container_MouseMove(sender, e);
      if (ignoreMouse)
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


    #endregion
  }
}