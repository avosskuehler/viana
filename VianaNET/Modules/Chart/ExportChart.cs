// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportChart.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
  using System.Text;
  using System.Windows;
  using System.Windows.Media.Imaging;

  using Microsoft.Office.Interop.Word;
  using Microsoft.Win32;

  using OxyPlot;
  using OxyPlot.Wpf;

  using VianaNET.MainWindow;


  using wordapp = Microsoft.Office.Interop.Word;

  /// <summary>
  ///   The export chart.
  /// </summary>
  public class ExportChart
  {


    /// <summary>
    /// The to clipboard.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToClipboard(PlotModel chart)
    {
      PngExporter exporter = new PngExporter();
      exporter.Width = (int)chart.Width;
      exporter.Height = (int)chart.Height;
      chart.Background = OxyColor.FromArgb(255, 255, 255, 255);
      BitmapSource bitmap = exporter.ExportToBitmap(chart);

      Clipboard.SetImage(bitmap);
      StatusBarContent.Instance.MessagesLabel = VianaNET.Localization.Labels.ChartExportedToClipboardMessage;
    }

    /// <summary>
    /// The to file.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToFile(PlotModel chart)
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.CheckFileExists = false;
      sfd.CheckPathExists = true;
      sfd.DefaultExt = ".png";
      sfd.AddExtension = true;
      sfd.Filter = VianaNET.Localization.Labels.GraphicFilesFilter;
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      sfd.Title = VianaNET.Localization.Labels.GraphicFilesSaveFileDialogTitle;
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
        {
          BitmapEncoder encoder = null;
          string extension = Path.GetExtension(sfd.FileName);
          ASCIIEncoding enc = new ASCIIEncoding();
          if (extension != null)
          {
            switch (extension.ToLower())
            {
              case ".bmp":
                encoder = new BmpBitmapEncoder();
                break;
              case ".png":
                encoder = new PngBitmapEncoder();
                break;
              case ".gif":
                encoder = new GifBitmapEncoder();
                break;
              case ".tif":
                encoder = new TiffBitmapEncoder();
                break;
              case ".wmp":
                encoder = new WmpBitmapEncoder();
                break;
              case ".svg":
                string svg = OxyPlot.SvgExporter.ExportToString(chart, chart.Width, chart.Height, true);
                stream.Write(enc.GetBytes(svg), 0, svg.Length);
                return;
              case ".xaml":
                string xaml = XamlExporter.ExportToString(chart, chart.Width, chart.Height);
                stream.Write(enc.GetBytes(xaml), 0, xaml.Length);
                return;
              case ".jpg":
              default:
                encoder = new JpegBitmapEncoder();
                break;
            }
          }

          PngExporter exporter = new PngExporter();
          exporter.Width = (int)chart.Width;
          exporter.Height = (int)chart.Height;
          chart.Background = OxyColor.FromArgb(255, 255, 255, 255);

          BitmapSource bitmap = exporter.ExportToBitmap(chart);

          // Save to file
          if (encoder != null)
          {
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
          }
        }
      }
    }

    /// <summary>
    /// The to word.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToWord(PlotModel chart)
    {
      ToClipboard(chart);

      try
      {
        // Insert in word
        var word = new wordapp.Application();
        word.Visible = true;

        object template = Missing.Value;
        object newTemplate = Missing.Value;
        object type = WdNewDocumentType.wdNewBlankDocument;
        object visible = true;
        word.Documents.Add(ref template, ref newTemplate, ref type, ref visible);
        word.Selection.Paste();
      }
      catch (Exception ex)
      {
        Logging.ErrorLogger.ProcessException(ex, true);
      }
    }


  }
}