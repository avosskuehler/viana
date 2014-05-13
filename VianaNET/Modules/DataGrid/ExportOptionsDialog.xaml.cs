// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportOptionsDialog.xaml.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Modules.DataGrid
{
  using System.Collections.Generic;
  using System.Windows;

  using VianaNET.Application;
  using VianaNET.Data.Collections;
  using VianaNET.Resources;

  /// <summary>
  ///   This dialog covers data export options
  /// </summary>
  public partial class ExportOptionsDialog
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ExportOptionsDialog"/> class. 
    /// </summary>
    public ExportOptionsDialog()
    {
      this.InitializeComponent();
      this.Object1CheckBox.Content = Labels.DataGridObjectPrefix + " 1";
      this.Object2CheckBox.Visibility = Viana.Project.ProcessingData.NumberOfTrackedObjects > 1
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
      this.Object2CheckBox.Content = Labels.DataGridObjectPrefix + " 2";
      this.Object3CheckBox.Visibility = Viana.Project.ProcessingData.NumberOfTrackedObjects > 2
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
      this.Object3CheckBox.Content = Labels.DataGridObjectPrefix + " 3";
    }

    /// <summary>
    /// Gets the export options
    /// </summary>
    public ExportOptions ExportOptions
    {
      get
      {
        var options = new ExportOptions();

        options.Objects = new List<int>(1);
        if (this.Object1CheckBox.IsChecked.GetValueOrDefault(true))
        {
          options.Objects.Add(0);
        }

        if (this.Object2CheckBox.IsChecked.GetValueOrDefault(false) && this.Object2CheckBox.IsVisible)
        {
          options.Objects.Add(1);
        }

        if (this.Object3CheckBox.IsChecked.GetValueOrDefault(false) && this.Object3CheckBox.IsVisible)
        {
          options.Objects.Add(2);
        }

        options.Axes = new List<DataAxis>();
        foreach (var item in this.ColumnsListBox.Items)
        {
          var axis = item as DataAxis;

          if (axis != null && axis.ShouldExport)
          {
            options.Axes.Add(axis);
          }
        }

        return options;
      }
    }

    /// <summary>
    /// Handles the Click event of the Cancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Handles the Click event of the OK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void OkClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// Handles the OnClick event of the SelectAllButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SelectAllButton_OnClick(object sender, RoutedEventArgs e)
    {
      foreach (var item in this.ColumnsListBox.Items)
      {
        var axis = item as DataAxis;

        if (axis != null)
        {
          axis.ShouldExport = true;
        }
      }
    }

    /// <summary>
    /// Handles the OnClick event of the UnselectAllButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void UnselectAllButton_OnClick(object sender, RoutedEventArgs e)
    {
      foreach (var item in this.ColumnsListBox.Items)
      {
        var axis = item as DataAxis;

        if (axis != null && axis.ShouldExport)
        {
          axis.ShouldExport = false;
        }
      }
    }
  }
}