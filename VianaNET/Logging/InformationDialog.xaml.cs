// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthDialog.xaml.cs" company="Freie Universität Berlin">
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
// <summary>
//   Dialog showing informations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Logging
{
  using System.Windows;
  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   Thid dialog displays video information
  /// </summary>
  public partial class InformationDialog
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InformationDialog"/> class. 
    /// </summary>
    /// <param name="videofile"> The videofile to be analyzed. </param>
    public InformationDialog(string title, string message, bool isYesNoCancel)
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Header.Title = title;
      this.ErrorMessageTextBlock.Text = message;

      if (isYesNoCancel)
      {
        this.YesNoCancelPanel.Visibility = Visibility.Visible;
        this.OkPanel.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.YesNoCancelPanel.Visibility = Visibility.Collapsed;
        this.OkPanel.Visibility = Visibility.Visible;
      }

      this.QuestionResult = QuestionResult.Cancel;
    }

    public QuestionResult QuestionResult { get; set; }

    /// <summary>
    /// Zeigt eine DialogBox mit den gegebenen Titel, Nachricht und ggf. verschiedenen Auswahlmöglichkeiten.
    /// </summary>
    /// <param name="caption">The text to display in the title of the message box. </param>
    /// <param name="message">The text to display in the message box.</param>
    /// <param name="isYesNoCancel"><strong>True</strong> when yes no cancel buttons should be shown,
    /// otherwise <strong>false</strong> only OK button is shown.</param>
    /// <returns>One of the <see cref="DialogResult"/> values.</returns>
    public static QuestionResult Show(string caption, string message, bool isYesNoCancel)
    {
      InformationDialog dlg = new InformationDialog(caption, message, isYesNoCancel);
      dlg.ShowDialog();
      return dlg.QuestionResult;
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the OK <see cref="Button"/>. Closes the dialog with the ok result.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void OkButtonClick(object sender, RoutedEventArgs e)
    {
      this.QuestionResult = QuestionResult.OK;
      this.Close();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the Yes <see cref="Button"/>. Closes the dialog with the yes result.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void YesButtonClick(object sender, RoutedEventArgs e)
    {
      this.QuestionResult = QuestionResult.Yes;
      this.Close();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the No <see cref="Button"/>. Closes the dialog with the no result.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void NoButtonClick(object sender, RoutedEventArgs e)
    {
      this.QuestionResult = QuestionResult.No;
      this.Close();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the Cancel <see cref="Button"/>. Closes the dialog with the cancel result.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
      this.QuestionResult = QuestionResult.Cancel;
      this.Close();
    }

    /// <summary>
    /// The <see cref="Control.Click"/> event handler for
    /// the Copy <see cref="Button"/>. Copies the message to the clipboard.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void CopyButtonClick(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(this.ErrorMessageTextBlock.Text);
    }
  }
}