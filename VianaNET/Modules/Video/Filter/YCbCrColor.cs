using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace VianaNET
{
   /// <summary>
   /// YCbCr color representation.
   /// Y in the reange [0, 1].
   /// Cb in the reange [-0.5, 0.5].
   /// Cr in the reange [-0.5, 0.5].
   /// According to http://www.poynton.com/notes/colour_and_gamma/ColorFAQ.html#RTFToC28
   /// </summary>
   public class YCbCrColor
   {
      public static YCbCrColor Min { get { return new YCbCrColor(0, -0.5f, -0.5f); } }
      public static YCbCrColor Max { get { return new YCbCrColor(1,  0.5f,  0.5f); } }

      public float Y { get; set; }
      public float Cb { get; set; }
      public float Cr { get; set; }

      public YCbCrColor(float y, float cb, float cr)
      {
         Y = y;
         Cb = cb;
         Cr = cr;
      }

      public Color ToArgbColor()
      {
         int c = ToArgbColori();
         return Color.FromArgb((byte)(c >> 24), (byte)(c >> 16), (byte)(c >> 8), (byte)c);
      }

      public int ToArgbColori()
      {
         // Convert to RGB
         var r = (Y + 1.402f * Cr) * 255;
         var g = (Y - 0.344136f * Cb - 0.714136f * Cr) * 255;
         var b = (Y + 1.772f * Cb) * 255;

         return (255 << 24) | ((byte)(r > 255 ? 255 : r) << 16) | ((byte)(g > 255 ? 255 : g) << 8) | (byte)(b > 255 ? 255 : b);
      }
      
      public YCbCrColor Interpolate(YCbCrColor c2, float amount)
      {
         return new YCbCrColor(  Y + (c2.Y - Y) * amount,
                                 Cb + (c2.Cb - Cb) * amount,
                                 Cr + (c2.Cr - Cr) * amount);
      }

      public static YCbCrColor FromRgb(byte r, byte g, byte b)
      {
         // Create new YCbCr color from rgb color
         const float f = 1f / 255f;
         var rf = r * f;
         var gf = g * f;
         var bf = b * f; 

         var y = 0.299f * rf + 0.587f * gf + 0.114f * bf;
         var cb = -0.168736f * rf + -0.331264f * gf + 0.5f * bf;
         var cr = 0.5f * rf + -0.418688f * gf + -0.081312f * bf;

         return new YCbCrColor(y, cb, cr);
      }

      public static YCbCrColor FromArgbColor(Color color)
      {
         return FromRgb(color.R, color.G, color.B);
      }

      public static YCbCrColor FromArgbColori(int color)
      {
         return FromRgb((byte)(color >> 16), (byte)(color >> 8), (byte)(color));
      }
   }
}
