﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorType.cs" company="Freie Universität Berlin">
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
//   Enumerates different color types that can
//   be HSL or HSV.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Colors
{
  /// <summary>
  ///   Enumerates different color types that can
  ///   be HSL or HSV.
  /// </summary>
  public enum ColorType
  {
    /// <summary>
    ///   Indicates HSL color. (Hue, Saturation, Luminance)
    /// </summary>
    HSL, 

    /// <summary>
    ///   Indicates HSV color. (Hue, Saturation, V??)
    /// </summary>
    HSV
  }
}