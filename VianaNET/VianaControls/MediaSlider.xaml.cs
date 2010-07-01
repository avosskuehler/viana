
namespace VianaNET
{
  using System.Windows.Controls;
  using System;
  using System.Windows.Input;
  using System.Windows;
  using System.Windows.Shapes;
  using System.Windows.Controls.Primitives;

  /// <summary>
  /// Interaction logic for MediaSlider.xaml
  /// </summary>
  public partial class MediaSlider : Slider
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS

    private const int timelineMouseCaptureMargin = 25;

    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private Rectangle timelineSelectionRange;
    private Thumb timelineThumb;
    private Canvas timelineSelectionCanvas;
    private RangeSelectionThumb currentRangeSelectionThumb;
    private RepeatButton timelineTickUpButton;
    private RepeatButton timelineTickDownButton;

    public event EventHandler SelectionEndReached;
    public event EventHandler SelectionStartReached;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    static MediaSlider()
    {
      MediaSlider.DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaSlider),
            new FrameworkPropertyMetadata(typeof(MediaSlider)));

      MediaSlider.TickDownClickedCommand = new RoutedCommand("TickDown", typeof(MediaSlider));
      MediaSlider.TickUpClickedCommand = new RoutedCommand("TickUp", typeof(MediaSlider));
    }

    public MediaSlider()
    {
      CommandBinding bindingTickDownClicked = new CommandBinding(
          MediaSlider.TickDownClickedCommand,
          new ExecutedRoutedEventHandler(this.TickDownClickedCommand_Executed));
      this.CommandBindings.Add(bindingTickDownClicked);

      CommandBinding bindingTickUpClicked = new CommandBinding(
        MediaSlider.TickUpClickedCommand,
        new ExecutedRoutedEventHandler(this.TickUpClickedCommand_Executed));
      this.CommandBindings.Add(bindingTickUpClicked);
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS

    private enum RangeSelectionThumb
    {
      None,
      Start,
      End,
    }

    public static RoutedCommand TickDownClickedCommand;
    public static RoutedCommand TickUpClickedCommand;

    public event EventHandler TickDownClicked;
    public event EventHandler TickUpClicked;

    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// Time between frames in ms units.
    /// </summary>
    public double FrameTime
    {
      get { return (double)GetValue(FrameTimeProperty); }
      set { SetValue(FrameTimeProperty, value); }
    }

    public static readonly DependencyProperty FrameTimeProperty =
      DependencyProperty.Register(
      "FrameTime",
      typeof(double),
      typeof(MediaSlider),
      new UIPropertyMetadata(
        default(double),
        new PropertyChangedCallback(OnFrameTimeChanged)));


    public static readonly DependencyProperty IsShowingTimesProperty = DependencyProperty.Register(
      "IsShowingTimes",
      typeof(Visibility),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata(Visibility.Visible,
        FrameworkPropertyMetadataOptions.AffectsMeasure));

    public Visibility IsShowingTimes
    {
      get { return (Visibility)GetValue(IsShowingTimesProperty); }
      set { SetValue(IsShowingTimesProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeStringProperty = DependencyProperty.Register(
      "CurrentTimeString",
      typeof(string),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata("0:000",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string CurrentTimeString
    {
      get { return (string)GetValue(CurrentTimeStringProperty); }
      set { SetValue(CurrentTimeStringProperty, value); }
    }

    public static readonly DependencyProperty MaximumStringProperty = DependencyProperty.Register(
      "MaximumString",
      typeof(string),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata("0:000",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string MaximumString
    {
      get { return (string)GetValue(MaximumStringProperty); }
      set { SetValue(MaximumStringProperty, value); }
    }

    public static readonly DependencyProperty MinimumStringProperty = DependencyProperty.Register(
      "MinimumString",
      typeof(string),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata("0:000",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string MinimumString
    {
      get { return (string)GetValue(MinimumStringProperty); }
      set { SetValue(MinimumStringProperty, value); }
    }

    public static readonly DependencyProperty SelectionStartStringProperty = DependencyProperty.Register(
      "SelectionStartString",
      typeof(string),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata("0:000",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string SelectionStartString
    {
      get { return (string)GetValue(SelectionStartStringProperty); }
      set { SetValue(SelectionStartStringProperty, value); }
    }

    public static readonly DependencyProperty SelectionEndStringProperty = DependencyProperty.Register(
      "SelectionEndString",
      typeof(string),
      typeof(MediaSlider),
      new FrameworkPropertyMetadata("0:000",
        FrameworkPropertyMetadataOptions.AffectsRender));

    public string SelectionEndString
    {
      get { return (string)GetValue(SelectionEndStringProperty); }
      set { SetValue(SelectionEndStringProperty, value); }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS
    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES


    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (this.Template != null)
      {
        this.timelineSelectionRange =
          this.Template.FindName("PART_SelectionRange", this) as Rectangle;
        this.timelineThumb =
          this.Template.FindName("Thumb", this) as Thumb;
        this.timelineSelectionCanvas =
          this.Template.FindName("SelectionCanvas", this) as Canvas;
        this.timelineTickDownButton =
          this.Template.FindName("TickDownButton", this) as RepeatButton;
        this.timelineTickUpButton =
         this.Template.FindName("TickUpButton", this) as RepeatButton;
      }
    }

    protected override void OnValueChanged(double oldValue, double newValue)
    {
      if (newValue > this.SelectionEnd)
      {
        if (this.SelectionEndReached != null)
        {
          this.SelectionEndReached(this, EventArgs.Empty);
        }
        this.Value = this.SelectionEnd;
        newValue = oldValue;
      }
      else if (newValue < this.SelectionStart)
      {
        if (this.SelectionStartReached != null)
        {
          this.SelectionStartReached(this, EventArgs.Empty);
        }
        this.Value = this.SelectionStart;
        newValue = oldValue;
      }

      this.CurrentTimeString = ConvertToTimeString(newValue);
      base.OnValueChanged(oldValue, newValue);
    }

    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);
      this.MaximumString = ConvertToTimeString(newMaximum);
      this.SelectionEnd = newMaximum;
      this.SelectionEndString = ConvertToTimeString(this.SelectionEnd);
    }

    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);
      this.MinimumString = ConvertToTimeString(newMinimum);
      this.SelectionStartString = ConvertToTimeString(this.SelectionStart);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);
      if (!this.timelineThumb.IsMouseOver &&
        !this.timelineTickUpButton.IsMouseOver &&
        !this.timelineTickDownButton.IsMouseOver)
      {
        CheckWhichRangeEndToChange(e.GetPosition(this.timelineSelectionRange));
        this.CaptureMouse();
      }
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);
      if (!this.timelineThumb.IsMouseOver &&
        !this.timelineTickUpButton.IsMouseOver &&
        !this.timelineTickDownButton.IsMouseOver)
      {
        UpdateSelectionRange(e.GetPosition(this.timelineSelectionCanvas));
        this.currentRangeSelectionThumb = RangeSelectionThumb.None;
        this.ReleaseMouseCapture();
      }
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      base.OnPreviewMouseMove(e);
      if (e.LeftButton == MouseButtonState.Pressed &&
        !this.timelineThumb.IsMouseOver &&
        !this.timelineTickUpButton.IsMouseOver &&
        !this.timelineTickDownButton.IsMouseOver)
      {
        UpdateSelectionRange(e.GetPosition(this.timelineSelectionCanvas));
      }
    }

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    private static void OnFrameTimeChanged(
  DependencyObject obj,
  DependencyPropertyChangedEventArgs args)
    {
      (obj as MediaSlider).OnFrameTimeChanged(args);
    }

    private void TickDownClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickDownClicked(this, e);
    }

    private void TickUpClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickUpClicked(this, e);
    }

    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS

    private void OnFrameTimeChanged(DependencyPropertyChangedEventArgs args)
    {
      UpdateTickStyle();
    }

    private void UpdateTickStyle()
    {
      this.IsSnapToTickEnabled = false;//true;
      //this.TickFrequency = 0;
      switch (Video.Instance.VideoPlayerElement.CurrentPositionFormat)
      {
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.None:
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.MediaTime:
          this.TickFrequency = this.FrameTime * 10000;
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Frame:
          this.TickFrequency = 1;
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Byte:
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Field:
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Sample:
          this.TickFrequency = 1;
          break;
      }
    }

    private void CheckWhichRangeEndToChange(Point mouseDownPosition)
    {
      if (this.IsMouseOver && !this.timelineThumb.IsMouseOver)
      {
        if (mouseDownPosition.X > -timelineMouseCaptureMargin && mouseDownPosition.X < timelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.Start;
        }
        else if (mouseDownPosition.X > this.timelineSelectionRange.ActualWidth - timelineMouseCaptureMargin &&
          mouseDownPosition.X < this.timelineSelectionRange.ActualWidth + timelineMouseCaptureMargin)
        {
          this.currentRangeSelectionThumb = RangeSelectionThumb.End;
        }
      }
    }

    private void UpdateSelectionRange(Point selectionPosition)
    {
      if (this.IsMouseOver && !this.timelineThumb.IsMouseOver)
      {
        double factor = this.Maximum / this.timelineSelectionCanvas.ActualWidth;
        switch (currentRangeSelectionThumb)
        {
          case RangeSelectionThumb.None:
            break;
          case RangeSelectionThumb.Start:
            this.SelectionStart = selectionPosition.X * factor;
            this.SelectionStartString = this.ConvertToTimeString(this.SelectionStart);
            if (this.Value < this.SelectionStart)
            {
              this.Value = this.SelectionStart;
            }

            break;
          case RangeSelectionThumb.End:
            this.SelectionEnd = selectionPosition.X * factor;
            this.SelectionEndString = this.ConvertToTimeString(this.SelectionEnd);
            if (this.Value > this.SelectionEnd)
            {
              this.Value = this.SelectionEnd;
            }

            break;
          default:
            break;
        }
      }
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    private string ConvertToTimeString(double value)
    {
      double timeInMS = 0;
      switch (Video.Instance.VideoPlayerElement.CurrentPositionFormat)
      {
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.MediaTime:
          timeInMS = value / 10000;
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Frame:
          timeInMS = value * this.FrameTime;
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Byte:
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Field:
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.Sample:
          timeInMS = value * this.FrameTime;
          break;
        case WPFMediaKit.DirectShow.MediaPlayers.MediaPositionFormat.None:
          break;
      }

      int seconds = (int)Math.Floor(timeInMS / 1000);
      int milliseconds = (int)(timeInMS - seconds * 1000);
      string timeValue = seconds.ToString("N0") +
        ":" + milliseconds.ToString("000");

      return timeValue;
    }

    #endregion //HELPER





  }
}
