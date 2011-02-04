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
      int[] validSamples = new int[Video.Instance.ImageProcessing.NumberOfTrackedObjects];

      int startIndex = (int)(this.NumberOfSamplesToInterpolate / 2f);
      //int end = Math.Min(startIndex, samples.Count);

      // Reset all interpolated values
      for (int i = 0; i < samples.Count; i++)
      {
        for (int j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
        {
          DataSample currentSample = samples[i].Object[j];
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

          List<DataSample>[] samplesForInterpolation =
            samples.GetRangeAtPosition(i - startIndex, this.NumberOfSamplesToInterpolate, j);

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
  }
}
