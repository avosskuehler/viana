// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberValidationRule.cs" company="Freie Universität Berlin">
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
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Validation
{
  using System;
  using System.Globalization;
  using System.Windows.Controls;

  using VianaNET.Resources;

  /// <summary>
  ///   TODO: Update summary.
  /// </summary>
  public class NumberValidationRule : ValidationRule
  {
    #region Public Methods and Operators

    /// <summary>
    /// The validate.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="cultureInfo">
    /// The culture info.
    /// </param>
    /// <returns>
    /// The <see cref="ValidationResult"/>.
    /// </returns>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (((string)value).Length > 0)
        {
          double.Parse((String)value);
        }
      }
      catch (Exception)
      {
        return new ValidationResult(false, Labels.CalibrationLengthErrorHint);
      }

      return new ValidationResult(true, null);
    }

    #endregion
  }
}