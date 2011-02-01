using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;

namespace VianaNET
{
  /// <summary>
  /// Enthält Hilfsfunktionen zum Erzeugen von Excel-Dateien mit SpreadsheetML.
  /// </summary>
  public class ExportData
  {
    /// <summary>
    /// Erzeugt aus einer DataTable ein Excel-XML-Dokument mit SpreadsheetML.
    /// </summary>        
    /// <param name="dataSource">Datenquelle, die in Excel exportiert werden soll</param>
    /// <param name="fileName">Dateiname der Ausgabe-XML-Datei</param>
    public static void ToXml(DataCollection dataSource, string fileName)
    {
      // XML-Schreiber erzeugen
      XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);

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

      // <Table ss:ExpandedColumnCount="2" ss:ExpandedRowCount="3" x:FullColumns="1" x:FullRows="1" ss:DefaultColumnWidth="60">
      writer.WriteStartElement("Table");
      int columnCount = Calibration.Instance.NumberOfTrackedObjects * 14 + 2;
      writer.WriteAttributeString("ss", "ExpandedColumnCount", null, columnCount.ToString());
      writer.WriteAttributeString("ss", "ExpandedRowCount", null, dataSource.Count.ToString());
      writer.WriteAttributeString("x", "FullColumns", null, "1");
      writer.WriteAttributeString("x", "FullRows", null, "1");
      writer.WriteAttributeString("ss", "DefaultColumnWidth", null, "60");

      // <Row>
      writer.WriteStartElement("Row");

      // Alle Zellen der aktuellen Zeile durchlaufen
      WriteCellValue(writer, "String", Localization.Labels.DataGridFramenumber);
      WriteCellValue(writer, "String", Localization.Labels.DataGridTimestamp);

      for (int i = 0; i < Calibration.Instance.NumberOfTrackedObjects; i++)
      {
        string title = Localization.Labels.DataGridObjectPrefix + i.ToString() + " ";
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridXPosition);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridYPosition);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridDistance);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridXDistance);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridYDistance);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridLength);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridXLength);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridYLength);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridVelocity);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridXVelocity);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridYVelocity);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridAcceleration);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridXAcceleration);
        WriteCellValue(writer, "String", title + Localization.Labels.DataGridYAcceleration);
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
        WriteCellValue(writer, "Number", row.Timestamp);
        for (int i = 0; i < Calibration.Instance.NumberOfTrackedObjects; i++)
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

    private static void WriteCellValue(XmlTextWriter writer, string cellType, object cellValue)
    {
      // <Cell>
      writer.WriteStartElement("Cell");

      // <Data ss:Type="String">xxx</Data>
      writer.WriteStartElement("Data");
      writer.WriteAttributeString("ss", "Type", null, cellType);

      // Zelleninhalt schreiben
      writer.WriteValue(cellValue);

      // </Data>
      writer.WriteEndElement();

      // </Cell>
      writer.WriteEndElement();
    }


    public static void ToXls(DataCollection dataSource)
    {
      Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();

      xla.Visible = true;
      Microsoft.Office.Interop.Excel.Workbook wb =
        xla.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);

      Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;

      int row = 1;
      ws.Cells[row, 1] = Localization.Labels.DataGridFramenumber;
      ws.Cells[row, 2] = Localization.Labels.DataGridTimestamp;
      for (int i = 0; i < Calibration.Instance.NumberOfTrackedObjects; i++)
      {
        string title = Localization.Labels.DataGridObjectPrefix + i.ToString() + " ";
        ws.Cells[row, 3 + i] = title + Localization.Labels.DataGridXPosition;
        ws.Cells[row, 4 + i] = title + Localization.Labels.DataGridYPosition;
        ws.Cells[row, 5 + i] = title + Localization.Labels.DataGridDistance;
        ws.Cells[row, 6 + i] = title + Localization.Labels.DataGridXDistance;
        ws.Cells[row, 7 + i] = title + Localization.Labels.DataGridYDistance;
        ws.Cells[row, 8 + i] = title + Localization.Labels.DataGridLength;
        ws.Cells[row, 9 + i] = title + Localization.Labels.DataGridXLength;
        ws.Cells[row, 10 + i] = title + Localization.Labels.DataGridYLength;
        ws.Cells[row, 11 + i] = title + Localization.Labels.DataGridVelocity;
        ws.Cells[row, 12 + i] = title + Localization.Labels.DataGridXVelocity;
        ws.Cells[row, 13 + i] = title + Localization.Labels.DataGridYVelocity;
        ws.Cells[row, 14 + i] = title + Localization.Labels.DataGridAcceleration;
        ws.Cells[row, 15 + i] = title + Localization.Labels.DataGridXAcceleration;
        ws.Cells[row, 16 + i] = title + Localization.Labels.DataGridYAcceleration;
      }

      foreach (TimeSample sample in dataSource)
      {
        row++;
        ws.Cells[row, 1] = sample.Framenumber;
        ws.Cells[row, 2] = sample.Timestamp;
        for (int i = 0; i < Calibration.Instance.NumberOfTrackedObjects; i++)
        {
          ws.Cells[row, 3] = sample.Object[i].PositionX;
          ws.Cells[row, 4] = sample.Object[i].PositionY;
          ws.Cells[row, 5] = sample.Object[i].Distance;
          ws.Cells[row, 6] = sample.Object[i].DistanceX;
          ws.Cells[row, 7] = sample.Object[i].DistanceY;
          ws.Cells[row, 8] = sample.Object[i].Length;
          ws.Cells[row, 9] = sample.Object[i].LengthX;
          ws.Cells[row, 10] = sample.Object[i].LengthY;
          ws.Cells[row, 11] = sample.Object[i].Velocity;
          ws.Cells[row, 12] = sample.Object[i].VelocityX;
          ws.Cells[row, 13] = sample.Object[i].VelocityY;
          ws.Cells[row, 14] = sample.Object[i].Acceleration;
          ws.Cells[row, 15] = sample.Object[i].AccelerationX;
          ws.Cells[row, 16] = sample.Object[i].AccelerationY;
        }
      }
    }

    public static void ToCsv(DataCollection dataSource, string fileName)
    {
      WriteToFileWithSeparator(dataSource, fileName, ";");
    }

    public static void ToTxt(DataCollection dataSource, string fileName)
    {
      WriteToFileWithSeparator(dataSource, fileName, "\t");
    }

    private static void WriteToFileWithSeparator(DataCollection dataSource, string fileName, string separator)
    {
      using (StreamWriter sw = new StreamWriter(fileName))
      {
        sw.Write(Localization.Labels.DataGridFramenumber);
        sw.Write(separator);
        sw.Write(Localization.Labels.DataGridTimestamp);
        sw.Write(separator);
        for (int i = 0; i < Calibration.Instance.NumberOfTrackedObjects; i++)
        {
          string title = Localization.Labels.DataGridObjectPrefix + i.ToString() + " ";
          sw.Write(title + Localization.Labels.DataGridXPosition);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridYPosition);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridDistance);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridXDistance);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridYDistance);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridLength);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridXLength);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridYLength);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridVelocity);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridXVelocity);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridYVelocity);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridAcceleration);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridXAcceleration);
          sw.Write(separator);
          sw.Write(title + Localization.Labels.DataGridYAcceleration);
          sw.WriteLine();
        }

        foreach (TimeSample sample in dataSource)
        {
          sw.Write(sample.Framenumber);
          sw.Write(separator);
          sw.Write(sample.Timestamp);
          sw.Write(separator);
          for (int i = 0; i < Calibration.Instance.NumberOfTrackedObjects; i++)
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
            sw.WriteLine();
          }
        }
      }
    }

  }
}