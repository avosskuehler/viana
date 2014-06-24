﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VianaSaveDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The viana dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.MainWindow
{
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  ///   The viana dialog.
  /// </summary>
  public partial class VianaSaveDialog
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="VianaSaveDialog" /> class. 
    /// </summary>
    public VianaSaveDialog()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the VianaDialog class.
    /// </summary>
    /// <param name="title">The title for the dialog.</param>
    /// <param name="messageDescription">The message header.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="isOnlyOk">True, if only the OK button should be displayed.</param>
    public VianaSaveDialog(string title, string messageDescription, string message)
    {
      this.InitializeComponent();
      this.VianaTitle = title;
      this.MessageDescription = messageDescription;
      this.Message = message;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Sets the description background.
    /// </summary>
    public Brush DescriptionBackground
    {
      set
      {
        this.DescriptionArea.Background = value;
      }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   Sets the message.
    /// </summary>
    public string Message
    {
      set
      {
        this.message.Text = value;
      }
    }

    /// <summary>
    ///   Sets the message description.
    /// </summary>
    public string MessageDescription
    {
      set
      {
        this.Description.Content = value;
      }
    }


    /// <summary>
    ///   Sets the viana title.
    /// </summary>
    public string VianaTitle
    {
      set
      {
        this.TopFrame.Title = value;
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    #region Methods

    /// <summary>
    /// Event handler for the button click event.
    /// SaveAndClose is clicked, so set the dialog result to true and exit.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void SaveAndCloseButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// Event handler for the button click event.
    /// DontSaveAndClose is clicked, so set the dialog result to false and exit.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments.</param>

    private void DontSaveAndCloseButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
    #endregion
  }
}