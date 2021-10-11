// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arrow.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.CustomStyles.Controls
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Shapes;

  /// <summary>
  ///   This class encapsulates an arrow shape with a head and a description
  /// </summary>
  public sealed class Arrow : Shape
  {


    /// <summary>
    ///   The head height property
    /// </summary>
    public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register(
      "HeadHeight",
      typeof(double),
      typeof(Arrow),
      new FrameworkPropertyMetadata(
        0.0,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The head width property
    /// </summary>
    public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register(
      "HeadWidth",
      typeof(double),
      typeof(Arrow),
      new FrameworkPropertyMetadata(
        0.0,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The x1 property
    /// </summary>
    public static readonly DependencyProperty X1Property = DependencyProperty.Register(
      "X1",
      typeof(double),
      typeof(Arrow),
      new FrameworkPropertyMetadata(
        0.0,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The x2 property
    /// </summary>
    public static readonly DependencyProperty X2Property = DependencyProperty.Register(
      "X2",
      typeof(double),
      typeof(Arrow),
      new FrameworkPropertyMetadata(
        0.0,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The y1 property
    /// </summary>
    public static readonly DependencyProperty Y1Property = DependencyProperty.Register(
      "Y1",
      typeof(double),
      typeof(Arrow),
      new FrameworkPropertyMetadata(
        0.0,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The y2 property
    /// </summary>
    public static readonly DependencyProperty Y2Property = DependencyProperty.Register(
      "Y2",
      typeof(double),
      typeof(Arrow),
      new FrameworkPropertyMetadata(
        0.0,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));





    /// <summary>
    ///   Gets or sets the height of the head.
    /// </summary>
    /// <value>
    ///   The height of the head.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double HeadHeight
    {
      get => (double)this.GetValue(HeadHeightProperty);

      set => this.SetValue(HeadHeightProperty, value);
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
      get => (double)this.GetValue(HeadWidthProperty);

      set => this.SetValue(HeadWidthProperty, value);
    }

    /// <summary>
    ///   Gets or sets the x1.
    /// </summary>
    /// <value>
    ///   The x1.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double X1
    {
      get => (double)this.GetValue(X1Property);

      set => this.SetValue(X1Property, value);
    }

    /// <summary>
    ///   Gets or sets the x2.
    /// </summary>
    /// <value>
    ///   The x2.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double X2
    {
      get => (double)this.GetValue(X2Property);

      set => this.SetValue(X2Property, value);
    }

    /// <summary>
    ///   Gets or sets the y1.
    /// </summary>
    /// <value>
    ///   The y1.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double Y1
    {
      get => (double)this.GetValue(Y1Property);

      set => this.SetValue(Y1Property, value);
    }

    /// <summary>
    ///   Gets or sets the y2.
    /// </summary>
    /// <value>
    ///   The y2.
    /// </value>
    [TypeConverter(typeof(LengthConverter))]
    public double Y2
    {
      get => (double)this.GetValue(Y2Property);

      set => this.SetValue(Y2Property, value);
    }





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
        StreamGeometry geometry = new StreamGeometry { FillRule = FillRule.EvenOdd };

        using (StreamGeometryContext context = geometry.Open())
        {
          this.InternalDrawArrowGeometry(context);
        }

        // Freeze the geometry for performance benefits
        geometry.Freeze();

        return geometry;
      }
    }





    /// <summary>
    /// Internals the draw arrow geometry.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void InternalDrawArrowGeometry(StreamGeometryContext context)
    {
      double theta = Math.Atan2(this.Y1 - this.Y2, this.X1 - this.X2);
      double sint = Math.Sin(theta);
      double cost = Math.Cos(theta);

      Point pt1 = new Point(this.X1, this.Y1);
      Point pt2 = new Point(this.X2, this.Y2);

      Point pt3 = new Point(
        this.X2 + (this.HeadWidth * cost - this.HeadHeight * sint),
        this.Y2 + (this.HeadWidth * sint + this.HeadHeight * cost));

      Point pt4 = new Point(
        this.X2 + (this.HeadWidth * cost + this.HeadHeight * sint),
        this.Y2 - (this.HeadHeight * cost - this.HeadWidth * sint));

      context.BeginFigure(pt1, true, false);
      context.LineTo(pt2, true, true);
      context.LineTo(pt3, true, true);
      context.LineTo(pt4, true, true);
      context.LineTo(pt2, true, true);
    }


  }
}