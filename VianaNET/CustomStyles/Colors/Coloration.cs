// <copyright file="Coloration.cs" company="FU Berlin">
// ************************************************************************
// Viana.NET - video analysis for physics education
// Copyright (C) 2010 Dr. Adrian Voßkühler  
// ------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License 
// along with this program; if not, write to the Free Software Foundation, 
// Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian.vosskuehler@fu-berlin.de</email>

namespace VianaNET
{
  using System;
  using System.Windows.Media;

  /// <summary>
  /// This class provides support for office style coloration of
  /// the VianaNET application.
  /// </summary>
  public class Coloration
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    /// <summary>
    /// The current <see cref="ColorType"/> of the pallet.
    /// Can be HSL or HSV
    /// </summary>
    private ColorType colorType;

    /// <summary>
    /// The array of office colors in the source pallet.
    /// </summary>
    private HSXColor[] sourcePallet;

    /// <summary>
    /// A <see cref="PalletInfo"/> describing the colors
    /// parameters.
    /// </summary>
    private PalletInfo palletInfo;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the Coloration class with
    /// given <see cref="ColorType"/> and a list of pallet
    /// colors.
    /// </summary>
    /// <param name="colorType">The <see cref="ColorType"/>
    /// the pallets are defined in.</param>
    /// <param name="pallet">An array of <see cref="Color"/>
    /// with the pallet.</param>
    public Coloration(ColorType colorType, params Color[] pallet)
    {
      if (pallet == null || pallet.Length == 0)
      {
        throw new InvalidOperationException("The pallet cannot be empty.");
      }

      this.colorType = colorType;
      this.SetSourcePallet(pallet);
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// Gets a value indicating whether this pallet
    /// is of <see cref="ColorType.HSL"/>
    /// </summary>
    private bool IsHSL
    {
      get { return this.colorType == ColorType.HSL; }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    /// <summary>
    /// Gets the average hue value of the pallet.
    /// </summary>
    /// <returns>A <see cref="Single"/> with the average
    /// hue value of the pallet.</returns>
    public float GetHueAverage()
    {
      return this.palletInfo.HueAvg;
    }

    /// <summary>
    /// Returns a colored pallet for the given hue and saturation.
    /// </summary>
    /// <param name="hue">A <see cref="Single"/> with the hue to use.</param>
    /// <param name="hueConstraint">A <see cref="Single"/> with the hue constraint to use.</param>
    /// <param name="saturation">A <see cref="Single"/> with the saturation to use.</param>
    /// <param name="x">A <see cref="Single"/> with ???</param>
    /// <returns>An array of colors defining the pallet.</returns>
    public Color[] GetColoredPallet(float hue, float hueConstraint, float saturation, float x)
    {
      if (this.colorType != ColorType.HSL && this.colorType != ColorType.HSV)
      {
        throw new NotImplementedException(string.Format(
              "Not implemented color type: \"{0}\"", this.colorType));
      }

      // Hue
      float hueDiff = this.palletInfo.HueAvg - hue;

      // Saturation
      float satDiff;
      float satConstraint;
      float satAux;

      this.CalculateParameters(
        saturation,
        this.palletInfo.SatMin,
        this.palletInfo.SatMax,
        out satDiff,
        out satConstraint,
        out satAux);

      // X
      float diffX;
      float constraintX;
      float auxX;

      this.CalculateParameters(
        x,
        this.palletInfo.XMin,
        this.palletInfo.XMax,
        out diffX,
        out constraintX,
        out auxX);

      Color[] result = new Color[this.sourcePallet.Length];
      HSXColor sourceColor;
      HSXColor resultColor;

      // Not Tested yet
      for (int i = 0; i < this.sourcePallet.Length; i++)
      {
        sourceColor = this.sourcePallet[i];
        resultColor = new HSXColor(
          sourceColor.Hue + ((this.palletInfo.HueAvg - sourceColor.Hue) * hueConstraint) - hueDiff,
          sourceColor.Saturation + ((satAux - sourceColor.Saturation) * satConstraint) + satDiff,
          sourceColor.ValueLuminanceBrightness + ((auxX - sourceColor.ValueLuminanceBrightness) * constraintX) + diffX);

        result[i] = this.IsHSL ? ConvertColor.HSLToRGB(resultColor) : ConvertColor.HSVToRGB(resultColor);
      }

      return result;
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER
    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS

    /// <summary>
    /// Fills coloration class with color array to 
    /// build pallet info.
    /// </summary>
    /// <param name="pallet">An array of <see cref="Color"/>
    /// defining the pallet</param>
    private void SetSourcePallet(Color[] pallet)
    {
      // if (!this._ColorType.In(ColorType.HSL, ColorType.HSV)) // C# 3.0
      if (this.colorType != ColorType.HSL && this.colorType != ColorType.HSV)
      {
        throw new NotImplementedException(string.Format(
              "Not implemented color type: \"{0}\"", this.colorType));
      }

      this.palletInfo.HueMin = float.MaxValue;
      this.palletInfo.SatMin = float.MaxValue;
      this.palletInfo.XMin = float.MaxValue;

      this.sourcePallet = new HSXColor[pallet.Length];

      this.palletInfo.HueAvg = 0F;

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

        this.sourcePallet[i] = hsx;

        if (hsx.Hue.HasValue)
        {
          if (hsx.Hue < this.palletInfo.HueMin)
          {
            this.palletInfo.HueMin = hsx.Hue.Value;
          }

          if (hsx.Hue > this.palletInfo.HueMax)
          {
            this.palletInfo.HueMax = hsx.Hue.Value;
          }

          this.palletInfo.HueAvg += hsx.Hue.Value;
        }

        if (hsx.Saturation < this.palletInfo.SatMin)
        {
          this.palletInfo.SatMin = hsx.Saturation;
        }

        if (hsx.Saturation > this.palletInfo.SatMax)
        {
          this.palletInfo.SatMax = hsx.Saturation;
        }

        if (hsx.ValueLuminanceBrightness < this.palletInfo.XMin)
        {
          this.palletInfo.XMin = hsx.ValueLuminanceBrightness;
        }

        if (hsx.ValueLuminanceBrightness > this.palletInfo.XMax)
        {
          this.palletInfo.XMax = hsx.ValueLuminanceBrightness;
        }
      }

      this.palletInfo.HueAvg /= pallet.Length;
    }

    /// <summary>
    /// Calculates color parameters.
    /// </summary>
    /// <param name="value">A <see cref="Single"/> with the value</param>
    /// <param name="min">A <see cref="Single"/> with the minimum for the value</param>
    /// <param name="max">A <see cref="Single"/> with the maximum for the value</param>
    /// <param name="diff">Out. Returns the diff value.</param>
    /// <param name="constraint">Out. Returns the constraint.</param>
    /// <param name="aux">Out. Returns the aux value.</param>
    private void CalculateParameters(
      float value,
      float min,
      float max,
      out float diff,
      out float constraint,
      out float aux)
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

        constraint = 0.5F + (((1F - max) / (1F - min)) / 2F);

        if (diff >= constraint)
        {
          // 100% + constraint
          constraint = 1F / (1F - constraint) * (diff - constraint);
          diff = 1F - max;
        }
        else
        {
          // 0 constraint
          diff = ((diff - 0.5F) / (constraint - 0.5F)) * (1F - max);
          constraint = 0F;
        }
      }
      else if (diff < 0.5F)
      {
        aux = min;

        constraint = 0.5F - ((min / max) / 2F);

        if (diff <= constraint)
        {
          // 100% + constraint
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

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    /// <summary>
    /// A structure specifying Hue and Saturation
    /// of the pallet.
    /// </summary>
    private struct PalletInfo
    {
      /// <summary>
      /// Minimum hue value
      /// </summary>
      public float HueMin;

      /// <summary>
      /// Maximum hue value
      /// </summary>
      public float HueMax;

      /// <summary>
      /// Average hue value
      /// </summary>
      public float HueAvg;

      /// <summary>
      /// Minimal saturation
      /// </summary>
      public float SatMin;

      /// <summary>
      /// Maximal saturation
      /// </summary>
      public float SatMax;

      /// <summary>
      /// Minimum x value
      /// </summary>
      public float XMin;

      /// <summary>
      /// Maximum x value
      /// </summary>
      public float XMax;
    }

    #endregion //HELPER
  }
}
