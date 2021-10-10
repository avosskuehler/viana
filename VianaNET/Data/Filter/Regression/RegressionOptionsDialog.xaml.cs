// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegressionControl.xaml.cs" company="Freie Universität Berlin">
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
//   Interaktionslogik für RegressiontingDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Regression
{
  using System.Windows;

  /// <summary>
  ///   Interaktionslogik für RegressionControl.xaml
  /// </summary>
  public partial class RegressionOptionsDialog
  {
    #region Fields
      public int negFlag;
    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RegressionOptionsDialog"/> class.
    /// </summary>
    /// <param name="regressionFilter">
    /// The regression Filter.
    /// </param>
    public RegressionOptionsDialog(RegressionFilter regressionFilter)
    {
      this.InitializeComponent();
      this.RegressionType = regressionFilter.RegressionType;

      double minX, minY, hilf;
      regressionFilter.GetMinMax(regressionFilter.WertX, out minX, out hilf);
      regressionFilter.GetMinMax(regressionFilter.WertY, out minY, out hilf);
      bool negativeX = minX < 0;
      bool negativeY = minY < 0;
      this.negFlag = 0;
      if (negativeX) { this.negFlag = 1; }
      if (negativeY) { this.negFlag = this.negFlag +2; }
      this.radioButtonPot.IsEnabled = (!negativeX) & (!negativeY);
      if ((!this.radioButtonPot.IsEnabled) & (this.RegressionType == RegressionType.Potenz))
      {
        this.RegressionType = RegressionType.Linear;
      }

      this.radioButtonLog.IsEnabled = !negativeX;
      if ((!this.radioButtonLog.IsEnabled) & (this.RegressionType == RegressionType.Logarithmisch))
      {
        this.RegressionType = RegressionType.Linear;
      }

      this.radioButtonExp.IsEnabled = !negativeY;
      if ((!this.radioButtonExp.IsEnabled) & (this.RegressionType == RegressionType.Exponentiell))
      {
        this.RegressionType = RegressionType.ExponentiellMitKonstante;
      }

      switch (this.RegressionType)
      {
        case RegressionType.Linear:
          this.radioButtonLin.IsChecked = true;
          break;
        case RegressionType.ExponentiellMitKonstante:
          this.radioButtonExpSpez.IsChecked = true;
          break;
        case RegressionType.Logarithmisch:
          this.radioButtonLog.IsChecked = true;
          break;
        case RegressionType.Potenz:
          this.radioButtonPot.IsChecked = true;
          break;
        case RegressionType.Quadratisch:
          this.radioButtonQuad.IsChecked = true;
          break;
        case RegressionType.Exponentiell:
          this.radioButtonExp.IsChecked = true;
          break;
        case RegressionType.Sinus:
          this.radioButtonSin.IsChecked = true;
          break;
        case RegressionType.SinusGedämpft:
          this.radioButtonSinExp.IsChecked = true;
          break;
        case RegressionType.Resonanz:
          this.radioButtonResonanz.IsChecked = true;
          break;
      }
    }

    #endregion

    /// <summary>
    ///   Gets the filter.
    /// </summary>
    public RegressionType RegressionType { get; private set; }

    #region Methods

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
        this.RegressionType = RegressionType.Linear;
      }
      else if (sender == this.radioButtonExpSpez)
      {
        this.RegressionType = RegressionType.ExponentiellMitKonstante;
      }
      else if (sender == this.radioButtonLog)
      {
        this.RegressionType = RegressionType.Logarithmisch;
      }
      else if (sender == this.radioButtonPot)
      {
        this.RegressionType = RegressionType.Potenz;
      }
      else if (sender == this.radioButtonQuad)
      {
        this.RegressionType = RegressionType.Quadratisch;
      }
      else if (sender == this.radioButtonExp)
      {
        this.RegressionType = RegressionType.Exponentiell;
      }
      else if (sender == this.radioButtonSin)
      {
        this.RegressionType = RegressionType.Sinus;
      }
      else if (sender == this.radioButtonSinExp)
      {
        this.RegressionType = RegressionType.SinusGedämpft;
      }
      else if (sender == this.radioButtonResonanz)
      {
        this.RegressionType = RegressionType.Resonanz;
      }
      else if (sender == this.radioButtonFindBestRegress)
      {
        this.RegressionType = RegressionType.Best;
      }
      else
      {
        this.RegressionType = RegressionType.Linear;
      }
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

    /// <summary>
    /// The cancel_ click.
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
    #endregion
  }
}