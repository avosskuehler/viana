// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LabeledTextBox.xaml.cs" company="Freie Universität Berlin">
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
//   The labeled text box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System;
  using System.Windows;
  using System.Windows.Controls;

  using VianaNET.Application;

  /// <summary>
  ///   The labeled text box.
  /// </summary>
  public partial class LabeledTextBox : UserControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="LabeledTextBox" /> class.
    /// </summary>
    public LabeledTextBox()
    {
      this.InitializeComponent();
      this.ElementTextBox.IsEnabled = this.IsChecked;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The checked changed.
    /// </summary>
    public event EventHandler CheckedChanged;

    /// <summary>
    ///   The text changed.
    /// </summary>
    public event EventHandler TextChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets a value indicating whether is checked.
    /// </summary>
    public bool IsChecked
    {
      get => this.ElementCheckBox.IsChecked();

      set
      {
        this.ElementCheckBox.IsChecked = value;
        this.OnCheckedChanged();
      }
    }

    /// <summary>
    ///   Gets or sets the label.
    /// </summary>
    public string Label
    {
      get => this.ElementCheckBox.Content as string;

      set => this.ElementCheckBox.Content = value;
    }

    /// <summary>
    ///   Gets or sets the text.
    /// </summary>
    public string Text
    {
      get => this.ElementTextBox.Text;

      set
      {
        this.ElementTextBox.Text = value;
        this.OnTextChanged();
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The element check box_ checked.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ElementCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      this.ElementTextBox.IsEnabled = this.ElementCheckBox.IsChecked();
      this.OnCheckedChanged();
    }

    /// <summary>
    /// The element text box_ got focus.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ElementTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      textBox.SelectAll();
    }

    /// <summary>
    /// The element text box_ text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ElementTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.OnTextChanged();
    }

    /// <summary>
    ///   The on checked changed.
    /// </summary>
    private void OnCheckedChanged()
    {
      this.CheckedChanged.InvokeEmpty(this);
    }

    /// <summary>
    ///   The on text changed.
    /// </summary>
    private void OnTextChanged()
    {
      this.TextChanged.InvokeEmpty(this);
    }

    #endregion
  }
}