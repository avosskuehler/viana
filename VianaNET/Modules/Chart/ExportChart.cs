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
  using System.Text;
  using System.Windows;
  using System.Windows.Media.Imaging;

  using Microsoft.Office.Interop.Word;
  using Microsoft.Win32;

  using OxyPlot;
  using OxyPlot.Wpf;

  using VianaNET.MainWindow;


  using Application = Microsoft.Office.Interop.Word.Application;

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
    public static void ToClipboard(PlotModel chart)
    {
      var bitmap = PngExporter.ExportToBitmap(chart, (int)chart.Width, (int)chart.Height, OxyColor.FromArgb(255, 255, 255, 255));
      Clipboard.SetImage(bitmap);
      StatusBarContent.Instance.MessagesLabel = VianaNET.Resources.Labels.ChartExportedToClipboardMessage;
    }

    /// <summary>
    /// The to file.
    /// </summary>
    /// <param name="chart">
    /// The chart. 
    /// </param>
    public static void ToFile(PlotModel chart)
    {
      var sfd = new SaveFileDialog();
      sfd.CheckFileExists = false;
      sfd.CheckPathExists = true;
      sfd.DefaultExt = ".png";
      sfd.AddExtension = true;
      sfd.Filter = VianaNET.Resources.Labels.GraphicFilesFilter;
      sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      sfd.Title = VianaNET.Resources.Labels.GraphicFilesSaveFileDialogTitle;
      if (sfd.ShowDialog().GetValueOrDefault())
      {
        using (var stream = new FileStream(sfd.FileName, FileMode.Create))
        {
          BitmapEncoder encoder = null;
          string extension = Path.GetExtension(sfd.FileName);
          var enc = new ASCIIEncoding();
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
                var rc = new ShapesRenderContext(null);
                var svg = OxyPlot.SvgExporter.ExportToString(chart, chart.Width, chart.Height, true, rc);
                stream.Write(enc.GetBytes(svg), 0, svg.Length);
                return;
              case ".xaml":
                var xaml = XamlExporter.ExportToString(chart, chart.Width, chart.Height, OxyColor.FromArgb(255, 255, 255, 255));
                stream.Write(enc.GetBytes(xaml), 0, xaml.Length);
                return;
              case ".jpg":
              default:
                encoder = new JpegBitmapEncoder();
                break;
            }
          }

          var bitmap = PngExporter.ExportToBitmap(chart, (int)chart.Width, (int)chart.Height, OxyColor.FromArgb(255, 255, 255, 255));

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
        var word = new Application();
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

    #endregion
  }
}