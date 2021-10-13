using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace VianaNET.CustomStyles.FontAwesome
{
  public class IconSource : MarkupExtension
  {
    private readonly IconChar _icon;
    private Brush _foreground = IconHelper.DefaultBrush;
    private ImageSource _imageSource;
    private double _size = IconHelper.DefaultSize;
    private AwesomeFontType _fontType = AwesomeFontType.Regular;

    public IconSource(IconChar icon)
    {
      this._icon = icon;
      this._imageSource = this._icon.ToImageSource(this._fontType, this._foreground, this._size);
    }

    public Brush Foreground
    {
      get => this._foreground;
      set
      {
        if (this._foreground.Equals(value)) return;
        this._foreground = value;
        this.UpdateImageSource();
      }
    }

    public AwesomeFontType FontType
    {
      get => this._fontType;
      set
      {
        this._fontType = value;
        this.UpdateImageSource();
      }
    }

    public double Size
    {
      get => this._size;
      set
      {
        if (Math.Abs(this._size - value) < 0.5) return;
        this._size = value;
        this.UpdateImageSource();
      }
    }

    private void UpdateImageSource()
    {
      this._imageSource = this._icon.ToImageSource(this._fontType, this._foreground, this._size);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this._imageSource;
    }
  }
}
