using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;

namespace VianaNET
{
  public class VideoData : DependencyObject, INotifyPropertyChanged
  {
    private static VideoData instance;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Initializes a new instance of the DataStream class.
    /// </summary>
    private VideoData()
    {
      this.Samples = new DataCollection();
    }

    /// <summary>
    /// Gets the <see cref="VideoData"/> singleton.
    /// If the underlying instance is null, a instance will be created.
    /// </summary>
    public static VideoData Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new VideoData();
        }

        // return the existing/new instance
        return instance;
      }
    }

    public DataCollection Samples
    {
      get { return (DataCollection)GetValue(SamplesProperty); }
      set { SetValue(SamplesProperty, value); }
    }

    public static readonly DependencyProperty SamplesProperty =
      DependencyProperty.Register(
      "Samples",
      typeof(DataCollection),
      typeof(VideoData),
      new UIPropertyMetadata(null));

    public int Count
    {
      get { return this.Samples.Count; }
    }

    public void RefreshDistanceVelocityAcceleration()
    {
      for (int i = 0; i < this.Samples.Count; i++)
      {
        Point calibratedPoint = CalibrateSample(this.Samples[i]);
        this.Samples[i].DistanceX = calibratedPoint.X;
        this.Samples[i].DistanceY = calibratedPoint.Y;

        if (i == 0)
        {
          continue;
        }

        if (i == 1)
        {
          this.Samples[1].Distance = GetDistance(this.Samples[1], this.Samples[0]);
          this.Samples[1].Velocity = GetVelocity(this.Samples[1], this.Samples[0]);
          this.Samples[1].VelocityX = GetXVelocity(this.Samples[1], this.Samples[0]);
          this.Samples[1].VelocityY = GetYVelocity(this.Samples[1], this.Samples[0]);

          continue;
        }

        DataSample previousSample = this.Samples[i - 1];
        this.Samples[i].Distance = GetDistance(this.Samples[i], previousSample);
        this.Samples[i].Velocity = GetVelocity(this.Samples[i], previousSample);
        this.Samples[i].VelocityX = GetXVelocity(this.Samples[i], previousSample);
        this.Samples[i].VelocityY = GetYVelocity(this.Samples[i], previousSample);
        this.Samples[i].Acceleration = GetAcceleration(this.Samples[i], previousSample);
        this.Samples[i].AccelerationX = GetXAcceleration(this.Samples[i], previousSample);
        this.Samples[i].AccelerationY = GetYAcceleration(this.Samples[i], previousSample);
      }
    }

    public void AddPoint(Point newSamplePosition)
    {
      DataSample newSample = new DataSample();

      newSample.Framenumber = Video.Instance.FrameIndex;
      newSample.Timestamp = Video.Instance.FrameTimestamp;
      newSample.CoordinateX = newSamplePosition.X;
      newSample.CoordinateY = newSamplePosition.Y;

      // Add new point
      int index;
      if (this.Samples.Contains(newSample, out index))
      {
        this.Samples[index] = newSample;
      }
      else
      {
        this.Samples.Add(newSample);
      }

      this.OnPropertyChanged("Samples");
    }

    private static Point CalibrateSample(DataSample value)
    {
      if (!Calibration.Instance.IsVideoCalibrated)
      {
        return new Point(value.CoordinateX, value.CoordinateY);
      }

      Point calibratedPoint = new Point(value.CoordinateX, value.CoordinateY);
      calibratedPoint.Offset(-Calibration.Instance.OriginInPixel.X, -Calibration.Instance.OriginInPixel.Y);
      calibratedPoint.X = calibratedPoint.X * Calibration.Instance.ScalePixelToUnit;
      calibratedPoint.Y = calibratedPoint.Y * Calibration.Instance.ScalePixelToUnit;

      return calibratedPoint;
    }

    private static double GetYAcceleration(DataSample newSample, DataSample previousSample)
    {
      return (newSample.VelocityY - previousSample.VelocityY) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetXAcceleration(DataSample newSample, DataSample previousSample)
    {
      return (newSample.VelocityX - previousSample.VelocityX) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetAcceleration(DataSample newSample, DataSample previousSample)
    {
      return (newSample.Velocity - previousSample.Velocity) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetYVelocity(DataSample newSample, DataSample previousSample)
    {
      return (newSample.CoordinateY - previousSample.CoordinateY) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetXVelocity(DataSample newSample, DataSample previousSample)
    {
      return (newSample.CoordinateX - previousSample.CoordinateX) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetVelocity(DataSample newSample, DataSample previousSample)
    {
      return (newSample.Distance / (newSample.Timestamp - previousSample.Timestamp) * 1000);
    }

    private static double GetDistance(DataSample newSample, DataSample previousSample)
    {
      return Math.Sqrt(Math.Pow(newSample.CoordinateY - previousSample.CoordinateY, 2) + Math.Pow(newSample.CoordinateX - previousSample.CoordinateX, 2));
    }

    public bool RemovePoint(long timeStamp)
    {
      bool success = this.Samples.Remove(timeStamp);
      this.OnPropertyChanged("Samples");
      return success;
    }

    public void Reset()
    {
      this.Samples.Clear();
      this.OnPropertyChanged("Samples");
    }

  }
}
