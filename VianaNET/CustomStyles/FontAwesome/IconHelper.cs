using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using VianaNET.Logging;
using VianaNET.Properties;

namespace VianaNET.CustomStyles.FontAwesome
{
  // cf.: 
  // * http://stackoverflow.com/questions/23108181/changing-font-icon-in-wpf-using-font-awesome
  // * http://www.codeproject.com/Tips/634540/Using-Font-Icons
  public static class IconHelper
  {
    public const double DefaultSize = 200.0;

    public static readonly Brush DefaultBrush = new SolidColorBrush(Settings.Default.Basisfarbe);

    private static readonly GlyphTypeface GlyphTypefaceLight;
    private static readonly GlyphTypeface GlyphTypefaceRegular;
    private static readonly GlyphTypeface GlyphTypefaceSolid;
    private static readonly GlyphTypeface GlyphTypefaceBrands;

    private static readonly int Dpi = GetDpi();

    static IconHelper()
    {
      var FontAwesomeLight = App.Current.FindResource("FontAwesomeLight") as FontFamily;
      var FontAwesomeRegular = App.Current.FindResource("FontAwesomeRegular") as FontFamily;
      var FontAwesomeSolid = App.Current.FindResource("FontAwesomeSolid") as FontFamily;
      var FontAwesomeBrands = App.Current.FindResource("FontAwesomeBrands") as FontFamily;

      var typeface = new Typeface(FontAwesomeLight, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceLight))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeLight.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceLight))
          ErrorLogger.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeRegular, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceRegular))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeRegular.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceRegular))
          ErrorLogger.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeSolid, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceSolid))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeSolid.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceSolid))
          ErrorLogger.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeBrands, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceBrands))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeBrands.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceBrands))
          ErrorLogger.ProcessErrorMessage("No glyphtypeface found");
      }
    }

    public static ImageSource ToImageSource(this IconChar iconChar, AwesomeFontType fontType = AwesomeFontType.Regular, Brush foregroundBrush = null, double size = DefaultSize)
    {
      var text = char.ConvertFromUtf32((int)iconChar);
      return ToImageSource(text, fontType, foregroundBrush ?? DefaultBrush, size);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static ImageSource ToImageSource(string text, AwesomeFontType fontType = AwesomeFontType.Regular, Brush foregroundBrush = null, double size = DefaultSize)
    {
      if (string.IsNullOrWhiteSpace(text)) return null;

      var glyphIndexes = new ushort[text.Length];
      var advanceWidths = new double[text.Length];
      GlyphTypeface glyphtypeface;

      switch (fontType)
      {
        case AwesomeFontType.Light:
          glyphtypeface = GlyphTypefaceLight;
          break;
        default:
        case AwesomeFontType.Regular:
          glyphtypeface = GlyphTypefaceRegular;
          break;
        case AwesomeFontType.Solid:
          glyphtypeface = GlyphTypefaceSolid;
          break;
        case AwesomeFontType.Brands:
          glyphtypeface = GlyphTypefaceBrands;
          break;
      }

      for (var n = 0; n < text.Length; n++)
      {
        ushort glyphIndex;
        try
        {
          glyphIndex = glyphtypeface.CharacterToGlyphMap[text[n]];
        }
        catch (Exception)
        {
          glyphIndex = 42;
        }
        glyphIndexes[n] = glyphIndex;

        var width = glyphtypeface.AdvanceWidths[glyphIndex] * 1.0;
        advanceWidths[n] = width;
      }

      try
      {
        var fontSize = PixelsToPoints(size);
        var glyphRun = new GlyphRun(
          glyphtypeface,
          0,
          false,
          fontSize,
          Dpi / 96f,
          glyphIndexes,
          new Point(0, 0), advanceWidths, null, null, null, null, null, null);

        var glyphRunDrawing = new GlyphRunDrawing(foregroundBrush ?? DefaultBrush, glyphRun);
        return new DrawingImage(glyphRunDrawing);
      }
      catch (Exception ex)
      {
        Trace.TraceError($"Error generating GlyphRun : {ex.Message}");
      }
      return null;
    }

    private static double PixelsToPoints(double size)
    {
      // pixels to points, cf.: http://stackoverflow.com/a/139712/2592915
      return size * (72.0 / Dpi);
    }

    private static int GetDpi()
    {
      // How can I get the DPI in WPF?, cf.: http://stackoverflow.com/a/12487917/2592915
      var dpiProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
      return (int)dpiProperty.GetValue(null, null);
    }
  }
}
