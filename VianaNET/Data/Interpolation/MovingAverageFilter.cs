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
  public class MovingAverageFilter : InterpolationBase
  {
    public MovingAverageFilter()
    {
      this.FilterType = Interpolation.FilterTypes.MovingAverage;
      this.NumberOfSamplesToInterpolate = 3;
      this.CustomUserControl = new MovingAverageUserControl(this);
    }

    public override void CalculateInterpolatedValues(DataCollection samples)
    {
      int startIndex = (int)(this.NumberOfSamplesToInterpolate / 2f);

      int end = Math.Min(startIndex, samples.Count);

      for (int z = 0; z < Calibration.Instance.NumberOfTrackedObjects; z++)
      {
        for (int i = 0; i < end; i++)
        {
          if (samples[i].Object[z] == null)
          {
            continue;
          }

          samples[i].Object[z].VelocityI = null;
          samples[i].Object[z].VelocityXI = null;
          samples[i].Object[z].VelocityYI = null;
          samples[i].Object[z].AccelerationI = null;
          samples[i].Object[z].AccelerationXI = null;
          samples[i].Object[z].AccelerationYI = null;
        }

        for (int i = samples.Count - 1; (i >= samples.Count - startIndex) && (i >= 0); i--)
        {
          if (samples[i].Object[z] == null)
          {
            continue;
          }

          samples[i].Object[z].VelocityI = null;
          samples[i].Object[z].VelocityXI = null;
          samples[i].Object[z].VelocityYI = null;
          samples[i].Object[z].AccelerationI = null;
          samples[i].Object[z].AccelerationXI = null;
          samples[i].Object[z].AccelerationYI = null;
        }

        for (int i = startIndex; i < samples.Count - startIndex; i++)
        {
          DataCollection samplesForInterpolation = samples.GetRangeAtPosition(i - startIndex, this.NumberOfSamplesToInterpolate);

          double velocityInterpolated = 0d;
          double velocityXInterpolated = 0d;
          double velocityYInterpolated = 0d;

          if (i - startIndex == 0)
          {
            continue;
          }

          if (samples[i].Object[z] == null)
          {
            continue;
          }
          
          foreach (TimeSample item in samplesForInterpolation)
          {
            velocityInterpolated += item.Object[z].Velocity.Value;
            velocityXInterpolated += item.Object[z].VelocityX.Value;
            velocityYInterpolated += item.Object[z].VelocityY.Value;
          }

          samples[i].Object[z].VelocityI = velocityInterpolated / this.NumberOfSamplesToInterpolate;
          samples[i].Object[z].VelocityXI = velocityXInterpolated / this.NumberOfSamplesToInterpolate;
          samples[i].Object[z].VelocityYI = velocityYInterpolated / this.NumberOfSamplesToInterpolate;

          if (i - startIndex == 1)
          {
            continue;
          }

          double accelerationInterpolated = 0d;
          double accelerationXInterpolated = 0d;
          double accelerationYInterpolated = 0d;

          foreach (TimeSample item in samplesForInterpolation)
          {
            accelerationInterpolated += item.Object[z].Acceleration.Value;
            accelerationXInterpolated += item.Object[z].AccelerationX.Value;
            accelerationYInterpolated += item.Object[z].AccelerationY.Value;
          }

          samples[i].Object[z].AccelerationI = accelerationInterpolated / this.NumberOfSamplesToInterpolate;
          samples[i].Object[z].AccelerationXI = accelerationXInterpolated / this.NumberOfSamplesToInterpolate;
          samples[i].Object[z].AccelerationYI = accelerationYInterpolated / this.NumberOfSamplesToInterpolate;
        }
      }
    }
  }
}
