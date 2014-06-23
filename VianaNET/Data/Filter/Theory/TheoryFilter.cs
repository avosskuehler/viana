// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TheoryFilter.cs" company="Freie Universit�t Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Vo�k�hler  
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
// <author>Herwig Niemeyer</author>
// <email>hn_muenster@web.de</email>
// <summary>
//   This class is a filter implementing FilterBase which is used to
//   display a theoretical function in the ChartWindow.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Theory
{
  using System;
  using System.Xml.Serialization;

  using VianaNET.Application;

  /// <summary>
  ///   This class is a filter implementing FilterBase which is used to
  ///   display a theoretical function in the ChartWindow.
  /// </summary>
  public class TheoryFilter : FilterBase
  {
    #region Fields

    /// <summary>
    ///   The theoretical function calculator tree
    /// </summary>
    private FunctionCalcTree theoreticalFunctionCalculatorTree;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the theoretical function as a calculator tree
    /// </summary>
    public FunctionCalcTree TheoreticalFunctionCalculatorTree
    {
      get
      {
        return this.theoreticalFunctionCalculatorTree;
      }

      set
      {
        this.theoreticalFunctionCalculatorTree = value;
        this.CalculateFilterValues();
        Viana.Project.CurrentFilterData.NotifyTheoryTermChange();
      }
    }

    /// <summary>
    ///   Gets or sets the theory funktion.
    /// </summary>
    [XmlIgnore]
    public Func<double, double> TheoryFunction { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Calculates the theoretical function sample values
    /// </summary>
    public override void CalculateFilterValues()
    {
      var parser = new Parse();
      this.TheoryFunction = x => parser.FreierFktWert(this.TheoreticalFunctionCalculatorTree, x);
    }

    #endregion
  }
}