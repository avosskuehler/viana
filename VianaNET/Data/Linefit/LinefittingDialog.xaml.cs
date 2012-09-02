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

namespace VianaNET.Data.Linefit
{
    /// <summary>
    /// Interaktionslogik für LinefittingDialog.xaml
    /// </summary>
    public partial class LinefittingDialog : Window
    {
        int auswahl;

        public LinefittingDialog(bool xNeg, bool yNeg, int startWahl)
        {
            InitializeComponent();
            if (startWahl == 0) { auswahl = Constants.linReg; } 
            else {auswahl = startWahl;}
            radioButtonPot.IsEnabled = (!xNeg) & (!yNeg);
            if ((!radioButtonPot.IsEnabled) & (auswahl == Constants.potReg)) { auswahl = Constants.linReg; }
            radioButtonLog.IsEnabled = !xNeg;
            if ((!radioButtonLog.IsEnabled) & (auswahl == Constants.logReg)) { auswahl = Constants.linReg; }
            radioButtonExp.IsEnabled = !yNeg;
            if ((!radioButtonExp.IsEnabled) & (auswahl == Constants.expReg)) { auswahl = Constants.expSpezReg; }

            switch (auswahl)
            {
                case Constants.linReg: radioButtonLin.IsChecked = true; break;
                case Constants.expSpezReg: radioButtonExpSpez.IsChecked = true; break;
                case Constants.logReg: radioButtonLog.IsChecked = true; break;
                case Constants.potReg: radioButtonPot.IsChecked = true; break;
                case Constants.quadReg: radioButtonQuad.IsChecked = true; break;
                case Constants.expReg: radioButtonExp.IsChecked = true; break;
                case Constants.sinReg: radioButtonSin.IsChecked = true; break;
                case Constants.sinExpReg: radioButtonSinExp.IsChecked = true; break;
                case Constants.resoReg: radioButtonResonanz.IsChecked = true; break;
            }
        }


        private void buttonRegressAuswahl_checked(object sender, RoutedEventArgs e)
        {
            if (sender == radioButtonLin) { auswahl = Constants.linReg; } 
            else if (sender == radioButtonExpSpez) { auswahl = Constants.expSpezReg; }
            else if (sender == radioButtonLog)     { auswahl = Constants.logReg; }
            else if (sender == radioButtonPot)     { auswahl = Constants.potReg; }
            else if (sender == radioButtonQuad)    { auswahl = Constants.quadReg; }
            else if (sender == radioButtonExp)     { auswahl = Constants.expReg; }
            else if (sender == radioButtonSin)     { auswahl = Constants.sinReg; }
            else if (sender == radioButtonSinExp)  { auswahl = Constants.sinExpReg; }
            else if (sender == radioButtonResonanz) { auswahl = Constants.resoReg; }
            else { auswahl = Constants.linReg; }
        }

        public int GetAuswahl()
        {
            return auswahl;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
