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
using System.Windows.Shapes;
using Parser;



namespace VianaNet
{
    /// <summary>
    /// Interaktionslogik für CalculatorAndFunctionEditor.xaml
    /// </summary>
    public partial class CalculatorAndFunctionEditor : Window
    {

        public string ergebnis;
        private TRechnerArt art;
        private TFktTerm scannedFkt;

        public CalculatorAndFunctionEditor(TRechnerArt modus)
        {
            int k;
            InitializeComponent();
            textBox1.Text = "";
            ergebnis = "";
            art = modus;
            if (modus == TRechnerArt.rechner)
            {
                buttonX.Visibility = Visibility.Hidden;
                buttonFertig.Content = "beenden";
            }
            else
            {
                buttonX.Visibility = Visibility.Visible; 
                buttonFertig.Content = "übernehmen";
            }
            for (k = 0; k < Constants.max_Anz_Konst; k++)
            {
                comboBox1.Items.Add(Constants.konstante[k].titel);
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
            string zStr = (string)(sender as Button).Content;
            Einfuegen(zStr);
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
                { textBox1.Text = textBox1.Text.Remove(caretPos, 1); }
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
            ergebnis = textBox1.Text;
            scannedFkt.name = ergebnis;
            return scannedFkt;
        }

      

        private void buttonTakeKonst_Click(object sender, RoutedEventArgs e)
        {
            int k = comboBox1.SelectedIndex;
            if (k >= 0)
            {
                Einfuegen(Constants.konstante[k].bez);
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
