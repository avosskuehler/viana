﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentFiles.cs" company="Freie Universität Berlin">
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
//   Derived from <see cref="DependencyObject" /> and implements <see cref="INotifyPropertyChanged" />.
//   Class to handle a string collection with the last recently used files.
//   Its of a singleton class type, to always have one unique complete list,
//   if you call <code>RecentFiles.List</code>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Application
{
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Drawing;
  using System.IO;
  using System.Runtime.Serialization;
  using System.Windows;
  using System.Xml;

  using VianaNET.Modules.Video.Control;
  using VianaNET.Properties;

  /// <summary>
  ///   Derived from <see cref="DependencyObject" /> and implements <see cref="INotifyPropertyChanged" />.
  ///   Class to handle a string collection with the last recently used files.
  ///   Its of a singleton class type, to always have one unique complete list,
  ///   if you call <code>RecentFiles.List</code>
  /// </summary>
  public class RecentFiles : DependencyObject, INotifyPropertyChanged
  {
    #region Constants

    /// <summary>
    ///   Maximum length of file name for display in recent file list
    /// </summary>
    private const int MaxLengthDisplay = 40;

    #endregion

    #region Static Fields

    /// <summary>
    ///   Represents the <see cref="DependencyProperty" /> for the
    ///   <see cref="RibbonList" />
    /// </summary>
    public static readonly DependencyProperty RecentFilesCollectionProperty =
      DependencyProperty.Register(
        "RecentFilesCollection", 
        typeof(ObservableCollection<ProjectEntry>), 
        typeof(RecentFiles), 
        new UIPropertyMetadata());

    /// <summary>
    ///   Maximum number of items in recent files list.
    /// </summary>
    private static decimal maxNumItems;

    /// <summary>
    ///   The static member, that holds the recent files list.
    /// </summary>
    private static RecentFiles recentFiles;

    #endregion

    #region Fields

    /// <summary>
    ///   The application settings of the main program,
    ///   where the property RecentFiles has the StringCollection.
    /// </summary>
    private readonly Settings appSettings;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Prevents a default instance of the RecentFiles class from being created.
    ///   Initializes Recentfiles list by reading application settings.
    /// </summary>
    private RecentFiles()
    {
      this.appSettings = Settings.Default;
      maxNumItems = this.appSettings.NumberOfRecentFiles;
      this.Load();

      if (this.RecentFilesCollection == null)
      {
        this.RecentFilesCollection = new ObservableCollection<ProjectEntry>();
      }
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the recent files list.
    /// </summary>
    /// <value> A <see cref="RecentFiles" /> with the recent files. </value>
    public static RecentFiles Instance
    {
      get
      {
        return recentFiles ?? (recentFiles = new RecentFiles());
      }
    }

    /// <summary>
    ///   Gets or sets the <see cref="RecentFilesCollection" /> array
    ///   which contains the recent files list for the ribbon.
    /// </summary>
    public ObservableCollection<ProjectEntry> RecentFilesCollection
    {
      get
      {
        return (ObservableCollection<ProjectEntry>)this.GetValue(RecentFilesCollectionProperty);
      }

      set
      {
        this.SetValue(RecentFilesCollectionProperty, value);
      }
    }

    /// <summary>
    ///   Gets the complete filename with path
    ///   to the settings file used to store recent file collection.
    /// </summary>
    public string SettingsFile
    {
      get
      {
        return Path.Combine(this.appSettings.SettingsPath, "VianaRecentFileList.xml");
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Scales the image.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="maxWidth">
    /// The maximum width.
    /// </param>
    /// <param name="maxHeight">
    /// The maximum height.
    /// </param>
    /// <returns>
    /// The scales image
    /// </returns>
    public static Bitmap ScaleImage(Bitmap image, int maxWidth, int maxHeight)
    {
      double ratioX = (double)maxWidth / image.Width;
      double ratioY = (double)maxHeight / image.Height;
      double ratio = Math.Min(ratioX, ratioY);

      var newWidth = (int)(image.Width * ratio);
      var newHeight = (int)(image.Height * ratio);

      var newImage = new Bitmap(newWidth, newHeight);
      Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
      return newImage;
    }

    /// <summary>
    /// Adds new filename to recent files list and saves list to application settings.
    /// </summary>
    /// <param name="file">
    /// A <see cref="string"/> with full path and filename to recent file
    /// </param>
    public void Add(string file)
    {
      int fileIndex = this.FindFile(file);
      Bitmap bitmap = Video.Instance.CreateBitmapFromCurrentImageSource() ?? new Bitmap(64, 64);

      bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
      Bitmap smallBitmap = ScaleImage(bitmap, 64, 64);
      var projectEntry = new ProjectEntry { ProjectFile = file, ProjectIcon = smallBitmap };

      if (fileIndex < 0)
      {
        this.RecentFilesCollection.Insert(0, projectEntry);

        while (this.RecentFilesCollection.Count > maxNumItems)
        {
          this.RecentFilesCollection.RemoveAt(this.RecentFilesCollection.Count - 1);
        }
      }
      else
      {
        this.RecentFilesCollection.RemoveAt(fileIndex);
        this.RecentFilesCollection.Insert(0, projectEntry);
      }

      this.Save();
    }

    /// <summary>
    ///   Deletes the recent files list in application settings.
    /// </summary>
    public void Delete()
    {
      this.RecentFilesCollection.Clear();
      this.Save();
    }

    /// <summary>
    /// Get display file name from full name.
    /// </summary>
    /// <param name="fullName">
    /// Full file name
    /// </param>
    /// <returns>
    /// A <see cref="string"/> with a short display name
    /// </returns>
    public string GetDisplayName(string fullName)
    {
      // if file is in current directory, show only file name
      var fileInfo = new FileInfo(fullName);

      // keep current directory in the time of initialization
      string currentDirectory = Directory.GetCurrentDirectory();

      if (fileInfo.DirectoryName == currentDirectory)
      {
        return fileInfo.ToString()
          .Substring(0, fileInfo.ToString().Length > MaxLengthDisplay ? MaxLengthDisplay : fileInfo.ToString().Length);
      }

      string filename = Path.GetFileName(fullName);
      if (filename != null && filename.Length > MaxLengthDisplay)
      {
        filename = filename.Substring(0, MaxLengthDisplay);
      }

      return filename;
    }

    /// <summary>
    /// Removes filename from recent files list and saves list to application settings.
    /// </summary>
    /// <param name="file">
    /// A <see cref="string"/> with full path and filename to recent file
    /// </param>
    public void Remove(string file)
    {
      int fileIndex = this.FindFile(file);

      if (fileIndex < 0)
      {
        while (this.RecentFilesCollection.Count > maxNumItems)
        {
          this.RecentFilesCollection.RemoveAt(this.RecentFilesCollection.Count - 1);
        }
      }
      else
      {
        this.RecentFilesCollection.RemoveAt(fileIndex);
      }

      this.Save();
    }

    /// <summary>
    ///   Stores current recent files list into application settings,
    ///   "|" separated.
    /// </summary>
    public void Save()
    {
      var dcs = new DataContractSerializer(typeof(ObservableCollection<ProjectEntry>));
      var fs = new FileStream(this.SettingsFile, FileMode.Create);
      dcs.WriteObject(fs, this.RecentFilesCollection);
      fs.Close();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">
    /// A <see cref="string"/> with the property that has changed
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Get index in recent file list from given file path.
    /// </summary>
    /// <param name="file">
    /// Full path name of search file.
    /// </param>
    /// <returns>
    /// Index in file list, if not found -1.
    /// </returns>
    private int FindFile(string file)
    {
      string fileLower = file.ToLower();
      for (int index = 0; index < this.RecentFilesCollection.Count; index++)
      {
        if (fileLower == this.RecentFilesCollection[index].ProjectFile.ToLower())
        {
          return index;
        }
      }

      return -1;
    }

    /// <summary>
    ///   Loads "|" separated values from application settings into StringCollection base class.
    /// </summary>
    private void Load()
    {
      if (File.Exists(this.SettingsFile))
      {
        var fs = new FileStream(this.SettingsFile, FileMode.Open);
        XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        var ser = new DataContractSerializer(typeof(ObservableCollection<ProjectEntry>));

        // Deserialize the data and read it from the instance.
        this.RecentFilesCollection = (ObservableCollection<ProjectEntry>)ser.ReadObject(reader, true);
        reader.Close();
        fs.Close();
      }
    }

    #endregion
  }
}