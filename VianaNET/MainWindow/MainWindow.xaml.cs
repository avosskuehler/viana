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
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using AvalonDock;

  using Microsoft.Win32;
  using Microsoft.Windows.Controls.Ribbon;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Interpolation;
  using VianaNET.Localization;
  using VianaNET.Modules.Chart;
  using VianaNET.Modules.DataAcquisition;
  using VianaNET.Modules.DataGrid;
  using VianaNET.Modules.Video;
  using VianaNET.Modules.Video.Control;
  using VianaNET.Modules.Video.Dialogs;

  using WPFLocalizeExtension.Engine;

  /// <summary>
  ///   The main window.
  /// </summary>
  public partial class MainWindow : RibbonWindow
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Fields

    /// <summary>
    ///   Saves the chart window of the application
    /// </summary>
    private readonly ChartWindow chartWindow;

    /// <summary>
    ///   Saves the data grid window of the application
    /// </summary>
    private readonly DataGridWindow datagridWindow;

    /// <summary>
    ///   Saves the xaml ribbon theme dictionary list.
    /// </summary>
    private readonly List<string> ribbonThemes;

    /// <summary>
    ///   Saves the video window of the application
    /// </summary>
    private readonly VideoWindow videoWindow;

    /// <summary>
    ///   Saves the index of the current theme
    /// </summary>
    private int themeCounter;

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

      this.ribbonThemes = new List<string>(3);
      this.ribbonThemes.Add("/RibbonControlsLibrary;component/Themes/Office2007Blue.xaml");
      this.ribbonThemes.Add("/RibbonControlsLibrary;component/Themes/Office2007Silver.xaml");
      this.ribbonThemes.Add("/RibbonControlsLibrary;component/Themes/Office2007Black.xaml");

      this.mainRibbon.DataContext = this;

      this.videoWindow = new VideoWindow();
      this.modulePane.Items.Add(this.videoWindow);
      this.datagridWindow = new DataGridWindow();
      this.modulePane.Items.Add(this.datagridWindow);
      this.chartWindow = new ChartWindow();
      this.modulePane.Items.Add(this.chartWindow);

      if (DShowUtils.GetVideoInputDevices().Count == 0)
      {
        this.VideoInputDeviceCombo.Text = "No Video device found.";
        this.VideoInputDeviceCombo.IsEnabled = false;
        this.CaptureVideoButton.Visibility = Visibility.Hidden;
        this.VideoCaptureRibbonGroup.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.VideoInputDeviceCombo.ItemsSource = DShowUtils.GetVideoInputDevices();
        this.VideoInputDeviceCombo.DisplayMemberPath = "Name";
        var captureDeviceBinding = new Binding();
        captureDeviceBinding.Source = Video.Instance;
        captureDeviceBinding.Mode = BindingMode.OneWayToSource;
        captureDeviceBinding.Path = new PropertyPath("VideoCapturerElement.VideoCaptureDevice");
        this.VideoInputDeviceCombo.SetBinding(Selector.SelectedItemProperty, captureDeviceBinding);
        this.VideoInputDeviceCombo.SelectedIndex = 0;
      }

      Video.Instance.ImageProcessing.PropertyChanged += this.ImageProcessing_PropertyChanged;

      // Initializes color scheme
      this.themeCounter = 0;
      this.SetColorScheme();
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
      ((RibbonCommand)this.selectColorRibbonButton.Command).LargeImageSource = new BitmapImage(largeSource);
      var smallSource = new Uri(@"/VianaNET;component/Images/SelectColor16.png", UriKind.Relative);
      ((RibbonCommand)this.selectColorRibbonButton.Command).SmallImageSource = new BitmapImage(smallSource);
      ((RibbonCommand)this.selectColorRibbonButton.Command).LabelTitle = Labels.ButtonSelectColorLabelTitle;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The about command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var aboutWindow = new AboutWindow();
      aboutWindow.ShowDialog();
    }

    /// <summary>
    /// The automatic data aquisition command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AutomaticDataAquisitionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.ImageProcessing.IsTargetColorSet;
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
    private void AutomaticDataAquisitionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.RunAutomaticDataAquisition();
    }

    /// <summary>
    /// The automatic data aquisition stop command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void AutomaticDataAquisitionStopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void AutomaticDataAquisitionStopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.StopAutomaticDataAquisition();
    }

    /// <summary>
    /// The button calculate velocity command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonCalculateVelocityCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 1;
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
    private void ButtonCalculateVelocityCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.Cursor = Cursors.Wait;
      VideoData.Instance.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
    }

    /// <summary>
    /// The button capture video command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonCaptureVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = DShowUtils.GetVideoInputDevices().Count > 0;
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
    private void ButtonCaptureVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.SetVideoMode(VideoMode.Capture);
    }

    /// <summary>
    /// The button choose analysis command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonChooseAnalysisCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    /// <summary>
    /// The button choose analysis command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonChooseAnalysisCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.RibbonTabAnalysis.IsSelected = true;
    }

    /// <summary>
    /// The button choose automatic analysis command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonChooseAutomaticAnalysisCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void ButtonChooseAutomaticAnalysisCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.RibbonTabAnalysis.IsSelected = true;
    }

    /// <summary>
    /// The button delete data command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonDeleteDataCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 0;
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
    private void ButtonDeleteDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      VideoData.Instance.Reset();
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
    private void ButtonExportChartToClipboard_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportChart.ToClipboard(this.chartWindow.DataChart);
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
    private void ButtonExportChartToFile_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportChart.ToFile(this.chartWindow.DataChart);
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
    private void ButtonExportChartToWord_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportChart.ToWord(this.chartWindow.DataChart);
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
    private void ButtonExportDataToCsv_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog();

      // Default file extension
      sfd.DefaultExt = "csv";

      // Available file extensions
      sfd.Filter = Labels.CsvFilter;

      // Adds a extension if the user does not
      sfd.AddExtension = true;

      // Restores the selected directory, next time
      sfd.RestoreDirectory = true;

      // Dialog title
      sfd.Title = Labels.ExportWhereToSaveFile;

      // Startup directory
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToCsv(VideoData.Instance.Samples, sfd.FileName);
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
    private void ButtonExportDataToTxt_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog();

      // Default file extension
      sfd.DefaultExt = "txt";

      // Available file extensions
      sfd.Filter = Labels.TxtFilter;

      // Adds a extension if the user does not
      sfd.AddExtension = true;

      // Restores the selected directory, next time
      sfd.RestoreDirectory = true;

      // Dialog title
      sfd.Title = Labels.ExportWhereToSaveFile;

      // Startup directory
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToTxt(VideoData.Instance.Samples, sfd.FileName);
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
    private void ButtonExportDataToXls_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportData.ToXls(VideoData.Instance.Samples);
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
    private void ButtonExportDataToXml_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      var sfd = new SaveFileDialog();

      // Default file extension
      sfd.DefaultExt = "xml";

      // Available file extensions
      sfd.Filter = Labels.XmlFilter;

      // Adds a extension if the user does not
      sfd.AddExtension = true;

      // Restores the selected directory, next time
      sfd.RestoreDirectory = true;

      // Dialog title
      sfd.Title = Labels.ExportWhereToSaveFile;

      // Startup directory
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToXml(VideoData.Instance.Samples, sfd.FileName);
      }
    }

    /// <summary>
    /// The button export data_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonExportData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 0;
    }

    /// <summary>
    /// The button interpolation properties_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonInterpolationProperties_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Interpolation.ShowInterpolationOptionsDialog();
    }

    /// <summary>
    /// The button is interpolating data command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonIsInterpolatingDataCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 1;
    }

    /// <summary>
    /// The button is interpolating data command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonIsInterpolatingDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Interpolation.Instance.IsInterpolatingData = this.ButtonIsInterpolatingData.IsChecked.GetValueOrDefault();
    }

    /// <summary>
    /// The button other options command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonOtherOptionsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    /// <summary>
    /// The button other options command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonOtherOptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.RibbonTabVideo.Visibility = Visibility.Visible;
      this.RibbonTabVideo.IsSelected = true;
    }

    /// <summary>
    /// The button record video command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonRecordVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = DShowUtils.GetVideoInputDevices().Count > 0;
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
    private void ButtonRecordVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      bool wasCapturing = false;
      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        wasCapturing = true;
        this.videoWindow.SetVideoMode(VideoMode.None);
      }

      var saveVideoDialog = new SaveVideoDialog();
      if (saveVideoDialog.ShowDialog().Value)
      {
        this.videoWindow.SetVideoMode(VideoMode.File);
        this.videoWindow.LoadVideo(saveVideoDialog.LastRecordedVideoFile);
      }
      else if (wasCapturing)
      {
        this.videoWindow.SetVideoMode(VideoMode.Capture);
      }
    }

    /// <summary>
    /// The button select number of objects command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonSelectNumberOfObjectsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void ButtonSelectNumberOfObjectsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Clear all data to correctly recreate data arrays.
      VideoData.Instance.Reset();

      // Increase number of objects, shrink to maximal 3.
      if (Video.Instance.ImageProcessing.NumberOfTrackedObjects == 3)
      {
        Video.Instance.ImageProcessing.NumberOfTrackedObjects = 1;
      }
      else
      {
        Video.Instance.ImageProcessing.NumberOfTrackedObjects++;
      }

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();
    }

    /// <summary>
    /// The button select object command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonSelectObjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void ButtonSelectObjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Video.Instance.ImageProcessing.IndexOfObject++;
    }

    /// <summary>
    /// The button video capture device properties command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonVideoCaptureDevicePropertiesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoCapturerElement.CurrentState == VideoBase.PlayState.Running;
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
    private void ButtonVideoCaptureDevicePropertiesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Video.Instance.VideoCapturerElement.ShowPropertyPageOfVideoDevice();
    }

    /// <summary>
    /// The calibrate video command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrateVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void CalibrateVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var calibrateWindow = new CalibrateVideoWindow();
      if (calibrateWindow.ShowDialog().GetValueOrDefault())
      {
        this.videoWindow.UpdateCalibration();
      }

      this.Refresh();
    }

    /// <summary>
    /// The calibration options show calibration command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationOptionsShowCalibrationCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void CalibrationOptionsShowCalibrationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.ShowCalibration(this.ShowCalibrationCheckbox.IsChecked());
    }

    /// <summary>
    /// The calibration options show clip region command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationOptionsShowClipRegionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void CalibrationOptionsShowClipRegionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.ShowClipRegion(this.ShowClipRegionCheckbox.IsChecked());
    }

    /// <summary>
    /// The change color command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChangeColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.SetColorScheme();
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
    private void ChangeLanguageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      switch (((RibbonComboBoxItem)this.LanguageCombo.SelectedItem).Content.ToString())
      {
        case "Deutsch":
          LocalizeDictionary.Instance.Culture = new CultureInfo("de");

          // LocalizationManager.UICulture = new CultureInfo("de");
          break;
        case "English":
          LocalizeDictionary.Instance.Culture = new CultureInfo("en");

          // LocalizationManager.UICulture = new CultureInfo("en");
          break;
      }

      this.UpdateAllRibbonToolTips();
    }

    /// <summary>
    /// The chart display options command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChartDisplayOptionsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void ChartDisplayOptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.chartWindow.PropertiesExpander.IsExpanded = true;
    }

    /// <summary>
    /// The chart window command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ChartWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void ChartWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.ShowWindow(this.chartWindow, DockableContentState.Docked);
    }

    /// <summary>
    /// The clip video command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ClipVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void ClipVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var clipWindow = new ClipVideoWindow();
      clipWindow.ShowDialog();
      this.videoWindow.UpdateClippingRegion();
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
    private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
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
        Video.Instance.ImageProcessing.NumberOfTrackedObjects.ToString("N0"), 
        LocalizeDictionary.Instance.Culture, 
        FlowDirection.LeftToRight, 
        new Typeface("Verdana"), 
        24d, 
        Brushes.Black);

      drawingContext.DrawText(text, new Point(8, 1));

      drawingContext.Close();
      var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      ((RibbonCommand)this.ButtonSelectNumberOfObjects.Command).LargeImageSource = bmp;
      if (Video.Instance.ImageProcessing.NumberOfTrackedObjects > 1)
      {
        ((RibbonCommand)this.ButtonSelectNumberOfObjects.Command).LabelTitle =
          Labels.ButtonSelectNumberOfObjectsLabelTitle2;
      }
      else
      {
        ((RibbonCommand)this.ButtonSelectNumberOfObjects.Command).LabelTitle =
          Labels.ButtonSelectNumberOfObjectsLabelTitle;
      }
    }

    /// <summary>
    /// The datagrid display units command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DatagridDisplayUnitsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void DatagridDisplayUnitsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // this.datagridWindow.Refresh();
    }

    /// <summary>
    /// The datagrid window command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void DatagridWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void DatagridWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.ShowWindow(this.datagridWindow, DockableContentState.Docked);
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
      if (e.PropertyName == "IndexOfObject" || e.PropertyName == "NumberOfTrackedObjects")
      {
        this.UpdateSelectObjectImage();
      }
    }

    /// <summary>
    /// The load video command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void LoadVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.ResetColorButton();
      this.videoWindow.SetVideoMode(VideoMode.File);
      this.videoWindow.LoadVideo(string.Empty);
      this.UpdateSelectObjectImage();
    }

    /// <summary>
    /// The manual data aquisition command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ManualDataAquisitionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void ManualDataAquisitionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      VideoData.Instance.Reset();

      var manualAquisitionWindow = new ManualDataAquisitionWindow();
      manualAquisitionWindow.ShowDialog();

      this.Refresh();

      // Switch to datagrid window
      this.ShowWindow(this.datagridWindow, DockableContentState.Docked);
    }

    /// <summary>
    /// The new command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    /// <summary>
    /// Occurs when the command associated with this CommandBinding executes.
    /// </summary>
    /// <param name="sender">
    /// Source of the event 
    /// </param>
    /// <param name="e">
    /// A <see cref="ExecutedRoutedEventArgs"/> with the event data. 
    /// </param>
    private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The open command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    /// <summary>
    /// The open command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The recent items list_ most recent file selected.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RecentItemsList_MostRecentFileSelected(object sender, MostRecentFileSelectedEventArgs e)
    {
      var list = e.Source as RibbonHighlightingList;
      foreach (RibbonHighlightingListItem item in list.Items)
      {
        if (item.Content.ToString() == e.SelectedItem.ToString())
        {
          var tooltip = item.ToolTip as ToolTip;
          Video.Instance.VideoPlayerElement.LoadMovie(tooltip.Content.ToString());
          break;
        }
      }
    }

    /// <summary>
    ///   The refresh.
    /// </summary>
    private void Refresh()
    {
      // Update data grid
      VideoData.Instance.RefreshDistanceVelocityAcceleration();

      // Update BlobsControl Dataview if visible
      if (Video.Instance.ImageProcessing.IsTargetColorSet)
      {
        this.videoWindow.BlobsControl.UpdateDataPoints();
      }

      // this.datagridWindow.Refresh();
    }

    /// <summary>
    /// The reset color command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ResetColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ColorFactory.ResetColors();
      this.themeCounter = 0;
      this.SetColorScheme();
    }

    /// <summary>
    /// The reset command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ResetCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    /// <summary>
    /// The reset command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ResetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The restore layout command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RestoreLayoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var ofd = new OpenFileDialog();
      ofd.CheckFileExists = true;
      ofd.CheckPathExists = true;
      ofd.DefaultExt = ".xml";
      ofd.Filter = "All Files|*.*|XML Layout files|*.xml";
      ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      ofd.Multiselect = false;
      ofd.Title = "Please select a data file ...";

      if (ofd.ShowDialog().Value)
      {
        this.modulePane.Items.Clear();

        // this.videoPane.Items.Clear();
        // this.chartPane.Items.Clear();
        // this.datagridPane.Items.Clear();
        this.dockingManager.DeserializationCallback = (s, e_args) =>
          {
            if (e_args.Name == "_contentDummy")
            {
              e_args.Content = new DockableContent();
              e_args.Content.Title = "Dummy Content";
              e_args.Content.Content = new TextBlock { Text = "Content Loaded On Demand!" };
            }
          };

        var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
        this.dockingManager.RestoreLayout(fs);

        // this.videoPane.Items.Add(videoWindow);
        // this.datagridPane.Items.Add(datagridWindow);
        // this.chartPane.Items.Add(this.chartWindow);
        this.modulePane.Items.Add(this.videoWindow);
        this.modulePane.Items.Add(this.datagridWindow);
        this.modulePane.Items.Add(this.chartWindow);

        fs.Close();
      }
    }

    /// <summary>
    /// The ribbon command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RibbonCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    /// <summary>
    /// The ribbon slider_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RibbonSlider_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void RibbonWindow_Closing(object sender, CancelEventArgs e)
    {
      Video.Instance.Cleanup();
    }

    /// <summary>
    /// The save as command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    /// <summary>
    /// The save command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save layout command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveLayoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var sfd = new SaveFileDialog();
      sfd.CheckFileExists = false;
      sfd.CheckPathExists = true;
      sfd.DefaultExt = ".xml";
      sfd.AddExtension = true;
      sfd.Filter = "XML Layout files (*.xml)|*.xml|All Files (*.*)|*.*";
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      sfd.Title = "Please select a data file ...";
      if (sfd.ShowDialog().Value)
      {
        this.dockingManager.SaveLayout(sfd.FileName);
      }
    }

    /// <summary>
    /// The save reconstruction as command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveReconstructionAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save reconstruction command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveReconstructionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save selection as command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveSelectionAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save selection command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveSelectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save video window image as command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveVideoWindowImageAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The save video window image command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveVideoWindowImageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The select color command_ can execute.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SelectColorCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
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
    private void SelectColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var fullScreenVideoWindow = new SelectColorWindow();
      if (fullScreenVideoWindow.ShowDialog().GetValueOrDefault())
      {
        ((RibbonCommand)this.selectColorRibbonButton.Command).LabelTitle = Labels.ButtonSelectedColorLabelTitle;

        var drawingVisual = new DrawingVisual();
        DrawingContext drawingContext = drawingVisual.RenderOpen();
        drawingContext.DrawRoundedRectangle(Brushes.Transparent, null, new Rect(0, 0, 32, 32), 5, 5);
        int count = Video.Instance.ImageProcessing.NumberOfTrackedObjects;
        float bandwidth = 26f / count;
        for (int i = 0; i < count; i++)
        {
          drawingContext.DrawRectangle(
            new SolidColorBrush(Video.Instance.ImageProcessing.TargetColor[i]), 
            null, 
            new Rect(3 + i * bandwidth, 3, bandwidth, 27));
        }

        drawingContext.DrawRoundedRectangle(
          Brushes.Transparent, new Pen(Brushes.White, 2f), new Rect(2, 2, 28, 28), 5, 5);

        drawingContext.Close();
        var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(drawingVisual);
        ((RibbonCommand)this.selectColorRibbonButton.Command).LargeImageSource = bmp;
        Video.Instance.ImageProcessing.IsTargetColorSet = false;
        Video.Instance.ImageProcessing.IsTargetColorSet = true;

        // this.videoWindow.BlobsControl.Visibility = Visibility.Visible;
        // Video.Instance.UpdateNativeBitmap();
      }
    }

    /// <summary>
    ///   The set color scheme.
    /// </summary>
    private void SetColorScheme()
    {
      foreach (ResourceDictionary dictionary in this.Resources.MergedDictionaries)
      {
        if (dictionary.Source.ToString().Contains("Office"))
        {
          dictionary.Source = new Uri(this.ribbonThemes[this.themeCounter], UriKind.RelativeOrAbsolute);

          foreach (object identifier in dictionary.Keys)
          {
            if (identifier is ComponentResourceKey)
            {
              var resource = identifier as ComponentResourceKey;
              if (resource.ResourceId.ToString() == "RibbonBackgroundBrush")
              {
                SolidColorBrush ribbonBackgroundBrush = ((SolidColorBrush)dictionary[identifier]).Clone();
                this.Background = ribbonBackgroundBrush;
                this.dockingManager.Background = ribbonBackgroundBrush;
              }

              if (resource.ResourceId.ToString() == "GroupHostBorderBrush")
              {
                SolidColorBrush ribbonBorderBackgroundBrush = ((SolidColorBrush)dictionary[identifier]).Clone();
                ColorFactory.ChangeColors(ribbonBorderBackgroundBrush.Color);
              }

              if (resource.ResourceId.ToString() == "GroupHostBackgroundBrush")
              {
                LinearGradientBrush groupHostBackgroundBrush = ((LinearGradientBrush)dictionary[identifier]).Clone();
                Calibration.Instance.GradientBackground = groupHostBackgroundBrush;
              }
            }
          }
        }
      }

      this.themeCounter++;
      if (this.themeCounter >= 3)
      {
        this.themeCounter = 0;
      }
    }

    /// <summary>
    /// The show ribbon tabs.
    /// </summary>
    /// <param name="window">
    /// The window. 
    /// </param>
    private void ShowRibbonTabs(DockableContent window)
    {
      if (window is VideoWindow)
      {
        // this.RibbonTabVideo.IsEnabled = true;
        this.RibbonTabAnalysis.IsSelected = true;

        // this.RibbonTabDatagrid.IsEnabled = false;
        // this.RibbonTabChart.IsEnabled = false;
      }

      if (window is DataGridWindow)
      {
        // this.RibbonTabVideo.IsEnabled = false;
        // this.RibbonTabDatagrid.IsEnabled = true;
        this.RibbonTabDatagrid.IsSelected = true;

        // this.RibbonTabChart.IsEnabled = false;
      }

      if (window is ChartWindow)
      {
        // this.RibbonTabVideo.IsEnabled = false;
        // this.RibbonTabDatagrid.IsEnabled = false;
        // this.RibbonTabChart.IsEnabled = true;
        this.RibbonTabChart.IsSelected = true;
      }
    }

    /// <summary>
    /// The show window.
    /// </summary>
    /// <param name="contentToShow">
    /// The content to show. 
    /// </param>
    /// <param name="desideredState">
    /// The desidered state. 
    /// </param>
    private void ShowWindow(DockableContent contentToShow, DockableContentState desideredState)
    {
      if (desideredState == DockableContentState.AutoHide || desideredState == DockableContentState.FloatingWindow)
      {
        this.dockingManager.Show(contentToShow, desideredState);
      }
      else
      {
        this.dockingManager.Show(contentToShow);
      }
    }

    /// <summary>
    /// The start command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void StartCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    /// The stop command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    /// <summary>
    ///   The update all ribbon tool tips.
    /// </summary>
    private void UpdateAllRibbonToolTips()
    {
      foreach (RibbonButton button in this.mainRibbon.QuickAccessToolBar.Items)
      {
        ((RibbonToolTip)button.ToolTip).Title = ((RibbonCommand)button.Command).ToolTipTitle;
        ((RibbonToolTip)button.ToolTip).Description = ((RibbonCommand)button.Command).ToolTipDescription;
      }

      foreach (object item in this.mainRibbon.ApplicationMenu.Items)
      {
        if (item is RibbonApplicationMenuItem)
        {
          var menuItem = item as RibbonApplicationMenuItem;
          ((RibbonToolTip)menuItem.ToolTip).Title = ((RibbonCommand)menuItem.Command).ToolTipTitle;
          ((RibbonToolTip)menuItem.ToolTip).Description = ((RibbonCommand)menuItem.Command).ToolTipDescription;
        }
      }

      foreach (RibbonTab tab in this.mainRibbon.Tabs)
      {
        foreach (RibbonGroup group in tab.Groups)
        {
          foreach (Control control in group.Controls)
          {
            if (control is RibbonButton)
            {
              var button = control as RibbonButton;
              ((RibbonToolTip)control.ToolTip).Title = ((RibbonCommand)button.Command).ToolTipTitle;
              ((RibbonToolTip)control.ToolTip).Description = ((RibbonCommand)button.Command).ToolTipDescription;
            }
            else if (control is RibbonComboBox)
            {
              var comboBox = control as RibbonComboBox;
              ((RibbonToolTip)control.ToolTip).Title = ((RibbonCommand)comboBox.Command).ToolTipTitle;
              ((RibbonToolTip)control.ToolTip).Description = ((RibbonCommand)comboBox.Command).ToolTipDescription;
            }
          }
        }
      }
    }

    /// <summary>
    ///   The update select object bindings.
    /// </summary>
    private void UpdateSelectObjectBindings()
    {
      var thresholdBinding =
        new Binding("ImageProcessing.ColorThreshold[" + Video.Instance.ImageProcessing.IndexOfObject + "]");
      thresholdBinding.Source = Video.Instance;

      // thresholdBinding.Converter = (IValueConverter)this.Resources["PercentToDoubleConverter"];
      this.SliderThreshold.SetBinding(RangeBase.ValueProperty, thresholdBinding);

      var minDiameterBinding =
        new Binding("ImageProcessing.BlobMinDiameter[" + Video.Instance.ImageProcessing.IndexOfObject + "]");
      minDiameterBinding.Source = Video.Instance;
      this.SliderMinDiameter.SetBinding(RangeBase.ValueProperty, minDiameterBinding);

      var maxDiameterBinding =
        new Binding("ImageProcessing.BlobMaxDiameter[" + Video.Instance.ImageProcessing.IndexOfObject + "]");
      maxDiameterBinding.Source = Video.Instance;
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
        (Video.Instance.ImageProcessing.IndexOfObject + 1).ToString("N0"), 
        LocalizeDictionary.Instance.Culture, 
        FlowDirection.LeftToRight, 
        new Typeface("Verdana"), 
        18d, 
        Brushes.Black);

      drawingContext.DrawText(text, new Point(10, 3));

      drawingContext.Close();
      var bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      ((RibbonCommand)this.ButtonSelectObject.Command).LargeImageSource = bmp;

      this.UpdateSelectObjectBindings();
    }

    /// <summary>
    /// Occurs when the command associated with this 
    ///   CommandBinding initiates a check to determine whether 
    ///   the command can be executed on the command target.
    /// </summary>
    /// <param name="sender">
    /// Source of the event 
    /// </param>
    /// <param name="e">
    /// A <see cref="CanExecuteRoutedEventArgs"/> with the event data. 
    /// </param>
    private void VideoWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
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
    private void VideoWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.ShowWindow(this.videoWindow, DockableContentState.Docked);
    }

    /// <summary>
    /// The module pane_ request bring into view.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void modulePane_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      if (e.TargetObject is DockableContent)
      {
        this.ShowRibbonTabs((DockableContent)e.TargetObject);
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}