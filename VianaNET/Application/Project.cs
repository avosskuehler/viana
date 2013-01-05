// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   //   Viana.NET - video analysis for physics education
//   //   Copyright (C) 2012 Dr. Adrian Voßkühler  
//   //   ------------------------------------------------------------------------
//   //   This program is free software; you can redistribute it and/or modify it 
//   //   under the terms of the GNU General Public License as published by the 
//   //   Free Software Foundation; either version 2 of the License, or 
//   //   (at your option) any later version.
//   //   This program is distributed in the hope that it will be useful, 
//   //   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   //   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   //   See the GNU General Public License for more details.
//   //   You should have received a copy of the GNU General Public License 
//   //   along with this program; if not, write to the Free Software Foundation, 
//   //   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//   //   ************************************************************************
// </copyright>
// <summary>
//   This class holds all project data that can be saved
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Application
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Windows;
  using System.Xml.Serialization;
  using CustomStyles.Types;
  using Data;
  using Data.Filter;
  using Logging;
  using Modules.Video.Control;

  /// <summary>
  /// This class is a singleton encapsulating all settings for a viana.net project
  /// </summary>
  [Serializable]
  [XmlInclude(typeof(System.Windows.Media.MatrixTransform))]
  [XmlInclude(typeof(Data.Filter.Interpolation.MovingAverageFilter))]
  [XmlInclude(typeof(Data.Filter.Interpolation.ExponentialSmoothFilter))]
  public class Project
  {
    /// <summary>
    ///   The static member, that holds the recent files list.
    /// </summary>
    private static Project project;

    /// <summary>
    /// Prevents a default instance of the <see cref="Project"/> class from being created. 
    /// </summary>
    private Project()
    {
      this.FilterData = new FilterData();
      this.CalibrationData = new CalibrationData();
      this.CalibrationData.PropertyChanged += this.CalibrationData_PropertyChanged;
      this.ProcessingData = new ProcessingData();
      this.VideoData = new VideoData { LastPoint = new Point[this.ProcessingData.NumberOfTrackedObjects] };
    }

    /// <summary>
    ///   Gets or sets the current project
    /// </summary>
    public static Project Instance
    {
      get { return project ?? (project = new Project()); }
      set { project = value; }
    }

    /// <summary>
    /// Gets or sets the filter data.
    /// </summary>
    public FilterData FilterData { get; set; }

    /// <summary>
    /// Gets or sets the calibration data.
    /// </summary>
    public CalibrationData CalibrationData { get; set; }

    /// <summary>
    /// Gets or sets the processing data.
    /// </summary>
    public ProcessingData ProcessingData { get; set; }

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
    /// The calibration_ property changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void CalibrationData_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "HasClipRegion" || e.PropertyName == "ClipRegion")
      {
        this.ProcessingData.InitializeImageFilters();
      }
    }
  }
}
