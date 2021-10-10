// -----------------------------------------------------------------------
// <copyright file="NumberValidationRule.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace VianaNET.CustomStyles.Validation
{
  using System;
  using System.Globalization;
  using System.Windows.Controls;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class NumberValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (((string)value).Length > 0)
        {
          Double.Parse((String)value);
        }
      }
      catch (Exception)
      {
        return new ValidationResult(false, VianaNET.Localization.Labels.CalibrationLengthErrorHint);
      }

      return new ValidationResult(true, null);
    }
  }
}