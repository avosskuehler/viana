// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistogramMinMaxSegmentator.cs" company="Freie Universität Berlin">
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
//   Segmentator that uses a histogram to find connected pixels and determines one region.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  ///   Segmentator that uses a histogram to find connected pixels and determines one region.
  /// </summary>
  public class HistogramMinMaxSegmentator
  {
    // private int jitteringCount;
    // private Segment lastSegment;
    // private int jitteringAmountThreshold;
    // private int jitteringAmountThresholdSq;
    #region Public Properties

    /// <summary>
    ///   Gets or sets the histogram.
    /// </summary>
    public Histogram Histogram { get; set; }

    /// <summary>
    ///   Gets or sets the max diameter.
    /// </summary>
    public double MaxDiameter { get; set; }

    /// <summary>
    ///   Gets or sets the min diameter.
    /// </summary>
    public double MinDiameter { get; set; }

    /// <summary>
    ///   Gets or sets the threshold luminance.
    /// </summary>
    public VectorInt ThresholdLuminance { get; set; }

    #endregion

    // public int JitteringCountThreshold { get; set; }
    // public int JitteringAmountThreshold
    // {
    // get { return jitteringAmountThreshold; }
    // set
    // {
    // jitteringAmountThreshold = value;
    // jitteringAmountThresholdSq = value * value;
    // }
    // }
    #region Public Methods and Operators

    /// <summary>
    ///   The process.
    /// </summary>
    /// <returns> The <see cref="Segment" /> . </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Segment Process()
    {
      if (this.Histogram == null)
      {
        throw new InvalidOperationException("The histogram must be provided");
      }

      int[] hx = this.Histogram.X;
      int[] hy = this.Histogram.Y;
      VectorInt histUpperThreshold = this.Histogram.Max * 0.5f;

      // Find seeds for the segmentation:
      // All the x and y histogram indices where the value is above the half maximum and have a min distance
      const int step = 10;

      // x
      List<int> ix = this.GetIndicesAboveThreshold(this.Histogram.MaxIndex.X, -step, hx, histUpperThreshold.X);
      ix.AddRange(this.GetIndicesAboveThreshold(this.Histogram.MaxIndex.X + step, step, hx, histUpperThreshold.X));

      // y
      List<int> iy = this.GetIndicesAboveThreshold(this.Histogram.MaxIndex.Y, -step, hy, histUpperThreshold.Y);
      iy.AddRange(this.GetIndicesAboveThreshold(this.Histogram.MaxIndex.Y + step, step, hy, histUpperThreshold.Y));

      // Find the boundaries for the segments defined by the seeds
      var segments = new List<Segment>();

      foreach (int y0 in iy)
      {
        foreach (int x0 in ix)
        {
          var segment = new Segment(0, 0, 0, 0);
          segment.Min.X = this.GetIndexBelowThreshold(x0, -1, hx, this.ThresholdLuminance.X);
          segment.Max.X = this.GetIndexBelowThreshold(x0, 1, hx, this.ThresholdLuminance.X);
          segment.Min.Y = this.GetIndexBelowThreshold(y0, -1, hy, this.ThresholdLuminance.Y);
          segment.Max.Y = this.GetIndexBelowThreshold(y0, 1, hy, this.ThresholdLuminance.Y);

          // Filter segments by diameter
          if (segment.Diagonal > this.MinDiameter && segment.Diagonal < this.MaxDiameter)
          {
            segments.Add(segment);
          }
        }
      }

      if (segments.Count > 0)
      {
        // Sort (only pick the largest for now)
        return segments.OrderByDescending(s => s.DiagonalSq).FirstOrDefault();

        // segments.OrderByDescending(s => s.Center.X);
        // VectorInt lastCenter = VectorInt.Zero;
        // for (int i = 0; i < segments.Count; i++)
        // {
        // if (segments[i].Center.Equals(lastCenter))
        // {
        // continue;
        // }
        // else
        // {
        // groupedSegments.Add(segments[i]);
        // lastCenter = segments[i].Center;
        // }
        // }

        // groupedSegments.OrderByDescending(s => s.DiagonalSq);

        //// Prevent jittering: 
        //// If the position doesn't change too much over a certain timespan, the last segment is always returned.

        //// Check for jittering position.
        // bool isJittering = true;
        // if ((lastSegment.Min - foundSegment.Min).LengthSq < jitteringAmountThresholdSq)
        // {
        // // Check for jittering size
        // if (Math.Abs(foundSegment.DiagonalSq - lastSegment.DiagonalSq) < jitteringAmountThresholdSq)
        // {
        // jitteringCount++;
        // isJittering = false;
        // }
        // }

        //// Freeze or not
        // if (isJittering)
        // {
        // jitteringCount = 0;
        // }
        // if (jitteringCount < JitteringCountThreshold)
        // {
        // lastSegment = foundSegment;
        // }
      }
      else
      {
        return new Segment();
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get index below threshold.
    /// </summary>
    /// <param name="start">
    /// The start. 
    /// </param>
    /// <param name="step">
    /// The step. 
    /// </param>
    /// <param name="hist">
    /// The hist. 
    /// </param>
    /// <param name="threshold">
    /// The threshold. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    private int GetIndexBelowThreshold(int start, int step, int[] hist, int threshold)
    {
      int result = start, hi;
      for (int i = start; i < hist.Length && i > 0; i += step)
      {
        hi = hist[i];
        result = i;
        if (hi < threshold)
        {
          break;
        }
      }

      return result;
    }

    /// <summary>
    /// The get indices above threshold.
    /// </summary>
    /// <param name="start">
    /// The start. 
    /// </param>
    /// <param name="step">
    /// The step. 
    /// </param>
    /// <param name="hist">
    /// The hist. 
    /// </param>
    /// <param name="threshold">
    /// The threshold. 
    /// </param>
    /// <returns>
    /// The <see cref="List"/> . 
    /// </returns>
    private List<int> GetIndicesAboveThreshold(int start, int step, int[] hist, int threshold)
    {
      var result = new List<int>();
      int hi;
      for (int i = start; i < hist.Length && i > 0; i += step)
      {
        hi = hist[i];
        if (hi > threshold)
        {
          result.Add(i);
        }
      }

      return result;
    }

    #endregion

    // public void Reset()
    // {
    // //jitteringCount = 0;
    // lastSegment = new Segment(-100, -100, -200, -200);
    // }
  }
}