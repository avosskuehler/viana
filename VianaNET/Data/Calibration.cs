using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLocalizeExtension.Extensions;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace VianaNET
{
  public class Calibration : DependencyObject, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private static void OnPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      (obj as Calibration).OnPropertyChanged(args);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// Holds the instance of singleton
    /// </summary>
    private static Calibration instance;

    /// <summary>
    /// Gets the <see cref="Calibration"/> singleton.
    /// If the underlying instance is null, a instance will be created.
    /// </summary>
    public static Calibration Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new Calibration();
        }

        // return the existing/new instance
        return instance;
      }
    }

    private Calibration()
    {
      InitFields();
    }

    private void InitFields()
    {
      this.OriginInPixel = new Point();
      this.RulerStartPointInPixel = new Point();
      this.RulerEndPointInPixel = new Point();
      this.RulerUnit = Unit.px;
      this.RulerValueInRulerUnits = 0d;
      this.ScalePixelToUnit = 1d;
      this.IsVideoCalibrated = false;
      this.HasClipRegion = false;
      this.RulerDescription = string.Empty;
      this.ClipRegion = Rect.Empty;
      this.PositionUnit = "px";
      this.VelocityUnit = "px/s";
      this.AccelerationUnit = "px/s^2";
      this.IsShowingUnits = false;
    }

    public bool IsShowingUnits
    {
      get { return (bool)this.GetValue(IsShowingUnitsProperty); }
      set { this.SetValue(IsShowingUnitsProperty, value); }
    }

    public static readonly DependencyProperty IsShowingUnitsProperty = DependencyProperty.Register(
      "IsShowingUnits",
      typeof(bool),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        false,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public LinearGradientBrush GradientBackground
    {
      get { return (LinearGradientBrush)this.GetValue(GradientBackgroundProperty); }
      set { this.SetValue(GradientBackgroundProperty, value); }
    }

    public static readonly DependencyProperty GradientBackgroundProperty = DependencyProperty.Register(
      "GradientBackground",
      typeof(LinearGradientBrush),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        default(LinearGradientBrush),
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    private Unit rulerUnit;

    public Point OriginInPixel { get; set; }
    public Point RulerStartPointInPixel { get; set; }
    public Point RulerEndPointInPixel { get; set; }
    public Unit RulerUnit
    {
      get { return this.rulerUnit; }
      set
      {
        this.rulerUnit = value;
        switch (value)
        {
          case Unit.px:
            this.PositionUnit = "px";
            this.VelocityUnit = "px/s";
            this.AccelerationUnit = "px/s^2";
            break;
          case Unit.mm:
            this.PositionUnit = "mm";
            this.VelocityUnit = "mm/s";
            this.AccelerationUnit = "mm/s^2";
            break;
          case Unit.cm:
            this.PositionUnit = "cm";
            this.VelocityUnit = "cm/s";
            this.AccelerationUnit = "cm/s^2";
            break;
          case Unit.m:
            this.PositionUnit = "m";
            this.VelocityUnit = "m/s";
            this.AccelerationUnit = "m/s^2";
            break;
          case Unit.km:
            this.PositionUnit = "km";
            this.VelocityUnit = "km/s";
            this.AccelerationUnit = "km/s^2";
            break;
        }
      }
    }

    public double ScalePixelToUnit { get; set; }
    public string RulerDescription { get; set; }

    public Rect ClipRegion
    {
      get { return (Rect)this.GetValue(ClipRegionProperty); }
      set { this.SetValue(ClipRegionProperty, value); }
    }

    public static readonly DependencyProperty ClipRegionProperty = DependencyProperty.Register(
      "ClipRegion",
      typeof(Rect),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        default(Rect),
        new PropertyChangedCallback(OnPropertyChanged)));

    public double RulerValueInRulerUnits
    {
      get { return (double)this.GetValue(RulerValueInRulerUnitsProperty); }
      set { this.SetValue(RulerValueInRulerUnitsProperty, value); }
    }

    public static readonly DependencyProperty RulerValueInRulerUnitsProperty = DependencyProperty.Register(
      "RulerValueInRulerUnits",
      typeof(double),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        default(double),
        new PropertyChangedCallback(OnPropertyChanged)));

    public string TimeUnit
    {
      get { return (string)this.GetValue(TimeUnitProperty); }
      set { this.SetValue(TimeUnitProperty, value); }
    }

    public static readonly DependencyProperty TimeUnitProperty = DependencyProperty.Register(
      "TimeUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        "ms",
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public string PixelUnit
    {
      get { return (string)this.GetValue(PixelUnitProperty); }
      set { this.SetValue(PixelUnitProperty, value); }
    }

    public static readonly DependencyProperty PixelUnitProperty = DependencyProperty.Register(
      "PixelUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        "px",
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public string PositionUnit
    {
      get { return (string)this.GetValue(PositionUnitProperty); }
      set { this.SetValue(PositionUnitProperty, value); }
    }

    public static readonly DependencyProperty PositionUnitProperty = DependencyProperty.Register(
      "PositionUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        "px",
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public string VelocityUnit
    {
      get { return (string)this.GetValue(VelocityUnitProperty); }
      set { this.SetValue(VelocityUnitProperty, value); }
    }

    public static readonly DependencyProperty VelocityUnitProperty = DependencyProperty.Register(
      "VelocityUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        "px/s",
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public string AccelerationUnit
    {
      get { return (string)this.GetValue(AccelerationUnitProperty); }
      set { this.SetValue(AccelerationUnitProperty, value); }
    }

    public static readonly DependencyProperty AccelerationUnitProperty = DependencyProperty.Register(
      "AccelerationUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        "px/s^2",
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public Boolean IsVideoCalibrated
    {
      get { return (Boolean)this.GetValue(IsVideoCalibratedProperty); }
      set { this.SetValue(IsVideoCalibratedProperty, value); }
    }

    public static readonly DependencyProperty IsVideoCalibratedProperty = DependencyProperty.Register(
      "IsVideoCalibrated",
      typeof(Boolean),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        false,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public Boolean HasClipRegion
    {
      get { return (Boolean)this.GetValue(HasClipRegionProperty); }
      set { this.SetValue(HasClipRegionProperty, value); }
    }

    public static readonly DependencyProperty HasClipRegionProperty = DependencyProperty.Register(
      "HasClipRegion",
      typeof(Boolean),
      typeof(Calibration),
      new FrameworkPropertyMetadata(
        false,
        FrameworkPropertyMetadataOptions.AffectsRender,
        new PropertyChangedCallback(OnPropertyChanged)));

    public void Reset()
    {
      this.InitFields();
    }
  }
}
