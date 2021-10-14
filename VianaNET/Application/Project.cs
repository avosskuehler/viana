// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Freie Universität Berlin">
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
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Application
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Windows;
  using System.Windows.Media;
  using System.Xml.Serialization;

  using VianaNET.CustomStyles.Types;
  using VianaNET.Data;
  using VianaNET.Data.Filter;
  using VianaNET.Data.Filter.Interpolation;
  using VianaNET.Logging;
  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   This class is a singleton encapsulating all settings for a viana.net project
  /// </summary>
  [Serializable]
  [XmlInclude(typeof(MatrixTransform))]
  [XmlInclude(typeof(MovingAverageFilter))]
  [XmlInclude(typeof(ExponentialSmoothFilter))]
  [XmlInclude(typeof(WpfMath.TexFormula))]
  public class Project : INotifyPropertyChanged
  {
    /// <summary>
    ///   The filter data for the specific <see cref="ChartType" />
    /// </summary>
    private Dictionary<ChartType, FilterData> filterData;

    /// <summary>
    ///   The current chart type
    /// </summary>
    private ChartType currentChartType;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Project" /> class.
    /// </summary>
    public Project()
    {
      //TexFormulaParser.Initialize();
      this.filterData = new Dictionary<ChartType, FilterData> { { ChartType.YoverX, new FilterData() } };
      this.CalibrationData = new CalibrationData();
      this.CalibrationData.PropertyChanged += this.CalibrationDataPropertyChanged;
      this.ProcessingData = new ProcessingData();
      this.VideoData = new VideoData { LastPoint = new Point[this.ProcessingData.NumberOfTrackedObjects] };
      this.CurrentFilterData = new FilterData();
      this.currentChartType = ChartType.YoverX;
    }

    /// <summary>
    ///   Implements INotifyPropertyChanged
    /// </summary>
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// The update chart requested.
    /// </summary>
    [field: NonSerialized]
    public event EventHandler<EventArgs> UpdateChartRequested;

    /// <summary>
    /// Gets or sets a value indicating whether we are in the deserialization process of a project.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is deserializing; otherwise, <c>false</c>.
    /// </value>
    public static bool IsDeserializing { get; set; }

    /// <summary>
    ///   Gets or sets the calibration data.
    /// </summary>
    public CalibrationData CalibrationData { get; set; }

    /// <summary>
    ///   Gets or sets the filter data in a serializable manner.
    /// </summary>
    public DictionaryProxy<ChartType, FilterData> SerializableFilterData
    {
      get => new DictionaryProxy<ChartType, FilterData>(this.filterData);

      set => this.filterData = value.ToDictionary();
    }

    /// <summary>
    ///   Gets or sets the current filter data.
    /// </summary>
    /// <value>
    ///   The current filter data.
    /// </value>
    [XmlIgnore]
    public FilterData CurrentFilterData { get; set; }

    /// <summary>
    ///   Gets or sets the processing data.
    /// </summary>
    public ProcessingData ProcessingData { get; set; }

    ///// <summary>
    ///// Gets or sets the chart data
    ///// </summary>
    // public ChartData ChartData { get; set; }

    /// <summary>
    ///   Gets or sets the filename with full path to the project file.
    /// </summary>
    public string ProjectFilename { get; set; }

    /// <summary>
    ///   Gets the path of the project
    /// </summary>
    [XmlIgnore]
    public string ProjectPath => Path.GetDirectoryName(this.ProjectFilename);

    /// <summary>
    ///   Gets the filename of the project
    /// </summary>
    [XmlIgnore]
    public string ProjectFile => Path.GetFileName(this.ProjectFilename);

    /// <summary>
    ///   Gets or sets the video data.
    /// </summary>
    public VideoData VideoData { get; set; }

    /// <summary>
    ///   Gets or sets the video file name with full path, if in player mode.
    /// </summary>
    public string VideoFile { get; set; }

    /// <summary>
    ///   Gets the video file full path, if in player mode.
    /// </summary>
    [XmlIgnore]
    public string VideoFilePath => Path.GetDirectoryName(this.VideoFile);

    /// <summary>
    ///   Gets or sets the video mode.
    /// </summary>
    public VideoMode VideoMode { get; set; }

    /// <summary>
    ///   Gets or sets the current selected <see cref="ChartType" /> in the charts module
    /// </summary>
    public ChartType CurrentChartType
    {
      get => this.currentChartType;

      set
      {
        if (!IsDeserializing)
        {
          // Write modified values back to filterData dictionary
          // but not on deserialising, cause it would overwrite the loaded values
          // with default values
          this.CopyFilterData(this.CurrentFilterData, this.filterData[this.currentChartType]);
        }

        // Go to new chart
        this.currentChartType = value;

        // Check if we already have this chart type used in this project
        // if not create a new filterdata entry
        if (!this.filterData.ContainsKey(this.currentChartType))
        {
          this.filterData.Add(this.currentChartType, new FilterData());
        }

        // Move filter data from dictionary to bindable currentfilterdata
        this.CopyFilterData(this.filterData[this.currentChartType], this.CurrentFilterData);
      }
    }

    /// <summary>
    /// Deserializes the experiment settings from the given xml file.
    /// </summary>
    /// <remarks>
    /// The xml file is named *.via to achieve its affiliation to Viana.NET
    /// </remarks>
    /// <param name="filePath">
    /// Full file path to the .via xml settings file.
    /// </param>
    /// <returns>
    /// A <see cref="Project"/> object if succesful
    ///   or <strong>null</strong> if failed.
    /// </returns>
    public static Project Deserialize(string filePath)
    {
      try
      {
        Project projectFromFile;

        // A FileStream is needed to read the XML document.
        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
          // Create an instance of the XmlSerializer class;
          // specify the type of object to be deserialized 
          XmlSerializer serializer = new XmlSerializer(typeof(Project));

          //* If the XML document has been altered with unknown 
          //nodes or attributes, handle them with the 
          //UnknownNode and UnknownAttribute events.*/
          serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
          serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

          /* Use the Deserialize method to restore the object's state with
          data from the XML document. */
          projectFromFile = (Project)serializer.Deserialize(fs);

          // Write modified values back to serialized filterData dictionary
          projectFromFile.CopyFilterData(projectFromFile.filterData[projectFromFile.currentChartType], projectFromFile.CurrentFilterData);
        }

        return projectFromFile;
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
      }

      return null;
    }

    private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
    {
      
    }

    private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
    {
    }

    /// <summary>
    /// Serializes the project into the given file in a xml structure.
    /// </summary>
    /// <remarks>
    /// The xml file should have the .via extension to achieve its affiliation to Viana.NET
    /// </remarks>
    /// <param name="projectToSerialize">
    /// The <see cref="Project"/> object to serialize.
    /// </param>
    /// <param name="fileWithPath">Full file path to the .via xml settings file.</param>
    /// <returns>
    /// <strong>True</strong> if succesful.
    /// </returns>
    public static bool Serialize(Project projectToSerialize, string fileWithPath)
    {
      try
      {
        using (TextWriter writer = new StreamWriter(fileWithPath))
        {
          // Write modified values back to serialized filterData dictionary
          projectToSerialize.CopyFilterData(projectToSerialize.CurrentFilterData, projectToSerialize.filterData[projectToSerialize.currentChartType]);

          // Create an instance of the XmlSerializer class;
          // specify the type of object to serialize 
          XmlSerializer serializer = new XmlSerializer(typeof(Project));
          projectToSerialize.ProjectFilename = fileWithPath;
          projectToSerialize.VideoMode = Video.Instance.VideoMode;
          projectToSerialize.VideoFile = Video.Instance.VideoPlayerElement.VideoFilename;

          // Serialize the ExperimentSettings, and close the TextWriter.
          serializer.Serialize(writer, projectToSerialize);
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
        return false;
      }

      return true;
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name.
    /// </param>
    public virtual void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///   Requests a chart update.
    /// </summary>
    public void RequestChartUpdate()
    {
      this.UpdateChartRequested?.Invoke(this, EventArgs.Empty);
    }





    /// <summary>
    /// The calibration property changed event handler which re-initializes
    ///   the image filters of the processing data.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private void CalibrationDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" || e.PropertyName == "ClipRegion")
      {
        this.ProcessingData.InitializeImageFilters();
      }
    }

    /// <summary>
    /// The copy filter data.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    private void CopyFilterData(FilterData source, FilterData target)
    {
      target.AxisX = source.AxisX;
      target.AxisY = source.AxisY;
      target.CurrentFilter = source.CurrentFilter;
      target.DataLineColor = source.DataLineColor;
      target.DataLineThickness = source.DataLineThickness;
      target.InterpolationFilter = source.InterpolationFilter;
      target.InterpolationLineColor = source.InterpolationLineColor;
      target.InterpolationLineThickness = source.InterpolationLineThickness;
      target.InterpolationSeries = source.InterpolationSeries;
      target.IsShowingDataSeries = source.IsShowingDataSeries;
      target.IsShowingInterpolationSeries = source.IsShowingInterpolationSeries;
      target.IsShowingRegressionSeries = source.IsShowingRegressionSeries;
      target.IsShowingTheorySeries = source.IsShowingTheorySeries;
      target.NumericPrecision = source.NumericPrecision;
      target.RegressionAberration = source.RegressionAberration;
      target.RegressionFilter = source.RegressionFilter;
      target.RegressionFunctionTexFormula = source.RegressionFunctionTexFormula;
      target.RegressionLineColor = source.RegressionLineColor;
      target.RegressionLineThickness = source.RegressionLineThickness;
      target.SelectionColor = source.SelectionColor;
      target.TheoryFilter = source.TheoryFilter;
      target.TheoryFunctionTexFormula = source.TheoryFunctionTexFormula;
      target.TheoryLineColor = source.TheoryLineColor;
      target.TheoryLineThickness = source.TheoryLineThickness;
    }


  }
}