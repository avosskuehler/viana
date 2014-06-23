// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericalPrecisionDialog.xaml.cs" company="Freie Universität Berlin">
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
// <summary>
//   The interpolation options dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter
{
  using System.Windows;

  /// <summary>
  ///   The interpolation options dialog.
  /// </summary>
  public partial class NumericalPrecisionDialog
  {
    #region Fields

    /// <summary>
    /// The big number.
    /// </summary>
    private readonly double bigNumber;

    /// <summary>
    /// The small number.
    /// </summary>
    private readonly double smallNumber;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericalPrecisionDialog" /> class.
    /// </summary>
    public NumericalPrecisionDialog()
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.smallNumber = 123456789E-12;
      this.bigNumber = 98765432109E-5;
      this.LabelBigNumber.Content = this.bigNumber.ToString("G0");
      this.LabelSmallNumber.Content = this.smallNumber.ToString("G0");
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the number of digits.
    /// </summary>
    public int NumberOfDigits { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Closes the dialog.
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
    /// Closes the dialog with DialogResult.Ok
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
    /// The slider value_ value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void SliderValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      int n = this.NumberOfDigits;
      string formatStr1 = string.Concat("G", n.ToString());
      string formatStr2 = string.Concat("G", n.ToString());

      // string formatStr2 = string.Concat("N", n.ToString());
      this.LabelFormatSmallNumber.Content = this.smallNumber.ToString(formatStr1);
      this.LabelFormatBigNumber.Content = this.bigNumber.ToString(formatStr2);
    }

    #endregion
  }
}