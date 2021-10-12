// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LabeledSlider.xaml.cs" company="Freie Universität Berlin">
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
//   The labeled slider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;

  using VianaNET.Application;

  /// <summary>
  ///   The labeled slider.
  /// </summary>
  public partial class LabeledSlider : UserControl
  {


    /// <summary>
    ///   The _decimals.
    /// </summary>
    private int _decimals;

    /// <summary>
    ///   The _value.
    /// </summary>
    private double _value;





    /// <summary>
    ///   Initializes a new instance of the <see cref="LabeledSlider" /> class.
    /// </summary>
    public LabeledSlider()
    {
      this.InitializeComponent();
      this.ElementSlider.IsEnabled = !this.IsCheckable || this.IsChecked;
      this.OnValueChanged(true);
    }





    /// <summary>
    ///   The checked changed.
    /// </summary>
    public event EventHandler CheckedChanged;

    /// <summary>
    ///   The value changed.
    /// </summary>
    public event EventHandler ValueChanged;





    /// <summary>
    ///   Gets or sets the decimals.
    /// </summary>
    public int Decimals
    {
      get => this._decimals;

      set
      {
        this._decimals = value;
        this.OnValueChanged(false);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether is checkable.
    /// </summary>
    public bool IsCheckable
    {
      get => Visibility.Visible == this.ElementCheckBox.Visibility;

      set
      {
        this.ElementCheckBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        this.ElementSlider.IsEnabled = !this.IsCheckable || this.IsChecked;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether is checked.
    /// </summary>
    public bool IsChecked
    {
      get => this.ElementCheckBox.IsChecked();

      set => this.ElementCheckBox.IsChecked = value;
    }

    /// <summary>
    ///   Gets or sets the label.
    /// </summary>
    public string Label
    {
      get => this.ElementLabel.Text;

      set => this.ElementLabel.Text = value;
    }

    /// <summary>
    ///   Gets or sets the maximum.
    /// </summary>
    public double Maximum
    {
      get => this.ElementSlider.Maximum;

      set => this.ElementSlider.Maximum = value;
    }

    /// <summary>
    ///   Gets or sets the minimum.
    /// </summary>
    public double Minimum
    {
      get => this.ElementSlider.Minimum;

      set => this.ElementSlider.Minimum = value;
    }

    /// <summary>
    ///   Gets or sets the value.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", 
      Justification = "Not easily confused with DependencyObject.GetValue().")]
    public double Value
    {
      get => this._value;

      set => this.ElementSlider.Value = value;
    }





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
      this.ElementSlider.IsEnabled = this.ElementCheckBox.IsChecked();
      this.OnCheckedChanged();
    }

    /// <summary>
    /// The element slider_ value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void ElementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this.OnValueChanged(false);
    }

    /// <summary>
    ///   The on checked changed.
    /// </summary>
    private void OnCheckedChanged()
    {
      this.CheckedChanged.InvokeEmpty(this);
    }

    /// <summary>
    /// The on value changed.
    /// </summary>
    /// <param name="force">
    /// The force. 
    /// </param>
    private void OnValueChanged(bool force)
    {
      double oldValue = this._value;
      this._value = Math.Round(this.ElementSlider.Value, this._decimals);
      if ((oldValue != this._value) || force)
      {
        this.ElementValue.Text = this._value.ToString(CultureInfo.CurrentCulture);
        this.ValueChanged.InvokeEmpty(this);
      }
    }


  }
}