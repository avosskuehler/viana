using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

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
