// <copyright file="OfficeColors.cs" company="FU Berlin">
// ************************************************************************
// Viana.NET - video analysis for physics education
// Copyright (C) 2010 Dr. Adrian Voßkühler  
// ------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License 
// along with this program; if not, write to the Free Software Foundation, 
// Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian.vosskuehler@fu-berlin.de</email>

namespace VianaNET
{
  using System;
  using System.Windows.Media;
  using System.Collections.Generic;

  /// <summary>
  /// This class encapsulated the office color list.
  /// </summary>
  public static class OfficeColors
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS
    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    static OfficeColors()
    {
      OfficeColors.RegistersTypes = new List<Type>();

      OfficeColors.Pallets = new OfficePallet[] {
			    new OfficePallet(typeof(Background)),
			    new OfficePallet(typeof(Foreground)),
			    new OfficePallet(typeof(HighLight)),
				new OfficePallet(typeof(Disabled))
			    };
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// Gets or sets the array of <see cref="OfficePallet"/> defining this office
    /// color set.
    /// </summary>
    public static OfficePallet[] Pallets { get; set; }

    /// <summary>
    /// Gets or sets the list of types this colors are registered for.
    /// </summary>
    public static List<Type> RegistersTypes { get; set; }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public class Background
    {
      public static OfficePallet Pallet
      {
        get
        {
          return OfficeColors.Pallets[(int)OfficeColorPallet.Background];
        }
      }

      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#e4e6e8");
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#dce0ed");
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#a8c3e0");
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#d3e4f3");
      public static Color OfficeColor5 = (Color)ColorConverter.ConvertFromString("#FF9EBEE7");
      public static Color OfficeColor6 = (Color)ColorConverter.ConvertFromString("#FF567DB1");
      public static Color OfficeColor7 = (Color)ColorConverter.ConvertFromString("#FF6591CD");
      public static Color OfficeColor8 = (Color)ColorConverter.ConvertFromString("#FF97B6DC");
      public static Color OfficeColor9 = (Color)ColorConverter.ConvertFromString("#FFAEC7E8");
      public static Color OfficeColor10 = (Color)ColorConverter.ConvertFromString("#FFAEC7E8");
      public static Color OfficeColor11 = (Color)ColorConverter.ConvertFromString("#FF8DB2E3");
      public static Color OfficeColor12 = (Color)ColorConverter.ConvertFromString("#FFBFDBFF");
      public static Color OfficeColor13 = (Color)ColorConverter.ConvertFromString("#FFA0BEE5");
      public static Color OfficeColor14 = (Color)ColorConverter.ConvertFromString("#FFDFECF7");
      public static Color OfficeColor15 = (Color)ColorConverter.ConvertFromString("#FFD1DFF1");
      public static Color OfficeColor16 = (Color)ColorConverter.ConvertFromString("#FFC7D8ED");
      public static Color OfficeColor17 = (Color)ColorConverter.ConvertFromString("#FFE3F4FD");
      public static Color OfficeColor18 = (Color)ColorConverter.ConvertFromString("#FFA5BDD4");
      public static Color OfficeColor19 = (Color)ColorConverter.ConvertFromString("#FFC0D8F1");
      public static Color OfficeColor20 = (Color)ColorConverter.ConvertFromString("#FF8DB2E3");
      public static Color OfficeColor21 = (Color)ColorConverter.ConvertFromString("#FFC8E0FF");
      public static Color OfficeColor22 = (Color)ColorConverter.ConvertFromString("#FFF3F8FE");
      public static Color OfficeColor23 = (Color)ColorConverter.ConvertFromString("#FFEBF3FE");
      public static Color OfficeColor24 = (Color)ColorConverter.ConvertFromString("#FFDFECF7");
      public static Color OfficeColor25 = (Color)ColorConverter.ConvertFromString("#FFC8E0FF");
      public static Color OfficeColor26 = (Color)ColorConverter.ConvertFromString("#FF8DB2E3");
      public static Color OfficeColor27 = (Color)ColorConverter.ConvertFromString("#FF455D80");
      public static Color OfficeColor28 = (Color)ColorConverter.ConvertFromString("#FFB1C9E8");
      public static Color OfficeColor29 = (Color)ColorConverter.ConvertFromString("#FF3B5A82");
      public static Color OfficeColor30 = (Color)ColorConverter.ConvertFromString("#FFA4C3EB");
      public static Color OfficeColor31 = (Color)ColorConverter.ConvertFromString("#FF97A5B7");
      public static Color OfficeColor32 = (Color)ColorConverter.ConvertFromString("#FFE3EBF6");
      public static Color OfficeColor33 = (Color)ColorConverter.ConvertFromString("#FFDAE9FD");
      public static Color OfficeColor34 = (Color)ColorConverter.ConvertFromString("#FFD5E5FA");
      public static Color OfficeColor35 = (Color)ColorConverter.ConvertFromString("#FFD9E7F9");
      public static Color OfficeColor36 = (Color)ColorConverter.ConvertFromString("#FFCADEF7");
      public static Color OfficeColor37 = (Color)ColorConverter.ConvertFromString("#FFE4EFFD");
      public static Color OfficeColor38 = (Color)ColorConverter.ConvertFromString("#FFDBF4FE");
      public static Color OfficeColor39 = (Color)ColorConverter.ConvertFromString("#FFB0CFF7");
      public static Color OfficeColor40 = (Color)ColorConverter.ConvertFromString("#FFDDE9F8");
      public static Color OfficeColor41 = (Color)ColorConverter.ConvertFromString("#FFC2D9F7");
      public static Color OfficeColor42 = (Color)ColorConverter.ConvertFromString("#FFB0CBEF");
      public static Color OfficeColor43 = (Color)ColorConverter.ConvertFromString("#FFD7E6F9");
      public static Color OfficeColor44 = (Color)ColorConverter.ConvertFromString("#FFCCE0FB");
      public static Color OfficeColor45 = (Color)ColorConverter.ConvertFromString("#FFC5DBF8");
      public static Color OfficeColor46 = (Color)ColorConverter.ConvertFromString("#FFB3D0F5");
      public static Color OfficeColor47 = (Color)ColorConverter.ConvertFromString("#FFD7E5F7");
      public static Color OfficeColor48 = (Color)ColorConverter.ConvertFromString("#FFCDE0F7");
      public static Color OfficeColor49 = (Color)ColorConverter.ConvertFromString("#FFBAD4F7");
      public static Color OfficeColor50 = (Color)ColorConverter.ConvertFromString("#FFC5DCF8");
      public static Color OfficeColor51 = (Color)ColorConverter.ConvertFromString("#FFA9CAF7");
      public static Color OfficeColor52 = (Color)ColorConverter.ConvertFromString("#FF90B6EA");
      public static Color OfficeColor53 = (Color)ColorConverter.ConvertFromString("#FF7495C2");
      public static Color OfficeColor54 = (Color)ColorConverter.ConvertFromString("#FF95ADCE");
      public static Color OfficeColor55 = (Color)ColorConverter.ConvertFromString("#FFCCDEF5");
      public static Color OfficeColor56 = (Color)ColorConverter.ConvertFromString("#FF6086B6");
      public static Color OfficeColor57 = (Color)ColorConverter.ConvertFromString("#FFE5E5E5");
      public static Color OfficeColor58 = (Color)ColorConverter.ConvertFromString("#95b0d2");
      public static Color OfficeColor59 = (Color)ColorConverter.ConvertFromString("#7395c0");
      public static Color OfficeColor60 = (Color)ColorConverter.ConvertFromString("#e4e6e8");
      public static Color OfficeColor61 = (Color)ColorConverter.ConvertFromString("#e2e5ee");
      public static Color OfficeColor62 = (Color)ColorConverter.ConvertFromString("#c2d0df");
      public static Color OfficeColor63 = (Color)ColorConverter.ConvertFromString("#e1e7ef");
      public static Color OfficeColor64 = (Color)ColorConverter.ConvertFromString("#9ebee9");
      public static Color OfficeColor65 = (Color)ColorConverter.ConvertFromString("#abccf6");
      public static Color OfficeColor66 = (Color)ColorConverter.ConvertFromString("#6ca3ec");
      public static Color OfficeColor67 = (Color)ColorConverter.ConvertFromString("#aacbf6");
      public static Color OfficeColor69 = (Color)ColorConverter.ConvertFromString("#e3e5e7");
      public static Color OfficeColor70 = (Color)ColorConverter.ConvertFromString("#e1e5ee");
      public static Color OfficeColor71 = (Color)ColorConverter.ConvertFromString("#c2cfde");
      public static Color OfficeColor72 = (Color)ColorConverter.ConvertFromString("#c9d5e3");
      public static Color OfficeColor73 = (Color)ColorConverter.ConvertFromString("#3c6eb0");
      public static Color OfficeColor74 = (Color)ColorConverter.ConvertFromString("#bed0e8");
      public static Color OfficeColor75 = (Color)ColorConverter.ConvertFromString("#cadffa");
      public static Color OfficeColor76 = (Color)ColorConverter.ConvertFromString("#aacbf6");
      public static Color OfficeColor77 = (Color)ColorConverter.ConvertFromString("#d3e4fa");
      public static Color OfficeColor78 = (Color)ColorConverter.ConvertFromString("#9dbde7");
      public static Color OfficeColor79 = (Color)ColorConverter.ConvertFromString("#a4c7f6");
      public static Color OfficeColor80 = (Color)ColorConverter.ConvertFromString("#6ea6f0");
      public static Color OfficeColor81 = (Color)ColorConverter.ConvertFromString("#b5d1f7");
      public static Color OfficeColor82 = (Color)ColorConverter.ConvertFromString("#9ebcd7");
      public static Color OfficeColor83 = (Color)ColorConverter.ConvertFromString("#a2acb9");
      public static Color OfficeColor84 = (Color)ColorConverter.ConvertFromString("#cacfd5");
      public static Color OfficeColor85 = (Color)ColorConverter.ConvertFromString("#ffffff");
      public static Color RibbonBorderColor = (Color)ColorConverter.ConvertFromString("#BFDBFF");
    }

    public class EditableControlsBackground
    {
      public static OfficePallet Pallet
      {
        get
        {
          return OfficeColors.Pallets[(int)OfficeColorPallet.EditableControlsBackground];
        }
      }

      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#FFFFFF");
    }

    public class Foreground
    {
      public static OfficePallet Pallet
      {
        get
        {
          return OfficeColors.Pallets[(int)OfficeColorPallet.Foreground];
        }
      }

      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#FF15428B");
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#FF7495C2");
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#567db1");
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#4b6c97");
    }

    public class HighLight
    {
      public static OfficePallet Pallet
      {
        get
        {
          return OfficeColors.Pallets[(int)OfficeColorPallet.HighLight];
        }
      }

      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#FFCBBFA4");
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#FFFCCE5F");
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#FFFFFDDE");
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#FFFFE795");
      public static Color OfficeColor5 = (Color)ColorConverter.ConvertFromString("#FFF6CF57");
      public static Color OfficeColor6 = (Color)ColorConverter.ConvertFromString("#FFFFE9A4");
      public static Color OfficeColor7 = (Color)ColorConverter.ConvertFromString("#FFC0A776");
      public static Color OfficeColor8 = (Color)ColorConverter.ConvertFromString("#FFB1905D");
      public static Color OfficeColor9 = (Color)ColorConverter.ConvertFromString("#FFF9BE6B");
      public static Color OfficeColor10 = (Color)ColorConverter.ConvertFromString("#FFFEAE62");
      public static Color OfficeColor11 = (Color)ColorConverter.ConvertFromString("#FFFD983F");
      public static Color OfficeColor12 = (Color)ColorConverter.ConvertFromString("#FFFFD086");
      public static Color OfficeColor13 = (Color)ColorConverter.ConvertFromString("#FF8B7654");
      public static Color OfficeColor14 = (Color)ColorConverter.ConvertFromString("#FFF6CF57");
      public static Color OfficeColor15 = (Color)ColorConverter.ConvertFromString("#FFFBE3A8");
      public static Color OfficeColor16 = (Color)ColorConverter.ConvertFromString("#FFFDF2D5");
      public static Color OfficeColor17 = (Color)ColorConverter.ConvertFromString("#FFFBDF9E");
      public static Color OfficeColor18 = (Color)ColorConverter.ConvertFromString("#FFFDF1D4");
      public static Color OfficeColor19 = (Color)ColorConverter.ConvertFromString("#FFFDE3A0");
      public static Color OfficeColor20 = (Color)ColorConverter.ConvertFromString("#C0A776");
      public static Color OfficeColor21 = (Color)ColorConverter.ConvertFromString("#8B7654");
      public static Color OfficeColor22 = (Color)ColorConverter.ConvertFromString("#ffefbe");
      public static Color OfficeColor23 = (Color)ColorConverter.ConvertFromString("#fff7d8");
    }

    public class Disabled
    {
      public static OfficePallet Pallet
      {
        get
        {
          return OfficeColors.Pallets[(int)OfficeColorPallet.Disabled];
        }
      }

      public static Color OfficeColor1 = (Color)ColorConverter.ConvertFromString("#d5d5d5");
      public static Color OfficeColor2 = (Color)ColorConverter.ConvertFromString("#e7e8e9");
      public static Color OfficeColor3 = (Color)ColorConverter.ConvertFromString("#b3b2b2");
      public static Color OfficeColor4 = (Color)ColorConverter.ConvertFromString("#cccccc");
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER
    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS
    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}