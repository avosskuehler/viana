﻿// --------------------------------------------------------------------------------------------------------------------
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
  using VianaNET.Data.Collections;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Filter;

  /// <summary>
  ///   Interaction logic for BlobsControl.xaml
  /// </summary>
  public partial class BlobsControl
  {


    /// <summary>
    ///   Initializes a new instance of the <see cref="BlobsControl" /> class.
    /// </summary>
    public BlobsControl()
    {
      this.DataContext = this;
      this.InitializeComponent();
      this.WirePropertyChangedEvents();
    }





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
      foreach (TimeSample sample in App.Project.VideoData.Samples)
      {
        for (int i = 0; i < App.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          if (sample.Object == null || sample.Object[i] == null)
          {
            continue;
          }

          Ellipse dataPoint = new Ellipse
          {
            Stroke = new SolidColorBrush(App.Project.ProcessingData.TargetColor[i]),
            StrokeThickness = 2,
            Width = 15,
            Height = 15
          };

          var location = new Point(sample.Object[i].PixelX, sample.Object[i].PixelY);

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
      App.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      App.Project.VideoData.PropertyChanged += this.VideoDataPropertyChanged;
      App.Project.CalibrationData.PropertyChanged += this.CalibrationDataPropertyChanged;
      Video.Instance.PropertyChanged += this.VideoPropertyChanged;

    }






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
      ScaleCanvasDataPoints();
    }

    /// <summary>
    /// Calculates the Scale Transform for the CanvasDataPoints depending on relation between orginal video and control size
    /// </summary>
    private void ScaleCanvasDataPoints()
    {
      ScaleTransform videoSizeToCanvasSize = new ScaleTransform
      {
        ScaleX = this.OverlayImageControl2.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth,
        ScaleY = this.OverlayImageControl2.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight
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
      if (this.Visibility != Visibility.Visible)
      {
        return;
      }

      if (this.GetScalesForProcessedFrame(out double scaleX, out double scaleY))
      {
        // it is zero during deserialization
        if (scaleX != 0 && scaleY != 0)
        {
          CombinedGeometry geometry = this.OuterRegion.Data as CombinedGeometry;
          RectangleGeometry outerRectangleGeometry = geometry.Geometry1 as RectangleGeometry;
          outerRectangleGeometry.Rect = new Rect(0, 0, this.OriginalImageControl.ActualWidth, this.OriginalImageControl.ActualHeight);
          RectangleGeometry innerRectangleGeometry = geometry.Geometry2 as RectangleGeometry;
          Rect innerRect = new Rect(
            new Point(App.Project.CalibrationData.ClipRegion.Left * scaleX, App.Project.CalibrationData.ClipRegion.Top * scaleY),
            new Point(App.Project.CalibrationData.ClipRegion.Right * scaleX, App.Project.CalibrationData.ClipRegion.Bottom * scaleY));
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
        if (App.Project.ProcessingData.IsUsingColorDetection && App.Project.ProcessingData.IsUsingMotionDetection)
        {
          this.ProcessedImageControl.Source = Video.Instance.MotionProcessedImageSource;
          this.ProcessingGroupBox.Header = VianaNET.Localization.Labels.BlobsControlBothProcessedImageHeader;
        }
        else if (App.Project.ProcessingData.IsUsingColorDetection)
        {
          this.ProcessedImageControl.Source = Video.Instance.ColorProcessedImageSource;
          this.ProcessingGroupBox.Header = VianaNET.Localization.Labels.BlobsControlColorProcessedImageHeader;
        }
        else if (App.Project.ProcessingData.IsUsingMotionDetection)
        {
          this.ProcessedImageControl.Source = Video.Instance.MotionProcessedImageSource;
          this.ProcessingGroupBox.Header = VianaNET.Localization.Labels.BlobsControlMotionProcessedImageHeader;
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
        this.OuterRegion.Visibility = App.Project.CalibrationData.HasClipRegion ? Visibility.Visible : Visibility.Hidden;
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


      if (!this.GetScales(out double scaleX, out double scaleY))
      {
        return;
      }

      for (int i = 0; i < App.Project.ProcessingData.DetectedBlob.Count; i++)
      {
        Segment blob = App.Project.ProcessingData.DetectedBlob[i];

        if (blob.Height < Video.Instance.VideoElement.NaturalVideoHeight - 10
            && blob.Width < Video.Instance.VideoElement.NaturalVideoWidth - 10 && blob.Diagonal > 0)
        {
          Color color = Color.FromArgb(128, 0, 0, 0);

          if (App.Project.ProcessingData.IsTargetColorSet && App.Project.ProcessingData.TargetColor.Count > i)
          {
            color = App.Project.ProcessingData.TargetColor[i];
            color.A = 128;
          }

          Ellipse blobEllipse = new Ellipse
          {
            Fill = new SolidColorBrush(color),
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
      //if (Video.Instance.IsDataAcquisitionRunning)
      {
        for (int i = 0; i < App.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          Color color = Colors.Black;

          if (App.Project.ProcessingData.IsTargetColorSet && App.Project.ProcessingData.TargetColor.Count > i)
          {
            color = App.Project.ProcessingData.TargetColor[i];
          }

          Ellipse dataPoint = new Ellipse
          {
            Stroke = new SolidColorBrush(color),
            StrokeThickness = 2,
            Width = 15,
            Height = 15
          };
          Point location = App.Project.VideoData.LastPoint[i];
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

      if (e.PropertyName == "HasVideo")
      {
        this.ScaleCanvasDataPoints();
      }
    }

    /// <summary>
    /// Handles the OnSizeChanged event of the BlobsControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
    private void BlobsControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.ResetOuterRegion();
    }



  }
}