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
using System.ComponentModel;
using System.Windows.Data;
using VianaNETShaderEffectLibrary;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Media.Effects;

namespace VianaNET
{
  /// <summary>
  /// Interaction logic for BlobsControl.xaml
  /// </summary>
  public partial class BlobsControl : UserControl
  {
    private ThresholdEffect thresholdEffect;
    private BackgroundEffect backgroundEffect;

    public BlobsControl()
    {
      this.DataContext = this;
      this.InitializeComponent();

      this.thresholdEffect = new ThresholdEffect();
      thresholdEffect.MinX = 0.001d;
      thresholdEffect.MaxX = 0.999d;
      thresholdEffect.MinY = 0.001d;
      thresholdEffect.MaxY = 0.999d;

      this.backgroundEffect = new BackgroundEffect();
      this.backgroundEffect.Threshold = 0.4;

      this.ProcessedImageControl.Effect = this.thresholdEffect;
      this.OverlayImageControl.Effect = this.backgroundEffect;
      Video.Instance.ImageProcessing.PropertyChanged += new PropertyChangedEventHandler(ImageProcessing_PropertyChanged);
      Calibration.Instance.PropertyChanged += new PropertyChangedEventHandler(Calibration_PropertyChanged);
      VideoData.Instance.PropertyChanged += new PropertyChangedEventHandler(VideoData_PropertyChanged);
      Video.Instance.PropertyChanged += new PropertyChangedEventHandler(Video_PropertyChanged);
    }

    void Video_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsDataAcquisitionRunning")
      {
        if (Video.Instance.IsDataAcquisitionRunning)
        {
          this.CanvasDataPoints.Children.Clear();
        }
      }
    }

    void VideoData_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        Ellipse dataPoint = new Ellipse();
        dataPoint.Stroke = Brushes.Red;
        dataPoint.StrokeThickness = 2;
        dataPoint.Width = 15;
        dataPoint.Height = 15;
        Point location = VideoData.Instance.LastPoint;
        this.CanvasDataPoints.Children.Add(dataPoint);
        Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
        Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
      }

      //this.CanvasDataPoints.Children.Clear();
    }

    public void UpdateDataPoints()
    {
      this.CanvasDataPoints.Children.Clear();
      foreach (DataSample sample in VideoData.Instance.Samples)
      {
        Ellipse dataPoint = new Ellipse();
        dataPoint.Stroke = Brushes.Red;
        dataPoint.StrokeThickness = 2;
        dataPoint.Width = 15;
        dataPoint.Height = 15;
        Point location = new Point(sample.CoordinateX, sample.CoordinateY);
        this.CanvasDataPoints.Children.Add(dataPoint);
        Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
        Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
      }
    }

    void Calibration_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" ||
        e.PropertyName == "ClipRegion")
      {
        double videoWidth = Video.Instance.VideoElement.NaturalVideoWidth;
        double videoHeight = Video.Instance.VideoElement.NaturalVideoHeight;
        if (Calibration.Instance.HasClipRegion)
        {
          thresholdEffect.MinX = Calibration.Instance.ClipRegion.Left / videoWidth;
          thresholdEffect.MaxX = Calibration.Instance.ClipRegion.Right / videoWidth;
          thresholdEffect.MinY = Calibration.Instance.ClipRegion.Top / videoHeight;
          thresholdEffect.MaxY = Calibration.Instance.ClipRegion.Bottom / videoHeight;
        }
        else
        {
          thresholdEffect.MinX = 0d;
          thresholdEffect.MaxX = 1d;
          thresholdEffect.MinY = 0d;
          thresholdEffect.MaxY = 1d;
        }

        this.SetBlobs();
      }
    }

    void ImageProcessing_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "TargetColor" ||
        e.PropertyName == "ColorThreshold")
      {
        thresholdEffect.Threshold = Video.Instance.ImageProcessing.ColorThreshold;
        thresholdEffect.TargetColor = Video.Instance.ImageProcessing.TargetColor;
        thresholdEffect.BlankColor = Colors.Black;
        thresholdEffect.CropColor = Colors.DarkGray;
      }

      this.SetBlobs();
    }

    private void SetBlobs()
    {
      if (this.Visibility != Visibility.Visible)
      {
        return;
      }

      this.OverlayCanvas.Children.Clear();

      double scaleX;
      double scaleY;

      if (GetScales(out scaleX, out scaleY))
      {
        Segment blob = Video.Instance.ImageProcessing.DetectedBlob;

        if (blob.Height < Video.Instance.VideoElement.NaturalVideoHeight - 10 &&
            blob.Width < Video.Instance.VideoElement.NaturalVideoWidth - 10)
        {
          Ellipse blobEllipse = new Ellipse();
          blobEllipse.Stroke = Brushes.Red;
          blobEllipse.StrokeThickness = 2;
          blobEllipse.Width = blob.Width * scaleX;
          blobEllipse.Height = blob.Height * scaleY;
          this.OverlayCanvas.Children.Add(blobEllipse);

          Canvas.SetLeft(
            blobEllipse,
            blob.Center.X * scaleX - blobEllipse.Width / 2);
          Canvas.SetTop(
            blobEllipse,
            blob.Center.Y * scaleY - blobEllipse.Height / 2);
        }
      }
    }

    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = this.OverlayImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.OverlayImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return (!double.IsInfinity(scaleX) && !double.IsNaN(scaleX));
    }

    private void BlobsControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.SetBlobs();
    }

    private void CanvasDataPoints_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ScaleTransform videoSizeToCanvasSize = new ScaleTransform();
      videoSizeToCanvasSize.ScaleX =
        this.CanvasDataPoints.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      videoSizeToCanvasSize.ScaleY =
        this.CanvasDataPoints.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;
      this.CanvasDataPoints.RenderTransform = videoSizeToCanvasSize;
    }
  }
}
