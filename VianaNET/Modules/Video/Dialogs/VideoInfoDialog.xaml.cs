// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The length dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.Video.Dialogs
{
  using OpenCvSharp;
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Windows;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   Thid dialog displays video information
  /// </summary>
  public partial class VideoInfoDialog
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoInfoDialog"/> class. 
    /// </summary>
    /// <param name="videofile"> The videofile to be analyzed. </param>
    public VideoInfoDialog()
    {
      this.InitializeComponent();
      UiServices.SetBusyState();

      this.DataContext = this;
      switch (Video.Instance.VideoMode)
      {
        case CustomStyles.Types.VideoMode.File:
          this.ParseVideoFile();
          break;
        case CustomStyles.Types.VideoMode.Capture:
          this.ParseLiveCamera();
          break;
        case CustomStyles.Types.VideoMode.None:
        default:
          this.ParseVideoFile();
          break;
      }
    }

    /// <summary>
    /// Gets the filename without path of the video file
    /// </summary>
    public string Filename { get; private set; }

    /// <summary>
    /// Gets the duration of the video file in format hh:mm:ss.ms
    /// </summary>
    public string DurationString { get; private set; }

    /// <summary>
    /// Gets the duration of the video file as a long
    /// </summary>
    public long Duration { get; private set; }

    /// <summary>
    /// Gets or sets the framerate of the video file in fps
    /// that can be modified
    /// </summary>
    public double FrameRate { get; set; }

    /// <summary>
    /// Gets or sets the framerate of the video file in fps
    /// that was automatical parsed
    /// </summary>
    public double DefaultFrameRate { get; set; }

    /// <summary>
    /// Gets or sets the number of frames of the video file
    /// </summary>
    public int FrameCount { get; private set; }

    /// <summary>
    /// Gets the dimensions width x height of the video file
    /// </summary>
    public string FrameSize { get; private set; }

    /// <summary>
    /// Gets the video codec of the video file
    /// </summary>
    public string Codec { get; private set; }

    /// <summary>
    /// Gets the container format of the video file
    /// </summary>
    public string Container { get; private set; }

    /// <summary>
    /// Gets the bitrate in kbps of the video file
    /// </summary>
    public string Bitrate { get; private set; }

    /// <summary>
    /// Parses the video file for its properties.
    /// </summary>
    private void ParseVideoFile()
    {
      if (!File.Exists(App.Project.VideoFile))
      {
        this.Filename = string.Empty;
        this.DurationString = string.Empty;
        this.Duration = 0;
        this.FrameRate = 25;
        this.DefaultFrameRate = 25;
        this.FrameCount = 0;
        this.FrameSize = string.Empty;
        this.Codec = string.Empty;
        this.Bitrate = 0 + " kbps";
        return;
      }

      this.Filename = App.Project.VideoFile;

      // Read out video properties
      using (MediaInfo.DotNetWrapper.MediaInfo info = new MediaInfo.DotNetWrapper.MediaInfo())
      {
        MediaInfo.DotNetWrapper.Enumerations.Status status = info.Open(App.Project.VideoFile);

        string frameratestring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameRate", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text);
        if (float.TryParse(frameratestring, out float fpsfactor1000))
        {
          float fps = fpsfactor1000 / 1000f;
          this.DefaultFrameRate = fps;

          double currentFrameRate = fps / App.Project.VideoData.FramerateFactor;
          if (currentFrameRate != fps)
          {
            this.FrameRate = currentFrameRate;
          }
          else
          {
            this.FrameRate = fps;
          }
        }

        // Dauer auslesen
        string durationmeasure = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "Duration", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Measure).Trim();
        if (durationmeasure == "ms")
        {
          string durationstring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "Duration", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
          if (int.TryParse(durationstring, out int duration))
          {
            this.Duration = duration;
            this.DurationString = string.Format("{0} ms", duration);
          }
        }

        string bitratestring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "BitRate_String", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text);

        // CFR = constant frame rate, VFR = variable frame rate
        string framerateMode = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameRate_Mode", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text);
        if (framerateMode == "VFR")
        {
          this.Bitrate = string.Format("{0} mit variabler Bildrate", bitratestring);
        }
        else
        {
          this.Bitrate = string.Format("{0} mit fester Bildrate", bitratestring);
        }

        // Anzahl der Frames auslesen
        string framestring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameCount", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
        if (int.TryParse(framestring, out int frames))
        {
          this.FrameCount = frames;
        }

        // Anzahl der Frames auslesen
        string containerformat = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.General, 0, "Format", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
        string containerformatprofile = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.General, 0, "Format_Profile", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
        string videoformatinfo = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "Format/Info", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
        string videoformatprofile = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "Format_Profile", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
        this.Container = string.Format("{0} ({1})", containerformat, containerformatprofile);
        this.Codec = string.Format("{0} ({1})", videoformatinfo, videoformatprofile);

        // Read out video properties
        var capturer = Video.Instance.VideoElement;
        this.FrameSize = string.Format("{0} x {1}", capturer.NaturalVideoWidth, capturer.NaturalVideoHeight);
      }


    }

    private void ParseLiveCamera()
    {
      // Read out video properties
      var capturer = Video.Instance.VideoCapturerElement;

      this.Filename = string.Empty;
      this.DurationString = string.Empty;
      this.Duration = 0;
      this.FrameRate = Video.Instance.FPS;
      this.DefaultFrameRate = Video.Instance.FPS;
      this.FrameCount = 0;
      this.FrameSize = string.Format("{0} x {1}", capturer.NaturalVideoWidth, capturer.NaturalVideoHeight);
      this.Codec = capturer.OpenCVObject.FourCC;
      this.Bitrate = Video.Instance.VideoElement.OpenCVObject.Get(OpenCvSharp.VideoCaptureProperties.BitRate).ToString();

      // Test capture because direct read out of framerate from opencv objects properties does not work
      double num_frames = 120;

      var watch = new Stopwatch();
      watch.Start();

      Mat mat = new Mat();
      for (int i = 0; i < num_frames; i++)
      {
        capturer.OpenCVObject.Read(mat);
      }

      var ms = watch.ElapsedMilliseconds;

      // Calculate frames per second
      this.DefaultFrameRate = (int)Math.Round(num_frames / ms * 1000, 0);
    }

    /// <summary>
    /// Handles the Click event of the Cancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the Click event of the OK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;

      // Update FPS, if needed
      if (this.DefaultFrameRate != this.FrameRate)
      {
        double factor = this.DefaultFrameRate / this.FrameRate;
        App.Project.VideoData.FramerateFactor = factor;
        Control.Video.Instance.VideoPlayerElement.MediaDurationInMS = this.Duration * factor;
        Control.Video.Instance.VideoElement.FrameTimeInMS = 1000d / this.FrameRate;
      }

      this.Close();
    }
  }
}