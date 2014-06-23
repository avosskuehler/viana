// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionCalcTree.cs" company="Freie Universität Berlin">
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
// <summary>
//   The calculator function term.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Theory
{
  /// <summary>
  ///   calculator tree for a function term.
  /// </summary>
  public class FunctionCalcTree
  {
    #region Fields

    /// <summary>
    ///   Gets or sets the left functionCalcTree.
    /// </summary>
    public FunctionCalcTree Li;

    /// <summary>
    ///   Gets or sets the right functionCalcTree.
    /// </summary>
    public FunctionCalcTree Re;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the symbol of operator.
    /// </summary>
    public symTyp Cwert { get; set; }

    /// <summary>
    ///   Gets or sets the string, containing the  term.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   Gets or sets the number of a variable.
    /// </summary>
    public ushort Nr { get; set; }

    /// <summary>
    ///   Gets or sets the value, when node contains a number or constant.
    /// </summary>
    public double Zwert { get; set; }

    #endregion
  }
}