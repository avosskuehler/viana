// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEntry.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   //   Viana.NET - video analysis for physics education
//   //   Copyright (C) 2021 Dr. Adrian Voßkühler  
//   //   ------------------------------------------------------------------------
//   //   This program is free software; you can redistribute it and/or modify it 
//   //   under the terms of the GNU General Public License as published by the 
//   //   Free Software Foundation; either version 2 of the License, or 
//   //   (at your option) any later version.
//   //   This program is distributed in the hope that it will be useful, 
//   //   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   //   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   //   See the GNU General Public License for more details.
//   //   You should have received a copy of the GNU General Public License 
//   //   along with this program; if not, write to the Free Software Foundation, 
//   //   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//   //   ************************************************************************
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Application
{
  using System.Drawing;
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media.Imaging;
  using System.Xml.Serialization;

  /// <summary>
  /// This class stores an recent files list entry.
  /// </summary>
  [Serializable]
  public class ProjectEntry
  {
    /// <summary>
    /// Gets or sets the filename with full path of the project file.
    /// </summary>
    public string ProjectFile { get; set; }

    /// <summary>
    /// Gets the file name of the current project.
    /// </summary>
    [XmlIgnore]
    public string ProjectFileName => Path.GetFileName(this.ProjectFile);

    /// <summary>
    /// Gets the <see cref="BitmapSource"/> to be used as an image source
    /// for the current projects quick view icon.
    /// </summary>
    [XmlIgnore]
    public BitmapSource ProjectImageSource => CreateBitmapSourceFromBitmap(this.ProjectIcon);

    /// <summary>
    /// Gets or sets an <see cref="Bitmap"/> with an icon of the
    /// project video.
    /// </summary>
    public Bitmap ProjectIcon { get; set; }

    /// <summary>
    /// This methods converts the given bitmap into a BitmapSource
    /// that can be used as an image source for wpf buttons.
    /// </summary>
    /// <param name="bmp">The <see cref="Bitmap"/> to be converted</param>
    /// <returns>A <see cref="BitmapSource"/> containing the given bitmap.</returns>
    public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bmp)
    {
      return Imaging.CreateBitmapSourceFromHBitmap(
        bmp.GetHbitmap(),
        IntPtr.Zero,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions());
    }
  }
}
