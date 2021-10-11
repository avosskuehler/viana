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





    /// <summary>
    ///   Initializes static members of the <see cref="DataAxis" /> class.
    /// </summary>
    static DataCharts()
    {
      // Double axis data position space
      DataCharts YoverX = new DataCharts(ChartType.YoverX);
      LocExtension locYoverX = new LocExtension("VianaNET:Labels:AxisYoverX");
      locYoverX.SetBinding(YoverX, DescriptionProperty);
      DataCharts XoverT = new DataCharts(ChartType.XoverT);
      LocExtension locXoverT = new LocExtension("VianaNET:Labels:AxisXoverT");
      locXoverT.SetBinding(XoverT, DescriptionProperty);
      DataCharts YoverT = new DataCharts(ChartType.YoverT);
      LocExtension locYoverT = new LocExtension("VianaNET:Labels:AxisYoverT");
      locYoverT.SetBinding(YoverT, DescriptionProperty);
      DataCharts SoverT = new DataCharts(ChartType.SoverT);
      LocExtension locSoverT = new LocExtension("VianaNET:Labels:AxisSoverT");
      locSoverT.SetBinding(SoverT, DescriptionProperty);
      DataCharts VoverT = new DataCharts(ChartType.VoverT);
      LocExtension locVoverT = new LocExtension("VianaNET:Labels:AxisVoverT");
      locVoverT.SetBinding(VoverT, DescriptionProperty);
      DataCharts VXoverT = new DataCharts(ChartType.VXoverT);
      LocExtension locVXoverT = new LocExtension("VianaNET:Labels:AxisVXoverT");
      locVXoverT.SetBinding(VXoverT, DescriptionProperty);
      DataCharts VYoverT = new DataCharts(ChartType.VYoverT);
      LocExtension locVYoverT = new LocExtension("VianaNET:Labels:AxisVYoverT");
      locVYoverT.SetBinding(VYoverT, DescriptionProperty);
      DataCharts AoverT = new DataCharts(ChartType.AoverT);
      LocExtension locAoverT = new LocExtension("VianaNET:Labels:AxisAoverT");
      locAoverT.SetBinding(AoverT, DescriptionProperty);
      DataCharts AXoverT = new DataCharts(ChartType.AXoverT);
      LocExtension locAXoverT = new LocExtension("VianaNET:Labels:AxisAXoverT");
      locAXoverT.SetBinding(AXoverT, DescriptionProperty);
      DataCharts AYoverT = new DataCharts(ChartType.AYoverT);
      LocExtension locAYoverT = new LocExtension("VianaNET:Labels:AxisAYoverT");
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
      DataCharts VoverD = new DataCharts(ChartType.VoverD);
      LocExtension locDoverV = new LocExtension("VianaNET:Labels:AxisDoverV");
      locDoverV.SetBinding(VoverD, DescriptionProperty);
      DataCharts VXoverDX = new DataCharts(ChartType.VXoverDX);
      LocExtension locDXoverVX = new LocExtension("VianaNET:Labels:AxisDXoverVX");
      locDXoverVX.SetBinding(VXoverDX, DescriptionProperty);
      DataCharts VYoverDY = new DataCharts(ChartType.VYoverDY);
      LocExtension locDYoverVY = new LocExtension("VianaNET:Labels:AxisDYoverVY");
      locDYoverVY.SetBinding(VYoverDY, DescriptionProperty);
      DataCharts VoverS = new DataCharts(ChartType.VoverS);
      LocExtension locSoverV = new LocExtension("VianaNET:Labels:AxisSoverV");
      locSoverV.SetBinding(VoverS, DescriptionProperty);
      DataCharts VXoverSX = new DataCharts(ChartType.VXoverSX);
      LocExtension locSXoverVX = new LocExtension("VianaNET:Labels:AxisSXoverVX");
      locSXoverVX.SetBinding(VXoverSX, DescriptionProperty);
      DataCharts VYoverSY = new DataCharts(ChartType.VYoverSY);
      LocExtension locSYoverVY = new LocExtension("VianaNET:Labels:AxisSYoverVY");
      locSYoverVY.SetBinding(VYoverSY, DescriptionProperty);
      DataCharts AoverV = new DataCharts(ChartType.AoverV);
      LocExtension locVoverA = new LocExtension("VianaNET:Labels:AxisVoverA");
      locVoverA.SetBinding(AoverV, DescriptionProperty);
      DataCharts AXoverVX = new DataCharts(ChartType.AXoverVX);
      LocExtension locVXoverAX = new LocExtension("VianaNET:Labels:AxisVXoverAX");
      locVXoverAX.SetBinding(AXoverVX, DescriptionProperty);
      DataCharts AYoverVY = new DataCharts(ChartType.AYoverVY);
      LocExtension locVYoverAY = new LocExtension("VianaNET:Labels:AxisVYoverAY");
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





    /// <summary>
    ///   Gets or sets the chart.
    /// </summary>
    public ChartType Chart
    {
      get => (ChartType)this.GetValue(ChartProperty);

      set => this.SetValue(ChartProperty, value);
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
    ///   The to string.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    public override string ToString()
    {
      return this.Description;
    }


  }
}