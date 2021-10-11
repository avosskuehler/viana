// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterData.cs" company="Freie Universität Berlin">
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
namespace VianaNET.Data.Filter
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Media;
  using System.Xml.Serialization;

  using OxyPlot;

  using VianaNET.CustomStyles.Types;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter.Interpolation;
  using VianaNET.Data.Filter.Regression;
  using VianaNET.Data.Filter.Theory;

  using WpfMath;

  /// <summary>
  ///   The video data.
  /// </summary>
  [Serializable]
  public class FilterData : DependencyObject, INotifyPropertyChanged
  {


    /// <summary>
    ///   The AxisX property.
    /// </summary>
    public static readonly DependencyProperty AxisXProperty = DependencyProperty.Register(
      "AxisX",
      typeof(DataAxis),
      typeof(FilterData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The AxisY property.
    /// </summary>
    public static readonly DependencyProperty AxisYProperty = DependencyProperty.Register(
      "AxisY",
      typeof(DataAxis),
      typeof(FilterData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="CurrentFilter" />.
    /// </summary>
    public static readonly DependencyProperty CurrentFilterProperty = DependencyProperty.Register(
      "CurrentFilter",
      typeof(FilterBase),
      typeof(FilterData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The data line color property.
    /// </summary>
    public static readonly DependencyProperty DataLineColorProperty = DependencyProperty.Register(
      "DataLineColor",
      typeof(Color),
      typeof(FilterData),
      new UIPropertyMetadata(Colors.LightBlue));

    /// <summary>
    ///   The data line marker type property.
    /// </summary>
    public static readonly DependencyProperty DataLineMarkerTypeProperty =
      DependencyProperty.Register(
        "DataLineMarkerType",
        typeof(MarkerType),
        typeof(FilterData),
        new UIPropertyMetadata(MarkerType.Circle));

    /// <summary>
    ///   The data line thickness property.
    /// </summary>
    public static readonly DependencyProperty DataLineThicknessProperty =
      DependencyProperty.Register(
      "DataLineThickness", 
      typeof(double), 
      typeof(FilterData), 
      new UIPropertyMetadata(2d));

    /// <summary>
    ///   The InterpolationFilter property.
    /// </summary>
    public static readonly DependencyProperty InterpolationFilterProperty =
      DependencyProperty.Register(
        "InterpolationFilter",
        typeof(InterpolationFilter),
        typeof(FilterData),
        new UIPropertyMetadata(null));

    /// <summary>
    ///   The Interpolation line color property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineColorProperty =
      DependencyProperty.Register(
        "InterpolationLineColor",
        typeof(Color),
        typeof(FilterData),
        new UIPropertyMetadata(Colors.Brown));

    /// <summary>
    ///   The Interpolation line marker type property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineMarkerTypeProperty =
      DependencyProperty.Register(
        "InterpolationLineMarkerType",
        typeof(MarkerType),
        typeof(FilterData),
        new UIPropertyMetadata(MarkerType.None));

    /// <summary>
    ///   The Interpolation line thickness property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineThicknessProperty =
      DependencyProperty.Register(
        "InterpolationLineThickness",
        typeof(double),
        typeof(FilterData),
        new UIPropertyMetadata(2d));

    /// <summary>
    ///   The InterpolationSeries property.
    /// </summary>
    public static readonly DependencyProperty InterpolationSeriesProperty =
      DependencyProperty.Register(
        "InterpolationSeries",
        typeof(SortedObservableCollection<XYSample>),
        typeof(FilterData),
        new UIPropertyMetadata(new SortedObservableCollection<XYSample>()));

    /// <summary>
    ///   The IsShowingDataSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingDataSeriesProperty =
      DependencyProperty.Register(
        "IsShowingDataSeries",
        typeof(bool),
        typeof(FilterData),
        new FrameworkPropertyMetadata(true));

    /// <summary>
    ///   The IsShowingInterpolationSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingInterpolationSeriesProperty =
      DependencyProperty.Register(
        "IsShowingInterpolationSeries",
        typeof(bool),
        typeof(FilterData),
        new FrameworkPropertyMetadata(false));

    /// <summary>
    ///   The IsShowingRegressionSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingRegressionSeriesProperty =
      DependencyProperty.Register(
        "IsShowingRegressionSeries",
        typeof(bool),
        typeof(FilterData),
        new FrameworkPropertyMetadata(false));

    /// <summary>
    ///   The IsShowingTheorySeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingTheorySeriesProperty =
      DependencyProperty.Register(
        "IsShowingTheorySeries",
        typeof(bool),
        typeof(FilterData),
        new FrameworkPropertyMetadata(false));

    /// <summary>
    ///   The NumericPrecision property.
    /// </summary>
    public static readonly DependencyProperty NumericPrecisionProperty = DependencyProperty.Register(
      "NumericPrecision",
      typeof(int),
      typeof(FilterData),
      new UIPropertyMetadata(2));

    /// <summary>
    ///   Gets the Abweichung der Ausgleichsfunktion
    /// </summary>
    public static readonly DependencyProperty RegressionAberrationProperty =
      DependencyProperty.Register(
        "RegressionAberration",
        typeof(double),
        typeof(FilterData),
        new UIPropertyMetadata(0d));

    /// <summary>
    ///   The RegressionFilter property.
    /// </summary>
    public static readonly DependencyProperty RegressionFilterProperty = DependencyProperty.Register(
      "RegressionFilter",
      typeof(RegressionFilter),
      typeof(FilterData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   Gets the function termin für die Ausgleichsfunktion
    /// </summary>
    public static readonly DependencyProperty RegressionFunctionTexFormulaProperty =
      DependencyProperty.Register(
        "RegressionFunctionTexFormula",
        typeof(TexFormula),
        typeof(FilterData),
        new UIPropertyMetadata(null));

    /// <summary>
    ///   The Regression line color property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineColorProperty =
      DependencyProperty.Register(
        "RegressionLineColor",
        typeof(Color),
        typeof(FilterData),
        new UIPropertyMetadata(Colors.Red));

    /// <summary>
    ///   The Regression line marker type property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineMarkerTypeProperty =
      DependencyProperty.Register(
        "RegressionLineMarkerType",
        typeof(MarkerType),
        typeof(FilterData),
        new UIPropertyMetadata(MarkerType.None));

    /// <summary>
    ///   The Regression line thickness property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineThicknessProperty =
      DependencyProperty.Register(
        "RegressionLineThickness",
        typeof(double),
        typeof(FilterData),
        new UIPropertyMetadata(2d));

    /// <summary>
    ///   The data line color property.
    /// </summary>
    public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register(
      "SelectionColor",
      typeof(Color),
      typeof(FilterData),
      new UIPropertyMetadata(Colors.Blue));

    /// <summary>
    ///   The InterpolationObject property.
    /// </summary>
    public static readonly DependencyProperty TheoryFilterProperty = DependencyProperty.Register(
      "TheoryFilter",
      typeof(TheoryFilter),
      typeof(FilterData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   Gets the function term for the theory function
    /// </summary>
    public static readonly DependencyProperty TheoryFunctionTexFormulaProperty =
      DependencyProperty.Register(
        "TheoryFunctionTexFormula",
        typeof(TexFormula),
        typeof(FilterData),
        new UIPropertyMetadata(null));

    /// <summary>
    ///   The Theory line color property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineColorProperty = DependencyProperty.Register(
      "TheoryLineColor",
      typeof(Color),
      typeof(FilterData),
      new UIPropertyMetadata(Colors.GreenYellow));

    /// <summary>
    ///   The Theory line marker type property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineMarkerTypeProperty =
      DependencyProperty.Register(
        "TheoryLineMarkerType",
        typeof(MarkerType),
        typeof(FilterData),
        new UIPropertyMetadata(MarkerType.None));

    /// <summary>
    ///   The Theory line thickness property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineThicknessProperty =
      DependencyProperty.Register(
        "TheoryLineThickness",
        typeof(double),
        typeof(FilterData),
        new UIPropertyMetadata(2d));





    /// <summary>
    ///   Initializes a new instance of the <see cref="FilterData" /> class.
    /// </summary>
    public FilterData()
    {
      this.InterpolationFilter = InterpolationFilter.Filter[InterpolationFilterTypes.MovingAverage];
      this.RegressionFilter = new RegressionFilter();
    }





    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;





    /// <summary>
    ///   Gets or sets the x axis currently selected in the chart window.
    /// </summary>
    public DataAxis AxisX
    {
      get => (DataAxis)this.GetValue(AxisXProperty);

      set
      {
        if (this.AxisX != value)
        {
          this.SetValue(AxisXProperty, value);
        }
      }
    }

    /// <summary>
    ///   Gets or sets the y axis currently selected in the chart window.
    /// </summary>
    public DataAxis AxisY
    {
      get => (DataAxis)this.GetValue(AxisYProperty);

      set
      {
        if (this.AxisY != value)
        {
          this.SetValue(AxisYProperty, value);
        }
      }
    }

    /// <summary>
    ///   Gets or sets a the interpolation filter to use for
    ///   smoothing data.
    /// </summary>
    public FilterBase CurrentFilter
    {
      get => (FilterBase)this.GetValue(CurrentFilterProperty);

      set => this.SetValue(CurrentFilterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the data line color.
    /// </summary>
    public Color DataLineColor
    {
      get => (Color)this.GetValue(DataLineColorProperty);

      set => this.SetValue(DataLineColorProperty, value);
    }

    /// <summary>
    ///   Gets or sets the data line marker type.
    /// </summary>
    public MarkerType DataLineMarkerType
    {
      get => (MarkerType)this.GetValue(DataLineMarkerTypeProperty);

      set => this.SetValue(DataLineMarkerTypeProperty, value);
    }

    /// <summary>
    ///   Gets or sets the data line thickness.
    /// </summary>
    public double DataLineThickness
    {
      get => (double)this.GetValue(DataLineThicknessProperty);

      set => this.SetValue(DataLineThicknessProperty, value);
    }

    /// <summary>
    ///   Gets a value indicating whether this instance has theory function.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has theory function; otherwise, <c>false</c>.
    /// </value>
    public bool HasTheoryFunction
    {
      get
      {
        if (this.TheoryFilter == null)
        {
          return false;
        }

        return this.TheoryFilter.TheoreticalFunctionCalculatorTree != null;
      }
    }

    /// <summary>
    ///   Gets or sets the InterpolationFilter
    /// </summary>
    public InterpolationFilter InterpolationFilter
    {
      get => (InterpolationFilter)this.GetValue(InterpolationFilterProperty);

      set => this.SetValue(InterpolationFilterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Interpolation line color.
    /// </summary>
    public Color InterpolationLineColor
    {
      get => (Color)this.GetValue(InterpolationLineColorProperty);

      set => this.SetValue(InterpolationLineColorProperty, value);
    }

    /// <summary>
    ///   Gets or sets the interpolation line marker type.
    /// </summary>
    public MarkerType InterpolationLineMarkerType
    {
      get => (MarkerType)this.GetValue(InterpolationLineMarkerTypeProperty);

      set => this.SetValue(InterpolationLineMarkerTypeProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Interpolation line  thickness.
    /// </summary>
    public double InterpolationLineThickness
    {
      get => (double)this.GetValue(InterpolationLineThicknessProperty);

      set => this.SetValue(InterpolationLineThicknessProperty, value);
    }

    /// <summary>
    ///   Gets or sets the InterpolationSeries.
    /// </summary>
    [XmlIgnore]
    public SortedObservableCollection<XYSample> InterpolationSeries
    {
      get => (SortedObservableCollection<XYSample>)this.GetValue(InterpolationSeriesProperty);

      set => this.SetValue(InterpolationSeriesProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the chart should display the data series.
    /// </summary>
    public bool IsShowingDataSeries
    {
      get => (bool)this.GetValue(IsShowingDataSeriesProperty);

      set => this.SetValue(IsShowingDataSeriesProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the chart should display the interpolation series.
    /// </summary>
    public bool IsShowingInterpolationSeries
    {
      get => (bool)this.GetValue(IsShowingInterpolationSeriesProperty);

      set => this.SetValue(IsShowingInterpolationSeriesProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the chart should display the regression series.
    /// </summary>
    public bool IsShowingRegressionSeries
    {
      get => (bool)this.GetValue(IsShowingRegressionSeriesProperty);

      set => this.SetValue(IsShowingRegressionSeriesProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the chart should display the theory series.
    /// </summary>
    public bool IsShowingTheorySeries
    {
      get => (bool)this.GetValue(IsShowingTheorySeriesProperty);

      set => this.SetValue(IsShowingTheorySeriesProperty, value);
    }

    /// <summary>
    ///   Gets or sets the NumericPrecision.
    /// </summary>
    public int NumericPrecision
    {
      get => (int)this.GetValue(NumericPrecisionProperty);

      set
      {
        this.SetValue(NumericPrecisionProperty, value);
        this.OnPropertyChanged("NumericPrecisionString");
        if (this.RegressionFilter != null)
        {
          this.RegressionFilter.UpdateRegressionFunctionString();
        }
      }
    }

    /// <summary>
    ///   Gets or sets the regression aberration.
    /// </summary>
    public double RegressionAberration
    {
      get => (double)this.GetValue(RegressionAberrationProperty);

      set
      {
        this.SetValue(RegressionAberrationProperty, value);
        if (this.IsShowingRegressionSeries)
        {
          this.OnPropertyChanged("RegressionAberration");
        }
      }
    }

    /// <summary>
    ///   Gets or sets the RegressionFilter
    /// </summary>
    public RegressionFilter RegressionFilter
    {
      get => (RegressionFilter)this.GetValue(RegressionFilterProperty);

      set => this.SetValue(RegressionFilterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the regression function tex formula.
    /// </summary>
    public TexFormula RegressionFunctionTexFormula
    {
      get => (TexFormula)this.GetValue(RegressionFunctionTexFormulaProperty);

      set
      {
        this.SetValue(RegressionFunctionTexFormulaProperty, value);
        this.OnPropertyChanged("RegressionFunctionTexFormula");
      }
    }

    /// <summary>
    ///   Gets or sets the Regression line color.
    /// </summary>
    public Color RegressionLineColor
    {
      get => (Color)this.GetValue(RegressionLineColorProperty);

      set => this.SetValue(RegressionLineColorProperty, value);
    }

    /// <summary>
    ///   Gets or sets the regression line marker type.
    /// </summary>
    public MarkerType RegressionLineMarkerType
    {
      get => (MarkerType)this.GetValue(RegressionLineMarkerTypeProperty);

      set => this.SetValue(RegressionLineMarkerTypeProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Regression line thickness.
    /// </summary>
    public double RegressionLineThickness
    {
      get => (double)this.GetValue(RegressionLineThicknessProperty);

      set => this.SetValue(RegressionLineThicknessProperty, value);
    }

    /// <summary>
    ///   Gets or sets the selection color.
    /// </summary>
    public Color SelectionColor
    {
      get => (Color)this.GetValue(SelectionColorProperty);

      set => this.SetValue(SelectionColorProperty, value);
    }

    /// <summary>
    ///   Gets or sets the TheoryObject
    /// </summary>
    public TheoryFilter TheoryFilter
    {
      get => (TheoryFilter)this.GetValue(TheoryFilterProperty);

      set => this.SetValue(TheoryFilterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Theory function tex formula.
    /// </summary>
    public TexFormula TheoryFunctionTexFormula
    {
      get => (TexFormula)this.GetValue(TheoryFunctionTexFormulaProperty);

      set
      {
        this.SetValue(TheoryFunctionTexFormulaProperty, value);
        this.OnPropertyChanged("TheoryFunctionTexFormula");
      }
    }

    /// <summary>
    ///   Gets or sets the Theory line color.
    /// </summary>
    public Color TheoryLineColor
    {
      get => (Color)this.GetValue(TheoryLineColorProperty);

      set => this.SetValue(TheoryLineColorProperty, value);
    }

    /// <summary>
    ///   Gets or sets the theory line marker type.
    /// </summary>
    public MarkerType TheoryLineMarkerType
    {
      get => (MarkerType)this.GetValue(TheoryLineMarkerTypeProperty);

      set => this.SetValue(TheoryLineMarkerTypeProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Theory line thickness.
    /// </summary>
    public double TheoryLineThickness
    {
      get => (double)this.GetValue(TheoryLineThicknessProperty);

      set => this.SetValue(TheoryLineThicknessProperty, value);
    }





    /// <summary>
    ///   Calculates Interpolation series data points.
    /// </summary>
    public void CalculateInterpolationSeriesDataPoints()
    {
      if (this.InterpolationFilter == null)
      {
        return;
      }

      if (this.IsShowingInterpolationSeries)
      {
        this.InterpolationFilter.CalculateFilterValues();
      }
      else
      {
        this.InterpolationSeries.Clear();
      }
    }

    /// <summary>
    ///   Calculates regression series data points.
    /// </summary>
    public void CalculateRegressionSeriesDataPoints()
    {
      if (this.RegressionFilter == null)
      {
        return;
      }

      if (this.IsShowingRegressionSeries)
      {
        this.RegressionFilter.CalculateFilterValues();
        this.RegressionFilter.UpdateLinefitFunctionData();
      }
    }

    /// <summary>
    ///   Calculates theory series data points.
    /// </summary>
    public void CalculateTheorySeriesDataPoints()
    {
      if (this.TheoryFilter == null)
      {
        this.TheoryFilter = new TheoryFilter();
      }

      if (this.TheoryFilter.TheoreticalFunctionCalculatorTree == null)
      {
        return;
      }

      if (this.IsShowingTheorySeries)
      {
        this.TheoryFilter.CalculateFilterValues();
      }
    }

    /// <summary>
    /// Returns a string that formatted the given value using
    ///   the current numeric precision value.
    /// </summary>
    /// <param name="a">
    /// A <see cref="double"/> which has to be converted to a string.
    /// </param>
    /// <returns>
    /// The converted string, e.g. 0,003 or 300
    /// </returns>
    public string GetFormattedString(double a)
    {
      if (a > -1 && a < 1)
      {
        return a.ToString("G" + this.NumericPrecision.ToString(CultureInfo.InvariantCulture));
      }

      return a.ToString("N" + this.NumericPrecision.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///   Notifies changes of all properties, so that
    ///   the data bindings to the view were updated.
    /// </summary>
    public void NotifyChanges()
    {
      this.OnPropertyChanged("CurrentFilter");
      this.OnPropertyChanged("DataLineThickness");
      this.OnPropertyChanged("DataLineColor");
      this.OnPropertyChanged("SelectionColor");
      this.OnPropertyChanged("RegressionLineThickness");
      this.OnPropertyChanged("RegressionLineColor");
      this.OnPropertyChanged("InterpolationLineThickness");
      this.OnPropertyChanged("InterpolationLineColor");
      this.OnPropertyChanged("TheoryLineThickness");
      this.OnPropertyChanged("TheoryLineColor");
      this.OnPropertyChanged("RegressionFilter");
      this.OnPropertyChanged("InterpolationFilter");
      this.OnPropertyChanged("TheoryFilter");
      this.OnPropertyChanged("IsShowingDataSeries");
      this.OnPropertyChanged("IsShowingInterpolationSeries");
      this.OnPropertyChanged("IsShowingRegressionSeries");
      this.OnPropertyChanged("IsShowingTheorySeries");
      this.OnPropertyChanged("NumericPrecision");
      this.OnPropertyChanged("InterpolationSeries");
      this.OnPropertyChanged("AxisX");
      this.OnPropertyChanged("AxisY");
      this.OnPropertyChanged("RegressionFunctionTexFormula");
      this.OnPropertyChanged("TheoryFunctionTexFormula");
      this.OnPropertyChanged("RegressionAberration");
    }

    /// <summary>
    ///   Notifies the theory term change.
    /// </summary>
    public void NotifyTheoryTermChange()
    {
      this.OnPropertyChanged("HasTheoryFunction");
    }





    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name.
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


  }
}