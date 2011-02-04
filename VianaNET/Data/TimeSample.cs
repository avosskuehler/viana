using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VianaNET
{
  public class TimeSample
  {
    public int Framenumber { get; set; }
    public long Timestamp { get; set; }
    public DataSample[] Object { get; set; }

    public TimeSample()
    {
      this.Object = new DataSample[Video.Instance.ImageProcessing.NumberOfTrackedObjects];
    }

    public class TimeComparer : IComparer<TimeSample>
    {
      public int Compare(TimeSample x, TimeSample y)
      {
        return (int)(x.Timestamp - y.Timestamp);
      }
    }
  }
}
