// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAxes.cs" company="Freie Universität Berlin">
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
    ///   The ShouldExport property.
    /// </summary>
    public static readonly DependencyProperty ShouldExportProperty = DependencyProperty.Register(
      "ShouldExport", 
      typeof(bool), 
      typeof(DataAxis), 
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None));
     
    /// <summary>
    ///   The data axes.
    /// </summary>
    public static List<DataAxis> DataAxes;





    /// <summary>
    ///   Initializes static members of the <see cref="DataAxis" /> class.
    /// </summary>
    static DataAxis()
    {
      // Single axis data
      DataAxis iAxis = new DataAxis(AxisType.I);
      LocExtension lociAxis = new LocExtension("VianaNET:Labels:AxisFrame");
      lociAxis.SetBinding(iAxis, DescriptionProperty);

      DataAxis tAxis = new DataAxis(AxisType.T);
      LocExtension loctAxis = new LocExtension("VianaNET:Labels:AxisTime");
      loctAxis.SetBinding(tAxis, DescriptionProperty);

      DataAxis xAxis = new DataAxis(AxisType.X);
      LocExtension locxAxis = new LocExtension("VianaNET:Labels:AxisPixelX");
      locxAxis.SetBinding(xAxis, DescriptionProperty);

      DataAxis yAxis = new DataAxis(AxisType.Y);
      LocExtension locyAxis = new LocExtension("VianaNET:Labels:AxisPixelY");
      locyAxis.SetBinding(yAxis, DescriptionProperty);

      DataAxis pxAxis = new DataAxis(AxisType.PX);
      LocExtension locpxAxis = new LocExtension("VianaNET:Labels:AxisPositionX");
      locpxAxis.SetBinding(pxAxis, DescriptionProperty);

      DataAxis pyAxis = new DataAxis(AxisType.PY);
      LocExtension locpyAxis = new LocExtension("VianaNET:Labels:AxisPositionY");
      locpyAxis.SetBinding(pyAxis, DescriptionProperty);

      DataAxis dAxis = new DataAxis(AxisType.D);
      LocExtension locdAxis = new LocExtension("VianaNET:Labels:AxisDistance");
      locdAxis.SetBinding(dAxis, DescriptionProperty);

      DataAxis dxAxis = new DataAxis(AxisType.DX);
      LocExtension locdxAxis = new LocExtension("VianaNET:Labels:AxisDistanceX");
      locdxAxis.SetBinding(dxAxis, DescriptionProperty);

      DataAxis dyAxis = new DataAxis(AxisType.DY);
      LocExtension locdyAxis = new LocExtension("VianaNET:Labels:AxisDistanceY");
      locdyAxis.SetBinding(dyAxis, DescriptionProperty);

      DataAxis sAxis = new DataAxis(AxisType.S);
      LocExtension locsAxis = new LocExtension("VianaNET:Labels:AxisLength");
      locsAxis.SetBinding(sAxis, DescriptionProperty);

      DataAxis sxAxis = new DataAxis(AxisType.SX);
      LocExtension locsxAxis = new LocExtension("VianaNET:Labels:AxisLengthX");
      locsxAxis.SetBinding(sxAxis, DescriptionProperty);

      DataAxis syAxis = new DataAxis(AxisType.SY);
      LocExtension locsyAxis = new LocExtension("VianaNET:Labels:AxisLengthY");
      locsyAxis.SetBinding(syAxis, DescriptionProperty);

      DataAxis vAxis = new DataAxis(AxisType.V);
      LocExtension locvAxis = new LocExtension("VianaNET:Labels:AxisVelocity");
      locvAxis.SetBinding(vAxis, DescriptionProperty);

      DataAxis vxAxis = new DataAxis(AxisType.VX);
      LocExtension locvxAxis = new LocExtension("VianaNET:Labels:AxisVelocityXDirection");
      locvxAxis.SetBinding(vxAxis, DescriptionProperty);

      DataAxis vyAxis = new DataAxis(AxisType.VY);
      LocExtension locvyAxis = new LocExtension("VianaNET:Labels:AxisVelocityYDirection");
      locvyAxis.SetBinding(vyAxis, DescriptionProperty);

      DataAxis aAxis = new DataAxis(AxisType.A);
      LocExtension locaAxis = new LocExtension("VianaNET:Labels:AxisAcceleration");
      locaAxis.SetBinding(aAxis, DescriptionProperty);

      DataAxis axAxis = new DataAxis(AxisType.AX);
      LocExtension locaxAxis = new LocExtension("VianaNET:Labels:AxisAccelerationXDirection");
      locaxAxis.SetBinding(axAxis, DescriptionProperty);

      DataAxis ayAxis = new DataAxis(AxisType.AY);
      LocExtension locayAxis = new LocExtension("VianaNET:Labels:AxisAccelerationYDirection");
      locayAxis.SetBinding(ayAxis, DescriptionProperty);


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
          aAxis,
          axAxis,
          ayAxis,
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





    /// <summary>
    ///   Gets or sets the axis.
    /// </summary>
    public AxisType Axis
    {
      get => (AxisType)this.GetValue(AxisProperty);

      set => this.SetValue(AxisProperty, value);
    }

    /// <summary>
    ///   Gets or sets the description.
    /// </summary>
    public string Description
    {
      get => (string)this.GetValue(DescriptionProperty);

      set => this.SetValue(DescriptionProperty, value);
    }

    /// <summary>
    ///   Gets or sets the description.
    /// </summary>
    public bool ShouldExport
    {
      get => (bool)this.GetValue(ShouldExportProperty);

      set => this.SetValue(ShouldExportProperty, value);
    }





    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    public override string ToString()
    {
      return this.Description;
    }


  }
}