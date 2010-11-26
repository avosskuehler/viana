# region Using Directives

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

# endregion

namespace VianaNET
{
  public class ImageButton : Button
  {
    # region Declarations

    public static readonly DependencyProperty OrientationProperty;

    public static readonly DependencyProperty ImageSourceProperty;

    public static readonly DependencyProperty IsToolStyleProperty;

    public static readonly DependencyProperty ContentHorizontalAlignmentProperty;

    public static readonly DependencyProperty ContentVerticalAlignmentProperty;

    # endregion

    # region Static Constructor

    static ImageButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton),
        new FrameworkPropertyMetadata(typeof(ImageButton)));

      ImageButton.OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation),
        typeof(ImageButton), new FrameworkPropertyMetadata(Orientation.Horizontal,
        FrameworkPropertyMetadataOptions.AffectsMeasure));

      ImageButton.ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender |
        FrameworkPropertyMetadataOptions.AffectsMeasure));

      ImageButton.IsToolStyleProperty = DependencyProperty.Register("IsToolStyle", typeof(bool), typeof(ImageButton),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender
        | FrameworkPropertyMetadataOptions.AffectsArrange));

      ImageButton.ContentHorizontalAlignmentProperty = DependencyProperty.Register(
        "ContentHorizontalAlignment", typeof(HorizontalAlignment),
        typeof(ImageButton), new FrameworkPropertyMetadata(HorizontalAlignment.Center,
        FrameworkPropertyMetadataOptions.AffectsRender));

      ImageButton.ContentVerticalAlignmentProperty = DependencyProperty.Register(
        "ContentVerticalAlignment", typeof(VerticalAlignment),
        typeof(ImageButton), new FrameworkPropertyMetadata(VerticalAlignment.Center,
        FrameworkPropertyMetadataOptions.AffectsRender));

    }

    # endregion

    # region ContentHorizontalAlignment

    public HorizontalAlignment ContentHorizontalAlignment
    {
      get
      {
        return (HorizontalAlignment)this.GetValue(ImageButton.ContentHorizontalAlignmentProperty);
      }
      set
      {
        this.SetValue(ImageButton.ContentHorizontalAlignmentProperty, value);
      }
    }

    # endregion

    # region ContentVerticalAlignment

    public VerticalAlignment ContentVerticalAlignment
    {
      get
      {
        return (VerticalAlignment)this.GetValue(ImageButton.ContentVerticalAlignmentProperty);
      }
      set
      {
        this.SetValue(ImageButton.ContentVerticalAlignmentProperty, value);
      }
    }

    # endregion

    # region Orientation

    public Orientation Orientation
    {
      get
      {
        return (Orientation)this.GetValue(ImageButton.OrientationProperty);
      }
      set
      {
        this.SetValue(ImageButton.OrientationProperty, value);
      }
    }

    # endregion

    # region ImageSource

    public ImageSource ImageSource
    {
      get
      {
        return (ImageSource)this.GetValue(ImageButton.ImageSourceProperty);
      }
      set
      {
        this.SetValue(ImageButton.ImageSourceProperty, value);
      }
    }

    # endregion

    #region IsToolStyle

    public bool IsToolStyle
    {
      get
      {
        return (bool)GetValue(IsToolStyleProperty);
      }
      set
      {
        SetValue(IsToolStyleProperty, value);
      }
    }

    #endregion
  }
}
