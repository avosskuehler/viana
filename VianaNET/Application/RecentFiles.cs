// <copyright file="RecentFiles.cs" company="FU Berlin">
// ************************************************************************
// Viana.NET - video analysis for physics education
// Copyright (C) 2010 Dr. Adrian Voßkühler  
// ------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License 
// along with this program; if not, write to the Free Software Foundation, 
// Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian.vosskuehler@fu-berlin.de</email>

namespace VianaNET
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.IO;
  using System.Text;
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Windows.Controls.Ribbon;

  /// <summary>
  /// Derived from <see cref="DependencyObject"/> and implements <see cref="INotifyPropertyChanged"/>.
  /// Class to handle a string collection with the last recently used files.
  /// Its of a singleton class type, to always have one unique complete list,
  /// if you call <code>RecentFiles.List</code>
  /// </summary>
  public class RecentFiles : DependencyObject, INotifyPropertyChanged
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    /// <summary>
    /// Represents the <see cref="DependencyProperty"/> for the
    /// <see cref="RibbonList"/>
    /// </summary>
    public static readonly DependencyProperty RibbonListProperty =
      DependencyProperty.Register(
      "RibbonList",
      typeof(RibbonHighlightingListItem[]),
      typeof(RecentFiles),
      new UIPropertyMetadata());

    /// <summary>
    /// The static member, that holds the recent files list.
    /// </summary>
    private static RecentFiles recentFiles;

    /// <summary>
    /// Maximum number of items in recent files list.
    /// </summary>
    private static decimal maxNumItems;

    /// <summary>
    /// The application settings of the main program,
    /// where the property RecentFiles has the StringCollection.
    /// </summary>
    private VianaNET.Properties.Settings appSettings;

    /// <summary>
    /// The <see cref="StringCollection"/> containg the recent files.
    /// </summary>
    private StringCollection fileCollection;

    /// <summary>
    /// Maximum length of file name for display in recent file list
    /// </summary>
    private int maxLengthDisplay = 40;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Prevents a default instance of the RecentFiles class from being created.
    /// Initializes Recentfiles list by reading application settings.
    /// </summary>
    private RecentFiles()
    {
      this.appSettings = VianaNET.Properties.Settings.Default;
      maxNumItems = this.appSettings.NumberOfRecentFiles;
      this.fileCollection = new StringCollection();
      this.Load();
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES
    
    /// <summary>
    /// Gets the recent files list.
    /// </summary>
    /// <value>A <see cref="RecentFiles"/> with the recent files.</value>
    public static RecentFiles Instance
    {
      get
      {
        if (recentFiles == null)
        {
          recentFiles = new RecentFiles();
        }

        return recentFiles;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="RibbonHighlightingListItem"/> array
    /// which contains the recent files list for the ribbon.
    /// </summary>
    public RibbonHighlightingListItem[] RibbonList
    {
      get { return (RibbonHighlightingListItem[])GetValue(RibbonListProperty); }
      set { SetValue(RibbonListProperty, value); }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    /// <summary>
    /// Stores current recent files list into application settings,
    /// "|" separated.
    /// </summary>
    public void Save()
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool first = true;
      foreach (string file in this.fileCollection)
      {
        if (first)
        {
          first = false;
        }
        else
        {
          stringBuilder.Append('|');
        }

        stringBuilder.Append(file);
      }

      this.appSettings.RecentFileList = stringBuilder.ToString();
      this.appSettings.Save();
    }

    /// <summary>
    /// Adds new filename to recent files list and saves list to application settings.
    /// </summary>
    /// <param name="file">A <see cref="string"/> with full path and
    /// filename to recent file</param>
    public void Add(string file)
    {
      int fileIndex = this.FindFile(file);

      if (fileIndex < 0)
      {
        this.fileCollection.Insert(0, file);

        while (this.fileCollection.Count > maxNumItems)
        {
          this.fileCollection.RemoveAt(this.fileCollection.Count - 1);
        }
      }
      else
      {
        this.fileCollection.RemoveAt(fileIndex);
        this.fileCollection.Insert(0, file);
      }

      this.Save();

      this.RebuildRibbonList();
    }

    /// <summary>
    /// Removes filename from recent files list and saves list to application settings.
    /// </summary>
    /// <param name="file">A <see cref="string"/> with full path and
    /// filename to recent file</param>
    public void Remove(string file)
    {
      int fileIndex = this.FindFile(file);

      if (fileIndex < 0)
      {
        this.fileCollection.Insert(0, file);

        while (this.fileCollection.Count > maxNumItems)
        {
          this.fileCollection.RemoveAt(this.fileCollection.Count - 1);
        }
      }
      else
      {
        this.fileCollection.RemoveAt(fileIndex);
      }

      this.Save();
      this.RebuildRibbonList();
    }

    /// <summary>
    /// Deletes the recent files list in application settings.
    /// </summary>
    public void Delete()
    {
      this.appSettings.RecentFileList = string.Empty;
      this.appSettings.Save();
      this.fileCollection.Clear();
      this.RebuildRibbonList();
    }

    /// <summary>
    /// Get display file name from full name.
    /// </summary>
    /// <param name="fullName">Full file name</param>
    /// <returns>A <see cref="string"/> with a short display name</returns>
    public string GetDisplayName(string fullName)
    {
      // if file is in current directory, show only file name
      FileInfo fileInfo = new FileInfo(fullName);

      // keep current directory in the time of initialization
      string currentDirectory = Directory.GetCurrentDirectory();

      if (fileInfo.DirectoryName == currentDirectory)
      {
        return fileInfo.ToString().Substring(0, fileInfo.ToString().Length > this.maxLengthDisplay ? this.maxLengthDisplay : fileInfo.ToString().Length);
      }

      string filename = Path.GetFileName(fullName);
      if (filename.Length > this.maxLengthDisplay)
      {
        filename = filename.Substring(0, this.maxLengthDisplay);
      }

      return filename;
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER
    
    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">A <see cref="String"/> with the property
    /// that has changed</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS
    
    /// <summary>
    /// This method creates a list of <see cref="RibbonHighlightingListItem"/>
    /// to display the recent files in the ribbon of the application.
    /// </summary>
    private void RebuildRibbonList()
    {
      List<RibbonHighlightingListItem> items = new List<RibbonHighlightingListItem>((int)maxNumItems);
      foreach (string item in this.fileCollection)
      {
        if (!File.Exists(item))
        {
          continue;
        }

        RibbonHighlightingListItem ribbonItem = new RibbonHighlightingListItem();
        ribbonItem.Content = Path.GetFileName(item);
        ToolTip itemToolTip = new ToolTip();
        itemToolTip.Content = item;
        ribbonItem.ToolTip = itemToolTip;
        items.Add(ribbonItem);
      }

      this.RibbonList = items.ToArray();
      this.OnPropertyChanged("RibbonList");
    }

    /// <summary>
    /// Loads "|" separated values from application settings into StringCollection base class.
    /// </summary>
    private void Load()
    {
      string listEntry = this.appSettings.RecentFileList;
      if (listEntry != null)
      {
        string[] files = listEntry.Split(new char[] { '|' });
        foreach (string file in files)
        {
          this.fileCollection.Add(file);
        }
      }

      this.RebuildRibbonList();
    }

    /// <summary>
    /// Get index in recent file list from given file path.
    /// </summary>
    /// <param name="file">Full path name of search file.</param>
    /// <returns>Index in file list, if not found -1.</returns>
    private int FindFile(string file)
    {
      string fileLower = file.ToLower();
      for (int index = 0; index < this.fileCollection.Count; index++)
      {
        if (fileLower == this.fileCollection[index].ToLower())
        {
          return index;
        }
      }

      return -1;
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}

