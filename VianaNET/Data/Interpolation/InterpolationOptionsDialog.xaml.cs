namespace VianaNET
{
  using System.Windows;
  using System.Reflection;
  using System.Windows.Media;
  using System.Windows.Controls;

  public partial class InterpolationOptionsDialog : Window
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

    public InterpolationBase currentInterpolationFilter;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the InterpolationOptionsDialog class.
    /// </summary>
    public InterpolationOptionsDialog()
    {
      InitializeComponent();
      UpdateUIWithFilter();
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

    public InterpolationBase ChoosenInterpolationFilter
    {
      get
      {
        return this.currentInterpolationFilter;
      }

      set
      {
        this.currentInterpolationFilter = value;
        UpdateUIWithFilter();
      }
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

    private void InterpolationFilterCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      InterpolationBase filter = (InterpolationBase)this.InterpolationFilterCombo.SelectedItem;

      // Remove old property sets.
      this.InterpolationFilterPropertyGrid.Children.Clear();

      // Add custom property control
      this.InterpolationFilterPropertyGrid.Children.Add(filter.CustomUserControl);
    }

    private void Dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      this.InterpolationFilterPropertyGrid.Children.Clear();
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

    private void UpdateUIWithFilter()
    {
      if (this.currentInterpolationFilter == null)
      {
        this.currentInterpolationFilter = Interpolation.Filter[Interpolation.FilterTypes.MovingAverage];
      }

      this.InterpolationFilterCombo.SelectedItem = this.currentInterpolationFilter;
    }

    #endregion //PRIVATEMETHODS

     ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}