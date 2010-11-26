// <copyright file="ExtensionMethods.cs" company="FU Berlin">
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
// <author>Adrian Voßkühler</author>
// <email>adrian.vosskuehler@fu-berlin.de</email>

namespace VianaNET
{
  using System;
  using System.Windows.Controls.Primitives;

  /// <summary>
  /// This class provides extension methods for ToggleButtons and 
  /// invoking of empty events.
  /// </summary>
  public static class ExtensionMethods
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
    /// Static. Returns the <see cref="ToggleButton.IsChecked.GetValueOrDefault()"/>
    /// value for the given <see cref="ToggleButton"/>
    /// </summary>
    /// <param name="toggleButton">The <see cref="ToggleButton"/>
    /// to check its value.</param>
    /// <returns><strong>True</strong> if button is checked, otherwise <strong>false</strong> or
    /// default value if is not set.</returns>
    public static bool IsChecked(this ToggleButton toggleButton)
    {
      return toggleButton.IsChecked.GetValueOrDefault();
    }

    /// <summary>
    /// Static. Invokes the given event with the given sender and 
    /// empty event arguments.
    /// </summary>
    /// <param name="eventHandler">The <see cref="EventHandler"/>
    /// to be sent.</param>
    /// <param name="sender">The sender of the event.</param>
    public static void InvokeEmpty(this EventHandler eventHandler, object sender)
    {
      var handler = eventHandler;
      if (null != handler)
      {
        handler.Invoke(sender, EventArgs.Empty);
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
