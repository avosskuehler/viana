﻿// -----------------------------------------------------------------------
// <copyright file="NumberValidationRule.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace VianaNET.CustomStyles.Validation
{
  using System;
  using System.Globalization;
  using System.Windows.Controls;

  using VianaNET.Localization;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class NumberValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      double parameter = 0;

      try
      {
        if (((string)value).Length > 0)
        {
          Double.Parse((String)value);
        }
      }
      catch (Exception e)
      {
        return new ValidationResult(false, Labels.CalibrationLengthErrorHint);
      }

      return new ValidationResult(true, null);
    }
  }
}