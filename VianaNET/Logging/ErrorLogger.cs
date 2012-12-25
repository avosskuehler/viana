// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorLogger.cs" company="Freie Universität Berlin">
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
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   This class is used to log errors and exceptions into a file that can
//   be used for debug purposes of user systems. (Can be send to support)
//   Its members are static so it can be called from every code line
//   without instatiation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Logging
{
  using System;
  using System.IO;
  using System.Text;

  using MainWindow;

  /// <summary>
  ///   This class is used to log errors and exceptions into a file that can 
  ///   be used for debug purposes of user systems. (Can be send to support)
  ///   Its members are static so it can be called from every code line
  ///   without instatiation.
  /// </summary>
  public class ErrorLogger
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The <see cref="FileStream" /> that gets the messages.
    /// </summary>
    private static FileStream fs;

    /// <summary>
    ///   The <see cref="StreamWriter" /> that performs the writing to file.
    /// </summary>
    private static StreamWriter logWriter;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Prevents a default instance of the ErrorLogger class from being created.
    ///   This class is only used statically.
    /// </summary>
    private ErrorLogger()
    {
    }

    /// <summary>
    ///   Finalizes an instance of the ErrorLogger class by closing the file connection.
    /// </summary>
    ~ErrorLogger()
    {
      if (logWriter != null)
      {
        logWriter.Close();
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Delegates

    /// <summary>
    ///   This is the delegate for the <see cref="TrackerError" /> event.
    /// </summary>
    /// <param name="message"> A <see cref="string" /> with the message to show in the error dialog. </param>
    public delegate void TrackerErrorMessageHandler(string message);

    #endregion

    #region Public Events

    /// <summary>
    ///   This event is raised when a customized ITU GazeTracker dialog 
    ///   should be shown with a specific message.
    /// </summary>
    public static event TrackerErrorMessageHandler TrackerError;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Methods and Operators

    /// <summary>
    /// This method writes the MethodName and Excpetion message into the ErrorLog.txt file.
    /// </summary>
    /// <param name="ex">
    /// The <see cref="Exception"/> to log. 
    /// </param>
    /// <param name="showMessageBox">
    /// True, if this exception should also be displayed in a message box. 
    /// </param>
    public static void ProcessException(Exception ex, bool showMessageBox)
    {
      string message = string.Empty;
      message = ex.Message;
      Exception innerException = ex;

      WriteLine("------------------------------------");

      // Loop inner exceptions.
      while (innerException.InnerException != null)
      {
        innerException = innerException.InnerException;
        WriteLine(GetLogEntryForException(innerException));
      }

      message = GetLogEntryForException(innerException);
      WriteLine(message);
      {
        // if (showMessageBox)
        var dlg = new VianaDialog("Exception occured", ex.Message, message, true);
        dlg.ShowDialog();
      }
    }

    /// <summary>
    /// This method writes the given line into the ErrorLog.txt file.
    /// </summary>
    /// <param name="line">
    /// A <see cref="string"/> toi be written to file. 
    /// </param>
    public static void WriteLine(string line)
    {
      // Use always ErrorLog.txt in LocalApplicationData
      if (logWriter == null)
      {
        fs =
          new FileStream(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VianaNETErrorLog.txt", 
            FileMode.Append);
        logWriter = new StreamWriter(fs);
      }

      logWriter.WriteLine(line);
      logWriter.Flush();
      Console.WriteLine(line);
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    /// Returns a human readable string for the exception
    /// </summary>
    /// <param name="e">
    /// An <see cref="Exception"/> to be processed 
    /// </param>
    /// <returns>
    /// A human readable <see cref="String"/> for the exception 
    /// </returns>
    private static string GetLogEntryForException(Exception e)
    {
      var sb = new StringBuilder();
      sb.AppendLine("Message: " + e.Message);
      sb.AppendLine("Source: " + e.Source);
      sb.AppendLine("TargetSite: " + e.TargetSite);
      sb.AppendLine("StackTrace: " + e.StackTrace);

      return sb.ToString();
    }

    /// <summary>
    /// Raises the <see cref="TrackerError"/> event with the given message.
    /// </summary>
    /// <param name="message">
    /// A <see cref="String"/> with the message to display. 
    /// </param>
    private static void OnTrackerError(string message)
    {
      if (TrackerError != null)
      {
        TrackerError(message);
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}