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

    public Point LastPoint
    {
      get { return (Point)GetValue(LastPointProperty); }
      set { SetValue(LastPointProperty, value); }
    }

    public static readonly DependencyProperty LastPointProperty =
      DependencyProperty.Register(
      "LastPoint",
      typeof(Point),
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
        this.Samples[i].Length = 0d;
        this.Samples[i].LengthX = 0d;
        this.Samples[i].LengthY = 0d;

        if (i == 0)
        {
          continue;
        }

        DataSample previousSample = this.Samples[i - 1];

        this.Samples[i].Distance = GetDistance(this.Samples[i], previousSample);
        this.Samples[i].Length = GetLength(this.Samples[i], previousSample);
        this.Samples[i].LengthX = GetXLength(this.Samples[i], previousSample);
        this.Samples[i].LengthY = GetYLength(this.Samples[i], previousSample);
        this.Samples[i].Velocity = GetVelocity(this.Samples[i], previousSample);
        this.Samples[i].VelocityX = GetXVelocity(this.Samples[i], previousSample);
        this.Samples[i].VelocityY = GetYVelocity(this.Samples[i], previousSample);

        if (i == 1)
        {
          continue;
        }

        this.Samples[i].Acceleration = GetAcceleration(this.Samples[i], previousSample);
        this.Samples[i].AccelerationX = GetXAcceleration(this.Samples[i], previousSample);
        this.Samples[i].AccelerationY = GetYAcceleration(this.Samples[i], previousSample);
      }

      Interpolation.Instance.CurrentInterpolationFilter.CalculateInterpolatedValues(this.Samples);

      // Refresh DataBinding to DataGrid.
      this.OnPropertyChanged("Samples");
      this.OnPropertyChanged("Interpolation");
    }

    public void AddPoint(Point newSamplePosition)
    {
      this.LastPoint = newSamplePosition;

      DataSample newSample = new DataSample();

      newSample.Framenumber = Video.Instance.FrameIndex;
      newSample.Timestamp = Video.Instance.FrameTimestampInMS;
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

      this.OnPropertyChanged("LastPoint");
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
      return (newSample.VelocityY.Value - previousSample.VelocityY.Value) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetXAcceleration(DataSample newSample, DataSample previousSample)
    {
      return (newSample.VelocityX.Value - previousSample.VelocityX.Value) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetAcceleration(DataSample newSample, DataSample previousSample)
    {
      return (newSample.Velocity.Value - previousSample.Velocity.Value) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetYVelocity(DataSample newSample, DataSample previousSample)
    {
      return (newSample.DistanceY - previousSample.DistanceY) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetXVelocity(DataSample newSample, DataSample previousSample)
    {
      return (newSample.DistanceX - previousSample.DistanceX) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetVelocity(DataSample newSample, DataSample previousSample)
    {
      double velocity = newSample.Distance / (newSample.Timestamp - previousSample.Timestamp) * 1000;
      return velocity;
    }

    private static double GetDistance(DataSample newSample, DataSample previousSample)
    {
      double distance = Math.Sqrt(Math.Pow(newSample.DistanceY - previousSample.DistanceY, 2) + Math.Pow(newSample.DistanceX - previousSample.DistanceX, 2));
      return distance;
    }

    private static double GetLength(DataSample newSample, DataSample previousSample)
    {
      double length = previousSample.Length + newSample.Distance;
      return length;
    }

    private static double GetXLength(DataSample newSample, DataSample previousSample)
    {
      double lengthX = previousSample.LengthX + newSample.DistanceX;
      return lengthX;
    }

    private static double GetYLength(DataSample newSample, DataSample previousSample)
    {
      double lengthY = previousSample.LengthY + newSample.DistanceY;
      return lengthY;
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
