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
    public event PropertyChangedEventHandler CalibrationPropertyChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    //private static void OnPropertyChanged(
    //  DependencyObject obj,
    //  DependencyPropertyChangedEventArgs args)
    //{
    //  (obj as Calibration).OnPropertyChanged(args);
    //}

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
      this.TargetColor = Colors.Red;
      this.ColorTolerance = 60;
      this.IsShowingUnits = false;
      this.BlobMinDiameter = 4;
      this.BlobMaxDiameter = 100;
      this.IsTargetColorSet = false;
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
        new PropertyChangedCallback(OnIsShowingUnitsPropertyChanged)));

    private static void OnIsShowingUnitsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as Calibration).OnPropertyChanged("IsShowingUnits");
    }

    public LinearGradientBrush GradientBackground
    {
      get { return (LinearGradientBrush)this.GetValue(GradientBackgroundProperty); }
      set { this.SetValue(GradientBackgroundProperty, value); }
    }

    public static readonly DependencyProperty GradientBackgroundProperty = DependencyProperty.Register(
      "GradientBackground",
      typeof(LinearGradientBrush),
      typeof(Calibration),
      new FrameworkPropertyMetadata(default(LinearGradientBrush),
        FrameworkPropertyMetadataOptions.AffectsRender));

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
    public Rect ClipRegion { get; set; }

    public System.Drawing.Rectangle SystemDrawingClipRegion
    {
      get
      {
        return new System.Drawing.Rectangle(
          (int)ClipRegion.X,
          (int)ClipRegion.Y,
          (int)ClipRegion.Width,
          (int)ClipRegion.Height);
      }
    }

    public double RulerValueInRulerUnits
    {
      get { return (double)this.GetValue(RulerValueInRulerUnitsProperty); }
      set { this.SetValue(RulerValueInRulerUnitsProperty, value); }
    }

    public static readonly DependencyProperty RulerValueInRulerUnitsProperty = DependencyProperty.Register(
      "RulerValueInRulerUnits",
      typeof(double),
      typeof(Calibration),
      new FrameworkPropertyMetadata(default(double)));

    public double ColorTolerance
    {
      get { return (double)this.GetValue(ColorToleranceProperty); }
      set { this.SetValue(ColorToleranceProperty, value); }
    }

    public static readonly DependencyProperty ColorToleranceProperty = DependencyProperty.Register(
      "ColorTolerance",
      typeof(double),
      typeof(Calibration),
      new FrameworkPropertyMetadata(60d,
        FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBlobDetectionPropertyChanged)));

    public double BlobMinDiameter
    {
      get { return (double)this.GetValue(BlobMinDiameterProperty); }
      set { this.SetValue(BlobMinDiameterProperty, value); }
    }

    public static readonly DependencyProperty BlobMinDiameterProperty = DependencyProperty.Register(
      "BlobMinDiameter",
      typeof(double),
      typeof(Calibration),
      new FrameworkPropertyMetadata(4d,
        FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBlobDetectionPropertyChanged)));

    public double BlobMaxDiameter
    {
      get { return (double)this.GetValue(BlobMaxDiameterProperty); }
      set { this.SetValue(BlobMaxDiameterProperty, value); }
    }

    public static readonly DependencyProperty BlobMaxDiameterProperty = DependencyProperty.Register(
      "BlobMaxDiameter",
      typeof(double),
      typeof(Calibration),
      new FrameworkPropertyMetadata(100d,
        FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBlobDetectionPropertyChanged)));

    public Color TargetColor
    {
      get { return (Color)this.GetValue(TargetColorProperty); }
      set { this.SetValue(TargetColorProperty, value); }
    }

    public static readonly DependencyProperty TargetColorProperty = DependencyProperty.Register(
      "TargetColor",
      typeof(Color),
      typeof(Calibration),
      new FrameworkPropertyMetadata(Colors.Red,
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string TimeUnit
    {
      get { return (string)this.GetValue(TimeUnitProperty); }
      set { this.SetValue(TimeUnitProperty, value); }
    }

    public static readonly DependencyProperty TimeUnitProperty = DependencyProperty.Register(
      "TimeUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata("ms",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string PixelUnit
    {
      get { return (string)this.GetValue(PixelUnitProperty); }
      set { this.SetValue(PixelUnitProperty, value); }
    }

    public static readonly DependencyProperty PixelUnitProperty = DependencyProperty.Register(
      "PixelUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata("px",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string PositionUnit
    {
      get { return (string)this.GetValue(PositionUnitProperty); }
      set { this.SetValue(PositionUnitProperty, value); }
    }

    public static readonly DependencyProperty PositionUnitProperty = DependencyProperty.Register(
      "PositionUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata("px",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string VelocityUnit
    {
      get { return (string)this.GetValue(VelocityUnitProperty); }
      set { this.SetValue(VelocityUnitProperty, value); }
    }

    public static readonly DependencyProperty VelocityUnitProperty = DependencyProperty.Register(
      "VelocityUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata("px/s",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string AccelerationUnit
    {
      get { return (string)this.GetValue(AccelerationUnitProperty); }
      set { this.SetValue(AccelerationUnitProperty, value); }
    }

    public static readonly DependencyProperty AccelerationUnitProperty = DependencyProperty.Register(
      "AccelerationUnit",
      typeof(string),
      typeof(Calibration),
      new FrameworkPropertyMetadata("px/s^2",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public Boolean IsTargetColorSet
    {
      get { return (Boolean)this.GetValue(IsTargetColorSetProperty); }
      set { this.SetValue(IsTargetColorSetProperty, value); }
    }

    public static readonly DependencyProperty IsTargetColorSetProperty = DependencyProperty.Register(
      "IsTargetColorSet",
      typeof(Boolean),
      typeof(Calibration),
      new FrameworkPropertyMetadata(false));

    public Boolean IsVideoCalibrated
    {
      get { return (Boolean)this.GetValue(IsVideoCalibratedProperty); }
      set { this.SetValue(IsVideoCalibratedProperty, value); }
    }

    public static readonly DependencyProperty IsVideoCalibratedProperty = DependencyProperty.Register(
      "IsVideoCalibrated",
      typeof(Boolean),
      typeof(Calibration),
      new FrameworkPropertyMetadata(false,
        FrameworkPropertyMetadataOptions.AffectsRender));

    public Boolean HasClipRegion
    {
      get { return (Boolean)this.GetValue(HasClipRegionProperty); }
      set { this.SetValue(HasClipRegionProperty, value); }
    }

    public static readonly DependencyProperty HasClipRegionProperty = DependencyProperty.Register(
      "HasClipRegion", typeof(Boolean), typeof(Calibration));


    private static void OnBlobDetectionPropertyChanged(
  DependencyObject obj,
  DependencyPropertyChangedEventArgs args)
    {
      (obj as Calibration).OnBlobDetectionPropertyChanged(args);
    }

    private void OnBlobDetectionPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.CalibrationPropertyChanged != null)
      {
        this.CalibrationPropertyChanged(this, new PropertyChangedEventArgs("BlobDetection"));
      }
    }

    public void Reset()
    {
      this.InitFields();
    }

  }
}
