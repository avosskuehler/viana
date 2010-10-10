#region Header
//
//   Project:           FaceLight - Simple Silverlight Real Time Face Detection.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Linq;
using System.Windows.Media.Imaging;

namespace VianaNET
{
  /// <summary>
  /// A Histogram for columns and rows.
  /// </summary>
  public class Histogram : FilterBase
  {
    public int[] X { get; private set; }
    public int[] Y { get; private set; }
    public VectorInt Max { get; private set; }
    public VectorInt MaxIndex { get; private set; }
    public static int CompareEmptyColor { get; set; }

    static Histogram()
    {
      CompareEmptyColor = 0;
    }

    public Histogram()
    {
    }

    public Histogram(int[] histX, int[] histY)
    {
      X = histX;
      Y = histY;

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

      Max = new VectorInt(mx, my);
      MaxIndex = new VectorInt(ix, iy);
    }

    public override void ProcessInPlace(IntPtr image)
    {
    }

    private int startX;
    private int startY;
    private int stopX;
    private int stopY;
    private int offset;

    public unsafe Histogram FromIntPtrMap(IntPtr map)
    {
      byte* ptr = (byte*)map;
      var w = this.ImageWidth;
      var h = this.ImageHeight;
      var histX = new int[w];
      var histY = new int[h];
      //var empty = -16777216;// CompareEmptyColor; // = 0
      var ipx = this.ImagePixelSize;
      startX = 0;
      startY = 0;
      stopX = this.ImageWidth;
      stopY = this.ImageHeight;
      offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;

      byte r, g, b;

      // allign pointer to the first pixel to process
      ptr += (startY * this.ImageStride + startX * ipx);

      // for each row
      for (int y = startY; y < stopY; y++)
      {
        //// Blank cropped area
        //if (y < yMin || y > yMax)
        //{
        //  // Blank whole line
        //  for (int x = startX; x < stopX; x++, ptr += ipx)
        //  {
        //    ptr[R] = blank.R;
        //    ptr[G] = blank.G;
        //    ptr[B] = blank.B;
        //  }

        //  // go to next line
        //  ptr += offset;
        //  continue;
        //}

        // for each pixel
        for (int x = startX; x < stopX; x++, ptr += ipx)
        {
          //// blank cropped pixels
          //if (x < xMin || x > xMax)
          //{
          //  ptr[R] = blank.R;
          //  ptr[G] = blank.G;
          //  ptr[B] = blank.B;
          //}
          //else
          //{
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

        ptr += offset;
      }

      //// Create row and column statistics / histogram
      //for (int y = 0; y < h; y++)
      //{
      //  for (int x = 0; x < w; x++)
      //  {
      //    YCbCrColor ycbcr = YCbCrColor.FromArgbColori(src[y * w + x]);

      //    if (src[y * w + x] != empty)
      //    {
      //      histX[x]++;
      //      histY[y]++;
      //    }
      //  }
      //}

      return new Histogram(histX, histY);
    }
  }
}
