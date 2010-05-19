# region Using Directives

using System;
using System.Windows.Media;

# endregion

namespace VianaNET
{

	/// <summary>
	/// Convert colors from RGB to HSL/HSV and vice-versa.
	/// http://en.wikipedia.org/wiki/HSL_color_space
	/// http://en.wikipedia.org/wiki/HSV_color_space
	/// </summary>
	public static class ConvertColor
	{
		# region Declarations

		const float _OneThird = 1F / 3F;
		const float _OneSixth = 1F / 6F;
		const float _TwoThirds = 2F / 3F; 

		# endregion

		# region RBGToHSL

		/// <summary>
		/// RGB to HSL Color Converter
		/// </summary>
		/// <param name="color">RGB Color.</param>
		/// <returns></returns>
		public static HSXColor RBGToHSL(Color color)
		{
			return ConvertColor.RBGToHSL(color.R, color.G, color.B);
		}
		/// <summary>
		/// RGB to HSL Color Converter
		/// </summary>
		/// <param name="r">Red value (0 to 255).</param>
		/// <param name="g">Green value (0 to 255).</param>
		/// <param name="b">Blue value (0 to 255).</param>
		/// <returns></returns>
		public static HSXColor RBGToHSL(byte r, byte g, byte b)
		{
			return ConvertColor.RBGToHSL(r / 255F, g / 255F, b / 255F);
		}
		/// <summary>
		/// RGB to HSL Color Converter
		/// </summary>
		/// <param name="r">Red value (0 to 1).</param>
		/// <param name="g">Green value (0 to 1).</param>
		/// <param name="b">Blue value (0 to 1).</param>
		/// <returns></returns>
		public static HSXColor RBGToHSL(float r, float g, float b)
		{			
			float min = Math.Min(r, Math.Min(g, b));
			float max = Math.Max(r, Math.Max(g, b));

			float? h = null;
			float s = 0F;
			
			float delta = max - min;
			
			// Brightness
			float l = (max + min) / 2F;
			
			if (delta != 0F)
			{
				// Hue
				if (r == max)
				{
					h = (g - b) / (6F * delta);

					if (g < b) h += 1F;
				}
				else if (g == max)
				{
					h = (b - r) / (6F * delta) + _OneThird;
				}
				else // b is max
				{
					h = (r - g) / (6F * delta) + _TwoThirds;
				}

				// Saturation
				if (l != 0F)
				{
					if (l < 0.5F)
					{
						s = delta / (2F * l);
					}
					else
					{
						s = delta / (2F - 2F * l);
					}
				}
			}

			return new HSXColor(h, s, l);
		}

		# endregion

		# region HSLToRGB

		public static Color HSLToRGB(HSXColor hslColor)
		{
			return ConvertColor.HSLToRGB(hslColor.Hue, hslColor.Saturation, hslColor.X);
		}
		public static Color HSLToRGB(float? h, float s, float l)
		{
			float r = 0F;
			float g = 0F;
			float b = 0F;

			if (h == null || s == 0)
			{
				r = g = b = l;
			}
			else
			{
				float q = 0F;

				if (l < 0.5F)
				{
					q = l * (1F + s);
				}
				else
				{
					q = l + s - (l * s);
				}

				float p = 2F * l - q;

				r = h.Value + _OneThird;

				if (r > 1F) r -= 1F;

				g = h.Value;

				b = h.Value - _OneThird;

				if (b < 0F)  b += 1F;

				r = ConvertColor.HSLToRGBAux(r, q, p);
				g = ConvertColor.HSLToRGBAux(g, q, p);
				b = ConvertColor.HSLToRGBAux(b, q, p);
			}

			return Color.FromRgb(Convert.ToByte(r * 255F), Convert.ToByte(g * 255F), Convert.ToByte(b * 255F));
		}

		private static float HSLToRGBAux(float value, float q, float p)
		{
			if (value < _OneSixth)
			{
				value = p + ((q - p) * 6F * value);
			}
			else if (value < 0.5F)
			{
				value = q;
			}
			else if (value < _TwoThirds)
			{
				value = p + ((q - p) * (_TwoThirds - value) * 6F);
			}
			else
			{
				value = p;
			}

			return value;
		}

		# endregion

		# region RBGToHSV

		/// <summary>
		/// RGB to HSV Color Converter
		/// </summary>
		/// <param name="color">RGB Color.</param>
		/// <returns></returns>
		public static HSXColor RBGToHSV(Color color)
		{
			return ConvertColor.RBGToHSV(color.R, color.G, color.B);
		}
		/// <summary>
		/// RGB to HSV Color Converter
		/// </summary>
		/// <param name="r">Red value (0 to 255).</param>
		/// <param name="g">Green value (0 to 255).</param>
		/// <param name="b">Blue value (0 to 255).</param>
		/// <returns></returns>
		public static HSXColor RBGToHSV(byte r, byte g, byte b)
		{
			return ConvertColor.RBGToHSV(r / 255F, g / 255F, b / 255F);
		}
		/// <summary>
		/// RGB to HSV Color Converter
		/// </summary>
		/// <param name="r">Red value (0 to 1).</param>
		/// <param name="g">Green value (0 to 1).</param>
		/// <param name="b">Blue value (0 to 1).</param>
		/// <returns></returns>
		public static HSXColor RBGToHSV(float r, float g, float b)		
		{
			float min = Math.Min(r, Math.Min(g, b));
			float max = Math.Max(r, Math.Max(g, b));

			float? h = null;
			float s = 0F;			

			float delta = max - min;

			// Brightness
			float v = max;

			if (delta != 0F)
			{
				// Hue
				if (r == max)
				{
					h = (g - b) / (6F * delta);

					if (g < b) h += 1F;
				}
				else if (g == max)
				{
					h = (b - r) / (6F * delta) + _OneThird;
				}
				else // b is max
				{
					h = (r - g) / (6F * delta) + _TwoThirds;
				}
			}

			// Saturation
			if (max != 0F)
			{
				s = 1F - (min / max);
			}

			return new HSXColor(h, s, v);
		}

		# endregion

		# region HSVToRGB

		public static Color HSVToRGB(HSXColor hsvColor)
		{
			return ConvertColor.HSVToRGB(hsvColor.Hue, hsvColor.Saturation, hsvColor.X);
		}
		public static Color HSVToRGB(float? h, float s, float v)
		{
			float r = 0F;
			float g = 0F;
			float b = 0F;

			if (h == null || s == 0)
			{
				r = g = b = v;
			}
			else
			{
				byte h1;

				if (1F - ((h.Value / _OneSixth) % 1) < 0.0001F)
				{
					h1 = (byte)(Math.Ceiling(h.Value / _OneSixth));
				}
				else
				{
					h1 = (byte)(Math.Floor(h.Value / _OneSixth));
				}

				float f = h.Value / _OneSixth - h1;
				float p = v * (1F - s);
				float q = v * (1F - f * s);
				float t = v * (1F - (1F - f) * s);
				
				switch (h1)
				{
					case 0:
					case 6:

						r = v; g = t; b = p;

						break;

					case 1:

						r = q; g = v; b = p;

						break;

					case 2:

						r = p; g = v; b = t;

						break;

					case 3:

						r = p; g = q; b = v;

						break;

					case 4:

						r = t; g = p; b = v;

						break;

					case 5:

						r = v; g = p; b = q;

						break;
				}
			}

			return Color.FromRgb(Convert.ToByte(r * 255F), Convert.ToByte(g * 255F), Convert.ToByte(b * 255F));
		}

		# endregion
	}	
}
