
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace VianaNET
{
  public class ColorFilter : FilterBase
  {
    public int Threshold { get; set; }
    public Color TargetColor { get; set; }
    public Color BlankColor { get; set; }

    public ColorFilter()
      : base()
    {
    }

    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image data.</param>
    public unsafe override void ProcessInPlace(IntPtr image)
    {
      int startX = 0;
      int startY = 0;
      int stopX = startX + this.ImageWidth;
      int stopY = startY + this.ImageHeight;
      int offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;

      // do the job
      byte* ptr = (byte*)image;
      byte r, g, b;

      // allign pointer to the first pixel to process
      ptr += (startY * this.ImageStride + startX * this.ImagePixelSize);

      // for each row
      for (int y = startY; y < stopY; y++)
      {
        // for each pixel
        for (int x = startX; x < stopX; x++, ptr += this.ImagePixelSize)
        {
          r = ptr[R];
          g = ptr[G];
          b = ptr[B];

          // check pixel
          if (
              (r < TargetColor.R - this.Threshold) || (r > TargetColor.R + this.Threshold) ||
              (g < TargetColor.G - this.Threshold) || (g > TargetColor.G + this.Threshold) ||
              (b < TargetColor.B - this.Threshold) || (b > TargetColor.B + this.Threshold)
              )
          {
            ptr[R] = BlankColor.R;
            ptr[G] = BlankColor.G;
            ptr[B] = BlankColor.B;
          }
        }
        ptr += offset;
      }
    }
  }
}
