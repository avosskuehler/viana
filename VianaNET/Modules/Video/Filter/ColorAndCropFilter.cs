
using System;
using System.Windows;
using System.Windows.Media;

namespace VianaNET
{
  public class ColorAndCropFilter : FilterBase
  {
    public int Threshold { get; set; }
    public Color TargetColor { get; set; }
    public Color BlankColor { get; set; }
    public Rect CropRectangle { get; set; }

    public ColorAndCropFilter()
      : base()
    {
    }

    private int startX;
    private int startY;
    private int stopX;
    private int stopY;
    private int offset;

    public void Init()
    {
      startX = 0;
      startY = 0;
      stopX = this.ImageWidth;
      stopY = this.ImageHeight;
      offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;
    }


    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image data.</param>
    public unsafe override void ProcessInPlace(IntPtr image)
    {
      var t = this.Threshold;
      var ipx = this.ImagePixelSize;
      var target = this.TargetColor;
      var blank = this.BlankColor;
      var rMin = target.R - t;
      var rMax = target.R + t;
      var gMin = target.G - t;
      var gMax = target.G + t;
      var bMin = target.B - t;
      var bMax = target.B + t;
      var cropRect = this.CropRectangle;
      var xMin = cropRect.Left;
      var xMax = cropRect.Right;
      var yMin = cropRect.Top;
      var yMax = cropRect.Bottom;

      //int startX = 0;
      //int startY = 0;
      //int stopX = startX + this.ImageWidth;
      //int stopY = startY + this.ImageHeight;
      //int offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;

      // do the job
      byte* ptr = (byte*)image;
      byte r, g, b;

      // allign pointer to the first pixel to process
      ptr += (startY * this.ImageStride + startX * ipx);

      // for each row
      for (int y = startY; y < stopY; y++)
      {
        // Blank cropped area
        if (y < yMin || y > yMax)
        {
          // Blank whole line
          for (int x = startX; x < stopX; x++, ptr += ipx)
          {
            ptr[R] = blank.R;
            ptr[G] = blank.G;
            ptr[B] = blank.B;
          }

          // go to next line
          ptr += offset;
          continue;
        }

        // for each pixel
        for (int x = startX; x < stopX; x++, ptr += ipx)
        {
          // blank cropped pixels
          if (x < xMin || x > xMax)
          {
            ptr[R] = blank.R;
            ptr[G] = blank.G;
            ptr[B] = blank.B;
          }
          else
          {
            // Otherwise check for color range
            r = ptr[R];
            g = ptr[G];
            b = ptr[B];

            // check pixel
            if (
                (r >= rMin) && (r <= rMax) &&
                (g >= gMin) && (g <= gMax) &&
                (b >= bMin) && (b <= bMax)
                )
            {
              continue;
            }
            else
            {
              ptr[R] = blank.R;
              ptr[G] = blank.G;
              ptr[B] = blank.B;
            }
          }
        }

        ptr += offset;
      }
    }
  }
}
