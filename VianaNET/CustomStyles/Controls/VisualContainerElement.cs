// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualContainerElement.cs" company="Freie Universität Berlin">
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
// <summary>
//   Defines the VisualContainerElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  /// The visual container element.
  /// </summary>
  public class VisualContainerElement : FrameworkElement
  {
    #region Fields

    /// <summary>
    /// The visual.
    /// </summary>
    private DrawingVisual visual;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualContainerElement"/> class.
    /// </summary>
    public VisualContainerElement()
    {
      this.visual = null;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the visual.
    /// </summary>
    public DrawingVisual Visual
    {
      get
      {
        return this.visual;
      }

      set
      {
        this.RemoveVisualChild(this.visual);
        this.visual = value;
        this.AddVisualChild(this.visual);

        this.InvalidateMeasure();
        this.InvalidateVisual();
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the visual children count.
    /// </summary>
    protected override int VisualChildrenCount
    {
      get
      {
        return 1;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get visual child.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// The <see cref="Visual"/>.
    /// </returns>
    protected override Visual GetVisualChild(int index)
    {
      return this.visual;
    }

    /// <summary>
    /// The measure override.
    /// </summary>
    /// <param name="availableSize">
    /// The available size.
    /// </param>
    /// <returns>
    /// The <see cref="Size"/>.
    /// </returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.visual != null)
      {
        return this.visual.ContentBounds.Size;
      }

      return base.MeasureOverride(availableSize);
    }

    #endregion
  }
}