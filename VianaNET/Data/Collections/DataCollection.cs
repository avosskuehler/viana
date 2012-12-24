// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCollection.cs" company="Freie Universität Berlin">
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
//   The data collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Collections
{
  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   The data collection.
  /// </summary>
  public class DataCollection : SortedObservableCollection<TimeSample>
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

    #region Public Methods and Operators

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
      else
      {
        return false;
      }
    }

    #endregion
  }
}