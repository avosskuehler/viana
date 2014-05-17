// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceListener.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Logging
{
  using System.Diagnostics;
  using System.Text;
  using System.Windows;

  /// <summary>
  /// </summary>
  public class BindingErrorTraceListener : DefaultTraceListener
  {
    #region Static Fields

    /// <summary>
    ///   The listener
    /// </summary>
    private static BindingErrorTraceListener listener;

    #endregion

    #region Fields

    /// <summary>
    ///   The _ message
    /// </summary>
    private readonly StringBuilder message = new StringBuilder();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Prevents a default instance of the <see cref="BindingErrorTraceListener" /> class from being created.
    /// </summary>
    private BindingErrorTraceListener()
    {
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Closes the trace.
    /// </summary>
    public static void CloseTrace()
    {
      if (listener == null)
      {
        return;
      }

      listener.Flush();
      listener.Close();
      PresentationTraceSources.DataBindingSource.Listeners.Remove(listener);
      listener = null;
    }

    /// <summary>
    ///   Sets the trace.
    /// </summary>
    public static void SetTrace()
    {
      SetTrace(SourceLevels.Error, TraceOptions.None);
    }

    /// <summary>
    /// Sets the trace.
    /// </summary>
    /// <param name="level">
    /// The level.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    public static void SetTrace(SourceLevels level, TraceOptions options)
    {
      if (listener == null)
      {
        listener = new BindingErrorTraceListener();
        PresentationTraceSources.DataBindingSource.Listeners.Add(listener);
      }

      listener.TraceOutputOptions = options;
      PresentationTraceSources.DataBindingSource.Switch.Level = level;
    }

    /// <summary>
    /// Writes the output to the OutputDebugString function and to the
    ///   <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)"/> method.
    /// </summary>
    /// <param name="message">
    /// The message to write to OutputDebugString and
    ///   <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)"/>.
    /// </param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence"/>
    /// </PermissionSet>
    public override void Write(string message)
    {
      this.message.Append(message);
    }

    /// <summary>
    /// Writes the output to the OutputDebugString function and to the
    ///   <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)"/> method, followed by a
    ///   carriage return and line feed (\r\n).
    /// </summary>
    /// <param name="message">
    /// The message to write to OutputDebugString and
    ///   <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)"/>.
    /// </param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence"/>
    /// </PermissionSet>
    public override void WriteLine(string message)
    {
      this.message.Append(message);

      string final = this.message.ToString();
      this.message.Length = 0;

      MessageBox.Show(final, "Binding Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    #endregion
  }
}