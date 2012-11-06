// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The chart window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Chart
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter;
  using VianaNET.Data.Filter.Regression;
  using VianaNET.Data.Filter.Theory;
  using VianaNET.Localization;
  using VianaNET.Modules.Video.Control;
  using Visifire.Charts;

  using WPFMath;

  /// <summary>
  ///   The chart window.
  /// </summary>
  public partial class ChartWindow
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="ObjectDescriptions" />.
    /// </summary>
    public static readonly DependencyProperty ObjectDescriptionsProperty =
      DependencyProperty.Register(
        "ObjectDescriptions",
        typeof(List<string>),
        typeof(ChartWindow),
        new FrameworkPropertyMetadata(new List<string>(), OnPropertyChanged));

    #endregion

    #region Fields

    /// <summary>
    ///   The is initialized.
    /// </summary>
    private readonly bool isInitialized;

    /// <summary>
    /// Provides a formula parser which reads tex formulas
    /// </summary>
    private readonly TexFormulaParser formulaParser;

    private char achsName = 'x';
    private char funcName = 'y';

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ChartWindow" /> class.
    /// </summary>
    public ChartWindow()
    {
      this.InitializeComponent();
      this.ObjectSelectionCombo.DataContext = this;
      this.PopulateObjectCombo();

      // VideoData.Instance.PropertyChanged +=
      // new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      // Calibration.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      Video.Instance.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.isInitialized = true;
      this.formulaParser = new TexFormulaParser();
      this.UpdateChartProperties();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the index of the currently tracked object
    /// </summary>
    public List<string> ObjectDescriptions
    {
      get
      {
        return (List<string>)this.GetValue(ObjectDescriptionsProperty);
      }

      set
      {
        this.SetValue(ObjectDescriptionsProperty, value);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The refresh.
    /// </summary>
    public void Refresh()
    {
      this.RefreshSeries();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="obj">
    /// The source of the event. This. 
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var window = obj as ChartWindow;
      if (window != null)
      {
        window.RefreshSeries();
      }
    }

    /// <summary>
    /// The chart content tab selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChartContentTabSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.PopulateAxesFromChartSelection();
    }

    /// <summary>
    /// The image processing property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ProcessingDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "NumberOfTrackedObjects")
      {
        this.PopulateObjectCombo();
      }
      else if (e.PropertyName == "IndexOfObject")
      {
        this.Refresh();
      }
    }

    /// <summary>
    /// The interpolation options button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void InterpolationOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      FilterData.Instance.InterpolationFilter.ShowInterpolationOptionsDialog();
      FilterData.Instance.CalculateInterpolationSeriesDataPoints();
    }

    /// <summary>
    /// The line fit options button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void FilterPrecisionButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new NumericalPrecisionDialog { NumberOfDigits = FilterData.Instance.NumericPrecision };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        FilterData.Instance.NumericPrecision = dlg.NumberOfDigits;
        if (this.RegressionCheckBox.IsChecked())
        {
          this.RefreshRegressionFuctionTerm();
          this.RefreshTheorieFunctionTerm();
        }
      }
    }

    /// <summary>
    /// Updates the function term visual with a tex representation of the regression function
    /// </summary>
    private void RefreshRegressionFuctionTerm()
    {
      if (FilterData.Instance.RegressionFunctionTexFormula != null)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        var renderer = FilterData.Instance.RegressionFunctionTexFormula.GetRenderer(TexStyle.Display, 14d);

        using (var drawingContext = visual.RenderOpen())
        {
          renderer.Render(drawingContext, 0, 1);
        }

        this.formulaContainerElement.Visual = visual;
      }
      else
      {
        // Formula is empty
        this.formulaContainerElement.Visual = null;
      }
    }

    /// <summary>
    /// Updates the theoretical term visual with a tex representation of the theoretical function
    /// </summary>
    private void RefreshTheorieFunctionTerm()
    {
      // Only if we have a formula and should display the theory series
      if (FilterData.Instance.TheoryFunctionTexFormula != null && FilterData.Instance.IsShowingTheorySeries)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        var renderer = FilterData.Instance.TheoryFunctionTexFormula.GetRenderer(TexStyle.Display, 14d);

        using (var drawingContext = visual.RenderOpen())
        {
          renderer.Render(drawingContext, 0, 1);
        }

        this.TheorieFormulaContainerElement.Visual = visual;
      }
      else
      {
        // Formula is empty
        this.TheorieFormulaContainerElement.Visual = null;
      }
    }

    /// <summary>
    /// The line fit theorie button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TheoryOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      var fktEditor = new CalculatorAndFktEditor(TRechnerArt.formelRechner);
      if (FilterData.Instance.TheoreticalFunction != null)
      {
        fktEditor.textBox1.Text = FilterData.Instance.TheoreticalFunction.Name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        FilterData.Instance.TheoreticalFunction = fktEditor.GetFunktion();
        this.UpdateTheoryFormula();
      }
    }

    /// <summary>
    /// Updates the LaTex display of the theoretical formula
    /// </summary>
    private void UpdateTheoryFormula()
    {
      try
      {
        var functionString = FilterData.Instance.TheoreticalFunction.Name;
        functionString = functionString.Replace("*", "{\\cdot}");
        functionString = functionString.Replace("(", "{(");
        functionString = functionString.Replace(")", ")}");
        functionString = functionString.Replace('x', this.achsName);
        var formula = this.formulaParser.Parse(functionString);
        if (formula != null)
        {
          FilterData.Instance.TheoryFunctionTexFormula = formula;
        }

        this.RefreshTheorieFunctionTerm();
      }
      catch (Exception)
      {
        FilterData.Instance.TheoryFunctionTexFormula = null;
      }
    }

    /// <summary>
    /// The object selection combo selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ObjectSelectionComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.ObjectSelectionCombo.SelectedItem == null)
      {
        return;
      }

      var entry = (string)this.ObjectSelectionCombo.SelectedItem;
      Video.Instance.ProcessingData.IndexOfObject = int.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
      VideoData.Instance.ActiveObject = Video.Instance.ProcessingData.IndexOfObject;
    }

    /// <summary>
    ///   The populate object combo.
    /// </summary>
    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < Video.Instance.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      // this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      var indexBinding = new Binding("ProcessingData.IndexOfObject") { Source = Video.Instance };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      Video.Instance.ProcessingData.IndexOfObject++;
    }

    /// <summary>
    /// The radio chart style checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RadioChartStyleChecked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (e.Source is RadioButton)
      {
        var checkedRadioButton = e.Source as RadioButton;
        this.UpdateChartStyle(checkedRadioButton);
        this.RefreshSeries();
      }
    }

    /// <summary>
    /// The rechner button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RechnerButtonClick(object sender, RoutedEventArgs e)
    {
      var calculator = new CalculatorAndFktEditor(TRechnerArt.rechner);
      calculator.ShowDialog();
    }

    /// <summary>
    ///   The refresh chart data points.
    /// </summary>
    private void RefreshChartDataPoints()
    {
      this.DefaultSeries.DataSource = null;
      this.DefaultSeries.DataSource = VideoData.Instance.Samples;
      this.UpdateChartProperties();
      this.UpdateFilters();
    }

    /// <summary>
    /// This methods updates the filter series for the currently shown filters
    /// </summary>
    private void UpdateFilters()
    {
      if (FilterData.Instance.IsShowingInterpolationSeries)
      {
        FilterData.Instance.CalculateInterpolationSeriesDataPoints();
      }

      if (FilterData.Instance.IsShowingRegressionSeries)
      {
        FilterData.Instance.CalculateRegressionSeriesDataPoints();
        this.RefreshRegressionFuctionTerm();
      }

      if (FilterData.Instance.IsShowingTheorySeries)
      {
        FilterData.Instance.CalculateTheorySeriesDataPoints();
      }
    }

    /// <summary>
    /// This method refreshes the whole series and chart layout
    /// </summary>
    private void RefreshSeries()
    {
      if (!this.UpdateMapping(true))
      {
        return;
      }

      if (!this.UpdateMapping(false))
      {
        return;
      }

      this.RefreshChartDataPoints();
    }

    /// <summary>
    ///   The set default diagramm.
    /// </summary>
    private void PopulateAxesFromChartSelection()
    {
      if (!this.IsInitialized)
      {
        return;
      }

      var chartType = ChartType.YoverX;
      char achsBez = 'x';
      char funcBez = 'y';

      if (this.TabPositionSpace.IsSelected)
      {
        var chart = (DataCharts)this.AxesContentPositionSpace.SelectedItem;
        chartType = chart.Chart;
      }
      else if (this.TabPhaseSpace.IsSelected)
      {
        var chart = (DataCharts)this.AxesContentPhaseSpace.SelectedItem;
        chartType = chart.Chart;
      }
      else if (this.TabOther.IsSelected)
      {
        var axisX = (DataAxis)this.xAxisContent.SelectedItem;
        var axisY = (DataAxis)this.yAxisContent.SelectedItem;
        this.xAxisContent.SelectedValue = axisX.Axis;
        this.yAxisContent.SelectedValue = axisY.Axis;

        FilterData.Instance.AxisX = axisX;
        FilterData.Instance.AxisY = axisY;
        this.RefreshChartDataPoints();
        // axes content already set, so return
        return;
      }

      switch (chartType)
      {
        case ChartType.YoverX:
          this.xAxisContent.SelectedValue = AxisType.PX;
          this.yAxisContent.SelectedValue = AxisType.PY;
          achsBez = 'x'; funcBez = 'y';
          break;
        case ChartType.XoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.PX;
          achsBez = 't'; funcBez = 'x';
          break;
        case ChartType.YoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.PY;
          achsBez = 't'; funcBez = 'y';
          break;
        case ChartType.VoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.V;
          achsBez = 't'; funcBez = 'v';
          break;
        case ChartType.VXoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.VX;
          achsBez = 't'; funcBez = 'v';
          break;
        case ChartType.VYoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.VY;
          achsBez = 't'; funcBez = 'v';
          break;
        case ChartType.AoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.A;
          achsBez = 't'; funcBez = 'a';
          break;
        case ChartType.AXoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.AX;
          achsBez = 't'; funcBez = 'a';
          break;
        case ChartType.AYoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.AY;
          achsBez = 't'; funcBez = 'a';
          break;
        case ChartType.VoverD:
          this.xAxisContent.SelectedValue = AxisType.D;
          this.yAxisContent.SelectedValue = AxisType.V;
          achsBez = 's'; funcBez = 'v';
          break;
        case ChartType.VXoverDX:
          this.xAxisContent.SelectedValue = AxisType.DX;
          this.yAxisContent.SelectedValue = AxisType.VX;
          achsBez = 's'; funcBez = 'v';
          break;
        case ChartType.VYoverDY:
          this.xAxisContent.SelectedValue = AxisType.DY;
          this.yAxisContent.SelectedValue = AxisType.VY;
          achsBez = 's'; funcBez = 'v';
          break;
        case ChartType.VoverS:
          this.xAxisContent.SelectedValue = AxisType.S;
          this.yAxisContent.SelectedValue = AxisType.V;
          achsBez = 's'; funcBez = 'v';
          break;
        case ChartType.VXoverSX:
          this.xAxisContent.SelectedValue = AxisType.SX;
          this.yAxisContent.SelectedValue = AxisType.VX;
          achsBez = 's'; funcBez = 'v';
          break;
        case ChartType.VYoverSY:
          this.xAxisContent.SelectedValue = AxisType.SY;
          this.yAxisContent.SelectedValue = AxisType.VY;
          achsBez = 's'; funcBez = 'v';
          break;
        case ChartType.AoverV:
          this.xAxisContent.SelectedValue = AxisType.V;
          this.yAxisContent.SelectedValue = AxisType.A;
          achsBez = 'v'; funcBez = 'a';
          break;
        case ChartType.AXoverVX:
          this.xAxisContent.SelectedValue = AxisType.VX;
          this.yAxisContent.SelectedValue = AxisType.AX;
          achsBez = 'v'; funcBez = 'a';
          break;
        case ChartType.AYoverVY:
          this.xAxisContent.SelectedValue = AxisType.VY;
          this.yAxisContent.SelectedValue = AxisType.AY;
          achsBez = 'v'; funcBez = 'a';
          break;
      }
      this.achsName = achsBez;
      this.funcName = funcBez;
      FilterData.Instance.RegressionFilter.SetBezeichnungen(achsBez, funcBez);
      FilterData.Instance.AxisX = (DataAxis)this.xAxisContent.SelectedItem;
      FilterData.Instance.AxisY = (DataAxis)this.yAxisContent.SelectedItem;
      this.RefreshChartDataPoints();
    }

    /// <summary>
    /// Updates the axis mappings for the data display.
    /// This method only updates the data series, because
    /// all other series are reevaluated depending on the
    /// display of the displayed orgininal data
    /// </summary>
    /// <param name="axis">The axis that changed</param>
    /// <param name="mapPoints">The new data mapping. </param>
    private void UpdateAxisMappings(DataAxis axis, DataMapping mapPoints)
    {
      string prefix = "Object[" + Video.Instance.ProcessingData.IndexOfObject.ToString(CultureInfo.InvariantCulture)
                      + "].";
      switch (axis.Axis)
      {
        case AxisType.I:
          mapPoints.Path = "Framenumber";
          break;
        case AxisType.T:
          mapPoints.Path = "Timestamp";
          break;
        case AxisType.PX:
          mapPoints.Path = "PositionX";
          break;
        case AxisType.PY:
          mapPoints.Path = "PositionY";
          break;
        case AxisType.D:
          mapPoints.Path = "Distance";
          break;
        case AxisType.DX:
          mapPoints.Path = "DistanceX";
          break;
        case AxisType.DY:
          mapPoints.Path = "DistanceY";
          break;
        case AxisType.S:
          mapPoints.Path = "Length";
          break;
        case AxisType.SX:
          mapPoints.Path = "LengthX";
          break;
        case AxisType.SY:
          mapPoints.Path = "LengthY";
          break;
        case AxisType.V:
          mapPoints.Path = "Velocity";
          break;
        case AxisType.VX:
          mapPoints.Path = "VelocityX";
          break;
        case AxisType.VY:
          mapPoints.Path = "VelocityY";
          break;
        case AxisType.VI:
          mapPoints.Path = "VelocityI";
          break;
        case AxisType.VXI:
          mapPoints.Path = "VelocityXI";
          break;
        case AxisType.VYI:
          mapPoints.Path = "VelocityYI";
          break;
        case AxisType.A:
          mapPoints.Path = "Acceleration";
          break;
        case AxisType.AX:
          mapPoints.Path = "AccelerationX";
          break;
        case AxisType.AY:
          mapPoints.Path = "AccelerationY";
          break;
        case AxisType.AI:
          mapPoints.Path = "AccelerationI";
          break;
        case AxisType.AXI:
          mapPoints.Path = "AccelerationXI";
          break;
        case AxisType.AYI:
          mapPoints.Path = "AccelerationYI";
          break;
      }

      // Don´t prefix the timestamp
      if (axis.Axis != AxisType.T)
      {
        mapPoints.Path = prefix + mapPoints.Path;
      }
    }

    /// <summary>
    ///   The update chart properties.
    /// </summary>
    private void UpdateChartProperties()
    {
      if (this.isInitialized)
      {
        this.DataChart.Titles[0].Text = this.ChartTitle.IsChecked ? this.ChartTitle.Text : null;
        this.DataChart.Legends[0].Title = this.LegendTitle.IsChecked ? this.LegendTitle.Text : null;
        this.DataChart.Legends[0].Enabled = this.LegendTitle.IsChecked || this.SeriesTitle.IsChecked;
        this.DefaultSeries.LegendText = this.SeriesTitle.IsChecked ? this.SeriesTitle.Text : null;

        if (this.DataChart.AxesX.Count > 0)
        {
          var axisX = this.DataChart.AxesX[0];
          axisX.Title = this.XAxisTitle.IsChecked ? this.XAxisTitle.Text : null;
          axisX.Grids[0].Enabled = this.XAxisShowGridLines.IsChecked();

          if (this.XAxisMinimum.Value > this.XAxisMaximum.Value)
          {
            this.XAxisMinimum.Value = this.XAxisMaximum.Value;
          }

          axisX.AxisMinimum = this.XAxisMinimum.IsChecked ? this.XAxisMinimum.Value : new double?();

          if (this.XAxisMaximum.Value < this.XAxisMinimum.Value)
          {
            this.XAxisMaximum.Value = this.XAxisMinimum.Value;
          }

          axisX.AxisMaximum = this.XAxisMaximum.IsChecked ? this.XAxisMaximum.Value : new double?();

          // if (XAxisInterval.IsChecked)
          // {
          // xAxis.Interval = XAxisInterval.Value;
          // }
          // else
          // {
          // xAxis.Interval = double.NaN;
          // }
        }

        if (this.DataChart.AxesY.Count > 0)
        {
          var axisY = this.DataChart.AxesY[0];
          axisY.Title = this.YAxisTitle.IsChecked ? this.YAxisTitle.Text : null;
          axisY.Grids[0].Enabled = this.YAxisShowGridLines.IsChecked();

          if (this.YAxisMinimum.Value > this.YAxisMaximum.Value)
          {
            this.YAxisMinimum.Value = this.YAxisMaximum.Value;
          }

          axisY.AxisMinimum = this.YAxisMinimum.IsChecked ? this.YAxisMinimum.Value : new double?();

          if (this.YAxisMaximum.Value < this.YAxisMinimum.Value)
          {
            this.YAxisMaximum.Value = this.YAxisMinimum.Value;
          }

          axisY.AxisMaximum = this.YAxisMaximum.IsChecked ? this.YAxisMaximum.Value : new double?();

          // if (YAxisInterval.IsChecked)
          // {
          // yAxis.Interval = YAxisInterval.Value;
          // }
          // else
          // {
          // yAxis.Interval = double.NaN;
          // }
        }
      }
    }

    /// <summary>
    /// The update chart style.
    /// </summary>
    /// <param name="checkedRadioButton">
    /// The checked radio button. 
    /// </param>
    private void UpdateChartStyle(RadioButton checkedRadioButton)
    {
      this.AxisControls.Visibility = Visibility.Visible;
      this.OtherContentGrid.RowDefinitions[0].Height = GridLength.Auto;

      this.RegressionSeries.RenderAs = RenderAs.Line;
      this.TheorySeries.RenderAs = RenderAs.Line;
      RenderAs? filterStyle = null;

      if (checkedRadioButton.Name.Contains("Scatter"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Point;
        filterStyle = RenderAs.Line;
      }
      else if (checkedRadioButton.Name.Contains("Line"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Line;
        filterStyle = RenderAs.Line;
      }
      else if (checkedRadioButton.Name.Contains("Pie"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Pie;
        this.AxisControls.Visibility = Visibility.Hidden;
        this.OtherContentGrid.RowDefinitions[0].Height = new GridLength(0);
      }
      else if (checkedRadioButton.Name.Contains("Column"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Column;
        filterStyle = RenderAs.Line;
      }
      else if (checkedRadioButton.Name.Contains("Bubble"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Bubble;
        filterStyle = RenderAs.Line;
      }
      else if (checkedRadioButton.Name.Contains("Area"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Area;
        filterStyle = RenderAs.Line;
      }

      var enabled = false;
      if (filterStyle.HasValue)
      {
        this.InterpolationSeries.RenderAs = filterStyle.Value;
        this.RegressionSeries.RenderAs = filterStyle.Value;
        this.TheorySeries.RenderAs = filterStyle.Value;
        enabled = true;
      }
      else
      {
        FilterData.Instance.IsShowingInterpolationSeries = false;
        FilterData.Instance.IsShowingRegressionSeries = false;
        FilterData.Instance.IsShowingTheorySeries = false;
      }

      this.InterpolationSeries.Enabled = enabled;
      this.RegressionSeries.Enabled = enabled;
      this.TheorySeries.Enabled = enabled;
    }

    /// <summary>
    /// This method updates the series mappings for the given axis.
    /// </summary>
    /// <param name="axisX">True, if the x axis should be mapped, otherwise for y Axis false.</param>
    /// <returns>A <see cref="bool"/> indicating success</returns>
    private bool UpdateMapping(bool axisX)
    {
      if (!this.isInitialized)
      {
        return false;
      }

      if (this.DataChart.Series.Count == 0)
      {
        return false;
      }

      if (this.DataChart.AxesX.Count == 0)
      {
        return false;
      }

      // Whenever changing the axes, the theory formula will be odd, so hide it
      FilterData.Instance.IsShowingTheorySeries = false;
      this.UpdateTheoryFormula();

      var axis = axisX ? (DataAxis)this.xAxisContent.SelectedItem : (DataAxis)this.yAxisContent.SelectedItem;

      if (axisX)
      {
        if (this.DataChart.AxesX.Count >= 1)
        {
          this.DataChart.AxesX[0].Title = this.XAxisTitle.IsChecked ? axis.Description : null;
          this.XAxisTitle.Text = axis.Description;
        }
      }
      else
      {
        if (this.DataChart.AxesY.Count >= 1)
        {
          this.DataChart.AxesY[0].Title = this.YAxisTitle.IsChecked ? axis.Description : null;
          this.YAxisTitle.Text = axis.Description;
        }
      }

      var map = this.DefaultSeries.DataMappings[axisX ? 0 : 1];
      this.UpdateAxisMappings(axis, map);
      return true;
    }

    /// <summary>
    /// The value changed update chart.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ValueChangedUpdateChart(object sender, EventArgs e)
    {
      this.UpdateChartProperties();
    }

    /// <summary>
    /// The line fit type button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void RegressionTypeButtonClick(object sender, RoutedEventArgs e)
    {
      var regressionOptionsDialog = new RegressionOptionsDialog(FilterData.Instance.RegressionFilter);
      if (regressionOptionsDialog.ShowDialog().GetValueOrDefault(false))
      {
        if (regressionOptionsDialog.RegressionType == RegressionType.Best)
        {
          RegressionType bestRegression;
          if (FilterData.Instance.RegressionFilter.WertX.Count == 0)
          {
              FilterData.Instance.RegressionFilter.CalculateFilterValues(VideoData.Instance.Samples, null);
          } 
          FilterData.Instance.RegressionFilter.GetBestRegressData(out bestRegression);
          this.UpdateRegressionImageButtonAndLabels(bestRegression,false);
        }
        else
        {
          this.UpdateRegressionImageButtonAndLabels(regressionOptionsDialog.RegressionType,true);
        }

      }
    }

    /// <summary>
    /// This method updates the regression button with
    /// an image corresponding to the selected regression type
    /// </summary>
    /// <param name="aktregressionType">
    /// The aktual selected regression type. 
    /// </param>
    private void UpdateRegressionImageButtonAndLabels(RegressionType aktregressionType, bool neuBerechnen)
    {
      string bildsource;
      FilterData.Instance.RegressionFilter.RegressionType = aktregressionType;
      switch (aktregressionType)
      {
        case RegressionType.Linear:
          bildsource = "/VianaNET;component/Images/RegressionLinear16.png";
          break;
        case RegressionType.Exponentiell:
          bildsource = "/VianaNET;component/Images/RegressionExponentialA16.png";
          break;
        case RegressionType.Logarithmisch:
          bildsource = "/VianaNET;component/Images/RegressionLogarithmus16.png";
          break;
        case RegressionType.Potenz:
          bildsource = "/VianaNET;component/Images/RegressionPotentiell16.png";
          break;
        case RegressionType.Quadratisch:
          bildsource = "/VianaNET;component/Images/RegressionQuadratisch16.png";
          break;
        case RegressionType.ExponentiellMitKonstante:
          bildsource = "/VianaNET;component/Images/RegressionExponentialB16.png";
          break;
        case RegressionType.Sinus:
          bildsource = "/VianaNET;component/Images/RegressionSinus16.png";
          break;
        case RegressionType.SinusGedämpft:
          bildsource = "/VianaNET;component/Images/RegressionSinusExponential16.png";
          break;
        case RegressionType.Resonanz:
          bildsource = "/VianaNET;component/Images/RegressionResonanz16.png";
          break;
        default:
          bildsource = "/VianaNET;component/Images/RegressionLinear16.png";
          break;
      }

      var neuBildsource = new Uri(bildsource, UriKind.RelativeOrAbsolute);
      this.RegressionTypeButton.ImageSource = new BitmapImage(neuBildsource);

      if (this.RegressionCheckBox.IsChecked.GetValueOrDefault(false))
      {
        FilterData.Instance.CalculateRegressionSeriesDataPoints();
        FilterData.Instance.RegressionFilter.UpdateLinefitFunctionData(neuBerechnen);
        this.RefreshRegressionFuctionTerm();
          
      }
    }

    /// <summary>
    /// The chart content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ChartContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // This populates the chart combo boxes with the selected x and y Axes
      // During population it updates also the mappings and refreshes the data points
      this.PopulateAxesFromChartSelection();

      //// This updates the xAxis mapping for the data series
      //if (!this.UpdateMapping(true))
      //{
      //  return;
      //}

      //// This updates the yAxis mapping for the data series
      //if (!this.UpdateMapping(false))
      //{
      //  return;
      //}

      //this.RefreshChartDataPoints();
    }

    /// <summary>
    /// The x axis content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void XAxisContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!this.UpdateMapping(true))
      {
        return;
      }
    }

    /// <summary>
    /// The y axis content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void YAxisContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!this.UpdateMapping(false))
      {
        return;
      }
    }

    /// <summary>
    /// The check box show theorie_ checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ShowTheorieCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      FilterData.Instance.IsShowingTheorySeries = true;
      this.UpdateTheoryFormula();
      this.RefreshTheorieFunctionTerm();
    }

    private void ShowTheorieCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      FilterData.Instance.IsShowingTheorySeries = false;
      FilterData.Instance.TheoryFunctionTexFormula = null;
      this.RefreshTheorieFunctionTerm();
    }

    private void ShowRegressionCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      // Funktionsterm und mittleres Fehlerquadrat anzeigen
      FilterData.Instance.IsShowingRegressionSeries = true;
      this.RefreshRegressionFuctionTerm();
    }

    private void ShowRegressionCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      // Funktionsterm und mittleres Fehlerquadrat nicht mehr anzeigen
      FilterData.Instance.IsShowingRegressionSeries = false;
      FilterData.Instance.RegressionFunctionTexFormula = null;
      FilterData.Instance.RegressionAberration = 0;
      this.RefreshRegressionFuctionTerm();
    }

    private void ShowInterpolationCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.InterpolationSeries.Enabled = true; 
      FilterData.Instance.IsShowingInterpolationSeries = true;
    }

    private void ShowInterpolationCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      FilterData.Instance.IsShowingInterpolationSeries = false;
    }

    /// <summary>
    /// The image button regress options_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RegressionStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FilterData.Instance.RegressionLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FilterData.Instance.RegressionLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FilterData.Instance.RegressionLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FilterData.Instance.RegressionLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }

    /// <summary>
    /// The image button regress options_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TheoryStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FilterData.Instance.TheoryLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FilterData.Instance.TheoryLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FilterData.Instance.TheoryLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FilterData.Instance.TheoryLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }

    /// <summary>
    /// The image button regress options_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DataStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FilterData.Instance.DataLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FilterData.Instance.DataLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FilterData.Instance.DataLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FilterData.Instance.DataLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }

    /// <summary>
    /// The image button regress options_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void InterpolationStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FilterData.Instance.InterpolationLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FilterData.Instance.InterpolationLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FilterData.Instance.InterpolationLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FilterData.Instance.InterpolationLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }

    #endregion
  }
}