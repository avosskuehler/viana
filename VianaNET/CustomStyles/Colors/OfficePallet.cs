// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfficePallet.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
//   ------------------------------------------------------------------------
//   This program is free software; you can redistribute it and/or modify it 
//   under the terms of the GNU General Public License as published by the 
//   Free Software Foundation; either version 2 of the License, or 
//   (at your option) any later version.
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License 
//   along with this program; if not, write to the Free Software Foundation, 
//   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//   ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   The office pallet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Colors
{
  using System;
  using System.Reflection;
  using System.Windows.Media;

  /// <summary>
  ///   The office pallet.
  /// </summary>
  public sealed class OfficePallet
  {
    #region Fields

    /// <summary>
    ///   The _ brightness.
    /// </summary>
    private float _Brightness;

    /// <summary>
    ///   The _ color type.
    /// </summary>
    private ColorType _ColorType;

    /// <summary>
    ///   The _ fields.
    /// </summary>
    private FieldInfo[] _Fields;

    /// <summary>
    ///   The _ hsl coloration.
    /// </summary>
    private Coloration _HSLColoration;

    /// <summary>
    ///   The _ hsv coloration.
    /// </summary>
    private Coloration _HSVColoration;

    /// <summary>
    ///   The _ hue.
    /// </summary>
    private float _Hue;

    /// <summary>
    ///   The _ hue constraint.
    /// </summary>
    private float _HueConstraint;

    /// <summary>
    ///   The _ saturation.
    /// </summary>
    private float _Saturation;

    /// <summary>
    ///   The _ type.
    /// </summary>
    private Type _Type;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OfficePallet"/> class.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    internal OfficePallet(Type type)
    {
      this.Startup(type);
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the brightness.
    /// </summary>
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

    /// <summary>
    ///   Gets or sets the color type.
    /// </summary>
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

    /// <summary>
    ///   Gets the coloration.
    /// </summary>
    public Coloration Coloration
    {
      get
      {
        return this._ColorType == ColorType.HSL ? this._HSLColoration : this._HSVColoration;
      }
    }

    /// <summary>
    ///   Gets or sets the hue.
    /// </summary>
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

    /// <summary>
    ///   Gets or sets the hue constraint.
    /// </summary>
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

    /// <summary>
    ///   Gets or sets the saturation.
    /// </summary>
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

    /// <summary>
    ///   Gets the type.
    /// </summary>
    public Type Type
    {
      get
      {
        return this._Type;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The reset.
    /// </summary>
    public void Reset()
    {
      this._Hue = this.Coloration.GetHueAverage();
      this._HueConstraint = 0F;
      this._Saturation = 0.5F;
      this._Brightness = 0.5F;

      this.ArrangeColors();
    }

    #endregion

    #region Methods

    /// <summary>
    ///   The arrange colors.
    /// </summary>
    private void ArrangeColors()
    {
      // Color[] colors = this.Coloration.GetColoredPallet(this._Hue,
      // this._HueConstraint, this._Saturation, this._Brightness).ToArray(); 
      Color[] colors = this.Coloration.GetColoredPallet(
        this._Hue, 
        this._HueConstraint, 
        this._Saturation, 
        this._Brightness);

      this.SetColors(colors);
    }

    /// <summary>
    /// The field info to color.
    /// </summary>
    /// <param name="fieldInfo">
    /// The field info.
    /// </param>
    /// <returns>
    /// The <see cref="Color"/> .
    /// </returns>
    private Color FieldInfoToColor(FieldInfo fieldInfo)
    {
      return (Color)fieldInfo.GetValue(null);
    }

    /// <summary>
    ///   The on brightness changed.
    /// </summary>
    private void OnBrightnessChanged()
    {
      this.ArrangeColors();
    }

    /// <summary>
    ///   The on color type changed.
    /// </summary>
    private void OnColorTypeChanged()
    {
      this.ArrangeColors();
    }

    /// <summary>
    ///   The on hue changed.
    /// </summary>
    private void OnHueChanged()
    {
      this.ArrangeColors();
    }

    /// <summary>
    ///   The on hue constraint changed.
    /// </summary>
    private void OnHueConstraintChanged()
    {
      this.ArrangeColors();
    }

    /// <summary>
    ///   The on saturation changed.
    /// </summary>
    private void OnSaturationChanged()
    {
      this.ArrangeColors();
    }

    // private IEnumerable<Color> RetrieveColors(Type type)
    /// <summary>
    /// The retrieve colors.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <returns>
    /// The <see cref="Color[]"/> .
    /// </returns>
    private Color[] RetrieveColors(Type type)
    {
      this._Fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

      // this._Fields = (
      // from
      // f in type.GetFields(BindingFlags.Static | BindingFlags.Public)
      // where
      // f.FieldType == typeof(Color)
      // select
      // f).ToArray();
      ////IEnumerable<Color> colors = this._Fields.Select(f => (Color)f.GetValue(null)); 
      // Color[] colors = this._Fields.Select(f => (Color)f.GetValue(null)).ToArray();
      Color[] colors = Array.ConvertAll(this._Fields, this.FieldInfoToColor);

      return colors;
    }

    /// <summary>
    /// The set colors.
    /// </summary>
    /// <param name="colors">
    /// The colors.
    /// </param>
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

    /// <summary>
    /// The startup.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    private void Startup(Type type)
    {
      this._Type = type;

      // IEnumerable<Color> colors = this.RetrieveColors(type);
      Color[] colors = this.RetrieveColors(type);
      this._HSLColoration = new Coloration(ColorType.HSL, colors);
      this._HSVColoration = new Coloration(ColorType.HSV, colors);

      this._ColorType = ColorType.HSL;
      this.Reset();
    }

    #endregion
  }
}