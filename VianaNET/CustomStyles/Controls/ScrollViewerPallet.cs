// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollViewerPallet.cs" company="Freie Universität Berlin">
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
//   The scroll viewer pallet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System.Windows.Media;

  using VianaNET.CustomStyles.Colors;

  #region ScrollViewerPallet

  /// <summary>
  ///   The scroll viewer pallet.
  /// </summary>
  public static class ScrollViewerPallet
  {
    #region Static Fields

    /// <summary>
    ///   The normal border brush.
    /// </summary>
    public static Color NormalBorderBrush;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="ScrollViewerPallet" /> class.
    /// </summary>
    static ScrollViewerPallet()
    {
      Reset();
      OfficeColors.RegistersTypes.Add(typeof(ScrollViewerPallet));
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset.
    /// </summary>
    public static void Reset()
    {
      NormalBorderBrush = OfficeColors.Background.OfficeColor82;
    }

    #endregion
  }

  #endregion
}