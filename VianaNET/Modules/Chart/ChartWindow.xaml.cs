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
  using VianaNET.Data.Interpolation;
  using VianaNET.Data.Linefit;
  using VianaNET.Localization;
  using VianaNET.Modules.Video.Control;
  using Visifire.Charts;

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
      Video.Instance.ImageProcessing.PropertyChanged += this.ImageProcessingPropertyChanged;
      this.isInitialized = true;
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
      this.UpdateChartProperties();
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
    private void ImageProcessingPropertyChanged(object sender, PropertyChangedEventArgs e)
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

    ///// <summary>
    ///// The interpolation line check box checked.
    ///// </summary>
    ///// <param name="sender">
    ///// The sender. 
    ///// </param>
    ///// <param name="e">
    ///// The e. 
    ///// </param>
    //private void InterpolationLineCheckBoxChecked(object sender, RoutedEventArgs e)
    //{
    //  if (!this.isInitialized)
    //  {
    //    return;
    //  }

    //  this.InterpolationSeries.Enabled = Interpolation.Instance.IsInterpolatingData;

    //  //if (this.RadioChartStyleScatter.IsChecked())
    //  //{
    //  //  this.UpdateChartStyle(this.RadioChartStyleScatter);
    //  //}
    //  //else if (this.RadioChartStyleLine.IsChecked())
    //  //{
    //  //  this.UpdateChartStyle(this.RadioChartStyleLine);
    //  //}
    //  //else if (this.RadioChartStyleArea.IsChecked())
    //  //{
    //  //  this.UpdateChartStyle(this.RadioChartStyleArea);
    //  //}
    //  //else if (this.RadioChartStyleColumn.IsChecked())
    //  //{
    //  //  this.UpdateChartStyle(this.RadioChartStyleColumn);
    //  //}
    //  //else if (this.RadioChartStyleBubble.IsChecked())
    //  //{
    //  //  this.UpdateChartStyle(this.RadioChartStyleBubble);
    //  //}
    //  //else if (this.RadioChartStylePie.IsChecked())
    //  //{
    //  //  this.UpdateChartStyle(this.RadioChartStylePie);
    //  //}
    //}

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
      Interpolation.ShowInterpolationOptionsDialog();
    }

    ///// <summary>
    ///// The line fit check box checked.
    ///// </summary>
    ///// <param name="sender">
    ///// The sender. 
    ///// </param>
    ///// <param name="e">
    ///// The e. 
    ///// </param>
    //private void LineFitCheckBoxChecked(object sender, RoutedEventArgs e)
    //{
    //  if (!this.isInitialized)
    //  {
    //    return;
    //  }

    //  if (this.LineFitCheckBox.IsChecked.GetValueOrDefault(false))
    //  {
    //    this.NewCalculationForLineFitting();
    //  }
    //  else
    //  {
    //    FittedData.Instance.LineFitObject.LineFitDisplaySample.Clear();
    //    this.LinefitFunctionLabel.Content = string.Empty;
    //  }

    //  this.RefreshSeries();
    //}

    /// <summary>
    /// The line fit options button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LinefitPrecisionButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new NumericalPrecisionDialog { NumberOfDigits = FittedData.Instance.NumericPrecision };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
          FittedData.Instance.NumericPrecision = dlg.NumberOfDigits;
          if (LineFitCheckBox.IsChecked())
          {
              LinefitFunctionLabel.Content = FittedData.Instance.RegressionFunctionString;
          }
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
    private void LineFitTypeButtonClick(object sender, RoutedEventArgs e)
    {
      double minX, minY, hilf;

      FittedData.Instance.LineFitObject.GetMinMax(FittedData.Instance.LineFitObject.WertX, out minX, out hilf);
      FittedData.Instance.LineFitObject.GetMinMax(FittedData.Instance.LineFitObject.WertY, out minY, out hilf);
      var linefittingDialog = new LinefittingDialog(minX < 0, minY < 0, FittedData.Instance.RegressionType);
      if (linefittingDialog.ShowDialog().GetValueOrDefault(false))
      {
        FittedData.Instance.RegressionType = linefittingDialog.SelectedRegressionType;
        string bildsource;
        switch (FittedData.Instance.RegressionType)
        {
          case Regression.Linear:
            bildsource = "/VianaNET;component/Images/LineFit_Linear_16.png";
            break;
          case Regression.Exponentiell:
            bildsource = "/VianaNET;component/Images/LineFit_Exponential1_16.png";
            break;
          case Regression.Logarithmisch:
            bildsource = "/VianaNET;component/Images/LineFit_Logarithmus_16.png";
            break;
          case Regression.Potenz:
            bildsource = "/VianaNET;component/Images/LineFit_Potentiell_16.png";
            break;
          case Regression.Quadratisch:
            bildsource = "/VianaNET;component/Images/LineFit_Quadratisch_16.png";
            break;
          case Regression.ExponentiellMitKonstante:
            bildsource = "/VianaNET;component/Images/LineFit_Exponential2_16.png";
            break;
          case Regression.Sinus:
            bildsource = "/VianaNET;component/Images/LineFit_Sinus_16.png";
            break;
          case Regression.SinusGedämpft:
            bildsource = "/VianaNET;component/Images/LineFit_SinusExponential_16.png";
            break;
          case Regression.Resonanz:
            bildsource = "/VianaNET;component/Images/LineFit_Resonanz_16.png";
            break;
          default:
            bildsource = "/VianaNET;component/Images/LineFit_Linear_16.png";
            break;
        }

        var neuBildsource = new Uri(bildsource, UriKind.RelativeOrAbsolute);
        this.LineFitTypeButton.ImageSource = new BitmapImage(neuBildsource);

        if (this.LineFitCheckBox.IsChecked.GetValueOrDefault(false))
        {
          FittedData.Instance.LineFitObject.CalculateLineFitFunction(FittedData.Instance.RegressionType);
          LinefitFunctionLabel.Content = FittedData.Instance.RegressionFunctionString;
          LinefitAberationLabel.Content = FittedData.Instance.RegressionAberrationString;
        }
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
    private void LineFitTheorieButtonClick(object sender, RoutedEventArgs e)
    {
      var fktEditor = new CalculatorAndFktEditor(TRechnerArt.formelRechner);
      if (FittedData.Instance.TheoreticalFunction != null)
      {
        fktEditor.textBox1.Text = FittedData.Instance.TheoreticalFunction.Name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        FittedData.Instance.TheoreticalFunction = fktEditor.GetFunktion();
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
      Video.Instance.ImageProcessing.IndexOfObject = int.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
      VideoData.Instance.ActiveObject = Video.Instance.ImageProcessing.IndexOfObject;
    }

    /// <summary>
    ///   The populate object combo.
    /// </summary>
    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      // this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      var indexBinding = new Binding("ImageProcessing.IndexOfObject") { Source = Video.Instance };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      Video.Instance.ImageProcessing.IndexOfObject++;
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
      //if (FittedData.Instance.IsShowingRegressionSeries)
      //{
      //  this.NewCalculationForLineFitting();
      //}

      this.DefaultSeries.DataSource = null;
      this.DefaultSeries.DataSource = VideoData.Instance.Samples;

      //this.InterpolationSeries.DataSource = null;
      //this.InterpolationSeries.DataSource = VideoData.Instance.Samples;
      //this.LineFitSeries.DataSource = null;
      //this.TheorieSeries.DataSource = null;
      //if ((FittedData.Instance.LineFitObject != null) & FittedData.Instance.IsInterpolationAllowed)
      //{
      //  if ((FittedData.Instance.LineFitObject.LineFitDisplaySample != null)
      //      && (FittedData.Instance.LineFitObject.LineFitDisplaySample.Count > 0))
      //  {
      //    this.LineFitSeries.DataSource = FittedData.Instance.LineFitObject.LineFitDisplaySample;
      //  }

      //  if ((FittedData.Instance.LineFitObject.TheorieDisplaySample != null)
      //      && (FittedData.Instance.LineFitObject.TheorieDisplaySample.Count > 0))
      //  {
      //    this.TheorieSeries.DataSource = FittedData.Instance.LineFitObject.TheorieDisplaySample;
      //  }
      //}
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
      this.UpdateChartProperties();
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

        // axes content already set, so return
        return;
      }

      switch (chartType)
      {
        case ChartType.YoverX:
          this.xAxisContent.SelectedValue = AxisType.PX;
          this.yAxisContent.SelectedValue = AxisType.PY;
          break;
        case ChartType.XoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.PX;
          break;
        case ChartType.YoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.PY;
          break;
        case ChartType.VoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.V;
          break;
        case ChartType.VXoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.VX;
          break;
        case ChartType.VYoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.VY;
          break;
        case ChartType.AoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.A;
          break;
        case ChartType.AXoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.AX;
          break;
        case ChartType.AYoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.AY;
          break;
        case ChartType.VoverD:
          this.xAxisContent.SelectedValue = AxisType.D;
          this.yAxisContent.SelectedValue = AxisType.V;
          break;
        case ChartType.VXoverDX:
          this.xAxisContent.SelectedValue = AxisType.DX;
          this.yAxisContent.SelectedValue = AxisType.VX;
          break;
        case ChartType.VYoverDY:
          this.xAxisContent.SelectedValue = AxisType.DY;
          this.yAxisContent.SelectedValue = AxisType.VY;
          break;
        case ChartType.VoverS:
          this.xAxisContent.SelectedValue = AxisType.S;
          this.yAxisContent.SelectedValue = AxisType.V;
          break;
        case ChartType.VXoverSX:
          this.xAxisContent.SelectedValue = AxisType.SX;
          this.yAxisContent.SelectedValue = AxisType.VX;
          break;
        case ChartType.VYoverSY:
          this.xAxisContent.SelectedValue = AxisType.SY;
          this.yAxisContent.SelectedValue = AxisType.VY;
          break;
        case ChartType.AoverV:
          this.xAxisContent.SelectedValue = AxisType.V;
          this.yAxisContent.SelectedValue = AxisType.A;
          break;
        case ChartType.AXoverVX:
          this.xAxisContent.SelectedValue = AxisType.VX;
          this.yAxisContent.SelectedValue = AxisType.AX;
          break;
        case ChartType.AYoverVY:
          this.xAxisContent.SelectedValue = AxisType.VY;
          this.yAxisContent.SelectedValue = AxisType.AY;
          break;
      }
      if ((FittedData.Instance.AxisX != (DataAxis)this.xAxisContent.SelectedItem) || (FittedData.Instance.AxisY != (DataAxis)this.yAxisContent.SelectedItem))
      {
          FittedData.Instance.AxisX = (DataAxis)this.xAxisContent.SelectedItem;
          FittedData.Instance.AxisY = (DataAxis)this.yAxisContent.SelectedItem;
          LineFitCheckBoxUnchecked(null, null);
      }
    }

    /// <summary>
    /// The update axis mappings.
    /// </summary>
    /// <param name="axis">
    /// The axis. 
    /// </param>
    /// <param name="mapPoints">
    /// The map points. 
    /// </param>
    /// <param name="mapInterpolationFit">
    /// The map interpolation fit. 
    /// </param>
    /// <param name="mapLineFit">
    /// The map line fit. 
    /// </param>
    /// <param name="mapTheorieFit">
    /// The map theorie fit. 
    /// </param>
    private void UpdateAxisMappings( DataAxis axis, DataMapping mapPoints)
   /* private void UpdateAxisMappings(
      DataAxis axis,
      DataMapping mapPoints,
      DataMapping mapInterpolationFit,
      DataMapping mapLineFit,
      DataMapping mapTheorieFit)  //Parameter mapInterpolationFit,mapLineFit und mapTheorieFit überflüssig */
    {
      string prefix = "Object[" + Video.Instance.ImageProcessing.IndexOfObject.ToString(CultureInfo.InvariantCulture)
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

      if (axis.Axis != AxisType.T)       // Don´t prefix the timestamp
      {
          mapPoints.Path = prefix + mapPoints.Path;
      }

 /*   mapPoints.Path = prefix + mapPoints.Path;

      // Don´t prefix the timestamp
      if (axis.Axis == AxisType.T)
      {
        mapPoints.Path = "Timestamp";
      }
*/   
      
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

      FittedData.Instance.IsInterpolationAllowed = false;
      this.LineFitSeries.RenderAs = RenderAs.Line;
      this.TheorieSeries.RenderAs = RenderAs.Line;

      if (checkedRadioButton.Name.Contains("Scatter"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Point;
        this.InterpolationSeries.RenderAs = RenderAs.Line;
        FittedData.Instance.IsInterpolationAllowed = true;
      }
      else if (checkedRadioButton.Name.Contains("Line"))
      {
        if (Interpolation.Instance.IsInterpolatingData)
        {
          this.DefaultSeries.RenderAs = RenderAs.Point;
          this.InterpolationSeries.RenderAs = RenderAs.Line;
        }
        else
        {
          this.DefaultSeries.RenderAs = RenderAs.Line;
        }

        FittedData.Instance.IsInterpolationAllowed = true;
      }
      else if (checkedRadioButton.Name.Contains("Pie"))
      {
        Interpolation.Instance.IsInterpolatingData = false;
        this.DefaultSeries.RenderAs = RenderAs.Pie;
        this.AxisControls.Visibility = Visibility.Hidden;
        this.OtherContentGrid.RowDefinitions[0].Height = new GridLength(0);
      }
      else if (checkedRadioButton.Name.Contains("Column"))
      {
        if (Interpolation.Instance.IsInterpolatingData)
        {
          this.DefaultSeries.RenderAs = RenderAs.Column;
          this.InterpolationSeries.RenderAs = RenderAs.Line;
        }
        else
        {
          this.DefaultSeries.RenderAs = RenderAs.Column;
        }
      }
      else if (checkedRadioButton.Name.Contains("Bubble"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Bubble;
        this.InterpolationSeries.RenderAs = RenderAs.Line;
      }
      else if (checkedRadioButton.Name.Contains("Area"))
      {
        if (Interpolation.Instance.IsInterpolatingData)
        {
          this.DefaultSeries.RenderAs = RenderAs.Area;
          this.InterpolationSeries.RenderAs = RenderAs.Line;
        }
        else
        {
          this.DefaultSeries.RenderAs = RenderAs.Area;
        }
      }
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

      DataMapping map = this.DefaultSeries.DataMappings[axisX ? 0 : 1];
   //   DataMapping mapInterpolationFit = this.InterpolationSeries.DataMappings[axisX ? 0 : 1];
   //   DataMapping mapLineFit = this.LineFitSeries.DataMappings[axisX ? 0 : 1];
   //   DataMapping mapTheorieFit = this.TheorieSeries.DataMappings[axisX ? 0 : 1];
      this.UpdateAxisMappings(axis, map);
   //   this.UpdateAxisMappings(axis, map, mapInterpolationFit, mapLineFit, mapTheorieFit);
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
      this.PopulateAxesFromChartSelection();

      // This updates the xAxis mapping for the data series
      if (!this.UpdateMapping(true))
      {
        return;
      }

      // This updates the yAxis mapping for the data series
      if (!this.UpdateMapping(false))
      {
        return;
      }

      this.RefreshChartDataPoints();
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

      this.RefreshChartDataPoints();
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

      this.RefreshChartDataPoints();
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
    private void CheckBoxShowTheorieChecked(object sender, RoutedEventArgs e)
    {
      FittedData.Instance.IsShowingTheorySeries = true;
    }

    private void CheckBoxShowTheorieUnchecked(object sender, RoutedEventArgs e)
    {
      FittedData.Instance.IsShowingTheorySeries = false;
    }

    private void LineFitCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      FittedData.Instance.IsShowingRegressionSeries = true;

     // Funktionsterm und mittleres Fehlerquadrat anzeigen
      LinefitFunctionLabel.Content = FittedData.Instance.RegressionFunctionString;
      LinefitAberationLabel.Content = FittedData.Instance.RegressionAberrationString;
    }

    private void LineFitCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
    //  FittedData.Instance.RegressionSeries.Clear();
      FittedData.Instance.IsShowingRegressionSeries = false;
      
    // Funktionsterm und mittleres Fehlerquadrat nicht mehr anzeigen
      FittedData.Instance.RegressionFunctionString = string.Empty;
      LinefitFunctionLabel.Content = string.Empty;
      LinefitAberationLabel.Content = string.Empty;
    }

    private void InterpolationLineCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      FittedData.Instance.IsShowingInterpolationSeries = true;
    }

    private void InterpolationLineCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      FittedData.Instance.IsShowingInterpolationSeries = false;
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FittedData.Instance.RegressionLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FittedData.Instance.RegressionLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FittedData.Instance.RegressionLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FittedData.Instance.RegressionLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FittedData.Instance.TheoryLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FittedData.Instance.TheoryLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FittedData.Instance.TheoryLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FittedData.Instance.TheoryLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FittedData.Instance.DataLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FittedData.Instance.DataLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FittedData.Instance.DataLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FittedData.Instance.DataLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
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
      lineOptionsDialog.LineStyleControl.ThicknessSlider.Value = FittedData.Instance.InterpolationLineThickness;
      lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor = FittedData.Instance.InterpolationLineColor.Color;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        FittedData.Instance.InterpolationLineThickness = lineOptionsDialog.LineStyleControl.ThicknessSlider.Value;
        FittedData.Instance.InterpolationLineColor = new SolidColorBrush(lineOptionsDialog.LineStyleControl.ColorPicker.SelectedColor);
      }
    }
    #endregion



  }
}