// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XYSample.cs" company="Freie Universität Berlin">
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
//   The data sample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Collections
{
  using System;

  /// <summary>
  ///   The data sample.
  /// </summary>
  public class XYSample : IComparable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="XYSample"/> class.
    /// This parameterless constructor is needed for serialization
    /// </summary>
    public XYSample()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XYSample"/> class.
    /// </summary>
    /// <param name="valueX"> The value x. </param>
    /// <param name="valueY">The value y.</param>
    public XYSample(double valueX, double valueY)
    {
      this.ValueX = valueX;
      this.ValueY = valueY;
    }

    /// <summary>
    ///   Gets or sets the x value for this data sample.
    /// </summary>
    public double ValueX { get; set; }

    /// <summary>
    ///   Gets or sets the y value for this data sample.
    /// </summary>
    public double ValueY { get; set; }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      if (obj is XYSample otherSample)
      {
        return otherSample.ValueX.CompareTo(this.ValueX);
      }

      throw new ArgumentException("Object is not a XYSample");
    }

  }
}