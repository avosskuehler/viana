// <copyright file="VLCWindow.xaml.cs" company="FU Berlin">
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2015 Dr. Adrian Voßkühler  
//   Licensed under GPL V3
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace VianaNET.Modules.Video.Dialogs
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls.Primitives;
  using System.Windows.Forms;

  using MediaInfoNET;

  using VianaNET.Application;
  using VianaNET.Resources;

  using Vlc.DotNet.Core;
  using Vlc.DotNet.Forms;

  /// <summary>
  ///   This window is used to convert a video, which is by default not readable for Viana (cause there is
  ///   no directshow codec installed on the system), into an default mpeg mp3 version to make it available
  ///   for the standard processing pipeline.
  ///   It uses the VLCPlayer transcoding options via VLCDotNet.
  /// </summary>
  public partial class VlcWindow : IDisposable
  {
    /// <summary>
    ///   The converted file
    /// </summary>
    private string convertedFile;

    /// <summary>
    ///   Indicates a value, whether this window is in converting mode.
    /// </summary>
    private bool isConverting;

    /// <summary>
    ///   Indicates a value, whether the vlc player is playing
    /// </summary>
    private bool isPlaying;

    /// <summary>
    ///   The video file to be converted
    /// </summary>
    private string videoFile;

    /// <summary>
    ///   Initializes a new instance of the <see cref="VlcWindow" /> class.
    /// </summary>
    public VlcWindow()
    {
      this.InitializeComponent();
    }

    /// <summary>
    ///   Gets or sets the video file to be converted.
    /// </summary>
    /// <value>The video file to be converted including complete path.</value>
    public string VideoFile
    {
      get
      {
        return this.videoFile;
      }

      set
      {
        this.videoFile = value;
        this.vlcConverterPlayer.Play(new Uri(this.videoFile));
        Thread.Sleep(200);
        this.vlcConverterPlayer.Pause();
      }
    }

    /// <summary>
    ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    ///   Especially disposes the vlcplayer to get access to the converted video file.
    /// </summary>
    public void Dispose()
    {
      this.vlcConverterPlayer.Dispose();
      GC.Collect();
    }

    /// <summary>
    ///   Handles the LengthChanged event of the VlcPlayer control.
    ///   Updates the maximum value of the timeslider.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="VlcMediaPlayerLengthChangedEventArgs" /> instance containing the event data.</param>
    private void VlcPlayerLengthChanged(object sender, VlcMediaPlayerLengthChangedEventArgs e)
    {
      this.Dispatcher.InvokeAsync(
        () => { this.TimelineSlider.Maximum = new TimeSpan((long)e.NewLength).Duration().TotalMilliseconds; });
    }

    /// <summary>
    ///   Handles the VlcLibDirectoryNeeded event of the VlcControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="VlcLibDirectoryNeededEventArgs" /> instance containing the event data.</param>
    private void VlcControlVlcLibDirectoryNeeded(object sender, VlcLibDirectoryNeededEventArgs e)
    {
      e.VlcLibDirectory = GetVlcLibDirectory();
    }

    /// <summary>
    ///   Gets the VLC library directory.
    /// </summary>
    /// <returns>DirectoryInfo.</returns>
    private static DirectoryInfo GetVlcLibDirectory()
    {
      var currentAssembly = Assembly.GetEntryAssembly();
      var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
      if (currentDirectory == null)
      {
        return null;
      }

      var returnInfo = new DirectoryInfo(Path.Combine(currentDirectory, @"VlcLibs"));

      if (!returnInfo.Exists)
      {
        var folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.Description = Labels.SelectVlcLibrariesDescription;
        folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
        folderBrowserDialog.ShowNewFolderButton = true;
        if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          returnInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
        }
      }

      return returnInfo;
    }

    /// <summary>
    ///   Handles the PositionChanged event of the VlcControl control.
    ///   Updates the converter progress bar and the sliders thumb.
    ///   Stops replay if selection end was reached.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="VlcMediaPlayerPositionChangedEventArgs" /> instance containing the event data.</param>
    private void VlcControlPositionChanged(object sender, VlcMediaPlayerPositionChangedEventArgs e)
    {
      this.Dispatcher.InvokeAsync(
        () =>
        {
          this.TimelineSlider.Value = e.NewPosition * this.vlcConverterPlayer.Length;
          if (e.NewPosition * this.vlcConverterPlayer.Length > this.TimelineSlider.SelectionEnd && !this.isConverting)
          {
            this.vlcConverterPlayer.Pause();
          }

          this.ConverterProgressbar.Value = e.NewPosition * 100;
        });
    }

    /// <summary>
    ///   Play button was clicked, so update the button to pause image and start replay.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnPlayClick(object sender, RoutedEventArgs e)
    {
      if (this.isPlaying)
      {
        this.BtnPlayImage.Source = Viana.GetImageSource("Start16.png");
        this.vlcConverterPlayer.Pause();
      }
      else
      {
        this.BtnPlayImage.Source = Viana.GetImageSource("Pause16.png");
        this.vlcConverterPlayer.Play();
      }

      this.isPlaying = !this.isPlaying;
    }

    /// <summary>
    ///   Stop was clicked, so stop the replay and rewind to start.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnStopClick(object sender, RoutedEventArgs e)
    {
      this.vlcConverterPlayer.Stop();
      this.TimelineSlider.Value = this.TimelineSlider.SelectionStart;
      this.isPlaying = false;
      this.BtnPlayImage.Source = Viana.GetImageSource("Start16.png");
    }

    /// <summary>
    ///   Sets left cutout time on the current timesliders caret position.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnSetCutoutLeftClick(object sender, RoutedEventArgs e)
    {
      this.TimelineSlider.SelectionStart = this.TimelineSlider.Value;
      this.TimelineSlider.UpdateSelectionTimes();
    }

    /// <summary>
    ///   Sets right cutout time on the current timesliders caret position.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnSetCutoutRightClick(object sender, RoutedEventArgs e)
    {
      this.TimelineSlider.SelectionEnd = this.TimelineSlider.Value;
      this.TimelineSlider.UpdateSelectionTimes();
    }

    /// <summary>
    ///   Handles the OnSelectionAndValueChanged event of the TimelineSlider control.
    ///   Updates the position of the video.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void TimelineSlider_OnSelectionAndValueChanged(object sender, EventArgs e)
    {
      this.vlcConverterPlayer.Position = (float)(this.TimelineSlider.Value / this.vlcConverterPlayer.Length);
    }

    /// <summary>
    ///   Timelines thumb drag completed.
    ///   Updates the position of the video.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DragCompletedEventArgs" /> instance containing the event data.</param>
    private void TimelineSliderDragCompleted(object sender, DragCompletedEventArgs e)
    {
      this.vlcConverterPlayer.Position = (float)(this.TimelineSlider.Value / this.vlcConverterPlayer.Length);
    }

    /// <summary>
    ///   Timelines thumb drag delta.
    ///   Updates the position of the video.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DragDeltaEventArgs" /> instance containing the event data.</param>
    private void TimelineSliderDragDelta(object sender, DragDeltaEventArgs e)
    {
      this.vlcConverterPlayer.Position = (float)(this.TimelineSlider.Value / this.vlcConverterPlayer.Length);
    }

    /// <summary>
    ///   Ok was clicked, so start transcoding using the vlc command line tools.
    ///   Saving the video to disk.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.isConverting = true;
      var path = Viana.Project.ProjectPath ?? Path.GetDirectoryName(this.videoFile);
      this.convertedFile = Path.Combine(path, Path.GetFileNameWithoutExtension(this.videoFile));
      this.convertedFile += "-conv.avi";
      var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
      var startTimeOption = "start-time=" + (this.TimelineSlider.SelectionStart / 1000f).ToString("N3", nfi);
      var stopTimeOption = "stop-time=" + (this.TimelineSlider.SelectionEnd / 1000f).ToString("N3", nfi);
      var transcodeOption =
        @"sout=#transcode{vcodec=mp4v,vb=8000,deinterlace,fps=25,acodec=mpga}:standard{access=file,mux=avi,dst="
        + this.convertedFile + "}";
      this.ProgressPanel.Visibility = Visibility.Visible;
      this.VideoPanel.Visibility = Visibility.Hidden;
      var opts = new[] { startTimeOption, stopTimeOption, transcodeOption };
      this.vlcConverterPlayer.Play(new Uri(this.videoFile), opts);
      this.vlcConverterPlayer.EndReached += this.VlcConverterPlayerEndReached;
      this.OK.IsEnabled = false;
      this.Cancel.IsEnabled = false;
    }

    /// <summary>
    ///   Cancel was clicked, so just close the dialog.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    ///   The Vlc player has reached the end of the video.
    ///   This callback is only called in transcode mode, so when this gets called, the conversion process was finished.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="VlcMediaPlayerEndReachedEventArgs" /> instance containing the event data.</param>
    private void VlcConverterPlayerEndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
    {
      this.FinishAndClose();
    }

    /// <summary>
    ///   Resets UI and closes the window.
    /// </summary>
    private void FinishAndClose()
    {
      Viana.Project.VideoFile = this.convertedFile;

      this.Dispatcher.Invoke(
        () =>
        {
          this.TimelineSlider.ResetSelection();
          this.TimelineSlider.UpdateSelectionTimes();

          this.OK.IsEnabled = true;
          this.Close();
        });
    }
  }
}