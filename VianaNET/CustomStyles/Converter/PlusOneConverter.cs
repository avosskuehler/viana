using System;
using System.Globalization;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(int), typeof(int))]
  public class PlusOneConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int valueToConvert = (int)value;
      return valueToConvert + 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int valueToConvertBack = (int)value;
      return valueToConvertBack - 1;
    }
  }
}
