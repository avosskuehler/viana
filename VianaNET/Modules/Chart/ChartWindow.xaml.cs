using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using AvalonDock;
using System.Windows.Data;

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

    private bool initialized;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public ChartWindow()
    {
      InitializeComponent();
      this.initialized = true;
      VideoData.Instance.PropertyChanged +=
        new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      UpdateChartProperties();
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
      DataAxis axis = (DataAxis)xAxisContent.SelectedItem;

      if (this.DataChart.Series.Count == 0)
      {
        return;
      }

      ScatterSeries series = this.DataChart.Series[0] as ScatterSeries;

      if (this.DataChart.ActualAxes.Count == 0)
      {
        return;
      }

      LinearAxis xAxis = this.DataChart.ActualAxes[0] as LinearAxis;
      xAxis.Title = XAxisTitle.IsChecked ? axis.Description : null;
      XAxisTitle.Text = axis.Description;

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
      DataAxis axis = (DataAxis)yAxisContent.SelectedItem;

      if (this.DataChart.Series.Count == 0)
      {
        return;
      }

      ScatterSeries series = this.DataChart.Series[0] as ScatterSeries;

      if (this.DataChart.ActualAxes.Count < 2)
      {
        return;
      }

      LinearAxis yAxis = this.DataChart.ActualAxes[1] as LinearAxis;
      yAxis.Title = YAxisTitle.IsChecked ? axis.Description : null;
      YAxisTitle.Text = axis.Description;

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
      if (this.initialized)
      {
        this.DataChart.Title = ChartTitle.IsChecked ? ChartTitle.Text : null;
        this.DataChart.LegendTitle = LegendTitle.IsChecked ? LegendTitle.Text : null;
        ((VianaChart)this.DataChart).IsShowingLegend = ShowLegend.IsChecked() ? Visibility.Visible : Visibility.Collapsed;
        this.DataPointsPanel.Visibility = ShowLegend.IsChecked() ? Visibility.Visible : Visibility.Collapsed;
        this.LegendTitle.Visibility = ShowLegend.IsChecked() ? Visibility.Visible : Visibility.Collapsed;
        ((Series)this.DataChart.Series[0]).Title = SeriesTitle.IsChecked ? SeriesTitle.Text : null;
        ((ScatterSeries)this.DataChart.Series[0]).IsSelectionEnabled = SelectionEnabled.IsChecked();
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

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
