// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterBase.cs" company="Freie Universität Berlin">
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
//   The interpolation base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter
{
  using System.Collections.Generic;
  using System.Windows;

  using VianaNET.CustomStyles.Types;
  using VianaNET.Data.Collections;

  /// <summary>
  ///   The interpolation base.
  /// </summary>
  public abstract class FilterBase : DependencyObject
  {
    #region Static Fields

    /// <summary>
    ///   The number of samples to interpolate property.
    /// </summary>
    public static readonly DependencyProperty NumberOfSamplesToInterpolateProperty =
      DependencyProperty.Register(
        "NumberOfSamplesToInterpolate",
        typeof(int),
        typeof(FilterBase),
        new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    /// <summary>
    /// Anzahl der Wertepaare, die für die Berechnungen der Ausgleichsfunktion benutzt werden
    /// </summary>
    protected int anzahl;

    /// <summary>
    ///   minimaler pixel wert im chart für x.
    /// </summary>
    protected double startPixelX;

    /// <summary>
    ///   maximaler pixel wert im chart für x.
    /// </summary>
    protected double endPixelX;

    /// <summary>
    ///   minimaler Wert auf der x-Achse, der bei Messdaten auftritt.
    /// </summary>
    protected double startX;

    /// <summary>
    ///   The step x.
    /// </summary>
    protected double stepX;

    /// <summary>
    ///   maximaler Wert auf der x-Achse, der bei Messdaten auftritt
    /// </summary>
    protected double endX;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterBase"/> class.
    /// </summary>
    protected FilterBase()
    {
      this.WertX = new List<double>();
      this.WertY = new List<double>();
    }

    #region Public Properties

    /// <summary>
    /// Gets or sets aus den Videodaten herausgelesene Messpaare - 
    /// auf zwei Arrays aufgeteilt, Grunddaten der Berechnung der Ausgleichsfunktion 
    /// </summary>
    public List<double> WertX { get; set; }

    /// <summary>
    /// Gets or sets aus den Videodaten herausgelesene Messpaare - 
    /// auf zwei Arrays aufgeteilt, Grunddaten der Berechnung der Ausgleichsfunktion 
    /// </summary>
    public List<double> WertY { get; set; }

    /// <summary>
    ///   Gets or sets the number of samples to interpolate.
    /// </summary>
    public int NumberOfSamplesToInterpolate
    {
      get
      {
        return (int)this.GetValue(NumberOfSamplesToInterpolateProperty);
      }

      set
      {
        this.SetValue(NumberOfSamplesToInterpolateProperty, value);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The calculate interpolated values.
    /// </summary>
    /// <param name="originalSamples">
    /// The original data samples to be fitted
    /// </param>
    /// <param name="fittedSamples">The collection to be populated with the fitted samples</param>
    public virtual void CalculateFilterValues(DataCollection originalSamples, SortedObservableCollection<XYSample> fittedSamples)
    {
      //if (this.WertX.Count == 0)
      {
        this.CopySampleColumnsToArrays(VideoData.Instance.ActiveObject, originalSamples);
      }
    }

    /// <summary>
    /// This method returns a list of samples beginning at the given
    /// start index and with the length of the given numberOfSamplesToReturn
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="numberOfSamplesToReturn">The number of samples to return.</param>
    /// <returns>
    /// The <see>
    ///       <cref>List{double}</cref>
    ///     </see> with the samples.
    /// </returns>
    protected List<double> GetRangeAtPosition(int startIndex, int numberOfSamplesToReturn)
    {
      var sampleCollection = new List<double>(numberOfSamplesToReturn);

      for (int i = startIndex; i < startIndex + numberOfSamplesToReturn; i++)
      {
        if (sampleCollection.Count < numberOfSamplesToReturn)
        {
          sampleCollection.Add(this.WertY[i]);
        }
        else
        {
          break;
        }
      }

      return sampleCollection;
    }

    #endregion

    /// <summary>
    /// The copy sample columns to arrays.
    /// </summary>
    /// <param name="aktObjectNr">
    /// The akt object nr. 
    /// </param>
    /// <param name="originalSamples">
    /// The original Samples.
    /// </param>
    private void CopySampleColumnsToArrays(int aktObjectNr, DataCollection originalSamples)
    {
      if (this.WertX == null)
      {
        return;
      }

      this.WertX.Clear();
      this.WertY.Clear();
      this.anzahl = 0;

      foreach (TimeSample sample in originalSamples)
      {
        var valueX = this.GetValueFromSample(true, aktObjectNr, sample);
        var valueY = this.GetValueFromSample(false, aktObjectNr, sample);

        if (valueX.HasValue && valueY.HasValue)
        {
          if (this.anzahl == 0)
          {
            this.startPixelX = sample.Object[aktObjectNr].PixelX;
          }

          this.WertX.Add(valueX.Value);
          this.WertY.Add(valueY.Value);
          this.endPixelX = sample.Object[aktObjectNr].PixelX;
          this.anzahl++;
        }
      }

      if (this.anzahl > 0)
      {
        this.startX = this.WertX[0];
        this.endX = this.WertX[this.anzahl - 1];
      }
      else
      {
        this.startX = 0;
        this.endX = 0;
      }

      if (this.startPixelX > this.endPixelX)
      {
        this.startX = this.WertX[this.anzahl - 1];
        this.endX = this.WertX[0];
        double hilf = this.startPixelX;
        this.startPixelX = this.endPixelX;
        this.endPixelX = hilf;
      }

      // Schrittweite zum Ausfüllen der Zwischenräume ( in x-Achsen-Richtung )
      this.stepX = (this.endX - this.startX) / (this.endPixelX - this.startPixelX);
    }

    /// <summary>
    /// Returns the value from the given timesample that corresponds
    /// to the given object and axis request.
    /// </summary>
    /// <param name="isXValue">True if should use xaxis, else uses yaxis</param>
    /// <param name="aktObjectNr">The index of the used object</param>
    /// <param name="sample">The time sample with the raw data</param>
    /// <returns>The nullable <see cref="double"/> with the value, if there is one found.</returns>
    private double? GetValueFromSample(bool isXValue, int aktObjectNr, TimeSample sample)
    {
      double? value = null;

      switch (isXValue ? FilterData.Instance.AxisX.Axis : FilterData.Instance.AxisY.Axis)
      {
        case AxisType.T:
          value = sample.Timestamp;
          break;
        case AxisType.X:
          value = sample.Object[aktObjectNr].PixelX;
          break;
        case AxisType.Y:
          value = sample.Object[aktObjectNr].PixelY;
          break;
        case AxisType.PX:
          value = sample.Object[aktObjectNr].PositionX;
          break;
        case AxisType.PY:
          value = sample.Object[aktObjectNr].PositionY;
          break;
        case AxisType.D:
          value = sample.Object[aktObjectNr].Distance;
          break;
        case AxisType.DX:
          value = sample.Object[aktObjectNr].DistanceX;
          break;
        case AxisType.DY:
          value = sample.Object[aktObjectNr].DistanceY;
          break;
        case AxisType.S:
          value = sample.Object[aktObjectNr].Length;
          break;
        case AxisType.SX:
          value = sample.Object[aktObjectNr].LengthX;
          break;
        case AxisType.SY:
          value = sample.Object[aktObjectNr].LengthY;
          break;
        case AxisType.V:
          value = sample.Object[aktObjectNr].Velocity;
          break;
        case AxisType.VX:
          value = sample.Object[aktObjectNr].VelocityX;
          break;
        case AxisType.VY:
          value = sample.Object[aktObjectNr].VelocityY;
          break;
        case AxisType.A:
          value = sample.Object[aktObjectNr].Acceleration;
          break;
        case AxisType.AX:
          value = sample.Object[aktObjectNr].AccelerationX;
          break;
        case AxisType.AY:
          value = sample.Object[aktObjectNr].AccelerationY;
          break;
      }

      return value;
    }
  }
}