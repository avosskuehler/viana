// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSlider.xaml.cs" company="Freie Universität Berlin">
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
//   Interaction logic for MediaSlider.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Controls
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Shapes;

  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   Interaction logic for MediaSlider.xaml
  /// </summary>
  public class MediaSlider : Slider
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constants

    /// <summary>
    ///   The timeline mouse capture margin.
    /// </summary>
    private const int timelineMouseCaptureMargin = 25;

    #endregion

    #region Static Fields

    /// <summary>
    ///   The current time string property.
    /// </summary>
    public static readonly DependencyProperty CurrentTimeStringProperty =
      DependencyProperty.Register(
        "CurrentTimeString", 
        typeof(string), 
        typeof(MediaSlider), 
        new FrameworkPropertyMetadata("0:000", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The frame time in nano seconds property.
    /// </summary>
    public static readonly DependencyProperty FrameTimeInNanoSecondsProperty =
      DependencyProperty.Register(
        "FrameTimeInNanoSeconds", 
        typeof(long), 
        typeof(MediaSlider), 
        new UIPropertyMetadata(default(long), OnFrameTimeChanged));

    /// <summary>
    ///   The is showing times property.
    /// </summary>
    public static readonly DependencyProperty IsShowingTimesProperty = DependencyProperty.Register(
      "IsShowingTimes", 
      typeof(Visibility), 
      typeof(MediaSlider), 
      new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The maximum string property.
    /// </summary>
    public static readonly DependencyProperty MaximumStringProperty = DependencyProperty.Register(
      "MaximumString", 
      typeof(string), 
      typeof(MediaSlider), 
      new FrameworkPropertyMetadata("0:000", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The minimum string property.
    /// </summary>
    public static readonly DependencyProperty MinimumStringProperty = DependencyProperty.Register(
      "MinimumString", 
      typeof(string), 
      typeof(MediaSlider), 
      new FrameworkPropertyMetadata("0:000", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The selection end string property.
    /// </summary>
    public static readonly DependencyProperty SelectionEndStringProperty =
      DependencyProperty.Register(
        "SelectionEndString", 
        typeof(string), 
        typeof(MediaSlider), 
        new FrameworkPropertyMetadata("0:000", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The selection start string property.
    /// </summary>
    public static readonly DependencyProperty SelectionStartStringProperty =
      DependencyProperty.Register(
        "SelectionStartString", 
        typeof(string), 
        typeof(MediaSlider), 
        new FrameworkPropertyMetadata("0:000", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    ///   The tick down clicked command.
    /// </summary>
    public static RoutedCommand TickDownClickedCommand;

    /// <summary>
    ///   The tick up clicked command.
    /// </summary>
    public static RoutedCommand TickUpClickedCommand;

    #endregion

    #region Fields

    /// <summary>
    ///   The current range selection thumb.
    /// </summary>
    private RangeSelectionThumb currentRangeSelectionThumb;

    /// <summary>
    ///   The timeline selection canvas.
    /// </summary>
    private Canvas timelineSelectionCanvas;

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   The timeline selection range.
    /// </summary>
    private Rectangle timelineSelectionRange;

    /// <summary>
    ///   The timeline thumb.
    /// </summary>
    private Thumb timelineThumb;

    /// <summary>
    ///   The timeline tick down button.
    /// </summary>
    private RepeatButton timelineTickDownButton;

    /// <summary>
    ///   The timeline tick up button.
    /// </summary>
    private RepeatButton timelineTickUpButton;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="MediaSlider" /> class.
    /// </summary>
    static MediaSlider()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaSlider), new FrameworkPropertyMetadata(typeof(MediaSlider)));

      TickDownClickedCommand = new RoutedCommand("TickDown", typeof(MediaSlider));
      TickUpClickedCommand = new RoutedCommand("TickUp", typeof(MediaSlider));
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="MediaSlider" /> class.
    /// </summary>
    public MediaSlider()
    {
      var bindingTickDownClicked = new CommandBinding(TickDownClickedCommand, this.TickDownClickedCommand_Executed);
      this.CommandBindings.Add(bindingTickDownClicked);

      var bindingTickUpClicked = new CommandBinding(TickUpClickedCommand, this.TickUpClickedCommand_Executed);
      this.CommandBindings.Add(bindingTickUpClicked);
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The selection and value changed.
    /// </summary>
    public event EventHandler SelectionAndValueChanged;

    /// <summary>
    ///   The selection end reached.
    /// </summary>
    public event EventHandler SelectionEndReached;

    /// <summary>
    ///   The selection start reached.
    /// </summary>
    public event EventHandler SelectionStartReached;

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   The tick down clicked.
    /// </summary>
    public event EventHandler TickDownClicked;

    /// <summary>
    ///   The tick up clicked.
    /// </summary>
    public event EventHandler TickUpClicked;

    #endregion

    #region Enums

    /// <summary>
    ///   The range selection thumb.
    /// </summary>
    private enum RangeSelectionThumb
    {
      /// <summary>
      ///   The none.
      /// </summary>
      None, 

      /// <summary>
      ///   The start.
      /// </summary>
      Start, 

      /// <summary>
      ///   The end.
      /// </summary>
      End, 
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the current time string.
    /// </summary>
    public string CurrentTimeString
    {
      get
      {
        return (string)this.GetValue(CurrentTimeStringProperty);
      }

      set
      {
        this.SetValue(CurrentTimeStringProperty, value);
      }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   Time between frames in ms units.
    /// </summary>
    public long FrameTimeInNanoSeconds
    {
      get
      {
        return (long)this.GetValue(FrameTimeInNanoSecondsProperty);
      }

      set
      {
        this.SetValue(FrameTimeInNanoSecondsProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the is showing times.
    /// </summary>
    public Visibility IsShowingTimes
    {
      get
      {
        return (Visibility)this.GetValue(IsShowingTimesProperty);
      }

      set
      {
        this.SetValue(IsShowingTimesProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the maximum string.
    /// </summary>
    public string MaximumString
    {
      get
      {
        return (string)this.GetValue(MaximumStringProperty);
      }

      set
      {
        this.SetValue(MaximumStringProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the minimum string.
    /// </summary>
    public string MinimumString
    {
      get
      {
        return (string)this.GetValue(MinimumStringProperty);
      }

      set
      {
        this.SetValue(MinimumStringProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the selection end string.
    /// </summary>
    public string SelectionEndString
    {
      get
      {
        return (string)this.GetValue(SelectionEndStringProperty);
      }

      set
      {
        this.SetValue(SelectionEndStringProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the selection start string.
    /// </summary>
    public string SelectionStartString
    {
      get
      {
        return (string)this.GetValue(SelectionStartStringProperty);
      }

      set
      {
        this.SetValue(SelectionStartStringProperty, value);
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Methods and Operators

    /// <summary>
    ///   The on apply template.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (this.Template != null)
      {
        this.timelineSelectionRange = this.Template.FindName("PART_SelectionRange", this) as Rectangle;
        this.timelineThumb = this.Template.FindName("Thumb", this) as Thumb;
        this.timelineSelectionCanvas = this.Template.FindName("SelectionCanvas", this) as Canvas;
        this.timelineTickDownButton = this.Template.FindName("TickDownButton", this) as RepeatButton;
        this.timelineTickUpButton = this.Template.FindName("TickUpButton", this) as RepeatButton;
      }
    }

    /// <summary>
    ///   Resets the selection range to 0 .. Maximum.
    ///   Selecting the whole data range.
    /// </summary>
    public void ResetSelection()
    {
      this.SelectionStart = 0;
      this.SelectionEnd = this.Maximum;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on maximum changed.
    /// </summary>
    /// <param name="oldMaximum">
    /// The old maximum. 
    /// </param>
    /// <param name="newMaximum">
    /// The new maximum. 
    /// </param>
    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);
      this.MaximumString = this.ConvertToTimeString(newMaximum);
      this.SelectionEnd = newMaximum;
      this.SelectionEndString = this.ConvertToTimeString(this.SelectionEnd);
    }

    /// <summary>
    /// The on minimum changed.
    /// </summary>
    /// <param name="oldMinimum">
    /// The old minimum. 
    /// </param>
    /// <param name="newMinimum">
    /// The new minimum. 
    /// </param>
    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);
      this.MinimumString = this.ConvertToTimeString(newMinimum);
      this.SelectionStartString = this.ConvertToTimeString(this.SelectionStart);
    }

    /// <summary>
    /// The on preview mouse left button down.
    /// </summary>
    /// <param name="e">
    /// The e. 
    /// </param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);
      if (!this.timelineThumb.IsMouseOver && !this.timelineTickUpButton.IsMouseOver
          && !this.timelineTickDownButton.IsMouseOver)
      {
        this.CheckWhichRangeEndToChange(e.GetPosition(this.timelineSelectionRange));
        this.CaptureMouse();
      }
    }

    /// <summary>
    /// The on preview mouse left button up.
    /// </summary>
    /// <param name="e">
    /// The e. 
    /// </param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);
      if (!this.timelineThumb.IsMouseOver && !this.timelineTickUpButton.IsMouseOver
          && !this.timelineTickDownButton.IsMouseOver)
      {
        this.UpdateSelectionRange(e.GetPosition(this.timelineSelectionCanvas));
        this.currentRangeSelectionThumb = RangeSelectionThumb.None;
        this.ReleaseMouseCapture();
      }
    }

    /// <summary>
    /// The on preview mouse move.
    /// </summary>
    /// <param name="e">
    /// The e. 
    /// </param>
    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      base.OnPreviewMouseMove(e);
      if (e.LeftButton == MouseButtonState.Pressed && !this.timelineThumb.IsMouseOver
          && !this.timelineTickUpButton.IsMouseOver && !this.timelineTickDownButton.IsMouseOver)
      {
        this.UpdateSelectionRange(e.GetPosition(this.timelineSelectionCanvas));
      }
    }

    /// <summary>
    /// The on value changed.
    /// </summary>
    /// <param name="oldValue">
    /// The old value. 
    /// </param>
    /// <param name="newValue">
    /// The new value. 
    /// </param>
    protected override void OnValueChanged(double oldValue, double newValue)
    {
      if (newValue > this.SelectionEnd)
      {
        if (this.SelectionEndReached != null)
        {
          this.SelectionEndReached(this, EventArgs.Empty);
        }

        this.Value = this.SelectionEnd;

        // newValue = oldValue;
        // this.Value = this.SelectionEnd;
        newValue = this.SelectionEnd;
      }
      else if (newValue < this.SelectionStart)
      {
        if (this.SelectionStartReached != null)
        {
          this.SelectionStartReached(this, EventArgs.Empty);
        }

        this.Value = this.SelectionStart;
        newValue = this.SelectionStart;
      }

      this.CurrentTimeString = this.ConvertToTimeString(newValue);
      base.OnValueChanged(oldValue, newValue);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The on frame time changed.
    /// </summary>
    /// <param name="obj">
    /// The obj. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    private static void OnFrameTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as MediaSlider).OnFrameTimeChanged(args);
    }

    /// <summary>
    /// The check which range end to change.
    /// </summary>
    /// <param name="mouseDownPosition">
    /// The mouse down position. 
    /// </param>
    private void CheckWhichRangeEndToChange(Point mouseDownPosition)
    {
      if (this.IsMouseOver && !this.timelineThumb.IsMouseOver)
      {
        if (mouseDownPosition.X > -timelineMouseCaptureMargin && mouseDownPosition.X < timelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.Start;
        }
        else if (mouseDownPosition.X > this.timelineSelectionRange.ActualWidth - timelineMouseCaptureMargin
                 && mouseDownPosition.X < this.timelineSelectionRange.ActualWidth + timelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.End;
        }
      }
    }

    /// <summary>
    /// The convert to time string.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    private string ConvertToTimeString(double value)
    {
      double timeInMS = value;
      var seconds = (int)Math.Floor(timeInMS / 1000);
      var milliseconds = (int)(timeInMS - seconds * 1000);
      string timeValue = seconds.ToString("N0") + ":" + milliseconds.ToString("000");

      return timeValue;
    }

    /// <summary>
    /// The on frame time changed.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    private void OnFrameTimeChanged(DependencyPropertyChangedEventArgs args)
    {
      this.UpdateTickStyle();
    }

    /// <summary>
    /// The tick down clicked command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TickDownClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickDownClicked(this, e);
    }

    /// <summary>
    /// The tick up clicked command_ executed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void TickUpClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickUpClicked(this, e);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The update selection range.
    /// </summary>
    /// <param name="selectionPosition">
    /// The selection position. 
    /// </param>
    private void UpdateSelectionRange(Point selectionPosition)
    {
      if (this.IsMouseOver && !this.timelineThumb.IsMouseOver)
      {
        double factor = this.Maximum / this.timelineSelectionCanvas.ActualWidth;
        switch (this.currentRangeSelectionThumb)
        {
          case RangeSelectionThumb.None:
            break;
          case RangeSelectionThumb.Start:
            this.SelectionStart = selectionPosition.X * factor;
            this.SelectionStartString = this.ConvertToTimeString(this.SelectionStart);
            if (this.Value < this.SelectionStart)
            {
              this.Value = this.SelectionStart;
              if (this.SelectionAndValueChanged != null)
              {
                this.SelectionAndValueChanged(this, EventArgs.Empty);
              }
            }

            break;
          case RangeSelectionThumb.End:
            this.SelectionEnd = selectionPosition.X * factor;
            this.SelectionEndString = this.ConvertToTimeString(this.SelectionEnd);
            if (this.Value > this.SelectionEnd)
            {
              this.Value = this.SelectionEnd;
              if (this.SelectionAndValueChanged != null)
              {
                this.SelectionAndValueChanged(this, EventArgs.Empty);
              }
            }

            break;
          default:
            break;
        }
      }
    }

    /// <summary>
    ///   The update tick style.
    /// </summary>
    private void UpdateTickStyle()
    {
      // Ggf. TODO
      this.IsSnapToTickEnabled = false;
      this.TickFrequency = this.FrameTimeInNanoSeconds * VideoBase.NanoSecsToMilliSecs;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}