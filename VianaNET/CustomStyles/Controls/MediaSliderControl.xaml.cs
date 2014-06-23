// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSliderControl.xaml.cs" company="Freie Universität Berlin">
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
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   Interaction logic for MediaSliderControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System.Windows.Input;

  /// <summary>
  ///   Interaction logic for MediaSliderControl.xaml
  /// </summary>
  public partial class MediaSliderControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MediaSliderControl" /> class.
    /// </summary>
    public MediaSliderControl()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The media slider_ mouse left button down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void MediaSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    /// <summary>
    /// The media slider_ mouse left button up.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void MediaSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    /// <summary>
    /// The media slider_ mouse move.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void MediaSlider_MouseMove(object sender, MouseEventArgs e)
    {
    }

    #endregion
  }
}