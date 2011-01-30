using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AvalonDock;
using System.Windows.Data;
using WPFLocalizeExtension.Extensions;
using Visifire.Charts;

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
      // Set localized Title Binding
      LocTextExtension locTitle = new LocTextExtension("VianaNET:Labels:ChartWindowChartSeries");

      this.DefaultSeries.MovingMarkerEnabled = true;
      this.DefaultSeries.SelectionEnabled = true;
      this.DefaultSeries.SelectionMode = SelectionModes.Multiple;
      this.DefaultSeries.ShowInLegend = true;
      this.DefaultSeries.RenderAs = RenderAs.Point;
    }

    void VideoData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Interpolation")
      {
        this.Refresh();
      }

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
      RefreshSeries();
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

    private void AxesContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SetDefaultDiagramm();
    }

    private void SetDefaultDiagramm()
    {
      if (!this.IsInitialized)
      {
        return;
      }

      AxisType chartType = AxisType.YoverX;

      if (this.TabOrtsraum.IsSelected)
      {
        DataAxis axis = (DataAxis)this.AxesContentOrtsraum.SelectedItem;
        chartType = axis.Axis;
      }
      else if (this.TabPhasenraum.IsSelected)
      {
        DataAxis axis = (DataAxis)this.AxesContentPhasenraum.SelectedItem;
        chartType = axis.Axis;
      }
      else if (this.TabOther.IsSelected)
      {
        DataAxis xAxis = (DataAxis)xAxisContent.SelectedItem;
        DataAxis yAxis = (DataAxis)yAxisContent.SelectedItem;
        xAxisContent.SelectedValue = xAxis.Axis;
        yAxisContent.SelectedValue = yAxis.Axis;

        // axes content already set, so return
        return;
      }

      switch (chartType)
      {
        case AxisType.YoverX:
          xAxisContent.SelectedValue = AxisType.DX;
          yAxisContent.SelectedValue = AxisType.DY;
          break;
        case AxisType.XoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.PX;
          break;
        case AxisType.YoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.PY;
          break;
        case AxisType.VoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.V;
          break;
        case AxisType.VXoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.VX;
          break;
        case AxisType.VYoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.VY;
          break;
        case AxisType.AoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.A;
          break;
        case AxisType.AXoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.AX;
          break;
        case AxisType.AYoverT:
          xAxisContent.SelectedValue = AxisType.T;
          yAxisContent.SelectedValue = AxisType.AY;
          break;
        case AxisType.VoverD:
          xAxisContent.SelectedValue = AxisType.D;
          yAxisContent.SelectedValue = AxisType.V;
          break;
        case AxisType.VXoverDX:
          xAxisContent.SelectedValue = AxisType.DX;
          yAxisContent.SelectedValue = AxisType.VX;
          break;
        case AxisType.VYoverDY:
          xAxisContent.SelectedValue = AxisType.DY;
          yAxisContent.SelectedValue = AxisType.VY;
          break;
        case AxisType.VoverS:
          xAxisContent.SelectedValue = AxisType.S;
          yAxisContent.SelectedValue = AxisType.V;
          break;
        case AxisType.VXoverSX:
          xAxisContent.SelectedValue = AxisType.SX;
          yAxisContent.SelectedValue = AxisType.VX;
          break;
        case AxisType.VYoverSY:
          xAxisContent.SelectedValue = AxisType.SY;
          yAxisContent.SelectedValue = AxisType.VY;
          break;
        case AxisType.AoverV:
          xAxisContent.SelectedValue = AxisType.V;
          yAxisContent.SelectedValue = AxisType.A;
          break;
        case AxisType.AXoverVX:
          xAxisContent.SelectedValue = AxisType.VX;
          yAxisContent.SelectedValue = AxisType.AX;
          break;
        case AxisType.AYoverVY:
          xAxisContent.SelectedValue = AxisType.VY;
          yAxisContent.SelectedValue = AxisType.AY;
          break;
      }
    }


    private void UpdateSeriesWithXAxis()
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (this.DataChart.Series.Count == 0)
      {
        return;
      }

      if (this.DataChart.AxesX.Count == 0)
      {
        return;
      }

      DataAxis axis = (DataAxis)xAxisContent.SelectedItem;

      if (this.DataChart.AxesX.Count >= 1)
      {
        this.DataChart.AxesX[0].Title = XAxisTitle.IsChecked ? axis.Description : null;
        XAxisTitle.Text = axis.Description;
      }

      DataMapping map = this.DefaultSeries.DataMappings[0];
      DataMapping map2 = this.InterpolationSeries.DataMappings[0];

      UpdateAxisMappings(axis, map, map2);

      RefreshChartDataPoints();
    }

    private static void UpdateAxisMappings(DataAxis axis, DataMapping map, DataMapping map2)
    {
      switch (axis.Axis)
      {
        case AxisType.I:
          map.Path = "Framenumber";
          map2.Path = "Framenumber";
          break;
        case AxisType.T:
          map.Path = "Timestamp";
          map2.Path = "Timestamp";
          break;
        case AxisType.PX:
          map.Path = "CoordinateX";
          map2.Path = "CoordinateX";
          break;
        case AxisType.PY:
          map.Path = "CoordinateY";
          map2.Path = "CoordinateY";
          break;
        case AxisType.D:
          map.Path = "Distance";
          map2.Path = "Distance";
          break;
        case AxisType.DX:
          map.Path = "DistanceX";
          map2.Path = "DistanceX";
          break;
        case AxisType.DY:
          map.Path = "DistanceY";
          map2.Path = "DistanceY";
          break;
        case AxisType.V:
          map.Path = "Velocity";
          map2.Path = "VelocityI";
          break;
        case AxisType.VX:
          map.Path = "VelocityX";
          map2.Path = "VelocityXI";
          break;
        case AxisType.VY:
          map.Path = "VelocityY";
          map2.Path = "VelocityYI";
          break;
        case AxisType.VI:
          map.Path = "VelocityI";
          map2.Path = "VelocityI";
          break;
        case AxisType.VXI:
          map.Path = "VelocityXI";
          map2.Path = "VelocityXI";
          break;
        case AxisType.VYI:
          map.Path = "VelocityYI";
          map2.Path = "VelocityYI";
          break;
        case AxisType.A:
          map.Path = "Acceleration";
          map2.Path = "AccelerationI";
          break;
        case AxisType.AX:
          map.Path = "AccelerationX";
          map2.Path = "AccelerationXI";
          break;
        case AxisType.AY:
          map.Path = "AccelerationY";
          map2.Path = "AccelerationYI";
          break;
        case AxisType.AI:
          map.Path = "AccelerationI";
          map2.Path = "AccelerationI";
          break;
        case AxisType.AXI:
          map.Path = "AccelerationXI";
          map2.Path = "AccelerationXI";
          break;
        case AxisType.AYI:
          map.Path = "AccelerationYI";
          map2.Path = "AccelerationYI";
          break;
      }
    }

    private void RefreshChartDataPoints()
    {
      this.DefaultSeries.DataSource = null;
      this.DefaultSeries.DataSource = VideoData.Instance.Samples;
      this.InterpolationSeries.DataSource = null;
      this.InterpolationSeries.DataSource = VideoData.Instance.Samples;
    }

    private void yAxisContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateSeriesWithYAxis();
    }

    private void UpdateSeriesWithYAxis()
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (this.DataChart.Series.Count == 0)
      {
        return;
      }

      DataAxis axis = (DataAxis)yAxisContent.SelectedItem;
      this.SeriesTitle.Text = axis.Description;

      if (this.DataChart.AxesY.Count >= 1)
      {
        this.DataChart.AxesY[0].Title = YAxisTitle.IsChecked ? axis.Description : null;
        YAxisTitle.Text = axis.Description;
      }

      DataMapping map = this.DefaultSeries.DataMappings[1];
      DataMapping map2 = this.InterpolationSeries.DataMappings[1];
      UpdateAxisMappings(axis, map, map2);

      RefreshChartDataPoints();
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
        this.DataChart.Titles[0].Text = ChartTitle.IsChecked ? ChartTitle.Text : null;
        this.DataChart.Legends[0].Title = LegendTitle.IsChecked ? LegendTitle.Text : null;
        this.DataChart.Legends[0].Enabled = this.LegendTitle.IsChecked || this.SeriesTitle.IsChecked ? true : false;
        this.DefaultSeries.LegendText = SeriesTitle.IsChecked ? SeriesTitle.Text : null;

        if (this.DataChart.AxesX.Count > 0)
        {
          Axis xAxis = this.DataChart.AxesX[0] as Axis;
          xAxis.Title = XAxisTitle.IsChecked ? XAxisTitle.Text : null;
          xAxis.Grids[0].Enabled = XAxisShowGridLines.IsChecked();

          if (null != xAxis)
          {
            if (XAxisMinimum.Value > XAxisMaximum.Value)
            {
              XAxisMinimum.Value = XAxisMaximum.Value;
            }

            xAxis.AxisMinimum = XAxisMinimum.IsChecked ? XAxisMinimum.Value : new double?();

            if (XAxisMaximum.Value < XAxisMinimum.Value)
            {
              XAxisMaximum.Value = XAxisMinimum.Value;
            }

            xAxis.AxisMaximum = XAxisMaximum.IsChecked ? XAxisMaximum.Value : new double?();
            if (XAxisInterval.IsChecked)
            {
              xAxis.Interval = XAxisInterval.Value;
            }
            else
            {
              xAxis.Interval = double.NaN;
            }
          }
        }

        if (this.DataChart.AxesY.Count > 0)
        {
          Axis yAxis = this.DataChart.AxesY[0] as Axis;
          yAxis.Title = YAxisTitle.IsChecked ? YAxisTitle.Text : null;
          yAxis.Grids[0].Enabled = YAxisShowGridLines.IsChecked();

          if (null != yAxis)
          {
            if (YAxisMinimum.Value > YAxisMaximum.Value)
            {
              YAxisMinimum.Value = YAxisMaximum.Value;
            }

            yAxis.AxisMinimum = YAxisMinimum.IsChecked ? YAxisMinimum.Value : new double?();

            if (YAxisMaximum.Value < YAxisMinimum.Value)
            {
              YAxisMaximum.Value = YAxisMinimum.Value;
            }

            yAxis.AxisMaximum = YAxisMaximum.IsChecked ? YAxisMaximum.Value : new double?();
            if (YAxisInterval.IsChecked)
            {
              yAxis.Interval = YAxisInterval.Value;
            }
            else
            {
              yAxis.Interval = double.NaN;
            }
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
        UpdateChartStyle(checkedRadioButton);
        this.RefreshSeries();
      }
    }

    private void UpdateChartStyle(RadioButton checkedRadioButton)
    {
      this.AxisControls.Visibility = Visibility.Visible;
      this.OtherContentGrid.RowDefinitions[0].Height = GridLength.Auto;
      //this.InterpolationLineCheckBox.Visibility = Visibility.Visible;

      if (checkedRadioButton.Name.Contains("Scatter"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Point;
        this.InterpolationSeries.RenderAs = RenderAs.Spline;
      }
      else if (checkedRadioButton.Name.Contains("Line"))
      {
        if (Interpolation.Instance.IsInterpolatingData)
        {
          this.DefaultSeries.RenderAs = RenderAs.Point;
          this.InterpolationSeries.RenderAs = RenderAs.Spline;
        }
        else
        {
          this.DefaultSeries.RenderAs = RenderAs.Spline;
        }
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
          this.InterpolationSeries.RenderAs = RenderAs.Spline;
        }
        else
        {
          this.DefaultSeries.RenderAs = RenderAs.Column;
        }
      }
      else if (checkedRadioButton.Name.Contains("Bubble"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Bubble;
        this.InterpolationSeries.RenderAs = RenderAs.Spline;
      }
      else if (checkedRadioButton.Name.Contains("Area"))
      {
        if (Interpolation.Instance.IsInterpolatingData)
        {
          this.DefaultSeries.RenderAs = RenderAs.Area;
          this.InterpolationSeries.RenderAs = RenderAs.Spline;
        }
        else
        {
          this.DefaultSeries.RenderAs = RenderAs.Area;
        }
      }
    }

    private void RefreshSeries()
    {
      UpdateSeriesWithXAxis();
      UpdateSeriesWithYAxis();
      UpdateChartProperties();
    }

    private void InterpolationLineCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      this.InterpolationSeries.Enabled = Interpolation.Instance.IsInterpolatingData;

      // Update static property
      //Interpolation.Instance.IsInterpolatingData = this.InterpolationLineCheckBox.IsChecked.Value;

      if (this.RadioChartStyleScatter.IsChecked.Value)
      {
        this.UpdateChartStyle(this.RadioChartStyleScatter);
      }
      else if (this.RadioChartStyleLine.IsChecked.Value)
      {
        this.UpdateChartStyle(this.RadioChartStyleLine);
      }
      else if (this.RadioChartStyleArea.IsChecked.Value)
      {
        this.UpdateChartStyle(this.RadioChartStyleArea);
      }
      else if (this.RadioChartStyleColumn.IsChecked.Value)
      {
        this.UpdateChartStyle(this.RadioChartStyleColumn);
      }
      else if (this.RadioChartStyleBubble.IsChecked.Value)
      {
        this.UpdateChartStyle(this.RadioChartStyleBubble);
      }
      else if (this.RadioChartStylePie.IsChecked.Value)
      {
        this.UpdateChartStyle(this.RadioChartStylePie);
      }
    }

    private void AxesExpander_Collapsed(object sender, RoutedEventArgs e)
    {
      SetDefaultDiagramm();
    }

    private void InterpolationOptionsButton_Click(object sender, RoutedEventArgs e)
    {
      Interpolation.ShowInterpolationOptionsDialog();
    }

    private void ChartContentTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SetDefaultDiagramm();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
