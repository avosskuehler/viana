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

  using AvalonDock;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Interpolation;
  using VianaNET.Data.Linefit;
  using VianaNET.Localization;
  using VianaNET.Modules.Video.Control;

  using Visifire.Charts;

  using WPFLocalizeExtension.Extensions;

  // letzte Änderung: 24.9.2012

  /// <summary>
  ///   The chart window.
  /// </summary>
  public partial class ChartWindow : DockableContent
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
    ///   The active line fit class.
    /// </summary>
    private LineFitClass activeLineFitClass;

    /// <summary>
    ///   The active regression type.
    /// </summary>
    private int activeRegressionType;

    /// <summary>
    ///   The line fit function line color.
    /// </summary>
    private SolidColorBrush lineFitFunctionLineColor = Brushes.Red;

    /// <summary>
    ///   The line fit function line thickness.
    /// </summary>
    private double lineFitFunctionLineThickness = 1;

    /// <summary>
    ///   The line fit theorie function.
    /// </summary>
    private TFktTerm lineFitTheorieFunction;

    /// <summary>
    ///   The showing line fit or theorie is allowed.
    /// </summary>
    private bool showingLineFitOrTheorieIsAllowed;

    /// <summary>
    ///   The stellen zahl format string.
    /// </summary>
    private string stellenZahlFormatString = "G4";

    /// <summary>
    ///   The theorie function line color.
    /// </summary>
    private SolidColorBrush theorieFunctionLineColor = Brushes.Green;

    /// <summary>
    ///   The theorie function line thickness.
    /// </summary>
    private double theorieFunctionLineThickness = 1;

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
      this.activeLineFitClass = null;
      this.showingLineFitOrTheorieIsAllowed = true;

      // VideoData.Instance.PropertyChanged +=
      // new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      // Calibration.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VideoData_PropertyChanged);
      Video.Instance.ImageProcessing.PropertyChanged += this.ImageProcessingPropertyChanged;
      this.CreateDataSeries();
      this.isInitialized = true;
      this.UpdateChartProperties();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    /// Raises the <see cref="ManagedContent.PropertyChanged"/> event.
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
    /// The axes content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AxesContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if ((this.LineFitCheckBox != null) && this.LineFitCheckBox.IsChecked.GetValueOrDefault(false)
          && this.showingLineFitOrTheorieIsAllowed)
      {
        this.NewCalculationForLineFitting();
      }

      this.SetDefaultDiagramm();
    }

    /// <summary>
    /// The axes expander collapsed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AxesExpanderCollapsed(object sender, RoutedEventArgs e)
    {
      this.SetDefaultDiagramm();
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
      this.SetDefaultDiagramm();
    }

    /// <summary>
    ///   The create data series.
    /// </summary>
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

    /// <summary>
    /// The interpolation line check box checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void InterpolationLineCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      this.InterpolationSeries.Enabled = Interpolation.Instance.IsInterpolatingData;

      // Update static property
      // Interpolation.Instance.IsInterpolatingData = this.InterpolationLineCheckBox.IsChecked.Value;
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

    /// <summary>
    /// The line fit check box checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LineFitCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      if (!this.isInitialized)
      {
        return;
      }

      if (this.LineFitCheckBox.IsChecked.GetValueOrDefault(false))
      {
        this.NewCalculationForLineFitting();
      }
      else
      {
        this.activeLineFitClass.LineFitDisplaySample.Clear();
        this.LabelLineFitFkt.Content = string.Empty;
      }

      this.RefreshSeries();
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
    private void LineFitOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      double minX, minY, hilf;

      this.MakePreparationsForLineFit();

      this.activeLineFitClass.getMinMax(this.activeLineFitClass.wertX, out minX, out hilf);
      this.activeLineFitClass.getMinMax(this.activeLineFitClass.wertY, out minY, out hilf);
      var auswahlDialog = new LinefittingDialog(minX < 0, minY < 0, this.activeRegressionType);
      if (auswahlDialog.ShowDialog().GetValueOrDefault(false))
      {
        this.activeRegressionType = auswahlDialog.GetAuswahl();
        string bildsource = "/VianaNET;component/Images/LineFit_Linear_16.png";
        switch (this.activeRegressionType)
        {
          case 1:
            bildsource = "/VianaNET;component/Images/LineFit_Linear_16.png";
            break;
          case 2:
            bildsource = "/VianaNET;component/Images/LineFit_Exponential1_16.png";
            break;
          case 3:
            bildsource = "/VianaNET;component/Images/LineFit_Logarithmus_16.png";
            break;
          case 4:
            bildsource = "/VianaNET;component/Images/LineFit_Potentiell_16.png";
            break;
          case 5:
            bildsource = "/VianaNET;component/Images/LineFit_Quadratisch_16.png";
            break;
          case 6:
            bildsource = "/VianaNET;component/Images/LineFit_Exponential2_16.png";
            break;
          case 7:
            bildsource = "/VianaNET;component/Images/LineFit_Sinus_16.png";
            break;
          case 8:
            bildsource = "/VianaNET;component/Images/LineFit_SinusExponential_16.png";
            break;
          case 9:
            bildsource = "/VianaNET;component/Images/LineFit_Resonanz_16.png";
            break;
        }

        var neuBildsource = new Uri(bildsource, UriKind.RelativeOrAbsolute);
        this.LineFitOptionsButton.ImageSource = new BitmapImage(neuBildsource);

        if (this.LineFitCheckBox.IsChecked.GetValueOrDefault(false))
        {
          this.activeLineFitClass.CalculateLineFitFunction(this.activeRegressionType);
          this.LabelLineFitFkt.Content = this.activeLineFitClass.LineFitFktStr;
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
      if (this.lineFitTheorieFunction != null)
      {
        fktEditor.textBox1.Text = this.lineFitTheorieFunction.name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        this.lineFitTheorieFunction = fktEditor.GetFunktion();
        if (this.lineFitTheorieFunction != null)
        {
          this.LabelLineFitTheorieFkt.Content = this.lineFitTheorieFunction.name;
          this.MakePreparationsForTheorieFit();
          this.activeLineFitClass.CalculateLineFitTheorieSeries(
            this.activeLineFitClass.TheorieDisplaySample, this.lineFitTheorieFunction);
        }
        else
        {
          this.LabelLineFitTheorieFkt.Content = " -- ";
          if ((this.activeLineFitClass != null) && (this.activeLineFitClass.TheorieDisplaySample != null))
          {
            this.activeLineFitClass.TheorieDisplaySample.Clear();
          }
        }

        if (this.checkBoxShowTheorie.IsChecked())
        {
          this.RefreshSeries();
        }
      }
    }

    /// <summary>
    ///   The make preparations for line fit.
    /// </summary>
    private void MakePreparationsForLineFit()
    {
      int columnNrFirstValue, columnNrSecondValue;
      int selIndex = this.AxesContentPositionSpace.SelectedIndex; // Ermittele die Spaltennummern der gewünschten Daten

      if (selIndex == 0)
      {
        columnNrFirstValue = 2;
        columnNrSecondValue = 3;
      }
      else
      {
        if (selIndex <= 9)
        {
          columnNrFirstValue = 1;
          columnNrSecondValue = selIndex + 1;
        }
        else
        {
          columnNrFirstValue = 0;
          columnNrSecondValue = 0;
        }
      }

      if (this.activeLineFitClass == null)
      {
        // übernehme die Daten in die Arrays wertX und wertY
        this.activeLineFitClass = new LineFitClass(
          VideoData.Instance.Samples, 0, columnNrFirstValue, columnNrSecondValue);
        this.activeLineFitClass.gueltigeStellenFormatString = this.stellenZahlFormatString;
      }
      else
      {
        this.activeLineFitClass.ExtractDataColumnsFromVideoSamples(
          VideoData.Instance.Samples, 0, columnNrFirstValue, columnNrSecondValue);
      }
    }

    /// <summary>
    ///   The make preparations for theorie fit.
    /// </summary>
    private void MakePreparationsForTheorieFit()
    {
      this.MakePreparationsForLineFit();
      if (this.activeLineFitClass.TheorieDisplaySample == null)
      {
        this.activeLineFitClass.TheorieDisplaySample = new DataCollection();
      }
    }

    /// <summary>
    ///   The new calculation for line fitting.
    /// </summary>
    private void NewCalculationForLineFitting()
    {
      this.MakePreparationsForLineFit();

      if (this.activeRegressionType == 0)
      {
        this.activeRegressionType = 1;
      }

      this.activeLineFitClass.CalculateLineFitFunction(this.activeRegressionType);
      this.LabelLineFitFkt.Content = this.activeLineFitClass.LineFitFktStr;
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
      this.DefaultSeries.DataSource = null;
      this.DefaultSeries.DataSource = VideoData.Instance.Samples;
      this.InterpolationSeries.DataSource = null;
      this.InterpolationSeries.DataSource = VideoData.Instance.Samples;
      this.LineFitSeries.DataSource = null;
      this.TheorieSeries.DataSource = null;
      if ((this.activeLineFitClass != null) & this.showingLineFitOrTheorieIsAllowed)
      {
        if ((this.activeLineFitClass.LineFitDisplaySample != null)
            && (this.activeLineFitClass.LineFitDisplaySample.Count > 0))
        {
          this.LineFitSeries.LineThickness = this.lineFitFunctionLineThickness;
          this.LineFitSeries.Color = this.lineFitFunctionLineColor;
          this.LineFitSeries.DataSource = this.activeLineFitClass.LineFitDisplaySample;
        }

        if ((this.activeLineFitClass.TheorieDisplaySample != null)
            && (this.activeLineFitClass.TheorieDisplaySample.Count > 0))
        {
          this.TheorieSeries.LineThickness = this.theorieFunctionLineThickness;
          this.TheorieSeries.Color = this.theorieFunctionLineColor;
          this.TheorieSeries.DataSource = this.activeLineFitClass.TheorieDisplaySample;
        }
      }
    }

    /// <summary>
    ///   The refresh series.
    /// </summary>
    private void RefreshSeries()
    {
      this.UpdateSeriesWithXAxis();
      this.UpdateSeriesWithYAxis();
      this.UpdateChartProperties();
    }

    /// <summary>
    ///   The set default diagramm.
    /// </summary>
    private void SetDefaultDiagramm()
    {
      if (!this.IsInitialized)
      {
        return;
      }

      var chartType = AxisType.YoverX;

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
        var xAxis = (DataAxis)this.xAxisContent.SelectedItem;
        var yAxis = (DataAxis)this.yAxisContent.SelectedItem;
        this.xAxisContent.SelectedValue = xAxis.Axis;
        this.yAxisContent.SelectedValue = yAxis.Axis;

        // axes content already set, so return
        return;
      }

      switch (chartType)
      {
        case AxisType.YoverX:
          this.xAxisContent.SelectedValue = AxisType.PX;
          this.yAxisContent.SelectedValue = AxisType.PY;
          break;
        case AxisType.XoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.PX;
          break;
        case AxisType.YoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.PY;
          break;
        case AxisType.VoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.V;
          break;
        case AxisType.VXoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.VX;
          break;
        case AxisType.VYoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.VY;
          break;
        case AxisType.AoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.A;
          break;
        case AxisType.AXoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.AX;
          break;
        case AxisType.AYoverT:
          this.xAxisContent.SelectedValue = AxisType.T;
          this.yAxisContent.SelectedValue = AxisType.AY;
          break;
        case AxisType.VoverD:
          this.xAxisContent.SelectedValue = AxisType.D;
          this.yAxisContent.SelectedValue = AxisType.V;
          break;
        case AxisType.VXoverDX:
          this.xAxisContent.SelectedValue = AxisType.DX;
          this.yAxisContent.SelectedValue = AxisType.VX;
          break;
        case AxisType.VYoverDY:
          this.xAxisContent.SelectedValue = AxisType.DY;
          this.yAxisContent.SelectedValue = AxisType.VY;
          break;
        case AxisType.VoverS:
          this.xAxisContent.SelectedValue = AxisType.S;
          this.yAxisContent.SelectedValue = AxisType.V;
          break;
        case AxisType.VXoverSX:
          this.xAxisContent.SelectedValue = AxisType.SX;
          this.yAxisContent.SelectedValue = AxisType.VX;
          break;
        case AxisType.VYoverSY:
          this.xAxisContent.SelectedValue = AxisType.SY;
          this.yAxisContent.SelectedValue = AxisType.VY;
          break;
        case AxisType.AoverV:
          this.xAxisContent.SelectedValue = AxisType.V;
          this.yAxisContent.SelectedValue = AxisType.A;
          break;
        case AxisType.AXoverVX:
          this.xAxisContent.SelectedValue = AxisType.VX;
          this.yAxisContent.SelectedValue = AxisType.AX;
          break;
        case AxisType.AYoverVY:
          this.xAxisContent.SelectedValue = AxisType.VY;
          this.yAxisContent.SelectedValue = AxisType.AY;
          break;
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
    private void UpdateAxisMappings(
      DataAxis axis, 
      DataMapping mapPoints, 
      DataMapping mapInterpolationFit, 
      DataMapping mapLineFit, 
      DataMapping mapTheorieFit)
    {
      string prefix = "Object[" + Video.Instance.ImageProcessing.IndexOfObject.ToString(CultureInfo.InvariantCulture)
                      + "].";
      switch (axis.Axis)
      {
        case AxisType.I:
          mapPoints.Path = "Framenumber";

          // mapInterpolationFit.Path = "Framenumber";
          // mapLineFit.Path = "Framenumber";
          break;
        case AxisType.T:
          mapPoints.Path = "Timestamp";

          // mapInterpolationFit.Path = "Timestamp";
          // mapLineFit.Path = "Timestamp";
          break;
        case AxisType.PX:
          mapPoints.Path = "PositionX";

          // mapInterpolationFit.Path = "PositionX";
          // mapLineFit.Path = "PositionX";
          break;
        case AxisType.PY:
          mapPoints.Path = "PositionY";

          // mapInterpolationFit.Path = "PositionY";
          // mapLineFit.Path = "PositionY";
          break;
        case AxisType.D:
          mapPoints.Path = "Distance";

          // mapInterpolationFit.Path = "Distance";
          // mapLineFit.Path = "Distance";
          break;
        case AxisType.DX:
          mapPoints.Path = "DistanceX";

          // mapInterpolationFit.Path = "DistanceX";
          // mapLineFit.Path = "DistanceX";
          break;
        case AxisType.DY:
          mapPoints.Path = "DistanceY";

          // mapInterpolationFit.Path = "DistanceY";
          // mapLineFit.Path = "DistanceY";
          break;
        case AxisType.S:
          mapPoints.Path = "Length";

          // mapInterpolationFit.Path = "Length";
          // mapLineFit.Path = "Length";
          break;
        case AxisType.SX:
          mapPoints.Path = "LengthX";

          // mapInterpolationFit.Path = "LengthX";
          // mapLineFit.Path = "LengthX";
          break;
        case AxisType.SY:
          mapPoints.Path = "LengthY";

          // mapInterpolationFit.Path = "LengthY";
          // mapLineFit.Path = "LengthY";
          break;
        case AxisType.V:
          mapPoints.Path = "Velocity";

          // mapInterpolationFit.Path = "VelocityI";
          // mapLineFit.Path = "VelocityI";
          break;
        case AxisType.VX:
          mapPoints.Path = "VelocityX";

          // mapInterpolationFit.Path = "VelocityXI";
          // mapLineFit.Path = "VelocityXI";
          break;
        case AxisType.VY:
          mapPoints.Path = "VelocityY";

          // mapInterpolationFit.Path = "VelocityYI";
          // mapLineFit.Path = "VelocityYI";
          break;
        case AxisType.VI:
          mapPoints.Path = "VelocityI";

          // mapInterpolationFit.Path = "VelocityI";
          // mapLineFit.Path = "VelocityI";
          break;
        case AxisType.VXI:
          mapPoints.Path = "VelocityXI";

          // mapInterpolationFit.Path = "VelocityXI";
          // mapLineFit.Path = "VelocityXI";
          break;
        case AxisType.VYI:
          mapPoints.Path = "VelocityYI";

          // mapInterpolationFit.Path = "VelocityYI";
          // mapLineFit.Path = "VelocityYI";
          break;
        case AxisType.A:
          mapPoints.Path = "Acceleration";

          // mapInterpolationFit.Path = "AccelerationI";
          // mapLineFit.Path = "AccelerationI";
          break;
        case AxisType.AX:
          mapPoints.Path = "AccelerationX";

          // mapInterpolationFit.Path = "AccelerationXI";
          // mapLineFit.Path = "AccelerationXI";
          break;
        case AxisType.AY:
          mapPoints.Path = "AccelerationY";

          // mapInterpolationFit.Path = "AccelerationYI";
          // mapLineFit.Path = "AccelerationYI";
          break;
        case AxisType.AI:
          mapPoints.Path = "AccelerationI";

          // mapInterpolationFit.Path = "AccelerationI";
          // mapLineFit.Path = "AccelerationI";
          break;
        case AxisType.AXI:
          mapPoints.Path = "AccelerationXI";

          // mapInterpolationFit.Path = "AccelerationXI";
          // mapLineFit.Path = "AccelerationXI";
          break;
        case AxisType.AYI:
          mapPoints.Path = "AccelerationYI";

          // mapInterpolationFit.Path = "AccelerationYI";
          // mapLineFit.Path = "AccelerationYI";
          break;
      }

      mapPoints.Path = prefix + mapPoints.Path;

      // mapInterpolationFit.Path = prefix + mapInterpolationFit.Path;
      // mapLineFit.Path = prefix + mapLineFit.Path;
      if (mapInterpolationFit != null)
      {
        mapInterpolationFit.Path = mapPoints.Path;
      }

      if (mapLineFit != null)
      {
        mapLineFit.Path = mapPoints.Path;
      }

      if (mapTheorieFit != null)
      {
        mapTheorieFit.Path = mapPoints.Path;
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
          Axis xAxis = this.DataChart.AxesX[0];
          xAxis.Title = this.XAxisTitle.IsChecked ? this.XAxisTitle.Text : null;
          xAxis.Grids[0].Enabled = this.XAxisShowGridLines.IsChecked();

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
          Axis yAxis = this.DataChart.AxesY[0];
          yAxis.Title = this.YAxisTitle.IsChecked ? this.YAxisTitle.Text : null;
          yAxis.Grids[0].Enabled = this.YAxisShowGridLines.IsChecked();

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

      this.showingLineFitOrTheorieIsAllowed = false;
      this.LineFitSeries.RenderAs = RenderAs.Line;
      this.TheorieSeries.RenderAs = RenderAs.Line;

      if (checkedRadioButton.Name.Contains("Scatter"))
      {
        this.DefaultSeries.RenderAs = RenderAs.Point;
        this.InterpolationSeries.RenderAs = RenderAs.Line;
        this.showingLineFitOrTheorieIsAllowed = true;
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

        this.showingLineFitOrTheorieIsAllowed = true;
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
    ///   The update series with x axis.
    /// </summary>
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

      var axis = (DataAxis)this.xAxisContent.SelectedItem;

      if (this.DataChart.AxesX.Count >= 1)
      {
        this.DataChart.AxesX[0].Title = this.XAxisTitle.IsChecked ? axis.Description : null;
        this.XAxisTitle.Text = axis.Description;
      }

      DataMapping map = this.DefaultSeries.DataMappings[0];
      DataMapping mapInterpolationFit = this.InterpolationSeries.DataMappings[0];
      DataMapping mapLineFit = this.LineFitSeries.DataMappings[0];
      DataMapping mapTheorieFit = this.TheorieSeries.DataMappings[0];

      this.UpdateAxisMappings(axis, map, mapInterpolationFit, mapLineFit, mapTheorieFit);

      this.RefreshChartDataPoints();
    }

    /// <summary>
    ///   The update series with y axis.
    /// </summary>
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

      var axis = (DataAxis)this.yAxisContent.SelectedItem;
      this.SeriesTitle.Text = axis.Description;

      if (this.DataChart.AxesY.Count >= 1)
      {
        this.DataChart.AxesY[0].Title = this.YAxisTitle.IsChecked ? axis.Description : null;
        this.YAxisTitle.Text = axis.Description;
      }

      DataMapping map = this.DefaultSeries.DataMappings[1];
      DataMapping mapInterpolationFit = this.InterpolationSeries.DataMappings[1];
      DataMapping mapLineFit = this.LineFitSeries.DataMappings[1];
      DataMapping mapTheorieFit = this.TheorieSeries.DataMappings[1];
      this.UpdateAxisMappings(axis, map, mapInterpolationFit, mapLineFit, mapTheorieFit);

      this.RefreshChartDataPoints();
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
      this.UpdateSeriesWithXAxis();
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
      this.UpdateSeriesWithYAxis();
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
    private void checkBoxShowTheorie_Checked(object sender, RoutedEventArgs e)
    {
      if ((!this.isInitialized) || (this.lineFitTheorieFunction == null) || (this.activeLineFitClass == null))
      {
        return;
      }

      if (this.checkBoxShowTheorie.IsChecked.GetValueOrDefault(false))
      {
        this.LabelLineFitTheorieFkt.IsEnabled = true;
        this.LabelLineFitTheorieFkt.Content = this.lineFitTheorieFunction.name;
        if ((this.activeLineFitClass.TheorieDisplaySample == null)
            || (this.activeLineFitClass.TheorieDisplaySample.Count == 0))
        {
          this.MakePreparationsForTheorieFit();
          this.activeLineFitClass.CalculateLineFitTheorieSeries(
            this.activeLineFitClass.TheorieDisplaySample, this.lineFitTheorieFunction);
        }

        this.RefreshSeries();
      }
      else
      {
        this.LabelLineFitTheorieFkt.IsEnabled = false;
        if ((this.activeLineFitClass.TheorieDisplaySample != null)
            && (this.activeLineFitClass.TheorieDisplaySample.Count != 0))
        {
          this.activeLineFitClass.TheorieDisplaySample.Clear();
          this.RefreshSeries();
        }
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
    private void imageButtonRegressOptions_Click(object sender, RoutedEventArgs e)
    {
      int k = Convert.ToInt16(this.stellenZahlFormatString.Substring(1));
      var myOptionsDialog = new LineOptionsDialog(
        this.lineFitFunctionLineThickness, 
        this.lineFitFunctionLineColor, 
        this.theorieFunctionLineThickness, 
        this.theorieFunctionLineColor, 
        k);
      myOptionsDialog.ShowDialog();
      if (myOptionsDialog.DialogResult == true)
      {
        this.lineFitFunctionLineThickness = myOptionsDialog.lineThicknessRegress;
        this.theorieFunctionLineThickness = myOptionsDialog.lineThicknessTheorie;
        this.lineFitFunctionLineColor = myOptionsDialog.lineColorRegress;
        this.theorieFunctionLineColor = myOptionsDialog.lineColorTheorie;
        this.stellenZahlFormatString = "G" + myOptionsDialog.stellenZahl.ToString();
        if (this.activeLineFitClass != null)
        {
          this.activeLineFitClass.gueltigeStellenFormatString = this.stellenZahlFormatString;
          this.LabelLineFitFkt.Content = this.activeLineFitClass.LineFitFktStr;
          if (((this.activeLineFitClass.LineFitDisplaySample != null)
               && (this.activeLineFitClass.LineFitDisplaySample.Count > 0))
              ||
              ((this.activeLineFitClass.TheorieDisplaySample != null)
               && (this.activeLineFitClass.TheorieDisplaySample.Count > 0)))
          {
            this.RefreshChartDataPoints();
          }
        }
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}