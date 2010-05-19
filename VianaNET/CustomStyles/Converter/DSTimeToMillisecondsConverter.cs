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
