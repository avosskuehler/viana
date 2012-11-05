// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Regression.cs" company="Freie Universität Berlin">
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
//   Defines the Regression type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Filter.Regression
{
  /// <summary>
  /// The regression enumeration
  /// </summary>
  public enum RegressionType
  {
    /// <summary>
    /// Optimale regression
    /// </summary>
    Best = 0,

    /// <summary>
    /// Lineare Regression, mx+n
    /// </summary>
    Linear = 1,

    /// <summary>
    /// Regression mit Exponentialfunktion, a*exp(b*x)
    /// </summary>
    Exponentiell = 2,

    /// <summary>
    /// Regression mit Logarithmusfunktion, a*ln(b*x)
    /// </summary>
    Logarithmisch = 3,

    /// <summary>
    /// Regression mit Potenzfunktion, a*x^b
    /// </summary>
    Potenz = 4,

    /// <summary>
    /// Regression mit quadratischer Funktion, ax²+bx+c
    /// </summary>
    Quadratisch = 5,

    /// <summary>
    /// Regression mit Exponentialfunktion mit Konstante, a*exp(b*x) +c
    /// </summary>
    ExponentiellMitKonstante = 6,

    /// <summary>
    /// Regression mit Sinusfunktion, a*sin(bx+c)+d
    /// </summary>
    Sinus = 7,

    /// <summary>
    /// Regression mit gedämpfter Sinusfunktion, a*sin(bx)*exp(c*x)
    /// </summary>
    SinusGedämpft = 8,

    /// <summary>
    /// Regression mit Resonanzfunktion, a/sqrt(1+b(x-c/x))
    /// </summary>
    Resonanz = 9
  }
}
