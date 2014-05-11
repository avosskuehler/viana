// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Freie Universität Berlin">
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
// <summary>
//   This class holds all project data that can be saved
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Application
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Xml.Serialization;
  using CustomStyles.Types;
  using Data;
  using Data.Filter;
  using Logging;

  using Modules.Video.Control;

  using Point = System.Windows.Point;

  /// <summary>
  /// This class is a singleton encapsulating all settings for a viana.net project
  /// </summary>
  [Serializable]
  [XmlInclude(typeof(System.Windows.Media.MatrixTransform))]
  [XmlInclude(typeof(Data.Filter.Interpolation.MovingAverageFilter))]
  [XmlInclude(typeof(Data.Filter.Interpolation.ExponentialSmoothFilter))]
  public class Project : INotifyPropertyChanged
  {
    /// <summary>
    /// The current chart type
    /// </summary>
    private ChartType currentChartType;

    /// <summary>
    /// Gets or sets the filter data for the specific <see cref="ChartType"/>
    /// </summary>
    private Dictionary<ChartType, FilterData> filterData;

    /// <summary>
    /// Initializes a new instance of the <see cref="Project"/> class. 
    /// </summary>
    public Project()
    {
      this.filterData = new Dictionary<ChartType, FilterData> { { ChartType.YoverX, new FilterData() } };
      this.CalibrationData = new CalibrationData();
      this.CalibrationData.PropertyChanged += this.CalibrationDataPropertyChanged;
      this.ProcessingData = new ProcessingData();
      this.VideoData = new VideoData { LastPoint = new Point[this.ProcessingData.NumberOfTrackedObjects] };
      this.CurrentFilterData = new FilterData();
      //this.ChartData = new ChartData();
      this.currentChartType = ChartType.YoverX;
    }

    /// <summary>
    /// Implements INotifyPropertyChanged
    /// </summary>
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets or sets the current filter data.
    /// </summary>
    /// <value>
    /// The current filter data.
    /// </value>
    public FilterData CurrentFilterData { get; set; }

    /// <summary>
    /// Gets or sets the calibration data.
    /// </summary>
    public CalibrationData CalibrationData { get; set; }

    /// <summary>
    /// Gets or sets the processing data.
    /// </summary>
    public ProcessingData ProcessingData { get; set; }

    ///// <summary>
    ///// Gets or sets the chart data
    ///// </summary>
    //public ChartData ChartData { get; set; }

    /// <summary>
    /// Gets or sets the video data.
    /// </summary>
    public VideoData VideoData { get; set; }

    /// <summary>
    /// Gets or sets the video mode.
    /// </summary>
    public VideoMode VideoMode { get; set; }

    /// <summary>
    /// Gets or sets the video file, if in player mode.
    /// </summary>
    public string VideoFile { get; set; }

    /// <summary>
    /// Gets or sets the filename.
    /// </summary>
    public string ProjectFilename { get; set; }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    public string ProjectPath { get; set; }

    /// <summary>
    /// Gets or sets the current selected <see cref="ChartType"/> in the charts module
    /// </summary>
    public ChartType CurrentChartType
    {
      get
      {
        return this.currentChartType;
      }
      
      set
      {
        // Write modified values back to filterData dictionary
        this.CopyFilterData(this.CurrentFilterData, this.filterData[this.currentChartType]);

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
      target.RegressionSeries = source.RegressionSeries;
      target.SelectionColor = source.SelectionColor;
      target.TheoreticalFunction = source.TheoreticalFunction;
      target.TheoryFilter = source.TheoryFilter;
      target.TheoryFunctionTexFormula = source.TheoryFunctionTexFormula;
      target.TheoryLineColor = source.TheoryLineColor;
      target.TheoryLineThickness = source.TheoryLineThickness;
      target.TheorySeries = source.TheorySeries;
    }

    /// <summary>
    /// Gets a value indicating whether there are video input devices
    /// available on the system
    /// </summary>
    public bool HasData
    {
      get
      {
        return this.VideoData.Samples.Count > 0;
      }
    }

    /// <summary>
    /// Serializes the project into the given file in a xml structure.
    /// </summary>
    /// <remarks>The xml file should have the .via extension to achieve its affiliation to Viana.NET</remarks>
    /// <param name="projectToSerialize">The <see cref="Project"/> object to serialize.</param>
    /// <param name="filePath">Full file path to the .via xml settings file.</param>
    /// <returns><strong>True</strong> if succesful.</returns>
    public static bool Serialize(Project projectToSerialize, string filePath)
    {
      try
      {
        using (TextWriter writer = new StreamWriter(filePath))
        {
          // Create an instance of the XmlSerializer class;
          // specify the type of object to serialize 
          var serializer = new XmlSerializer(typeof(Project));
          projectToSerialize.ProjectFilename = Path.GetFileName(filePath);
          projectToSerialize.ProjectPath = Path.GetDirectoryName(filePath);
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
    /// Deserializes the experiment settings from the given xml file.
    /// </summary>
    /// <remarks>The xml file is named *.via to achieve its affiliation to Viana.NET</remarks>
    /// <param name="filePath">Full file path to the .via xml settings file.</param>
    /// <returns>A <see cref="Project"/> object if succesful 
    /// or <strong>null</strong> if failed.</returns>
    public static Project Deserialize(string filePath)
    {
      try
      {
        Project projectFromFile;

        // A FileStream is needed to read the XML document.
        using (var fs = new FileStream(filePath, FileMode.Open))
        {
          // Create an instance of the XmlSerializer class;
          // specify the type of object to be deserialized 
          var serializer = new XmlSerializer(typeof(Project));

          ////* If the XML document has been altered with unknown 
          ////nodes or attributes, handle them with the 
          ////UnknownNode and UnknownAttribute events.*/
          ////serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
          ////serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

          /* Use the Deserialize method to restore the object's state with
          data from the XML document. */
          projectFromFile = (Project)serializer.Deserialize(fs);
        }

        return projectFromFile;
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
      }

      return null;
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name. 
    /// </param>
    public virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// The calibration property changed event handler which re-initializes
    /// the image filters of the processing data.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void CalibrationDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" || e.PropertyName == "ClipRegion")
      {
        this.ProcessingData.InitializeImageFilters();
      }
    }
  }
}
