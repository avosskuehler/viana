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
    public ExponentialSmoothFilter()
    {
      this.FilterType = Interpolation.FilterTypes.ExponentialSmooth;
      this.NumberOfSamplesToInterpolate = 3;
    }

    public override void CalculateInterpolatedValues(DataCollection samples)
    {
      //int startIndex = (int)(this.NumberOfSamplesToInterpolate / 2f);
      //for (int i = startIndex; i < samples.Count - startIndex; i++)
      //{
      //  DataCollection samplesForInterpolation = samples.GetRangeAtPosition(i - startIndex, this.NumberOfSamplesToInterpolate);

      //  double velocityInterpolated = 0d;
      //  double velocityXInterpolated = 0d;
      //  double velocityYInterpolated = 0d;
      //  double accelerationInterpolated = 0d;
      //  double accelerationXInterpolated = 0d;
      //  double accelerationYInterpolated = 0d;
      //  foreach (DataSample item in samplesForInterpolation)
      //  {
      //    velocityInterpolated += item.Velocity.Value;
      //    velocityXInterpolated += item.VelocityX.Value;
      //    velocityYInterpolated += item.VelocityY.Value;
      //    accelerationInterpolated += item.Acceleration.Value;
      //    accelerationXInterpolated += item.AccelerationX.Value;
      //    accelerationYInterpolated += item.AccelerationY.Value;
      //  }

      //  samples[i].VelocityI = velocityInterpolated / this.NumberOfSamplesToInterpolate;
      //  samples[i].VelocityXI = velocityXInterpolated / this.NumberOfSamplesToInterpolate;
      //  samples[i].VelocityYI = velocityYInterpolated / this.NumberOfSamplesToInterpolate;
      //  samples[i].AccelerationI = accelerationInterpolated / this.NumberOfSamplesToInterpolate;
      //  samples[i].AccelerationXI = accelerationXInterpolated / this.NumberOfSamplesToInterpolate;
      //  samples[i].AccelerationYI = accelerationYInterpolated / this.NumberOfSamplesToInterpolate;
      //}
    }

  }
}
