# region Using Directives

using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using AvalonDock;
using System.Windows.Documents;
using System.IO;
using System.Reflection;
using Microsoft.Windows.Controls.Ribbon;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Globalization;
using WPFLocalizeExtension.Engine;
using System.Windows.Data;
# endregion

namespace VianaNET
{
  public partial class MainWindow : RibbonWindow
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    /// <summary>
    /// Saves the video window of the application
    /// </summary>
    private VideoWindow videoWindow;

    /// <summary>
    /// Saves the chart window of the application
    /// </summary>
    private ChartWindow chartWindow;

    /// <summary>
    /// Saves the data grid window of the application
    /// </summary>
    private DataGridWindow datagridWindow;

    /// <summary>
    /// Saves the index of the current theme
    /// </summary>
    private int themeCounter;

    /// <summary>
    /// Saves the xaml ribbon theme dictionary list.
    /// </summary>
    private List<string> ribbonThemes;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

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
        Binding captureDeviceBinding = new Binding();
        captureDeviceBinding.Source = Video.Instance;
        captureDeviceBinding.Mode = BindingMode.OneWayToSource;
        captureDeviceBinding.Path = new PropertyPath("VideoCapturerElement.VideoCaptureDevice");
        this.VideoInputDeviceCombo.SetBinding(RibbonComboBoxSimple.SelectedItemProperty, captureDeviceBinding);
        this.VideoInputDeviceCombo.SelectedIndex = 0;
      }

      // Initializes color scheme
      this.themeCounter = 0;
      SetColorScheme();
      CreateImageSourceForNumberOfObjects();

      this.Show();
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES
    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS
    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    #region CanExecute

    /// <summary>
    /// Occurs when the command associated with this 
    /// CommandBinding initiates a check to determine whether 
    /// the command can be executed on the command target.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> with the event data.</param>
    private void VideoWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void DatagridWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void ChartWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void ResetCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void RibbonCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void CalibrationOptionsShowCalibrationCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void CalibrationOptionsShowClipRegionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void DatagridDisplayUnitsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void SelectColorCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void RibbonSlider_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void ButtonSelectNumberOfObjectsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void CalibrateVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void ManualDataAquisitionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void AutomaticDataAquisitionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.ImageProcessing.IsTargetColorSet;
    }

    private void ButtonOtherOptionsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void ClipVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void ButtonExportData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 0;
    }

    #endregion //CanExecute

    #region Executed

    /// <summary>
    /// Occurs when the command associated with this CommandBinding executes.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">A <see cref="ExecutedRoutedEventArgs"/> with the event data.</param>
    private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveVideoWindowImageAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveVideoWindowImageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveReconstructionAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveReconstructionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveSelectionAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void SaveSelectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.Close();
    }

    private void ResetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void StartCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      AboutWindow aboutWindow = new AboutWindow();
      aboutWindow.ShowDialog();
    }

    private void DatagridWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ShowWindow(this.datagridWindow, DockableContentState.Docked);
    }

    private void ChartWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ShowWindow(this.chartWindow, DockableContentState.Docked);
    }

    private void VideoWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ShowWindow(this.videoWindow, DockableContentState.Docked);
    }

    private void DatagridDisplayUnitsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      //this.datagridWindow.Refresh();
    }

    private void CalibrationOptionsShowCalibrationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.ShowCalibration(this.ShowCalibrationCheckbox.IsChecked());
    }

    private void CalibrationOptionsShowClipRegionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.ShowClipRegion(this.ShowClipRegionCheckbox.IsChecked());
    }

    private void ChangeColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SetColorScheme();
    }

    private void ChangeLanguageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      switch (((RibbonComboBoxItem)this.LanguageCombo.SelectedItem).Content.ToString())
      {
        case "Deutsch":
          LocalizeDictionary.Instance.Culture = new CultureInfo("de");
          //LocalizationManager.UICulture = new CultureInfo("de");
          break;
        case "English":
          LocalizeDictionary.Instance.Culture = new CultureInfo("en");
          //LocalizationManager.UICulture = new CultureInfo("en");
          break;
      }

      UpdateAllRibbonToolTips();
    }

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
          RibbonApplicationMenuItem menuItem = item as RibbonApplicationMenuItem;
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
              RibbonButton button = control as RibbonButton;
              ((RibbonToolTip)control.ToolTip).Title = ((RibbonCommand)button.Command).ToolTipTitle;
              ((RibbonToolTip)control.ToolTip).Description = ((RibbonCommand)button.Command).ToolTipDescription;
            }
            else if (control is RibbonComboBox)
            {
              RibbonComboBox comboBox = control as RibbonComboBox;
              ((RibbonToolTip)control.ToolTip).Title = ((RibbonCommand)comboBox.Command).ToolTipTitle;
              ((RibbonToolTip)control.ToolTip).Description = ((RibbonCommand)comboBox.Command).ToolTipDescription;
            }
          }
        }
      }
    }

    private void ResetColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ColorFactory.ResetColors();
      this.themeCounter = 0;
      SetColorScheme();
    }

    private void SaveLayoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.CheckFileExists = false;
      sfd.CheckPathExists = true;
      sfd.DefaultExt = ".xml";
      sfd.AddExtension = true;
      sfd.Filter = "XML Layout files (*.xml)|*.xml|All Files (*.*)|*.*";
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      sfd.Title = "Please select a data file ...";
      if (sfd.ShowDialog().Value)
      {
        dockingManager.SaveLayout(sfd.FileName);
      }
    }

    private void RestoreLayoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
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
        //this.videoPane.Items.Clear();
        //this.chartPane.Items.Clear();
        //this.datagridPane.Items.Clear();
        dockingManager.DeserializationCallback = (s, e_args) =>
        {
          if (e_args.Name == "_contentDummy")
          {
            e_args.Content = new DockableContent();
            e_args.Content.Title = "Dummy Content";
            e_args.Content.Content = new TextBlock() { Text = "Content Loaded On Demand!" };
          }
        };

        FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
        dockingManager.RestoreLayout(fs);
        //this.videoPane.Items.Add(videoWindow);
        //this.datagridPane.Items.Add(datagridWindow);
        //this.chartPane.Items.Add(this.chartWindow);
        this.modulePane.Items.Add(videoWindow);
        this.modulePane.Items.Add(this.datagridWindow);
        this.modulePane.Items.Add(this.chartWindow);

        fs.Close();
      }
    }

    private void LoadVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.ResetColorButton();
      this.videoWindow.SetVideoMode(VideoMode.File);
      this.videoWindow.LoadVideo(string.Empty);
    }

    public void ResetColorButton()
    {
      var largeSource = new Uri(@"/VianaNET;component/Images/SelectColor32.png", UriKind.Relative);
      ((RibbonCommand)this.selectColorRibbonButton.Command).LargeImageSource = new BitmapImage(largeSource);
      var smallSource = new Uri(@"/VianaNET;component/Images/SelectColor16.png", UriKind.Relative);
      ((RibbonCommand)this.selectColorRibbonButton.Command).SmallImageSource = new BitmapImage(smallSource);
      ((RibbonCommand)this.selectColorRibbonButton.Command).LabelTitle = Localization.Labels.ButtonSelectColorLabelTitle;
    }

    private void SelectColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      SelectColorWindow fullScreenVideoWindow = new SelectColorWindow();
      if (fullScreenVideoWindow.ShowDialog().GetValueOrDefault())
      {
        ((RibbonCommand)this.selectColorRibbonButton.Command).LabelTitle = Localization.Labels.ButtonSelectedColorLabelTitle;

        DrawingVisual drawingVisual = new DrawingVisual();
        DrawingContext drawingContext = drawingVisual.RenderOpen();
        drawingContext.DrawRoundedRectangle(
          Brushes.Transparent,
          null,
          new Rect(0, 0, 32, 32),
          5,
          5);
        drawingContext.DrawRoundedRectangle(
          new SolidColorBrush(Video.Instance.ImageProcessing.TargetColor),
          new Pen(Brushes.White, 2f),
          new Rect(2, 2, 28, 28),
          5,
          5);

        drawingContext.Close();
        RenderTargetBitmap bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(drawingVisual);
        ((RibbonCommand)this.selectColorRibbonButton.Command).LargeImageSource = bmp;
        Video.Instance.ImageProcessing.IsTargetColorSet = true;
        //this.videoWindow.BlobsControl.Visibility = Visibility.Visible;
        //Video.Instance.UpdateNativeBitmap();
      }
    }

    private void CreateImageSourceForNumberOfObjects()
    {
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRoundedRectangle(
        Brushes.Transparent,
        null,
        new Rect(0, 0, 32, 32),
        5,
        5);
      //drawingContext.DrawRoundedRectangle((Brush)this.Resources["DefaultOfficeBackgroundBrush"],
      //  new Pen(Brushes.White, 2f),
      //  new Rect(2, 2, 28, 28),
      //  5,
      //  5);
      FormattedText text = new FormattedText(
        Calibration.Instance.NumberOfTrackedObjects.ToString("N0"),
        LocalizeDictionary.Instance.Culture,
        FlowDirection.LeftToRight,
        new Typeface("Verdana"),
        24d,
        Brushes.Black);

      drawingContext.DrawText(text, new Point(8, 1));

      drawingContext.Close();
      RenderTargetBitmap bmp = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      ((RibbonCommand)this.ButtonSelectNumberOfObjects.Command).LargeImageSource = bmp;
      if (Calibration.Instance.NumberOfTrackedObjects > 1)
      {
        ((RibbonCommand)this.ButtonSelectNumberOfObjects.Command).LabelTitle =
          Localization.Labels.ButtonSelectNumberOfObjectsLabelTitle2;
      }
      else
      {
        ((RibbonCommand)this.ButtonSelectNumberOfObjects.Command).LabelTitle =
         Localization.Labels.ButtonSelectNumberOfObjectsLabelTitle;
      }
    }

    private void CalibrateVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      CalibrateVideoWindow calibrateWindow = new CalibrateVideoWindow();
      if (calibrateWindow.ShowDialog().GetValueOrDefault())
      {
        this.videoWindow.UpdateCalibration();
      }

      this.Refresh();
    }

    private void Refresh()
    {
      // Update data grid
      VideoData.Instance.RefreshDistanceVelocityAcceleration();

      // Update BlobsControl Dataview if visible
      if (Video.Instance.ImageProcessing.IsTargetColorSet)
      {
        this.videoWindow.BlobsControl.UpdateDataPoints();
      }
      //this.datagridWindow.Refresh();
    }

    private void ManualDataAquisitionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      VideoData.Instance.Reset();

      ManualDataAquisitionWindow manualAquisitionWindow = new ManualDataAquisitionWindow();
      manualAquisitionWindow.ShowDialog();

      this.Refresh();

      // Switch to datagrid window
      ShowWindow(this.datagridWindow, DockableContentState.Docked);
    }

    private void ButtonOtherOptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.RibbonTabVideo.Visibility = Visibility.Visible;
      this.RibbonTabVideo.IsSelected = true;
    }

    private void AutomaticDataAquisitionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.RunAutomaticDataAquisition();
    }

    private void ClipVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ClipVideoWindow clipWindow = new ClipVideoWindow();
      clipWindow.ShowDialog();
      this.videoWindow.UpdateClippingRegion();
    }

    private void ButtonInterpolationProperties_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Interpolation.ShowInterpolationOptionsDialog();
    }

    private void ButtonIsInterpolatingDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Interpolation.Instance.IsInterpolatingData = this.ButtonIsInterpolatingData.IsChecked.GetValueOrDefault();
    }


    #endregion //Executed

    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS

    private void ShowWindow(DockableContent contentToShow, DockableContentState desideredState)
    {
      if (desideredState == DockableContentState.AutoHide ||
          desideredState == DockableContentState.FloatingWindow)
      {
        dockingManager.Show(contentToShow, desideredState);
      }
      else
        dockingManager.Show(contentToShow);
    }

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
              ComponentResourceKey resource = identifier as ComponentResourceKey;
              if (resource.ResourceId.ToString() == "RibbonBackgroundBrush")
              {
                SolidColorBrush ribbonBackgroundBrush =
                  ((SolidColorBrush)dictionary[identifier]).Clone();
                this.Background = ribbonBackgroundBrush;
                this.dockingManager.Background = ribbonBackgroundBrush;
              }

              if (resource.ResourceId.ToString() == "GroupHostBorderBrush")
              {
                SolidColorBrush ribbonBorderBackgroundBrush =
                  ((SolidColorBrush)dictionary[identifier]).Clone();
                ColorFactory.ChangeColors(ribbonBorderBackgroundBrush.Color);
              }

              if (resource.ResourceId.ToString() == "GroupHostBackgroundBrush")
              {
                LinearGradientBrush groupHostBackgroundBrush =
                  ((LinearGradientBrush)dictionary[identifier]).Clone();
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

    #endregion //PRIVATEMETHODS

    private void modulePane_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      if (e.TargetObject is DockableContent)
      {
        ShowRibbonTabs((DockableContent)e.TargetObject);
      }
    }

    private void ShowRibbonTabs(DockableContent window)
    {
      if (window is VideoWindow)
      {
        //this.RibbonTabVideo.IsEnabled = true;
        this.RibbonTabAnalysis.IsSelected = true;
        //this.RibbonTabDatagrid.IsEnabled = false;
        //this.RibbonTabChart.IsEnabled = false;
      }

      if (window is DataGridWindow)
      {
        //this.RibbonTabVideo.IsEnabled = false;
        //this.RibbonTabDatagrid.IsEnabled = true;
        this.RibbonTabDatagrid.IsSelected = true;
        //this.RibbonTabChart.IsEnabled = false;
      }

      if (window is ChartWindow)
      {
        //this.RibbonTabVideo.IsEnabled = false;
        //this.RibbonTabDatagrid.IsEnabled = false;
        //this.RibbonTabChart.IsEnabled = true;
        this.RibbonTabChart.IsSelected = true;
      }
    }

    private void ButtonExportDataToTxt_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      SaveFileDialog sfd = new SaveFileDialog();

      // Default file extension
      sfd.DefaultExt = "txt";

      // Available file extensions
      sfd.Filter = Localization.Labels.TxtFilter;

      // Adds a extension if the user does not
      sfd.AddExtension = true;

      // Restores the selected directory, next time
      sfd.RestoreDirectory = true;

      // Dialog title
      sfd.Title = Localization.Labels.ExportWhereToSaveFile;

      // Startup directory
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToTxt(VideoData.Instance.Samples, sfd.FileName);
      }
    }

    private void ButtonExportDataToCsv_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      SaveFileDialog sfd = new SaveFileDialog();

      // Default file extension
      sfd.DefaultExt = "csv";

      // Available file extensions
      sfd.Filter = Localization.Labels.CsvFilter;

      // Adds a extension if the user does not
      sfd.AddExtension = true;

      // Restores the selected directory, next time
      sfd.RestoreDirectory = true;

      // Dialog title
      sfd.Title = Localization.Labels.ExportWhereToSaveFile;

      // Startup directory
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToCsv(VideoData.Instance.Samples, sfd.FileName);
      }
    }

    private void ButtonExportDataToXml_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Create new SaveFileDialog object
      SaveFileDialog sfd = new SaveFileDialog();

      // Default file extension
      sfd.DefaultExt = "xml";

      // Available file extensions
      sfd.Filter = Localization.Labels.XmlFilter;

      // Adds a extension if the user does not
      sfd.AddExtension = true;

      // Restores the selected directory, next time
      sfd.RestoreDirectory = true;

      // Dialog title
      sfd.Title = Localization.Labels.ExportWhereToSaveFile;

      // Startup directory
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      // Show the dialog and process the result
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        ExportData.ToXml(VideoData.Instance.Samples, sfd.FileName);
      }
    }

    private void ButtonExportDataToXls_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportData.ToXls(VideoData.Instance.Samples);
    }

    private void ButtonRecordVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = DShowUtils.GetVideoInputDevices().Count > 0;
    }

    private void ButtonRecordVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      bool wasCapturing = false;
      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        wasCapturing = true;
        this.videoWindow.SetVideoMode(VideoMode.None);
      }

      SaveVideoDialog saveVideoDialog = new SaveVideoDialog();
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

    private void ButtonCaptureVideoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = DShowUtils.GetVideoInputDevices().Count > 0;
    }

    private void ButtonCaptureVideoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.SetVideoMode(VideoMode.Capture);
    }

    private void ButtonExportChartToClipboard_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportChart.ToClipboard(this.chartWindow.DataChart);
    }

    private void ButtonExportChartToFile_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportChart.ToFile(this.chartWindow.DataChart);
    }

    private void ButtonExportChartToWord_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ExportChart.ToWord(this.chartWindow.DataChart);
    }

    private void ButtonChooseAnalysisCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void ButtonChooseAnalysisCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.RibbonTabAnalysis.IsSelected = true;
    }

    private void ChartDisplayOptionsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void ChartDisplayOptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.chartWindow.PropertiesExpander.IsExpanded = true;
    }

    private void ButtonChooseAutomaticAnalysisCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void ButtonChooseAutomaticAnalysisCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.RibbonTabAnalysis.IsSelected = true;
    }

    private void AutomaticDataAquisitionStopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Video.Instance.VideoElement.HasVideo;
    }

    private void AutomaticDataAquisitionStopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.videoWindow.StopAutomaticDataAquisition();
    }

    private void ButtonDeleteDataCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 0;
    }

    private void ButtonDeleteDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      VideoData.Instance.Reset();
      this.Refresh();
    }

    private void ButtonCalculateVelocityCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 1;
    }

    private void ButtonIsInterpolatingDataCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = VideoData.Instance.Count > 1;
    }

    private void ButtonCalculateVelocityCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      VideoData.Instance.RefreshDistanceVelocityAcceleration();
    }

    private void ButtonVideoCaptureDevicePropertiesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (Video.Instance.VideoCapturerElement.CurrentState == VideoBase.PlayState.Running);
    }

    private void ButtonVideoCaptureDevicePropertiesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Video.Instance.VideoCapturerElement.ShowPropertyPageOfVideoDevice();
    }

    private void RecentItemsList_MostRecentFileSelected(object sender, MostRecentFileSelectedEventArgs e)
    {
      RibbonHighlightingList list = e.Source as RibbonHighlightingList;
      foreach (RibbonHighlightingListItem item in list.Items)
      {
        if (item.Content.ToString() == e.SelectedItem.ToString())
        {
          ToolTip tooltip = item.ToolTip as ToolTip;
          Video.Instance.VideoPlayerElement.LoadMovie(tooltip.Content.ToString());
          break;
        }
      }
    }

    private void RibbonWindow_Closing(object sender, CancelEventArgs e)
    {
      Video.Instance.Cleanup();
    }

    private void ButtonSelectNumberOfObjectsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      // Increase number of objects, shrink to maximal 3.
      if (Calibration.Instance.NumberOfTrackedObjects == 3)
      {
        Calibration.Instance.NumberOfTrackedObjects = 1;
      }
      else
      {
        Calibration.Instance.NumberOfTrackedObjects++;
      }

      // Clear all data to correctly recreate data arrays.
      VideoData.Instance.Reset();

      // Update button image source
      this.CreateImageSourceForNumberOfObjects();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER

  }
}