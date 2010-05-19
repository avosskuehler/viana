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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VianaNET
{
  /// <summary>
  /// Interaction logic for TopFrame.xaml
  /// </summary>
  public partial class TopFrame : UserControl
  {
    public TopFrame()
    {
      InitializeComponent();
    }

    public string Title
    {
      get { return this.title.Text; }
      set { this.title.Text = value; }
    }

    public ImageSource Icon
    {
      set { this.icon.Source = value; }
    }
  }
}
