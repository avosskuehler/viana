// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartWindow.xaml.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using OxyPlot;
  using OxyPlot.Legends;
  using OxyPlot.Series;
  using OxyPlot.Wpf;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter;
  using VianaNET.Data.Filter.Regression;
  using VianaNET.Data.Filter.Theory;
  using VianaNET.MainWindow;
  using VianaNET.Modules.DataAcquisition;


  using WpfMath;

  using HitTestResult = OxyPlot.HitTestResult;

  /// <summary>
  ///   The chart window.
  /// </summary>
  public partial class ChartWindow
  {


    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="ObjectDescriptions" />.
    /// </summary>
    public static readonly DependencyProperty ObjectDescriptionsProperty =
      DependencyProperty.Register(
        "ObjectDescriptions",
        typeof(List<string>),
        typeof(ChartWindow),
        new FrameworkPropertyMetadata(new List<string>(), OnPropertyChanged));





    /// <summary>
    ///   Determines maximum distance of two points (in pixel) given by mouse input
    ///   that should be considered as different.
    /// </summary>
    private const int Maxdistancepoints = 10;





    /// <summary>
    ///   Provides a formula parser which reads tex formulas
    /// </summary>
    private readonly TexFormulaParser formulaParser;

    /// <summary>
    ///   The is initialized
    /// </summary>
    private readonly bool isInitialized;

    /// <summary>
    ///   The minus cursor.
    /// </summary>
    private readonly Cursor minusCursor =
      new Cursor(
        App.GetResourceStream(new Uri("pack://application:,,,/CustomStyles/Cursors/CursorMinus.cur")).Stream);

    /// <summary>
    ///   The plus cursor.
    /// </summary>
    private readonly Cursor plusCursor =
      new Cursor(
        App.GetResourceStream(new Uri("pack://application:,,,/CustomStyles/Cursors/CursorPlus.cur")).Stream);

    /// <summary>
    ///   A character for the axis name
    /// </summary>
    private char axisName = 'x';

    /// <summary>
    ///   Indicates whether data point selection is enabled.
    /// </summary>
    private bool isSelectionEnabled;

    /// <summary>
    /// Indicates setting default axis pair
    /// </summary>
    private bool isSettingDefaultAxisPair;

    /// <summary>
    ///   Indicates whether the mouse button is pressed.
    /// </summary>
    private bool mouseDown;

    /// <summary>
    ///   The start x value.
    /// </summary>
    private DataPoint mouseDownPositionInAxesCoordinates;

    /// <summary>
    ///   The start pos.
    /// </summary>
    private Point mouseDownPositionInCanvasCoordinates;

    /// <summary>
    ///   The end x value.
    /// </summary>
    private DataPoint mouseUpPositionInAxesCoordinates;





    /// <summary>
    ///   Initializes a new instance of the <see cref="ChartWindow" /> class.
    /// </summary>
    public ChartWindow()
    {
      this.ChartData = new ChartData();
      this.isSettingDefaultAxisPair = true;
      this.InitializeComponent();
      this.ObjectSelectionCombo.DataContext = this;
      this.PopulateObjectCombo();
      App.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      App.Project.UpdateChartRequested += this.ProjectUpdateChartRequested;
      //App.Project.VideoData.SelectionChanged += this.SamplesSelectionChanged;
      this.isInitialized = true;
      this.formulaParser = new TexFormulaParser();
      this.isSettingDefaultAxisPair = false;
      this.PopulateAxesFromChartSelection();
    }





    /// <summary>
    ///   Gets or sets the chart data. This is the view model for the chart.
    /// </summary>
    /// <value>
    ///   The charts view model.
    /// </value>
    public ChartData ChartData { get; set; }

    /// <summary>
    ///   Gets or sets the index of the currently tracked object
    /// </summary>
    public List<string> ObjectDescriptions
    {
      get => (List<string>)this.GetValue(ObjectDescriptionsProperty);

      set => this.SetValue(ObjectDescriptionsProperty, value);
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
      // App.Project.CurrentFilterData.IsShowingTheorySeries = false;
      // this.UpdateTheoryFormula();
      DataAxis axisX = (DataAxis)this.XAxisContent.SelectedItem;
      DataAxis axisY = (DataAxis)this.YAxisContent.SelectedItem;

      App.Project.CurrentFilterData.AxisX = axisX;
      App.Project.CurrentFilterData.AxisY = axisY;

      this.UpdateAxisMappings(axisX, out string propertyX, out string unitNameX);

      this.UpdateAxisMappings(axisY, out string propertyY, out string unitNameY);

      this.ChartData.XAxis.Title = axisX.Description;
      this.ChartData.XAxis.Unit = unitNameX;
      this.ChartData.YAxis.Title = axisY.Description;
      this.ChartData.YAxis.Unit = unitNameY;

      this.XAxisTitleTextBox.Text = axisX.Description;
      this.YAxisTitleTextBox.Text = axisY.Description;

      this.ChartData.UpdateDefaultSeriesMapping(propertyX, propertyY);

      App.Project.CurrentFilterData.CalculateInterpolationSeriesDataPoints();
      App.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();
      this.RefreshRegressionFuctionTerm();
      this.UpdateTheoryFormula();
      App.Project.CurrentFilterData.NotifyTheoryTermChange();
      App.Project.CurrentFilterData.CalculateTheorySeriesDataPoints();

      this.ChartData.ChartDataModel.ResetAllAxes();
      this.ChartData.UpdateModel();
    }





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
      if (obj is ChartWindow window)
      {
        window.Refresh();
      }
    }

    /// <summary>
    /// Axis x title text box text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="TextChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void AxisXTitleTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.XAxis.Title = this.XAxisTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Axis y title text box text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="TextChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void AxisYTitleTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.YAxis.Title = this.YAxisTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// The chart content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="SelectionChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void ChartContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // This populates the chart combo boxes with the selected x and y Axes
      // During population it updates also the mappings and refreshes the data points
      this.PopulateAxesFromChartSelection();
    }

    /// <summary>
    /// Handles the OnMouseDoubleClick event of the DataChart control.
    ///   If we had a double click on a data point, show the modify data window to adapt the
    ///   point location.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void DataChart_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      Point point = e.GetPosition(this.DataChart);
      ScreenPoint screenPoint = new ScreenPoint(point.X, point.Y);
      HitTestResult result = this.ChartData.DataScatterSeries.HitTest(new HitTestArguments(screenPoint, 20));
      if (result == null)
      {
        return;
      }

      ModifyDataWindow modifyWindow = new ModifyDataWindow();
      if (result.Item is TimeSample sample)
      {
        modifyWindow.MoveToFrame(sample.Framenumber);
      }

      modifyWindow.ShowDialog();
      App.Project.VideoData.RefreshDistanceVelocityAcceleration();
    }

    /// <summary>
    /// Handles the OnTextChanged event of the DataSeriesTitleTextBox control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="TextChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void DataSeriesTitleTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.DataScatterSeries.Title = this.DataSeriesTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Data style button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void DataStyleButtonClick(object sender, RoutedEventArgs e)
    {
      LineOptionsDialog lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = App.Project.CurrentFilterData.DataLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = App.Project.CurrentFilterData.DataLineColor.ToOxyColor();
      lineOptionsDialog.LineStyleControl.MarkerType = App.Project.CurrentFilterData.DataLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        App.Project.CurrentFilterData.DataLineThickness = lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        App.Project.CurrentFilterData.DataLineColor = lineOptionsDialog.LineStyleControl.SeriesColor.ToColor();
        App.Project.CurrentFilterData.DataLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Handles the OnChecked event of the EnableTrackerCheckBox control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void EnableTrackerCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
      this.isSelectionEnabled = true;
    }

    /// <summary>
    /// Handles the OnUnchecked event of the EnableTrackerCheckBox control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void EnableTrackerCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
    {
      this.isSelectionEnabled = false;
      this.ChartData.DataScatterSeries.ClearSelection();
      foreach (TimeSample sample in App.Project.VideoData.FilteredSamples)
      {
        sample.IsSelected = true;
      }

      this.Refresh();
    }

    /// <summary>
    /// Filter precision button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void FilterPrecisionButtonClick(object sender, RoutedEventArgs e)
    {
      NumericalPrecisionDialog dlg = new NumericalPrecisionDialog { NumberOfDigits = App.Project.CurrentFilterData.NumericPrecision };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        App.Project.CurrentFilterData.NumericPrecision = dlg.NumberOfDigits;
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
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void InterpolationOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      App.Project.CurrentFilterData.InterpolationFilter.ShowInterpolationOptionsDialog();
      App.Project.CurrentFilterData.CalculateInterpolationSeriesDataPoints();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// Interpolation style button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void InterpolationStyleButtonClick(object sender, RoutedEventArgs e)
    {
      LineOptionsDialog lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness =
        App.Project.CurrentFilterData.InterpolationLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = App.Project.CurrentFilterData.InterpolationLineColor.ToOxyColor();
      lineOptionsDialog.LineStyleControl.MarkerType = App.Project.CurrentFilterData.InterpolationLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        App.Project.CurrentFilterData.InterpolationLineThickness =
          lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        App.Project.CurrentFilterData.InterpolationLineColor = lineOptionsDialog.LineStyleControl.SeriesColor.ToColor();
        App.Project.CurrentFilterData.InterpolationLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Handles the OnChecked event of the LegendPlacementInsideRadioButton control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void LegendPlacementInsideRadioButton_OnChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.Legends[0].LegendPlacement = LegendPlacement.Inside;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Handles the OnChecked event of the LegendPlacementOutsideRadioButton control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void LegendPlacementOutsideRadioButton_OnChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.Legends[0].LegendPlacement = LegendPlacement.Outside;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Legend text box text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="TextChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void LegendTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.ChartDataModel.Legends[0].LegendTitle = this.LegendTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Legend visible CheckBox checked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void LegendVisibleCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.IsLegendVisible = true;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Legend visible CheckBox unchecked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void LegendVisibleCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.IsLegendVisible = false;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// The object selection combo selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="SelectionChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void ObjectSelectionComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.ObjectSelectionCombo.SelectedItem == null)
      {
        return;
      }

      string entry = (string)this.ObjectSelectionCombo.SelectedItem;
      App.Project.ProcessingData.IndexOfObject = int.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
    }

    /// <summary>
    /// Plot area mouse enter.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaMouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mouseDown)
      {
        this.SelectRect.Visibility = Visibility.Visible;
      }
    }

    /// <summary>
    /// Plot area mouse leave.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaMouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mouseDown)
      {
        this.SelectRect.Visibility = Visibility.Collapsed;
      }
    }

    /// <summary>
    /// Plot area mouse left button down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.isSelectionEnabled)
      {
        return;
      }

      this.mouseDown = true;
      this.SelectRect.Width = 0;
      this.SelectRect.Height = 0;

      this.mouseDownPositionInCanvasCoordinates = new Point(
        e.GetPosition(this.MyCanvas).X,
        e.GetPosition(this.MyCanvas).Y);
      this.SelectRect.SetValue(Canvas.LeftProperty, this.mouseDownPositionInCanvasCoordinates.X);
      this.SelectRect.SetValue(Canvas.TopProperty, this.mouseDownPositionInCanvasCoordinates.Y);
      this.mouseDownPositionInAxesCoordinates =
        this.ChartData.DataScatterSeries.InverseTransform(
          new ScreenPoint(this.mouseDownPositionInCanvasCoordinates.X, this.mouseDownPositionInCanvasCoordinates.Y));

      this.SelectRect.Visibility = Visibility.Visible;

      // Control button is pressed so more points are to be selected
      if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        return;
      }

      // Remove selection in VideoData
      foreach (TimeSample sample in App.Project.VideoData.FilteredSamples)
      {
        sample.IsSelected = false;
      }

      // Remove selection in series
      this.ChartData.DataScatterSeries.ClearSelection();
    }

    /// <summary>
    /// Plot area mouse left button up.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mouseDown)
      {
        Point currentPos = new Point(e.GetPosition(this.MyCanvas).X, e.GetPosition(this.MyCanvas).Y);
        this.mouseUpPositionInAxesCoordinates =
          this.ChartData.DataScatterSeries.InverseTransform(new ScreenPoint(currentPos.X, currentPos.Y));

        if (this.PointsAreNear(this.mouseDownPositionInCanvasCoordinates, currentPos))
        {
          DataPoint newDownPoint = new DataPoint(
            this.mouseDownPositionInAxesCoordinates.X - Math.Abs(this.mouseDownPositionInAxesCoordinates.X) * 0.02,
            this.mouseDownPositionInAxesCoordinates.Y - Math.Abs(this.mouseDownPositionInAxesCoordinates.Y) * 0.02);
          this.mouseDownPositionInAxesCoordinates = newDownPoint;

          DataPoint newUpPoint = new DataPoint(
            this.mouseUpPositionInAxesCoordinates.X - Math.Abs(this.mouseUpPositionInAxesCoordinates.X) * 0.02,
            this.mouseUpPositionInAxesCoordinates.Y - Math.Abs(this.mouseUpPositionInAxesCoordinates.Y) * 0.02);
          this.mouseUpPositionInAxesCoordinates = newUpPoint;

          //this.mouseDownPositionInAxesCoordinates.X -= Math.Abs(this.mouseDownPositionInAxesCoordinates.X) * 0.02;
          //this.mouseUpPositionInAxesCoordinates.X += Math.Abs(this.mouseUpPositionInAxesCoordinates.X) * 0.02;
          //this.mouseDownPositionInAxesCoordinates.Y -= Math.Abs(this.mouseDownPositionInAxesCoordinates.Y) * 0.02;
          //this.mouseUpPositionInAxesCoordinates.Y += Math.Abs(this.mouseUpPositionInAxesCoordinates.Y) * 0.02;
        }

        // Create current point list
        List<ScatterPoint> actualDataPoints = new List<ScatterPoint>();
        foreach (TimeSample item in App.Project.VideoData.FilteredSamples)
        {
          actualDataPoints.Add(this.ChartData.DataScatterSeries.Mapping(item));
        }

        // Get points in rectangle
        IOrderedEnumerable<ScatterPoint> selectedDataPoints = from dp in actualDataPoints
                                                              where
                                                                (dp.X
                                                                 >= Math.Min(
                                                                   this.mouseDownPositionInAxesCoordinates.X,
                                                                   this.mouseUpPositionInAxesCoordinates.X))
                                                                && (dp.X
                                                                    <= Math.Max(
                                                                      this.mouseDownPositionInAxesCoordinates.X,
                                                                      this.mouseUpPositionInAxesCoordinates.X))
                                                                && (dp.Y
                                                                    >= Math.Min(
                                                                      this.mouseDownPositionInAxesCoordinates.Y,
                                                                      this.mouseUpPositionInAxesCoordinates.Y))
                                                                && (dp.Y
                                                                    <= Math.Max(
                                                                      this.mouseDownPositionInAxesCoordinates.Y,
                                                                      this.mouseUpPositionInAxesCoordinates.Y))
                                                              orderby dp.X
                                                              select dp;

        if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0 && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
        {
          foreach (ScatterPoint dataPoint in selectedDataPoints)
          {
            int index = actualDataPoints.IndexOf(dataPoint);
            this.ChartData.DataScatterSeries.UnselectItem(index);
            App.Project.VideoData.FilteredSamples[index].IsSelected = false;
          }
        }
        else
        {
          foreach (ScatterPoint dataPoint in selectedDataPoints)
          {
            int index = actualDataPoints.IndexOf(dataPoint);
            this.ChartData.DataScatterSeries.SelectItem(index);
            App.Project.VideoData.FilteredSamples[index].IsSelected = true;
          }
        }

        // Reset selection to whole series, if no point is selected
        if (!selectedDataPoints.Any())
        {
          foreach (TimeSample sample in App.Project.VideoData.FilteredSamples)
          {
            sample.IsSelected = true;
          }
        }
      }

      this.mouseDown = false;
      this.SelectRect.Visibility = Visibility.Collapsed;

      this.Refresh();
    }

    /// <summary>
    /// Plot area mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaMouseMove(object sender, MouseEventArgs e)
    {
      if (this.mouseDown)
      {
        Point currentPos = new Point(e.GetPosition(this.MyCanvas).X, e.GetPosition(this.MyCanvas).Y);

        this.SelectRect.Visibility = Visibility.Visible;
        this.SelectRect.Width = Math.Abs(this.mouseDownPositionInCanvasCoordinates.X - currentPos.X);
        this.SelectRect.Height = Math.Abs(this.mouseDownPositionInCanvasCoordinates.Y - currentPos.Y);
        if (currentPos.X < this.mouseDownPositionInCanvasCoordinates.X)
        {
          this.SelectRect.SetValue(Canvas.LeftProperty, currentPos.X);
        }

        if (currentPos.Y < this.mouseDownPositionInCanvasCoordinates.Y)
        {
          this.SelectRect.SetValue(Canvas.TopProperty, currentPos.Y);
        }
      }
      else
      {
        this.SelectRect.Visibility = Visibility.Collapsed;
      }
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the PlotArea control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="KeyEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (Keyboard.Modifiers == ModifierKeys.Control)
      {
        this.DataChart.Cursor = this.plusCursor;
      }
      else if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0 && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
      {
        this.DataChart.Cursor = this.minusCursor;
      }
      else if (e.Key == Key.Delete)
      {
        if (App.Project.VideoData.Samples.AllSamplesSelected)
        {
          return;
        }

        VianaDialog dlg = new VianaDialog(
          VianaNET.Localization.Labels.AskDeleteDataTitle,
          VianaNET.Localization.Labels.AskDeleteDataMessageTitle,
          VianaNET.Localization.Labels.AskDeleteDataMessage,
          false);
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          App.Project.VideoData.DeleteSelectedSamples();
          this.ChartData.DataScatterSeries.ClearSelection();
        }
      }
    }

    ///// <summary>
    ///// Handles the SelectionChanged event of the Samples collection.
    ///// </summary>
    ///// <param name="sender">The source of the event.</param>
    ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    //private void SamplesSelectionChanged(object sender, EventArgs e)
    //{
    //this.ChartData.DataScatterSeries.ClearSelection();

    //if (!App.Project.VideoData.Samples.AllSamplesSelected)
    //{
    //  foreach (TimeSample timesample in App.Project.VideoData.FilteredSamples.Where(o => o.IsSelected))
    //  {
    //    int index = App.Project.VideoData.FilteredSamples.IndexOf(timesample);
    //    this.ChartData.DataScatterSeries.SelectItem(index);
    //  }
    //}
    //}

    /// <summary>
    /// Handles the PreviewKeyUp event of the PlotArea control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="KeyEventArgs"/> instance containing the event data.
    /// </param>
    private void PlotAreaPreviewKeyUp(object sender, KeyEventArgs e)
    {
      this.DataChart.Cursor = Cursors.Arrow;
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

      this.isSettingDefaultAxisPair = true;

      ChartType chartType = ChartType.YoverX;
      char achsBez = 'x';
      char funcBez = 'y';

      if (this.TabPositionSpace.IsSelected)
      {
        DataCharts chart = (DataCharts)this.AxesContentPositionSpace.SelectedItem;
        chartType = chart.Chart;
      }
      else if (this.TabPhaseSpace.IsSelected)
      {
        DataCharts chart = (DataCharts)this.AxesContentPhaseSpace.SelectedItem;
        chartType = chart.Chart;
      }

      App.Project.CurrentChartType = chartType;
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
      App.Project.CurrentFilterData.RegressionFilter.SetBezeichnungen(achsBez, funcBez);
      this.Refresh();

      this.isSettingDefaultAxisPair = false;
    }

    /// <summary>
    ///   The populate object combo.
    /// </summary>
    private void PopulateObjectCombo()
    {
      // Erase old entries
      this.ObjectDescriptions.Clear();

      for (int i = 0; i < App.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.ObjectDescriptions.Add(VianaNET.Localization.Labels.DataGridObjectPrefix + " " + (i + 1).ToString(CultureInfo.InvariantCulture));
      }

      // this.ObjectSelectionCombo.ItemsSource = null;
      this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
      Binding indexBinding = new Binding("ProcessingData.IndexOfObject") { Source = App.Project };
      this.ObjectSelectionCombo.SetBinding(Selector.SelectedIndexProperty, indexBinding);
      App.Project.ProcessingData.IndexOfObject++;
    }

    /// <summary>
    /// The image processing property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
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
    /// Handles the UpdateChartRequested event of the Project control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="EventArgs"/> instance containing the event data.
    /// </param>
    private void ProjectUpdateChartRequested(object sender, EventArgs e)
    {
      if (Project.IsDeserializing)
      {
        this.XAxisContent.SelectedValue = App.Project.CurrentFilterData.AxisX.Axis;
        this.YAxisContent.SelectedValue = App.Project.CurrentFilterData.AxisY.Axis;
        this.TabOther.IsSelected = true;
      }

      this.Refresh();
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
      CalculatorAndFktEditor calculator = new CalculatorAndFktEditor(TRechnerArt.rechner);
      calculator.ShowDialog();
    }

    /// <summary>
    ///   Updates the function term visual with a tex representation of the regression function
    /// </summary>
    private void RefreshRegressionFuctionTerm()
    {
      if (App.Project.CurrentFilterData.RegressionFunctionTexFormula != null)
      {
        // Render formula to visual.
        DrawingVisual visual = new DrawingVisual();
        TexRenderer renderer = App.Project.CurrentFilterData.RegressionFunctionTexFormula.GetRenderer(
          TexStyle.Display,
          14d,
          "Arial");

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
    ///   Updates the theoretical term visual with a tex representation of the theoretical function
    /// </summary>
    private void RefreshTheorieFunctionTerm()
    {
      // Only if we have a formula and should display the theory series
      if (App.Project.CurrentFilterData.TheoryFunctionTexFormula != null)
      {
        // Render formula to visual.
        DrawingVisual visual = new DrawingVisual();
        TexRenderer renderer = App.Project.CurrentFilterData.TheoryFunctionTexFormula.GetRenderer(
          TexStyle.Display,
          14d,
          "Arial");

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

      App.Project.CurrentFilterData.NotifyTheoryTermChange();
    }

    /// <summary>
    /// Regression style button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void RegressionStyleButtonClick(object sender, RoutedEventArgs e)
    {
      LineOptionsDialog lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = App.Project.CurrentFilterData.RegressionLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = App.Project.CurrentFilterData.RegressionLineColor.ToOxyColor();
      lineOptionsDialog.LineStyleControl.MarkerType = App.Project.CurrentFilterData.RegressionLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        App.Project.CurrentFilterData.RegressionLineThickness =
          lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        App.Project.CurrentFilterData.RegressionLineColor = lineOptionsDialog.LineStyleControl.SeriesColor.ToColor();
        App.Project.CurrentFilterData.RegressionLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Regression type button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void RegressionTypeButtonClick(object sender, RoutedEventArgs e)
    {
      RegressionOptionsDialog regressionOptionsDialog = new RegressionOptionsDialog(App.Project.CurrentFilterData.RegressionFilter);
      if (!regressionOptionsDialog.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      if (regressionOptionsDialog.RegressionType == RegressionType.Best)
      {
        if (App.Project.CurrentFilterData.RegressionFilter.WertX.Count == 0)
        {
          App.Project.CurrentFilterData.RegressionFilter.CalculateFilterValues();
        }

        App.Project.CurrentFilterData.RegressionFilter.GetBestRegressData(
          out RegressionType bestRegression,
          regressionOptionsDialog.negFlag);
        this.UpdateRegressionImageButtonAndLabels(bestRegression);
      }
      else
      {
        this.UpdateRegressionImageButtonAndLabels(regressionOptionsDialog.RegressionType);
      }
    }

    /// <summary>
    /// Handles the OnClick event of the RescaleAxesButton control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void RescaleAxesButton_OnClick(object sender, RoutedEventArgs e)
    {
      this.ChartData.ChartDataModel.ResetAllAxes();
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Shortcut help button was clicked, so show the dialog.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ShortCutHelpButtonClick(object sender, RoutedEventArgs e)
    {
      ChartHelpDialog dialog = new ChartHelpDialog();
      dialog.ShowDialog();
    }

    /// <summary>
    /// The interpolation check box was checked.
    ///   So update the interpolation series.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ShowInterpolationCheckBoxCheckedChanged(object sender, RoutedEventArgs e)
    {
      App.Project.CurrentFilterData.CalculateInterpolationSeriesDataPoints();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The show regression check box checked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ShowRegressionCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      App.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();

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
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ShowRegressionCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      App.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();

      // Funktionsterm und mittleres Fehlerquadrat nicht mehr anzeigen
      App.Project.CurrentFilterData.RegressionFunctionTexFormula = null;
      App.Project.CurrentFilterData.RegressionAberration = 0;
      this.RefreshRegressionFuctionTerm();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The check box show theorie checked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ShowTheorieCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      App.Project.CurrentFilterData.CalculateTheorySeriesDataPoints();
      this.UpdateTheoryFormula();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The show theorie check box unchecked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void ShowTheorieCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      App.Project.CurrentFilterData.CalculateTheorySeriesDataPoints();

      // App.Project.CurrentFilterData.TheoryFunctionTexFormula = null;
      // this.RefreshTheorieFunctionTerm();
      this.ChartData.UpdateModel();
    }

    /// <summary>
    /// The theory options button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void TheoryOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      CalculatorAndFktEditor fktEditor = new CalculatorAndFktEditor(TRechnerArt.formelRechner);
      fktEditor.buttonX.Content = string.Concat(this.axisName);
      Constants.varName = string.Concat(this.axisName);
      if (App.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree != null)
      {
        fktEditor.textBox1.Text = App.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree.Name;
        fktEditor.textBox1.SelectAll();
      }

      fktEditor.ShowDialog();

      if (fktEditor.DialogResult.GetValueOrDefault(false))
      {
        App.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree = fktEditor.GetFunktion();
        App.Project.CurrentFilterData.IsShowingTheorySeries = true;
        this.UpdateTheoryFormula();
      }
    }

    /// <summary>
    /// Theory style button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void TheoryStyleButtonClick(object sender, RoutedEventArgs e)
    {
      LineOptionsDialog lineOptionsDialog = new LineOptionsDialog();
      lineOptionsDialog.LineStyleControl.SeriesStrokeThickness = App.Project.CurrentFilterData.TheoryLineThickness;
      lineOptionsDialog.LineStyleControl.SeriesColor = App.Project.CurrentFilterData.TheoryLineColor.ToOxyColor();
      lineOptionsDialog.LineStyleControl.MarkerType = App.Project.CurrentFilterData.TheoryLineMarkerType;
      lineOptionsDialog.ShowDialog();

      if (lineOptionsDialog.DialogResult == true)
      {
        App.Project.CurrentFilterData.TheoryLineThickness = lineOptionsDialog.LineStyleControl.SeriesStrokeThickness;
        App.Project.CurrentFilterData.TheoryLineColor = lineOptionsDialog.LineStyleControl.SeriesColor.ToColor();
        App.Project.CurrentFilterData.TheoryLineMarkerType = lineOptionsDialog.LineStyleControl.MarkerType;
        this.ChartData.UpdateAppearance();
      }
    }

    /// <summary>
    /// Title text box text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="TextChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void TitleTextBoxChanged(object sender, TextChangedEventArgs e)
    {
      this.ChartData.ChartDataModel.Title = this.ChartTitleTextBox.Text;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
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
          unitname = App.Project.CalibrationData.TimeUnit.ToString();
          break;
        case AxisType.X:
          datafield = "PixelX";
          unitname = App.Project.CalibrationData.PixelUnit;
          break;
        case AxisType.Y:
          datafield = "PixelY";
          unitname = App.Project.CalibrationData.PixelUnit;
          break;
        case AxisType.PX:
          datafield = "PositionX";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.PY:
          datafield = "PositionY";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.D:
          datafield = "Distance";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.DX:
          datafield = "DistanceX";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.DY:
          datafield = "DistanceY";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.S:
          datafield = "Length";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.SX:
          datafield = "LengthX";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.SY:
          datafield = "LengthY";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
        case AxisType.V:
          datafield = "Velocity";
          unitname = App.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VX:
          datafield = "VelocityX";
          unitname = App.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VY:
          datafield = "VelocityY";
          unitname = App.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VI:
          datafield = "VelocityI";
          unitname = App.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VXI:
          datafield = "VelocityXI";
          unitname = App.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.VYI:
          datafield = "VelocityYI";
          unitname = App.Project.CalibrationData.VelocityUnit;
          break;
        case AxisType.A:
          datafield = "Acceleration";
          unitname = App.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AX:
          datafield = "AccelerationX";
          unitname = App.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AY:
          datafield = "AccelerationY";
          unitname = App.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AI:
          datafield = "AccelerationI";
          unitname = App.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AXI:
          datafield = "AccelerationXI";
          unitname = App.Project.CalibrationData.AccelerationUnit;
          break;
        case AxisType.AYI:
          datafield = "AccelerationYI";
          unitname = App.Project.CalibrationData.AccelerationUnit;
          break;
        default:
          datafield = "PositionX";
          unitname = App.Project.CalibrationData.LengthUnit.ToString();
          break;
      }
    }

    /// <summary>
    /// This method updates the regression button with
    ///   an image corresponding to the selected regression type
    /// </summary>
    /// <param name="aktregressionType">
    /// The aktual selected regression type.
    /// </param>
    private void UpdateRegressionImageButtonAndLabels(RegressionType aktregressionType)
    {
      string bildsource;
      App.Project.CurrentFilterData.RegressionFilter.RegressionType = aktregressionType;
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

      Uri neuBildsource = new Uri(bildsource, UriKind.RelativeOrAbsolute);
      this.RegressionTypeButton.ImageSource = new BitmapImage(neuBildsource);

      if (this.RegressionCheckBox.IsChecked.GetValueOrDefault(false))
      {
        App.Project.CurrentFilterData.CalculateRegressionSeriesDataPoints();
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
        if (App.Project.CurrentFilterData.TheoryFilter == null
            || App.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree == null)
        {
          return;
        }

        string functionString = App.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree.Name;
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
          App.Project.CurrentFilterData.TheoryFunctionTexFormula = formula;
        }

        this.RefreshTheorieFunctionTerm();
      }
      catch (Exception)
      {
        App.Project.CurrentFilterData.TheoryFunctionTexFormula = null;
      }
    }

    /// <summary>
    /// The x axis content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="SelectionChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void XAxisContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.isSettingDefaultAxisPair)
      {
        return;
      }

      App.Project.CurrentChartType = ChartType.Custom;
      DataAxis axisX = (DataAxis)this.XAxisContent.SelectedItem;
      char achsBez = axisX.Axis == AxisType.T ? 't' : 'x';
      char funcBez = 'y';
      this.axisName = achsBez;
      App.Project.CurrentFilterData.RegressionFilter.SetBezeichnungen(achsBez, funcBez);

      this.Refresh();
    }

    /// <summary>
    /// X Axis show grid lines CheckBox checked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void XAxisShowGridLinesCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.XAxis.MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139);
      this.ChartData.XAxis.MajorGridlineStyle = LineStyle.Solid;
      this.ChartData.XAxis.MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139);
      this.ChartData.XAxis.MinorGridlineStyle = LineStyle.Solid;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// X axis show grid lines CheckBox unchecked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void XAxisShowGridLinesCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.XAxis.MajorGridlineStyle = LineStyle.None;
      this.ChartData.XAxis.MinorGridlineStyle = LineStyle.None;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// The y axis content selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="SelectionChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void YAxisContentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.isSettingDefaultAxisPair)
      {
        return;
      }

      App.Project.CurrentChartType = ChartType.Custom;
      DataAxis axisY = (DataAxis)this.YAxisContent.SelectedItem;
      char achsBez = axisY.Axis == AxisType.T ? 't' : 'y';
      char funcBez = 'y';
      this.axisName = achsBez;
      App.Project.CurrentFilterData.RegressionFilter.SetBezeichnungen(achsBez, funcBez);

      this.Refresh();
    }

    /// <summary>
    /// Y axis show grid lines CheckBox checked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void YAxisShowGridLinesCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.YAxis.MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139);
      this.ChartData.YAxis.MajorGridlineStyle = LineStyle.Solid;
      this.ChartData.YAxis.MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139);
      this.ChartData.YAxis.MinorGridlineStyle = LineStyle.Solid;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }

    /// <summary>
    /// Y axis show grid lines CheckBox unchecked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void YAxisShowGridLinesCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      this.ChartData.YAxis.MajorGridlineStyle = LineStyle.None;
      this.ChartData.YAxis.MinorGridlineStyle = LineStyle.None;
      this.ChartData.ChartDataModel.InvalidatePlot(false);
    }


  }
}