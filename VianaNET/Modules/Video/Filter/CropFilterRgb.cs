// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CropFilterRgb.cs" company="Freie Universität Berlin">
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
//   The crop rgb filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  ///   The crop rgb filter.
  /// </summary>
  public class CropFilterRgb : FilterBase
  {
    #region Fields

    /// <summary>
    ///   The offset.
    /// </summary>
    private int offset;

    /// <summary>
    ///   The start x.
    /// </summary>
    private int startX;

    /// <summary>
    ///   The start y.
    /// </summary>
    private int startY;

    /// <summary>
    ///   The stop x.
    /// </summary>
    private int stopX;

    /// <summary>
    ///   The stop y.
    /// </summary>
    private int stopY;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the blank color.
    /// </summary>
    public Color BlankColor { get; set; }

    /// <summary>
    ///   Gets or sets the crop rectangle.
    /// </summary>
    public Rect CropRectangle { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The init.
    /// </summary>
    public void Init()
    {
      this.startX = 0;
      this.startY = 0;
      this.stopX = this.ImageWidth;
      this.stopY = this.ImageHeight;
      this.offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;
    }

    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// <param name="image">
    /// Source image data.
    /// </param>
    public override unsafe void ProcessInPlace(IntPtr image)
    {
      int ipx = this.ImagePixelSize;
      Rect cropRect = this.CropRectangle;
      double xMin = cropRect.Left;
      double xMax = cropRect.Right;
      double yMin = cropRect.Top;
      double yMax = cropRect.Bottom;
      Color blank = this.BlankColor;

      // do the job
      var ptr = (byte*)image;

      // align pointer to the first pixel to process
      ptr += this.startY * this.ImageStride + this.startX * ipx;

      // for each row
      for (int y = this.startY; y < this.stopY; y++)
      {
        // Blank cropped area
        if (y < yMin || y > yMax)
        {
          // Blank whole line
          for (int x = this.startX; x < this.stopX; x++, ptr += ipx)
          {
            ptr[R] = blank.R;
            ptr[G] = blank.G;
            ptr[B] = blank.B;
          }

          // go to next line
          ptr += this.offset;
          continue;
        }

        // for each pixel
        for (int x = this.startX; x < this.stopX; x++, ptr += ipx)
        {
          // blank cropped pixels
          if (x < xMin || x > xMax)
          {
            ptr[R] = blank.R;
            ptr[G] = blank.G;
            ptr[B] = blank.B;
          }
        }

        ptr += this.offset;
      }
    }

    #endregion
  }
}