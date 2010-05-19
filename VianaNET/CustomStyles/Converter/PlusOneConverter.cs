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
