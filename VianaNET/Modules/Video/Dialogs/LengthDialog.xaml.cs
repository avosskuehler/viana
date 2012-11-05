// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The length dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Dialogs
{
  using System.Windows;

  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Localization;
  using VianaNET.MainWindow;

  /// <summary>
  ///   The length dialog.
  /// </summary>
  public partial class LengthDialog : Window
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="LengthDialog" /> class. 
    ///   Initializes a new instance of the MainWindow class.
    /// </summary>
    public LengthDialog()
    {
      this.InitializeComponent();
      this.txbLength.Focus();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

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
      double result;
      string addOn = string.Empty;
      if (double.TryParse(this.txbLength.Text, out result))
      {
        if (this.rdbKM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.km;
        }
        else if (this.rdbM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.m;
        }
        else if (this.rdbCM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.cm;
        }
        else if (this.rdbMM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.mm;
        }

        // This line is necessary to get an update event for the ruler value
        // even if only the ruler unit was changed
        Calibration.Instance.RulerValueInRulerUnits = -1;
        Calibration.Instance.RulerValueInRulerUnits = result;

        this.DialogResult = true;
        this.Close();
      }
      else
      {
        var dlg = new VianaDialog(
          Labels.CalibrationErrorTitle, Labels.CalibrationErrorDescription, Labels.CalibrationErrorMessage);
        dlg.ShowDialog();
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}