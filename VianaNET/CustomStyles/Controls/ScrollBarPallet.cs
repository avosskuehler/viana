// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollBarPallet.cs" company="Freie Universität Berlin">
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
//   The scroll bar pallet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System.Windows.Media;

  using VianaNET.CustomStyles.Colors;

  #region ScrollBarPallet

  /// <summary>
  ///   The scroll bar pallet.
  /// </summary>
  public static class ScrollBarPallet
  {


    /// <summary>
    ///   The scroll bar bg 1.
    /// </summary>
    public static Color ScrollBarBg1;

    /// <summary>
    ///   The scroll bar bg 2.
    /// </summary>
    public static Color ScrollBarBg2;

    /// <summary>
    ///   The scroll bar btn bg mouse over 1.
    /// </summary>
    public static Color ScrollBarBtnBgMouseOver1;

    /// <summary>
    ///   The scroll bar btn bg mouse over 2.
    /// </summary>
    public static Color ScrollBarBtnBgMouseOver2;

    /// <summary>
    ///   The scroll bar btn bg mouse over 3.
    /// </summary>
    public static Color ScrollBarBtnBgMouseOver3;

    /// <summary>
    ///   The scroll bar btn bg mouse over 4.
    /// </summary>
    public static Color ScrollBarBtnBgMouseOver4;

    /// <summary>
    ///   The scroll bar btn bg pressed 1.
    /// </summary>
    public static Color ScrollBarBtnBgPressed1;

    /// <summary>
    ///   The scroll bar btn bg pressed 2.
    /// </summary>
    public static Color ScrollBarBtnBgPressed2;

    /// <summary>
    ///   The scroll bar btn bg pressed 3.
    /// </summary>
    public static Color ScrollBarBtnBgPressed3;

    /// <summary>
    ///   The scroll bar btn bg pressed 4.
    /// </summary>
    public static Color ScrollBarBtnBgPressed4;

    /// <summary>
    ///   The scroll bar normal border brush.
    /// </summary>
    public static Color ScrollBarNormalBorderBrush;

    /// <summary>
    ///   The scroll bar thumb background 1.
    /// </summary>
    public static Color ScrollBarThumbBackground1;

    /// <summary>
    ///   The scroll bar thumb background 2.
    /// </summary>
    public static Color ScrollBarThumbBackground2;

    /// <summary>
    ///   The scroll bar thumb background 3.
    /// </summary>
    public static Color ScrollBarThumbBackground3;

    /// <summary>
    ///   The scroll bar thumb background 4.
    /// </summary>
    public static Color ScrollBarThumbBackground4;

    /// <summary>
    ///   The scroll bar thumb over brush 1.
    /// </summary>
    public static Color ScrollBarThumbOverBrush1;

    /// <summary>
    ///   The scroll bar thumb over brush 2.
    /// </summary>
    public static Color ScrollBarThumbOverBrush2;

    /// <summary>
    ///   The scroll bar thumb over brush 3.
    /// </summary>
    public static Color ScrollBarThumbOverBrush3;

    /// <summary>
    ///   The scroll bar thumb over brush 4.
    /// </summary>
    public static Color ScrollBarThumbOverBrush4;

    /// <summary>
    ///   The scroll bar thumb pressed brush 1.
    /// </summary>
    public static Color ScrollBarThumbPressedBrush1;

    /// <summary>
    ///   The scroll bar thumb pressed brush 2.
    /// </summary>
    public static Color ScrollBarThumbPressedBrush2;

    /// <summary>
    ///   The scroll bar thumb pressed brush 3.
    /// </summary>
    public static Color ScrollBarThumbPressedBrush3;

    /// <summary>
    ///   The scroll bar thumb pressed brush 4.
    /// </summary>
    public static Color ScrollBarThumbPressedBrush4;





    /// <summary>
    ///   Initializes static members of the <see cref="ScrollBarPallet" /> class.
    /// </summary>
    static ScrollBarPallet()
    {
      Reset();
      OfficeColors.RegistersTypes.Add(typeof(ScrollBarPallet));
    }





    /// <summary>
    ///   The reset.
    /// </summary>
    public static void Reset()
    {
      ScrollBarNormalBorderBrush = OfficeColors.Background.OfficeColor73;

      ScrollBarBg1 = OfficeColors.Background.OfficeColor58;
      ScrollBarBg2 = OfficeColors.Background.OfficeColor59;

      ScrollBarBtnBgMouseOver1 = OfficeColors.Background.OfficeColor60;
      ScrollBarBtnBgMouseOver2 = OfficeColors.Background.OfficeColor61;
      ScrollBarBtnBgMouseOver3 = OfficeColors.Background.OfficeColor62;
      ScrollBarBtnBgMouseOver4 = OfficeColors.Background.OfficeColor63;

      ScrollBarBtnBgPressed1 = OfficeColors.Background.OfficeColor64;
      ScrollBarBtnBgPressed2 = OfficeColors.Background.OfficeColor65;
      ScrollBarBtnBgPressed3 = OfficeColors.Background.OfficeColor66;

      ScrollBarBtnBgPressed4 = OfficeColors.Background.OfficeColor67;

      ScrollBarThumbBackground1 = OfficeColors.Background.OfficeColor1;
      ScrollBarThumbBackground2 = OfficeColors.Background.OfficeColor2;
      ScrollBarThumbBackground3 = OfficeColors.Background.OfficeColor3;
      ScrollBarThumbBackground4 = OfficeColors.Background.OfficeColor4;

      ScrollBarThumbOverBrush1 = OfficeColors.Background.OfficeColor74;
      ScrollBarThumbOverBrush2 = OfficeColors.Background.OfficeColor75;
      ScrollBarThumbOverBrush3 = OfficeColors.Background.OfficeColor76;
      ScrollBarThumbOverBrush4 = OfficeColors.Background.OfficeColor77;

      ScrollBarThumbPressedBrush1 = OfficeColors.Background.OfficeColor78;
      ScrollBarThumbPressedBrush2 = OfficeColors.Background.OfficeColor79;
      ScrollBarThumbPressedBrush3 = OfficeColors.Background.OfficeColor80;
      ScrollBarThumbPressedBrush4 = OfficeColors.Background.OfficeColor81;
    }


  }

  #endregion
}