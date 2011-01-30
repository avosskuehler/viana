using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VianaNET
{
  public class DataSample
  {
    public int Framenumber { get; set; }
    public long Timestamp { get; set; }
    public double CoordinateX { get; set; }
    public double CoordinateY { get; set; }
    public double Distance { get; set; }
    public double DistanceX { get; set; }
    public double DistanceY { get; set; }
    public double Length { get; set; }
    public double LengthX { get; set; }
    public double LengthY { get; set; }
    public double? Velocity { get; set; }
    public double? VelocityX { get; set; }
    public double? VelocityY { get; set; }
    public double? VelocityI { get; set; }
    public double? VelocityXI { get; set; }
    public double? VelocityYI { get; set; }
    public double? Acceleration { get; set; }
    public double? AccelerationX { get; set; }
    public double? AccelerationY { get; set; }
    public double? AccelerationI { get; set; }
    public double? AccelerationXI { get; set; }
    public double? AccelerationYI { get; set; }

    public class TimeComparer : IComparer<DataSample>
    {
      public int Compare(DataSample x, DataSample y)
      {
        return (int)(x.Timestamp - y.Timestamp);
      }
    }
  }
}
