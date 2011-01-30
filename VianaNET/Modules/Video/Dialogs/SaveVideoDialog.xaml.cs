namespace VianaNET
{
  using System.Windows;
  using System.Reflection;
  using System.Windows.Media;
  using DirectShowLib;
  using System;
  using System.IO;
  using System.Windows.Interop;
  using System.Windows.Threading;

  public partial class SaveVideoDialog : Window
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

    // An instance of the PreviewController class (where all the real work is done)
    private LiveVideoController liveVideoController;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    // The auto incremented number to tack on the end of the file to make a unique filename
    private int fileIndex = -1;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the SaveVideoDialog class.
    /// </summary>
    public SaveVideoDialog()
    {
      InitializeComponent();
      InitializeDeviceCombo();
      folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      folderBrowserDialog.Description = Localization.Labels.VideoSaveFolderBrowserDescriptionTitle;
      folderBrowserDialog.ShowNewFolderButton = true;
      folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
      this.FolderTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      this.FileNameTextBox.Text = "Video";
      liveVideoController = new LiveVideoController();
    }

    private void InitializeDeviceCombo()
    {
      // Load the listbox with video compressor device names
      this.CompressorComboBox.ItemsSource = DShowUtils.GetVideoCompressors();
      this.CompressorComboBox.SelectedIndex = 0;
      for (int i = 0; i < this.CompressorComboBox.Items.Count; i++)
      {
        if (this.CompressorComboBox.Items[i].ToString().Contains("ffdshow"))
        {
          this.CompressorComboBox.SelectedIndex = i;
          break;
        }
      }
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

    public string LastRecordedVideoFile { get; set; }

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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.liveVideoController.SelectDevice(
        Video.Instance.VideoCapturerElement.VideoCaptureDevice,
        this.VideoHost);

      this.liveVideoController.VideoCompressorName =
        this.CompressorComboBox.SelectedValue.ToString();
    }

    private void Record_Click(object sender, RoutedEventArgs e)
    {
      // If we aren't already capturing
      if (!this.liveVideoController.Capturing)
      {
        // If we have a device
        if (this.liveVideoController.Selected)
        {
          // Generate a name and send it to the m_Previewer
          MakeNewCaptureFile();

          SwitchControlsEnabled(false);

          // Start the capture graph
          try
          {
            this.liveVideoController.StartCapture();
          }
          catch (Exception ex)
          {
            ErrorLogger.ProcessException(ex, true);
            SwitchControlsEnabled(true);
          }
        }
      }
    }

    private void SwitchControlsEnabled(bool isEnabled)
    {
      this.StopButton.IsEnabled = !isEnabled;
      this.RecordButton.IsEnabled = isEnabled;
      this.FileNameTextBox.IsEnabled = isEnabled;
      this.FolderTextBox.IsEnabled = isEnabled;
      this.FolderButton.IsEnabled = isEnabled;
      this.CompressorComboBox.IsEnabled = isEnabled;
      this.StatusBarReadyLabel.Content = isEnabled ? Localization.Labels.StatusBarReady : Localization.Labels.IsRecordingVideo;
      this.RunAnalysisButton.IsEnabled = isEnabled;

      if (isEnabled)
      {
        this.StatusBarProgressBar.IsIndeterminate = false;
      }
      else
      {
        this.StatusBarProgressBar.IsIndeterminate = true;
      }
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
      // Only stop if we are started
      if (this.liveVideoController.Capturing)
      {
        try
        {
          this.liveVideoController.StopCapture();
          SwitchControlsEnabled(true);

          // StatusBarReadyLabel
          string s = string.Format("Captured to {0}", this.liveVideoController.FileName);
          this.LastRecordedVideoFile = this.liveVideoController.FileName;
          this.StatusBarReadyLabel.Content = s;
        }
        catch (Exception ex)
        {
          ErrorLogger.ProcessException(ex, true);
          SwitchControlsEnabled(true);
        }
      }
    }

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

    private void MakeNewCaptureFile()
    {
      string sFileName;

      // Find an unused file name
      do
      {
        sFileName = string.Format(
                     @"{0}\{1}{2}.avi",
                     this.FolderTextBox.Text,
                     this.FileNameTextBox.Text,
                     ++fileIndex);
      } while (File.Exists(sFileName));

      try
      {
        // Tell the previewer what name to use
        this.liveVideoController.SetNextFilename(sFileName);
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.liveVideoController != null)
      {
        this.liveVideoController.Dispose();
        this.liveVideoController = null;
      }
    }

    private void FolderButton_Click(object sender, RoutedEventArgs e)
    {
      // Show the folder browser
      if (this.folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        this.FolderTextBox.Text = this.folderBrowserDialog.SelectedPath;
      }
    }

    private void CompressorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (this.liveVideoController != null)
      {
        this.liveVideoController.VideoCompressorName = this.CompressorComboBox.SelectedValue.ToString();
      }
    }

    #endregion //PRIVATEMETHODS

    private void RunAnalysisButton_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void CompressionOptionsButton_Click(object sender, RoutedEventArgs e)
    {
      this.liveVideoController.ShowVideoCompressorOptionsDialog();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}