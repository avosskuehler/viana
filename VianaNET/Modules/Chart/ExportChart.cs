using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using Microsoft.Win32;
using System;
using System.IO;
using Visifire.Charts;

namespace VianaNET
{
  public class ExportChart
  {

    public static void ToClipboard(Chart chart)
    {
      int margin = 20;
      int width = (int)((chart.ActualWidth + margin) * 300 / 96);
      int height = (int)((chart.ActualHeight + margin) * 300 / 96);
      var rtb = new RenderTargetBitmap(
        width,
        height,
        300, //dpi x 
        300, //dpi y 
        PixelFormats.Pbgra32 // pixelformat 
        );

      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

      drawingContext.Close();

      // Render drawing to bitmap
      rtb.Render(drawingVisual);

      rtb.Render(chart);

      Clipboard.SetImage(rtb);
    }

    public static void ToWord(Chart chart)
    {
      ToClipboard(chart);

      // Insert in word
      Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
      word.Visible = true;

      object template = System.Reflection.Missing.Value;
      object newTemplate = System.Reflection.Missing.Value;
      object type = Microsoft.Office.Interop.Word.WdNewDocumentType.wdNewBlankDocument;
      object visible = true;
      word.Documents.Add(ref template, ref newTemplate, ref type, ref visible);
      word.Selection.Paste();
    }

    public static void ToFile(Chart chart)
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.CheckFileExists = false;
      sfd.CheckPathExists = true;
      sfd.DefaultExt = ".png";
      sfd.AddExtension = true;
      sfd.Filter = Localization.Labels.GraphicFilesFilter;
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      sfd.Title = Localization.Labels.GraphicFilesSaveFileDialogTitle;
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        int margin = 20;
        int width = (int)((chart.ActualWidth + margin) * 300 / 96);
        int height = (int)((chart.ActualHeight + margin) * 300 / 96);
        var rtb = new RenderTargetBitmap(
          width,
          height,
          300, //dpi x 
          300, //dpi y 
          PixelFormats.Pbgra32 // pixelformat 
          );

        using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
        {
          BitmapEncoder encoder;
          bool transparentBackground = false;
          string extension = System.IO.Path.GetExtension(sfd.FileName);
          switch (extension.ToLower())
          {
            case ".jpg":
            default:
              encoder = new JpegBitmapEncoder();
              break;
            case ".bmp":
              encoder = new BmpBitmapEncoder();
              break;
            case ".png":
              encoder = new PngBitmapEncoder();
              transparentBackground = true;
              break;
            case ".gif":
              encoder = new GifBitmapEncoder();
              transparentBackground = true;
              break;
            case ".tif":
              encoder = new TiffBitmapEncoder();
              break;
            case ".wmp":
              encoder = new WmpBitmapEncoder();
              transparentBackground = true;
              break;
          }

          if (!transparentBackground)
          {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

            drawingContext.Close();

            // Render background 
            rtb.Render(drawingVisual);
          }

          // Render chart
          rtb.Render(chart);

          // Save to file
          encoder.Frames.Add(BitmapFrame.Create(rtb));
          encoder.Save(stream);
        }
      }
    }
  }
}
