// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadioButtonPallet.cs" company="Freie Universität Berlin">
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
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   The radio button pallet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System.Windows.Media;

  using VianaNET.CustomStyles.Colors;

  #region RadioButtonPallet

  /// <summary>
  ///   The radio button pallet.
  /// </summary>
  public static class RadioButtonPallet
  {
    #region Static Fields

    /// <summary>
    ///   The disabled background.
    /// </summary>
    public static Color DisabledBackground;

    /// <summary>
    ///   The disabled border.
    /// </summary>
    public static Color DisabledBorder;

    /// <summary>
    ///   The disabled foreground.
    /// </summary>
    public static Color DisabledForeground;

    /// <summary>
    ///   The glyph.
    /// </summary>
    public static Color Glyph;

    /// <summary>
    ///   The light background 1.
    /// </summary>
    public static Color LightBackground1;

    /// <summary>
    ///   The light background 2.
    /// </summary>
    public static Color LightBackground2;

    /// <summary>
    ///   The normal background 1.
    /// </summary>
    public static Color NormalBackground1;

    /// <summary>
    ///   The normal background 2.
    /// </summary>
    public static Color NormalBackground2;

    /// <summary>
    ///   The normal border.
    /// </summary>
    public static Color NormalBorder;

    /// <summary>
    ///   The plus light background 1.
    /// </summary>
    public static Color PlusLightBackground1;

    /// <summary>
    ///   The plus light background 2.
    /// </summary>
    public static Color PlusLightBackground2;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="RadioButtonPallet" /> class.
    /// </summary>
    static RadioButtonPallet()
    {
      Reset();
      OfficeColors.RegistersTypes.Add(typeof(RadioButtonPallet));
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset.
    /// </summary>
    public static void Reset()
    {
      NormalBorder = OfficeColors.Background.OfficeColor82;
      Glyph = OfficeColors.Foreground.OfficeColor3;

      NormalBackground1 = OfficeColors.Background.OfficeColor84;
      NormalBackground2 = OfficeColors.Background.OfficeColor85;

      LightBackground1 = OfficeColors.HighLight.OfficeColor12;
      LightBackground2 = OfficeColors.Background.OfficeColor85;

      PlusLightBackground1 = OfficeColors.HighLight.OfficeColor11;
      PlusLightBackground2 = OfficeColors.Background.OfficeColor85;

      DisabledBorder = OfficeColors.Disabled.OfficeColor3;
      DisabledBackground = OfficeColors.Disabled.OfficeColor2;
      DisabledForeground = OfficeColors.Disabled.OfficeColor3;
    }

    #endregion
  }

  #endregion
}