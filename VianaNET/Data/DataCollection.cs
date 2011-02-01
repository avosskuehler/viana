using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Visifire.Charts;

namespace VianaNET
{
  public class DataCollection : SortedObservableCollection<TimeSample>
  {
    public DataCollection() :
      base(new TimeSample.TimeComparer())
    {

    }

    public bool Contains(TimeSample sampleToCheck, out int index)
    {
      index = -1;
      foreach (TimeSample sample in this)
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
      foreach (TimeSample sample in this)
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
  }
}
