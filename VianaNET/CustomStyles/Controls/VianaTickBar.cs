using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Collections;
using MS.Internal;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Data;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MS.Internal.KnownBoxes;

using MS.Win32;
using System.Windows.Controls.Primitives;
using VianaNET.CustomStyles.Types;

namespace VianaNET.CustomStyles.Controls
{
  public class VianaTickBar : TickBar
  {
    /// <summary>
    /// Draw ticks.
    /// Ticks can be draw in 8 diffrent ways depends on Placment property and IsDirectionReversed property.
    ///
    /// This function also draw selection-tick(s) if IsSelectionRangeEnabled is 'true' and
    /// SelectionStart and SelectionEnd are valid.
    ///
    /// The primary ticks (for Mininum and Maximum value) height will be 100% of TickBar's render size (use Width or Height
    /// depends on Placement property).
    ///
    /// The secondary ticks (all other ticks, including selection-tics) height will be 75% of TickBar's render size.
    ///
    /// Brush that use to fill ticks is specified by Shape.Fill property.
    ///
    /// Pen that use to draw ticks is specified by Shape.Pen property.
    /// </summary>
    protected override void OnRender(DrawingContext dc)
    {
      Size size = new Size(ActualWidth, ActualHeight);
      double range = Maximum - Minimum;
      double tickLen = 0.0d;  // Height for Primary Tick (for Mininum and Maximum value)
      double tickLen2;        // Height for Secondary Tick
      double logicalToPhysical = 1.0;
      double progression = 1.0d;
      Point startPoint = new Point(0d, 0d);
      Point endPoint = new Point(0d, 0d);

      // Take Thumb size in to account
      double halfReservedSpace = ReservedSpace * 0.5;

      switch (Placement)
      {
        case TickBarPlacement.Top:
          if (DoubleUtilities.GreaterThanOrClose(ReservedSpace, size.Width))
          {
            return;
          }
          size.Width -= ReservedSpace;
          tickLen = -size.Height;
          startPoint = new Point(halfReservedSpace, size.Height);
          endPoint = new Point(halfReservedSpace + size.Width, size.Height);
          logicalToPhysical = size.Width / range;
          progression = 1;
          break;

        case TickBarPlacement.Bottom:
          if (DoubleUtilities.GreaterThanOrClose(ReservedSpace, size.Width))
          {
            return;
          }
          size.Width -= ReservedSpace;
          tickLen = size.Height;
          startPoint = new Point(halfReservedSpace, 0d);
          endPoint = new Point(halfReservedSpace + size.Width, 0d);
          logicalToPhysical = size.Width / range;
          progression = 1;
          break;

        case TickBarPlacement.Left:
          if (DoubleUtilities.GreaterThanOrClose(ReservedSpace, size.Height))
          {
            return;
          }
          size.Height -= ReservedSpace;
          tickLen = -size.Width;
          startPoint = new Point(size.Width, size.Height + halfReservedSpace);
          endPoint = new Point(size.Width, halfReservedSpace);
          logicalToPhysical = size.Height / range * -1;
          progression = -1;
          break;

        case TickBarPlacement.Right:
          if (DoubleUtilities.GreaterThanOrClose(ReservedSpace, size.Height))
          {
            return;
          }
          size.Height -= ReservedSpace;
          tickLen = size.Width;
          startPoint = new Point(0d, size.Height + halfReservedSpace);
          endPoint = new Point(0d, halfReservedSpace);
          logicalToPhysical = size.Height / range * -1;
          progression = -1;
          break;
      };

      tickLen2 = tickLen * 0.75;

      // Invert direciton of the ticks
      if (IsDirectionReversed)
      {
        progression = -progression;
        logicalToPhysical *= -1;

        // swap startPoint & endPoint
        Point pt = startPoint;
        startPoint = endPoint;
        endPoint = pt;
      }

      Pen pen = new Pen(Fill, 0.1d);

      bool snapsToDevicePixels = SnapsToDevicePixels;
      DoubleCollection xLines = snapsToDevicePixels ? new DoubleCollection() : null;
      DoubleCollection yLines = snapsToDevicePixels ? new DoubleCollection() : null;

      // Is it Vertical?
      if ((Placement == TickBarPlacement.Left) || (Placement == TickBarPlacement.Right))
      {
        // Reduce tick interval if it is more than would be visible on the screen
        double interval = TickFrequency;
        if (interval > 0.0)
        {
          double minInterval = (Maximum - Minimum) / size.Height;
          if (interval < minInterval)
          {
            interval = minInterval;
          }
        }

        // Draw Min & Max tick
        dc.DrawLine(pen, startPoint, new Point(startPoint.X + tickLen, startPoint.Y));
        dc.DrawLine(pen, new Point(startPoint.X, endPoint.Y),
                         new Point(startPoint.X + tickLen, endPoint.Y));

        if (snapsToDevicePixels)
        {
          xLines.Add(startPoint.X);
          yLines.Add(startPoint.Y - 0.5);
          xLines.Add(startPoint.X + tickLen);
          yLines.Add(endPoint.Y - 0.5);
          xLines.Add(startPoint.X + tickLen2);
        }

        // This property is rarely set so let's try to avoid the GetValue
        // caching of the mutable default value
        //DoubleCollection ticks = null;
        //bool hasModifiers;
        //if (GetValueSource(TicksProperty, null, out hasModifiers)
        //    != BaseValueSourceInternal.Default || hasModifiers)
        //{
        DoubleCollection ticks = Ticks;
        //}

        // Draw ticks using specified Ticks collection
        if ((ticks != null) && (ticks.Count > 0))
        {
          for (int i = 0; i < ticks.Count; i++)
          {
            if (DoubleUtilities.LessThanOrClose(ticks[i], Minimum) || DoubleUtilities.GreaterThanOrClose(ticks[i], Maximum))
            {
              continue;
            }

            double adjustedTick = ticks[i] - Minimum;

            double y = adjustedTick * logicalToPhysical + startPoint.Y;
            dc.DrawLine(pen,
                new Point(startPoint.X, y),
                new Point(startPoint.X + tickLen2, y));

            if (snapsToDevicePixels)
            {
              yLines.Add(y - 0.5);
            }
          }
        }
        // Draw ticks using specified TickFrequency
        else if (interval > 0.0)
        {
          for (double i = interval; i < range; i += interval)
          {
            double y = i * logicalToPhysical + startPoint.Y;

            dc.DrawLine(pen,
                new Point(startPoint.X, y),
                new Point(startPoint.X + tickLen2, y));

            if (snapsToDevicePixels)
            {
              yLines.Add(y - 0.5);
            }
          }
        }

        // Draw Selection Ticks
        if (IsSelectionRangeEnabled)
        {
          double y0 = (SelectionStart - Minimum) * logicalToPhysical + startPoint.Y;
          Point pt0 = new Point(startPoint.X, y0);
          Point pt1 = new Point(startPoint.X + tickLen2, y0);
          Point pt2 = new Point(startPoint.X + tickLen2, y0 + Math.Abs(tickLen2) * progression);

          PathSegment[] segments = new PathSegment[] {
                        new LineSegment(pt2, true),
                        new LineSegment(pt0, true),
                    };
          PathGeometry geo = new PathGeometry(new PathFigure[] { new PathFigure(pt1, segments, true) });

          dc.DrawGeometry(Fill, pen, geo);

          y0 = (SelectionEnd - Minimum) * logicalToPhysical + startPoint.Y;
          pt0 = new Point(startPoint.X, y0);
          pt1 = new Point(startPoint.X + tickLen2, y0);
          pt2 = new Point(startPoint.X + tickLen2, y0 - Math.Abs(tickLen2) * progression);

          segments = new PathSegment[] {
                        new LineSegment(pt2, true),
                        new LineSegment(pt0, true),
                    };
          geo = new PathGeometry(new PathFigure[] { new PathFigure(pt1, segments, true) });
          dc.DrawGeometry(Fill, pen, geo);
        }
      }
      else  // Placement == Top || Placement == Bottom
      {
        // Reduce tick interval if it is more than would be visible on the screen
        double interval = TickFrequency;
        if (interval > 0.0)
        {
          double minInterval = (Maximum - Minimum) / size.Width;
          if (interval < minInterval)
          {
            interval = minInterval;
          }
        }

        // Draw Min & Max tick
        dc.DrawLine(pen, startPoint, new Point(startPoint.X, startPoint.Y + tickLen2));
        dc.DrawLine(pen, new Point(endPoint.X, startPoint.Y),
                         new Point(endPoint.X, startPoint.Y + tickLen2));

        if (snapsToDevicePixels)
        {
          xLines.Add(startPoint.X - 0.5);
          yLines.Add(startPoint.Y);
          xLines.Add(startPoint.X - 0.5);
          yLines.Add(endPoint.Y + tickLen);
          yLines.Add(endPoint.Y + tickLen2);
        }

        //// This property is rarely set so let's try to avoid the GetValue
        //// caching of the mutable default value
        //DoubleCollection ticks = null;
        //bool hasModifiers;
        //if (GetValueSource(TicksProperty, null, out hasModifiers)
        //    != BaseValueSourceInternal.Default || hasModifiers)
        //{
        DoubleCollection ticks = Ticks;
        //}

        // Draw ticks using specified Ticks collection
        if ((ticks != null) && (ticks.Count > 0))
        {
          for (int i = 0; i < ticks.Count; i++)
          {
            if (DoubleUtilities.LessThanOrClose(ticks[i], Minimum) || DoubleUtilities.GreaterThanOrClose(ticks[i], Maximum))
            {
              continue;
            }
            double adjustedTick = ticks[i] - Minimum;

            double x = adjustedTick * logicalToPhysical + startPoint.X;
            dc.DrawLine(pen,
                new Point(x, startPoint.Y),
                new Point(x, startPoint.Y + tickLen2));

            if (snapsToDevicePixels)
            {
              xLines.Add(x - 0.5);
            }
          }
        }
        // Draw ticks using specified TickFrequency
        else if (interval > 0.0)
        {
          for (double i = interval; i < range; i += interval)
          {
            double x = i * logicalToPhysical + startPoint.X;
            dc.DrawLine(pen,
                new Point(x, startPoint.Y),
                new Point(x, startPoint.Y + tickLen2));

            if (snapsToDevicePixels)
            {
              xLines.Add(x - 0.5);
            }
          }
        }

        // Draw Selection Ticks
        if (IsSelectionRangeEnabled)
        {
          double x0 = (SelectionStart - Minimum) * logicalToPhysical + startPoint.X;
          Point pt0 = new Point(x0, startPoint.Y + 1);
          Point pt1 = new Point(x0, startPoint.Y + tickLen);
          Point pt2 = new Point(x0 + Math.Abs(tickLen) * progression, startPoint.Y + tickLen);

          PathSegment[] segments = new PathSegment[] {
                        new LineSegment(pt2, true),
                        new LineSegment(pt0, true),
                    };
          PathGeometry geo = new PathGeometry(new PathFigure[] { new PathFigure(pt1, segments, true) });

          dc.DrawGeometry(Fill, pen, geo);

          x0 = (SelectionEnd - Minimum) * logicalToPhysical + startPoint.X;
          pt0 = new Point(x0, startPoint.Y + 1);
          pt1 = new Point(x0, startPoint.Y + tickLen);
          pt2 = new Point(x0 - Math.Abs(tickLen) * progression, startPoint.Y + tickLen);

          segments = new PathSegment[] {
                        new LineSegment(pt2, true),
                        new LineSegment(pt0, true),
                    };
          geo = new PathGeometry(new PathFigure[] { new PathFigure(pt1, segments, true) });
          dc.DrawGeometry(Fill, pen, geo);
        }
      }

      if (snapsToDevicePixels)
      {
        xLines.Add(ActualWidth);
        yLines.Add(ActualHeight);
        VisualXSnappingGuidelines = xLines;
        VisualYSnappingGuidelines = yLines;
      }
      return;
    }
  }
}
