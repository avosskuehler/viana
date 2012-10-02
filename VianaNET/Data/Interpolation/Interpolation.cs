// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs" company="Freie Universität Berlin">
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
//   This singleton class provides static access to the properties
//   used to measure lenght in the video, if the user has provided
//   a calibration point and length.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Interpolation
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Windows;

  /// <summary>
  ///   This singleton class provides static access to the properties
  ///   used to measure lenght in the video, if the user has provided
  ///   a calibration point and length.
  /// </summary>
  public class Interpolation : DependencyObject, INotifyPropertyChanged
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="CurrentInterpolationFilter" />.
    /// </summary>
    public static readonly DependencyProperty CurrentInterpolationFilterProperty =
      DependencyProperty.Register(
        "CurrentInterpolationFilter", 
        typeof(InterpolationBase), 
        typeof(Calibration), 
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   The <see cref="DependencyProperty" /> for the property <see cref="IsInterpolatingData" />.
    /// </summary>
    public static readonly DependencyProperty IsInterpolatingDataProperty =
      DependencyProperty.Register(
        "IsInterpolatingData", 
        typeof(bool), 
        typeof(Calibration), 
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    ///   Holds the instance of singleton
    /// </summary>
    private static Interpolation instance;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="Interpolation" /> class. 
    ///   Initializes an instance of the Interpolation class.
    /// </summary>
    static Interpolation()
    {
      // Populate predefined interpolation filters
      Filter = new Dictionary<FilterTypes, InterpolationBase>();
      Filter.Add(FilterTypes.MovingAverage, new MovingAverageFilter());
      Filter.Add(FilterTypes.ExponentialSmooth, new ExponentialSmoothFilter());
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Interpolation" /> class.
    /// </summary>
    public Interpolation()
    {
      this.InitFields();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Events

    /// <summary>
    ///   Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Enums

    /// <summary>
    ///   Enumerates the available interpolation filter types
    /// </summary>
    public enum FilterTypes
    {
      /// <summary>
      ///   Describes the moving average filter which averages 
      ///   a specific amount of surrounding sample values.
      /// </summary>
      MovingAverage = 1, 

      /// <summary>
      ///   Describes the exponential smoothing algorithm.
      /// </summary>
      ExponentialSmooth = 2
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    ///   Gets or sets the List of available interpolation filters.
    /// </summary>
    public static Dictionary<FilterTypes, InterpolationBase> Filter { get; private set; }

    /// <summary>
    ///   Gets the <see cref="Interpolation" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static Interpolation Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new Interpolation();
        }

        // return the existing/new instance
        return instance;
      }
    }

    /// <summary>
    ///   Gets or sets a the interpolation filter to use for
    ///   smoothing data.
    /// </summary>
    public InterpolationBase CurrentInterpolationFilter
    {
      get
      {
        return (InterpolationBase)this.GetValue(CurrentInterpolationFilterProperty);
      }

      set
      {
        this.SetValue(CurrentInterpolationFilterProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether to interpolate
    ///   velocity and acceleration values.
    /// </summary>
    public bool IsInterpolatingData
    {
      get
      {
        return (bool)this.GetValue(IsInterpolatingDataProperty);
      }

      set
      {
        this.SetValue(IsInterpolatingDataProperty, value);
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Methods and Operators

    /// <summary>
    ///   The show interpolation options dialog.
    /// </summary>
    public static void ShowInterpolationOptionsDialog()
    {
      var dlg = new InterpolationOptionsDialog();
      dlg.ChoosenInterpolationFilter = Instance.CurrentInterpolationFilter;

      if (dlg.ShowDialog().GetValueOrDefault())
      {
        Instance.CurrentInterpolationFilter = dlg.ChoosenInterpolationFilter;
        VideoData.Instance.RefreshDistanceVelocityAcceleration();
      }
    }

    /// <summary>
    ///   Resets the properties to their default values.
    /// </summary>
    public void Reset()
    {
      this.InitFields();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="obj">
    /// The source of the event. This. 
    /// </param>
    /// <param name="args">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> with the event data. 
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as Interpolation).OnPropertyChanged(args);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   Initially set the properties default values.
    /// </summary>
    private void InitFields()
    {
      this.IsInterpolatingData = false;
      this.CurrentInterpolationFilter = Filter[FilterTypes.MovingAverage];
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}