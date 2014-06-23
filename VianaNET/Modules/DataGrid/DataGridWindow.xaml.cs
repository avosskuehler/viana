// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The data grid window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.DataGrid
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;

  using VianaNET.Application;
  using VianaNET.Data.Collections;
  using VianaNET.Modules.DataAcquisition;
  using VianaNET.Resources;

  /// <summary>
  ///   The data grid window.
  /// </summary>
  public partial class DataGridWindow
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="DataGridWindow" /> class.
    /// </summary>
    public DataGridWindow()
    {
      this.InitializeComponent();
      this.PopulateDataGridWithColumns();
      Viana.Project.CalibrationData.PropertyChanged += this.DataPropertyChanged;
      Viana.Project.VideoData.PropertyChanged += this.DataPropertyChanged;

      // Viana.Project.VideoData.SelectionChanged += VideoData_SelectionChanged;
      Viana.Project.ProcessingData.PropertyChanged += this.DataPropertyChanged;
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Update the datagrids items source
    /// </summary>
    public void Refresh()
    {
      this.DataGrid.ItemsSource = null;
      this.DataGrid.ItemsSource = Viana.Project.VideoData.Samples;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates the column.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <param name="header">
    /// The header.
    /// </param>
    /// <param name="cellstyles">
    /// The cellstyles.
    /// </param>
    /// <param name="measurement">
    /// The measurement.
    /// </param>
    private void CreateColumn(string path, string header, string[] cellstyles, string measurement)
    {
      var newColumn = new DataGridTextColumn
                        {
                          Header = header, 
                          HeaderStyle = (Style)this.Resources[cellstyles[0]], 
                          CellStyle = (Style)this.Resources[cellstyles[1]], 
                          CanUserReorder = true, 
                          IsReadOnly = true, 
                          CanUserSort = false
                        };

      // newColumn.SortMemberPath = path;
      var valueBinding = new Binding(path)
                           {
                             Converter = (IValueConverter)this.Resources["UnitDoubleStringConverter"], 
                             ConverterParameter = this.Resources[measurement]
                           };

      newColumn.Binding = valueBinding;
      this.DataGrid.Columns.Add(newColumn);
    }

    /// <summary>
    /// Data grid row mouse double click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void DataGridRowMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (sender == null)
      {
        return;
      }

      var row = sender as DataGridRow;
      if (row == null)
      {
        return;
      }

      var sample = row.Item as TimeSample;
      var modifyWindow = new ModifyDataWindow();
      if (sample != null)
      {
        modifyWindow.MoveToFrame(sample.Framenumber);
      }

      modifyWindow.ShowDialog();
      Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
    }

    /// <summary>
    /// The data grid_ on key down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DataGrid_OnKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Delete)
      {
        // Viana.Project.VideoData.DeleteSelectedSamples();
        if (this.DataGrid.SelectedItems.Count > 0)
        {
          List<TimeSample> removeItems =
            (from object item in this.DataGrid.SelectedItems select item as TimeSample).ToList();

          foreach (TimeSample removeItem in removeItems)
          {
            Viana.Project.VideoData.Samples.Remove(removeItem);
          }
        }

        Viana.Project.VideoData.RefreshDistanceVelocityAcceleration();
      }
    }

    /// <summary>
    /// The data property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Samples" || e.PropertyName == "IsShowingUnits")
      {
        this.Refresh();
      }
      else if (e.PropertyName == "NumberOfTrackedObjects")
      {
        this.PopulateDataGridWithColumns();
        this.Refresh();
      }
      else if (e.PropertyName == "UseEveryNthPoint")
      {
        this.DataGrid.AlternationCount = Viana.Project.VideoData.UseEveryNthPoint;
      }
    }

    /// <summary>
    ///   The populate data grid with columns.
    /// </summary>
    private void PopulateDataGridWithColumns()
    {
      // Clear existing columns
      this.DataGrid.Columns.Clear();

      // Create style string arrays
      var cellStyles = new List<string[]>
                         {
                           new[] { "DataGridColumnHeaderStyleRed", "DataGridCellStyle" }, 
                           new[] { "DataGridColumnHeaderStyleGreen", "DataGridCellStyle" }, 
                           new[] { "DataGridColumnHeaderStyleBlue", "DataGridCellStyle" }
                         };

      // Create default framenumber colum
      var frameColumn = new DataGridTextColumn
                          {
                            Header = Labels.DataGridFramenumber, 
                            HeaderStyle = (Style)this.Resources["DataGridColumnHeaderStyle"], 
                            CellStyle = (Style)this.Resources["DataGridCellStyle"], 
                            CanUserReorder = false, 
                            IsReadOnly = true, 
                            CanUserSort = false
                          };

      // frameColumn.SortMemberPath = "Framenumber";
      var valueBinding = new Binding("Framenumber") { StringFormat = "N0" };
      frameColumn.Binding = valueBinding;
      this.DataGrid.Columns.Add(frameColumn);

      // Create default time column
      var timeColumn = new DataGridTextColumn
                         {
                           Header = Labels.DataGridTimestamp, 
                           HeaderStyle = (Style)this.Resources["DataGridColumnHeaderStyle"], 
                           CellStyle = (Style)this.Resources["DataGridCellStyle"], 
                           CanUserReorder = false, 
                           IsReadOnly = true, 
                           CanUserSort = false
                         };

      // timeColumn.SortMemberPath = "Timestamp";
      var valueBindingTime = new Binding("Timestamp")
                               {
                                 Converter =
                                   (IValueConverter)this.Resources["UnitDoubleStringConverter"], 
                                 ConverterParameter = this.Resources["TimeMeasurement"]
                               };

      timeColumn.Binding = valueBindingTime;
      this.DataGrid.Columns.Add(timeColumn);

      // For each tracked object create the whole bunch of columns
      for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        string prefix = Viana.Project.ProcessingData.NumberOfTrackedObjects > 1
                          ? "Nr." + (i + 1).ToString(CultureInfo.InvariantCulture) + " "
                          : string.Empty;
        string obj = "Object[" + i.ToString(CultureInfo.InvariantCulture) + "].";
        this.CreateColumn(obj + "PixelX", prefix + Labels.DataGridXPixel, cellStyles[i], "PixelMeasurement");
        this.CreateColumn(obj + "PixelY", prefix + Labels.DataGridYPixel, cellStyles[i], "PixelMeasurement");
        this.CreateColumn(obj + "PositionX", prefix + Labels.DataGridXPosition, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "PositionY", prefix + Labels.DataGridYPosition, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "Distance", prefix + Labels.DataGridDistance, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "DistanceX", prefix + Labels.DataGridXDistance, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "DistanceY", prefix + Labels.DataGridYDistance, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "Length", prefix + Labels.DataGridLength, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "LengthX", prefix + Labels.DataGridXLength, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "LengthY", prefix + Labels.DataGridYLength, cellStyles[i], "PositionMeasurement");
        this.CreateColumn(obj + "Velocity", prefix + Labels.DataGridVelocity, cellStyles[i], "VelocityMeasurement");
        this.CreateColumn(obj + "VelocityX", prefix + Labels.DataGridXVelocity, cellStyles[i], "VelocityMeasurement");
        this.CreateColumn(obj + "VelocityY", prefix + Labels.DataGridYVelocity, cellStyles[i], "VelocityMeasurement");
        this.CreateColumn(
          obj + "Acceleration", 
          prefix + Labels.DataGridAcceleration, 
          cellStyles[i], 
          "AccelerationMeasurement");
        this.CreateColumn(
          obj + "AccelerationX", 
          prefix + Labels.DataGridXAcceleration, 
          cellStyles[i], 
          "AccelerationMeasurement");
        this.CreateColumn(
          obj + "AccelerationY", 
          prefix + Labels.DataGridYAcceleration, 
          cellStyles[i], 
          "AccelerationMeasurement");
      }
    }

    #endregion

    // private bool isCallingItself;

    // void VideoData_SelectionChanged(object sender, System.EventArgs e)
    // {
    // if (this.isCallingItself)
    // {
    // return;
    // }

    // this.DataGrid.UnselectAll();
    // if (Viana.Project.VideoData.Samples.AllSamplesSelected)
    // {
    // return;
    // }

    // foreach (var selectedSample in Viana.Project.VideoData.Samples.Select(o => o.IsSelected))
    // {
    // this.DataGrid.SelectedItems.Add(selectedSample);
    // }
    // }

    // private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    // {
    // foreach (var item in e.AddedItems)
    // {
    // var sample = item as TimeSample;
    // if (sample != null)
    // {
    // sample.IsSelected = true;
    // }
    // }

    // foreach (var item in e.RemovedItems)
    // {
    // var sample = item as TimeSample;
    // if (sample != null)
    // {
    // sample.IsSelected = false;
    // }
    // }
    // this.isCallingItself = true;
    // Viana.Project.VideoData.OnSelectionChanged();
    // this.isCallingItself = false;
    // }
  }
}