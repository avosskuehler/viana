// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderStyles.xaml.cs" company="Freie Universität Berlin">
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
//   The slider styles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Styles
{
  using System.Windows.Media;

  using VianaNET.CustomStyles.Colors;

  /// <summary>
  ///   The slider styles.
  /// </summary>
  public static class SliderStyles
  {
    #region Static Fields

    /// <summary>
    ///   The glyph.
    /// </summary>
    public static Color Glyph;

    /// <summary>
    ///   The normal background.
    /// </summary>
    public static Color NormalBackground;

    /// <summary>
    ///   The normal border.
    /// </summary>
    public static Color NormalBorder;

    /// <summary>
    ///   The side buttons background 1.
    /// </summary>
    public static Color SideButtonsBackground1;

    /// <summary>
    ///   The side buttons background 2.
    /// </summary>
    public static Color SideButtonsBackground2;

    /// <summary>
    ///   The side buttons background 3.
    /// </summary>
    public static Color SideButtonsBackground3;

    /// <summary>
    ///   The side buttons background 4.
    /// </summary>
    public static Color SideButtonsBackground4;

    /// <summary>
    ///   The side buttons external border.
    /// </summary>
    public static Color SideButtonsExternalBorder;

    /// <summary>
    ///   The side buttons internal border.
    /// </summary>
    public static Color SideButtonsInternalBorder;

    /// <summary>
    ///   The side buttons light background 1.
    /// </summary>
    public static Color SideButtonsLightBackground1;

    /// <summary>
    ///   The side buttons light background 2.
    /// </summary>
    public static Color SideButtonsLightBackground2;

    /// <summary>
    ///   The side buttons light background 3.
    /// </summary>
    public static Color SideButtonsLightBackground3;

    /// <summary>
    ///   The side buttons light background 4.
    /// </summary>
    public static Color SideButtonsLightBackground4;

    /// <summary>
    ///   The side buttons plus light background 1.
    /// </summary>
    public static Color SideButtonsPlusLightBackground1;

    /// <summary>
    ///   The side buttons plus light background 2.
    /// </summary>
    public static Color SideButtonsPlusLightBackground2;

    /// <summary>
    ///   The side buttons plus light background 3.
    /// </summary>
    public static Color SideButtonsPlusLightBackground3;

    /// <summary>
    ///   The side buttons plus light background 4.
    /// </summary>
    public static Color SideButtonsPlusLightBackground4;

    /// <summary>
    ///   The thumb background 1.
    /// </summary>
    public static Color ThumbBackground1;

    /// <summary>
    ///   The thumb background 2.
    /// </summary>
    public static Color ThumbBackground2;

    /// <summary>
    ///   The thumb background 3.
    /// </summary>
    public static Color ThumbBackground3;

    /// <summary>
    ///   The thumb background 4.
    /// </summary>
    public static Color ThumbBackground4;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="SliderStyles" /> class.
    /// </summary>
    static SliderStyles()
    {
      Reset();
      OfficeColors.RegistersTypes.Add(typeof(SliderStyles));
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset.
    /// </summary>
    public static void Reset()
    {
      Glyph = OfficeColors.Foreground.OfficeColor4;
      NormalBorder = OfficeColors.Background.OfficeColor6;
      NormalBackground = OfficeColors.Background.OfficeColor5;

      SideButtonsExternalBorder = OfficeColors.Background.OfficeColor42;
      SideButtonsInternalBorder = OfficeColors.Background.OfficeColor73;

      SideButtonsBackground1 = OfficeColors.Background.OfficeColor85;
      SideButtonsBackground2 = OfficeColors.Background.OfficeColor74;
      SideButtonsBackground3 = OfficeColors.Background.OfficeColor53;
      SideButtonsBackground4 = OfficeColors.Background.OfficeColor85;

      ThumbBackground1 = OfficeColors.Background.OfficeColor85;
      ThumbBackground2 = OfficeColors.Background.OfficeColor66;
      ThumbBackground3 = OfficeColors.Background.OfficeColor65;
      ThumbBackground4 = OfficeColors.Background.OfficeColor85;

      SideButtonsLightBackground1 = OfficeColors.Background.OfficeColor85;
      SideButtonsLightBackground2 = OfficeColors.HighLight.OfficeColor10;
      SideButtonsLightBackground3 = OfficeColors.HighLight.OfficeColor5;
      SideButtonsLightBackground4 = OfficeColors.Background.OfficeColor85;

      SideButtonsPlusLightBackground1 = OfficeColors.Background.OfficeColor85;
      SideButtonsPlusLightBackground2 = OfficeColors.HighLight.OfficeColor11;
      SideButtonsPlusLightBackground3 = OfficeColors.HighLight.OfficeColor10;
      SideButtonsPlusLightBackground4 = OfficeColors.Background.OfficeColor85;
    }

    #endregion
  }
}