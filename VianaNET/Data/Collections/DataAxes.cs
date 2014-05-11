// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAxes.cs" company="Freie Universität Berlin">
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
//   The data axis.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Collections
{
  using System.Collections.Generic;
  using System.Windows;

  using VianaNET.CustomStyles.Types;

  using WPFLocalizeExtension.Extensions;

  /// <summary>
  ///   The data axis.
  /// </summary>
  public class DataAxis : DependencyObject
  {
    #region Static Fields

    /// <summary>
    ///   The axis property.
    /// </summary>
    public static readonly DependencyProperty AxisProperty = DependencyProperty.Register(
      "Axis", 
      typeof(AxisType), 
      typeof(DataAxis), 
      new FrameworkPropertyMetadata(default(AxisType), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The description property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
      "Description", 
      typeof(string), 
      typeof(DataAxis), 
      new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The data axes.
    /// </summary>
    public static List<DataAxis> DataAxes;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="DataAxis" /> class.
    /// </summary>
    static DataAxis()
    {
      // Single axis data
      var iAxis = new DataAxis(AxisType.I);
      var lociAxis = new LocExtension("VianaNET:Labels:AxisFrame");
      lociAxis.SetBinding(iAxis, DescriptionProperty);

      var tAxis = new DataAxis(AxisType.T);
      var loctAxis = new LocExtension("VianaNET:Labels:AxisTime");
      loctAxis.SetBinding(tAxis, DescriptionProperty);

      var xAxis = new DataAxis(AxisType.X);
      var locxAxis = new LocExtension("VianaNET:Labels:AxisPixelX");
      locxAxis.SetBinding(xAxis, DescriptionProperty);

      var yAxis = new DataAxis(AxisType.Y);
      var locyAxis = new LocExtension("VianaNET:Labels:AxisPixelY");
      locyAxis.SetBinding(yAxis, DescriptionProperty);

      var pxAxis = new DataAxis(AxisType.PX);
      var locpxAxis = new LocExtension("VianaNET:Labels:AxisPositionX");
      locpxAxis.SetBinding(pxAxis, DescriptionProperty);

      var pyAxis = new DataAxis(AxisType.PY);
      var locpyAxis = new LocExtension("VianaNET:Labels:AxisPositionY");
      locpyAxis.SetBinding(pyAxis, DescriptionProperty);

      var dAxis = new DataAxis(AxisType.D);
      var locdAxis = new LocExtension("VianaNET:Labels:AxisDistance");
      locdAxis.SetBinding(dAxis, DescriptionProperty);

      var dxAxis = new DataAxis(AxisType.DX);
      var locdxAxis = new LocExtension("VianaNET:Labels:AxisDistanceX");
      locdxAxis.SetBinding(dxAxis, DescriptionProperty);

      var dyAxis = new DataAxis(AxisType.DY);
      var locdyAxis = new LocExtension("VianaNET:Labels:AxisDistanceY");
      locdyAxis.SetBinding(dyAxis, DescriptionProperty);

      var sAxis = new DataAxis(AxisType.S);
      var locsAxis = new LocExtension("VianaNET:Labels:AxisLength");
      locsAxis.SetBinding(sAxis, DescriptionProperty);

      var sxAxis = new DataAxis(AxisType.SX);
      var locsxAxis = new LocExtension("VianaNET:Labels:AxisLengthX");
      locsxAxis.SetBinding(sxAxis, DescriptionProperty);

      var syAxis = new DataAxis(AxisType.SY);
      var locsyAxis = new LocExtension("VianaNET:Labels:AxisLengthY");
      locsyAxis.SetBinding(syAxis, DescriptionProperty);

      var vAxis = new DataAxis(AxisType.V);
      var locvAxis = new LocExtension("VianaNET:Labels:AxisVelocity");
      locvAxis.SetBinding(vAxis, DescriptionProperty);

      var vxAxis = new DataAxis(AxisType.VX);
      var locvxAxis = new LocExtension("VianaNET:Labels:AxisVelocityXDirection");
      locvxAxis.SetBinding(vxAxis, DescriptionProperty);

      var vyAxis = new DataAxis(AxisType.VY);
      var locvyAxis = new LocExtension("VianaNET:Labels:AxisVelocityYDirection");
      locvyAxis.SetBinding(vyAxis, DescriptionProperty);

      var viAxis = new DataAxis(AxisType.VI);
      var locviAxis = new LocExtension("VianaNET:Labels:AxisVelocityInterpolated");
      locviAxis.SetBinding(viAxis, DescriptionProperty);

      var vxiAxis = new DataAxis(AxisType.VXI);
      var locvxiAxis = new LocExtension("VianaNET:Labels:AxisVelocityXDirectionInterpolated");
      locvxiAxis.SetBinding(vxiAxis, DescriptionProperty);

      var vyiAxis = new DataAxis(AxisType.VYI);
      var locvyiAxis = new LocExtension("VianaNET:Labels:AxisVelocityYDirectionInterpolated");
      locvyiAxis.SetBinding(vyiAxis, DescriptionProperty);

      var aAxis = new DataAxis(AxisType.A);
      var locaAxis = new LocExtension("VianaNET:Labels:AxisAcceleration");
      locaAxis.SetBinding(aAxis, DescriptionProperty);

      var axAxis = new DataAxis(AxisType.AX);
      var locaxAxis = new LocExtension("VianaNET:Labels:AxisAccelerationXDirection");
      locaxAxis.SetBinding(axAxis, DescriptionProperty);

      var ayAxis = new DataAxis(AxisType.AY);
      var locayAxis = new LocExtension("VianaNET:Labels:AxisAccelerationYDirection");
      locayAxis.SetBinding(ayAxis, DescriptionProperty);

      var aiAxis = new DataAxis(AxisType.AI);
      var locaiAxis = new LocExtension("VianaNET:Labels:AxisAccelerationInterpolated");
      locaiAxis.SetBinding(aiAxis, DescriptionProperty);

      var axiAxis = new DataAxis(AxisType.AXI);
      var locaxiAxis = new LocExtension("VianaNET:Labels:AxisAccelerationXDirectionInterpolated");
      locaxiAxis.SetBinding(axiAxis, DescriptionProperty);

      var ayiAxis = new DataAxis(AxisType.AYI);
      var locayiAxis = new LocExtension("VianaNET:Labels:AxisAccelerationYDirectionInterpolated");
      locayiAxis.SetBinding(ayiAxis, DescriptionProperty);

      DataAxes = new List<DataAxis>
        {
          iAxis,
          tAxis,
          xAxis,
          yAxis,
          pxAxis,
          pyAxis,
          dAxis,
          dxAxis,
          dyAxis,
          sAxis,
          sxAxis,
          syAxis,
          vAxis,
          vxAxis,
          vyAxis,
          viAxis,
          vxiAxis,
          vyiAxis,
          aAxis,
          axAxis,
          ayAxis,
          aiAxis,
          axiAxis,
          ayiAxis
        };
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="DataAxis" /> class.
    /// </summary>
    public DataAxis()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataAxis"/> class.
    /// </summary>
    /// <param name="axis">
    /// The axis. 
    /// </param>
    public DataAxis(AxisType axis)
    {
      this.Axis = axis;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the axis.
    /// </summary>
    public AxisType Axis
    {
      get
      {
        return (AxisType)this.GetValue(AxisProperty);
      }

      set
      {
        this.SetValue(AxisProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the description.
    /// </summary>
    public string Description
    {
      get
      {
        return (string)this.GetValue(DescriptionProperty);
      }

      set
      {
        this.SetValue(DescriptionProperty, value);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    public override string ToString()
    {
      return this.Description;
    }

    #endregion
  }
}