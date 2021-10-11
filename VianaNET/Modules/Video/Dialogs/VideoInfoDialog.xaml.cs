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
  using System.IO;
  using System.Windows;
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
    private void ParseVideoFile()
    {
      if (App.Project == null || App.Project.ProjectPath == null)
      {
        return;
      }

      string fileWithPath = Path.Combine(App.Project.ProjectPath, App.Project.VideoFile);

      if (!System.IO.File.Exists(fileWithPath))
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

      this.Filename = fileWithPath;

      // Read out video properties
      using (MediaInfo.DotNetWrapper.MediaInfo info = new MediaInfo.DotNetWrapper.MediaInfo())
      {
        MediaInfo.DotNetWrapper.Enumerations.Status status = info.Open(fileWithPath);
        string komplett = info.Inform();

        string frameratestring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameRate", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text);
        if (float.TryParse(frameratestring, out float fpsfactor1000))
        {
          float fps = fpsfactor1000 / 1000f;
          //this.DefaultFrameRate = fps;

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
      }


      //this.FrameCount = aviFile.FrameCount;
      //this.FrameSize = aviFile.Video[0].FrameSize;
      //this.Codec = aviFile.Video[0].Format;
      //this.Bitrate = aviFile.General.Bitrate.ToString(CultureInfo.InvariantCulture) + " kbps";
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
      this.Codec = string.Empty;
      this.Bitrate = Video.Instance.VideoElement.OpenCVObject.Get(OpenCvSharp.VideoCaptureProperties.BitRate).ToString();
      return;
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