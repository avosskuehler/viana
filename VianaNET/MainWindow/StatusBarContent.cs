﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarContent.cs" company="Freie Universität Berlin">
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
//   The status bar content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.MainWindow
{
  using System.Windows;

  using VianaNET.Data;

  /// <summary>
  ///   The status bar content.
  /// </summary>
  public class StatusBarContent : DependencyObject
  {


    /// <summary>
    ///   The messages label property.
    /// </summary>
    public static readonly DependencyProperty MessagesLabelProperty = DependencyProperty.Register(
      "MessagesLabel", typeof(string), typeof(StatusBarContent), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    ///   The progress bar value property.
    /// </summary>
    public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register(
      "ProgressBarValue", typeof(double), typeof(StatusBarContent), new FrameworkPropertyMetadata(default(double)));

    /// <summary>
    ///   The status label property.
    /// </summary>
    public static readonly DependencyProperty StatusLabelProperty = DependencyProperty.Register(
      "StatusLabel", typeof(string), typeof(StatusBarContent), new FrameworkPropertyMetadata("Ready"));

    /// <summary>
    ///   The video filename property for the status bar
    /// </summary>
    public static readonly DependencyProperty VideoFilenameProperty = DependencyProperty.Register(
      "VideoFilename", typeof(string), typeof(StatusBarContent), new FrameworkPropertyMetadata("No video input loaded."));

    /// <summary>
    ///   Holds the instance of singleton
    /// </summary>
    private static StatusBarContent instance;





    /// <summary>
    ///   Prevents a default instance of the <see cref="StatusBarContent" /> class from being created.
    /// </summary>
    private StatusBarContent()
    {
      // LocExtension ready = new LocExtension("VianaNET:Labels:StatusBarReady");
      // ready.SetBinding(this, StatusBarContent.StatusLabelProperty);
    }





    /// <summary>
    ///   Gets the <see cref="CalibrationData" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static StatusBarContent Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new StatusBarContent();
        }

        // return the existing/new instance
        return instance;
      }
    }

    /// <summary>
    ///   Gets or sets the messages label.
    /// </summary>
    public string MessagesLabel
    {
      get => (string)this.GetValue(MessagesLabelProperty);

      set => this.SetValue(MessagesLabelProperty, value);
    }

    /// <summary>
    ///   Gets or sets the progress bar value.
    /// </summary>
    public double ProgressBarValue
    {
      get => (double)this.GetValue(ProgressBarValueProperty);

      set => this.SetValue(ProgressBarValueProperty, value);
    }

    /// <summary>
    ///   Gets or sets the status label.
    /// </summary>
    public string StatusLabel
    {
      get => (string)this.GetValue(StatusLabelProperty);

      set => this.SetValue(StatusLabelProperty, value);
    }

    /// <summary>
    ///   Gets or sets the video filename.
    /// </summary>
    public string VideoFilename
    {
      get => (string)this.GetValue(VideoFilenameProperty);

      set => this.SetValue(VideoFilenameProperty, value);
    }


  }
}