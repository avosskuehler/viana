namespace VianaNET
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Media.Imaging;

  using AvalonDock;
  using System.Windows.Data;
  using WPFLocalizeExtension.Extensions;
  using Visifire.Charts;
  using System.Collections.Generic;
  using VianaNET.Data.Linefit;
  using Parser;

    //letzte Änderung: 9.9.2012

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

    private readonly bool isInitialized;
    private LineFitClass activeLineFitClass;
    private TFktTerm lineFitTheorieFunction;
    private int activeRegressionType;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public ChartWindow()
    {
      InitializeComponent();
      this.ObjectSelectionCombo.DataContext = this;
      this.PopulateObjectCombo();
      activeLineFitClass = null;
      //VideoData.Instance.PropertyChanged +=
      //  new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      //Calibration.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      Video.Instance.ImageProcessing.PropertyChanged += this.ImageProcessingPropertyChanged;
      CreateDataSeries();
      this.isInitialized = true;
      UpdateChartProperties();
    }

    private void ImageProcessingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

    //void VideoData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //  if (e.PropertyName == "Interpolation")
    //  {
    //    this.Refresh();
    //  }
    //}

    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(Localization.Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      //this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      var indexBinding = new Binding("ImageProcessing.IndexOfObject") { Source = Video.Instance };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      Video.Instance.ImageProcessing.IndexOfObject++;
    }

    private void CreateDataSeries()
    {
      // Set localized Title Binding
      var locTitle = new LocTextExtension("VianaNET:Labels:ChartWindowChartSeries");

      this.DefaultSeries.MovingMarkerEnabled = true;
      this.DefaultSeries.SelectionEnabled = true;
      this.DefaultSeries.SelectionMode = SelectionModes.Multiple;
      this.DefaultSeries.ShowInLegend = true;
      this.DefaultSeries.RenderAs = RenderAs.Point;
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

    /// <summary>
    /// Gets or sets the index of the currently tracked object
    /// </summary>
    public List<string> ObjectDescriptions
    {
      get { return (List<string>)this.GetValue(ObjectDescriptionsProperty); }
      set { this.SetValue(ObjectDescriptionsProperty, value); }
    }

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the property <see cref="ObjectDescriptions"/>.
    /// </summary>
    public static readonly DependencyProperty ObjectDescriptionsProperty = DependencyProperty.Register(
      "ObjectDescriptions",
      typeof(List<string>),
      typeof(ChartWindow),
      new FrameworkPropertyMetadata(new List<string>(), OnPropertyChanged));

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

    /// <summary>
    /// Raises the <see cref="ManagedContent.PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">The source of the event. This.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> with 
    /// the event data.</param>
    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      var window = obj as ChartWindow;
      if (window != null)
      {
        window.RefreshSeries();
      }
    }

    private void ValueChangedUpdateChart(object sender, EventArgs e)
    {
      UpdateChartProperties();
    }

    private void XAxisContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateSeriesWithXAxis();
    }

    private void AxesContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if ( (LineFitCheckBox != null) && (LineFitCheckBox.IsChecked.GetValueOrDefault(false)) )
        { NewCalculationForLineFitting(); }
      SetDefaultDiagramm();
    }

    private void SetDefaultDiagramm()
    {
      if (!this.IsInitialized)
      {
        return;
      }

      AxisType chartType = AxisType.YoverX;

      if (this.TabPositionSpace.IsSelected)
      {
        var axis = (DataAxis)this.AxesContentPositionSpace.SelectedItem;
        chartType = axis.Axis;
      }
      else if (this.TabPhaseSpace.IsSelected)
      {
        var axis = (DataAxis)this.AxesContentPhaseSpace.SelectedItem;
        chartType = axis.Axis;
      }
      else if (this.TabOther.IsSelected)
      {
        var xAxis = (DataAxis)xAxisContent.SelectedItem;
        var yAxis = (DataAxis)yAxisContent.SelectedItem;
        xAxisContent.SelectedValue = xAxis.Axis;
        yAxisContent.SelectedValue = yAxis.Axis;

        // axes content already set, so return
        return;
      }

      switch (chartType)
      {
        case AxisType.YoverX:
          xAxisContent.SelectedValue = AxisType.PX;
          yAxisContent.SelectedValue = AxisType.PY;
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

      var axis = (DataAxis)xAxisContent.SelectedItem;

      if (this.DataChart.AxesX.Count >= 1)
      {
        this.DataChart.AxesX[0].Title = XAxisTitle.IsChecked ? axis.Description : null;
        XAxisTitle.Text = axis.Description;
      }

      var map = this.DefaultSeries.DataMappings[0];
      var mapInterpolationFit = this.InterpolationSeries.DataMappings[0];
      var mapLineFit = this.LineFitSeries.DataMappings[0];

      
      UpdateAxisMappings(axis, map, mapInterpolationFit, mapLineFit);

      RefreshChartDataPoints();
    }

    private void UpdateAxisMappings(DataAxis axis, DataMapping mapPoints, DataMapping mapInterpolationFit, DataMapping mapLineFit)
    {
      var prefix = "Object[" + Video.Instance.ImageProcessing.IndexOfObject.ToString(CultureInfo.InvariantCulture) + "].";
      switch (axis.Axis)
      {
        case AxisType.I:
          mapPoints.Path = "Framenumber";
      //    mapInterpolationFit.Path = "Framenumber";
      //    mapLineFit.Path = "Framenumber";
          break;
        case AxisType.T:
          mapPoints.Path = "Timestamp";
      //    mapInterpolationFit.Path = "Timestamp";
      //    mapLineFit.Path = "Timestamp";
          break;
        case AxisType.PX:
          mapPoints.Path = "PositionX";
      //   mapInterpolationFit.Path = "PositionX";
      //   mapLineFit.Path = "PositionX";
          break;
        case AxisType.PY:
          mapPoints.Path = "PositionY";
      //    mapInterpolationFit.Path = "PositionY";
      //    mapLineFit.Path = "PositionY";
          break;
        case AxisType.D:
          mapPoints.Path = "Distance";
       //   mapInterpolationFit.Path = "Distance";
       //   mapLineFit.Path = "Distance";
          break;
        case AxisType.DX:
          mapPoints.Path = "DistanceX";
       //   mapInterpolationFit.Path = "DistanceX";
       //   mapLineFit.Path = "DistanceX";
          break;
        case AxisType.DY:
          mapPoints.Path = "DistanceY";
       //   mapInterpolationFit.Path = "DistanceY";
       //   mapLineFit.Path = "DistanceY";
          break;
        case AxisType.S:
          mapPoints.Path = "Length";
       //   mapInterpolationFit.Path = "Length";
       //   mapLineFit.Path = "Length";
          break;
        case AxisType.SX:
          mapPoints.Path = "LengthX";
       //   mapInterpolationFit.Path = "LengthX";
       //   mapLineFit.Path = "LengthX";
          break;
        case AxisType.SY:
          mapPoints.Path = "LengthY";
       //   mapInterpolationFit.Path = "LengthY";
       //   mapLineFit.Path = "LengthY";
          break;
        case AxisType.V:
          mapPoints.Path = "Velocity";
       //   mapInterpolationFit.Path = "VelocityI";
       //   mapLineFit.Path = "VelocityI";
          break;
        case AxisType.VX:
          mapPoints.Path = "VelocityX";
       //   mapInterpolationFit.Path = "VelocityXI";
       //   mapLineFit.Path = "VelocityXI";
          break;
        case AxisType.VY:
          mapPoints.Path = "VelocityY";
        //  mapInterpolationFit.Path = "VelocityYI";
        //  mapLineFit.Path = "VelocityYI";
          break;
        case AxisType.VI:
          mapPoints.Path = "VelocityI";
        //  mapInterpolationFit.Path = "VelocityI";
        //  mapLineFit.Path = "VelocityI";
          break;
        case AxisType.VXI:
          mapPoints.Path = "VelocityXI";
       //   mapInterpolationFit.Path = "VelocityXI";
       //   mapLineFit.Path = "VelocityXI";
          break;
        case AxisType.VYI:
          mapPoints.Path = "VelocityYI";
       //   mapInterpolationFit.Path = "VelocityYI";
       //   mapLineFit.Path = "VelocityYI";
          break;
        case AxisType.A:
          mapPoints.Path = "Acceleration";
        //  mapInterpolationFit.Path = "AccelerationI";
        //  mapLineFit.Path = "AccelerationI";
          break;
        case AxisType.AX:
          mapPoints.Path = "AccelerationX";
        //  mapInterpolationFit.Path = "AccelerationXI";
        //  mapLineFit.Path = "AccelerationXI";
          break;
        case AxisType.AY:
          mapPoints.Path = "AccelerationY";
        //  mapInterpolationFit.Path = "AccelerationYI";
        //  mapLineFit.Path = "AccelerationYI";
          break;
        case AxisType.AI:
          mapPoints.Path = "AccelerationI";
        //  mapInterpolationFit.Path = "AccelerationI";
        //  mapLineFit.Path = "AccelerationI";
          break;
        case AxisType.AXI:
          mapPoints.Path = "AccelerationXI";
        //  mapInterpolationFit.Path = "AccelerationXI";
        //  mapLineFit.Path = "AccelerationXI";
          break;
        case AxisType.AYI:
          mapPoints.Path = "AccelerationYI";
        //  mapInterpolationFit.Path = "AccelerationYI";
        //  mapLineFit.Path = "AccelerationYI";
          break;
      }
      
      mapPoints.Path = prefix + mapPoints.Path;
     // mapInterpolationFit.Path = prefix + mapInterpolationFit.Path;
     // mapLineFit.Path = prefix + mapLineFit.Path;

      if (mapInterpolationFit != null) { mapInterpolationFit.Path = mapPoints.Path; }
      if (mapLineFit != null) { mapLineFit.Path = mapPoints.Path; }
    }

    private void RefreshChartDataPoints()
    {
      this.DefaultSeries.DataSource = null;
      this.DefaultSeries.DataSource = VideoData.Instance.Samples;
      this.InterpolationSeries.DataSource = null;
      this.InterpolationSeries.DataSource = VideoData.Instance.Samples;
      this.LineFitSeries.DataSource = null;
      if ((activeLineFitClass != null) && (activeLineFitClass.LineFitPoints != null) && (activeLineFitClass.LineFitPoints.Count > 0))
      {
          this.LineFitSeries.LineThickness = 1;
          this.LineFitSeries.DataSource = activeLineFitClass.LineFitPoints;
      }
   //   this.LineFitSeries.DataSource = VideoData.Instance.Samples;
    }

    private void YAxisContentSelectionChanged(object sender, SelectionChangedEventArgs e)
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

      var axis = (DataAxis)yAxisContent.SelectedItem;
      this.SeriesTitle.Text = axis.Description;

      if (this.DataChart.AxesY.Count >= 1)
      {
        this.DataChart.AxesY[0].Title = YAxisTitle.IsChecked ? axis.Description : null;
        YAxisTitle.Text = axis.Description;
      }

      var map = this.DefaultSeries.DataMappings[1];
      var mapInterpolationFit = this.InterpolationSeries.DataMappings[1];
      var mapLineFit = this.LineFitSeries.DataMappings[1];
      UpdateAxisMappings(axis, map, mapInterpolationFit, mapLineFit);

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
        this.DataChart.Legends[0].Enabled = this.LegendTitle.IsChecked || this.SeriesTitle.IsChecked;
        this.DefaultSeries.LegendText = SeriesTitle.IsChecked ? SeriesTitle.Text : null;

        if (this.DataChart.AxesX.Count > 0)
        {
          var xAxis = this.DataChart.AxesX[0];
          xAxis.Title = XAxisTitle.IsChecked ? XAxisTitle.Text : null;
          xAxis.Grids[0].Enabled = XAxisShowGridLines.IsChecked();

          if (this.XAxisMinimum.Value > this.XAxisMaximum.Value)
          {
            this.XAxisMinimum.Value = this.XAxisMaximum.Value;
          }

          xAxis.AxisMinimum = this.XAxisMinimum.IsChecked ? this.XAxisMinimum.Value : new double?();

          if (this.XAxisMaximum.Value < this.XAxisMinimum.Value)
          {
            this.XAxisMaximum.Value = this.XAxisMinimum.Value;
          }

          xAxis.AxisMaximum = this.XAxisMaximum.IsChecked ? this.XAxisMaximum.Value : new double?();
          //if (XAxisInterval.IsChecked)
          //{
          //  xAxis.Interval = XAxisInterval.Value;
          //}
          //else
          //{
          //  xAxis.Interval = double.NaN;
          //}
        }

        if (this.DataChart.AxesY.Count > 0)
        {
          var yAxis = this.DataChart.AxesY[0];
          yAxis.Title = YAxisTitle.IsChecked ? YAxisTitle.Text : null;
          yAxis.Grids[0].Enabled = YAxisShowGridLines.IsChecked();

          if (this.YAxisMinimum.Value > this.YAxisMaximum.Value)
          {
            this.YAxisMinimum.Value = this.YAxisMaximum.Value;
          }

          yAxis.AxisMinimum = this.YAxisMinimum.IsChecked ? this.YAxisMinimum.Value : new double?();

          if (this.YAxisMaximum.Value < this.YAxisMinimum.Value)
          {
            this.YAxisMaximum.Value = this.YAxisMinimum.Value;
          }

          yAxis.AxisMaximum = this.YAxisMaximum.IsChecked ? this.YAxisMaximum.Value : new double?();
          //if (YAxisInterval.IsChecked)
          //{
          //  yAxis.Interval = YAxisInterval.Value;
          //}
          //else
          //{
          //  yAxis.Interval = double.NaN;
          //}
        }
      }
    }

    #endregion //PRIVATEMETHODS

    private void RadioChartStyleChecked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (e.Source is RadioButton)
      {
        var checkedRadioButton = e.Source as RadioButton;
        UpdateChartStyle(checkedRadioButton);
        this.RefreshSeries();
      }
    }

    private void UpdateChartStyle(RadioButton checkedRadioButton)
    {
      this.AxisControls.Visibility = Visibility.Visible;
      this.OtherContentGrid.RowDefinitions[0].Height = GridLength.Auto;

      this.LineFitSeries.RenderAs = RenderAs.Line;

      if (checkedRadioButton.Name.Contains("Scatter"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Point;
        this.InterpolationSeries.RenderAs = RenderAs.Line;
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

    private void RefreshSeries()
    {
      UpdateSeriesWithXAxis();
      UpdateSeriesWithYAxis();
      UpdateChartProperties();
    }

    private void InterpolationLineCheckBoxChecked(object sender, RoutedEventArgs e)
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

    private void AxesExpanderCollapsed(object sender, RoutedEventArgs e)
    {
      SetDefaultDiagramm();
    }

    private void InterpolationOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      Interpolation.ShowInterpolationOptionsDialog();
    }

    private void ChartContentTabSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SetDefaultDiagramm();
    }

    private void ObjectSelectionComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.ObjectSelectionCombo.SelectedItem == null)
      {
        return;
      }

      var entry = (string)this.ObjectSelectionCombo.SelectedItem;
      Video.Instance.ImageProcessing.IndexOfObject = Int32.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing LineFitting                                             //
    ///////////////////////////////////////////////////////////////////////////////

    private void MakePreparationsForLineFit()
    {
      int columnNrFirstValue, columnNrSecondValue;
      int selIndex = AxesContentPositionSpace.SelectedIndex;

      if (selIndex == 0)
      {
          columnNrFirstValue = 2; columnNrSecondValue = 3;
      }
      else
      {
          if (selIndex <= 9)
          {
              columnNrFirstValue = 1;
              columnNrSecondValue = selIndex + 1;
          }
          else
          { columnNrFirstValue = 0; columnNrSecondValue = 0; }
      }
      
      if (this.activeLineFitClass == null)
      {
          this.activeLineFitClass = new LineFitClass(VideoData.Instance.Samples, 0, columnNrFirstValue, columnNrSecondValue);
      }
      else 
      {
          this.activeLineFitClass.ExtractDataColumnsFromVideoSamples(VideoData.Instance.Samples, 0, columnNrFirstValue, columnNrSecondValue); 
      }
    }


    private void NewCalculationForLineFitting()
    {
        MakePreparationsForLineFit();
        
        if (this.activeRegressionType == 0)
        {
            this.activeRegressionType = 1;
        }
        this.activeLineFitClass.TesteAusgleich(this.activeRegressionType);
        LabelLineFitFkt.Content = this.activeLineFitClass.LineFitFktStr;
    }

    private void LineFitCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (LineFitCheckBox.IsChecked.GetValueOrDefault(false))
      {
          NewCalculationForLineFitting();
      }
      else
      {
          this.activeLineFitClass.LineFitPoints.Clear();
          LabelLineFitFkt.Content = "";
      }

      RefreshSeries();
    }


    private void LineFitOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      double minX, minY, hilf;

      MakePreparationsForLineFit();
      
      this.activeLineFitClass.getMinMax(this.activeLineFitClass.wertX, this.activeLineFitClass.wertX.Count, out minX, out hilf);
      this.activeLineFitClass.getMinMax(this.activeLineFitClass.wertY, this.activeLineFitClass.wertY.Count, out minY, out hilf);
      var auswahlDialog = new LinefittingDialog(minX < 0, minY < 0, this.activeRegressionType);
      if (auswahlDialog.ShowDialog().GetValueOrDefault((false)))
      {
        this.activeRegressionType = auswahlDialog.GetAuswahl();
        string bildsource = "/VianaNET;component/Images/LineFit_Linear_16.png";
        switch (this.activeRegressionType)
        {
          case 1: bildsource = "/VianaNET;component/Images/LineFit_Linear_16.png"; break;
          case 2: bildsource = "/VianaNET;component/Images/LineFit_Exponential1_16.png"; break;
          case 3: bildsource = "/VianaNET;component/Images/LineFit_Logarithmus_16.png"; break;
          case 4: bildsource = "/VianaNET;component/Images/LineFit_Potentiell_16.png"; break;
          case 5: bildsource = "/VianaNET;component/Images/LineFit_Quadratisch_16.png"; break;
          case 6: bildsource = "/VianaNET;component/Images/LineFit_Exponential2_16.png"; break;
          case 7: bildsource = "/VianaNET;component/Images/LineFit_Sinus_16.png"; break;
          case 8: bildsource = "/VianaNET;component/Images/LineFit_SinusExponential_16.png"; break;
          case 9: bildsource = "/VianaNET;component/Images/LineFit_Resonanz_16.png"; break;
        }

        var neuBildsource = new Uri(bildsource, UriKind.RelativeOrAbsolute);
        LineFitOptionsButton.ImageSource = new BitmapImage(neuBildsource);

        if (LineFitCheckBox.IsChecked.GetValueOrDefault(false))
        {
          this.activeLineFitClass.TesteAusgleich(this.activeRegressionType);
          LabelLineFitFkt.Content = this.activeLineFitClass.LineFitFktStr;
        }
      }
    }

    private void LineFitTheorieButtonClick(object sender, RoutedEventArgs e)
    {
      var fktEditor = new CalculatorAndFktEditor(TRechnerArt.formelRechner);
      if (this.lineFitTheorieFunction != null)
      {
        fktEditor.textBox1.Text = this.lineFitTheorieFunction.name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        this.lineFitTheorieFunction = fktEditor.GetFunktion();
        this.LabelLineFitTheorieFkt.Content = this.lineFitTheorieFunction != null ? this.lineFitTheorieFunction.name : "keine Funktion angegeben oder erkannt";
      }
      //     else 
      //    {
      //        LabelLineFitTheorieFkt.Content = "Dialog abgebrochen"; 
      //    }
    }

    private void RechnerButtonClick(object sender, RoutedEventArgs e)
    {
      var calculator = new CalculatorAndFktEditor(TRechnerArt.rechner);
      calculator.ShowDialog();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
