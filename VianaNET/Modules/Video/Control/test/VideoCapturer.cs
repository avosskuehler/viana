namespace VianaNET
{
  using System;
  using System.Diagnostics;
  using System.Runtime.InteropServices;
  using System.Threading;
  using DirectShowLib;
  using System.Windows.Interop;
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  /// This is the main class for the DirectShow interop.
  /// It creates a graph that pushes video frames from a Video Input Device
  /// through the filter chain to a SampleGrabber, from which the
  /// frames can be catched and send into the processing tree of
  /// the application.
  /// </summary>
  public class VideoCapturer : DependencyObject, ISampleGrabberCB, IDisposable
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

    private static VideoCapturer instance;

    /// <summary>
    /// This indicates if the graph is running.
    /// </summary>
    private bool isRunning = false;

    /// <summary>
    /// This indicates if the frames of the capture callback should be skipped.
    /// </summary>
    private bool skipFrameMode = false;

    /// <summary>
    /// The IAMVideoControl interface controls certain video capture operations 
    /// such as enumerating available frame rates and image orientation.
    /// </summary>
    private IAMVideoControl videoControl = null;

    /// <summary>
    /// The IAMCameraControl interface controls camera settings such as zoom, 
    /// pan, aperture adjustment, or shutter speed. To obtain this interface, 
    /// query the filter that controls the camera. 
    /// </summary>
    private IAMCameraControl cameraControl = null;

    /// <summary>
    /// The IAMVideoProcAmp interface adjusts the qualities of an incoming 
    /// video signal, such as brightness, contrast, hue, saturation, gamma, and sharpness.
    /// </summary>
    private IAMVideoProcAmp videoProcAmp = null;

    /// <summary>
    /// The IAMStreamConfig interface sets the output format on certain capture 
    /// and compression filters, for both audio and video. Applications can use 
    /// this interface to set format properties, such as the output dimensions and 
    /// frame rate (for video) or the sample rate and number of channels (for audio).
    /// </summary>
    private IAMStreamConfig videoStreamConfig = null;

    /// <summary>
    /// This interface provides methods that enable an application to build a filter graph. 
    /// The Filter Graph Manager implements this interface.
    /// </summary>
    private IFilterGraph2 graphBuilder = null;

    /// <summary>
    /// The IMediaControl interface provides methods for controlling the 
    /// flow of data through the filter graph. It includes methods for running, pausing, and stopping the graph. 
    /// </summary>
    private IMediaControl mediaControl = null;

    /// <summary>
    /// The ISampleGrabber interface is exposed by the Sample Grabber Filter.
    /// It enables an application to retrieve individual media samples as they move through the filter graph.
    /// </summary>
    private ISampleGrabber sampGrabber = null;

    /// <summary>
    /// This member holds the <see cref="IBaseFilter"/> of the capture device filter.
    /// </summary>
    private IBaseFilter capFilter = null;

    /// <summary>
    /// The ICaptureGraphBuilder2 interface builds capture graphs and other custom filter graphs. 
    /// </summary>
    private ICaptureGraphBuilder2 capGraph = null;

    /// <summary>
    /// Saves the width of the video stream
    /// </summary>
    private int videoWidth;

    /// <summary>
    /// Saves the height of the video stream
    /// </summary>
    private int videoHeight;

    /// <summary>
    /// Saves the stride of the video stream
    /// </summary>
    private int stride;

    /// <summary>
    /// Saves the bufferLength of the video stream
    /// </summary>
    private int bufferLength;

    /// <summary>
    /// Saves the framerate of the video stream
    /// </summary>
    private int fps = 0;

    /// <summary>
    /// Indicates whether this class has a valid rendered graph.
    /// </summary>
    private bool hasValidGraph;

    /// <summary>
    /// This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr section = IntPtr.Zero;

    /// <summary>
    /// This points to the starting address of the mapped view of the video frame.
    /// </summary>
    private IntPtr map = IntPtr.Zero;

    /// <summary>
    /// This member contains a copy of the captured video frames.
    /// </summary>
    private System.Drawing.Bitmap videoImage;

    public InteropBitmap BitmapSource
    {
      get { return (InteropBitmap)GetValue(BitmapSourceProperty); }
      private set { SetValue(BitmapSourcePropertyKey, value); }
    }

    private static readonly DependencyPropertyKey BitmapSourcePropertyKey =
        DependencyProperty.RegisterReadOnly(
        "BitmapSource", 
        typeof(InteropBitmap), 
        typeof(VideoCapturer), 
        new UIPropertyMetadata(default(InteropBitmap)));

    public static readonly DependencyProperty BitmapSourceProperty = 
      BitmapSourcePropertyKey.DependencyProperty;

    /// <summary>
    /// This is a debug purpose timer.
    /// </summary>
    private Stopwatch stopwatch;

#if DEBUG
    /// <summary>
    /// Helps showing capture graph in GraphBuilder
    /// </summary>
    private DsROTEntry rotEntry;
#endif

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the Capture class.
    /// Use capture device zero, default frame rate and size
    /// </summary>
    public VideoCapturer()
    {
      this.Initialize();
    }

    /// <summary> 
    /// Initializes a new instance of the Capture class.
    /// Use specified capture device, default frame rate and size
    /// </summary>
    /// <param name="device">The <see cref="DsDevice"/> to use.</param>
    public VideoCapturer(DsDevice device)
    {
      this.Initialize();
      this.NewCamera(device, 0, 0, 0);
    }

    /// <summary> 
    /// Initializes a new instance of the Capture class.
    /// Use specified capture device, specified frame rate and default size
    /// </summary>
    /// <param name="device">The <see cref="DsDevice"/> to use.</param>
    /// <param name="frameRate">The framerate for the capturing.</param>
    public VideoCapturer(DsDevice device, int frameRate)
    {
      this.Initialize();
      this.NewCamera(device, frameRate, 0, 0);
    }

    /// <summary> 
    /// Initializes a new instance of the Capture class.
    /// Use specified capture device, specified frame rate and size
    /// </summary>
    /// <param name="device">The <see cref="DsDevice"/> to use.</param>
    /// <param name="frameRate">The framerate for the capturing.</param>
    /// <param name="width">The width of the video stream.</param>
    /// <param name="height">The height of the video stream.</param>
    public VideoCapturer(DsDevice device, int frameRate, int width, int height)
    {
      this.Initialize();
      this.NewCamera(device, frameRate, width, height);
    }

    /// <summary>
    /// Finalizes an instance of the Capture class by removing the handlers.
    /// </summary>
    ~VideoCapturer()
    {
      this.Dispose();
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS

    /// <summary>
    /// This is the delegate for the <see cref="FrameCaptureComplete"/> event.
    /// </summary>
    /// <param name="newFrame">A <see cref="Emgu.CV.IImage"/> with the new frame.</param>
    public delegate void FrameCapHandler(System.Drawing.Bitmap newFrame);

    /// <summary>
    /// This event is raised whenever the callback method received a new frame from the 
    /// capture stream.
    /// </summary>
    public event FrameCapHandler FrameCaptureComplete;

    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// Gets the <see cref="VideoCapturer"/> singleton.
    /// If the underlying instance is null, a instance will be created.
    /// </summary>
    public static VideoCapturer Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new VideoCapturer();
        }

        // return the existing/new instance
        return instance;
      }
    }


    /// <summary>
    /// Gets the debug purpose performance clock that is in time with
    /// the capture graph.
    /// </summary>
    public Stopwatch PerformanceClock
    {
      get { return this.stopwatch; }
    }

    /// <summary>
    /// Gets the width of the video stream.
    /// </summary>
    public int NaturalVideoWidth
    {
      get { return this.videoWidth; }
    }

    /// <summary>
    /// Gets the height of the video stream.
    /// </summary>
    public int NaturalVideoHeight
    {
      get { return this.videoHeight; }
    }

    /// <summary>
    /// Gets the framerate of the video stream.
    /// </summary>
    public int FPS
    {
      get { return this.fps; }
    }

    /// <summary>
    /// Gets a value indicating whether the graph is 
    /// succesfully rendered.
    /// </summary>
    public bool HasValidGraph
    {
      get { return this.hasValidGraph; }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    /// <summary>
    /// This method checks the system for available video input devices.
    /// </summary>
    /// <returns>True, if at least on video input device is
    /// connected to the system.</returns>
    public static bool CameraExists()
    {
      DsDevice[] capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

      return !(capDevices.Length == 0);
    }

    /// <summary>
    /// This method initializes the underlying <see cref="Capture"/> class with the
    /// new camera given by the parameters.
    /// </summary>
    /// <param name="deviceNumber">Index of the capture device to be used.</param>
    /// <param name="deviceMode">Index of the capture mode to be used.</param>
    public void SetCamera(int deviceNumber, int deviceMode)
    {
      try
      {
        // Specific deviceMode (e.g. 800x600 @ 30fps)
        if (deviceMode > -1)
        {
          this.NewCamera(
            Devices.Current.Cameras[deviceNumber].DirectshowDevice,
            Devices.Current.Cameras[deviceNumber].SupportedSizesAndFPS[deviceMode].FPS,
            Devices.Current.Cameras[deviceNumber].SupportedSizesAndFPS[deviceMode].Width,
            Devices.Current.Cameras[deviceNumber].SupportedSizesAndFPS[deviceMode].Height);
        }
        else
        {
          this.NewCamera(
            Devices.Current.Cameras[deviceNumber].DirectshowDevice,
            0,
            0,
            0); // No specific deviceMode, try default
          deviceMode = 0;
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }

      // Update settings with new device
      //GTSettings.Current.CameraSettings.DeviceNumber = deviceNumber;
      //GTSettings.Current.CameraSettings.DeviceMode = deviceMode;

      if (!this.HasValidGraph)
      {
        string message = "The " +
          Devices.Current.Cameras[deviceNumber].Name
          + " camera could not be initialized (Maybe it is already in use)."
          + Environment.NewLine + "If there is another device, we will try the next one.";
        ErrorLogger.WriteLine(message);
      }
    }

    /// <summary>
    /// This method creates a new graph for the given capture device and
    /// properties.
    /// </summary>
    /// <param name="deviceNum">The index of the new capture device.</param>
    /// <param name="frameRate">The framerate to use.</param>
    /// <param name="width">The width to use.</param>
    /// <param name="height">The height to use.</param>
    public void NewCamera(DsDevice device, int frameRate, int width, int height)
    {
      this.Dispose();
      this.stopwatch.Start();

      try
      {
        // Set up the capture graph
        if (this.SetupGraph(device, frameRate, width, height))
        {
          this.isRunning = false;

          this.SetVideoProcMinMaxRanges();
          this.SetCameraControlMinMaxRanges();
        }
        else
        {
          this.hasValidGraph = false;
          return;
        }
      }
      catch
      {
        this.Dispose();
        ErrorLogger.WriteLine("Error in Camera.Capture(), Could not initialize graphs");
        this.hasValidGraph = false;
        return;
      }

      this.hasValidGraph = true;
    }

    /// <summary>
    /// Start the capture graph
    /// </summary>
    public void Start()
    {
      if (!this.isRunning && this.mediaControl != null)
      {
        int hr = this.mediaControl.Run();

        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error while starting camera. Message: " + DsError.GetErrorText(hr));
          this.isRunning = false;
        }

        this.isRunning = true;
      }
    }

    /// <summary>
    /// Pause the capture graph. Running the graph takes up a lot of resources.  
    /// Pause it when it isn't needed.
    /// </summary>
    public void Pause()
    {
      if (this.isRunning)
      {
        int hr = this.mediaControl.Pause();
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error while pausing camera. Message: " + DsError.GetErrorText(hr));
        }

        this.isRunning = false;
      }
    }

    /// <summary>
    /// Shut down capture.
    /// This is used to release all resources needed by the capture graph.
    /// </summary>
    public void Dispose()
    {
      try
      {
        // To stop the capture filter before stopping the media control
        // seems to solve the problem described in the next comment.
        // sancta simplicitas...
        if (this.capFilter != null)
        {
          this.capFilter.Stop();
        }

        // The stop or stopwhenready methods sometimes hang ... 
        // This is a multithreading issue but I don´t solved it yet
        // But stopping is needed, otherwise the video device
        // is not disposed fast enough (due to GC) so at next initialization
        // with other params the video device seems to be in 
        // use and the GraphBuilder render mehtod fails.
        if (this.mediaControl != null)
        {
          int hr = this.mediaControl.Stop();
        }

        this.isRunning = false;
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }

      if (this.capFilter != null)
      {
        Marshal.ReleaseComObject(this.capFilter);
        this.capFilter = null;
        this.cameraControl = null;
        this.videoControl = null;
        this.videoStreamConfig = null;
      }

      if (this.videoProcAmp != null)
      {
        Marshal.ReleaseComObject(this.videoProcAmp);
        this.videoProcAmp = null;
      }

      if (this.sampGrabber != null)
      {
        Marshal.ReleaseComObject(this.sampGrabber);
        this.sampGrabber = null;
      }

      if (this.graphBuilder != null)
      {
        Marshal.ReleaseComObject(this.graphBuilder);
        this.graphBuilder = null;
        this.mediaControl = null;
        this.hasValidGraph = false;
      }

      if (this.capGraph != null)
      {
        Marshal.ReleaseComObject(this.capGraph);
        this.capGraph = null;
      }

      if (this.map != IntPtr.Zero)
      {
        UnmapViewOfFile(this.map);
        this.map = IntPtr.Zero;
      }

      if (this.section != IntPtr.Zero)
      {
        CloseHandle(this.section);
        this.section = IntPtr.Zero;
      }

#if DEBUG
      if (this.rotEntry != null)
      {
        this.rotEntry.Dispose();
      }
#endif
    }

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

      if (buffer != IntPtr.Zero)
      {
        // Do not skip frames here by default.
        // We skip the processing in the Tracker class if it is not fast enough
        if (!this.skipFrameMode)
        {
          // Check mapping if it is not already released and the buffer is running
          if (this.map != IntPtr.Zero)
          {
            // This is fast and lasts less than 1 millisecond.
            CopyMemory(this.map, buffer, bufferLength);

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
              (SendOrPostCallback)delegate
              {
                BitmapSource.Invalidate();
              }, null);

            try
            {
              // Send new image to processing thread
              if (this.FrameCaptureComplete != null)
              {
                this.FrameCaptureComplete(this.videoImage);
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
      }

      return 0;
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

    /// <summary>
    /// The event handler for the <see cref="OnCameraControlPropertyChanged"/> event.
    /// Updates the video capture device with new brightness, contrast, etc.
    /// </summary>
    /// <param name="property">The <see cref="VideoProcAmpProperty"/> to be changed</param>
    /// <param name="value">The new value for the property</param>
    private void OnVideoProcAmpPropertyChanged(VideoProcAmpProperty property, int value)
    {
      if (this.videoProcAmp == null)
      {
        return;
      }

      int min, max, steppingDelta, defaultValue;
      VideoProcAmpFlags flags;

      try
      {
        this.videoProcAmp.GetRange(property, out min, out max, out steppingDelta, out defaultValue, out flags);

        if (value >= min && value <= max)
        {
          this.videoProcAmp.Set(property, value, flags);
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
    }

    /// <summary>
    /// The event handler for the <see cref="OnVideoProcAmpPropertyChanged"/> event.
    /// Updates the video capture device with new zoom, pan, etc.
    /// </summary>
    /// <param name="property">The <see cref="CameraControlProperty"/> to be changed</param>
    /// <param name="value">The new value for the property</param>
    private void OnCameraControlPropertyChanged(CameraControlProperty property, int value)
    {
      if (this.cameraControl == null)
      {
        return;
      }

      // Todo: Disabled focus as it turns on autofocus
      if (property.Equals(CameraControlProperty.Focus))
      {
        return;
      }

      int min, max, steppingDelta, defaultValue;
      CameraControlFlags flags;
      try
      {
        this.cameraControl.GetRange(property, out min, out max, out steppingDelta, out defaultValue, out flags);

        if (value >= min && value <= max)
        {
          this.cameraControl.Set(property, value, flags);
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
    }

    /// <summary>
    /// The event handler for the <see cref="OnVideoControlFlagsChanged"/> event.
    /// Updates the video capture device with new video control properties.
    /// </summary>
    /// <remarks> This method has been disabled, because it was easier to flip the incoming image
    /// with the CV image flip in ImageProcessing.cs.
    /// The direct show flipping didn't work with some webcams, e.g. the PlayStationEye3 cam or an HP Laptop Webcam</remarks>
    /// <param name="property">The <see cref="VideoControlFlags"/> to be changed</param>
    /// <param name="value">The new value for the property</param>
    private void OnVideoControlFlagsChanged(VideoControlFlags property, int value)
    {
      ////if (this.graphBuilder == null)
      ////{
      ////  return;
      ////}

      ////if (videoControl == null)
      ////  return;

      ////VideoControlFlags pCapsFlags;

      ////IPin pPin = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
      ////int hr = videoControl.GetCaps(pPin, out pCapsFlags);

      ////if (hr != 0)
      ////  ErrorLogger.WriteLine("Error: videoControl.GetCaps in Camera.Capture. Message: " + DsError.GetErrorText(hr));

      ////hr = videoControl.GetMode(pPin, out pCapsFlags);

      ////if (hr != 0)
      ////  ErrorLogger.WriteLine("Error while getting mode in videoControl.GetMode in Camera.Capture. Message: " + DsError.GetErrorText(hr));

      ////if (value == 0)
      ////{
      ////  if ((pCapsFlags&VideoControlFlags.FlipVertical)==VideoControlFlags.FlipVertical)
      ////  {
      ////    pCapsFlags |= ~VideoControlFlags.FlipVertical;
      ////  }
      ////}
      ////else
      ////{
      ////  pCapsFlags |= VideoControlFlags.FlipVertical;
      ////}

      ////hr = videoControl.SetMode(pPin, pCapsFlags);

      ////if (hr != 0)
      ////  ErrorLogger.WriteLine("Error while getting mode in videoControl.SetMode in Camera.Capture. Message: " + DsError.GetErrorText(hr));
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
    /// Copies a block of memory from one location to another.
    /// </summary>
    /// <param name="destination">A pointer to the starting address of the copied block's destination.</param>
    /// <param name="source">A pointer to the starting address of the block of memory to copy</param>
    /// <param name="length">The size of the block of memory to copy, in bytes.</param>
    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(IntPtr destination, IntPtr source, int length);

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
    private static extern IntPtr CreateFileMapping(IntPtr file, IntPtr fileMappingAttributes, uint protect, uint maximumSizeHigh, uint maximumSizeLow, string name);

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
    private static extern IntPtr MapViewOfFile(IntPtr fileMappingObject, uint desiredAccess, uint fileOffsetHigh, uint fileOffsetLow, uint numberOfBytesToMap);

    /// <summary>
    /// Unmaps a mapped view of a file from the calling process's address space.
    /// </summary>
    /// <param name="map">A pointer to the base address of the mapped view of a file that is to be unmapped. </param>
    /// <returns>If the function succeeds, the return value is nonzero, and all dirty pages within the specified range are written "lazily" to disk.
    /// If the function fails, the return value is zero. </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool UnmapViewOfFile(IntPtr map);

    /// <summary>
    /// Closes an open object handle.
    /// </summary>
    /// <param name="handle">A valid handle to an open object.</param>
    /// <returns>If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    /// <summary>
    /// Connects to the property changed events of the camera settings.
    /// </summary>
    private void Initialize()
    {
      this.stopwatch = new Stopwatch();
    }

    /// <summary>
    /// Build the capture graph for grabber. 
    /// </summary>
    /// <param name="dev">The index of the new capture device.</param>
    /// <param name="frameRate">The framerate to use.</param>
    /// <param name="width">The width to use.</param>
    /// <param name="height">The height to use.</param>
    /// <returns>True, if succesfull, otherwise false.</returns>
    private bool SetupGraph(DsDevice dev, int frameRate, int width, int height)
    {
      int hr;

      this.fps = frameRate; // Not measured, only to expose FPS externally 

      this.cameraControl = null;
      this.capFilter = null;

      // Get the graphbuilder object
      this.graphBuilder = (IFilterGraph2)new FilterGraph();
      this.mediaControl = this.graphBuilder as IMediaControl;

      try
      {
        // Create the ICaptureGraphBuilder2
        this.capGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        // Create the SampleGrabber interface
        this.sampGrabber = (ISampleGrabber)new SampleGrabber();

        // Start building the graph
        hr = this.capGraph.SetFiltergraph(this.graphBuilder);
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error in capGraph.SetFiltergraph. Could not build graph. Message: " + DsError.GetErrorText(hr));
        }

#if DEBUG
        this.rotEntry = new DsROTEntry(this.graphBuilder);
#endif

        // Add the video device
        hr = this.graphBuilder.AddSourceFilterForMoniker(dev.Mon, null, "Video input", out this.capFilter);
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error in m_graphBuilder.AddSourceFilterForMoniker(). Could not add source filter. Message: " + DsError.GetErrorText(hr));
        }

        IBaseFilter baseGrabFlt = (IBaseFilter)this.sampGrabber;

        this.ConfigureSampleGrabber(this.sampGrabber);

        // Add the frame grabber to the graph
        hr = this.graphBuilder.AddFilter(baseGrabFlt, "Ds.NET Grabber");
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error in m_graphBuilder.AddFilter(). Could not add filter. Message: " + DsError.GetErrorText(hr));
        }

        this.cameraControl = this.capFilter as IAMCameraControl;

        // turn on the infrared leds ONLY FOR THE GENIUS WEBCAM
        /*
        if (!defaultMode)
        {
            m_icc = capFilter as IAMCameraControl;
            CameraControlFlags CamFlags = new CameraControlFlags();
            int pMin, pMax, pStep, pDefault;

            hr = m_icc.GetRange(CameraControlProperty.Focus, out pMin, out pMax, out pStep, out pDefault, out CamFlags);
            m_icc.Set(CameraControlProperty.Focus, pMax, CameraControlFlags.None);
        }
        */

        // Set videoProcAmp
        object obj;
        Guid iid_IBaseFilter = new Guid("56a86895-0ad4-11ce-b03a-0020af0ba770");
        Devices.Current.Cameras[0].DirectshowDevice.Mon.BindToObject(
            null,
            null,
            ref iid_IBaseFilter,
            out obj);

        this.videoProcAmp = obj as IAMVideoProcAmp;

        // If any of the default config items are set
        if (frameRate + height + width > 0)
        {
          this.SetConfigParms(this.capGraph, this.capFilter, frameRate, width, height);
        }

        hr = this.capGraph.RenderStream(PinCategory.Capture, MediaType.Video, this.capFilter, null, baseGrabFlt);
        // Check for succesful rendering, if this failed the class cannot be used, so dispose the resources and return false.
        if (hr < 0)
        {
          string error = DsError.GetErrorText(hr);
          MessageBox.Show(error);
          this.Dispose();
          return false;
        }
        else
        {
          // Otherwise update the SampleGrabber.
          this.SaveSizeInfo(this.sampGrabber);

          hr = this.sampGrabber.SetBufferSamples(false);

          if (hr == 0)
          {
            hr = this.sampGrabber.SetOneShot(false);
          }

          if (hr == 0)
          {
            hr = this.sampGrabber.SetCallback(this, 1);
          }

          if (hr < 0)
          {
            ErrorLogger.WriteLine("Could not set callback function (SetupGraph) in Camera.Capture()");
          }
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);

        this.Dispose();
        return false;
      }

      return true;
    }

    /// <summary>
    /// Configure the sample grabber with default Video RGB24 mode.
    /// </summary>
    /// <param name="sampGrabber">The <see cref="ISampleGrabber"/> to be configured.</param>
    private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
    {
      AMMediaType media;
      int hr;

      // Set the media type to Video/RBG24
      media = new AMMediaType();
      media.majorType = MediaType.Video;
      media.subType = MediaSubType.RGB32;
      media.formatType = FormatType.VideoInfo;

      hr = this.sampGrabber.SetMediaType(media);

      if (hr != 0)
      {
        ErrorLogger.WriteLine("Could not ConfigureSampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      DsUtils.FreeAMMediaType(media);
      media = null;

      // Configure the samplegrabber
      hr = this.sampGrabber.SetCallback(this, 1);

      if (hr != 0)
      {
        ErrorLogger.WriteLine("Could not set callback method for sampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }
    }

    /// <summary>
    /// Set the Framerate, and video size
    /// </summary>
    /// <param name="capGraph">The <see cref="ICaptureGraphBuilder2"/> interface.</param>
    /// <param name="capFilter">The <see cref="IBaseFilter"/> of the capture device.</param>
    /// <param name="frameRate">The new framerate to be used.</param>
    /// <param name="width">The new video width to be used.</param>
    /// <param name="height">The new video height to be used.</param>
    private void SetConfigParms(ICaptureGraphBuilder2 capGraph, IBaseFilter capFilter, int frameRate, int width, int height)
    {
      int hr;
      object o;
      AMMediaType media;

      // Find the stream config interface
      hr = this.capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out o);

      this.videoControl = capFilter as IAMVideoControl;
      this.videoStreamConfig = o as IAMStreamConfig;

      if (this.videoStreamConfig == null)
      {
        ErrorLogger.WriteLine("Error in Capture.SetConfigParams(). Failed to get IAMStreamConfig");
      }

      // Get the existing format block
      hr = this.videoStreamConfig.GetFormat(out media);

      if (hr != 0)
      {
        ErrorLogger.WriteLine("Could not SetConfigParms in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      // copy out the videoinfoheader
      VideoInfoHeader v = new VideoInfoHeader();
      Marshal.PtrToStructure(media.formatPtr, v);

      // if overriding set values
      if (frameRate > 0)
      {
        v.AvgTimePerFrame = 10000000 / frameRate;
      }

      if (width > 0)
      {
        v.BmiHeader.Width = width;
      }

      if (height > 0)
      {
        v.BmiHeader.Height = height;
      }

      // Copy the media structure back
      Marshal.StructureToPtr(v, media.formatPtr, true);

      // Set the new format
      hr = this.videoStreamConfig.SetFormat(media);
      if (hr != 0)
      {
        ErrorLogger.WriteLine("Error while setting new camera format (videoStreamConfig) in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      DsUtils.FreeAMMediaType(media);
      media = null;
    }

    /// <summary>
    /// Saves the video properties of the SampleGrabber into member fields
    /// and creates a file mapping for the captured frames.
    /// </summary>
    /// <param name="sampGrabber">The <see cref="ISampleGrabber"/>
    /// from which to retreive the sample information.</param>
    private void SaveSizeInfo(ISampleGrabber sampGrabber)
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
      this.videoWidth = videoInfoHeader.BmiHeader.Width;
      this.videoHeight = videoInfoHeader.BmiHeader.Height;
      this.stride = this.videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

      this.bufferLength = this.videoWidth * this.videoHeight * 4; // RGB24 = 3 bytes

      // create memory section and map for the OpenCV Image.
      this.section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);
      this.map = MapViewOfFile(this.section, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.videoImage = new System.Drawing.Bitmap(
        this.videoWidth,
        this.videoHeight,
        this.stride,
        System.Drawing.Imaging.PixelFormat.Format32bppRgb,
        this.map);

      this.BitmapSource = Imaging.CreateBitmapSourceFromMemorySection(
        this.section, this.videoWidth, this.videoHeight, PixelFormats.Bgr32, this.videoWidth * 4, 0) as InteropBitmap;

      DsUtils.FreeAMMediaType(media);
      media = null;
    }

    /// <summary>
    /// This method is used to set minimum and maximum values for the
    /// <see cref="VideoProcAmpProperty"/>s of the capture device.
    /// </summary>
    private void SetVideoProcMinMaxRanges()
    {
      if (this.videoProcAmp == null)
      {
        return;
      }

      int min, max, steppingDelta, defaultValue;
      VideoProcAmpFlags flags;

      foreach (VideoProcAmpProperty item in Enum.GetValues(typeof(VideoProcAmpProperty)))
      {
        switch (item)
        {
          case VideoProcAmpProperty.Brightness:
            this.videoProcAmp.GetRange(VideoProcAmpProperty.Brightness, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.Saturation:
            this.videoProcAmp.GetRange(VideoProcAmpProperty.Saturation, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.Sharpness:
            this.videoProcAmp.GetRange(VideoProcAmpProperty.Sharpness, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.Contrast:
            this.videoProcAmp.GetRange(VideoProcAmpProperty.Contrast, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.ColorEnable:
            // videoProcAmp.GetRange(VideoProcAmpProperty.ColorEnable, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.Gain:
            // videoProcAmp.GetRange(VideoProcAmpProperty.Gain, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.Gamma:
            // videoProcAmp.GetRange(VideoProcAmpProperty.Gamma, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.Hue:
            // videoProcAmp.GetRange(VideoProcAmpProperty.Hue, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.WhiteBalance:
            // videoProcAmp.GetRange(VideoProcAmpProperty.WhiteBalance, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;

          case VideoProcAmpProperty.BacklightCompensation:
            // videoProcAmp.GetRange(VideoProcAmpProperty.BacklightCompensation, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;
        }
      }
    }

    /// <summary>
    /// This method is used to set minimum and maximum values for the
    /// <see cref="CameraControlProperty"/>s of the capture device.
    /// </summary>
    private void SetCameraControlMinMaxRanges()
    {
      if (this.cameraControl == null)
      {
        return;
      }

      int min, max, steppingDelta, defaultValue;
      CameraControlFlags flags;

      foreach (CameraControlProperty item in Enum.GetValues(typeof(CameraControlProperty)))
      {
        switch (item)
        {
          case CameraControlProperty.Exposure:
            this.cameraControl.GetRange(CameraControlProperty.Exposure, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;
          case CameraControlProperty.Focus:
            this.cameraControl.GetRange(CameraControlProperty.Focus, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;
          case CameraControlProperty.Zoom:
            this.cameraControl.GetRange(CameraControlProperty.Focus, out min, out max, out steppingDelta, out defaultValue, out flags);
            break;
          case CameraControlProperty.Iris:
            break;
          case CameraControlProperty.Pan:
            break;
          case CameraControlProperty.Roll:
            break;
          case CameraControlProperty.Tilt:
            break;
        }
      }
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}