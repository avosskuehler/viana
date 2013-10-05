// --------------------------------------------------------------------------------------------------------------------
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

namespace VianaNET.MainWindow
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using Microsoft.Win32;
  using Microsoft.Windows.Controls.Ribbon;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Localization;
  using VianaNET.Modules.Chart;
  using VianaNET.Modules.DataAcquisition;
  using VianaNET.Modules.DataGrid;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Dialogs;
  using VianaNET.Properties;

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

      this.MainRibbon.DataContext = this;
      VianaNetApplication.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.CreateImageSourceForNumberOfObjects();
      this.UpdateSelectObjectImage();
      if (Video.Instance.VideoInputDevices.Count > 0)
      {
        this.VideoInputDeviceCombo.SelectedItem = Video.Instance.VideoInputDevices[0];
      }

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
    /// This method saves the current project into a new file using
    /// the file name give by the called save file dialog.
    /// </summary>
    private static void SaveProjectToNewFile()
    {
      var dlg = new SaveFileDialog
      {
        Filter = "Viana.NET projects|*.via",
        Title = Labels.SaveProjectDialogTitle,
        DefaultExt = "via"
      };

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        Project.Serialize(VianaNetApplication.Project, dlg.FileName);

        // Add project file to recent files list
        RecentFiles.Instance.Add(dlg.FileName);
      }
    }

    /// <summary>
    /// The save project on click event handler.
    /// Saves all settings of the project overwriting old values.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void SaveProjectClick(object sender, RoutedEventArgs e)
    {
      var filename = VianaNetApplication.Project.ProjectPath + Path.DirectorySeparatorChar + VianaNetApplication.Project.ProjectFilename;

      if (File.Exists(filename))
      {
        // Save file
        Project.Serialize(VianaNetApplication.Project, filename);

        // Add project file to recent files list
        RecentFiles.Instance.Add(filename);
      }
      else
      {
        // If filename is not valid ask for correct name.
        SaveProjectToNewFile();
      }
    }

    /// <summary>
    /// The save project as on click event handler.
    /// Calls a save file dialog and saves all settings of the project
    /// into the given file
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void SaveProjectAsClick(object sender, RoutedEventArgs e)
    {
      SaveProjectToNewFile();
    }

    /// <summary>
    /// The open project on click event handler.
    /// Calls an open file dialog and restores all settings of the project
    /// from the given file
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void OpenProjectOnClick(object sender, RoutedEventArgs e)
    {
      var dlg = new OpenFileDialog
      {
        Filter = "Viana.NET projects|*.via",
        Title = Labels.OpenProjectDialogTitle,
        DefaultExt = "via",
        Multiselect = false
      };

      if (!dlg.ShowDialog().GetValueOrDefault(false))
      {
        return;
      }

      // first open project before adding to recentfilelist
      this.OpenGivenProject(dlg.FileName);

      // Add project file to recent files list
      RecentFiles.Instance.Add(dlg.FileName);
    }

    /// <summary>
    /// This method opens the given project updating all modules.
    /// </summary>
    /// <param name="filename">The filename with full path to the project file</param>
    private void OpenGivenProject(string filename)
    {
      // Restore project settings
      var openedProject = Project.Deserialize(filename);

      // Direct overwrite of Project static instance will kill all xaml driven bindings...
      // so update all properties individually.
      // VianaNetApplication.Project = openedProject;
      VianaNetApplication.Project.VideoData.Samples = openedProject.VideoData.Samples;
      VianaNetApplication.Project.VideoData.ActiveObject = openedProject.VideoData.ActiveObject;
      VianaNetApplication.Project.VideoData.LastPoint = openedProject.VideoData.LastPoint;
      VianaNetApplication.Project.VideoData.TimeZeroPositionInMs = openedProject.VideoData.TimeZeroPositionInMs;

      VianaNetApplication.Project.CalibrationData.ClipRegion = openedProject.CalibrationData.ClipRegion;
      VianaNetApplication.Project.CalibrationData.GradientBackground = openedProject.CalibrationData.GradientBackground;
      VianaNetApplication.Project.CalibrationData.HasClipRegion = openedProject.CalibrationData.HasClipRegion;
      VianaNetApplication.Project.CalibrationData.IsShowingUnits = openedProject.CalibrationData.IsShowingUnits;
      VianaNetApplication.Project.CalibrationData.IsVideoCalibrated = openedProject.CalibrationData.IsVideoCalibrated;
      VianaNetApplication.Project.CalibrationData.OriginInPixel = openedProject.CalibrationData.OriginInPixel;
      VianaNetApplication.Project.CalibrationData.LengthUnit = openedProject.CalibrationData.LengthUnit;
      VianaNetApplication.Project.CalibrationData.RulerDescription = openedProject.CalibrationData.RulerDescription;
      VianaNetApplication.Project.CalibrationData.RulerEndPointInPixel = openedProject.CalibrationData.RulerEndPointInPixel;
      VianaNetApplication.Project.CalibrationData.RulerStartPointInPixel = openedProject.CalibrationData.RulerStartPointInPixel;
      VianaNetApplication.Project.CalibrationData.RulerUnit = openedProject.CalibrationData.RulerUnit;
      VianaNetApplication.Project.CalibrationData.RulerValueInRulerUnits = openedProject.CalibrationData.RulerValueInRulerUnits;
      VianaNetApplication.Project.CalibrationData.ScalePixelToUnit = openedProject.CalibrationData.ScalePixelToUnit;
      VianaNetApplication.Project.CalibrationData.TimeUnit = openedProject.CalibrationData.TimeUnit;

      VianaNetApplication.Project.FilterData.NumericPrecision = openedProject.FilterData.NumericPrecision;
      VianaNetApplication.Project.FilterData.DataLineColor = openedProject.FilterData.DataLineColor;
      VianaNetApplication.Project.FilterData.DataLineThickness = openedProject.FilterData.DataLineThickness;
      VianaNetApplication.Project.FilterData.InterpolationLineColor = openedProject.FilterData.InterpolationLineColor;
      VianaNetApplication.Project.FilterData.InterpolationLineThickness = openedProject.FilterData.InterpolationLineThickness;
      VianaNetApplication.Project.FilterData.RegressionLineColor = openedProject.FilterData.RegressionLineColor;
      VianaNetApplication.Project.FilterData.RegressionLineThickness = openedProject.FilterData.RegressionLineThickness;
      VianaNetApplication.Project.FilterData.TheoryLineColor = openedProject.FilterData.TheoryLineColor;
      VianaNetApplication.Project.FilterData.TheoryLineThickness = openedProject.FilterData.TheoryLineThickness;
      VianaNetApplication.Project.FilterData.TheoreticalFunction = openedProject.FilterData.TheoreticalFunction;
      VianaNetApplication.Project.FilterData.AxisX = openedProject.FilterData.AxisX;
      VianaNetApplication.Project.FilterData.AxisY = openedProject.FilterData.AxisY;
      VianaNetApplication.Project.FilterData.IsShowingDataSeries = openedProject.FilterData.IsShowingDataSeries;
      VianaNetApplication.Project.FilterData.IsShowingInterpolationSeries = openedProject.FilterData.IsShowingInterpolationSeries;
      VianaNetApplication.Project.FilterData.IsShowingRegressionSeries = openedProject.FilterData.IsShowingRegressionSeries;
      VianaNetApplication.Project.FilterData.IsShowingTheorySeries = openedProject.FilterData.IsShowingTheorySeries;

      VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects = openedProject.ProcessingData.NumberOfTrackedObjects;
      VianaNetApplication.Project.ProcessingData.Reset();
      for (int i = 0; i < openedProject.ProcessingData.BlobMaxDiameter.Count; i++)
      {
        VianaNetApplication.Project.ProcessingData.BlobMaxDiameter[i] = openedProject.ProcessingData.BlobMaxDiameter[i];
        VianaNetApplication.Project.ProcessingData.BlobMinDiameter[i] = openedProject.ProcessingData.BlobMinDiameter[i];
        VianaNetApplication.Project.ProcessingData.ColorThreshold[i] = openedProject.ProcessingData.ColorThreshold[i];
      }

      VianaNetApplication.Project.ProcessingData.CurrentBlobCenter = openedProject.ProcessingData.CurrentBlobCenter;
      VianaNetApplication.Project.ProcessingData.DetectedBlob = openedProject.ProcessingData.DetectedBlob;
      VianaNetApplication.Project.ProcessingData.IndexOfObject = openedProject.ProcessingData.IndexOfObject;
      VianaNetApplication.Project.ProcessingData.IsTargetColorSet = openedProject.ProcessingData.IsTargetColorSet;
      VianaNetApplication.Project.ProcessingData.TargetColor = openedProject.ProcessingData.TargetColor;

      VianaNetApplication.Project.ProjectFilename = openedProject.ProjectFilename;
      VianaNetApplication.Project.ProjectPath = openedProject.ProjectPath;
      VianaNetApplication.Project.VideoFile = openedProject.VideoFile;
      VianaNetApplication.Project.VideoMode = openedProject.VideoMode;

      // after updating series do calculation
      VianaNetApplication.Project.FilterData.RegressionFilter = VianaNetApplication.Project.FilterData.RegressionFilter;
      VianaNetApplication.Project.FilterData.RegressionAberration = VianaNetApplication.Project.FilterData.RegressionAberration;

      // Restore video mode
      Video.Instance.VideoMode = VianaNetApplication.Project.VideoMode;
      switch (VianaNetApplication.Project.VideoMode)
      {
        case VideoMode.File:
          // load video
          this.VideoWindow.LoadVideo(VianaNetApplication.Project.VideoFile);
          break;
        case VideoMode.Capture:
          break;
      }

      // Set selection after loading video
      VianaNetApplication.Project.VideoData.SelectionStart = openedProject.VideoData.SelectionStart;
      VianaNetApplication.Project.VideoData.SelectionEnd = openedProject.VideoData.SelectionEnd;
      this.VideoWindow.TimelineSlider.UpdateSelectionTimes();
      Video.Instance.Revert();

      // Update datagrid
      VianaNetApplication.Project.VideoData.NotifyLoading();
      this.DataGridWindow.Refresh();

      // Update data series values
      this.ChartWindow.Refresh();

      this.VideoWindow.BlobsControl.ResetBlobsControl();
      this.Refresh();
      this.UpdateColorButton();
      this.UpdateSelectObjectImage();
    }

    /// <summary>
    /// The about command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void AboutButtonClick(object sender, RoutedEventArgs e)
    {
      var aboutWindow = new AboutWindow();
      aboutWindow.ShowDialog();
    }

    /// <summary>
    /// The automatic data aquisition command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void AutomaticDataAquisitionButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.RunAutomaticDataAquisition();
    }

    /// <summary>
    /// The automatic data aquisition stop command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void AutomaticDataAquisitionStopButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.StopAutomaticDataAquisition();
    }

    /// <summary>
    /// The button calculate velocity command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void CalculateVelocityButtonClick(object sender, RoutedEventArgs e)
    {
      this.Cursor = Cursors.Wait;
      VianaNetApplication.Project.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// The switch time unit button click event handler.
    /// Calls a dialog to change the time unit.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void SwitchTimeUnitButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new TimeUnitDialog();
      dlg.ShowDialog();
      this.Cursor = Cursors.Wait;
      VianaNetApplication.Project.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// The button capture video command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void CaptureVideoButtonClick(object sender, RoutedEventArgs e)
    {
      if (Video.Instance.VideoCapturerElement.VideoCaptureDevice == null)
      {
        Video.Instance.VideoCapturerElement.VideoCaptureDevice = Video.Instance.VideoInputDevices[0];
      }

      this.VideoWindow.SetVideoMode(VideoMode.Capture);
    }

    /// <summary>
    /// The button choose automatic analysis command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ChooseAutomaticAnalysisButtonClick(object sender, RoutedEventArgs e)
    {
      this.RibbonTabAnalysis.IsSelected = true;
    }

    /// <summary>
    /// The button delete data command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void DeleteDataButtonClick(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.VideoData.Reset();
      this.Refresh();
    }

    /// <summary>
    /// The button export chart to clipboard_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportChartToClipboardButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToClipboard(this.ChartWindow.DataChart);
    }

    /// <summary>
    /// The button export chart to file_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportChartToFileButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToFile(this.ChartWindow.DataChart);
    }

    /// <summary>
    /// The button export chart to word_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportChartToWordButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToWord(this.ChartWindow.DataChart);
    }

    /// <summary>
    /// The button export data to csv_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
        ExportData.ToCsv(VianaNetApplication.Project.VideoData.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button export data to txt_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
        ExportData.ToTxt(VianaNetApplication.Project.VideoData.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button export data to xls_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportDataToXlsButtonClick(object sender, RoutedEventArgs e)
    {
      ExportData.ToXls(VianaNetApplication.Project.VideoData.Samples);
    }

    /// <summary>
    /// The button export data to xml_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
        ExportData.ToXml(VianaNetApplication.Project.VideoData.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button record video command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void SelectNumberOfObjectsButtonClick(object sender, RoutedEventArgs e)
    {
      // Clear all data to correctly recreate data arrays.
      VianaNetApplication.Project.VideoData.Reset();

      // Increase number of objects, shrink to maximal 3.
      if (VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects == 3)
      {
        VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects = 1;
      }
      else
      {
        VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects++;
      }

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();
    }

    /// <summary>
    /// The button select object command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void SelectObjectButtonClick(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.ProcessingData.IndexOfObject++;
    }

    /// <summary>
    /// The button video capture device properties command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void VideoCaptureDevicePropertiesButtonClick(object sender, RoutedEventArgs e)
    {
      Video.Instance.VideoCapturerElement.ShowPropertyPageOfVideoDevice();
    }

    /// <summary>
    /// The calibrate video command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void CalibrationOptionsShowCalibrationButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.ShowCalibration(this.ShowCalibrationCheckbox.IsChecked());
    }

    /// <summary>
    /// The calibration options show clip region command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void CalibrationOptionsShowClipRegionButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.ShowClipRegion(this.ShowClipRegionCheckbox.IsChecked());
    }

    /// <summary>
    /// The change language command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ChartDisplayOptionsButtonClick(object sender, RoutedEventArgs e)
    {
      this.ChartWindow.PropertiesExpander.IsExpanded = true;
    }

    /// <summary>
    /// The chart window command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ChartWindowButtonClick(object sender, RoutedEventArgs e)
    {
      this.ChartTab.IsSelected = true;
    }

    /// <summary>
    /// The clip video command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ClipVideoButtonClick(object sender, RoutedEventArgs e)
    {
      var clipWindow = new ClipVideoWindow();
      clipWindow.ShowDialog();
      this.VideoWindow.UpdateClippingRegion();
    }

    /// <summary>
    /// The close command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
        VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects.ToString("N0"),
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

      if (VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects > 1)
      {
        this.SelectNumberOfObjectsButton.Label = Labels.ButtonSelectNumberOfObjectsLabelTitle2;
      }
      else
      {
        this.SelectNumberOfObjectsButton.Label = Labels.ButtonSelectNumberOfObjectsLabelTitle;
      }
    }

    /// <summary>
    /// The datagrid window command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void DatagridWindowButtonClick(object sender, RoutedEventArgs e)
    {
      this.DatagridTab.IsSelected = true;
    }

    /// <summary>
    /// The image processing_ property changed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
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
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ManualDataAquisitionButtonClick(object sender, RoutedEventArgs e)
    {
      VianaNetApplication.Project.VideoData.Reset();

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
      VianaNetApplication.Project.OnPropertyChanged("HasData");

      // Update data grid
      VianaNetApplication.Project.VideoData.RefreshDistanceVelocityAcceleration();

      // Update BlobsControl Dataview if visible
      if (VianaNetApplication.Project.ProcessingData.IsTargetColorSet)
      {
        this.VideoWindow.BlobsControl.UpdateDataPoints();
      }
    }

    /// <summary>
    /// This method is called whenever the application is closed.
    /// So clean up and ask for saving project.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void MainWindowClosing(object sender, CancelEventArgs e)
    {
      Settings.Default.Save();
      var currentProjectFile = VianaNetApplication.Project.ProjectPath + Path.DirectorySeparatorChar + VianaNetApplication.Project.ProjectFilename;
      var title = Labels.SaveProjectDialogTitle;
      title = title.Replace("%1", VianaNetApplication.Project.ProjectFilename);

      if (File.Exists(currentProjectFile))
      {
        var description = Labels.SaveProjectDialogDescription;
        description = description.Replace("%1", VianaNetApplication.Project.ProjectFilename);
        var dlg = new VianaSaveDialog(
          title,
          description,
          Labels.SaveProjectDialogMessage);

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          Project.Serialize(VianaNetApplication.Project, currentProjectFile);
        }
      }
      else
      {
        var description = Labels.AskSaveProjectDialogDescription;
        description = description.Replace("%1", VianaNetApplication.Project.ProjectFilename);
        var dlg = new VianaSaveDialog(
          title,
          description,
          Labels.AskSaveProjectDialogMessage);

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          SaveProjectToNewFile();
        }
      }

      Video.Instance.Cleanup();
    }

    /// <summary>
    /// The select color command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void SelectColorButtonClick(object sender, RoutedEventArgs e)
    {
      var fullScreenVideoWindow = new SelectColorWindow();
      if (fullScreenVideoWindow.ShowDialog().GetValueOrDefault())
      {
        this.UpdateColorButton();
      }
    }

    /// <summary>
    /// Updates the color button with the correct colors
    /// </summary>
    private void UpdateColorButton()
    {
      this.SelectColorRibbonButton.Label = Labels.ButtonSelectedColorLabelTitle;

      var drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRoundedRectangle(Brushes.Transparent, null, new Rect(0, 0, 32, 32), 5, 5);
      int count = VianaNetApplication.Project.ProcessingData.NumberOfTrackedObjects;
      float bandwidth = 26f / count;
      for (int i = 0; i < count; i++)
      {
        drawingContext.DrawRectangle(
          new SolidColorBrush(VianaNetApplication.Project.ProcessingData.TargetColor[i]),
          null,
          new Rect(3 + i * bandwidth, 3, bandwidth, 27));
      }

      drawingContext.DrawRoundedRectangle(Brushes.Transparent, new Pen(Brushes.White, 2f), new Rect(2, 2, 28, 28), 5, 5);

      drawingContext.Close();
      var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      this.SelectColorRibbonButton.LargeImageSource = bmp;
      VianaNetApplication.Project.ProcessingData.IsTargetColorSet = false;
      VianaNetApplication.Project.ProcessingData.IsTargetColorSet = true;
    }

    /// <summary>
    ///   The update select object bindings.
    /// </summary>
    private void UpdateSelectObjectBindings()
    {
      var thresholdBinding = new Binding("ProcessingData.ColorThreshold[" + VianaNetApplication.Project.ProcessingData.IndexOfObject + "]") { Source = VianaNetApplication.Project };
      this.SliderThreshold.SetBinding(RangeBase.ValueProperty, thresholdBinding);

      var minDiameterBinding = new Binding("ProcessingData.BlobMinDiameter[" + VianaNetApplication.Project.ProcessingData.IndexOfObject + "]") { Source = VianaNetApplication.Project };
      this.SliderMinDiameter.SetBinding(RangeBase.ValueProperty, minDiameterBinding);

      var maxDiameterBinding = new Binding("ProcessingData.BlobMaxDiameter[" + VianaNetApplication.Project.ProcessingData.IndexOfObject + "]") { Source = VianaNetApplication.Project };
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
        (VianaNetApplication.Project.ProcessingData.IndexOfObject + 1).ToString("N0"),
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
    /// The video window is selected.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void VideoWindowButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoTab.IsSelected = true;
    }

    /// <summary>
    /// This method is called whenever a recent project is selected from the list.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void RecentProjectMouseDown(object sender, RoutedEventArgs e)
    {
      var contentPresenter = sender as ContentPresenter;
      if (contentPresenter == null)
      {
        return;
      }

      var projectEntry = contentPresenter.Content as ProjectEntry;

      if (projectEntry == null)
      {
        return;
      }

      // now we got the projects filename and may load it.
      this.OpenGivenProject(projectEntry.ProjectFile);

      // Close the application menu
      this.MainRibbon.ApplicationMenu.IsDropDownOpen = false;
    }

    #endregion


  }
}