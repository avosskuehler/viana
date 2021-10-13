using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace VianaNET.CustomStyles.FontAwesome
{
  public class Icon : MarkupExtension
  {
    private readonly IconBlock _iconBlock;

    public Icon(IconChar icon)
    {
      this._iconBlock = new IconBlock
      {
        FontSize = 24,
        Icon = icon
      };
    }

    public Brush Foreground
    {
      get => this._iconBlock.Foreground;
      set => this._iconBlock.Foreground = value;
    }

    public AwesomeFontType FontType
    {
      get => this._iconBlock.FontType;
      set => this._iconBlock.FontType = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this._iconBlock;
    }
  }
}
