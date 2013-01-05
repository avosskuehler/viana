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

  using Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter;
  using VianaNET.Data.Filter.Regression;
  using VianaNET.Data.Filter.Theory;
  using VianaNET.Localization;
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

    /// <summary>
    /// A character for the axis name
    /// </summary>
    private char axisName = 'x';

    /// <summary>
    /// The axis unit name.
    /// </summary>
    private string axisUnitName;

    /// <summary>
    /// The x-axis unit name.
    /// </summary>
    private string axisXUnitName;

    /// <summary>
    /// The y-axis unit name.
    /// </summary>
    private string axisYUnitName;

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

      // Project.Instance.VideoData.PropertyChanged +=
      // new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      // Calibration.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      Project.Instance.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
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
      Project.Instance.FilterData.InterpolationFilter.ShowInterpolationOptionsDialog();
      Project.Instance.FilterData.CalculateInterpolationSeriesDataPoints();
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
      var dlg = new NumericalPrecisionDialog { NumberOfDigits = Project.Instance.FilterData.NumericPrecision };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        Project.Instance.FilterData.NumericPrecision = dlg.NumberOfDigits;
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
      if (Project.Instance.FilterData.RegressionFunctionTexFormula != null)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        var renderer = Project.Instance.FilterData.RegressionFunctionTexFormula.GetRenderer(TexStyle.Display, 14d);

        using (var drawingContext = visual.RenderOpen())
        {
          renderer.Render(drawingContext, 0, 1);
        }

        this.FormulaContainerElement.Visual = visual;
      }
      else
      {
        // Formula is empty
        this.FormulaContainerElement.Visual = null;
      }
    }

    /// <summary>
    /// Updates the theoretical term visual with a tex representation of the theoretical function
    /// </summary>
    private void RefreshTheorieFunctionTerm()
    {
      // Only if we have a formula and should display the theory series
      if (Project.Instance.FilterData.TheoryFunctionTexFormula != null && Project.Instance.FilterData.IsShowingTheorySeries)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        var renderer = Project.Instance.FilterData.TheoryFunctionTexFormula.GetRenderer(TexStyle.Display, 14d);

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
      fktEditor.buttonX.Content = string.Concat(this.axisName);
      Constants.varName = string.Concat(this.axisName);
      if (Project.Instance.FilterData.TheoreticalFunction != null)
      {
        fktEditor.textBox1.Text = Project.Instance.FilterData.TheoreticalFunction.Name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        Project.Instance.FilterData.TheoreticalFunction = fktEditor.GetFunktion();
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
        if (Project.Instance.FilterData.TheoreticalFunction == null)
        {
          return;
        }

        var functionString = Project.Instance.FilterData.TheoreticalFunction.Name;
        functionString = functionString.Replace("*", "{\\cdot}");
        functionString = functionString.Replace("(", "{(");
        functionString = functionString.Replace(")", ")}");
        if (functionString.IndexOf("§", StringComparison.Ordinal) >= 0)
        {
          functionString = functionString.Replace("§#e", "\\epsilon");
          functionString = functionString.Replace("§#m", "\\mu");
          functionString = functionString.Replace("§#l", "\\lambda");
          functionString = functionString.Replace("§#g", "\\gamma");
          functionString = functionString.Replace("§", string.Empty);
          /* functionString = functionString.Replace("§m_e", "m_e");
             functionString = functionString.Replace("§e", "e");
             functionString = functionString.Replace("§h", "h");
             functionString = functionString.Replace("§c", "c");            
             functionString = functionString.Replace("§g", "g");
          */
        }

        functionString = functionString.Replace('x', this.axisName);
        var formula = this.formulaParser.Parse(functionString);
        if (formula != null)
        {
          Project.Instance.FilterData.TheoryFunctionTexFormula = formula;
        }

        this.RefreshTheorieFunctionTerm();
      }
      catch (Exception)
      {
        Project.Instance.FilterData.TheoryFunctionTexFormula = null;
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
      Project.Instance.ProcessingData.IndexOfObject = int.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
      Project.Instance.VideoData.ActiveObject = Project.Instance.ProcessingData.IndexOfObject;
    }

    /// <summary>
    ///   The populate object combo.
    /// </summary>
    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < Project.Instance.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      // this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      var indexBinding = new Binding("ProcessingData.IndexOfObject") { Source = Project.Instance };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      Project.Instance.ProcessingData.IndexOfObject++;
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
      this.DefaultSeries.DataSource = Project.Instance.VideoData.Samples;
      this.UpdateChartProperties();
      this.UpdateFilters();
    }

    /// <summary>
    /// This methods updates the filter series for the currently shown filters
    /// </summary>
    private void UpdateFilters()
    {
      Project.Instance.FilterData.CalculateInterpolationSeriesDataPoints();
      Project.Instance.FilterData.CalculateRegressionSeriesDataPoints();
      this.RefreshRegressionFuctionTerm();
      Project.Instance.FilterData.CalculateTheorySeriesDataPoints();
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
        var axisX = (DataAxis)this.XAxisContent.SelectedItem;
        var axisY = (DataAxis)this.YAxisContent.SelectedItem;
        this.XAxisContent.SelectedValue = axisX.Axis;
        this.YAxisContent.SelectedValue = axisY.Axis;

        Project.Instance.FilterData.AxisX = axisX;
        Project.Instance.FilterData.AxisY = axisY;
        this.RefreshChartDataPoints();

        // axes content already set, so return
        return;
      }

      switch (chartType)
      {
        case ChartType.YoverX:
          this.XAxisContent.SelectedValue = AxisType.PX;
          this.YAxisContent.SelectedValue = AxisType.PY;
          achsBez = 'x';
          funcBez = 'y';
          break;
        case ChartType.XoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.PX;
          achsBez = 't';
          funcBez = 'x';
          break;
        case ChartType.YoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.PY;
          achsBez = 't';
          funcBez = 'y';
          break;
        case ChartType.VoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.V;
          achsBez = 't';
          funcBez = 'v';
          break;
        case ChartType.VXoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.VX;
          achsBez = 't';
          funcBez = 'v';
          break;
        case ChartType.VYoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.VY;
          achsBez = 't';
          funcBez = 'v';
          break;
        case ChartType.AoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.A;
          achsBez = 't';
          funcBez = 'a';
          break;
        case ChartType.AXoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.AX;
          achsBez = 't';
          funcBez = 'a';
          break;
        case ChartType.AYoverT:
          this.XAxisContent.SelectedValue = AxisType.T;
          this.YAxisContent.SelectedValue = AxisType.AY;
          achsBez = 't';
          funcBez = 'a';
          break;
        case ChartType.VoverD:
          this.XAxisContent.SelectedValue = AxisType.D;
          this.YAxisContent.SelectedValue = AxisType.V;
          achsBez = 's';
          funcBez = 'v';
          break;
        case ChartType.VXoverDX:
          this.XAxisContent.SelectedValue = AxisType.DX;
          this.YAxisContent.SelectedValue = AxisType.VX;
          achsBez = 's';
          funcBez = 'v';
          break;
        case ChartType.VYoverDY:
          this.XAxisContent.SelectedValue = AxisType.DY;
          this.YAxisContent.SelectedValue = AxisType.VY;
          achsBez = 's';
          funcBez = 'v';
          break;
        case ChartType.VoverS:
          this.XAxisContent.SelectedValue = AxisType.S;
          this.YAxisContent.SelectedValue = AxisType.V;
          achsBez = 's';
          funcBez = 'v';
          break;
        case ChartType.VXoverSX:
          this.XAxisContent.SelectedValue = AxisType.SX;
          this.YAxisContent.SelectedValue = AxisType.VX;
          achsBez = 's';
          funcBez = 'v';
          break;
        case ChartType.VYoverSY:
          this.XAxisContent.SelectedValue = AxisType.SY;
          this.YAxisContent.SelectedValue = AxisType.VY;
          achsBez = 's';
          funcBez = 'v';
          break;
        case ChartType.AoverV:
          this.XAxisContent.SelectedValue = AxisType.V;
          this.YAxisContent.SelectedValue = AxisType.A;
          achsBez = 'v';
          funcBez = 'a';
          break;
        case ChartType.AXoverVX:
          this.XAxisContent.SelectedValue = AxisType.VX;
          this.YAxisContent.SelectedValue = AxisType.AX;
          achsBez = 'v';
          funcBez = 'a';
          break;
        case ChartType.AYoverVY:
          this.XAxisContent.SelectedValue = AxisType.VY;
          this.YAxisContent.SelectedValue = AxisType.AY;
          achsBez = 'v';
          funcBez = 'a';
          break;
      }

      this.axisName = achsBez;
      Project.Instance.FilterData.RegressionFilter.SetBezeichnungen(achsBez, funcBez);
      Project.Instance.FilterData.AxisX = (DataAxis)this.XAxisContent.SelectedItem;
      Project.Instance.FilterData.AxisY = (DataAxis)this.YAxisContent.SelectedItem;
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
      string prefix = "Object[" + Project.Instance.ProcessingData.IndexOfObject.ToString(CultureInfo.InvariantCulture)
                      + "].";

      string rulerUnitName = string.Empty;
      switch (Project.Instance.CalibrationData.RulerUnit)
      {
        case Unit.mm:
          rulerUnitName = "mm";
          break;
        case Unit.cm:
          rulerUnitName = "cm";
          break;
        case Unit.m:
          rulerUnitName = "m";
          break;
        case Unit.km:
          rulerUnitName = "km";
          break;
        case Unit.px:
          rulerUnitName = "pixel";
          break;
      }

      switch (axis.Axis)
      {
        case AxisType.I:
          mapPoints.Path = "Framenumber";
          this.axisUnitName = string.Empty;
          break;
        case AxisType.T:
          mapPoints.Path = "Timestamp";
          this.axisUnitName = "ms";
          break;
        case AxisType.PX:
          mapPoints.Path = "PositionX";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.PY:
          mapPoints.Path = "PositionY";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.D:
          mapPoints.Path = "Distance";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.DX:
          mapPoints.Path = "DistanceX";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.DY:
          mapPoints.Path = "DistanceY";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.S:
          mapPoints.Path = "Length";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.SX:
          mapPoints.Path = "LengthX";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.SY:
          mapPoints.Path = "LengthY";
          this.axisUnitName = rulerUnitName;
          break;
        case AxisType.V:
          mapPoints.Path = "Velocity";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms");
          break;
        case AxisType.VX:
          mapPoints.Path = "VelocityX";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms");
          break;
        case AxisType.VY:
          mapPoints.Path = "VelocityY";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms");
          break;
        case AxisType.VI:
          mapPoints.Path = "VelocityI";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms");
          break;
        case AxisType.VXI:
          mapPoints.Path = "VelocityXI";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms");
          break;
        case AxisType.VYI:
          mapPoints.Path = "VelocityYI";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms");
          break;
        case AxisType.A:
          mapPoints.Path = "Acceleration";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms²");
          break;
        case AxisType.AX:
          mapPoints.Path = "AccelerationX";
          axisUnitName = string.Concat(rulerUnitName, "/ms²");
          break;
        case AxisType.AY:
          mapPoints.Path = "AccelerationY";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms²");
          break;
        case AxisType.AI:
          mapPoints.Path = "AccelerationI";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms²");
          break;
        case AxisType.AXI:
          mapPoints.Path = "AccelerationXI";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms²");
          break;
        case AxisType.AYI:
          mapPoints.Path = "AccelerationYI";
          this.axisUnitName = string.Concat(rulerUnitName, "/ms²");
          break;
      }

      // Don´t prefix the timestamp
      if (axis.Axis != AxisType.T)
      {
        mapPoints.Path = prefix + mapPoints.Path;
      }

      this.axisUnitName = "  [" + this.axisUnitName + "]";
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
          axisX.Title = this.XAxisTitle.IsChecked ? this.XAxisTitle.Text + axisXUnitName : null;
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
          axisY.Title = this.YAxisTitle.IsChecked ? this.YAxisTitle.Text + axisYUnitName : null;
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

      Project.Instance.FilterData.IsShowingInterpolationSeries = enabled;
      Project.Instance.FilterData.IsShowingRegressionSeries = enabled;
      Project.Instance.FilterData.IsShowingTheorySeries = enabled;
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
      Project.Instance.FilterData.IsShowingTheorySeries = false;
      this.UpdateTheoryFormula();

      var axis = axisX ? (DataAxis)this.XAxisContent.SelectedItem : (DataAxis)this.YAxisContent.SelectedItem;

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
      if (axisX) { axisXUnitName = axisUnitName; }
      else
      {
        axisYUnitName = axisUnitName;
      }
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
      var regressionOptionsDialog = new RegressionOptionsDialog(Project.Instance.FilterData.RegressionFilter);
      if (!regressionOptionsDialog.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      if (regressionOptionsDialog.RegressionType == RegressionType.Best)
      {
        RegressionType bestRegression;
        if (Project.Instance.FilterData.RegressionFilter.WertX.Count == 0)
        {
          Project.Instance.FilterData.RegressionFilter.CalculateFilterValues();
        }

        Project.Instance.FilterData.RegressionFilter.GetBestRegressData(out bestRegression);
        this.UpdateRegressionImageButtonAndLabels(bestRegression, false);
      }
      else
      {
        this.UpdateRegressionImageButtonAndLabels(regressionOptionsDialog.RegressionType, true);
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
      Project.Instance.FilterData.RegressionFilter.RegressionType = aktregressionType;
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
        Project.Instance.FilterData.CalculateRegressionSeriesDataPoints();
        Project.Instance.FilterData.RegressionFilter.UpdateLinefitFunctionData(neuBerechnen);
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
    /// The interpolation check box was checked.
    /// So update the interpolation series.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">Event arguments</param>
    private void ShowInterpolationCheckBoxCheckedChanged(object sender, RoutedEventArgs e)
    {
      Project.Instance.FilterData.CalculateInterpolationSeriesDataPoints();
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
      Project.Instance.FilterData.CalculateTheorySeriesDataPoints(); 
      this.UpdateTheoryFormula();
      this.RefreshTheorieFunctionTerm();
    }

    private void ShowTheorieCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      Project.Instance.FilterData.CalculateTheorySeriesDataPoints(); 
      Project.Instance.FilterData.TheoryFunctionTexFormula = null;
      this.RefreshTheorieFunctionTerm();
    }

    private void ShowRegressionCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      Project.Instance.FilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat anzeigen
      this.RefreshRegressionFuctionTerm();
    }

    private void ShowRegressionCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      Project.Instance.FilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat nicht mehr anzeigen
      Project.Instance.FilterData.RegressionFunctionTexFormula = null;
      Project.Instance.FilterData.RegressionAberration = 0;
      this.RefreshRegressionFuctionTerm();
    }

    ///// <summary>
    ///// The interpolation check box was checked.
    ///// So update the property.
    ///// </summary>
    ///// <param name="sender">Source of the event</param>
    ///// <param name="e">Event arguments</param>
    //private void ShowInterpolationCheckBoxChecked(object sender, RoutedEventArgs e)
    //{
    //  Project.Instance.FilterData.IsShowingInterpolationSeries = true;
    //}

    ///// <summary>
    ///// The interpolation check box was unchecked.
    ///// So update the property.
    ///// </summary>
    ///// <param name="sender">Source of the event</param>
    ///// <param name="e">Event arguments</param>
    //private void ShowInterpolationCheckBoxUnchecked(object sender, RoutedEventArgs e)
    //{
    //  Project.Instance.FilterData.IsShowingInterpolationSeries = false;
    //}

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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = Project.Instance.FilterData.RegressionLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = Project.Instance.FilterData.RegressionLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Project.Instance.FilterData.RegressionLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        Project.Instance.FilterData.RegressionLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = Project.Instance.FilterData.TheoryLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = Project.Instance.FilterData.TheoryLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Project.Instance.FilterData.TheoryLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        Project.Instance.FilterData.TheoryLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = Project.Instance.FilterData.DataLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = Project.Instance.FilterData.DataLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Project.Instance.FilterData.DataLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        Project.Instance.FilterData.DataLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = Project.Instance.FilterData.InterpolationLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = Project.Instance.FilterData.InterpolationLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Project.Instance.FilterData.InterpolationLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        Project.Instance.FilterData.InterpolationLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }

    #endregion

  }
}