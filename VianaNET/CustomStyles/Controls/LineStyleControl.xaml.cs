// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineStyleControl.xaml.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  using OxyPlot;
  using OxyPlot.Axes;
  using OxyPlot.Series;

  /// <summary>
  ///   Interaction logic for LineStyleControl.xaml
  /// </summary>
  public partial class LineStyleControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="LineStyleControl" /> class.
    /// </summary>
    public LineStyleControl()
    {
      this.ChartModel = this.CreateLineSeries();
      this.InitializeComponent();

      // Populate markertype combo
      foreach (MarkerType markerType in Enum.GetValues(typeof(MarkerType)).Cast<MarkerType>())
      {
        if (markerType == MarkerType.Custom)
        {
          continue;
        }

        this.MarkerTypeComboBox.Items.Add(markerType);
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the chart model.
    /// </summary>
    /// <value>
    ///   The chart model.
    /// </value>
    public PlotModel ChartModel { get; set; }

    /// <summary>
    /// Gets or sets the marker type.
    /// </summary>
    public MarkerType MarkerType
    {
      get => this.Series.MarkerType;

      set
      {
        this.Series.MarkerType = value;
        switch (value)
        {
          case MarkerType.Circle:
          case MarkerType.Square:
          case MarkerType.Diamond:
          case MarkerType.Triangle:
            this.Series.MarkerStroke = OxyColors.White;
            break;
          case MarkerType.Cross:
          case MarkerType.Plus:
          case MarkerType.Star:
          case MarkerType.Custom:
            this.Series.MarkerStroke = OxyColors.Black;
            break;
        }

        this.Series.MarkerStrokeThickness = 1.5;
        this.MarkerTypeComboBox.SelectedItem = value;

        this.ChartModel.InvalidatePlot(false);
      }
    }

    /// <summary>
    ///   Gets or sets the color of the series.
    /// </summary>
    /// <value>
    ///   The color of the series.
    /// </value>
    public OxyColor SeriesColor
    {
      get => this.Series.Color;

      set
      {
        this.Series.Color = value;
        this.Series.MarkerFill = value;
        this.ColorPicker.SelectedColor = Color.FromArgb(value.A, value.R, value.G, value.B);
        this.ChartModel.InvalidatePlot(false);
      }
    }

    /// <summary>
    ///   Gets or sets the series stroke thickness.
    /// </summary>
    /// <value>
    ///   The series stroke thickness.
    /// </value>
    public double SeriesStrokeThickness
    {
      get => this.Series.StrokeThickness;

      set
      {
        this.Series.StrokeThickness = value;
        this.Series.MarkerSize = value + 3;
        this.ThicknessSlider.Value = value;
        this.ChartModel.InvalidatePlot(false);
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the sample series
    /// </summary>
    private LineSeries Series => (LineSeries)this.ChartModel.Series[0];

    #endregion

    #region Methods

    /// <summary>
    /// The color picker selected color changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedPropertyChangedEventArgs{Color}"/> instance containing the event
    ///   data.
    /// </param>
    private void ColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
    {
      Color c = this.ColorPicker.SelectedColor.Value;
      this.SeriesColor = OxyColor.FromArgb(c.A, c.R, c.G, c.B);
    }

    /// <summary>
    /// The create line series.
    /// </summary>
    /// <returns>
    /// The <see cref="PlotModel"/>.
    /// </returns>
    private PlotModel CreateLineSeries()
    {
      PlotModel plotModel1 = new PlotModel();
      plotModel1.IsLegendVisible = false;
      plotModel1.Title = VianaNET.Localization.Labels.LineStyleControlChartTitle;
      LinearAxis linearAxis1 = new LinearAxis();
      linearAxis1.Position = AxisPosition.Bottom;
      linearAxis1.MaximumPadding = 0.1;
      linearAxis1.MinimumPadding = 0.1;
      plotModel1.Axes.Add(linearAxis1);
      LinearAxis linearAxis2 = new LinearAxis();
      linearAxis2.MaximumPadding = 0.1;
      linearAxis2.MinimumPadding = 0.1;
      plotModel1.Axes.Add(linearAxis2);
      LineSeries lineSeries1 = new LineSeries();
      lineSeries1.Color = OxyColors.SkyBlue;
      lineSeries1.MarkerFill = OxyColors.SkyBlue;
      lineSeries1.MarkerSize = 6;
      lineSeries1.MarkerStroke = OxyColors.White;
      lineSeries1.MarkerStrokeThickness = 1.5;
      lineSeries1.MarkerType = MarkerType.Circle;
      lineSeries1.Points.Add(new DataPoint(0, 10));
      lineSeries1.Points.Add(new DataPoint(10, 40));
      lineSeries1.Points.Add(new DataPoint(40, 20));
      lineSeries1.Points.Add(new DataPoint(60, 30));
      plotModel1.Series.Add(lineSeries1);
      return plotModel1;
    }

    /// <summary>
    /// Handles the OnSelectionChanged event of the MarkerTypeComboBox control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="SelectionChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void MarkerTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.MarkerType = (MarkerType)this.MarkerTypeComboBox.SelectedItem;
    }

    /// <summary>
    /// Thicknesse slider value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="double"/> instance containing the event data.
    /// </param>
    private void ThicknessSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this.SeriesStrokeThickness = e.NewValue;
    }

    #endregion
  }
}