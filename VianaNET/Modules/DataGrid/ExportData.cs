// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportData.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.DataGrid
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Windows.Input;
  using System.Xml;

  using Ionic.Zip;

  using Microsoft.Office.Interop.Excel;

  using VianaNET.Application;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data.Collections;
  using VianaNET.Logging;

  using Application = System.Windows.Application;
  using Labels = VianaNET.Resources.Labels;

  /// <summary>
  ///   Enthält Hilfsfunktionen zum Erzeugen von Excel-Dateien mit SpreadsheetML.
  /// </summary>
  public class ExportData
  {
    #region Static Fields

    /// <summary>
    /// Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
    /// Used for Ods Export.
    /// </summary>
    private static readonly string[,] Namespaces =
    {
      { "table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0" }, 
      { "office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0" }, 
      { "style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0" }, 
      { "text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0" }, 
      { "draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0" }, 
      {
        "fo", 
        "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"
      }, 
      { "dc", "http://purl.org/dc/elements/1.1/" }, 
      { "meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0" }, 
      { "number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0" }, 
      {
        "presentation", 
        "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"
      }, 
      {
        "svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"
      }, 
      { "chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0" }, 
      { "dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0" }, 
      { "math", "http://www.w3.org/1998/Math/MathML" }, 
      { "form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0" }, 
      { "script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0" }, 
      { "ooo", "http://openoffice.org/2004/office" }, 
      { "ooow", "http://openoffice.org/2004/writer" }, 
      { "oooc", "http://openoffice.org/2004/calc" }, 
      { "dom", "http://www.w3.org/2001/xml-events" }, 
      { "xforms", "http://www.w3.org/2002/xforms" }, 
      { "xsd", "http://www.w3.org/2001/XMLSchema" }, 
      { "xsi", "http://www.w3.org/2001/XMLSchema-instance" }, 
      { "rpt", "http://openoffice.org/2005/report" }, 
      { "of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2" }, 
      { "rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#" }, 
      { "config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0" }
    };

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// This method exports the given datasource with the given options to the given file
    ///   in csv format.
    /// </summary>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    public static void ToCsv(DataCollection dataSource, ExportOptions options, string fileName)
    {
      List<List<object>> arrays = GenerateExportArray(dataSource, options);
      WriteToFileWithSeparator(arrays, fileName, ";");
    }

    /// <summary>
    /// Writes DataCollection as .ods file.
    /// </summary>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
    /// <param name="outputFilePath">
    /// The name of the file to save to.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    public static void ToOds(DataCollection dataSource, string outputFilePath, ExportOptions options)
    {
      // Generate arrays
      var arrays = GenerateExportArray(dataSource, options);

      var templateFile =
        ZipFile.Read(
          Assembly.GetExecutingAssembly().GetManifestResourceStream("VianaNET.Modules.DataGrid.template.ods"));
      var contentXml = GetContentXmlFile(templateFile);
      var nmsManager = new XmlNamespaceManager(contentXml.NameTable);

      for (int i = 0; i < Namespaces.GetLength(0); i++)
      {
        nmsManager.AddNamespace(Namespaces[i, 0], Namespaces[i, 1]);
      }

      var tableNodes =
        contentXml.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
      var sheetsRootNode = tableNodes.Item(0).ParentNode;

      // remove sheets from template file
      foreach (XmlNode tableNode in tableNodes)
      {
        sheetsRootNode.RemoveChild(tableNode);
      }

      // Save data
      var ownerDocument = sheetsRootNode.OwnerDocument;

      XmlNode sheetNode = ownerDocument.CreateElement("table:table", GetNamespaceUri("table"));

      var sheetName = ownerDocument.CreateAttribute("table:name", GetNamespaceUri("table"));
      sheetName.Value = Viana.Project.ProjectFilename;
      sheetNode.Attributes.Append(sheetName);

      // SaveColumnDefinition
      var columnDefinition = ownerDocument.CreateElement("table:table-column", GetNamespaceUri("table"));
      var columnsCount = ownerDocument.CreateAttribute(
        "table:number-columns-repeated",
        GetNamespaceUri("table"));
      columnsCount.Value = arrays[0].Count.ToString(CultureInfo.InvariantCulture);
      columnDefinition.Attributes.Append(columnsCount);
      sheetNode.AppendChild(columnDefinition);

      // Save rows
      foreach (var row in arrays)
      {
        XmlNode rowNode = ownerDocument.CreateElement("table:table-row", GetNamespaceUri("table"));
        foreach (object column in row)
        {
          XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));

          if (column is string)
          {
            // We save values as text (string)
            XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
            valueType.Value = "string";
            cellNode.Attributes.Append(valueType);

            XmlElement cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
            cellValue.InnerText = column.ToString();
            cellNode.AppendChild(cellValue);
          }
          else if (column is double)
          {
            // We save values as text (string)
            var valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
            valueType.Value = "float";
            cellNode.Attributes.Append(valueType);

            var value = ownerDocument.CreateAttribute("office:value", GetNamespaceUri("office"));
            var doubleValue = (double)column;
            value.Value = doubleValue.ToString("N2", new CultureInfo("en-US"));
            cellNode.Attributes.Append(value);

            var cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
            cellValue.InnerText = column.ToString();
            cellNode.AppendChild(cellValue);
          }
          else if (column is int)
          {
            // We save values as text (string)
            var valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
            valueType.Value = "float";
            cellNode.Attributes.Append(valueType);

            var value = ownerDocument.CreateAttribute("office:value", GetNamespaceUri("office"));
            value.Value = column.ToString();
            cellNode.Attributes.Append(value);

            var cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
            cellValue.InnerText = column.ToString();
            cellNode.AppendChild(cellValue);
          }

          rowNode.AppendChild(cellNode);
        }

        sheetNode.AppendChild(rowNode);
      }

      sheetsRootNode.AppendChild(sheetNode);

      // SaveContentXml
      templateFile.RemoveEntry("content.xml");

      var memStream = new MemoryStream();
      contentXml.Save(memStream);
      memStream.Seek(0, SeekOrigin.Begin);

      templateFile.AddEntry("content.xml", memStream);
      templateFile.Save(outputFilePath);
    }

    /// <summary>
    /// This method exports the given datasource with the given options to the given file
    ///   in txt format.
    /// </summary>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    public static void ToTxt(DataCollection dataSource, ExportOptions options, string fileName)
    {
      List<List<object>> arrays = GenerateExportArray(dataSource, options);
      WriteToFileWithSeparator(arrays, fileName, "\t");
    }

    /// <summary>
    /// This method exports the given datasource with the given options directly to
    ///   excel, if it is installed
    /// </summary>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    public static void ToXls(DataCollection dataSource, ExportOptions options)
    {
      Application.Current.MainWindow.Cursor = Cursors.Wait;
      try
      {
        var xla = new Microsoft.Office.Interop.Excel.Application { Visible = true };

        xla.Workbooks.Add(XlSheetType.xlWorksheet);

        var ws = (Worksheet)xla.ActiveSheet;

        List<List<object>> arrays = GenerateExportArray(dataSource, options);

        for (int i = 1; i <= arrays.Count; i++)
        {
          List<object> row = arrays[i - 1];
          for (int j = 1; j <= row.Count; j++)
          {
            object column = row[j - 1];
            ws.Cells[i, j] = column;
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
    /// <param name="options">
    /// Die Exporteinstellungen
    /// </param>
    /// <param name="fileName">
    /// Dateiname der Ausgabe-XML-Datei
    /// </param>
    public static void ToXml(DataCollection dataSource, ExportOptions options, string fileName)
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
      writer.WriteAttributeString("xmlns", "o", string.Empty, "urn:schemas-microsoft-com:office:office");
      writer.WriteAttributeString("xmlns", "x", string.Empty, "urn:schemas-microsoft-com:office:excel");
      writer.WriteAttributeString("xmlns", "ss", string.Empty, "urn:schemas-microsoft-com:office:spreadsheet");
      writer.WriteAttributeString("xmlns", "html", string.Empty, "http://www.w3.org/TR/REC-html40");

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
      writer.WriteAttributeString("ss", "ID", string.Empty, "Default");
      writer.WriteAttributeString("ss", "Name", string.Empty, "Normal");

      // <Alignment ss:Vertical="Bottom"/>
      writer.WriteStartElement("Alignment");
      writer.WriteAttributeString("ss", "Vertical", string.Empty, "Bottom");
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
      writer.WriteAttributeString("ss", "Name", string.Empty, "VianaNET Data");

      // <Table x:FullColumns="1" x:FullRows="1" ss:DefaultColumnWidth="60">
      writer.WriteStartElement("Table");
      writer.WriteAttributeString("x", "FullColumns", string.Empty, "1");
      writer.WriteAttributeString("x", "FullRows", string.Empty, "1");
      writer.WriteAttributeString("ss", "DefaultColumnWidth", string.Empty, "60");

      List<List<object>> arrays = GenerateExportArray(dataSource, options);
      foreach (var row in arrays)
      {
        // <Row>
        writer.WriteStartElement("Row");

        foreach (object column in row)
        {
          WriteCellValue(writer, "Number", column);
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
      writer.WriteAttributeString("x", "Margin", string.Empty, "0.4921259845");
      writer.WriteEndElement();
      writer.WriteStartElement("Footer");
      writer.WriteAttributeString("x", "Margin", string.Empty, "0.4921259845");
      writer.WriteEndElement();
      writer.WriteStartElement("PageMargins");
      writer.WriteAttributeString("x", "Bottom", string.Empty, "0.984251969");
      writer.WriteAttributeString("x", "Left", string.Empty, "0.78740157499999996");
      writer.WriteAttributeString("x", "Right", string.Empty, "0.78740157499999996");
      writer.WriteAttributeString("x", "Top", string.Empty, "0.984251969");
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
    /// Generates the export array.
    /// </summary>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <returns>
    /// An array with the columns and data that should be exported according to options
    /// </returns>
    private static List<List<object>> GenerateExportArray(DataCollection dataSource, ExportOptions options)
    {
      var exportArray = new List<List<object>>();

      // Write column header
      var header = new List<object>();
      if (options.Axes.Contains(DataAxis.DataAxes[0]))
      {
        header.Add(DataAxis.DataAxes[0].Description);
      }

      if (options.Axes.Contains(DataAxis.DataAxes[1]))
      {
        header.Add(DataAxis.DataAxes[1].Description + " [" + Viana.Project.CalibrationData.TimeUnit + "]");
      }

      foreach (int objectIndex in options.Objects)
      {
        int oneBased = objectIndex + 1;
        string title = options.Objects.Count > 1
                         ? Labels.DataGridObjectPrefix + " " + oneBased.ToString(CultureInfo.InvariantCulture) + ": "
                         : string.Empty;
        if (options.Axes.Contains(DataAxis.DataAxes[2]))
        {
          header.Add(title + DataAxis.DataAxes[2].Description + " [" + Viana.Project.CalibrationData.PixelUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[3]))
        {
          header.Add(title + DataAxis.DataAxes[3].Description + " [" + Viana.Project.CalibrationData.PixelUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[4]))
        {
          header.Add(title + DataAxis.DataAxes[4].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[5]))
        {
          header.Add(title + DataAxis.DataAxes[5].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[6]))
        {
          header.Add(title + DataAxis.DataAxes[6].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[7]))
        {
          header.Add(title + DataAxis.DataAxes[7].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[8]))
        {
          header.Add(title + DataAxis.DataAxes[8].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[9]))
        {
          header.Add(title + DataAxis.DataAxes[9].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[10]))
        {
          header.Add(title + DataAxis.DataAxes[10].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[11]))
        {
          header.Add(title + DataAxis.DataAxes[11].Description + " [" + Viana.Project.CalibrationData.LengthUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[12]))
        {
          header.Add(
            title + DataAxis.DataAxes[12].Description + " [" + Viana.Project.CalibrationData.VelocityUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[13]))
        {
          header.Add(
            title + DataAxis.DataAxes[13].Description + " [" + Viana.Project.CalibrationData.VelocityUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[14]))
        {
          header.Add(
            title + DataAxis.DataAxes[14].Description + " [" + Viana.Project.CalibrationData.VelocityUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[15]))
        {
          header.Add(
            title + DataAxis.DataAxes[15].Description + " [" + Viana.Project.CalibrationData.AccelerationUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[16]))
        {
          header.Add(
            title + DataAxis.DataAxes[16].Description + " [" + Viana.Project.CalibrationData.AccelerationUnit + "]");
        }

        if (options.Axes.Contains(DataAxis.DataAxes[17]))
        {
          header.Add(
            title + DataAxis.DataAxes[17].Description + " [" + Viana.Project.CalibrationData.AccelerationUnit + "]");
        }
      }

      exportArray.Add(header);

      foreach (TimeSample sample in dataSource)
      {
        var columns = new List<object>();
        if (options.Axes.Contains(DataAxis.DataAxes[0]))
        {
          columns.Add(sample.Framenumber);
        }

        if (options.Axes.Contains(DataAxis.DataAxes[1]))
        {
          TimeUnit timeunit = Viana.Project.CalibrationData.TimeUnit;
          switch (timeunit)
          {
            case TimeUnit.ms:
              columns.Add(sample.Timestamp);
              break;
            case TimeUnit.s:
              columns.Add(Math.Round(sample.Timestamp / 1000d, 4));
              break;
            default:
              throw new ArgumentOutOfRangeException("Wrong TimeUnit");
          }
        }

        foreach (int objectIndex in options.Objects)
        {
          if (options.Axes.Contains(DataAxis.DataAxes[2]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].PixelX, 0));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[3]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].PixelY, 0));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[4]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].PositionX, 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[5]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].PositionY, 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[6]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].Distance.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[7]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].DistanceX.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[8]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].DistanceY.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[9]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].Length.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[10]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].LengthX.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[11]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].LengthY.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[12]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].Velocity.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[13]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].VelocityX.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[14]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].VelocityY.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[15]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].Acceleration.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[16]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].AccelerationX.GetValueOrDefault(0), 2));
            }
          }

          if (options.Axes.Contains(DataAxis.DataAxes[17]))
          {
            if (sample.Object[objectIndex] == null)
            {
              columns.Add(string.Empty);
            }
            else
            {
              columns.Add(Math.Round(sample.Object[objectIndex].AccelerationY.GetValueOrDefault(0), 2));
            }
          }
        }

        exportArray.Add(columns);
      }

      return exportArray;
    }

    /// <summary>
    /// The get content xml file.
    /// </summary>
    /// <param name="zipFile">
    /// The zip file.
    /// </param>
    /// <returns>
    /// The <see cref="XmlDocument"/>.
    /// </returns>
    private static XmlDocument GetContentXmlFile(ZipFile zipFile)
    {
      // Get file(in zip archive) that contains data ("content.xml").
      ZipEntry contentZipEntry = zipFile["content.xml"];

      // Extract that file to MemoryStream.
      Stream contentStream = new MemoryStream();
      contentZipEntry.Extract(contentStream);
      contentStream.Seek(0, SeekOrigin.Begin);

      // Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
      var contentXml = new XmlDocument();
      contentXml.Load(contentStream);

      return contentXml;
    }

    /// <summary>
    /// Gets the namespace URI.
    /// </summary>
    /// <param name="prefix">
    /// The prefix.
    /// </param>
    /// <returns>
    /// The string with the namespace uri
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// Can't find that namespace URI
    /// </exception>
    private static string GetNamespaceUri(string prefix)
    {
      for (int i = 0; i < Namespaces.GetLength(0); i++)
      {
        if (Namespaces[i, 0] == prefix)
        {
          return Namespaces[i, 1];
        }
      }

      throw new InvalidOperationException("Can't find that namespace URI");
    }

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
      writer.WriteAttributeString("ss", "Type", string.Empty, cellType);

      // Zelleninhalt schreiben
      writer.WriteValue(cellValue ?? string.Empty);

      // </Data>
      writer.WriteEndElement();

      // </Cell>
      writer.WriteEndElement();
    }

    /// <summary>
    /// Writes to file with separator.
    /// </summary>
    /// <param name="arrays">
    /// The arrays.
    /// </param>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <param name="separator">
    /// The separator.
    /// </param>
    private static void WriteToFileWithSeparator(List<List<object>> arrays, string fileName, string separator)
    {
      using (var sw = new StreamWriter(fileName))
      {
        foreach (var row in arrays)
        {
          foreach (object column in row)
          {
            sw.Write(column);
            sw.Write(separator);
          }

          sw.WriteLine();
        }
      }
    }

    #endregion
  }
}