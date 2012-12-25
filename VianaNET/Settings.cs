// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Freie Universität Berlin">
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
// <summary>
//   Defines the Settings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Properties
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Runtime.Serialization;
  using System.Xml;
  using Application;

  // This class allows you to handle specific events on the settings class:
  //  The SettingChanging event is raised before a setting's value is changed.
  //  The PropertyChanged event is raised after a setting's value is changed.
  //  The SettingsLoaded event is raised after the setting values are loaded.
  //  The SettingsSaving event is raised before the setting values are saved.
  internal sealed partial class Settings
  {
    public Settings()
    {
      // Setup LogfilePath
      this.SettingsPath = System.Windows.Forms.Application.UserAppDataPath +
                          Path.DirectorySeparatorChar + "Settings" +
                          Path.DirectorySeparatorChar;

      // Create directory if not already existing.
      if (!Directory.Exists(this.SettingsPath))
      {
        Directory.CreateDirectory(this.SettingsPath);
      }
    }
  }
}
