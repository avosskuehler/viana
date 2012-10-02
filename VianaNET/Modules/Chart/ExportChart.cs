// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportChart.cs" company="Freie Universität Berlin">
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
//   The export chart.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Chart
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  using Microsoft.Office.Interop.Word;
  using Microsoft.Win32;

  using VianaNET.Localization;

  using Application = Microsoft.Office.Interop.Word.Application;
  using Chart = Visifire.Charts.Chart;

  /// <summary>
  ///   The export chart.
  /// </summary>
  public class ExportChart
  {
    #region Public Methods and Operators

    /// <summary>
    /// The to clipboard.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToClipboard(Chart chart)
    {
      int margin = 20;
      var width = (int)((chart.ActualWidth + margin) * 300 / 96);
      var height = (int)((chart.ActualHeight + margin) * 300 / 96);
      var rtb = new RenderTargetBitmap(
        width, 
        height, 
        300, 
        // dpi x 
        300, 
        // dpi y 
        PixelFormats.Pbgra32 // pixelformat 
        );

      var drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

      drawingContext.Close();

      // Render drawing to bitmap
      rtb.Render(drawingVisual);

      rtb.Render(chart);

      Clipboard.SetImage(rtb);
    }

    /// <summary>
    /// The to file.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToFile(Chart chart)
    {
      var sfd = new SaveFileDialog();
      sfd.CheckFileExists = false;
      sfd.CheckPathExists = true;
      sfd.DefaultExt = ".png";
      sfd.AddExtension = true;
      sfd.Filter = Labels.GraphicFilesFilter;
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      sfd.Title = Labels.GraphicFilesSaveFileDialogTitle;
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        int margin = 20;
        var width = (int)((chart.ActualWidth + margin) * 300 / 96);
        var height = (int)((chart.ActualHeight + margin) * 300 / 96);
        var rtb = new RenderTargetBitmap(
          width, 
          height, 
          300, 
          // dpi x 
          300, 
          // dpi y 
          PixelFormats.Pbgra32 // pixelformat 
          );

        using (var stream = new FileStream(sfd.FileName, FileMode.Create))
        {
          BitmapEncoder encoder;
          bool transparentBackground = false;
          string extension = Path.GetExtension(sfd.FileName);
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
            var drawingVisual = new DrawingVisual();
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

    /// <summary>
    /// The to word.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToWord(Chart chart)
    {
      ToClipboard(chart);

      // Insert in word
      var word = new Application();
      word.Visible = true;

      object template = Missing.Value;
      object newTemplate = Missing.Value;
      object type = WdNewDocumentType.wdNewBlankDocument;
      object visible = true;
      word.Documents.Add(ref template, ref newTemplate, ref type, ref visible);
      word.Selection.Paste();
    }

    #endregion
  }
}