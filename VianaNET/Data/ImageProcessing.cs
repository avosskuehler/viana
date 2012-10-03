// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageProcessing.cs" company="Freie Universität Berlin">
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
//   The image processing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Media;

  using VianaNET.Localization;
  using VianaNET.MainWindow;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Filter;

  /// <summary>
  ///   The image processing.
  /// </summary>
  public class ImageProcessing : DependencyObject, INotifyPropertyChanged
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The blob max diameter property.
    /// </summary>
    public static readonly DependencyProperty BlobMaxDiameterProperty = DependencyProperty.Register(
      "BlobMaxDiameter",
      typeof(ObservableCollection<double>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<double>()));

    /// <summary>
    ///   The blob min diameter property.
    /// </summary>
    public static readonly DependencyProperty BlobMinDiameterProperty = DependencyProperty.Register(
      "BlobMinDiameter",
      typeof(ObservableCollection<double>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<double>()));

    /// <summary>
    ///   The color threshold property.
    /// </summary>
    public static readonly DependencyProperty ColorThresholdProperty = DependencyProperty.Register(
      "ColorThreshold",
      typeof(ObservableCollection<int>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<int>()));

    /// <summary>
    ///   The current blob center property.
    /// </summary>
    public static readonly DependencyProperty CurrentBlobCenterProperty =
      DependencyProperty.Register(
        "CurrentBlobCenter",
        typeof(ObservableCollection<Point?>),
        typeof(ImageProcessing),
        new FrameworkPropertyMetadata(new ObservableCollection<Point?>()));

    /// <summary>
    ///   The detected blobs property.
    /// </summary>
    public static readonly DependencyProperty DetectedBlobsProperty = DependencyProperty.Register(
      "DetectedBlob",
      typeof(ObservableCollection<Segment>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<Segment>()));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IndexOfObject" />.
    /// </summary>
    public static readonly DependencyProperty IndexOfObjectProperty = DependencyProperty.Register(
      "IndexOfObject", typeof(int), typeof(ImageProcessing), new FrameworkPropertyMetadata(0, OnPropertyChanged));

    /// <summary>
    ///   The is target color set property.
    /// </summary>
    public static readonly DependencyProperty IsTargetColorSetProperty = DependencyProperty.Register(
      "IsTargetColorSet",
      typeof(Boolean),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="NumberOfTrackedObjects" />.
    /// </summary>
    public static readonly DependencyProperty NumberOfTrackedObjectsProperty =
      DependencyProperty.Register(
        "NumberOfTrackedObjects",
        typeof(int),
        typeof(Calibration),
        new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The target color property.
    /// </summary>
    public static readonly DependencyProperty TargetColorProperty = DependencyProperty.Register(
      "TargetColor",
      typeof(ObservableCollection<Color>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<Color>()));

    #endregion

    #region Fields

    /// <summary>
    ///   The color and crop filter.
    /// </summary>
    private readonly ColorAndCropFilterRGB colorAndCropFilter;

    /// <summary>
    ///   The color range filter.
    /// </summary>
    private readonly ColorAndCropFilterYCbCr colorRangeFilter;

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

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="ImageProcessing" /> class.
    /// </summary>
    static ImageProcessing()
    {
      TrackObjectColors = new List<SolidColorBrush>();
      TrackObjectColors.Add(Brushes.Red);
      TrackObjectColors.Add(Brushes.Green);
      TrackObjectColors.Add(Brushes.Blue);
      TrackObjectColors.Add(Brushes.Yellow);
      TrackObjectColors.Add(Brushes.Magenta);
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ImageProcessing" /> class.
    /// </summary>
    public ImageProcessing()
    {
      this.ColorThreshold = new ObservableCollection<int>();
      this.BlobMinDiameter = new ObservableCollection<double>();
      this.BlobMaxDiameter = new ObservableCollection<double>();
      this.ResetProcessing(1);
      this.colorAndCropFilter = new ColorAndCropFilterRGB();
      this.colorRangeFilter = new ColorAndCropFilterYCbCr();
      this.histogrammFilter = new Histogram();
      this.segmentator = new HistogramMinMaxSegmentator();

      this.TargetColor.CollectionChanged += this.TargetColor_CollectionChanged;
      this.CurrentBlobCenter.CollectionChanged += this.Blob_CollectionChanged;
      this.DetectedBlob.CollectionChanged += this.Blob_CollectionChanged;
      this.ColorThreshold.CollectionChanged += this.HLSLParams_CollectionChanged;
      this.BlobMinDiameter.CollectionChanged += this.HLSLParams_CollectionChanged;
      this.BlobMaxDiameter.CollectionChanged += this.HLSLParams_CollectionChanged;

      this.PropertyChanged += this.ImageProcessing_PropertyChanged;
      Calibration.Instance.PropertyChanged += this.Calibration_PropertyChanged;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Events

    /// <summary>
    ///   The frame processed.
    /// </summary>
    public event EventHandler FrameProcessed;

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    ///   Gets or sets  a list of brushes for the different tracked objects
    /// </summary>
    public static List<SolidColorBrush> TrackObjectColors { get; set; }

    /// <summary>
    ///   Gets or sets the blob max diameter.
    /// </summary>
    public ObservableCollection<double> BlobMaxDiameter
    {
      get
      {
        return (ObservableCollection<double>)this.GetValue(BlobMaxDiameterProperty);
      }

      set
      {
        this.SetValue(BlobMaxDiameterProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the blob min diameter.
    /// </summary>
    public ObservableCollection<double> BlobMinDiameter
    {
      get
      {
        return (ObservableCollection<double>)this.GetValue(BlobMinDiameterProperty);
      }

      set
      {
        this.SetValue(BlobMinDiameterProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the color threshold.
    /// </summary>
    public ObservableCollection<int> ColorThreshold
    {
      get
      {
        return (ObservableCollection<int>)this.GetValue(ColorThresholdProperty);
      }

      set
      {
        this.SetValue(ColorThresholdProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the current blob center.
    /// </summary>
    public ObservableCollection<Point?> CurrentBlobCenter
    {
      get
      {
        return (ObservableCollection<Point?>)this.GetValue(CurrentBlobCenterProperty);
      }

      set
      {
        this.SetValue(CurrentBlobCenterProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the detected blob.
    /// </summary>
    public ObservableCollection<Segment> DetectedBlob
    {
      get
      {
        return (ObservableCollection<Segment>)this.GetValue(DetectedBlobsProperty);
      }

      set
      {
        this.SetValue(DetectedBlobsProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the index of the currently tracked object
    /// </summary>
    public int IndexOfObject
    {
      get
      {
        return (int)this.GetValue(IndexOfObjectProperty);
      }

      set
      {
        this.SetValue(IndexOfObjectProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether is target color set.
    /// </summary>
    public bool IsTargetColorSet
    {
      get
      {
        return (Boolean)this.GetValue(IsTargetColorSetProperty);
      }

      set
      {
        this.SetValue(IsTargetColorSetProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the number of tracked objects
    /// </summary>
    public int NumberOfTrackedObjects
    {
      get
      {
        return (int)this.GetValue(NumberOfTrackedObjectsProperty);
      }

      set
      {
        this.SetValue(NumberOfTrackedObjectsProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the target color.
    /// </summary>
    public ObservableCollection<Color> TargetColor
    {
      get
      {
        return (ObservableCollection<Color>)this.GetValue(TargetColorProperty);
      }

      set
      {
        this.SetValue(TargetColorProperty, value);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The initialize image filters.
    /// </summary>
    public void InitializeImageFilters()
    {
      var videoWidth = (int)Video.Instance.VideoElement.NaturalVideoWidth;
      var videoHeight = (int)Video.Instance.VideoElement.NaturalVideoHeight;

      this.colorAndCropFilter.ImageWidth = videoWidth;
      this.colorAndCropFilter.ImageHeight = videoHeight;
      this.colorAndCropFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.colorAndCropFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.colorAndCropFilter.BlankColor = Colors.Black;
      this.colorAndCropFilter.TargetColor = this.TargetColor[0];
      this.colorAndCropFilter.Threshold = this.ColorThreshold[0];
      if (Calibration.Instance.HasClipRegion)
      {
        this.colorAndCropFilter.CropRectangle = Calibration.Instance.ClipRegion;
      }
      else
      {
        this.colorAndCropFilter.CropRectangle = new Rect(0, 0, videoWidth, videoHeight);
      }

      this.colorAndCropFilter.Init();

      this.colorRangeFilter.ImageWidth = videoWidth;
      this.colorRangeFilter.ImageHeight = videoHeight;
      this.colorRangeFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.colorRangeFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.colorRangeFilter.BlankColor = Colors.Black;
      this.colorRangeFilter.TargetColor = this.TargetColor[0];
      this.colorRangeFilter.Threshold = this.ColorThreshold[0];
      if (Calibration.Instance.HasClipRegion)
      {
        this.colorRangeFilter.CropRectangle = Calibration.Instance.ClipRegion;
      }
      else
      {
        this.colorRangeFilter.CropRectangle = new Rect(0, 0, videoWidth, videoHeight);
      }

      this.colorRangeFilter.Init();

      this.histogrammFilter.ImageWidth = videoWidth;
      this.histogrammFilter.ImageHeight = videoHeight;
      this.histogrammFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.histogrammFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;

      Video.Instance.RefreshProcessingMap();
      this.ProcessImage();
    }

    /// <summary>
    ///   The process image.
    /// </summary>
    /// <returns> The <see cref="bool" /> . </returns>
    public bool ProcessImage()
    {
      // Skip if no target color is available
      if (!this.IsTargetColorSet)
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

      for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
      {
        // Console.Write("BeforeColorFilter: ");
        // Console.WriteLine(watch.ElapsedMilliseconds.ToString());
        Video.Instance.RefreshProcessingMap();
        this.colorAndCropFilter.TargetColor = this.TargetColor[i];
        this.colorAndCropFilter.Threshold = this.ColorThreshold[i];

        this.colorAndCropFilter.ProcessInPlace(Video.Instance.VideoElement.ProcessingMapping);

        // this.colorRangeFilter.ProcessInPlace(Video.Instance.VideoElement.ProcessingMapping);

        // Segment
        Histogram histogram = this.histogrammFilter.FromIntPtrMap(Video.Instance.VideoElement.ProcessingMapping);
        this.segmentator.Histogram = histogram;
        this.segmentator.ThresholdLuminance = histogram.Max * 0.1f;
        this.segmentator.MinDiameter = this.BlobMinDiameter[i];
        this.segmentator.MaxDiameter = this.BlobMaxDiameter[i];

        Segment foundSegment = this.segmentator.Process();
        this.DetectedBlob[i] = foundSegment;

        // Console.Write("AfterBlobDetection: ");
        // Console.WriteLine(watch.ElapsedMilliseconds.ToString());
        if (foundSegment.Diagonal != 0 && (foundSegment.Height < (this.colorAndCropFilter.ImageHeight - 10))
            && (foundSegment.Width < (this.colorAndCropFilter.ImageWidth - 10)))
        {
          this.CurrentBlobCenter[i] = new Point(foundSegment.Center.X, foundSegment.Center.Y);

          if (Video.Instance.IsDataAcquisitionRunning)
          {
            VideoData.Instance.AddPoint(i, this.CurrentBlobCenter[i].Value);
          }

          objectsFound = true;
        }
        else
        {
          this.CurrentBlobCenter[i] = null;
        }
      }

      this.OnFrameProcessed();

      // Console.Write("Finished: ");
      // Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      this.totalProcessingTime += this.watch.ElapsedMilliseconds - start;
      this.counter++;

      Console.Write("AverageProcessingTime: ");
      Console.WriteLine(this.totalProcessingTime / this.counter);
      this.isReady = true;
      if (objectsFound)
      {
        StatusBarContent.Instance.MessagesLabel = Labels.BlobsObjectsFound;
        return true;
      }
      else
      {
        StatusBarContent.Instance.MessagesLabel = Labels.BlobsNoObjectFound;
        return false;
      }
    }

    /// <summary>
    ///   The reset.
    /// </summary>
    public void Reset()
    {
      this.ResetProcessing(Video.Instance.ImageProcessing.NumberOfTrackedObjects);
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

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

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

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
      (obj as ImageProcessing).OnPropertyChanged(args);
    }

    /// <summary>
    /// The blob_ collection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Blob_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.OnPropertyChanged("Blob");
      }
    }

    /// <summary>
    /// The calibration_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Calibration_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" || e.PropertyName == "ClipRegion")
      {
        this.InitializeImageFilters();
      }
    }

    /// <summary>
    /// The hlsl params_ collection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void HLSLParams_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.ProcessImage();
        this.OnPropertyChanged("HLSLParams");
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
    private void ImageProcessing_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IndexOfObject")
      {
        if (this.IndexOfObject >= this.NumberOfTrackedObjects || this.IndexOfObject < 0)
        {
          this.IndexOfObject = 0;
        }
      }
      else if (e.PropertyName == "NumberOfTrackedObjects")
      {
        this.Reset();
      }
      else if (e.PropertyName == "IsTargetColorSet")
      {
        this.ProcessImage();
      }
    }

    /// <summary>
    ///   The on frame processed.
    /// </summary>
    private void OnFrameProcessed()
    {
      if (this.FrameProcessed != null)
      {
        this.FrameProcessed(this, EventArgs.Empty);
      }
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
    /// The reset processing.
    /// </summary>
    /// <param name="numberOfObjects">
    /// The number of objects. 
    /// </param>
    private void ResetProcessing(int numberOfObjects)
    {
      this.IndexOfObject = 0;
      this.doNotThrowPropertyChanged = true;
      this.TargetColor.Clear();
      this.ColorThreshold.Clear();
      this.BlobMinDiameter.Clear();
      this.BlobMaxDiameter.Clear();
      this.CurrentBlobCenter.Clear();
      this.DetectedBlob.Clear();
      for (int i = 0; i < numberOfObjects; i++)
      {
        this.TargetColor.Add(Colors.Red);
        this.ColorThreshold.Add(30);
        this.BlobMinDiameter.Add(4);
        this.BlobMaxDiameter.Add(100);
        this.CurrentBlobCenter.Add(null);
        this.DetectedBlob.Add(new Segment());
      }

      this.IsTargetColorSet = false;
      this.isReady = true;
      this.doNotThrowPropertyChanged = false;
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
    private void TargetColor_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.ProcessImage();
        this.OnPropertyChanged("TargetColor");
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}