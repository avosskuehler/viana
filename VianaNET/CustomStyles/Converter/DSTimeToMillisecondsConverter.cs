using System;
using System.Globalization;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(long), typeof(double))]
  public class DSTimeToMillisecondsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      long valueToConvert = (long)value;
      return (double)(valueToConvert / 10000d);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double valueToConvertBack = (double)value;
      return (long)(valueToConvertBack * 10000);
    }
  }
}
