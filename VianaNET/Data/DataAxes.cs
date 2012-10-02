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
namespace VianaNET.Data
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

    /// <summary>
    ///   The predefined chart axes phase space.
    /// </summary>
    public static List<DataAxis> PredefinedChartAxesPhaseSpace;

    /// <summary>
    ///   The predefined chart axes position space.
    /// </summary>
    public static List<DataAxis> PredefinedChartAxesPositionSpace;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="DataAxis" /> class.
    /// </summary>
    static DataAxis()
    {
      // Single axis data
      var iAxis = new DataAxis(AxisType.I);
      var lociAxis = new LocTextExtension("VianaNET:Labels:AxisFrame");
      lociAxis.SetBinding(iAxis, DescriptionProperty);

      var tAxis = new DataAxis(AxisType.T);
      var loctAxis = new LocTextExtension("VianaNET:Labels:AxisTime");
      loctAxis.SetBinding(tAxis, DescriptionProperty);

      var xAxis = new DataAxis(AxisType.X);
      var locxAxis = new LocTextExtension("VianaNET:Labels:AxisPixelX");
      locxAxis.SetBinding(xAxis, DescriptionProperty);

      var yAxis = new DataAxis(AxisType.Y);
      var locyAxis = new LocTextExtension("VianaNET:Labels:AxisPixelY");
      locyAxis.SetBinding(yAxis, DescriptionProperty);

      var pxAxis = new DataAxis(AxisType.PX);
      var locpxAxis = new LocTextExtension("VianaNET:Labels:AxisPositionX");
      locpxAxis.SetBinding(pxAxis, DescriptionProperty);

      var pyAxis = new DataAxis(AxisType.PY);
      var locpyAxis = new LocTextExtension("VianaNET:Labels:AxisPositionY");
      locpyAxis.SetBinding(pyAxis, DescriptionProperty);

      var dAxis = new DataAxis(AxisType.D);
      var locdAxis = new LocTextExtension("VianaNET:Labels:AxisDistance");
      locdAxis.SetBinding(dAxis, DescriptionProperty);

      var dxAxis = new DataAxis(AxisType.DX);
      var locdxAxis = new LocTextExtension("VianaNET:Labels:AxisDistanceX");
      locdxAxis.SetBinding(dxAxis, DescriptionProperty);

      var dyAxis = new DataAxis(AxisType.DY);
      var locdyAxis = new LocTextExtension("VianaNET:Labels:AxisDistanceY");
      locdyAxis.SetBinding(dyAxis, DescriptionProperty);

      var sAxis = new DataAxis(AxisType.S);
      var locsAxis = new LocTextExtension("VianaNET:Labels:AxisLength");
      locsAxis.SetBinding(sAxis, DescriptionProperty);

      var sxAxis = new DataAxis(AxisType.SX);
      var locsxAxis = new LocTextExtension("VianaNET:Labels:AxisLengthX");
      locsxAxis.SetBinding(sxAxis, DescriptionProperty);

      var syAxis = new DataAxis(AxisType.SY);
      var locsyAxis = new LocTextExtension("VianaNET:Labels:AxisLengthY");
      locsyAxis.SetBinding(syAxis, DescriptionProperty);

      var vAxis = new DataAxis(AxisType.V);
      var locvAxis = new LocTextExtension("VianaNET:Labels:AxisVelocity");
      locvAxis.SetBinding(vAxis, DescriptionProperty);

      var vxAxis = new DataAxis(AxisType.VX);
      var locvxAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityXDirection");
      locvxAxis.SetBinding(vxAxis, DescriptionProperty);

      var vyAxis = new DataAxis(AxisType.VY);
      var locvyAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityYDirection");
      locvyAxis.SetBinding(vyAxis, DescriptionProperty);

      var viAxis = new DataAxis(AxisType.VI);
      var locviAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityInterpolated");
      locviAxis.SetBinding(viAxis, DescriptionProperty);

      var vxiAxis = new DataAxis(AxisType.VXI);
      var locvxiAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityXDirectionInterpolated");
      locvxiAxis.SetBinding(vxiAxis, DescriptionProperty);

      var vyiAxis = new DataAxis(AxisType.VYI);
      var locvyiAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityYDirectionInterpolated");
      locvyiAxis.SetBinding(vyiAxis, DescriptionProperty);

      var aAxis = new DataAxis(AxisType.A);
      var locaAxis = new LocTextExtension("VianaNET:Labels:AxisAcceleration");
      locaAxis.SetBinding(aAxis, DescriptionProperty);

      var axAxis = new DataAxis(AxisType.AX);
      var locaxAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationXDirection");
      locaxAxis.SetBinding(axAxis, DescriptionProperty);

      var ayAxis = new DataAxis(AxisType.AY);
      var locayAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationYDirection");
      locayAxis.SetBinding(ayAxis, DescriptionProperty);

      var aiAxis = new DataAxis(AxisType.AI);
      var locaiAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationInterpolated");
      locaiAxis.SetBinding(aiAxis, DescriptionProperty);

      var axiAxis = new DataAxis(AxisType.AXI);
      var locaxiAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationXDirectionInterpolated");
      locaxiAxis.SetBinding(axiAxis, DescriptionProperty);

      var ayiAxis = new DataAxis(AxisType.AYI);
      var locayiAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationYDirectionInterpolated");
      locayiAxis.SetBinding(ayiAxis, DescriptionProperty);

      DataAxes = new List<DataAxis>();
      DataAxes.Add(iAxis);
      DataAxes.Add(tAxis);
      DataAxes.Add(xAxis);
      DataAxes.Add(yAxis);
      DataAxes.Add(pxAxis);
      DataAxes.Add(pyAxis);
      DataAxes.Add(dAxis);
      DataAxes.Add(dxAxis);
      DataAxes.Add(dyAxis);
      DataAxes.Add(sAxis);
      DataAxes.Add(sxAxis);
      DataAxes.Add(syAxis);
      DataAxes.Add(vAxis);
      DataAxes.Add(vxAxis);
      DataAxes.Add(vyAxis);
      DataAxes.Add(viAxis);
      DataAxes.Add(vxiAxis);
      DataAxes.Add(vyiAxis);
      DataAxes.Add(aAxis);
      DataAxes.Add(axAxis);
      DataAxes.Add(ayAxis);
      DataAxes.Add(aiAxis);
      DataAxes.Add(axiAxis);
      DataAxes.Add(ayiAxis);

      // Double axis data position space
      var YoverX = new DataAxis(AxisType.YoverX);
      var locYoverX = new LocTextExtension("VianaNET:Labels:AxisYoverX");
      locYoverX.SetBinding(YoverX, DescriptionProperty);
      var XoverT = new DataAxis(AxisType.XoverT);
      var locXoverT = new LocTextExtension("VianaNET:Labels:AxisXoverT");
      locXoverT.SetBinding(XoverT, DescriptionProperty);
      var YoverT = new DataAxis(AxisType.YoverT);
      var locYoverT = new LocTextExtension("VianaNET:Labels:AxisYoverT");
      locYoverT.SetBinding(YoverT, DescriptionProperty);
      var SoverT = new DataAxis(AxisType.SoverT);
      var locSoverT = new LocTextExtension("VianaNET:Labels:AxisSoverT");
      locSoverT.SetBinding(SoverT, DescriptionProperty);
      var VoverT = new DataAxis(AxisType.VoverT);
      var locVoverT = new LocTextExtension("VianaNET:Labels:AxisVoverT");
      locVoverT.SetBinding(VoverT, DescriptionProperty);
      var VXoverT = new DataAxis(AxisType.VXoverT);
      var locVXoverT = new LocTextExtension("VianaNET:Labels:AxisVXoverT");
      locVXoverT.SetBinding(VXoverT, DescriptionProperty);
      var VYoverT = new DataAxis(AxisType.VYoverT);
      var locVYoverT = new LocTextExtension("VianaNET:Labels:AxisVYoverT");
      locVYoverT.SetBinding(VYoverT, DescriptionProperty);
      var AoverT = new DataAxis(AxisType.AoverT);
      var locAoverT = new LocTextExtension("VianaNET:Labels:AxisAoverT");
      locAoverT.SetBinding(AoverT, DescriptionProperty);
      var AXoverT = new DataAxis(AxisType.AXoverT);
      var locAXoverT = new LocTextExtension("VianaNET:Labels:AxisAXoverT");
      locAXoverT.SetBinding(AXoverT, DescriptionProperty);
      var AYoverT = new DataAxis(AxisType.AYoverT);
      var locAYoverT = new LocTextExtension("VianaNET:Labels:AxisAYoverT");
      locAYoverT.SetBinding(AYoverT, DescriptionProperty);

      PredefinedChartAxesPositionSpace = new List<DataAxis>();
      PredefinedChartAxesPositionSpace.Add(YoverX);
      PredefinedChartAxesPositionSpace.Add(XoverT);
      PredefinedChartAxesPositionSpace.Add(YoverT);
      PredefinedChartAxesPositionSpace.Add(VoverT);
      PredefinedChartAxesPositionSpace.Add(VXoverT);
      PredefinedChartAxesPositionSpace.Add(VYoverT);
      PredefinedChartAxesPositionSpace.Add(AoverT);
      PredefinedChartAxesPositionSpace.Add(AXoverT);
      PredefinedChartAxesPositionSpace.Add(AYoverT);

      // Double axis data phase space
      var VoverD = new DataAxis(AxisType.VoverD);
      var locDoverV = new LocTextExtension("VianaNET:Labels:AxisDoverV");
      locDoverV.SetBinding(VoverD, DescriptionProperty);
      var VXoverDX = new DataAxis(AxisType.VXoverDX);
      var locDXoverVX = new LocTextExtension("VianaNET:Labels:AxisDXoverVX");
      locDXoverVX.SetBinding(VXoverDX, DescriptionProperty);
      var VYoverDY = new DataAxis(AxisType.VYoverDY);
      var locDYoverVY = new LocTextExtension("VianaNET:Labels:AxisDYoverVY");
      locDYoverVY.SetBinding(VYoverDY, DescriptionProperty);
      var VoverS = new DataAxis(AxisType.VoverS);
      var locSoverV = new LocTextExtension("VianaNET:Labels:AxisSoverV");
      locSoverV.SetBinding(VoverS, DescriptionProperty);
      var VXoverSX = new DataAxis(AxisType.VXoverSX);
      var locSXoverVX = new LocTextExtension("VianaNET:Labels:AxisSXoverVX");
      locSXoverVX.SetBinding(VXoverSX, DescriptionProperty);
      var VYoverSY = new DataAxis(AxisType.VYoverSY);
      var locSYoverVY = new LocTextExtension("VianaNET:Labels:AxisSYoverVY");
      locSYoverVY.SetBinding(VYoverSY, DescriptionProperty);
      var AoverV = new DataAxis(AxisType.AoverV);
      var locVoverA = new LocTextExtension("VianaNET:Labels:AxisVoverA");
      locVoverA.SetBinding(AoverV, DescriptionProperty);
      var AXoverVX = new DataAxis(AxisType.AXoverVX);
      var locVXoverAX = new LocTextExtension("VianaNET:Labels:AxisVXoverAX");
      locVXoverAX.SetBinding(AXoverVX, DescriptionProperty);
      var AYoverVY = new DataAxis(AxisType.AYoverVY);
      var locVYoverAY = new LocTextExtension("VianaNET:Labels:AxisVYoverAY");
      locVYoverAY.SetBinding(AYoverVY, DescriptionProperty);

      PredefinedChartAxesPhaseSpace = new List<DataAxis>();
      PredefinedChartAxesPhaseSpace.Add(VoverD);
      PredefinedChartAxesPhaseSpace.Add(VXoverDX);
      PredefinedChartAxesPhaseSpace.Add(VYoverDY);
      PredefinedChartAxesPhaseSpace.Add(VoverS);
      PredefinedChartAxesPhaseSpace.Add(VXoverSX);
      PredefinedChartAxesPhaseSpace.Add(VYoverSY);
      PredefinedChartAxesPhaseSpace.Add(AoverV);
      PredefinedChartAxesPhaseSpace.Add(AXoverVX);
      PredefinedChartAxesPhaseSpace.Add(AYoverVY);
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