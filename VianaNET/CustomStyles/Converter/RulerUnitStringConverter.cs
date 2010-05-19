using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(double), typeof(String))]
  public class RulerUnitStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double valueToConvert = (double)value;
      string unit = " " + Calibration.Instance.PositionUnit.ToString();
      MeasurementType param = (MeasurementType)parameter;
      switch (param)
      {
        case MeasurementType.Pixel:
          unit = " " + Calibration.Instance.PixelUnit.ToString();
          break;
        case MeasurementType.Position:
          unit = " " + Calibration.Instance.PositionUnit.ToString();
          break;
      }

      string formatting = "N0";
      if (valueToConvert<1)
      {
        formatting = "N2";
      }
      else if (valueToConvert<5)
      {
        formatting = "N1";
      }

      return valueToConvert.ToString(formatting) + unit;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string valueToConvertBack = (string)value;
      return double.Parse(valueToConvertBack);
    }
  }
}
