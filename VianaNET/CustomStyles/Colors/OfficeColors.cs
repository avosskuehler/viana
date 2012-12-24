// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfficeColors.cs" company="Freie Universität Berlin">
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
//   This class encapsulated the office color list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Colors
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Media;

  /// <summary>
  ///   This class encapsulated the office color list.
  /// </summary>
  public static class OfficeColors
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
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="OfficeColors" /> class.
    /// </summary>
    static OfficeColors()
    {
      RegistersTypes = new List<Type>();

      Pallets = new[]
        {
          new OfficePallet(typeof(Background)), new OfficePallet(typeof(Foreground)), 
          new OfficePallet(typeof(HighLight)), new OfficePallet(typeof(Disabled))
        };
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    ///   Gets or sets the array of <see cref="OfficePallet" /> defining this office
    ///   color set.
    /// </summary>
    public static OfficePallet[] Pallets { get; set; }

    /// <summary>
    ///   Gets or sets the list of types this colors are registered for.
    /// </summary>
    public static List<Type> RegistersTypes { get; set; }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   The background.
    /// </summary>
    public class Background
    {
      #region Static Fields

      /// <summary>
      ///   The office color 1.
      /// </summary>
      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#e4e6e8");

      /// <summary>
      ///   The office color 10.
      /// </summary>
      public static Color OfficeColor10 = (Color)ColorConverter.ConvertFromString("#FFAEC7E8");

      /// <summary>
      ///   The office color 11.
      /// </summary>
      public static Color OfficeColor11 = (Color)ColorConverter.ConvertFromString("#FF8DB2E3");

      /// <summary>
      ///   The office color 12.
      /// </summary>
      public static Color OfficeColor12 = (Color)ColorConverter.ConvertFromString("#FFBFDBFF");

      /// <summary>
      ///   The office color 13.
      /// </summary>
      public static Color OfficeColor13 = (Color)ColorConverter.ConvertFromString("#FFA0BEE5");

      /// <summary>
      ///   The office color 14.
      /// </summary>
      public static Color OfficeColor14 = (Color)ColorConverter.ConvertFromString("#FFDFECF7");

      /// <summary>
      ///   The office color 15.
      /// </summary>
      public static Color OfficeColor15 = (Color)ColorConverter.ConvertFromString("#FFD1DFF1");

      /// <summary>
      ///   The office color 16.
      /// </summary>
      public static Color OfficeColor16 = (Color)ColorConverter.ConvertFromString("#FFC7D8ED");

      /// <summary>
      ///   The office color 17.
      /// </summary>
      public static Color OfficeColor17 = (Color)ColorConverter.ConvertFromString("#FFE3F4FD");

      /// <summary>
      ///   The office color 18.
      /// </summary>
      public static Color OfficeColor18 = (Color)ColorConverter.ConvertFromString("#FFA5BDD4");

      /// <summary>
      ///   The office color 19.
      /// </summary>
      public static Color OfficeColor19 = (Color)ColorConverter.ConvertFromString("#FFC0D8F1");

      /// <summary>
      ///   The office color 2.
      /// </summary>
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#dce0ed");

      /// <summary>
      ///   The office color 20.
      /// </summary>
      public static Color OfficeColor20 = (Color)ColorConverter.ConvertFromString("#FF8DB2E3");

      /// <summary>
      ///   The office color 21.
      /// </summary>
      public static Color OfficeColor21 = (Color)ColorConverter.ConvertFromString("#FFC8E0FF");

      /// <summary>
      ///   The office color 22.
      /// </summary>
      public static Color OfficeColor22 = (Color)ColorConverter.ConvertFromString("#FFF3F8FE");

      /// <summary>
      ///   The office color 23.
      /// </summary>
      public static Color OfficeColor23 = (Color)ColorConverter.ConvertFromString("#FFEBF3FE");

      /// <summary>
      ///   The office color 24.
      /// </summary>
      public static Color OfficeColor24 = (Color)ColorConverter.ConvertFromString("#FFDFECF7");

      /// <summary>
      ///   The office color 25.
      /// </summary>
      public static Color OfficeColor25 = (Color)ColorConverter.ConvertFromString("#FFC8E0FF");

      /// <summary>
      ///   The office color 26.
      /// </summary>
      public static Color OfficeColor26 = (Color)ColorConverter.ConvertFromString("#FF8DB2E3");

      /// <summary>
      ///   The office color 27.
      /// </summary>
      public static Color OfficeColor27 = (Color)ColorConverter.ConvertFromString("#FF455D80");

      /// <summary>
      ///   The office color 28.
      /// </summary>
      public static Color OfficeColor28 = (Color)ColorConverter.ConvertFromString("#FFB1C9E8");

      /// <summary>
      ///   The office color 29.
      /// </summary>
      public static Color OfficeColor29 = (Color)ColorConverter.ConvertFromString("#FF3B5A82");

      /// <summary>
      ///   The office color 3.
      /// </summary>
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#a8c3e0");

      /// <summary>
      ///   The office color 30.
      /// </summary>
      public static Color OfficeColor30 = (Color)ColorConverter.ConvertFromString("#FFA4C3EB");

      /// <summary>
      ///   The office color 31.
      /// </summary>
      public static Color OfficeColor31 = (Color)ColorConverter.ConvertFromString("#FF97A5B7");

      /// <summary>
      ///   The office color 32.
      /// </summary>
      public static Color OfficeColor32 = (Color)ColorConverter.ConvertFromString("#FFE3EBF6");

      /// <summary>
      ///   The office color 33.
      /// </summary>
      public static Color OfficeColor33 = (Color)ColorConverter.ConvertFromString("#FFDAE9FD");

      /// <summary>
      ///   The office color 34.
      /// </summary>
      public static Color OfficeColor34 = (Color)ColorConverter.ConvertFromString("#FFD5E5FA");

      /// <summary>
      ///   The office color 35.
      /// </summary>
      public static Color OfficeColor35 = (Color)ColorConverter.ConvertFromString("#FFD9E7F9");

      /// <summary>
      ///   The office color 36.
      /// </summary>
      public static Color OfficeColor36 = (Color)ColorConverter.ConvertFromString("#FFCADEF7");

      /// <summary>
      ///   The office color 37.
      /// </summary>
      public static Color OfficeColor37 = (Color)ColorConverter.ConvertFromString("#FFE4EFFD");

      /// <summary>
      ///   The office color 38.
      /// </summary>
      public static Color OfficeColor38 = (Color)ColorConverter.ConvertFromString("#FFDBF4FE");

      /// <summary>
      ///   The office color 39.
      /// </summary>
      public static Color OfficeColor39 = (Color)ColorConverter.ConvertFromString("#FFB0CFF7");

      /// <summary>
      ///   The office color 4.
      /// </summary>
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#d3e4f3");

      /// <summary>
      ///   The office color 40.
      /// </summary>
      public static Color OfficeColor40 = (Color)ColorConverter.ConvertFromString("#FFDDE9F8");

      /// <summary>
      ///   The office color 41.
      /// </summary>
      public static Color OfficeColor41 = (Color)ColorConverter.ConvertFromString("#FFC2D9F7");

      /// <summary>
      ///   The office color 42.
      /// </summary>
      public static Color OfficeColor42 = (Color)ColorConverter.ConvertFromString("#FFB0CBEF");

      /// <summary>
      ///   The office color 43.
      /// </summary>
      public static Color OfficeColor43 = (Color)ColorConverter.ConvertFromString("#FFD7E6F9");

      /// <summary>
      ///   The office color 44.
      /// </summary>
      public static Color OfficeColor44 = (Color)ColorConverter.ConvertFromString("#FFCCE0FB");

      /// <summary>
      ///   The office color 45.
      /// </summary>
      public static Color OfficeColor45 = (Color)ColorConverter.ConvertFromString("#FFC5DBF8");

      /// <summary>
      ///   The office color 46.
      /// </summary>
      public static Color OfficeColor46 = (Color)ColorConverter.ConvertFromString("#FFB3D0F5");

      /// <summary>
      ///   The office color 47.
      /// </summary>
      public static Color OfficeColor47 = (Color)ColorConverter.ConvertFromString("#FFD7E5F7");

      /// <summary>
      ///   The office color 48.
      /// </summary>
      public static Color OfficeColor48 = (Color)ColorConverter.ConvertFromString("#FFCDE0F7");

      /// <summary>
      ///   The office color 49.
      /// </summary>
      public static Color OfficeColor49 = (Color)ColorConverter.ConvertFromString("#FFBAD4F7");

      /// <summary>
      ///   The office color 5.
      /// </summary>
      public static Color OfficeColor5 = (Color)ColorConverter.ConvertFromString("#FF9EBEE7");

      /// <summary>
      ///   The office color 50.
      /// </summary>
      public static Color OfficeColor50 = (Color)ColorConverter.ConvertFromString("#FFC5DCF8");

      /// <summary>
      ///   The office color 51.
      /// </summary>
      public static Color OfficeColor51 = (Color)ColorConverter.ConvertFromString("#FFA9CAF7");

      /// <summary>
      ///   The office color 52.
      /// </summary>
      public static Color OfficeColor52 = (Color)ColorConverter.ConvertFromString("#FF90B6EA");

      /// <summary>
      ///   The office color 53.
      /// </summary>
      public static Color OfficeColor53 = (Color)ColorConverter.ConvertFromString("#FF7495C2");

      /// <summary>
      ///   The office color 54.
      /// </summary>
      public static Color OfficeColor54 = (Color)ColorConverter.ConvertFromString("#FF95ADCE");

      /// <summary>
      ///   The office color 55.
      /// </summary>
      public static Color OfficeColor55 = (Color)ColorConverter.ConvertFromString("#FFCCDEF5");

      /// <summary>
      ///   The office color 56.
      /// </summary>
      public static Color OfficeColor56 = (Color)ColorConverter.ConvertFromString("#FF6086B6");

      /// <summary>
      ///   The office color 57.
      /// </summary>
      public static Color OfficeColor57 = (Color)ColorConverter.ConvertFromString("#FFE5E5E5");

      /// <summary>
      ///   The office color 58.
      /// </summary>
      public static Color OfficeColor58 = (Color)ColorConverter.ConvertFromString("#95b0d2");

      /// <summary>
      ///   The office color 59.
      /// </summary>
      public static Color OfficeColor59 = (Color)ColorConverter.ConvertFromString("#7395c0");

      /// <summary>
      ///   The office color 6.
      /// </summary>
      public static Color OfficeColor6 = (Color)ColorConverter.ConvertFromString("#FF567DB1");

      /// <summary>
      ///   The office color 60.
      /// </summary>
      public static Color OfficeColor60 = (Color)ColorConverter.ConvertFromString("#e4e6e8");

      /// <summary>
      ///   The office color 61.
      /// </summary>
      public static Color OfficeColor61 = (Color)ColorConverter.ConvertFromString("#e2e5ee");

      /// <summary>
      ///   The office color 62.
      /// </summary>
      public static Color OfficeColor62 = (Color)ColorConverter.ConvertFromString("#c2d0df");

      /// <summary>
      ///   The office color 63.
      /// </summary>
      public static Color OfficeColor63 = (Color)ColorConverter.ConvertFromString("#e1e7ef");

      /// <summary>
      ///   The office color 64.
      /// </summary>
      public static Color OfficeColor64 = (Color)ColorConverter.ConvertFromString("#9ebee9");

      /// <summary>
      ///   The office color 65.
      /// </summary>
      public static Color OfficeColor65 = (Color)ColorConverter.ConvertFromString("#abccf6");

      /// <summary>
      ///   The office color 66.
      /// </summary>
      public static Color OfficeColor66 = (Color)ColorConverter.ConvertFromString("#6ca3ec");

      /// <summary>
      ///   The office color 67.
      /// </summary>
      public static Color OfficeColor67 = (Color)ColorConverter.ConvertFromString("#aacbf6");

      /// <summary>
      ///   The office color 69.
      /// </summary>
      public static Color OfficeColor69 = (Color)ColorConverter.ConvertFromString("#e3e5e7");

      /// <summary>
      ///   The office color 7.
      /// </summary>
      public static Color OfficeColor7 = (Color)ColorConverter.ConvertFromString("#FF6591CD");

      /// <summary>
      ///   The office color 70.
      /// </summary>
      public static Color OfficeColor70 = (Color)ColorConverter.ConvertFromString("#e1e5ee");

      /// <summary>
      ///   The office color 71.
      /// </summary>
      public static Color OfficeColor71 = (Color)ColorConverter.ConvertFromString("#c2cfde");

      /// <summary>
      ///   The office color 72.
      /// </summary>
      public static Color OfficeColor72 = (Color)ColorConverter.ConvertFromString("#c9d5e3");

      /// <summary>
      ///   The office color 73.
      /// </summary>
      public static Color OfficeColor73 = (Color)ColorConverter.ConvertFromString("#3c6eb0");

      /// <summary>
      ///   The office color 74.
      /// </summary>
      public static Color OfficeColor74 = (Color)ColorConverter.ConvertFromString("#bed0e8");

      /// <summary>
      ///   The office color 75.
      /// </summary>
      public static Color OfficeColor75 = (Color)ColorConverter.ConvertFromString("#cadffa");

      /// <summary>
      ///   The office color 76.
      /// </summary>
      public static Color OfficeColor76 = (Color)ColorConverter.ConvertFromString("#aacbf6");

      /// <summary>
      ///   The office color 77.
      /// </summary>
      public static Color OfficeColor77 = (Color)ColorConverter.ConvertFromString("#d3e4fa");

      /// <summary>
      ///   The office color 78.
      /// </summary>
      public static Color OfficeColor78 = (Color)ColorConverter.ConvertFromString("#9dbde7");

      /// <summary>
      ///   The office color 79.
      /// </summary>
      public static Color OfficeColor79 = (Color)ColorConverter.ConvertFromString("#a4c7f6");

      /// <summary>
      ///   The office color 8.
      /// </summary>
      public static Color OfficeColor8 = (Color)ColorConverter.ConvertFromString("#FF97B6DC");

      /// <summary>
      ///   The office color 80.
      /// </summary>
      public static Color OfficeColor80 = (Color)ColorConverter.ConvertFromString("#6ea6f0");

      /// <summary>
      ///   The office color 81.
      /// </summary>
      public static Color OfficeColor81 = (Color)ColorConverter.ConvertFromString("#b5d1f7");

      /// <summary>
      ///   The office color 82.
      /// </summary>
      public static Color OfficeColor82 = (Color)ColorConverter.ConvertFromString("#9ebcd7");

      /// <summary>
      ///   The office color 83.
      /// </summary>
      public static Color OfficeColor83 = (Color)ColorConverter.ConvertFromString("#a2acb9");

      /// <summary>
      ///   The office color 84.
      /// </summary>
      public static Color OfficeColor84 = (Color)ColorConverter.ConvertFromString("#cacfd5");

      /// <summary>
      ///   The office color 85.
      /// </summary>
      public static Color OfficeColor85 = (Color)ColorConverter.ConvertFromString("#ffffff");

      /// <summary>
      ///   The office color 9.
      /// </summary>
      public static Color OfficeColor9 = (Color)ColorConverter.ConvertFromString("#FFAEC7E8");

      /// <summary>
      ///   The ribbon border color.
      /// </summary>
      public static Color RibbonBorderColor = (Color)ColorConverter.ConvertFromString("#BFDBFF");

      #endregion

      #region Public Properties

      /// <summary>
      ///   Gets the pallet.
      /// </summary>
      public static OfficePallet Pallet
      {
        get
        {
          return Pallets[(int)OfficeColorPallet.Background];
        }
      }

      #endregion
    }

    /// <summary>
    ///   The disabled.
    /// </summary>
    public class Disabled
    {
      #region Static Fields

      /// <summary>
      ///   The office color 1.
      /// </summary>
      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#d5d5d5");

      /// <summary>
      ///   The office color 2.
      /// </summary>
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#e7e8e9");

      /// <summary>
      ///   The office color 3.
      /// </summary>
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#b3b2b2");

      /// <summary>
      ///   The office color 4.
      /// </summary>
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#cccccc");

      #endregion

      #region Public Properties

      /// <summary>
      ///   Gets the pallet.
      /// </summary>
      public static OfficePallet Pallet
      {
        get
        {
          return Pallets[(int)OfficeColorPallet.Disabled];
        }
      }

      #endregion
    }

    /// <summary>
    ///   The editable controls background.
    /// </summary>
    public class EditableControlsBackground
    {
      #region Static Fields

      /// <summary>
      ///   The office color 1.
      /// </summary>
      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#FFFFFF");

      #endregion

      #region Public Properties

      /// <summary>
      ///   Gets the pallet.
      /// </summary>
      public static OfficePallet Pallet
      {
        get
        {
          return Pallets[(int)OfficeColorPallet.EditableControlsBackground];
        }
      }

      #endregion
    }

    /// <summary>
    ///   The foreground.
    /// </summary>
    public class Foreground
    {
      #region Static Fields

      /// <summary>
      ///   The office color 1.
      /// </summary>
      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#FF15428B");

      /// <summary>
      ///   The office color 2.
      /// </summary>
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#FF7495C2");

      /// <summary>
      ///   The office color 3.
      /// </summary>
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#567db1");

      /// <summary>
      ///   The office color 4.
      /// </summary>
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#4b6c97");

      #endregion

      #region Public Properties

      /// <summary>
      ///   Gets the pallet.
      /// </summary>
      public static OfficePallet Pallet
      {
        get
        {
          return Pallets[(int)OfficeColorPallet.Foreground];
        }
      }

      #endregion
    }

    /// <summary>
    ///   The high light.
    /// </summary>
    public class HighLight
    {
      #region Static Fields

      /// <summary>
      ///   The office color 1.
      /// </summary>
      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#FFCBBFA4");

      /// <summary>
      ///   The office color 10.
      /// </summary>
      public static Color OfficeColor10 = (Color)ColorConverter.ConvertFromString("#FFFEAE62");

      /// <summary>
      ///   The office color 11.
      /// </summary>
      public static Color OfficeColor11 = (Color)ColorConverter.ConvertFromString("#FFFD983F");

      /// <summary>
      ///   The office color 12.
      /// </summary>
      public static Color OfficeColor12 = (Color)ColorConverter.ConvertFromString("#FFFFD086");

      /// <summary>
      ///   The office color 13.
      /// </summary>
      public static Color OfficeColor13 = (Color)ColorConverter.ConvertFromString("#FF8B7654");

      /// <summary>
      ///   The office color 14.
      /// </summary>
      public static Color OfficeColor14 = (Color)ColorConverter.ConvertFromString("#FFF6CF57");

      /// <summary>
      ///   The office color 15.
      /// </summary>
      public static Color OfficeColor15 = (Color)ColorConverter.ConvertFromString("#FFFBE3A8");

      /// <summary>
      ///   The office color 16.
      /// </summary>
      public static Color OfficeColor16 = (Color)ColorConverter.ConvertFromString("#FFFDF2D5");

      /// <summary>
      ///   The office color 17.
      /// </summary>
      public static Color OfficeColor17 = (Color)ColorConverter.ConvertFromString("#FFFBDF9E");

      /// <summary>
      ///   The office color 18.
      /// </summary>
      public static Color OfficeColor18 = (Color)ColorConverter.ConvertFromString("#FFFDF1D4");

      /// <summary>
      ///   The office color 19.
      /// </summary>
      public static Color OfficeColor19 = (Color)ColorConverter.ConvertFromString("#FFFDE3A0");

      /// <summary>
      ///   The office color 2.
      /// </summary>
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#FFFCCE5F");

      /// <summary>
      ///   The office color 20.
      /// </summary>
      public static Color OfficeColor20 = (Color)ColorConverter.ConvertFromString("#C0A776");

      /// <summary>
      ///   The office color 21.
      /// </summary>
      public static Color OfficeColor21 = (Color)ColorConverter.ConvertFromString("#8B7654");

      /// <summary>
      ///   The office color 22.
      /// </summary>
      public static Color OfficeColor22 = (Color)ColorConverter.ConvertFromString("#ffefbe");

      /// <summary>
      ///   The office color 23.
      /// </summary>
      public static Color OfficeColor23 = (Color)ColorConverter.ConvertFromString("#fff7d8");

      /// <summary>
      ///   The office color 3.
      /// </summary>
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#FFFFFDDE");

      /// <summary>
      ///   The office color 4.
      /// </summary>
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#FFFFE795");

      /// <summary>
      ///   The office color 5.
      /// </summary>
      public static Color OfficeColor5 = (Color)ColorConverter.ConvertFromString("#FFF6CF57");

      /// <summary>
      ///   The office color 6.
      /// </summary>
      public static Color OfficeColor6 = (Color)ColorConverter.ConvertFromString("#FFFFE9A4");

      /// <summary>
      ///   The office color 7.
      /// </summary>
      public static Color OfficeColor7 = (Color)ColorConverter.ConvertFromString("#FFC0A776");

      /// <summary>
      ///   The office color 8.
      /// </summary>
      public static Color OfficeColor8 = (Color)ColorConverter.ConvertFromString("#FFB1905D");

      /// <summary>
      ///   The office color 9.
      /// </summary>
      public static Color OfficeColor9 = (Color)ColorConverter.ConvertFromString("#FFF9BE6B");

      #endregion

      #region Public Properties

      /// <summary>
      ///   Gets the pallet.
      /// </summary>
      public static OfficePallet Pallet
      {
        get
        {
          return Pallets[(int)OfficeColorPallet.HighLight];
        }
      }

      #endregion
    }

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
  }
}