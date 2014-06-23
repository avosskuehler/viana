// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoordinateSystemWindow.cs" company="Freie Universität Berlin">
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
// <summary>
//   The window to specify the coordinate system
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Dialogs
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Controls;
  using VianaNET.Modules.Base;
  using VianaNET.Resources;

  /// <summary>
  ///   The window to specify the coordinate system
  /// </summary>
  public class CoordinateSystemWindow : WindowWithHelp
  {
    #region Constants

    /// <summary>
    ///   The padding.
    /// </summary>
    private const double AxisDescriptionPadding = 10d;

    #endregion

    #region Fields

    /// <summary>
    ///   The x-direction arrow.
    /// </summary>
    private readonly Arrow directionX;

    /// <summary>
    ///   The y-direction arrow.
    /// </summary>
    private readonly Arrow directionY;

    /// <summary>
    ///   The label x.
    /// </summary>
    private readonly Border labelX;

    /// <summary>
    ///   The label y.
    /// </summary>
    private readonly Border labelY;

    /// <summary>
    ///   The origin path.
    /// </summary>
    private readonly Path originPath;

    /// <summary>
    ///   The direction x was set
    /// </summary>
    private bool directionXSet;

    /// <summary>
    ///   Mausbewegungen ignorieren, da über Control panel
    /// </summary>
    private bool ignoreMouse;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="CoordinateSystemWindow" /> class.
    /// </summary>
    public CoordinateSystemWindow()
    {
      this.playerContainerGrid.Margin = new Thickness(100d);
      this.Title = Labels.CoordinateSystemWindowTitle;
      this.LabelTitle.Content = Labels.CoordinateSystemHelpControlTitle;
      this.DescriptionTitle.Content = Labels.CoordinateSystemHowToHeader;
      this.DescriptionMessage.Text = Labels.CoordinateSystemHowToDescription;
      this.originPath = (Path)this.Resources["OriginPath"];
      this.originPath.Visibility = Visibility.Visible;
      this.windowCanvas.Children.Add(this.originPath);
      this.directionX = (Arrow)this.Resources["DirectionArrowX"];
      this.directionY = (Arrow)this.Resources["DirectionArrowY"];

      this.labelX = (Border)this.Resources["XLabelBorder"];
      this.labelY = (Border)this.Resources["YLabelBorder"];

      this.windowCanvas.Children.Add(this.directionX);
      this.windowCanvas.Children.Add(this.directionY);
      this.windowCanvas.Children.Add(this.labelX);
      this.windowCanvas.Children.Add(this.labelY);
      this.Loaded += this.CoordinateSystemWindowLoaded;
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

      Point mouseLocation = e.GetPosition(this.VideoImage);

      if (!this.directionXSet)
      {
        this.directionX.X2 = mouseLocation.X;
        this.directionX.Y2 = mouseLocation.Y;
        this.directionY.Visibility = Visibility.Visible;
        this.labelY.Visibility = Visibility.Visible;
        this.directionXSet = true;
        this.SetYAxis(e.GetPosition(this.VideoImage));
      }
      else
      {
        this.DialogResult = true;
        var vectorDefault = new Vector(1, 0);
        var vectorXAxis = new Vector(this.directionX.X2 - this.directionX.X1, this.directionX.Y2 - this.directionX.Y1);
        var vectorYAxis = new Vector(this.directionY.X2 - this.directionY.X1, this.directionY.Y2 - this.directionY.Y1);
        double angle = Vector.AngleBetween(vectorXAxis, vectorYAxis);
        int scaleY = angle > 0 ? 1 : -1;
        var matrix = new Matrix();
        double angleX = Vector.AngleBetween(vectorDefault, vectorXAxis);
        matrix.Scale(1, scaleY);
        matrix.Rotate(-angleX);

        Viana.Project.CalibrationData.CoordinateTransform = matrix;
        this.Close();
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

      Point mouseLocation = e.GetPosition(this.VideoImage);
      if (!this.directionXSet)
      {
        this.directionX.X2 = mouseLocation.X;
        this.directionX.Y2 = mouseLocation.Y;
        this.SetLabelXPosition(mouseLocation);
      }
      else
      {
        this.SetYAxis(mouseLocation);
      }
    }

    /// <summary>
    /// Mouse is over the control panel.
    /// </summary>
    /// <param name="isOver">
    /// if set to <c>true</c> is over.
    /// </param>
    protected override void MouseOverControlPanel(bool isOver)
    {
      base.MouseOverControlPanel(isOver);
      this.ignoreMouse = isOver;
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/> attached event reaches an
    ///   element
    ///   in its route that is derived from this class. Implement this method to add class handling for this event.
    ///   Resets the clipping on F10.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.
    /// </param>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      if (e.SystemKey == Key.F10)
      {
        // Reset coordinate system
        Viana.Project.CalibrationData.CoordinateTransform = Matrix.Identity;
        this.DialogResult = true;
        this.Close();
      }
    }

    /// <summary>
    /// Handles the Loaded event of the CoordinateSystemWindow control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void CoordinateSystemWindowLoaded(object sender, RoutedEventArgs e)
    {
      double scaleX;
      double scaleY;
      if (this.GetScales(out scaleX, out scaleY))
      {
        double originXInScreenPixel = Viana.Project.CalibrationData.OriginInPixel.X * scaleX;
        double originYInScreenPixel = Viana.Project.CalibrationData.OriginInPixel.Y * scaleY;
        Canvas.SetLeft(this.originPath, originXInScreenPixel - this.originPath.ActualWidth / 2);
        Canvas.SetTop(this.originPath, originYInScreenPixel - this.originPath.ActualHeight / 2);
        this.directionX.X1 = originXInScreenPixel;
        this.directionX.Y1 = originYInScreenPixel;
        this.directionY.X1 = originXInScreenPixel;
        this.directionY.Y1 = originYInScreenPixel;
        Point mousePoint = Mouse.GetPosition(this.VideoImage);
        this.directionX.X2 = mousePoint.X;
        this.directionX.Y2 = mousePoint.Y;
        this.SetLabelXPosition(mousePoint);
      }
    }

    /// <summary>
    /// Gets the scales.
    /// </summary>
    /// <param name="scaleX">
    /// The scale x.
    /// </param>
    /// <param name="scaleY">
    /// The scale y.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = this.VideoImage.ActualWidth / this.VideoImage.Source.Width;
      scaleY = this.VideoImage.ActualHeight / this.VideoImage.Source.Height;

      return !double.IsInfinity(scaleX) && !double.IsNaN(scaleX);
    }

    /// <summary>
    /// The set label x position.
    /// </summary>
    /// <param name="arrowTip">
    /// The arrow tip.
    /// </param>
    private void SetLabelXPosition(Point arrowTip)
    {
      double moveX = this.directionX.X2 > this.directionX.X1 ? AxisDescriptionPadding : -AxisDescriptionPadding;
      double moveY = this.directionX.Y2 > this.directionX.Y1 ? AxisDescriptionPadding : -AxisDescriptionPadding;
      Canvas.SetLeft(this.labelX, arrowTip.X - this.labelX.ActualWidth / 2 + moveX);
      Canvas.SetTop(this.labelX, arrowTip.Y - this.labelX.ActualHeight / 2 + moveY);
    }

    /// <summary>
    /// The set label y position.
    /// </summary>
    /// <param name="arrowTip">
    /// The arrow tip.
    /// </param>
    private void SetLabelYPosition(Point arrowTip)
    {
      double moveX = this.directionY.X2 > this.directionY.X1 ? AxisDescriptionPadding : -AxisDescriptionPadding;
      double moveY = this.directionY.Y2 > this.directionY.Y1 ? AxisDescriptionPadding : -AxisDescriptionPadding;
      Canvas.SetLeft(this.labelY, arrowTip.X - this.labelY.ActualWidth / 2 + moveX);
      Canvas.SetTop(this.labelY, arrowTip.Y - this.labelY.ActualHeight / 2 + moveY);
    }

    /// <summary>
    /// Sets the y axis location and description.
    /// </summary>
    /// <param name="mouseLocation">
    /// The current mouse location.
    /// </param>
    private void SetYAxis(Point mouseLocation)
    {
      var lineXEndpoint = new Point(this.directionX.X2, this.directionX.Y2);
      this.SetLabelXPosition(lineXEndpoint);

      double scaleX;
      double scaleY;
      if (this.GetScales(out scaleX, out scaleY))
      {
        double originXInScreenPixel = Viana.Project.CalibrationData.OriginInPixel.X * scaleX;
        double originYInScreenPixel = Viana.Project.CalibrationData.OriginInPixel.Y * scaleY;

        var vectorXAxis = new Vector(this.directionX.X2 - this.directionX.X1, this.directionX.Y2 - this.directionX.Y1);
        var vectorMouse = new Vector(mouseLocation.X - originXInScreenPixel, mouseLocation.Y - originYInScreenPixel);
        double angleMouseXAxis = Vector.AngleBetween(vectorXAxis, vectorMouse);
        Vector vectorY;
        if (angleMouseXAxis >= 0 && angleMouseXAxis <= 180)
        {
          vectorY = new Vector(-vectorXAxis.Y, vectorXAxis.X);
        }
        else
        {
          vectorY = new Vector(vectorXAxis.Y, -vectorXAxis.X);
        }

        this.directionY.X2 = originXInScreenPixel + vectorY.X;
        this.directionY.Y2 = originYInScreenPixel + vectorY.Y;
      }

      this.SetLabelYPosition(new Point(this.directionY.X2, this.directionY.Y2));
    }

    #endregion
  }
}