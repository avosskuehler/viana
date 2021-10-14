// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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


    /// <summary>
    ///   Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
      //BindingErrorTraceListener.SetTrace();
      this.InitializeComponent();

      this.MainRibbon.DataContext = this;
      App.Project.ProcessingData.PropertyChanged += this.ProcessingDataPropertyChanged;
      this.CreateImageSourceForNumberOfObjects();
      this.UpdateSelectObjectImage();

      List<CameraDevice> devices = Video.Instance.VideoInputDevicesMSMF;

      this.VideoSourceGalleryCategory.ItemsSource = devices;
      if (Video.Instance.VideoInputDevicesMSMF.Count > 0)
      {
        this.VideoInputDeviceCombo.SelectedItem = Video.Instance.VideoInputDevicesMSMF[0];
      }

      this.Show();
    }





    /// <summary>
    ///   The reset color button.
    /// </summary>
    public void ResetColorButton()
    {
      Uri largeSource = new Uri(@"/VianaNET;component/Images/SelectColor32.png", UriKind.Relative);
      this.SelectColorRibbonButton.LargeImageSource = new BitmapImage(largeSource);
      Uri smallSource = new Uri(@"/VianaNET;component/Images/SelectColor16.png", UriKind.Relative);
      this.SelectColorRibbonButton.SmallImageSource = new BitmapImage(smallSource);
      this.SelectColorRibbonButton.Label = VianaNET.Localization.Labels.ButtonSelectColorLabelTitle;
    }





    /// <summary>
    /// This method saves the current project into a new file using
    /// the file name give by the called save file dialog.
    /// </summary>
    private static void SaveProjectToNewFile()
    {
      SaveFileDialog dlg = new SaveFileDialog
      {
        Filter = "Viana.NET projects|*.via",
        Title = VianaNET.Localization.Labels.SaveProjectDialogTitle,
        DefaultExt = "via"
      };

      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        Project.Serialize(App.Project, dlg.FileName);

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
      if (File.Exists(App.Project.ProjectFilename))
      {
        // Save file
        Project.Serialize(App.Project, App.Project.ProjectFilename);

        // Add project file to recent files list
        RecentFiles.Instance.Add(App.Project.ProjectFilename);
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
      if (!string.IsNullOrEmpty(App.Project.ProjectFilename))
      {
        VianaDialog dlg = new VianaDialog(
          VianaNET.Localization.Labels.AskSaveProjectDialogTitle,
          VianaNET.Localization.Labels.AskSaveProjectDialogDescription,
          VianaNET.Localization.Labels.AskSaveProjectDialogMessage,
          false);
        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          SaveCurrentProject();
        }
      }

      // Note that we want to overwrite the defaults
      Project.IsDeserializing = true;
      App.Project = new Project();
      //App.Project.ProjectFilename = openedProject.ProjectFilename;
      //App.Project.ProjectPath = openedProject.ProjectPath;
      //App.Project.VideoFile = openedProject.VideoFile;
      //App.Project.VideoMode = openedProject.VideoMode;

      // Restore video mode
      Video.Instance.VideoMode = VideoMode.File;

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();

      // Update datagrid
      App.Project.VideoData.NotifyLoading();
      this.DataGridWindow.Refresh();

      // Update data series values
      this.ChartWindow.Refresh();

      this.VideoWindow.BlobsControl.ResetBlobsControl();
      Video.Instance.OriginalImageSource = App.GetImageSource("NoVideo800600.png");

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
      OpenFileDialog dlg = new OpenFileDialog
      {
        Filter = "Viana.NET projects|*.via",
        Title = VianaNET.Localization.Labels.OpenProjectDialogTitle,
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
      Project openedProject = Project.Deserialize(filename);

      // Direct overwrite of Project static instance will kill all xaml driven bindings...
      // so update all properties individually.
      // VianaNetApplication.Project = openedProject;
      App.Project.VideoData.Samples = openedProject.VideoData.Samples;
      //App.Project.VideoData.ActiveObject = openedProject.VideoData.ActiveObject;
      App.Project.VideoData.LastPoint = openedProject.VideoData.LastPoint;
      App.Project.VideoData.TimeZeroPositionInMs = openedProject.VideoData.TimeZeroPositionInMs;

      App.Project.CalibrationData.ClipRegion = openedProject.CalibrationData.ClipRegion;
      App.Project.CalibrationData.GradientBackground = openedProject.CalibrationData.GradientBackground;
      App.Project.CalibrationData.HasClipRegion = openedProject.CalibrationData.HasClipRegion;
      App.Project.CalibrationData.IsShowingUnits = openedProject.CalibrationData.IsShowingUnits;
      App.Project.CalibrationData.IsVideoCalibrated = openedProject.CalibrationData.IsVideoCalibrated;
      App.Project.CalibrationData.OriginInPixel = openedProject.CalibrationData.OriginInPixel;
      App.Project.CalibrationData.LengthUnit = openedProject.CalibrationData.LengthUnit;
      App.Project.CalibrationData.RulerDescription = openedProject.CalibrationData.RulerDescription;
      App.Project.CalibrationData.RulerEndPointInPixel = openedProject.CalibrationData.RulerEndPointInPixel;
      App.Project.CalibrationData.RulerStartPointInPixel = openedProject.CalibrationData.RulerStartPointInPixel;
      App.Project.CalibrationData.RulerUnit = openedProject.CalibrationData.RulerUnit;
      App.Project.CalibrationData.RulerValueInRulerUnits = openedProject.CalibrationData.RulerValueInRulerUnits;
      App.Project.CalibrationData.ScalePixelToUnit = openedProject.CalibrationData.ScalePixelToUnit;
      App.Project.CalibrationData.TimeUnit = openedProject.CalibrationData.TimeUnit;

      App.Project.SerializableFilterData = openedProject.SerializableFilterData;
      App.Project.CurrentChartType = openedProject.CurrentChartType;

      //App.Project.CurrentFilterData.NumericPrecision = openedProject.CurrentFilterData.NumericPrecision;
      //App.Project.CurrentFilterData.DataLineColor = openedProject.CurrentFilterData.DataLineColor;
      //App.Project.CurrentFilterData.DataLineThickness = openedProject.CurrentFilterData.DataLineThickness;
      //App.Project.CurrentFilterData.InterpolationLineColor = openedProject.CurrentFilterData.InterpolationLineColor;
      //App.Project.CurrentFilterData.InterpolationLineThickness = openedProject.CurrentFilterData.InterpolationLineThickness;
      //App.Project.CurrentFilterData.RegressionLineColor = openedProject.CurrentFilterData.RegressionLineColor;
      //App.Project.CurrentFilterData.RegressionLineThickness = openedProject.CurrentFilterData.RegressionLineThickness;
      //App.Project.CurrentFilterData.TheoryLineColor = openedProject.CurrentFilterData.TheoryLineColor;
      //App.Project.CurrentFilterData.TheoryLineThickness = openedProject.CurrentFilterData.TheoryLineThickness;
      //App.Project.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree =
      //  openedProject.CurrentFilterData.TheoryFilter.TheoreticalFunctionCalculatorTree;
      //App.Project.CurrentFilterData.AxisX = openedProject.CurrentFilterData.AxisX;
      //App.Project.CurrentFilterData.AxisY = openedProject.CurrentFilterData.AxisY;
      //App.Project.CurrentFilterData.IsShowingDataSeries = openedProject.CurrentFilterData.IsShowingDataSeries;
      //App.Project.CurrentFilterData.IsShowingInterpolationSeries = openedProject.CurrentFilterData.IsShowingInterpolationSeries;
      //App.Project.CurrentFilterData.IsShowingRegressionSeries = openedProject.CurrentFilterData.IsShowingRegressionSeries;
      //App.Project.CurrentFilterData.IsShowingTheorySeries = openedProject.CurrentFilterData.IsShowingTheorySeries;

      //App.Project.ProcessingData.Reset();
      for (int i = 0; i < openedProject.ProcessingData.BlobMaxDiameter.Count; i++)
      {
        if (App.Project.ProcessingData.BlobMaxDiameter.Count > i)
        {
          App.Project.ProcessingData.BlobMaxDiameter[i] = openedProject.ProcessingData.BlobMaxDiameter[i];
          App.Project.ProcessingData.BlobMinDiameter[i] = openedProject.ProcessingData.BlobMinDiameter[i];
          App.Project.ProcessingData.ColorThreshold[i] = openedProject.ProcessingData.ColorThreshold[i];
        }
        else
        {
          App.Project.ProcessingData.BlobMaxDiameter.Add(openedProject.ProcessingData.BlobMaxDiameter[i]);
          App.Project.ProcessingData.BlobMinDiameter.Add(openedProject.ProcessingData.BlobMinDiameter[i]);
          App.Project.ProcessingData.ColorThreshold.Add(openedProject.ProcessingData.ColorThreshold[i]);
        }
      }

      App.Project.ProcessingData.CurrentBlobCenter = openedProject.ProcessingData.CurrentBlobCenter;
      App.Project.ProcessingData.DetectedBlob = openedProject.ProcessingData.DetectedBlob;
      App.Project.ProcessingData.IndexOfObject = openedProject.ProcessingData.IndexOfObject;

      App.Project.ProcessingData.IsTargetColorSet = openedProject.ProcessingData.IsTargetColorSet;
      for (int i = 0; i < openedProject.ProcessingData.TargetColor.Count - openedProject.ProcessingData.NumberOfTrackedObjects; i++)
      {
        App.Project.ProcessingData.TargetColor.RemoveAt(i);
      }

      App.Project.ProcessingData.NumberOfTrackedObjects = openedProject.ProcessingData.NumberOfTrackedObjects;

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();

      this.UpdateColorButton();

      App.Project.ProjectFilename = filename;
      App.Project.VideoFile = openedProject.VideoFile;
      App.Project.VideoMode = openedProject.VideoMode;
      //this.VideoWindow.CreateCrossHairLines();

      // after updating series do calculation
      App.Project.CurrentFilterData.RegressionFilter = App.Project.CurrentFilterData.RegressionFilter;
      App.Project.CurrentFilterData.RegressionAberration = App.Project.CurrentFilterData.RegressionAberration;

      // Restore video mode
      Video.Instance.VideoMode = App.Project.VideoMode;
      switch (App.Project.VideoMode)
      {
        case VideoMode.File:
          // load video
          this.VideoWindow.LoadVideo(App.Project.VideoFile);
          break;
        case VideoMode.Capture:
          break;
      }

      // Set selection after loading video
      App.Project.VideoData.SelectionStart = openedProject.VideoData.SelectionStart;
      App.Project.VideoData.SelectionEnd = openedProject.VideoData.SelectionEnd;
      this.VideoWindow.TimelineSlider.UpdateSelectionTimes();

      Video.Instance.Revert();

      // Update datagrid
      App.Project.VideoData.NotifyLoading();
      this.DataGridWindow.Refresh();

      // Update data series values
      this.ChartWindow.Refresh();

      this.VideoWindow.BlobsControl.ResetBlobsControl();
      this.Refresh();
      this.UpdateColorButton();
      this.UpdateSelectObjectImage();

      this.VideoWindow.UpdateClippingRegion();
      this.VideoWindow.UpdateCalibration();

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
      AboutWindow aboutWindow = new AboutWindow();
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
      App.Project.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// Shows the video infos and allows to change the time scale.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ShowVideoInfosButtonClick(object sender, RoutedEventArgs e)
    {
      VideoInfoDialog dialog = new VideoInfoDialog();
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
      TimeUnitDialog dlg = new TimeUnitDialog();
      dlg.ShowDialog();
      this.Cursor = Cursors.Wait;
      App.Project.VideoData.RefreshDistanceVelocityAcceleration();
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
      DifferenceQuotientDialog dlg = new DifferenceQuotientDialog();
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
        Video.Instance.VideoCapturerElement.VideoCaptureDevice = Video.Instance.VideoInputDevicesMSMF[0];
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
      App.Project.VideoData.Reset();
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
      SaveFileDialog sfd = new SaveFileDialog
      {
        // Default file extension
        DefaultExt = "csv",

        // Available file extensions
        Filter = VianaNET.Localization.Labels.CsvFilter,

        // Adds a extension if the user does not
        AddExtension = true,

        // Restores the selected directory, next time
        RestoreDirectory = true,

        // Dialog title
        Title = VianaNET.Localization.Labels.ExportWhereToSaveFile,

        // Startup directory
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
      };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportOptionsDialog optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToCsv(App.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions, sfd.FileName);
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
      SaveFileDialog sfd = new SaveFileDialog
      {
        DefaultExt = "txt",
        Filter = VianaNET.Localization.Labels.TxtFilter,
        AddExtension = true,
        RestoreDirectory = true,
        Title = VianaNET.Localization.Labels.ExportWhereToSaveFile,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
      };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportOptionsDialog optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToTxt(App.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions, sfd.FileName);
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
      ExportOptionsDialog optionsDialog = new ExportOptionsDialog();
      if (optionsDialog.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToXls(App.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions);
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
      SaveFileDialog sfd = new SaveFileDialog
      {
        DefaultExt = "xml",
        Filter = VianaNET.Localization.Labels.XmlFilter,
        AddExtension = true,
        RestoreDirectory = true,
        Title = VianaNET.Localization.Labels.ExportWhereToSaveFile,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
      };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportOptionsDialog optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToXml(App.Project.VideoData.FilteredSamples, optionsDialog.ExportOptions, sfd.FileName);
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
      SaveFileDialog sfd = new SaveFileDialog
      {
        DefaultExt = "ods",
        Filter = VianaNET.Localization.Labels.OdsFilter,
        AddExtension = true,
        RestoreDirectory = true,
        Title = VianaNET.Localization.Labels.ExportWhereToSaveFile,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
      };

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportOptionsDialog optionsDialog = new ExportOptionsDialog();
        if (optionsDialog.ShowDialog().GetValueOrDefault())
        {
          ExportData.ToOds(App.Project.VideoData.FilteredSamples, sfd.FileName, optionsDialog.ExportOptions);
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

      using (SaveVideoDialog saveVideoDialog = new SaveVideoDialog())
      {
        bool? showDialog = saveVideoDialog.ShowDialog();
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
      App.Project.VideoData.Reset();

      // Increase number of objects, shrink to maximal 3.
      if (App.Project.ProcessingData.NumberOfTrackedObjects == 3)
      {
        App.Project.ProcessingData.NumberOfTrackedObjects = 1;
      }
      else
      {
        App.Project.ProcessingData.NumberOfTrackedObjects++;
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
      App.Project.ProcessingData.IndexOfObject++;
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
      CalibrateVideoWindow calibrateWindow = new CalibrateVideoWindow();
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
      CoordinateSystemWindow coordinateSystemWindow = new CoordinateSystemWindow();
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
      ClipVideoWindow clipWindow = new ClipVideoWindow();
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
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRoundedRectangle(Brushes.Transparent, null, new Rect(0, 0, 32, 32), 5, 5);

      // drawingContext.DrawRoundedRectangle((Brush)this.Resources["DefaultOfficeBackgroundBrush"],
      // new Pen(Brushes.White, 2f),
      // new Rect(2, 2, 28, 28),
      // 5,
      // 5);
      FormattedText text = new FormattedText(
        App.Project.ProcessingData.NumberOfTrackedObjects.ToString("N0"),
        LocalizeDictionary.Instance.Culture,
        FlowDirection.LeftToRight,
        new Typeface("Verdana"),
        24d,
        Brushes.Black,
        1);

      drawingContext.DrawText(text, new Point(8, 1));

      drawingContext.Close();
      RenderTargetBitmap bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      this.SelectNumberOfObjectsButton.LargeImageSource = bmp;

      if (App.Project.ProcessingData.NumberOfTrackedObjects > 1)
      {
        this.SelectNumberOfObjectsButton.Label = VianaNET.Localization.Labels.ButtonSelectNumberOfObjectsLabelTitle2;
      }
      else
      {
        this.SelectNumberOfObjectsButton.Label = VianaNET.Localization.Labels.ButtonSelectNumberOfObjectsLabelTitle;
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
      App.Project.VideoData.Reset();

      ManualDataAquisitionWindow manualAquisitionWindow = new ManualDataAquisitionWindow();
      manualAquisitionWindow.ShowDialog();

      this.Refresh();

      // Switch to chart window
      this.ChartTab.IsSelected = true;
    }

    /// <summary>
    /// Modifiy data button click.
    /// Start the modify control.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void ModifyDataButtonClick(object sender, RoutedEventArgs e)
    {
      ModifyDataWindow modifyDataWindow = new ModifyDataWindow();
      modifyDataWindow.ShowDialog();
      App.Project.VideoData.RefreshDistanceVelocityAcceleration();
    }

    /// <summary>
    ///   The refresh.
    /// </summary>
    private void Refresh()
    {
      // Update data grid
      App.Project.VideoData.RefreshDistanceVelocityAcceleration();

      // Update BlobsControl Dataview if visible
      if (App.Project.ProcessingData.IsTargetColorSet)
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

      string title = VianaNET.Localization.Labels.SaveProjectDialogTitle;
      title = title.Replace("%1", App.Project.ProjectFile);

      if (File.Exists(App.Project.ProjectFilename))
      {
        string description = VianaNET.Localization.Labels.SaveProjectDialogDescription;
        description = description.Replace("%1", App.Project.ProjectFile);
        VianaSaveDialog dlg = new VianaSaveDialog(
          title,
          description,
          VianaNET.Localization.Labels.SaveProjectDialogMessage);

        if (dlg.ShowDialog().GetValueOrDefault(false))
        {
          Project.Serialize(App.Project, App.Project.ProjectFilename);
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(App.Project.VideoFile) || Video.Instance.HasVideo)
        {
          string description = VianaNET.Localization.Labels.AskSaveProjectDialogDescription;
          description = description.Replace("%1", App.Project.ProjectFile);
          VianaSaveDialog dlg = new VianaSaveDialog(
            title,
            description,
            VianaNET.Localization.Labels.AskSaveProjectDialogMessage);

          if (dlg.ShowDialog().GetValueOrDefault(false))
          {
            SaveProjectToNewFile();
          }
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
      SelectColorWindow fullScreenVideoWindow = new SelectColorWindow();
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
      this.SelectColorRibbonButton.Label = VianaNET.Localization.Labels.ButtonSelectedColorLabelTitle;

      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRoundedRectangle(Brushes.Transparent, null, new Rect(0, 0, 32, 32), 5, 5);
      int count = App.Project.ProcessingData.NumberOfTrackedObjects;
      float bandwidth = 26f / count;
      for (int i = 0; i < count; i++)
      {
        drawingContext.DrawRectangle(
          new SolidColorBrush(App.Project.ProcessingData.TargetColor[i]),
          null,
          new Rect(3 + i * bandwidth, 3, bandwidth, 27));
      }

      drawingContext.DrawRoundedRectangle(Brushes.Transparent, new Pen(Brushes.White, 2f), new Rect(2, 2, 28, 28), 5, 5);

      drawingContext.Close();
      RenderTargetBitmap bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      this.SelectColorRibbonButton.LargeImageSource = bmp;
      App.Project.ProcessingData.IsTargetColorSet = false;
      App.Project.ProcessingData.IsTargetColorSet = true;
    }

    /// <summary>
    ///   The update select object bindings.
    /// </summary>
    private void UpdateSelectObjectBindings()
    {
      Binding thresholdBinding = new Binding("ProcessingData.ColorThreshold[" + App.Project.ProcessingData.IndexOfObject + "]") { Source = App.Project };
      this.SliderThreshold.SetBinding(RangeBase.ValueProperty, thresholdBinding);

      Binding minDiameterBinding = new Binding("ProcessingData.BlobMinDiameter[" + App.Project.ProcessingData.IndexOfObject + "]") { Source = App.Project };
      this.SliderMinDiameter.SetBinding(RangeBase.ValueProperty, minDiameterBinding);

      Binding maxDiameterBinding = new Binding("ProcessingData.BlobMaxDiameter[" + App.Project.ProcessingData.IndexOfObject + "]") { Source = App.Project };
      this.SliderMaxDiameter.SetBinding(RangeBase.ValueProperty, maxDiameterBinding);

      Binding motionThresholdBinding = new Binding("ProcessingData.MotionThreshold[" + App.Project.ProcessingData.IndexOfObject + "]") { Source = App.Project };
      this.SliderMotionPixelThreshold.SetBinding(RangeBase.ValueProperty, motionThresholdBinding);

      Binding suppressNoiseBinding = new Binding("ProcessingData.SuppressNoise[" + App.Project.ProcessingData.IndexOfObject + "]") { Source = App.Project };
      this.SuppressNoiseCheckbox.SetBinding(ToggleButton.IsCheckedProperty, suppressNoiseBinding);

      Binding positiveContrastBinding = new Binding("ProcessingData.PositiveContrast[" + App.Project.ProcessingData.IndexOfObject + "]") { Source = App.Project };
      this.UsePositiveThresholdCheckbox.SetBinding(ToggleButton.IsCheckedProperty, positiveContrastBinding);
    }

    /// <summary>
    ///   The update select object image.
    /// </summary>
    private void UpdateSelectObjectImage()
    {
      BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/VianaNET;component/Images/SelectObject32.png"));

      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawImage(icon, new Rect(0, 0, 32, 32));
      FormattedText text = new FormattedText(
        (App.Project.ProcessingData.IndexOfObject + 1).ToString("N0"),
        LocalizeDictionary.Instance.Culture,
        FlowDirection.LeftToRight,
        new Typeface("Verdana"),
        18d,
        Brushes.Black,
        1);

      drawingContext.DrawText(text, new Point(10, 3));

      drawingContext.Close();
      RenderTargetBitmap bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
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
      MeasureLengthWindow measureLengthWindow = new MeasureLengthWindow();
      measureLengthWindow.ShowDialog();
    }

    /// <summary>
    /// The measure angle button is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void MeasureAngleButtonClick(object sender, RoutedEventArgs e)
    {
      MeasureAngleWindow measureAngleWindow = new MeasureAngleWindow();
      measureAngleWindow.ShowDialog();
    }

    /// <summary>
    /// This method is called whenever a recent project is selected from the list.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments</param>
    private void RecentProjectMouseDown(object sender, RoutedEventArgs e)
    {
      if (!(sender is ContentPresenter contentPresenter))
      {
        return;
      }


      if (!(contentPresenter.Content is ProjectEntry projectEntry))
      {
        return;
      }

      // now we got the projects filename and may load it.
      this.OpenGivenProject(projectEntry.ProjectFile);

      // Close the application menu
      this.MainRibbon.ApplicationMenu.IsDropDownOpen = false;
    }



    /// <summary>
    /// Handles the OnSelectionChanged event of the VideoInputDeviceCombo control.
    /// Updates the video capturer with the new device.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Object}"/> instance containing the event data.</param>
    private void VideoInputDeviceCombo_OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Video.Instance.VideoCapturerElement.VideoCaptureDevice = Video.Instance.VideoInputDevicesMSMF.FirstOrDefault(o => o.Index == ((CameraDevice)this.VideoInputDeviceCombo.SelectedItem).Index);
    }

    /// <summary>
    /// Skip points button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SkipPointsButtonClick(object sender, RoutedEventArgs e)
    {
      SkipPointsDialog dlg = new SkipPointsDialog();
      dlg.ShowDialog();
    }
  }
}