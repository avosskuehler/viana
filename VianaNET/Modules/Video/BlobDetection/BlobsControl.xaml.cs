using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.ComponentModel;
using System.Windows.Data;
using VianaNETShaderEffectLibrary;
using System.Windows.Interop;

namespace VianaNET
{
  /// <summary>
  /// Interaction logic for BlobsControl.xaml
  /// </summary>
  public partial class BlobsControl : UserControl
  {

    //    Registering to a DependencyProperty's Change Event
    //DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(   
    // MyType.MyDependencyProperty,    typeof(MyType));prop.AddValueChanged(this, this.OnMyDependencyPropertyChanged);

    //private System.Drawing.Bitmap filteredImage;
    //private HistogramMinMaxSegmentator segmentator;
    private BlobDetectionFilter blobDetection;
    private ColorFilter colorFilter;

    public System.Drawing.Bitmap NativeBitmap
    {
      get { return (System.Drawing.Bitmap)GetValue(NativeBitmapProperty); }
      set { SetValue(NativeBitmapProperty, value); }
    }

    public static readonly DependencyProperty NativeBitmapProperty =
      DependencyProperty.Register(
      "NativeBitmap",
      typeof(System.Drawing.Bitmap),
      typeof(BlobsControl), new PropertyMetadata(null, new PropertyChangedCallback(OnNativeBitmapChanged)));

    private static void OnNativeBitmapChanged(
DependencyObject obj,
DependencyPropertyChangedEventArgs args)
    {
      (obj as BlobsControl).UpdatedProcessedImage();
    }

    //public delegate void BlobSelectionHandler(object sender, Blob blob);
    private ThresholdEffect thresholdEffect;
    private BackgroundEffect backgroundEffect;

    public BlobsControl()
    {
      this.DataContext = this;
      InitializeComponent();
      Calibration.Instance.CalibrationPropertyChanged +=
        new PropertyChangedEventHandler(Calibration_PropertyChanged);
      Video.Instance.VideoFrameChanged += new EventHandler(OnVideoFrameChanged);
      //Binding bitmapBinding = new Binding();
      //bitmapBinding.Source = Video.Instance;
      //bitmapBinding.Path = new PropertyPath("CurrentFrameBitmap");
      //this.SetBinding(BlobsControl.NativeBitmapProperty, bitmapBinding);

      //Binding bitmapBinding2 = new Binding();
      //bitmapBinding2.Source = VideoCapturer.Instance;
      //bitmapBinding2.Path = new PropertyPath("CurrentFrameBitmap");
      //this.SetBinding(BlobsControl.NativeBitmapProperty, bitmapBinding2);

      //Binding bitmapBinding2 = new Binding();
      //bitmapBinding2.Source = VideoCapturer.Instance;
      //bitmapBinding2.Path = new PropertyPath("CurrentFrameBitmap");
      //this.SetBinding(BlobsControl.NativeBitmapProperty, bitmapBinding2);

      thresholdEffect = new ThresholdEffect();
      thresholdEffect.Threshold = 0.3;
      thresholdEffect.BlankColor = Colors.Green;
      thresholdEffect.TargetColor = Colors.Red;

      backgroundEffect = new BackgroundEffect();
      backgroundEffect.Threshold = 0.3;

      this.ProcessedImageControl.Effect = thresholdEffect;
      this.OverlayImageControl.Effect = backgroundEffect;
      this.blobDetection = new BlobDetectionFilter();
      this.colorFilter = new ColorFilter();

      //this.CanvasHulls.Children.Add(blobEllipse);

      InitializeImageFilters();
    }

    private void InitializeImageFilters()
    {
      this.blobDetection.ImageWidth = (int)Video.Instance.VideoElement.NaturalVideoWidth;
      this.blobDetection.ImageHeight = (int)Video.Instance.VideoElement.NaturalVideoHeight;
      this.blobDetection.ImageStride = Video.Instance.VideoElement.Stride;
      this.blobDetection.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.blobDetection.FilterBlobs = true;
      this.blobDetection.CoupledSizeFiltering = true;
      this.blobDetection.MinHeight = (int)Calibration.Instance.BlobMinDiameter;
      this.blobDetection.MinWidth = (int)Calibration.Instance.BlobMinDiameter;
      this.blobDetection.MaxWidth = (int)Calibration.Instance.BlobMaxDiameter;
      this.blobDetection.MaxHeight = (int)Calibration.Instance.BlobMaxDiameter;
      this.blobDetection.ObjectsOrder = ObjectsOrder.Area;

      this.blobCounter.FilterBlobs = true;
      this.blobCounter.CoupledSizeFiltering = true;
      this.blobCounter.MinHeight = (int)Calibration.Instance.BlobMinDiameter;
      this.blobCounter.MinWidth = (int)Calibration.Instance.BlobMinDiameter;
      this.blobCounter.MaxWidth = (int)Calibration.Instance.BlobMaxDiameter;
      this.blobCounter.MaxHeight = (int)Calibration.Instance.BlobMaxDiameter;
      this.blobCounter.ObjectsOrder =AForge.Imaging.ObjectsOrder.Area;

      this.colorFilter.ImageWidth = (int)Video.Instance.VideoElement.NaturalVideoWidth;
      this.colorFilter.ImageHeight = (int)Video.Instance.VideoElement.NaturalVideoHeight;
      this.colorFilter.ImageStride = Video.Instance.VideoElement.Stride;
      this.colorFilter.ImagePixelSize = Video.Instance.VideoElement.PixelSize;
      this.colorFilter.BlankColor = Colors.Green;
      this.colorFilter.TargetColor = Calibration.Instance.TargetColor;
      this.colorFilter.Threshold = (int)(Calibration.Instance.ColorThreshold * 255);

      thresholdEffect.Threshold = Calibration.Instance.ColorThreshold;
      thresholdEffect.TargetColor = Calibration.Instance.TargetColor;
    }

    void OnVideoFrameChanged(object sender, EventArgs e)
    {
      if (this.blobDetection.ImageWidth >= 1)
      {
        this.UpdatedProcessedImage();
      }
    }

    void Calibration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "BlobDetection")
      {
        this.InitializeImageFilters();
      }
    }


    //public double XScale
    //{
    //  get { return (double)GetValue(XScaleProperty); }
    //  set { SetValue(XScaleProperty, value); }
    //}

    //public static readonly DependencyProperty XScaleProperty =
    //  DependencyProperty.Register("XScale", typeof(double), typeof(BlobsControl), new PropertyMetadata(1d));

    //public double YScale
    //{
    //  get { return (double)GetValue(YScaleProperty); }
    //  set { SetValue(YScaleProperty, value); }
    //}

    //public static readonly DependencyProperty YScaleProperty =
    //  DependencyProperty.Register("YScale", typeof(double), typeof(BlobsControl), new PropertyMetadata(1d));

    public ImageSource ProcessedImage
    {
      get { return (ImageSource)GetValue(ProcessedImageProperty); }
      set { SetValue(ProcessedImageProperty, value); }
    }

    public static readonly DependencyProperty ProcessedImageProperty =
      DependencyProperty.Register("ProcessedImage", typeof(ImageSource), typeof(BlobsControl));

    //public void UpdatedProcessedImage()
    //{
    //  //if (this.NativeBitmap == null)
    //  //{
    //  //  return;
    //  //}

    //  // Do not process if we are invisible
    //  if (this.Visibility != Visibility.Visible)
    //  {
    //    return;
    //  }

    //  this.filteredImage = AForge.Imaging.Image.Clone(
    //    this.NativeBitmap,
    //    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

    //  // Flip video image of capture
    //  switch (Video.Instance.VideoMode)
    //  {
    //    case VideoMode.File:
    //      break;
    //    case VideoMode.Capture:
    //      this.filteredImage.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
    //      break;
    //  }

    //  if (!Calibration.Instance.ClipRegion.IsEmpty)
    //  {
    //    // Preprocess image with search area filter
    //    CanvasCrop cropFilter = new CanvasCrop(
    //      Calibration.Instance.SystemDrawingClipRegion,
    //      System.Drawing.Color.Black);

    //    // apply the filter
    //    cropFilter.ApplyInPlace(this.filteredImage);
    //  }

    //  // create color search filter
    //  ColorFiltering filter = new ColorFiltering();
    //  Color targetColor = Calibration.Instance.TargetColor;

    //  // set color ranges to keep
    //  filter.Red = ApplyTolerance(targetColor.R);
    //  filter.Green = ApplyTolerance(targetColor.G);
    //  filter.Blue = ApplyTolerance(targetColor.B);

    //  // apply the filter
    //  filter.ApplyInPlace(this.filteredImage);

    //  this.ProcessedImage = BitmapToSource(this.filteredImage);
    //  this.UpdateScale();

    //  this.SetImage(this.filteredImage);
    //}

    //public ColorRangeFilter SkinColorFilter { get; set; }

    //Ellipse blobEllipse = new Ellipse();
    private BlobCounter blobCounter = new BlobCounter();

    private void UpdatedProcessedImage()
    {
      // Do not process if we are invisible
      if (this.Visibility != Visibility.Visible)
      {
        return;
      }

      // create color search filter
      //this.colorFilter.TargetColor = Colors.Red;
      //this.colorFilter.Threshold = 60;// (int)Calibration.Instance.ColorTolerance;
      //this.colorFilter.BlankColor = Colors.Black;
      //this.colorFilter.ProcessInPlace(Video.Instance.VideoElement.Map);
      //BitmapSource bmp = Imaging.CreateBitmapSourceFromMemorySection(
      //  Video.Instance.VideoElement.section,
      //  (int)Video.Instance.VideoElement.NaturalVideoWidth, 
      //  (int)Video.Instance.VideoElement.NaturalVideoHeight,
      //  PixelFormats.Bgra32, Video.Instance.VideoElement.Stride, 0);

      //using (FileStream outStream = new FileStream(@"c:\Dumps\test.jpg",FileMode.Create))
      //{
      //  PngBitmapEncoder pnge = new PngBitmapEncoder();
      //  pnge.Frames.Add(BitmapFrame.Create(bmp));
      //  pnge.Save(outStream);
      //}
      System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
        (int)Video.Instance.VideoElement.NaturalVideoWidth,
        (int)Video.Instance.VideoElement.NaturalVideoHeight,
        Video.Instance.VideoElement.Stride,
        System.Drawing.Imaging.PixelFormat.Format32bppArgb,
        Video.Instance.VideoElement.Map);

      // create color search filter
      ColorFiltering filter = new ColorFiltering();
      Color targetColor = Calibration.Instance.TargetColor;

      // set color ranges to keep
      filter.Red = ApplyTolerance(targetColor.R);
      filter.Green = ApplyTolerance(targetColor.G);
      filter.Blue = ApplyTolerance(targetColor.B);

      // apply the filter
      filter.ApplyInPlace(bmp);

      blobCounter.ProcessImage(bmp);
      AForge.Imaging.Blob[] blobs = blobCounter.GetObjectsInformation();

      double scaleX;
      double scaleY;
      if (GetScales(out scaleX, out scaleY))
      {
        foreach (AForge.Imaging.Blob blob in blobs)
        {
          BlobEllipse.Width = blob.Rectangle.Width * scaleX;
          BlobEllipse.Height = blob.Rectangle.Height * scaleY;
          Canvas.SetLeft(BlobEllipse, blob.Rectangle.Left * scaleX);
          Canvas.SetTop(BlobEllipse, blob.Rectangle.Top * scaleY);

          //  //if (Video.Instance.IsDataAcquisitionRunning)
          {
            double blobX = blob.CenterOfGravity.X;
            double blobY = blob.CenterOfGravity.Y;

            // Flip y coordinate in video file mode.
            switch (Video.Instance.VideoMode)
            {
              case VideoMode.File:
                blobY = Video.Instance.VideoElement.NaturalVideoHeight - blobY;
                break;
              case VideoMode.Capture:
                break;
            }

            VideoData.Instance.AddPoint(new Point(blobX, blobY));
          }

        }
      }


      // Get video memory of new bitmap
      //this.blobDetection.ProcessInPlace(Video.Instance.VideoElement.Map);
      //this.CanvasHulls.Children.Clear();
      //foreach (Blob blob in this.blobDetection.GetObjects(Video.Instance.VideoElement.Map))
      //{
      //  blob.Image.Save(@"c:\Dumps\test.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
      //}

      ////GrahamConvexHull grahamScan = new GrahamConvexHull();
      //foreach (Blob blob in this.blobDetection.Blobs)
      //{
      //  //BlobEllipse.Stroke = Brushes.Red;
      //  //blobEllipse.StrokeThickness = 3;
      //  double scaleX;
      //  double scaleY;

      //  if (GetScales(out scaleX, out scaleY))
      //  {
      //    BlobEllipse.Width = blob.Rectangle.Width * scaleX;
      //    BlobEllipse.Height = blob.Rectangle.Height * scaleY;
      //    Canvas.SetLeft(BlobEllipse, blob.Rectangle.Left * scaleX);
      //    Canvas.SetTop(BlobEllipse, blob.Rectangle.Top * scaleY);
      //  }

      //  //if (Video.Instance.IsDataAcquisitionRunning)
      //  {
      //    double blobX = blob.CenterOfGravity.X;
      //    double blobY = blob.CenterOfGravity.Y;

      //    // Flip y coordinate in video file mode.
      //    switch (Video.Instance.VideoMode)
      //    {
      //      case VideoMode.File:
      //        blobY = Video.Instance.VideoElement.NaturalVideoHeight - blobY;
      //        break;
      //      case VideoMode.Capture:
      //        break;
      //    }

      //    VideoData.Instance.AddPoint(new Point(blobX, blobY));
      //  }
      //}

      if (blobs.Length == 0)
      {
        StatusBarContent.Instance.MessagesLabel = Localization.Labels.BlobsNoObjectFound;
      }
      else
      {
        StatusBarContent.Instance.MessagesLabel =
          blobs.Length.ToString() + " " + Localization.Labels.BlobsObjectsFound;
      }

      //this.UpdateScale();
    }

    //public void UpdatedProcessedImage()
    //{
    //  // Do not process if we are invisible
    //  if (this.Visibility != Visibility.Visible)
    //  {
    //    return;
    //  }

    //  this.filteredImage = AForge.Imaging.Image.Clone(
    //    this.NativeBitmap,
    //    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

    //  // Flip video image of capture
    //  switch (Video.Instance.VideoMode)
    //  {
    //    case VideoMode.File:
    //      break;
    //    case VideoMode.Capture:
    //      this.filteredImage.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
    //      break;
    //  }

    //  if (!Calibration.Instance.ClipRegion.IsEmpty)
    //  {
    //    // Preprocess image with search area filter
    //    CanvasCrop cropFilter = new CanvasCrop(
    //      Calibration.Instance.SystemDrawingClipRegion,
    //      System.Drawing.Color.Black);

    //    // apply the filter
    //    cropFilter.ApplyInPlace(this.filteredImage);
    //  }

    //  // create color search filter
    //  ColorFiltering filter = new ColorFiltering();
    //  Color targetColor = Calibration.Instance.TargetColor;

    //  // set color ranges to keep
    //  filter.Red = ApplyTolerance(targetColor.R);
    //  filter.Green = ApplyTolerance(targetColor.G);
    //  filter.Blue = ApplyTolerance(targetColor.B);

    //  // apply the filter
    //  filter.ApplyInPlace(this.filteredImage);

    //  this.ProcessedImage = BitmapToSource(this.filteredImage);
    //  this.UpdateScale();

    //  this.SetImage(this.filteredImage);
    //}

    private IntRange ApplyTolerance(byte color)
    {
      double tolerance = Calibration.Instance.ColorThreshold*255;
      int min = (int)color - (int)tolerance;
      int max = (int)color + (int)tolerance;

      if (min < 0)
      {
        min = 0;
      }

      if (max > 255)
      {
        max = 255;
      }

      return new IntRange(min, max);
    }

    //private BlobCounter blobCounter = new BlobCounter();
    //private Blob[] blobs;
    ////private int selectedBlobID;

    //Dictionary<int, List<IntPoint>> hulls = new Dictionary<int, List<IntPoint>>();

    //// Event to notify about selected blob
    //public event BlobSelectionHandler BlobSelected;


    //// Set image to display by the control
    //private int SetImage(System.Drawing.Bitmap image)
    //{
    //  this.CanvasHulls.Children.Clear();
    //  //hulls.Clear();

    //  //this.image = AForge.Imaging.Image.Clone(image, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
    //  try
    //  {
    //    blobCounter.FilterBlobs = true;
    //    blobCounter.CoupledSizeFiltering = false;
    //    blobCounter.MinHeight = (int)Calibration.Instance.BlobMinDiameter;
    //    blobCounter.MinWidth = (int)Calibration.Instance.BlobMinDiameter;
    //    blobCounter.MaxWidth = (int)Calibration.Instance.BlobMaxDiameter;
    //    blobCounter.MaxHeight = (int)Calibration.Instance.BlobMaxDiameter;
    //    blobCounter.ObjectsOrder = ObjectsOrder.Area;
    //    blobCounter.ProcessImage(image);
    //    blobs = blobCounter.GetObjectsInformation();

    //    //GrahamConvexHull grahamScan = new GrahamConvexHull();
    //    foreach (Blob blob in blobs)
    //    {
    //      Ellipse blobEllipse = new Ellipse();
    //      blobEllipse.Stroke = Brushes.Red;
    //      blobEllipse.StrokeThickness = 3;
    //      blobEllipse.Width = blob.Rectangle.Width;
    //      blobEllipse.Height = blob.Rectangle.Height;
    //      this.CanvasHulls.Children.Add(blobEllipse);
    //      Canvas.SetTop(blobEllipse, blob.Rectangle.Top);
    //      Canvas.SetLeft(blobEllipse, blob.Rectangle.Left);
    //      //Polygon blobPolygon = new Polygon();
    //      //blobPolygon.Stroke = Brushes.Red;
    //      //blobPolygon.StrokeThickness = 3;
    //      //List<IntPoint> quadrilateral = PointsCloud.FindQuadrilateralCorners(blobCounter.GetBlobsEdgePoints(blob));
    //      //blobPolygon.Points = PointsListToArray(quadrilateral);
    //      //this.CanvasHulls.Children.Add(blobPolygon);

    //      //List<IntPoint> leftEdge = new List<IntPoint>();
    //      //List<IntPoint> rightEdge = new List<IntPoint>();
    //      //List<IntPoint> topEdge = new List<IntPoint>();
    //      //List<IntPoint> bottomEdge = new List<IntPoint>();

    //      //// collect edge points
    //      //blobCounter.GetBlobsLeftAndRightEdges(blob, out leftEdge, out rightEdge);
    //      //blobCounter.GetBlobsTopAndBottomEdges(blob, out topEdge, out bottomEdge);

    //      //// find convex hull
    //      //List<IntPoint> edgePoints = new List<IntPoint>();
    //      //edgePoints.AddRange(leftEdge);
    //      //edgePoints.AddRange(rightEdge);

    //      //List<IntPoint> hull = grahamScan.FindHull(edgePoints);
    //      //hulls.Add(blob.ID, hull);

    //      //// shift all points for vizualization
    //      //IntPoint shift = new IntPoint(1, 1);

    //      //PointsCloud.Shift(hull, shift);

    //      //this.BlobHull.Points = PointsListToArray(hulls[blob.ID]);

    //      if (Video.Instance.IsDataAcquisitionRunning)
    //      {
    //        double blobX = blob.CenterOfGravity.X;
    //        double blobY = blob.CenterOfGravity.Y;

    //        // Flip y coordinate in video file mode.
    //        switch (Video.Instance.VideoMode)
    //        {
    //          case VideoMode.File:
    //            blobY = this.NativeBitmap.Height - blobY;
    //            break;
    //          case VideoMode.Capture:
    //            break;
    //        }

    //        VideoData.Instance.AddPoint(new Point(blobX, blobY));
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    VianaDialog message = new VianaDialog("Error", "Blob detection failed", ex.Message);
    //    message.ShowDialog();
    //  }

    //  if (blobs.Length == 0)
    //  {
    //    StatusBarContent.Instance.MessagesLabel = Localization.Labels.BlobsNoObjectFound;
    //  }
    //  else
    //  {
    //    StatusBarContent.Instance.MessagesLabel =
    //      blobs.Length.ToString() + " " + Localization.Labels.BlobsObjectsFound;
    //  }

    //  return blobs.Length;
    //}

    //// On mouse moving - update cursor
    //private void BlobsBrowser_MouseMove(object sender, MouseEventArgs e)
    //{
    //  int x = e.X - 1;
    //  int y = e.Y - 1;

    //  if ((image != null) && (x >= 0) && (y >= 0) &&
    //       (x < imageWidth) && (y < imageHeight) &&
    //       (blobCounter.ObjectLabels[y * imageWidth + x] != 0))
    //  {
    //    this.Cursor = Cursors.Hand;
    //  }
    //  else
    //  {
    //    this.Cursor = Cursors.Default;
    //  }
    //}

    //// On mouse click - notify user if blob was clicked
    //private void BlobsBrowser_MouseClick(object sender, MouseEventArgs e)
    //{
    //  int x = e.X - 1;
    //  int y = e.Y - 1;

    //  if ((image != null) && (x >= 0) && (y >= 0) &&
    //       (x < imageWidth) && (y < imageHeight))
    //  {
    //    int blobID = blobCounter.ObjectLabels[y * imageWidth + x];

    //    if (blobID != 0)
    //    {
    //      selectedBlobID = blobID;
    //      Invalidate();

    //      if (BlobSelected != null)
    //      {
    //        for (int i = 0; i < blobs.Length; i++)
    //        {
    //          if (blobs[i].ID == blobID)
    //          {
    //            BlobSelected(this, blobs[i]);
    //          }
    //        }
    //      }
    //    }
    //  }
    //}

    //// Convert list of AForge.NET's IntPoint to array of .NET's Point
    //private PointCollection PointsListToArray(List<IntPoint> list)
    //{
    //  PointCollection array = new PointCollection(list.Count);

    //  for (int i = 0, n = list.Count; i < n; i++)
    //  {
    //    array.Add(new Point(list[i].X, list[i].Y));
    //  }

    //  return array;
    //}


    //public static System.Drawing.Bitmap BitmapFromSource(ImageSource imageSource)
    //{
    //  System.Drawing.Bitmap bitmap;

    //  // craete and render surface and push bitmap to it
    //  RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)imageSource.Width, (int)imageSource.Height, 96d, 96d, PixelFormats.Pbgra32);
    //  // now render surface to bitmap
    //  renderBitmap.Render(imageSource);

    //  using (MemoryStream outStream = new MemoryStream())
    //  {
    //    // from System.Media.BitmapImage to System.Drawing.Bitmap 
    //    BitmapEncoder enc = new BmpBitmapEncoder();
    //    enc.Frames.Add(BitmapFrame.Create(bitmapsource));
    //    enc.Save(outStream);
    //    bitmap = new System.Drawing.Bitmap(outStream);
    //  }

    //  return bitmap;
    //}

    ///// <summary>
    ///// Converts a given <see cref="BitmapSource"/> into a <see cref="System.Drawing.Bitmap"/>
    ///// </summary>
    ///// <param name="bitmapsource">The <see cref="BitmapSource"/> to be converted.</param>
    ///// <returns>An exact copy of the <see cref="BitmapSource"/> as a <see cref="System.Drawing.Bitmap"/></returns>
    //public static System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
    //{
    //  System.Drawing.Bitmap bitmap;
    //  using (MemoryStream outStream = new MemoryStream())
    //  {
    //    // from System.Media.BitmapImage to System.Drawing.Bitmap 
    //    BitmapEncoder enc = new BmpBitmapEncoder();
    //    enc.Frames.Add(BitmapFrame.Create(bitmapsource));
    //    enc.Save(outStream);
    //    bitmap = new System.Drawing.Bitmap(outStream);
    //  }

    //  return bitmap;
    //}

    //[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    //public static extern IntPtr DeleteObject(IntPtr hDc);

    ///// <summary>
    ///// Converts a given <see cref="System.Drawing.Bitmap"/> into a <see cref="BitmapSource"/>
    ///// </summary>
    ///// <param name="bitmap">The <see cref="System.Drawing.Bitmap"/> to be converted.</param>
    ///// <returns>An exact copy of the <see cref="System.Drawing.Bitmap"/> as a <see cref="BitmapSource"/></returns>
    //public static BitmapSource BitmapToSource(System.Drawing.Bitmap bitmap)
    //{
    //  BitmapSource destination;
    //  IntPtr bitmapPointer = bitmap.GetHbitmap();
    //  BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
    //  destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapPointer, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
    //  destination.Freeze();

    //  // Must delete int pointer to avoid memory leak
    //  DeleteObject(bitmapPointer);

    //  return destination;
    //}

    //private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    //{
    //  UpdateScale();
    //}

    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = this.OverlayImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.OverlayImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return (!double.IsInfinity(scaleX) && !double.IsNaN(scaleX));
    }


    //public void UpdateScale()
    //{
    //  if (Video.Instance.VideoElement != null)
    //  {
    //    this.XScale = this.ProcessedImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
    //    this.YScale = this.ProcessedImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;
    //    //this.CanvasHulls.RenderTransform = new ScaleTransform(this.XScale, this.YScale);
    //    //this.CanvasHulls.RenderTransformOrigin = new Point(1, 1);
    //  }
    //}
  }
}
