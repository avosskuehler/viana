namespace VianaNET
{
  using System;
  using System.Diagnostics;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Windows.Forms;
  using DirectShowLib;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Interop;

  /// <summary>
  /// This is the main class for the DirectShow interop.
  /// It creates a graph that pushes video frames from a Video Input Device
  /// through the filter chain to a SampleGrabber, from which the
  /// frames can be catched and send into the processing tree of
  /// the application.
  /// </summary>
  public abstract class VideoBase : DependencyObject, IDisposable
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTANTS
    #endregion //CONSTANTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////
    #region FIELDS

    /// <summary>
    /// This points to a file mapping of the video frames.
    /// </summary>
    public IntPtr section { get; set; }

    /// <summary>
    /// Saves the bufferLength of the video stream
    /// </summary>
    protected int bufferLength;


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
    public IntPtr Map { get; set; }

    /// <summary>
    /// Saves the stride of the video stream
    /// </summary>
    public int Stride { get; set; }

    /// <summary>
    /// Saves the pixel size of the video stream.
    /// RGB24 = 3 bytes
    /// </summary>
    public int PixelSize { get; set; }

    public double NaturalVideoWidth
    {
      get { return (double)GetValue(NaturalVideoWidthProperty); }
      set { SetValue(NaturalVideoWidthProperty, value); }
    }

    public static readonly DependencyProperty NaturalVideoWidthProperty =
      DependencyProperty.Register(
      "NaturalVideoWidth",
      typeof(double),
      typeof(VideoBase),
      new UIPropertyMetadata(default(double)));

    public double NaturalVideoHeight
    {
      get { return (double)GetValue(NaturalVideoHeightProperty); }
      set { SetValue(NaturalVideoHeightProperty, value); }
    }

    public static readonly DependencyProperty NaturalVideoHeightProperty =
      DependencyProperty.Register(
      "NaturalVideoHeight",
      typeof(double),
      typeof(VideoBase),
      new UIPropertyMetadata(default(double)));

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

    public bool IsRunning
    {
      get { return (bool)GetValue(IsRunningProperty); }
      set { SetValue(IsRunningProperty, value); }
    }

    public static readonly DependencyProperty IsRunningProperty =
      DependencyProperty.Register(
      "IsRunning",
      typeof(bool),
      typeof(VideoBase),
      new UIPropertyMetadata(false));

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
    /// Time between frames in ms units.
    /// </summary>
    public double FrameTime
    {
      get { return (double)GetValue(FrameTimeProperty); }
      set { SetValue(FrameTimeProperty, value); }
    }

    public static readonly DependencyProperty FrameTimeProperty =
      DependencyProperty.Register(
      "FrameTime",
      typeof(double),
      typeof(VideoBase),
      new UIPropertyMetadata(default(double)));

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

    public virtual long MediaPositionInMS { get; set; }

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
      this.IsRunning = true;
    }

    /// <summary>
    /// Pause the capture graph. Running the graph takes up a lot of resources.  
    /// Pause it when it isn't needed.
    /// </summary>
    public virtual void Pause()
    {
      this.IsRunning = false;
    }

    public virtual void Stop()
    {
      this.IsRunning = false;
    }

    public abstract void Revert();

    /// <summary>
    /// Shut down capture.
    /// This is used to release all resources needed by the capture graph.
    /// </summary>
    public virtual void Dispose()
    {
      this.Stop();

      this.HasVideo = false;

      if (this.Map != IntPtr.Zero)
      {
        UnmapViewOfFile(this.Map);
        this.Map = IntPtr.Zero;
      }

      if (this.section != IntPtr.Zero)
      {
        CloseHandle(this.section);
        this.section = IntPtr.Zero;
      }
    }

    #endregion //PUBLICMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region OVERRIDES
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

    protected void OnVideoFrameChanged()
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

    protected void CreateMemoryMapping(int byteCountOfBitmap)
    {
      this.PixelSize = byteCountOfBitmap;

      this.Stride = (int)this.NaturalVideoWidth * this.PixelSize;

      this.bufferLength = (int)(this.NaturalVideoWidth * this.NaturalVideoHeight * this.PixelSize);

      // create memory section and map for the OpenCV Image.
      this.section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);
      this.Map = MapViewOfFile(this.section, 0xF001F, 0, 0, (uint)this.bufferLength);
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