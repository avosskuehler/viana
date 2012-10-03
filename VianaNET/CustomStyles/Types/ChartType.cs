// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartType.cs" company="Freie Universität Berlin">
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
//   The axis type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Types
{
  /// <summary>
  ///   The chart type.
  /// </summary>
  public enum ChartType
  {
    /// <summary>
    ///   The yover x.
    /// </summary>
    YoverX, 

    /// <summary>
    ///   The xover t.
    /// </summary>
    XoverT, 

    /// <summary>
    ///   The sover t.
    /// </summary>
    SoverT, 

    /// <summary>
    ///   The yover t.
    /// </summary>
    YoverT, 

    /// <summary>
    ///   The vover t.
    /// </summary>
    VoverT, 

    /// <summary>
    ///   The v xover t.
    /// </summary>
    VXoverT, 

    /// <summary>
    ///   The v yover t.
    /// </summary>
    VYoverT, 

    /// <summary>
    ///   The aover t.
    /// </summary>
    AoverT, 

    /// <summary>
    ///   The a xover t.
    /// </summary>
    AXoverT, 

    /// <summary>
    ///   The a yover t.
    /// </summary>
    AYoverT, 

    /// <summary>
    ///   The vover d.
    /// </summary>
    VoverD, 

    /// <summary>
    ///   The v xover dx.
    /// </summary>
    VXoverDX, 

    /// <summary>
    ///   The v yover dy.
    /// </summary>
    VYoverDY, 

    /// <summary>
    ///   The vover s.
    /// </summary>
    VoverS, 

    /// <summary>
    ///   The v xover sx.
    /// </summary>
    VXoverSX, 

    /// <summary>
    ///   The v yover sy.
    /// </summary>
    VYoverSY, 

    /// <summary>
    ///   The aover v.
    /// </summary>
    AoverV, 

    /// <summary>
    ///   The a xover vx.
    /// </summary>
    AXoverVX, 

    /// <summary>
    ///   The a yover vy.
    /// </summary>
    AYoverVY
  }
}