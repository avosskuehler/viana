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

      Video.Instance.ImageProcessing.PropertyChanged += this.ImageProcessingPropertyChanged;

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
      this.SelectColorRibbonButton.LargeImageSource = new BitmapImage(largeSource);
      var smallSource = new Uri(@"/VianaNET;component/Images/SelectColor16.png", UriKind.Relative);
      this.SelectColorRibbonButton.SmallImageSource = new BitmapImage(smallSource);
      this.SelectColorRibbonButton.Label = Labels.ButtonSelectColorLabelTitle;
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
      this.videoWindow.RunAutomaticDataAquisition();
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
      this.videoWindow.StopAutomaticDataAquisition();
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
      VideoData.Instance.RefreshDistanceVelocityAcceleration();
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
      this.videoWindow.SetVideoMode(VideoMode.Capture);
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
    private void ExportChartToClipboardButtonClick(object sender, RoutedEventArgs e)
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
    private void ExportChartToFileButtonClick(object sender, RoutedEventArgs e)
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
    private void ExportChartToWordButtonClick(object sender, RoutedEventArgs e)
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
    private void ExportDataToXlsButtonClick(object sender, RoutedEventArgs e)
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
        ExportData.ToXml(VideoData.Instance.Samples, sfd.FileName);
      }
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
    private void InterpolationPropertiesButtonClick(object sender, RoutedEventArgs e)
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
    private void IsInterpolatingDataCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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
    private void IsInterpolatingDataButtonClick(object sender, RoutedEventArgs e)
    {
      Interpolation.Instance.IsInterpolatingData = this.ButtonIsInterpolatingData.IsChecked.GetValueOrDefault();
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
      Video.Instance.ImageProcessing.IndexOfObject++;
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
        this.videoWindow.UpdateCalibration();
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
      this.videoWindow.ShowCalibration(this.ShowCalibrationCheckbox.IsChecked());
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
    private void ChangeColorButtonClick(object sender, RoutedEventArgs e)
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
      this.chartWindow.PropertiesExpander.IsExpanded = true;
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
      this.ShowWindow(this.chartWindow, DockableContentState.Docked);
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
      this.SelectNumberOfObjectsButton.LargeImageSource = bmp;
      if (Video.Instance.ImageProcessing.NumberOfTrackedObjects > 1)
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
    private void ImageProcessingPropertyChanged(object sender, PropertyChangedEventArgs e)
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
      this.videoWindow.SetVideoMode(VideoMode.File);
      this.videoWindow.LoadVideo(string.Empty);
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
      VideoData.Instance.Reset();

      var manualAquisitionWindow = new ManualDataAquisitionWindow();
      manualAquisitionWindow.ShowDialog();

      this.Refresh();

      // Switch to datagrid window
      this.ShowWindow(this.datagridWindow, DockableContentState.Docked);
    }

    ///// <summary>
    ///// The recent items list_ most recent file selected.
    ///// </summary>
    ///// <param name="sender">
    ///// The sender. 
    ///// </param>
    ///// <param name="e">
    ///// The e. 
    ///// </param>
    //private void RecentItemsList_MostRecentFileSelected(object sender, MostRecentFileSelectedEventArgs e)
    //{
    //  var list = e.Source as RibbonHighlightingList;
    //  foreach (RibbonHighlightingListItem item in list.Items)
    //  {
    //    if (item.Content.ToString() == e.SelectedItem.ToString())
    //    {
    //      var tooltip = item.ToolTip as ToolTip;
    //      Video.Instance.VideoPlayerElement.LoadMovie(tooltip.Content.ToString());
    //      break;
    //    }
    //  }
    //}

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
    private void ResetColorButtonClick(object sender, RoutedEventArgs e)
    {
      ColorFactory.ResetColors();
      this.themeCounter = 0;
      this.SetColorScheme();
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
    private void SaveLayoutButtonClick(object sender, RoutedEventArgs e)
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
    /// The restore layout command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RestoreLayoutButtonClick(object sender, RoutedEventArgs e)
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
    /// The save video window image as command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void SaveVideoWindowImageAsButtonClick(object sender, RoutedEventArgs e)
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
    private void SaveVideoWindowImageButtonClick(object sender, RoutedEventArgs e)
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
    private void SelectColorButtonClick(object sender, RoutedEventArgs e)
    {
      var fullScreenVideoWindow = new SelectColorWindow();
      if (fullScreenVideoWindow.ShowDialog().GetValueOrDefault())
      {
        //((RibbonCommand)this.selectColorRibbonButton.Command).LabelTitle = Labels.ButtonSelectedColorLabelTitle;

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
        //((RibbonCommand)this.selectColorRibbonButton.Command).LargeImageSource = bmp;
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
        //this.RibbonTabAnalysis.IsSelected = true;

        // this.RibbonTabDatagrid.IsEnabled = false;
        // this.RibbonTabChart.IsEnabled = false;
      }

      if (window is DataGridWindow)
      {
        // this.RibbonTabVideo.IsEnabled = false;
        // this.RibbonTabDatagrid.IsEnabled = true;
        //this.RibbonTabDatagrid.IsSelected = true;

        // this.RibbonTabChart.IsEnabled = false;
      }

      if (window is ChartWindow)
      {
        // this.RibbonTabVideo.IsEnabled = false;
        // this.RibbonTabDatagrid.IsEnabled = false;
        // this.RibbonTabChart.IsEnabled = true;
        //this.RibbonTabChart.IsSelected = true;
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
    private void StartButtonClick(object sender, RoutedEventArgs e)
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
    private void StopButtonClick(object sender, RoutedEventArgs e)
    {
    }

    /// <summary>
    ///   The update select object bindings.
    /// </summary>
    private void UpdateSelectObjectBindings()
    {
      var thresholdBinding =
        new Binding("ImageProcessing.ColorThreshold[" + Video.Instance.ImageProcessing.IndexOfObject + "]");
      thresholdBinding.Source = Video.Instance;

      //// thresholdBinding.Converter = (IValueConverter)this.Resources["PercentToDoubleConverter"];
      //this.SliderThreshold.SetBinding(RangeBase.ValueProperty, thresholdBinding);

      //var minDiameterBinding =
      //  new Binding("ImageProcessing.BlobMinDiameter[" + Video.Instance.ImageProcessing.IndexOfObject + "]");
      //minDiameterBinding.Source = Video.Instance;
      //this.SliderMinDiameter.SetBinding(RangeBase.ValueProperty, minDiameterBinding);

      //var maxDiameterBinding =
      //  new Binding("ImageProcessing.BlobMaxDiameter[" + Video.Instance.ImageProcessing.IndexOfObject + "]");
      //maxDiameterBinding.Source = Video.Instance;
      //this.SliderMaxDiameter.SetBinding(RangeBase.ValueProperty, maxDiameterBinding);
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
      //((RibbonCommand)this.ButtonSelectObject.Command).LargeImageSource = bmp;

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
    private void VideoWindowButtonClick(object sender, RoutedEventArgs e)
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