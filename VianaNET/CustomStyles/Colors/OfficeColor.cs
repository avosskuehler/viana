// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfficeColor.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   This class defines an office color.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Colors
{
  using System.Windows.Media;

  /// <summary>
  ///   This class defines an office color.
  /// </summary>
  public class OfficeColor
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   The b.
    /// </summary>
    public byte B;





    /// <summary>
    /// Initializes a new instance of the OfficeColor class.
    /// </summary>
    /// <param name="r">
    /// The red component byte value. 
    /// </param>
    /// <param name="g">
    /// The green component byte value. 
    /// </param>
    /// <param name="b">
    /// The blue component byte value. 
    /// </param>
    /// <param name="pallet">
    /// The <see cref="OfficeColorPallet"/> this color is assigned to. 
    /// </param>
    public OfficeColor(byte r, byte g, byte b, OfficeColorPallet pallet)
      : this(255, r, g, b, pallet)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OfficeColor class.
    /// </summary>
    /// <param name="color">
    /// The color that defines this office color. 
    /// </param>
    /// <param name="pallet">
    /// The <see cref="OfficeColorPallet"/> this color is assigned to. 
    /// </param>
    public OfficeColor(Color color, OfficeColorPallet pallet)
      : this(color.A, color.R, color.G, color.B, pallet)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OfficeColor class
    ///   without using a <see cref="OfficeColorPallet"/>.
    /// </summary>
    /// <param name="c">
    /// The color that defines this office color. 
    /// </param>
    public OfficeColor(Color c)
      : this(c.A, c.R, c.G, c.B, OfficeColorPallet.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OfficeColor class.
    /// </summary>
    /// <param name="a">
    /// The alpha component of the color. 
    /// </param>
    /// <param name="r">
    /// The red component of the color. 
    /// </param>
    /// <param name="g">
    /// The green component of the color. 
    /// </param>
    /// <param name="b">
    /// The blue component of the color. 
    /// </param>
    /// <param name="pallet">
    /// The <see cref="OfficeColorPallet"/> this color is assigned to. 
    /// </param>
    public OfficeColor(byte a, byte r, byte g, byte b, OfficeColorPallet pallet)
    {
      this.Pallet = pallet;
      this.A = a;
      this.R = r;
      this.G = g;
      this.B = b;
    }

    /// <summary>
    /// Initializes a new instance of the OfficeColor class.
    /// </summary>
    /// <param name="color">
    /// The color that defines this office color. 
    /// </param>
    /// <param name="pallet">
    /// The <see cref="OfficeColorPallet"/> this color is assigned to. 
    /// </param>
    public OfficeColor(string color, OfficeColorPallet pallet)
    {
      Color c = (Color)ColorConverter.ConvertFromString(color);
      this.Pallet = pallet;
      this.A = c.A;
      this.R = c.R;
      this.G = c.G;
      this.B = c.B;
    }



    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Gets or sets the a.
    /// </summary>
    public byte A { get; set; }

    /// <summary>
    ///   Gets or sets the g.
    /// </summary>
    public byte G { get; set; }

    /// <summary>
    ///   Gets or sets the pallet.
    /// </summary>
    public OfficeColorPallet Pallet { get; set; }

    /// <summary>
    ///   Gets or sets the r.
    /// </summary>
    public byte R { get; set; }



    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

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

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   The op_ implicit.
    /// </summary>
    /// <param name="oc"> The oc. </param>
    /// <returns> </returns>
    public static implicit operator Color(OfficeColor oc)
    {
      return Color.FromArgb(oc.A, oc.R, oc.G, oc.B);
    }

    /// <summary>
    ///   The op_ implicit.
    /// </summary>
    /// <param name="c"> The c. </param>
    /// <returns> </returns>
    public static implicit operator OfficeColor(Color c)
    {
      return new OfficeColor(c.A, c.R, c.G, c.B, OfficeColorPallet.None);
    }


  }
}