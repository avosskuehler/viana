// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentToDoubleConverter.cs" company="Freie Universität Berlin">
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
//   The percent to double converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  ///   The percent to double converter.
  /// </summary>
  [ValueConversion(typeof(double), typeof(double))]
  public class PercentToDoubleConverter : IValueConverter
  {
    #region Public Methods and Operators

    /// <summary>
    /// The convert.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="targetType">
    /// The target type.
    /// </param>
    /// <param name="parameter">
    /// The parameter.
    /// </param>
    /// <param name="culture">
    /// The culture.
    /// </param>
    /// <returns>
    /// The <see cref="object"/> .
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var valueToConvert = (double)value;
      return valueToConvert * 100d;
    }

    /// <summary>
    /// The convert back.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="targetType">
    /// The target type.
    /// </param>
    /// <param name="parameter">
    /// The parameter.
    /// </param>
    /// <param name="culture">
    /// The culture.
    /// </param>
    /// <returns>
    /// The <see cref="object"/> .
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var valueToConvertBack = (double)value;
      return valueToConvertBack / 100d;
    }

    #endregion
  }
}