// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExponentialSmoothFilter.cs" company="Freie Universität Berlin">
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
//   The exponential smooth filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Interpolation
{
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The exponential smooth filter.
  /// </summary>
  public class ExponentialSmoothFilter : InterpolationBase
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ExponentialSmoothFilter" /> class.
    /// </summary>
    public ExponentialSmoothFilter()
    {
      this.FilterType = Interpolation.FilterTypes.ExponentialSmooth;
      this.NumberOfSamplesToInterpolate = 2;
      this.SmoothingFactor = 0.7f;
      this.CustomUserControl = new ExponentialSmoothControl(this);
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the smoothing factor.
    /// </summary>
    public float SmoothingFactor { get; set; }

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
      for (int z = 0; z < Video.Instance.ImageProcessing.NumberOfTrackedObjects; z++)
      {
        // Sanity check
        int j = 0;
        while (samples[j].Object[z] == null || samples[j].Object[z].Velocity == null)
        {
          j++;
          if (j == samples.Count)
          {
            return;
          }
        }

        samples[j].Object[z].VelocityI = samples[j].Object[z].Velocity;
        samples[j].Object[z].VelocityXI = samples[j].Object[z].VelocityX;
        samples[j].Object[z].VelocityYI = samples[j].Object[z].VelocityY;
        samples[j].Object[z].AccelerationI = samples[j].Object[z].Acceleration;
        samples[j].Object[z].AccelerationXI = samples[j].Object[z].AccelerationX;
        samples[j].Object[z].AccelerationYI = samples[j].Object[z].AccelerationY;

        DataSample lastSample = samples[j].Object[z];

        for (int i = j + 1; i < samples.Count; i++)
        {
          if (samples[i].Object[z] == null)
          {
            continue;
          }

          samples[i].Object[z].VelocityI = this.SmoothingFactor * samples[i].Object[z].Velocity
                                           + (1 - this.SmoothingFactor) * lastSample.VelocityI;
          samples[i].Object[z].VelocityXI = this.SmoothingFactor * samples[i].Object[z].VelocityX
                                            + (1 - this.SmoothingFactor) * lastSample.VelocityXI;
          samples[i].Object[z].VelocityYI = this.SmoothingFactor * samples[i].Object[z].VelocityY
                                            + (1 - this.SmoothingFactor) * lastSample.VelocityYI;

          samples[i].Object[z].AccelerationI = this.SmoothingFactor * samples[i].Object[z].Acceleration
                                               + (1 - this.SmoothingFactor) * lastSample.AccelerationI;
          samples[i].Object[z].AccelerationXI = this.SmoothingFactor * samples[i].Object[z].AccelerationX
                                                + (1 - this.SmoothingFactor) * lastSample.AccelerationXI;
          samples[i].Object[z].AccelerationYI = this.SmoothingFactor * samples[i].Object[z].AccelerationY
                                                + (1 - this.SmoothingFactor) * lastSample.AccelerationYI;

          lastSample = samples[i].Object[z];
        }
      }
    }

    #endregion
  }
}