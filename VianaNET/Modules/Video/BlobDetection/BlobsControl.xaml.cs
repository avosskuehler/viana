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
namespace VianaNET.Modules.Video.BlobDetection
{
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using VianaNET.Data;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Filter;

  using VianaNETShaderEffectLibrary;

  /// <summary>
  ///   Interaction logic for BlobsControl.xaml
  /// </summary>
  public partial class BlobsControl : UserControl
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

      this.thresholdEffect = new ThresholdEffect();
      this.thresholdEffect.MinX = 0.001d;
      this.thresholdEffect.MaxX = 0.999d;
      this.thresholdEffect.MinY = 0.001d;
      this.thresholdEffect.MaxY = 0.999d;

      this.backgroundEffect = new BackgroundEffect();
      this.backgroundEffect.Threshold = 0.4;

      this.ProcessedImageControl.Effect = this.thresholdEffect;
      this.OverlayImageControl.Effect = this.backgroundEffect;

      // this.PopulateObjectCombo();
      Video.Instance.ImageProcessing.PropertyChanged += this.ImageProcessing_PropertyChanged;
      Calibration.Instance.PropertyChanged += this.Calibration_PropertyChanged;
      VideoData.Instance.PropertyChanged += this.VideoData_PropertyChanged;
      Video.Instance.PropertyChanged += this.Video_PropertyChanged;
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
      foreach (TimeSample sample in VideoData.Instance.Samples)
      {
        for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
        {
          var dataPoint = new Ellipse();
          dataPoint.Stroke = ImageProcessing.TrackObjectColors[i];
          dataPoint.StrokeThickness = 2;
          dataPoint.Width = 15;
          dataPoint.Height = 15;
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
      window.UpdateThresholdForHLSL();
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
    private void BlobsControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
    private void Calibration_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" || e.PropertyName == "ClipRegion")
      {
        double videoWidth = Video.Instance.VideoElement.NaturalVideoWidth;
        double videoHeight = Video.Instance.VideoElement.NaturalVideoHeight;
        if (Calibration.Instance.HasClipRegion)
        {
          this.thresholdEffect.MinX = Calibration.Instance.ClipRegion.Left / videoWidth;
          this.thresholdEffect.MaxX = Calibration.Instance.ClipRegion.Right / videoWidth;
          this.thresholdEffect.MinY = Calibration.Instance.ClipRegion.Top / videoHeight;
          this.thresholdEffect.MaxY = Calibration.Instance.ClipRegion.Bottom / videoHeight;
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
    private void CanvasDataPoints_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      var videoSizeToCanvasSize = new ScaleTransform();
      videoSizeToCanvasSize.ScaleX = this.CanvasDataPoints.ActualWidth / Video.Instance.VideoElement.NaturalVideoWidth;
      videoSizeToCanvasSize.ScaleY = this.CanvasDataPoints.ActualHeight / Video.Instance.VideoElement.NaturalVideoHeight;
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
    private void ImageProcessing_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "TargetColor" || e.PropertyName == "HLSLParams" || e.PropertyName == "IndexOfObject")
      {
        this.UpdateThresholdForHLSL();
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

      if (this.GetScales(out scaleX, out scaleY))
      {
        for (int i = 0; i < Video.Instance.ImageProcessing.DetectedBlob.Count; i++)
        {
          Segment blob = Video.Instance.ImageProcessing.DetectedBlob[i];

          if (blob.Height < Video.Instance.VideoElement.NaturalVideoHeight - 10
              && blob.Width < Video.Instance.VideoElement.NaturalVideoWidth - 10 && blob.Diagonal > 0)
          {
            var blobEllipse = new Ellipse();
            blobEllipse.Fill = ImageProcessing.TrackObjectColors[i];
            blobEllipse.Stroke = Brushes.Black; // ImageProcessing.TrackObjectColors[i];
            blobEllipse.StrokeThickness = 1;
            blobEllipse.Width = blob.Width * scaleX;
            blobEllipse.Height = blob.Height * scaleY;
            this.OverlayCanvas.Children.Add(blobEllipse);

            Canvas.SetLeft(blobEllipse, blob.Center.X * scaleX - blobEllipse.Width / 2);
            Canvas.SetTop(blobEllipse, blob.Center.Y * scaleY - blobEllipse.Height / 2);
          }
        }
      }
    }

    /// <summary>
    ///   The update threshold for hlsl.
    /// </summary>
    private void UpdateThresholdForHLSL()
    {
      this.thresholdEffect.Threshold =
        Video.Instance.ImageProcessing.ColorThreshold[Video.Instance.ImageProcessing.IndexOfObject] / 255d;
      this.thresholdEffect.BlankColor = Colors.Black;
      this.thresholdEffect.CropColor = Colors.DarkGray;
      this.thresholdEffect.TargetColor =
        Video.Instance.ImageProcessing.TargetColor[Video.Instance.ImageProcessing.IndexOfObject];
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
    private void VideoData_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
        {
          var dataPoint = new Ellipse();
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

    /// <summary>
    /// The video_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Video_PropertyChanged(object sender, PropertyChangedEventArgs e)
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