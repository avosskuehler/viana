// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveVideoDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The save video dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Dialogs
{
  using OpenCvSharp;
  using OpenCvSharp.WpfExtensions;
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Logging;
  using VianaNET.Modules.Video.Control;
  using WPFFolderBrowser;

  /// <summary>
  ///   The save video dialog.
  /// </summary>
  public partial class SaveVideoDialog : System.Windows.Window
  {
    /// <summary>
    ///   The folder browser dialog.
    /// </summary>
    private readonly WPFFolderBrowserDialog folderBrowserDialog;

    // The auto incremented number to tack on the end of the file to make a unique filename
    /// <summary>
    ///   The file index.
    /// </summary>
    private int fileIndex = -1;

    /// <summary>
    /// The OpenCV VideoCapture Device Control
    /// </summary>
    public VideoCapture OpenCVObject { get; private set; }

    /// <summary>
    /// Frame Available Background worker process
    /// </summary>
    protected readonly BackgroundWorker bkgWorker;


    /// <summary>
    /// Indicates whether the capturing is in progress
    /// </summary>
    private bool isCapturing;

    /// <summary>
    /// The <see cref="VideoWriter"/> instance to write the video to disk
    /// </summary>
    private VideoWriter videoWriter;

    /// <summary>
    ///   Initializes a new instance of the SaveVideoDialog class.
    /// </summary>
    public SaveVideoDialog()
    {
      this.InitializeComponent();
      this.folderBrowserDialog = new WPFFolderBrowserDialog();
      this.folderBrowserDialog.Title = VianaNET.Localization.Labels.VideoSaveFolderBrowserDescriptionTitle;
      this.folderBrowserDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      this.FolderTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      this.FileNameTextBox.Text = "Video";

      this.bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
      this.bkgWorker.DoWork += this.Worker_DoWork;
      this.MakeNewCaptureFile();

      this.OpenCVObject = new VideoCapture();

      this.CameraComboBox.ItemsSource = Video.Instance.VideoInputDevicesMSMF;
      if (Video.Instance.VideoInputDevicesMSMF.Count > 0)
      {
        this.CameraComboBox.SelectedIndex = 0;
      }
      else
      {
        InformationDialog.Show("Keine Kameras gefunden", "Viana kann keine angeschlossenen Kameras finden, daher kann auch keine Videoaufnahme gemacht werden.", false);
        this.Close();
      }
    }

    ~SaveVideoDialog()
    {
      if (this.OpenCVObject != null)
      {
        this.OpenCVObject.Dispose();
      }

      if (this.videoWriter != null)
      {
        this.videoWriter.Dispose();
      }
    }

    /// <summary>
    ///   Gets or sets the last recorded video file.
    /// </summary>
    public string LastRecordedVideoFile { get; set; }

    private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UiServices.SetBusyState();
      UiServices.WaitUntilReady();

      if (this.bkgWorker != null && this.bkgWorker.IsBusy)
      {
        this.bkgWorker.CancelAsync();
      }

      while (this.bkgWorker.IsBusy)
      {
        UiServices.WaitUntilReady();
      }

      var cameraIndex = ((CameraDevice)this.CameraComboBox.SelectedItem).Index;

      try
      {
        if (!this.OpenCVObject.Open(cameraIndex, OpenCvSharp.VideoCaptureAPIs.ANY))
        {
          return;
        }

        this.bkgWorker.RunWorkerAsync();
        //Video.Instance.VideoElement.SaveSizeInfo(this.OpenCVObject);
        //Video.Instance.FPS = this.OpenCVObject.Fps;
        //Video.Instance.FrameSize = new System.Windows.Size(this.OpenCVObject.FrameWidth, this.OpenCVObject.FrameHeight);
      }
      catch
      {
        ErrorLogger.WriteLine("Error in OpenCVObject.Open(), Could not initialize camera");
        return;
      }
    }

    private void CameraOptionsButton_Click(object sender, RoutedEventArgs e)
    {
      this.OpenCVObject.Set(VideoCaptureProperties.Settings, 1);
    }

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
      //if (this.videoCompressorFilter != null)
      //{
      //  DShowUtils.DisplayPropertyPage(IntPtr.Zero, this.videoCompressorFilter);
      //}
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
      //this.VideoCompressorName = this.CompressorComboBox.SelectedValue.ToString();
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
      if (this.folderBrowserDialog.ShowDialog().GetValueOrDefault(false))
      {
        this.FolderTextBox.Text = this.folderBrowserDialog.FileName;
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
        sFileName = string.Format(@"{0}\{1}{2}.mp4", this.FolderTextBox.Text, this.FileNameTextBox.Text, ++this.fileIndex);
      }
      while (File.Exists(sFileName));

      try
      {
        // Tell the previewer what name to use
        this.FileNameTextBox.Text = sFileName;
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
      }
    }

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
      if (!this.isCapturing)
      {
        var filename = Path.Combine(this.FolderTextBox.Text, this.FileNameTextBox.Text);
        
        // Using fixed 25 fps
        this.videoWriter = new VideoWriter(filename, FourCC.MP4V, 25, new OpenCvSharp.Size(this.OpenCVObject.FrameWidth, this.OpenCVObject.FrameHeight));
        if (this.videoWriter.IsOpened())
        {
          this.isCapturing = true;
          this.SwitchControlsEnabled(false);
        }
        else
        {
          InformationDialog.Show("Fehler beim Aufzeichnen", "Konnte Videodatei mit dem MJPEG Kompressor nicht aufzeichen", false);
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
      if (this.isCapturing)
      {
        this.isCapturing = false;
        this.LastRecordedVideoFile = this.videoWriter.FileName;
        // StatusBarReadyLabel
        string s = string.Format("Captured to {0}", this.LastRecordedVideoFile);
        this.StatusBarReadyLabel.Content = s;
        this.SwitchControlsEnabled(true);
        this.videoWriter.Release();
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
      this.CameraComboBox.IsEnabled = isEnabled;
      //this.CompressorComboBox.IsEnabled = isEnabled;
      this.StatusBarReadyLabel.Content = isEnabled ? VianaNET.Localization.Labels.StatusBarReady : VianaNET.Localization.Labels.IsRecordingVideo;
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

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = (BackgroundWorker)sender;
      var watch = new Stopwatch();
      watch.Start();

      // With 25 FPS all webcams will work
      double frametimeInMS = 40;
      long starttime = 0;

      while (!worker.CancellationPending)
      {
        using (Mat frameMat = this.OpenCVObject.RetrieveMat())
        {
          if (frameMat.Empty())
          {
            break;
          }

          // Must create and use WriteableBitmap in the same thread(UI Thread).
          this.Dispatcher.Invoke(() =>
          {
            this.VideoImage.Source = frameMat.ToWriteableBitmap();
          });

          if (this.isCapturing)
          {
            this.videoWriter.Write(frameMat);
          }

          // Ensure only a picture every frametimeInMS time interval
          while (watch.ElapsedMilliseconds - starttime < frametimeInMS)
          {
            Thread.Sleep(1);
          }

          starttime = watch.ElapsedMilliseconds;
        }

      }
    }
  }
}