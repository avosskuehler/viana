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
      this.LastPoint = new Point[Video.Instance.ImageProcessing.NumberOfTrackedObjects];
      Interpolation.Instance.PropertyChanged += new PropertyChangedEventHandler(Interpolation_PropertyChanged);
    }

    void Interpolation_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsInterpolatingData" || e.PropertyName == "CurrentInterpolationFilter")
      {
        this.RefreshDistanceVelocityAcceleration();
      }
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
      TimeSample[] previousSamples = new TimeSample[Video.Instance.ImageProcessing.NumberOfTrackedObjects];
      int[] validSamples = new int[Video.Instance.ImageProcessing.NumberOfTrackedObjects];

      for (int i = 0; i < this.Samples.Count; i++)
      {
        for (int j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
        {
          DataSample currentSample = this.Samples[i].Object[j];
          if (currentSample == null)
          {
            continue;
          }

          validSamples[j]++;

          Point calibratedPoint = CalibrateSample(currentSample);
          currentSample.PositionX = calibratedPoint.X;
          currentSample.PositionY = calibratedPoint.Y;

          if (validSamples[j] == 1)
          {
            currentSample.Distance = 0d;
            currentSample.DistanceX = 0d;
            currentSample.DistanceY = 0d;
            currentSample.Length = 0d;
            currentSample.LengthX = 0d;
            currentSample.LengthY = 0d;
            previousSamples[j] = this.Samples[i];
            continue;
          }
          DataSample previousSample = previousSamples[j].Object[j];
          long timeTifference = this.Samples[i].Timestamp - previousSamples[j].Timestamp;
          currentSample.Distance = GetDistance(j, currentSample, previousSample);
          currentSample.DistanceX = GetXDistance(j, currentSample, previousSample);
          currentSample.DistanceY = GetYDistance(j, currentSample, previousSample);
          currentSample.Length = GetLength(j, currentSample, previousSample);
          currentSample.LengthX = GetXLength(j, currentSample, previousSample);
          currentSample.LengthY = GetYLength(j, currentSample, previousSample);
          currentSample.Velocity = GetVelocity(j, currentSample, timeTifference);
          currentSample.VelocityX = GetXVelocity(j, currentSample, previousSample, timeTifference);
          currentSample.VelocityY = GetYVelocity(j, currentSample, previousSample, timeTifference);

          if (validSamples[j] == 2)
          {
            previousSamples[j] = this.Samples[i];
            continue;
          }

          currentSample.Acceleration = GetAcceleration(j, currentSample, previousSample, timeTifference);
          currentSample.AccelerationX = GetXAcceleration(j, currentSample, previousSample, timeTifference);
          currentSample.AccelerationY = GetYAcceleration(j, currentSample, previousSample, timeTifference);

          previousSamples[j] = this.Samples[i];
        }
      }

      if (Interpolation.Instance.IsInterpolatingData)
      {
        Interpolation.Instance.CurrentInterpolationFilter.CalculateInterpolatedValues(this.Samples);
        this.OnPropertyChanged("Interpolation");
      }

      // Refresh DataBinding to DataGrid.
      this.OnPropertyChanged("Samples");
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

    private static double GetDistance(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double distance = Math.Sqrt(Math.Pow(newSample.PositionY - previousSample.PositionY, 2) + Math.Pow(newSample.PositionX - previousSample.PositionX, 2));
      return distance;
    }

    private static double GetXDistance(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double distance = Math.Abs(newSample.PositionX - previousSample.PositionX);
      return distance;
    }

    private static double GetYDistance(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double distance = Math.Abs(newSample.PositionY - previousSample.PositionY);
      return distance;
    }

    private static double GetLength(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double length = previousSample.Length + newSample.Distance;
      return length;
    }

    private static double GetXLength(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double lengthX = previousSample.LengthX + newSample.DistanceX;
      return lengthX;
    }

    private static double GetYLength(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double lengthY = previousSample.LengthY + newSample.DistanceY;
      return lengthY;
    }

    private static double GetVelocity(int objectIndex, DataSample newSample, long timedifference)
    {
      double velocity = newSample.Distance / timedifference * 1000;
      return velocity;
    }

    private static double GetXVelocity(int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      double xvelocity = (newSample.PositionX - previousSample.PositionX) / timedifference * 1000;
      return xvelocity;
    }

    private static double GetYVelocity(int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      return (newSample.PositionY - previousSample.PositionY) / timedifference * 1000;
    }

    private static double GetAcceleration(int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      double value = (newSample.Velocity.Value - previousSample.Velocity.Value) / timedifference * 1000;
      return value;
    }

    private static double GetXAcceleration(int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      return (newSample.VelocityX.Value - previousSample.VelocityX.Value) / timedifference * 1000;
    }

    private static double GetYAcceleration(int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      return (newSample.VelocityY.Value - previousSample.VelocityY.Value) / timedifference * 1000;
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
      this.LastPoint = new Point[Video.Instance.ImageProcessing.NumberOfTrackedObjects];
      this.OnPropertyChanged("Samples");
    }
  }
}
