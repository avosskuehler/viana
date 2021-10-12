// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Segment.cs" company="Freie Universität Berlin">
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
//   Four integer values decribing a segment which is actually a square.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  /// <summary>
  ///   Four integer values decribing a segment which is actually a square.
  /// </summary>
  public struct Segment
  {


    /// <summary>
    ///   The max.
    /// </summary>
    public VectorInt Max;

    /// <summary>
    ///   The min.
    /// </summary>
    public VectorInt Min;





    /// <summary>
    /// Initializes a new instance of the <see cref="Segment"/> struct.
    /// </summary>
    /// <param name="x1">
    /// The x 1. 
    /// </param>
    /// <param name="y1">
    /// The y 1. 
    /// </param>
    /// <param name="x2">
    /// The x 2. 
    /// </param>
    /// <param name="y2">
    /// The y 2. 
    /// </param>
    public Segment(int x1, int y1, int x2, int y2)
      : this(new VectorInt(x1, y1), new VectorInt(x2, y2))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Segment"/> struct.
    /// </summary>
    /// <param name="min">
    /// The min. 
    /// </param>
    /// <param name="max">
    /// The max. 
    /// </param>
    public Segment(VectorInt min, VectorInt max)
    {
      this.Min = min;
      this.Max = max;
    }





    /// <summary>
    ///   Gets the center.
    /// </summary>
    public VectorInt Center => this.Min.Interpolate(this.Max, 0.5f);

    /// <summary>
    ///   Gets the diagonal.
    /// </summary>
    public int Diagonal => (this.Max - this.Min).Length;

    /// <summary>
    ///   Gets the diagonal sq.
    /// </summary>
    public int DiagonalSq => (this.Max - this.Min).LengthSq;

    /// <summary>
    ///   Gets the height.
    /// </summary>
    public int Height => this.Max.Y - this.Min.Y;

    /// <summary>
    ///   Gets the width.
    /// </summary>
    public int Width => this.Max.X - this.Min.X;


  }
}