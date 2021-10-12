// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExponentialSmoothFilter.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   The exponential smooth filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Filter.Interpolation
{
  using System.Windows.Controls;
  using VianaNET.Data.Collections;
  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   The exponential smooth filter.
  /// </summary>
  public class ExponentialSmoothFilter : InterpolationFilter
  {


    /// <summary>
    ///   Initializes a new instance of the <see cref="ExponentialSmoothFilter" /> class.
    /// </summary>
    public ExponentialSmoothFilter()
    {
      this.InterpolationFilterType = InterpolationFilterTypes.ExponentialSmooth;
      this.NumberOfSamplesToInterpolate = 2;
      this.SmoothingFactor = 0.7f;
    }





    /// <summary>
    /// Gets the ExponentialSmoothControl.
    /// </summary>
    public override UserControl CustomUserControl => new ExponentialSmoothControl(this);

    /// <summary>
    ///   Gets or sets the smoothing factor.
    /// </summary>
    public float SmoothingFactor { get; set; }





    /// <summary>
    /// Calculate exponential smoothed values for the current data series.
    /// </summary>
    public override void CalculateFilterValues()
    {
      base.CalculateFilterValues();

      SortedObservableCollection<XYSample> fittedSamples = new SortedObservableCollection<XYSample>();

      for (int i = 1; i < this.WertY.Count; i++)
      {
        double smoothValue = this.SmoothingFactor * this.WertY[i] + (1 - this.SmoothingFactor) * this.WertY[i - 1];
        fittedSamples.Add(new XYSample(this.WertX[i], smoothValue));
      }

      App.Project.CurrentFilterData.InterpolationSeries = fittedSamples;
    }


  }
}