namespace VianaNET
{
using System.Windows;
  using System.Reflection;

  public partial class AboutWindow : Window
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
    public AboutWindow()
    {
      InitializeComponent();
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

    /// <summary>
    /// Gets the assemblies title.
    /// </summary>
    public static string AssemblyTitle
    {
      get
      {
        // Get all Title attributes on this assembly
        object[] attributes =
          Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

        // If there is at least one Title attribute
        if (attributes.Length > 0)
        {
          // Select the first one
          AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];

          // If it is not an empty string, return it
          if (titleAttribute.Title != string.Empty)
          {
            return titleAttribute.Title;
          }
        }

        // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    /// <summary>
    /// Gets OGAMAs current version.
    /// </summary>
    public static string AssemblyVersionShort
    {
      get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(2); }
    }

    /// <summary>
    /// Gets OGAMAs current version.
    /// </summary>
    public static string AssemblyVersionLong
    {
      get { return "(" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")"; }
    }

    /// <summary>
    /// Gets assembly description.
    /// </summary>
    public static string AssemblyDescription
    {
      get
      {
        // Get all Description attributes on this assembly
        object[] attributes =
          Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

        // If there aren't any Description attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Description attribute, return its value
        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    /// <summary>
    /// Gets the product property from the assembly.
    /// </summary>
    public static string AssemblyProduct
    {
      get
      {
        // Get all Product attributes on this assembly
        object[] attributes =
          Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

        // If there aren't any Product attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Product attribute, return its value
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    /// <summary>
    /// Gets the copyright property from the assembly.
    /// </summary>
    public static string AssemblyCopyright
    {
      get
      {
        // Get all Copyright attributes on this assembly
        object[] attributes =
          Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

        // If there aren't any Copyright attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Copyright attribute, return its value
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    /// <summary>
    /// Gets the company string from the assembly.
    /// </summary>
    public static string AssemblyCompany
    {
      get
      {
        // Get all Company attributes on this assembly
        object[] attributes =
          Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

        // If there aren't any Company attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Company attribute, return its value
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }

    #endregion //PROPERTIES

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

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