using System;
using System.Windows.Media;
using System.Windows;

namespace VianaNET
{
  /// <summary>
  /// A filter that thresholds the color in the image between a lower and upper threshold.
  /// </summary>
  public class ColorAndCropFilterYCbCr : FilterBase
  {
    private int threshold;
    private Color targetColor;

    public int Threshold
    {
      get
      {
        return this.threshold;
      }

      set
      {
        this.threshold = value;
        UpdateThresholds();
      }
    }

    public Color TargetColor
    {
      get
      {
        return this.targetColor;
      }

      set
      {
        this.targetColor = value;
        UpdateThresholds();
      }
    }

    private void UpdateThresholds()
    {
      YCbCrColor targetColorInYCbCr = YCbCrColor.FromArgbColor(this.targetColor);

      this.LowerThreshold = new YCbCrColor(
        0.1f,
        targetColorInYCbCr.Cb - this.threshold / 500f,
        targetColorInYCbCr.Cr - this.threshold / 500f);

      this.UpperThreshold = new YCbCrColor(
        1.0f,
        targetColorInYCbCr.Cb + this.threshold / 500f,
        targetColorInYCbCr.Cr + this.threshold / 500f);
    }

    public Color BlankColor { get; set; }
    public Rect CropRectangle { get; set; }

    private YCbCrColor LowerThreshold { get; set; }
    private YCbCrColor UpperThreshold { get; set; }
    //private int startX;
    //private int startY;
    private int stopX;
    private int stopY;
    private int offset;
    private YCbCrColor pixelColor;

    public ColorAndCropFilterYCbCr()
      : base()
    {
      LowerThreshold = YCbCrColor.Min;
      UpperThreshold = YCbCrColor.Max;
      pixelColor = new YCbCrColor(0, 0, 0);
    }

    //public override unsafe void ProcessInPlace(IntPtr input)
    //{
    //  var r = this.ResultColor;
    //  var b = this.BlankColor;
    //  YCbCrColor ycbcr;
    //  int* ptr = (int*)input;
    //  int bufferSize = this.ImageHeight * this.ImageWidth;

    //  // Threshold every pixel
    //  for (int i = 0; i < bufferSize; i++)
    //  {
    //    ycbcr = YCbCrColor.FromArgbColori(ptr[i]);
    //    if (ycbcr.Y >= LowerThreshold.Y && ycbcr.Y <= UpperThreshold.Y
    //     && ycbcr.Cb >= LowerThreshold.Cb && ycbcr.Cb <= UpperThreshold.Cb
    //     && ycbcr.Cr >= LowerThreshold.Cr && ycbcr.Cr <= UpperThreshold.Cr)
    //    {
    //      ptr[i] = r;
    //    }
    //    else
    //    {
    //      ptr[i] = b;
    //    }
    //  }
    //}

    public void Init()
    {
      //startX = 0;
      //startY = 0;
      stopX = this.ImageWidth;
      stopY = this.ImageHeight;
      offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;
      this.UpdateThresholds();
    }

    private void UpdatePixelColorFromRgb(byte r, byte g, byte b)
    {
      // Create new YCbCr color from rgb color
      const float f = 1f / 255f;
      var rf = r * f;
      var gf = g * f;
      var bf = b * f;

      this.pixelColor.Y = 0.299f * rf + 0.587f * gf + 0.114f * bf;
      this.pixelColor.Cb = -0.168736f * rf + -0.331264f * gf + 0.5f * bf;
      this.pixelColor.Cr = 0.5f * rf + -0.418688f * gf + -0.081312f * bf;
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
      var lowThres = this.LowerThreshold;
      var hightThres = this.UpperThreshold;
      var lowThresY = lowThres.Y;
      var lowThresCb = lowThres.Cb;
      var lowThresCr = lowThres.Cr;
      var highThresY = hightThres.Y;
      var highThresCb = hightThres.Cb;
      var highThresCr = hightThres.Cr;

      //int startX = 0;
      //int startY = 0;
      //int stopX = startX + this.ImageWidth;
      //int stopY = startY + this.ImageHeight;
      int offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;

      // do the job
      byte* ptr = (byte*)image;

      // Threshold every pixel
      for (int i = 0; i < this.ImageWidth * this.ImageHeight; i++, ptr += ipx)
      {
        UpdatePixelColorFromRgb(ptr[R], ptr[G], ptr[B]);
        if (this.pixelColor.Y >= lowThresY && this.pixelColor.Y <= highThresY
         && this.pixelColor.Cb >= lowThresCb && this.pixelColor.Cb <= highThresCb
         && this.pixelColor.Cr >= lowThresCr && this.pixelColor.Cr <= highThresCr)
        {
          continue;
        }
        else
        {
          ptr[R] = blank.R;
          ptr[G] = blank.G;
          ptr[B] = blank.B;
        }

        //ptr += offset;
      }

      //byte r, g, b;

      //// allign pointer to the first pixel to process
      //ptr += (startY * this.ImageStride + startX * ipx);

      //// for each row
      //for (int y = startY; y < stopY; y++)
      //{
      //  // Blank cropped area
      //  if (y < yMin || y > yMax)
      //  {
      //    // Blank whole line
      //    for (int x = startX; x < stopX; x++, ptr += ipx)
      //    {
      //      ptr[R] = blank.R;
      //      ptr[G] = blank.G;
      //      ptr[B] = blank.B;
      //    }

      //    // go to next line
      //    ptr += offset;
      //    continue;
      //  }

      //  // for each pixel
      //  for (int x = startX; x < stopX; x++, ptr += ipx)
      //  {
      //    // blank cropped pixels
      //    if (x < xMin || x > xMax)
      //    {
      //      ptr[R] = blank.R;
      //      ptr[G] = blank.G;
      //      ptr[B] = blank.B;
      //    }
      //    else
      //    {
      //      ycbcr = YCbCrColor.FromRgb(ptr[R], ptr[G], ptr[B]);
      //      if (ycbcr.Y >= lowThresY && ycbcr.Y <= highThresY
      //       && ycbcr.Cb >= lowThresCb && ycbcr.Cb <= highThresCb
      //       && ycbcr.Cr >= lowThresCr && ycbcr.Cr <= highThresCr)
      //      {
      //        continue;
      //      }
      //      else
      //      {
      //        ptr[R] = blank.R;
      //        ptr[G] = blank.G;
      //        ptr[B] = blank.B;
      //      }
      //    }
      //  }

      //  ptr += offset;
      //}
    }
  }
}
