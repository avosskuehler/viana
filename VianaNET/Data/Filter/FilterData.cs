// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterData.cs" company="Freie Universität Berlin">
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
//   The video data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Filter
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Media;
  using System.Xml.Serialization;
  using Collections;
  using CustomStyles.Types;
  using Interpolation;
  using Regression;
  using Theory;
  using WPFMath;

  /// <summary>
  ///   The video data.
  /// </summary>
  [Serializable]
  public class FilterData : DependencyObject, INotifyPropertyChanged
  {
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="CurrentFilter" />.
    /// </summary>
    public static readonly DependencyProperty CurrentFilterProperty = DependencyProperty.Register(
      "CurrentFilter", typeof(FilterBase), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The data line thickness property.
    /// </summary>
    public static readonly DependencyProperty DataLineThicknessProperty = DependencyProperty.Register(
      "DataLineThickness", typeof(double), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The data line color property.
    /// </summary>
    public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register(
      "SelectionColor", typeof(SolidColorBrush), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The data line color property.
    /// </summary>
    public static readonly DependencyProperty DataLineColorProperty = DependencyProperty.Register(
      "DataLineColor", typeof(SolidColorBrush), typeof(FilterData), new UIPropertyMetadata(null));
    
    /// <summary>
    /// The Regression line thickness property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineThicknessProperty = DependencyProperty.Register(
      "RegressionLineThickness", typeof(double), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Regression line color property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineColorProperty = DependencyProperty.Register(
      "RegressionLineColor", typeof(SolidColorBrush), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Interpolation line thickness property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineThicknessProperty = DependencyProperty.Register(
      "InterpolationLineThickness", typeof(double), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Interpolation line color property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineColorProperty = DependencyProperty.Register(
      "InterpolationLineColor", typeof(SolidColorBrush), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Theory line thickness property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineThicknessProperty = DependencyProperty.Register(
      "TheoryLineThickness", typeof(double), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Theory line color property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineColorProperty = DependencyProperty.Register(
      "TheoryLineColor", typeof(SolidColorBrush), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The RegressionObject property.
    /// </summary>
    public static readonly DependencyProperty RegressionObjectProperty = DependencyProperty.Register(
      "RegressionObject", typeof(RegressionFilter), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The InterpolationFilter property.
    /// </summary>
    public static readonly DependencyProperty InterpolationFilterProperty = DependencyProperty.Register(
      "InterpolationFilter", typeof(InterpolationFilter), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The InterpolationObject property.
    /// </summary>
    public static readonly DependencyProperty TheoryObjectProperty = DependencyProperty.Register(
      "TheoryObjectProperty", typeof(TheoryFilter), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The TheoreticalFunction property.
    /// </summary>
    public static readonly DependencyProperty TheoreticalFunctionProperty = DependencyProperty.Register(
      "TheoreticalFunction", typeof(FunctionCalcTree), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The NumericPrecision property.
    /// </summary>
    public static readonly DependencyProperty NumericPrecisionProperty = DependencyProperty.Register(
      "NumericPrecision", typeof(int), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The IsShowingDataSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingDataSeriesProperty = DependencyProperty.Register(
      "IsShowingDataSeries", typeof(bool), typeof(FilterData), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// The IsShowingInterpolationSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingInterpolationSeriesProperty = DependencyProperty.Register(
      "IsShowingInterpolationSeries", typeof(bool), typeof(FilterData), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The IsShowingRegressionSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingRegressionSeriesProperty = DependencyProperty.Register(
      "IsShowingRegressionSeries", typeof(bool), typeof(FilterData), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The IsShowingTheorySeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingTheorySeriesProperty = DependencyProperty.Register(
      "IsShowingTheorySeries", typeof(bool), typeof(FilterData), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The InterpolationSeries property.
    /// </summary>
    public static readonly DependencyProperty InterpolationSeriesProperty = DependencyProperty.Register(
      "InterpolationSeries", typeof(SortedObservableCollection<XYSample>), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The RegressionSeries property.
    /// </summary>
    public static readonly DependencyProperty RegressionSeriesProperty = DependencyProperty.Register(
      "RegressionSeries", typeof(SortedObservableCollection<XYSample>), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The TheorySeries property.
    /// </summary>
    public static readonly DependencyProperty TheorySeriesProperty = DependencyProperty.Register(
      "TheorySeries", typeof(SortedObservableCollection<XYSample>), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The AxisX property.
    /// </summary>
    public static readonly DependencyProperty AxisXProperty = DependencyProperty.Register(
      "AxisX", typeof(DataAxis), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// The AxisY property.
    /// </summary>
    public static readonly DependencyProperty AxisYProperty = DependencyProperty.Register(
      "AxisY", typeof(DataAxis), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the function termin für die Ausgleichsfunktion
    /// </summary>
    public static readonly DependencyProperty RegressionFunctionTexFormulaProperty = DependencyProperty.Register(
      "RegressionFunctionTexFormula", typeof(TexFormula), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the function term for the theory function
    /// </summary>
    public static readonly DependencyProperty TheoryFunctionTexFormulaProperty = DependencyProperty.Register(
      "TheoryFunctionTexFormula", typeof(TexFormula), typeof(FilterData), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the Abweichung der Ausgleichsfunktion
    /// </summary>
    public static readonly DependencyProperty RegressionAberrationProperty = DependencyProperty.Register(
      "RegressionAberration", typeof(double), typeof(FilterData), new UIPropertyMetadata(null));

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterData"/> class. 
    /// </summary>
    public FilterData()
    {
      this.NumericPrecision = 2;
      this.InterpolationSeries = new SortedObservableCollection<XYSample>();
      this.RegressionSeries = new SortedObservableCollection<XYSample>();
      this.TheorySeries = new SortedObservableCollection<XYSample>();
      this.SelectionColor = Brushes.Blue;
      this.DataLineColor = Brushes.LightBlue;
      this.InterpolationLineColor = Brushes.Brown;
      this.RegressionLineColor = Brushes.Red;
      this.TheoryLineColor = Brushes.GreenYellow;
      this.DataLineThickness = 2;
      this.InterpolationLineThickness = 2;
      this.RegressionLineThickness = 2;
      this.TheoryLineThickness = 2;
      this.RegressionFilter = new RegressionFilter();
      this.RegressionFunctionTexFormula = null;
      this.RegressionAberration = 0d;
      this.InterpolationFilter = InterpolationFilter.Filter[InterpolationFilterTypes.MovingAverage];
      this.IsShowingRegressionSeries = false;
      this.IsShowingTheorySeries = false;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets a the interpolation filter to use for
    ///   smoothing data.
    /// </summary>
    public FilterBase CurrentFilter
    {
      get
      {
        return (FilterBase)this.GetValue(CurrentFilterProperty);
      }

      set
      {
        this.SetValue(CurrentFilterProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the data line thickness.
    /// </summary>
    public double DataLineThickness
    {
      get
      {
        return (double)this.GetValue(DataLineThicknessProperty);
      }

      set
      {
        this.SetValue(DataLineThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the data line color.
    /// </summary>
    public SolidColorBrush DataLineColor
    {
      get
      {
        return (SolidColorBrush)this.GetValue(DataLineColorProperty);
      }

      set
      {
        this.SetValue(DataLineColorProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the selection color.
    /// </summary>
    public SolidColorBrush SelectionColor
    {
      get
      {
        return (SolidColorBrush)this.GetValue(SelectionColorProperty);
      }

      set
      {
        this.SetValue(SelectionColorProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Regression line thickness.
    /// </summary>
    public double RegressionLineThickness
    {
      get
      {
        return (double)this.GetValue(RegressionLineThicknessProperty);
      }

      set
      {
        this.SetValue(RegressionLineThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Regression line color.
    /// </summary>
    public SolidColorBrush RegressionLineColor
    {
      get
      {
        return (SolidColorBrush)this.GetValue(RegressionLineColorProperty);
      }

      set
      {
        this.SetValue(RegressionLineColorProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Interpolation line  thickness.
    /// </summary>
    public double InterpolationLineThickness
    {
      get
      {
        return (double)this.GetValue(InterpolationLineThicknessProperty);
      }

      set
      {
        this.SetValue(InterpolationLineThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Interpolation line color.
    /// </summary>
    public SolidColorBrush InterpolationLineColor
    {
      get
      {
        return (SolidColorBrush)this.GetValue(InterpolationLineColorProperty);
      }

      set
      {
        this.SetValue(InterpolationLineColorProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Theory line thickness.
    /// </summary>
    public double TheoryLineThickness
    {
      get
      {
        return (double)this.GetValue(TheoryLineThicknessProperty);
      }

      set
      {
        this.SetValue(TheoryLineThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Theory line color.
    /// </summary>
    public SolidColorBrush TheoryLineColor
    {
      get
      {
        return (SolidColorBrush)this.GetValue(TheoryLineColorProperty);
      }

      set
      {
        this.SetValue(TheoryLineColorProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the RegressionObject
    /// </summary>
    public RegressionFilter RegressionFilter
    {
      get
      {
        return (RegressionFilter)this.GetValue(RegressionObjectProperty);
      }

      set
      {
        this.SetValue(RegressionObjectProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the InterpolationFilter
    /// </summary>
    public InterpolationFilter InterpolationFilter
    {
      get
      {
        return (InterpolationFilter)this.GetValue(InterpolationFilterProperty);
      }

      set
      {
        this.SetValue(InterpolationFilterProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the TheoryObject
    /// </summary>
    public TheoryFilter TheoryFilter
    {
      get
      {
        return (TheoryFilter)this.GetValue(TheoryObjectProperty);
      }

      set
      {
        this.SetValue(TheoryObjectProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the theoretical function.
    /// </summary>
    public FunctionCalcTree TheoreticalFunction
    {
      get
      {
        return (FunctionCalcTree)this.GetValue(TheoreticalFunctionProperty);
      }

      set
      {
        this.SetValue(TheoreticalFunctionProperty, value);
        if (this.IsShowingTheorySeries)
        {
          this.CalculateTheorySeriesDataPoints();
        }
        else
        {
          this.TheorySeries.Clear();
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chart should display the data series.
    /// </summary>
    public bool IsShowingDataSeries
    {
      get
      {
        return (bool)this.GetValue(IsShowingDataSeriesProperty);
      }

      set
      {
        this.SetValue(IsShowingDataSeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chart should display the interpolation series.
    /// </summary>
    public bool IsShowingInterpolationSeries
    {
      get
      {
        return (bool)this.GetValue(IsShowingInterpolationSeriesProperty);
      }

      set
      {
        this.SetValue(IsShowingInterpolationSeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chart should display the regression series.
    /// </summary>
    public bool IsShowingRegressionSeries
    {
      get
      {
        return (bool)this.GetValue(IsShowingRegressionSeriesProperty);
      }

      set
      {
        this.SetValue(IsShowingRegressionSeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chart should display the theory series.
    /// </summary>
    public bool IsShowingTheorySeries
    {
      get
      {
        return (bool)this.GetValue(IsShowingTheorySeriesProperty);
      }

      set
      {
        this.SetValue(IsShowingTheorySeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the NumericPrecision.
    /// </summary>
    public int NumericPrecision
    {
      get
      {
        return (int)this.GetValue(NumericPrecisionProperty);
      }

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
    /// Gets or sets the InterpolationSeries.
    /// </summary>
    [XmlIgnore]
    public SortedObservableCollection<XYSample> InterpolationSeries
    {
      get
      {
        return (SortedObservableCollection<XYSample>)this.GetValue(InterpolationSeriesProperty);
      }

      set
      {
        this.SetValue(InterpolationSeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the RegressionSeries.
    /// </summary>
    [XmlIgnore]
    public SortedObservableCollection<XYSample> RegressionSeries
    {
      get
      {
        return (SortedObservableCollection<XYSample>)this.GetValue(RegressionSeriesProperty);
      }

      set
      {
        this.SetValue(RegressionSeriesProperty, value);
        this.OnPropertyChanged("RegressionSeries");
      }
    }

    /// <summary>
    /// Gets or sets the TheorySeries.
    /// </summary>
    [XmlIgnore]
    public SortedObservableCollection<XYSample> TheorySeries
    {
      get
      {
        return (SortedObservableCollection<XYSample>)this.GetValue(TheorySeriesProperty);
      }

      set
      {
        this.SetValue(TheorySeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the x axis currently selected in the chart window.
    /// </summary>
    public DataAxis AxisX
    {
      get
      {
        return (DataAxis)this.GetValue(AxisXProperty);
      }

      set
      {
        if (this.GetValue(AxisXProperty) != value)
        {
          this.SetValue(AxisXProperty, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the y axis currently selected in the chart window.
    /// </summary>
    public DataAxis AxisY
    {
      get
      {
        return (DataAxis)this.GetValue(AxisYProperty);
      }

      set
      {
        this.SetValue(AxisYProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the regression function tex formula.
    /// </summary>
    public TexFormula RegressionFunctionTexFormula
    {
      get
      {
        return (TexFormula)this.GetValue(RegressionFunctionTexFormulaProperty);
      }

      set
      {
        this.SetValue(RegressionFunctionTexFormulaProperty, value);
        this.OnPropertyChanged("RegressionFunctionTexFormula");
      }
    }

    /// <summary>
    /// Gets or sets the Theory function tex formula.
    /// </summary>
    public TexFormula TheoryFunctionTexFormula
    {
      get
      {
        return (TexFormula)this.GetValue(TheoryFunctionTexFormulaProperty);
      }

      set
      {
        this.SetValue(TheoryFunctionTexFormulaProperty, value);
        this.OnPropertyChanged("TheoryFunctionTexFormula");
      }
    }

    /// <summary>
    /// Gets or sets the regression aberration.
    /// </summary>
    public double RegressionAberration
    {
      get
      {
        return (double)this.GetValue(RegressionAberrationProperty);
      }

      set
      {
        this.SetValue(RegressionAberrationProperty, value);
        if (this.IsShowingRegressionSeries)
        {
          this.OnPropertyChanged("RegressionAberration");
        }
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Returns a string that formatted the given value using
    /// the current numeric precision value.
    /// </summary>
    /// <param name="a">A <see cref="double"/> which has to be converted to a string.</param>
    /// <returns>The converted string, e.g. 0,003 or 300</returns>
    public string GetFormattedString(double a)
    {
      if (a > -1 && a < 1)
      {
        return a.ToString("G" + this.NumericPrecision.ToString(CultureInfo.InvariantCulture));
      }

      return a.ToString("N" + this.NumericPrecision.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Calculates Interpolation series data points.
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
    /// Calculates regression series data points.
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
        this.RegressionFilter.UpdateLinefitFunctionData(true);
      }
      else
      {
        this.RegressionSeries.Clear();
      }
    }

    /// <summary>
    /// Calculates theory series data points.
    /// </summary>
    public void CalculateTheorySeriesDataPoints()
    {
      if (this.TheoreticalFunction == null)
      {
        return;
      }

      if (this.TheoryFilter == null)
      {
        this.TheoryFilter = new TheoryFilter();
      }

      if (this.IsShowingTheorySeries)
      {
        this.TheoryFilter.CalculateFilterValues();
      }
      else
      {
        this.TheorySeries.Clear();
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name. 
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}