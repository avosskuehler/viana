
namespace VianaNET
{
  using System.Windows.Controls;
  using System;
  using System.Windows.Input;
  using System.Windows;

  /// <summary>
  /// Interaction logic for MediaSlider.xaml
  /// </summary>
  public partial class MediaSlider : Slider
  {
    private void UpdateTickStyle()
    {
      this.IsSnapToTickEnabled = true;
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

    private static void OnFrameTimeChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      (obj as MediaSlider).OnFrameTimeChanged(args);
    }

    private void OnFrameTimeChanged(DependencyPropertyChangedEventArgs args)
    {
      UpdateTickStyle();
    }

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

    protected override void OnValueChanged(double oldValue, double newValue)
    {
      base.OnValueChanged(oldValue, newValue);
      this.CurrentTimeString = ConvertToTimeString(newValue);
    }

    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);
      this.MaximumString = ConvertToTimeString(newMaximum);
    }

    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);
      this.MinimumString = ConvertToTimeString(newMinimum);
    }

    public static RoutedCommand TickDownClickedCommand;
    public static RoutedCommand TickUpClickedCommand;
    //public static RoutedCommand SelectionRangeLeftMouseDownCommand;
    //public static RoutedCommand SelectionRangeMouseMoveCommand;
    //public static RoutedCommand SelectionRangeLeftMouseUpCommand;

    public event EventHandler TickDownClicked;
    public event EventHandler TickUpClicked;
    public event MouseButtonEventHandler SelectionRangeMouseLeftButtonDown;
    public event MouseEventHandler SelectionRangeMouseMove;
    public event MouseButtonEventHandler SelectionRangeMouseLeftButtonUp;

    static MediaSlider()
    {
      MediaSlider.DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaSlider),
            new FrameworkPropertyMetadata(typeof(MediaSlider)));

      MediaSlider.TickDownClickedCommand = new RoutedCommand("TickDown", typeof(MediaSlider));
      MediaSlider.TickUpClickedCommand = new RoutedCommand("TickUp", typeof(MediaSlider));
      //MediaSlider.SelectionRangeLeftMouseDownCommand = new RoutedCommand("SelectionRangeLeftMouseDown", typeof(MediaSlider));
      //MediaSlider.SelectionRangeMouseMoveCommand = new RoutedCommand("SelectionRangeMouseMove", typeof(MediaSlider));
      //MediaSlider.SelectionRangeLeftMouseUpCommand = new RoutedCommand("SelectionRangeLeftMouseUp", typeof(MediaSlider));
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

                        //      MouseLeftButtonDown="SelectionRange_MouseLeftButtonDown"
                        //MouseLeftButtonUp="SelectionRange_MouseLeftButtonUp"
                        //MouseMove="SelectionRange_MouseMove"


      //CommandBinding bindingSelectionRangeLeftMouseDown = new CommandBinding(
      //  MediaSlider.SelectionRangeLeftMouseDownCommand,
      //  new ExecutedRoutedEventHandler(this.SelectionRangeLeftMouseDownCommand_Executed));
      //this.CommandBindings.Add(bindingSelectionRangeLeftMouseDown);

      //CommandBinding bindingSelectionRangeMouseMove = new CommandBinding(
      //  MediaSlider.SelectionRangeMouseMoveCommand,
      //  new ExecutedRoutedEventHandler(this.SelectionRangeMouseMoveCommand_Executed));
      //this.CommandBindings.Add(bindingSelectionRangeMouseMove);

      //CommandBinding bindingSelectionRangeLeftMouseUp = new CommandBinding(
      //  MediaSlider.SelectionRangeLeftMouseUpCommand,
      //  new ExecutedRoutedEventHandler(this.SelectionRangeLeftMouseUpCommand_Executed));
      //this.CommandBindings.Add(bindingSelectionRangeLeftMouseUp);
    }

    private void SelectionRange_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.SelectionRangeMouseLeftButtonDown(this, e);
    }

    private void SelectionRange_MouseMove(object sender, MouseEventArgs e)
    {
      this.SelectionRangeMouseMove(this, e);
    }

    private void SelectionRange_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.SelectionRangeMouseLeftButtonUp(this, e);
    }

    //private void SelectionRangeLeftMouseDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    //{
    //  this.SelectionRangeMouseLeftButtonDown(this, null);
    //}

    //private void SelectionRangeMouseMoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    //{
    //  this.SelectionRangeMouseMove(this, null);
    //}

    //private void SelectionRangeLeftMouseUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    //{
    //  this.SelectionRangeMouseLeftButtonUp(this, null);
    //}

    private void TickDownClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickDownClicked(this, e);
    }

    private void TickUpClickedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.TickUpClicked(this, e);
    }


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

  }
}
