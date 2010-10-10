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
      Calibration.Instance.PropertyChanged += new PropertyChangedEventHandler(DataPropertyChanged);
      VideoData.Instance.PropertyChanged += new PropertyChangedEventHandler(DataPropertyChanged);
    }

    private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Samples" || e.PropertyName == "IsShowingUnits")
      {
        this.Refresh();
      }
    }

    private void Refresh()
    {
      this.dataGrid.ItemsSource = null;
      this.dataGrid.ItemsSource = VideoData.Instance.Samples;
    }
  }
}
