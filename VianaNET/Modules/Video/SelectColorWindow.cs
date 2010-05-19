using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VianaNET
{
  public partial class SelectColorWindow : WindowWithHelp
  {
    public SelectColorWindow()
    {
      this.Cursor = Cursors.Pen;
      this.Title = Localization.Labels.SelectColorWindowTitle;
      this.LabelTitle.Content = Localization.Labels.SelectColorWindowHelpControlTitle;
      this.DescriptionTitle.Content = Localization.Labels.SelectColorWindowDescriptionTitle;
      this.DescriptionMessage.Text = Localization.Labels.SelectColorWindowDescriptionMessage;
    }

    protected override void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      base.Container_MouseLeftButtonUp(sender, e);

      if (this.ControlPanel.IsMouseOver)
      {
        return;
      }

      try
      {
        double scaledX = e.GetPosition(this.VideoImage).X;
        double scaledY = e.GetPosition(this.VideoImage).Y;
        double factorX = this.VideoImage.Source.Width / this.VideoImage.ActualWidth;
        double factorY = this.VideoImage.Source.Height / this.VideoImage.ActualHeight;
        double originalX = factorX * scaledX;
        double originalY = factorY * scaledY;
        Int32Rect rect = new Int32Rect((int)originalX, (int)originalY, 1, 1);

        System.Drawing.Bitmap frame = Video.Instance.CreateBitmapFromCurrentImageSource();
        if (frame == null)
        {
          throw new ArgumentNullException("Native Bitmap is null.");
        }

        System.Drawing.Color color = frame.GetPixel((int)originalX, (int)originalY);

        Calibration.Instance.TargetColor = Color.FromArgb(255, color.R, color.G, color.B);
        this.DialogResult = true;

      }
      catch (Exception)
      {
        VianaDialog error = new VianaDialog(
          "Error",
          "No Color selected",
          "Could not detect the color at the given position");
        error.ShowDialog();
      }

      this.Close();
    }
  }
}
