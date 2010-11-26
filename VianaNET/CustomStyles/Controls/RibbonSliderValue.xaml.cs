
namespace VianaNET
{
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Windows.Controls.Ribbon;

  /// <summary>
  /// Interaction logic for RibbonSliderValue.xaml
  /// </summary>
  public partial class RibbonSliderValue : Slider, IRibbonControl
  {
    public string SliderDescription
    {
      get { return (string)GetValue(SliderDescriptionProperty); }
      set { SetValue(SliderDescriptionProperty, value); }
    }

    public static readonly DependencyProperty SliderDescriptionProperty =
      DependencyProperty.Register(
      "SliderDescription",
      typeof(string),
      typeof(RibbonSliderValue),
      new UIPropertyMetadata("Description"));
  }
}
