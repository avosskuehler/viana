// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitDoubleStringConverter.cs" company="Freie Universität Berlin">
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
//   The unit double string converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.CustomStyles.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   The unit double string converter.
  /// </summary>
  [ValueConversion(typeof(double), typeof(String))]
  public class UnitDoubleStringConverter : IValueConverter
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
      double valueToConvert = 0;
      if (value is double)
      {
        valueToConvert = (double)value;
      }
      else if (value is int)
      {
        valueToConvert = (int)value;
      }
      else if (value is long)
      {
        valueToConvert = (long)value;
      }
      else if (value == null)
      {
        return string.Empty;
      }

      var param = (MeasurementType)parameter;

      string unit = " " + Project.Instance.CalibrationData.PositionUnit;
      string formatting = "N2";
      switch (param)
      {
        case MeasurementType.Time:
          unit = " " + Project.Instance.CalibrationData.TimeUnit;
          formatting = "N0";
          break;
        case MeasurementType.Pixel:
          unit = " " + Project.Instance.CalibrationData.PixelUnit;
          formatting = "N0";
          break;
        case MeasurementType.Position:
          unit = " " + Project.Instance.CalibrationData.PositionUnit;
          formatting = "N2";
          break;
        case MeasurementType.Velocity:
          unit = " " + Project.Instance.CalibrationData.VelocityUnit;
          formatting = "N2";
          break;
        case MeasurementType.Acceleration:
          unit = " " + Project.Instance.CalibrationData.AccelerationUnit;
          formatting = "N2";
          break;
      }

      if (Project.Instance.CalibrationData.IsShowingUnits)
      {
        return valueToConvert.ToString(formatting) + unit;
      }

      return valueToConvert.ToString(formatting);
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
      var valueToConvertBack = (string)value;
      return double.Parse(valueToConvertBack);
    }

    #endregion
  }
}