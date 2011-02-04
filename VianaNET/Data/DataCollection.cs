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

    public List<DataSample>[] GetRangeAtPosition(
      int startIndex,
      int numberOfSamplesToReturn,
      int objectIndex)
    {
      List<DataSample>[] returnCollection = new List<DataSample>[2];

      List<DataSample> velocityCollection = new List<DataSample>(numberOfSamplesToReturn);
      List<DataSample> accelerationCollection = new List<DataSample>(numberOfSamplesToReturn);
      returnCollection[0] = velocityCollection;
      returnCollection[1] = accelerationCollection;

      int counter = numberOfSamplesToReturn;

      for (int i = startIndex; i < startIndex + counter; i++)
      {
        if (this[i].Object[objectIndex] == null)
        {
          counter++;
          if (startIndex + counter >= this.Count)
          {
            break;
          }
          continue;
        }
        else if (this[i].Object[objectIndex].Velocity == null)
        {
          counter++;
          if (startIndex + counter >= this.Count)
          {
            break;
          }
          continue;
        }
        else
        {
          if (velocityCollection.Count < numberOfSamplesToReturn)
          {
            velocityCollection.Add(this[i].Object[objectIndex]);
          }
          if (this[i].Object[objectIndex].Acceleration == null)
          {
            counter++;
            if (startIndex + counter >= this.Count)
            {
              break;
            }
            continue;
          }
          else
          {
            accelerationCollection.Add(this[i].Object[objectIndex]);
          }
        }
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
