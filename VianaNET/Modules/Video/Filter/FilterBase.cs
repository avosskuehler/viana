namespace VianaNET
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Media;

  public abstract class FilterBase
  {
    public const short A = 3;
    public const short B = 0;
    public const short G = 1;
    public const short R = 2;

    /// <summary>
    /// Width of processed image.
    /// </summary>
    public int ImageWidth { get; set; }

    /// <summary>
    /// Height of processed image.
    /// </summary>
    public int ImageHeight { get; set; }

    /// <summary>
    /// Width of processed image.
    /// </summary>
    public int ImageStride { get; set; }

    /// <summary>
    /// Height of processed image.
    /// </summary>
    public int ImagePixelSize { get; set; }


    public FilterBase()
    {
    }

    public abstract void ProcessInPlace(IntPtr image);
  }
}
