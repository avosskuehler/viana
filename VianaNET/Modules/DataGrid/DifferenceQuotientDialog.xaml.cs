// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DifferenceQuotientDialog.xaml.cs" company="Freie Universität Berlin">
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
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   The length dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.DataGrid
{
  using System;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;

  using WPFMath;

  /// <summary>
  ///   The length dialog.
  /// </summary>
  public partial class DifferenceQuotientDialog
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="DifferenceQuotientDialog" /> class.
    /// </summary>
    public DifferenceQuotientDialog()
    {
      this.InitializeComponent();

      var formulaParser = new TexFormulaParser();
      TexFormula forwardFormula = formulaParser.Parse("v(t)=\\frac{s(t+\\Delta t)-s(t)}{\\Delta t}");
      TexFormula centralFormula = formulaParser.Parse("v(t)=\\frac{s(t+\\Delta t)-s(t-\\Delta t )}{2\\cdot \\Delta t}");
      TexFormula backwardFormula = formulaParser.Parse("v(t)=\\frac{s(t)-s(t-\\Delta t )}{\\Delta t}");

      // Render formula to visual.
      var forwardVisual = new DrawingVisual();
      TexRenderer forwardRenderer = forwardFormula.GetRenderer(TexStyle.Display, 14d);
      using (DrawingContext drawingContext = forwardVisual.RenderOpen())
      {
        forwardRenderer.Render(drawingContext, 0, 1);
      }

      this.ForwardFormulaContainerElement.Visual = forwardVisual;

      // Render formula to visual.
      var centralVisual = new DrawingVisual();
      TexRenderer centralRenderer = centralFormula.GetRenderer(TexStyle.Display, 14d);
      using (DrawingContext drawingContext = centralVisual.RenderOpen())
      {
        centralRenderer.Render(drawingContext, 0, 1);
      }

      this.CentralFormulaContainerElement.Visual = centralVisual;

      // Render formula to visual.
      var backwardVisual = new DrawingVisual();
      TexRenderer backwardRenderer = backwardFormula.GetRenderer(TexStyle.Display, 14d);
      using (DrawingContext drawingContext = backwardVisual.RenderOpen())
      {
        backwardRenderer.Render(drawingContext, 0, 1);
      }

      this.BackwardFormulaContainerElement.Visual = backwardVisual;

      switch (Viana.Project.ProcessingData.DifferenceQuotientType)
      {
        case DifferenceQuotientType.Forward:
          this.ForwardRadioButton.IsChecked = true;
          break;
        case DifferenceQuotientType.Backward:
          this.BackwardRadioButton.IsChecked = true;
          break;
        case DifferenceQuotientType.Central:
          this.CentralRadioButton.IsChecked = true;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the Click event of the Cancel control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the Click event of the OK control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      var newQuotientType = DifferenceQuotientType.Forward;

      this.DialogResult = true;
      if (this.ForwardRadioButton.IsChecked.GetValueOrDefault(false))
      {
        newQuotientType = DifferenceQuotientType.Forward;
      }
      else if (this.CentralRadioButton.IsChecked.GetValueOrDefault(false))
      {
        newQuotientType = DifferenceQuotientType.Central;
      }
      else if (this.BackwardRadioButton.IsChecked.GetValueOrDefault(false))
      {
        newQuotientType = DifferenceQuotientType.Backward;
      }

      Viana.Project.ProcessingData.DifferenceQuotientType = newQuotientType;
      this.Cursor = Cursors.Wait;
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
      this.Cursor = Cursors.Arrow;
      this.Close();
    }

    #endregion
  }
}