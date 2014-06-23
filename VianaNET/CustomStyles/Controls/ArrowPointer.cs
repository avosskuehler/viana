// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrowPointer.cs" company="Freie Universit�t Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Vo�k�hler  
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
// <summary>
//   The arrow pointer is a cursor with four arrows pointing towards a center point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.CustomStyles.Controls
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Shapes;

  /// <summary>
  ///   The arrow pointer is a cursor with four arrows pointing towards a center point.
  /// </summary>
  public sealed class ArrowPointer : Shape
  {
    #region Static Fields

    /// <summary>
    ///   The center space property
    /// </summary>
    public static readonly DependencyProperty CenterSpaceProperty = DependencyProperty.Register(
      "CenterSpace", 
      typeof(double), 
      typeof(ArrowPointer), 
      new FrameworkPropertyMetadata(
        5.0, 
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The head height property
    /// </summary>
    public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register(
      "HeadHeight", 
      typeof(double), 
      typeof(ArrowPointer), 
      new FrameworkPropertyMetadata(
        10.0, 
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The head width property
    /// </summary>
    public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register(
      "HeadWidth", 
      typeof(double), 
      typeof(ArrowPointer), 
      new FrameworkPropertyMetadata(
        10.0, 
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The length property
    /// </summary>
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
      "Length", 
      typeof(double), 
      typeof(ArrowPointer), 
      new FrameworkPropertyMetadata(
        0.0, 
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The x property
    /// </summary>
    public static readonly DependencyProperty XProperty = DependencyProperty.Register(
      "X", 
      typeof(double), 
      typeof(ArrowPointer), 
      new FrameworkPropertyMetadata(
        0.0, 
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The y property
    /// </summary>
    public static readonly DependencyProperty YProperty = DependencyProperty.Register(
      "Y", 
      typeof(double), 
      typeof(ArrowPointer), 
      new FrameworkPropertyMetadata(
        0.0, 
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the center space value.
    /// </summary>
    /// <value>
    ///   The length.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double CenterSpace
    {
      get
      {
        return (double)this.GetValue(CenterSpaceProperty);
      }

      set
      {
        this.SetValue(CenterSpaceProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the height of the head.
    /// </summary>
    /// <value>
    ///   The height of the head.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double HeadHeight
    {
      get
      {
        return (double)this.GetValue(HeadHeightProperty);
      }

      set
      {
        this.SetValue(HeadHeightProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the width of the head.
    /// </summary>
    /// <value>
    ///   The width of the head.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double HeadWidth
    {
      get
      {
        return (double)this.GetValue(HeadWidthProperty);
      }

      set
      {
        this.SetValue(HeadWidthProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the length.
    /// </summary>
    /// <value>
    ///   The length.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double Length
    {
      get
      {
        return (double)this.GetValue(LengthProperty);
      }

      set
      {
        this.SetValue(LengthProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the x.
    /// </summary>
    /// <value>
    ///   The x.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double X
    {
      get
      {
        return (double)this.GetValue(XProperty);
      }

      set
      {
        this.SetValue(XProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the y.
    /// </summary>
    /// <value>
    ///   The y.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double Y
    {
      get
      {
        return (double)this.GetValue(YProperty);
      }

      set
      {
        this.SetValue(YProperty, value);
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value that represents the <see cref="T:System.Windows.Media.Geometry" /> of the
    ///   <see cref="T:System.Windows.Shapes.Shape" />.
    /// </summary>
    /// <returns>The <see cref="T:System.Windows.Media.Geometry" /> of the <see cref="T:System.Windows.Shapes.Shape" />.</returns>
    protected override Geometry DefiningGeometry
    {
      get
      {
        // Create a StreamGeometry for describing the shape
        var geometry = new StreamGeometry { FillRule = FillRule.EvenOdd };

        using (StreamGeometryContext context = geometry.Open())
        {
          this.InternalDrawArrowGeometry(context);
        }

        // Freeze the geometry for performance benefits
        geometry.Freeze();

        return geometry;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The draw arrow.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="y1">
    /// The y 1.
    /// </param>
    /// <param name="y2">
    /// The y 2.
    /// </param>
    /// <param name="x1">
    /// The x 1.
    /// </param>
    /// <param name="x2">
    /// The x 2.
    /// </param>
    private void DrawArrow(StreamGeometryContext context, double y1, double y2, double x1, double x2)
    {
      double theta = Math.Atan2(y1 - y2, x1 - x2);
      double sint = Math.Sin(theta);
      double cost = Math.Cos(theta);

      var pt1 = new Point(x1, y1);
      var pt2 = new Point(x2, y2);

      var pt3 = new Point(
        x2 + (this.HeadWidth * cost - this.HeadHeight * sint), 
        y2 + (this.HeadWidth * sint + this.HeadHeight * cost));

      var pt4 = new Point(
        x2 + (this.HeadWidth * cost + this.HeadHeight * sint), 
        y2 - (this.HeadHeight * cost - this.HeadWidth * sint));

      context.LineTo(pt1, false, false);
      context.LineTo(pt2, true, true);
      context.LineTo(pt3, true, true);
      context.LineTo(pt2, true, true);
      context.LineTo(pt4, true, true);
    }

    /// <summary>
    /// Draw the arrow pointers geometry.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void InternalDrawArrowGeometry(StreamGeometryContext context)
    {
      context.BeginFigure(new Point(this.X, this.Y), false, false);

      double x1s = this.X - this.CenterSpace - this.Length;
      double y1s = this.Y - this.CenterSpace - this.Length;
      double x1e = this.X - this.CenterSpace;
      double y1e = this.Y - this.CenterSpace;
      this.DrawArrow(context, y1s, y1e, x1s, x1e);

      double x2s = this.X + this.CenterSpace + this.Length;
      double y2s = this.Y + this.CenterSpace + this.Length;
      double x2e = this.X + this.CenterSpace;
      double y2e = this.Y + this.CenterSpace;
      this.DrawArrow(context, y2s, y2e, x2s, x2e);

      double x3s = this.X + this.CenterSpace + this.Length;
      double y3s = this.Y - this.CenterSpace - this.Length;
      double x3e = this.X + this.CenterSpace;
      double y3e = this.Y - this.CenterSpace;
      this.DrawArrow(context, y3s, y3e, x3s, x3e);

      double x4s = this.X - this.CenterSpace - this.Length;
      double y4s = this.Y + this.CenterSpace + this.Length;
      double x4e = this.X - this.CenterSpace;
      double y4e = this.Y + this.CenterSpace;
      this.DrawArrow(context, y4s, y4e, x4s, x4e);

      context.BeginFigure(new Point(x1s, y1s), true, false);
      context.LineTo(new Point(x3s, y3s), false, false);
      context.LineTo(new Point(x2s, y2s), false, false);
      context.LineTo(new Point(x4s, y4s), false, false);
      context.LineTo(new Point(x1s, y1s), false, false);
    }

    #endregion
  }
}