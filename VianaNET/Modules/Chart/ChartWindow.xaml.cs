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
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
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
  using Visifire.Commons;

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

    private readonly Cursor plusCursor = new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/CustomStyles/Cursors/CursorPlus.cur")).Stream);
    private readonly Cursor minusCursor = new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/CustomStyles/Cursors/CursorMinus.cur")).Stream);

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

      // VianaNetApplication.Project.VideoData.PropertyChanged +=
      // new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      // Calibration.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      VianaNetApplication.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.isInitialized = true;
      this.formulaParser = new TexFormulaParser();
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
      //this.PopulateAxesFromChartSelection();
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
      VianaNetApplication.Project.FilterData.InterpolationFilter.ShowInterpolationOptionsDialog();
      VianaNetApplication.Project.FilterData.CalculateInterpolationSeriesDataPoints();
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
      var dlg = new NumericalPrecisionDialog { NumberOfDigits = VianaNetApplication.Project.FilterData.NumericPrecision };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        VianaNetApplication.Project.FilterData.NumericPrecision = dlg.NumberOfDigits;
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
      if (VianaNetApplication.Project.FilterData.RegressionFunctionTexFormula != null)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        var renderer = VianaNetApplication.Project.FilterData.RegressionFunctionTexFormula.GetRenderer(TexStyle.Display, 14d);

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
      if (VianaNetApplication.Project.FilterData.TheoryFunctionTexFormula != null && VianaNetApplication.Project.FilterData.IsShowingTheorySeries)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        var renderer = VianaNetApplication.Project.FilterData.TheoryFunctionTexFormula.GetRenderer(TexStyle.Display, 14d);

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
      if (VianaNetApplication.Project.FilterData.TheoreticalFunction != null)
      {
        fktEditor.textBox1.Text = VianaNetApplication.Project.FilterData.TheoreticalFunction.Name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        VianaNetApplication.Project.FilterData.TheoreticalFunction = fktEditor.GetFunktion();
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
        if (VianaNetApplication.Project.FilterData.TheoreticalFunction == null)
        {
          return;
        }

        var functionString = VianaNetApplication.Project.FilterData.TheoreticalFunction.Name;
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
          VianaNetApplication.Project.FilterData.TheoryFunctionTexFormula = formula;
        }

        this.RefreshTheorieFunctionTerm();
      }
      catch (Exception)
      {
        VianaNetApplication.Project.FilterData.TheoryFunctionTexFormula = null;
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
      VianaNetApplication.Project.ProcessingData.IndexOfObject = int.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
      VianaNetApplication.Project.VideoData.ActiveObject = VianaNetApplication.Project.ProcessingData.IndexOfObject;
    }

    /// <summary>
    ///   The populate object combo.
    /// </summary>
    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      // this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      var indexBinding = new Binding("ProcessingData.IndexOfObject") { Source = VianaNetApplication.Project };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      VianaNetApplication.Project.ProcessingData.IndexOfObject++;
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
      this.DefaultSeries.DataSource = VianaNetApplication.Project.VideoData.Samples;
      foreach (DataPoint dataPoint in this.DefaultSeries.DataPoints)
      {
        dataPoint.Color = VianaNetApplication.Project.FilterData.SelectionColor;
      }

      this.UpdateChartProperties();
      this.UpdateFilters();
    }

    /// <summary>
    /// This methods updates the filter series for the currently shown filters
    /// </summary>
    private void UpdateFilters()
    {
      VianaNetApplication.Project.FilterData.CalculateInterpolationSeriesDataPoints();
      VianaNetApplication.Project.FilterData.CalculateRegressionSeriesDataPoints();
      this.RefreshRegressionFuctionTerm();
      VianaNetApplication.Project.FilterData.CalculateTheorySeriesDataPoints();
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

        VianaNetApplication.Project.FilterData.AxisX = axisX;
        VianaNetApplication.Project.FilterData.AxisY = axisY;
        this.RefreshSeries();
        //this.RefreshChartDataPoints();

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
      VianaNetApplication.Project.FilterData.RegressionFilter.SetBezeichnungen(achsBez, funcBez);
      VianaNetApplication.Project.FilterData.AxisX = (DataAxis)this.XAxisContent.SelectedItem;
      VianaNetApplication.Project.FilterData.AxisY = (DataAxis)this.YAxisContent.SelectedItem;
      //this.RefreshChartDataPoints();
      this.RefreshSeries();
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
      string prefix = "Object[" + VianaNetApplication.Project.ProcessingData.IndexOfObject.ToString(CultureInfo.InvariantCulture)
                      + "].";

      switch (axis.Axis)
      {
        case AxisType.I:
          mapPoints.Path = "Framenumber";
          this.axisUnitName = string.Empty;
          break;
        case AxisType.T:
          mapPoints.Path = "Time";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.TimeUnit.ToString();
          break;
        case AxisType.PX:
          mapPoints.Path = "PositionX";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.PY:
          mapPoints.Path = "PositionY";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.D:
          mapPoints.Path = "Distance";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.DX:
          mapPoints.Path = "DistanceX";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.DY:
          mapPoints.Path = "DistanceY";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.S:
          mapPoints.Path = "Length";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.SX:
          mapPoints.Path = "LengthX";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.SY:
          mapPoints.Path = "LengthY";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.V:
          mapPoints.Path = "Velocity";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VX:
          mapPoints.Path = "VelocityX";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VY:
          mapPoints.Path = "VelocityY";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VI:
          mapPoints.Path = "VelocityI";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VXI:
          mapPoints.Path = "VelocityXI";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VYI:
          mapPoints.Path = "VelocityYI";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.A:
          mapPoints.Path = "Acceleration";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AX:
          mapPoints.Path = "AccelerationX";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AY:
          mapPoints.Path = "AccelerationY";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AI:
          mapPoints.Path = "AccelerationI";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AXI:
          mapPoints.Path = "AccelerationXI";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AYI:
          mapPoints.Path = "AccelerationYI";
          this.axisUnitName = VianaNetApplication.Project.CalibrationData.AccelerationUnit;
          break;
      }

      mapPoints.Path = prefix + mapPoints.Path;

      this.axisUnitName = "  [" + this.axisUnitName + "]";
    }

    /// <summary>
    ///   The update chart properties.
    /// </summary>
    private void UpdateChartProperties()
    {
      if (this.isInitialized)
      {
        double interval, tickInterval, min, max;

        if (this.DataChart.AxesX.Count > 0)
        {
          var axisX = this.DataChart.AxesX[0];
          if (this.GetAxisBounds(true, out interval, out tickInterval, out min, out max))
          {
            axisX.AxisMinimum = min;
            axisX.AxisMaximum = max;
            axisX.Interval = interval;
            axisX.Ticks[0].Interval = tickInterval;
          }
          //axisX.Title = this.XAxisTitle.IsChecked ? this.XAxisTitle.Text + this.axisXUnitName : null;
          //axisX.Grids[0].Enabled = this.XAxisShowGridLines.IsChecked();

          //if (this.XAxisMinimum.Value > this.XAxisMaximum.Value)
          //{
          //  this.XAxisMinimum.Value = this.XAxisMaximum.Value;
          //}

          //if (this.XAxisMinimum.IsChecked)
          //{
          //  axisX.AxisMinimum = this.XAxisMinimum.Value;
          //}
          //else
          //{
          //  double interval;
          //  axisX.AxisMinimum = this.GetAxisBounds(this.DataChart.AxesX[0], out interval);
          //  axisX.Interval = interval;
          //}

          //if (this.XAxisMaximum.Value < this.XAxisMinimum.Value)
          //{
          //  this.XAxisMaximum.Value = this.XAxisMinimum.Value;
          //}

          //if (this.XAxisMaximum.IsChecked)
          //{
          //  axisX.AxisMaximum = this.XAxisMaximum.Value;
          //}
          //else
          //{
          //  axisX.AxisMaximum = null;
          //}

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
          if (this.GetAxisBounds(false, out interval, out tickInterval, out min, out max))
          {
            axisY.AxisMinimum = min;
            axisY.AxisMaximum = max;
            axisY.Interval = interval;
            axisY.Ticks[0].Interval = tickInterval;
          }

          //  axisY.Title = this.YAxisTitle.IsChecked ? this.YAxisTitle.Text + this.axisYUnitName : null;
          //  axisY.Grids[0].Enabled = this.YAxisShowGridLines.IsChecked();

          //  if (this.YAxisMinimum.Value > this.YAxisMaximum.Value)
          //  {
          //    this.YAxisMinimum.Value = this.YAxisMaximum.Value;
          //  }

          //  if (this.YAxisMinimum.IsChecked)
          //  {
          //    axisY.AxisMinimum = this.YAxisMinimum.Value;
          //  }
          //  else
          //  {
          //    double interval;
          //    axisY.AxisMinimum = this.GetAxisBounds(this.DataChart.AxesY[0], out interval);
          //    axisY.Interval = interval;
          //  }

          //  if (this.YAxisMaximum.Value < this.YAxisMinimum.Value)
          //  {
          //    this.YAxisMaximum.Value = this.YAxisMinimum.Value;
          //  }

          //  if (this.YAxisMaximum.IsChecked)
          //  {
          //    axisY.AxisMaximum = this.YAxisMaximum.Value;
          //  }
          //  else
          //  {
          //    axisY.AxisMaximum = null;
          //  }

          //  // if (YAxisInterval.IsChecked)
          //  // {
          //  // yAxis.Interval = YAxisInterval.Value;
          //  // }
          //  // else
          //  // {
          //  // yAxis.Interval = double.NaN;
          //  // }
        }
      }
    }

    private bool GetAxisBounds(bool xAxis, out double interval, out double tickInterval, out double min, out double max)
    {
      interval = double.NaN;
      tickInterval = double.NaN;
      min = double.NaN;
      max = double.NaN;

      if (this.DefaultSeries.DataPoints.Count == 0)
      {
        return false;
      }

      double span;
      if (xAxis)
      {
        min = (double)this.DefaultSeries.DataPoints.Min(o => o.XValue);
        max = (double)this.DefaultSeries.DataPoints.Max(o => o.XValue);
        //var span = (double)axis.ActualAxisMaximum - (double)axis.ActualAxisMinimum;
        span = max - min;
      }
      else
      {
        min = this.DefaultSeries.DataPoints.Min(o => o.YValue);
        max = this.DefaultSeries.DataPoints.Max(o => o.YValue);
        span = max - min;
      }

      if (Double.IsNaN(span))
      {
        return false;
      }

      var roundDigit = 0;
      interval = 1;
      tickInterval = 1;
      if (span < 0.001)
      {
        roundDigit = 4;
        interval = 0.0001;
        tickInterval = 0.00001;
      }
      else if (span < 0.005)
      {
        roundDigit = 3;
        interval = 0.001;
        tickInterval = 0.0001;
      }
      else if (span < 0.01)
      {
        roundDigit = 3;
        interval = 0.001;
        tickInterval = 0.0001;
      }
      else if (span < 0.05)
      {
        roundDigit = 2;
        interval = 0.01;
        tickInterval = 0.001;
      }
      else if (span < 0.1)
      {
        roundDigit = 2;
        interval = 0.01;
        tickInterval = 0.001;
      }
      else if (span < 0.5)
      {
        roundDigit = 1;
        interval = 0.1;
        tickInterval = 0.01;
      }
      else if (span < 1)
      {
        roundDigit = 1;
        interval = 0.1;
        tickInterval = 0.01;
      }
      else if (span < 5)
      {
        roundDigit = 0;
        interval = 1;
        tickInterval = 0.1;
      }
      else if (span < 10)
      {
        roundDigit = 0;
        interval = 1;
        tickInterval = 0.1;
      }
      else if (span < 50)
      {
        roundDigit = -1;
        interval = 5;
        tickInterval = 1;
      }
      else if (span < 100)
      {
        roundDigit = -1;
        interval = 10;
        tickInterval = 1;
      }
      else if (span < 500)
      {
        roundDigit = -2;
        interval = 50;
        tickInterval = 5;
      }
      else if (span < 1000)
      {
        roundDigit = -2;
        interval = 100;
        tickInterval = 10;
      }
      else if (span < 5000)
      {
        roundDigit = -3;
        interval = 500;
        tickInterval = 50;
      }
      else if (span < 10000)
      {
        roundDigit = -3;
        interval = 1000;
        tickInterval = 100;
      }

      min = Round(min, roundDigit, true);
      max = Round(max, roundDigit, false);
      return true;

    }

    static double Round(double value, int digits, bool min)
    {
      if ((digits < -15) || (digits > 15))
      {
        throw new ArgumentOutOfRangeException("digits", "Rounding digits must be between -15 and 15, inclusive.");
      }

      if (digits >= 0)
      {
        var factor = Math.Pow(10, digits);
        double roundedValue = min ? Math.Floor(value * factor) : Math.Ceiling(value * factor);

        return roundedValue / factor;
      }

      double n = Math.Pow(10, -digits);
      return Math.Round(value / n, 0) * n;
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

      VianaNetApplication.Project.FilterData.IsShowingInterpolationSeries = enabled;
      VianaNetApplication.Project.FilterData.IsShowingRegressionSeries = enabled;
      VianaNetApplication.Project.FilterData.IsShowingTheorySeries = enabled;
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
      VianaNetApplication.Project.FilterData.IsShowingTheorySeries = false;
      this.UpdateTheoryFormula();

      var axis = axisX ? (DataAxis)this.XAxisContent.SelectedItem : (DataAxis)this.YAxisContent.SelectedItem;

      if (axisX)
      {
        if (this.DataChart.AxesX.Count >= 1)
        {
          this.DataChart.AxesX[0].Title = axis.Description;
        }
      }
      else
      {
        if (this.DataChart.AxesY.Count >= 1)
        {
          this.DataChart.AxesY[0].Title = axis.Description;
        }
      }

      var map = this.DefaultSeries.DataMappings[axisX ? 0 : 1];
      this.UpdateAxisMappings(axis, map);
      if (axisX)
      {
        this.axisXUnitName = this.axisUnitName;
      }
      else
      {
        this.axisYUnitName = this.axisUnitName;
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
      //this.UpdateChartProperties();
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
      var regressionOptionsDialog = new RegressionOptionsDialog(VianaNetApplication.Project.FilterData.RegressionFilter);
      if (!regressionOptionsDialog.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      if (regressionOptionsDialog.RegressionType == RegressionType.Best)
      {
        RegressionType bestRegression;
        if (VianaNetApplication.Project.FilterData.RegressionFilter.WertX.Count == 0)
        {
          VianaNetApplication.Project.FilterData.RegressionFilter.CalculateFilterValues();
        }

        VianaNetApplication.Project.FilterData.RegressionFilter.GetBestRegressData(out bestRegression, regressionOptionsDialog.negFlag);
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
    /// <param name="neuBerechnen">True, wenn die Regression neu berechnet werden soll </param>
    private void UpdateRegressionImageButtonAndLabels(RegressionType aktregressionType, bool neuBerechnen)
    {
      string bildsource;
      VianaNetApplication.Project.FilterData.RegressionFilter.RegressionType = aktregressionType;
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
        VianaNetApplication.Project.FilterData.CalculateRegressionSeriesDataPoints();
        VianaNetApplication.Project.FilterData.RegressionFilter.UpdateLinefitFunctionData(neuBerechnen);
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
      //if (!this.UpdateMapping(true))
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
      //if (!this.UpdateMapping(false))
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
      VianaNetApplication.Project.FilterData.CalculateInterpolationSeriesDataPoints();
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
      VianaNetApplication.Project.FilterData.CalculateTheorySeriesDataPoints();
      this.UpdateTheoryFormula();
      this.RefreshTheorieFunctionTerm();
    }

    private void ShowTheorieCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.FilterData.CalculateTheorySeriesDataPoints();
      VianaNetApplication.Project.FilterData.TheoryFunctionTexFormula = null;
      this.RefreshTheorieFunctionTerm();
    }

    private void ShowRegressionCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.FilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat anzeigen
      this.RefreshRegressionFuctionTerm();
    }

    private void ShowRegressionCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.FilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat nicht mehr anzeigen
      VianaNetApplication.Project.FilterData.RegressionFunctionTexFormula = null;
      VianaNetApplication.Project.FilterData.RegressionAberration = 0;
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
    //  VianaNetApplication.Project.FilterData.IsShowingInterpolationSeries = true;
    //}

    ///// <summary>
    ///// The interpolation check box was unchecked.
    ///// So update the property.
    ///// </summary>
    ///// <param name="sender">Source of the event</param>
    ///// <param name="e">Event arguments</param>
    //private void ShowInterpolationCheckBoxUnchecked(object sender, RoutedEventArgs e)
    //{
    //  VianaNetApplication.Project.FilterData.IsShowingInterpolationSeries = false;
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = VianaNetApplication.Project.FilterData.RegressionLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = VianaNetApplication.Project.FilterData.RegressionLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        VianaNetApplication.Project.FilterData.RegressionLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        VianaNetApplication.Project.FilterData.RegressionLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = VianaNetApplication.Project.FilterData.TheoryLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = VianaNetApplication.Project.FilterData.TheoryLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        VianaNetApplication.Project.FilterData.TheoryLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        VianaNetApplication.Project.FilterData.TheoryLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = VianaNetApplication.Project.FilterData.DataLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = VianaNetApplication.Project.FilterData.DataLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        VianaNetApplication.Project.FilterData.DataLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        VianaNetApplication.Project.FilterData.DataLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = VianaNetApplication.Project.FilterData.InterpolationLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = VianaNetApplication.Project.FilterData.InterpolationLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        VianaNetApplication.Project.FilterData.InterpolationLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        VianaNetApplication.Project.FilterData.InterpolationLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }

    #endregion

    private void DataChart_Rendered(object sender, EventArgs e)
    {

    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
    }
    // MouseMove event handler

    void PlotArea_MouseMove(object sender, PlotAreaMouseEventArgs e)
    {
      if (this.mouseDown)
      {
        Point currentPos = new Point(e.MouseEventArgs.GetPosition(MyCanvas).X, e.MouseEventArgs.GetPosition(MyCanvas).Y);

        SelectRect.Visibility = Visibility.Visible;
        SelectRect.Width = Math.Abs(startPos.X - currentPos.X);
        SelectRect.Height = Math.Abs(startPos.Y - currentPos.Y);
        if (currentPos.X < startPos.X)
          SelectRect.SetValue(Canvas.LeftProperty, currentPos.X);
        if (currentPos.Y < startPos.Y)
          SelectRect.SetValue(Canvas.TopProperty, currentPos.Y);

        endXValue = (Double)e.XValue;
        endYValue = e.YValue;
      }
      else
        SelectRect.Visibility = Visibility.Collapsed;
    }

    // MouseLeftButtonDown event handler
    void PlotArea_MouseLeftButtonDown(object sender, PlotAreaMouseButtonEventArgs e)
    {
      this.mouseDown = true;
      SelectRect.Width = 0;
      SelectRect.Height = 0;
      startXValue = (Double)e.XValue;
      startYValue = e.YValue;

      startPos = new Point(e.MouseButtonEventArgs.GetPosition(MyCanvas).X, e.MouseButtonEventArgs.GetPosition(MyCanvas).Y);
      SelectRect.SetValue(Canvas.LeftProperty, startPos.X);
      SelectRect.SetValue(Canvas.TopProperty, startPos.Y);

      SelectRect.Visibility = Visibility.Visible;

      // Control button is pressed so more points are to be selected
      if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        return;
      }

      var selectedDataPoints = (from dp in this.DataChart.Series[0].DataPoints
                                where dp.Color == VianaNetApplication.Project.FilterData.SelectionColor
                                select dp);

      var selectedIndizes = new List<int>();
      foreach (DataPoint dataPoint in selectedDataPoints)
      {
        dataPoint.Color = VianaNetApplication.Project.FilterData.DataLineColor;
        var number = Convert.ToInt32(dataPoint.LegendText.Replace("DataPoint", string.Empty));
        selectedIndizes.Add(number);
      }

      foreach (var selectedIndex in selectedIndizes)
      {
        VianaNetApplication.Project.VideoData.Samples[selectedIndex].IsSelected = false;
      }
    }

    // MouseLeftButtonUp event handler
    void PlotAreaMouseLeftButtonUp(object sender, PlotAreaMouseButtonEventArgs e)
    {
      if (this.mouseDown)
      {
        var currentPos = new Point(e.MouseButtonEventArgs.GetPosition(MyCanvas).X, e.MouseButtonEventArgs.GetPosition(MyCanvas).Y);
        if (startPos == currentPos)
        {
          this.mouseDown = false;
          return;
        }

        var selectedDataPoints = (from dp in this.DataChart.Series[0].DataPoints
                                  where (Convert.ToDouble(dp.XValue) > Math.Min(startXValue, endXValue))
                                  && (Convert.ToDouble(dp.XValue) < Math.Max(startXValue, endXValue))
                                  && (dp.YValue > Math.Min(startYValue, endYValue)
                                  && dp.YValue < Math.Max(startYValue, endYValue))
                                  orderby dp.XValue
                                  select dp);

        var selectedIndizes = new List<int>();

        if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0 && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
        {
          foreach (var dataPoint in selectedDataPoints)
          {
            dataPoint.Color = VianaNetApplication.Project.FilterData.DataLineColor;
            var number = Convert.ToInt32(dataPoint.LegendText.Replace("DataPoint", string.Empty));
            selectedIndizes.Add(number);
          }

          foreach (var selectedIndex in selectedIndizes)
          {
            VianaNetApplication.Project.VideoData.Samples[selectedIndex].IsSelected = false;
          }
        }
        else
        {
          foreach (var dataPoint in selectedDataPoints)
          {
            dataPoint.Color = VianaNetApplication.Project.FilterData.SelectionColor;
            var number = Convert.ToInt32(dataPoint.LegendText.Replace("DataPoint", string.Empty));
            selectedIndizes.Add(number);
          }

          foreach (var selectedIndex in selectedIndizes)
          {
            VianaNetApplication.Project.VideoData.Samples[selectedIndex].IsSelected = true;
          }
        }
      }

      this.mouseDown = false;
      SelectRect.Visibility = Visibility.Collapsed;
    }

    // MouseLeave event handler
    void PlotAreaMouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mouseDown)
        SelectRect.Visibility = Visibility.Collapsed;
    }

    void PlotAreaMouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mouseDown)
        SelectRect.Visibility = Visibility.Visible;
    }

    private Double startXValue, endXValue;
    private Double startYValue, endYValue;
    private Boolean mouseDown;
    private Point startPos;


    private void PlotArea_PreviewKeyDown(object sender, KeyEventArgs e)
    {

      if (Keyboard.Modifiers == ModifierKeys.Control)
      {
        this.DataChart.PlotArea.Cursor = plusCursor;
      }
      else if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0 && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        this.DataChart.PlotArea.Cursor = minusCursor;
      }

    }

    private void PlotArea_PreviewKeyUp(object sender, KeyEventArgs e)
    {
      this.DataChart.PlotArea.Cursor = Cursors.Arrow;
    }
  }
}