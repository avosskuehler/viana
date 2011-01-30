using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLocalizeExtension.Extensions;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VianaNET
{
  public abstract class InterpolationBase : DependencyObject
  {
    public Interpolation.FilterTypes FilterType
    {
      get { return (Interpolation.FilterTypes)GetValue(FilterTypeProperty); }
      set { SetValue(FilterTypeProperty, value); }
    }

    public static readonly DependencyProperty FilterTypeProperty =
      DependencyProperty.Register(
      "FilterType",
      typeof(Interpolation.FilterTypes),
      typeof(InterpolationBase),
      new FrameworkPropertyMetadata(Interpolation.FilterTypes.MovingAverage, FrameworkPropertyMetadataOptions.AffectsRender));

    public int NumberOfSamplesToInterpolate
    {
      get { return (int)GetValue(NumberOfSamplesToInterpolateProperty); }
      set { SetValue(NumberOfSamplesToInterpolateProperty, value); }
    }

    public static readonly DependencyProperty NumberOfSamplesToInterpolateProperty =
      DependencyProperty.Register(
      "NumberOfSamplesToInterpolate",
      typeof(int),
      typeof(InterpolationBase),
      new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

    public UserControl CustomUserControl { get; set; }

    public InterpolationBase()
    {
    }

    public override string ToString()
    {
      return this.FilterType.ToString();
    }

    public abstract void CalculateInterpolatedValues(DataCollection samples);
  }
}
