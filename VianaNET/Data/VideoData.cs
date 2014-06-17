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
  using System.Linq;
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
      this.FramerateFactor = 1;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///   Occurs when the time sample selection changed.
    /// </summary>
    public event EventHandler SelectionChanged;

    #endregion

    #region Public Properties

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
        Point origin = Viana.Project.CalibrationData.OriginInPixel;
        Point transformedPoint = newSamplePosition.Value;
        transformedPoint.Offset(-origin.X, -origin.Y);
        transformedPoint = Viana.Project.CalibrationData.CoordinateTransform.Transform(transformedPoint);
        newObjectSample = new DataSample { Time = newTime, PixelX = transformedPoint.X, PixelY = transformedPoint.Y };
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
    ///   Deletes all samples that are in the selected state.
    /// </summary>
    public void DeleteSelectedSamples()
    {
      IEnumerable<TimeSample> selectedItems = this.Samples.Where(timesample => timesample.IsSelected);
      IList<TimeSample> timeSamples = selectedItems as IList<TimeSample> ?? selectedItems.ToList();
      if (timeSamples.Count() >= this.Samples.Count)
      {
        return;
      }

      foreach (TimeSample selectedItem in timeSamples)
      {
        this.Samples.Remove(selectedItem);
      }

      foreach (TimeSample sample in this.Samples)
      {
        sample.IsSelected = true;
      }

      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();

      this.OnSelectionChanged();
    }

    /// <summary>
    ///   Filters the samples by using only every <see cref="UseEveryNthPoint" />s sample.
    /// </summary>
    public void FilterSamples()
    {
      this.FilteredSamples.Clear();
      for (int i = 1; i <= this.Samples.Count; i++)
      {
        if (i % this.UseEveryNthPoint == 0)
        {
          TimeSample sampleToAdd = this.Samples[i - 1];
          if (sampleToAdd.Object != null && sampleToAdd.Object[Viana.Project.ProcessingData.IndexOfObject] != null)
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
    ///   Called when selection changed.
    /// </summary>
    public void OnSelectionChanged()
    {
      if (this.SelectionChanged != null)
      {
        this.SelectionChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    ///   The refresh distance velocity acceleration.
    /// </summary>
    public void RefreshDistanceVelocityAcceleration()
    {
      this.GeneratePosition();

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
        Point origin = Viana.Project.CalibrationData.OriginInPixel;
        Point transformedPoint = newLocation;
        transformedPoint.Offset(-origin.X, -origin.Y);
        transformedPoint = Viana.Project.CalibrationData.CoordinateTransform.Transform(transformedPoint);
        sample.Object[objectIndex].PixelX = transformedPoint.X;
        sample.Object[objectIndex].PixelY = transformedPoint.Y;
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

      // calibratedPoint.Offset(
      // -Viana.Project.CalibrationData.OriginInPixel.X,
      // -Viana.Project.CalibrationData.OriginInPixel.Y);
      calibratedPoint.X = calibratedPoint.X * Viana.Project.CalibrationData.ScalePixelToUnit;
      calibratedPoint.Y = calibratedPoint.Y * Viana.Project.CalibrationData.ScalePixelToUnit;

      return calibratedPoint;
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

    #region Distance

    /// <summary>
    /// Gets the distance.
    /// </summary>
    /// <param name="newSample">The new sample.</param>
    /// <param name="previousSample">The previous sample.</param>
    /// <returns>The pythagorean distance of the given points</returns>
    private static double GetSimpleDistance(DataSample newSample, DataSample previousSample)
    {
      double distance =
        Math.Sqrt(
          Math.Pow(newSample.PositionY - previousSample.PositionY, 2)
          + Math.Pow(newSample.PositionX - previousSample.PositionX, 2));
      return distance;
    }

    /// <summary>
    /// Gets the x distance.
    /// </summary>
    /// <param name="newSample">The new sample.</param>
    /// <param name="previousSample">The previous sample.</param>
    /// <returns>The distance of the x positions</returns>
    private static double GetSimpleXDistance(DataSample newSample, DataSample previousSample)
    {
      double distance = newSample.PositionX - previousSample.PositionX;
      return distance;
    }

    /// <summary>
    /// Gets the y distance.
    /// </summary>
    /// <param name="newSample">The new sample.</param>
    /// <param name="previousSample">The previous sample.</param>
    /// <returns>The distance of the y positions</returns>
    private static double GetSimpleYDistance(DataSample newSample, DataSample previousSample)
    {
      double distance = newSample.PositionY - previousSample.PositionY;
      return distance;
    }

    /// <summary>
    /// Gets the distance for central difference
    /// returns phythagorean distance between previouse and next point / 2
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns>Phythagorean distance between previouse and next point / 2 </returns>
    private static double? GetCentralDistance(
      TimeSample previousSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // d(t) = sqrt([PosX(t+dt) - PosX(t-dt)]^2+[PosY(t+dt) - PosY(t-dt)]^2)/2
      double distance =
        Math.Sqrt(
          Math.Pow(nextSample.Object[objectIndex].PositionY - previousSample.Object[objectIndex].PositionY, 2)
          + Math.Pow(nextSample.Object[objectIndex].PositionX - previousSample.Object[objectIndex].PositionX, 2));
      return distance / 2;
    }

    /// <summary>
    /// Gets the x distance for central difference
    /// returns  [PosX(t+dt) - PosX(t-dt)]/2
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns> [PosX(t+dt) - PosX(t-dt)]/2</returns>
    private static double? GetCentralXDistance(
      TimeSample previousSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // d(t) = [PosX(t+dt) - PosX(t-dt)]/2
      return (nextSample.Object[objectIndex].PositionX - previousSample.Object[objectIndex].PositionX) / 2;
    }

    /// <summary>
    /// Gets the y distance for central difference
    /// returns  [PosY(t+dt) - PosY(t-dt)]/2
    /// </summary>
    /// <param name="previousSample">
    /// The previous sample.
    /// </param>
    /// <param name="nextSample">
    /// The next sample.
    /// </param>
    /// <param name="objectIndex">
    /// Index of the object.
    /// </param>
    /// <returns> [PosY(t+dt) - PosY(t-dt)]/2</returns>
    private static double? GetCentralYDistance(
      TimeSample previousSample,
      TimeSample nextSample,
      int objectIndex)
    {
      // d(t) = [PosY(t+dt) - PosY(t-dt)]/2
      return (nextSample.Object[objectIndex].PositionY - previousSample.Object[objectIndex].PositionY) / 2;
    }

    #endregion

    #region Length

    /// <summary>
    /// Gets the length at new samples timestamp.
    /// </summary>
    /// <param name="newSample">The new sample.</param>
    /// <param name="previousSample">The previous sample.</param>
    /// <returns>The length which is the previous sample length plus the new samples distance.</returns>
    private static double GetLength(DataSample newSample, DataSample previousSample)
    {
      double length = previousSample.Length.Value + newSample.Distance.Value;
      return length;
    }

    /// <summary>
    /// Gets the length in x-Direction at new samples timestamp.
    /// </summary>
    /// <param name="newSample">The new sample.</param>
    /// <param name="previousSample">The previous sample.</param>
    /// <returns>The length in x-Direction which is the previous sample length plus the new samples distance.</returns>
    private static double GetXLength(DataSample newSample, DataSample previousSample)
    {
      double lengthX = previousSample.LengthX.Value + newSample.DistanceX.Value;
      return lengthX;
    }

    /// <summary>
    /// Gets the length in y-Direction at new samples timestamp.
    /// </summary>
    /// <param name="newSample">The new sample.</param>
    /// <param name="previousSample">The previous sample.</param>
    /// <returns>The length in y-Direction which is the previous sample length plus the new samples distance.</returns>
    private static double GetYLength(DataSample newSample, DataSample previousSample)
    {
      double lengthY = previousSample.LengthY.Value + newSample.DistanceY.Value;
      return lengthY;
    }

    #endregion

    #region Velocity

    /// <summary>
    /// Gets the velocity.
    ///   returns Distance(currentSample)/dt
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
    /// <returns>The Distance(currentSample)]/dt</returns>
    private static double? GetVelocity(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return currentSample.Object[objectIndex].Distance / dt * GetTimeFactor();
    }

    /// <summary>
    /// Gets the X velocity.
    ///   returns DistanceX(currentSample)/dt
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
      return currentSample.Object[objectIndex].DistanceX / dt * GetTimeFactor();
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
      return currentSample.Object[objectIndex].DistanceY / dt * GetTimeFactor();
    }


    ///// <summary>
    ///// Gets the velocity for central difference
    /////   returns [Distance(currentSample)]/dt
    ///// </summary>
    ///// <param name="previousSample">
    ///// The previous sample.
    ///// </param>
    ///// <param name="currentSample">
    ///// The current sample.
    ///// </param>
    ///// <param name="nextSample">
    ///// The next sample.
    ///// </param>
    ///// <param name="objectIndex">
    ///// Index of the object.
    ///// </param>
    ///// <returns>The [Distance(currentSample)]/dt
    ///// </returns>
    //private static double? GetVelocity(
    //  TimeSample previousSample,
    //  TimeSample currentSample,
    //  TimeSample nextSample,
    //  int objectIndex)
    //{
    //  long dt = nextSample.Timestamp - currentSample.Timestamp;
    //  return currentSample.Object[objectIndex].Distance / dt * GetTimeFactor();
    //}

    ///// <summary>
    ///// Gets the x velocity for central difference
    /////   returns [DistanceX(nextSample) - DistanceX(previousSample)]/2dt
    ///// </summary>
    ///// <param name="previousSample">
    ///// The previous sample.
    ///// </param>
    ///// <param name="currentSample">
    ///// The current sample.
    ///// </param>
    ///// <param name="nextSample">
    ///// The next sample.
    ///// </param>
    ///// <param name="objectIndex">
    ///// Index of the object.
    ///// </param>
    ///// <returns>
    ///// [DistanceX(nextSample) - DistanceX(previousSample)]/2dt
    ///// </returns>
    //private static double? GetXVelocity(
    //  TimeSample previousSample,
    //  TimeSample currentSample,
    //  TimeSample nextSample,
    //  int objectIndex)
    //{
    //  // v(t) = [s(t+dt) - s(t-dt)]/2dt
    //  long dt = nextSample.Timestamp - currentSample.Timestamp;
    //  return currentSample.Object[objectIndex].DistanceX / dt * GetTimeFactor();
    //}

    ///// <summary>
    ///// Gets the y velocity for central difference
    /////   returns [DistanceY(nextSample) - DistanceY(previousSample)]/2dt
    ///// </summary>
    ///// <param name="previousSample">
    ///// The previous sample.
    ///// </param>
    ///// <param name="currentSample">
    ///// The current sample.
    ///// </param>
    ///// <param name="nextSample">
    ///// The next sample.
    ///// </param>
    ///// <param name="objectIndex">
    ///// Index of the object.
    ///// </param>
    ///// <returns>
    ///// [DistanceY(nextSample) - DistanceY(previousSample)]/2dt
    ///// </returns>
    //private static double? GetYVelocity(
    //  TimeSample previousSample,
    //  TimeSample currentSample,
    //  TimeSample nextSample,
    //  int objectIndex)
    //{
    //  // v(t) = [s(t+dt) - s(t-dt)]/2dt
    //  long dt = nextSample.Timestamp - currentSample.Timestamp;
    //  return currentSample.Object[objectIndex].DistanceY / dt * GetTimeFactor();
    //}

    #endregion

    #region Acceleration

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
    private static double? GetSimpleAcceleration(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // a(t) = [v(t + dt) - v(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].Velocity - currentSample.Object[objectIndex].Velocity) / dt
             * GetTimeFactor();
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
    private static double? GetSimpleXAcceleration(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // a(t) = [v(t + dt) - v(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].VelocityX - currentSample.Object[objectIndex].VelocityX) / dt
             * GetTimeFactor();
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
    private static double? GetSimpleYAcceleration(TimeSample currentSample, TimeSample nextSample, int objectIndex)
    {
      // a(t) = [v(t + dt) - v(t)]/dt
      long dt = nextSample.Timestamp - currentSample.Timestamp;
      return (nextSample.Object[objectIndex].VelocityY - currentSample.Object[objectIndex].VelocityY) / dt
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
    private static double? GetCentralAcceleration(
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
    /// Gets the x acceleration for central difference
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
    private static double? GetCentralXAcceleration(
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
    private static double? GetCentralYAcceleration(
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

    #endregion

    /// <summary>
    ///   Calculates velocity and acceleration with backward difference.
    ///   Formula: v(t) = [s(t) - s(t-dt)]/dt
    ///   a(t) = [v(t) - v(t-dt)]/dt
    /// </summary>
    private void CalculateWithBackwardDifference()
    {
      for (int j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        // Calculate distance and length
        for (int i = 1; i < this.validDataSamples[j].Count; i++)
        {
          DataSample currentSample = this.validDataSamples[j][i].Object[j];
          DataSample previousSample = this.validDataSamples[j][i - 1].Object[j];

          // Calculate distance and length except first point
          if (i == 1)
          {
            currentSample.Distance = GetSimpleDistance(currentSample, previousSample);
            currentSample.DistanceX = GetSimpleXDistance(currentSample, previousSample);
            currentSample.DistanceY = GetSimpleYDistance(currentSample, previousSample);
            currentSample.Length = currentSample.Distance;
            currentSample.LengthX = currentSample.DistanceX;
            currentSample.LengthY = currentSample.DistanceY;
          }
          else if (i < this.Samples.Count)
          {
            currentSample.Distance = GetSimpleDistance(currentSample, previousSample);
            currentSample.DistanceX = GetSimpleXDistance(currentSample, previousSample);
            currentSample.DistanceY = GetSimpleYDistance(currentSample, previousSample);
            currentSample.Length = GetLength(currentSample, previousSample);
            currentSample.LengthX = GetXLength(currentSample, previousSample);
            currentSample.LengthY = GetYLength(currentSample, previousSample);
          }
        }

        // Calculate velocity
        for (int i = 1; i < this.validDataSamples[j].Count; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample previousSample = this.validDataSamples[j][i - 1];

          currentSample.Object[j].Velocity = -GetVelocity(currentSample, previousSample, j);
          currentSample.Object[j].VelocityX = -GetXVelocity(currentSample, previousSample, j);
          currentSample.Object[j].VelocityY = -GetYVelocity(currentSample, previousSample, j);
        }

        for (int i = 1; i < this.validDataSamples[j].Count; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample previousSample = this.validDataSamples[j][i - 1];

          currentSample.Object[j].Acceleration = GetSimpleAcceleration(previousSample, currentSample, j);
          currentSample.Object[j].AccelerationX = GetSimpleXAcceleration(previousSample, currentSample, j);
          currentSample.Object[j].AccelerationY = GetSimpleYAcceleration(previousSample, currentSample, j);
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
      for (int j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        // Calculate distance and length
        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          // Calculate distance and length except last point
          if (i == 0)
          {
            currentSample.Object[j].Distance = GetSimpleDistance(nextSample.Object[j], currentSample.Object[j]);
            currentSample.Object[j].DistanceX = GetSimpleXDistance(nextSample.Object[j], currentSample.Object[j]);
            currentSample.Object[j].DistanceY = GetSimpleYDistance(nextSample.Object[j], currentSample.Object[j]);
            currentSample.Object[j].Length = currentSample.Object[j].Distance;
            currentSample.Object[j].LengthX = currentSample.Object[j].DistanceX;
            currentSample.Object[j].LengthY = currentSample.Object[j].DistanceY;
          }
          else if (i < this.Samples.Count - 1)
          {
            TimeSample previousSample = this.validDataSamples[j][i - 1];
            currentSample.Object[j].Distance = GetCentralDistance(previousSample, nextSample, j);
            currentSample.Object[j].DistanceX = GetCentralXDistance(previousSample, nextSample, j);
            currentSample.Object[j].DistanceY = GetCentralYDistance(previousSample, nextSample, j);
            currentSample.Object[j].Length = GetLength(currentSample.Object[j], previousSample.Object[j]);
            currentSample.Object[j].LengthX = GetXLength(currentSample.Object[j], previousSample.Object[j]);
            currentSample.Object[j].LengthY = GetYLength(currentSample.Object[j], previousSample.Object[j]);
          }
        }

        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          // Calculate velocity except last point
          if (i == 0)
          {
            currentSample.Object[j].Velocity = GetVelocity(currentSample, nextSample, j);
            currentSample.Object[j].VelocityX = GetXVelocity(currentSample, nextSample, j);
            currentSample.Object[j].VelocityY = GetYVelocity(currentSample, nextSample, j);
          }
          else if (i < this.Samples.Count - 1)
          {
            currentSample.Object[j].Velocity = GetVelocity(currentSample, nextSample, j);
            currentSample.Object[j].VelocityX = GetXVelocity(currentSample, nextSample, j);
            currentSample.Object[j].VelocityY = GetYVelocity(currentSample, nextSample, j);
          }
        }

        // Calculate acceleration
        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          // Calculate acceleration except last point
          if (i == 0)
          {
            currentSample.Object[j].Acceleration = GetSimpleAcceleration(currentSample, nextSample, j);
            currentSample.Object[j].AccelerationX = GetSimpleXAcceleration(currentSample, nextSample, j);
            currentSample.Object[j].AccelerationY = GetSimpleYAcceleration(currentSample, nextSample, j);
          }
          else if (i < this.Samples.Count - 1)
          {
            TimeSample previousSample = this.validDataSamples[j][i - 1];
            currentSample.Object[j].Acceleration = GetCentralAcceleration(previousSample, currentSample, nextSample, j);
            currentSample.Object[j].AccelerationX = GetCentralXAcceleration(previousSample, currentSample, nextSample, j);
            currentSample.Object[j].AccelerationY = GetCentralYAcceleration(previousSample, currentSample, nextSample, j);
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
      for (int j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
      {
        // Calculate distance and length
        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          DataSample currentSample = this.validDataSamples[j][i].Object[j];
          DataSample nextSample = this.validDataSamples[j][i + 1].Object[j];

          // Calculate distance and length except last point
          if (i == 0)
          {
            currentSample.Distance = GetSimpleDistance(nextSample, currentSample);
            currentSample.DistanceX = GetSimpleXDistance(nextSample, currentSample);
            currentSample.DistanceY = GetSimpleYDistance(nextSample, currentSample);
            currentSample.Length = currentSample.Distance;
            currentSample.LengthX = currentSample.DistanceX;
            currentSample.LengthY = currentSample.DistanceY;
          }
          else if (i < this.Samples.Count - 1)
          {
            DataSample previousSample = this.validDataSamples[j][i - 1].Object[j];
            currentSample.Distance = GetSimpleDistance(nextSample, currentSample);
            currentSample.DistanceX = GetSimpleXDistance(nextSample, currentSample);
            currentSample.DistanceY = GetSimpleYDistance(nextSample, currentSample);
            currentSample.Length = GetLength(currentSample, previousSample);
            currentSample.LengthX = GetXLength(currentSample, previousSample);
            currentSample.LengthY = GetYLength(currentSample, previousSample);
          }
        }

        // Calculate Velocity
        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          currentSample.Object[j].Velocity = GetVelocity(currentSample, nextSample, j);
          currentSample.Object[j].VelocityX = GetXVelocity(currentSample, nextSample, j);
          currentSample.Object[j].VelocityY = GetYVelocity(currentSample, nextSample, j);
        }

        // Calculate Acceleration
        for (int i = 0; i < this.validDataSamples[j].Count - 1; i++)
        {
          TimeSample currentSample = this.validDataSamples[j][i];
          TimeSample nextSample = this.validDataSamples[j][i + 1];

          currentSample.Object[j].Acceleration = GetSimpleAcceleration(currentSample, nextSample, j);
          currentSample.Object[j].AccelerationX = GetSimpleXAcceleration(currentSample, nextSample, j);
          currentSample.Object[j].AccelerationY = GetSimpleYAcceleration(currentSample, nextSample, j);
        }
      }
    }

    /// <summary>
    ///   Generates the position, distance and length values.
    ///   Starting by adding the distance between point 1 and point 2 to point 1.
    ///   The last point has no length and distance values.
    /// </summary>
    private void GeneratePosition()
    {
      this.validDataSamples = new List<TimeSample>[Viana.Project.ProcessingData.NumberOfTrackedObjects];
      for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        this.validDataSamples[i] = new List<TimeSample>();
      }

      foreach (TimeSample timeSample in this.Samples)
      {
        for (int j = 0; j < Viana.Project.ProcessingData.NumberOfTrackedObjects; j++)
        {
          if (timeSample.Object == null)
          {
            continue;
          }

          DataSample currentSample = timeSample.Object[j];
          if (currentSample == null)
          {
            continue;
          }

          this.validDataSamples[j].Add(timeSample);

          Point calibratedPoint = CalibrateSample(currentSample);
          currentSample.Time = (double)timeSample.Timestamp / GetTimeFactor();
          currentSample.PositionX = calibratedPoint.X;
          currentSample.PositionY = calibratedPoint.Y;
          currentSample.Acceleration = null;
          currentSample.AccelerationX = null;
          currentSample.AccelerationY = null;
          currentSample.Velocity = null;
          currentSample.VelocityX = null;
          currentSample.VelocityY = null;
          currentSample.Length = null;
          currentSample.LengthX = null;
          currentSample.LengthY = null;
          currentSample.Distance = null;
          currentSample.DistanceX = null;
          currentSample.DistanceY = null;
        }
      }
    }

    #endregion
  }
}