// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoData.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Windows;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data.Collections;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The video data.
  /// </summary>
  [Serializable]
  public class VideoData : DependencyObject, INotifyPropertyChanged
  {
    #region Static Fields

    /// <summary>
    ///   The ActiveObject property.
    /// </summary>
    public static readonly DependencyProperty ActiveObjectProperty = DependencyProperty.Register(
      "ActiveObject",
      typeof(int),
      typeof(VideoData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The Filtered samples property.
    /// </summary>
    public static readonly DependencyProperty FilteredSamplesProperty = DependencyProperty.Register(
      "FilteredSamples",
      typeof(DataCollection),
      typeof(VideoData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The selection start property.
    /// </summary>
    public static readonly DependencyProperty FramerateFactorProperty = DependencyProperty.Register(
      "FramerateFactor",
      typeof(double),
      typeof(VideoData));

    /// <summary>
    ///   The last point property.
    /// </summary>
    public static readonly DependencyProperty LastPointProperty = DependencyProperty.Register(
      "LastPoint",
      typeof(Point[]),
      typeof(VideoData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The samples property.
    /// </summary>
    public static readonly DependencyProperty SamplesProperty = DependencyProperty.Register(
      "Samples",
      typeof(DataCollection),
      typeof(VideoData),
      new UIPropertyMetadata(null));

    /// <summary>
    ///   The selection end property.
    /// </summary>
    public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register(
      "SelectionEnd",
      typeof(double),
      typeof(VideoData));

    /// <summary>
    ///   The selection start property.
    /// </summary>
    public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
      "SelectionStart",
      typeof(double),
      typeof(VideoData));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="UseEveryNthPoint" />.
    /// </summary>
    public static readonly DependencyProperty UseEveryNthPointProperty = DependencyProperty.Register(
      "UseEveryNthPoint",
      typeof(int),
      typeof(VideoData),
      new UIPropertyMetadata(1));

    #endregion

    #region Fields

    /// <summary>
    ///   The valid data samples for each object
    /// </summary>
    private List<TimeSample>[] validDataSamples;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="VideoData" /> class.
    /// </summary>
    public VideoData()
    {
      this.FilteredSamples = new DataCollection();
      this.Samples = new DataCollection();
      this.ActiveObject = 0;
      this.FramerateFactor = 1;
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
    ///   Gets or sets the filtered sample collection containing the detected samples
    ///   downsampled by the <see cref="UseEveryNthPoint" /> value.
    /// </summary>
    public DataCollection FilteredSamples
    {
      get
      {
        return (DataCollection)this.GetValue(FilteredSamplesProperty);
      }

      set
      {
        this.SetValue(FilteredSamplesProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the frame rate of the video
    ///   This can be automatically set or manually defined.
    /// </summary>
    public double FramerateFactor
    {
      get
      {
        return (double)this.GetValue(FramerateFactorProperty);
      }

      set
      {
        this.SetValue(FramerateFactorProperty, value);
      }
    }

    /// <summary>
    ///   Gets a value indicating whether there is at least one data sample
    /// </summary>
    public bool HasSamples
    {
      get
      {
        return this.Samples.Count > 0;
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
    ///   Gets or sets the complete sample collection containing all originally detected samples.
    ///   It can be filtered (downsampled) by the <see cref="UseEveryNthPoint" /> value
    ///   to fill the <see cref="FilteredSamples" /> collection.
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

        // this.FilterSamples();
        // this.OnPropertyChanged("Samples");
      }
    }

    /// <summary>
    ///   Gets or sets the ending position of the video selection
    ///   in milliseconds
    /// </summary>
    public double SelectionEnd
    {
      get
      {
        return (double)this.GetValue(SelectionEndProperty);
      }

      set
      {
        this.SetValue(SelectionEndProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the starting position of the video selection
    ///   in milliseconds
    /// </summary>
    public double SelectionStart
    {
      get
      {
        return (double)this.GetValue(SelectionStartProperty);
      }

      set
      {
        this.SetValue(SelectionStartProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the time position in milliseconds, where
    ///   the video time should be zero.
    /// </summary>
    public long TimeZeroPositionInMs { get; set; }

    /// <summary>
    ///   Gets or sets the skip point count.
    ///   This value indicates how many samples the video transporter should
    ///   advance or go back on one frame step.
    ///   This is a kind of downsampling
    /// </summary>
    public int UseEveryNthPoint
    {
      get
      {
        return (int)this.GetValue(UseEveryNthPointProperty);
      }

      set
      {
        this.SetValue(UseEveryNthPointProperty, value);
        this.FilterSamples();
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
    public void AddPoint(int objectIndex, Point? newSamplePosition)
    {
      if (newSamplePosition.HasValue)
      {
        this.LastPoint[objectIndex] = newSamplePosition.Value;
      }

      var timeSample = new TimeSample
                         {
                           Framenumber = Video.Instance.FrameIndex,
                           Timestamp = Video.Instance.FrameTimestampInMs,
                           IsSelected = true
                         };

      double newTime = Video.Instance.FrameTimestampInMs;
      switch (Viana.Project.CalibrationData.TimeUnit)
      {
        case TimeUnit.ms:
          break;
        case TimeUnit.s:
          newTime = newTime / 1000;
          break;
        default:
          throw new ArgumentOutOfRangeException("TimeUnit not given");
      }

      DataSample newObjectSample = null;
      if (newSamplePosition.HasValue)
      {
        newObjectSample = new DataSample
                            {
                              Time = newTime,
                              PixelX = newSamplePosition.Value.X,
                              PixelY = newSamplePosition.Value.Y
                            };
      }

      // Add new point
      int index;
      if (this.Samples.Contains(timeSample, out index))
      {
        if (newObjectSample != null)
        {
          if (this.Samples[index].Object == null)
          {
            this.Samples[index].Object = new DataSample[Viana.Project.ProcessingData.NumberOfTrackedObjects];
          }

          this.Samples[index].Object[objectIndex] = newObjectSample;
        }
      }
      else
      {
        if (newObjectSample != null)
        {
          if (timeSample.Object == null)
          {
            timeSample.Object = new DataSample[Viana.Project.ProcessingData.NumberOfTrackedObjects];
          }

          timeSample.Object[objectIndex] = newObjectSample;
        }

        this.Samples.Add(timeSample);
      }

      this.OnPropertyChanged("LastPoint");
    }

    /// <summary>
    ///   Notifies the loading.
    /// </summary>
    public void NotifyLoading()
    {
      // Recalculate dependent data values
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();

      // Update dependencies
      this.OnPropertyChanged("Samples");
    }

    /// <summary>
    ///   The refresh distance velocity acceleration.
    /// </summary>
    public void RefreshDistanceVelocityAcceleration()
    {
      this.GeneratePositionDistanceLength();

      switch (Viana.Project.ProcessingData.DifferenceQuotientType)
      {
        case DifferenceQuotientType.Forward:
          this.CalculateWithForwardDifference();
          break;
        case DifferenceQuotientType.Backward:
          this.CalculateWithBackwardDifference();
          break;
        case DifferenceQuotientType.Central:
          this.CalculateWithCentralDifference();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      // Refresh DataBinding to DataGrid.
      this.OnPropertyChanged("Samples");
      this.FilterSamples();
    }

    /// <summary>
    /// Removes the point.
    /// </summary>
    /// <param name="timeStamp">
    /// The time stamp.
    /// </param>
    /// <returns>
    /// True if succesfull
    /// </returns>
    public bool RemovePoint(long timeStamp)
    {
      bool success = this.Samples.Remove(timeStamp);
      this.OnPropertyChanged("Samples");
      return success;
    }

    /// <summary>
    ///   Resets this instance.
    /// </summary>
    public void Reset()
    {
      this.Samples.Clear();
      this.FilteredSamples.Clear();
      this.LastPoint = new Point[Viana.Project.ProcessingData.NumberOfTrackedObjects];
      this.OnPropertyChanged("Samples");
    }

    /// <summary>
    /// Updates the point at given frameindex and object index with
    ///   new location.
    /// </summary>
    /// <param name="frameIndex">
    /// Index of the frame.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <param name="newLocation">
    /// The new location.
    /// </param>
    public void UpdatePoint(int frameIndex, int objectIndex, Point newLocation)
    {
      TimeSample sample = this.Samples.GetSampleByFrameindex(frameIndex);
      if (sample != null)
      {
        sample.Object[objectIndex].PixelX = newLocation.X;
        sample.Object[objectIndex].PixelY = newLocation.Y;
      }
      else
      {
        // Add point if not already detected.
        this.AddPoint(objectIndex, newLocation);
      }
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
      if (!Viana.Project.CalibrationData.IsVideoCalibrated)
      {
        return new Point(value.PixelX, value.PixelY);
      }

      var calibratedPoint = new Point(value.PixelX, value.PixelY);
      calibratedPoint.Offset(
        -Viana.Project.CalibrationData.OriginInPixel.X,
        -Viana.Project.CalibrationData.OriginInPixel.Y);
      calibratedPoint.X = calibratedPoint.X * Viana.Project.CalibrationData.ScalePixelToUnit;
      calibratedPoint.Y = calibratedPoint.Y * Viana.Project.CalibrationData.ScalePixelToUnit;

      return calibratedPoint;
    }

    /// <summary>
    /// Gets the acceleration.
    ///   returns [Velocity(nextSample) - Velocity(currentSample)]/dt
    /// </summary>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [Velocity(nextSample) - Velocity(currentSample)]/dt
    /// </returns>
    private static double? GetAcceleration(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // a(t) = [v(t + dt) - v(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].Velocity - currentSample.Object[objectIndex].Velocity) / dt
             * GetTimeFactor();
    }

    /// <summary>
    /// Gets the acceleration for central difference
    ///   returns [Velocity(nextSample) - Velocity(previousSample)]/2dt
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [Velocity(nextSample) - Velocity(previousSample)]/2dt
    /// </returns>
    private static double? GetAcceleration(
      TimeSample previousSample,
      TimeSample currentSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // a(t) = [v(t+dt) - v(t-dt)]/2dt
      long timeTifference1 = currentSample.Timestamp - previousSample.Timestamp;
      long timeTifference2 = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].Velocity - previousSample.Object[objectIndex].Velocity)
             / (timeTifference1 + timeTifference2) * GetTimeFactor();
    }

    /// <summary>
    /// The get distance.
    /// </summary>
    /// <param name="newSample">
    /// The new sample.
    /// </param>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> .
    /// </returns>
    private static double GetDistance(DataSample newSample, DataSample previousSample)
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
    /// <param name="newSample">
    /// The new sample.
    /// </param>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> .
    /// </returns>
    private static double GetLength(DataSample newSample, DataSample previousSample)
    {
      double length = previousSample.Length.Value + newSample.Distance.Value;
      return length;
    }

    /// <summary>
    ///   Returns the factor to convert sample with milliseconds in the current time unit;
    /// </summary>
    /// <returns>The factor to multiply with</returns>
    private static int GetTimeFactor()
    {
      int timefactor;
      switch (Viana.Project.CalibrationData.TimeUnit)
      {
        case TimeUnit.ms:
          timefactor = 1;
          break;
        case TimeUnit.s:
          timefactor = 1000;
          break;
        default:
          throw new ArgumentOutOfRangeException("TimeUnit not specified");
      }

      return timefactor;
    }

    /// <summary>
    /// Gets the velocity.
    ///   returns [Distance(nextSample) - Distance(currentSample)]/dt
    /// </summary>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [Distance(nextSample) - Distance(currentSample)]/dt
    /// </returns>
    private static double? GetVelocity(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].Distance - currentSample.Object[objectIndex].Distance) / dt
             * GetTimeFactor();
    }

    /// <summary>
    /// Gets the velocity for central difference
    ///   returns [Distance(nextSample) - Distance(previousSample)]/2dt
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [Distance(nextSample) - Distance(previousSample)]/2dt
    /// </returns>
    private static double? GetVelocity(
      TimeSample previousSample,
      TimeSample currentSample,
      TimeSample nextSample,
      int objectIndex)
    {
      long timeTifference1 = currentSample.Timestamp - previousSample.Timestamp;
      long timeTifference2 = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].Distance - previousSample.Object[objectIndex].Distance)
             / (timeTifference1 + timeTifference2) * GetTimeFactor();
    }

    /// <summary>
    /// Gets the x acceleration.
    ///   returns [VelocityX(nextSample) - VelocityX(currentSample)]/dt
    /// </summary>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [VelocityX(nextSample) - VelocityX(currentSample)]/dt
    /// </returns>
    private static double? GetXAcceleration(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // a(t) = [v(t + dt) - v(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].VelocityX - currentSample.Object[objectIndex].VelocityX) / dt
             * GetTimeFactor();
    }

    /// <summary>
    /// Gets the  x acceleration for central difference
    ///   returns [VelocityX(nextSample) - VelocityX(previousSample)]/2dt
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [VelocityX(nextSample) - VelocityX(previousSample)]/2dt
    /// </returns>
    private static double? GetXAcceleration(
      TimeSample previousSample,
      TimeSample currentSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // a(t) = [v(t+dt) - v(t-dt)]/2dt
      long timeTifference1 = currentSample.Timestamp - previousSample.Timestamp;
      long timeTifference2 = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].VelocityX - previousSample.Object[objectIndex].VelocityX)
             / (timeTifference1 + timeTifference2) * GetTimeFactor();
    }

    /// <summary>
    /// The get x distance.
    /// </summary>
    /// <param name="newSample">
    /// The new sample.
    /// </param>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> .
    /// </returns>
    private static double GetXDistance(DataSample newSample, DataSample previousSample)
    {
      double distance = newSample.PositionX - previousSample.PositionX;
      return distance;
    }

    /// <summary>
    /// The get x length.
    /// </summary>
    /// <param name="newSample">
    /// The new sample.
    /// </param>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> .
    /// </returns>
    private static double GetXLength(DataSample newSample, DataSample previousSample)
    {
      double lengthX = previousSample.LengthX.Value + newSample.DistanceX.Value;
      return lengthX;
    }

    /// <summary>
    /// Gets the X velocity.
    ///   returns [DistanceX(nextSample) - DistanceX(currentSample)]/dt
    /// </summary>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [DistanceX(nextSample) - DistanceX(currentSample)]/dt
    /// </returns>
    private static double? GetXVelocity(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].DistanceX - currentSample.Object[objectIndex].DistanceX) / dt
             * GetTimeFactor();
    }

    /// <summary>
    /// Gets the x velocity for central difference
    ///   returns [DistanceX(nextSample) - DistanceX(previousSample)]/2dt
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [DistanceX(nextSample) - DistanceX(previousSample)]/2dt
    /// </returns>
    private static double? GetXVelocity(
      TimeSample previousSample,
      TimeSample currentSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // v(t) = [s(t+dt) - s(t-dt)]/2dt
      long timeTifference1 = currentSample.Timestamp - previousSample.Timestamp;
      long timeTifference2 = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].DistanceX - previousSample.Object[objectIndex].DistanceX)
             / (timeTifference1 + timeTifference2) * GetTimeFactor();
    }

    /// <summary>
    /// Gets the y acceleration.
    ///   returns [VelocityY(nextSample) - VelocityY(currentSample)]/dt
    /// </summary>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [VelocityY(nextSample) - VelocityY(currentSample)]/dt
    /// </returns>
    private static double? GetYAcceleration(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // a(t) = [v(t + dt) - v(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].VelocityY - currentSample.Object[objectIndex].VelocityY) / dt
             * GetTimeFactor();
    }

    /// <summary>
    /// Gets the y acceleration for central difference
    ///   returns [VelocityY(nextSample) - VelocityY(previousSample)]/2dt
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [VelocityY(nextSample) - VelocityY(previousSample)]/2dt
    /// </returns>
    private static double? GetYAcceleration(
      TimeSample previousSample,
      TimeSample currentSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // a(t) = [v(t+dt) - v(t-dt)]/2dt
      long timeTifference1 = currentSample.Timestamp - previousSample.Timestamp;
      long timeTifference2 = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].VelocityY - previousSample.Object[objectIndex].VelocityY)
             / (timeTifference1 + timeTifference2) * GetTimeFactor();
    }

    /// <summary>
    /// The get y distance.
    /// </summary>
    /// <param name="newSample">
    /// The new sample.
    /// </param>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> .
    /// </returns>
    private static double GetYDistance(DataSample newSample, DataSample previousSample)
    {
      double distance = newSample.PositionY - previousSample.PositionY;
      return distance;
    }

    /// <summary>
    /// The get y length.
    /// </summary>
    /// <param name="newSample">
    /// The new sample.
    /// </param>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> .
    /// </returns>
    private static double GetYLength(DataSample newSample, DataSample previousSample)
    {
      double lengthY = previousSample.LengthY.Value + newSample.DistanceY.Value;
      return lengthY;
    }

    /// <summary>
    /// Gets the Y velocity.
    ///   returns [DistanceY(nextSample) - DistanceY(currentSample)]/dt
    /// </summary>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [DistanceY(nextSample) - DistanceY(currentSample)]/dt
    /// </returns>
    private static double? GetYVelocity(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // v(t) = [s(t + dt) - s(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].DistanceY - currentSample.Object[objectIndex].DistanceY) / dt
             * GetTimeFactor();
    }

    /// <summary>
    /// Gets the y velocity for central difference
    ///   returns [DistanceY(nextSample) - DistanceY(previousSample)]/2dt
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="currentSample">
    /// The current sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>
    /// [DistanceY(nextSample) - DistanceY(previousSample)]/2dt
    /// </returns>
    private static double? GetYVelocity(
      TimeSample previousSample,
      TimeSample currentSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // v(t) = [s(t+dt) - s(t-dt)]/2dt
      long timeTifference1 = currentSample.Timestamp - previousSample.Timestamp;
      long timeTifference2 = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].DistanceY - previousSample.Object[objectIndex].DistanceY)
             / (timeTifference1 + timeTifference2) * GetTimeFactor();
    }

    /// <summary>
    ///   Calculates velocity and acceleration with backward difference.
    ///   Formula: v(t) = [s(t) - s(t-dt)]/dt
    ///   a(t) = [v(t) - v(t-dt)]/dt
    /// </summary>
    private void CalculateWithBackwardDifference()
    {
      for (var j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        for (var i = 1; i < this.validDataSamples[j].Count; i++)
        {
          var currentSample = this.validDataSamples[j][i];
          var previousSample = this.validDataSamples[j][i - 1];

          currentSample.Object[j].Velocity = GetVelocity(previousSample, currentSample, j);
          currentSample.Object[j].VelocityX = GetXVelocity(previousSample, currentSample, j);
          currentSample.Object[j].VelocityY = GetYVelocity(previousSample, currentSample, j);
        }

        for (var i = 1; i < this.validDataSamples[j].Count; i++)
        {
          var currentSample = this.validDataSamples[j][i];
          var previousSample = this.validDataSamples[j][i - 1];

          currentSample.Object[j].Acceleration = GetAcceleration(previousSample, currentSample, j);
          currentSample.Object[j].AccelerationX = GetXAcceleration(previousSample, currentSample, j);
          currentSample.Object[j].AccelerationY = GetYAcceleration(previousSample, currentSample, j);
        }
      }
    }

    /// <summary>
    ///   Calculates velocity and acceleration with central difference.
    ///   first point formula v(t) = [s(t+dt) - s(t)]/dt
    ///   other points formula v(t) = [s(t+dt) - s(t-dt)]/2dt
    ///   first point formula a(t) = [v(t+dt) - v(t)]/dt
    ///   other points formula a(t) = [v(t+dt) - v(t-dt)]/2dt
    /// </summary>
    private void CalculateWithCentralDifference()
    {
      for (var j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        for (var i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          var currentSample = this.validDataSamples[j][i];
          var nextSample = this.validDataSamples[j][i + 1];

          // Calculate velocity except last point
          if (i == 0)
          {
            currentSample.Object[j].Velocity = GetVelocity(currentSample, nextSample, j);
            currentSample.Object[j].VelocityX = GetXVelocity(currentSample, nextSample, j);
            currentSample.Object[j].VelocityY = GetYVelocity(currentSample, nextSample, j);
          }
          else if (i < this.Samples.Count - 1)
          {
            TimeSample previousSample = this.validDataSamples[j][i - 1];
            currentSample.Object[j].Velocity = GetVelocity(previousSample, currentSample, nextSample, j);
            currentSample.Object[j].VelocityX = GetXVelocity(previousSample, currentSample, nextSample, j);
            currentSample.Object[j].VelocityY = GetYVelocity(previousSample, currentSample, nextSample, j);
          }
        }

        // Calculate acceleration
        for (var i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          var currentSample = this.validDataSamples[j][i];
          var nextSample = this.validDataSamples[j][i + 1];

          // Calculate acceleration except last point
          if (i == 0)
          {
            currentSample.Object[j].Acceleration = GetAcceleration(currentSample, nextSample, j);
            currentSample.Object[j].AccelerationX = GetXAcceleration(currentSample, nextSample, j);
            currentSample.Object[j].AccelerationY = GetYAcceleration(currentSample, nextSample, j);
          }
          else if (i < this.Samples.Count - 1)
          {
            TimeSample previousSample = this.validDataSamples[j][i - 1];
            currentSample.Object[j].Acceleration = GetAcceleration(previousSample, currentSample, nextSample, j);
            currentSample.Object[j].AccelerationX = GetXAcceleration(previousSample, currentSample, nextSample, j);
            currentSample.Object[j].AccelerationY = GetYAcceleration(previousSample, currentSample, nextSample, j);
          }
        }
      }
    }

    /// <summary>
    ///   Calculates velocity and acceleration with forward difference.
    ///   Formula: v(t) = [s(t+dt) - s(t)]/dt
    ///   a(t) = [v(t+dt) - v(t)]/dt
    /// </summary>
    private void CalculateWithForwardDifference()
    {
      for (var j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        for (var i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          currentSample.Object[j].Velocity = GetVelocity(currentSample, nextSample, j);
          currentSample.Object[j].VelocityX = GetXVelocity(currentSample, nextSample, j);
          currentSample.Object[j].VelocityY = GetYVelocity(currentSample, nextSample, j);
        }

        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          currentSample.Object[j].Acceleration = GetAcceleration(currentSample, nextSample, j);
          currentSample.Object[j].AccelerationX = GetXAcceleration(currentSample, nextSample, j);
          currentSample.Object[j].AccelerationY = GetYAcceleration(currentSample, nextSample, j);
        }
      }
    }

    /// <summary>
    ///   Filters the samples by using only every <see cref="UseEveryNthPoint" />s sample.
    /// </summary>
    private void FilterSamples()
    {
      this.FilteredSamples.Clear();
      for (int i = 1; i <= this.Samples.Count; i++)
      {
        if (i % this.UseEveryNthPoint == 0)
        {
          TimeSample sampleToAdd = this.Samples[i - 1];
          if (sampleToAdd.Object != null)
          {
            this.FilteredSamples.Add(sampleToAdd);
          }
        }
      }

      // Updates the chart plot
      Viana.Project.RequestChartUpdate();

      // Update views
      this.OnPropertyChanged("FilteredSamples");
      this.OnPropertyChanged("HasSamples");
      this.OnPropertyChanged("UseEveryNthPoint");
    }

    /// <summary>
    ///   Generates the position, distance and length values.
    ///   Starting by adding the distance between point 1 and point 2 to point 1.
    ///   The last point has no length and distance values.
    /// </summary>
    private void GeneratePositionDistanceLength()
    {
      this.validDataSamples = new List<TimeSample>[Viana.Project.ProcessingData.NumberOfTrackedObjects];
      for (var i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.validDataSamples[i] = new List<TimeSample>();
      }

      foreach (var timeSample in this.Samples)
      {
        for (var j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
        {
          if (timeSample.Object == null)
          {
            continue;
          }

          var currentSample = timeSample.Object[j];
          if (currentSample == null)
          {
            continue;
          }

          this.validDataSamples[j].Add(timeSample);

          var calibratedPoint = CalibrateSample(currentSample);
          currentSample.Time = (double)timeSample.Timestamp / GetTimeFactor();
          currentSample.PositionX = calibratedPoint.X;
          currentSample.PositionY = calibratedPoint.Y;
        }
      }

      for (var j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        for (var i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          var currentSample = this.validDataSamples[j][i].Object[j];
          var nextSample = this.validDataSamples[j][i + 1].Object[j];

          // Calculate distance and length except last point
          if (i == 0)
          {
            currentSample.Distance = GetDistance(nextSample, currentSample);
            currentSample.DistanceX = GetXDistance(nextSample, currentSample);
            currentSample.DistanceY = GetYDistance(nextSample, currentSample);
            currentSample.Length = currentSample.Distance;
            currentSample.LengthX = currentSample.DistanceX;
            currentSample.LengthY = currentSample.DistanceY;
          }
          else if (i < this.Samples.Count - 1)
          {
            var previousSample = this.validDataSamples[j][i - 1].Object[j];
            currentSample.Distance = GetDistance(nextSample, currentSample);
            currentSample.DistanceX = GetXDistance(nextSample, currentSample);
            currentSample.DistanceY = GetYDistance(nextSample, currentSample);
            currentSample.Length = GetLength(currentSample, previousSample);
            currentSample.LengthX = GetXLength(currentSample, previousSample);
            currentSample.LengthY = GetYLength(currentSample, previousSample);
          }
        }
      }
    }

    #endregion
  }
}