// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasureLengthWindow.cs" company="Freie Universität Berlin">
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
  using System.Windows.Input;
  using System.Windows.Shapes;

  using VianaNET.Application;
  using VianaNET.Modules.Base;
  using VianaNET.Resources;

  /// <summary>
  ///   The calibrate video window.
  /// </summary>
  public class MeasureLengthWindow : WindowWithHelp
  {
    #region Fields

    /// <summary>
    ///   The length label border
    /// </summary>
    private readonly Border lengthLabelBorder;

    /// <summary>
    ///   The ruler.
    /// </summary>
    private readonly Line ruler;

    /// <summary>
    ///   The end point.
    /// </summary>
    private Point endPoint;

    /// <summary>
    ///   Mausbewegungen ignorieren, da über Control panel
    /// </summary>
    private bool ignoreMouse;

    /// <summary>
    ///   The start point.
    /// </summary>
    private Point startPoint;

    /// <summary>
    ///   The start point is set.
    /// </summary>
    private bool startPointIsSet;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MeasureLengthWindow" /> class.
    /// </summary>
    public MeasureLengthWindow()
    {
      this.Title = Labels.MeasureLengthWindowTitle;
      this.LabelTitle.Content = Labels.MeasureLengthHelpControlTitle;
      this.DescriptionTitle.Content = Labels.MeasureLengthHowToMeasureHeader;
      this.DescriptionMessage.Text = Labels.MeasureLengthHowToMeasureDescription;
      this.ruler = (Line)this.Resources["Ruler"];
      this.windowCanvas.Children.Add(this.ruler);
      this.lengthLabelBorder = (Border)this.Resources["LengthLabelBorder"];
      this.windowCanvas.Children.Add(this.lengthLabelBorder);
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

      if (!this.startPointIsSet)
      {
        this.startPoint = new Point(originalX, originalY);
        this.ruler.X1 = scaledX;
        this.ruler.Y1 = scaledY;
        this.ruler.X2 = scaledX;
        this.ruler.Y2 = scaledY;
        this.ruler.Visibility = Visibility.Visible;
        this.lengthLabelBorder.Visibility = Visibility.Visible;
        this.startPointIsSet = true;
      }
      else if (this.startPointIsSet)
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
    protected override void ContainerMouseMove(object sender, MouseEventArgs e)
    {
      base.ContainerMouseMove(sender, e);
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
    protected override void MouseOverControlPanel(bool isOver)
    {
      base.MouseOverControlPanel(isOver);
      this.ignoreMouse = isOver;
    }

    /// <summary>
    ///   Updates the length label by calculating the length of the current line.
    /// </summary>
    private void UpdateLengthLabel()
    {
      // Abstand berechnen
      double distance =
        Math.Sqrt(Math.Pow(this.endPoint.Y - this.startPoint.Y, 2) + Math.Pow(this.endPoint.X - this.startPoint.X, 2));
      double result = distance * Viana.Project.CalibrationData.ScalePixelToUnit;

      ((Label)this.lengthLabelBorder.Child).Content = result.ToString("N2") + " "
                                                      + Viana.Project.CalibrationData.LengthUnit;
      double centerLineX = (this.ruler.X1 + this.ruler.X2) / 2;
      double centerLineY = (this.ruler.Y1 + this.ruler.Y2) / 2;

      Canvas.SetLeft(this.lengthLabelBorder, centerLineX - this.lengthLabelBorder.ActualWidth / 2);
      Canvas.SetTop(this.lengthLabelBorder, centerLineY - this.lengthLabelBorder.ActualHeight / 2);
    }

    #endregion
  }
}