#region Header
//
//   Project:           FaceLight - Simple Silverlight Real Time Face Detection.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright © 2010 Rene Schulte
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System;
using System.Windows;

namespace VianaNET
{
   /// <summary>
   /// Integer VectorInt.
   /// </summary>
   public struct VectorInt
   {
      public int X;
      public int Y;


      public static VectorInt Zero { get { return new VectorInt(0, 0); } }
      public static VectorInt One { get { return new VectorInt(1, 1); } }
      public static VectorInt UnitX { get { return new VectorInt(1, 0); } }
      public static VectorInt UnitY { get { return new VectorInt(0, 1); } }
      
      public int Length { get { return (int)System.Math.Sqrt(X * X + Y * Y); } }
      public int LengthSq { get { return X * X + Y * Y; } }


      public VectorInt(int x, int y)
      {
         this.X = x;
         this.Y = y;
      }

      public VectorInt(Point point)
         : this((int)point.X, (int) point.Y)
      {
      }


      public static VectorInt operator +(VectorInt v1, VectorInt v2)
      {
        return new VectorInt(v1.X + v2.X, v1.Y + v2.Y);
      }

      public static VectorInt operator -(VectorInt v1, VectorInt v2)
      {
        return new VectorInt(v1.X - v2.X, v1.Y - v2.Y);
      }

      public static VectorInt operator *(VectorInt p, int s)
      {
         return new VectorInt(p.X * s, p.Y * s);
      }

      public static VectorInt operator *(int s, VectorInt p)
      {
         return new VectorInt(p.X * s, p.Y * s);
      }

      public static VectorInt operator *(VectorInt p, float s)
      {
         return new VectorInt((int)(p.X * s), (int)(p.Y * s));
      }

      public static VectorInt operator *(float s, VectorInt p)
      {
         return new VectorInt((int)(p.X * s), (int)(p.Y * s));
      }

      public static bool operator ==(VectorInt v1, VectorInt v2)
      {
         return v1.X == v2.X && v1.Y == v2.Y;
      }

      public static bool operator !=(VectorInt v1, VectorInt v2)
      {
         return v1.X != v2.X || v1.Y != v2.Y;
      }


      public VectorInt Interpolate(VectorInt v2, float amount)
      {
         return new VectorInt((int)(this.X + ((v2.X - this.X) * amount)), (int)(this.Y + ((v2.Y - this.Y) * amount)));
      }

      public int Dot(VectorInt v2)
      {
         return this.X * v2.X + this.Y * v2.Y;
      }

      public int AngleDeg(VectorInt v2)
      {
         // Normalize this
         var len = this.Length;
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
         var rad = Math.Atan2(y2, x2) - Math.Atan2(y1, x1);
         
         // return the angle in degrees
         return (int)(ToDegrees(rad));
      }

      /// <summary>
      /// Converts a radian into a degreee value.
      /// </summary>
      /// <param name="radians">An angle in rad.</param>
      /// <returns>The angle converted to degress.</returns>
      public static double ToDegrees(double radians)
      {
        return radians * 57.295779513082320876798154814105;
      }

      /// <summary>
      /// Converts a degree into a radian value.
      /// </summary>
      /// <param name="degrees">A angle in deg.</param>
      /// <returns>The angle converted to radians.</returns>
      public static double ToRadians(double degrees)
      {
        return degrees * 0.017453292519943295769236907684886;
      }

      public override bool Equals(object obj)
      {
         if (obj is VectorInt)
         {
            return ((VectorInt)obj) == this;
         }
         return false;
      }

      public override int GetHashCode()
      {
         return X.GetHashCode() ^ Y.GetHashCode();
      }

      public override string ToString()
      {
         return String.Format("({0}, {1})", X, Y);;
      }
   }
}