// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculatorAndFktEditor.xaml.cs" company="Freie Universität Berlin">
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
// <author>Herwig Niemeyer</author>
// <email>hn_muenster@web.de</email>
// <summary>
//   Interaktionslogik für CalculatorAndFktEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Theory
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;

  using VianaNET.Resources;

  using WPFLocalizeExtension.Extensions;

  /// <summary>
  ///   Interaktionslogik für CalculatorAndFktEditor.xaml
  /// </summary>
  public partial class CalculatorAndFktEditor
  {
    #region Fields

    /// <summary>
    /// The uses decimalkomma.
    /// </summary>
    public readonly bool usesDecimalkomma;

    /// <summary>
    ///   The art.
    /// </summary>
    private readonly TRechnerArt art;

    /// <summary>
    ///   The scanned function - coded in a tree.
    /// </summary>
    private FunctionCalcTree scannedFkt;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculatorAndFktEditor"/> class.
    /// </summary>
    /// <param name="modus">
    /// The modus.
    /// </param>
    public CalculatorAndFktEditor(TRechnerArt modus)
    {
      int k;
      this.InitializeComponent();
      this.textBox1.Text = string.Empty;
      this.Ergebnis = string.Empty;
      this.art = modus;
      this.buttonTakeKonst.IsEnabled = true;
      this.usesDecimalkomma = 15 == Convert.ToDouble("1.5");
      if (modus == TRechnerArt.rechner)
      {
        this.Title = Labels.CalculatorDialogTitleCalc;
        this.buttonX.Visibility = Visibility.Hidden;
        this.buttonFertig.Content = Labels.CalculatorDialogButtonDoneCalc;
      }
      else
      {
        this.Title = Labels.CalculatorDialogTitleFunctionEditor;
        this.buttonX.Visibility = Visibility.Visible;
        this.buttonFertig.Content = Labels.CalculatorDialogButtonDoneFktEdit;
      }

      for (k = 0; k < Constants.konstante.Length; k++)
      {
        string uiString;
        var locExtension = new LocExtension("VianaNET:Labels:" + Constants.konstante[k].titel);
        locExtension.ResolveLocalizedValue(out uiString);
        this.comboBox1.Items.Add(uiString);
      }

      this.buttonKomma.Content = this.usesDecimalkomma ? "," : ".";
      this.textBox1.Focus();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the ergebnis.
    /// </summary>
    public string Ergebnis { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The get funktion.
    /// </summary>
    /// <returns> The <see cref="CalculatorFunctionTerm" /> . </returns>
    public FunctionCalcTree GetFunktion()
    {
      if (this.scannedFkt != null)
      {
        this.Ergebnis = this.textBox1.Text;
        this.scannedFkt.Name = this.Ergebnis;
      }
      else
      {
        this.scannedFkt = null;
      }

      return this.scannedFkt;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The button back_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonBackClick(object sender, RoutedEventArgs e)
    {
      int offset;
      var button = sender as Button;

      if (button == null)
      {
        return;
      }

      if ((string)button.Tag == "1")
      {
        offset = 0;
      }
      else
      {
        offset = -1;
      }

      int caretPos = this.textBox1.CaretIndex + offset;
      int selLen = this.textBox1.SelectionLength;
      if (caretPos < 0)
      {
        return;
      }

      if (selLen == 0)
      {
        if ((this.textBox1.CaretIndex < this.textBox1.Text.Length) | (offset == -1))
        {
          this.textBox1.Text = this.textBox1.Text.Remove(caretPos, 1);
        }
      }
      else
      {
        if (caretPos > this.textBox1.SelectionStart)
        {
          caretPos = this.textBox1.SelectionStart + offset;
        }

        this.textBox1.Text = this.textBox1.Text.Remove(caretPos, selLen);
      }

      this.textBox1.Focus();
      this.textBox1.SelectionStart = caretPos;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button END  click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonEndClick(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      this.textBox1.SelectionStart = this.textBox1.Text.Length;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button ESC click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonEscClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    /// <summary>
    /// The button Done click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonFertigClick(object sender, RoutedEventArgs e)
    {
      this.Ergebnis = this.textBox1.Text;
      this.DialogResult = true;
    }

    /// <summary>
    /// The button insert functionstring click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonFktClick(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      if (button == null)
      {
        return;
      }

      string stringValue = button.Content + "(";
      this.Einfuegen(stringValue);
    }

    /// <summary>
    /// The button left click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonLeftClick(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      if (this.textBox1.SelectionStart > 0)
      {
        this.textBox1.SelectionStart--;
      }

      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button POS1 click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonPos1Click(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      this.textBox1.SelectionStart = 0;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button right click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonRightClick(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      if (this.textBox1.SelectionStart < this.textBox1.Text.Length)
      {
        this.textBox1.SelectionStart++;
      }

      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button take constant click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonTakeKonstClick(object sender, RoutedEventArgs e)
    {
      int k = this.comboBox1.SelectedIndex;
      if (k >= 0)
      {
        this.Einfuegen(Constants.konstante[k].bez);
        this.buttonTakeKonst.IsEnabled = false;
      }
    }

    /// <summary>
    /// The button ziffer_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ButtonZifferClick(object sender, RoutedEventArgs e)
    {
      const string OpStr = "^+-*/";
      var button = sender as Button;
      if (button == null)
      {
        return;
      }

      var stringValue = (string)button.Content;
      this.Einfuegen(stringValue);
      if ((!this.buttonTakeKonst.IsEnabled) & (OpStr.IndexOf(stringValue, StringComparison.Ordinal) >= 0))
      {
        this.buttonTakeKonst.IsEnabled = true;
      }
    }

    /// <summary>
    /// The einfuegen.
    /// </summary>
    /// <param name="insertString">
    /// The z str.
    /// </param>
    private void Einfuegen(string insertString)
    {
      int caretPos = this.textBox1.CaretIndex;
      int selLen = this.textBox1.SelectionLength;
      if (caretPos > this.textBox1.SelectionStart)
      {
        caretPos = this.textBox1.SelectionStart;
      }

      if (selLen > 0)
      {
        this.textBox1.Text = this.textBox1.Text.Remove(caretPos, selLen);
      }

      this.textBox1.Text = this.textBox1.Text.Insert(caretPos, insertString);
      this.textBox1.Focus();
      this.textBox1.SelectionStart = caretPos + insertString.Length;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The textbox1 text changed_1.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void TextBox1TextChanged1(object sender, TextChangedEventArgs e)
    {
      string formelStr = this.textBox1.Text;
      if (this.usesDecimalkomma)
      {
        formelStr = formelStr.Replace('.', ',');
      }
      else
      {
        formelStr = formelStr.Replace(',', '.');
      }

      string hilfStr = string.Empty;
      var aktParser = new Parse();
      this.scannedFkt = null;
      aktParser.ScanFkt(ref this.scannedFkt, formelStr);
      if (aktParser.lastErrNr > 0)
      {
        if (this.art == TRechnerArt.formelRechner)
        {
          this.buttonFertig.IsEnabled = false;
        }
        else
        {
          this.textBoxErgebnis.Text = string.Empty;
        }

        aktParser.ErrMsg(aktParser.lastErrNr, ref hilfStr);

        // Fehlerposition anzeigen mit ^
        hilfStr = string.Concat("^ ", hilfStr);
        for (int i = 1; i <= aktParser.lastErrPos; i++)
        {
          hilfStr = string.Concat(" ", hilfStr);
        }

        this.textBoxErgebnis.Text = hilfStr;
      }
      else
      {
        if (this.art == TRechnerArt.formelRechner)
        {
          this.buttonFertig.IsEnabled = true;
          this.textBoxErgebnis.Text = string.Empty;
        }
        else
        {
          double wert = aktParser.FktWert_Berechne(this.scannedFkt, -1);
          this.textBoxErgebnis.Text = wert.ToString(CultureInfo.InvariantCulture);
        }
      }
    }

    #endregion
  }
}