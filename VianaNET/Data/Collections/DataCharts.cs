// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCharts.cs" company="Freie Universität Berlin">
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
  public class DataCharts : DependencyObject
  {
    #region Static Fields

    /// <summary>
    ///   The axis property.
    /// </summary>
    public static readonly DependencyProperty ChartProperty = DependencyProperty.Register(
      "Chart", 
      typeof(ChartType), 
      typeof(DataCharts), 
      new FrameworkPropertyMetadata(default(ChartType), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The description property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
      "Description", 
      typeof(string),
      typeof(DataCharts), 
      new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The predefined charts in phase space.
    /// </summary>
    public static List<DataCharts> PredefinedDataChartsPhaseSpace;

    /// <summary>
    ///   The predefined charts in position space.
    /// </summary>
    public static List<DataCharts> PredefinedDataChartsPositionSpace;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="DataAxis" /> class.
    /// </summary>
    static DataCharts()
    {
      // Double axis data position space
      var YoverX = new DataCharts(ChartType.YoverX);
      var locYoverX = new LocExtension("VianaNET:Labels:AxisYoverX");
      locYoverX.SetBinding(YoverX, DescriptionProperty);
      var XoverT = new DataCharts(ChartType.XoverT);
      var locXoverT = new LocExtension("VianaNET:Labels:AxisXoverT");
      locXoverT.SetBinding(XoverT, DescriptionProperty);
      var YoverT = new DataCharts(ChartType.YoverT);
      var locYoverT = new LocExtension("VianaNET:Labels:AxisYoverT");
      locYoverT.SetBinding(YoverT, DescriptionProperty);
      var SoverT = new DataCharts(ChartType.SoverT);
      var locSoverT = new LocExtension("VianaNET:Labels:AxisSoverT");
      locSoverT.SetBinding(SoverT, DescriptionProperty);
      var VoverT = new DataCharts(ChartType.VoverT);
      var locVoverT = new LocExtension("VianaNET:Labels:AxisVoverT");
      locVoverT.SetBinding(VoverT, DescriptionProperty);
      var VXoverT = new DataCharts(ChartType.VXoverT);
      var locVXoverT = new LocExtension("VianaNET:Labels:AxisVXoverT");
      locVXoverT.SetBinding(VXoverT, DescriptionProperty);
      var VYoverT = new DataCharts(ChartType.VYoverT);
      var locVYoverT = new LocExtension("VianaNET:Labels:AxisVYoverT");
      locVYoverT.SetBinding(VYoverT, DescriptionProperty);
      var AoverT = new DataCharts(ChartType.AoverT);
      var locAoverT = new LocExtension("VianaNET:Labels:AxisAoverT");
      locAoverT.SetBinding(AoverT, DescriptionProperty);
      var AXoverT = new DataCharts(ChartType.AXoverT);
      var locAXoverT = new LocExtension("VianaNET:Labels:AxisAXoverT");
      locAXoverT.SetBinding(AXoverT, DescriptionProperty);
      var AYoverT = new DataCharts(ChartType.AYoverT);
      var locAYoverT = new LocExtension("VianaNET:Labels:AxisAYoverT");
      locAYoverT.SetBinding(AYoverT, DescriptionProperty);

      PredefinedDataChartsPositionSpace = new List<DataCharts>();
      PredefinedDataChartsPositionSpace.Add(YoverX);
      PredefinedDataChartsPositionSpace.Add(XoverT);
      PredefinedDataChartsPositionSpace.Add(YoverT);
      PredefinedDataChartsPositionSpace.Add(VoverT);
      PredefinedDataChartsPositionSpace.Add(VXoverT);
      PredefinedDataChartsPositionSpace.Add(VYoverT);
      PredefinedDataChartsPositionSpace.Add(AoverT);
      PredefinedDataChartsPositionSpace.Add(AXoverT);
      PredefinedDataChartsPositionSpace.Add(AYoverT);

      // Double axis data phase space
      var VoverD = new DataCharts(ChartType.VoverD);
      var locDoverV = new LocExtension("VianaNET:Labels:AxisDoverV");
      locDoverV.SetBinding(VoverD, DescriptionProperty);
      var VXoverDX = new DataCharts(ChartType.VXoverDX);
      var locDXoverVX = new LocExtension("VianaNET:Labels:AxisDXoverVX");
      locDXoverVX.SetBinding(VXoverDX, DescriptionProperty);
      var VYoverDY = new DataCharts(ChartType.VYoverDY);
      var locDYoverVY = new LocExtension("VianaNET:Labels:AxisDYoverVY");
      locDYoverVY.SetBinding(VYoverDY, DescriptionProperty);
      var VoverS = new DataCharts(ChartType.VoverS);
      var locSoverV = new LocExtension("VianaNET:Labels:AxisSoverV");
      locSoverV.SetBinding(VoverS, DescriptionProperty);
      var VXoverSX = new DataCharts(ChartType.VXoverSX);
      var locSXoverVX = new LocExtension("VianaNET:Labels:AxisSXoverVX");
      locSXoverVX.SetBinding(VXoverSX, DescriptionProperty);
      var VYoverSY = new DataCharts(ChartType.VYoverSY);
      var locSYoverVY = new LocExtension("VianaNET:Labels:AxisSYoverVY");
      locSYoverVY.SetBinding(VYoverSY, DescriptionProperty);
      var AoverV = new DataCharts(ChartType.AoverV);
      var locVoverA = new LocExtension("VianaNET:Labels:AxisVoverA");
      locVoverA.SetBinding(AoverV, DescriptionProperty);
      var AXoverVX = new DataCharts(ChartType.AXoverVX);
      var locVXoverAX = new LocExtension("VianaNET:Labels:AxisVXoverAX");
      locVXoverAX.SetBinding(AXoverVX, DescriptionProperty);
      var AYoverVY = new DataCharts(ChartType.AYoverVY);
      var locVYoverAY = new LocExtension("VianaNET:Labels:AxisVYoverAY");
      locVYoverAY.SetBinding(AYoverVY, DescriptionProperty);

      PredefinedDataChartsPhaseSpace = new List<DataCharts>();
      PredefinedDataChartsPhaseSpace.Add(VoverD);
      PredefinedDataChartsPhaseSpace.Add(VXoverDX);
      PredefinedDataChartsPhaseSpace.Add(VYoverDY);
      PredefinedDataChartsPhaseSpace.Add(VoverS);
      PredefinedDataChartsPhaseSpace.Add(VXoverSX);
      PredefinedDataChartsPhaseSpace.Add(VYoverSY);
      PredefinedDataChartsPhaseSpace.Add(AoverV);
      PredefinedDataChartsPhaseSpace.Add(AXoverVX);
      PredefinedDataChartsPhaseSpace.Add(AYoverVY);
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="DataCharts" /> class.
    /// </summary>
    public DataCharts()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataCharts"/> class.
    /// </summary>
    /// <param name="chart">
    /// The chart type. 
    /// </param>
    public DataCharts(ChartType chart)
    {
      this.Chart = chart;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the chart.
    /// </summary>
    public ChartType Chart
    {
      get
      {
        return (ChartType)this.GetValue(ChartProperty);
      }

      set
      {
        this.SetValue(ChartProperty, value);
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