﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VianaNET
{
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class InverseBooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool valueToConvert = (bool)value;
      return valueToConvert ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Visibility valueToConvertBack = (Visibility)value;
      if (valueToConvertBack == Visibility.Visible)
      {
        return false;
      }
      else
      {
        return true;
      }
    }
  }
}
