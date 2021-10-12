// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorInt.cs" company="Freie Universität Berlin">
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
//   Integer VectorInt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;
  using System.Windows;

  /// <summary>
  ///   Integer VectorInt.
  /// </summary>
  public struct VectorInt
  {


    /// <summary>
    ///   The x.
    /// </summary>
    public int X;

    /// <summary>
    ///   The y.
    /// </summary>
    public int Y;





    /// <summary>
    /// Initializes a new instance of the <see cref="VectorInt"/> struct.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <param name="y">
    /// The y. 
    /// </param>
    public VectorInt(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorInt"/> struct.
    /// </summary>
    /// <param name="point">
    /// The point. 
    /// </param>
    public VectorInt(Point point)
      : this((int)point.X, (int)point.Y)
    {
    }





    /// <summary>
    ///   Gets the one.
    /// </summary>
    public static VectorInt One => new VectorInt(1, 1);

    /// <summary>
    ///   Gets the unit x.
    /// </summary>
    public static VectorInt UnitX => new VectorInt(1, 0);

    /// <summary>
    ///   Gets the unit y.
    /// </summary>
    public static VectorInt UnitY => new VectorInt(0, 1);

    /// <summary>
    ///   Gets the zero.
    /// </summary>
    public static VectorInt Zero => new VectorInt(0, 0);

    /// <summary>
    ///   Gets the length.
    /// </summary>
    public int Length => (int)Math.Sqrt(this.X * this.X + this.Y * this.Y);

    /// <summary>
    ///   Gets the length sq.
    /// </summary>
    public int LengthSq => this.X * this.X + this.Y * this.Y;





    /// <summary>
    /// Converts a radian into a degreee value.
    /// </summary>
    /// <param name="radians">
    /// An angle in rad. 
    /// </param>
    /// <returns>
    /// The angle converted to degress. 
    /// </returns>
    public static double ToDegrees(double radians)
    {
      return radians * 57.295779513082320876798154814105;
    }

    /// <summary>
    /// Converts a degree into a radian value.
    /// </summary>
    /// <param name="degrees">
    /// A angle in deg. 
    /// </param>
    /// <returns>
    /// The angle converted to radians. 
    /// </returns>
    public static double ToRadians(double degrees)
    {
      return degrees * 0.017453292519943295769236907684886;
    }

    /// <summary>
    ///   The +.
    /// </summary>
    /// <param name="v1"> The v 1. </param>
    /// <param name="v2"> The v 2. </param>
    /// <returns> </returns>
    public static VectorInt operator +(VectorInt v1, VectorInt v2)
    {
      return new VectorInt(v1.X + v2.X, v1.Y + v2.Y);
    }

    /// <summary>
    ///   The ==.
    /// </summary>
    /// <param name="v1"> The v 1. </param>
    /// <param name="v2"> The v 2. </param>
    /// <returns> </returns>
    public static bool operator ==(VectorInt v1, VectorInt v2)
    {
      return v1.X == v2.X && v1.Y == v2.Y;
    }

    /// <summary>
    ///   The !=.
    /// </summary>
    /// <param name="v1"> The v 1. </param>
    /// <param name="v2"> The v 2. </param>
    /// <returns> </returns>
    public static bool operator !=(VectorInt v1, VectorInt v2)
    {
      return v1.X != v2.X || v1.Y != v2.Y;
    }

    /// <summary>
    ///   The *.
    /// </summary>
    /// <param name="p"> The p. </param>
    /// <param name="s"> The s. </param>
    /// <returns> </returns>
    public static VectorInt operator *(VectorInt p, int s)
    {
      return new VectorInt(p.X * s, p.Y * s);
    }

    /// <summary>
    ///   The *.
    /// </summary>
    /// <param name="s"> The s. </param>
    /// <param name="p"> The p. </param>
    /// <returns> </returns>
    public static VectorInt operator *(int s, VectorInt p)
    {
      return new VectorInt(p.X * s, p.Y * s);
    }

    /// <summary>
    ///   The *.
    /// </summary>
    /// <param name="p"> The p. </param>
    /// <param name="s"> The s. </param>
    /// <returns> </returns>
    public static VectorInt operator *(VectorInt p, float s)
    {
      return new VectorInt((int)(p.X * s), (int)(p.Y * s));
    }

    /// <summary>
    ///   The *.
    /// </summary>
    /// <param name="s"> The s. </param>
    /// <param name="p"> The p. </param>
    /// <returns> </returns>
    public static VectorInt operator *(float s, VectorInt p)
    {
      return new VectorInt((int)(p.X * s), (int)(p.Y * s));
    }

    /// <summary>
    ///   The -.
    /// </summary>
    /// <param name="v1"> The v 1. </param>
    /// <param name="v2"> The v 2. </param>
    /// <returns> </returns>
    public static VectorInt operator -(VectorInt v1, VectorInt v2)
    {
      return new VectorInt(v1.X - v2.X, v1.Y - v2.Y);
    }

    /// <summary>
    /// The angle deg.
    /// </summary>
    /// <param name="v2">
    /// The v 2. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int AngleDeg(VectorInt v2)
    {
      // Normalize this
      int len = this.Length;
      double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
      if (len != 0)
      {
        double s1 = 1.0f / len;
        x1 = this.X * s1;
        y1 = this.Y * s1;
      }

      // Normalize v2
      len = v2.Length;
      if (len != 0)
      {
        double s2 = 1.0f / len;
        x2 = v2.X * s2;
        y2 = v2.Y * s2;
      }

      // Acos of the dot product would only return degrees between 0° and 180° without a sign
      double rad = Math.Atan2(y2, x2) - Math.Atan2(y1, x1);

      // return the angle in degrees
      return (int)ToDegrees(rad);
    }

    /// <summary>
    /// The dot.
    /// </summary>
    /// <param name="v2">
    /// The v 2. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Dot(VectorInt v2)
    {
      return this.X * v2.X + this.Y * v2.Y;
    }

    /// <summary>
    /// The equals.
    /// </summary>
    /// <param name="obj">
    /// The obj. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public override bool Equals(object obj)
    {
      if (obj is VectorInt)
      {
        return ((VectorInt)obj) == this;
      }

      return false;
    }

    /// <summary>
    ///   The get hash code.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    public override int GetHashCode()
    {
      return this.X.GetHashCode() ^ this.Y.GetHashCode();
    }

    /// <summary>
    /// The interpolate.
    /// </summary>
    /// <param name="v2">
    /// The v 2. 
    /// </param>
    /// <param name="amount">
    /// The amount. 
    /// </param>
    /// <returns>
    /// The <see cref="VectorInt"/> . 
    /// </returns>
    public VectorInt Interpolate(VectorInt v2, float amount)
    {
      return new VectorInt((int)(this.X + ((v2.X - this.X) * amount)), (int)(this.Y + ((v2.Y - this.Y) * amount)));
    }

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    public override string ToString()
    {
      return string.Format("({0}, {1})", this.X, this.Y);
    }


  }
}