namespace VianaNET
{
  using System.Windows;
  using System.Reflection;
  using System.Windows.Media;

  public partial class LengthDialog : Window
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS
    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public LengthDialog()
    {
      InitializeComponent();
      this.txbLength.Focus();
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
      double result;
      string addOn = string.Empty;
      if (double.TryParse(this.txbLength.Text, out result))
      {
        if (rdbKM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.km;
        }
        else if (rdbM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.m;
        }
        else if (rdbCM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.cm;
        }
        else if (rdbMM.IsChecked.Value)
        {
          Calibration.Instance.RulerUnit = Unit.mm;
        }

        // This line is necessary to get an update event for the ruler value
        // even if only the ruler unit was changed
        Calibration.Instance.RulerValueInRulerUnits = -1;
        Calibration.Instance.RulerValueInRulerUnits = result;

        this.DialogResult = true;
        this.Close();
      }
      else
      {
        VianaDialog dlg = new VianaDialog(
          Localization.Labels.CalibrationErrorTitle,
          Localization.Labels.CalibrationErrorDescription,
          Localization.Labels.CalibrationErrorMessage);
        dlg.ShowDialog();
      }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
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
    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}