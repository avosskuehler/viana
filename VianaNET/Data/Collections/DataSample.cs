// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSample.cs" company="Freie Universität Berlin">
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
//   The data sample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.Data.Collections
{
  using System.Xml.Serialization;

  /// <summary>
  /// This class is the container for the data samples without a time stamp.
  /// The XmlIgnore attribute is used to reduce the data,
  /// cause we can recalculate the values after loading.
  /// </summary>
  public class DataSample
  {
    #region Public Properties

    /// <summary>
    ///   Gets or sets the time in units of the current time unit
    /// </summary>
    public double Time { get; set; }

    /// <summary>
    ///   Gets or sets the pixel x.
    /// </summary>
    public double PixelX { get; set; }

    /// <summary>
    ///   Gets or sets the pixel y.
    /// </summary>
    public double PixelY { get; set; }

    /// <summary>
    ///   Gets or sets the acceleration.
    /// </summary>
    [XmlIgnore]
    public double? Acceleration { get; set; }

    /// <summary>
    ///   Gets or sets the acceleration x.
    /// </summary>
    [XmlIgnore]
    public double? AccelerationX { get; set; }

    /// <summary>
    ///   Gets or sets the acceleration y.
    /// </summary>
    [XmlIgnore]
    public double? AccelerationY { get; set; }

    /// <summary>
    ///   Gets or sets the distance.
    /// </summary>
    [XmlIgnore]
    public double Distance { get; set; }

    /// <summary>
    ///   Gets or sets the distance x.
    /// </summary>
    [XmlIgnore]
    public double DistanceX { get; set; }

    /// <summary>
    ///   Gets or sets the distance y.
    /// </summary>
    [XmlIgnore]
    public double DistanceY { get; set; }

    /// <summary>
    ///   Gets or sets the length.
    /// </summary>
    [XmlIgnore]
    public double Length { get; set; }

    /// <summary>
    ///   Gets or sets the length x.
    /// </summary>
    [XmlIgnore]
    public double LengthX { get; set; }

    /// <summary>
    ///   Gets or sets the length y.
    /// </summary>
    [XmlIgnore]
    public double LengthY { get; set; }

    /// <summary>
    ///   Gets or sets the position x.
    /// </summary>
    [XmlIgnore]
    public double PositionX { get; set; }

    /// <summary>
    ///   Gets or sets the position y.
    /// </summary>
    [XmlIgnore]
    public double PositionY { get; set; }

    /// <summary>
    ///   Gets or sets the velocity.
    /// </summary>
    [XmlIgnore]
    public double? Velocity { get; set; }

    /// <summary>
    ///   Gets or sets the velocity x.
    /// </summary>
    [XmlIgnore]
    public double? VelocityX { get; set; }

    /// <summary>
    ///   Gets or sets the velocity y.
    /// </summary>
    [XmlIgnore]
    public double? VelocityY { get; set; }

    #endregion
  }
}