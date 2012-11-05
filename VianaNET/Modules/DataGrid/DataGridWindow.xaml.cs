// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The data grid window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.DataGrid
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;

  using VianaNET.Data;
  using VianaNET.Data.Interpolation;
  using VianaNET.Localization;
  using VianaNET.Modules.Video.Control;

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
      Calibration.Instance.PropertyChanged += this.DataPropertyChanged;
      VideoData.Instance.PropertyChanged += this.DataPropertyChanged;
      Video.Instance.ImageProcessing.PropertyChanged += this.DataPropertyChanged;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create column.
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
    /// <param name="bindVisibility">
    /// The bind visibility. 
    /// </param>
    private void CreateColumn(string path, string header, string[] cellstyles, string measurement, bool bindVisibility)
    {
      var newColumn = new DataGridTextColumn();
      newColumn.Header = header;
      newColumn.HeaderStyle = (Style)this.Resources[cellstyles[0]];
      newColumn.CellStyle = (Style)this.Resources[cellstyles[1]];
      newColumn.CanUserReorder = true;
      newColumn.IsReadOnly = true;
      newColumn.CanUserSort = false;

      // newColumn.SortMemberPath = path;
      var valueBinding = new Binding(path);
      valueBinding.Converter = (IValueConverter)this.Resources["UnitDoubleStringConverter"];
      valueBinding.ConverterParameter = this.Resources[measurement];
      newColumn.Binding = valueBinding;
      if (bindVisibility)
      {
        var visibilityBinding = new Binding("IsInterpolatingData");
        visibilityBinding.Converter = (IValueConverter)this.Resources["BoolVisibleConverter"];
        visibilityBinding.Source = Interpolation.Instance;
        BindingOperations.SetBinding(newColumn, DataGridColumn.VisibilityProperty, visibilityBinding);
      }

      this.dataGrid.Columns.Add(newColumn);
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
      else
      {
        if (e.PropertyName == "NumberOfTrackedObjects")
        {
          this.PopulateDataGridWithColumns();
          this.Refresh();
        }
      }
    }

    /// <summary>
    ///   The populate data grid with columns.
    /// </summary>
    private void PopulateDataGridWithColumns()
    {
      // Clear existing columns
      this.dataGrid.Columns.Clear();

      // Create style string arrays
      var cellStyles = new List<string[]>();
      cellStyles.Add(new[] { "DataGridColumnHeaderStyleRed", "DataGridCellStyle" });
      cellStyles.Add(new[] { "DataGridColumnHeaderStyleGreen", "DataGridCellStyle" });
      cellStyles.Add(new[] { "DataGridColumnHeaderStyleBlue", "DataGridCellStyle" });

      // Create default framenumber colum
      var frameColumn = new DataGridTextColumn();
      frameColumn.Header = Labels.DataGridFramenumber;
      frameColumn.HeaderStyle = (Style)this.Resources["DataGridColumnHeaderStyle"];
      frameColumn.CellStyle = (Style)this.Resources["DataGridCellStyle"];
      frameColumn.CanUserReorder = false;
      frameColumn.IsReadOnly = true;
      frameColumn.CanUserSort = false;

      // frameColumn.SortMemberPath = "Framenumber";
      var valueBinding = new Binding("Framenumber");
      valueBinding.StringFormat = "N0";
      frameColumn.Binding = valueBinding;
      this.dataGrid.Columns.Add(frameColumn);

      // Create default time column
      var timeColumn = new DataGridTextColumn();
      timeColumn.Header = Labels.DataGridTimestamp;
      timeColumn.HeaderStyle = (Style)this.Resources["DataGridColumnHeaderStyle"];
      timeColumn.CellStyle = (Style)this.Resources["DataGridCellStyle"];
      timeColumn.CanUserReorder = false;
      timeColumn.IsReadOnly = true;
      timeColumn.CanUserSort = false;

      // timeColumn.SortMemberPath = "Timestamp";
      var valueBindingTime = new Binding("Timestamp");
      valueBindingTime.Converter = (IValueConverter)this.Resources["UnitDoubleStringConverter"];
      valueBindingTime.ConverterParameter = this.Resources["TimeMeasurement"];
      timeColumn.Binding = valueBindingTime;
      this.dataGrid.Columns.Add(timeColumn);

      // For each tracked object create the whole bunch of columns
      for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
      {
        string prefix = Video.Instance.ImageProcessing.NumberOfTrackedObjects > 1
                          ? "Nr." + (i + 1).ToString(CultureInfo.InvariantCulture) + " "
                          : string.Empty;
        string obj = "Object[" + i.ToString(CultureInfo.InvariantCulture) + "].";
        this.CreateColumn(obj + "PixelX", prefix + Labels.DataGridXPixel, cellStyles[i], "PixelMeasurement", false);
        this.CreateColumn(obj + "PixelY", prefix + Labels.DataGridYPixel, cellStyles[i], "PixelMeasurement", false);
        this.CreateColumn(
          obj + "Distance", prefix + Labels.DataGridDistance, cellStyles[i], "PositionMeasurement", false);
        this.CreateColumn(
          obj + "DistanceX", prefix + Labels.DataGridXDistance, cellStyles[i], "PositionMeasurement", false);
        this.CreateColumn(
          obj + "DistanceY", prefix + Labels.DataGridYDistance, cellStyles[i], "PositionMeasurement", false);
        this.CreateColumn(obj + "Length", prefix + Labels.DataGridLength, cellStyles[i], "PositionMeasurement", false);
        this.CreateColumn(obj + "LengthX", prefix + Labels.DataGridXLength, cellStyles[i], "PositionMeasurement", false);
        this.CreateColumn(obj + "LengthY", prefix + Labels.DataGridYLength, cellStyles[i], "PositionMeasurement", false);
        this.CreateColumn(
          obj + "Velocity", prefix + Labels.DataGridVelocity, cellStyles[i], "VelocityMeasurement", false);
        this.CreateColumn(
          obj + "VelocityX", prefix + Labels.DataGridXVelocity, cellStyles[i], "VelocityMeasurement", false);
        this.CreateColumn(
          obj + "VelocityY", prefix + Labels.DataGridYVelocity, cellStyles[i], "VelocityMeasurement", false);
        this.CreateColumn(
          obj + "VelocityI", prefix + Labels.DataGridVelocityInterpolated, cellStyles[i], "VelocityMeasurement", true);
        this.CreateColumn(
          obj + "VelocityXI", prefix + Labels.DataGridXVelocityInterpolated, cellStyles[i], "VelocityMeasurement", true);
        this.CreateColumn(
          obj + "VelocityYI", prefix + Labels.DataGridYVelocityInterpolated, cellStyles[i], "VelocityMeasurement", true);
        this.CreateColumn(
          obj + "Acceleration", prefix + Labels.DataGridAcceleration, cellStyles[i], "AccelerationMeasurement", false);
        this.CreateColumn(
          obj + "AccelerationX", prefix + Labels.DataGridXAcceleration, cellStyles[i], "AccelerationMeasurement", false);
        this.CreateColumn(
          obj + "AccelerationY", prefix + Labels.DataGridYAcceleration, cellStyles[i], "AccelerationMeasurement", false);
        this.CreateColumn(
          obj + "AccelerationI", 
          prefix + Labels.DataGridAccelerationInterpolated, 
          cellStyles[i], 
          "AccelerationMeasurement", 
          true);
        this.CreateColumn(
          obj + "AccelerationXI", 
          prefix + Labels.DataGridXAccelerationInterpolated, 
          cellStyles[i], 
          "AccelerationMeasurement", 
          true);
        this.CreateColumn(
          obj + "AccelerationYI", 
          prefix + Labels.DataGridYAccelerationInterpolated, 
          cellStyles[i], 
          "AccelerationMeasurement", 
          true);
      }
    }

    /// <summary>
    ///   The refresh.
    /// </summary>
    private void Refresh()
    {
      this.dataGrid.ItemsSource = null;
      this.dataGrid.ItemsSource = VideoData.Instance.Samples;
    }

    #endregion
  }
}