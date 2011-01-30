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
    public static List<DataAxis> PredefinedChartAxesOrtsraum;
    public static List<DataAxis> PredefinedChartAxesPhasenraum;

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
      // Single axis data
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

      DataAxis sAxis = new DataAxis(AxisType.S);
      LocTextExtension locsAxis = new LocTextExtension("VianaNET:Labels:AxisLength");
      locsAxis.SetBinding(sAxis, DataAxis.DescriptionProperty);

      DataAxis sxAxis = new DataAxis(AxisType.SX);
      LocTextExtension locsxAxis = new LocTextExtension("VianaNET:Labels:AxisLengthX");
      locsxAxis.SetBinding(sxAxis, DataAxis.DescriptionProperty);

      DataAxis syAxis = new DataAxis(AxisType.SY);
      LocTextExtension locsyAxis = new LocTextExtension("VianaNET:Labels:AxisLengthY");
      locsyAxis.SetBinding(syAxis, DataAxis.DescriptionProperty);

      DataAxis vAxis = new DataAxis(AxisType.V);
      LocTextExtension locvAxis = new LocTextExtension("VianaNET:Labels:AxisVelocity");
      locvAxis.SetBinding(vAxis, DataAxis.DescriptionProperty);

      DataAxis vxAxis = new DataAxis(AxisType.VX);
      LocTextExtension locvxAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityXDirection");
      locvxAxis.SetBinding(vxAxis, DataAxis.DescriptionProperty);

      DataAxis vyAxis = new DataAxis(AxisType.VY);
      LocTextExtension locvyAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityYDirection");
      locvyAxis.SetBinding(vyAxis, DataAxis.DescriptionProperty);

      DataAxis viAxis = new DataAxis(AxisType.VI);
      LocTextExtension locviAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityInterpolated");
      locviAxis.SetBinding(viAxis, DataAxis.DescriptionProperty);

      DataAxis vxiAxis = new DataAxis(AxisType.VXI);
      LocTextExtension locvxiAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityXDirectionInterpolated");
      locvxiAxis.SetBinding(vxiAxis, DataAxis.DescriptionProperty);

      DataAxis vyiAxis = new DataAxis(AxisType.VYI);
      LocTextExtension locvyiAxis = new LocTextExtension("VianaNET:Labels:AxisVelocityYDirectionInterpolated");
      locvyiAxis.SetBinding(vyiAxis, DataAxis.DescriptionProperty);

      DataAxis aAxis = new DataAxis(AxisType.A);
      LocTextExtension locaAxis = new LocTextExtension("VianaNET:Labels:AxisAcceleration");
      locaAxis.SetBinding(aAxis, DataAxis.DescriptionProperty);

      DataAxis axAxis = new DataAxis(AxisType.AX);
      LocTextExtension locaxAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationXDirection");
      locaxAxis.SetBinding(axAxis, DataAxis.DescriptionProperty);

      DataAxis ayAxis = new DataAxis(AxisType.AY);
      LocTextExtension locayAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationYDirection");
      locayAxis.SetBinding(ayAxis, DataAxis.DescriptionProperty);

      DataAxis aiAxis = new DataAxis(AxisType.AI);
      LocTextExtension locaiAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationInterpolated");
      locaiAxis.SetBinding(aiAxis, DataAxis.DescriptionProperty);

      DataAxis axiAxis = new DataAxis(AxisType.AXI);
      LocTextExtension locaxiAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationXDirectionInterpolated");
      locaxiAxis.SetBinding(axiAxis, DataAxis.DescriptionProperty);

      DataAxis ayiAxis = new DataAxis(AxisType.AYI);
      LocTextExtension locayiAxis = new LocTextExtension("VianaNET:Labels:AxisAccelerationYDirectionInterpolated");
      locayiAxis.SetBinding(ayiAxis, DataAxis.DescriptionProperty);

      DataAxes = new List<DataAxis>();
      DataAxes.Add(iAxis);
      DataAxes.Add(tAxis);
      DataAxes.Add(pxAxis);
      DataAxes.Add(pyAxis);
      DataAxes.Add(dAxis);
      DataAxes.Add(dxAxis);
      DataAxes.Add(dyAxis);
      DataAxes.Add(sAxis);
      DataAxes.Add(sxAxis);
      DataAxes.Add(syAxis);
      DataAxes.Add(vAxis);
      DataAxes.Add(vxAxis);
      DataAxes.Add(vyAxis);
      DataAxes.Add(viAxis);
      DataAxes.Add(vxiAxis);
      DataAxes.Add(vyiAxis);
      DataAxes.Add(aAxis);
      DataAxes.Add(axAxis);
      DataAxes.Add(ayAxis);
      DataAxes.Add(aiAxis);
      DataAxes.Add(axiAxis);
      DataAxes.Add(ayiAxis);

      // Double axis data ortsraum
      DataAxis YoverX = new DataAxis(AxisType.YoverX);
      LocTextExtension locYoverX = new LocTextExtension("VianaNET:Labels:AxisYoverX");
      locYoverX.SetBinding(YoverX, DataAxis.DescriptionProperty);
      DataAxis XoverT = new DataAxis(AxisType.XoverT);
      LocTextExtension locXoverT = new LocTextExtension("VianaNET:Labels:AxisXoverT");
      locXoverT.SetBinding(XoverT, DataAxis.DescriptionProperty);
      DataAxis YoverT = new DataAxis(AxisType.YoverT);
      LocTextExtension locYoverT = new LocTextExtension("VianaNET:Labels:AxisYoverT");
      locYoverT.SetBinding(YoverT, DataAxis.DescriptionProperty);
      DataAxis SoverT = new DataAxis(AxisType.SoverT);
      LocTextExtension locSoverT = new LocTextExtension("VianaNET:Labels:AxisSoverT");
      locSoverT.SetBinding(SoverT, DataAxis.DescriptionProperty);
      DataAxis VoverT = new DataAxis(AxisType.VoverT);
      LocTextExtension locVoverT = new LocTextExtension("VianaNET:Labels:AxisVoverT");
      locVoverT.SetBinding(VoverT, DataAxis.DescriptionProperty);
      DataAxis VXoverT = new DataAxis(AxisType.VXoverT);
      LocTextExtension locVXoverT = new LocTextExtension("VianaNET:Labels:AxisVXoverT");
      locVXoverT.SetBinding(VXoverT, DataAxis.DescriptionProperty);
      DataAxis VYoverT = new DataAxis(AxisType.VYoverT);
      LocTextExtension locVYoverT = new LocTextExtension("VianaNET:Labels:AxisVYoverT");
      locVYoverT.SetBinding(VYoverT, DataAxis.DescriptionProperty);
      DataAxis AoverT = new DataAxis(AxisType.AoverT);
      LocTextExtension locAoverT = new LocTextExtension("VianaNET:Labels:AxisAoverT");
      locAoverT.SetBinding(AoverT, DataAxis.DescriptionProperty);
      DataAxis AXoverT = new DataAxis(AxisType.AXoverT);
      LocTextExtension locAXoverT = new LocTextExtension("VianaNET:Labels:AxisAXoverT");
      locAXoverT.SetBinding(AXoverT, DataAxis.DescriptionProperty);
      DataAxis AYoverT = new DataAxis(AxisType.AYoverT);
      LocTextExtension locAYoverT = new LocTextExtension("VianaNET:Labels:AxisAYoverT");
      locAYoverT.SetBinding(AYoverT, DataAxis.DescriptionProperty);

      PredefinedChartAxesOrtsraum = new List<DataAxis>();
      PredefinedChartAxesOrtsraum.Add(YoverX);
      PredefinedChartAxesOrtsraum.Add(XoverT);
      PredefinedChartAxesOrtsraum.Add(YoverT);
      PredefinedChartAxesOrtsraum.Add(VoverT);
      PredefinedChartAxesOrtsraum.Add(VXoverT);
      PredefinedChartAxesOrtsraum.Add(VYoverT);
      PredefinedChartAxesOrtsraum.Add(AoverT);
      PredefinedChartAxesOrtsraum.Add(AXoverT);
      PredefinedChartAxesOrtsraum.Add(AYoverT);

      // Double axis data phasenraum
      DataAxis VoverD = new DataAxis(AxisType.VoverD);
      LocTextExtension locDoverV = new LocTextExtension("VianaNET:Labels:AxisDoverV");
      locDoverV.SetBinding(VoverD, DataAxis.DescriptionProperty);
      DataAxis VXoverDX = new DataAxis(AxisType.VXoverDX);
      LocTextExtension locDXoverVX = new LocTextExtension("VianaNET:Labels:AxisDXoverVX");
      locDXoverVX.SetBinding(VXoverDX, DataAxis.DescriptionProperty);
      DataAxis VYoverDY = new DataAxis(AxisType.VYoverDY);
      LocTextExtension locDYoverVY = new LocTextExtension("VianaNET:Labels:AxisDYoverVY");
      locDYoverVY.SetBinding(VYoverDY, DataAxis.DescriptionProperty);
      DataAxis VoverS = new DataAxis(AxisType.AYoverT);
      LocTextExtension locSoverV = new LocTextExtension("VianaNET:Labels:AxisSoverV");
      locSoverV.SetBinding(VoverS, DataAxis.DescriptionProperty);
      DataAxis VXoverSX = new DataAxis(AxisType.VXoverSX);
      LocTextExtension locSXoverVX = new LocTextExtension("VianaNET:Labels:AxisSXoverVX");
      locSXoverVX.SetBinding(VXoverSX, DataAxis.DescriptionProperty);
      DataAxis VYoverSY = new DataAxis(AxisType.VYoverSY);
      LocTextExtension locSYoverVY = new LocTextExtension("VianaNET:Labels:AxisSYoverVY");
      locSYoverVY.SetBinding(AYoverT, DataAxis.DescriptionProperty);
      DataAxis AoverV = new DataAxis(AxisType.AoverV);
      LocTextExtension locVoverA = new LocTextExtension("VianaNET:Labels:AxisVoverA");
      locVoverA.SetBinding(AoverV, DataAxis.DescriptionProperty);
      DataAxis AXoverVX = new DataAxis(AxisType.AYoverT);
      LocTextExtension locVXoverAX = new LocTextExtension("VianaNET:Labels:AxisVXoverAX");
      locVXoverAX.SetBinding(AXoverVX, DataAxis.DescriptionProperty);
      DataAxis AYoverVY = new DataAxis(AxisType.AYoverVY);
      LocTextExtension locVYoverAY = new LocTextExtension("VianaNET:Labels:AxisVYoverAY");
      locVYoverAY.SetBinding(AYoverVY, DataAxis.DescriptionProperty);

      PredefinedChartAxesPhasenraum = new List<DataAxis>();
      PredefinedChartAxesPhasenraum.Add(VoverD);
      PredefinedChartAxesPhasenraum.Add(VXoverDX);
      PredefinedChartAxesPhasenraum.Add(VYoverDY);
      PredefinedChartAxesPhasenraum.Add(VoverS);
      PredefinedChartAxesPhasenraum.Add(VXoverSX);
      PredefinedChartAxesPhasenraum.Add(VYoverSY);
      PredefinedChartAxesPhasenraum.Add(AoverV);
      PredefinedChartAxesPhasenraum.Add(AXoverVX);
      PredefinedChartAxesPhasenraum.Add(AYoverVY);
    }
  }
}
