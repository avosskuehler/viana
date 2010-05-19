namespace VianaNET
{
  using System;
  using System.IO;
  using System.Windows;

  /// <summary>
  /// This class is used to log errors and exceptions into a file that can 
  /// be used for debug purposes of user systems. (Can be send to support)
  /// Its members are static so it can be called from every code line
  /// without instatiation.
  /// </summary>
  public class ErrorLogger
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

    /// <summary>
    /// The <see cref="FileStream"/> that gets the messages.
    /// </summary>
    private static FileStream fs;

    /// <summary>
    /// The <see cref="StreamWriter"/> that performs the writing to file.
    /// </summary>
    private static StreamWriter logWriter;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Prevents a default instance of the ErrorLogger class from being created.
    /// This class is only used statically.
    /// </summary>
    private ErrorLogger()
    {
    }

    /// <summary>
    /// Finalizes an instance of the ErrorLogger class by closing the file connection.
    /// </summary>
    ~ErrorLogger()
    {
      if (logWriter != null)
      {
        logWriter.Close();
      }
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS

    /// <summary>
    /// This is the delegate for the <see cref="TrackerError"/> event.
    /// </summary>
    /// <param name="message">A <see cref="String"/> with the 
    /// message to show in the error dialog.</param>
    public delegate void TrackerErrorMessageHandler(string message);

    /// <summary>
    /// This event is raised when a customized ITU GazeTracker dialog 
    /// should be shown with a specific message.
    /// </summary>
    public static event TrackerErrorMessageHandler TrackerError;

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

    /// <summary>
    /// This method writes the given line into the ErrorLog.txt file.
    /// </summary>
    /// <param name="line">A <see cref="String"/> toi be written to file.</param>
    public static void WriteLine(string line)
    {
      // Use always ErrorLog.txt in LocalApplicationData
      if (logWriter == null)
      {
        fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VianaNETErrorLog.txt", FileMode.Append);
        logWriter = new StreamWriter(fs);
      }

      logWriter.WriteLine(line);
      logWriter.Flush();
      Console.WriteLine(line);
    }

    /// <summary>
    /// This method writes the MethodName and Excpetion message into the ErrorLog.txt file.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to log.</param>
    /// <param name="showMessageBox">True, if this exception should also be displayed in a message box.</param>
    public static void ProcessException(Exception ex, bool showMessageBox)
    {
      WriteLine("Error in: " + ex.TargetSite.ToString());
      WriteLine("Message: " + ex.Message);
      //if (showMessageBox)
      {
        MessageBox.Show(ex.Message);
      }
    }

    /// <summary>
    /// This method raises the TrackerError event with the given message,
    /// so that subscribers can display the appropriate Error Window.
    /// </summary>
    /// <param name="message">A <see cref="String"/> with the message to display.</param>
    public static void RaiseGazeTrackerMessage(string message)
    {
      OnTrackerError(message);
      Console.WriteLine(message);
    }

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

    /// <summary>
    /// Raises the <see cref="TrackerError"/> event with the given message.
    /// </summary>
    /// <param name="message">A <see cref="String"/> with the message to display.</param>
    private static void OnTrackerError(string message)
    {
      if (TrackerError != null)
      {
        TrackerError(message);
      }
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
