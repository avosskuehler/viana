// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterBase.cs" company="Freie Universität Berlin">
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
//   The filter base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;

  /// <summary>
  ///   The filter base.
  /// </summary>
  public abstract class FilterBase
  {


    /// <summary>
    ///   The a.
    /// </summary>
    public const short A = 3;

    /// <summary>
    ///   The b.
    /// </summary>
    public const short B = 0;

    /// <summary>
    ///   The g.
    /// </summary>
    public const short G = 1;

    /// <summary>
    ///   The r.
    /// </summary>
    public const short R = 2;





    /// <summary>
    ///   Height of processed image.
    /// </summary>
    public int ImageHeight { get; set; }

    /// <summary>
    ///   Height of processed image.
    /// </summary>
    public int ImagePixelSize { get; set; }

    /// <summary>
    ///   Width of processed image.
    /// </summary>
    public int ImageStride { get; set; }

    /// <summary>
    ///   Width of processed image.
    /// </summary>
    public int ImageWidth { get; set; }





    /// <summary>
    /// The process in place.
    /// </summary>
    /// <param name="image">
    /// The image. 
    /// </param>
    public abstract void ProcessInPlace(IntPtr image);


  }
}