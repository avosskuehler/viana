// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSample.cs" company="Freie Universität Berlin">
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
//   The data sample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.DataGrid
{
  using System.Collections.Generic;

  using VianaNET.Data.Collections;

  /// <summary>
  /// This class contains options for data export functionality
  /// </summary>
  public class ExportOptions
  {


    /// <summary>
    ///   Gets or sets the list of axes for each object beeing exportet
    /// </summary>
    public List<DataAxis> Axes { get; set; }

    /// <summary>
    ///   Gets or sets the list of zero-based indexes of objects beeing exported
    /// </summary>
    public List<int> Objects { get; set; }


  }
}