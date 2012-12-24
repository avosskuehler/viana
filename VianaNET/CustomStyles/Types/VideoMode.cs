﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoMode.cs" company="Freie Universität Berlin">
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
//   The video mode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Types
{
  /// <summary>
  /// Enumerates the availanle video modes.
  /// Currently two: file, for file playback and capture for live streams
  /// </summary>
  public enum VideoMode
  {
    /// <summary>
    /// No video mode selected.
    /// </summary>
    None,

    /// <summary>
    /// File video mode.
    /// A video file is loaded from disk.
    /// </summary>
    File,

    /// <summary>
    /// Capture file mode.
    /// A video is captured from a live stream.
    /// </summary>
    Capture,
  }
}