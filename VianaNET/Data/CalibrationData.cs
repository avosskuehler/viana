// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalibrationData.cs" company="Freie Universität Berlin">
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
//   This singleton class provides static access to the properties
//   used to measure lenght in the video, if the user has provided
//   a calibration point and length.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Media;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;

  /// <summary>
  ///   This singleton class provides static access to the properties
  ///   used to measure lenght in the video, if the user has provided
  ///   a calibration point and length.
  /// </summary>
  [Serializable]
  public class CalibrationData : DependencyObject, INotifyPropertyChanged
  {
     #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IsShowingUnits" />.
    /// </summary>
    public static readonly DependencyProperty ClipRegionProperty = DependencyProperty.Register(
      "ClipRegion", typeof(Rect), typeof(CalibrationData), new FrameworkPropertyMetadata(default(Rect), OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IsShowingUnits" />.
    /// </summary>
    public static readonly DependencyProperty GradientBackgroundProperty =
      DependencyProperty.Register(
        "GradientBackground",
        typeof(LinearGradientBrush),
        typeof(CalibrationData),
        new FrameworkPropertyMetadata(
          default(LinearGradientBrush), FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="HasClipRegion" />.
    /// </summary>
    public static readonly DependencyProperty HasClipRegionProperty = DependencyProperty.Register(
      "HasClipRegion",
      typeof(bool),
      typeof(CalibrationData),
      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IsShowingUnits" />.
    /// </summary>
    public static readonly DependencyProperty IsShowingUnitsProperty = DependencyProperty.Register(
      "IsShowingUnits",
      typeof(bool),
      typeof(CalibrationData),
      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IsVideoCalibrated" />.
    /// </summary>
    public static readonly DependencyProperty IsVideoCalibratedProperty =
      DependencyProperty.Register(
        "IsVideoCalibrated",
        typeof(bool),
        typeof(CalibrationData),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="LengthUnit" />.
    /// </summary>
    public static readonly DependencyProperty LengthUnitProperty = DependencyProperty.Register(
      "LengthUnit",
      typeof(LengthUnit),
      typeof(CalibrationData),
      new FrameworkPropertyMetadata(LengthUnit.px, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IsShowingUnits" />.
    /// </summary>
    public static readonly DependencyProperty RulerValueInRulerUnitsProperty =
      DependencyProperty.Register(
        "RulerValueInRulerUnits",
        typeof(double),
        typeof(CalibrationData),
        new FrameworkPropertyMetadata(default(double), OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="TimeUnit" />.
    /// </summary>
    public static readonly DependencyProperty TimeUnitProperty = DependencyProperty.Register(
      "TimeUnit",
      typeof(TimeUnit),
      typeof(CalibrationData),
      new FrameworkPropertyMetadata(TimeUnit.s, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    #endregion

    #region Fields

    /// <summary>
    ///   Saves the <see cref="CustomStyles.Types.LengthUnit" /> used for the calibration ruler
    ///   during measuring distancs.
    /// </summary>
    private LengthUnit rulerUnit;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CalibrationData"/> class. 
    /// </summary>
    public CalibrationData()
    {
      this.InitFields();
    }

    #endregion

     #region Public Events

    /// <summary>
    ///   Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the clip region.
    /// </summary>
    public Rect ClipRegion
    {
      get
      {
        return (Rect)this.GetValue(ClipRegionProperty);
      }

      set
      {
        this.SetValue(ClipRegionProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the <see cref="LinearGradientBrush" /> for the background
    ///   of the dialogs.
    /// </summary>
    public LinearGradientBrush GradientBackground
    {
      get
      {
        return (LinearGradientBrush)this.GetValue(GradientBackgroundProperty);
      }

      set
      {
        this.SetValue(GradientBackgroundProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the video analysis frame has been
    ///   clipped by a window.
    /// </summary>
    public bool HasClipRegion
    {
      get
      {
        return (bool)this.GetValue(HasClipRegionProperty);
      }

      set
      {
        this.SetValue(HasClipRegionProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether to display or hide the units for
    ///   the measured values.
    /// </summary>
    public bool IsShowingUnits
    {
      get
      {
        return (bool)this.GetValue(IsShowingUnitsProperty);
      }

      set
      {
        this.SetValue(IsShowingUnitsProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the user has set an 
    ///   origin and a ruler measurement of a length.
    /// </summary>
    public bool IsVideoCalibrated
    {
      get
      {
        return (bool)this.GetValue(IsVideoCalibratedProperty);
      }

      set
      {
        this.SetValue(IsVideoCalibratedProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="Point" /> in video coordinates with
    ///   the origin set for measuring length.
    /// </summary>
    public Point OriginInPixel { get; set; }

    /// <summary>
    ///   Gets the string for the pixel unit.
    /// </summary>
    public string PixelUnit
    {
      get
      {
        return LengthUnit.px.ToString();
      }
    }

    /// <summary>
    ///   Gets or sets the string value
    /// </summary>
    public TimeUnit TimeUnit
    {
      get
      {
        return (TimeUnit)this.GetValue(TimeUnitProperty);
      }

      set
      {
        this.SetValue(TimeUnitProperty, value);
        this.OnPropertyChanged("VelocityUnit");
        this.OnPropertyChanged("AccelerationUnit");
      }
    }

    /// <summary>
    ///   Gets or sets the position unit string. (Pixel by default).
    /// </summary>
    public LengthUnit LengthUnit
    {
      get
      {
        return (LengthUnit)this.GetValue(LengthUnitProperty);
      }

      set
      {
        this.SetValue(LengthUnitProperty, value);
        this.OnPropertyChanged("VelocityUnit");
        this.OnPropertyChanged("AccelerationUnit");
      }
    }

    /// <summary>
    ///   Gets the velocity unit string. (Pixel per second by default)
    /// </summary>
    public string VelocityUnit
    {
      get
      {
        return string.Format("{0}/{1}", this.LengthUnit, this.TimeUnit);
      }
    }

    /// <summary>
    ///   Gets the acceleration unit string. (Pixel per second squared by default)
    /// </summary>
    public string AccelerationUnit
    {
      get
      {
        return string.Format("{0}/{1}²", this.LengthUnit, this.TimeUnit);
      }
    }

    /// <summary>
    ///   Gets or sets a <see cref="string" /> with the addon string
    ///   for the ruler value (unit).
    /// </summary>
    public string RulerDescription { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Point" /> with the ending point in
    ///   video coordinates the user placed the ruler.
    /// </summary>
    public Point RulerEndPointInPixel { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="Point" /> with the starting point in
    ///   video coordinates the user placed the ruler.
    /// </summary>
    public Point RulerStartPointInPixel { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="CustomStyles.Types.LengthUnit" /> the ruler value is measured in.
    /// </summary>
    public LengthUnit RulerUnit
    {
      get
      {
        return this.rulerUnit;
      }

      set
      {
        this.rulerUnit = value;
        this.LengthUnit = this.rulerUnit;
      }
    }

    /// <summary>
    ///   Gets or sets the ruler value in ruler units.
    /// </summary>
    public double RulerValueInRulerUnits
    {
      get
      {
        return (double)this.GetValue(RulerValueInRulerUnitsProperty);
      }

      set
      {
        this.SetValue(RulerValueInRulerUnitsProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets <see cref="double" /> containing the scale factor to
    ///   convert pixels to unit values.
    /// </summary>
    public double ScalePixelToUnit { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Resets the properties to their default values.
    /// </summary>
    public void Reset()
    {
      this.InitFields();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">
    /// The source of the event. This. 
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as CalibrationData).OnPropertyChanged(args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that changed
    /// </param>
    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    ///   Initially set the properties default values.
    /// </summary>
    private void InitFields()
    {
      this.OriginInPixel = new Point();
      this.RulerStartPointInPixel = new Point();
      this.RulerEndPointInPixel = new Point();
      this.RulerUnit = LengthUnit.px;
      this.RulerValueInRulerUnits = 0d;
      this.ScalePixelToUnit = 1d;
      this.IsVideoCalibrated = false;
      this.HasClipRegion = false;
      this.RulerDescription = string.Empty;
      this.ClipRegion = new Rect(0, 0, 0, 0);
      this.LengthUnit = LengthUnit.px;
      this.TimeUnit = TimeUnit.s;
      this.IsShowingUnits = false;
    }

    #endregion
  }
}