// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageButton.xaml.cs" company="Freie Universität Berlin">
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
//   The image button.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  ///   The image button.
  /// </summary>
  public class ImageButton : Button
  {
    #region Static Fields

    /// <summary>
    ///   The content horizontal alignment property.
    /// </summary>
    public static readonly DependencyProperty ContentHorizontalAlignmentProperty;

    /// <summary>
    ///   The content vertical alignment property.
    /// </summary>
    public static readonly DependencyProperty ContentVerticalAlignmentProperty;

    /// <summary>
    ///   The image source property.
    /// </summary>
    public static readonly DependencyProperty ImageSourceProperty;

    /// <summary>
    ///   The is tool style property.
    /// </summary>
    public static readonly DependencyProperty IsToolStyleProperty;

    /// <summary>
    ///   The orientation property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref="ImageButton" /> class.
    /// </summary>
    static ImageButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));

      OrientationProperty = DependencyProperty.Register(
        "Orientation", 
        typeof(Orientation), 
        typeof(ImageButton), 
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

      ImageSourceProperty = DependencyProperty.Register(
        "ImageSource", 
        typeof(ImageSource), 
        typeof(ImageButton), 
        new FrameworkPropertyMetadata(
          null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

      IsToolStyleProperty = DependencyProperty.Register(
        "IsToolStyle", 
        typeof(bool), 
        typeof(ImageButton), 
        new FrameworkPropertyMetadata(
          false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange));

      ContentHorizontalAlignmentProperty = DependencyProperty.Register(
        "ContentHorizontalAlignment", 
        typeof(HorizontalAlignment), 
        typeof(ImageButton), 
        new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsRender));

      ContentVerticalAlignmentProperty = DependencyProperty.Register(
        "ContentVerticalAlignment", 
        typeof(VerticalAlignment), 
        typeof(ImageButton), 
        new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsRender));
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets or sets the content horizontal alignment.
    /// </summary>
    public HorizontalAlignment ContentHorizontalAlignment
    {
      get => (HorizontalAlignment)this.GetValue(ContentHorizontalAlignmentProperty);

      set => this.SetValue(ContentHorizontalAlignmentProperty, value);
    }

    /// <summary>
    ///   Gets or sets the content vertical alignment.
    /// </summary>
    public VerticalAlignment ContentVerticalAlignment
    {
      get => (VerticalAlignment)this.GetValue(ContentVerticalAlignmentProperty);

      set => this.SetValue(ContentVerticalAlignmentProperty, value);
    }

    /// <summary>
    ///   Gets or sets the image source.
    /// </summary>
    public ImageSource ImageSource
    {
      get => (ImageSource)this.GetValue(ImageSourceProperty);

      set => this.SetValue(ImageSourceProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether is tool style.
    /// </summary>
    public bool IsToolStyle
    {
      get => (bool)this.GetValue(IsToolStyleProperty);

      set => this.SetValue(IsToolStyleProperty, value);
    }

    /// <summary>
    ///   Gets or sets the orientation.
    /// </summary>
    public Orientation Orientation
    {
      get => (Orientation)this.GetValue(OrientationProperty);

      set => this.SetValue(OrientationProperty, value);
    }

    #endregion
  }
}