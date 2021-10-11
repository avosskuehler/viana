// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSlider.xaml.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
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


    /// <summary>
    ///   The timeline mouse capture margin.
    /// </summary>
    private const int TimelineMouseCaptureMargin = 25;





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
    ///   The frame time in milli seconds property.
    /// </summary>
    public static readonly DependencyProperty FrameTimeInMSProperty =
      DependencyProperty.Register(
        "FrameTimeInMS",
        typeof(double),
        typeof(MediaSlider),
        new UIPropertyMetadata(default(double), OnFrameTimeChanged));

    /// <summary>
    ///   The is showing times property.
    /// </summary>
    public static readonly DependencyProperty IsShowingTimesProperty = DependencyProperty.Register(
      "IsShowingTimes",
      typeof(Visibility),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///   The is showing times property.
    /// </summary>
    public static readonly DependencyProperty IsShowingTickButtonsProperty = DependencyProperty.Register(
      "IsShowingTickButtons",
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





    /// <summary>
    ///   The current range selection thumb.
    /// </summary>
    private RangeSelectionThumb currentRangeSelectionThumb;

    /// <summary>
    ///   The timeline selection canvas.
    /// </summary>
    private Canvas timelineSelectionCanvas;

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
      CommandBinding bindingTickDownClicked = new CommandBinding(TickDownClickedCommand, this.TickDownClickedCommand_Executed);
      this.CommandBindings.Add(bindingTickDownClicked);

      CommandBinding bindingTickUpClicked = new CommandBinding(TickUpClickedCommand, this.TickUpClickedCommand_Executed);
      this.CommandBindings.Add(bindingTickUpClicked);
    }





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

    /// <summary>
    ///   The tick down clicked.
    /// </summary>
    public event EventHandler TickDownClicked;

    /// <summary>
    ///   The tick up clicked.
    /// </summary>
    public event EventHandler TickUpClicked;





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





    /// <summary>
    ///   Gets or sets the current time string.
    /// </summary>
    public string CurrentTimeString
    {
      get => (string)this.GetValue(CurrentTimeStringProperty);

      set => this.SetValue(CurrentTimeStringProperty, value);
    }

    /// <summary>
    ///   Gets or sets the Time between frames in ms units.
    /// </summary>
    public double FrameTimeInMS
    {
      get => (double)this.GetValue(FrameTimeInMSProperty);

      set => this.SetValue(FrameTimeInMSProperty, value);
    }

    /// <summary>
    ///   Gets or sets the is showing times.
    /// </summary>
    public Visibility IsShowingTimes
    {
      get => (Visibility)this.GetValue(IsShowingTimesProperty);

      set => this.SetValue(IsShowingTimesProperty, value);
    }

    /// <summary>
    ///   Gets or sets the is showing tick buttons.
    /// </summary>
    public Visibility IsShowingTickButtons
    {
      get => (Visibility)this.GetValue(IsShowingTickButtonsProperty);

      set => this.SetValue(IsShowingTickButtonsProperty, value);
    }

    /// <summary>
    ///   Gets or sets the maximum string.
    /// </summary>
    public string MaximumString
    {
      get => (string)this.GetValue(MaximumStringProperty);

      set => this.SetValue(MaximumStringProperty, value);
    }

    /// <summary>
    ///   Gets or sets the minimum string.
    /// </summary>
    public string MinimumString
    {
      get => (string)this.GetValue(MinimumStringProperty);

      set => this.SetValue(MinimumStringProperty, value);
    }

    /// <summary>
    ///   Gets or sets the selection end string.
    /// </summary>
    public string SelectionEndString
    {
      get => (string)this.GetValue(SelectionEndStringProperty);

      set => this.SetValue(SelectionEndStringProperty, value);
    }

    /// <summary>
    ///   Gets or sets the selection start string.
    /// </summary>
    public string SelectionStartString
    {
      get => (string)this.GetValue(SelectionStartStringProperty);

      set => this.SetValue(SelectionStartStringProperty, value);
    }





    /// <summary>
    ///   Builds the visual tree for the <see cref="T:System.Windows.Controls.Slider" /> control.
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

    /// <summary>
    ///   Updates the selection times.
    /// </summary>
    public void UpdateSelectionTimes()
    {
      this.SelectionStartString = this.ConvertToTimeString(this.SelectionStart);
      this.SelectionEndString = this.ConvertToTimeString(this.SelectionEnd);
    }





    /// <summary>
    /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/>
    ///   property.
    /// </summary>
    /// <param name="oldMaximum">
    /// The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/>
    ///   property.
    /// </param>
    /// <param name="newMaximum">
    /// The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/>
    ///   property.
    /// </param>
    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);
      this.MaximumString = this.ConvertToTimeString(newMaximum);
      this.SelectionEnd = newMaximum;
      this.SelectionEndString = this.ConvertToTimeString(this.SelectionEnd);
    }

    /// <summary>
    /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/>
    ///   property.
    /// </summary>
    /// <param name="oldMinimum">
    /// The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/>
    ///   property.
    /// </param>
    /// <param name="newMinimum">
    /// The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/>
    ///   property.
    /// </param>
    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);
      this.MinimumString = this.ConvertToTimeString(newMinimum);
      this.SelectionStartString = this.ConvertToTimeString(this.SelectionStart);
    }

    /// <summary>
    /// Provides class handling for the <see cref="E:System.Windows.ContentElement.PreviewMouseLeftButtonDown"/> routed
    ///   event.
    /// </summary>
    /// <param name="e">
    /// The event data.
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
    /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonUp"/> routed
    ///   event reaches an element in its route that is derived from this class. Implement this method to add class handling
    ///   for this event.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event
    ///   data reports that the left mouse button was released.
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
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove"/> attached event reaches an
    ///   element
    ///   in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.
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
    /// Updates the current position of the <see cref="T:System.Windows.Controls.Slider"/> when the
    ///   <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value"/> property changes.
    /// </summary>
    /// <param name="oldValue">
    /// The old <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value"/> of the
    ///   <see cref="T:System.Windows.Controls.Slider"/>.
    /// </param>
    /// <param name="newValue">
    /// The new <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value"/> of the
    ///   <see cref="T:System.Windows.Controls.Slider"/>.
    /// </param>
    protected override void OnValueChanged(double oldValue, double newValue)
    {
      if (newValue > this.SelectionEnd)
      {
        this.SelectionEndReached?.Invoke(this, EventArgs.Empty);

        this.Value = this.SelectionEnd;

        // newValue = oldValue;
        // this.Value = this.SelectionEnd;
        newValue = this.SelectionEnd;
      }
      else if (newValue < this.SelectionStart)
      {
        this.SelectionStartReached?.Invoke(this, EventArgs.Empty);

        this.Value = this.SelectionStart;
        newValue = this.SelectionStart;
      }

      this.CurrentTimeString = this.ConvertToTimeString(newValue);
      base.OnValueChanged(oldValue, newValue);
    }

    /// <summary>
    /// Called when frame time changed.
    /// </summary>
    /// <param name="obj">
    /// The object.
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
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
        if (mouseDownPosition.X > -TimelineMouseCaptureMargin && mouseDownPosition.X < TimelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.Start;
        }
        else if (mouseDownPosition.X > this.timelineSelectionRange.ActualWidth - TimelineMouseCaptureMargin
                 && mouseDownPosition.X < this.timelineSelectionRange.ActualWidth + TimelineMouseCaptureMargin)
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
      //int timeInMS = value;
      TimeSpan time = new TimeSpan(0, 0, 0, 0, (int)value);
      //var seconds = (int)Math.Floor(timeInMS / 1000);
      //var milliseconds = (int)(timeInMS - seconds * 1000);
      string timeValue = time.ToString(@"mm\:ss\.fff");//seconds.ToString("N0") + ":" + milliseconds.ToString("000");

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
    /// Handles the Executed event of the TickDownClickedCommand control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void TickDownClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickDownClicked(this, e);
    }

    /// <summary>
    /// Handles the Executed event of the TickUpClickedCommand control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void TickUpClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickUpClicked(this, e);
    }

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
              this.SelectionAndValueChanged?.Invoke(this, EventArgs.Empty);
            }

            break;
          case RangeSelectionThumb.End:
            this.SelectionEnd = selectionPosition.X * factor;
            this.SelectionEndString = this.ConvertToTimeString(this.SelectionEnd);
            if (this.Value > this.SelectionEnd)
            {
              this.Value = this.SelectionEnd;
              this.SelectionAndValueChanged?.Invoke(this, EventArgs.Empty);
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
      // this.IsSnapToTickEnabled = false;
      this.TickFrequency = this.FrameTimeInMS;
    }
  }
}