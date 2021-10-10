// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterpolationOptionsDialog.xaml.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Interpolation
{
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  ///   The interpolation options dialog.
  /// </summary>
  public partial class InterpolationOptionsDialog
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the InterpolationOptionsDialog class.
    /// </summary>
    public InterpolationOptionsDialog()
    {
      this.InitializeComponent();
      this.UpdateUIWithFilter();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the choosen interpolation filter.
    /// </summary>
    public InterpolationFilter ChoosenInterpolationFilter
    {
      get => this.CurrentInterpolationFilter;

      set
      {
        this.CurrentInterpolationFilter = value;
        this.UpdateUIWithFilter();
      }
    }

    /// <summary>
    ///   Gets or sets the current interpolation filter.
    /// </summary>
    public InterpolationFilter CurrentInterpolationFilter { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Cancel click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// The dialog_ closing.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DialogClosing(object sender, CancelEventArgs e)
    {
      this.InterpolationFilterPropertyGrid.Children.Clear();
    }

    /// <summary>
    /// The interpolation filter combo_ selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void InterpolationFilterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      InterpolationFilter filter = (InterpolationFilter)this.InterpolationFilterCombo.SelectedItem;

      this.CurrentInterpolationFilter = filter;

      // Remove old property sets.
      this.InterpolationFilterPropertyGrid.Children.Clear();

      // Add custom property control
      this.InterpolationFilterPropertyGrid.Children.Add(filter.CustomUserControl);
    }

    /// <summary>
    /// The o k_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   The update ui with filter.
    /// </summary>
    private void UpdateUIWithFilter()
    {
      if (this.CurrentInterpolationFilter == null)
      {
        this.CurrentInterpolationFilter = InterpolationFilter.Filter[InterpolationFilterTypes.MovingAverage];
      }

      this.InterpolationFilterCombo.SelectedItem = this.CurrentInterpolationFilter;
    }

    #endregion
  }
}