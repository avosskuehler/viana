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


namespace VianaNET
{
    /// <summary>
    /// Interaktionslogik für SelectLinefitTypDialog.xaml
    /// </summary>
    public partial class SelectLinefitTypDialog : Window
    {
        int auswahl;

        public void SelectLineFitTypDialog(bool xNeg, bool yNeg)
        {
            InitializeComponent();
            radioButtonPot.IsEnabled = (!xNeg) & (!yNeg);
            radioButtonLog.IsEnabled = !xNeg;
            radioButtonExp.IsEnabled = !yNeg;
            auswahl = 1; //linReg  
        }

       
        private void buttonRegressAuswahl_checked(object sender, RoutedEventArgs e)
        {
            if (sender == radioButtonLin) { auswahl = 1; } //linReg;
            else if (sender == radioButtonExpSpez) { auswahl = 2; }
            else if (sender == radioButtonLog) { auswahl = 3; }
            else if (sender == radioButtonPot) { auswahl = 4; }
            else if (sender == radioButtonQuad) { auswahl = 5; }
            else if (sender == radioButtonExp) { auswahl = 6; }
            else if (sender == radioButtonSin) { auswahl = 7; }
            else if (sender == radioButtonSinExp) { auswahl = 8; }
            else if (sender == radioButtonResonanz) { auswahl = 9; }
            else { auswahl = 1; }
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
