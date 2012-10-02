// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComboBoxPallet.cs" company="Freie Universität Berlin">
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
//   The combo box pallet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Controls
{
  using System.Windows.Media;

  using VianaNET.CustomStyles.Colors;

  #region ComboBoxPallet

  /// <summary>
  ///   The combo box pallet.
  /// </summary>
  public static class ComboBoxPallet
  {
    #region Static Fields

    /// <summary>
    ///   The default control mouse over 1.
    /// </summary>
    public static Color DefaultControlMouseOver1;

    /// <summary>
    ///   The default control mouse over 2.
    /// </summary>
    public static Color DefaultControlMouseOver2;

    /// <summary>
    ///   The default control mouse over 3.
    /// </summary>
    public static Color DefaultControlMouseOver3;

    /// <summary>
    ///   The default control mouse over 4.
    /// </summary>
    public static Color DefaultControlMouseOver4;

    /// <summary>
    ///   The default control pressed 1.
    /// </summary>
    public static Color DefaultControlPressed1;

    /// <summary>
    ///   The default control pressed 2.
    /// </summary>
    public static Color DefaultControlPressed2;

    /// <summary>
    ///   The default control pressed 3.
    /// </summary>
    public static Color DefaultControlPressed3;

    /// <summary>
    ///   The default control pressed 4.
    /// </summary>
    public static Color DefaultControlPressed4;

    /// <summary>
    ///   The default control pressed 5.
    /// </summary>
    public static Color DefaultControlPressed5;

    /// <summary>
    ///   The disable back ground 1.
    /// </summary>
    public static Color DisableBackGround1;

    /// <summary>
    ///   The disable back ground 2.
    /// </summary>
    public static Color DisableBackGround2;

    /// <summary>
    ///   The disabled border.
    /// </summary>
    public static Color DisabledBorder;

    /// <summary>
    ///   The disabled foreground.
    /// </summary>
    public static Color DisabledForeground;

    /// <summary>
    ///   The editable control background.
    /// </summary>
    public static Color EditableControlBackground;

    /// <summary>
    ///   The foreground.
    /// </summary>
    public static Color Foreground;

    /// <summary>
    ///   The glyph.
    /// </summary>
    public static Color Glyph;

    /// <summary>
    ///   The light border.
    /// </summary>
    public static Color LightBorder;

    /// <summary>
    ///   The normal back ground 1.
    /// </summary>
    public static Color NormalBackGround1;

    /// <summary>
    ///   The normal back ground 2.
    /// </summary>
    public static Color NormalBackGround2;

    /// <summary>
    ///   The normal back ground 3.
    /// </summary>
    public static Color NormalBackGround3;

    /// <summary>
    ///   The normal back ground 4.
    /// </summary>
    public static Color NormalBackGround4;

    /// <summary>
    ///   The normal border.
    /// </summary>
    public static Color NormalBorder;

    /// <summary>
    ///   The plus light border.
    /// </summary>
    public static Color PlusLightBorder;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="ComboBoxPallet" /> class.
    /// </summary>
    static ComboBoxPallet()
    {
      Reset();
      OfficeColors.RegistersTypes.Add(typeof(ComboBoxPallet));
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset.
    /// </summary>
    public static void Reset()
    {
      Foreground = OfficeColors.Foreground.OfficeColor1;
      NormalBorder = OfficeColors.Background.OfficeColor82;
      Glyph = OfficeColors.Foreground.OfficeColor3;
      EditableControlBackground = OfficeColors.EditableControlsBackground.OfficeColor1;

      LightBorder = OfficeColors.HighLight.OfficeColor20;
      PlusLightBorder = OfficeColors.HighLight.OfficeColor21;
      DisabledBorder = OfficeColors.Disabled.OfficeColor3;
      DisabledForeground = OfficeColors.Disabled.OfficeColor4;

      NormalBackGround1 = OfficeColors.Background.OfficeColor1;
      NormalBackGround2 = OfficeColors.Background.OfficeColor2;
      NormalBackGround3 = OfficeColors.Background.OfficeColor3;
      NormalBackGround4 = OfficeColors.Background.OfficeColor4;

      DefaultControlMouseOver1 = OfficeColors.HighLight.OfficeColor3;
      DefaultControlMouseOver2 = OfficeColors.HighLight.OfficeColor4;
      DefaultControlMouseOver3 = OfficeColors.HighLight.OfficeColor5;
      DefaultControlMouseOver4 = OfficeColors.HighLight.OfficeColor6;

      DefaultControlPressed1 = OfficeColors.HighLight.OfficeColor8;
      DefaultControlPressed2 = OfficeColors.HighLight.OfficeColor9;
      DefaultControlPressed3 = OfficeColors.HighLight.OfficeColor10;
      DefaultControlPressed4 = OfficeColors.HighLight.OfficeColor11;
      DefaultControlPressed5 = OfficeColors.HighLight.OfficeColor12;

      DisableBackGround1 = OfficeColors.Disabled.OfficeColor1;
      DisableBackGround2 = OfficeColors.Disabled.OfficeColor2;
    }

    #endregion
  }

  #endregion
}