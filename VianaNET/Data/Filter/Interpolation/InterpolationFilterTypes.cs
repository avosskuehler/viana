// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FitTypes.cs" company="Freie Universität Berlin">
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
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Filter.Interpolation
{
  /// <summary>
  ///   Enumerates the available interpolation filter types
  /// </summary>
  public enum InterpolationFilterTypes
  {
    /// <summary>
    ///   Describes the moving average filter which averages 
    ///   a specific amount of surrounding sample values.
    /// </summary>
    MovingAverage = 1,

    /// <summary>
    ///   Describes the exponential smoothing algorithm.
    /// </summary>
    ExponentialSmooth = 2,
  }
}
