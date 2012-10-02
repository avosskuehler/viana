// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConvertColor.cs" company="Freie Universität Berlin">
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
//   Convert colors from RGB to HSL/HSV and vice-versa.
//   http://en.wikipedia.org/wiki/HSL_color_space
//   http://en.wikipedia.org/wiki/HSV_color_space
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Colors
{
  using System;
  using System.Windows.Media;

  /// <summary>
  ///   Convert colors from RGB to HSL/HSV and vice-versa.
  ///   http://en.wikipedia.org/wiki/HSL_color_space
  ///   http://en.wikipedia.org/wiki/HSV_color_space
  /// </summary>
  public static class ConvertColor
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constants

    /// <summary>
    ///   Saves the 1/6 value
    /// </summary>
    private const float OneSixth = 1F / 6F;

    /// <summary>
    ///   saves the 1/3 value
    /// </summary>
    private const float OneThird = 1F / 3F;

    /// <summary>
    ///   Saves the value of 2/3.
    /// </summary>
    private const float TwoThirds = 2F / 3F;

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// HSL to RGB Color Converter
    /// </summary>
    /// <param name="hslColor">
    /// HSL Color to be converted. 
    /// </param>
    /// <returns>
    /// The HSL color as a RGB <see cref="Color"/> 
    /// </returns>
    public static Color HSLToRGB(HSXColor hslColor)
    {
      return HSLToRGB(hslColor.Hue, hslColor.Saturation, hslColor.ValueLuminanceBrightness);
    }

    /// <summary>
    /// HSL to RGB Color Converter
    /// </summary>
    /// <param name="h">
    /// The hue value 
    /// </param>
    /// <param name="s">
    /// The saturation value 
    /// </param>
    /// <param name="l">
    /// The luminance value 
    /// </param>
    /// <returns>
    /// The HSL color as a RGB <see cref="Color"/> 
    /// </returns>
    public static Color HSLToRGB(float? h, float s, float l)
    {
      float r = 0F;
      float g = 0F;
      float b = 0F;

      if (h == null || s == 0)
      {
        r = g = b = l;
      }
      else
      {
        float q = 0F;

        if (l < 0.5F)
        {
          q = l * (1F + s);
        }
        else
        {
          q = l + s - (l * s);
        }

        float p = 2F * l - q;

        r = h.Value + OneThird;

        if (r > 1F)
        {
          r -= 1F;
        }

        g = h.Value;

        b = h.Value - OneThird;

        if (b < 0F)
        {
          b += 1F;
        }

        r = HSLToRGBAux(r, q, p);
        g = HSLToRGBAux(g, q, p);
        b = HSLToRGBAux(b, q, p);
      }

      return Color.FromRgb(Convert.ToByte(r * 255F), Convert.ToByte(g * 255F), Convert.ToByte(b * 255F));
    }

    /// <summary>
    /// HSV to RGB Color Converter
    /// </summary>
    /// <param name="hsvColor">
    /// HSV Color to be converted. 
    /// </param>
    /// <returns>
    /// The HSV color as a RGB <see cref="Color"/> 
    /// </returns>
    public static Color HSVToRGB(HSXColor hsvColor)
    {
      return HSVToRGB(hsvColor.Hue, hsvColor.Saturation, hsvColor.ValueLuminanceBrightness);
    }

    /// <summary>
    /// HSV to RGB Color Converter
    /// </summary>
    /// <param name="h">
    /// The hue value 
    /// </param>
    /// <param name="s">
    /// The saturation value 
    /// </param>
    /// <param name="v">
    /// The XXX value 
    /// </param>
    /// <returns>
    /// The HSV color as a RGB <see cref="Color"/> 
    /// </returns>
    public static Color HSVToRGB(float? h, float s, float v)
    {
      float r = 0F;
      float g = 0F;
      float b = 0F;

      if (h == null || s == 0)
      {
        r = g = b = v;
      }
      else
      {
        byte h1;

        if (1F - ((h.Value / OneSixth) % 1) < 0.0001F)
        {
          h1 = (byte)Math.Ceiling(h.Value / OneSixth);
        }
        else
        {
          h1 = (byte)Math.Floor(h.Value / OneSixth);
        }

        float f = h.Value / OneSixth - h1;
        float p = v * (1F - s);
        float q = v * (1F - f * s);
        float t = v * (1F - (1F - f) * s);

        switch (h1)
        {
          case 0:
          case 6:
            r = v;
            g = t;
            b = p;
            break;
          case 1:
            r = q;
            g = v;
            b = p;
            break;
          case 2:
            r = p;
            g = v;
            b = t;
            break;
          case 3:
            r = p;
            g = q;
            b = v;
            break;
          case 4:
            r = t;
            g = p;
            b = v;
            break;
          case 5:
            r = v;
            g = p;
            b = q;
            break;
        }
      }

      return Color.FromRgb(Convert.ToByte(r * 255F), Convert.ToByte(g * 255F), Convert.ToByte(b * 255F));
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// RGB to HSL Color Converter
    /// </summary>
    /// <param name="color">
    /// RGB Color to be converted 
    /// </param>
    /// <returns>
    /// The RGB color as a <see cref="HSXColor"/> 
    /// </returns>
    public static HSXColor RBGToHSL(Color color)
    {
      return RBGToHSL(color.R, color.G, color.B);
    }

    /// <summary>
    /// RGB to HSL Color Converter
    /// </summary>
    /// <param name="r">
    /// Red value (0 to 255). 
    /// </param>
    /// <param name="g">
    /// Green value (0 to 255). 
    /// </param>
    /// <param name="b">
    /// Blue value (0 to 255). 
    /// </param>
    /// <returns>
    /// The RGB color as a <see cref="HSXColor"/> 
    /// </returns>
    public static HSXColor RBGToHSL(byte r, byte g, byte b)
    {
      return RBGToHSL(r / 255F, g / 255F, b / 255F);
    }

    /// <summary>
    /// RGB to HSL Color Converter
    /// </summary>
    /// <param name="r">
    /// Red value (0 to 1). 
    /// </param>
    /// <param name="g">
    /// Green value (0 to 1). 
    /// </param>
    /// <param name="b">
    /// Blue value (0 to 1). 
    /// </param>
    /// <returns>
    /// The RGB color as a <see cref="HSXColor"/> 
    /// </returns>
    public static HSXColor RBGToHSL(float r, float g, float b)
    {
      float min = Math.Min(r, Math.Min(g, b));
      float max = Math.Max(r, Math.Max(g, b));

      float? h = null;
      float s = 0F;

      float delta = max - min;

      // Brightness
      float l = (max + min) / 2F;

      if (delta != 0F)
      {
        // Hue
        if (r == max)
        {
          h = (g - b) / (6F * delta);

          if (g < b)
          {
            h += 1F;
          }
        }
        else if (g == max)
        {
          h = ((b - r) / (6F * delta)) + OneThird;
        }
        else
        {
          // b is max
          h = ((r - g) / (6F * delta)) + TwoThirds;
        }

        // Saturation
        if (l != 0F)
        {
          if (l < 0.5F)
          {
            s = delta / (2F * l);
          }
          else
          {
            s = delta / (2F - 2F * l);
          }
        }
      }

      return new HSXColor(h, s, l);
    }

    /// <summary>
    /// RGB to HSV Color Converter
    /// </summary>
    /// <param name="color">
    /// RGB Color. 
    /// </param>
    /// <returns>
    /// The RGB color as a <see cref="HSXColor"/> 
    /// </returns>
    public static HSXColor RBGToHSV(Color color)
    {
      return RBGToHSV(color.R, color.G, color.B);
    }

    /// <summary>
    /// RGB to HSV Color Converter
    /// </summary>
    /// <param name="r">
    /// Red value (0 to 255). 
    /// </param>
    /// <param name="g">
    /// Green value (0 to 255). 
    /// </param>
    /// <param name="b">
    /// Blue value (0 to 255). 
    /// </param>
    /// <returns>
    /// The RGB color as a <see cref="HSXColor"/> 
    /// </returns>
    public static HSXColor RBGToHSV(byte r, byte g, byte b)
    {
      return RBGToHSV(r / 255F, g / 255F, b / 255F);
    }

    /// <summary>
    /// RGB to HSV Color Converter
    /// </summary>
    /// <param name="r">
    /// Red value (0 to 1). 
    /// </param>
    /// <param name="g">
    /// Green value (0 to 1). 
    /// </param>
    /// <param name="b">
    /// Blue value (0 to 1). 
    /// </param>
    /// <returns>
    /// The RGB color as a <see cref="HSXColor"/> 
    /// </returns>
    public static HSXColor RBGToHSV(float r, float g, float b)
    {
      float min = Math.Min(r, Math.Min(g, b));
      float max = Math.Max(r, Math.Max(g, b));

      float? h = null;
      float s = 0F;

      float delta = max - min;

      // Brightness
      float v = max;

      if (delta != 0F)
      {
        // Hue
        if (r == max)
        {
          h = (g - b) / (6F * delta);

          if (g < b)
          {
            h += 1F;
          }
        }
        else if (g == max)
        {
          h = (b - r) / (6F * delta) + OneThird;
        }
        else
        {
          // b is max
          h = (r - g) / (6F * delta) + TwoThirds;
        }
      }

      // Saturation
      if (max != 0F)
      {
        s = 1F - (min / max);
      }

      return new HSXColor(h, s, v);
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    /// Converts colors. Don´t know exactly what it does.
    /// </summary>
    /// <param name="value">
    /// r,g,b value 
    /// </param>
    /// <param name="q">
    /// q value (don´t know what that is) 
    /// </param>
    /// <param name="p">
    /// p value (don´t know what that is) 
    /// </param>
    /// <returns>
    /// A float with the r,g or b value. 
    /// </returns>
    private static float HSLToRGBAux(float value, float q, float p)
    {
      if (value < OneSixth)
      {
        value = p + ((q - p) * 6F * value);
      }
      else if (value < 0.5F)
      {
        value = q;
      }
      else if (value < TwoThirds)
      {
        value = p + ((q - p) * (TwoThirds - value) * 6F);
      }
      else
      {
        value = p;
      }

      return value;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}