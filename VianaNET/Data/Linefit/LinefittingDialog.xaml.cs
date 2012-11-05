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
  public partial class LinefittingDialog
  {
    #region Fields
    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LinefittingDialog"/> class.
    /// </summary>
    /// <param name="negativeX">
    /// flag showing if at least one x-value is negativ. 
    /// </param>
    /// <param name="negativeY">
    /// flag showing if at least one y-value is negativ
    /// </param>
    /// <param name="initialRegression">
    /// regression type to start with. 
    /// </param>
    public LinefittingDialog(bool negativeX, bool negativeY, Regression initialRegression)
    {
      this.InitializeComponent();

      this.SelectedRegressionType = initialRegression;

      this.radioButtonPot.IsEnabled = (!negativeX) & (!negativeY);
      if ((!this.radioButtonPot.IsEnabled) & (this.SelectedRegressionType == Regression.Potenz))
      {
        this.SelectedRegressionType = Regression.Linear;
      }

      this.radioButtonLog.IsEnabled = !negativeX;
      if ((!this.radioButtonLog.IsEnabled) & (this.SelectedRegressionType == Regression.Logarithmisch))
      {
        this.SelectedRegressionType = Regression.Linear;
      }

      this.radioButtonExp.IsEnabled = !negativeY;
      if ((!this.radioButtonExp.IsEnabled) & (this.SelectedRegressionType == Regression.Exponentiell))
      {
        this.SelectedRegressionType = Regression.ExponentiellMitKonstante;
      }

      switch (this.SelectedRegressionType)
      {
        case Regression.Linear:
          this.radioButtonLin.IsChecked = true;
          break;
        case Regression.ExponentiellMitKonstante:
          this.radioButtonExpSpez.IsChecked = true;
          break;
        case Regression.Logarithmisch:
          this.radioButtonLog.IsChecked = true;
          break;
        case Regression.Potenz:
          this.radioButtonPot.IsChecked = true;
          break;
        case Regression.Quadratisch:
          this.radioButtonQuad.IsChecked = true;
          break;
        case Regression.Exponentiell:
          this.radioButtonExp.IsChecked = true;
          break;
        case Regression.Sinus:
          this.radioButtonSin.IsChecked = true;
          break;
        case Regression.SinusGedämpft:
          this.radioButtonSinExp.IsChecked = true;
          break;
        case Regression.Resonanz:
          this.radioButtonResonanz.IsChecked = true;
          break;
      }
    }

    #endregion

    /// <summary>
    /// Gets or sets the selected regression type.
    /// </summary>
    public Regression SelectedRegressionType { get; set; }

    #region Methods

    /// <summary>
    /// The cancel click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// The ok click.
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

    /// <summary>
    /// The button regress auswahl_checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ButtonRegressAuswahlChecked(object sender, RoutedEventArgs e)
    {
      if (sender == this.radioButtonLin)
      {
        this.SelectedRegressionType = Regression.Linear;
      }
      else if (sender == this.radioButtonExpSpez)
      {
        this.SelectedRegressionType = Regression.ExponentiellMitKonstante;
      }
      else if (sender == this.radioButtonLog)
      {
        this.SelectedRegressionType = Regression.Logarithmisch;
      }
      else if (sender == this.radioButtonPot)
      {
        this.SelectedRegressionType = Regression.Potenz;
      }
      else if (sender == this.radioButtonQuad)
      {
        this.SelectedRegressionType = Regression.Quadratisch;
      }
      else if (sender == this.radioButtonExp)
      {
        this.SelectedRegressionType = Regression.Exponentiell;
      }
      else if (sender == this.radioButtonSin)
      {
        this.SelectedRegressionType = Regression.Sinus;
      }
      else if (sender == this.radioButtonSinExp)
      {
        this.SelectedRegressionType = Regression.SinusGedämpft;
      }
      else if (sender == this.radioButtonResonanz)
      {
        this.SelectedRegressionType = Regression.Resonanz;
      }
      else if (sender == this.radioButtonFindBestRegress)
      {
          this.SelectedRegressionType = Regression.best;
      }
      else
      {
        this.SelectedRegressionType = Regression.Linear;
      }
    }

    #endregion

  }
}