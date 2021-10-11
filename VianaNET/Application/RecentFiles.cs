// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentFiles.cs" company="Freie Universität Berlin">
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
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Represents the <see cref="DependencyProperty" /> for the
    ///   <see cref="RibbonList" />
    /// </summary>
    public static readonly DependencyProperty RecentFilesCollectionProperty = DependencyProperty.Register(
      "RecentFilesCollection", typeof(ObservableCollection<ProjectEntry>), typeof(RecentFiles), new UIPropertyMetadata());

    /// <summary>
    ///   Maximum number of items in recent files list.
    /// </summary>
    private static decimal maxNumItems;

    /// <summary>
    ///   The static member, that holds the recent files list.
    /// </summary>
    private static RecentFiles recentFiles;





    /// <summary>
    ///   The application settings of the main program,
    ///   where the property RecentFiles has the StringCollection.
    /// </summary>
    private readonly Settings appSettings;

    /// <summary>
    ///   Maximum length of file name for display in recent file list
    /// </summary>
    private int maxLengthDisplay = 40;



    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////


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



    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;



    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Gets the recent files list.
    /// </summary>
    /// <value> A <see cref="RecentFiles" /> with the recent files. </value>
    public static RecentFiles Instance => recentFiles ?? (recentFiles = new RecentFiles());

    /// <summary>
    ///   Gets or sets the <see cref="RecentFilesCollection" /> array
    ///   which contains the recent files list for the ribbon.
    /// </summary>
    public ObservableCollection<ProjectEntry> RecentFilesCollection
    {
      get => (ObservableCollection<ProjectEntry>)this.GetValue(RecentFilesCollectionProperty);

      set => this.SetValue(RecentFilesCollectionProperty, value);
    }

    /// <summary>
    /// Gets the complete filename with path
    /// to the settings file used to store recent file collection.
    /// </summary>
    public string SettingsFile => Path.Combine(this.appSettings.SettingsPath, "VianaRecentFileList.xml");



    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Adds new filename to recent files list and saves list to application settings.
    /// </summary>
    /// <param name="file">
    /// A <see cref="string"/> with full path and filename to recent file 
    /// </param>
    public void Add(string file)
    {
      int fileIndex = this.FindFile(file);
      Bitmap bitmap = Video.Instance.CreateBitmapFromCurrentImageSource();
      if (bitmap == null)
      {
        bitmap = new Bitmap(64, 64);
      }

      bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
      Bitmap smallBitmap = ScaleImage(bitmap, 64, 64);
      ProjectEntry projectEntry = new ProjectEntry { ProjectFile = file, ProjectIcon = smallBitmap };

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
      this.RebuildRibbonList();
    }

    /// <summary>
    ///   Deletes the recent files list in application settings.
    /// </summary>
    public void Delete()
    {
      this.RecentFilesCollection.Clear();
      this.Save();
      this.RebuildRibbonList();
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
      FileInfo fileInfo = new FileInfo(fullName);

      // keep current directory in the time of initialization
      string currentDirectory = Directory.GetCurrentDirectory();

      if (fileInfo.DirectoryName == currentDirectory)
      {
        return fileInfo.ToString().Substring(
          0, fileInfo.ToString().Length > this.maxLengthDisplay ? this.maxLengthDisplay : fileInfo.ToString().Length);
      }

      string filename = Path.GetFileName(fullName);
      if (filename.Length > this.maxLengthDisplay)
      {
        filename = filename.Substring(0, this.maxLengthDisplay);
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
        //this.appSettings.RecentProjectEntries.Insert(0, file);

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
      this.RebuildRibbonList();
    }

    /// <summary>
    ///   Stores current recent files list into application settings,
    ///   "|" separated.
    /// </summary>
    public void Save()
    {
      DataContractSerializer dcs = new DataContractSerializer(typeof(ObservableCollection<ProjectEntry>));
      FileStream fs = new FileStream(this.SettingsFile, FileMode.Create);
      dcs.WriteObject(fs, this.RecentFilesCollection);
      fs.Close();
    }



    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">
    /// A <see cref="string"/> with the property that has changed 
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

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
        FileStream fs = new FileStream(this.SettingsFile, FileMode.Open);
        XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<ProjectEntry>));

        // Deserialize the data and read it from the instance.
        this.RecentFilesCollection = (ObservableCollection<ProjectEntry>)ser.ReadObject(reader, true);
        reader.Close();
        fs.Close();
      }

      this.RebuildRibbonList();
    }

    /// <summary>
    ///   This method creates a list of <see cref="RibbonHighlightingListItem" />
    ///   to display the recent files in the ribbon of the application.
    /// </summary>
    private void RebuildRibbonList()
    {
      //var items = new List<RibbonApplicationMenuItem>((int)maxNumItems);
      //if (this.appSettings.RecentProjectEntries == null)
      //{
      //  return;
      //}

      //foreach (ProjectEntry item in this.appSettings.RecentProjectEntries)
      //{
      //  if (!File.Exists(item.ProjectFile))
      //  {
      //    continue;
      //  }

      //  var ribbonItem = new RibbonApplicationMenuItem();
      //  ribbonItem.Header = Path.GetFileName(item.ProjectFile);
      //  ribbonItem.ImageSource = CreateBitmapSourceFromBitmap(item.ProjectIcon);
      //  var itemToolTip = new ToolTip();
      //  itemToolTip.Content = item.ProjectFile;
      //  ribbonItem.ToolTip = itemToolTip;
      //  items.Add(ribbonItem);
      //}

      //this.RecentFilesCollection = this.appSettings.RecentProjectEntries;
      //this.OnPropertyChanged("RecentFilesCollection");
    }

    public static Bitmap ScaleImage(Bitmap image, int maxWidth, int maxHeight)
    {
      double ratioX = (double)maxWidth / image.Width;
      double ratioY = (double)maxHeight / image.Height;
      double ratio = Math.Min(ratioX, ratioY);

      int newWidth = (int)(image.Width * ratio);
      int newHeight = (int)(image.Height * ratio);

      Bitmap newImage = new Bitmap(newWidth, newHeight);
      System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
      return newImage;
    }


  }
}