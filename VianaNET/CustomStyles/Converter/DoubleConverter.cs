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
  [ValueConversion(typeof(double), typeof(string))]
  public class DoubleConverter : IValueConverter
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
