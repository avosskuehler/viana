using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using AvalonDock;
using System.Windows.Data;
using WPFLocalizeExtension.Extensions;

namespace VianaNET
{
  public partial class ChartWindow : DockableContent
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private bool isInitialized;

    private ScatterSeries scatterSeries;
    private LineSeries lineSeries;
    private AreaSeries areaSeries;
    private BubbleSeries bubbleSeries;
    private PieSeries pieSeries;
    private ColumnSeries columnSeries;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public ChartWindow()
    {
      InitializeComponent();
      VideoData.Instance.PropertyChanged +=
        new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      CreateDataSeries();
      this.isInitialized = true;
      UpdateChartProperties();
    }

    private void CreateDataSeries()
    {
      this.scatterSeries = new ScatterSeries();
      this.lineSeries = new LineSeries();
      this.pieSeries = new PieSeries();
      this.columnSeries = new ColumnSeries();
      this.bubbleSeries = new BubbleSeries();
      this.areaSeries = new AreaSeries();

      // Set localized Title Binding
      LocTextExtension locTitle = new LocTextExtension("VianaNET:Labels:ChartWindowChartSeries");

      // Set Itemssource Bindung
      Binding itemsSourceBinding = new Binding();
      itemsSourceBinding.Source = VideoData.Instance;
      itemsSourceBinding.Mode = BindingMode.OneWay;
      itemsSourceBinding.Path = new PropertyPath("Samples");

      LinearAxis xAxis = new LinearAxis();
      xAxis.Orientation = AxisOrientation.X;
      LocTextExtension locXAxisTitle = new LocTextExtension("VianaNET:Labels:AxisDistanceX");
      locXAxisTitle.SetBinding(xAxis, LinearAxis.TitleProperty);
      xAxis.TitleStyle = (Style)this.FindResource("VianaAxisLabelStyle");

      LinearAxis yAxis = new LinearAxis();
      yAxis.Orientation = AxisOrientation.Y;
      LocTextExtension locYAxisTitle = new LocTextExtension("VianaNET:Labels:AxisDistanceY");
      locYAxisTitle.SetBinding(yAxis, LinearAxis.TitleProperty);
      yAxis.TitleStyle = (Style)this.FindResource("VianaAxisLabelStyle");

      locTitle.SetBinding(this.scatterSeries, ScatterSeries.TitleProperty);
      this.scatterSeries.IndependentAxis = xAxis;
      this.scatterSeries.DependentRangeAxis = yAxis;
      this.scatterSeries.SetBinding(ScatterSeries.ItemsSourceProperty, itemsSourceBinding);
      this.scatterSeries.IndependentValuePath = "DistanceX";
      this.scatterSeries.DependentValuePath = "DistanceY";
      this.scatterSeries.IsSelectionEnabled = true;

      locTitle.SetBinding(this.lineSeries, LineSeries.TitleProperty);
      this.lineSeries.IndependentAxis = xAxis;
      this.lineSeries.DependentRangeAxis = yAxis;
      this.lineSeries.SetBinding(LineSeries.ItemsSourceProperty, itemsSourceBinding);
      this.lineSeries.IndependentValuePath = "DistanceX";
      this.lineSeries.DependentValuePath = "DistanceY";
      this.lineSeries.IsSelectionEnabled = true;

      locTitle.SetBinding(this.pieSeries, PieSeries.TitleProperty);
      //this.pieSeries.IndependentAxis = xAxis;
      //this.pieSeries.DependentRangeAxis = yAxis;
      this.pieSeries.SetBinding(PieSeries.ItemsSourceProperty, itemsSourceBinding);
      this.pieSeries.IndependentValuePath = "DistanceX";
      this.pieSeries.DependentValuePath = "DistanceY";
      this.pieSeries.IsSelectionEnabled = true;

      locTitle.SetBinding(this.columnSeries, ColumnSeries.TitleProperty);
      this.columnSeries.IndependentAxis = xAxis;
      this.columnSeries.DependentRangeAxis = yAxis;
      this.columnSeries.SetBinding(ColumnSeries.ItemsSourceProperty, itemsSourceBinding);
      this.columnSeries.IndependentValuePath = "DistanceX";
      this.columnSeries.DependentValuePath = "DistanceY";
      this.columnSeries.IsSelectionEnabled = true;

      locTitle.SetBinding(this.bubbleSeries, BubbleSeries.TitleProperty);
      this.bubbleSeries.IndependentAxis = xAxis;
      this.bubbleSeries.DependentRangeAxis = yAxis;
      this.bubbleSeries.SetBinding(BubbleSeries.ItemsSourceProperty, itemsSourceBinding);
      this.bubbleSeries.IndependentValuePath = "DistanceX";
      this.bubbleSeries.DependentValuePath = "DistanceY";
      this.bubbleSeries.IsSelectionEnabled = true;

      locTitle.SetBinding(this.areaSeries, AreaSeries.TitleProperty);
      this.areaSeries.IndependentAxis = xAxis;
      this.areaSeries.DependentRangeAxis = yAxis;
      this.areaSeries.SetBinding(AreaSeries.ItemsSourceProperty, itemsSourceBinding);
      this.areaSeries.IndependentValuePath = "DistanceX";
      this.areaSeries.DependentValuePath = "DistanceY";
      this.areaSeries.IsSelectionEnabled = true;

      this.DataChart.Series.Add(this.scatterSeries);
    }

    void VideoData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      //ScatterSeries data = ((ScatterSeries)this.DataChart.Series[0]);
      //BindingExpression be = data.GetBindingExpression(ScatterSeries.ItemsSourceProperty);
      //be.UpdateTarget();
      //be.UpdateSource();
      //data.Refresh();
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES
    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void Refresh()
    {
      UpdateChartProperties();
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    private void ValueChanged_UpdateChart(object sender, EventArgs e)
    {
      UpdateChartProperties();
    }

    private void xAxisContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateSeriesWithXAxis();
    }

    private void UpdateSeriesWithXAxis()
    {
      DataAxis axis = (DataAxis)xAxisContent.SelectedItem;

      if (this.DataChart.Series.Count == 0)
      {
        return;
      }


      if (this.DataChart.ActualAxes.Count == 0)
      {
        return;
      }

      if (this.DataChart.ActualAxes.Count >= 1)
      {
        LinearAxis xAxis = this.DataChart.ActualAxes[0] as LinearAxis;
        if (xAxis != null)
        {
          xAxis.Title = XAxisTitle.IsChecked ? axis.Description : null;
          XAxisTitle.Text = axis.Description;
        }
      }

      DataPointSeries series = this.DataChart.Series[0] as DataPointSeries;
      switch (axis.Axis)
      {
        case AxisType.I:
          series.IndependentValuePath = "Framenumber";
          break;
        case AxisType.T:
          series.IndependentValuePath = "Timestamp";
          break;
        case AxisType.PX:
          series.IndependentValuePath = "CoordinateX";
          break;
        case AxisType.PY:
          series.IndependentValuePath = "CoordinateY";
          break;
        case AxisType.D:
          series.IndependentValuePath = "Distance";
          break;
        case AxisType.DX:
          series.IndependentValuePath = "DistanceX";
          break;
        case AxisType.DY:
          series.IndependentValuePath = "DistanceY";
          break;
        case AxisType.V:
          series.IndependentValuePath = "Velocity";
          break;
        case AxisType.VX:
          series.IndependentValuePath = "VelocityX";
          break;
        case AxisType.VY:
          series.IndependentValuePath = "VelocityY";
          break;
        case AxisType.A:
          series.IndependentValuePath = "Acceleration";
          break;
        case AxisType.AX:
          series.IndependentValuePath = "AccelerationX";
          break;
        case AxisType.AY:
          series.IndependentValuePath = "AccelerationY";
          break;
      }
    }

    private void yAxisContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateSeriesWithYAxis();
    }

    private void UpdateSeriesWithYAxis()
    {
      DataAxis axis = (DataAxis)yAxisContent.SelectedItem;

      if (this.DataChart.Series.Count == 0)
      {
        return;
      }

      DataPointSeries series = this.DataChart.Series[0] as DataPointSeries;

      if (this.DataChart.ActualAxes.Count >= 2)
      {
        LinearAxis yAxis = this.DataChart.ActualAxes[1] as LinearAxis;
        if (yAxis != null)
        {
          yAxis.Title = YAxisTitle.IsChecked ? axis.Description : null;
          YAxisTitle.Text = axis.Description;
        }
      }

      switch (axis.Axis)
      {
        case AxisType.I:
          series.DependentValuePath = "Framenumber";
          break;
        case AxisType.T:
          series.DependentValuePath = "Timestamp";
          break;
        case AxisType.PX:
          series.DependentValuePath = "CoordinateX";
          break;
        case AxisType.PY:
          series.DependentValuePath = "CoordinateY";
          break;
        case AxisType.D:
          series.DependentValuePath = "Distance";
          break;
        case AxisType.DX:
          series.DependentValuePath = "DistanceX";
          break;
        case AxisType.DY:
          series.DependentValuePath = "DistanceY";
          break;
        case AxisType.V:
          series.DependentValuePath = "Velocity";
          break;
        case AxisType.VX:
          series.DependentValuePath = "VelocityX";
          break;
        case AxisType.VY:
          series.DependentValuePath = "VelocityY";
          break;
        case AxisType.A:
          series.DependentValuePath = "Acceleration";
          break;
        case AxisType.AX:
          series.DependentValuePath = "AccelerationX";
          break;
        case AxisType.AY:
          series.DependentValuePath = "AccelerationY";
          break;
      }
    }

    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS

    private void UpdateChartProperties()
    {
      if (this.isInitialized)
      {
        this.DataChart.Title = ChartTitle.IsChecked ? ChartTitle.Text : null;
        this.DataChart.LegendTitle = LegendTitle.IsChecked ? LegendTitle.Text : null;
        ((VianaChart)this.DataChart).IsShowingLegend =
          this.LegendTitle.IsChecked || this.SeriesTitle.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        foreach (DataPointSeries series in this.DataChart.Series)
        {
          series.Title = SeriesTitle.IsChecked ? SeriesTitle.Text : null;
        }
        //((ScatterSeries)this.DataChart.Series[0]).ItemsSource = null;
        //((ScatterSeries)this.DataChart.Series[0]).ItemsSource = VideoData.Instance.Samples;
        //((ScatterSeries)this.DataChart.Series[0]).Refresh();


        if (this.DataChart.ActualAxes.Count > 0)
        {
          LinearAxis xAxis = this.DataChart.ActualAxes[0] as LinearAxis;
          xAxis.Title = XAxisTitle.IsChecked ? XAxisTitle.Text : null;
          xAxis.ShowGridLines = XAxisShowGridLines.IsChecked();
          var today = DateTime.Now.Date;

          if (null != xAxis)
          {
            if (XAxisMinimum.Value > XAxisMaximum.Value)
            {
              XAxisMinimum.Value = XAxisMaximum.Value;
            }

            xAxis.Minimum = XAxisMinimum.IsChecked ? XAxisMinimum.Value : new double?();

            if (XAxisMaximum.Value < XAxisMinimum.Value)
            {
              XAxisMaximum.Value = XAxisMinimum.Value;
            }

            xAxis.Maximum = XAxisMaximum.IsChecked ? XAxisMaximum.Value : new double?();
            xAxis.Interval = XAxisInterval.IsChecked ? XAxisInterval.Value : new double?();
          }
        }
        if (this.DataChart.ActualAxes.Count > 1)
        {
          LinearAxis yAxis = this.DataChart.ActualAxes[1] as LinearAxis;
          yAxis.Title = YAxisTitle.IsChecked ? YAxisTitle.Text : null;
          yAxis.ShowGridLines = YAxisShowGridLines.IsChecked();
          var today = DateTime.Now.Date;

          if (null != yAxis)
          {
            if (YAxisMinimum.Value > YAxisMaximum.Value)
            {
              YAxisMinimum.Value = YAxisMaximum.Value;
            }

            yAxis.Minimum = YAxisMinimum.IsChecked ? YAxisMinimum.Value : new double?();

            if (YAxisMaximum.Value < YAxisMinimum.Value)
            {
              YAxisMaximum.Value = YAxisMinimum.Value;
            }

            yAxis.Maximum = YAxisMaximum.IsChecked ? YAxisMaximum.Value : new double?();
            yAxis.Interval = YAxisInterval.IsChecked ? YAxisInterval.Value : new double?();
          }
        }
      }
    }

    #endregion //PRIVATEMETHODS

    private void RadioChartStyle_Checked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (e.Source is RadioButton)
      {
        RadioButton checkedRadioButton = e.Source as RadioButton;
        this.DataChart.Series.Clear();
        this.AxisControls.Visibility = Visibility.Visible;
        this.xAxisContentGrid.Visibility = Visibility.Visible;
        if (checkedRadioButton.Name.Contains("Scatter"))
        {
          this.DataChart.Series.Add(this.scatterSeries);
        }
        else if (checkedRadioButton.Name.Contains("Line"))
        {
          this.DataChart.Series.Add(this.lineSeries);
        }
        else if (checkedRadioButton.Name.Contains("Pie"))
        {
          this.DataChart.Series.Add(this.pieSeries);
          this.AxisControls.Visibility = Visibility.Hidden;
          this.xAxisContentGrid.Visibility = Visibility.Hidden;
        }
        else if (checkedRadioButton.Name.Contains("Column"))
        {
          this.DataChart.Series.Add(this.columnSeries);
        }
        else if (checkedRadioButton.Name.Contains("Bubble"))
        {
          this.DataChart.Series.Add(this.bubbleSeries);
        }
        else if (checkedRadioButton.Name.Contains("Area"))
        {
          this.DataChart.Series.Add(this.areaSeries);
        }

        this.RefreshSeries();
      }
    }

    private void RefreshSeries()
    {
      UpdateSeriesWithXAxis();
      UpdateSeriesWithYAxis();
      UpdateChartProperties();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
