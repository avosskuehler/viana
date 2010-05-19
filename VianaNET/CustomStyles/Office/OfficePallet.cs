# region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Reflection;

# endregion

namespace VianaNET
{
	public sealed class OfficePallet
	{
		# region Declarations

		private FieldInfo[] _Fields;

		# endregion

		# region Constructor

		internal OfficePallet(Type type)
		{
			this.Startup(type);
		}

		# endregion

		# region Startup

		private void Startup(Type type)
		{
			this._Type = type;
			//IEnumerable<Color> colors = this.RetrieveColors(type);
			Color[] colors = this.RetrieveColors(type);
			this._HSLColoration = new Coloration(ColorType.HSL, colors);
			this._HSVColoration = new Coloration(ColorType.HSV, colors);

			this._ColorType = ColorType.HSL;
			this.Reset();
		}

		# endregion

		# region Properties

		# region Coloration

		private Coloration _HSLColoration;
		private Coloration _HSVColoration;

		public Coloration Coloration
		{
			get
			{
				return this._ColorType == ColorType.HSL ? this._HSLColoration : this._HSVColoration;
			}
		}

		# endregion

		# region Type

		private Type _Type;

		public Type Type
		{
			get
			{
				return this._Type;
			}
		}

		# endregion

		# region ColorType

		private ColorType _ColorType;

		public ColorType ColorType
		{
			get
			{
				return this._ColorType;
			}
			set
			{
				if (this._ColorType != value)
				{
					this._ColorType = value;
					this.OnColorTypeChanged();
				}
			}
		}

		private void OnColorTypeChanged()
		{
			this.ArrangeColors();
		}

		# endregion

		# region Hue

		private float _Hue;

		public float Hue
		{
			get
			{
				return this._Hue;
			}
			set
			{
				if (this._Hue != value)
				{
					this._Hue = value;
					this.OnHueChanged();
				}
			}
		}

		private void OnHueChanged()
		{
			this.ArrangeColors();
		}

		# endregion

		# region HueConstraint

		private float _HueConstraint;

		public float HueConstraint
		{
			get
			{
				return this._HueConstraint;
			}
			set
			{
				if (this._HueConstraint != value)
				{
					this._HueConstraint = value;
					this.OnHueConstraintChanged();
				}
			}
		}

		private void OnHueConstraintChanged()
		{
			this.ArrangeColors();
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
				if (this._Saturation != value)
				{
					this._Saturation = value;
					this.OnSaturationChanged();
				}
			}
		}

		private void OnSaturationChanged()
		{
			this.ArrangeColors();
		}

		# endregion

		# region Brightness

		private float _Brightness;

		public float Brightness
		{
			get
			{
				return this._Brightness;
			}
			set
			{
				if (this._Brightness != value)
				{
					this._Brightness = value;
					this.OnBrightnessChanged();
				}
			}
		}

		private void OnBrightnessChanged()
		{
			this.ArrangeColors();
		}

		# endregion 

		# endregion

		# region RetrieveColors

		//private IEnumerable<Color> RetrieveColors(Type type)
		private Color[] RetrieveColors(Type type)
		{
            this._Fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

			#region // Linq

            //this._Fields = (
            //        from
            //            f in type.GetFields(BindingFlags.Static | BindingFlags.Public)
            //        where
            //            f.FieldType == typeof(Color)
            //        select
            //            f).ToArray();
			////IEnumerable<Color> colors = this._Fields.Select(f => (Color)f.GetValue(null)); 
            //Color[] colors = this._Fields.Select(f => (Color)f.GetValue(null)).ToArray();

    	    #endregion

            Color[] colors = Array.ConvertAll<FieldInfo, Color>(this._Fields, new Converter<FieldInfo,Color>(FieldInfoToColor));

			return colors;
		}

        private Color FieldInfoToColor(FieldInfo fieldInfo)
        {
            return (Color)fieldInfo.GetValue(null);
        }

		# endregion

		# region ArrangeColors

		private void ArrangeColors()
		{
            #region Linq

            //Color[] colors = this.Coloration.GetColoredPallet(this._Hue,
            //    this._HueConstraint, this._Saturation, this._Brightness).ToArray(); 

            #endregion

            Color[] colors = this.Coloration.GetColoredPallet(this._Hue,
                this._HueConstraint, this._Saturation, this._Brightness);

			this.SetColors(colors);
		}

		# endregion

		# region Reset

		public void Reset()
		{
			this._Hue = this.Coloration.GetHueAverage();
			this._HueConstraint = 0F;
			this._Saturation = 0.5F;
			this._Brightness = 0.5F;

			this.ArrangeColors();
		}

		# endregion

		# region SetColors

		private void SetColors(Color[] colors)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				this._Fields[i].SetValue(null, colors[i]);
			}

            foreach (Type type in OfficeColors.RegistersTypes)
            {
                type.GetMethod("Reset").Invoke(null, null);
            }
		}

		# endregion
	}
}
