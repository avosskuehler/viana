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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Chart
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using Microsoft.Office.Interop.Excel;

  using OxyPlot;
  using OxyPlot.Series;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter;
  using VianaNET.Data.Filter.Regression;
  using VianaNET.Data.Filter.Theory;

  using WPFMath;

  using Application = System.Windows.Application;
  using Constants = VianaNET.Data.Filter.Theory.Constants;
  using Labels = VianaNET.Resources.Labels;
  using Point = System.Windows.Point;
  using SelectionMode = OxyPlot.SelectionMode;

  /// <summary>
  ///   The chart window.
  /// </summary>
  public partial class ChartWindow
  {
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

    #region Constants

    /// <summary>
    ///   Determines maximum distance of two points (in pixel) given by mouse input
    ///   that should be considered as different.
    /// </summary>
    private const int Maxdistancepoints = 10;

    #endregion

    #region Fields

    /// <summary>
    ///   Provides a formula parser which reads tex formulas
    /// </summary>
    private readonly TexFormulaParser formulaParser;

    /// <summary>
    ///   The is initialized
    /// </summary>
    private readonly bool isInitialized;

    /// <summary>
    /// The minus cursor.
    /// </summary>
    private readonly Cursor minusCursor =
      new Cursor(
        Application.GetResourceStream(new Uri("pack://application:,,,/CustomStyles/Cursors/CursorMinus.cur")).Stream);

    /// <summary>
    /// The plus cursor.
    /// </summary>
    private readonly Cursor plusCursor =
      new Cursor(
        Application.GetResourceStream(new Uri("pack://application:,,,/CustomStyles/Cursors/CursorPlus.cur")).Stream);

    /// <summary>
    ///   A character for the axis name
    /// </summary>
    private char axisName = 'x';

    /// <summary>
    /// The end x value.
    /// </summary>
    private double endXValue;

    /// <summary>
    /// The end y value.
    /// </summary>
    private double endYValue;

    /// <summary>
    /// The mouse down.
    /// </summary>
    private bool mouseDown;

    /// <summary>
    /// The start pos.
    /// </summary>
    private Point startPos;

    /// <summary>
    /// The start x value.
    /// </summary>
    private double startXValue;

    /// <summary>
    /// The start y value.
    /// </summary>
    private double startYValue;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ChartWindow" /> class.
    /// </summary>
    public ChartWindow()
    {
      this.ChartData = new ChartData();
      this.InitializeComponent();
      this.ObjectSelectionCombo.DataContext = this;
      this.PopulateObjectCombo();
      Viana.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.isInitialized = true;
      this.formulaParser = new TexFormulaParser();
      this.PopulateAxesFromChartSelection();

      //this.DefaultSeries.Mapping =
      //  item => new ScatterPoint(GetTargetValue(item, "PositionX"), GetTargetValue(item, "PositionY"));
      //this.InterpolationSeries.Mapping = item => new DataPoint(((XYSample)item).ValueX, ((XYSample)item).ValueY);
      //this.RegressionSeries.Mapping = item => new DataPoint(((XYSample)item).ValueX, ((XYSample)item).ValueY);
      //this.TheorySeries.Mapping = item => new DataPoint(((XYSample)item).ValueX, ((XYSample)item).ValueY);
      //this.DefaultSeries.SelectionMode = SelectionMode.Multiple;
    }

    #endregion

    #region Public Properties

    public ChartData ChartData { get; set; }

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

    ///// <summary>
    ///// Updates the default series mapping.
    ///// </summary>
    ///// <param name="propertyX">
    ///// The property x.
    ///// </param>
    ///// <param name="propertyY">
    ///// The property y.
    ///// </param>
    //public void UpdateDefaultSeriesMapping(string propertyX, string propertyY)
    //{
    //  this.DefaultSeries.Mapping =
    //    item => new ScatterPoint(GetTargetValue(item, propertyX), GetTargetValue(item, propertyY));
    //}

    #endregion

    #region Methods

    /// <summary>
    /// Calculate Distance of two Vectors per Phythagoras
    /// </summary>
    /// <param name="pt1">
    /// Point 1 to calculate the distance for.
    /// </param>
    /// <param name="pt2">
    /// Point 2 to calculate the distance for.
    /// </param>
    /// <returns>
    /// Distance of the given Points in picture coordinates.
    /// </returns>
    private static float Distance(Point pt1, Point pt2)
    {
      double squaredX = Math.Pow(pt1.X - pt2.X, 2);
      double squaredY = Math.Pow(pt1.Y - pt2.Y, 2);
      return Convert.ToSingle(Math.Sqrt(squaredX + squaredY));
    }

    /// <summary>
    /// Gets the target value for the given object and the given property string.
    ///   Uses reflection.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="propertyString">
    /// The property string.
    /// </param>
    /// <returns>
    /// A <see cref="double"/> with the propertys value of the object.
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
        window.Refresh();
      }
    }

    /// <summary>
    /// The chart content selection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    private void ChartContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // This populates the chart combo boxes with the selected x and y Axes
      // During population it updates also the mappings and refreshes the data points
      this.PopulateAxesFromChartSelection();
    }

    /// <summary>
    /// Filter precision button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void FilterPrecisionButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new NumericalPrecisionDialog { NumberOfDigits = Viana.Project.CurrentFilterData.NumericPrecision };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        Viana.Project.CurrentFilterData.NumericPrecision = dlg.NumberOfDigits;
        if (this.RegressionCheckBox.IsChecked())
        {
          this.RefreshRegressionFuctionTerm();
          this.RefreshTheorieFunctionTerm();
        }
      }
    }

    /// <summary>
    /// The interpolation options button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void InterpolationOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      Viana.Project.CurrentFilterData.InterpolationFilter.ShowInterpolationOptionsDialog();
      Viana.Project.CurrentFilterData.CalculateInterpolationSeriesDataPoints();
    }

    /// <summary>
    /// Data style button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void DataStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = Viana.Project.CurrentFilterData.DataLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = Viana.Project.CurrentFilterData.DataLineColor;
      lineOptionsDialog.LineStyleControl.MarkerType = Viana.Project.CurrentFilterData.DataLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Viana.Project.CurrentFilterData.DataLineThickness = lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        Viana.Project.CurrentFilterData.DataLineColor = lineOptionsDialog.LineStyleControl.SeriesColor;
        Viana.Project.CurrentFilterData.DataLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Interpolation style button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void InterpolationStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = Viana.Project.CurrentFilterData.InterpolationLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = Viana.Project.CurrentFilterData.InterpolationLineColor;
      lineOptionsDialog.LineStyleControl.MarkerType = Viana.Project.CurrentFilterData.InterpolationLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Viana.Project.CurrentFilterData.InterpolationLineThickness = lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        Viana.Project.CurrentFilterData.InterpolationLineColor = lineOptionsDialog.LineStyleControl.SeriesColor;
        Viana.Project.CurrentFilterData.InterpolationLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Regression style button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void RegressionStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = Viana.Project.CurrentFilterData.RegressionLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = Viana.Project.CurrentFilterData.RegressionLineColor;
      lineOptionsDialog.LineStyleControl.MarkerType = Viana.Project.CurrentFilterData.RegressionLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Viana.Project.CurrentFilterData.RegressionLineThickness = lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        Viana.Project.CurrentFilterData.RegressionLineColor = lineOptionsDialog.LineStyleControl.SeriesColor;
        Viana.Project.CurrentFilterData.RegressionLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Theory style button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void TheoryStyleButtonClick(object sender, RoutedEventArgs e)
    {
      var lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = Viana.Project.CurrentFilterData.TheoryLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = Viana.Project.CurrentFilterData.TheoryLineColor;
      lineOptionsDialog.LineStyleControl.MarkerType = Viana.Project.CurrentFilterData.TheoryLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        Viana.Project.CurrentFilterData.TheoryLineThickness = lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        Viana.Project.CurrentFilterData.TheoryLineColor = lineOptionsDialog.LineStyleControl.SeriesColor;
        Viana.Project.CurrentFilterData.TheoryLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// The object selection combo selection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    private void ObjectSelectionComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.ObjectSelectionCombo.SelectedItem == null)
      {
        return;
      }

      var entry = (string)this.ObjectSelectionCombo.SelectedItem;
      Viana.Project.ProcessingData.IndexOfObject = int.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
      Viana.Project.VideoData.ActiveObject = Viana.Project.ProcessingData.IndexOfObject;
    }

    /// <summary>
    /// Checks if two points are nearer than MAX_DISTANCE_POLYLINE_CLOSE
    /// </summary>
    /// <remarks>
    /// Polyline is automatically closed, if they are.
    /// </remarks>
    /// <param name="point1">
    /// A <see cref="Point"/> with point one 
    /// </param>
    /// <param name="point2">
    /// A <see cref="Point"/> with point two
    /// </param>
    /// <returns>
    /// <strong>True</strong>, if points are nearer than MAX_DISTANCE_POLYLINE_CLOSE,
    ///   otherwise <strong>false</strong>.
    /// </returns>
    private bool PointsAreNear(Point point1, Point point2)
    {
      return Distance(point1, point2) < Maxdistancepoints;
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
        chartType = ChartType.Custom;
      }

      Viana.Project.CurrentChartType = chartType;

      switch (chartType)
      {
        case ChartType.Custom:
          var axisX = (DataAxis)this.XAxisContent.SelectedItem;
          var axisY = (DataAxis)this.YAxisContent.SelectedItem;
          this.XAxisContent.SelectedValue = axisX.Axis;
          this.YAxisContent.SelectedValue = axisY.Axis;
          achsBez = axisX.Axis == AxisType.T ? 't' : 'x';
          funcBez = 'y';
          break;
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
      Viana.Project.CurrentFilterData.RegressionFilter.SetBezeichnungen(achsBez, funcBez);
      Viana.Project.CurrentFilterData.AxisX = (DataAxis)this.XAxisContent.SelectedItem;
      Viana.Project.CurrentFilterData.AxisY = (DataAxis)this.YAxisContent.SelectedItem;
      this.Refresh();
    }

    /// <summary>
    ///   The populate object combo.
    /// </summary>
    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      // this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      var indexBinding = new Binding("ProcessingData.IndexOfObject") { Source = Viana.Project };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      Viana.Project.ProcessingData.IndexOfObject++;
    }

    /// <summary>
    /// The image processing property changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
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
        this.Refresh();
      }
    }

    /// <summary>
    /// Rechners the button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void RechnerButtonClick(object sender, RoutedEventArgs e)
    {
      var calculator = new CalculatorAndFktEditor(TRechnerArt.rechner);
      calculator.ShowDialog();
    }

    /// <summary>
    ///   Updates the function term visual with a tex representation of the regression function
    /// </summary>
    private void RefreshRegressionFuctionTerm()
    {
      if (Viana.Project.CurrentFilterData.RegressionFunctionTexFormula != null)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        TexRenderer renderer = Viana.Project.CurrentFilterData.RegressionFunctionTexFormula.GetRenderer(
          TexStyle.Display,
          14d);

        using (DrawingContext drawingContext = visual.RenderOpen())
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
    ///   This method refreshes the whole series and chart layout
    /// </summary>
    public void Refresh()
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (this.ChartData.ChartDataModel.Series.Count == 0)
      {
        return;
      }

      // Whenever changing the axes, the theory formula will be odd, so hide it
      Viana.Project.CurrentFilterData.IsShowingTheorySeries = false;
      this.UpdateTheoryFormula();

      var axisX = (DataAxis)this.XAxisContent.SelectedItem;
      var axisY = (DataAxis)this.YAxisContent.SelectedItem;

      string propertyX;
      string unitNameX;
      this.UpdateAxisMappings(axisX, out propertyX, out unitNameX);

      string propertyY;
      string unitNameY;
      this.UpdateAxisMappings(axisY, out propertyY, out unitNameY);

      this.ChartData.XAxis.Title = axisX.Description;
      this.ChartData.XAxis.Unit = unitNameX;
      this.ChartData.YAxis.Title = axisY.Description;
      this.ChartData.YAxis.Unit = unitNameY;

      this.XAxisTitleTextBox.Text = axisX.Description;
      this.YAxisTitleTextBox.Text = axisY.Description;

      this.ChartData.UpdateDefaultSeriesMapping(propertyX, propertyY);
      this.UpdateFilters();
      this.DataChart.ResetAllAxes();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    ///   Updates the theoretical term visual with a tex representation of the theoretical function
    /// </summary>
    private void RefreshTheorieFunctionTerm()
    {
      // Only if we have a formula and should display the theory series
      if (Viana.Project.CurrentFilterData.TheoryFunctionTexFormula != null
          && Viana.Project.CurrentFilterData.IsShowingTheorySeries)
      {
        // Render formula to visual.
        var visual = new DrawingVisual();
        TexRenderer renderer = Viana.Project.CurrentFilterData.TheoryFunctionTexFormula.GetRenderer(
          TexStyle.Display,
          14d);

        using (DrawingContext drawingContext = visual.RenderOpen())
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
      var regressionOptionsDialog = new RegressionOptionsDialog(Viana.Project.CurrentFilterData.RegressionFilter);
      if (!regressionOptionsDialog.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      if (regressionOptionsDialog.RegressionType == RegressionType.Best)
      {
        RegressionType bestRegression;
        if (Viana.Project.CurrentFilterData.RegressionFilter.WertX.Count == 0)
        {
          Viana.Project.CurrentFilterData.RegressionFilter.CalculateFilterValues();
        }

        Viana.Project.CurrentFilterData.RegressionFilter.GetBestRegressData(
          out bestRegression,
          regressionOptionsDialog.negFlag);
        this.UpdateRegressionImageButtonAndLabels(bestRegression, false);
      }
      else
      {
        this.UpdateRegressionImageButtonAndLabels(regressionOptionsDialog.RegressionType, true);
      }
    }

    /// <summary>
    /// The interpolation check box was checked.
    ///   So update the interpolation series.
    /// </summary>
    /// <param name="sender">
    /// Source of the event
    /// </param>
    /// <param name="e">
    /// Event arguments
    /// </param>
    private void ShowInterpolationCheckBoxCheckedChanged(object sender, RoutedEventArgs e)
    {
      Viana.Project.CurrentFilterData.CalculateInterpolationSeriesDataPoints();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The show regression check box checked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ShowRegressionCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      Viana.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat anzeigen
      this.RefreshRegressionFuctionTerm();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The show regression check box unchecked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ShowRegressionCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      Viana.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat nicht mehr anzeigen
      Viana.Project.CurrentFilterData.RegressionFunctionTexFormula = null;
      Viana.Project.CurrentFilterData.RegressionAberration = 0;
      this.RefreshRegressionFuctionTerm();
      this.ChartData.UpdateModel();
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
      Viana.Project.CurrentFilterData.CalculateTheorySeriesDataPoints();
      this.UpdateTheoryFormula();
      this.RefreshTheorieFunctionTerm();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The show theorie check box unchecked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ShowTheorieCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      Viana.Project.CurrentFilterData.CalculateTheorySeriesDataPoints();
      Viana.Project.CurrentFilterData.TheoryFunctionTexFormula = null;
      this.RefreshTheorieFunctionTerm();
      this.ChartData.UpdateModel();
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
      if (Viana.Project.CurrentFilterData.TheoreticalFunction != null)
      {
        fktEditor.textBox1.Text = Viana.Project.CurrentFilterData.TheoreticalFunction.Name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        Viana.Project.CurrentFilterData.TheoreticalFunction = fktEditor.GetFunktion();
        this.UpdateTheoryFormula();
      }
    }

    /// <summary>
    /// Updates the axis mappings for the data display.
    ///   This method only updates the data series, because
    ///   all other series are reevaluated depending on the
    ///   display of the displayed orgininal data
    /// </summary>
    /// <param name="axis">
    /// The axis that changed
    /// </param>
    /// <param name="datafield">
    /// The datafield.
    /// </param>
    /// <param name="unitname">
    /// The unitname.
    /// </param>
    private void UpdateAxisMappings(DataAxis axis, out string datafield, out string unitname)
    {
      switch (axis.Axis)
      {
        case AxisType.I:
          datafield = "Framenumber";
          unitname = string.Empty;
          break;
        case AxisType.T:
          datafield = "Time";
          unitname = Viana.Project.CalibrationData.TimeUnit.ToString();
          break;
        case AxisType.X:
          datafield = "PixelX";
          unitname = Viana.Project.CalibrationData.PixelUnit;
          break;
        case AxisType.Y:
          datafield = "PixelY";
          unitname = Viana.Project.CalibrationData.PixelUnit;
          break;
        case AxisType.PX:
          datafield = "PositionX";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.PY:
          datafield = "PositionY";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.D:
          datafield = "Distance";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.DX:
          datafield = "DistanceX";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.DY:
          datafield = "DistanceY";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.S:
          datafield = "Length";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.SX:
          datafield = "LengthX";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.SY:
          datafield = "LengthY";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.V:
          datafield = "Velocity";
          unitname = Viana.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VX:
          datafield = "VelocityX";
          unitname = Viana.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VY:
          datafield = "VelocityY";
          unitname = Viana.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VI:
          datafield = "VelocityI";
          unitname = Viana.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VXI:
          datafield = "VelocityXI";
          unitname = Viana.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VYI:
          datafield = "VelocityYI";
          unitname = Viana.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.A:
          datafield = "Acceleration";
          unitname = Viana.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AX:
          datafield = "AccelerationX";
          unitname = Viana.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AY:
          datafield = "AccelerationY";
          unitname = Viana.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AI:
          datafield = "AccelerationI";
          unitname = Viana.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AXI:
          datafield = "AccelerationXI";
          unitname = Viana.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AYI:
          datafield = "AccelerationYI";
          unitname = Viana.Project.CalibrationData.AccelerationUnit;
          break;
        default:
          datafield = "PositionX";
          unitname = Viana.Project.CalibrationData.LengthUnit.ToString();
          break;
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
      // this.XAxisOptions.Visibility = Visibility.Visible;
      // this.YAxisOptions.Visibility = Visibility.Visible;
      // this.OtherContentGrid.RowDefinitions[0].Height = GridLength.Auto;

      // this.RegressionSeries.RenderAs = RenderAs.Line;
      // this.TheorySeries.RenderAs = RenderAs.Line;
      // RenderAs? filterStyle = null;

      // if (checkedRadioButton.Name.Contains("Scatter"))
      // {
      // this.DefaultSeries.RenderAs = RenderAs.Point;
      // filterStyle = RenderAs.Line;
      // }
      // else if (checkedRadioButton.Name.Contains("Line"))
      // {
      // this.DefaultSeries.RenderAs = RenderAs.Line;
      // filterStyle = RenderAs.Line;
      // }
      // else if (checkedRadioButton.Name.Contains("Pie"))
      // {
      // this.DefaultSeries.RenderAs = RenderAs.Pie;
      // this.XAxisOptions.Visibility = Visibility.Hidden;
      // this.YAxisOptions.Visibility = Visibility.Hidden;
      // this.OtherContentGrid.RowDefinitions[0].Height = new GridLength(0);
      // }
      // else if (checkedRadioButton.Name.Contains("Column"))
      // {
      // this.DefaultSeries.RenderAs = RenderAs.Column;
      // filterStyle = RenderAs.Line;
      // }
      // else if (checkedRadioButton.Name.Contains("Bubble"))
      // {
      // this.DefaultSeries.RenderAs = RenderAs.Bubble;
      // filterStyle = RenderAs.Line;
      // }
      // else if (checkedRadioButton.Name.Contains("Area"))
      // {
      // this.DefaultSeries.RenderAs = RenderAs.Area;
      // filterStyle = RenderAs.Line;
      // }

      // var enabled = false;
      // if (filterStyle.HasValue)
      // {
      // this.InterpolationSeries.RenderAs = filterStyle.Value;
      // this.RegressionSeries.RenderAs = filterStyle.Value;
      // this.TheorySeries.RenderAs = filterStyle.Value;
      // enabled = true;
      // }

      // Viana.Project.CurrentFilterData.IsShowingInterpolationSeries = enabled;
      // Viana.Project.CurrentFilterData.IsShowingRegressionSeries = enabled;
      // Viana.Project.CurrentFilterData.IsShowingTheorySeries = enabled;
    }

    /// <summary>
    ///   This methods updates the filter series for the currently shown filters
    /// </summary>
    private void UpdateFilters()
    {
      Viana.Project.CurrentFilterData.CalculateInterpolationSeriesDataPoints();
      Viana.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();
      this.RefreshRegressionFuctionTerm();
      Viana.Project.CurrentFilterData.CalculateTheorySeriesDataPoints();
    }

    /// <summary>
    /// This method updates the regression button with
    ///   an image corresponding to the selected regression type
    /// </summary>
    /// <param name="aktregressionType">
    /// The aktual selected regression type.
    /// </param>
    /// <param name="neuBerechnen">
    /// True, wenn die Regression neu berechnet werden soll 
    /// </param>
    private void UpdateRegressionImageButtonAndLabels(RegressionType aktregressionType, bool neuBerechnen)
    {
      string bildsource;
      Viana.Project.CurrentFilterData.RegressionFilter.RegressionType = aktregressionType;
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
        Viana.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();
        Viana.Project.CurrentFilterData.RegressionFilter.UpdateLinefitFunctionData(neuBerechnen);
        this.RefreshRegressionFuctionTerm();
      }
    }

    /// <summary>
    ///   Updates the LaTex display of the theoretical formula
    /// </summary>
    private void UpdateTheoryFormula()
    {
      try
      {
        if (Viana.Project.CurrentFilterData.TheoreticalFunction == null)
        {
          return;
        }

        string functionString = Viana.Project.CurrentFilterData.TheoreticalFunction.Name;
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
        TexFormula formula = this.formulaParser.Parse(functionString);
        if (formula != null)
        {
          Viana.Project.CurrentFilterData.TheoryFunctionTexFormula = formula;
        }

        this.RefreshTheorieFunctionTerm();
      }
      catch (Exception)
      {
        Viana.Project.CurrentFilterData.TheoryFunctionTexFormula = null;
      }
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
      // this.UpdateChartProperties();
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
      this.Refresh();
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
      this.Refresh();
    }

    #endregion

    private void TitleTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.ChartDataModel.Title = this.ChartTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void LegendVisibleCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.IsLegendVisible = true;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void LegendVisibleCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.IsLegendVisible = false;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void LegendTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.ChartDataModel.LegendTitle = this.LegendTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void AxisXTitleTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.XAxis.Title = this.XAxisTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void XAxisShowGridLinesCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.XAxis.MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139);
      this.ChartData.XAxis.MajorGridlineStyle = LineStyle.Solid;
      this.ChartData.XAxis.MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139);
      this.ChartData.XAxis.MinorGridlineStyle = LineStyle.Solid;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void XAxisShowGridLinesCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.XAxis.MajorGridlineStyle = LineStyle.None;
      this.ChartData.XAxis.MinorGridlineStyle = LineStyle.None;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void AxisYTitleTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.YAxis.Title = this.YAxisTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void YAxisShowGridLinesCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.YAxis.MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139);
      this.ChartData.YAxis.MajorGridlineStyle = LineStyle.Solid;
      this.ChartData.YAxis.MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139);
      this.ChartData.YAxis.MinorGridlineStyle = LineStyle.Solid;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    private void YAxisShowGridLinesCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.YAxis.MajorGridlineStyle = LineStyle.None;
      this.ChartData.YAxis.MinorGridlineStyle = LineStyle.None;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Handles the OnClick event of the RescaleAxesButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void RescaleAxesButton_OnClick(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.ResetAllAxes();
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Handles the OnTextChanged event of the DataSeriesTitleTextBox control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
    private void DataSeriesTitleTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.DefaultSeries.Title = this.DataSeriesTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Handles the OnChecked event of the LegendPlacementInsideRadioButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void LegendPlacementInsideRadioButton_OnChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.LegendPlacement = LegendPlacement.Inside;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Handles the OnChecked event of the LegendPlacementOutsideRadioButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void LegendPlacementOutsideRadioButton_OnChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.LegendPlacement = LegendPlacement.Outside;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }
  }
}