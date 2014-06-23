// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoBase.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
  using System.Windows.Threading;

  using AForge.Imaging;

  using DirectShowLib;

  using VianaNET.Application;
  using VianaNET.Logging;

  using PixelFormat = System.Drawing.Imaging.PixelFormat;

  /// <summary>
  ///   This is the main class for the DirectShow interop.
  ///   It creates a graph that pushes video frames from a Video Input Device
  ///   through the filter chain to a SampleGrabber, from which the
  ///   frames can be catched and send into the processing tree of
  ///   the application.
  /// </summary>
  public abstract class VideoBase : DependencyObject, ISampleGrabberCB, IDisposable
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
      "CurrentState", 
      typeof(PlayState), 
      typeof(VideoBase), 
      new UIPropertyMetadata(PlayState.Stopped));

    /// <summary>
    ///   The frame count property.
    /// </summary>
    public static readonly DependencyProperty FrameCountProperty = DependencyProperty.Register(
      "FrameCount", 
      typeof(int), 
      typeof(VideoBase), 
      new UIPropertyMetadata(default(int)));

    /// <summary>
    ///   The frame time in nano seconds property.
    /// </summary>
    public static readonly DependencyProperty FrameTimeInNanoSecondsProperty =
      DependencyProperty.Register(
        "FrameTimeInNanoSeconds", 
        typeof(long), 
        typeof(VideoBase), 
        new UIPropertyMetadata(default(long)));

    /// <summary>
    ///   The media position frame index property.
    /// </summary>
    public static readonly DependencyProperty MediaPositionFrameIndexProperty =
      DependencyProperty.Register(
        "MediaPositionFrameIndex", 
        typeof(int), 
        typeof(VideoBase), 
        new UIPropertyMetadata(default(int)));

    #endregion

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
    ///   Helps showing capture graph in GraphBuilder
    /// </summary>
    protected DsROTEntry rotEntry;

    /// <summary>
    ///   The ISampleGrabber interface is exposed by the Sample Grabber Filter.
    ///   It enables an application to retrieve individual media samples as they move through the filter graph.
    /// </summary>
    protected ISampleGrabber sampleGrabber;

    /// <summary>
    ///   This indicates if the frames of the capture callback should be skipped.
    /// </summary>
    protected bool skipFrameMode = false;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr colorProcessingSection;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr motionProcessingSection;

    /// <summary>
    ///   This points to the starting address of the mapped view of the video frame.
    /// </summary>
    private IntPtr originalMapping;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr originalSection;

    #endregion

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

    #region Public Properties

    /// <summary>
    ///   Gets or sets the starting address of the mapped view of the video frame.
    ///   Used for the color processing.
    /// </summary>
    public IntPtr ColorProcessingMapping { get; set; }

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
    ///   Gets or sets the time between frames in nanoseconds.
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
    ///   Gets or sets the starting address of the mapped view of the video frame.
    ///   Used for the motion processing.
    /// </summary>
    public IntPtr MotionProcessingMapping { get; set; }

    /// <summary>
    ///   Gets or sets the natural video height.
    /// </summary>
    public double NaturalVideoHeight { get; set; }

    /// <summary>
    ///   Gets or sets the natural video width.
    /// </summary>
    public double NaturalVideoWidth { get; set; }

    /// <summary>
    ///   Gets or sets the pixel size of the video stream.
    ///   RGB24 = 3 bytes
    /// </summary>
    public int PixelSize { get; set; }

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
    /// The copy processed data to processing map.
    /// </summary>
    public void CopyProcessedDataToProcessingMap()
    {
      CopyMemory(this.MotionProcessingMapping, this.UnmanagedImage.ImageData, this.bufferLength);
    }

    /// <summary>
    /// The copy processing map to unmanaged image.
    /// </summary>
    public void CopyProcessingMapToUnmanagedImage()
    {
      this.UnmanagedImage = new UnmanagedImage(
        this.ColorProcessingMapping, 
        (int)this.NaturalVideoWidth, 
        (int)this.NaturalVideoHeight, 
        this.Stride, 
        PixelFormat.Format32bppRgb);
      CopyMemory(this.UnmanagedImage.ImageData, this.ColorProcessingMapping, this.bufferLength);
    }

    /// <summary>
    ///   Shut down capture.
    ///   This is used to release all resources needed by the capture graph.
    /// </summary>
    public virtual void Dispose()
    {
      this.Stop();

      // #if DEBUG
      if (this.rotEntry != null)
      {
        this.rotEntry.Dispose();
        this.rotEntry = null;
      }

      // #endif
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

        if (this.sampleGrabber != null)
        {
          Marshal.ReleaseComObject(this.sampleGrabber);
          this.sampleGrabber = null;
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

    /// <summary>
    /// The update image sources.
    /// </summary>
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

    /// <summary>
    /// The update processed image source.
    /// </summary>
    public void UpdateProcessedImageSource()
    {
      if (Viana.Project.ProcessingData.IsUsingColorDetection)
      {
        // Update ColorProcessedVideoSource
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render, 
          (SendOrPostCallback)delegate { ((InteropBitmap)Video.Instance.ColorProcessedImageSource).Invalidate(); }, 
          null);
      }

      if (Viana.Project.ProcessingData.IsUsingMotionDetection)
      {
        // Update MotionProcessedVideoSource
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render, 
          (SendOrPostCallback)delegate { ((InteropBitmap)Video.Instance.MotionProcessedImageSource).Invalidate(); }, 
          null);
      }
    }

    #endregion

    #region Explicit Interface Methods

    /// <summary>
    /// The <see cref="ISampleGrabberCB.BufferCB{Double,IntPtr, Int32}"/> buffer callback method.
    ///   Gets called whenever a new frame arrives down the stream in the SampleGrabber.
    ///   Updates the memory mapping of the OpenCV image and raises the
    ///   <see cref="FrameCaptureComplete"/> event.
    /// </summary>
    /// <param name="sampleTime">
    /// Starting time of the sample, in seconds.
    /// </param>
    /// <param name="buffer">
    /// Pointer to a buffer that contains the sample data.
    /// </param>
    /// <param name="bufferLength">
    /// Length of the buffer pointed to by pBuffer, in bytes.
    /// </param>
    /// <returns>
    /// Returns S_OK if successful, or an HRESULT error code otherwise.
    /// </returns>
    int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr buffer, int bufferLength)
    {
      ////long last = this.stopwatch.ElapsedMilliseconds;
      ////Console.Write("BufferCB called at: ");
      ////Console.Write(last);
      ////Console.WriteLine(" ms");
      // this.MediaPositionFrameIndex++;
      // Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (SendOrPostCallback)delegate
      // {
      // this.MediaPositionInMilliSeconds = (long)(sampleTime * 1000);
      this.frameCounter++;

      // }, null);
      if (buffer != IntPtr.Zero)
      {
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
              CopyMemory(this.originalMapping, buffer, bufferLength);

              this.Dispatcher.BeginInvoke(
                DispatcherPriority.Render, 
                (SendOrPostCallback)delegate
                  {
                    ((InteropBitmap)Video.Instance.OriginalImageSource).Invalidate();

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

      return 0;
    }

    /// <summary>
    /// The <see cref="ISampleGrabberCB.SampleCB{Double,IMediaSample}"/> sample callback method.
    ///   NOT USED.
    /// </summary>
    /// <param name="sampleTime">
    /// Starting time of the sample, in seconds.
    /// </param>
    /// <param name="sample">
    /// Pointer to the IMediaSample interface of the sample.
    /// </param>
    /// <returns>
    /// Returns S_OK if successful, or an HRESULT error code otherwise.
    /// </returns>
    int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample sample)
    {
      Marshal.ReleaseComObject(sample);
      return 0;
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
    /// A pointer to a SECURITY_ATTRIBUTES structure that determines whether a returned handle can be inherited by child
    ///   processes.
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
    /// If the function succeeds, the return value is a handle to the file mapping object. If the object exists before the
    ///   function call, the function returns a handle to the existing object (with its current size, not the specified size),
    ///   and GetLastError returns ERROR_ALREADY_EXISTS. If the function fails, the return value is NULL. To get extended error
    ///   information, call GetLastError.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr CreateFileMapping(
      IntPtr file, 
      IntPtr fileMappingAttributes, 
      uint protect, 
      uint maximumSizeHigh, 
      uint maximumSizeLow, 
      string name);

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
    /// A low-order DWORD of the file offset where the view is to begin. The combination of the high and low offsets must
    ///   specify an offset within the file mapping. They must also match the memory allocation granularity of the system. That
    ///   is, the offset must be a multiple of the allocation granularity.
    /// </param>
    /// <param name="numberOfBytesToMap">
    /// The number of bytes of a file mapping to map to the view.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is the starting address of the mapped view. If the function fails, the
    ///   return value is NULL.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr MapViewOfFile(
      IntPtr fileMappingObject, 
      uint desiredAccess, 
      uint fileOffsetHigh, 
      uint fileOffsetLow, 
      uint numberOfBytesToMap);

    /// <summary>
    /// Unmaps a mapped view of a file from the calling process's address space.
    /// </summary>
    /// <param name="map">
    /// A pointer to the base address of the mapped view of a file that is to be unmapped.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero, and all dirty pages within the specified range are written
    ///   "lazily" to disk. If the function fails, the return value is zero.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern bool UnmapViewOfFile(IntPtr map);

    /// <summary>
    /// Configure the sample grabber with default Video RGB24 mode.
    /// </summary>
    /// <param name="sampGrabber">
    /// The <see cref="ISampleGrabber"/> to be configured.
    /// </param>
    protected void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
    {
      AMMediaType media;
      int hr;

      // Set the media type to Video/RBG24
      media = new AMMediaType();
      media.majorType = MediaType.Video;

      // media.subType=MediaSubType.UYVY;
      media.subType = MediaSubType.RGB32;
      media.formatType = FormatType.VideoInfo;

      hr = this.sampleGrabber.SetMediaType(media);

      if (hr != 0)
      {
        ErrorLogger.WriteLine(
          "Could not ConfigureSampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      DsUtils.FreeAMMediaType(media);
      media = null;

      // Configure the samplegrabber
      hr = this.sampleGrabber.SetCallback(this, 1);

      if (hr != 0)
      {
        ErrorLogger.WriteLine(
          "Could not set callback method for sampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }
    }

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
      this.colorProcessingSection = CreateFileMapping(
        new IntPtr(-1), 
        IntPtr.Zero, 
        0x04, 
        0, 
        (uint)this.bufferLength, 
        null);
      this.motionProcessingSection = CreateFileMapping(
        new IntPtr(-1), 
        IntPtr.Zero, 
        0x04, 
        0, 
        (uint)this.bufferLength, 
        null);

      this.originalMapping = MapViewOfFile(this.originalSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.ColorProcessingMapping = MapViewOfFile(this.colorProcessingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.MotionProcessingMapping = MapViewOfFile(this.motionProcessingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.UnmanagedImage = new UnmanagedImage(
        this.originalMapping, 
        (int)this.NaturalVideoWidth, 
        (int)this.NaturalVideoHeight, 
        this.Stride, 
        PixelFormat.Format32bppRgb);
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
    /// <param name="sampGrabber">
    /// The <see cref="ISampleGrabber"/> from which to retreive the sample information.
    /// </param>
    protected void SaveSizeInfo(ISampleGrabber sampGrabber)
    {
      int hr;

      // Get the media type from the SampleGrabber
      var media = new AMMediaType();
      hr = sampGrabber.GetConnectedMediaType(media);

      if (hr != 0)
      {
        ErrorLogger.WriteLine("Could not SaveSizeInfo in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
      {
        ErrorLogger.WriteLine("Error in Camera.Capture. Unknown Grabber Media Format");
      }

      // Grab the size info
      var videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
      this.NaturalVideoWidth = videoInfoHeader.BmiHeader.Width;
      this.NaturalVideoHeight = videoInfoHeader.BmiHeader.Height;

      this.CreateMemoryMapping(4);
      Video.Instance.OriginalImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.originalSection, 
          (int)this.NaturalVideoWidth, 
          (int)this.NaturalVideoHeight, 
          PixelFormats.Bgr32, 
          this.Stride, 
          0) as InteropBitmap;

      Video.Instance.ColorProcessedImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.colorProcessingSection, 
          (int)this.NaturalVideoWidth, 
          (int)this.NaturalVideoHeight, 
          PixelFormats.Bgr32, 
          this.Stride, 
          0) as InteropBitmap;

      Video.Instance.MotionProcessedImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.motionProcessingSection, 
          (int)this.NaturalVideoWidth, 
          (int)this.NaturalVideoHeight, 
          PixelFormats.Bgr32, 
          this.Stride, 
          0) as InteropBitmap;

      DsUtils.FreeAMMediaType(media);
      media = null;
    }

    #endregion
  }
}