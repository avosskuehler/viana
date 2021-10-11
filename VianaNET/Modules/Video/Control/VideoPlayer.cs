﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlayer.cs" company="Freie Universität Berlin">
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
//   The video player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using System.Windows.Threading;

  using DirectShowLib;
  using OpenCvSharp;
  using OpenCvSharp.WpfExtensions;
  using VianaNET.Logging;
  using VianaNET.MainWindow;


  using File = System.IO.File;
  using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

  /// <summary>
  ///   The video player.
  /// </summary>
  public class VideoPlayer : VideoBase
  {
    /// <summary>
    ///   The volume full.
    /// </summary>
    private const int VolumeFull = 0;

    /// <summary>
    ///   The volume silence.
    /// </summary>
    private const int VolumeSilence = -10000;

    public VideoPlayer()
    {
    }

    ~VideoPlayer()
    {
    }

    /// <summary>
    ///   The media duration in ms property.
    /// </summary>
    public static readonly DependencyProperty MediaDurationInMSProperty =
      DependencyProperty.Register(
        "MediaDurationInMS", typeof(double), typeof(VideoPlayer), new UIPropertyMetadata(default(double)));

    /// <summary>
    ///   The file complete.
    /// </summary>
    public event EventHandler FileComplete;

    /// <summary>
    ///   The step complete.
    /// </summary>
    public event EventHandler StepComplete;

    /// <summary>
    /// Gets or sets the filename of the video file
    /// </summary>
    public string VideoFilename { get; set; }

    /// <summary>
    ///   Gets or sets the media duration in ms.
    /// </summary>
    public double MediaDurationInMS
    {
      get => (double)this.GetValue(MediaDurationInMSProperty);

      set => this.SetValue(MediaDurationInMSProperty, value);
    }

    /// <summary>
    ///   Gets or sets the media position in milli seconds.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public override double MediaPositionInMS
    {
      get
      {
        var pos = this.OpenCVObject.Get(VideoCaptureProperties.PosMsec);
        return pos;
      }

      set
      {
        this.OpenCVObject.Set(VideoCaptureProperties.PosMsec, value);
        this.GrabCurrentFrame();
        this.UpdateFrameIndex();
      }
    }

    /// <summary>
    ///   The dispose.
    /// </summary>
    public override void Dispose()
    {
      // Release DirectShow interfaces
      base.Dispose();

      // Clear file name to allow selection of new file with open dialog
      this.VideoFilename = string.Empty;
    }

    // this.openFileDialog1.Filter = @"Video Files (*.avi; *.qt; *.mov; *.mpg; *.mpeg; *.m1v)|*.avi; *.qt; *.mov; *.mpg; *.mpeg; *.m1v|Audio files (*.wav; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd)|*.wav; *.mpa; *.mp2; *.mp3; *.au; *.aif; *.aiff; *.snd|MIDI Files (*.mid, *.midi, *.rmi)|*.mid; *.midi; *.rmi|Image Files (*.jpg, *.bmp, *.gif, *.tga)|*.jpg; *.bmp; *.gif; *.tga|All Files (*.*)|*.*";

    /// <summary>
    /// The load movie.
    /// </summary>
    /// <param name="fileName">
    /// The file name. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool LoadMovie(string fileName)
    {
      try
      {
        if (App.Project.ProjectPath == null)
        {
          App.Project.ProjectPath = string.Empty;
        }

        string fileWithPath = Path.Combine(App.Project.ProjectPath, fileName);
        if (!File.Exists(fileWithPath))
        {
          if (fileName != string.Empty)
          {
            string messageTitle = VianaNET.Localization.Labels.AskVideoNotFoundMessageTitle;
            messageTitle = messageTitle.Replace("%1", Path.GetFileName(fileWithPath));
            messageTitle = messageTitle.Replace("%2", Path.GetDirectoryName(fileWithPath));

            VianaDialog dlg = new VianaDialog(VianaNET.Localization.Labels.AskVideoNotFoundTitle, messageTitle, VianaNET.Localization.Labels.AskVideoNotFoundMessage, false);
            if (!dlg.ShowDialog().GetValueOrDefault(false))
            {
              return false;
            }
          }

          OpenFileDialog ofd = new OpenFileDialog();
          ofd.CheckFileExists = true;
          ofd.CheckPathExists = true;
          ofd.FilterIndex = 1;
          ofd.Filter = VianaNET.Localization.Labels.VideoFilesFilter;
          ofd.Title = VianaNET.Localization.Labels.LoadVideoFilesTitle;
          if (ofd.ShowDialog().Value)
          {
            fileName = ofd.FileName;
          }
          else
          {
            return false;
          }
        }

        this.VideoFilename = Path.GetFileName(fileName);
        App.Project.ProjectPath = Path.GetDirectoryName(fileName);
        fileWithPath = Path.Combine(App.Project.ProjectPath, fileName);

        // Reset status variables
        this.CurrentState = PlayState.Stopped;

        // Read out video properties
        using (MediaInfo.DotNetWrapper.MediaInfo info = new MediaInfo.DotNetWrapper.MediaInfo())
        {
          MediaInfo.DotNetWrapper.Enumerations.Status status = info.Open(fileWithPath);
          string komplett = info.Inform();
          string[] einzeln = komplett.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

          string[] names = new string[500];
          for (int i = 0; i < 500; i++)
          {
            names[i] = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, i, MediaInfo.DotNetWrapper.Enumerations.InfoKind.Name);
          }

          // CFR = constant frame rate, VFR = variable frame rate
          string framerateMode = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameRate_Mode", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text);
          if (framerateMode == "VFR")
          {
            ErrorLogger.ProcessException(new ArgumentOutOfRangeException("Bildrate", "Die Datei hat eine variable Bildrate, sie muss zunächst konvertiert werden"), true);
            return false;
          }

          // Framerate auslesen
          string frameratestring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameRate", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text);
          if (!float.TryParse(frameratestring, out float fpsfactor1000))
          {
            ErrorLogger.ProcessException(new ArgumentOutOfRangeException("Bildrate", "Konnte die Bildrate der Datei nicht auslesen."), true);
            return false;
          }
          float fps = fpsfactor1000 / 1000f;

          // Dauer auslesen
          string durationmeasure = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "Duration", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Measure).Trim();
          if (durationmeasure != "ms")
          {
            ErrorLogger.ProcessException(new ArgumentOutOfRangeException("Dauer", "Die Einheit der Videodauer ist nicht in Millisekunden angegeben."), true);
            return false;
          }

          string durationstring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "Duration", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
          if (!int.TryParse(durationstring, out int duration))
          {
            ErrorLogger.ProcessException(new ArgumentOutOfRangeException("Dauer", "Konnte die Dauer der Datei nicht auslesen."), true);
            return false;
          }

          // Anzahl der Frames auslesen
          string framestring = info.Get(MediaInfo.DotNetWrapper.Enumerations.StreamKind.Video, 0, "FrameCount", MediaInfo.DotNetWrapper.Enumerations.InfoKind.Text).Trim();
          if (!int.TryParse(framestring, out int frames))
          {
            ErrorLogger.ProcessException(new ArgumentOutOfRangeException("Frames", "Konnte die Gesamtanzahl der Frames der Datei nicht auslesen."), true);
            return false;
          }

          this.FrameTimeInMS = 1000d / fps;
          this.MediaDurationInMS = duration;
          this.FrameCount = frames;

        }

        App.Project.VideoData.FramerateFactor = 1;
        try
        {
          if (this.OpenCVObject.Open(fileWithPath))
          {
            Video.Instance.FPS = this.OpenCVObject.Fps;
            Video.Instance.FrameSize = new System.Windows.Size(this.OpenCVObject.FrameWidth, this.OpenCVObject.FrameHeight);
          }
          else
          {
            ErrorLogger.ProcessException(new ArgumentOutOfRangeException("Videodatei öffnen", "Konnte die Videodatei nicht öffnen."), true);
            return false;
          }
        }
        catch (Exception)
        {
          return false;
        }

        App.Project.VideoFile = this.VideoFilename;
        Video.Instance.HasVideo = true;
        App.Project.ProcessingData.InitializeImageFilters();
        this.Revert();
        this.OnVideoAvailable();
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
        this.Dispose();
      }

      return true;
    }


    /// <summary>
    ///   The revert.
    /// </summary>
    public override void Revert()
    {
      base.Revert();
      double zeroPosition = App.Project.VideoData.SelectionStart;

      if (zeroPosition < 0)
      {
        zeroPosition = 0;
      }

      // Seek to the beginning
      this.OpenCVObject.Set(VideoCaptureProperties.PosMsec, zeroPosition);
      this.GrabCurrentFrame();

      this.UpdateFrameIndex();
    }

    /// <summary>
    /// This method steps the video the given number of frames in the given direction
    /// </summary>
    /// <param name="forward">True, if we should go forward in the video stream. 
    /// False to go backwards. </param>
    /// <param name="count">The number of frames to move</param>
    public void StepFrames(bool forward, int count)
    {
      if (forward)
      {
        this.StepFrames(count);
      }
      else
      {
        this.StepFrames(-count);
      }

      // Throw event
      this.StepComplete?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// This method steps the video one frame in the given direction
    /// </summary>
    /// <param name="forward">True, if we should go forward in the video stream. 
    /// False to go backwards. </param>
    public void StepOneFrame(bool forward)
    {
      if (forward)
      {
        this.StepFrames(1);
      }
      else
      {
        this.StepFrames(-1);
      }

      // Throw event
      this.StepComplete?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Steps the given number of frames
    /// </summary>
    /// <param name="numberOfFramesToStep">The n frames to step. </param>
    private void StepFrames(int numberOfFramesToStep)
    {
      if (numberOfFramesToStep < 0)
      {
        numberOfFramesToStep--;
      }

      if (numberOfFramesToStep > 0)
      {
        numberOfFramesToStep--;
      }

      var currentFrame = this.OpenCVObject.Get(VideoCaptureProperties.PosFrames);
      this.OpenCVObject.Set(VideoCaptureProperties.PosFrames, currentFrame + numberOfFramesToStep);
      this.GrabCurrentFrame();

      this.UpdateFrameIndex();
    }

    public void RaiseFileComplete()
    {
      // Throw event
      this.FileComplete?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///   The update frame index.
    /// </summary>
    public void UpdateFrameIndex()
    {
      double index = this.MediaPositionInMS / this.FrameTimeInMS;
      this.MediaPositionFrameIndex = (int)Math.Round(index);
    }
  }
}