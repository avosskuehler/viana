// Autor: H. Niemeyer
// letzte Änderung: 24.09.2012

namespace VianaNET
{
    using System.Windows;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Controls;

    public partial class LineOptionsDialog : Window
    {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
      double beispielZahl = 0.00123456789;
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS
      private double _lineThicknessRegress;
      private double _lineThicknessTheorie;
      private SolidColorBrush _lineColorRegress;
      private SolidColorBrush _lineColorTheorie;
      private Border oldBorderRegress;
      private Border oldBorderTheorie;
    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the LineOptionsDialog class.
    /// </summary>
       public LineOptionsDialog(double breiteRegress, SolidColorBrush regressLineColor, double breiteTheorie, SolidColorBrush theorieLineColor, int stellenAnzahl)
    {
      InitializeComponent();
      stellenZahl = 0;
      lineThicknessRegress = breiteRegress;
      lineThicknessTheorie = breiteTheorie;
      lineColorRegress = regressLineColor;
      lineColorTheorie = theorieLineColor;
      stellenZahl = stellenAnzahl;
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    public double lineThicknessRegress
    {
        get { return _lineThicknessRegress; }
        set 
        {
            _lineThicknessRegress = value;
            if (value != sliderLinienDickeA.Value)  { sliderLinienDickeA.Value = value; }
        }
    }

    public double lineThicknessTheorie
    {
        get { return _lineThicknessTheorie; }
        set
        {
            _lineThicknessTheorie = value;
            if (value != sliderLinienDickeT.Value) { sliderLinienDickeT.Value = value; }
        }
    }

    public SolidColorBrush lineColorRegress
    {
        get { return _lineColorRegress; }
        set
        {
            _lineColorRegress = value;
            ShowSelectedRegressColor(value);
        }
    }

    public SolidColorBrush lineColorTheorie
    {
        get { return _lineColorTheorie; }
        set
        {
            _lineColorTheorie = value;
            ShowSelectedTheorieColor(value);
        }
    }

    public int stellenZahl
    {
        get { return (int)this.sliderGenauigkeit.Value; }
        set { this.sliderGenauigkeit.Value = value; }
    }
   
    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS
    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
  

        private void sliderGenauigkeit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string gx;
            if (labelBeispiel == null)
            {
                return;
            }
            stellenZahl = (int)sliderGenauigkeit.Value;
            gx = "G" + stellenZahl.ToString();
            labelBeispiel.Content = beispielZahl.ToString(gx);
        }


        private void sliderLinienDickeT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _lineThicknessTheorie = sliderLinienDickeT.Value;
        }

        private void sliderLinienDickeA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _lineThicknessRegress = sliderLinienDickeA.Value;
        }

        private void borderR_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border aktBorder = (sender as Border);
            if ((oldBorderRegress != null) && (oldBorderRegress == aktBorder)) { return; }
            if (oldBorderRegress != null) { oldBorderRegress.BorderBrush = null; }
            aktBorder.BorderBrush = Brushes.Black;
            oldBorderRegress = aktBorder;
            lineColorRegress = (SolidColorBrush)aktBorder.Background; 
        }

        private void borderT_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border aktBorder = (sender as Border);
            if ((oldBorderTheorie != null) && (oldBorderTheorie == aktBorder)) { return; }
            if (oldBorderTheorie != null) { oldBorderTheorie.BorderBrush = null; }
            aktBorder.BorderBrush = Brushes.Black;
            oldBorderTheorie = aktBorder;
            lineColorTheorie = (SolidColorBrush)aktBorder.Background; 
        }

    #endregion //EVENTHANDLER

        
        ///////////////////////////////////////////////////////////////////////////////
        // Methods and Eventhandling for Background tasks                            //
        ///////////////////////////////////////////////////////////////////////////////
        #region THREAD
        #endregion //THREAD

        ///////////////////////////////////////////////////////////////////////////////
        // Methods for doing main class job                                          //
        ///////////////////////////////////////////////////////////////////////////////
        #region PRIVATEMETHODS

        private void ShowSelectedRegressColor(SolidColorBrush aktColor)
        {
            if (aktColor == borderR1.Background) { borderR_MouseDown(borderR1, null); return; }
            if (aktColor == borderR2.Background) { borderR_MouseDown(borderR2, null); return; }
            if (aktColor == borderR3.Background) { borderR_MouseDown(borderR3, null); return; }
            if (aktColor == borderR4.Background) { borderR_MouseDown(borderR4, null); return; }
            if (aktColor == borderR5.Background) { borderR_MouseDown(borderR5, null); return; }
            if (aktColor == borderR6.Background) { borderR_MouseDown(borderR6, null); return; }
            if (aktColor == borderR7.Background) { borderR_MouseDown(borderR7, null); return; }
            if (aktColor == borderR8.Background) { borderR_MouseDown(borderR8, null); return; }
            if (aktColor == borderR9.Background) { borderR_MouseDown(borderR9, null); return; }
            if (aktColor == borderR10.Background) { borderR_MouseDown(borderR10, null); return; }
            if (aktColor == borderR11.Background) { borderR_MouseDown(borderR11, null); return; }
            /*        if (aktColor == borderR12.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR13.Background) { border_MouseDown(borderR10, null); return;}
                    if (aktColor == borderR14.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR15.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR16.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR17.Background) { border_MouseDown(borderR10, null); return; }  */
        }

        private void ShowSelectedTheorieColor(SolidColorBrush aktColor)
        {
            if (aktColor == borderT1.Background) { borderT_MouseDown(borderT1, null); return; }
            if (aktColor == borderT2.Background) { borderT_MouseDown(borderT2, null); return; }
            if (aktColor == borderT3.Background) { borderT_MouseDown(borderT3, null); return; }
            if (aktColor == borderT4.Background) { borderT_MouseDown(borderT4, null); return; }
            if (aktColor == borderT5.Background) { borderT_MouseDown(borderT5, null); return; }
            if (aktColor == borderT6.Background) { borderT_MouseDown(borderT6, null); return; }
            if (aktColor == borderT7.Background) { borderT_MouseDown(borderT7, null); return; }
            if (aktColor == borderT8.Background) { borderT_MouseDown(borderT8, null); return; }
            if (aktColor == borderT9.Background) { borderT_MouseDown(borderT9, null); return; }
            if (aktColor == borderT10.Background) { borderT_MouseDown(borderT10, null); return; }
            if (aktColor == borderT11.Background) { borderT_MouseDown(borderT11, null); return; }
            /*       if (aktColor == borderT12.Background) { border_MouseDown(borderT12, null); return; }
                   if (aktColor == borderT13.Background) { border_MouseDown(borderT13, null); return;}
                   if (aktColor == borderT14.Background) { border_MouseDown(borderT14, null); return; }
                   if (aktColor == borderT15.Background) { border_MouseDown(borderT15, null); return; }
                   if (aktColor == borderT16.Background) { border_MouseDown(borderT16, null); return; }
                   if (aktColor == borderT17.Background) { border_MouseDown(borderT17, null); return; }  */
        }

        #endregion //PRIVATEMETHODS
  

        ///////////////////////////////////////////////////////////////////////////////
        // Small helping Methods                                                     //
        ///////////////////////////////////////////////////////////////////////////////
        #region HELPER
        #endregion //HELPER
 
    }
}
