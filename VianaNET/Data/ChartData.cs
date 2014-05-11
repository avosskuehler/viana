// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartData.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data
{
  using System;
  using System.Reflection;

  using OxyPlot;
  using OxyPlot.Axes;
  using OxyPlot.Series;

  using VianaNET.Application;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter;
  using VianaNET.Resources;

  /// <summary>
  ///   This class contains the view model for the charting module.
  /// </summary>
  [Serializable]
  public class ChartData
  {

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ChartData" /> class.
    /// </summary>
    public ChartData()
    {
      this.ChartDataModel = new PlotModel();
      this.ChartDataModel.Title = Labels.ChartWindowChartTitle;
      this.ChartDataModel.LegendPlacement = LegendPlacement.Inside;
      this.ChartDataModel.LegendSymbolLength = 24;
      this.ChartDataModel.LegendBackground = OxyColor.FromArgb(200, 255, 255, 255);
      this.ChartDataModel.LegendBorder = OxyColors.LightGray;

      this.DefaultSeries = new LineSeries();
      this.InterpolationSeries = new LineSeries();
      this.RegressionSeries = new LineSeries();
      this.TheorySeries = new LineSeries();

      this.XAxis = new LinearAxis();
      this.XAxis.Position = AxisPosition.Bottom;
      this.XAxis.ExtraGridlines = new double[1];
      this.XAxis.ExtraGridlines[0] = 0;
      //this.XAxis.PositionAtZeroCrossing = true;
      //this.XAxis.TickStyle = TickStyle.Crossing;
      this.XAxis.MaximumPadding = 0.05;
      this.XAxis.MinimumPadding = 0.05;

      this.YAxis = new LinearAxis();
      this.YAxis.Position = AxisPosition.Left;
      this.YAxis.ExtraGridlines = new double[1];
      this.YAxis.ExtraGridlines[0] = 0;
      //this.YAxis.PositionAtZeroCrossing = true;
      //this.YAxis.TickStyle = TickStyle.Crossing;
      this.YAxis.MaximumPadding = 0.05;
      this.YAxis.MinimumPadding = 0.05;

      this.ChartDataModel.Series.Add(this.DefaultSeries);
      this.ChartDataModel.Series.Add(this.InterpolationSeries);
      this.ChartDataModel.Series.Add(this.RegressionSeries);
      this.ChartDataModel.Series.Add(this.TheorySeries);
      this.ChartDataModel.Axes.Add(this.XAxis);
      this.ChartDataModel.Axes.Add(this.YAxis);

      // Property info of target object
      this.DefaultSeries.Mapping =
        item => new DataPoint(GetTargetValue(item, "PositionX"), GetTargetValue(item, "PositionY"));
      this.InterpolationSeries.Mapping = item => new DataPoint(((XYSample)item).ValueX, ((XYSample)item).ValueY);
      this.RegressionSeries.Mapping = item => new DataPoint(((XYSample)item).ValueX, ((XYSample)item).ValueY);
      this.TheorySeries.Mapping = item => new DataPoint(((XYSample)item).ValueX, ((XYSample)item).ValueY);

      this.DefaultSeries.SelectionMode = SelectionMode.Multiple;

      this.UpdateModel();
      this.UpdateAppearance();

      Viana.Project.CurrentFilterData.PropertyChanged += this.CurrentFilterDataPropertyChanged;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the chart data model.
    /// </summary>
    /// <value>
    ///   The chart data model.
    /// </value>
    public PlotModel ChartDataModel { get; private set; }

    /// <summary>
    ///   Gets or sets the default series.
    /// </summary>
    /// <value>
    ///   The default series.
    /// </value>
    public LineSeries DefaultSeries { get; set; }

    /// <summary>
    ///   Gets or sets the interpolation series.
    /// </summary>
    /// <value>
    ///   The interpolation series.
    /// </value>
    public LineSeries InterpolationSeries { get; set; }

    /// <summary>
    ///   Gets or sets the regression series.
    /// </summary>
    /// <value>
    ///   The regression series.
    /// </value>
    public LineSeries RegressionSeries { get; set; }

    /// <summary>
    ///   Gets or sets the theory series.
    /// </summary>
    /// <value>
    ///   The theory series.
    /// </value>
    public LineSeries TheorySeries { get; set; }

    /// <summary>
    ///   Gets or sets the x axis.
    /// </summary>
    /// <value>
    ///   The x axis.
    /// </value>
    public LinearAxis XAxis { get; set; }

    /// <summary>
    ///   Gets or sets the y axis.
    /// </summary>
    /// <value>
    ///   The y axis.
    /// </value>
    public LinearAxis YAxis { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Updates the model.
    /// </summary>
    public void UpdateModel()
    {
      this.DefaultSeries.ItemsSource = Viana.Project.VideoData.Samples;
      this.InterpolationSeries.ItemsSource = Viana.Project.CurrentFilterData.InterpolationSeries;
      this.RegressionSeries.ItemsSource = Viana.Project.CurrentFilterData.RegressionSeries;
      this.TheorySeries.ItemsSource = Viana.Project.CurrentFilterData.TheorySeries;
      this.InterpolationSeries.IsVisible = Viana.Project.CurrentFilterData.IsShowingInterpolationSeries;
      this.RegressionSeries.IsVisible = Viana.Project.CurrentFilterData.IsShowingRegressionSeries;
      this.TheorySeries.IsVisible = Viana.Project.CurrentFilterData.IsShowingTheorySeries;
      this.ChartDataModel.InvalidatePlot(true);
    }

    public void UpdateAppearance()
    {
      this.DefaultSeries.Title = Labels.ChartWindowDataSeriesLabel;
      this.DefaultSeries.MarkerSize = Viana.Project.CurrentFilterData.DataLineThickness + 2;
      this.DefaultSeries.MarkerFill = Viana.Project.CurrentFilterData.DataLineColor;
      this.SetMarkerStroke(Viana.Project.CurrentFilterData.DataLineMarkerType, this.DefaultSeries);
      this.DefaultSeries.MarkerType = Viana.Project.CurrentFilterData.DataLineMarkerType;
      this.DefaultSeries.Color = Viana.Project.CurrentFilterData.DataLineColor;
      this.DefaultSeries.StrokeThickness = Viana.Project.CurrentFilterData.DataLineThickness;

      this.InterpolationSeries.Title = Labels.ChartWindowInterpolationSeriesLabel;
      this.InterpolationSeries.MarkerSize = Viana.Project.CurrentFilterData.InterpolationLineThickness + 2;
      this.InterpolationSeries.MarkerFill = Viana.Project.CurrentFilterData.InterpolationLineColor;
      this.SetMarkerStroke(Viana.Project.CurrentFilterData.InterpolationLineMarkerType, this.InterpolationSeries);
      this.InterpolationSeries.MarkerType = Viana.Project.CurrentFilterData.InterpolationLineMarkerType;
      this.InterpolationSeries.Color = Viana.Project.CurrentFilterData.InterpolationLineColor;
      this.InterpolationSeries.StrokeThickness = Viana.Project.CurrentFilterData.InterpolationLineThickness;

      this.RegressionSeries.Title = Labels.ChartWindowRegressionSeriesLabel;
      this.RegressionSeries.MarkerSize = Viana.Project.CurrentFilterData.RegressionLineThickness + 2;
      this.RegressionSeries.MarkerFill = Viana.Project.CurrentFilterData.RegressionLineColor;
      this.SetMarkerStroke(Viana.Project.CurrentFilterData.RegressionLineMarkerType, this.RegressionSeries);
      this.RegressionSeries.MarkerType = Viana.Project.CurrentFilterData.RegressionLineMarkerType;
      this.RegressionSeries.Color = Viana.Project.CurrentFilterData.RegressionLineColor;
      this.RegressionSeries.StrokeThickness = Viana.Project.CurrentFilterData.RegressionLineThickness;

      this.TheorySeries.Title = Labels.ChartWindowTheorySeriesLabel;
      this.TheorySeries.MarkerSize = Viana.Project.CurrentFilterData.TheoryLineThickness + 2;
      this.TheorySeries.MarkerFill = Viana.Project.CurrentFilterData.TheoryLineColor;
      this.SetMarkerStroke(Viana.Project.CurrentFilterData.TheoryLineMarkerType, this.TheorySeries);
      this.TheorySeries.MarkerType = Viana.Project.CurrentFilterData.TheoryLineMarkerType;
      this.TheorySeries.Color = Viana.Project.CurrentFilterData.TheoryLineColor;
      this.TheorySeries.StrokeThickness = Viana.Project.CurrentFilterData.TheoryLineThickness;

      this.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Sets the marker stroke for the given line series.
    /// </summary>
    /// <param name="type">The type to be set.</param>
    /// <param name="series">The series to set the marker type for</param>
    private void SetMarkerStroke(MarkerType type, LineSeries series)
    {
      switch (type)
      {
        case MarkerType.Circle:
        case MarkerType.Square:
        case MarkerType.Diamond:
        case MarkerType.Triangle:
          series.MarkerStroke = OxyColors.White;
          break;
        case MarkerType.Cross:
        case MarkerType.Plus:
        case MarkerType.Star:
        case MarkerType.Custom:
          series.MarkerStroke = OxyColors.Black;
          break;
      }

      series.MarkerStrokeThickness = 1.5;
    }

    /// <summary>
    /// Updates the default series mapping with the properties given
    /// </summary>
    /// <param name="propertyX">
    /// The property for the x-axis.
    /// </param>
    /// <param name="propertyY">
    /// The property for the y-axis.
    /// </param>
    public void UpdateDefaultSeriesMapping(string propertyX, string propertyY)
    {
      this.DefaultSeries.Mapping =
        item => new DataPoint(GetTargetValue(item, propertyX), GetTargetValue(item, propertyY));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Current filter data property changed.
    /// We need to update unbound model properties
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void CurrentFilterDataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      this.UpdateModel();
    }

    /// <summary>
    /// Gets the target value.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="propertyString">
    /// The property string.
    /// </param>
    /// <returns>
    /// The <see cref="double"/>.
    /// </returns>
    private static double GetTargetValue(object item, string propertyString)
    {
      PropertyInfo objectPropertyInfo = typeof(TimeSample).GetProperty("Object");
      PropertyInfo targetPropertyInfo = typeof(DataSample).GetProperty(propertyString);
      var propertyValue = (object[])objectPropertyInfo.GetValue(item);
      object test = propertyValue[Viana.Project.ProcessingData.IndexOfObject];
      object result = targetPropertyInfo.GetValue(test);
      return result != null ? (double)result : 0;
    }

    #endregion
  }
}