// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculatorAndFktEditor.xaml.cs" company="Freie Universität Berlin">
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
//   Interaktionslogik für CalculatorAndFktEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Linefit
{
  using System.Reflection;
  using System.Resources;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  ///   Interaktionslogik für CalculatorAndFktEditor.xaml
  /// </summary>
  public partial class CalculatorAndFktEditor : Window
  {
    #region Fields

    /// <summary>
    ///   The ergebnis.
    /// </summary>
    public string ergebnis;

    /// <summary>
    ///   The art.
    /// </summary>
    private readonly TRechnerArt art;

    /// <summary>
    ///   The scanned fkt.
    /// </summary>
    private TFktTerm scannedFkt;

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
      var resMgr = new ResourceManager("VianaNET.Localization.Labels", Assembly.GetExecutingAssembly());
      this.textBox1.Text = string.Empty;
      this.ergebnis = string.Empty;
      this.art = modus;
      this.buttonTakeKonst.IsEnabled = true;
      if (modus == TRechnerArt.rechner)
      {
        this.Title = resMgr.GetString("CalculatorDialogTitleCalc");
        this.buttonX.Visibility = Visibility.Hidden;
        this.buttonFertig.Content = resMgr.GetString("CalculatorDialogButtonDoneCalc");
      }
      else
      {
        this.Title = resMgr.GetString("CalculatorDialogTitleFunctionEditor");
        this.buttonX.Visibility = Visibility.Visible;
        this.buttonFertig.Content = resMgr.GetString("CalculatorDialogButtonDoneFktEdit");
      }

      string s;
      for (k = 0; k < Constants.max_Anz_Konst; k++)
      {
        s = resMgr.GetString(Constants.konstante[k].titel);
        this.comboBox1.Items.Add(s);
      }

      this.textBox1.Focus();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The get funktion.
    /// </summary>
    /// <returns> The <see cref="TFktTerm" /> . </returns>
    public TFktTerm GetFunktion()
    {
      if (this.scannedFkt != null)
      {
        this.ergebnis = this.textBox1.Text;
        this.scannedFkt.name = this.ergebnis;
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
    /// The einfuegen.
    /// </summary>
    /// <param name="zStr">
    /// The z str. 
    /// </param>
    private void Einfuegen(string zStr)
    {
      int caretPos, selLen;
      caretPos = this.textBox1.CaretIndex;
      selLen = this.textBox1.SelectionLength;
      if (caretPos > this.textBox1.SelectionStart)
      {
        caretPos = this.textBox1.SelectionStart;
      }

      if (selLen > 0)
      {
        this.textBox1.Text = this.textBox1.Text.Remove(caretPos, selLen);
      }

      this.textBox1.Text = this.textBox1.Text.Insert(caretPos, zStr);
      this.textBox1.Focus();
      this.textBox1.SelectionStart = caretPos + zStr.Length;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button back_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonBack_Click(object sender, RoutedEventArgs e)
    {
      int caretPos, selLen, offset;
      if ((string)(sender as Button).Tag == "1")
      {
        offset = 0;
      }
      else
      {
        offset = -1;
      }

      caretPos = this.textBox1.CaretIndex + offset;
      selLen = this.textBox1.SelectionLength;
      if (caretPos >= 0)
      {
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
    }

    /// <summary>
    /// The button es c_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonESC_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    /// <summary>
    /// The button end_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonEnd_Click(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      this.textBox1.SelectionStart = this.textBox1.Text.Length;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button fertig_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonFertig_Click(object sender, RoutedEventArgs e)
    {
      this.ergebnis = this.textBox1.Text;
      this.DialogResult = true;
    }

    /// <summary>
    /// The button left_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonLeft_Click(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      if (this.textBox1.SelectionStart > 0)
      {
        this.textBox1.SelectionStart--;
      }

      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button pos 1_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonPos1_Click(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      this.textBox1.SelectionStart = 0;
      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button right_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonRight_Click(object sender, RoutedEventArgs e)
    {
      this.textBox1.Focus();
      if (this.textBox1.SelectionStart < this.textBox1.Text.Length)
      {
        this.textBox1.SelectionStart++;
      }

      this.textBox1.SelectionLength = 0;
    }

    /// <summary>
    /// The button take konst_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void buttonTakeKonst_Click(object sender, RoutedEventArgs e)
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
    private void buttonZiffer_Click(object sender, RoutedEventArgs e)
    {
      string opStr = "^+-*/";
      var zStr = (string)(sender as Button).Content;
      this.Einfuegen(zStr);
      if ((!this.buttonTakeKonst.IsEnabled) & (opStr.IndexOf(zStr) >= 0))
      {
        this.buttonTakeKonst.IsEnabled = true;
      }
    }

    /// <summary>
    /// The button_ fkt_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void button_Fkt_Click(object sender, RoutedEventArgs e)
    {
      string zStr = (sender as Button).Content + "(";
      this.Einfuegen(zStr);
    }

    /// <summary>
    /// The text box 1_ text changed_1.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void textBox1_TextChanged_1(object sender, TextChangedEventArgs e)
    {
      string formelStr = this.textBox1.Text;
      double wert = 0;

      // TFktTerm fx;
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
      }
      else
      {
        if (this.art == TRechnerArt.formelRechner)
        {
          this.buttonFertig.IsEnabled = true;
        }
        else
        {
          wert = aktParser.FktWert_Berechne(this.scannedFkt, -1);
          this.textBoxErgebnis.Text = wert.ToString();
        }
      }
    }

    #endregion
  }
}