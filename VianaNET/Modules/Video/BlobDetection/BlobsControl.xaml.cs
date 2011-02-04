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
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private ThresholdEffect thresholdEffect;
    private BackgroundEffect backgroundEffect;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

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

      //this.PopulateObjectCombo();

      Video.Instance.ImageProcessing.PropertyChanged += new PropertyChangedEventHandler(ImageProcessing_PropertyChanged);
      Calibration.Instance.PropertyChanged += new PropertyChangedEventHandler(Calibration_PropertyChanged);
      VideoData.Instance.PropertyChanged += new PropertyChangedEventHandler(VideoData_PropertyChanged);
      Video.Instance.PropertyChanged += new PropertyChangedEventHandler(Video_PropertyChanged);
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES
    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void UpdateDataPoints()
    {
      this.CanvasDataPoints.Children.Clear();
      foreach (TimeSample sample in VideoData.Instance.Samples)
      {
        for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
        {
          Ellipse dataPoint = new Ellipse();
          dataPoint.Stroke = ImageProcessing.TrackObjectColors[i];
          dataPoint.StrokeThickness = 2;
          dataPoint.Width = 15;
          dataPoint.Height = 15;
          Point location = new Point(sample.Object[i].PositionX, sample.Object[i].PositionY);
          this.CanvasDataPoints.Children.Add(dataPoint);
          Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
          Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
        }
      }
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">The source of the event. This.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> with 
    /// the event data.</param>
    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      BlobsControl window = obj as BlobsControl;
      window.UpdateThresholdForHLSL();
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
        for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
        {
          Ellipse dataPoint = new Ellipse();
          dataPoint.Stroke = ImageProcessing.TrackObjectColors[i];
          dataPoint.StrokeThickness = 2;
          dataPoint.Width = 15;
          dataPoint.Height = 15;
          Point location = VideoData.Instance.LastPoint[i];
          this.CanvasDataPoints.Children.Add(dataPoint);
          Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
          Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
        }
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
        e.PropertyName == "HLSLParams" ||
        e.PropertyName == "IndexOfObject")
      {
        UpdateThresholdForHLSL();
      }

      this.SetBlobs();
    }

    private void UpdateThresholdForHLSL()
    {
      thresholdEffect.Threshold =
        Video.Instance.ImageProcessing.ColorThreshold[Video.Instance.ImageProcessing.IndexOfObject] / 255d;
      thresholdEffect.BlankColor = Colors.Black;
      thresholdEffect.CropColor = Colors.DarkGray;
      thresholdEffect.TargetColor =
        Video.Instance.ImageProcessing.TargetColor[Video.Instance.ImageProcessing.IndexOfObject];
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

    //private void ObjectSelectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //  if (this.ObjectSelectionCombo.SelectedItem != null)
    //  {
    //    string entry = (string)this.ObjectSelectionCombo.SelectedItem;
    //    this.IndexOfObject = Int32.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
    //    this.thresholdEffect.TargetColor = Video.Instance.ImageProcessing.TargetColor[this.IndexOfObject];
    //  }
    //}

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
        for (int i = 0; i < Video.Instance.ImageProcessing.DetectedBlob.Count; i++)
        {
          Segment blob = Video.Instance.ImageProcessing.DetectedBlob[i];

          if (blob.Height < Video.Instance.VideoElement.NaturalVideoHeight - 10 &&
              blob.Width < Video.Instance.VideoElement.NaturalVideoWidth - 10 &&
              blob.Diagonal > 0)
          {
            Ellipse blobEllipse = new Ellipse();
            blobEllipse.Fill = ImageProcessing.TrackObjectColors[i];
            blobEllipse.Stroke=Brushes.Black;//ImageProcessing.TrackObjectColors[i];
            blobEllipse.StrokeThickness = 1;
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
    }

    //private void PopulateObjectCombo()
    //{
    //  // Erase old entries
    //  this.ObjectDescriptions.Clear();

    //  for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
    //  {
    //    this.ObjectDescriptions.Add(Localization.Labels.DataGridObjectPrefix + " " + (i + 1).ToString());
    //  }

    //  this.ObjectSelectionCombo.ItemsSource = null;
    //  this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
    //  this.ObjectSelectionCombo.SelectedIndex = 0;
    //}

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = this.OverlayImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.OverlayImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return (!double.IsInfinity(scaleX) && !double.IsNaN(scaleX));
    }

    #endregion //HELPER
  }
}
