// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSample.cs" company="Freie Universität Berlin">
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
//   The time sample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VianaNET.Application;

namespace VianaNET.Data.Collections
{
  using System.Collections.Generic;

  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The time sample.
  /// </summary>
  public class TimeSample
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="TimeSample" /> class.
    /// </summary>
    public TimeSample()
    {
      //this.Object = new DataSample[Viana.Project.ProcessingData.NumberOfTrackedObjects];
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the framenumber.
    /// </summary>
    public int Framenumber { get; set; }

    /// <summary>
    ///   Gets or sets the object.
    /// </summary>
    public DataSample[] Object { get; set; }

    /// <summary>
    ///   Gets or sets the timestamp.
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether this sample is selected
    /// </summary>
    public bool IsSelected { get; set; }

    #endregion

    /// <summary>
    ///   The time comparer.
    /// </summary>
    public class TimeComparer : IComparer<TimeSample>
    {
      #region Public Methods and Operators

      /// <summary>
      /// The compare.
      /// </summary>
      /// <param name="x">
      /// The x. 
      /// </param>
      /// <param name="y">
      /// The y. 
      /// </param>
      /// <returns>
      /// The <see cref="int"/> . 
      /// </returns>
      public int Compare(TimeSample x, TimeSample y)
      {
        return (int)(x.Timestamp - y.Timestamp);
      }

      #endregion
    }
  }
}