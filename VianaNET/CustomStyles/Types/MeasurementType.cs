// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementType.cs" company="Freie Universität Berlin">
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
//   The measurement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Types
{
  /// <summary>
  ///   The measurement type.
  /// </summary>
  public enum MeasurementType
  {
    /// <summary>
    ///   The time.
    /// </summary>
    Time, 

    /// <summary>
    ///   The pixel.
    /// </summary>
    Pixel, 

    /// <summary>
    ///   The position.
    /// </summary>
    Position, 

    /// <summary>
    ///   The velocity.
    /// </summary>
    Velocity, 

    /// <summary>
    ///   The acceleration.
    /// </summary>
    Acceleration, 
  }
}