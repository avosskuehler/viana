// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterpolationFilter.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   The exponential smooth filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Filter.Interpolation
{
  using System.Collections.Generic;
  using System.Windows.Controls;
  using System.Xml.Serialization;

  /// <summary>
  ///   The exponential smooth filter.
  /// </summary>
  public abstract class InterpolationFilter : FilterBase
  {


    /// <summary>
    /// Initializes static members of the <see cref="InterpolationFilter"/> class.
    /// </summary>
    static InterpolationFilter()
    {
      // Populate predefined interpolation filters
      Filter = new Dictionary<InterpolationFilterTypes, InterpolationFilter>
        {
          { InterpolationFilterTypes.MovingAverage, new MovingAverageFilter() },
          { InterpolationFilterTypes.ExponentialSmooth, new ExponentialSmoothFilter() },
        };
    }





    /// <summary>
    ///   Gets the List of available fitting algorithms.
    /// </summary>
    public static Dictionary<InterpolationFilterTypes, InterpolationFilter> Filter { get; private set; }

    /// <summary>
    ///   Gets the custom user control.
    /// </summary>
    [XmlIgnore]
    public abstract UserControl CustomUserControl { get; }

    /// <summary>
    /// Gets or sets the interpolation filter type.
    /// </summary>
    public InterpolationFilterTypes InterpolationFilterType { get; set; }





    /// <summary>
    ///   The show interpolation options dialog.
    /// </summary>
    public void ShowInterpolationOptionsDialog()
    {
      InterpolationOptionsDialog dlg = new InterpolationOptionsDialog { ChoosenInterpolationFilter = this };

      if (dlg.ShowDialog().GetValueOrDefault())
      {
        App.Project.CurrentFilterData.InterpolationFilter = dlg.ChoosenInterpolationFilter;
      }
    }


  }
}