// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Freie Universität Berlin" file="MotionHighlightingSpecial.cs">
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System.Drawing;
  using System.Drawing.Imaging;

  using AForge.Imaging;
  using AForge.Vision.Motion;

  using Image = System.Drawing.Image;

  /// <summary>
  ///   Motion processing algorithm, which highlights motion areas.
  /// This is an adapted filter from the AForge library. All credits to AForge.NET
  /// </summary>
  /// <seealso cref="MotionDetector" />
  /// <seealso cref="IMotionDetector" />
  public class MotionAreaHighlightingSpecial : IMotionProcessing
  {


    /// <summary>
    /// The highlight color.
    /// </summary>
    private Color highlightColor = Color.Red;





    /// <summary>
    ///   Initializes a new instance of the <see cref="MotionAreaHighlighting" /> class.
    /// </summary>
    public MotionAreaHighlightingSpecial()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionAreaHighlighting"/> class.
    /// </summary>
    /// <param name="highlightColor">
    /// Color used to highlight motion regions.
    /// </param>
    public MotionAreaHighlightingSpecial(Color highlightColor)
    {
      this.highlightColor = highlightColor;
    }





    /// <summary>
    ///   Color used to highlight motion regions.
    /// </summary>
    /// <remarks>
    ///   <para>Default value is set to <b>red</b> color.</para>
    /// </remarks>
    public Color HighlightColor
    {
      get => this.highlightColor;

      set => this.highlightColor = value;
    }





    /// <summary>
    /// Process video and motion frames doing further post processing after
    ///   performed motion detection.
    /// </summary>
    /// <param name="videoFrame">
    /// Original video frame.
    /// </param>
    /// <param name="motionFrame">
    /// Motion frame provided by motion detection
    ///   algorithm (see <see cref="IMotionDetector"/>).
    /// </param>
    /// <remarks>
    /// <para>
    /// Processes provided motion frame and highlights motion areas
    ///     on the original video frame with <see cref="HighlightColor">specified color</see>.
    ///   </para>
    /// </remarks>
    /// <exception cref="InvalidImagePropertiesException">
    /// Motion frame is not 8 bpp image, but it must be so.
    /// </exception>
    /// <exception cref="UnsupportedImageFormatException">
    /// Video frame must be 8 bpp grayscale image or 24/32 bpp color image.
    /// </exception>
    public unsafe void ProcessFrame(UnmanagedImage videoFrame, UnmanagedImage motionFrame)
    {
      if (motionFrame.PixelFormat != PixelFormat.Format8bppIndexed)
      {
        throw new InvalidImagePropertiesException("Motion frame must be 8 bpp image.");
      }

      if ((videoFrame.PixelFormat != PixelFormat.Format8bppIndexed)
          && (videoFrame.PixelFormat != PixelFormat.Format24bppRgb)
          && (videoFrame.PixelFormat != PixelFormat.Format32bppRgb)
          && (videoFrame.PixelFormat != PixelFormat.Format32bppArgb))
      {
        throw new UnsupportedImageFormatException("Video frame must be 8 bpp grayscale image or 24/32 bpp color image.");
      }

      int width = videoFrame.Width;
      int height = videoFrame.Height;
      int pixelSize = Image.GetPixelFormatSize(videoFrame.PixelFormat) / 8;

      if ((motionFrame.Width != width) || (motionFrame.Height != height))
      {
        return;
      }

      byte* src = (byte*)videoFrame.ImageData.ToPointer();
      byte* motion = (byte*)motionFrame.ImageData.ToPointer();

      int srcOffset = videoFrame.Stride - width * pixelSize;
      int motionOffset = motionFrame.Stride - width;

      if (pixelSize == 1)
      {
        // grayscale case
        byte fillG =
          (byte)(0.2125 * this.highlightColor.R + 0.7154 * this.highlightColor.G + 0.0721 * this.highlightColor.B);

        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++, motion++, src++)
          {
            if (*motion != 0)
            {
              // && (((x + y) & 1) == 0))
              *src = fillG;
            }
          }

          src += srcOffset;
          motion += motionOffset;
        }
      }
      else
      {
        // color case
        byte fillR = this.highlightColor.R;
        byte fillG = this.highlightColor.G;
        byte fillB = this.highlightColor.B;

        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++, motion++, src += pixelSize)
          {
            if (*motion != 0)
            {
              // && (((x + y) & 1) == 0))
              src[RGB.R] = fillR;
              src[RGB.G] = fillG;
              src[RGB.B] = fillB;
            }
            else
            {
              src[RGB.R] = 0;
              src[RGB.G] = 0;
              src[RGB.B] = 0;
            }
          }

          src += srcOffset;
          motion += motionOffset;
        }
      }
    }

    /// <summary>
    ///   Reset internal state of motion processing algorithm.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The method allows to reset internal state of motion processing
    ///     algorithm and prepare it for processing of next video stream or to restart
    ///     the algorithm.
    ///   </para>
    /// </remarks>
    public void Reset()
    {
    }


  }
}