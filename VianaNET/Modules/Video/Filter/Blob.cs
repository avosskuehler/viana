// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace VianaNET
{
  using System;
  using System.Windows;
  using System.ComponentModel;
  using System.Windows.Media;

  /// <summary>
  /// Image's blob.
  /// </summary>
  /// 
  /// <remarks><para>The class represents a blob - part of another images. The
  /// class encapsulates the blob itself and information about its position
  /// in parent image.</para>
  /// 
  /// <para><note>The class is not responsible for blob's image disposing, so it should be
  /// done manually when it is required.</note></para>
  /// </remarks>
  /// 
  public class Blob
  {
    // blob's rectangle in the original image
    private Rect rect;
    // blob's ID in the original image
    private int id;
    // area of the blob
    private int area;
    // center of gravity
    private Point cog;
    // mean color of the blob
    private Color colorMean = Colors.Black;
    // color's standard deviation of the blob
    private Color colorStdDev = Colors.Black;

    /// <summary>
    /// Blob's rectangle in the original image.
    /// </summary>
    /// 
    /// <remarks><para>The property specifies position of the blob in the original image
    /// and its size.</para></remarks>
    /// 
    public Rect Rectangle
    {
      get { return rect; }
    }

    /// <summary>
    /// Blob's ID in the original image.
    /// </summary>
    [Browsable(false)]
    public int ID
    {
      get { return id; }
      internal set { id = value; }
    }

    /// <summary>
    /// Blob's area.
    /// </summary>
    /// 
    /// <remarks><para>The property equals to blob's area measured in number of pixels
    /// contained by the blob.</para></remarks>
    /// 
    public int Area
    {
      get { return area; }
      internal set { area = value; }
    }

    /// <summary>
    /// Blob's center of gravity point.
    /// </summary>
    /// 
    /// <remarks><para>The property keeps center of gravity point, which is calculated as
    /// mean value of X and Y coordinates of blob's points.</para></remarks>
    /// 
    public Point CenterOfGravity
    {
      get { return cog; }
      internal set { cog = value; }
    }

    /// <summary>
    /// Blob's mean color.
    /// </summary>
    /// 
    /// <remarks><para>The property keeps mean color of pixels comprising the blob.</para></remarks>
    /// 
    public Color ColorMean
    {
      get { return colorMean; }
      internal set { colorMean = value; }
    }

    /// <summary>
    /// Blob color's standard deviation.
    /// </summary>
    /// 
    /// <remarks><para>The property keeps standard deviation of pixels' colors comprising the blob.</para></remarks>
    /// 
    public Color ColorStdDev
    {
      get { return colorStdDev; }
      internal set { colorStdDev = value; }
    }

    public System.Drawing.Bitmap Image { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Blob"/> class.
    /// </summary>
    /// 
    /// <param name="id">Blob's ID in the original image.</param>
    /// <param name="rect">Blob's rectangle in the original image.</param>
    /// 
    /// <remarks><para>This constructor leaves <see cref="Image"/> property not initialized. The blob's
    /// image may be extracted later using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob, bool )"/>
    /// or <see cref="BlobCounterBase.ExtractBlobsImage( BitmapData, Blob, bool )"/> method.</para></remarks>
    /// 
    internal Blob(int id, Rect rect)
    {
      this.id = id;
      this.rect = rect;
    }

    // Copy constructur
    internal Blob(Blob source)
    {
      // copy everything except image
      id = source.id;
      rect = source.rect;
      cog = source.cog;
      area = source.area;
      colorMean = source.colorMean;
      colorStdDev = source.colorStdDev;
    }
  }
}
