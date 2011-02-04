using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLocalizeExtension.Extensions;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Diagnostics;

namespace VianaNET
{
  public class ImageProcessing : DependencyObject, INotifyPropertyChanged
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private ColorAndCropFilterRGB colorAndCropFilter;
    private ColorAndCropFilterYCbCr colorRangeFilter;
    private Histogram histogrammFilter;
    private HistogramMinMaxSegmentator segmentator;
    private bool isReady;
    private Stopwatch watch = new Stopwatch();
    private int counter;
    private long totalProcessingTime;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    public ImageProcessing()
    {
      ResetProcessing(1);
      this.colorAndCropFilter = new ColorAndCropFilterRGB();
      this.colorRangeFilter = new ColorAndCropFilterYCbCr();
      this.histogrammFilter = new Histogram();
      this.segmentator = new HistogramMinMaxSegmentator();

      this.TargetColor.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TargetColor_CollectionChanged);
      this.CurrentBlobCenter.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Blob_CollectionChanged);
      this.DetectedBlob.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Blob_CollectionChanged);
      this.ColorThreshold.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HLSLParams_CollectionChanged);
      this.BlobMinDiameter.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HLSLParams_CollectionChanged);
      this.BlobMaxDiameter.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HLSLParams_CollectionChanged);

      this.PropertyChanged += new PropertyChangedEventHandler(ImageProcessing_PropertyChanged);
      Calibration.Instance.PropertyChanged += new PropertyChangedEventHandler(Calibration_PropertyChanged);
    }

    void Blob_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.OnPropertyChanged("Blob");
      }
    }

    void HLSLParams_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.ProcessImage();
        this.OnPropertyChanged("HLSLParams");
      }
    }

    void TargetColor_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (!this.doNotThrowPropertyChanged)
      {
        this.ProcessImage();
        this.OnPropertyChanged("TargetColor");
      }
    }

    static ImageProcessing()
    {
      TrackObjectColors = new List<SolidColorBrush>();
      TrackObjectColors.Add(Brushes.Red);
      TrackObjectColors.Add(Brushes.Green);
      TrackObjectColors.Add(Brushes.Blue);
      TrackObjectColors.Add(Brushes.Yellow);
      TrackObjectColors.Add(Brushes.Magenta);
    }


    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler FrameProcessed;

    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// Gets or sets  a list of brushes for the different tracked objects
    /// </summary>
    public static List<SolidColorBrush> TrackObjectColors { get; set; }

    /// <summary>
    /// Gets or sets the number of tracked objects
    /// </summary>
    public int NumberOfTrackedObjects
    {
      get { return (int)this.GetValue(NumberOfTrackedObjectsProperty); }
      set { this.SetValue(NumberOfTrackedObjectsProperty, value); }
    }

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the property <see cref="NumberOfTrackedObjects"/>.
    /// </summary>
    public static readonly DependencyProperty NumberOfTrackedObjectsProperty = DependencyProperty.Register(
      "NumberOfTrackedObjects",
      typeof(int),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        1,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    /// <summary>
    /// Gets or sets the index of the currently tracked object
    /// </summary>
    public int IndexOfObject
    {
      get { return (int)this.GetValue(IndexOfObjectProperty); }
      set { this.SetValue(IndexOfObjectProperty, value); }
    }

    /// <summary>
    /// The <see cref="DependencyProperty"/> for the property <see cref="IndexOfObject"/>.
    /// </summary>
    public static readonly DependencyProperty IndexOfObjectProperty = DependencyProperty.Register(
      "IndexOfObject",
      typeof(int),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));

    public ObservableCollection<int> ColorThreshold
    {
      get { return (ObservableCollection<int>)this.GetValue(ColorThresholdProperty); }
      set { this.SetValue(ColorThresholdProperty, value); }
    }

    public static readonly DependencyProperty ColorThresholdProperty = DependencyProperty.Register(
      "ColorThreshold",
      typeof(ObservableCollection<int>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        new ObservableCollection<int>()));

    public ObservableCollection<double> BlobMinDiameter
    {
      get { return (ObservableCollection<double>)this.GetValue(BlobMinDiameterProperty); }
      set { this.SetValue(BlobMinDiameterProperty, value); }
    }

    public static readonly DependencyProperty BlobMinDiameterProperty = DependencyProperty.Register(
      "BlobMinDiameter",
      typeof(ObservableCollection<double>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        new ObservableCollection<double>()));

    public ObservableCollection<double> BlobMaxDiameter
    {
      get { return (ObservableCollection<double>)this.GetValue(BlobMaxDiameterProperty); }
      set { this.SetValue(BlobMaxDiameterProperty, value); }
    }

    public static readonly DependencyProperty BlobMaxDiameterProperty = DependencyProperty.Register(
      "BlobMaxDiameter",
      typeof(ObservableCollection<double>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        new ObservableCollection<double>()));

    public ObservableCollection<Color> TargetColor
    {
      get { return (ObservableCollection<Color>)this.GetValue(TargetColorProperty); }
      set { this.SetValue(TargetColorProperty, value); }
    }

    public static readonly DependencyProperty TargetColorProperty = DependencyProperty.Register(
      "TargetColor",
      typeof(ObservableCollection<Color>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        new ObservableCollection<Color>()));

    public Boolean IsTargetColorSet
    {
      get { return (Boolean)this.GetValue(IsTargetColorSetProperty); }
      set { this.SetValue(IsTargetColorSetProperty, value); }
    }

    public static readonly DependencyProperty IsTargetColorSetProperty = DependencyProperty.Register(
      "IsTargetColorSet",
      typeof(Boolean),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        false,
        new PropertyChangedCallback(OnPropertyChanged)));

    public ObservableCollection<Point?> CurrentBlobCenter
    {
      get { return (ObservableCollection<Point?>)this.GetValue(CurrentBlobCenterProperty); }
      set { this.SetValue(CurrentBlobCenterProperty, value); }
    }

    public static readonly DependencyProperty CurrentBlobCenterProperty = DependencyProperty.Register(
      "CurrentBlobCenter",
      typeof(ObservableCollection<Point?>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<Point?>()));

    public ObservableCollection<Segment> DetectedBlob
    {
      get { return (ObservableCollection<Segment>)this.GetValue(DetectedBlobsProperty); }
      set { this.SetValue(DetectedBlobsProperty, value); }
    }

    public static readonly DependencyProperty DetectedBlobsProperty = DependencyProperty.Register(
      "DetectedBlob",
      typeof(ObservableCollection<Segment>),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(new ObservableCollection<Segment>()));

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void Reset()
    {
      ResetProcessing(Video.Instance.ImageProcessing.NumberOfTrackedObjects);
    }

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

    public void InitializeImageFilters()
    {
      int videoWidth = (int)Video.Instance.VideoElement.NaturalVideoWidth;
      int videoHeight = (int)Video.Instance.VideoElement.NaturalVideoHeight;

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

      watch.Start();
      var start = watch.ElapsedMilliseconds;

      for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
      {
        //Console.Write("BeforeColorFilter: ");
        //Console.WriteLine(watch.ElapsedMilliseconds.ToString());
        Video.Instance.RefreshProcessingMap();
        this.colorAndCropFilter.TargetColor = this.TargetColor[i];
        this.colorAndCropFilter.Threshold = this.ColorThreshold[i];

        this.colorAndCropFilter.ProcessInPlace(Video.Instance.VideoElement.ProcessingMapping);
        //this.colorRangeFilter.ProcessInPlace(Video.Instance.VideoElement.ProcessingMapping);

        // Segment
        var histogram = this.histogrammFilter.FromIntPtrMap(Video.Instance.VideoElement.ProcessingMapping);
        segmentator.Histogram = histogram;
        segmentator.ThresholdLuminance = histogram.Max * 0.1f;
        segmentator.MinDiameter = this.BlobMinDiameter[i];
        segmentator.MaxDiameter = this.BlobMaxDiameter[i];

        var foundSegment = segmentator.Process();
        this.DetectedBlob[i] = foundSegment;
        //Console.Write("AfterBlobDetection: ");
        //Console.WriteLine(watch.ElapsedMilliseconds.ToString());

        if (foundSegment.Diagonal != 0 &&
          (foundSegment.Height < (this.colorAndCropFilter.ImageHeight - 10)) &&
          (foundSegment.Width < (this.colorAndCropFilter.ImageWidth - 10)))
        {
          this.CurrentBlobCenter[i] = new Point(
            foundSegment.Center.X,
            foundSegment.Center.Y);

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

      //Console.Write("Finished: ");
      //Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      totalProcessingTime += this.watch.ElapsedMilliseconds - start;
      counter++;

      Console.Write("AverageProcessingTime: ");
      Console.WriteLine(totalProcessingTime / counter);
      this.isReady = true;
      if (objectsFound)
      {
        StatusBarContent.Instance.MessagesLabel = Localization.Labels.BlobsObjectsFound;
        return true;
      }
      else
      {
        StatusBarContent.Instance.MessagesLabel = Localization.Labels.BlobsNoObjectFound;
        return false;
      }
    }

    #endregion //PUBLICMETHODS

    private bool doNotThrowPropertyChanged;

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null && !this.doNotThrowPropertyChanged)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      (obj as ImageProcessing).OnPropertyChanged(args);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null && !this.doNotThrowPropertyChanged)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private void OnFrameProcessed()
    {
      if (this.FrameProcessed != null)
      {
        this.FrameProcessed(this, EventArgs.Empty);
      }
    }

    void Calibration_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" ||
        e.PropertyName == "ClipRegion")
      {
        InitializeImageFilters();
      }
    }

    void ImageProcessing_PropertyChanged(object sender, PropertyChangedEventArgs e)
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


    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS
    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
