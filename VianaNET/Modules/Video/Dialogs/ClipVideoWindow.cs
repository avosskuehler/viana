// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClipVideoWindow.cs" company="Freie Universität Berlin">
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
//   The clip video window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Dialogs
{
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using VianaNET.Application;
  using VianaNET.Modules.Base;
  using VianaNET.Resources;

  /// <summary>
  ///   The clip video window.
  /// </summary>
  public class ClipVideoWindow : WindowWithHelp
  {
    #region Constants

    /// <summary>
    ///   The margin.
    /// </summary>
    private const int DefaultMargin = 10;

    #endregion

    #region Fields

    /// <summary>
    ///   The bottom line.
    /// </summary>
    private readonly Line bottomLine;

    /// <summary>
    ///   The left line.
    /// </summary>
    private readonly Line leftLine;

    /// <summary>
    ///   The outer region.
    /// </summary>
    private readonly Path outerRegion;

    /// <summary>
    ///   The right line.
    /// </summary>
    private readonly Line rightLine;

    /// <summary>
    ///   The top line.
    /// </summary>
    private readonly Line topLine;

    /// <summary>
    ///   The current line.
    /// </summary>
    private Line currentLine;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ClipVideoWindow" /> class.
    /// </summary>
    public ClipVideoWindow()
    {
      this.Title = Labels.ClipVideoWindowTitle;
      this.LabelTitle.Content = Labels.ClipVideoHelpControlTitle;
      this.DescriptionTitle.Content = Labels.ClipVideoDescriptionTitle;
      this.DescriptionMessage.Text = Labels.ClipVideoDescriptionMessage;
      this.topLine = (Line)this.Resources["TopLine"];
      this.topLine.Name = "Top";
      this.topLine.MouseEnter += this.TopBottomLineMouseEnter;
      this.topLine.MouseLeave += this.LineMouseLeave;
      this.topLine.MouseLeftButtonDown += this.LineMouseLeftButtonDown;
      this.topLine.MouseMove += this.LineMouseMove;
      this.topLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.topLine);

      this.leftLine = (Line)this.Resources["LeftLine"];

      this.leftLine.Name = "Left";
      this.leftLine.MouseEnter += this.LeftRightLineMouseEnter;
      this.leftLine.MouseLeave += this.LineMouseLeave;
      this.leftLine.MouseLeftButtonDown += this.LineMouseLeftButtonDown;
      this.leftLine.MouseMove += this.LineMouseMove;
      this.leftLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.leftLine);

      this.bottomLine = (Line)this.Resources["BottomLine"];

      this.bottomLine.Name = "Bottom";
      this.bottomLine.MouseEnter += this.TopBottomLineMouseEnter;
      this.bottomLine.MouseLeave += this.LineMouseLeave;
      this.bottomLine.MouseLeftButtonDown += this.LineMouseLeftButtonDown;
      this.bottomLine.MouseMove += this.LineMouseMove;
      this.bottomLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.bottomLine);

      this.rightLine = (Line)this.Resources["RightLine"];

      this.rightLine.Name = "Right";
      this.rightLine.MouseEnter += this.LeftRightLineMouseEnter;
      this.rightLine.MouseLeave += this.LineMouseLeave;
      this.rightLine.MouseLeftButtonDown += this.LineMouseLeftButtonDown;
      this.rightLine.MouseMove += this.LineMouseMove;
      this.rightLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.rightLine);

      this.outerRegion = (Path)this.Resources["OuterRegion"];

      this.windowCanvas.Children.Insert(0, this.outerRegion);

      this.Loaded += this.WindowLoaded;
    }

    #endregion

    #region Methods

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
        // Reset clip region
        Viana.Project.CalibrationData.ClipRegion = new Rect(0, 0, 0, 0);
        Viana.Project.CalibrationData.HasClipRegion = false;
        this.DialogResult = true;
        this.Close();
      }
    }

    /// <summary>
    /// Handles the MouseEnter event of the LeftRightLine control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void LeftRightLineMouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }

    /// <summary>
    /// Handles the MouseLeave event of the Line control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void LineMouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
    }

    /// <summary>
    /// Handles the MouseLeftButtonDown event of the Line control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void LineMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.currentLine = sender as Line;
      Mouse.Capture(this.currentLine);
    }

    /// <summary>
    /// Handles the MouseMove event of the Line control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void LineMouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && this.currentLine != null)
      {
        double newX = e.GetPosition(this.VideoImage).X;
        double newY = e.GetPosition(this.VideoImage).Y;
        if (newX >= 0 && newY >= 0 && newX <= this.VideoImage.ActualWidth && newY <= this.VideoImage.ActualHeight)
        {
          switch (this.currentLine.Name)
          {
            case "Top":
              if (newY + DefaultMargin < this.bottomLine.Y1)
              {
                this.currentLine.Y1 = newY;
                this.currentLine.Y2 = newY;
                this.leftLine.Y1 = newY;
                this.rightLine.Y1 = newY;
              }

              break;
            case "Bottom":
              if (newY > this.topLine.Y1 + DefaultMargin)
              {
                this.currentLine.Y1 = newY;
                this.currentLine.Y2 = newY;
                this.leftLine.Y2 = newY;
                this.rightLine.Y2 = newY;
              }

              break;
            case "Left":
              if (newX + DefaultMargin < this.rightLine.X1)
              {
                this.currentLine.X1 = newX;
                this.currentLine.X2 = newX;
                this.topLine.X1 = newX;
                this.bottomLine.X1 = newX;
              }

              break;
            case "Right":
              if (newX > this.leftLine.X1 + DefaultMargin)
              {
                this.currentLine.X1 = newX;
                this.currentLine.X2 = newX;
                this.topLine.X2 = newX;
                this.bottomLine.X2 = newX;
              }

              break;
          }

          this.ResetOuterRegion();
        }
      }
    }

    /// <summary>
    /// Handles the MouseLeftButtonUp event of the Line control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    /// <summary>
    ///   Resets the outer region.
    /// </summary>
    private void ResetOuterRegion()
    {
      var geometry = this.outerRegion.Data as CombinedGeometry;
      if (geometry == null)
      {
        return;
      }

      var outerRect = geometry.Geometry1 as RectangleGeometry;
      if (outerRect != null)
      {
        outerRect.Rect = new Rect(0, 0, this.VideoImage.ActualWidth, this.VideoImage.ActualHeight);
      }

      var innerRect = geometry.Geometry2 as RectangleGeometry;
      if (innerRect != null)
      {
        innerRect.Rect = new Rect(
          new Point(this.leftLine.X1, this.topLine.Y1), 
          new Point(this.rightLine.X1, this.bottomLine.Y1));
      }

      double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
      double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;

      var rect = new Rect
                   {
                     Location = new Point(this.leftLine.X1 * factorX, this.topLine.Y1 * factorY), 
                     Width = (this.rightLine.X1 - this.leftLine.X1) * factorX, 
                     Height = (this.bottomLine.Y1 - this.topLine.Y1) * factorY
                   };

      Viana.Project.CalibrationData.ClipRegion = rect;
      Viana.Project.CalibrationData.HasClipRegion = true;
    }

    /// <summary>
    /// Handles the MouseEnter event of the TopBottomLine control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void TopBottomLineMouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
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
      if (!Viana.Project.CalibrationData.HasClipRegion)
      {
        this.topLine.X1 = DefaultMargin;
        this.topLine.X2 = this.VideoImage.ActualWidth - DefaultMargin;
        this.leftLine.Y1 = DefaultMargin;
        this.leftLine.Y2 = this.VideoImage.ActualHeight - DefaultMargin;
        this.bottomLine.X1 = DefaultMargin;
        this.bottomLine.Y1 = this.VideoImage.ActualHeight - DefaultMargin;
        this.bottomLine.X2 = this.VideoImage.ActualWidth - DefaultMargin;
        this.bottomLine.Y2 = this.VideoImage.ActualHeight - DefaultMargin;
        this.rightLine.X1 = this.VideoImage.ActualWidth - DefaultMargin;
        this.rightLine.Y1 = DefaultMargin;
        this.rightLine.X2 = this.VideoImage.ActualWidth - DefaultMargin;
        this.rightLine.Y2 = this.VideoImage.ActualHeight - DefaultMargin;
      }
      else
      {
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;

        this.topLine.X1 = Viana.Project.CalibrationData.ClipRegion.Left / factorX;
        this.topLine.X2 = Viana.Project.CalibrationData.ClipRegion.Right / factorX;
        this.topLine.Y1 = Viana.Project.CalibrationData.ClipRegion.Top / factorY;
        this.topLine.Y2 = Viana.Project.CalibrationData.ClipRegion.Top / factorY;
        this.leftLine.X1 = Viana.Project.CalibrationData.ClipRegion.Left / factorX;
        this.leftLine.X2 = Viana.Project.CalibrationData.ClipRegion.Left / factorX;
        this.leftLine.Y1 = Viana.Project.CalibrationData.ClipRegion.Top / factorY;
        this.leftLine.Y2 = Viana.Project.CalibrationData.ClipRegion.Bottom / factorY;
        this.bottomLine.X1 = Viana.Project.CalibrationData.ClipRegion.Left / factorX;
        this.bottomLine.Y1 = Viana.Project.CalibrationData.ClipRegion.Bottom / factorY;
        this.bottomLine.X2 = Viana.Project.CalibrationData.ClipRegion.Right / factorX;
        this.bottomLine.Y2 = Viana.Project.CalibrationData.ClipRegion.Bottom / factorY;
        this.rightLine.X1 = Viana.Project.CalibrationData.ClipRegion.Right / factorX;
        this.rightLine.Y1 = Viana.Project.CalibrationData.ClipRegion.Top / factorY;
        this.rightLine.X2 = Viana.Project.CalibrationData.ClipRegion.Right / factorX;
        this.rightLine.Y2 = Viana.Project.CalibrationData.ClipRegion.Bottom / factorY;
        Viana.Project.CalibrationData.HasClipRegion = true;
      }

      this.ResetOuterRegion();
    }

    #endregion
  }
}