using System;
using System.Globalization;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(double), typeof(string))]
  public class DoubleToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double valueToConvert = (double)value;
      return valueToConvert.ToString("N2");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string valueToConvertBack = (string)value;
      return Double.Parse(valueToConvertBack);
    }
  }
}
