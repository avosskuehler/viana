using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace VianaNET
{
  public class ClipVideoWindow : WindowWithHelp
  {
    private const int margin = 10;

    private Line currentLine;
    private Line TopLine;
    private Line LeftLine;
    private Line BottomLine;
    private Line RightLine;
    private Path OuterRegion;

    public ClipVideoWindow()
    {
      this.Title = Localization.Labels.ClipVideoWindowTitle;
      this.LabelTitle.Content = Localization.Labels.ClipVideoHelpControlTitle;
      this.DescriptionTitle.Content = Localization.Labels.ClipVideoDescriptionTitle;
      this.DescriptionMessage.Text = Localization.Labels.ClipVideoDescriptionMessage;
      this.TopLine = (Line)this.Resources["TopLine"];
      this.TopLine.Name = "Top";
      this.TopLine.MouseEnter += new MouseEventHandler(TopBottomLine_MouseEnter);
      this.TopLine.MouseLeave += new MouseEventHandler(Line_MouseLeave);
      this.TopLine.MouseLeftButtonDown += new MouseButtonEventHandler(Line_MouseLeftButtonDown);
      this.TopLine.MouseMove += new MouseEventHandler(Line_MouseMove);
      this.TopLine.MouseLeftButtonUp += new MouseButtonEventHandler(Line_MouseLeftButtonUp);
      this.windowCanvas.Children.Insert(0, this.TopLine);

      this.LeftLine = (Line)this.Resources["LeftLine"]; ;
      this.LeftLine.Name = "Left";
      this.LeftLine.MouseEnter += new MouseEventHandler(LeftRightLine_MouseEnter);
      this.LeftLine.MouseLeave += new MouseEventHandler(Line_MouseLeave);
      this.LeftLine.MouseLeftButtonDown += new MouseButtonEventHandler(Line_MouseLeftButtonDown);
      this.LeftLine.MouseMove += new MouseEventHandler(Line_MouseMove);
      this.LeftLine.MouseLeftButtonUp += new MouseButtonEventHandler(Line_MouseLeftButtonUp);
      this.windowCanvas.Children.Insert(0, this.LeftLine);

      this.BottomLine = (Line)this.Resources["BottomLine"]; ;
      this.BottomLine.Name = "Bottom";
      this.BottomLine.MouseEnter += new MouseEventHandler(TopBottomLine_MouseEnter);
      this.BottomLine.MouseLeave += new MouseEventHandler(Line_MouseLeave);
      this.BottomLine.MouseLeftButtonDown += new MouseButtonEventHandler(Line_MouseLeftButtonDown);
      this.BottomLine.MouseMove += new MouseEventHandler(Line_MouseMove);
      this.BottomLine.MouseLeftButtonUp += new MouseButtonEventHandler(Line_MouseLeftButtonUp);
      this.windowCanvas.Children.Insert(0, this.BottomLine);

      this.RightLine = (Line)this.Resources["RightLine"]; ;
      this.RightLine.Name = "Right";
      this.RightLine.MouseEnter += new MouseEventHandler(LeftRightLine_MouseEnter);
      this.RightLine.MouseLeave += new MouseEventHandler(Line_MouseLeave);
      this.RightLine.MouseLeftButtonDown += new MouseButtonEventHandler(Line_MouseLeftButtonDown);
      this.RightLine.MouseMove += new MouseEventHandler(Line_MouseMove);
      this.RightLine.MouseLeftButtonUp += new MouseButtonEventHandler(Line_MouseLeftButtonUp);
      this.windowCanvas.Children.Insert(0,this.RightLine);

      this.OuterRegion = (Path)this.Resources["OuterRegion"]; ;
      this.windowCanvas.Children.Insert(0,this.OuterRegion);

      this.Loaded += new RoutedEventHandler(Window_Loaded);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (!Calibration.Instance.HasClipRegion)
      {
        this.TopLine.X1 = margin;
        this.TopLine.X2 = this.VideoImage.ActualWidth - margin;
        this.LeftLine.Y1 = margin;
        this.LeftLine.Y2 = this.VideoImage.ActualHeight - margin;
        this.BottomLine.X1 = margin;
        this.BottomLine.Y1 = this.VideoImage.ActualHeight - margin;
        this.BottomLine.X2 = this.VideoImage.ActualWidth - margin;
        this.BottomLine.Y2 = this.VideoImage.ActualHeight - margin;
        this.RightLine.X1 = this.VideoImage.ActualWidth - margin;
        this.RightLine.Y1 = margin;
        this.RightLine.X2 = this.VideoImage.ActualWidth - margin;
        this.RightLine.Y2 = this.VideoImage.ActualHeight - margin;
      }
      else
      {
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;

        this.TopLine.X1 = Calibration.Instance.ClipRegion.Left / factorX;
        this.TopLine.X2 = Calibration.Instance.ClipRegion.Right / factorX;
        this.TopLine.Y1 = Calibration.Instance.ClipRegion.Top / factorY;
        this.TopLine.Y2 = Calibration.Instance.ClipRegion.Top / factorY;
        this.LeftLine.X1 = Calibration.Instance.ClipRegion.Left / factorX;
        this.LeftLine.X2 = Calibration.Instance.ClipRegion.Left / factorX;
        this.LeftLine.Y1 = Calibration.Instance.ClipRegion.Top / factorY;
        this.LeftLine.Y2 = Calibration.Instance.ClipRegion.Bottom / factorY;
        this.BottomLine.X1 = Calibration.Instance.ClipRegion.Left / factorX;
        this.BottomLine.Y1 = Calibration.Instance.ClipRegion.Bottom / factorY;
        this.BottomLine.X2 = Calibration.Instance.ClipRegion.Right / factorX;
        this.BottomLine.Y2 = Calibration.Instance.ClipRegion.Bottom / factorY;
        this.RightLine.X1 = Calibration.Instance.ClipRegion.Right / factorX;
        this.RightLine.Y1 = Calibration.Instance.ClipRegion.Top / factorY;
        this.RightLine.X2 = Calibration.Instance.ClipRegion.Right / factorX;
        this.RightLine.Y2 = Calibration.Instance.ClipRegion.Bottom / factorY;
        Calibration.Instance.HasClipRegion = true;
      }

      ResetOuterRegion();
    }

    private void TopBottomLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNS;
    }

    private void LeftRightLine_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeWE;
    }

    private void Line_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
    }

    private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.currentLine = sender as Line;
      Mouse.Capture(this.currentLine);
    }

    private void Line_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && this.currentLine != null)
      {
        double newX = e.GetPosition(this.VideoImage).X;
        double newY = e.GetPosition(this.VideoImage).Y;
        if (newX >= 0 && newY >= 0 && newX <= this.VideoImage.ActualWidth && newY <= this.VideoImage.ActualHeight)
        {
          switch (this.currentLine.Name)
          {
            case "Top":
              if (newY + margin < this.BottomLine.Y1)
              {
                this.currentLine.Y1 = newY;
                this.currentLine.Y2 = newY;
                this.LeftLine.Y1 = newY;
                this.RightLine.Y1 = newY;
              }
              break;
            case "Bottom":
              if (newY > this.TopLine.Y1 + margin)
              {
                this.currentLine.Y1 = newY;
                this.currentLine.Y2 = newY;
                this.LeftLine.Y2 = newY;
                this.RightLine.Y2 = newY;
              }
              break;
            case "Left":
              if (newX + margin < this.RightLine.X1)
              {
                this.currentLine.X1 = newX;
                this.currentLine.X2 = newX;
                this.TopLine.X1 = newX;
                this.BottomLine.X1 = newX;
              }
              break;
            case "Right":
              if (newX > this.LeftLine.X1 + margin)
              {
                this.currentLine.X1 = newX;
                this.currentLine.X2 = newX;
                this.TopLine.X2 = newX;
                this.BottomLine.X2 = newX;
              }
              break;
          }

          ResetOuterRegion();
        }
      }
    }

    private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    private void ResetOuterRegion()
    {
      CombinedGeometry geometry = this.OuterRegion.Data as CombinedGeometry;
      RectangleGeometry outerRect = geometry.Geometry1 as RectangleGeometry;
      outerRect.Rect = new Rect(0, 0, this.VideoImage.ActualWidth, this.VideoImage.ActualHeight);
      RectangleGeometry innerRect = geometry.Geometry2 as RectangleGeometry;
      innerRect.Rect = new Rect(new Point(this.LeftLine.X1, this.TopLine.Y1), new Point(this.RightLine.X1, this.BottomLine.Y1));

      double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
      double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;

      Rect rect = new Rect();
      rect.Location = new Point(this.LeftLine.X1 * factorX, this.TopLine.Y1 * factorY);
      rect.Width = (this.RightLine.X1 - this.LeftLine.X1) * factorX;
      rect.Height = (this.BottomLine.Y1 - this.TopLine.Y1) * factorY;
      Calibration.Instance.ClipRegion = rect;
      Calibration.Instance.HasClipRegion = true;
    }
  }
}
