// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterpolationBase.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2012 Dr. Adrian Voßkühler  
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
//   The interpolation base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Interpolation
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  ///   The interpolation base.
  /// </summary>
  public abstract class InterpolationBase : DependencyObject
  {
    #region Static Fields

    /// <summary>
    ///   The filter type property.
    /// </summary>
    public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register(
      "FilterType", 
      typeof(Interpolation.FilterTypes), 
      typeof(InterpolationBase), 
      new FrameworkPropertyMetadata(
        Interpolation.FilterTypes.MovingAverage, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The number of samples to interpolate property.
    /// </summary>
    public static readonly DependencyProperty NumberOfSamplesToInterpolateProperty =
      DependencyProperty.Register(
        "NumberOfSamplesToInterpolate", 
        typeof(int), 
        typeof(InterpolationBase), 
        new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the custom user control.
    /// </summary>
    public UserControl CustomUserControl { get; set; }

    /// <summary>
    ///   Gets or sets the filter type.
    /// </summary>
    public Interpolation.FilterTypes FilterType
    {
      get
      {
        return (Interpolation.FilterTypes)this.GetValue(FilterTypeProperty);
      }

      set
      {
        this.SetValue(FilterTypeProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the number of samples to interpolate.
    /// </summary>
    public int NumberOfSamplesToInterpolate
    {
      get
      {
        return (int)this.GetValue(NumberOfSamplesToInterpolateProperty);
      }

      set
      {
        this.SetValue(NumberOfSamplesToInterpolateProperty, value);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The calculate interpolated values.
    /// </summary>
    /// <param name="samples">
    /// The samples. 
    /// </param>
    public abstract void CalculateInterpolatedValues(DataCollection samples);

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    public override string ToString()
    {
      return this.FilterType.ToString();
    }

    #endregion
  }
}