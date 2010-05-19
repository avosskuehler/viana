using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows;
using System.Windows.Controls.DataVisualization;

namespace VianaNET
{
  public class VianaChart : Chart
  {
    static VianaChart()
    {
      VianaChart.DefaultStyleKeyProperty.OverrideMetadata(typeof(VianaChart),
            new FrameworkPropertyMetadata(typeof(VianaChart)));
    }

    public VianaChart()
    {
    }

    public static readonly DependencyProperty IsShowingLegendProperty = DependencyProperty.Register(
  "IsShowingLegend",
  typeof(Visibility),
  typeof(VianaChart),
  new FrameworkPropertyMetadata(Visibility.Visible,
    FrameworkPropertyMetadataOptions.AffectsMeasure));

    public Visibility IsShowingLegend
    {
      get { return (Visibility)GetValue(IsShowingLegendProperty); }
      set { SetValue(IsShowingLegendProperty, value); }
    }

  //  public static readonly DependencyProperty IsShowingAxisDescriptionProperty = DependencyProperty.Register(
  //"IsShowingAxisDescription",
  //typeof(Visibility),
  //typeof(VianaChart),
  //new FrameworkPropertyMetadata(Visibility.Visible,
  //  FrameworkPropertyMetadataOptions.AffectsMeasure));

  //  public Visibility IsShowingAxisDescription
  //  {
  //    get { return (Visibility)GetValue(IsShowingAxisDescriptionProperty); }
  //    set { SetValue(IsShowingAxisDescriptionProperty, value); }
  //  }

  }
}
