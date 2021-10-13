namespace VianaNET.CustomStyles.FontAwesome
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Documents;
  using System.Windows.Markup;
  using System.Windows.Media;

  [ContentProperty("Text")]
  public class IconBlock : FrameworkElement
  {
    public static FontFamily FontAwesomeLight = new FontFamily(new Uri("pack://application:,,,/"), "VianaNET;component/Fonts/#Font Awesome 5 Pro Light");
    public static FontFamily FontAwesomeRegular = new FontFamily(new Uri("pack://application:,,,/"), "VianaNET;component/Fonts/#Font Awesome 5 Pro Regular");
    public static FontFamily FontAwesomeSolid = new FontFamily(new Uri("pack://application:,,,/"), "VianaNET;component/Fonts/#Font Awesome 5 Pro Solid");
    public static FontFamily FontAwesomeBrands = new FontFamily(new Uri("pack://application:,,,/"), "VianaNET;component/Fonts/#Font Awesome 5 Brands Regular");

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
     nameof(Icon),
     typeof(IconChar),
     typeof(IconBlock),
     new PropertyMetadata(IconChar.None, OnIconPropertyChanged));

    public static readonly DependencyProperty FontTypeProperty = DependencyProperty.Register(
      nameof(FontType),
      typeof(AwesomeFontType),
      typeof(IconBlock),
      new PropertyMetadata(AwesomeFontType.Light, OnFormattedTextUpdated));

    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
      "Foreground",
      typeof(Brush),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
      "Stroke",
      typeof(Brush),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, StrokePropertyChangedCallback));

    private static void StrokePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      (dependencyObject as IconBlock)?.UpdatePen();
    }

    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
      "StrokeThickness",
      typeof(double),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, StrokePropertyChangedCallback));

    //public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
    //  typeof(IconBlock),
    //  new FrameworkPropertyMetadata(OnFormattedTextUpdated));

    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
      typeof(IconBlock),
      new FrameworkPropertyMetadata(16d, OnFormattedTextUpdated));

    public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
      typeof(IconBlock),
      new FrameworkPropertyMetadata(OnFormattedTextUpdated));

    public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
      typeof(IconBlock),
      new FrameworkPropertyMetadata(OnFormattedTextUpdated));

    public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
      typeof(IconBlock),
      new FrameworkPropertyMetadata(OnFormattedTextUpdated));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      "Text",
      typeof(string),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(OnFormattedTextInvalidated));

    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
      "TextAlignment",
      typeof(TextAlignment),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(TextAlignment.Center, OnFormattedTextUpdated));

    public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
      "TextDecorations",
      typeof(TextDecorationCollection),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(OnFormattedTextUpdated));

    public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
      "TextTrimming",
      typeof(TextTrimming),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(OnFormattedTextUpdated));

    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
      "TextWrapping",
      typeof(TextWrapping),
      typeof(IconBlock),
      new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnFormattedTextUpdated));

    private FormattedText _FormattedText;
    private Geometry _TextGeometry;
    private Pen _Pen;

    public Brush Foreground
    {
      get => (Brush)this.GetValue(ForegroundProperty);
      set => this.SetValue(ForegroundProperty, value);
    }

    //public FontFamily FontFamily
    //{
    //  get { return (FontFamily)GetValue(FontFamilyProperty); }
    //  set { SetValue(FontFamilyProperty, value); }
    //}

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
      get => (double)this.GetValue(FontSizeProperty);
      set => this.SetValue(FontSizeProperty, value);
    }

    public FontStretch FontStretch
    {
      get => (FontStretch)this.GetValue(FontStretchProperty);
      set => this.SetValue(FontStretchProperty, value);
    }

    public FontStyle FontStyle
    {
      get => (FontStyle)this.GetValue(FontStyleProperty);
      set => this.SetValue(FontStyleProperty, value);
    }

    public FontWeight FontWeight
    {
      get => (FontWeight)this.GetValue(FontWeightProperty);
      set => this.SetValue(FontWeightProperty, value);
    }

    public Brush Stroke
    {
      get => (Brush)this.GetValue(StrokeProperty);
      set => this.SetValue(StrokeProperty, value);
    }

    public double StrokeThickness
    {
      get => (double)this.GetValue(StrokeThicknessProperty);
      set => this.SetValue(StrokeThicknessProperty, value);
    }

    public string Text
    {
      get => (string)this.GetValue(TextProperty);
      set => this.SetValue(TextProperty, value);
    }

    public TextAlignment TextAlignment
    {
      get => (TextAlignment)this.GetValue(TextAlignmentProperty);
      set => this.SetValue(TextAlignmentProperty, value);
    }

    public TextDecorationCollection TextDecorations
    {
      get => (TextDecorationCollection)this.GetValue(TextDecorationsProperty);
      set => this.SetValue(TextDecorationsProperty, value);
    }

    public TextTrimming TextTrimming
    {
      get => (TextTrimming)this.GetValue(TextTrimmingProperty);
      set => this.SetValue(TextTrimmingProperty, value);
    }

    public TextWrapping TextWrapping
    {
      get => (TextWrapping)this.GetValue(TextWrappingProperty);
      set => this.SetValue(TextWrappingProperty, value);
    }

    public IconChar Icon
    {
      get => (IconChar)this.GetValue(IconProperty);
      set => this.SetValue(IconProperty, value);
    }

    public AwesomeFontType FontType
    {
      get => (AwesomeFontType)this.GetValue(FontTypeProperty);
      set => this.SetValue(FontTypeProperty, value);
    }


    static IconBlock()
    {
      //FontAwesomeLight = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Pro Light");
      //FontAwesomeRegular = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Pro Regular");
      //FontAwesomeSolid = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Pro Solid");
      //FontAwesomeBrands = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Brands Regular");
      //Application.Current.FindResource("FontAwesomeLight") as FontFamily;
      //FontAwesomeRegular = Application.Current.FindResource("FontAwesomeRegular") as FontFamily;
      //FontAwesomeSolid = Application.Current.FindResource("FontAwesomeSolid") as FontFamily;
      //FontAwesomeBrands = Application.Current.FindResource("FontAwesomeBrands") as FontFamily;
      //< FontFamily x: Key = "FontAwesomeLight" >/ BESS; component / Fonts / FontAwesome5Pro - Light - 300.otf#Font Awesome 5 Pro Light</FontFamily>
    }

    public IconBlock()
    {
      this.UpdatePen();
      this.TextDecorations = new TextDecorationCollection();

      //FontFamily = IconBlock.FontAwesomeLight;
      //VerticalAlignment = VerticalAlignment.Bottom;
      //TextAlignment = TextAlignment.Center;

      var descriptor = DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(IconBlock));
      descriptor.AddValueChanged(this, this.OnTextValueChanged);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      this.EnsureGeometry();
      drawingContext.DrawGeometry(null, this._Pen, this._TextGeometry);
      drawingContext.DrawGeometry(this.Foreground, null, this._TextGeometry);
      var centerX = this.Width / 2;
      var centerY = this.Width / 2;
      drawingContext.DrawEllipse(Brushes.Transparent, null, new Point(centerX, centerY), centerX, centerY);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.EnsureFormattedText();

      // constrain the formatted text according to the available size

      double w = availableSize.Width;
      double h = availableSize.Height;

      // the Math.Min call is important - without this constraint (which seems arbitrary, but is the maximum allowable text width), things blow up when availableSize is infinite in both directions
      // the Math.Max call is to ensure we don't hit zero, which will cause MaxTextHeight to throw
      this._FormattedText.MaxTextWidth = Math.Min(3579139, w);
      this._FormattedText.MaxTextHeight = Math.Max(0.0001d, h);

      // return the desired size
      return new Size(Math.Ceiling(this._FormattedText.Width), Math.Ceiling(this._FormattedText.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.EnsureFormattedText();

      // update the formatted text with the final size
      this._FormattedText.MaxTextWidth = finalSize.Width;
      this._FormattedText.MaxTextHeight = Math.Max(0.0001d, finalSize.Height);

      // need to re-generate the geometry now that the dimensions have changed
      this._TextGeometry = null;

      return finalSize;
    }

    private void UpdatePen()
    {
      this._Pen = new Pen(this.Stroke, this.StrokeThickness)
      {
        DashCap = PenLineCap.Round,
        EndLineCap = PenLineCap.Round,
        LineJoin = PenLineJoin.Round,
        StartLineCap = PenLineCap.Round
      };

      this.InvalidateVisual();
    }

    private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //var fontType = (AwesomeFontType)d.GetValue(FontTypeProperty);
      //switch (fontType)
      //{
      //  case AwesomeFontType.Light:
      //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeLight);
      //    break;
      //  default:
      //  case AwesomeFontType.Regular:
      //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeRegular);
      //    break;
      //  case AwesomeFontType.Solid:
      //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeSolid);
      //    break;
      //  case AwesomeFontType.Brands:
      //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeBrands);
      //    break;
      //}
      d.SetValue(TextAlignmentProperty, TextAlignment.Center);
      //var alignment = (VerticalAlignment)d.GetValue(VerticalAlignmentProperty);
      d.SetValue(TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }

    //private static void OnFontTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //  //var fontType = (AwesomeFontType)d.GetValue(FontTypeProperty);
    //  //switch (fontType)
    //  //{
    //  //  case AwesomeFontType.Light:
    //  //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeLight);
    //  //    break;
    //  //  default:
    //  //  case AwesomeFontType.Regular:
    //  //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeRegular);
    //  //    break;
    //  //  case AwesomeFontType.Solid:
    //  //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeSolid);
    //  //    break;
    //  //  case AwesomeFontType.Brands:
    //  //    d.SetValue(FontFamilyProperty, IconBlock.FontAwesomeBrands);
    //  //    break;
    //  //}

    //  //d.SetValue(TextAlignmentProperty, TextAlignment.Center);
    //  //d.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
    //}

    private void OnTextValueChanged(object sender, EventArgs e)
    {
      var str = this.Text;
      if (str.Length != 1 || !Enum.IsDefined(typeof(IconChar), char.ConvertToUtf32(str, 0)))
        throw new FormatException();
    }

    private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var outlinedTextBlock = (IconBlock)dependencyObject;
      outlinedTextBlock._FormattedText = null;
      outlinedTextBlock._TextGeometry = null;

      outlinedTextBlock.InvalidateMeasure();
      outlinedTextBlock.InvalidateVisual();
    }

    private static void OnFormattedTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var outlinedTextBlock = (IconBlock)dependencyObject;
      outlinedTextBlock.UpdateFormattedText();
      outlinedTextBlock._TextGeometry = null;

      outlinedTextBlock.InvalidateMeasure();
      outlinedTextBlock.InvalidateVisual();
    }

    private void EnsureFormattedText()
    {
      if (this._FormattedText != null)
      {
        return;
      }

      FontFamily family;
      switch (this.FontType)
      {
        case AwesomeFontType.Light:
          family = IconBlock.FontAwesomeLight;
          break;
        default:
        case AwesomeFontType.Regular:
          family = IconBlock.FontAwesomeRegular;
          break;
        case AwesomeFontType.Solid:
          family = IconBlock.FontAwesomeSolid;
          break;
        case AwesomeFontType.Brands:
          family = IconBlock.FontAwesomeBrands;
          break;
      }

      this._FormattedText = new FormattedText(
        this.Text ?? "",
        CultureInfo.CurrentUICulture,
        this.FlowDirection,
        new Typeface(family, this.FontStyle, this.FontWeight, this.FontStretch),
        this.FontSize,
        Brushes.Black,
        VisualTreeHelper.GetDpi(this).PixelsPerDip);

      this.UpdateFormattedText();
    }

    private void UpdateFormattedText()
    {
      if (this._FormattedText == null)
      {
        return;
      }

      FontFamily family;
      switch (this.FontType)
      {
        case AwesomeFontType.Light:
          family = IconBlock.FontAwesomeLight;
          break;
        default:
        case AwesomeFontType.Regular:
          family = IconBlock.FontAwesomeRegular;
          break;
        case AwesomeFontType.Solid:
          family = IconBlock.FontAwesomeSolid;
          break;
        case AwesomeFontType.Brands:
          family = IconBlock.FontAwesomeBrands;
          break;
      }

      this._FormattedText.MaxLineCount = this.TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
      this._FormattedText.TextAlignment = this.TextAlignment;
      this._FormattedText.Trimming = this.TextTrimming;

      this._FormattedText.SetFontSize(this.FontSize);
      this._FormattedText.SetFontStyle(this.FontStyle);
      this._FormattedText.SetFontWeight(this.FontWeight);
      this._FormattedText.SetFontFamily(family);
      this._FormattedText.SetFontStretch(this.FontStretch);
      this._FormattedText.SetTextDecorations(this.TextDecorations);
    }

    private void EnsureGeometry()
    {
      if (this._TextGeometry != null)
      {
        return;
      }

      this.EnsureFormattedText();
      this._TextGeometry = this._FormattedText.BuildGeometry(new Point(0, 0));
    }
  }
}