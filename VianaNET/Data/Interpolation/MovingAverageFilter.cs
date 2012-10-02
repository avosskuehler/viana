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
namespace VianaNET.Data.Interpolation
{
  using System.Collections.Generic;

  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The moving average filter.
  /// </summary>
  public class MovingAverageFilter : InterpolationBase
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MovingAverageFilter" /> class.
    /// </summary>
    public MovingAverageFilter()
    {
      this.FilterType = Interpolation.FilterTypes.MovingAverage;
      this.NumberOfSamplesToInterpolate = 3;
      this.CustomUserControl = new MovingAverageUserControl(this);
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The calculate interpolated values.
    /// </summary>
    /// <param name="samples">
    /// The samples. 
    /// </param>
    public override void CalculateInterpolatedValues(DataCollection samples)
    {
      var validSamples = new int[Video.Instance.ImageProcessing.NumberOfTrackedObjects];

      var startIndex = (int)(this.NumberOfSamplesToInterpolate / 2f);

      // int end = Math.Min(startIndex, samples.Count);

      // Reset all interpolated values
      foreach (TimeSample t in samples)
      {
        for (int j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
        {
          DataSample currentSample = t.Object[j];
          if (currentSample == null)
          {
            continue;
          }

          currentSample.VelocityI = null;
          currentSample.VelocityXI = null;
          currentSample.VelocityYI = null;
          currentSample.AccelerationI = null;
          currentSample.AccelerationXI = null;
          currentSample.AccelerationYI = null;
        }
      }

      // Calculate interpolation
      for (int i = startIndex; i < samples.Count - startIndex; i++)
      {
        for (int j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
        {
          DataSample currentSample = samples[i].Object[j];

          if (currentSample == null)
          {
            continue;
          }

          List<DataSample>[] samplesForInterpolation = samples.GetRangeAtPosition(
            i - startIndex, this.NumberOfSamplesToInterpolate, j);

          double velocityInterpolated = 0d;
          double velocityXInterpolated = 0d;
          double velocityYInterpolated = 0d;

          foreach (DataSample item in samplesForInterpolation[0])
          {
            velocityInterpolated += item.Velocity.Value;
            velocityXInterpolated += item.VelocityX.Value;
            velocityYInterpolated += item.VelocityY.Value;
          }

          currentSample.VelocityI = velocityInterpolated / samplesForInterpolation[0].Count;
          currentSample.VelocityXI = velocityXInterpolated / samplesForInterpolation[0].Count;
          currentSample.VelocityYI = velocityYInterpolated / samplesForInterpolation[0].Count;

          double accelerationInterpolated = 0d;
          double accelerationXInterpolated = 0d;
          double accelerationYInterpolated = 0d;

          foreach (DataSample item in samplesForInterpolation[1])
          {
            accelerationInterpolated += item.Acceleration.Value;
            accelerationXInterpolated += item.AccelerationX.Value;
            accelerationYInterpolated += item.AccelerationY.Value;
          }

          currentSample.AccelerationI = accelerationInterpolated / samplesForInterpolation[1].Count;
          currentSample.AccelerationXI = accelerationXInterpolated / samplesForInterpolation[1].Count;
          currentSample.AccelerationYI = accelerationYInterpolated / samplesForInterpolation[1].Count;
        }
      }
    }

    #endregion
  }
}