// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorAndCropFilterYCbCr.cs" company="Freie Universität Berlin">
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
//   A filter that thresholds the color in the image between a lower and upper threshold.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  ///   A filter that thresholds the color in the image between a lower and upper threshold.
  /// </summary>
  public class ColorAndCropFilterYCbCr : FilterBase
  {
    #region Fields

    /// <summary>
    ///   The pixel color.
    /// </summary>
    private readonly YCbCrColor pixelColor;

    /// <summary>
    ///   The offset.
    /// </summary>
    private int offset;

    /// <summary>
    ///   The stop x.
    /// </summary>
    private int stopX;

    /// <summary>
    ///   The stop y.
    /// </summary>
    private int stopY;

    /// <summary>
    ///   The target color.
    /// </summary>
    private Color targetColor;

    /// <summary>
    ///   The threshold.
    /// </summary>
    private int threshold;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ColorAndCropFilterYCbCr" /> class.
    /// </summary>
    public ColorAndCropFilterYCbCr()
    {
      this.LowerThreshold = YCbCrColor.Min;
      this.UpperThreshold = YCbCrColor.Max;
      this.pixelColor = new YCbCrColor(0, 0, 0);
    }

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

    /// <summary>
    ///   Gets or sets the target color.
    /// </summary>
    public Color TargetColor
    {
      get => this.targetColor;

      set
      {
        this.targetColor = value;
        this.UpdateThresholds();
      }
    }

    /// <summary>
    ///   Gets or sets the threshold.
    /// </summary>
    public int Threshold
    {
      get => this.threshold;

      set
      {
        this.threshold = value;
        this.UpdateThresholds();
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets the lower threshold.
    /// </summary>
    private YCbCrColor LowerThreshold { get; set; }

    /// <summary>
    ///   Gets or sets the upper threshold.
    /// </summary>
    private YCbCrColor UpperThreshold { get; set; }

    #endregion

    // private int startX;
    // private int startY;

    // public override unsafe void ProcessInPlace(IntPtr input)
    // {
    // var r = this.ResultColor;
    // var b = this.BlankColor;
    // YCbCrColor ycbcr;
    // int* ptr = (int*)input;
    // int bufferSize = this.ImageHeight * this.ImageWidth;

    // // Threshold every pixel
    // for (int i = 0; i < bufferSize; i++)
    // {
    // ycbcr = YCbCrColor.FromArgbColori(ptr[i]);
    // if (ycbcr.Y >= LowerThreshold.Y && ycbcr.Y <= UpperThreshold.Y
    // && ycbcr.Cb >= LowerThreshold.Cb && ycbcr.Cb <= UpperThreshold.Cb
    // && ycbcr.Cr >= LowerThreshold.Cr && ycbcr.Cr <= UpperThreshold.Cr)
    // {
    // ptr[i] = r;
    // }
    // else
    // {
    // ptr[i] = b;
    // }
    // }
    // }
    #region Public Methods and Operators

    /// <summary>
    ///   The init.
    /// </summary>
    public void Init()
    {
      // startX = 0;
      // startY = 0;
      this.stopX = this.ImageWidth;
      this.stopY = this.ImageHeight;
      this.offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;
      this.UpdateThresholds();
    }

    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// <param name="image">
    /// Source image data. 
    /// </param>
    public unsafe override void ProcessInPlace(IntPtr image)
    {
      int t = this.Threshold;
      int ipx = this.ImagePixelSize;
      Color target = this.TargetColor;
      Color blank = this.BlankColor;
      int rMin = target.R - t;
      int rMax = target.R + t;
      int gMin = target.G - t;
      int gMax = target.G + t;
      int bMin = target.B - t;
      int bMax = target.B + t;
      Rect cropRect = this.CropRectangle;
      double xMin = cropRect.Left;
      double xMax = cropRect.Right;
      double yMin = cropRect.Top;
      double yMax = cropRect.Bottom;
      YCbCrColor lowThres = this.LowerThreshold;
      YCbCrColor hightThres = this.UpperThreshold;
      float lowThresY = lowThres.Y;
      float lowThresCb = lowThres.Cb;
      float lowThresCr = lowThres.Cr;
      float highThresY = hightThres.Y;
      float highThresCb = hightThres.Cb;
      float highThresCr = hightThres.Cr;

      // int startX = 0;
      // int startY = 0;
      // int stopX = startX + this.ImageWidth;
      // int stopY = startY + this.ImageHeight;
      int offset = this.ImageStride - this.ImageWidth * this.ImagePixelSize;

      // do the job
      byte* ptr = (byte*)image;

      // Threshold every pixel
      for (int i = 0; i < this.ImageWidth * this.ImageHeight; i++, ptr += ipx)
      {
        this.UpdatePixelColorFromRgb(ptr[R], ptr[G], ptr[B]);
        if (this.pixelColor.Y >= lowThresY && this.pixelColor.Y <= highThresY && this.pixelColor.Cb >= lowThresCb
            && this.pixelColor.Cb <= highThresCb && this.pixelColor.Cr >= lowThresCr
            && this.pixelColor.Cr <= highThresCr)
        {
          continue;
        }
        else
        {
          ptr[R] = blank.R;
          ptr[G] = blank.G;
          ptr[B] = blank.B;
        }

        // ptr += offset;
      }

      // byte r, g, b;

      //// allign pointer to the first pixel to process
      // ptr += (startY * this.ImageStride + startX * ipx);

      //// for each row
      // for (int y = startY; y < stopY; y++)
      // {
      // // Blank cropped area
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

      // // for each pixel
      // for (int x = startX; x < stopX; x++, ptr += ipx)
      // {
      // // blank cropped pixels
      // if (x < xMin || x > xMax)
      // {
      // ptr[R] = blank.R;
      // ptr[G] = blank.G;
      // ptr[B] = blank.B;
      // }
      // else
      // {
      // ycbcr = YCbCrColor.FromRgb(ptr[R], ptr[G], ptr[B]);
      // if (ycbcr.Y >= lowThresY && ycbcr.Y <= highThresY
      // && ycbcr.Cb >= lowThresCb && ycbcr.Cb <= highThresCb
      // && ycbcr.Cr >= lowThresCr && ycbcr.Cr <= highThresCr)
      // {
      // continue;
      // }
      // else
      // {
      // ptr[R] = blank.R;
      // ptr[G] = blank.G;
      // ptr[B] = blank.B;
      // }
      // }
      // }

      // ptr += offset;
      // }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The update pixel color from rgb.
    /// </summary>
    /// <param name="r">
    /// The r. 
    /// </param>
    /// <param name="g">
    /// The g. 
    /// </param>
    /// <param name="b">
    /// The b. 
    /// </param>
    private void UpdatePixelColorFromRgb(byte r, byte g, byte b)
    {
      // Create new YCbCr color from rgb color
      const float f = 1f / 255f;
      float rf = r * f;
      float gf = g * f;
      float bf = b * f;

      this.pixelColor.Y = 0.299f * rf + 0.587f * gf + 0.114f * bf;
      this.pixelColor.Cb = -0.168736f * rf + -0.331264f * gf + 0.5f * bf;
      this.pixelColor.Cr = 0.5f * rf + -0.418688f * gf + -0.081312f * bf;
    }

    /// <summary>
    ///   The update thresholds.
    /// </summary>
    private void UpdateThresholds()
    {
      YCbCrColor targetColorInYCbCr = YCbCrColor.FromArgbColor(this.targetColor);

      this.LowerThreshold = new YCbCrColor(
        0.1f, targetColorInYCbCr.Cb - this.threshold / 500f, targetColorInYCbCr.Cr - this.threshold / 500f);

      this.UpperThreshold = new YCbCrColor(
        1.0f, targetColorInYCbCr.Cb + this.threshold / 500f, targetColorInYCbCr.Cr + this.threshold / 500f);
    }

    #endregion
  }
}