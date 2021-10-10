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
  using System.Xml.Serialization;
  using VianaNET.Data.Collections;
  using VianaNET.CustomStyles.Types;

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
    [XmlIgnore]
    public List<double> WertX { get; set; }

    /// <summary>
    /// Gets or sets the minimal value of the WertX Array
    /// </summary>
    [XmlIgnore]
    public double WertXMin { get; set; }

    /// <summary>
    /// Gets or sets the maximal value of the WertX Array
    /// </summary>
    [XmlIgnore]
    public double WertXMax { get; set; }

    /// <summary>
    /// Gets or sets aus den Videodaten herausgelesene Messpaare - 
    /// auf zwei Arrays aufgeteilt, Grunddaten der Berechnung der Ausgleichsfunktion 
    /// </summary>
    [XmlIgnore]
    public List<double> WertY { get; set; }

    /// <summary>
    ///   Gets or sets the number of samples to interpolate.
    /// </summary>
    public int NumberOfSamplesToInterpolate
    {
      get => (int)this.GetValue(NumberOfSamplesToInterpolateProperty);

      set => this.SetValue(NumberOfSamplesToInterpolateProperty, value);
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// This virtual method should be overridden from
    /// a filter to calculate filtered values.
    /// </summary>
    public virtual void CalculateFilterValues()
    {
      this.CopySampleColumnsToArrays();
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
      List<double> sampleCollection = new List<double>(numberOfSamplesToReturn);

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
    /// Copies data sample columns to WertX and WertY arrays.
    /// Both arrays are used to determine the filter values.
    /// </summary>
    private void CopySampleColumnsToArrays()
    {
      if (this.WertX == null)
      {
        return;
      }

      int aktObjectNr = App.Project.ProcessingData.IndexOfObject;
      this.WertX.Clear();
      this.WertY.Clear();
      this.WertXMin = double.MaxValue;
      this.WertXMax = double.MinValue;
      this.anzahl = 0;

      foreach (TimeSample sample in App.Project.VideoData.FilteredSamples)
      {
        if (!sample.IsSelected)
        {
          continue;
        }

        double? valueX = this.GetValueFromSample(true, aktObjectNr, sample);
        double? valueY = this.GetValueFromSample(false, aktObjectNr, sample);

        if (valueX.HasValue && valueY.HasValue)
        {
          if (this.anzahl == 0)
          {
            this.startPixelX = sample.Object[aktObjectNr].PixelX;
          }

          this.WertX.Add(valueX.Value);
          this.WertY.Add(valueY.Value);
          if (valueX.Value > this.WertXMax)
          {
            this.WertXMax = valueX.Value;
          }

          if (valueX.Value < this.WertXMin)
          {
            this.WertXMin = valueX.Value;
          }

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
        return;
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
      if (sample.Object[aktObjectNr] == null)
      {
        return null;
      }

      switch (isXValue ? App.Project.CurrentFilterData.AxisX.Axis : App.Project.CurrentFilterData.AxisY.Axis)
      {
        case AxisType.I:
          value = sample.Framenumber;
          break;
        case AxisType.T:
          value = sample.Object[aktObjectNr].Time;
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