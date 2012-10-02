// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaTickBar.cs" company="Freie Universität Berlin">
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
//   Interaction logic for MediaSlider.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Controls
{
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls.Primitives;
  using System.Windows.Media;

  /// <summary>
  ///   Interaction logic for MediaSlider.xaml
  /// </summary>
  public class MediaTickBar : TickBar
  {
    #region Methods

    /// <summary>
    /// The on render.
    /// </summary>
    /// <param name="dc">
    /// The dc. 
    /// </param>
    protected override void OnRender(DrawingContext dc)
    {
      var size = new Size(base.ActualWidth, base.ActualHeight);
      double num = this.Maximum - this.Minimum;
      var point = new Point(0, 0);
      var point2 = new Point(0, 0);
      dc.DrawRectangle(Brushes.Red, null, new Rect(size));
      double y = this.ReservedSpace * 0.5;
      FormattedText formattedText = null;
      double x = 0;
      for (int i = 0; i <= num; i += 10)
      {
        formattedText = new FormattedText(
          i.ToString(), 
          CultureInfo.GetCultureInfo("en-us"), 
          FlowDirection.LeftToRight, 
          new Typeface("Verdana"), 
          8, 
          Brushes.Black);
        if (this.Minimum == i)
        {
          x = 0;
        }
        else
        {
          x += this.ActualWidth / 10;
        }

        dc.DrawText(formattedText, new Point(x, 10));
      }
    }

    #endregion
  }
}