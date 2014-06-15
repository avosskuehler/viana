// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobsControl.xaml.cs" company="Freie Universität Berlin">
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
namespace VianaNET.Modules.Video.BlobDetection
{
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using VianaNET.Application;
  using VianaNET.Data.Collections;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Filter;

  /// <summary>
  ///   Interaction logic for BlobsControl.xaml
  /// </summary>
  public partial class BlobsControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="BlobsControl" /> class.
    /// </summary>
    public BlobsControl()
    {
      this.DataContext = this;
      this.InitializeComponent();
      this.WirePropertyChangedEvents();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Resets the whole control with the existing data
    /// </summary>
    public void ResetBlobsControl()
    {
      this.UpdateDataPoints();
      this.SetBlobs();
    }

    /// <summary>
    ///   The update data points.
    /// </summary>
    public void UpdateDataPoints()
    {
      this.CanvasDataPoints.Children.Clear();
      foreach (TimeSample sample in Viana.Project.VideoData.Samples)
      {
        for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          if (sample.Object == null || sample.Object[i] == null)
          {
            continue;
          }

          var dataPoint = new Ellipse
                            {
                              Stroke = new SolidColorBrush(Viana.Project.ProcessingData.TargetColor[i]),
                              StrokeThickness = 2,
                              Width = 15,
                              Height = 15
                            };
          var location = new Point(sample.Object[i].PositionX, sample.Object[i].PositionY);
          this.CanvasDataPoints.Children.Add(dataPoint);
          Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
          Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
        }
      }
    }

    /// <summary>
    ///   Wires the property changed events.
    /// </summary>
    public void WirePropertyChangedEvents()
    {
      Viana.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      Viana.Project.VideoData.PropertyChanged += this.VideoDataPropertyChanged;
      Viana.Project.CalibrationData.PropertyChanged += this.CalibrationDataPropertyChanged;
      Video.Instance.PropertyChanged += this.VideoPropertyChanged;

    }


    #endregion

    #region Methods

    /// <summary>
    /// The blobs control_ is visible changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void BlobsControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.SetBlobs();
    }

    /// <summary>
    /// Canvas data points size changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="SizeChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void CanvasDataPointsSizeChanged(object sender, SizeChangedEventArgs e)
    {
      var videoSizeToCanvasSize = new ScaleTransform
                                    {
                                      ScaleX =
                                        this.CanvasDataPoints.ActualWidth
                                        / Video.Instance.VideoElement.NaturalVideoWidth,
                                      ScaleY =
                                        this.CanvasDataPoints.ActualHeight
                                        / Video.Instance.VideoElement.NaturalVideoHeight
                                    };
      this.CanvasDataPoints.RenderTransform = videoSizeToCanvasSize;
    }

    /// <summary>
    /// Gets the scales.
    /// </summary>
    /// <param name="scaleX">The scale x.</param>
    /// <param name="scaleY">The scale y.</param>
    /// <returns>True if successfull</returns>
    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = this.OverlayImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.OverlayImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return !double.IsInfinity(scaleX) && !double.IsNaN(scaleX);
    }

    /// <summary>
    /// Gets the scales for processed frame.
    /// </summary>
    /// <param name="scaleX">The scale x.</param>
    /// <param name="scaleY">The scale y.</param>
    /// <returns>True if successfull</returns>
    private bool GetScalesForProcessedFrame(out double scaleX, out double scaleY)
    {
      scaleX = this.OriginalImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.OriginalImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return !double.IsInfinity(scaleX) && !double.IsNaN(scaleX);
    }

    /// <summary>
    ///   The reset outer region.
    /// </summary>
    private void ResetOuterRegion()
    {
      double scaleX;
      double scaleY;
      if (this.GetScalesForProcessedFrame(out scaleX, out scaleY))
      {
        // it is zero during deserialization
        if (scaleX != 0 && scaleY != 0)
        {
          var geometry = this.OuterRegion.Data as CombinedGeometry;
          var outerRectangleGeometry = geometry.Geometry1 as RectangleGeometry;
          outerRectangleGeometry.Rect = new Rect(0, 0, this.OriginalImageControl.ActualWidth, this.OriginalImageControl.ActualHeight);
          var innerRectangleGeometry = geometry.Geometry2 as RectangleGeometry;
          var innerRect = new Rect(
            new Point(Viana.Project.CalibrationData.ClipRegion.Left * scaleX, Viana.Project.CalibrationData.ClipRegion.Top * scaleY),
            new Point(Viana.Project.CalibrationData.ClipRegion.Right * scaleX, Viana.Project.CalibrationData.ClipRegion.Bottom * scaleY));
          innerRectangleGeometry.Rect = innerRect;
        }
      }
    }

    /// <summary>
    /// The image processing property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ProcessingDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsUsingMotionDetection" || e.PropertyName == "IsUsingColorDetection")
      {
        if (Viana.Project.ProcessingData.IsUsingColorDetection && Viana.Project.ProcessingData.IsUsingMotionDetection)
        {
          this.ProcessedImageControl.Source = Video.Instance.VideoElement.MotionProcessedVideoSource;
        }
        else if (Viana.Project.ProcessingData.IsUsingColorDetection)
        {
          this.ProcessedImageControl.Source = Video.Instance.VideoElement.ColorProcessedVideoSource;
        }
        else if (Viana.Project.ProcessingData.IsUsingMotionDetection)
        {
          this.ProcessedImageControl.Source = Video.Instance.VideoElement.MotionProcessedVideoSource;
        }
      }

      this.SetBlobs();
    }

    /// <summary>
    /// Calibration data property changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void CalibrationDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ClipRegion")
      {
        this.ResetOuterRegion();
      }

      if (e.PropertyName == "HasClipRegion")
      {
        this.OuterRegion.Visibility = Viana.Project.CalibrationData.HasClipRegion ? Visibility.Visible : Visibility.Hidden;
      }
    }

    /// <summary>
    ///   The set blobs.
    /// </summary>
    private void SetBlobs()
    {
      if (this.Visibility != Visibility.Visible)
      {
        return;
      }

      this.OverlayCanvas.Children.Clear();

      double scaleX;
      double scaleY;

      if (!this.GetScales(out scaleX, out scaleY))
      {
        return;
      }

      for (int i = 0; i < Viana.Project.ProcessingData.DetectedBlob.Count; i++)
      {
        Segment blob = Viana.Project.ProcessingData.DetectedBlob[i];

        if (blob.Height < Video.Instance.VideoElement.NaturalVideoHeight - 10
            && blob.Width < Video.Instance.VideoElement.NaturalVideoWidth - 10 && blob.Diagonal > 0)
        {
          SolidColorBrush fill = Viana.Project.ProcessingData.TargetColor.Count > i
                                   ? new SolidColorBrush(Viana.Project.ProcessingData.TargetColor[i])
                                   : Brushes.Black;
          var blobEllipse = new Ellipse
                              {
                                Fill = fill,
                                Stroke = Brushes.Black,
                                StrokeThickness = 1,
                                Width = blob.Width * scaleX,
                                Height = blob.Height * scaleY
                              };

          this.OverlayCanvas.Children.Add(blobEllipse);

          Canvas.SetLeft(blobEllipse, blob.Center.X * scaleX - blobEllipse.Width / 2);
          Canvas.SetTop(blobEllipse, blob.Center.Y * scaleY - blobEllipse.Height / 2);
        }
      }
    }

    /// <summary>
    /// Video data property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void VideoDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          var dataPoint = new Ellipse
                            {
                              Stroke = new SolidColorBrush(Viana.Project.ProcessingData.TargetColor[i]),
                              StrokeThickness = 2,
                              Width = 15,
                              Height = 15
                            };
          Point location = Viana.Project.VideoData.LastPoint[i];
          this.CanvasDataPoints.Children.Add(dataPoint);
          Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
          Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
        }
      }
    }

    /// <summary>
    /// Video property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void VideoPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsDataAcquisitionRunning")
      {
        if (Video.Instance.IsDataAcquisitionRunning)
        {
          this.CanvasDataPoints.Children.Clear();
        }
      }
    }

    #endregion
  }
}