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
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Controls.Ribbon;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using Microsoft.Win32;
  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Logging;
  using VianaNET.Resources;
  using VianaNET.Modules.Chart;
  using VianaNET.Modules.DataAcquisition;
  using VianaNET.Modules.DataGrid;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Dialogs;
  using VianaNET.Properties;

  using WPFLocalizeExtension.Engine;

  using WPFMath;

  /// <summary>
  ///   The main window.
  /// </summary>
  public partial class MainWindow
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
      //BindingErrorTraceListener.SetTrace();
      this.InitializeComponent();

      this.MainRibbon.DataContext = this;
      Viana.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.CreateImageSourceForNumberOfObjects();
      this.UpdateSelectObjectImage();
      var devices = Video.Instance.VideoInputDevices.Select(device => device.Name).ToList();

      this.VideoSourceGalleryCategory.ItemsSource = devices;
      if (Video.Instance.VideoInputDevices.Count > 0)
      {
        this.VideoInputDeviceCombo.SelectedItem = Video.Instance.VideoInputDevices[0].Name;
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
        Project.Serialize(Viana.Project, dlg.FileName);

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
      SaveCurrentProject();
    }

    /// <summary>
    /// Saves the current project.
    /// </summary>
    private static void SaveCurrentProject()
    {
      var filename = Viana.Project.ProjectPath + Path.DirectorySeparatorChar + Viana.Project.ProjectFilename;

      if (File.Exists(filename))
      {
        // Save file
        Project.Serialize(Viana.Project, filename);

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
    /// The new project on click event handler.
    /// Clears all data and video settings.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void NewProjectOnClick(object sender, RoutedEventArgs e)
    {
      if (Viana.Project.ProjectFilename != string.Empty && Viana.Project.ProjectFilename != null)
      {
        var dlg = new VianaDialog(
          Labels.AskSaveProjectDialogTitle,
          Labels.AskSaveProjectDialogDescription,
          Labels.AskSaveProjectDialogMessage,
          false);
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          SaveCurrentProject();
        }
      }

      // Note that we want to overwrite the defaults
      Project.IsDeserializing = true;
      Viana.Project = new Project();
      //Viana.Project.ProjectFilename = openedProject.ProjectFilename;
      //Viana.Project.ProjectPath = openedProject.ProjectPath;
      //Viana.Project.VideoFile = openedProject.VideoFile;
      //Viana.Project.VideoMode = openedProject.VideoMode;

      // Restore video mode
      Video.Instance.VideoMode = VideoMode.File;

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();

      // Update datagrid
      Viana.Project.VideoData.NotifyLoading();
      this.DataGridWindow.Refresh();

      // Update data series values
      this.ChartWindow.Refresh();

      this.VideoWindow.BlobsControl.ResetBlobsControl();
      Video.Instance.OriginalImageSource = Viana.GetImageSource("NoVideo800600.png");

      this.Refresh();
      this.UpdateColorButton();
      this.UpdateSelectObjectImage();

      // Reset flag
      Project.IsDeserializing = false;
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
      // Note that we want to overwrite the defaults
      Project.IsDeserializing = true;

      // Restore project settings
      var openedProject = Project.Deserialize(filename);

      // Direct overwrite of Project static instance will kill all xaml driven bindings...
      // so update all properties individually.
      // VianaNetApplication.Project = openedProject;
      Viana.Project.VideoData.Samples = openedProject.VideoData.Samples;
      //Viana.Project.VideoData.ActiveObject = openedProject.VideoData.ActiveObject;
      Viana.Project.VideoData.LastPoint = openedProject.VideoData.LastPoint;
      Viana.Project.VideoData.TimeZeroPositionInMs = openedProject.VideoData.TimeZeroPositionInMs;

      Viana.Project.CalibrationData.ClipRegion = openedProject.CalibrationData.ClipRegion;
      Viana.Project.CalibrationData.GradientBackground = openedProject.CalibrationData.GradientBackground;
      Viana.Project.CalibrationData.HasClipRegion = openedProject.CalibrationData.HasClipRegion;
      Viana.Project.CalibrationData.IsShowingUnits = openedProject.CalibrationData.IsShowingUnits;
      Viana.Project.CalibrationData.IsVideoCalibrated = openedProject.CalibrationData.IsVideoCalibrated;
      Viana.Project.CalibrationData.OriginInPixel = openedProject.CalibrationData.OriginInPixel;
      Viana.Project.CalibrationData.LengthUnit = openedProject.CalibrationData.LengthUnit;
      Viana.Project.CalibrationData.RulerDescription = openedProject.CalibrationData.RulerDescription;
      Viana.Project.CalibrationData.RulerEndPointInPixel = openedProject.CalibrationData.RulerEndPointInPixel;
      Viana.Project.CalibrationData.RulerStartPointInPixel = openedProject.CalibrationData.RulerStartPointInPixel;
      Viana.Project.CalibrationData.RulerUnit = openedProject.CalibrationData.RulerUnit;
      Viana.Project.CalibrationData.RulerValueInRulerUnits = openedProject.CalibrationData.RulerValueInRulerUnits;
      Viana.Project.CalibrationData.ScalePixelToUnit = openedProject.CalibrationData.ScalePixelToUnit;
      Viana.Project.CalibrationData.TimeUnit = openedProject.CalibrationData.TimeUnit;

      Viana.Project.SerializableFilterData = openedProject.SerializableFilterData;
      Viana.Project.CurrentChartType = openedProject.CurrentChartType;

      //Viana.Project.CurrentFilterData.NumericPrecision = openedProject.CurrentFilterData.NumericPrecision;
      //Viana.Project.CurrentFilterData.DataLineColor = openedProject.CurrentFilterData.DataLineColor;
      //Viana.Project.CurrentFilterData.DataLineThickness = openedProject.CurrentFilterData.DataLineThickness;
      //Viana.Project.CurrentFilterData.InterpolationLineColor = openedProject.CurrentFilterData.InterpolationLineColor;
      //Viana.Project.CurrentFilterData.InterpolationLineThickness = openedProject.CurrentFilterData.InterpolationLineThickness;
      //Viana.Project.CurrentFilterData.RegressionLineColor = openedProject.CurrentFilterData.RegressionLineColor;
      //Viana.Project.CurrentFilterData.RegressionLineThickness = openedProject.CurrentFilterData.RegressionLineThickness;
      //Viana.Project.CurrentFilterData.TheoryLineColor = openedProject.CurrentFilterData.TheoryLineColor;
      //Viana.Project.CurrentFilterData.TheoryLineThickness = openedProject.CurrentFilterData.TheoryLineThickness;
      //Viana.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree =
      //  openedProject.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree;
      //Viana.Project.CurrentFilterData.AxisX = openedProject.CurrentFilterData.AxisX;
      //Viana.Project.CurrentFilterData.AxisY = openedProject.CurrentFilterData.AxisY;
      //Viana.Project.CurrentFilterData.IsShowingDataSeries = openedProject.CurrentFilterData.IsShowingDataSeries;
      //Viana.Project.CurrentFilterData.IsShowingInterpolationSeries = openedProject.CurrentFilterData.IsShowingInterpolationSeries;
      //Viana.Project.CurrentFilterData.IsShowingRegressionSeries = openedProject.CurrentFilterData.IsShowingRegressionSeries;
      //Viana.Project.CurrentFilterData.IsShowingTheorySeries = openedProject.CurrentFilterData.IsShowingTheorySeries;

      //Viana.Project.ProcessingData.Reset();
      for (int i = 0; i < openedProject.ProcessingData.BlobMaxDiameter.Count; i++)
      {
        if (Viana.Project.ProcessingData.BlobMaxDiameter.Count > i)
        {
          Viana.Project.ProcessingData.BlobMaxDiameter[i] = openedProject.ProcessingData.BlobMaxDiameter[i];
          Viana.Project.ProcessingData.BlobMinDiameter[i] = openedProject.ProcessingData.BlobMinDiameter[i];
          Viana.Project.ProcessingData.ColorThreshold[i] = openedProject.ProcessingData.ColorThreshold[i];
        }
        else
        {
          Viana.Project.ProcessingData.BlobMaxDiameter.Add(openedProject.ProcessingData.BlobMaxDiameter[i]);
          Viana.Project.ProcessingData.BlobMinDiameter.Add(openedProject.ProcessingData.BlobMinDiameter[i]);
          Viana.Project.ProcessingData.ColorThreshold.Add(openedProject.ProcessingData.ColorThreshold[i]);
        }
      }

      Viana.Project.ProcessingData.CurrentBlobCenter = openedProject.ProcessingData.CurrentBlobCenter;
      Viana.Project.ProcessingData.DetectedBlob = openedProject.ProcessingData.DetectedBlob;
      Viana.Project.ProcessingData.IndexOfObject = openedProject.ProcessingData.IndexOfObject;

      Viana.Project.ProcessingData.IsTargetColorSet = openedProject.ProcessingData.IsTargetColorSet;
      for (int i = 0; i < openedProject.ProcessingData.TargetColor.Count - openedProject.ProcessingData.NumberOfTrackedObjects; i++)
      {
        Viana.Project.ProcessingData.TargetColor.RemoveAt(i);
      }

      Viana.Project.ProcessingData.NumberOfTrackedObjects = openedProject.ProcessingData.NumberOfTrackedObjects;

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();

      this.UpdateColorButton();

      Viana.Project.ProjectFilename = openedProject.ProjectFilename;
      Viana.Project.ProjectPath = openedProject.ProjectPath;
      Viana.Project.VideoFile = openedProject.VideoFile;
      Viana.Project.VideoMode = openedProject.VideoMode;
      //this.VideoWindow.CreateCrossHairLines();

      // after updating series do calculation
      Viana.Project.CurrentFilterData.RegressionFilter = Viana.Project.CurrentFilterData.RegressionFilter;
      Viana.Project.CurrentFilterData.RegressionAberration = Viana.Project.CurrentFilterData.RegressionAberration;

      // Restore video mode
      Video.Instance.VideoMode = Viana.Project.VideoMode;
      switch (Viana.Project.VideoMode)
      {
        case VideoMode.File:
          // load video
          this.VideoWindow.LoadVideo(Viana.Project.VideoFile);
          break;
        case VideoMode.Capture:
          break;
      }

      // Set selection after loading video
      Viana.Project.VideoData.SelectionStart = openedProject.VideoData.SelectionStart;
      Viana.Project.VideoData.SelectionEnd = openedProject.VideoData.SelectionEnd;
      this.VideoWindow.TimelineSlider.UpdateSelectionTimes();

      Video.Instance.Revert();

      // Update datagrid
      Viana.Project.VideoData.NotifyLoading();
      this.DataGridWindow.Refresh();

      // Update data series values
      this.ChartWindow.Refresh();

      this.VideoWindow.BlobsControl.ResetBlobsControl();
      this.Refresh();
      this.UpdateColorButton();
      this.UpdateSelectObjectImage();

      if (openedProject.CalibrationData.HasClipRegion)
      {
        this.VideoWindow.UpdateClippingRegion();
      }

      if (openedProject.CalibrationData.IsVideoCalibrated)
      {
        this.VideoWindow.UpdateCalibration();
      }

      // Reset flag
      Project.IsDeserializing = false;
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
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// Shows the video infos and allows to change the time scale.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ShowVideoInfosButtonClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VideoInfoDialog(Viana.Project.VideoFile);
      dialog.ShowDialog();
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
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// The difference quotient button click event handler.
    /// Calls a dialog to change the difference quotient calculation type.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void DifferenceQuotientButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new DifferenceQuotientDialog();
      dlg.ShowDialog();
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
      Viana.Project.VideoData.Reset();
      this.Refresh();
    }

    /// <summary>
    /// The button export chart to clipboard_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportChartToClipboardButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToClipboard(this.ChartWindow.ChartData.ChartDataModel);
    }

    /// <summary>
    /// The button export chart to file_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportChartToFileButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToFile(this.ChartWindow.ChartData.ChartDataModel);
    }

    /// <summary>
    /// The button export chart to word_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportChartToWordButtonClick(object sender, RoutedEventArgs e)
    {
      ExportChart.ToWord(this.ChartWindow.ChartData.ChartDataModel);
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
        var optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToCsv(Viana.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions, sfd.FileName);
        }
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
        var optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToTxt(Viana.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions, sfd.FileName);
        }
      }
    }

    /// <summary>
    /// The button export data to xls_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportDataToXlsButtonClick(object sender, RoutedEventArgs e)
    {
      var optionsDialog = new ExportOptionsDialog();
      if (optionsDialog.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToXls(Viana.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions);
      }
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
        var optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToXml(Viana.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions, sfd.FileName);
        }
      }
    }

    /// <summary>
    /// The button export data to ods executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void ExportDataToOdsButtonClick(object sender, RoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog
      {
        DefaultExt = "ods",
        Filter = Labels.OdsFilter,
        AddExtension = true,
        RestoreDirectory = true,
        Title = Labels.ExportWhereToSaveFile,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
      };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        var optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToOds(Viana.Project.VideoData.FilteredSamples, sfd.FileName, optionsDialog.ExportOptions);
        }
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
      if (Video.Instance.IsDataAcquisitionRunning)
      {
        return;
      }

      // Clear all data to correctly recreate data arrays.
      Viana.Project.VideoData.Reset();

      // Increase number of objects, shrink to maximal 3.
      if (Viana.Project.ProcessingData.NumberOfTrackedObjects == 3)
      {
        Viana.Project.ProcessingData.NumberOfTrackedObjects = 1;
      }
      else
      {
        Viana.Project.ProcessingData.NumberOfTrackedObjects++;
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
      Viana.Project.ProcessingData.IndexOfObject++;
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
    /// Coordinate system button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CoordinateSystemButtonClick(object sender, RoutedEventArgs e)
    {
      var coordinateSystemWindow = new CoordinateSystemWindow();
      coordinateSystemWindow.ShowDialog();
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
    /// The calibration options show calibration command_ executed.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void CalibrationOptionsShowPixelLengthButtonClick(object sender, RoutedEventArgs e)
    {
      this.VideoWindow.ShowPixelLength(this.ShowPixelLengthCheckbox.IsChecked());
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
        Viana.Project.ProcessingData.NumberOfTrackedObjects.ToString("N0"),
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

      if (Viana.Project.ProcessingData.NumberOfTrackedObjects > 1)
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
      Viana.Project.VideoData.Reset();

      var manualAquisitionWindow = new ManualDataAquisitionWindow();
      manualAquisitionWindow.ShowDialog();

      this.Refresh();

      // Switch to datagrid window
      this.DatagridTab.IsSelected = true;
    }

    /// <summary>
    /// Modifiy data button click.
    /// Start the modify control.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ModifyDataButtonClick(object sender, RoutedEventArgs e)
    {
      var modifyDataWindow = new ModifyDataWindow();
      modifyDataWindow.ShowDialog();
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
    }

    /// <summary>
    ///   The refresh.
    /// </summary>
    private void Refresh()
    {
      // Update data grid
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();

      // Update BlobsControl Dataview if visible
      if (Viana.Project.ProcessingData.IsTargetColorSet)
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
      var currentProjectFile = Viana.Project.ProjectPath + Path.DirectorySeparatorChar + Viana.Project.ProjectFilename;
      var title = Labels.SaveProjectDialogTitle;
      title = title.Replace("%1", Viana.Project.ProjectFilename);

      if (File.Exists(currentProjectFile))
      {
        var description = Labels.SaveProjectDialogDescription;
        description = description.Replace("%1", Viana.Project.ProjectFilename);
        var dlg = new VianaSaveDialog(
          title,
          description,
          Labels.SaveProjectDialogMessage);

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          Project.Serialize(Viana.Project, currentProjectFile);
        }
      }
      else
      {
        var description = Labels.AskSaveProjectDialogDescription;
        description = description.Replace("%1", Viana.Project.ProjectFilename);
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
      int count = Viana.Project.ProcessingData.NumberOfTrackedObjects;
      float bandwidth = 26f / count;
      for (int i = 0; i < count; i++)
      {
        drawingContext.DrawRectangle(
          new SolidColorBrush(Viana.Project.ProcessingData.TargetColor[i]),
          null,
          new Rect(3 + i * bandwidth, 3, bandwidth, 27));
      }

      drawingContext.DrawRoundedRectangle(Brushes.Transparent, new Pen(Brushes.White, 2f), new Rect(2, 2, 28, 28), 5, 5);

      drawingContext.Close();
      var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      this.SelectColorRibbonButton.LargeImageSource = bmp;
      Viana.Project.ProcessingData.IsTargetColorSet = false;
      Viana.Project.ProcessingData.IsTargetColorSet = true;
    }

    /// <summary>
    ///   The update select object bindings.
    /// </summary>
    private void UpdateSelectObjectBindings()
    {
      var thresholdBinding = new Binding("ProcessingData.ColorThreshold[" + Viana.Project.ProcessingData.IndexOfObject + "]") { Source = Viana.Project };
      this.SliderThreshold.SetBinding(RangeBase.ValueProperty, thresholdBinding);

      var minDiameterBinding = new Binding("ProcessingData.BlobMinDiameter[" + Viana.Project.ProcessingData.IndexOfObject + "]") { Source = Viana.Project };
      this.SliderMinDiameter.SetBinding(RangeBase.ValueProperty, minDiameterBinding);

      var maxDiameterBinding = new Binding("ProcessingData.BlobMaxDiameter[" + Viana.Project.ProcessingData.IndexOfObject + "]") { Source = Viana.Project };
      this.SliderMaxDiameter.SetBinding(RangeBase.ValueProperty, maxDiameterBinding);

      var motionThresholdBinding = new Binding("ProcessingData.MotionThreshold[" + Viana.Project.ProcessingData.IndexOfObject + "]") { Source = Viana.Project };
      this.SliderMotionPixelThreshold.SetBinding(RangeBase.ValueProperty, motionThresholdBinding);

      var suppressNoiseBinding = new Binding("ProcessingData.SuppressNoise[" + Viana.Project.ProcessingData.IndexOfObject + "]") { Source = Viana.Project };
      this.SuppressNoiseCheckbox.SetBinding(ToggleButton.IsCheckedProperty, suppressNoiseBinding);

      var positiveContrastBinding = new Binding("ProcessingData.PositiveContrast[" + Viana.Project.ProcessingData.IndexOfObject + "]") { Source = Viana.Project };
      this.UsePositiveThresholdCheckbox.SetBinding(ToggleButton.IsCheckedProperty, positiveContrastBinding);
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
        (Viana.Project.ProcessingData.IndexOfObject + 1).ToString("N0"),
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
    /// The measure length button is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MeasureLengthButtonClick(object sender, RoutedEventArgs e)
    {
      var measureLengthWindow = new MeasureLengthWindow();
      measureLengthWindow.ShowDialog();
    }

    /// <summary>
    /// The measure angle button is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MeasureAngleButtonClick(object sender, RoutedEventArgs e)
    {
      var measureAngleWindow = new MeasureAngleWindow();
      measureAngleWindow.ShowDialog();
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

    /// <summary>
    /// Handles the OnSelectionChanged event of the VideoInputDeviceCombo control.
    /// Updates the video capturer with the new device.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Object}"/> instance containing the event data.</param>
    private void VideoInputDeviceCombo_OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      foreach (var videoInputDevice in Video.Instance.VideoInputDevices)
      {
        if (videoInputDevice.Name == this.VideoInputDeviceCombo.SelectedItem.ToString())
        {
          Video.Instance.VideoCapturerElement.VideoCaptureDevice = videoInputDevice;
          break;
        }
      }
    }

    /// <summary>
    /// Skip points button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SkipPointsButtonClick(object sender, RoutedEventArgs e)
    {
      var dlg = new SkipPointsDialog();
      dlg.ShowDialog();
    }


  }
}