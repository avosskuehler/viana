using System;

namespace VianaNET
{
	public struct HSXColor
	{
		# region Constructor

		public HSXColor(HSXColor clone)
			: this(clone.Hue, clone.Saturation, clone.X)
		{
		}
		public HSXColor(float? hue, float saturation, float x)
		{
			this._Hue = null;
			this._Saturation = 0;
			this._X = 0;

			this.Hue = hue;
			this.Saturation = saturation;
			this.X = x;
		}

		# endregion

		# region Hue

		private float? _Hue;

		public float? Hue
		{
			get
			{
				return this._Hue;
			}
			set
			{
				if (value == null && this._Hue != null)
				{
					this._Hue = null;
				}
				else
				{
					if (this._Hue == null)
					{
						this._Hue = value;
					}
					else if (this._Hue != value)
					{						
						if (value < 0)
						{
							this._Hue = value % 1F + 1;
						}
						else
						{
							this._Hue = value % 1F;
						}
					}
				}
			}
		}

		# endregion

		# region Saturation

		private float _Saturation;

		public float Saturation
		{
			get
			{
				return this._Saturation;
			}
			set
			{
				this._Saturation = value;
			}
		}

		# endregion

		# region X (Value/Luminance/Brightness)

		private float _X;

		/// <summary>
		/// Value/Luminance/Brightness
		/// </summary>
		public float X
		{
			get
			{
				return this._X;
			}
			set
			{
				this._X = value;
			}
		}

		# endregion

		# region ToString()

		public override string ToString()
		{
			return string.Format("Hue: {0}, Saturation: {1:P}, X: {2:P}",
				this._Hue.HasValue ? this._Hue.Value.ToString("P") : "NULL", this._Saturation, this._X);
		}

		# endregion
	}
}
