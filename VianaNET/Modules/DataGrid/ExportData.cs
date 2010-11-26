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
      writer.WriteAttributeString("ss", "ExpandedColumnCount", null, "13");
      writer.WriteAttributeString("ss", "ExpandedRowCount", null, dataSource.Count.ToString());
      writer.WriteAttributeString("x", "FullColumns", null, "1");
      writer.WriteAttributeString("x", "FullRows", null, "1");
      writer.WriteAttributeString("ss", "DefaultColumnWidth", null, "60");

      // <Row>
      writer.WriteStartElement("Row");

      // Alle Zellen der aktuellen Zeile durchlaufen
      WriteCellValue(writer, "String", Localization.Labels.DataGridFramenumber);
      WriteCellValue(writer, "String", Localization.Labels.DataGridTimestamp);
      WriteCellValue(writer, "String", Localization.Labels.DataGridXCoordinate);
      WriteCellValue(writer, "String", Localization.Labels.DataGridYCoordinate);
      WriteCellValue(writer, "String", Localization.Labels.DataGridDistance);
      WriteCellValue(writer, "String", Localization.Labels.DataGridXDistance);
      WriteCellValue(writer, "String", Localization.Labels.DataGridYDistance);
      WriteCellValue(writer, "String", Localization.Labels.DataGridVelocity);
      WriteCellValue(writer, "String", Localization.Labels.DataGridXVelocity);
      WriteCellValue(writer, "String", Localization.Labels.DataGridYVelocity);
      WriteCellValue(writer, "String", Localization.Labels.DataGridAcceleration);
      WriteCellValue(writer, "String", Localization.Labels.DataGridXAcceleration);
      WriteCellValue(writer, "String", Localization.Labels.DataGridYAcceleration);

      // </Row>
      writer.WriteEndElement();

      // Alle Zeilen der Datenquelle durchlaufen
      foreach (DataSample row in dataSource)
      {
        // <Row>
        writer.WriteStartElement("Row");

        // Alle Zellen der aktuellen Zeile durchlaufen
        WriteCellValue(writer, "Number", row.Framenumber);
        WriteCellValue(writer, "Number", row.Timestamp);
        WriteCellValue(writer, "Number", row.CoordinateX);
        WriteCellValue(writer, "Number", row.CoordinateY);
        WriteCellValue(writer, "Number", row.Distance);
        WriteCellValue(writer, "Number", row.DistanceX);
        WriteCellValue(writer, "Number", row.DistanceY);
        WriteCellValue(writer, "Number", row.Velocity);
        WriteCellValue(writer, "Number", row.VelocityX);
        WriteCellValue(writer, "Number", row.VelocityY);
        WriteCellValue(writer, "Number", row.Acceleration);
        WriteCellValue(writer, "Number", row.AccelerationX);
        WriteCellValue(writer, "Number", row.AccelerationY);

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
      ws.Cells[row, 3] = Localization.Labels.DataGridXCoordinate;
      ws.Cells[row, 4] = Localization.Labels.DataGridYCoordinate;
      ws.Cells[row, 5] = Localization.Labels.DataGridDistance;
      ws.Cells[row, 6] = Localization.Labels.DataGridXDistance;
      ws.Cells[row, 7] = Localization.Labels.DataGridYDistance;
      ws.Cells[row, 8] = Localization.Labels.DataGridVelocity;
      ws.Cells[row, 9] = Localization.Labels.DataGridXVelocity;
      ws.Cells[row, 10] = Localization.Labels.DataGridYVelocity;
      ws.Cells[row, 11] = Localization.Labels.DataGridAcceleration;
      ws.Cells[row, 12] = Localization.Labels.DataGridXAcceleration;
      ws.Cells[row, 13] = Localization.Labels.DataGridYAcceleration;

      foreach (DataSample sample in dataSource)
      {
        row++;
        ws.Cells[row, 1] = sample.Framenumber;
        ws.Cells[row, 2] = sample.Timestamp;
        ws.Cells[row, 3] = sample.CoordinateX;
        ws.Cells[row, 4] = sample.CoordinateY;
        ws.Cells[row, 5] = sample.Distance;
        ws.Cells[row, 6] = sample.DistanceX;
        ws.Cells[row, 7] = sample.DistanceY;
        ws.Cells[row, 8] = sample.Velocity;
        ws.Cells[row, 9] = sample.VelocityX;
        ws.Cells[row, 10] = sample.VelocityY;
        ws.Cells[row, 11] = sample.Acceleration;
        ws.Cells[row, 12] = sample.AccelerationX;
        ws.Cells[row, 13] = sample.AccelerationY;
      }
    }

    public static void ToCsv(DataCollection dataSource, string fileName)
    {
      using (StreamWriter sw = new StreamWriter(fileName))
      {
        sw.Write(Localization.Labels.DataGridFramenumber);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridTimestamp);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridXCoordinate);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridYCoordinate);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridDistance);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridXDistance);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridYDistance);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridVelocity);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridXVelocity);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridYVelocity);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridAcceleration);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridXAcceleration);
        sw.Write(";");
        sw.Write(Localization.Labels.DataGridYAcceleration);
        sw.WriteLine();

        foreach (DataSample sample in dataSource)
        {
          sw.Write(sample.Framenumber);
          sw.Write(";");
          sw.Write(sample.Timestamp);
          sw.Write(";");
          sw.Write(sample.CoordinateX);
          sw.Write(";");
          sw.Write(sample.CoordinateY);
          sw.Write(";");
          sw.Write(sample.Distance);
          sw.Write(";");
          sw.Write(sample.DistanceX);
          sw.Write(";");
          sw.Write(sample.DistanceY);
          sw.Write(";");
          sw.Write(sample.Velocity);
          sw.Write(";");
          sw.Write(sample.VelocityX);
          sw.Write(";");
          sw.Write(sample.VelocityY);
          sw.Write(";");
          sw.Write(sample.Acceleration);
          sw.Write(";");
          sw.Write(sample.AccelerationX);
          sw.Write(";");
          sw.Write(sample.AccelerationY);
          sw.WriteLine();
        }
      }
    }

    public static void ToTxt(DataCollection dataSource, string fileName)
    {
      using (StreamWriter sw = new StreamWriter(fileName))
      {
        sw.Write(Localization.Labels.DataGridFramenumber);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridTimestamp);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridXCoordinate);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridYCoordinate);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridDistance);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridXDistance);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridYDistance);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridVelocity);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridXVelocity);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridYVelocity);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridAcceleration);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridXAcceleration);
        sw.Write("\t");
        sw.Write(Localization.Labels.DataGridYAcceleration);
        sw.WriteLine();

        foreach (DataSample sample in dataSource)
        {
          sw.Write(sample.Framenumber);
          sw.Write("\t");
          sw.Write(sample.Timestamp);
          sw.Write("\t");
          sw.Write(sample.CoordinateX);
          sw.Write("\t");
          sw.Write(sample.CoordinateY);
          sw.Write("\t");
          sw.Write(sample.Distance);
          sw.Write("\t");
          sw.Write(sample.DistanceX);
          sw.Write("\t");
          sw.Write(sample.DistanceY);
          sw.Write("\t");
          sw.Write(sample.Velocity);
          sw.Write("\t");
          sw.Write(sample.VelocityX);
          sw.Write("\t");
          sw.Write(sample.VelocityY);
          sw.Write("\t");
          sw.Write(sample.Acceleration);
          sw.Write("\t");
          sw.Write(sample.AccelerationX);
          sw.Write("\t");
          sw.Write(sample.AccelerationY);
          sw.WriteLine();
        }
      }
    }
  }
}