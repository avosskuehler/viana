// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualContainerElement.cs" company="Freie Universität Berlin">
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
// <summary>
//   Defines the VisualContainerElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET
{
  using System.Windows;
  using System.Windows.Media;

  public class VisualContainerElement : FrameworkElement
  {
    private DrawingVisual visual;

    public VisualContainerElement()
      : base()
    {
      this.visual = null;
    }

    public DrawingVisual Visual
    {
      get { return this.visual; }
      set
      {
        RemoveVisualChild(this.visual);
        this.visual = value;
        AddVisualChild(this.visual);

        InvalidateMeasure();
        InvalidateVisual();
      }
    }

    protected override int VisualChildrenCount
    {
      get { return 1; }
    }

    protected override Visual GetVisualChild(int index)
    {
      return this.visual;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.visual != null)
      {
        return this.visual.ContentBounds.Size;
      }

      return base.MeasureOverride(availableSize);
    }
  }
}
