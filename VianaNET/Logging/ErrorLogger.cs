// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorLogger.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Logging
{
  using System;
  using System.IO;
  using System.Text;

  using VianaNET.MainWindow;

  /// <summary>
  ///   This class is used to log errors and exceptions into a file that can
  ///   be used for debug purposes of user systems. (Can be send to support)
  ///   Its members are static so it can be called from every code line
  ///   without instatiation.
  /// </summary>
  public class ErrorLogger
  {


    /// <summary>
    ///   The <see cref="FileStream" /> that gets the messages.
    /// </summary>
    private static FileStream fs;

    /// <summary>
    ///   The <see cref="StreamWriter" /> that performs the writing to file.
    /// </summary>
    private static StreamWriter logWriter;





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
      Exception innerException = ex;

      WriteLine("------------------------------------");

      // Loop inner exceptions.
      while (innerException.InnerException != null)
      {
        innerException = innerException.InnerException;
        WriteLine(GetLogEntryForException(innerException));
      }

      string message = GetLogEntryForException(innerException);
      WriteLine(message);
      {
        // if (showMessageBox)
        VianaDialog dlg = new VianaDialog("Exception occured", ex.Message, message, true);
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

    /// <summary>
    /// Raises a "user-friendly" error message dialog.
    /// </summary>
    /// <param name="message">A <see cref="string"/> with the message to display.</param>
    public static void ProcessErrorMessage(string message)
    {
      if (message == null)
      {
        throw new ArgumentException("Error message string is NULL");
      }

      // Add error to error log
      var errorLogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VianaNETErrorLog.txt");
      using (var w = File.AppendText(errorLogFile))
      {
        LogMessage(message, w);
      }

      // Show error message dialog
      InformationDialog.Show("Fehler", message, false);
    }

    /// <summary>
    /// This method logs the given message into the file
    /// given by the <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="logMessage">A <see cref="string"/> with the message to log.</param>
    /// <param name="w">The <see cref="TextWriter"/> to write the message to.</param>
    public static void LogMessage(string logMessage, TextWriter w)
    {
      w.Write("Error on ");
      w.WriteLine("{0}, {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
      w.WriteLine("  - {0}", logMessage);
      w.WriteLine("--------------------------------------------------------------------------------");

      // Update the underlying file.
      w.Flush();
    }

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
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Message: " + e.Message);
      sb.AppendLine("Source: " + e.Source);
      sb.AppendLine("TargetSite: " + e.TargetSite);
      sb.AppendLine("StackTrace: " + e.StackTrace);

      return sb.ToString();
    }


  }
}