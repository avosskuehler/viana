using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(int), typeof(Visibility))]
  public class NumberOfObjectsToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int valueToConvert = (int)value;
      return valueToConvert > 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Visibility valueToConvertBack = (Visibility)value;
      if (valueToConvertBack == Visibility.Visible)
      {
        return 3;
      }
      else
      {
        return 1;
      }
    }
  }
}
