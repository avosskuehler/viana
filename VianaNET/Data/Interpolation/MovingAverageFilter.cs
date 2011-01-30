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

      for (int i = 0; i < startIndex; i++)
      {
        samples[i].VelocityI = null;
        samples[i].VelocityXI = null;
        samples[i].VelocityYI = null;
        samples[i].AccelerationI = null;
        samples[i].AccelerationXI = null;
        samples[i].AccelerationYI = null;
      }

      for (int i = samples.Count-1; i >= samples.Count - startIndex; i--)
      {
        samples[i].VelocityI = null;
        samples[i].VelocityXI = null;
        samples[i].VelocityYI = null;
        samples[i].AccelerationI = null;
        samples[i].AccelerationXI = null;
        samples[i].AccelerationYI = null;
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

        foreach (DataSample item in samplesForInterpolation)
        {
          velocityInterpolated += item.Velocity.Value;
          velocityXInterpolated += item.VelocityX.Value;
          velocityYInterpolated += item.VelocityY.Value;
        }

        samples[i].VelocityI = velocityInterpolated / this.NumberOfSamplesToInterpolate;
        samples[i].VelocityXI = velocityXInterpolated / this.NumberOfSamplesToInterpolate;
        samples[i].VelocityYI = velocityYInterpolated / this.NumberOfSamplesToInterpolate;

        if (i - startIndex == 1)
        {
          continue;
        }

        double accelerationInterpolated = 0d;
        double accelerationXInterpolated = 0d;
        double accelerationYInterpolated = 0d;

        foreach (DataSample item in samplesForInterpolation)
        {
          accelerationInterpolated += item.Acceleration.Value;
          accelerationXInterpolated += item.AccelerationX.Value;
          accelerationYInterpolated += item.AccelerationY.Value;
        }

        samples[i].AccelerationI = accelerationInterpolated / this.NumberOfSamplesToInterpolate;
        samples[i].AccelerationXI = accelerationXInterpolated / this.NumberOfSamplesToInterpolate;
        samples[i].AccelerationYI = accelerationYInterpolated / this.NumberOfSamplesToInterpolate;
      }
    }
  }
}
