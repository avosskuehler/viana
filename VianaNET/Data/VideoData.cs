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
      this.LastPoint = new Point[Calibration.Instance.NumberOfTrackedObjects];
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

    public Point[] LastPoint
    {
      get { return (Point[])GetValue(LastPointProperty); }
      set { SetValue(LastPointProperty, value); }
    }

    public static readonly DependencyProperty LastPointProperty =
      DependencyProperty.Register(
      "LastPoint",
      typeof(Point[]),
      typeof(VideoData),
      new UIPropertyMetadata(null));

    public int Count
    {
      get { return this.Samples.Count; }
    }

    public void RefreshDistanceVelocityAcceleration()
    {
      for (int j = 0; j < Calibration.Instance.NumberOfTrackedObjects; j++)
      {
        TimeSample previousSample=null;

        for (int i = 0; i < this.Samples.Count; i++)
        {
          if (this.Samples[i].Object[j] == null)
          {
            continue;
          }

          Point calibratedPoint = CalibrateSample(this.Samples[i].Object[j]);
          this.Samples[i].Object[j].PositionX = calibratedPoint.X;
          this.Samples[i].Object[j].PositionY = calibratedPoint.Y;

          if (i == 0)
          {
            this.Samples[i].Object[j].Distance = 0d;
            this.Samples[i].Object[j].DistanceX = 0d;
            this.Samples[i].Object[j].DistanceY = 0d;
            this.Samples[i].Object[j].Length = 0d;
            this.Samples[i].Object[j].LengthX = 0d;
            this.Samples[i].Object[j].LengthY = 0d;
            previousSample = this.Samples[i];
            continue;
          }

          this.Samples[i].Object[j].Distance = GetDistance(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].DistanceX = GetXDistance(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].DistanceY = GetYDistance(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].Length = GetLength(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].LengthX = GetXLength(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].LengthY = GetYLength(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].Velocity = GetVelocity(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].VelocityX = GetXVelocity(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].VelocityY = GetYVelocity(j, this.Samples[i], previousSample);

          if (i == 1)
          {
            previousSample = this.Samples[i];
            continue;
          }

          this.Samples[i].Object[j].Acceleration = GetAcceleration(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].AccelerationX = GetXAcceleration(j, this.Samples[i], previousSample);
          this.Samples[i].Object[j].AccelerationY = GetYAcceleration(j, this.Samples[i], previousSample);

          previousSample = this.Samples[i];
        }
      }

      Interpolation.Instance.CurrentInterpolationFilter.CalculateInterpolatedValues(this.Samples);

      // Refresh DataBinding to DataGrid.
      this.OnPropertyChanged("Samples");
      this.OnPropertyChanged("Interpolation");
    }

    public void AddPoint(int objectIndex, Point newSamplePosition)
    {
      this.LastPoint[objectIndex] = newSamplePosition;

      TimeSample timeSample = new TimeSample();
      timeSample.Framenumber = Video.Instance.FrameIndex;
      timeSample.Timestamp = Video.Instance.FrameTimestampInMS;

      DataSample newObjectSample = new DataSample();
      newObjectSample.PixelX = newSamplePosition.X;
      newObjectSample.PixelY = newSamplePosition.Y;

      // Add new point
      int index;
      if (this.Samples.Contains(timeSample, out index))
      {
        this.Samples[index].Object[objectIndex] = newObjectSample;
      }
      else
      {
        timeSample.Object[objectIndex] = newObjectSample;
        this.Samples.Add(timeSample);
      }

      this.OnPropertyChanged("LastPoint");
    }

    private static Point CalibrateSample(DataSample value)
    {
      if (!Calibration.Instance.IsVideoCalibrated)
      {
        return new Point(value.PixelX, value.PixelY);
      }

      Point calibratedPoint = new Point(value.PixelX, value.PixelY);
      calibratedPoint.Offset(-Calibration.Instance.OriginInPixel.X, -Calibration.Instance.OriginInPixel.Y);
      calibratedPoint.X = calibratedPoint.X * Calibration.Instance.ScalePixelToUnit;
      calibratedPoint.Y = calibratedPoint.Y * Calibration.Instance.ScalePixelToUnit;

      return calibratedPoint;
    }

    private static double GetDistance(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double distance = Math.Sqrt(Math.Pow(newSample.Object[objectIndex].PositionY - previousSample.Object[objectIndex].PositionY, 2) + Math.Pow(newSample.Object[objectIndex].PositionX - previousSample.Object[objectIndex].PositionX, 2));
      return distance;
    }

    private static double GetXDistance(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double distance = Math.Abs(newSample.Object[objectIndex].PositionX - previousSample.Object[objectIndex].PositionX);
      return distance;
    }

    private static double GetYDistance(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double distance = Math.Abs(newSample.Object[objectIndex].PositionY - previousSample.Object[objectIndex].PositionY);
      return distance;
    }

    private static double GetLength(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double length = previousSample.Object[objectIndex].Length + newSample.Object[objectIndex].Distance;
      return length;
    }

    private static double GetXLength(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double lengthX = previousSample.Object[objectIndex].LengthX + newSample.Object[objectIndex].DistanceX;
      return lengthX;
    }

    private static double GetYLength(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double lengthY = previousSample.Object[objectIndex].LengthY + newSample.Object[objectIndex].DistanceY;
      return lengthY;
    }

    private static double GetVelocity(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      double velocity = newSample.Object[objectIndex].Distance / (newSample.Timestamp - previousSample.Timestamp) * 1000;
      return velocity;
    }

    private static double GetXVelocity(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      return (newSample.Object[objectIndex].PositionX - previousSample.Object[objectIndex].PositionX) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetYVelocity(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      return (newSample.Object[objectIndex].PositionY - previousSample.Object[objectIndex].PositionY) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetAcceleration(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      return (newSample.Object[objectIndex].Velocity.Value - previousSample.Object[objectIndex].Velocity.Value) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetXAcceleration(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      return (newSample.Object[objectIndex].VelocityX.Value - previousSample.Object[objectIndex].VelocityX.Value) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
    }

    private static double GetYAcceleration(int objectIndex, TimeSample newSample, TimeSample previousSample)
    {
      return (newSample.Object[objectIndex].VelocityY.Value - previousSample.Object[objectIndex].VelocityY.Value) / (newSample.Timestamp - previousSample.Timestamp) * 1000;
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
      this.LastPoint = new Point[Calibration.Instance.NumberOfTrackedObjects];
      this.OnPropertyChanged("Samples");
    }
  }
}
