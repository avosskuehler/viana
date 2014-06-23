// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arc.cs" company="Freie Universität Berlin">
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
// <summary>
//   The arc is a part of a circle with start and end angle
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Controls
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Shapes;

  /// <summary>
  ///   The arc is a part of a circle with start and end angle
  /// </summary>
  public sealed class Arc : Shape
  {
    #region Static Fields

    /// <summary>
    ///   The center property.
    /// </summary>
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
      "Center", 
      typeof(Point), 
      typeof(Arc), 
      new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The end angle property.
    /// </summary>
    public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
      "EndAngle", 
      typeof(double), 
      typeof(Arc), 
      new FrameworkPropertyMetadata(180d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The radius property.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
      "Radius", 
      typeof(double), 
      typeof(Arc), 
      new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The small angle property.
    /// </summary>
    public static readonly DependencyProperty SmallAngleProperty = DependencyProperty.Register(
      "SmallAngle", 
      typeof(bool), 
      typeof(Arc), 
      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The start angle property.
    /// </summary>
    public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
      "StartAngle", 
      typeof(double), 
      typeof(Arc), 
      new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="Arc" /> class.
    /// </summary>
    static Arc()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Arc), new FrameworkPropertyMetadata(typeof(Arc)));
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the center.
    /// </summary>
    public Point Center
    {
      get
      {
        return (Point)this.GetValue(CenterProperty);
      }

      set
      {
        this.SetValue(CenterProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the end angle in degrees
    /// </summary>
    public double EndAngle
    {
      get
      {
        return (double)this.GetValue(EndAngleProperty);
      }

      set
      {
        this.SetValue(EndAngleProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the radius.
    /// </summary>
    public double Radius
    {
      get
      {
        return (double)this.GetValue(RadiusProperty);
      }

      set
      {
        this.SetValue(RadiusProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether small angle.
    /// </summary>
    public bool SmallAngle
    {
      get
      {
        return (bool)this.GetValue(SmallAngleProperty);
      }

      set
      {
        this.SetValue(SmallAngleProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the start angle in degrees
    /// </summary>
    public double StartAngle
    {
      get
      {
        return (double)this.GetValue(StartAngleProperty);
      }

      set
      {
        this.SetValue(StartAngleProperty, value);
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the defining geometry.
    /// </summary>
    protected override Geometry DefiningGeometry
    {
      get
      {
        double start = this.StartAngle * Math.PI / 180; // 0
        double end = this.EndAngle * Math.PI / 180; // PI/2

        double a0 = start < 0 ? start + 2 * Math.PI : start; // 0
        double a1 = end < 0 ? end + 2 * Math.PI : end; // PI/2

        if (a1 < a0)
        {
          a1 += Math.PI * 2;
        }

        var d = SweepDirection.Clockwise;
        bool large;

        if (this.SmallAngle)
        {
          large = false;
          d = (a1 - a0) > Math.PI ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
        }
        else
        {
          large = Math.Abs(a1 - a0) > Math.PI; // false
        }

        var dir1 = new Vector(Math.Cos(a0), Math.Sin(a0));
        dir1.Normalize();
        var dir2 = new Vector(Math.Cos(a1), Math.Sin(a1));
        dir2.Normalize();

        Point p0 = this.Center + dir1 * this.Radius;
        Point p1 = this.Center + dir2 * this.Radius;

        var segments = new List<PathSegment>(1);
        segments.Add(new ArcSegment(p1, new Size(this.Radius, this.Radius), 0.0, large, d, true));

        var figures = new List<PathFigure>(1);
        var pf = new PathFigure(p0, segments, true);
        pf.IsClosed = false;
        figures.Add(pf);

        Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);
        return g;
      }
    }

    #endregion
  }
}