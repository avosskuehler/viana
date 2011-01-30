using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Visifire.Charts;

namespace VianaNET
{
  public class DataCollection : SortedObservableCollection<DataSample>
  {
    public DataCollection() :
      base(new DataSample.TimeComparer())
    {

    }

    public bool Contains(DataSample sampleToCheck, out int index)
    {
      index = -1;
      foreach (DataSample sample in this)
      {
        if (sample.Framenumber == sampleToCheck.Framenumber ||
          sample.Timestamp == sampleToCheck.Timestamp)
        {
          index = this.IndexOf(sample);
          return true;
        }
      }

      return false;
    }

    public DataCollection GetRangeAtPosition(int startIndex, int numberOfSamplesToReturn)
    {
      DataCollection returnCollection = new DataCollection();
      for (int i = startIndex; i < startIndex + numberOfSamplesToReturn; i++)
      {
        returnCollection.Add(this[i]);
      }

      return returnCollection;
    }

    public bool Remove(long timeStamp)
    {
      int index = -1;
      foreach (DataSample sample in this)
      {
        if (sample.Timestamp == timeStamp)
        {
          index = this.IndexOf(sample);
          break;
        }
      }

      if (index >= 0)
      {
        this.RemoveAt(index);
        return true;
      }
      else
      {
        return false;
      }

    }

    //public ObservableCollection<KeyValuePair<Double, Double>> GetPointCollection(AxisType xAxis, AxisType yAxis)
    //{
    //  this.Select
    //  ObservableCollection<KeyValuePair<Double, Double>> points = new ObservableCollection<KeyValuePair<Double, Double>>();
    //  foreach (DataSample sample in this)
    //  {
    //    points.Add(new KeyValuePair<double, double>());
    //    double key = 0d;
    //    double value = 0d;
    //    switch (xAxis)
    //    {
    //      case AxisType.I:
    //        key = sample.Framenumber;
    //        break;
    //      case AxisType.T:
    //        dp.Key = sample.Timestamp;
    //        break;
    //      case AxisType.PX:
    //        dp.Key = sample.CoordinateX;
    //        break;
    //      case AxisType.PY:
    //        dp.Key = sample.CoordinateY;
    //        break;
    //      case AxisType.D:
    //        dp.Key = sample.Distance;
    //        break;
    //      case AxisType.DX:
    //        dp.Key = sample.DistanceX;
    //        break;
    //      case AxisType.DY:
    //        dp.Key = sample.DistanceY;
    //        break;
    //      case AxisType.V:
    //        dp.Key = sample.Velocity;
    //        break;
    //      case AxisType.VX:
    //        dp.Key = sample.VelocityX;
    //        break;
    //      case AxisType.VY:
    //        dp.Key = sample.VelocityY;
    //        break;
    //      case AxisType.VI:
    //        dp.Key = sample.VelocityYI;
    //        break;
    //      case AxisType.VXI:
    //        dp.Key = sample.VelocityXI;
    //        break;
    //      case AxisType.VYI:
    //        dp.Key = sample.VelocityYI;
    //        break;
    //      case AxisType.A:
    //        dp.Key = sample.Acceleration;
    //        break;
    //      case AxisType.AX:
    //        dp.Key = sample.AccelerationX;
    //        break;
    //      case AxisType.AY:
    //        dp.Key = sample.AccelerationY;
    //        break;
    //      case AxisType.AI:
    //        dp.Key = sample.AccelerationI;
    //        break;
    //      case AxisType.AXI:
    //        dp.Key = sample.AccelerationXI;
    //        break;
    //      case AxisType.AYI:
    //        dp.Key = sample.AccelerationYI;
    //        break;
    //    }

    //    switch (yAxis)
    //    {
    //      case AxisType.I:
    //        dp.YValue = sample.Framenumber;
    //        break;
    //      case AxisType.T:
    //        dp.YValue = sample.Timestamp;
    //        break;
    //      case AxisType.PX:
    //        dp.YValue = sample.CoordinateX;
    //        break;
    //      case AxisType.PY:
    //        dp.YValue = sample.CoordinateY;
    //        break;
    //      case AxisType.D:
    //        dp.YValue = sample.Distance;
    //        break;
    //      case AxisType.DX:
    //        dp.YValue = sample.DistanceX;
    //        break;
    //      case AxisType.DY:
    //        dp.YValue = sample.DistanceY;
    //        break;
    //      case AxisType.V:
    //        dp.YValue = sample.Velocity.Value;
    //        break;
    //      case AxisType.VX:
    //        dp.YValue = sample.VelocityX.Value;
    //        break;
    //      case AxisType.VY:
    //        dp.YValue = sample.VelocityY.Value;
    //        break;
    //      case AxisType.VI:
    //        dp.YValue = sample.VelocityYI.Value;
    //        break;
    //      case AxisType.VXI:
    //        dp.YValue = sample.VelocityXI.Value;
    //        break;
    //      case AxisType.VYI:
    //        dp.YValue = sample.VelocityYI.Value;
    //        break;
    //      case AxisType.A:
    //        dp.YValue = sample.Acceleration.Value;
    //        break;
    //      case AxisType.AX:
    //        dp.YValue = sample.AccelerationX.Value;
    //        break;
    //      case AxisType.AY:
    //        dp.YValue = sample.AccelerationY.Value;
    //        break;
    //      case AxisType.AI:
    //        dp.YValue = sample.AccelerationI.Value;
    //        break;
    //      case AxisType.AXI:
    //        dp.YValue = sample.AccelerationXI.Value;
    //        break;
    //      case AxisType.AYI:
    //        dp.YValue = sample.AccelerationYI.Value;
    //        break;
    //    }

    //    points.Add(dp);
    //  }

    //  return points;
    //}

    //public PreviousFound GetPrevious(long timeStamp, out DataSample previous, out DataSample previousPrevious)
    //{
    //  PreviousFound foundFlag = PreviousFound.None;
    //  previous = null;
    //  previousPrevious = null;

    //  int index = -1;
    //  foreach (DataSample sample in this)
    //  {
    //    if (sample.Timestamp < timeStamp)
    //    {
    //      index = this.IndexOf(sample);
    //    }
    //    else
    //    {
    //      break;
    //    }
    //  }

    //  if (index >= 0)
    //  {
    //    foundFlag = PreviousFound.One;
    //    previous = this[index];
    //    if (index > 0)
    //    {
    //      foundFlag = PreviousFound.Two;
    //      previousPrevious = this[index - 1];
    //    }
    //  }

    //  return foundFlag;
    //}
  }
}
