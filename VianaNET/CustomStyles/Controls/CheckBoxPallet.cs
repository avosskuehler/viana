// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckBoxPallet.cs" company="Freie Universität Berlin">
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
//   The check box pallet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Controls
{
  using System.Windows.Media;

  using VianaNET.CustomStyles.Colors;

  #region CheckBoxPallet

  /// <summary>
  ///   The check box pallet.
  /// </summary>
  public static class CheckBoxPallet
  {
    #region Static Fields

    /// <summary>
    ///   The disabled border.
    /// </summary>
    public static Color DisabledBorder;

    /// <summary>
    ///   The disabled foreground.
    /// </summary>
    public static Color DisabledForeground;

    /// <summary>
    ///   The disabled internal border.
    /// </summary>
    public static Color DisabledInternalBorder;

    /// <summary>
    ///   The foreground.
    /// </summary>
    public static Color Foreground;

    /// <summary>
    ///   The hight light internal border.
    /// </summary>
    public static Color HightLightInternalBorder;

    /// <summary>
    ///   The internal high light backgroung 1.
    /// </summary>
    public static Color InternalHighLightBackgroung1;

    /// <summary>
    ///   The internal high light backgroung 2.
    /// </summary>
    public static Color InternalHighLightBackgroung2;

    /// <summary>
    ///   The internal normal backgroung 1.
    /// </summary>
    public static Color InternalNormalBackgroung1;

    /// <summary>
    ///   The internal normal backgroung 2.
    /// </summary>
    public static Color InternalNormalBackgroung2;

    /// <summary>
    ///   The internal normal border.
    /// </summary>
    public static Color InternalNormalBorder;

    /// <summary>
    ///   The mouse over border.
    /// </summary>
    public static Color MouseOverBorder;

    /// <summary>
    ///   The normal border.
    /// </summary>
    public static Color NormalBorder;

    /// <summary>
    ///   The normalbackground.
    /// </summary>
    public static Color Normalbackground;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="CheckBoxPallet" /> class.
    /// </summary>
    static CheckBoxPallet()
    {
      Reset();
      OfficeColors.RegistersTypes.Add(typeof(CheckBoxPallet));
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset.
    /// </summary>
    public static void Reset()
    {
      Foreground = OfficeColors.Foreground.OfficeColor4;
      NormalBorder = OfficeColors.Background.OfficeColor6;
      Normalbackground = OfficeColors.Background.OfficeColor85;
      MouseOverBorder = OfficeColors.Background.OfficeColor6;

      InternalNormalBorder = OfficeColors.Background.OfficeColor83;

      HightLightInternalBorder = OfficeColors.HighLight.OfficeColor12;

      InternalNormalBackgroung1 = OfficeColors.Background.OfficeColor84;
      InternalNormalBackgroung2 = OfficeColors.Background.OfficeColor85;

      InternalHighLightBackgroung1 = OfficeColors.HighLight.OfficeColor15;
      InternalHighLightBackgroung2 = OfficeColors.HighLight.OfficeColor15;
      InternalHighLightBackgroung2.A = 0;

      DisabledForeground = OfficeColors.Disabled.OfficeColor3;
      DisabledBorder = OfficeColors.Disabled.OfficeColor4;
      DisabledInternalBorder = OfficeColors.Disabled.OfficeColor1;
    }

    #endregion
  }

  #endregion
}