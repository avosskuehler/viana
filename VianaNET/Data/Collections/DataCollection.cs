// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCollection.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Collections
{
  using System;
  using System.Linq;

  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   The data collection.
  /// </summary>
  public class DataCollection : SortedObservableCollection<TimeSample>, ICloneable
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="DataCollection" /> class.
    /// </summary>
    public DataCollection()
      : base(new TimeSample.TimeComparer())
    {
    }

    #endregion

    #region Public Events

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets a value indicating whether all samples are selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if all samples are selected otherwise, <c>false</c>.
    /// </value>
    public bool AllSamplesSelected => this.All(timesample => timesample.IsSelected);

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    ///   A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      DataCollection returnCollection = new DataCollection();
      foreach (TimeSample sample in this)
      {
        returnCollection.Add(sample);
      }

      return returnCollection;
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="sampleToCheck">
    /// The sample to check.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public bool Contains(TimeSample sampleToCheck, out int index)
    {
      index = -1;
      foreach (TimeSample sample in this)
      {
        if (sample.Framenumber == sampleToCheck.Framenumber || sample.Timestamp == sampleToCheck.Timestamp)
        {
          index = this.IndexOf(sample);
          return true;
        }
      }

      return false;
    }


    /// <summary>
    /// Gets the sample located at the given frameindex.
    /// </summary>
    /// <param name="framenumber">
    /// The framenumber where to get the sample for.
    /// </param>
    /// <returns>
    /// The <see cref="TimeSample"/> at the given frame
    /// </returns>
    public TimeSample GetSampleByFrameindex(int framenumber)
    {
      return this.FirstOrDefault(timesample => timesample.Framenumber == framenumber);
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="timeStamp">
    /// The time stamp.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public bool Remove(long timeStamp)
    {
      int index = -1;
      foreach (TimeSample sample in this)
      {
        if (sample.Timestamp == timeStamp)
        {
          index = this.IndexOf(sample);
          break;
        }
      }

      if (index >= 0)
      {
        this.RemoveAt(index);
        return true;
      }

      return false;
    }

    #endregion

  }
}