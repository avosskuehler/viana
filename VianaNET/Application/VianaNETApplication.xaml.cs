// <copyright file="VianaNETApplication.xaml.cs" company="FU Berlin">
// ************************************************************************
// Viana.NET - video analysis for physics education
// Copyright (C) 2010 Dr. Adrian Voßkühler  
// ------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License 
// along with this program; if not, write to the Free Software Foundation, 
// Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian.vosskuehler@fu-berlin.de</email>

namespace VianaNET
{
  using System.Globalization;
  using System.Threading;
  using System.Windows;
  using System.Windows.Threading;
  using WPFLocalizeExtension.Engine;

  /// <summary>
  /// This is the main entry point for the application and
  /// represents the base for VianaNET.
  /// </summary>
  public partial class VianaNETApplication : Application
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
    
    /// <summary>
    /// Is a replacement for the winform Application.DoEvents function
    /// for WPF. Process the current dispatcher queue.
    /// </summary>
    public static void DoEvents()
    {
      if (Application.Current != null)
      {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      }
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
    /// The <see cref="Application.Startup"/> event handler.
    /// Defaults localization to german locale and registers
    /// to unhandled exception event.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="args">The <see cref="StartupEventArgs"/> with the event data.</param>
    private void VianaNETApplication_Startup(object sender, StartupEventArgs args)
    {
      this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("de");
      this.MainWindow = new MainWindow();
    }

    /// <summary>
    /// The <see cref="Application.DispatcherUnhandledException"/> event handler.
    /// Displays a message for each otherwise unhandled exception.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">The <see cref="StartupEventArgs"/> with the event data.</param>
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      // Raise message box and log to file.
      ErrorLogger.ProcessException(e.Exception, true);
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