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
    //private BlobDetectionFilter blobDetection;
    private ColorAndCropFilterRGB colorAndCropFilter;
    private ColorAndCropFilterYCbCr colorRangeFilter;
    private Histogram histogrammFilter;
    private HistogramMinMaxSegmentator segmentator;

    private bool isReady;
    public event PropertyChangedEventHandler PropertyChanged;

    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      (obj as ImageProcessing).OnPropertyChanged(args);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    public ImageProcessing()
    {
      Reset();
      //this.blobDetection = new BlobDetectionFilter();
      this.colorAndCropFilter = new ColorAndCropFilterRGB();
      this.colorRangeFilter = new ColorAndCropFilterYCbCr();
      this.histogrammFilter = new Histogram();
      this.segmentator = new HistogramMinMaxSegmentator();

      this.PropertyChanged += new PropertyChangedEventHandler(ImageProcessing_PropertyChanged);
      Calibration.Instance.PropertyChanged += new PropertyChangedEventHandler(Calibration_PropertyChanged);
    }

    public void Reset()
    {
      this.TargetColor = Colors.Red;
      this.ColorThreshold = 0.3;
      this.BlobMinDiameter = 4;
      this.BlobMaxDiameter = 100;
      this.IsTargetColorSet = false;
      this.isReady = true;
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
      if (e.PropertyName != "DetectedBlobs")
      {
        InitializeImageFilters();
      }
    }

    public void InitializeImageFilters()
    {
      int videoWidth = (int)Video.Instance.VideoElement.NaturalVideoWidth;
      int videoHeight = (int)Video.Instance.VideoElement.NaturalVideoHeight;
      //this.blobDetection.ImageWidth = videoWidth;
      //this.blobDetection.ImageHeight = videoHeight;
      //this.blobDetection.ImageStride = Video.Instance.VideoElement.Stride;
      //this.blobDetection.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      //this.blobDetection.FilterBlobs = true;
      //this.blobDetection.CoupledSizeFiltering = true;
      //this.blobDetection.MinHeight = (int)this.BlobMinDiameter;
      //this.blobDetection.MinWidth = (int)this.BlobMinDiameter;
      //this.blobDetection.MaxWidth = (int)this.BlobMaxDiameter;
      //this.blobDetection.MaxHeight = (int)this.BlobMaxDiameter;
      //this.blobDetection.ObjectsOrder = ObjectsOrder.Area;

      this.colorAndCropFilter.ImageWidth = videoWidth;
      this.colorAndCropFilter.ImageHeight = videoHeight;
      this.colorAndCropFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.colorAndCropFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.colorAndCropFilter.BlankColor = Colors.Black;
      this.colorAndCropFilter.TargetColor = this.TargetColor;
      this.colorAndCropFilter.Threshold = (int)(this.ColorThreshold * 255);
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
      this.colorRangeFilter.TargetColor = this.TargetColor;
      this.colorRangeFilter.Threshold = (int)(this.ColorThreshold * 255);
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

    public double ColorThreshold
    {
      get { return (double)this.GetValue(ColorThresholdProperty); }
      set { this.SetValue(ColorThresholdProperty, value); }
    }

    public static readonly DependencyProperty ColorThresholdProperty = DependencyProperty.Register(
      "ColorThreshold",
      typeof(double),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        0.3d,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public double BlobMinDiameter
    {
      get { return (double)this.GetValue(BlobMinDiameterProperty); }
      set { this.SetValue(BlobMinDiameterProperty, value); }
    }

    public static readonly DependencyProperty BlobMinDiameterProperty = DependencyProperty.Register(
      "BlobMinDiameter",
      typeof(double),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        4d,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public double BlobMaxDiameter
    {
      get { return (double)this.GetValue(BlobMaxDiameterProperty); }
      set { this.SetValue(BlobMaxDiameterProperty, value); }
    }

    public static readonly DependencyProperty BlobMaxDiameterProperty = DependencyProperty.Register(
      "BlobMaxDiameter",
      typeof(double),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        100d,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public Color TargetColor
    {
      get { return (Color)this.GetValue(TargetColorProperty); }
      set { this.SetValue(TargetColorProperty, value); }
    }

    public static readonly DependencyProperty TargetColorProperty = DependencyProperty.Register(
      "TargetColor",
      typeof(Color),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        Colors.Red,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

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

    public Point? CurrentBlobCenter
    {
      get { return (Point?)this.GetValue(CurrentBlobCenterProperty); }
      set { this.SetValue(CurrentBlobCenterProperty, value); }
    }

    public static readonly DependencyProperty CurrentBlobCenterProperty = DependencyProperty.Register(
      "CurrentBlobCenter",
      typeof(Point?),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        null,
        new PropertyChangedCallback(OnPropertyChanged)));

    public Segment DetectedBlob
    {
      get { return (Segment)this.GetValue(DetectedBlobsProperty); }
      set { this.SetValue(DetectedBlobsProperty, value); }
    }

    public static readonly DependencyProperty DetectedBlobsProperty = DependencyProperty.Register(
      "DetectedBlob",
      typeof(Segment),
      typeof(ImageProcessing),
      new FrameworkPropertyMetadata(
        new Segment(),
        new PropertyChangedCallback(OnPropertyChanged)));

    private Stopwatch watch = new Stopwatch();

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

      watch.Start();
      var start = watch.ElapsedMilliseconds;

      //Console.Write("BeforeColorFilter: ");
      //Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      this.colorAndCropFilter.ProcessInPlace(Video.Instance.VideoElement.ProcessingMapping);
      //this.colorRangeFilter.ProcessInPlace(Video.Instance.VideoElement.ProcessingMapping);

      //Console.Write("AfterColorFilter: ");
      //Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      //this.blobDetection.ProcessInPlace(Video.Instance.VideoElement.Map);
      //Console.Write("AfterBlobDetection: ");
      //Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      //this.DetectedBlobs = this.blobDetection.Blobs;

      // Segment
      var histogram = this.histogrammFilter.FromIntPtrMap(Video.Instance.VideoElement.ProcessingMapping);
      segmentator.Histogram = histogram;
      segmentator.ThresholdLuminance = histogram.Max * 0.1f;
      var foundSegment = segmentator.Process();
      this.DetectedBlob = foundSegment;
      //Console.Write("AfterBlobDetection: ");
      //Console.WriteLine(watch.ElapsedMilliseconds.ToString());

      if (foundSegment.Diagonal != 0 &&
        (foundSegment.Height < (this.colorAndCropFilter.ImageHeight - 10)) &&
        (foundSegment.Width < (this.colorAndCropFilter.ImageWidth - 10)))
      {
        this.CurrentBlobCenter = new Point(
          foundSegment.Center.X,
          foundSegment.Center.Y);

        if (Video.Instance.IsDataAcquisitionRunning)
        {
          VideoData.Instance.AddPoint(0, this.CurrentBlobCenter.Value);
        }

        StatusBarContent.Instance.MessagesLabel = Localization.Labels.BlobsObjectsFound;
        this.isReady = true;
      }
      else
      {
        this.CurrentBlobCenter = null;
        StatusBarContent.Instance.MessagesLabel = Localization.Labels.BlobsNoObjectFound;
        this.isReady = true;
        return false;
      }

      //Console.Write("Finished: ");
      //Console.WriteLine(watch.ElapsedMilliseconds.ToString());
      totalProcessingTime += this.watch.ElapsedMilliseconds - start;
      counter++;

      Console.Write("AverageProcessingTime: ");
      Console.WriteLine(totalProcessingTime / counter);
      return true;
    }

    int counter;
    long totalProcessingTime;

  }
}
