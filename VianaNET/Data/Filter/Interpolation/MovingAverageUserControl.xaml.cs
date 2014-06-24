// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MovingAverageControl.xaml.cs" company="Freie Universität Berlin">
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
//   The moving average user control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Interpolation
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  ///   The moving average user control.
  /// </summary>
  public partial class MovingAverageUserControl : UserControl
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the MovingAverageUserControl class.
    /// </summary>
    /// <param name="filter">
    /// The filter. 
    /// </param>
    public MovingAverageUserControl(MovingAverageFilter filter)
    {
      this.InitializeComponent();
      this.Filter = filter;
      this.NumberOfPointsNumeric.Value = this.Filter.NumberOfSamplesToInterpolate;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the filter.
    /// </summary>
    public MovingAverageFilter Filter { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the OnValueChanged event of the NumberOfPointsNumeric control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Object}"/> instance containing the event data.</param>
    private void NumberOfPointsNumeric_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      this.Filter.NumberOfSamplesToInterpolate = (int)this.NumberOfPointsNumeric.Value;
    }

    #endregion

  }
}