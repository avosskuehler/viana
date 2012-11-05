// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoData.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2012 Dr. Adrian Voßkühler  
//   ------------------------------------------------------------------------
//   This program is free software; you can redistribute it and/or modify it 
//   under the terms of the GNU General Public License as published by the 
//   Free Software Foundation; either version 2 of the License, or 
//   (at your option) any later version.
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License 
//   along with this program; if not, write to the Free Software Foundation, 
//   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//   ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   The video data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data
{
  using System;
  using System.ComponentModel;
  using System.Windows;

  using VianaNET.Data.Collections;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The video data.
  /// </summary>
  public class VideoData : DependencyObject, INotifyPropertyChanged
  {
    #region Static Fields

    /// <summary>
    ///   The last point property.
    /// </summary>
    public static readonly DependencyProperty LastPointProperty = DependencyProperty.Register(
      "LastPoint", typeof(Point[]), typeof(VideoData), new UIPropertyMetadata(null));

    /// <summary>
    ///   The ActiveObject property.
    /// </summary>
    public static readonly DependencyProperty ActiveObjectProperty = DependencyProperty.Register(
      "ActiveObject", typeof(int), typeof(VideoData), new UIPropertyMetadata(null));

    /// <summary>
    ///   The samples property.
    /// </summary>
    public static readonly DependencyProperty SamplesProperty = DependencyProperty.Register(
      "Samples", typeof(DataCollection), typeof(VideoData), new UIPropertyMetadata(null));

    /// <summary>
    ///   The instance.
    /// </summary>
    private static VideoData instance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Prevents a default instance of the <see cref="VideoData" /> class from being created. 
    ///   Initializes a new instance of the DataStream class.
    /// </summary>
    private VideoData()
    {
      this.Samples = new DataCollection();
      this.LastPoint = new Point[Video.Instance.ProcessingData.NumberOfTrackedObjects];
      this.ActiveObject = 0;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the <see cref="VideoData" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static VideoData Instance
    {
      get
      {
        // check again, if the underlying instance is null
        // return the existing/new instance
        return instance ?? (instance = new VideoData());
      }
    }

    /// <summary>
    ///   Gets the count.
    /// </summary>
    public int Count
    {
      get
      {
        return this.Samples.Count;
      }
    }

    /// <summary>
    ///   Gets or sets the last point.
    /// </summary>
    public Point[] LastPoint
    {
      get
      {
        return (Point[])this.GetValue(LastPointProperty);
      }

      set
      {
        this.SetValue(LastPointProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the active object. This is zero-based.
    /// </summary>
    public int ActiveObject
    {
      get
      {
        return (int)this.GetValue(ActiveObjectProperty);
      }

      set
      {
        this.SetValue(ActiveObjectProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the samples.
    /// </summary>
    public DataCollection Samples
    {
      get
      {
        return (DataCollection)this.GetValue(SamplesProperty);
      }

      set
      {
        this.SetValue(SamplesProperty, value);
      }
    }

    /// <summary>
    /// Gets a value indicating whether there is at least one data sample
    /// </summary>
    public bool HasSamples
    {
      get
      {
        return this.Count > 0;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The add point.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSamplePosition">
    /// The new sample position. 
    /// </param>
    public void AddPoint(int objectIndex, Point newSamplePosition)
    {
      this.LastPoint[objectIndex] = newSamplePosition;

      var timeSample = new TimeSample
        {
          Framenumber = Video.Instance.FrameIndex,
          Timestamp = Video.Instance.FrameTimestampInMS
        };

      var newObjectSample = new DataSample
      {
        PixelX = newSamplePosition.X,
        PixelY = newSamplePosition.Y
      };

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

    /// <summary>
    ///   The refresh distance velocity acceleration.
    /// </summary>
    public void RefreshDistanceVelocityAcceleration()
    {
      var previousSamples = new TimeSample[Video.Instance.ProcessingData.NumberOfTrackedObjects];
      var validSamples = new int[Video.Instance.ProcessingData.NumberOfTrackedObjects];

      foreach (TimeSample timeSample in this.Samples)
      {
        for (int j = 0; j < Video.Instance.ProcessingData.NumberOfTrackedObjects; j++)
        {
          DataSample currentSample = timeSample.Object[j];
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
            previousSamples[j] = timeSample;
            continue;
          }

          DataSample previousSample = previousSamples[j].Object[j];
          long timeTifference = timeSample.Timestamp - previousSamples[j].Timestamp;
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
            previousSamples[j] = timeSample;
            continue;
          }

          currentSample.Acceleration = GetAcceleration(j, currentSample, previousSample, timeTifference);
          currentSample.AccelerationX = GetXAcceleration(j, currentSample, previousSample, timeTifference);
          currentSample.AccelerationY = GetYAcceleration(j, currentSample, previousSample, timeTifference);

          previousSamples[j] = timeSample;
        }
      }

      // Refresh DataBinding to DataGrid.
      this.OnPropertyChanged("Samples");
    }

    /// <summary>
    /// The remove point.
    /// </summary>
    /// <param name="timeStamp">
    /// The time stamp. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool RemovePoint(long timeStamp)
    {
      bool success = this.Samples.Remove(timeStamp);
      this.OnPropertyChanged("Samples");
      return success;
    }

    /// <summary>
    ///   The reset.
    /// </summary>
    public void Reset()
    {
      this.Samples.Clear();
      this.LastPoint = new Point[Video.Instance.ProcessingData.NumberOfTrackedObjects];
      this.OnPropertyChanged("Samples");
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name. 
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// The calibrate sample.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="Point"/> . 
    /// </returns>
    private static Point CalibrateSample(DataSample value)
    {
      if (!CalibrationData.Instance.IsVideoCalibrated)
      {
        return new Point(value.PixelX, value.PixelY);
      }

      var calibratedPoint = new Point(value.PixelX, value.PixelY);
      calibratedPoint.Offset(-CalibrationData.Instance.OriginInPixel.X, -CalibrationData.Instance.OriginInPixel.Y);
      calibratedPoint.X = calibratedPoint.X * CalibrationData.Instance.ScalePixelToUnit;
      calibratedPoint.Y = calibratedPoint.Y * CalibrationData.Instance.ScalePixelToUnit;

      return calibratedPoint;
    }

    /// <summary>
    /// The get acceleration.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <param name="timedifference">
    /// The timedifference. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetAcceleration(
      int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      double value = (newSample.Velocity.Value - previousSample.Velocity.Value) / timedifference * 1000;
      return value;
    }

    /// <summary>
    /// The get distance.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetDistance(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double distance =
        Math.Sqrt(
          Math.Pow(newSample.PositionY - previousSample.PositionY, 2)
          + Math.Pow(newSample.PositionX - previousSample.PositionX, 2));
      return distance;
    }

    /// <summary>
    /// The get length.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetLength(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double length = previousSample.Length + newSample.Distance;
      return length;
    }

    /// <summary>
    /// The get velocity.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="timedifference">
    /// The timedifference. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetVelocity(int objectIndex, DataSample newSample, long timedifference)
    {
      double velocity = newSample.Distance / timedifference * 1000;
      return velocity;
    }

    /// <summary>
    /// The get x acceleration.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <param name="timedifference">
    /// The timedifference. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetXAcceleration(
      int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      return (newSample.VelocityX.Value - previousSample.VelocityX.Value) / timedifference * 1000;
    }

    /// <summary>
    /// The get x distance.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetXDistance(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double distance = Math.Abs(newSample.PositionX - previousSample.PositionX);
      return distance;
    }

    /// <summary>
    /// The get x length.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetXLength(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double lengthX = previousSample.LengthX + newSample.DistanceX;
      return lengthX;
    }

    /// <summary>
    /// The get x velocity.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <param name="timedifference">
    /// The timedifference. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetXVelocity(
      int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      double xvelocity = (newSample.PositionX - previousSample.PositionX) / timedifference * 1000;
      return xvelocity;
    }

    /// <summary>
    /// The get y acceleration.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <param name="timedifference">
    /// The timedifference. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetYAcceleration(
      int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      return (newSample.VelocityY.Value - previousSample.VelocityY.Value) / timedifference * 1000;
    }

    /// <summary>
    /// The get y distance.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetYDistance(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double distance = Math.Abs(newSample.PositionY - previousSample.PositionY);
      return distance;
    }

    /// <summary>
    /// The get y length.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetYLength(int objectIndex, DataSample newSample, DataSample previousSample)
    {
      double lengthY = previousSample.LengthY + newSample.DistanceY;
      return lengthY;
    }

    /// <summary>
    /// The get y velocity.
    /// </summary>
    /// <param name="objectIndex">
    /// The object index. 
    /// </param>
    /// <param name="newSample">
    /// The new sample. 
    /// </param>
    /// <param name="previousSample">
    /// The previous sample. 
    /// </param>
    /// <param name="timedifference">
    /// The timedifference. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    private static double GetYVelocity(
      int objectIndex, DataSample newSample, DataSample previousSample, long timedifference)
    {
      return (newSample.PositionY - previousSample.PositionY) / timedifference * 1000;
    }

    #endregion
  }
}