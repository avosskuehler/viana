// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FittedData.cs" company="Freie Universität Berlin">
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
namespace VianaNET.Data.Linefit
{
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Media;

  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   The video data.
  /// </summary>
  public class FittedData : DependencyObject, INotifyPropertyChanged
  {
    #region Static Fields

    /// <summary>
    /// The data line thickness property.
    /// </summary>
    public static readonly DependencyProperty DataLineThicknessProperty = DependencyProperty.Register(
      "DataLineThickness", typeof(double), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The data line color property.
    /// </summary>
    public static readonly DependencyProperty DataLineColorProperty = DependencyProperty.Register(
      "DataLineColor", typeof(SolidColorBrush), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Regression line thickness property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineThicknessProperty = DependencyProperty.Register(
      "RegressionLineThickness", typeof(double), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Regression line color property.
    /// </summary>
    public static readonly DependencyProperty RegressionLineColorProperty = DependencyProperty.Register(
      "RegressionLineColor", typeof(SolidColorBrush), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Interpolation line thickness property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineThicknessProperty = DependencyProperty.Register(
      "InterpolationLineThickness", typeof(double), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Interpolation line color property.
    /// </summary>
    public static readonly DependencyProperty InterpolationLineColorProperty = DependencyProperty.Register(
      "InterpolationLineColor", typeof(SolidColorBrush), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Theory line thickness property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineThicknessProperty = DependencyProperty.Register(
      "TheoryLineThickness", typeof(double), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The Theory line color property.
    /// </summary>
    public static readonly DependencyProperty TheoryLineColorProperty = DependencyProperty.Register(
      "TheoryLineColor", typeof(SolidColorBrush), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The LineFitClass property.
    /// </summary>
    public static readonly DependencyProperty LineFitClassProperty = DependencyProperty.Register(
      "LineFitClassType", typeof(LineFit), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The RegressionType property.
    /// </summary>
    public static readonly DependencyProperty RegressionTypeProperty = DependencyProperty.Register(
      "RegressionType", typeof(Regression), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The TheoreticalFunction property.
    /// </summary>
    public static readonly DependencyProperty TheoreticalFunctionProperty = DependencyProperty.Register(
      "TheoreticalFunction", typeof(FunctionCalcTree), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The IsInterpolationAllowed property.
    /// </summary>
    public static readonly DependencyProperty IsInterpolationAllowedProperty = DependencyProperty.Register(
      "IsInterpolationAllowed", typeof(bool), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The NumericPrecision property.
    /// </summary>
    public static readonly DependencyProperty NumericPrecisionProperty = DependencyProperty.Register(
      "NumericPrecision", typeof(int), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The IsShowingDataSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingDataSeriesProperty = DependencyProperty.Register(
      "IsShowingDataSeries", typeof(bool), typeof(FittedData), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// The IsShowingInterpolationSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingInterpolationSeriesProperty = DependencyProperty.Register(
      "IsShowingInterpolationSeries", typeof(bool), typeof(FittedData), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The IsShowingRegressionSeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingRegressionSeriesProperty = DependencyProperty.Register(
      "IsShowingRegressionSeries", typeof(bool), typeof(FittedData), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The IsShowingTheorySeries property.
    /// </summary>
    public static readonly DependencyProperty IsShowingTheorySeriesProperty = DependencyProperty.Register(
      "IsShowingTheorySeries", typeof(bool), typeof(FittedData), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The InterpolationSeries property.
    /// </summary>
    public static readonly DependencyProperty InterpolationSeriesProperty = DependencyProperty.Register(
      "InterpolationSeries", typeof(SortedObservableCollection<XYSample>), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The RegressionSeries property.
    /// </summary>
    public static readonly DependencyProperty RegressionSeriesProperty = DependencyProperty.Register(
      "RegressionSeries", typeof(SortedObservableCollection<XYSample>), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The TheorySeries property.
    /// </summary>
    public static readonly DependencyProperty TheorySeriesProperty = DependencyProperty.Register(
      "TheorySeries", typeof(SortedObservableCollection<XYSample>), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The AxisX property.
    /// </summary>
    public static readonly DependencyProperty AxisXProperty = DependencyProperty.Register(
      "AxisX", typeof(DataAxis), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// The AxisY property.
    /// </summary>
    public static readonly DependencyProperty AxisYProperty = DependencyProperty.Register(
      "AxisY", typeof(DataAxis), typeof(FittedData), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the Ausgabestring für die Ausgleichsfunktion
    /// </summary>
    public static readonly DependencyProperty RegressionFunctionStringProperty = DependencyProperty.Register(
      "RegressionFunctionString", typeof(string), typeof(FittedData), new UIPropertyMetadata(null));

      
    /// <summary>
    /// Gets the Ausgabestring für die Abweichung der Ausgleichsfunktion
    /// </summary>
    public static readonly DependencyProperty RegressionAberrationStringProperty = DependencyProperty.Register(
      "RegressionAberrationString", typeof(string), typeof(FittedData), new UIPropertyMetadata(null));


    /// <summary>
    ///   The instance.
    /// </summary>
    private static FittedData instance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Prevents a default instance of the <see cref="FittedData"/> class from being created. 
    /// </summary>
    private FittedData()
    {
      this.NumericPrecision = 2;
      this.IsInterpolationAllowed = true;
      this.RegressionType = Regression.Linear;
      this.InterpolationSeries = new SortedObservableCollection<XYSample>();
      this.RegressionSeries = new SortedObservableCollection<XYSample>();
      this.TheorySeries = new SortedObservableCollection<XYSample>();
      this.DataLineColor = Brushes.Blue;
      this.InterpolationLineColor = Brushes.Brown;
      this.RegressionLineColor = Brushes.Red;
      this.TheoryLineColor = Brushes.GreenYellow;
      this.DataLineThickness = 2;
      this.InterpolationLineThickness = 2;
      this.RegressionLineThickness = 2;
      this.TheoryLineThickness = 2;
      this.LineFitObject = new LineFit();
      this.RegressionFunctionString = "---";
      this.RegressionAberrationString = " ";
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
    ///   Gets the <see cref="FittedData" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static FittedData Instance
    {
      get
      {
        return instance ?? (instance = new FittedData());
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
    /// Gets or sets the LineFitClassType
    /// </summary>
    public LineFit LineFitObject
    {
      get
      {
        return (LineFit)this.GetValue(LineFitClassProperty);
      }

      set
      {
        this.SetValue(LineFitClassProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the Theory line color.
    /// </summary>
    public Regression RegressionType
    {
      get
      {
        return (Regression)this.GetValue(RegressionTypeProperty);
      }

      set
      {
        this.SetValue(RegressionTypeProperty, value);
        if (this.LineFitObject != null && this.LineFitObject.WertX.Count > 0)
        {
          // sind Datenreihen ausgewählt ?
          // neu berechnen !
          this.LineFitObject.CalculateLineFitFunction(value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the Theory line color.
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
    /// Gets or sets a value indicating whether is interpolation allowed.
    /// </summary>
    public bool IsInterpolationAllowed
    {
      get
      {
        return (bool)this.GetValue(IsInterpolationAllowedProperty);
      }

      set
      {
        this.SetValue(IsInterpolationAllowedProperty, value);
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
        if (value)
        {
          this.CalculateRegressionSeriesDataPoints();
        }
        else
        {
          this.RegressionSeries.Clear();
        }

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
        if (value)
        {
          this.CalculateTheorySeriesDataPoints();
        }
        else
        {
          this.TheorySeries.Clear();
        }

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
        if (this.LineFitObject != null)
        {
          this.LineFitObject.UpdateRegressionFunctionString();
        }
      }
    }

    /// <summary>
    /// Gets or sets the InterpolationSeries.
    /// </summary>
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
    public SortedObservableCollection<XYSample> RegressionSeries
    {
      get
      {
        return (SortedObservableCollection<XYSample>)this.GetValue(RegressionSeriesProperty);
      }

      set
      {
        this.SetValue(RegressionSeriesProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the TheorySeries.
    /// </summary>
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
          if ((DataAxis)this.GetValue(AxisXProperty) != value)
          {       
              IsShowingRegressionSeries = false;
              RegressionFunctionString = string.Empty;
              RegressionAberrationString = string.Empty;
              RegressionSeries.Clear();
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

    public string RegressionFunctionString
    {
      get
      {
        return (string)this.GetValue(RegressionFunctionStringProperty);
      }

      set
      {
        this.SetValue(RegressionFunctionStringProperty, value);
        this.OnPropertyChanged("RegressionFunctionString");
      }
    }

    public string RegressionAberrationString
    {
        get
        {
            return (string)this.GetValue(RegressionAberrationStringProperty);
        }

        set
        {
            this.SetValue(RegressionAberrationStringProperty, value);
            this.OnPropertyChanged("RegressionAberrationString");
        }
    }

    /// <summary>
    /// Gets the numeric precision string.
    /// </summary>
    public string NumericPrecisionString
    {
      get
      {
        return "G" + this.NumericPrecision.ToString(CultureInfo.InvariantCulture);
      }
    }


    #endregion

    #region Public Methods and Operators
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

    /// <summary>
    ///   The make preparations for line fit.
    /// </summary>
    private void PopulateSampleArrayFromVideoData()
    {
      if (this.LineFitObject == null)
      {
        this.LineFitObject = new LineFit();
      }

      // übernehme die Daten in die Arrays wertX und wertY
      this.LineFitObject.ExtractDataColumnsFromVideoSamples(
         VideoData.Instance.Samples,
         VideoData.Instance.ActiveObject,
         this.AxisX,
         this.AxisY);
    }

    /// <summary>
    /// Calculates regression series data points.
    /// </summary>
    private void CalculateRegressionSeriesDataPoints()
    {
      if (this.LineFitObject == null)
      {
        return;
      }

      if (this.IsShowingRegressionSeries)
      {
        this.PopulateSampleArrayFromVideoData();
        this.LineFitObject.CalculateLineFitFunction(this.RegressionType);
      }
      else
      {
        this.RegressionSeries.Clear();
      }
    }

    /// <summary>
    /// Calculates theory series data points.
    /// </summary>
    private void CalculateTheorySeriesDataPoints()
    {
      if (this.TheoreticalFunction == null)
      {
        return;
      }

      if (this.LineFitObject == null)
      {
        this.LineFitObject = new LineFit();
      }

      if (this.IsShowingTheorySeries)
      {
        this.PopulateSampleArrayFromVideoData();
        this.LineFitObject.CalculateLineFitTheorieSeries(this.TheoreticalFunction, this.TheorySeries);
      }
      else
      {
        this.TheorySeries.Clear();
      }
    }

    #endregion
  }
}