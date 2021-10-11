// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HSXColor.cs" company="Freie Universität Berlin">
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
//   Contains a HSL or HSV color value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Colors
{
  /// <summary>
  ///   Contains a HSL or HSV color value.
  /// </summary>
  public struct HSXColor
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   The hue value of the current HSXColor.
    /// </summary>
    private float? hue;

    /// <summary>
    ///   The saturation value of the current HSXColor
    /// </summary>
    private float saturation;

    /// <summary>
    ///   The X value for the current HSXColor
    /// </summary>
    private float valueLuminanceBrightness;



    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Initializes a new instance of the HSXColor struct.
    ///   Clone constructor
    /// </summary>
    /// <param name="clone">
    /// The <see cref="HSXColor"/> to clone. 
    /// </param>
    public HSXColor(HSXColor clone)
      : this(clone.Hue, clone.Saturation, clone.ValueLuminanceBrightness)
    {
    }

    /// <summary>
    /// Initializes a new instance of the HSXColor struct.
    /// </summary>
    /// <param name="hue">
    /// A <see cref="float"/> hue value or null. 
    /// </param>
    /// <param name="saturation">
    /// The saturation value 
    /// </param>
    /// <param name="x">
    /// The luminance or v value for the color. 
    /// </param>
    public HSXColor(float? hue, float saturation, float x)
    {
      this.hue = null;
      this.saturation = 0;
      this.valueLuminanceBrightness = 0;

      this.Hue = hue;
      this.Saturation = saturation;
      this.ValueLuminanceBrightness = x;
    }



    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Gets or sets the hue value of this HSXColor
    /// </summary>
    public float? Hue
    {
      get => this.hue;

      set
      {
        if (value == null && this.hue != null)
        {
          this.hue = null;
        }
        else
        {
          if (this.hue == null)
          {
            this.hue = value;
          }
          else if (this.hue != value)
          {
            if (value < 0)
            {
              this.hue = value % 1F + 1;
            }
            else
            {
              this.hue = value % 1F;
            }
          }
        }
      }
    }

    /// <summary>
    ///   Gets or sets the saturation of the current HSXColor.
    /// </summary>
    public float Saturation
    {
      get => this.saturation;

      set => this.saturation = value;
    }

    /// <summary>
    ///   Gets or sets Value/Luminance/Brightness
    /// </summary>
    public float ValueLuminanceBrightness
    {
      get => this.valueLuminanceBrightness;

      set => this.valueLuminanceBrightness = value;
    }



    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Overridden <see cref="object.ToString()" />.
    /// </summary>
    /// <returns> A human readable string for this HSXColor. </returns>
    public override string ToString()
    {
      return string.Format(
        "Hue: {0}, Saturation: {1:P}, X: {2:P}", 
        this.hue.HasValue ? this.hue.Value.ToString("P") : "NULL", 
        this.Saturation, 
        this.ValueLuminanceBrightness);
    }



    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}