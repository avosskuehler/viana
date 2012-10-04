// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculatorFunctionTerm.cs" company="Freie Universität Berlin">
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
//   The calculator function term.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Linefit
{
  /// <summary>
  /// The calculator function term.
  /// </summary>
  public class CalculatorFunctionTerm
  {
    /// <summary>
    /// Gets or sets the li.
    /// </summary>
    public CalculatorFunctionTerm Li;

    /// <summary>
    /// Gets or sets the re.
    /// </summary>
    public CalculatorFunctionTerm Re;

    /// <summary>
    /// Gets or sets the cwert.
    /// </summary>
    public symTyp Cwert { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the nr.
    /// </summary>
    public ushort Nr { get; set; }



    /// <summary>
    /// Gets or sets the zwert.
    /// </summary>
    public double Zwert { get; set; }
  }
}
