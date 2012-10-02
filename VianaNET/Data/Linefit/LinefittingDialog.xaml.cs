// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinefittingDialog.xaml.cs" company="Freie Universität Berlin">
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
// <author>Herwig Niemeyer</author>
// <email>hn_muenster@web.de</email>
// <summary>
//   Interaktionslogik für LinefittingDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Linefit
{
  using System.Windows;

  /// <summary>
  ///   Interaktionslogik für LinefittingDialog.xaml
  /// </summary>
  public partial class LinefittingDialog : Window
  {
    #region Fields

    /// <summary>
    ///   The auswahl.
    /// </summary>
    private int auswahl;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LinefittingDialog"/> class.
    /// </summary>
    /// <param name="xNeg">
    /// The x neg. 
    /// </param>
    /// <param name="yNeg">
    /// The y neg. 
    /// </param>
    /// <param name="startWahl">
    /// The start wahl. 
    /// </param>
    public LinefittingDialog(bool xNeg, bool yNeg, int startWahl)
    {
      this.InitializeComponent();
      if (startWahl == 0)
      {
        this.auswahl = Constants.linReg;
      }
      else
      {
        this.auswahl = startWahl;
      }

      this.radioButtonPot.IsEnabled = (!xNeg) & (!yNeg);
      if ((!this.radioButtonPot.IsEnabled) & (this.auswahl == Constants.potReg))
      {
        this.auswahl = Constants.linReg;
      }

      this.radioButtonLog.IsEnabled = !xNeg;
      if ((!this.radioButtonLog.IsEnabled) & (this.auswahl == Constants.logReg))
      {
        this.auswahl = Constants.linReg;
      }

      this.radioButtonExp.IsEnabled = !yNeg;
      if ((!this.radioButtonExp.IsEnabled) & (this.auswahl == Constants.expReg))
      {
        this.auswahl = Constants.expSpezReg;
      }

      switch (this.auswahl)
      {
        case Constants.linReg:
          this.radioButtonLin.IsChecked = true;
          break;
        case Constants.expSpezReg:
          this.radioButtonExpSpez.IsChecked = true;
          break;
        case Constants.logReg:
          this.radioButtonLog.IsChecked = true;
          break;
        case Constants.potReg:
          this.radioButtonPot.IsChecked = true;
          break;
        case Constants.quadReg:
          this.radioButtonQuad.IsChecked = true;
          break;
        case Constants.expReg:
          this.radioButtonExp.IsChecked = true;
          break;
        case Constants.sinReg:
          this.radioButtonSin.IsChecked = true;
          break;
        case Constants.sinExpReg:
          this.radioButtonSinExp.IsChecked = true;
          break;
        case Constants.resoReg:
          this.radioButtonResonanz.IsChecked = true;
          break;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The get auswahl.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    public int GetAuswahl()
    {
      return this.auswahl;
    }

    #endregion

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
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// The button regress auswahl_checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonRegressAuswahl_checked(object sender, RoutedEventArgs e)
    {
      if (sender == this.radioButtonLin)
      {
        this.auswahl = Constants.linReg;
      }
      else if (sender == this.radioButtonExpSpez)
      {
        this.auswahl = Constants.expSpezReg;
      }
      else if (sender == this.radioButtonLog)
      {
        this.auswahl = Constants.logReg;
      }
      else if (sender == this.radioButtonPot)
      {
        this.auswahl = Constants.potReg;
      }
      else if (sender == this.radioButtonQuad)
      {
        this.auswahl = Constants.quadReg;
      }
      else if (sender == this.radioButtonExp)
      {
        this.auswahl = Constants.expReg;
      }
      else if (sender == this.radioButtonSin)
      {
        this.auswahl = Constants.sinReg;
      }
      else if (sender == this.radioButtonSinExp)
      {
        this.auswahl = Constants.sinExpReg;
      }
      else if (sender == this.radioButtonResonanz)
      {
        this.auswahl = Constants.resoReg;
      }
      else
      {
        this.auswahl = Constants.linReg;
      }
    }

    #endregion
  }
}