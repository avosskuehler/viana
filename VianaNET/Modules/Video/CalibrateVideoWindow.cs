using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using WPFMediaKit.DirectShow.Controls;
using System.Windows.Shapes;

namespace VianaNET
{
  public class CalibrateVideoWindow : WindowWithHelp
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    private bool originIsSet;
    private bool startPointIsSet;
    private Point startPoint;
    private Point endPoint;
    private Path originPath;
    private Line ruler;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION
    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES
    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public CalibrateVideoWindow()
    {
      this.Title = Localization.Labels.CalibrateVideoWindowTitle;
      this.LabelTitle.Content = Localization.Labels.CalibrateWindowHelpControlTitle;
      this.DescriptionTitle.Content = Localization.Labels.CalibrateWindowSpecifyOriginHeader;
      this.DescriptionMessage.Text = Localization.Labels.CalibrateWindowSpecifyOriginDescription;
      this.originPath = (Path)this.Resources["OriginPath"];
      this.windowCanvas.Children.Add(this.originPath);
      this.ruler = (Line)this.Resources["Ruler"];
      this.windowCanvas.Children.Add(this.ruler);
      this.originIsSet = false;
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES

    protected override void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      base.Container_MouseLeftButtonDown(sender, e);

      if (this.originIsSet)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
        double originalX = factorX * scaledX;
        double originalY = factorY * scaledY;

        if (!this.startPointIsSet)
        {
          this.startPoint = new Point(originalX, originalY);
          this.ruler.X1 = scaledX;
          this.ruler.Y1 = scaledY;
          this.ruler.X2 = scaledX;
          this.ruler.Y2 = scaledY;
          this.ruler.Visibility = Visibility.Visible;
          this.startPointIsSet = true;
        }
      }
    }

    protected override void Container_MouseMove(object sender, MouseEventArgs e)
    {
      base.Container_MouseMove(sender, e);

      if (this.originIsSet && this.startPointIsSet)
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        this.ruler.X2 = scaledX;
        this.ruler.Y2 = scaledY;
      }
    }

    protected override void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      base.Container_MouseLeftButtonUp(sender, e);

      double scaledX = e.GetPosition(this.VideoImage).X;
      double scaledY = e.GetPosition(this.VideoImage).Y;
      double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
      double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
      double originalX = factorX * scaledX;
      double originalY = factorY * scaledY;

      if (!this.originIsSet)
      {
        Calibration.Instance.OriginInPixel = new Point(originalX, originalY);
        this.originIsSet = true;
        this.originPath.Visibility = Visibility.Visible;
        Canvas.SetLeft(this.originPath, scaledX - this.originPath.ActualWidth / 2);
        Canvas.SetTop(this.originPath, scaledY - this.originPath.ActualHeight / 2);

        this.DescriptionTitle.Content = Localization.Labels.CalibrateWindowSpecifyLengthHeader;
        this.DescriptionMessage.Text = Localization.Labels.CalibrateWindowSpecifyLengthDescription;
      }
      else if (this.startPointIsSet)
      {
        ProcessSecondPoint(e);
      }
    }

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER
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

    private void ProcessSecondPoint(MouseButtonEventArgs e)
    {
      double scaledX = e.GetPosition(this.VideoImage).X;
      double scaledY = e.GetPosition(this.VideoImage).Y;
      double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
      double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
      double originalX = factorX * scaledX;
      double originalY = factorY * scaledY;
      this.endPoint = new Point(originalX, originalY);

      LengthDialog lengthDialog = new LengthDialog();
      if (lengthDialog.ShowDialog().Value)
      {
        // Save ruler points to Settings
        Calibration.Instance.RulerEndPointInPixel = this.endPoint;
        Calibration.Instance.RulerStartPointInPixel = this.startPoint;

        Vector lengthVector = new Vector();
        lengthVector = Vector.Add(lengthVector, new Vector(Calibration.Instance.RulerStartPointInPixel.X, Calibration.Instance.RulerStartPointInPixel.Y));
        lengthVector.Negate();
        lengthVector = Vector.Add(lengthVector, new Vector(Calibration.Instance.RulerEndPointInPixel.X, Calibration.Instance.RulerEndPointInPixel.Y));
        double length = lengthVector.Length;
        Calibration.Instance.ScalePixelToUnit = Calibration.Instance.RulerValueInRulerUnits / length;
        Calibration.Instance.IsVideoCalibrated = true;
        this.DialogResult = true;
      }

      this.Close();
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}
