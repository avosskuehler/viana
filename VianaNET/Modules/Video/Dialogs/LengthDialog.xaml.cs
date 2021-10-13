// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthDialog.xaml.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   The length dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.Video.Dialogs
{
  using System.Windows;
  using VianaNET.CustomStyles.Types;
  using VianaNET.MainWindow;

  /// <summary>
  ///   The length dialog.
  /// </summary>
  public partial class LengthDialog
  {


    /// <summary>
    /// Initializes a new instance of the <see cref="LengthDialog" /> class. 
    /// </summary>
    public LengthDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.txbLength.Focus();
    }



    public string UnitValue { get; set; }



    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
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
    private void OK_Click(object sender, RoutedEventArgs e)
    {
      string addOn = string.Empty;

      if (double.TryParse(this.txbLength.Text, out double result))
      {
        if (this.rdbKM.IsChecked.GetValueOrDefault(false))
        {
          App.Project.CalibrationData.RulerUnit = LengthUnit.km;
        }
        else if (this.rdbM.IsChecked.GetValueOrDefault(false))
        {
          App.Project.CalibrationData.RulerUnit = LengthUnit.m;
        }
        else if (this.rdbCM.IsChecked.GetValueOrDefault(false))
        {
          App.Project.CalibrationData.RulerUnit = LengthUnit.cm;
        }
        else if (this.rdbMM.IsChecked.GetValueOrDefault(false))
        {
          App.Project.CalibrationData.RulerUnit = LengthUnit.mm;
        }

        // This line is necessary to get an update event for the ruler value
        // even if only the ruler unit was changed
        App.Project.CalibrationData.RulerValueInRulerUnits = -1;
        App.Project.CalibrationData.RulerValueInRulerUnits = result;

        this.DialogResult = true;
        this.Close();
      }
      else
      {
        VianaDialog dlg = new VianaDialog(
          VianaNET.Localization.Labels.CalibrationErrorTitle,
          VianaNET.Localization.Labels.CalibrationErrorDescription,
          VianaNET.Localization.Labels.CalibrationErrorMessage,
          true);

        dlg.ShowDialog();
      }
    }


  }
}