// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MovingAverageFilter.cs" company="Freie Universität Berlin">
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
//   The moving average filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Filter.Interpolation
{
  using System.Linq;
  using Application;
  using Collections;
  using CustomStyles.Types;
  
  /// <summary>
  ///   The moving average filter.
  /// </summary>
  public class MovingAverageFilter : InterpolationFilter
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MovingAverageFilter" /> class.
    /// </summary>
    public MovingAverageFilter()
    {
      this.InterpolationFilterType = InterpolationFilterTypes.MovingAverage;
      this.NumberOfSamplesToInterpolate = 3;
      this.CustomUserControl = new MovingAverageUserControl(this);
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Calculate moving average values for the current data series.
    /// </summary>
    public override void CalculateFilterValues()
    {
      base.CalculateFilterValues();

      var fittedSamples = new SortedObservableCollection<XYSample>();
      var startIndex = (int)(this.NumberOfSamplesToInterpolate / 2f);

      // Calculate interpolation
      for (int i = startIndex; i < this.WertX.Count - startIndex; i++)
      {
        var samplesForInterpolation = this.GetRangeAtPosition(i - startIndex, this.NumberOfSamplesToInterpolate);
        fittedSamples.Add(new XYSample(this.WertX[i], samplesForInterpolation.Sum() / this.NumberOfSamplesToInterpolate));
      }

      Viana.Project.CurrentFilterData.InterpolationSeries = fittedSamples;
    }

    #endregion
  }
}