// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportData.cs" company="Freie Universität Berlin">
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
//   Enthält Hilfsfunktionen zum Erzeugen von Excel-Dateien mit SpreadsheetML.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VianaNET.Application;

namespace VianaNET.Modules.DataGrid
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Text;
  using System.Windows.Input;
  using System.Xml;

  using Microsoft.Office.Interop.Excel;

  using VianaNET.Data;
  using VianaNET.Data.Collections;
  using VianaNET.Logging;
  using VianaNET.Modules.Video.Control;

  using Application = System.Windows.Application;
  using Labels = VianaNET.Resources.Labels;

  /// <summary>
  ///   Enthält Hilfsfunktionen zum Erzeugen von Excel-Dateien mit SpreadsheetML.
  /// </summary>
  public class ExportData
  {
    #region Public Methods and Operators

    /// <summary>
    /// The to csv.
    /// </summary>
    /// <param name="dataSource">
    /// The data source. 
    /// </param>
    /// <param name="fileName">
    /// The file name. 
    /// </param>
    public static void ToCsv(DataCollection dataSource, string fileName)
    {
      WriteToFileWithSeparator(dataSource, fileName, ";");
    }

    /// <summary>
    /// The to txt.
    /// </summary>
    /// <param name="dataSource">
    /// The data source. 
    /// </param>
    /// <param name="fileName">
    /// The file name. 
    /// </param>
    public static void ToTxt(DataCollection dataSource, string fileName)
    {
      WriteToFileWithSeparator(dataSource, fileName, "\t");
    }

    /// <summary>
    /// The to xls.
    /// </summary>
    /// <param name="dataSource">
    /// The data source. 
    /// </param>
    public static void ToXls(DataCollection dataSource)
    {
      Application.Current.MainWindow.Cursor = Cursors.Wait;
      try
      {
        var xla = new Microsoft.Office.Interop.Excel.Application { Visible = true };

        xla.Workbooks.Add(XlSheetType.xlWorksheet);

        var ws = (Worksheet)xla.ActiveSheet;

        var row = 1;
        ws.Cells[row, 1] = Labels.DataGridFramenumber;
        ws.Cells[row, 2] = Labels.DataGridTimestamp;
        for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          string title = Labels.DataGridObjectPrefix + i.ToString(CultureInfo.InvariantCulture) + " ";
          ws.Cells[row, 3 + i * 20] = title + Labels.DataGridXPosition;
          ws.Cells[row, 4 + i * 20] = title + Labels.DataGridYPosition;
          ws.Cells[row, 5 + i * 20] = title + Labels.DataGridDistance;
          ws.Cells[row, 6 + i * 20] = title + Labels.DataGridXDistance;
          ws.Cells[row, 7 + i * 20] = title + Labels.DataGridYDistance;
          ws.Cells[row, 8 + i * 20] = title + Labels.DataGridLength;
          ws.Cells[row, 9 + i * 20] = title + Labels.DataGridXLength;
          ws.Cells[row, 10 + i * 20] = title + Labels.DataGridYLength;
          ws.Cells[row, 11 + i * 20] = title + Labels.DataGridVelocity;
          ws.Cells[row, 12 + i * 20] = title + Labels.DataGridXVelocity;
          ws.Cells[row, 13 + i * 20] = title + Labels.DataGridYVelocity;
          ws.Cells[row, 14 + i * 20] = title + Labels.DataGridAcceleration;
          ws.Cells[row, 15 + i * 20] = title + Labels.DataGridXAcceleration;
          ws.Cells[row, 16 + i * 20] = title + Labels.DataGridYAcceleration;
        }

        foreach (TimeSample sample in dataSource)
        {
          row++;
          ws.Cells[row, 1] = sample.Framenumber;
          ws.Cells[row, 2] = sample.Object[0].Time;
          for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
          {
            if (sample.Object[i] == null)
            {
              continue;
            }

            ws.Cells[row, 3 + i * 20] = sample.Object[i].PositionX;
            ws.Cells[row, 4 + i * 20] = sample.Object[i].PositionY;
            ws.Cells[row, 5 + i * 20] = sample.Object[i].Distance;
            ws.Cells[row, 6 + i * 20] = sample.Object[i].DistanceX;
            ws.Cells[row, 7 + i * 20] = sample.Object[i].DistanceY;
            ws.Cells[row, 8 + i * 20] = sample.Object[i].Length;
            ws.Cells[row, 9 + i * 20] = sample.Object[i].LengthX;
            ws.Cells[row, 10 + i * 20] = sample.Object[i].LengthY;
            ws.Cells[row, 11 + i * 20] = sample.Object[i].Velocity;
            ws.Cells[row, 12 + i * 20] = sample.Object[i].VelocityX;
            ws.Cells[row, 13 + i * 20] = sample.Object[i].VelocityY;
            ws.Cells[row, 14 + i * 20] = sample.Object[i].Acceleration;
            ws.Cells[row, 15 + i * 20] = sample.Object[i].AccelerationX;
            ws.Cells[row, 16 + i * 20] = sample.Object[i].AccelerationY;
          }
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
      }
      finally
      {
        Application.Current.MainWindow.Cursor = Cursors.Arrow;
      }
    }

    /// <summary>
    /// Erzeugt aus einer DataTable ein Excel-XML-Dokument mit SpreadsheetML.
    /// </summary>
    /// <param name="dataSource">
    /// Datenquelle, die in Excel exportiert werden soll 
    /// </param>
    /// <param name="fileName">
    /// Dateiname der Ausgabe-XML-Datei 
    /// </param>
    public static void ToXml(DataCollection dataSource, string fileName)
    {
      // XML-Schreiber erzeugen
      var writer = new XmlTextWriter(fileName, Encoding.UTF8);

      // Ausgabedatei für bessere Lesbarkeit formatieren (einrücken etc.)
      writer.Formatting = Formatting.Indented;

      // <?xml version="1.0"?>
      writer.WriteStartDocument();

      // <?mso-application progid="Excel.Sheet"?>
      writer.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");

      // <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet >"
      writer.WriteStartElement("Workbook", "urn:schemas-microsoft-com:office:spreadsheet");

      // Definition der Namensräume schreiben 
      writer.WriteAttributeString("xmlns", "o", null, "urn:schemas-microsoft-com:office:office");
      writer.WriteAttributeString("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
      writer.WriteAttributeString("xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet");
      writer.WriteAttributeString("xmlns", "html", null, "http://www.w3.org/TR/REC-html40");

      // <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">
      writer.WriteStartElement("DocumentProperties", "urn:schemas-microsoft-com:office:office");

      // Dokumenteingeschaften schreiben
      writer.WriteElementString("Author", Environment.UserName);
      writer.WriteElementString("LastAuthor", Environment.UserName);
      writer.WriteElementString("Created", DateTime.Now.ToString("u") + "Z");
      writer.WriteElementString("Company", "Freie Universität Berlin");
      writer.WriteElementString("Version", "1");

      // </DocumentProperties>
      writer.WriteEndElement();

      // <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">
      writer.WriteStartElement("ExcelWorkbook", "urn:schemas-microsoft-com:office:excel");

      // Arbeitsmappen-Einstellungen schreiben
      writer.WriteElementString("WindowHeight", "13170");
      writer.WriteElementString("WindowWidth", "17580");
      writer.WriteElementString("WindowTopX", "120");
      writer.WriteElementString("WindowTopY", "60");
      writer.WriteElementString("ProtectStructure", "False");
      writer.WriteElementString("ProtectWindows", "False");

      // </ExcelWorkbook>
      writer.WriteEndElement();

      // <Styles>
      writer.WriteStartElement("Styles");

      // <Style ss:ID="Default" ss:Name="Normal">
      writer.WriteStartElement("Style");
      writer.WriteAttributeString("ss", "ID", null, "Default");
      writer.WriteAttributeString("ss", "Name", null, "Normal");

      // <Alignment ss:Vertical="Bottom"/>
      writer.WriteStartElement("Alignment");
      writer.WriteAttributeString("ss", "Vertical", null, "Bottom");
      writer.WriteEndElement();

      // Verbleibende Sytle-Eigenschaften leer schreiben
      writer.WriteElementString("Borders", null);
      writer.WriteElementString("Font", null);
      writer.WriteElementString("Interior", null);
      writer.WriteElementString("NumberFormat", null);
      writer.WriteElementString("Protection", null);

      // </Style>
      writer.WriteEndElement();

      // </Styles>
      writer.WriteEndElement();

      // <Worksheet ss:Name="xxx">
      writer.WriteStartElement("Worksheet");
      writer.WriteAttributeString("ss", "Name", null, "VianaNET Data");

      // <Table x:FullColumns="1" x:FullRows="1" ss:DefaultColumnWidth="60">
      writer.WriteStartElement("Table");
      writer.WriteAttributeString("x", "FullColumns", null, "1");
      writer.WriteAttributeString("x", "FullRows", null, "1");
      writer.WriteAttributeString("ss", "DefaultColumnWidth", null, "60");

      // <Row>
      writer.WriteStartElement("Row");

      // Alle Zellen der aktuellen Zeile durchlaufen
      WriteCellValue(writer, "String", Labels.DataGridFramenumber);
      WriteCellValue(writer, "String", Labels.DataGridTimestamp);

      for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
      {
        string title = Labels.DataGridObjectPrefix + i.ToString(CultureInfo.InvariantCulture) + " ";
        WriteCellValue(writer, "String", title + Labels.DataGridXPosition);
        WriteCellValue(writer, "String", title + Labels.DataGridYPosition);
        WriteCellValue(writer, "String", title + Labels.DataGridDistance);
        WriteCellValue(writer, "String", title + Labels.DataGridXDistance);
        WriteCellValue(writer, "String", title + Labels.DataGridYDistance);
        WriteCellValue(writer, "String", title + Labels.DataGridLength);
        WriteCellValue(writer, "String", title + Labels.DataGridXLength);
        WriteCellValue(writer, "String", title + Labels.DataGridYLength);
        WriteCellValue(writer, "String", title + Labels.DataGridVelocity);
        WriteCellValue(writer, "String", title + Labels.DataGridXVelocity);
        WriteCellValue(writer, "String", title + Labels.DataGridYVelocity);
        WriteCellValue(writer, "String", title + Labels.DataGridAcceleration);
        WriteCellValue(writer, "String", title + Labels.DataGridXAcceleration);
        WriteCellValue(writer, "String", title + Labels.DataGridYAcceleration);
      }

      // </Row>
      writer.WriteEndElement();

      // Alle Zeilen der Datenquelle durchlaufen
      foreach (TimeSample row in dataSource)
      {
        // <Row>
        writer.WriteStartElement("Row");

        // Alle Zellen der aktuellen Zeile durchlaufen
        WriteCellValue(writer, "Number", row.Framenumber);
        WriteCellValue(writer, "Number", row.Object[0].Time);
        for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          WriteCellValue(writer, "Number", row.Object[i].PositionX);
          WriteCellValue(writer, "Number", row.Object[i].PositionY);
          WriteCellValue(writer, "Number", row.Object[i].Distance);
          WriteCellValue(writer, "Number", row.Object[i].DistanceX);
          WriteCellValue(writer, "Number", row.Object[i].DistanceY);
          WriteCellValue(writer, "Number", row.Object[i].Length);
          WriteCellValue(writer, "Number", row.Object[i].LengthX);
          WriteCellValue(writer, "Number", row.Object[i].LengthY);
          WriteCellValue(writer, "Number", row.Object[i].Velocity);
          WriteCellValue(writer, "Number", row.Object[i].VelocityX);
          WriteCellValue(writer, "Number", row.Object[i].VelocityY);
          WriteCellValue(writer, "Number", row.Object[i].Acceleration);
          WriteCellValue(writer, "Number", row.Object[i].AccelerationX);
          WriteCellValue(writer, "Number", row.Object[i].AccelerationY);
        }

        // </Row>
        writer.WriteEndElement();
      }

      // </Table>
      writer.WriteEndElement();

      // <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">
      writer.WriteStartElement("WorksheetOptions", "urn:schemas-microsoft-com:office:excel");

      // Seiteneinstellungen schreiben
      writer.WriteStartElement("PageSetup");
      writer.WriteStartElement("Header");
      writer.WriteAttributeString("x", "Margin", null, "0.4921259845");
      writer.WriteEndElement();
      writer.WriteStartElement("Footer");
      writer.WriteAttributeString("x", "Margin", null, "0.4921259845");
      writer.WriteEndElement();
      writer.WriteStartElement("PageMargins");
      writer.WriteAttributeString("x", "Bottom", null, "0.984251969");
      writer.WriteAttributeString("x", "Left", null, "0.78740157499999996");
      writer.WriteAttributeString("x", "Right", null, "0.78740157499999996");
      writer.WriteAttributeString("x", "Top", null, "0.984251969");
      writer.WriteEndElement();
      writer.WriteEndElement();

      // <Selected/>
      writer.WriteElementString("Selected", null);

      // <Panes>
      writer.WriteStartElement("Panes");

      // <Pane>
      writer.WriteStartElement("Pane");

      // Bereichseigenschaften schreiben
      writer.WriteElementString("Number", "1");
      writer.WriteElementString("ActiveRow", "1");
      writer.WriteElementString("ActiveCol", "1");

      // </Pane>
      writer.WriteEndElement();

      // </Panes>
      writer.WriteEndElement();

      // <ProtectObjects>False</ProtectObjects>
      writer.WriteElementString("ProtectObjects", "False");

      // <ProtectScenarios>False</ProtectScenarios>
      writer.WriteElementString("ProtectScenarios", "False");

      // </WorksheetOptions>
      writer.WriteEndElement();

      // </Worksheet>
      writer.WriteEndElement();

      // </Workbook>
      writer.WriteEndElement();

      // Datei auf Festplatte schreiben
      writer.Flush();
      writer.Close();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The write cell value.
    /// </summary>
    /// <param name="writer">
    /// The writer. 
    /// </param>
    /// <param name="cellType">
    /// The cell type. 
    /// </param>
    /// <param name="cellValue">
    /// The cell value. 
    /// </param>
    private static void WriteCellValue(XmlTextWriter writer, string cellType, object cellValue)
    {
      // <Cell>
      writer.WriteStartElement("Cell");

      // <Data ss:Type="String">xxx</Data>
      writer.WriteStartElement("Data");
      writer.WriteAttributeString("ss", "Type", null, cellType);

      // Zelleninhalt schreiben
      writer.WriteValue(cellValue ?? string.Empty);

      // </Data>
      writer.WriteEndElement();

      // </Cell>
      writer.WriteEndElement();
    }

    /// <summary>
    /// The write to file with separator.
    /// </summary>
    /// <param name="dataSource">
    /// The data source. 
    /// </param>
    /// <param name="fileName">
    /// The file name. 
    /// </param>
    /// <param name="separator">
    /// The separator. 
    /// </param>
    private static void WriteToFileWithSeparator(DataCollection dataSource, string fileName, string separator)
    {
      using (var sw = new StreamWriter(fileName))
      {
        sw.Write(Labels.DataGridFramenumber);
        sw.Write(separator);
        sw.Write(Labels.DataGridTimestamp);
        sw.Write(separator);
        for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
        {
          string title = Labels.DataGridObjectPrefix + i.ToString(CultureInfo.InvariantCulture) + " ";
          sw.Write(title + Labels.DataGridXPosition);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridYPosition);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridDistance);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridXDistance);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridYDistance);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridLength);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridXLength);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridYLength);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridVelocity);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridXVelocity);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridYVelocity);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridAcceleration);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridXAcceleration);
          sw.Write(separator);
          sw.Write(title + Labels.DataGridYAcceleration);
          sw.WriteLine();
        }

        foreach (TimeSample sample in dataSource)
        {
          sw.Write(sample.Framenumber);
          sw.Write(separator);
          sw.Write(sample.Object[0].Time);
          sw.Write(separator);
          for (int i = 0; i < Viana.Project.ProcessingData.NumberOfTrackedObjects; i++)
          {
            sw.Write(sample.Object[i].PositionX);
            sw.Write(separator);
            sw.Write(sample.Object[i].PositionY);
            sw.Write(separator);
            sw.Write(sample.Object[i].Distance);
            sw.Write(separator);
            sw.Write(sample.Object[i].DistanceX);
            sw.Write(separator);
            sw.Write(sample.Object[i].DistanceY);
            sw.Write(separator);
            sw.Write(sample.Object[i].Length);
            sw.Write(separator);
            sw.Write(sample.Object[i].LengthX);
            sw.Write(separator);
            sw.Write(sample.Object[i].LengthY);
            sw.Write(separator);
            sw.Write(sample.Object[i].Velocity);
            sw.Write(separator);
            sw.Write(sample.Object[i].VelocityX);
            sw.Write(separator);
            sw.Write(sample.Object[i].VelocityY);
            sw.Write(separator);
            sw.Write(sample.Object[i].Acceleration);
            sw.Write(separator);
            sw.Write(sample.Object[i].AccelerationX);
            sw.Write(separator);
            sw.Write(sample.Object[i].AccelerationY);
            sw.Write(separator);
          }
        }
      }
    }

    #endregion
  }
}