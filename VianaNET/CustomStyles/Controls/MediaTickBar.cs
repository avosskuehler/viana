
namespace VianaNET
{
  using System.Windows.Controls;
  using System;
  using System.Windows.Input;
  using System.Windows;
  using System.Windows.Controls.Primitives;
  using System.Windows.Media;
  using System.Globalization;

  /// <summary>
  /// Interaction logic for MediaSlider.xaml
  /// </summary>
  public class MediaTickBar : TickBar
  {
    protected override void OnRender(DrawingContext dc)
    {
      Size size = new Size(base.ActualWidth, base.ActualHeight);
      double num = this.Maximum - this.Minimum;
      Point point = new Point(0, 0);
      Point point2 = new Point(0, 0);
      dc.DrawRectangle(Brushes.Red, null, new Rect(size));
      double y = this.ReservedSpace * 0.5;
      FormattedText formattedText = null;
      double x = 0;
      for (int i = 0; i <= num; i += 10)
      {
        formattedText = new FormattedText(
    i.ToString(),
    CultureInfo.GetCultureInfo("en-us"),
    FlowDirection.LeftToRight,
    new Typeface("Verdana"),
    8,
    Brushes.Black);
        if (Minimum == i)
          x = 0;
        else
          x += this.ActualWidth / 10;

        dc.DrawText(formattedText, new Point(x, 10));
      }
    }
  }
}