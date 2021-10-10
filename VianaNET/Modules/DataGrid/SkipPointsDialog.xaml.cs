// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkipPointsDialog.xaml.cs" company="Freie Universität Berlin">
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
namespace VianaNET.Modules.DataGrid
{
  using System.Windows;

  /// <summary>
  ///   The length dialog.
  /// </summary>
  public partial class SkipPointsDialog
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SkipPointsDialog" /> class.
    /// </summary>
    public SkipPointsDialog()
    {
      this.InitializeComponent();
      this.UseEveryNthPointNumericUpDown.Focus();
      this.UseEveryNthPointNumericUpDown.Value = App.Project.VideoData.UseEveryNthPoint;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the Click event of the Cancel control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the Click event of the OK control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      App.Project.VideoData.UseEveryNthPoint = (int)this.UseEveryNthPointNumericUpDown.Value;
      this.Close();
    }

    #endregion
  }
}