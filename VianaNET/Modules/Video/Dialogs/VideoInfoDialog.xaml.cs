// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The length dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.Video.Dialogs
{
  using System.Globalization;
  using System.Windows;

  using MediaInfoNET;

  using VianaNET.Application;
  using VianaNET.Logging;

  /// <summary>
  ///   Thid dialog displays video information
  /// </summary>
  public partial class VideoInfoDialog
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoInfoDialog"/> class. 
    /// </summary>
    /// <param name="videofile"> The videofile to be analyzed. </param>
    public VideoInfoDialog(string videofile)
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.ParseVideoFile(videofile);
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
    /// Gets the codec of the video file
    /// </summary>
    public string Codec { get; private set; }

    /// <summary>
    /// Gets the bitrate in kbps of the video file
    /// </summary>
    public string Bitrate { get; private set; }

    /// <summary>
    /// Parses the video file for its properties.
    /// </summary>
    /// <param name="videoFilename">The video filename.</param>
    private void ParseVideoFile(string videoFilename)
    {
      if (!System.IO.File.Exists(videoFilename))
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

      var aviFile = new MediaFile(videoFilename);
      this.Filename = aviFile.Name;
      this.DurationString = aviFile.General.DurationStringAccurate;
      this.Duration = aviFile.General.DurationMillis;
      var currentFrameRate = aviFile.Video[0].FrameRate / Viana.Project.VideoData.FramerateFactor;
      if (currentFrameRate != aviFile.Video[0].FrameRate)
      {
        this.FrameRate = currentFrameRate;
      }
      else
      {
        this.FrameRate = aviFile.Video[0].FrameRate;
      }

      this.DefaultFrameRate = aviFile.Video[0].FrameRate;
      this.FrameCount = aviFile.FrameCount;
      this.FrameSize = aviFile.Video[0].FrameSize;
      this.Codec = aviFile.Video[0].Format;
      this.Bitrate = aviFile.General.Bitrate.ToString(CultureInfo.InvariantCulture) + " kbps";
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
        var factor = this.DefaultFrameRate / this.FrameRate;
        Viana.Project.VideoData.FramerateFactor = factor;
        Control.Video.Instance.VideoPlayerElement.MediaDurationInMS = this.Duration * factor;
        Control.Video.Instance.VideoElement.FrameTimeInNanoSeconds = (long)(10000000d / this.FrameRate);
      }

      this.Close();
    }
  }
}