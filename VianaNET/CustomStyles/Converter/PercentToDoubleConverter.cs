using System;
using System.Globalization;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(double), typeof(double))]
  public class PercentToDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double valueToConvert = (double)value;
      return (double)(valueToConvert * 100d);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double valueToConvertBack = (double)value;
      return (double)(valueToConvertBack / 100d);
    }
  }
}
