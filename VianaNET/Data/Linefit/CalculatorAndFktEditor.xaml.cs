using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Parser;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Reflection;


//  letzte Änderung: 30.9.2012

namespace VianaNET.Data.Linefit
{
    /// <summary>
    /// Interaktionslogik für CalculatorAndFktEditor.xaml
    /// </summary>
    public partial class CalculatorAndFktEditor : Window
    {
        public string ergebnis;
        private TRechnerArt art;
        private TFktTerm scannedFkt;

        public CalculatorAndFktEditor(TRechnerArt modus)
        {
            int k;
            InitializeComponent();
            ResourceManager resMgr = new ResourceManager("VianaNET.Localization.Labels", Assembly.GetExecutingAssembly());
            textBox1.Text = "";
            ergebnis = "";
            art = modus;
            buttonTakeKonst.IsEnabled = true;
            if (modus == TRechnerArt.rechner)
            {
                this.Title = resMgr.GetString("CalculatorDialogTitleCalc");   
                buttonX.Visibility = Visibility.Hidden;
                buttonFertig.Content = resMgr.GetString("CalculatorDialogButtonDoneCalc");  
            }
            else
            {
                this.Title = resMgr.GetString("CalculatorDialogTitleFunctionEditor"); 
                buttonX.Visibility = Visibility.Visible;
                buttonFertig.Content = resMgr.GetString("CalculatorDialogButtonDoneFktEdit"); 
            }

            String s;
            for (k = 0; k < Constants.max_Anz_Konst; k++)
            {
              s = resMgr.GetString(Constants.konstante[k].titel);
              comboBox1.Items.Add(s);
            }
            textBox1.Focus();
        }

        private void Einfuegen(string zStr)
        {
            int caretPos, selLen;
            caretPos = textBox1.CaretIndex;
            selLen = textBox1.SelectionLength;
            if (caretPos > textBox1.SelectionStart) { caretPos = textBox1.SelectionStart; }
            if (selLen > 0) { textBox1.Text = textBox1.Text.Remove(caretPos, selLen); }
            textBox1.Text = textBox1.Text.Insert(caretPos, zStr);
            textBox1.Focus();
            textBox1.SelectionStart = caretPos + zStr.Length;
            textBox1.SelectionLength = 0;
        }


        private void buttonZiffer_Click(object sender, RoutedEventArgs e)
        { 
            string opStr = "^+-*/";
            string zStr = (string)(sender as Button).Content;
            Einfuegen(zStr);
            if ((!buttonTakeKonst.IsEnabled)&( opStr.IndexOf(zStr)>=0))
            {
                buttonTakeKonst.IsEnabled = true;
            }
        }

        private void button_Fkt_Click(object sender, RoutedEventArgs e)
        {
            string zStr = (sender as Button).Content + "(";
            Einfuegen(zStr);
        }


        private void textBox1_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string formelStr = textBox1.Text;
            double wert = 0;
            // TFktTerm fx;
            Parse aktParser = new Parse();
            scannedFkt = null;
            aktParser.ScanFkt(ref scannedFkt, formelStr);
            if (aktParser.lastErrNr > 0)
            {
                if (art == TRechnerArt.formelRechner) { buttonFertig.IsEnabled = false; }
                else { textBoxErgebnis.Text = ""; }
            }
            else
            {
                if (art == TRechnerArt.formelRechner) { buttonFertig.IsEnabled = true; }
                else
                {
                    wert = aktParser.FktWert_Berechne(scannedFkt, -1);
                    textBoxErgebnis.Text = wert.ToString();
                }
            }
        }



        private void buttonPos1_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
            if (textBox1.SelectionStart > 0) { textBox1.SelectionStart--; }
            textBox1.SelectionLength = 0;
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
            if (textBox1.SelectionStart < textBox1.Text.Length) { textBox1.SelectionStart++; }
            textBox1.SelectionLength = 0;
        }

        private void buttonEnd_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            int caretPos, selLen, offset;
            if ((string)(sender as Button).Tag == "1") { offset = 0; }
            else { offset = -1; }

            caretPos = textBox1.CaretIndex + offset;
            selLen = textBox1.SelectionLength;
            if (caretPos >= 0)
            {
                if (selLen == 0)
                {
                    if ((textBox1.CaretIndex < textBox1.Text.Length)|(offset==-1))
                    { textBox1.Text = textBox1.Text.Remove(caretPos, 1); }
                }
                else
                {
                    if (caretPos > textBox1.SelectionStart)
                    {
                        caretPos = textBox1.SelectionStart + offset;
                    }
                    textBox1.Text = textBox1.Text.Remove(caretPos, selLen);
                }
                textBox1.Focus();
                textBox1.SelectionStart = caretPos;
                textBox1.SelectionLength = 0;
            }
        }



        public TFktTerm GetFunktion()
        {
            if (scannedFkt!=null)
            {
                ergebnis = textBox1.Text;
                scannedFkt.name = ergebnis;       
            }
            else { scannedFkt = null; }
            return scannedFkt;
        }



        private void buttonTakeKonst_Click(object sender, RoutedEventArgs e)
        {
            int k = comboBox1.SelectedIndex;
            if (k >= 0)
            {
                Einfuegen(Constants.konstante[k].bez);
                buttonTakeKonst.IsEnabled = false;
            }
        }

        private void buttonFertig_Click(object sender, RoutedEventArgs e)
        {
            ergebnis = textBox1.Text;
            DialogResult = true;
        }

        private void buttonESC_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
  
    }


}
