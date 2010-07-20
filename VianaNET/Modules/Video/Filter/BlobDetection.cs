// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com
//
// Copyright © Frank Nagl, 2009
// (adding the code for extracting blobs in original image's size)
// admin@franknagl.de
//

namespace VianaNET
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  /// Possible object orders.
  /// </summary>
  /// 
  /// <remarks>The enumeration defines possible sorting orders of objects, found by blob
  /// counting classes.</remarks>
  /// 
  public enum ObjectsOrder
  {
    /// <summary>
    /// Unsorted order (as it is collected by algorithm).
    /// </summary>
    None,

    /// <summary>
    /// Objects are sorted by size in descending order (bigger objects go first).
    /// Size is calculated as <b>Width * Height</b>.
    /// </summary>
    Size,

    /// <summary>
    /// Objects are sorted by area in descending order (bigger objects go first).
    /// </summary>
    Area,

    /// <summary>
    /// Objects are sorted by Y coordinate, then by X coordinate in ascending order
    /// (smaller coordinates go first).
    /// </summary>
    YX,

    /// <summary>
    /// Objects are sorted by X coordinate, then by Y coordinate in ascending order
    /// (smaller coordinates go first).
    /// </summary>
    XY
  }

  /// <summary>
  /// Base class for different blob counting algorithms.
  /// </summary>
  /// 
  /// <remarks><para>The class is abstract and serves as a base for different blob counting algorithms.
  /// Classes, which inherit from this base class, require to implement <see cref="BuildObjectsMap"/>
  /// method, which does actual building of object's label's map.</para>
  /// 
  /// <para>For blobs' searcing usually all inherited classes accept binary images, which are actually
  /// grayscale thresholded images. But the exact supported format should be checked in particular class,
  /// inheriting from the base class. For blobs' extraction the class supports grayscale (8 bpp indexed)
  /// and color images (24 and 32 bpp).</para>
  /// 
  /// <para>Sample usage:</para>
  /// <code>
  /// // create an instance of blob counter algorithm
  /// BlobCounterBase bc = new ...
  /// // set filtering options
  /// bc.FilterBlobs = true;
  /// bc.MinWidth  = 5;
  /// bc.MinHeight = 5;
  /// // process binary image
  /// bc.ProcessImage( image );
  /// Blob[] blobs = bc.GetObjects( image, false );
  /// // process blobs
  /// foreach ( Blob blob in blobs )
  /// {
  ///     // ...
  ///     // blob.Rectangle - blob's rectangle
  ///     // blob.Image - blob's image
  /// }
  /// </code>
  /// </remarks>
  /// 
  public class BlobDetectionFilter : FilterBase
  {
    // found blobs
    List<Blob> blobs = new List<Blob>();

    // objects' sort order
    private ObjectsOrder objectsOrder = ObjectsOrder.None;

    // filtering by size is required or nor
    private bool filterBlobs = false;

    // coupled size filtering or not
    private bool coupledSizeFiltering = false;

    // blobs' minimal and maximal size
    private int minWidth = 1;
    private int minHeight = 1;
    private int maxWidth = int.MaxValue;
    private int maxHeight = int.MaxValue;

    /// <summary>
    /// Objects count.
    /// </summary>
    protected int objectsCount;

    /// <summary>
    /// Objects' labels.
    /// </summary>
    protected int[] objectLabels;

    /// <summary>
    /// Objects count.
    /// </summary>
    /// 
    /// <remarks><para>Number of objects (blobs) found by <see cref="ProcessImage(Bitmap)"/> method.
    /// </para></remarks>
    /// 
    public int ObjectsCount
    {
      get { return objectsCount; }
    }

    /// <summary>
    /// Objects' labels.
    /// </summary>
    /// 
    /// <remarks>The array of <b>width</b> * <b>height</b> size, which holds
    /// labels for all objects. Background is represented with <b>0</b> value,
    /// but objects are represented with labels starting from <b>1</b>.</remarks>
    /// 
    public int[] ObjectLabels
    {
      get { return objectLabels; }
    }

    /// <summary>
    /// Objects sort order.
    /// </summary>
    /// 
    /// <remarks><para>The property specifies objects' sort order, which are provided
    /// by <see cref="GetObjectsRectangles"/>, <see cref="GetObjectsInformation"/>, etc.
    /// </para></remarks>
    /// 
    public ObjectsOrder ObjectsOrder
    {
      get { return objectsOrder; }
      set { objectsOrder = value; }
    }

    /// <summary>
    /// Specifies if blobs should be filtered.
    /// </summary>
    /// 
    /// <remarks><para>If the property is equal to <b>false</b>, then there is no any additional
    /// post processing after image was processed. If the property is set to <b>true</b>, then
    /// blobs filtering is done right after image processing routine. Blobs are filtered according
    /// to dimensions specified in <see cref="MinWidth"/>, <see cref="MinHeight"/>, <see cref="MaxWidth"/>
    /// and <see cref="MaxHeight"/> properties.</para>
    /// 
    /// <para>Default value is set to <see langword="false"/>.</para></remarks>
    /// 
    public bool FilterBlobs
    {
      get { return filterBlobs; }
      set { filterBlobs = value; }
    }

    /// <summary>
    /// Specifies if size filetering should be coupled or not.
    /// </summary>
    /// 
    /// <remarks><para>In uncoupled filtering mode, objects are filtered out in the case if
    /// their width is smaller than <see cref="MinWidth"/> or height is smaller than 
    /// <see cref="MinHeight"/>. But in coupled filtering mode, objects are filtered out in
    /// the case if their width is smaller than <see cref="MinWidth"/> <b>and</b> height is
    /// smaller than <see cref="MinHeight"/>. In both modes the idea with filtering by objects'
    /// maximum size is the same as filtering by objects' minimum size.</para>
    /// 
    /// <para>Default value is set to <see langword="false"/>, what means uncoupled filtering by size.</para>
    /// </remarks>
    /// 
    public bool CoupledSizeFiltering
    {
      get { return coupledSizeFiltering; }
      set { coupledSizeFiltering = value; }
    }

    /// <summary>
    /// Minimum allowed width of blob.
    /// </summary>
    /// 
    /// <remarks><para>The property specifies minimum object's width acceptable by blob counting
    /// routine and has power only when <see cref="FilterBlobs"/> property is set to
    /// <see langword="true"/>.</para>
    /// 
    /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
    /// </remarks>
    /// 
    public int MinWidth
    {
      get { return minWidth; }
      set { minWidth = value; }
    }

    /// <summary>
    /// Minimum allowed height of blob.
    /// </summary>
    /// 
    /// <remarks><para>The property specifies minimum object's height acceptable by blob counting
    /// routine and has power only when <see cref="FilterBlobs"/> property is set to
    /// <see langword="true"/>.</para>
    /// 
    /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
    /// </remarks>
    /// 
    public int MinHeight
    {
      get { return minHeight; }
      set { minHeight = value; }
    }

    /// <summary>
    /// Maximum allowed width of blob.
    /// </summary>
    /// 
    /// <remarks><para>The property specifies maximum object's width acceptable by blob counting
    /// routine and has power only when <see cref="FilterBlobs"/> property is set to
    /// <see langword="true"/>.</para>
    /// 
    /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
    /// </remarks>
    /// 
    public int MaxWidth
    {
      get { return maxWidth; }
      set { maxWidth = value; }
    }

    /// <summary>
    /// Maximum allowed height of blob.
    /// </summary>
    /// 
    /// <remarks><para>The property specifies maximum object's height acceptable by blob counting
    /// routine and has power only when <see cref="FilterBlobs"/> property is set to
    /// <see langword="true"/>.</para>
    /// 
    /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
    /// </remarks>
    /// 
    public int MaxHeight
    {
      get { return maxHeight; }
      set { maxHeight = value; }
    }

    // found blobs
    public List<Blob> Blobs
    {
      get { return blobs; }
      set { blobs = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobCounterBase"/> class.
    /// </summary>
    /// 
    /// <remarks>Creates new instance of the <see cref="BlobCounterBase"/> class with
    /// an empty objects map. Before using methods, which provide information about blobs
    /// or extract them, the <see cref="ProcessImage(Bitmap)"/>,
    /// <see cref="ProcessImage(BitmapData)"/> or <see cref="ProcessImage(UnmanagedImage)"/>
    /// method should be called to collect objects map.</remarks>
    /// 
    public BlobDetectionFilter()
      : base()
    {
    }

    /// <summary>
    /// Build object map from raw image data.
    /// </summary>
    /// 
    /// <param name="image">Source unmanaged binary image data.</param>
    /// 
    /// <remarks><para>Processes the image and builds objects map, which is used later to extracts blobs.</para></remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    /// 
    public override void ProcessInPlace(IntPtr image)
    {
      // do actual objects map building
      BuildObjectsMap(image);

      // collect information about blobs
      CollectObjectsInfo(image);

      // filter blobs by size if required
      if (filterBlobs)
      {
        // labels remapping array
        int[] labelsMap = new int[objectsCount + 1];
        for (int i = 1; i <= objectsCount; i++)
        {
          labelsMap[i] = i;
        }

        // check dimension of all objects and filter them
        int objectsToRemove = 0;

        for (int i = objectsCount - 1; i >= 0; i--)
        {
          double blobWidth = blobs[i].Rectangle.Width;
          double blobHeight = blobs[i].Rectangle.Height;

          if (coupledSizeFiltering == false)
          {
            // uncoupled filtering
            if (
                (blobWidth < minWidth) || (blobHeight < minHeight) ||
                (blobWidth > maxWidth) || (blobHeight > maxHeight))
            {
              labelsMap[i + 1] = 0;
              objectsToRemove++;
              blobs.RemoveAt(i);
            }
          }
          else
          {
            // coupled filtering
            if (
                ((blobWidth < minWidth) && (blobHeight < minHeight)) ||
                ((blobWidth > maxWidth) && (blobHeight > maxHeight)))
            {
              labelsMap[i + 1] = 0;
              objectsToRemove++;
              blobs.RemoveAt(i);
            }
          }
        }

        // update labels remapping array
        int label = 0;
        for (int i = 1; i <= objectsCount; i++)
        {
          if (labelsMap[i] != 0)
          {
            label++;
            // update remapping array
            labelsMap[i] = label;
          }
        }

        // repair object labels
        for (int i = 0, n = objectLabels.Length; i < n; i++)
        {
          objectLabels[i] = labelsMap[objectLabels[i]];
        }

        objectsCount -= objectsToRemove;

        // repair IDs
        for (int i = 0, n = blobs.Count; i < n; i++)
        {
          blobs[i].ID = i + 1;
        }
      }

      // do we need to sort the list?
      if (objectsOrder != ObjectsOrder.None)
      {
        blobs.Sort(new BlobsSorter(objectsOrder));
      }
    }

    public Blob[] GetObjects(IntPtr image)
    {
      // check if objects map was collected
      if (objectLabels == null)
        throw new ApplicationException("Image should be processed before to collect objects map.");

      Blob[] objects = new Blob[objectsCount];

      // create each image
      for (int k = 0; k < objectsCount; k++)
      {
        int objectWidth = (int)blobs[k].Rectangle.Width;
        int objectHeight = (int)blobs[k].Rectangle.Height;

        int blobImageWidth = objectWidth;
        int blobImageHeight = objectHeight;

        int xmin = (int)blobs[k].Rectangle.X;
        int xmax = xmin + objectWidth - 1;
        int ymin = (int)blobs[k].Rectangle.Y;
        int ymax = ymin + objectHeight - 1;

        int label = blobs[k].ID;

        // create new image
        System.Drawing.Bitmap dstImg = new System.Drawing.Bitmap(blobImageWidth, blobImageHeight,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        // lock destination bitmap data
        System.Drawing.Imaging.BitmapData dstData = dstImg.LockBits(
            new System.Drawing.Rectangle(0, 0, blobImageWidth, blobImageHeight),
            System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        // copy image
        unsafe
        {
          byte* src = (byte*)image + ymin * this.ImageStride + xmin * this.ImagePixelSize;
          byte* dst = (byte*)dstData.Scan0.ToPointer();
          int p = ymin * this.ImageWidth + xmin;

          int srcOffset = this.ImageStride - objectWidth * this.ImagePixelSize;
          int dstOffset = dstData.Stride - objectWidth * this.ImagePixelSize;
          int labelsOffset = this.ImageWidth - objectWidth;

          // for each line
          for (int y = ymin; y <= ymax; y++)
          {
            // copy each pixel
            for (int x = xmin; x <= xmax; x++, p++, dst += this.ImagePixelSize, src += this.ImagePixelSize)
            {
              if (objectLabels[p] == label)
              {
                // copy pixel
                *dst = *src;

                if (this.ImagePixelSize > 1)
                {
                  dst[1] = src[1];
                  dst[2] = src[2];

                  if (this.ImagePixelSize > 3)
                  {
                    dst[3] = src[3];
                  }
                }
              }
            }
            src += srcOffset;
            dst += dstOffset;
            p += labelsOffset;
          }
        }
        // unlock destination image
        dstImg.UnlockBits(dstData);

        objects[k] = new Blob(blobs[k]);
        objects[k].Image = dstImg;
      }

      return objects;
    }


    /// <summary>
    /// Actual objects map building.
    /// </summary>
    /// 
    /// <param name="image">Unmanaged image to process.</param>
    /// 
    /// <remarks>The method supports 8 bpp indexed grayscale images and 24/32 bpp color images.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    /// 
    protected void BuildObjectsMap(IntPtr image)
    {
      // we don't want one pixel width images
      if (ImageWidth <= 1)
        throw new ArgumentException("Too small image.");

      // allocate labels array
      objectLabels = new int[ImageWidth * ImageHeight];

      // initial labels count
      int labelsCount = 0;

      // create map
      int maxObjects = ((ImageWidth / 2) + 1) * ((ImageHeight / 2) + 1) + 1;
      int[] map = new int[maxObjects];

      // initially map all labels to themself
      for (int i = 0; i < maxObjects; i++)
      {
        map[i] = i;
      }

      // do the job
      unsafe
      {
        byte* src = (byte*)image;
        int p = 0;

        // color images
        int offset = this.ImageStride - ImageWidth * this.ImagePixelSize;

        int strideM1 = this.ImageStride - this.ImagePixelSize;
        int strideP1 = this.ImageStride + this.ImagePixelSize;

        // 1 - for pixels of the first row
        if ((src[R] | src[G] | src[B]) != 0)
        {
          objectLabels[p] = ++labelsCount;
        }
        src += this.ImagePixelSize;
        ++p;

        // process the rest of the first row
        for (int x = 1; x < ImageWidth; x++, src += this.ImagePixelSize, p++)
        {
          // check if we need to label current pixel
          if ((src[R] | src[G] | src[B]) != 0)
          {
            // check if the previous pixel already was labeled
            if ((src[R - this.ImagePixelSize] | src[G - this.ImagePixelSize] | src[B - this.ImagePixelSize]) != 0)
            {
              // label current pixel, as the previous
              objectLabels[p] = objectLabels[p - 1];
            }
            else
            {
              // create new label
              objectLabels[p] = ++labelsCount;
            }
          }
        }
        src += offset;

        // 2 - for other rows
        // for each row
        for (int y = 1; y < ImageHeight; y++)
        {
          // for the first pixel of the row, we need to check
          // only upper and upper-right pixels
          if ((src[R] | src[G] | src[B]) != 0)
          {
            // check surrounding pixels
            if ((src[R - this.ImageStride] | src[G - this.ImageStride] | src[B - this.ImageStride]) != 0)
            {
              // label current pixel, as the above
              objectLabels[p] = objectLabels[p - ImageWidth];
            }
            else if ((src[R - strideM1] |
                        src[G - strideM1] |
                        src[B - strideM1]) != 0)
            {
              // label current pixel, as the above right
              objectLabels[p] = objectLabels[p + 1 - ImageWidth];
            }
            else
            {
              // create new label
              objectLabels[p] = ++labelsCount;
            }
          }
          src += this.ImagePixelSize;
          ++p;

          // check left pixel and three upper pixels for the rest of pixels
          for (int x = 1; x < ImageWidth - 1; x++, src += this.ImagePixelSize, p++)
          {
            if ((src[R] | src[G] | src[B]) != 0)
            {
              // check surrounding pixels
              if ((src[R - this.ImagePixelSize] | src[G - this.ImagePixelSize] | src[B - this.ImagePixelSize]) != 0)
              {
                // label current pixel, as the left
                objectLabels[p] = objectLabels[p - 1];
              }
              else if ((src[R - strideP1] |
                          src[G - strideP1] |
                          src[B - strideP1]) != 0)
              {
                // label current pixel, as the above left
                objectLabels[p] = objectLabels[p - 1 - ImageWidth];
              }
              else if ((src[R - this.ImageStride] | src[G - this.ImageStride] | src[B - this.ImageStride]) != 0)
              {
                // label current pixel, as the above
                objectLabels[p] = objectLabels[p - ImageWidth];
              }

              if ((src[R - strideM1] |
                     src[G - strideM1] |
                     src[B - strideM1]) != 0)
              {
                if (objectLabels[p] == 0)
                {
                  // label current pixel, as the above right
                  objectLabels[p] = objectLabels[p + 1 - ImageWidth];
                }
                else
                {
                  int l1 = objectLabels[p];
                  int l2 = objectLabels[p + 1 - ImageWidth];

                  if ((l1 != l2) && (map[l1] != map[l2]))
                  {
                    // merge
                    if (map[l1] == l1)
                    {
                      // map left value to the right
                      map[l1] = map[l2];
                    }
                    else if (map[l2] == l2)
                    {
                      // map right value to the left
                      map[l2] = map[l1];
                    }
                    else
                    {
                      // both values already mapped
                      map[map[l1]] = map[l2];
                      map[l1] = map[l2];
                    }

                    // reindex
                    for (int i = 1; i <= labelsCount; i++)
                    {
                      if (map[i] != i)
                      {
                        // reindex
                        int j = map[i];
                        while (j != map[j])
                        {
                          j = map[j];
                        }
                        map[i] = j;
                      }
                    }
                  }
                }
              }

              // label the object if it is not yet
              if (objectLabels[p] == 0)
              {
                // create new label
                objectLabels[p] = ++labelsCount;
              }
            }
          }

          // for the last pixel of the row, we need to check
          // only upper and upper-left pixels
          if ((src[R] | src[G] | src[B]) != 0)
          {
            // check surrounding pixels
            if ((src[R - this.ImagePixelSize] | src[G - this.ImagePixelSize] | src[B - this.ImagePixelSize]) != 0)
            {
              // label current pixel, as the left
              objectLabels[p] = objectLabels[p - 1];
            }
            else if ((src[R - strideP1] |
                        src[G - strideP1] |
                        src[B - strideP1]) != 0)
            {
              // label current pixel, as the above left
              objectLabels[p] = objectLabels[p - 1 - ImageWidth];
            }
            else if ((src[R - this.ImageStride] | src[G - this.ImageStride] | src[B - this.ImageStride]) != 0)
            {
              // label current pixel, as the above
              objectLabels[p] = objectLabels[p - ImageWidth];
            }
            else
            {
              // create new label
              objectLabels[p] = ++labelsCount;
            }
          }
          src += this.ImagePixelSize;
          ++p;

          src += offset;
        }
      }

      //for (int i = 0; i < objectLabels.Length; i++)
      //{
      //  int lab = objectLabels[i];
      //  if (lab > 0)
      //  {
      //    bool stop = true;
      //  }
      //}
      // allocate remapping array
      int[] reMap = new int[map.Length];

      // count objects and prepare remapping array
      objectsCount = 0;
      for (int i = 1; i <= labelsCount; i++)
      {
        if (map[i] == i)
        {
          // increase objects count
          reMap[i] = ++objectsCount;
        }
      }
      // second pass to complete remapping
      for (int i = 1; i <= labelsCount; i++)
      {
        if (map[i] != i)
        {
          reMap[i] = reMap[map[i]];
        }
      }

      // repair object labels
      for (int i = 0, n = objectLabels.Length; i < n; i++)
      {
        objectLabels[i] = reMap[objectLabels[i]];
      }
    }

    // Collect objects' rectangles
    private unsafe void CollectObjectsInfo(IntPtr image)
    {
      int i = 0, label;

      // create object coordinates arrays
      int[] x1 = new int[objectsCount + 1];
      int[] y1 = new int[objectsCount + 1];
      int[] x2 = new int[objectsCount + 1];
      int[] y2 = new int[objectsCount + 1];

      int[] area = new int[objectsCount + 1];
      long[] xc = new long[objectsCount + 1];
      long[] yc = new long[objectsCount + 1];

      long[] meanR = new long[objectsCount + 1];
      long[] meanG = new long[objectsCount + 1];
      long[] meanB = new long[objectsCount + 1];

      long[] stdDevR = new long[objectsCount + 1];
      long[] stdDevG = new long[objectsCount + 1];
      long[] stdDevB = new long[objectsCount + 1];

      for (int j = 1; j <= objectsCount; j++)
      {
        x1[j] = ImageWidth;
        y1[j] = ImageHeight;
      }

      byte* src = (byte*)image;

      // color images
      int offset = this.ImageStride - ImageWidth * this.ImagePixelSize;
      byte r, g, b; // RGB value

      // walk through labels array
      for (int y = 0; y < ImageHeight; y++)
      {
        for (int x = 0; x < ImageWidth; x++, i++, src += this.ImagePixelSize)
        {
          // get current label
          label = objectLabels[i];

          // skip unlabeled pixels
          if (label == 0)
            continue;

          // check and update all coordinates

          if (x < x1[label])
          {
            x1[label] = x;
          }
          if (x > x2[label])
          {
            x2[label] = x;
          }
          if (y < y1[label])
          {
            y1[label] = y;
          }
          if (y > y2[label])
          {
            y2[label] = y;
          }

          area[label]++;
          xc[label] += x;
          yc[label] += y;

          r = src[R];
          g = src[G];
          b = src[B];

          meanR[label] += r;
          meanG[label] += g;
          meanB[label] += b;

          stdDevR[label] += r * r;
          stdDevG[label] += g * g;
          stdDevB[label] += b * b;
        }

        src += offset;
      }

      // create blobs
      blobs.Clear();

      for (int j = 1; j <= objectsCount; j++)
      {
        int blobArea = area[j];

        Blob blob = new Blob(j, new Rect(x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1));
        blob.Area = blobArea;
        blob.CenterOfGravity = new Point((int)(xc[j] / blobArea), (int)(yc[j] / blobArea));
        blob.ColorMean = Color.FromArgb(255, (byte)(meanR[j] / blobArea), (byte)(meanG[j] / blobArea), (byte)(meanB[j] / blobArea));
        blob.ColorStdDev = Color.FromArgb(255,
            (byte)(Math.Sqrt(stdDevR[j] / blobArea - blob.ColorMean.R * blob.ColorMean.R)),
            (byte)(Math.Sqrt(stdDevG[j] / blobArea - blob.ColorMean.G * blob.ColorMean.G)),
            (byte)(Math.Sqrt(stdDevB[j] / blobArea - blob.ColorMean.B * blob.ColorMean.B)));

        blobs.Add(blob);
      }
    }

    // Rectangles' and blobs' sorter
    private class BlobsSorter : System.Collections.Generic.IComparer<Blob>
    {
      private ObjectsOrder order;

      public BlobsSorter(ObjectsOrder order)
      {
        this.order = order;
      }

      public int Compare(Blob a, Blob b)
      {
        Rect aRect = a.Rectangle;
        Rect bRect = b.Rectangle;

        switch (order)
        {
          case ObjectsOrder.Size: // sort by size

            // the order is changed to descending
            return (int)(bRect.Width * bRect.Height - aRect.Width * aRect.Height);

          case ObjectsOrder.Area: // sort by area
            return b.Area - a.Area;

          case ObjectsOrder.YX:   // YX order

            return (int)((aRect.Y * 100000 + aRect.X) - (bRect.Y * 100000 + bRect.X));

          case ObjectsOrder.XY:   // XY order

            return (int)((aRect.X * 100000 + aRect.Y) - (bRect.X * 100000 + bRect.Y));
        }
        return 0;
      }
    }
  }
}
