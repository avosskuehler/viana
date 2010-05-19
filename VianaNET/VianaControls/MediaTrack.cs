
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
  public class MediaTrack : Track
  {
    protected override void OnRender(DrawingContext dc)
    {
      Size size = new Size(base.ActualWidth, base.ActualHeight);
      double num = this.Maximum - this.Minimum;
    }
  }
}