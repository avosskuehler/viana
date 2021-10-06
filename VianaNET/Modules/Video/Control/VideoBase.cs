// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoBase.cs" company="Freie Universität Berlin">
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
//   This is the main class for the DirectShow interop.
//   It creates a graph that pushes video frames from a Video Input Device
//   through the filter chain to a SampleGrabber, from which the
//   frames can be catched and send into the processing tree of
//   the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Threading;

  using AForge.Imaging;

  using DirectShowLib;
  using OpenCvSharp;
  using VianaNET.Application;
  using VianaNET.Logging;

  /// <summary>
  ///   This is the main class for the DirectShow interop.
  ///   It creates a graph that pushes video frames from a Video Input Device
  ///   through the filter chain to a SampleGrabber, from which the
  ///   frames can be catched and send into the processing tree of
  ///   the application.
  /// </summary>
  public abstract class VideoBase : DependencyObject, IDisposable
  {
    #region Constants

    /// <summary>
    ///   The nano secs to milli secs.
    /// </summary>
    public const float NanoSecsToMilliSecs = 0.0001f;

    #endregion

    #region Static Fields

    /// <summary>
    ///   The current state property.
    /// </summary>
    public static readonly DependencyProperty CurrentStateProperty = DependencyProperty.Register(
      "CurrentState", typeof(PlayState), typeof(VideoBase), new UIPropertyMetadata(PlayState.Stopped));

    /// <summary>
    ///   The frame count property.
    /// </summary>
    public static readonly DependencyProperty FrameCountProperty = DependencyProperty.Register(
      "FrameCount", typeof(int), typeof(VideoBase), new UIPropertyMetadata(default(int)));

    /// <summary>
    ///   The frame time in nano seconds property.
    /// </summary>
    public static readonly DependencyProperty FrameTimeInNanoSecondsProperty =
      DependencyProperty.Register(
        "FrameTimeInNanoSeconds", typeof(long), typeof(VideoBase), new UIPropertyMetadata(default(long)));

    /// <summary>
    ///   The media position frame index property.
    /// </summary>
    public static readonly DependencyProperty MediaPositionFrameIndexProperty =
      DependencyProperty.Register(
        "MediaPositionFrameIndex", typeof(int), typeof(VideoBase), new UIPropertyMetadata(default(int)));

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region Fields

    /// <summary>
    ///   Saves the bufferLength of the video stream
    /// </summary>
    protected int bufferLength;

    /// <summary>
    ///   This interface provides methods that enable an application to build a filter graph. 
    ///   The Filter Graph Manager implements this interface.
    /// </summary>
    protected IFilterGraph2 filterGraph;

    /// <summary>
    ///   The frame counter.
    /// </summary>
    protected int frameCounter;

    ///// <summary>
    ///// The ICaptureGraphBuilder2 interface builds capture graphs and other custom filter graphs. 
    ///// </summary>
    // protected ICaptureGraphBuilder2 capGraph = null;

    /// <summary>
    ///   The IMediaControl interface provides methods for controlling the 
    ///   flow of data through the filter graph. It includes methods for running, pausing, and stopping the graph.
    /// </summary>
    protected IMediaControl mediaControl;

    /// <summary>
    ///   This points to the starting address of the mapped view of the video frame.
    /// </summary>
    private IntPtr originalMapping;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr originalSection;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr colorProcessingSection;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr motionProcessingSection;

    /// <summary>
    ///   This indicates if the frames of the capture callback should be skipped.
    /// </summary>
    protected bool skipFrameMode = false;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    ///   Finalizes an instance of the <see cref="VideoBase" /> class.
    /// </summary>
    ~VideoBase()
    {
      // Console.WriteLine("VideoBase Destructor");
      this.Dispose();

      // Console.WriteLine("VideoBase Finished");
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Events

    /// <summary>
    ///   The video available.
    /// </summary>
    public event EventHandler VideoAvailable;

    /// <summary>
    ///   The video frame changed.
    /// </summary>
    public event EventHandler VideoFrameChanged;

    #endregion

    #region Enums

    /// <summary>
    ///   The play state.
    /// </summary>
    public enum PlayState
    {
      /// <summary>
      ///   The stopped.
      /// </summary>
      Stopped,

      /// <summary>
      ///   The paused.
      /// </summary>
      Paused,

      /// <summary>
      ///   The running.
      /// </summary>
      Running,

      /// <summary>
      ///   The init.
      /// </summary>
      Init
    };

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    ///   Gets or sets the current state.
    /// </summary>
    public PlayState CurrentState
    {
      get
      {
        return (PlayState)this.GetValue(CurrentStateProperty);
      }

      set
      {
        this.SetValue(CurrentStateProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the frame count.
    /// </summary>
    public int FrameCount
    {
      get
      {
        return (int)this.GetValue(FrameCountProperty);
      }

      set
      {
        this.SetValue(FrameCountProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the time between frames in nanoseconds.
    /// </summary>
    public long FrameTimeInNanoSeconds
    {
      get
      {
        return (long)this.GetValue(FrameTimeInNanoSecondsProperty);
      }

      set
      {
        this.SetValue(FrameTimeInNanoSecondsProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the media position frame index.
    /// </summary>
    public int MediaPositionFrameIndex
    {
      get
      {
        return (int)this.GetValue(MediaPositionFrameIndexProperty);
      }

      set
      {
        this.SetValue(MediaPositionFrameIndexProperty, value);
      }
    }

    /// <summary>
    ///   Gets or sets the media position in nano seconds.
    /// </summary>
    public virtual long MediaPositionInNanoSeconds { get; set; }

    /// <summary>
    ///   Gets or sets the natural video height.
    /// </summary>
    public double NaturalVideoHeight { get; set; }

    /// <summary>
    ///   Gets or sets the natural video width.
    /// </summary>
    public double NaturalVideoWidth { get; set; }

    /// <summary>
    ///  Gets or sets the pixel size of the video stream.
    ///   RGB24 = 3 bytes
    /// </summary>
    public int PixelSize { get; set; }

    /// <summary>
    ///  Gets or sets the starting address of the mapped view of the video frame.
    /// Used for the motion processing.
    /// </summary>
    public IntPtr MotionProcessingMapping { get; set; }

    /// <summary>
    ///  Gets or sets the starting address of the mapped view of the video frame.
    /// Used for the color processing.
    /// </summary>
    public IntPtr ColorProcessingMapping { get; set; }

    /// <summary>
    ///   Gets or sets the stride of the video stream
    /// </summary>
    public int Stride { get; set; }

    /// <summary>
    ///   Gets or sets the unmanaged image needed for the aforge motion detecton
    /// </summary>
    public UnmanagedImage UnmanagedImage { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Shut down capture.
    ///   This is used to release all resources needed by the capture graph.
    /// </summary>
    public virtual void Dispose()
    {
      this.Stop();

      lock (this)
      {
        this.Dispatcher.Invoke(
          DispatcherPriority.Normal,
          (SendOrPostCallback)delegate
            {
              Video.Instance.HasVideo = false;
              this.CurrentState = PlayState.Init;
              this.frameCounter = 0;
            },
          null);

        if (this.originalMapping != IntPtr.Zero)
        {
          UnmapViewOfFile(this.originalMapping);
          this.originalMapping = IntPtr.Zero;
        }

        if (this.ColorProcessingMapping != IntPtr.Zero)
        {
          UnmapViewOfFile(this.ColorProcessingMapping);
          this.ColorProcessingMapping = IntPtr.Zero;
        }

        if (this.originalSection != IntPtr.Zero)
        {
          CloseHandle(this.originalSection);
          this.originalSection = IntPtr.Zero;
        }

        if (this.colorProcessingSection != IntPtr.Zero)
        {
          CloseHandle(this.colorProcessingSection);
          this.colorProcessingSection = IntPtr.Zero;
        }

        if (this.motionProcessingSection != IntPtr.Zero)
        {
          CloseHandle(this.motionProcessingSection);
          this.motionProcessingSection = IntPtr.Zero;
        }


        if (this.UnmanagedImage != null)
        {
          this.UnmanagedImage.Dispose();
        }
 
        if (this.filterGraph != null)
        {
          Marshal.FinalReleaseComObject(this.filterGraph);
          this.filterGraph = null;
          this.mediaControl = null;
          Video.Instance.HasVideo = false;
        }
      }

      // #if DEBUG
      // Double check to make sure we aren't releasing something
      // important.
      GC.Collect();

      // GC.WaitForPendingFinalizers();
      // #endif
    }

    /// <summary>
    ///   Pause the capture graph. Running the graph takes up a lot of resources.  
    ///   Pause it when it isn't needed.
    /// </summary>
    public virtual void Pause()
    {
      if (this.mediaControl == null)
      {
        return;
      }
      {
        // if ((this.CurrentState == PlayState.Running))
        if (this.mediaControl.Pause() >= 0)
        {
          this.CurrentState = PlayState.Paused;
        }
      }

      //// Toggle play/pause behavior
      // if ((this.currentState == PlayState.Paused) || (this.currentState == PlayState.Stopped))
      // {
      // if (this.mediaControl.Run() >= 0)
      // this.currentState = PlayState.Running;
      // }
      // else
      // {
      // if (this.mediaControl.Pause() >= 0)
      // this.currentState = PlayState.Paused;
      // }
    }

    /// <summary>
    ///   Start the capture graph
    /// </summary>
    public virtual void Play()
    {
      if (this.CurrentState != PlayState.Running && this.mediaControl != null)
      {
        int hr = this.mediaControl.Run();

        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error while starting to play. Message: " + DsError.GetErrorText(hr));
        }
        else
        {
          this.CurrentState = PlayState.Running;
        }
      }
    }

    /// <summary>
    ///   The refresh processing map.
    /// </summary>
    public void RefreshProcessingMap()
    {
      CopyMemory(this.ColorProcessingMapping, this.originalMapping, this.bufferLength);
    }

    public void CopyProcessedDataToProcessingMap()
    {
      CopyMemory(this.MotionProcessingMapping, this.UnmanagedImage.ImageData, this.bufferLength);
    }

    public void UpdateProcessedImageSource()
    {
      if (Viana.Project.ProcessingData.IsUsingColorDetection && Video.Instance.ColorProcessedImageSource != null)
      {
        // Update ColorProcessedVideoSource
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render,
          (SendOrPostCallback)delegate { ((InteropBitmap)Video.Instance.ColorProcessedImageSource).Invalidate(); },
          null);
      }

      if (Viana.Project.ProcessingData.IsUsingMotionDetection && Video.Instance.MotionProcessedImageSource != null)
      {
        // Update MotionProcessedVideoSource
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render,
          (SendOrPostCallback)delegate { ((InteropBitmap)Video.Instance.MotionProcessedImageSource).Invalidate(); },
          null);
      }
    }

    public void UpdateImageSources()
    {
      this.Dispatcher.BeginInvoke(
        DispatcherPriority.Render,
        (SendOrPostCallback)delegate
          {
            if (Video.Instance.ColorProcessedImageSource != null && Video.Instance.MotionProcessedImageSource != null)
            {
              ((InteropBitmap)Video.Instance.ColorProcessedImageSource).Invalidate();
              ((InteropBitmap)Video.Instance.MotionProcessedImageSource).Invalidate();
            }
          },
        null);
    }

    public void CopyProcessingMapToUnmanagedImage()
    {
      this.UnmanagedImage = new UnmanagedImage(this.ColorProcessingMapping, (int)this.NaturalVideoWidth, (int)this.NaturalVideoHeight, this.Stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
      CopyMemory(this.UnmanagedImage.ImageData, this.ColorProcessingMapping, this.bufferLength);
    }

    /// <summary>
    ///   The revert.
    /// </summary>
    public virtual void Revert()
    {
      this.Stop();
      this.frameCounter = 0;
    }

    /// <summary>
    ///   The stop.
    /// </summary>
    public virtual void Stop()
    {
      try
      {
        if (this.mediaControl != null)
        {
          int hr = this.mediaControl.Stop();
          if (hr != 0)
          {
            ErrorLogger.WriteLine("Error while stopping. Message: " + DsError.GetErrorText(hr));
          }
          else
          {
            this.CurrentState = PlayState.Stopped;
          }
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region Explicit Interface Methods
    /// <summary>
    ///   Gets called whenever a new frame arrives
    ///   Updates the memory mapping of the OpenCV image and raises the 
    ///   <see cref="FrameCaptureComplete"/> event.
    /// </summary>
    public void NewFrameCallback(WriteableBitmap newFrame)
    {
      this.frameCounter++;

      // Do not skip frames here by default.
      // We skip the processing in the Tracker class if it is not fast enough
      if (!this.skipFrameMode)
      {
        try
        {
          // Check mapping if it is not already released and the buffer is running
          if (this.originalMapping != IntPtr.Zero)
          {
            // This is fast and lasts less than 1 millisecond.
            CopyMemory(this.originalMapping, newFrame.BackBuffer, bufferLength);

            this.Dispatcher.BeginInvoke(
              DispatcherPriority.Render,
              (SendOrPostCallback)delegate
              {

                  // Send new image to processing thread
                  this.OnVideoFrameChanged();
                },
              null);
          }
        }
        catch (ThreadInterruptedException e)
        {
          ErrorLogger.ProcessException(e, false);
        }
        catch (Exception we)
        {
          ErrorLogger.ProcessException(we, false);
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Closes an open object handle.
    /// </summary>
    /// <param name="handle">
    /// A valid handle to an open object. 
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern bool CloseHandle(IntPtr handle);

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Copies a block of memory from one location to another.
    /// </summary>
    /// <param name="destination">
    /// A pointer to the starting address of the copied block's destination. 
    /// </param>
    /// <param name="source">
    /// A pointer to the starting address of the block of memory to copy 
    /// </param>
    /// <param name="length">
    /// The size of the block of memory to copy, in bytes. 
    /// </param>
    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
    protected static extern void CopyMemory(IntPtr destination, IntPtr source, int length);

    /// <summary>
    /// Creates or opens a named or unnamed file mapping object for a specified file.
    /// </summary>
    /// <param name="file">
    /// A handle to the file from which to create a file mapping object. 
    /// </param>
    /// <param name="fileMappingAttributes">
    /// A pointer to a SECURITY_ATTRIBUTES structure that determines whether a returned handle can be inherited by child processes. 
    /// </param>
    /// <param name="protect">
    /// The protection for the file view, when the file is mapped. 
    /// </param>
    /// <param name="maximumSizeHigh">
    /// The high-order DWORD of the maximum size of the file mapping object. 
    /// </param>
    /// <param name="maximumSizeLow">
    /// The low-order DWORD of the maximum size of the file mapping object. 
    /// </param>
    /// <param name="name">
    /// The name of the file mapping object. 
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is a handle to the file mapping object. If the object exists before the function call, the function returns a handle to the existing object (with its current size, not the specified size), and GetLastError returns ERROR_ALREADY_EXISTS. If the function fails, the return value is NULL. To get extended error information, call GetLastError. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr CreateFileMapping(
      IntPtr file, IntPtr fileMappingAttributes, uint protect, uint maximumSizeHigh, uint maximumSizeLow, string name);

    /// <summary>
    /// Maps a view of a file mapping into the address space of a calling process.
    /// </summary>
    /// <param name="fileMappingObject">
    /// A handle to a file mapping object. The CreateFileMapping and OpenFileMapping functions return this handle. 
    /// </param>
    /// <param name="desiredAccess">
    /// The type of access to a file mapping object, which ensures the protection of the pages. 
    /// </param>
    /// <param name="fileOffsetHigh">
    /// A high-order DWORD of the file offset where the view begins. 
    /// </param>
    /// <param name="fileOffsetLow">
    /// A low-order DWORD of the file offset where the view is to begin. The combination of the high and low offsets must specify an offset within the file mapping. They must also match the memory allocation granularity of the system. That is, the offset must be a multiple of the allocation granularity. 
    /// </param>
    /// <param name="numberOfBytesToMap">
    /// The number of bytes of a file mapping to map to the view. 
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is the starting address of the mapped view. If the function fails, the return value is NULL. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr MapViewOfFile(
      IntPtr fileMappingObject, uint desiredAccess, uint fileOffsetHigh, uint fileOffsetLow, uint numberOfBytesToMap);

    /// <summary>
    /// Unmaps a mapped view of a file from the calling process's address space.
    /// </summary>
    /// <param name="map">
    /// A pointer to the base address of the mapped view of a file that is to be unmapped. 
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero, and all dirty pages within the specified range are written "lazily" to disk. If the function fails, the return value is zero. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern bool UnmapViewOfFile(IntPtr map);

    /// <summary>
    /// The create memory mapping.
    /// </summary>
    /// <param name="byteCountOfBitmap">
    /// The byte count of bitmap. 
    /// </param>
    protected void CreateMemoryMapping(int byteCountOfBitmap)
    {
      this.PixelSize = byteCountOfBitmap;

      this.Stride = (int)this.NaturalVideoWidth * this.PixelSize;

      this.bufferLength = (int)(this.NaturalVideoWidth * this.NaturalVideoHeight * this.PixelSize);

      // create memory sections and map for the image.
      this.originalSection = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);
      this.colorProcessingSection = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);
      this.motionProcessingSection = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);

      this.originalMapping = MapViewOfFile(this.originalSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.ColorProcessingMapping = MapViewOfFile(this.colorProcessingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.MotionProcessingMapping = MapViewOfFile(this.motionProcessingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.UnmanagedImage = new UnmanagedImage(this.originalMapping, (int)this.NaturalVideoWidth, (int)this.NaturalVideoHeight, this.Stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
    }

    /// <summary>
    ///   The on video available.
    /// </summary>
    protected void OnVideoAvailable()
    {
      if (this.VideoAvailable != null)
      {
        this.VideoAvailable(this, EventArgs.Empty);
      }
    }

    /// <summary>
    ///   The on video frame changed.
    /// </summary>
    protected virtual void OnVideoFrameChanged()
    {
      if (this.VideoFrameChanged != null)
      {
        this.VideoFrameChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Saves the video properties of the SampleGrabber into member fields
    ///   and creates a file mapping for the captured frames.
    /// </summary>
    /// <param name="opencvCapture">
    /// The <see cref="OpenCvSharp.VideoCapture"/> from which to retreive the sample information. 
    /// </param>
    public void SaveSizeInfo(VideoCapture opencvCapture)
    {
      // Grab the size info
      this.NaturalVideoWidth = opencvCapture.FrameWidth;
      this.NaturalVideoHeight = opencvCapture.FrameHeight;
      this.FrameTimeInNanoSeconds = (long)(10000000d / opencvCapture.Fps) + 1;

      this.CreateMemoryMapping(3);
      Video.Instance.OriginalImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.originalSection,
          (int)this.NaturalVideoWidth,
          (int)this.NaturalVideoHeight,
          PixelFormats.Bgr24,
          this.Stride,
          0) as InteropBitmap;

      Video.Instance.ColorProcessedImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.colorProcessingSection,
          (int)this.NaturalVideoWidth,
          (int)this.NaturalVideoHeight,
          PixelFormats.Bgr24,
          this.Stride,
          0) as InteropBitmap;

      Video.Instance.MotionProcessedImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.motionProcessingSection,
          (int)this.NaturalVideoWidth,
          (int)this.NaturalVideoHeight,
          PixelFormats.Bgr24,
          this.Stride,
          0) as InteropBitmap;
    }

    #endregion
  }
}