using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLocalizeExtension.Extensions;
using System.Collections.ObjectModel;

namespace VianaNET
{
  public class DataAxis : DependencyObject
  {
    public static List<DataAxis> DataAxes;

    public AxisType Axis
    {
      get { return (AxisType)GetValue(AxisProperty); }
      set { SetValue(AxisProperty, value); }
    }

    public static readonly DependencyProperty AxisProperty =
      DependencyProperty.Register(
      "Axis",
      typeof(AxisType),
      typeof(DataAxis),
      new FrameworkPropertyMetadata(default(AxisType), FrameworkPropertyMetadataOptions.AffectsRender));

    public string Description
    {
      get { return (string)GetValue(DescriptionProperty); }
      set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
      DependencyProperty.Register(
      "Description",
      typeof(string),
      typeof(DataAxis),
      new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

    public DataAxis()
    {
    }

    public DataAxis(AxisType axis)
    {
      this.Axis = axis;
    }

    public override string ToString()
    {
      return this.Description;
    }

    static DataAxis()
    {
      DataAxis iAxis = new DataAxis(AxisType.I);
      LocTextExtension lociAxis = new LocTextExtension("VianaNET:Labels:AxisFrame");
      lociAxis.SetBinding(iAxis, DataAxis.DescriptionProperty);

      DataAxis tAxis = new DataAxis(AxisType.T);
      LocTextExtension loctAxis = new LocTextExtension("VianaNET:Labels:AxisTime");
      loctAxis.SetBinding(tAxis, DataAxis.DescriptionProperty);

      DataAxis pxAxis = new DataAxis(AxisType.PX);
      LocTextExtension locpxAxis = new LocTextExtension("VianaNET:Labels:AxisCoordinateX");
      locpxAxis.SetBinding(pxAxis, DataAxis.DescriptionProperty);

      DataAxis pyAxis = new DataAxis(AxisType.PY);
      LocTextExtension locpyAxis = new LocTextExtension("VianaNET:Labels:AxisCoordinateY");
      locpyAxis.SetBinding(pyAxis, DataAxis.DescriptionProperty);

      DataAxis dAxis = new DataAxis(AxisType.D);
      LocTextExtension locdAxis = new LocTextExtension("VianaNET:Labels:AxisDistance");
      locdAxis.SetBinding(dAxis, DataAxis.DescriptionProperty);

      DataAxis dxAxis = new DataAxis(AxisType.DX);
      LocTextExtension locdxAxis = new LocTextExtension("VianaNET:Labels:AxisDistanceX");
      locdxAxis.SetBinding(dxAxis, DataAxis.DescriptionProperty);

      DataAxis dyAxis = new DataAxis(AxisType.DY);
      LocTextExtension locdyAxis = new LocTextExtension("VianaNET:Labels:AxisDistanceY");
      locdyAxis.SetBinding(dyAxis, DataAxis.DescriptionProperty);

      DataAxis vAxis = new DataAxis(AxisType.V);
      LocTextExtension locvAxis = new LocTextExtension("VianaNET:Labels:AxisVelocity");
      locvAxis.SetBinding(vAxis, DataAxis.DescriptionProperty);

      DataAxis vxAxis = new DataAxis(AxisType.VX);
      LocTextExtension locvxAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityXDirection");
      locvxAxis.SetBinding(vxAxis, DataAxis.DescriptionProperty);

      DataAxis vyAxis = new DataAxis(AxisType.VY);
      LocTextExtension locvyAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityYDirection");
      locvyAxis.SetBinding(vyAxis, DataAxis.DescriptionProperty);

      DataAxis aAxis = new DataAxis(AxisType.A);
      LocTextExtension locaAxis = new LocTextExtension("VianaNET:Labels:AxisAcceleration");
      locaAxis.SetBinding(aAxis, DataAxis.DescriptionProperty);

      DataAxis axAxis = new DataAxis(AxisType.AX);
      LocTextExtension locaxAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationXDirection");
      locaxAxis.SetBinding(axAxis, DataAxis.DescriptionProperty);

      DataAxis ayAxis = new DataAxis(AxisType.AY);
      LocTextExtension locayAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationYDirection");
      locayAxis.SetBinding(ayAxis, DataAxis.DescriptionProperty);

      DataAxes = new List<DataAxis>();
      DataAxes.Add(iAxis);
      DataAxes.Add(tAxis);
      DataAxes.Add(pxAxis);
      DataAxes.Add(pyAxis);
      DataAxes.Add(dAxis);
      DataAxes.Add(dxAxis);
      DataAxes.Add(dyAxis);
      DataAxes.Add(vAxis);
      DataAxes.Add(vxAxis);
      DataAxes.Add(vyAxis);
      DataAxes.Add(aAxis);
      DataAxes.Add(axAxis);
      DataAxes.Add(ayAxis);
    }
  }
}
