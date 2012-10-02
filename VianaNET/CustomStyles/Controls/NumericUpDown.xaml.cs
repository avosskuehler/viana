// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericUpDown.xaml.cs" company="Freie Universität Berlin">
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
//   The numeric up down.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Controls
{
  using System;
  using System.Diagnostics;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Automation;
  using System.Windows.Automation.Peers;
  using System.Windows.Automation.Provider;
  using System.Windows.Controls;
  using System.Windows.Input;

  /// <summary>
  ///   The numeric up down.
  /// </summary>
  public class NumericUpDown : Control
  {
    #region Constants

    /// <summary>
    ///   The default change.
    /// </summary>
    private const decimal DefaultChange = 1;

    /// <summary>
    ///   The default decimal places.
    /// </summary>
    private const int DefaultDecimalPlaces = 0;

    /// <summary>
    ///   The default max value.
    /// </summary>
    private const decimal DefaultMaxValue = 100;

    /// <summary>
    ///   The default min value.
    /// </summary>
    private const decimal DefaultMinValue = 0;

    /// <summary>
    ///   The default value.
    /// </summary>
    private const decimal DefaultValue = DefaultMinValue;

    #endregion

    #region Static Fields

    /// <summary>
    ///   The change property.
    /// </summary>
    public static readonly DependencyProperty ChangeProperty = DependencyProperty.Register(
      "Change",
      typeof(decimal),
      typeof(NumericUpDown),
      new FrameworkPropertyMetadata(DefaultChange, OnChangeChanged, CoerceChange),
      ValidateChange);

    /// <summary>
    ///   The decimal places property.
    /// </summary>
    public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register(
      "DecimalPlaces",
      typeof(int),
      typeof(NumericUpDown),
      new FrameworkPropertyMetadata(DefaultDecimalPlaces, OnDecimalPlacesChanged),
      ValidateDecimalPlaces);

    /// <summary>
    ///   The maximum property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      "Maximum",
      typeof(decimal),
      typeof(NumericUpDown),
      new FrameworkPropertyMetadata(DefaultMaxValue, OnMaximumChanged, CoerceMaximum));

    /// <summary>
    ///   The minimum property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      "Minimum",
      typeof(decimal),
      typeof(NumericUpDown),
      new FrameworkPropertyMetadata(DefaultMinValue, OnMinimumChanged, CoerceMinimum));

    /// <summary>
    ///   Identifies the ValueChanged routed event.
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
      "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(NumericUpDown));

    /// <summary>
    ///   Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value",
      typeof(decimal),
      typeof(NumericUpDown),
      new FrameworkPropertyMetadata(DefaultValue, OnValueChanged, CoerceValue));

    /// <summary>
    ///   The value string property key.
    /// </summary>
    private static readonly DependencyPropertyKey ValueStringPropertyKey =
      DependencyProperty.RegisterAttachedReadOnly(
        "ValueString", typeof(string), typeof(NumericUpDown), new PropertyMetadata());

    /// <summary>
    ///   The value string property.
    /// </summary>
    public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

    /// <summary>
    ///   The _decrease command.
    /// </summary>
    private static RoutedCommand _decreaseCommand;

    /// <summary>
    ///   The _increase command.
    /// </summary>
    private static RoutedCommand _increaseCommand;

    #endregion

    #region Fields

    /// <summary>
    ///   The _number format info.
    /// </summary>
    private readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="NumericUpDown" /> class.
    /// </summary>
    static NumericUpDown()
    {
      InitializeCommands();

      // Listen to MouseLeftButtonDown event to determine if NumericUpDown should move focus to itself
      EventManager.RegisterClassHandler(
        typeof(NumericUpDown), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);

      DefaultStyleKeyProperty.OverrideMetadata(
        typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NumericUpDown" /> class.
    /// </summary>
    public NumericUpDown()
    {
      this.updateValueString();
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   Occurs when the Value property changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
    {
      add
      {
        this.AddHandler(ValueChangedEvent, value);
      }

      remove
      {
        this.RemoveHandler(ValueChangedEvent, value);
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the decrease command.
    /// </summary>
    public static RoutedCommand DecreaseCommand
    {
      get
      {
        return _decreaseCommand;
      }
    }

    /// <summary>
    ///   Gets the increase command.
    /// </summary>
    public static RoutedCommand IncreaseCommand
    {
      get
      {
        return _increaseCommand;
      }
    }

    /// <summary>
    ///   Gets or sets the change.
    /// </summary>
    public decimal Change
    {
      get
      {
        return (decimal)this.GetValue(ChangeProperty);
      }

      set
      {
        this.SetValue(ChangeProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the decimal places.
    /// </summary>
    public int DecimalPlaces
    {
      get
      {
        return (int)this.GetValue(DecimalPlacesProperty);
      }

      set
      {
        this.SetValue(DecimalPlacesProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the maximum.
    /// </summary>
    public decimal Maximum
    {
      get
      {
        return (decimal)this.GetValue(MaximumProperty);
      }

      set
      {
        this.SetValue(MaximumProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the minimum.
    /// </summary>
    public decimal Minimum
    {
      get
      {
        return (decimal)this.GetValue(MinimumProperty);
      }

      set
      {
        this.SetValue(MinimumProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the value.
    /// </summary>
    public decimal Value
    {
      get
      {
        return (decimal)this.GetValue(ValueProperty);
      }

      set
      {
        this.SetValue(ValueProperty, value);
      }
    }

    /// <summary>
    ///   Gets the value string.
    /// </summary>
    public string ValueString
    {
      get
      {
        return (string)this.GetValue(ValueStringProperty);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the provided truth is false.
    /// </summary>
    /// <param name="truth">
    /// The value assumed to be true. 
    /// </param>
    /// <param name="parameterName">
    /// The string for <see cref="ArgumentOutOfRangeException"/> , if thrown. 
    /// </param>
    [DebuggerStepThrough]
    public static void RequireArgumentRange(bool truth, string parameterName)
    {
      RequireNotNullOrEmpty(parameterName, "parameterName");

      if (!truth)
      {
        throw new ArgumentOutOfRangeException(parameterName);
      }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the
    ///   provided string is null.
    ///   Throws an <see cref="ArgumentOutOfRangeException"/> if the
    ///   provided string is empty.
    /// </summary>
    /// <param name="stringParameter">
    /// The object to test for null and empty. 
    /// </param>
    /// <param name="parameterName">
    /// The string for the ArgumentException parameter, if thrown. 
    /// </param>
    [DebuggerStepThrough]
    public static void RequireNotNullOrEmpty(string stringParameter, string parameterName)
    {
      if (stringParameter == null)
      {
        throw new ArgumentNullException(parameterName);
      }
      else if (stringParameter.Length == 0)
      {
        throw new ArgumentOutOfRangeException(parameterName);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    ///   The on create automation peer.
    /// </summary>
    /// <returns> The <see cref="AutomationPeer" /> . </returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new NumericUpDownAutomationPeer(this);
    }

    /// <summary>
    ///   The on decrease.
    /// </summary>
    protected virtual void OnDecrease()
    {
      this.Value -= this.Change;
    }

    /// <summary>
    ///   The on increase.
    /// </summary>
    protected virtual void OnIncrease()
    {
      this.Value += this.Change;
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    /// <param name="args">
    /// Arguments associated with the ValueChanged event. 
    /// </param>
    protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> args)
    {
      this.RaiseEvent(args);
    }

    /// <summary>
    /// The coerce change.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="object"/> . 
    /// </returns>
    private static object CoerceChange(DependencyObject element, object value)
    {
      var newChange = (decimal)value;
      var control = (NumericUpDown)element;

      decimal coercedNewChange = decimal.Round(newChange, control.DecimalPlaces);

      // If Change is .1 and DecimalPlaces is changed from 1 to 0, we want Change to go to 1, not 0.
      // Put another way, Change should always be rounded to DecimalPlaces, but never smaller than the 
      // previous Change
      if (coercedNewChange < newChange)
      {
        coercedNewChange = smallestForDecimalPlaces(control.DecimalPlaces);
      }

      return coercedNewChange;
    }

    /// <summary>
    /// The coerce maximum.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="object"/> . 
    /// </returns>
    private static object CoerceMaximum(DependencyObject element, object value)
    {
      var control = (NumericUpDown)element;
      var newMaximum = (decimal)value;
      return decimal.Round(Math.Max(newMaximum, control.Minimum), control.DecimalPlaces);
    }

    /// <summary>
    /// The coerce minimum.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="object"/> . 
    /// </returns>
    private static object CoerceMinimum(DependencyObject element, object value)
    {
      var minimum = (decimal)value;
      var control = (NumericUpDown)element;
      return decimal.Round(minimum, control.DecimalPlaces);
    }

    /// <summary>
    /// The coerce value.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="object"/> . 
    /// </returns>
    private static object CoerceValue(DependencyObject element, object value)
    {
      var newValue = (decimal)value;
      var control = (NumericUpDown)element;

      newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));
      newValue = decimal.Round(newValue, control.DecimalPlaces);

      return newValue;
    }

    /// <summary>
    ///   The initialize commands.
    /// </summary>
    private static void InitializeCommands()
    {
      _increaseCommand = new RoutedCommand("IncreaseCommand", typeof(NumericUpDown));
      CommandManager.RegisterClassCommandBinding(
        typeof(NumericUpDown), new CommandBinding(_increaseCommand, OnIncreaseCommand));
      CommandManager.RegisterClassInputBinding(
        typeof(NumericUpDown), new InputBinding(_increaseCommand, new KeyGesture(Key.Up)));

      _decreaseCommand = new RoutedCommand("DecreaseCommand", typeof(NumericUpDown));
      CommandManager.RegisterClassCommandBinding(
        typeof(NumericUpDown), new CommandBinding(_decreaseCommand, OnDecreaseCommand));
      CommandManager.RegisterClassInputBinding(
        typeof(NumericUpDown), new InputBinding(_decreaseCommand, new KeyGesture(Key.Down)));
    }

    /// <summary>
    /// The on change changed.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

    /// <summary>
    /// The on decimal places changed.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      var control = (NumericUpDown)element;
      control.CoerceValue(ChangeProperty);
      control.CoerceValue(MinimumProperty);
      control.CoerceValue(MaximumProperty);
      control.CoerceValue(ValueProperty);
      control.updateValueString();
    }

    /// <summary>
    /// The on decrease command.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
    {
      var control = sender as NumericUpDown;
      if (control != null)
      {
        control.OnDecrease();
      }
    }

    /// <summary>
    /// The on increase command.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
    {
      var control = sender as NumericUpDown;
      if (control != null)
      {
        control.OnIncrease();
      }
    }

    /// <summary>
    /// The on maximum changed.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnMaximumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      element.CoerceValue(ValueProperty);
    }

    /// <summary>
    /// The on minimum changed.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      element.CoerceValue(MaximumProperty);
      element.CoerceValue(ValueProperty);
    }

    /// <summary>
    /// This is a class handler for MouseLeftButtonDown event.
    ///   The purpose of this handle is to move input focus to NumericUpDown when user pressed
    ///   mouse left button on any part of slider that is not focusable.
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var control = (NumericUpDown)sender;

      // When someone click on a part in the NumericUpDown and it's not focusable
      // NumericUpDown needs to take the focus in order to process keyboard correctly
      if (!control.IsKeyboardFocusWithin)
      {
        e.Handled = control.Focus() || e.Handled;
      }
    }

    /// <summary>
    /// The on value changed.
    /// </summary>
    /// <param name="obj">
    /// The obj. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var control = (NumericUpDown)obj;

      var oldValue = (decimal)args.OldValue;
      var newValue = (decimal)args.NewValue;

      var peer = UIElementAutomationPeer.FromElement(control) as NumericUpDownAutomationPeer;
      if (peer != null)
      {
        peer.RaiseValueChangedEvent(oldValue, newValue);
      }

      var e = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent);

      control.OnValueChanged(e);

      control.updateValueString();
    }

    /// <summary>
    /// The validate change.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    private static bool ValidateChange(object value)
    {
      var change = (decimal)value;
      return change > 0;
    }

    /// <summary>
    /// The validate decimal places.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    private static bool ValidateDecimalPlaces(object value)
    {
      var decimalPlaces = (int)value;
      return decimalPlaces >= 0;
    }

    /// <summary>
    /// The smallest for decimal places.
    /// </summary>
    /// <param name="decimalPlaces">
    /// The decimal places. 
    /// </param>
    /// <returns>
    /// The <see cref="decimal"/> . 
    /// </returns>
    private static decimal smallestForDecimalPlaces(int decimalPlaces)
    {
      RequireArgumentRange(decimalPlaces >= 0, "decimalPlaces");

      decimal d = 1;

      for (int i = 0; i < decimalPlaces; i++)
      {
        d /= 10;
      }

      return d;
    }

    /// <summary>
    ///   The update value string.
    /// </summary>
    private void updateValueString()
    {
      this._numberFormatInfo.NumberDecimalDigits = this.DecimalPlaces;
      string newValueString = this.Value.ToString("f", this._numberFormatInfo);
      this.SetValue(ValueStringPropertyKey, newValueString);
    }

    #endregion
  }

  /// <summary>
  ///   The numeric up down automation peer.
  /// </summary>
  public class NumericUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericUpDownAutomationPeer"/> class.
    /// </summary>
    /// <param name="control">
    /// The control. 
    /// </param>
    public NumericUpDownAutomationPeer(NumericUpDown control)
      : base(control)
    {
    }

    #endregion

    #region Explicit Interface Properties

    /// <summary>
    ///   Gets a value indicating whether is read only.
    /// </summary>
    bool IRangeValueProvider.IsReadOnly
    {
      get
      {
        return !this.IsEnabled();
      }
    }

    /// <summary>
    ///   Gets the large change.
    /// </summary>
    double IRangeValueProvider.LargeChange
    {
      get
      {
        return (double)this.MyOwner.Change;
      }
    }

    /// <summary>
    ///   Gets the maximum.
    /// </summary>
    double IRangeValueProvider.Maximum
    {
      get
      {
        return (double)this.MyOwner.Maximum;
      }
    }

    /// <summary>
    ///   Gets the minimum.
    /// </summary>
    double IRangeValueProvider.Minimum
    {
      get
      {
        return (double)this.MyOwner.Minimum;
      }
    }

    /// <summary>
    ///   Gets the small change.
    /// </summary>
    double IRangeValueProvider.SmallChange
    {
      get
      {
        return (double)this.MyOwner.Change;
      }
    }

    /// <summary>
    ///   Gets the value.
    /// </summary>
    double IRangeValueProvider.Value
    {
      get
      {
        return (double)this.MyOwner.Value;
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the my owner.
    /// </summary>
    private NumericUpDown MyOwner
    {
      get
      {
        return (NumericUpDown)base.Owner;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The get pattern.
    /// </summary>
    /// <param name="patternInterface">
    /// The pattern interface. 
    /// </param>
    /// <returns>
    /// The <see cref="object"/> . 
    /// </returns>
    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.RangeValue)
      {
        return this;
      }

      return base.GetPattern(patternInterface);
    }

    #endregion

    #region Explicit Interface Methods

    /// <summary>
    /// The set value.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    void IRangeValueProvider.SetValue(double value)
    {
      if (!this.IsEnabled())
      {
        throw new ArgumentOutOfRangeException();
      }

      var val = (decimal)value;
      if (val < this.MyOwner.Minimum || val > this.MyOwner.Maximum)
      {
        throw new ArgumentOutOfRangeException("value");
      }

      this.MyOwner.Value = val;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The raise value changed event.
    /// </summary>
    /// <param name="oldValue">
    /// The old value. 
    /// </param>
    /// <param name="newValue">
    /// The new value. 
    /// </param>
    internal void RaiseValueChangedEvent(decimal oldValue, decimal newValue)
    {
      this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, (double)oldValue, (double)newValue);
    }

    /// <summary>
    ///   The get automation control type core.
    /// </summary>
    /// <returns> The <see cref="AutomationControlType" /> . </returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Spinner;
    }

    /// <summary>
    ///   The get class name core.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    protected override string GetClassNameCore()
    {
      return "NumericUpDown";
    }

    #endregion
  }
}