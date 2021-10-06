// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveVideoDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The save video dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Dialogs
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Forms;


  using VianaNET.Logging;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The save video dialog.
  /// </summary>
  public partial class SaveVideoDialog : Window
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////

    // An instance of the PreviewController class (where all the real work is done)
    #region Fields

    /// <summary>
    ///   The folder browser dialog.
    /// </summary>
    private readonly FolderBrowserDialog folderBrowserDialog;

    // The auto incremented number to tack on the end of the file to make a unique filename
    /// <summary>
    ///   The file index.
    /// </summary>
    private int fileIndex = -1;

    /// <summary>
    ///   The live video controller.
    /// </summary>
    private LiveVideoController liveVideoController;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the SaveVideoDialog class.
    /// </summary>
    public SaveVideoDialog()
    {
      this.InitializeComponent();
      this.InitializeDeviceCombo();
      this.folderBrowserDialog = new FolderBrowserDialog();
      this.folderBrowserDialog.Description = VianaNET.Resources.Labels.VideoSaveFolderBrowserDescriptionTitle;
      this.folderBrowserDialog.ShowNewFolderButton = true;
      this.folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
      this.FolderTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      this.FileNameTextBox.Text = "Video";
      this.liveVideoController = new LiveVideoController();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    ///   Gets or sets the last recorded video file.
    /// </summary>
    public string LastRecordedVideoFile { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// The compression options button_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CompressionOptionsButton_Click(object sender, RoutedEventArgs e)
    {
      this.liveVideoController.ShowVideoCompressorOptionsDialog();
    }

    /// <summary>
    /// The compressor combo box_ selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CompressorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.liveVideoController != null)
      {
        this.liveVideoController.VideoCompressorName = this.CompressorComboBox.SelectedValue.ToString();
      }
    }

    /// <summary>
    /// The folder button_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void FolderButton_Click(object sender, RoutedEventArgs e)
    {
      // Show the folder browser
      if (this.folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        this.FolderTextBox.Text = this.folderBrowserDialog.SelectedPath;
      }
    }

    /// <summary>
    ///   The initialize device combo.
    /// </summary>
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

    /// <summary>
    ///   The make new capture file.
    /// </summary>
    private void MakeNewCaptureFile()
    {
      string sFileName;

      // Find an unused file name
      do
      {
        sFileName = string.Format(
          @"{0}\{1}{2}.avi", this.FolderTextBox.Text, this.FileNameTextBox.Text, ++this.fileIndex);
      }
      while (File.Exists(sFileName));

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

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The record_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Record_Click(object sender, RoutedEventArgs e)
    {
      // If we aren't already capturing
      if (!this.liveVideoController.Capturing)
      {
        // If we have a device
        if (this.liveVideoController.Selected)
        {
          // Generate a name and send it to the m_Previewer
          this.MakeNewCaptureFile();

          this.SwitchControlsEnabled(false);

          // Start the capture graph
          try
          {
            this.liveVideoController.StartCapture();
          }
          catch (Exception ex)
          {
            ErrorLogger.ProcessException(ex, true);
            this.SwitchControlsEnabled(true);
          }
        }
      }
    }

    /// <summary>
    /// The run analysis button_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void RunAnalysisButton_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// The stop_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Stop_Click(object sender, RoutedEventArgs e)
    {
      // Only stop if we are started
      if (this.liveVideoController.Capturing)
      {
        try
        {
          this.liveVideoController.StopCapture();
          this.SwitchControlsEnabled(true);

          // StatusBarReadyLabel
          string s = string.Format("Captured to {0}", this.liveVideoController.FileName);
          this.LastRecordedVideoFile = this.liveVideoController.FileName;
          this.StatusBarReadyLabel.Content = s;
        }
        catch (Exception ex)
        {
          ErrorLogger.ProcessException(ex, true);
          this.SwitchControlsEnabled(true);
        }
      }
    }

    /// <summary>
    /// The switch controls enabled.
    /// </summary>
    /// <param name="isEnabled">
    /// The is enabled. 
    /// </param>
    private void SwitchControlsEnabled(bool isEnabled)
    {
      this.StopButton.IsEnabled = !isEnabled;
      this.RecordButton.IsEnabled = isEnabled;
      this.FileNameTextBox.IsEnabled = isEnabled;
      this.FolderTextBox.IsEnabled = isEnabled;
      this.FolderButton.IsEnabled = isEnabled;
      this.CompressorComboBox.IsEnabled = isEnabled;
      this.StatusBarReadyLabel.Content = isEnabled ? VianaNET.Resources.Labels.StatusBarReady : VianaNET.Resources.Labels.IsRecordingVideo;
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

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The window_ closing.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Window_Closing(object sender, CancelEventArgs e)
    {
      if (this.liveVideoController != null)
      {
        this.liveVideoController.Dispose();
        this.liveVideoController = null;
      }
    }

    /// <summary>
    /// The window_ loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.liveVideoController.SelectDevice(Video.Instance.VideoCapturerElement.VideoCaptureDevice, this.VideoHost);

      this.liveVideoController.VideoCompressorName = this.CompressorComboBox.SelectedValue.ToString();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}