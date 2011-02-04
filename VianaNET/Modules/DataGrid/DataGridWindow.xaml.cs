using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using AvalonDock;
using System.Reflection;
using System.ComponentModel;

namespace VianaNET
{
  public partial class DataGridWindow : DockableContent
  {
    public DataGridWindow()
    {
      InitializeComponent();
      this.PopulateDataGridWithColumns();
      Calibration.Instance.PropertyChanged += new PropertyChangedEventHandler(DataPropertyChanged);
      VideoData.Instance.PropertyChanged += new PropertyChangedEventHandler(DataPropertyChanged);
      Video.Instance.ImageProcessing.PropertyChanged += new PropertyChangedEventHandler(DataPropertyChanged);
    }

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

    private void Refresh()
    {
      this.dataGrid.ItemsSource = null;
      this.dataGrid.ItemsSource = VideoData.Instance.Samples;
    }

    private void CreateColumn(string path, string header, string[] cellstyles, string measurement, bool bindVisibility)
    {
      DataGridTextColumn newColumn = new DataGridTextColumn();
      newColumn.Header = header;
      newColumn.HeaderStyle = (Style)this.Resources[cellstyles[0]];
      newColumn.CellStyle = (Style)this.Resources[cellstyles[1]];
      newColumn.CanUserReorder = true;
      newColumn.IsReadOnly = true;
      newColumn.CanUserSort = false;
      //newColumn.SortMemberPath = path;
      Binding valueBinding = new Binding(path);
      valueBinding.Converter = (IValueConverter)this.Resources["UnitDoubleStringConverter"];
      valueBinding.ConverterParameter = this.Resources[measurement];
      newColumn.Binding = valueBinding;
      if (bindVisibility)
      {
        Binding visibilityBinding = new Binding("IsInterpolatingData");
        visibilityBinding.Converter = (IValueConverter)this.Resources["BoolVisibleConverter"];
        visibilityBinding.Source = Interpolation.Instance;
        BindingOperations.SetBinding(newColumn, DataGridTextColumn.VisibilityProperty, visibilityBinding);
      }
      this.dataGrid.Columns.Add(newColumn);
    }

    private void PopulateDataGridWithColumns()
    {
      // Clear existing columns
      this.dataGrid.Columns.Clear();

      // Create style string arrays
      List<string[]> cellStyles = new List<string[]>();
      cellStyles.Add(new string[2] { "DataGridColumnHeaderStyleRed", "DataGridCellStyle" });
      cellStyles.Add(new string[2] { "DataGridColumnHeaderStyleGreen", "DataGridCellStyle" });
      cellStyles.Add(new string[2] { "DataGridColumnHeaderStyleBlue", "DataGridCellStyle" });

      // Create default framenumber colum
      DataGridTextColumn frameColumn = new DataGridTextColumn();
      frameColumn.Header = Localization.Labels.DataGridFramenumber;
      frameColumn.HeaderStyle = (Style)this.Resources["DataGridColumnHeaderStyle"];
      frameColumn.CellStyle = (Style)this.Resources["DataGridCellStyle"];
      frameColumn.CanUserReorder = false;
      frameColumn.IsReadOnly = true;
      frameColumn.CanUserSort = false;
      //frameColumn.SortMemberPath = "Framenumber";
      Binding valueBinding = new Binding("Framenumber");
      valueBinding.StringFormat = "N0";
      frameColumn.Binding = valueBinding;
      this.dataGrid.Columns.Add(frameColumn);

      // Create default time column
      DataGridTextColumn timeColumn = new DataGridTextColumn();
      timeColumn.Header = Localization.Labels.DataGridTimestamp;
      timeColumn.HeaderStyle = (Style)this.Resources["DataGridColumnHeaderStyle"];
      timeColumn.CellStyle = (Style)this.Resources["DataGridCellStyle"];
      timeColumn.CanUserReorder = false;
      timeColumn.IsReadOnly = true;
      timeColumn.CanUserSort = false;
      //timeColumn.SortMemberPath = "Timestamp";
      Binding valueBindingTime = new Binding("Timestamp");
      valueBindingTime.Converter = (IValueConverter)this.Resources["UnitDoubleStringConverter"];
      valueBindingTime.ConverterParameter = this.Resources["TimeMeasurement"];
      timeColumn.Binding = valueBindingTime;
      this.dataGrid.Columns.Add(timeColumn);

      // For each tracked object create the whole bunch of columns
      for (int i = 0; i < Video.Instance.ImageProcessing.NumberOfTrackedObjects; i++)
      {
        string prefix = Video.Instance.ImageProcessing.NumberOfTrackedObjects > 1 ? "Nr." + (i + 1).ToString() + " " : string.Empty;
        string obj = "Object[" + i.ToString() + "].";
        CreateColumn(
          obj + "PixelX",
          prefix + Localization.Labels.DataGridXPixel,
          cellStyles[i],
          "PixelMeasurement",
          false);
        CreateColumn(
          obj + "PixelY",
          prefix + Localization.Labels.DataGridYPixel,
          cellStyles[i],
          "PixelMeasurement",
          false);
        CreateColumn(
          obj + "Distance",
          prefix + Localization.Labels.DataGridDistance,
          cellStyles[i],
          "PositionMeasurement",
          false);
        CreateColumn(
          obj + "DistanceX",
          prefix + Localization.Labels.DataGridXDistance,
          cellStyles[i],
          "PositionMeasurement",
          false);
        CreateColumn(
          obj + "DistanceY",
          prefix + Localization.Labels.DataGridYDistance,
          cellStyles[i],
          "PositionMeasurement",
          false);
        CreateColumn(
          obj + "Length",
          prefix + Localization.Labels.DataGridLength,
          cellStyles[i],
          "PositionMeasurement",
          false);
        CreateColumn(
          obj + "LengthX",
          prefix + Localization.Labels.DataGridXLength,
          cellStyles[i],
          "PositionMeasurement",
          false);
        CreateColumn(
          obj + "LengthY",
          prefix + Localization.Labels.DataGridYLength,
          cellStyles[i],
          "PositionMeasurement",
          false);
        CreateColumn(
          obj + "Velocity",
          prefix + Localization.Labels.DataGridVelocity,
          cellStyles[i],
          "VelocityMeasurement",
          false);
        CreateColumn(
          obj + "VelocityX",
          prefix + Localization.Labels.DataGridXVelocity,
          cellStyles[i],
          "VelocityMeasurement",
          false);
        CreateColumn(
          obj + "VelocityY",
          prefix + Localization.Labels.DataGridYVelocity,
          cellStyles[i],
          "VelocityMeasurement",
          false);
        CreateColumn(
          obj + "VelocityI",
          prefix + Localization.Labels.DataGridVelocityInterpolated,
          cellStyles[i],
          "VelocityMeasurement",
          true);
        CreateColumn(
          obj + "VelocityXI",
          prefix + Localization.Labels.DataGridXVelocityInterpolated,
          cellStyles[i],
          "VelocityMeasurement",
          true);
        CreateColumn(
          obj + "VelocityYI",
          prefix + Localization.Labels.DataGridYVelocityInterpolated,
          cellStyles[i],
          "VelocityMeasurement",
          true);
        CreateColumn(
          obj + "Acceleration",
          prefix + Localization.Labels.DataGridAcceleration,
          cellStyles[i],
          "AccelerationMeasurement",
          false);
        CreateColumn(
          obj + "AccelerationX",
          prefix + Localization.Labels.DataGridXAcceleration,
          cellStyles[i],
          "AccelerationMeasurement",
          false);
        CreateColumn(
          obj + "AccelerationY",
          prefix + Localization.Labels.DataGridYAcceleration,
          cellStyles[i],
          "AccelerationMeasurement",
          false);
        CreateColumn(
          obj + "AccelerationI",
          prefix + Localization.Labels.DataGridAccelerationInterpolated,
          cellStyles[i],
          "AccelerationMeasurement",
          true);
        CreateColumn(
          obj + "AccelerationXI",
          prefix + Localization.Labels.DataGridXAccelerationInterpolated,
          cellStyles[i],
          "AccelerationMeasurement",
          true);
        CreateColumn(
          obj + "AccelerationYI",
          prefix + Localization.Labels.DataGridYAccelerationInterpolated,
          cellStyles[i],
          "AccelerationMeasurement",
          true);
      }
    }
  }
}
