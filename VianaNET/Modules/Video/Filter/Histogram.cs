// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Histogram.cs" company="Freie Universität Berlin">
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
//   A Histogram for columns and rows.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;

  /// <summary>
  ///   A Histogram for columns and rows.
  /// </summary>
  public class Histogram : FilterBase
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

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="Histogram" /> class.
    /// </summary>
    static Histogram()
    {
      CompareEmptyColor = 0;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Histogram" /> class.
    /// </summary>
    public Histogram()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> class.
    /// </summary>
    /// <param name="histX">
    /// The hist x.
    /// </param>
    /// <param name="histY">
    /// The hist y.
    /// </param>
    public Histogram(int[] histX, int[] histY)
    {
      this.X = histX;
      this.Y = histY;

      // Find maximum value and index (coordinate) for x, y
      int ix = 0, iy = 0, mx = 0, my = 01;
      for (int i = 0; i < histX.Length; i++)
      {
        if (histX[i] > mx)
        {
          mx = histX[i];
          ix = i;
        }
      }

      for (int i = 0; i < histY.Length; i++)
      {
        if (histY[i] > my)
        {
          my = histY[i];
          iy = i;
        }
      }

      this.Max = new VectorInt(mx, my);
      this.MaxIndex = new VectorInt(ix, iy);
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the compare empty color.
    /// </summary>
    public static int CompareEmptyColor { get; set; }

    /// <summary>
    ///   Gets the max.
    /// </summary>
    public VectorInt Max { get; private set; }

    /// <summary>
    ///   Gets the max index.
    /// </summary>
    public VectorInt MaxIndex { get; private set; }

    /// <summary>
    ///   Gets the x.
    /// </summary>
    public int[] X { get; private set; }

    /// <summary>
    ///   Gets the y.
    /// </summary>
    public int[] Y { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The from int ptr map.
    /// </summary>
    /// <param name="map">
    /// The map.
    /// </param>
    /// <returns>
    /// The <see cref="Histogram"/> .
    /// </returns>
    public unsafe Histogram FromIntPtrMap(IntPtr map)
    {
      var ptr = (byte*)map;
      int w = this.ImageWidth;
      int h = this.ImageHeight;
      var histX = new int[w];
      var histY = new int[h];

      // var empty = -16777216;// CompareEmptyColor; // = 0
      int ipx = this.ImagePixelSize;
      this.startX = 0;
      this.startY = 0;
      this.stopX = this.ImageWidth;
      this.stopY = this.ImageHeight;
      this.offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;

      byte r, g, b;

      // allign pointer to the first pixel to process
      ptr += this.startY * this.ImageStride + this.startX * ipx;

      // for each row
      for (int y = this.startY; y < this.stopY; y++)
      {
        //// Blank cropped area
        // if (y < yMin || y > yMax)
        // {
        // // Blank whole line
        // for (int x = startX; x < stopX; x++, ptr += ipx)
        // {
        // ptr[R] = blank.R;
        // ptr[G] = blank.G;
        // ptr[B] = blank.B;
        // }

        // // go to next line
        // ptr += offset;
        // continue;
        // }

        // for each pixel
        for (int x = this.startX; x < this.stopX; x++, ptr += ipx)
        {
          //// blank cropped pixels
          // if (x < xMin || x > xMax)
          // {
          // ptr[R] = blank.R;
          // ptr[G] = blank.G;
          // ptr[B] = blank.B;
          // }
          // else
          // {
          // Otherwise check for color range
          r = ptr[R];
          g = ptr[G];
          b = ptr[B];

          if (r != 0 || g != 0 || b != 0)
          {
            histX[x]++;
            histY[y]++;
          }
        }

        ptr += this.offset;
      }

      //// Create row and column statistics / histogram
      // for (int y = 0; y < h; y++)
      // {
      // for (int x = 0; x < w; x++)
      // {
      // YCbCrColor ycbcr = YCbCrColor.FromArgbColori(src[y * w + x]);

      // if (src[y * w + x] != empty)
      // {
      // histX[x]++;
      // histY[y]++;
      // }
      // }
      // }
      return new Histogram(histX, histY);
    }

    /// <summary>
    /// The process in place.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    public override void ProcessInPlace(IntPtr image)
    {
    }

    #endregion
  }
}