// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobsControl.xaml.cs" company="Freie Universität Berlin">
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
//   Interaction logic for BlobsControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using VianaNET.Application;

namespace VianaNET.Modules.Video.BlobDetection
{
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using VianaNET.Data;
  using VianaNET.Data.Collections;
  using VianaNET.Modules.Video.Control;

  using VianaNETShaderEffectLibrary;

  /// <summary>
  ///   Interaction logic for BlobsControl.xaml
  /// </summary>
  public partial class BlobsControl
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Fields

    /// <summary>
    ///   The background effect.
    /// </summary>
    private readonly BackgroundEffect backgroundEffect;

    /// <summary>
    ///   The threshold effect.
    /// </summary>
    private readonly ThresholdEffect thresholdEffect;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="BlobsControl" /> class.
    /// </summary>
    public BlobsControl()
    {
      this.DataContext = this;
      this.InitializeComponent();

      this.thresholdEffect = new ThresholdEffect { MinX = 0.001d, MaxX = 0.999d, MinY = 0.001d, MaxY = 0.999d };

      this.backgroundEffect = new BackgroundEffect { Threshold = 0.4 };

      this.ProcessedImageControl.Effect = this.thresholdEffect;
      this.OverlayImageControl.Effect = this.backgroundEffect;

      // this.PopulateObjectCombo();
      Project.Instance.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      Project.Instance.CalibrationData.PropertyChanged += this.CalibrationPropertyChanged;
      Project.Instance.VideoData.PropertyChanged += this.VideoDataPropertyChanged;
      Video.Instance.PropertyChanged += this.VideoPropertyChanged;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Methods and Operators

    /// <summary>
    ///   The update data points.
    /// </summary>
    public void UpdateDataPoints()
    {
      this.CanvasDataPoints.Children.Clear();
      foreach (TimeSample sample in Project.Instance.VideoData.Samples)
      {
        for (int i = 0; i < Project.Instance.ProcessingData.NumberOfTrackedObjects; i++)
        {
          var dataPoint = new Ellipse
            {
              Stroke = ProcessingData.TrackObjectColors[i],
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

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">
    /// The source of the event. This. 
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var window = obj as BlobsControl;
      window.UpdateThresholdForHlsl();
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
    /// The calibration_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" || e.PropertyName == "ClipRegion")
      {
        double videoWidth = Video.Instance.VideoElement.NaturalVideoWidth;
        double videoHeight = Video.Instance.VideoElement.NaturalVideoHeight;
        if (Project.Instance.CalibrationData.HasClipRegion)
        {
          this.thresholdEffect.MinX = Project.Instance.CalibrationData.ClipRegion.Left / videoWidth;
          this.thresholdEffect.MaxX = Project.Instance.CalibrationData.ClipRegion.Right / videoWidth;
          this.thresholdEffect.MinY = Project.Instance.CalibrationData.ClipRegion.Top / videoHeight;
          this.thresholdEffect.MaxY = Project.Instance.CalibrationData.ClipRegion.Bottom / videoHeight;
        }
        else
        {
          this.thresholdEffect.MinX = 0d;
          this.thresholdEffect.MaxX = 1d;
          this.thresholdEffect.MinY = 0d;
          this.thresholdEffect.MaxY = 1d;
        }

        this.SetBlobs();
      }
    }

    /// <summary>
    /// The canvas data points_ size changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CanvasDataPointsSizeChanged(object sender, SizeChangedEventArgs e)
    {
      var videoSizeToCanvasSize = new ScaleTransform
        {
          ScaleX = this.CanvasDataPoints.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth,
          ScaleY = this.CanvasDataPoints.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight
        };
      this.CanvasDataPoints.RenderTransform = videoSizeToCanvasSize;
    }

    // private void ObjectSelectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    // {
    // if (this.ObjectSelectionCombo.SelectedItem != null)
    // {
    // string entry = (string)this.ObjectSelectionCombo.SelectedItem;
    // this.IndexOfObject = Int32.Parse(entry.Substring(entry.Length - 1, 1)) - 1;
    // this.thresholdEffect.TargetColor = Video.Instance.ImageProcessing.TargetColor[this.IndexOfObject];
    // }
    // }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    // private void PopulateObjectCombo()
    // {
    // // Erase old entries
    // this.ObjectDescriptions.Clear();

    // for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
    // {
    // this.ObjectDescriptions.Add(Localization.Labels.DataGridObjectPrefix + " " + (i + 1).ToString());
    // }

    // this.ObjectSelectionCombo.ItemsSource = null;
    // this.ObjectSelectionCombo.ItemsSource = this.ObjectDescriptions;
    // this.ObjectSelectionCombo.SelectedIndex = 0;
    // }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The get scales.
    /// </summary>
    /// <param name="scaleX">
    /// The scale x. 
    /// </param>
    /// <param name="scaleY">
    /// The scale y. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    private bool GetScales(out double scaleX, out double scaleY)
    {
      scaleX = this.OverlayImageControl.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      scaleY = this.OverlayImageControl.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;

      return !double.IsInfinity(scaleX) && !double.IsNaN(scaleX);
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
      if (e.PropertyName == "TargetColor" || e.PropertyName == "HLSLParams" || e.PropertyName == "IndexOfObject")
      {
        this.UpdateThresholdForHlsl();
      }

      this.SetBlobs();
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
      { return; }

      for (var i = 0; i < Project.Instance.ProcessingData.DetectedBlob.Count; i++)
      {
        var blob = Project.Instance.ProcessingData.DetectedBlob[i];

        if (blob.Height < Video.Instance.VideoElement.NaturalVideoHeight - 10
            && blob.Width < Video.Instance.VideoElement.NaturalVideoWidth - 10 && blob.Diagonal > 0)
        {
          var blobEllipse = new Ellipse
            {
              Fill = ProcessingData.TrackObjectColors[i],
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
    ///   The update threshold for hlsl.
    /// </summary>
    private void UpdateThresholdForHlsl()
    {
      // Sanity checks
      if (Project.Instance.ProcessingData.ColorThreshold.Count > Project.Instance.ProcessingData.IndexOfObject)
      {
        this.thresholdEffect.Threshold =
         Project.Instance.ProcessingData.ColorThreshold[Project.Instance.ProcessingData.IndexOfObject] / 255d;
        //Project.Instance.ProcessingData.ColorThreshold = new ObservableCollection<int>();
        //for (var i = 0; i < Project.Instance.ProcessingData.NumberOfTrackedObjects; i++)
        //{
        //  Project.Instance.ProcessingData.ColorThreshold.Add(30);
        //}
      }

      // Sanity check two
      if (Project.Instance.ProcessingData.TargetColor.Count > Project.Instance.ProcessingData.IndexOfObject)
      {
        this.thresholdEffect.TargetColor =
           Project.Instance.ProcessingData.TargetColor[Project.Instance.ProcessingData.IndexOfObject];
        //Project.Instance.ProcessingData.TargetColor = new ObservableCollection<Color>();
        //for (var i = 0; i < Project.Instance.ProcessingData.NumberOfTrackedObjects; i++)
        //{
        //  Project.Instance.ProcessingData.TargetColor.Add(Colors.Red);
        //}
      }

      this.thresholdEffect.BlankColor = Colors.Black;
      this.thresholdEffect.CropColor = Colors.DarkGray;
    }

    /// <summary>
    /// The video data_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void VideoDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        for (int i = 0; i < Project.Instance.ProcessingData.NumberOfTrackedObjects; i++)
        {
          var dataPoint = new Ellipse
            {
              Stroke = ProcessingData.TrackObjectColors[i],
              StrokeThickness = 2,
              Width = 15,
              Height = 15
            };
          Point location = Project.Instance.VideoData.LastPoint[i];
          this.CanvasDataPoints.Children.Add(dataPoint);
          Canvas.SetTop(dataPoint, location.Y - dataPoint.Height / 2);
          Canvas.SetLeft(dataPoint, location.X - dataPoint.Width / 2);
        }
      }
    }

    /// <summary>
    /// The video_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
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