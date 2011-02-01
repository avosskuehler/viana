namespace VianaNET
{
  using System;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media;
  using DirectShowLib;

  /// <summary>
  /// This is the main class for the DirectShow interop.
  /// It creates a graph that pushes video frames from a Video Input Device
  /// through the filter chain to a SampleGrabber, from which the
  /// frames can be catched and send into the processing tree of
  /// the application.
  /// </summary>
  public abstract class VideoBase : DependencyObject, ISampleGrabberCB, IDisposable
  {
    public enum PlayState
    {
      Stopped,
      Paused,
      Running,
      Init
    };

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS

    public const float NanoSecsToMilliSecs = 0.0001f;

    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    /// <summary>
    /// This points to a file mapping of the video frames.
    /// </summary>
    protected IntPtr originalSection;

    /// <summary>
    /// This points to a file mapping of the video frames.
    /// </summary>
    protected IntPtr processingSection;

    /// <summary>
    /// This points to the starting address of the mapped view of the video frame.
    /// </summary>
    protected IntPtr originalMapping;

    /// <summary>
    /// Saves the bufferLength of the video stream
    /// </summary>
    protected int bufferLength;

    /// <summary>
    /// This interface provides methods that enable an application to build a filter graph. 
    /// The Filter Graph Manager implements this interface.
    /// </summary>
    protected IFilterGraph2 filterGraph;

    ///// <summary>
    ///// The ICaptureGraphBuilder2 interface builds capture graphs and other custom filter graphs. 
    ///// </summary>
    //protected ICaptureGraphBuilder2 capGraph = null;

    /// <summary>
    /// The IMediaControl interface provides methods for controlling the 
    /// flow of data through the filter graph. It includes methods for running, pausing, and stopping the graph. 
    /// </summary>
    protected IMediaControl mediaControl;

    /// <summary>
    /// The ISampleGrabber interface is exposed by the Sample Grabber Filter.
    /// It enables an application to retrieve individual media samples as they move through the filter graph.
    /// </summary>
    protected ISampleGrabber sampleGrabber;

    //#if DEBUG
    /// <summary>
    /// Helps showing capture graph in GraphBuilder
    /// </summary>
    protected DsROTEntry rotEntry;
    //#endif

    /// <summary>
    /// This indicates if the frames of the capture callback should be skipped.
    /// </summary>
    protected bool skipFrameMode = false;

    protected int frameCounter;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the VideoBase class.
    /// Use capture device zero, default frame rate and size
    /// </summary>
    public VideoBase()
    {
    }

    ~VideoBase()
    {
      Dispose();
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS

    public event EventHandler VideoAvailable;
    public event EventHandler VideoFrameChanged;

    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// This points to the starting address of the mapped view of the video frame.
    /// </summary>
    public IntPtr ProcessingMapping { get; set; }

    /// <summary>
    /// Saves the stride of the video stream
    /// </summary>
    public int Stride { get; set; }

    /// <summary>
    /// Saves the pixel size of the video stream.
    /// RGB24 = 3 bytes
    /// </summary>
    public int PixelSize { get; set; }

    public double NaturalVideoWidth { get; set; }

    public double NaturalVideoHeight { get; set; }


    //public double NaturalVideoWidth
    //{
    //  get { return (double)GetValue(NaturalVideoWidthProperty); }
    //  set { SetValue(NaturalVideoWidthProperty, value); }
    //}

    //public static readonly DependencyProperty NaturalVideoWidthProperty =
    //  DependencyProperty.Register(
    //  "NaturalVideoWidth",
    //  typeof(double),
    //  typeof(VideoBase),
    //  new UIPropertyMetadata(default(double)));

    //public double NaturalVideoHeight
    //{
    //  get { return (double)GetValue(NaturalVideoHeightProperty); }
    //  set { SetValue(NaturalVideoHeightProperty, value); }
    //}

    //public static readonly DependencyProperty NaturalVideoHeightProperty =
    //  DependencyProperty.Register(
    //  "NaturalVideoHeight",
    //  typeof(double),
    //  typeof(VideoBase),
    //  new UIPropertyMetadata(default(double)));

    public ImageSource ImageSource
    {
      get { return (ImageSource)GetValue(ImageSourceProperty); }
      set { SetValue(ImageSourceProperty, value); }
    }

    public static readonly DependencyProperty ImageSourceProperty =
      DependencyProperty.Register(
      "ImageSource",
      typeof(ImageSource),
      typeof(VideoBase),
      new UIPropertyMetadata(null));

    public PlayState CurrentState
    {
      get { return (PlayState)GetValue(CurrentStateProperty); }
      set { SetValue(CurrentStateProperty, value); }
    }

    public static readonly DependencyProperty CurrentStateProperty =
      DependencyProperty.Register(
      "CurrentState",
      typeof(PlayState),
      typeof(VideoBase),
      new UIPropertyMetadata(PlayState.Stopped));

    public bool HasVideo
    {
      get { return (bool)GetValue(HasVideoProperty); }
      set { SetValue(HasVideoProperty, value); }
    }

    public static readonly DependencyProperty HasVideoProperty =
      DependencyProperty.Register(
      "HasVideo",
      typeof(bool),
      typeof(VideoBase),
      new UIPropertyMetadata(false));

    /// <summary>
    /// Time between frames in 100ns units.
    /// </summary>
    public long FrameTimeInNanoSeconds
    {
      get { return (long)GetValue(FrameTimeInNanoSecondsProperty); }
      set { SetValue(FrameTimeInNanoSecondsProperty, value); }
    }

    public static readonly DependencyProperty FrameTimeInNanoSecondsProperty =
      DependencyProperty.Register(
      "FrameTimeInNanoSeconds",
      typeof(long),
      typeof(VideoBase),
      new UIPropertyMetadata(default(long)));

    public int MediaPositionFrameIndex
    {
      get { return (int)GetValue(MediaPositionFrameIndexProperty); }
      set { SetValue(MediaPositionFrameIndexProperty, value); }
    }

    public static readonly DependencyProperty MediaPositionFrameIndexProperty =
      DependencyProperty.Register(
      "MediaPositionFrameIndex",
      typeof(int),
      typeof(VideoBase),
      new UIPropertyMetadata(default(int)));

    public virtual long MediaPositionInNanoSeconds { get; set; }

    //public long MediaPositionInMilliSeconds
    //{
    //  get { return (long)GetValue(MediaPositionInMilliSecondsProperty); }
    //  set { SetValue(MediaPositionInMilliSecondsProperty, value); }
    //}

    //public static readonly DependencyProperty MediaPositionInMilliSecondsProperty =
    //  DependencyProperty.Register(
    //  "MediaPositionInMilliSeconds",
    //  typeof(long),
    //  typeof(VideoBase),
    //  new UIPropertyMetadata(default(long)));

    public int FrameCount
    {
      get { return (int)GetValue(FrameCountProperty); }
      set { SetValue(FrameCountProperty, value); }
    }

    public static readonly DependencyProperty FrameCountProperty =
      DependencyProperty.Register(
      "FrameCount",
      typeof(int),
      typeof(VideoBase),
      new UIPropertyMetadata(default(int)));

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    /// <summary>
    /// Start the capture graph
    /// </summary>
    public virtual void Play()
    {
      if (!(this.CurrentState == PlayState.Running) && this.mediaControl != null)
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
    /// Pause the capture graph. Running the graph takes up a lot of resources.  
    /// Pause it when it isn't needed.
    /// </summary>
    public virtual void Pause()
    {
      if (this.mediaControl == null)
        return;

      //if ((this.CurrentState == PlayState.Running))
      {
        if (this.mediaControl.Pause() >= 0)
        {
          this.CurrentState = PlayState.Paused;
        }
      }

      //// Toggle play/pause behavior
      //if ((this.currentState == PlayState.Paused) || (this.currentState == PlayState.Stopped))
      //{
      //  if (this.mediaControl.Run() >= 0)
      //    this.currentState = PlayState.Running;
      //}
      //else
      //{
      //  if (this.mediaControl.Pause() >= 0)
      //    this.currentState = PlayState.Paused;
      //}
    }

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

    public virtual void Revert()
    {
      this.Stop();
      this.frameCounter = 0;
    }

    /// <summary>
    /// Shut down capture.
    /// This is used to release all resources needed by the capture graph.
    /// </summary>
    public virtual void Dispose()
    {

      this.Stop();

      //#if DEBUG
      if (this.rotEntry != null)
      {
        this.rotEntry.Dispose();
        this.rotEntry = null;
      }
      //#endif

      lock (this)
      {
        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (SendOrPostCallback)delegate
        {
          this.HasVideo = false;
          this.CurrentState = PlayState.Init;
          this.frameCounter = 0;
        }, null);


        if (this.originalMapping != IntPtr.Zero)
        {
          UnmapViewOfFile(this.originalMapping);
          this.originalMapping = IntPtr.Zero;
        }

        if (this.ProcessingMapping != IntPtr.Zero)
        {
          UnmapViewOfFile(this.ProcessingMapping);
          this.ProcessingMapping = IntPtr.Zero;
        }

        if (this.originalSection != IntPtr.Zero)
        {
          CloseHandle(this.originalSection);
          this.originalSection = IntPtr.Zero;
        }

        if (this.processingSection != IntPtr.Zero)
        {
          CloseHandle(this.processingSection);
          this.processingSection = IntPtr.Zero;
        }

        if (this.sampleGrabber != null)
        {
          Marshal.ReleaseComObject(this.sampleGrabber);
          this.sampleGrabber = null;
        }

        if (this.filterGraph != null)
        {
          Marshal.ReleaseComObject(this.filterGraph);
          this.filterGraph = null;
          this.mediaControl = null;
          this.HasVideo = false;
        }
      }

      //#if DEBUG
      // Double check to make sure we aren't releasing something
      // important.
      GC.Collect();
      //GC.WaitForPendingFinalizers();
      //#endif
    }

    public void RefreshProcessingMap()
    {
      CopyMemory(this.ProcessingMapping, this.originalMapping, this.bufferLength);
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES

    /// <summary> 
    /// The <see cref="ISampleGrabberCB.SampleCB{Double,IMediaSample}"/> sample callback method.
    /// NOT USED.
    /// </summary>
    /// <param name="sampleTime">Starting time of the sample, in seconds.</param>
    /// <param name="sample">Pointer to the IMediaSample interface of the sample.</param>
    /// <returns>Returns S_OK if successful, or an HRESULT error code otherwise.</returns>
    int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample sample)
    {
      Marshal.ReleaseComObject(sample);
      return 0;
    }

    /// <summary> 
    /// The <see cref="ISampleGrabberCB.BufferCB{Double,IntPtr, Int32}"/> buffer callback method.
    /// Gets called whenever a new frame arrives down the stream in the SampleGrabber.
    /// Updates the memory mapping of the OpenCV image and raises the 
    /// <see cref="FrameCaptureComplete"/> event.
    /// </summary>
    /// <param name="sampleTime">Starting time of the sample, in seconds.</param>
    /// <param name="buffer">Pointer to a buffer that contains the sample data.</param>
    /// <param name="bufferLength">Length of the buffer pointed to by pBuffer, in bytes.</param>
    /// <returns>Returns S_OK if successful, or an HRESULT error code otherwise.</returns>
    int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr buffer, int bufferLength)
    {
      ////long last = this.stopwatch.ElapsedMilliseconds;
      ////Console.Write("BufferCB called at: ");
      ////Console.Write(last);
      ////Console.WriteLine(" ms");
      //this.MediaPositionFrameIndex++;
      //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (SendOrPostCallback)delegate
      //{
      //  this.MediaPositionInMilliSeconds = (long)(sampleTime * 1000);
      this.frameCounter++;
      //}, null);

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
              CopyMemory(this.ProcessingMapping, buffer, bufferLength);

              Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
             {
               ((InteropBitmap)this.ImageSource).Invalidate();

               // Send new image to processing thread
               this.OnVideoFrameChanged();
             }, null);
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

    #endregion //OVERRIDES

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTHANDLER

    protected void OnVideoAvailable()
    {
      if (this.VideoAvailable != null)
      {
        this.VideoAvailable(this, EventArgs.Empty);
      }
    }

    protected virtual void OnVideoFrameChanged()
    {
      if (this.VideoFrameChanged != null)
      {
        this.VideoFrameChanged(this, EventArgs.Empty);
      }
    }

    #endregion //EVENTHANDLER

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region THREAD
    #endregion //THREAD

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////
    #region PRIVATEMETHODS

    /// <summary>
    /// Configure the sample grabber with default Video RGB24 mode.
    /// </summary>
    /// <param name="sampGrabber">The <see cref="ISampleGrabber"/> to be configured.</param>
    protected void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
    {
      AMMediaType media;
      int hr;

      // Set the media type to Video/RBG24
      media = new AMMediaType();
      media.majorType = MediaType.Video;
      media.subType = MediaSubType.RGB32;
      media.formatType = FormatType.VideoInfo;

      hr = this.sampleGrabber.SetMediaType(media);

      if (hr != 0)
      {
        ErrorLogger.WriteLine("Could not ConfigureSampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      DsUtils.FreeAMMediaType(media);
      media = null;

      // Configure the samplegrabber
      hr = this.sampleGrabber.SetCallback(this, 1);

      if (hr != 0)
      {
        ErrorLogger.WriteLine("Could not set callback method for sampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }
    }

    /// <summary>
    /// Saves the video properties of the SampleGrabber into member fields
    /// and creates a file mapping for the captured frames.
    /// </summary>
    /// <param name="sampGrabber">The <see cref="ISampleGrabber"/>
    /// from which to retreive the sample information.</param>
    protected void SaveSizeInfo(ISampleGrabber sampGrabber)
    {
      int hr;

      // Get the media type from the SampleGrabber
      AMMediaType media = new AMMediaType();
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
      VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
      this.NaturalVideoWidth = videoInfoHeader.BmiHeader.Width;
      this.NaturalVideoHeight = videoInfoHeader.BmiHeader.Height;

      this.CreateMemoryMapping(4);
      this.ImageSource = Imaging.CreateBitmapSourceFromMemorySection(
        this.originalSection,
        (int)this.NaturalVideoWidth,
        (int)this.NaturalVideoHeight,
        PixelFormats.Bgr32,
        this.Stride,
        0) as InteropBitmap;

      DsUtils.FreeAMMediaType(media);
      media = null;
    }

    protected void CreateMemoryMapping(int byteCountOfBitmap)
    {
      this.PixelSize = byteCountOfBitmap;

      this.Stride = (int)this.NaturalVideoWidth * this.PixelSize;

      this.bufferLength = (int)(this.NaturalVideoWidth * this.NaturalVideoHeight * this.PixelSize);

      // create memory section and map for the Image.
      this.originalSection = CreateFileMapping(
        new IntPtr(-1),
        IntPtr.Zero,
        0x04,
        0,
        (uint)this.bufferLength,
        null);

      // create memory section and map for the Image.
      this.processingSection = CreateFileMapping(
        new IntPtr(-1),
        IntPtr.Zero,
        0x04,
        0,
        (uint)this.bufferLength,
        null);

      this.originalMapping = MapViewOfFile(this.originalSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.ProcessingMapping = MapViewOfFile(this.processingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
    }

    /// <summary>
    /// Copies a block of memory from one location to another.
    /// </summary>
    /// <param name="destination">A pointer to the starting address of the copied block's destination.</param>
    /// <param name="source">A pointer to the starting address of the block of memory to copy</param>
    /// <param name="length">The size of the block of memory to copy, in bytes.</param>
    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
    protected static extern void CopyMemory(IntPtr destination, IntPtr source, int length);

    /// <summary>
    /// Creates or opens a named or unnamed file mapping object for a specified file.
    /// </summary>
    /// <param name="file">A handle to the file from which to create a file mapping object.</param>
    /// <param name="fileMappingAttributes">A pointer to a SECURITY_ATTRIBUTES structure that determines whether a returned handle can be inherited by child processes.</param>
    /// <param name="protect">The protection for the file view, when the file is mapped.</param>
    /// <param name="maximumSizeHigh">The high-order DWORD of the maximum size of the file mapping object.</param>
    /// <param name="maximumSizeLow">The low-order DWORD of the maximum size of the file mapping object.</param>
    /// <param name="name">The name of the file mapping object.</param>
    /// <returns>If the function succeeds, the return value is a handle to the file mapping object.
    /// If the object exists before the function call, the function returns a handle to the existing object
    /// (with its current size, not the specified size), and GetLastError returns ERROR_ALREADY_EXISTS. 
    /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr CreateFileMapping(IntPtr file, IntPtr fileMappingAttributes, uint protect, uint maximumSizeHigh, uint maximumSizeLow, string name);

    /// <summary>
    /// Maps a view of a file mapping into the address space of a calling process.
    /// </summary>
    /// <param name="fileMappingObject">A handle to a file mapping object. The CreateFileMapping and OpenFileMapping functions return this handle.</param>
    /// <param name="desiredAccess">The type of access to a file mapping object, which ensures the protection of the pages.</param>
    /// <param name="fileOffsetHigh">A high-order DWORD of the file offset where the view begins.</param>
    /// <param name="fileOffsetLow">A low-order DWORD of the file offset where the view is to begin. 
    /// The combination of the high and low offsets must specify an offset within the file mapping. 
    /// They must also match the memory allocation granularity of the system. That is, 
    /// the offset must be a multiple of the allocation granularity. </param>
    /// <param name="numberOfBytesToMap">The number of bytes of a file mapping to map to the view.</param>
    /// <returns>If the function succeeds, the return value is the starting address of the mapped view.
    /// If the function fails, the return value is NULL.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr MapViewOfFile(IntPtr fileMappingObject, uint desiredAccess, uint fileOffsetHigh, uint fileOffsetLow, uint numberOfBytesToMap);

    /// <summary>
    /// Unmaps a mapped view of a file from the calling process's address space.
    /// </summary>
    /// <param name="map">A pointer to the base address of the mapped view of a file that is to be unmapped. </param>
    /// <returns>If the function succeeds, the return value is nonzero, and all dirty pages within the specified range are written "lazily" to disk.
    /// If the function fails, the return value is zero. </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern bool UnmapViewOfFile(IntPtr map);

    /// <summary>
    /// Closes an open object handle.
    /// </summary>
    /// <param name="handle">A valid handle to an open object.</param>
    /// <returns>If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern bool CloseHandle(IntPtr handle);

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}