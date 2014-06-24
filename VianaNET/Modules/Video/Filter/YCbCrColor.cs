// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YCbCrColor.cs" company="Freie Universität Berlin">
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
//   YCbCr color representation.
//   Y in the reange [0, 1].
//   Cb in the reange [-0.5, 0.5].
//   Cr in the reange [-0.5, 0.5].
//   According to http://www.poynton.com/notes/colour_and_gamma/ColorFAQ.html#RTFToC28
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System.Windows.Media;

  /// <summary>
  ///   YCbCr color representation.
  ///   Y in the reange [0, 1].
  ///   Cb in the reange [-0.5, 0.5].
  ///   Cr in the reange [-0.5, 0.5].
  ///   According to http://www.poynton.com/notes/colour_and_gamma/ColorFAQ.html#RTFToC28
  /// </summary>
  public class YCbCrColor
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YCbCrColor"/> class.
    /// </summary>
    /// <param name="y">
    /// The y. 
    /// </param>
    /// <param name="cb">
    /// The cb. 
    /// </param>
    /// <param name="cr">
    /// The cr. 
    /// </param>
    public YCbCrColor(float y, float cb, float cr)
    {
      this.Y = y;
      this.Cb = cb;
      this.Cr = cr;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the max.
    /// </summary>
    public static YCbCrColor Max
    {
      get
      {
        return new YCbCrColor(1, 0.5f, 0.5f);
      }
    }

    /// <summary>
    ///   Gets the min.
    /// </summary>
    public static YCbCrColor Min
    {
      get
      {
        return new YCbCrColor(0, -0.5f, -0.5f);
      }
    }

    /// <summary>
    ///   Gets or sets the cb.
    /// </summary>
    public float Cb { get; set; }

    /// <summary>
    ///   Gets or sets the cr.
    /// </summary>
    public float Cr { get; set; }

    /// <summary>
    ///   Gets or sets the y.
    /// </summary>
    public float Y { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The from argb color.
    /// </summary>
    /// <param name="color">
    /// The color. 
    /// </param>
    /// <returns>
    /// The <see cref="YCbCrColor"/> . 
    /// </returns>
    public static YCbCrColor FromArgbColor(Color color)
    {
      return FromRgb(color.R, color.G, color.B);
    }

    /// <summary>
    /// The from argb colori.
    /// </summary>
    /// <param name="color">
    /// The color. 
    /// </param>
    /// <returns>
    /// The <see cref="YCbCrColor"/> . 
    /// </returns>
    public static YCbCrColor FromArgbColori(int color)
    {
      return FromRgb((byte)(color >> 16), (byte)(color >> 8), (byte)color);
    }

    /// <summary>
    /// The from rgb.
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
    /// <returns>
    /// The <see cref="YCbCrColor"/> . 
    /// </returns>
    public static YCbCrColor FromRgb(byte r, byte g, byte b)
    {
      // Create new YCbCr color from rgb color
      const float f = 1f / 255f;
      float rf = r * f;
      float gf = g * f;
      float bf = b * f;

      float y = 0.299f * rf + 0.587f * gf + 0.114f * bf;
      float cb = -0.168736f * rf + -0.331264f * gf + 0.5f * bf;
      float cr = 0.5f * rf + -0.418688f * gf + -0.081312f * bf;

      return new YCbCrColor(y, cb, cr);
    }

    /// <summary>
    /// The interpolate.
    /// </summary>
    /// <param name="c2">
    /// The c 2. 
    /// </param>
    /// <param name="amount">
    /// The amount. 
    /// </param>
    /// <returns>
    /// The <see cref="YCbCrColor"/> . 
    /// </returns>
    public YCbCrColor Interpolate(YCbCrColor c2, float amount)
    {
      return new YCbCrColor(
        this.Y + (c2.Y - this.Y) * amount, this.Cb + (c2.Cb - this.Cb) * amount, this.Cr + (c2.Cr - this.Cr) * amount);
    }

    /// <summary>
    ///   The to argb color.
    /// </summary>
    /// <returns> The <see cref="Color" /> . </returns>
    public Color ToArgbColor()
    {
      int c = this.ToArgbColori();
      return Color.FromArgb((byte)(c >> 24), (byte)(c >> 16), (byte)(c >> 8), (byte)c);
    }

    /// <summary>
    ///   The to argb colori.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    public int ToArgbColori()
    {
      // Convert to RGB
      float r = (this.Y + 1.402f * this.Cr) * 255;
      float g = (this.Y - 0.344136f * this.Cb - 0.714136f * this.Cr) * 255;
      float b = (this.Y + 1.772f * this.Cb) * 255;

      return (255 << 24) | ((byte)(r > 255 ? 255 : r) << 16) | ((byte)(g > 255 ? 255 : g) << 8)
             | (byte)(b > 255 ? 255 : b);
    }

    #endregion
  }
}