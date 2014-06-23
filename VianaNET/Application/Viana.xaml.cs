﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viana.xaml.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
//   This is the main entry point for the application and
//   represents the base for VianaNET.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Application
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Threading;

  using VianaNET.Logging;
  using VianaNET.MainWindow;

  using WPFLocalizeExtension.Engine;

  /// <summary>
  ///   This is the main entry point for the application and
  ///   represents the base for VianaNET.
  /// </summary>
  public partial class Viana
  {
    #region Static Fields

    /// <summary>
    ///   The static member, that holds the project.
    /// </summary>
    private static Project project;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the static member, that holds the project.
    /// </summary>
    public static Project Project
    {
      get
      {
        return project ?? (project = new Project());
      }

      set
      {
        project = value;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Is a replacement for the winform Application.DoEvents function
    ///   for WPF. Process the current dispatcher queue.
    /// </summary>
    public static void DoEvents()
    {
      if (Current != null)
      {
        Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      }
    }

    /// <summary>
    /// This static mehtod returns an <see cref="Image"/>
    ///   for the given filename string, if the image is in the Images
    ///   subfolder of the solution.
    /// </summary>
    /// <param name="imageName">
    /// A <see cref="string"/> with the images file name
    /// </param>
    /// <returns>
    /// The <see cref="Image"/> that can be used as a source for
    ///   an icon property.
    /// </returns>
    public static Image GetImage(string imageName)
    {
      var terminMenuentryIcon = new Image();
      var terminMenuentryIconImage = new BitmapImage();
      terminMenuentryIconImage.BeginInit();
      terminMenuentryIconImage.UriSource = new Uri("pack://application:,,,/Images/" + imageName);
      terminMenuentryIconImage.EndInit();
      terminMenuentryIcon.Source = terminMenuentryIconImage;
      return terminMenuentryIcon;
    }

    /// <summary>
    /// This static mehtod returns an <see cref="ImageSource"/>
    ///   for the given filename string, if the image is in the Images
    ///   subfolder of the solution.
    /// </summary>
    /// <param name="imageName">
    /// A <see cref="string"/> with the images file name
    /// </param>
    /// <returns>
    /// The <see cref="ImageSource"/> that can be used as a source for
    ///   an imagesource property.
    /// </returns>
    public static ImageSource GetImageSource(string imageName)
    {
      var terminMenuentryIconImage = new BitmapImage();
      terminMenuentryIconImage.BeginInit();
      terminMenuentryIconImage.UriSource = new Uri("pack://application:,,,/Images/" + imageName);
      terminMenuentryIconImage.EndInit();
      return terminMenuentryIconImage;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The <see cref="System.Windows.Application.DispatcherUnhandledException"/> event handler.
    ///   Displays a message for each otherwise unhandled exception.
    /// </summary>
    /// <param name="sender">
    /// Source of the event
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.StartupEventArgs"/> with the event data.
    /// </param>
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      // Raise message box and log to file.
      ErrorLogger.ProcessException(e.Exception, true);
    }

    /// <summary>
    /// The <see cref="Application.Startup"/> event handler.
    ///   Defaults localization to german locale and registers
    ///   to unhandled exception event.
    /// </summary>
    /// <param name="sender">
    /// Source of the event
    /// </param>
    /// <param name="args">
    /// The <see cref="StartupEventArgs"/> with the event data.
    /// </param>
    private void VianaNetApplicationStartup(object sender, StartupEventArgs args)
    {
      this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("de");

      this.MainWindow = new MainWindow();

      // Support for command line arguments.
      if (args.Args.Length == 1)
      {
        if (File.Exists(args.Args[0]))
        {
          string fileName = args.Args[0];
          ((MainWindow)this.MainWindow).OpenGivenProject(fileName);
        }
      }
    }

    #endregion
  }
}