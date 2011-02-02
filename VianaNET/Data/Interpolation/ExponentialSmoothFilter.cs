using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLocalizeExtension.Extensions;
using System.Collections.ObjectModel;

namespace VianaNET
{
  public class ExponentialSmoothFilter : InterpolationBase
  {
    public float SmoothingFactor { get; set; }

    public ExponentialSmoothFilter()
    {
      this.FilterType = Interpolation.FilterTypes.ExponentialSmooth;
      this.NumberOfSamplesToInterpolate = 2;
      this.SmoothingFactor = 0.7f;
      this.CustomUserControl = new ExponentialSmoothControl(this);
    }

    public override void CalculateInterpolatedValues(DataCollection samples)
    {
      for (int z = 0; z < Calibration.Instance.NumberOfTrackedObjects; z++)
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

          samples[i].Object[z].VelocityI = this.SmoothingFactor * samples[i].Object[z].Velocity +
            (1 - this.SmoothingFactor) * lastSample.VelocityI;
          samples[i].Object[z].VelocityXI = this.SmoothingFactor * samples[i].Object[z].VelocityX +
            (1 - this.SmoothingFactor) * lastSample.VelocityXI;
          samples[i].Object[z].VelocityYI = this.SmoothingFactor * samples[i].Object[z].VelocityY +
            (1 - this.SmoothingFactor) * lastSample.VelocityYI;

          samples[i].Object[z].AccelerationI = this.SmoothingFactor * samples[i].Object[z].Acceleration +
            (1 - this.SmoothingFactor) * lastSample.AccelerationI;
          samples[i].Object[z].AccelerationXI = this.SmoothingFactor * samples[i].Object[z].AccelerationX +
             (1 - this.SmoothingFactor) * lastSample.AccelerationXI;
          samples[i].Object[z].AccelerationYI = this.SmoothingFactor * samples[i].Object[z].AccelerationY +
             (1 - this.SmoothingFactor) * lastSample.AccelerationYI;

          lastSample = samples[i].Object[z];
        }
      }
    }

  }
}
