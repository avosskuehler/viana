using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(double), typeof(String))]
  public class UnitDoubleStringConverter : IValueConverter
  {
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

      MeasurementType param = (MeasurementType)parameter;

      string unit = " " + Calibration.Instance.PositionUnit.ToString();
      string formatting = "N2";
      switch (param)
      {
        case MeasurementType.Time:
          unit = " " + Calibration.Instance.TimeUnit.ToString();
          formatting = "N0";
          break;
        case MeasurementType.Pixel:
          unit = " " + Calibration.Instance.PixelUnit.ToString();
          formatting = "N0";
          break;
        case MeasurementType.Position:
          unit = " " + Calibration.Instance.PositionUnit.ToString();
          formatting = "N2";
          break;
        case MeasurementType.Velocity:
          unit = " " + Calibration.Instance.VelocityUnit.ToString();
          formatting = "N2";
          break;
        case MeasurementType.Acceleration:
          unit = " " + Calibration.Instance.AccelerationUnit.ToString();
          formatting = "N2";
          break;
      }

      if (Calibration.Instance.IsShowingUnits)
      {
        return valueToConvert.ToString(formatting) + unit;
      }
      else
      {
        return valueToConvert.ToString(formatting);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string valueToConvertBack = (string)value;
      return double.Parse(valueToConvertBack);
    }
  }
}
