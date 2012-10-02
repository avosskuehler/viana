// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClipVideoWindow.cs" company="Freie Universität Berlin">
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
//   The clip video window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Dialogs
{
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using VianaNET.Data;
  using VianaNET.Localization;
  using VianaNET.Modules.Base;

  /// <summary>
  ///   The clip video window.
  /// </summary>
  public class ClipVideoWindow : WindowWithHelp
  {
    #region Constants

    /// <summary>
    ///   The margin.
    /// </summary>
    private const int margin = 10;

    #endregion

    #region Fields

    /// <summary>
    ///   The bottom line.
    /// </summary>
    private readonly Line BottomLine;

    /// <summary>
    ///   The left line.
    /// </summary>
    private readonly Line LeftLine;

    /// <summary>
    ///   The outer region.
    /// </summary>
    private readonly Path OuterRegion;

    /// <summary>
    ///   The right line.
    /// </summary>
    private readonly Line RightLine;

    /// <summary>
    ///   The top line.
    /// </summary>
    private readonly Line TopLine;

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
      this.TopLine = (Line)this.Resources["TopLine"];
      this.TopLine.Name = "Top";
      this.TopLine.MouseEnter += this.TopBottomLine_MouseEnter;
      this.TopLine.MouseLeave += this.Line_MouseLeave;
      this.TopLine.MouseLeftButtonDown += this.Line_MouseLeftButtonDown;
      this.TopLine.MouseMove += this.Line_MouseMove;
      this.TopLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.TopLine);

      this.LeftLine = (Line)this.Resources["LeftLine"];

      this.LeftLine.Name = "Left";
      this.LeftLine.MouseEnter += this.LeftRightLine_MouseEnter;
      this.LeftLine.MouseLeave += this.Line_MouseLeave;
      this.LeftLine.MouseLeftButtonDown += this.Line_MouseLeftButtonDown;
      this.LeftLine.MouseMove += this.Line_MouseMove;
      this.LeftLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.LeftLine);

      this.BottomLine = (Line)this.Resources["BottomLine"];

      this.BottomLine.Name = "Bottom";
      this.BottomLine.MouseEnter += this.TopBottomLine_MouseEnter;
      this.BottomLine.MouseLeave += this.Line_MouseLeave;
      this.BottomLine.MouseLeftButtonDown += this.Line_MouseLeftButtonDown;
      this.BottomLine.MouseMove += this.Line_MouseMove;
      this.BottomLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.BottomLine);

      this.RightLine = (Line)this.Resources["RightLine"];

      this.RightLine.Name = "Right";
      this.RightLine.MouseEnter += this.LeftRightLine_MouseEnter;
      this.RightLine.MouseLeave += this.Line_MouseLeave;
      this.RightLine.MouseLeftButtonDown += this.Line_MouseLeftButtonDown;
      this.RightLine.MouseMove += this.Line_MouseMove;
      this.RightLine.MouseLeftButtonUp += this.Line_MouseLeftButtonUp;
      this.windowCanvas.Children.Insert(0, this.RightLine);

      this.OuterRegion = (Path)this.Resources["OuterRegion"];

      this.windowCanvas.Children.Insert(0, this.OuterRegion);

      this.Loaded += this.Window_Loaded;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The left right line_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LeftRightLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }

    /// <summary>
    /// The line_ mouse leave.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Line_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
    }

    /// <summary>
    /// The line_ mouse left button down.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.currentLine = sender as Line;
      Mouse.Capture(this.currentLine);
    }

    /// <summary>
    /// The line_ mouse left button up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    /// <summary>
    /// The line_ mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Line_MouseMove(object sender, MouseEventArgs e)
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
              if (newY + margin < this.BottomLine.Y1)
              {
                this.currentLine.Y1 = newY;
                this.currentLine.Y2 = newY;
                this.LeftLine.Y1 = newY;
                this.RightLine.Y1 = newY;
              }

              break;
            case "Bottom":
              if (newY > this.TopLine.Y1 + margin)
              {
                this.currentLine.Y1 = newY;
                this.currentLine.Y2 = newY;
                this.LeftLine.Y2 = newY;
                this.RightLine.Y2 = newY;
              }

              break;
            case "Left":
              if (newX + margin < this.RightLine.X1)
              {
                this.currentLine.X1 = newX;
                this.currentLine.X2 = newX;
                this.TopLine.X1 = newX;
                this.BottomLine.X1 = newX;
              }

              break;
            case "Right":
              if (newX > this.LeftLine.X1 + margin)
              {
                this.currentLine.X1 = newX;
                this.currentLine.X2 = newX;
                this.TopLine.X2 = newX;
                this.BottomLine.X2 = newX;
              }

              break;
          }

          this.ResetOuterRegion();
        }
      }
    }

    /// <summary>
    ///   The reset outer region.
    /// </summary>
    private void ResetOuterRegion()
    {
      var geometry = this.OuterRegion.Data as CombinedGeometry;
      var outerRect = geometry.Geometry1 as RectangleGeometry;
      outerRect.Rect = new Rect(0, 0, this.VideoImage.ActualWidth, this.VideoImage.ActualHeight);
      var innerRect = geometry.Geometry2 as RectangleGeometry;
      innerRect.Rect = new Rect(
        new Point(this.LeftLine.X1, this.TopLine.Y1), new Point(this.RightLine.X1, this.BottomLine.Y1));

      double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
      double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;

      var rect = new Rect();
      rect.Location = new Point(this.LeftLine.X1 * factorX, this.TopLine.Y1 * factorY);
      rect.Width = (this.RightLine.X1 - this.LeftLine.X1) * factorX;
      rect.Height = (this.BottomLine.Y1 - this.TopLine.Y1) * factorY;
      Calibration.Instance.ClipRegion = rect;
      Calibration.Instance.HasClipRegion = true;
    }

    /// <summary>
    /// The top bottom line_ mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TopBottomLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
    }

    /// <summary>
    /// The window_ loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (!Calibration.Instance.HasClipRegion)
      {
        this.TopLine.X1 = margin;
        this.TopLine.X2 = this.VideoImage.ActualWidth - margin;
        this.LeftLine.Y1 = margin;
        this.LeftLine.Y2 = this.VideoImage.ActualHeight - margin;
        this.BottomLine.X1 = margin;
        this.BottomLine.Y1 = this.VideoImage.ActualHeight - margin;
        this.BottomLine.X2 = this.VideoImage.ActualWidth - margin;
        this.BottomLine.Y2 = this.VideoImage.ActualHeight - margin;
        this.RightLine.X1 = this.VideoImage.ActualWidth - margin;
        this.RightLine.Y1 = margin;
        this.RightLine.X2 = this.VideoImage.ActualWidth - margin;
        this.RightLine.Y2 = this.VideoImage.ActualHeight - margin;
      }
      else
      {
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;

        this.TopLine.X1 = Calibration.Instance.ClipRegion.Left / factorX;
        this.TopLine.X2 = Calibration.Instance.ClipRegion.Right / factorX;
        this.TopLine.Y1 = Calibration.Instance.ClipRegion.Top / factorY;
        this.TopLine.Y2 = Calibration.Instance.ClipRegion.Top / factorY;
        this.LeftLine.X1 = Calibration.Instance.ClipRegion.Left / factorX;
        this.LeftLine.X2 = Calibration.Instance.ClipRegion.Left / factorX;
        this.LeftLine.Y1 = Calibration.Instance.ClipRegion.Top / factorY;
        this.LeftLine.Y2 = Calibration.Instance.ClipRegion.Bottom / factorY;
        this.BottomLine.X1 = Calibration.Instance.ClipRegion.Left / factorX;
        this.BottomLine.Y1 = Calibration.Instance.ClipRegion.Bottom / factorY;
        this.BottomLine.X2 = Calibration.Instance.ClipRegion.Right / factorX;
        this.BottomLine.Y2 = Calibration.Instance.ClipRegion.Bottom / factorY;
        this.RightLine.X1 = Calibration.Instance.ClipRegion.Right / factorX;
        this.RightLine.Y1 = Calibration.Instance.ClipRegion.Top / factorY;
        this.RightLine.X2 = Calibration.Instance.ClipRegion.Right / factorX;
        this.RightLine.Y2 = Calibration.Instance.ClipRegion.Bottom / factorY;
        Calibration.Instance.HasClipRegion = true;
      }

      this.ResetOuterRegion();
    }

    #endregion
  }
}