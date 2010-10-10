using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

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
