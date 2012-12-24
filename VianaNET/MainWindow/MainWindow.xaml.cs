﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Visifire.Charts;

namespace VianaNET.MainWindow
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using Microsoft.Win32;
  using Microsoft.Windows.Controls.Ribbon;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Styles;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Filter.Interpolation;
  using VianaNET.Localization;
  using VianaNET.Modules.Chart;
  using VianaNET.Modules.DataAcquisition;
  using VianaNET.Modules.DataGrid;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Dialogs;

  using WPFLocalizeExtension.Engine;

  /// <summary>
  ///   The main window.
  /// </summary>
  public partial class MainWindow
  {
    #region Fields
    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
      this.InitializeComponent();

      this.mainRibbon.DataContext = this;
      Project.Instance.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.CreateImageSourceForNumberOfObjects();
      this.UpdateSelectObjectImage();

      this.Show();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset color button.
    /// </summary>
    public void ResetColorButton()
    {
      var largeSource = new Uri(@"/VianaNET;component/Images/SelectColor32.png", UriKind.Relative);
      this.SelectColorRibbonButton.LargeImageSource = new BitmapImage(largeSource);
      var smallSource = new Uri(@"/VianaNET;component/Images/SelectColor16.png", UriKind.Relative);
      this.SelectColorRibbonButton.SmallImageSource = new BitmapImage(smallSource);
      this.SelectColorRibbonButton.Label = Labels.ButtonSelectColorLabelTitle;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The save project on click event handler.
    /// Calls a save file dialog and saves all settings of the project
    /// into the given file
    /// </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="e"> The e. </param>
    private void SaveProjectOnClick(object sender, RoutedEventArgs e)
    {
      var dlg = new SaveFileDialog
        {
          Filter = "Viana.NET projects|*.via",
          Title = Labels.SaveProjectDialogTitle,
          DefaultExt = "via"
        };

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        Project.Serialize(Project.Instance, dlg.FileName);
      }
    }

    /// <summary>
    /// The open project on click event handler.
    /// Calls an open file dialog and restores all settings of the project
    /// from the given file
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">The event args</param>
    private void OpenProjectOnClick(object sender, RoutedEventArgs e)
    {
      var dlg = new OpenFileDialog
      {
        Filter = "Viana.NET projects|*.via",
        Title = Labels.SaveProjectDialogTitle,
        DefaultExt = "via",
        Multiselect = false
      };

      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      // Restore project settings
      Project.Instance = Project.Deserialize(dlg.FileName);

      // Restore video mode
      Video.Instance.VideoMode = Project.Instance.VideoMode;
      switch (Project.Instance.VideoMode)
      {
        case VideoMode.File:
          // load video
          this.VideoWindow.LoadVideo(Project.Instance.VideoFile);
          break;
        case VideoMode.Capture:
          break;
      }

      // Update datagrid
      Project.Instance.VideoData.NotifyLoading();
      this.DataGridWindow.Refresh();

      // Update data series values
      this.ChartWindow.Refresh();

      // Update bindings that got lost during deserialization
      var regressionSeriesBinding = new Binding("FilterData.RegressionSeries") { Source = Project.Instance };
      this.ChartWindow.RegressionSeries.SetBinding(DataSeries.DataSourceProperty, regressionSeriesBinding);
      var interpolationSeriesBinding = new Binding("FilterData.InterpolationSeries") { Source = Project.Instance };
      this.ChartWindow.InterpolationSeries.SetBinding(DataSeries.DataSourceProperty, interpolationSeriesBinding);
      var theorySeriesBinding = new Binding("FilterData.TheorySeries") { Source = Project.Instance };
      this.ChartWindow.TheorySeries.SetBinding(DataSeries.DataSourceProperty, theorySeriesBinding);
    }

    /// <summary>
    /// The about command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AboutButtonClick(object sender, RoutedEventArgs e)
    {
      var aboutWindow = new AboutWindow();
      aboutWindow.ShowDialog();
    }

    /// <summary>
    /// The automatic data aquisition command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AutomaticDataAquisitionButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.RunAutomaticDataAquisition();
    }

    /// <summary>
    /// The automatic data aquisition stop command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AutomaticDataAquisitionStopButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.StopAutomaticDataAquisition();
    }

    /// <summary>
    /// The button calculate velocity command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalculateVelocityButtonClick(object sender, RoutedEventArgs e)
    {
      this.Cursor = Cursors.Wait;
      Project.Instance.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// The button capture video command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CaptureVideoButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.SetVideoMode(VideoMode.Capture);
    }

    /// <summary>
    /// The button choose automatic analysis command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChooseAutomaticAnalysisButtonClick(object sender, RoutedEventArgs e)
    {
      this.RibbonTabAnalysis.IsSelected = true;
    }

    /// <summary>
    /// The button delete data command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DeleteDataButtonClick(object sender, RoutedEventArgs e)
    {
      Project.Instance.VideoData.Reset();
      this.Refresh();
    }

    /// <summary>
    /// The button export chart to clipboard_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportChartToClipboardButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToClipboard(this.ChartWindow.DataChart);
    }

    /// <summary>
    /// The button export chart to file_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportChartToFileButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToFile(this.ChartWindow.DataChart);
    }

    /// <summary>
    /// The button export chart to word_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportChartToWordButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToWord(this.ChartWindow.DataChart);
    }

    /// <summary>
    /// The button export data to csv_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportDataToCsvButtonClick(object sender, RoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog
        {
          // Default file extension
          DefaultExt = "csv",

          // Available file extensions
          Filter = Labels.CsvFilter,

          // Adds a extension if the user does not
          AddExtension = true,

          // Restores the selected directory, next time
          RestoreDirectory = true,

          // Dialog title
          Title = Labels.ExportWhereToSaveFile,

          // Startup directory
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToCsv(Project.Instance.VideoData.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button export data to txt_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportDataToTxtButtonClick(object sender, RoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog
        {
          DefaultExt = "txt",
          Filter = Labels.TxtFilter,
          AddExtension = true,
          RestoreDirectory = true,
          Title = Labels.ExportWhereToSaveFile,
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToTxt(Project.Instance.VideoData.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button export data to xls_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportDataToXlsButtonClick(object sender, RoutedEventArgs e)
    {
      ExportData.ToXls(Project.Instance.VideoData.Samples);
    }

    /// <summary>
    /// The button export data to xml_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ExportDataToXmlButtonClick(object sender, RoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog
        {
          DefaultExt = "xml",
          Filter = Labels.XmlFilter,
          AddExtension = true,
          RestoreDirectory = true,
          Title = Labels.ExportWhereToSaveFile,
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToXml(Project.Instance.VideoData.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button record video command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RecordVideoButtonClick(object sender, RoutedEventArgs e)
    {
      bool wasCapturing = false;
      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        wasCapturing = true;
        this.VideoWindow.SetVideoMode(VideoMode.None);
      }

      var saveVideoDialog = new SaveVideoDialog();
      var showDialog = saveVideoDialog.ShowDialog();
      if (showDialog != null && showDialog.Value)
      {
        this.VideoWindow.SetVideoMode(VideoMode.File);
        this.VideoWindow.LoadVideo(saveVideoDialog.LastRecordedVideoFile);
      }
      else if (wasCapturing)
      {
        this.VideoWindow.SetVideoMode(VideoMode.Capture);
      }
    }

    /// <summary>
    /// The button select number of objects command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SelectNumberOfObjectsButtonClick(object sender, RoutedEventArgs e)
    {
      // Clear all data to correctly recreate data arrays.
      Project.Instance.VideoData.Reset();

      // Increase number of objects, shrink to maximal 3.
      if (Project.Instance.ProcessingData.NumberOfTrackedObjects == 3)
      {
        Project.Instance.ProcessingData.NumberOfTrackedObjects = 1;
      }
      else
      {
        Project.Instance.ProcessingData.NumberOfTrackedObjects++;
      }

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();
    }

    /// <summary>
    /// The button select object command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SelectObjectButtonClick(object sender, RoutedEventArgs e)
    {
      Project.Instance.ProcessingData.IndexOfObject++;
    }

    /// <summary>
    /// The button video capture device properties command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void VideoCaptureDevicePropertiesButtonClick(object sender, RoutedEventArgs e)
    {
      Video.Instance.VideoCapturerElement.ShowPropertyPageOfVideoDevice();
    }

    /// <summary>
    /// The calibrate video command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrateVideoButtonClick(object sender, RoutedEventArgs e)
    {
      var calibrateWindow = new CalibrateVideoWindow();
      if (calibrateWindow.ShowDialog().GetValueOrDefault())
      {
        this.VideoWindow.UpdateCalibration();
      }

      this.Refresh();
    }

    /// <summary>
    /// The calibration options show calibration command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationOptionsShowCalibrationButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.ShowCalibration(this.ShowCalibrationCheckbox.IsChecked());
    }

    /// <summary>
    /// The calibration options show clip region command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationOptionsShowClipRegionButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.ShowClipRegion(this.ShowClipRegionCheckbox.IsChecked());
    }

    /// <summary>
    /// The change language command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LanguageComboSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      switch (((RibbonGalleryItem)this.LanguageCombo.SelectedItem).Content.ToString())
      {
        case "Deutsch":
          LocalizeDictionary.Instance.Culture = new CultureInfo("de");
          break;
        case "English":
          LocalizeDictionary.Instance.Culture = new CultureInfo("en");
          break;
      }
    }

    /// <summary>
    /// The chart display options command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChartDisplayOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      this.ChartWindow.PropertiesExpander.IsExpanded = true;
    }

    /// <summary>
    /// The chart window command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChartWindowButtonClick(object sender, RoutedEventArgs e)
    {
      this.ChartTab.IsSelected = true;
    }

    /// <summary>
    /// The clip video command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ClipVideoButtonClick(object sender, RoutedEventArgs e)
    {
      var clipWindow = new ClipVideoWindow();
      clipWindow.ShowDialog();
      this.VideoWindow.UpdateClippingRegion();
    }

    /// <summary>
    /// The close command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CloseButtonClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    ///   The create image source for number of objects.
    /// </summary>
    private void CreateImageSourceForNumberOfObjects()
    {
      var drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRoundedRectangle(Brushes.Transparent, null, new Rect(0, 0, 32, 32), 5, 5);

      // drawingContext.DrawRoundedRectangle((Brush)this.Resources["DefaultOfficeBackgroundBrush"],
      // new Pen(Brushes.White, 2f),
      // new Rect(2, 2, 28, 28),
      // 5,
      // 5);
      var text = new FormattedText(
        Project.Instance.ProcessingData.NumberOfTrackedObjects.ToString("N0"),
        LocalizeDictionary.Instance.Culture,
        FlowDirection.LeftToRight,
        new Typeface("Verdana"),
        24d,
        Brushes.Black);

      drawingContext.DrawText(text, new Point(8, 1));

      drawingContext.Close();
      var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      this.SelectNumberOfObjectsButton.LargeImageSource = bmp;

      if (Project.Instance.ProcessingData.NumberOfTrackedObjects > 1)
      {
        this.SelectNumberOfObjectsButton.Label = Labels.ButtonSelectNumberOfObjectsLabelTitle2;
      }
      else
      {
        this.SelectNumberOfObjectsButton.Label = Labels.ButtonSelectNumberOfObjectsLabelTitle;
      }
    }

    /// <summary>
    /// The datagrid display units command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DatagridDisplayUnitsButtonClick(object sender, RoutedEventArgs e)
    {
      //this.datagridWindow.Refresh();
    }

    /// <summary>
    /// The datagrid window command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DatagridWindowButtonClick(object sender, RoutedEventArgs e)
    {
      this.DatagridTab.IsSelected = true;
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
      if (e.PropertyName == "IndexOfObject" || e.PropertyName == "NumberOfTrackedObjects")
      {
        this.UpdateSelectObjectImage();
      }
    }

    /// <summary>
    /// The load video button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void LoadVideoButtonClick(object sender, RoutedEventArgs e)
    {
      this.ResetColorButton();
      this.VideoWindow.SetVideoMode(VideoMode.File);
      this.VideoWindow.LoadVideo(string.Empty);
      this.UpdateSelectObjectImage();
    }

    /// <summary>
    /// The manual data aquisition command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ManualDataAquisitionButtonClick(object sender, RoutedEventArgs e)
    {
      Project.Instance.VideoData.Reset();

      var manualAquisitionWindow = new ManualDataAquisitionWindow();
      manualAquisitionWindow.ShowDialog();

      this.Refresh();

      // Switch to datagrid window
      this.DatagridTab.IsSelected = true;
    }

    /// <summary>
    ///   The refresh.
    /// </summary>
    private void Refresh()
    {
      // Update data grid
      Project.Instance.VideoData.RefreshDistanceVelocityAcceleration();

      // Update BlobsControl Dataview if visible
      if (Project.Instance.ProcessingData.IsTargetColorSet)
      {
        this.VideoWindow.BlobsControl.UpdateDataPoints();
      }

      // this.datagridWindow.Refresh();
    }

    /// <summary>
    /// The ribbon window_ closing.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RibbonWindowClosing(object sender, CancelEventArgs e)
    {
      Video.Instance.Cleanup();
    }

    /// <summary>
    /// The select color command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SelectColorButtonClick(object sender, RoutedEventArgs e)
    {
      var fullScreenVideoWindow = new SelectColorWindow();
      if (fullScreenVideoWindow.ShowDialog().GetValueOrDefault())
      {
        this.SelectColorRibbonButton.Label = Labels.ButtonSelectedColorLabelTitle;

        var drawingVisual = new DrawingVisual();
        DrawingContext drawingContext = drawingVisual.RenderOpen();
        drawingContext.DrawRoundedRectangle(Brushes.Transparent, null, new Rect(0, 0, 32, 32), 5, 5);
        int count = Project.Instance.ProcessingData.NumberOfTrackedObjects;
        float bandwidth = 26f / count;
        for (int i = 0; i < count; i++)
        {
          drawingContext.DrawRectangle(
            new SolidColorBrush(Project.Instance.ProcessingData.TargetColor[i]),
            null,
            new Rect(3 + i * bandwidth, 3, bandwidth, 27));
        }

        drawingContext.DrawRoundedRectangle(
          Brushes.Transparent, new Pen(Brushes.White, 2f), new Rect(2, 2, 28, 28), 5, 5);

        drawingContext.Close();
        var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(drawingVisual);
        this.SelectColorRibbonButton.LargeImageSource = bmp;
        Project.Instance.ProcessingData.IsTargetColorSet = false;
        Project.Instance.ProcessingData.IsTargetColorSet = true;

        // this.VideoWindow.BlobsControl.Visibility = Visibility.Visible;
        // Video.Instance.UpdateNativeBitmap();
      }
    }

    /// <summary>
    ///   The update select object bindings.
    /// </summary>
    private void UpdateSelectObjectBindings()
    {
      var thresholdBinding = new Binding("ProcessingData.ColorThreshold[" + Project.Instance.ProcessingData.IndexOfObject + "]") { Source = Project.Instance };
      this.SliderThreshold.SetBinding(RangeBase.ValueProperty, thresholdBinding);

      var minDiameterBinding = new Binding("ProcessingData.BlobMinDiameter[" + Project.Instance.ProcessingData.IndexOfObject + "]") { Source = Project.Instance };
      this.SliderMinDiameter.SetBinding(RangeBase.ValueProperty, minDiameterBinding);

      var maxDiameterBinding = new Binding("ProcessingData.BlobMaxDiameter[" + Project.Instance.ProcessingData.IndexOfObject + "]") { Source = Project.Instance };
      this.SliderMaxDiameter.SetBinding(RangeBase.ValueProperty, maxDiameterBinding);
    }

    /// <summary>
    ///   The update select object image.
    /// </summary>
    private void UpdateSelectObjectImage()
    {
      var icon = new BitmapImage(new Uri("pack://application:,,,/VianaNET;component/Images/SelectObject32.png"));

      var drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawImage(icon, new Rect(0, 0, 32, 32));
      var text = new FormattedText(
        (Project.Instance.ProcessingData.IndexOfObject + 1).ToString("N0"),
        LocalizeDictionary.Instance.Culture,
        FlowDirection.LeftToRight,
        new Typeface("Verdana"),
        18d,
        Brushes.Black);

      drawingContext.DrawText(text, new Point(10, 3));

      drawingContext.Close();
      var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      this.ButtonSelectObject.LargeImageSource = bmp;

      this.UpdateSelectObjectBindings();
    }

    /// <summary>
    /// The video window command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void VideoWindowButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoTab.IsSelected = true;
    }

    #endregion

  }
}