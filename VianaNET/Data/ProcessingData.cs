﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingData.cs" company="Freie Universität Berlin">
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
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Media;

  using AForge.Vision.Motion;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Filter;

  /// <summary>
  ///   The image processing.
  /// </summary>
  [Serializable]
  public class ProcessingData : DependencyObject, INotifyPropertyChanged
  {


    /// <summary>
    ///   The blob max diameter property.
    /// </summary>
    public static readonly DependencyProperty BlobMaxDiameterProperty = DependencyProperty.Register(
      "BlobMaxDiameter",
      typeof(ObservableCollection<double>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<double>()));

    /// <summary>
    ///   The target color property.
    /// </summary>
    public static readonly DependencyProperty TargetColorProperty = DependencyProperty.Register(
      "TargetColor",
      typeof(ObservableCollection<Color>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<Color>()));

    /// <summary>
    ///   The blob min diameter property.
    /// </summary>
    public static readonly DependencyProperty BlobMinDiameterProperty = DependencyProperty.Register(
      "BlobMinDiameter",
      typeof(ObservableCollection<double>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<double>()));

    /// <summary>
    ///   The color threshold property.
    /// </summary>
    public static readonly DependencyProperty ColorThresholdProperty = DependencyProperty.Register(
      "ColorThreshold",
      typeof(ObservableCollection<int>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<int>()));

    /// <summary>
    ///   The current blob center property.
    /// </summary>
    public static readonly DependencyProperty CurrentBlobCenterProperty =
      DependencyProperty.Register(
        "CurrentBlobCenter",
        typeof(ObservableCollection<Point?>),
        typeof(ProcessingData),
        new FrameworkPropertyMetadata(new ObservableCollection<Point?>()));

    /// <summary>
    ///   The detected blobs property.
    /// </summary>
    public static readonly DependencyProperty DetectedBlobsProperty = DependencyProperty.Register(
      "DetectedBlob",
      typeof(ObservableCollection<Segment>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<Segment>()));

    /// <summary>
    ///   The motion threshold property.
    /// </summary>
    public static readonly DependencyProperty MotionThresholdProperty = DependencyProperty.Register(
      "MotionThreshold",
      typeof(ObservableCollection<int>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<int>()));

    /// <summary>
    ///   The is positive contrast property.
    /// </summary>
    public static readonly DependencyProperty PositiveContrastProperty = DependencyProperty.Register(
      "PositiveContrast",
      typeof(ObservableCollection<bool>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<bool>(), OnPropertyChanged));

    /// <summary>
    ///   The is the suppress noise property.
    /// </summary>
    public static readonly DependencyProperty SuppressNoiseProperty = DependencyProperty.Register(
      "SuppressNoise",
      typeof(ObservableCollection<bool>),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(new ObservableCollection<bool>(), OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IndexOfObject" />.
    /// </summary>
    public static readonly DependencyProperty IndexOfObjectProperty = DependencyProperty.Register(
      "IndexOfObject",
      typeof(int),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(0, OnPropertyChanged));

    /// <summary>
    ///   The is target color set property.
    /// </summary>
    public static readonly DependencyProperty IsTargetColorSetProperty = DependencyProperty.Register(
      "IsTargetColorSet",
      typeof(bool),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="NumberOfTrackedObjects" />.
    /// </summary>
    public static readonly DependencyProperty NumberOfTrackedObjectsProperty =
      DependencyProperty.Register(
        "NumberOfTrackedObjects",
        typeof(int),
        typeof(CalibrationData),
        new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The difference quotient type property.
    /// </summary>
    public static readonly DependencyProperty DifferenceQuotientTypeProperty = DependencyProperty.Register(
      "DifferenceQuotientType",
      typeof(DifferenceQuotientType),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(DifferenceQuotientType.Forward));

    /// <summary>
    /// The is using color detection property
    /// </summary>
    public static readonly DependencyProperty IsUsingColorDetectionProperty = DependencyProperty.Register(
      "IsUsingColorDetection",
      typeof(bool),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(true));

    /// <summary>
    /// The is using motion detection property
    /// </summary>
    public static readonly DependencyProperty IsUsingMotionDetectionProperty = DependencyProperty.Register(
      "IsUsingMotionDetection",
      typeof(bool),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(false));

    /// <summary>
    /// The is using motion detection property
    /// </summary>
    public static readonly DependencyProperty IsDetectionActivatedProperty = DependencyProperty.Register(
      "IsDetectionActivated",
      typeof(bool),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(true));

    /// <summary>
    ///   The cursorcolor type property.
    /// </summary>
    public static readonly DependencyProperty CursorcolorTypeProperty = DependencyProperty.Register(
      "CursorcolorType",
      typeof(CursorcolorType),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(CursorcolorType.Flex));

    /// <summary>
    ///   The default color of the cursor property.
    /// </summary>
    public static readonly DependencyProperty CursorcolorProperty = DependencyProperty.Register(
      "Cursorcolor",
      typeof(Color),
      typeof(ProcessingData),
      new FrameworkPropertyMetadata(Colors.Black));

    /// <summary>
    ///   The color and crop filter.
    /// </summary>
    private readonly ColorAndCropFilterRgb colorAndCropFilter;

    /// <summary>
    ///   The crop filter.
    /// </summary>
    private readonly CropFilterRgb cropFilter;

    /// <summary>
    ///   The histogramm filter.
    /// </summary>
    private readonly Histogram histogrammFilter;

    /// <summary>
    ///   The segmentator.
    /// </summary>
    private readonly HistogramMinMaxSegmentator segmentator;

    /// <summary>
    ///   The watch.
    /// </summary>
    private readonly Stopwatch watch = new Stopwatch();

    /// <summary>
    /// The motion detector
    /// </summary>
    private readonly MotionDetector detector;

    /// <summary>
    ///   The counter.
    /// </summary>
    private int counter;

    /// <summary>
    ///   The do not throw property changed.
    /// </summary>
    private bool doNotThrowPropertyChanged;

    /// <summary>
    ///   The is ready.
    /// </summary>
    private bool isReady;

    /// <summary>
    ///   The total processing time.
    /// </summary>
    private long totalProcessingTime;


    /// <summary>
    ///   Initializes a new instance of the <see cref="ProcessingData" /> class.
    /// </summary>
    public ProcessingData()
    {
      //this.ColorThreshold = new ObservableCollection<int>();
      //this.BlobMinDiameter = new ObservableCollection<double>();
      //this.BlobMaxDiameter = new ObservableCollection<double>();

      this.colorAndCropFilter = new ColorAndCropFilterRgb();
      this.cropFilter = new CropFilterRgb();
      this.histogrammFilter = new Histogram();
      this.segmentator = new HistogramMinMaxSegmentator();

      this.TargetColor.CollectionChanged += this.TargetColorCollectionChanged;
      this.CurrentBlobCenter.CollectionChanged += this.BlobCollectionChanged;
      this.DetectedBlob.CollectionChanged += this.BlobCollectionChanged;
      this.ColorThreshold.CollectionChanged += this.HlslParamsCollectionChanged;
      this.BlobMinDiameter.CollectionChanged += this.HlslParamsCollectionChanged;
      this.BlobMaxDiameter.CollectionChanged += this.HlslParamsCollectionChanged;
      TwoFramesDifferenceDetectorSpecial motionDetector = new TwoFramesDifferenceDetectorSpecial();
      MotionAreaHighlightingSpecial motionProcessor = new MotionAreaHighlightingSpecial();
      this.detector = new MotionDetector(motionDetector, motionProcessor);
      this.MotionThreshold.CollectionChanged += this.MotionDetectionParameterCollectionChanged;
      this.SuppressNoise.CollectionChanged += this.MotionDetectionParameterCollectionChanged;
      this.PositiveContrast.CollectionChanged += this.MotionDetectionParameterCollectionChanged;

      this.PropertyChanged += this.ProcessingDataPropertyChanged;

      this.IsDetectionActivated = this.IsUsingMotionDetection || (this.IsUsingColorDetection && this.IsTargetColorSet);
    }





    /// <summary>
    ///   The frame processed.
    /// </summary>
    public event EventHandler FrameProcessed;

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;






    /// <summary>
    ///   Gets or sets the blob max diameter.
    /// </summary>
    public ObservableCollection<double> BlobMaxDiameter
    {
      get => (ObservableCollection<double>)this.GetValue(BlobMaxDiameterProperty);

      set => this.SetValue(BlobMaxDiameterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the blob min diameter.
    /// </summary>
    public ObservableCollection<double> BlobMinDiameter
    {
      get => (ObservableCollection<double>)this.GetValue(BlobMinDiameterProperty);

      set => this.SetValue(BlobMinDiameterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the color threshold.
    /// </summary>
    public ObservableCollection<int> ColorThreshold
    {
      get => (ObservableCollection<int>)this.GetValue(ColorThresholdProperty);

      set => this.SetValue(ColorThresholdProperty, value);
    }

    /// <summary>
    ///   Gets or sets the current blob center.
    /// </summary>
    public ObservableCollection<Point?> CurrentBlobCenter
    {
      get => (ObservableCollection<Point?>)this.GetValue(CurrentBlobCenterProperty);

      set => this.SetValue(CurrentBlobCenterProperty, value);
    }

    /// <summary>
    ///   Gets or sets the detected blob.
    /// </summary>
    public ObservableCollection<Segment> DetectedBlob
    {
      get => (ObservableCollection<Segment>)this.GetValue(DetectedBlobsProperty);

      set => this.SetValue(DetectedBlobsProperty, value);
    }

    /// <summary>
    ///   Gets or sets the motion thresholds.
    /// </summary>
    public ObservableCollection<int> MotionThreshold
    {
      get => (ObservableCollection<int>)this.GetValue(MotionThresholdProperty);

      set => this.SetValue(MotionThresholdProperty, value);
    }

    /// <summary>
    ///   Gets or sets the positive contrast  values.
    /// </summary>
    public ObservableCollection<bool> PositiveContrast
    {
      get => (ObservableCollection<bool>)this.GetValue(PositiveContrastProperty);

      set => this.SetValue(PositiveContrastProperty, value);
    }

    /// <summary>
    ///   Gets or sets the suppress noise values.
    /// </summary>
    public ObservableCollection<bool> SuppressNoise
    {
      get => (ObservableCollection<bool>)this.GetValue(SuppressNoiseProperty);

      set => this.SetValue(SuppressNoiseProperty, value);
    }

    /// <summary>
    ///   Gets or sets the index of the currently tracked object
    /// </summary>
    public int IndexOfObject
    {
      get => (int)this.GetValue(IndexOfObjectProperty);

      set => this.SetValue(IndexOfObjectProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether is target color set.
    /// </summary>
    public bool IsTargetColorSet
    {
      get => (bool)this.GetValue(IsTargetColorSetProperty);

      set => this.SetValue(IsTargetColorSetProperty, value);
    }

    /// <summary>
    ///   Gets or sets the number of tracked objects
    /// </summary>
    public int NumberOfTrackedObjects
    {
      get => (int)this.GetValue(NumberOfTrackedObjectsProperty);

      set => this.SetValue(NumberOfTrackedObjectsProperty, value);
    }

    /// <summary>
    ///   Gets or sets the target color.
    /// </summary>
    public ObservableCollection<Color> TargetColor
    {
      get => (ObservableCollection<Color>)this.GetValue(TargetColorProperty);

      set => this.SetValue(TargetColorProperty, value);
    }

    /// <summary>
    ///   Gets or sets the DifferenceQuotientType
    /// </summary>
    public DifferenceQuotientType DifferenceQuotientType
    {
      get => (DifferenceQuotientType)this.GetValue(DifferenceQuotientTypeProperty);

      set => this.SetValue(DifferenceQuotientTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is using color detection.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is using color detection; otherwise, <c>false</c>.
    /// </value>
    public bool IsUsingColorDetection
    {
      get => (bool)this.GetValue(IsUsingColorDetectionProperty);

      set => this.SetValue(IsUsingColorDetectionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is using motion detection.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is using motion detection; otherwise, <c>false</c>.
    /// </value>
    public bool IsUsingMotionDetection
    {
      get => (bool)this.GetValue(IsUsingMotionDetectionProperty);

      set => this.SetValue(IsUsingMotionDetectionProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether a detection algorithm is activated
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is using detection; otherwise, <c>false</c>.
    /// </value>
    public bool IsDetectionActivated
    {
      get => (bool)this.GetValue(IsDetectionActivatedProperty);

      set => this.SetValue(IsDetectionActivatedProperty, value);
      //get => this.IsUsingColorDetection || this.IsUsingMotionDetection;
    }

    /// <summary>
    ///   Gets or sets the CursorcolorType
    /// </summary>
    public CursorcolorType CursorcolorType
    {
      get => (CursorcolorType)this.GetValue(CursorcolorTypeProperty);
      set => this.SetValue(CursorcolorTypeProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Cursorcolor
    /// </summary>
    public Color Cursorcolor
    {
      get => (Color)this.GetValue(CursorcolorProperty);
      set => this.SetValue(CursorcolorProperty, value);
    }

    /// <summary>
    ///   The initialize image filters.
    /// </summary>
    public void InitializeImageFilters()
    {
      int videoWidth = (int)Video.Instance.VideoElement.NaturalVideoWidth;
      int videoHeight = (int)Video.Instance.VideoElement.NaturalVideoHeight;

      this.colorAndCropFilter.ImageWidth = videoWidth;
      this.colorAndCropFilter.ImageHeight = videoHeight;
      this.colorAndCropFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.colorAndCropFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.colorAndCropFilter.BlankColor = Colors.Black;
      this.colorAndCropFilter.TargetColor = this.TargetColor.Count > 0 ? this.TargetColor[0] : Colors.Red;
      this.colorAndCropFilter.Threshold = this.ColorThreshold[0];
      if (App.Project.CalibrationData.HasClipRegion)
      {
        this.colorAndCropFilter.CropRectangle = App.Project.CalibrationData.ClipRegion;
      }
      else
      {
        this.colorAndCropFilter.CropRectangle = new Rect(0, 0, videoWidth, videoHeight);
      }

      this.colorAndCropFilter.Init();

      this.cropFilter.ImageWidth = videoWidth;
      this.cropFilter.ImageHeight = videoHeight;
      this.cropFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.cropFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.cropFilter.BlankColor = Colors.Black;
      if (App.Project.CalibrationData.HasClipRegion)
      {
        this.cropFilter.CropRectangle = App.Project.CalibrationData.ClipRegion;
      }
      else
      {
        this.cropFilter.CropRectangle = new Rect(0, 0, videoWidth, videoHeight);
      }

      this.cropFilter.Init();

      this.histogrammFilter.ImageWidth = videoWidth;
      this.histogrammFilter.ImageHeight = videoHeight;
      this.histogrammFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.histogrammFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;

      if (this.detector.MotionDetectionAlgorithm is TwoFramesDifferenceDetectorSpecial algorithm)
      {
        algorithm.DifferenceThreshold = App.Project.ProcessingData.MotionThreshold[0];
        algorithm.IsPositiveThreshold = App.Project.ProcessingData.PositiveContrast[0];
        algorithm.SuppressNoise = App.Project.ProcessingData.SuppressNoise[0];
      }

      Video.Instance.RefreshProcessingMap();
      this.ProcessImage();
    }

    /// <summary>
    ///   The process image.
    /// </summary>
    /// <returns> The <see cref="bool" /> . </returns>
    public bool ProcessImage()
    {
      // Console.WriteLine("ProcessImage: #" + Video.Instance.FrameIndex);
      // Skip if no target color is available
      //if (!this.IsTargetColorSet)
      //{
      //  return false;
      //}
      if (!Video.Instance.HasVideo)
      {
        return false;
      }

      if (Project.IsDeserializing)
      {
        return false;
      }

      // Skip if working
      if (!this.isReady)
      {
        return false;
      }

      this.isReady = false;
      bool objectsFound = false;

      this.watch.Start();
      long start = this.watch.ElapsedMilliseconds;

      if (App.Project.ProcessingData.IsDetectionActivated)
      {
        for (int i = 0; i < App.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          // Console.Write("BeforeColorFilte ");
          // Console.WriteLine(watch.ElapsedMilliseconds.ToString());
          if (this.ColorThreshold.Count <= i || this.BlobMinDiameter.Count <= i
              || this.BlobMaxDiameter.Count <= i || this.CurrentBlobCenter.Count <= i)
          {
            break;
          }

          // Get original picture
          Video.Instance.RefreshProcessingMap();

          if (this.IsUsingColorDetection)
          {
            // Apply color and crop filter if applicable
            this.colorAndCropFilter.TargetColor = this.TargetColor.Count > i ? this.TargetColor[i] : Colors.Black;
            this.colorAndCropFilter.Threshold = this.ColorThreshold[i];
            this.colorAndCropFilter.ProcessInPlace(Video.Instance.VideoElement.ColorProcessingMapping);
          }
          else
          {
            // Only apply crop filter
            this.cropFilter.ProcessInPlace(Video.Instance.VideoElement.ColorProcessingMapping);
          }

          // Apply motion detection if applicable
          if (this.IsUsingMotionDetection)
          {
            if (this.detector.MotionDetectionAlgorithm is TwoFramesDifferenceDetectorSpecial algorithm)
            {
              algorithm.DifferenceThreshold = App.Project.ProcessingData.MotionThreshold[i];
              algorithm.IsPositiveThreshold = App.Project.ProcessingData.PositiveContrast[i];
              algorithm.SuppressNoise = App.Project.ProcessingData.SuppressNoise[i];
            }

            Video.Instance.VideoElement.CopyProcessingMapToUnmanagedImage();
            this.detector.ProcessFrame(Video.Instance.VideoElement.UnmanagedImage);
            Video.Instance.VideoElement.CopyProcessedDataToProcessingMap();
          }

          // Send modified image to blobs control
          Video.Instance.VideoElement.UpdateProcessedImageSource();

          // Get blobs from filtered process
          IntPtr mapToUse;
          if (this.IsUsingColorDetection && !this.IsUsingMotionDetection)
          {
            mapToUse = Video.Instance.VideoElement.ColorProcessingMapping;
          }
          else
          {
            mapToUse = Video.Instance.VideoElement.MotionProcessingMapping;
          }

          Histogram histogram = this.histogrammFilter.FromIntPtrMap(mapToUse);
          this.segmentator.Histogram = histogram;
          this.segmentator.ThresholdLuminance = histogram.Max * 0.5f;
          this.segmentator.MinDiameter = this.BlobMinDiameter[i];
          this.segmentator.MaxDiameter = this.BlobMaxDiameter[i];

          Segment foundSegment = this.segmentator.Process();
          while (this.DetectedBlob.Count <= i)
          {
            this.DetectedBlob.Add(new Segment());
          }

          this.DetectedBlob[i] = foundSegment;

          // Console.Write("AfterBlobDetection: ");
          // Console.WriteLine(watch.ElapsedMilliseconds.ToString());
          if (foundSegment.Diagonal != 0 && (foundSegment.Height < (this.colorAndCropFilter.ImageHeight - 10))
              && (foundSegment.Width < (this.colorAndCropFilter.ImageWidth - 10)))
          {
            this.CurrentBlobCenter[i] = new Point(foundSegment.Center.X, foundSegment.Center.Y);
            objectsFound = true;
          }
          else
          {
            this.CurrentBlobCenter[i] = null;
          }

          if (Video.Instance.IsDataAcquisitionRunning)
          {
            if (this.CurrentBlobCenter[i].HasValue)
            {
              var flippedPoint = new Point(this.CurrentBlobCenter[i].Value.X, Video.Instance.VideoElement.NaturalVideoHeight - this.CurrentBlobCenter[i].Value.Y);
              App.Project.VideoData.AddPoint(i, flippedPoint);
            }
          }
        }
      }

      this.OnFrameProcessed();

      // Console.Write("Finished: ");
      // Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      this.totalProcessingTime += this.watch.ElapsedMilliseconds - start;
      this.counter++;

#if DEBUG

      // Console.Write("AverageProcessingTime: ");
      // Console.WriteLine(this.totalProcessingTime / this.counter);
#endif

      this.isReady = true;
      if (objectsFound)
      {
        StatusBarContent.Instance.MessagesLabel = VianaNET.Localization.Labels.BlobsObjectsFound;
        return true;
      }

      StatusBarContent.Instance.MessagesLabel = VianaNET.Localization.Labels.BlobsNoObjectFound;
      return false;
    }

    /// <summary>
    ///   The reset.
    /// </summary>
    public void Reset(bool complete)
    {
      this.ResetProcessing(App.Project.ProcessingData.NumberOfTrackedObjects, complete);
    }





    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null && !this.doNotThrowPropertyChanged)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as ProcessingData).OnPropertyChanged(args);
    }

    /// <summary>
    /// Blob collection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    private void BlobCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.OnPropertyChanged("Blob");
      }
    }

    /// <summary>
    /// HLSL parameters collection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    private void HlslParamsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.ProcessImage();
        this.OnPropertyChanged("HLSLParams");
      }
    }

    /// <summary>
    /// Motion detection parameter collection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    /// <exception cref="NotImplementedException"></exception>
    void MotionDetectionParameterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.ProcessImage();
        this.OnPropertyChanged("MotionDetectionParams");
      }
    }


    /// <summary>
    ///   The on frame processed.
    /// </summary>
    private void OnFrameProcessed()
    {
      this.FrameProcessed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name.
    /// </param>
    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null && !this.doNotThrowPropertyChanged)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// The image processing_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ProcessingDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IndexOfObject")
      {
        if (this.IndexOfObject >= this.NumberOfTrackedObjects || this.IndexOfObject < 0)
        {
          this.IndexOfObject = 0;
        }

        if (!Project.IsDeserializing)
        {
          App.Project.VideoData.FilterSamples();
        }
      }
      else if (e.PropertyName == "NumberOfTrackedObjects")
      {
        if (!Project.IsDeserializing)
        {
          this.Reset(true);
        }
      }
      else if (e.PropertyName == "IsUsingMotionDetection" || e.PropertyName == "IsUsingColorDetection" || e.PropertyName == "IsTargetColorSet")
      {
        this.IsDetectionActivated = this.IsUsingMotionDetection || (this.IsUsingColorDetection && this.IsTargetColorSet);
        this.detector.Reset();
        Video.Instance.RefreshProcessingMap();
        Video.Instance.VideoElement.CopyProcessingMapToUnmanagedImage();
        Video.Instance.VideoElement.UpdateImageSources();
        this.ProcessImage();
      }
    }

    /// <summary>
    /// The reset processing.
    /// </summary>
    /// <param name="numberOfObjects">
    /// The number of objects.
    /// </param>
    private void ResetProcessing(int numberOfObjects, bool complete)
    {
      this.IndexOfObject = 0;
      this.doNotThrowPropertyChanged = true;
      this.ColorThreshold.Clear();
      this.BlobMinDiameter.Clear();
      this.BlobMaxDiameter.Clear();
      this.CurrentBlobCenter.Clear();
      this.DetectedBlob.Clear();
      this.detector.Reset();
      this.MotionThreshold.Clear();
      this.PositiveContrast.Clear();
      this.SuppressNoise.Clear();
      if (complete)
      {
        this.TargetColor.Clear();
      }

      for (int i = 0; i < numberOfObjects; i++)
      {
        //if (complete)
        //{
        //  this.TargetColor.Add(Colors.Transparent);
        //}
        this.ColorThreshold.Add(30);
        this.BlobMinDiameter.Add(4);
        this.BlobMaxDiameter.Add(100);
        this.CurrentBlobCenter.Add(null);
        this.MotionThreshold.Add(15);
        this.PositiveContrast.Add(true);
        this.SuppressNoise.Add(false);
      }
      if (complete)
      {
        this.IsTargetColorSet = false;
      }

      this.isReady = true;
      this.doNotThrowPropertyChanged = false;

      Video.Instance.RefreshProcessingMap();
      Video.Instance.VideoElement.CopyProcessingMapToUnmanagedImage();
      Video.Instance.VideoElement.UpdateImageSources();
    }

    /// <summary>
    /// The target color_ collection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void TargetColorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged && !Project.IsDeserializing)
      {
        this.ProcessImage();
        this.OnPropertyChanged("TargetColor");
      }
    }


  }
}