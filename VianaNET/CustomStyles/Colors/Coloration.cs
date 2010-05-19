# region Using Directives

using System;
using System.Windows.Media;
using System.Collections.Generic;

# endregion

namespace VianaNET
{
	public class Coloration
	{
		# region Declarations

		private ColorType _ColorType;
		//private IEnumerable<HSXColor> _SourcePallet;
		private HSXColor[] _SourcePallet;
		private PalletInfo _PalletInfo;

		# endregion

		# region Constructor

		public Coloration(ColorType colorType, params Color[] pallet)
		//	: this(colorType, pallet.ToList())
		//{
		//}
		//public Coloration(ColorType colorType, IEnumerable<Color> pallet)
		{
			if (pallet == null || pallet.Length == 0)
			{
				throw new InvalidOperationException("The pallet cannot be empty.");
			}
			this._ColorType = colorType;
			this.SetSourcePallet(pallet);
		}

		# endregion

		# region PalletInfo Struct

		private struct PalletInfo
		{
			public float HueMin;
			public float HueMax;
			public float HueAvg;
			public float SatMin;
			public float SatMax;
			public float XMin;
			public float XMax;
		}

		# endregion

		# region IsHSL

		private bool IsHSL
		{
			get
			{
				return this._ColorType == ColorType.HSL;
			}
		} 

		# endregion

		# region SetSourcePallet

		//private void SetSourcePallet(IEnumerable<Color> pallet)
		private void SetSourcePallet(Color[] pallet)
		{
            //if (!this._ColorType.In(ColorType.HSL, ColorType.HSV)) // C# 3.0
			if (this._ColorType != ColorType.HSL && this._ColorType != ColorType.HSV)
            {
                throw new NotImplementedException(string.Format(
                      "Not implemented color type: \"{0}\"", this._ColorType));
            }

			# region Linq

			//this._SourcePallet = pallet.Select(c => IsHSL ? ConvertColor.RBGToHSL(c) : ConvertColor.RBGToHSV(c)).ToArray();

			//IEnumerable<float> values = this._SourcePallet.Select(c => c.Hue);
			//this._PalletInfo.HueMin = values.Min();
			//this._PalletInfo.HueMax = values.Max();
			//this._PalletInfo.HueAvg = (this._PalletInfo.HueMin + this._PalletInfo.HueMax) / 2F;

			//values = this._SourcePallet.Select(c => c.Saturation);
			//this._PalletInfo.SatMin = values.Min();
			//this._PalletInfo.SatMax = values.Max();

			//values = this._SourcePallet.Select(c => c.X);
			//this._PalletInfo.XMin = values.Min();
			//this._PalletInfo.XMax = values.Max(); 

			# endregion

            this._PalletInfo.HueMin = float.MaxValue;
            this._PalletInfo.SatMin = float.MaxValue;
            this._PalletInfo.XMin = float.MaxValue;

            this._SourcePallet = new HSXColor[pallet.Length];

			this._PalletInfo.HueAvg = 0F;

            for (int i = 0; i < pallet.Length; i++)
            {
				HSXColor hsx;

				if (this.IsHSL)
				{
					hsx = ConvertColor.RBGToHSL(pallet[i]);
				}
				else
				{
					hsx = ConvertColor.RBGToHSV(pallet[i]);
				}

				this._SourcePallet[i] = hsx;

				if (hsx.Hue.HasValue)
				{
					if (hsx.Hue < this._PalletInfo.HueMin)
					{
						this._PalletInfo.HueMin = hsx.Hue.Value;
					}
					if (hsx.Hue > this._PalletInfo.HueMax)
					{
						this._PalletInfo.HueMax = hsx.Hue.Value;
					}

					this._PalletInfo.HueAvg += hsx.Hue.Value;
				}

				if (hsx.Saturation < this._PalletInfo.SatMin)
				{
					this._PalletInfo.SatMin = hsx.Saturation;
				}
				if (hsx.Saturation > this._PalletInfo.SatMax)
				{
					this._PalletInfo.SatMax = hsx.Saturation;
				}

				if (hsx.X < this._PalletInfo.XMin)
				{
					this._PalletInfo.XMin = hsx.X;
				}
				if (hsx.X > this._PalletInfo.XMax)
				{
					this._PalletInfo.XMax = hsx.X;
				}				
            }

			this._PalletInfo.HueAvg /= pallet.Length;			
		}

		# endregion

		# region GetHueAverage

		public float GetHueAverage()
		{
			return this._PalletInfo.HueAvg;
		}

		# endregion

		# region CalculateParameters

		private void CalculateParameters(float value, float min, float max,
			out float diff, out float constraint, out float aux)
		{
			diff = value;
			constraint = 0F;
			aux = 0F;

			if (diff == 0.5F)
			{
				diff = 0F;
			}
			else if (diff > 0.5F)
			{
				aux = max;

				constraint = 0.5F + (1F - max) / (1F - min) / 2F;

				if (diff >= constraint) // 100% + constraint
				{
					constraint = 1F / (1F - constraint) * (diff - constraint);
					diff = 1F - max;
				}
				else // 0 constraint
				{
					diff = ((diff - 0.5F) / (constraint - 0.5F)) * (1F - max);
					constraint = 0F;
				}
			}
			else if (diff < 0.5F)
			{
				aux = min;

				constraint = 0.5F - min / max / 2F;

				if (diff <= constraint) // 100% + constraint
				{
					constraint = (constraint - diff) / constraint;
					diff = -min;
				}
				else
				{
					diff = -(0.5F - diff) / (0.5F - constraint) * min;
					constraint = 0;
				}
			}
		}

		# endregion

		# region GetColoredPallet

		//public IEnumerable<Color> GetColoredPallet(float hue, float hueConstraint, float saturation, float x)
		public Color[] GetColoredPallet(float hue, float hueConstraint, float saturation, float x)
		{
            if (this._ColorType != ColorType.HSL && this._ColorType != ColorType.HSV)
            {
                throw new NotImplementedException(string.Format(
                      "Not implemented color type: \"{0}\"", this._ColorType));
            }

            #region Linq

            //if (!this._ColorType.In(ColorType.HSL, ColorType.HSV))
            //{
            //    throw new NotImplementedException(string.Format(
            //          "Not implemented color type: \"{0}\"", this._ColorType));
            //} 

            #endregion

			// Hue
			float hueDiff = this._PalletInfo.HueAvg - hue;

			// Saturation
			float satDiff;
			float satConstraint;
			float satAux;

			this.CalculateParameters(saturation, this._PalletInfo.SatMin,
				this._PalletInfo.SatMax, out satDiff, out satConstraint, out satAux);

			// X
			float xDiff;
			float xConstraint;
			float xAux;

			this.CalculateParameters(x, this._PalletInfo.XMin,
				this._PalletInfo.XMax, out xDiff, out xConstraint, out xAux);

            Color[] result = new Color[this._SourcePallet.Length];
            HSXColor sourceColor;
            HSXColor resultColor;

            //Not Tested yet

            for(int i = 0; i < this._SourcePallet.Length; i++)
            {
                sourceColor = this._SourcePallet[i];
                resultColor = new HSXColor(
						sourceColor.Hue + (this._PalletInfo.HueAvg - sourceColor.Hue) * hueConstraint - hueDiff,
						sourceColor.Saturation + (satAux - sourceColor.Saturation) * satConstraint + satDiff,
						sourceColor.X + (xAux - sourceColor.X) * xConstraint + xDiff);

                result[i] = (this.IsHSL ? ConvertColor.HSLToRGB(resultColor) : ConvertColor.HSVToRGB(resultColor));
            }

			#region Linq

            //Color[] result = (
            //    from
            //        c in this._SourcePallet
            //    select
            //        new HSXColor(
            //            c.Hue + (this._PalletInfo.HueAvg - c.Hue) * hueConstraint - hueDiff,
            //            c.Saturation + (satAux - c.Saturation) * satConstraint + satDiff,
            //            c.X + (xAux - c.X) * xConstraint + xDiff)
            //    into n
            //        select
            //            IsHSL ? ConvertColor.HSLToRGB(n) : ConvertColor.HSVToRGB(n)
            //    ).ToArray(); 

	        #endregion

			return result;
		}

		# endregion
	}
}
