namespace VianaNET
{
  using System;
  using System.Diagnostics;
  using System.Runtime.InteropServices;
  using System.Windows;
  using DirectShowLib;

  /// <summary>
  /// This is the main class for the DirectShow interop.
  /// It creates a graph that pushes video frames from a Video Input Device
  /// through the filter chain to a SampleGrabber, from which the
  /// frames can be catched and send into the processing tree of
  /// the application.
  /// </summary>
  public class VideoCapturer : VideoBase
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
    /// The IAMVideoControl interface controls certain video capture operations 
    /// such as enumerating available frame rates and image orientation.
    /// </summary>
    private IAMVideoControl videoControl = null;

    /// <summary>
    /// The IAMStreamConfig interface sets the output format on certain capture 
    /// and compression filters, for both audio and video. Applications can use 
    /// this interface to set format properties, such as the output dimensions and 
    /// frame rate (for video) or the sample rate and number of channels (for audio).
    /// </summary>
    private IAMStreamConfig videoStreamConfig = null;

    /// <summary>
    /// This member holds the <see cref="IBaseFilter"/> of the capture device filter.
    /// </summary>
    private IBaseFilter captureFilter = null;

    /// <summary>
    /// Saves the framerate of the video stream
    /// </summary>
    private int fps = 0;

    private Stopwatch frameTimer;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Initializes a new instance of the VideoCapturer class.
    /// Use capture device zero, default frame rate and size
    /// </summary>
    public VideoCapturer()
    {
    }

    /// <summary> 
    /// Initializes a new instance of the VideoCapturer class.
    /// Use specified capture device, default frame rate and size
    /// </summary>
    /// <param name="device">The <see cref="DsDevice"/> to use.</param>
    public VideoCapturer(DsDevice device)
    {
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
      this.NewCamera(device, frameRate, 0, 0);
    }

    /// <summary> 
    /// Initializes a new instance of the VideoCapturer class.
    /// Use specified capture device, specified frame rate and size
    /// </summary>
    /// <param name="device">The <see cref="DsDevice"/> to use.</param>
    /// <param name="frameRate">The framerate for the capturing.</param>
    /// <param name="width">The width of the video stream.</param>
    /// <param name="height">The height of the video stream.</param>
    public VideoCapturer(DsDevice device, int frameRate, int width, int height)
    {
      this.NewCamera(device, frameRate, width, height);
    }

    #endregion //CONSTRUCTION

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////
    #region EVENTS
    #endregion EVENTS

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region PROPERTIES

    /// <summary>
    /// Gets the framerate of the video stream.
    /// </summary>
    public int FPS
    {
      get { return this.fps; }
    }

    public DsDevice VideoCaptureDevice
    {
      get { return (DsDevice)GetValue(VideoCaptureDeviceProperty); }
      set { SetValue(VideoCaptureDeviceProperty, value); }
    }

    public static readonly DependencyProperty VideoCaptureDeviceProperty =
      DependencyProperty.Register(
      "VideoCaptureDevice",
      typeof(DsDevice),
      typeof(VideoCapturer),
      new PropertyMetadata(OnVideoCaptureDevicePropertyChanged));

    private static void OnVideoCaptureDevicePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (Video.Instance.VideoMode == VideoMode.Capture)
      {
        (obj as VideoCapturer).NewCamera(args.NewValue as DsDevice, 0, 0, 0);
      }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void ShowPropertyPageOfVideoDevice()
    {
      if (this.captureFilter != null)
      {
        DShowUtils.DisplayPropertyPage(IntPtr.Zero, this.captureFilter);
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

      this.frameCounter = 0;
      this.frameTimer = new Stopwatch();

      try
      {
        // Set up the capture graph
        if (!this.SetupGraph(device, frameRate, width, height))
        {
          this.HasVideo = false;
          return;
        }
      }
      catch
      {
        this.Dispose();
        ErrorLogger.WriteLine("Error in Camera.Capture(), Could not initialize graphs");
        this.HasVideo = false;
        return;
      }

      Video.Instance.ImageProcessing.InitializeImageFilters();

      this.Play();
      this.HasVideo = true;
      this.OnVideoAvailable();
    }

    public override void Play()
    {
      base.Play();
      this.frameTimer.Start();
    }

    public override void Stop()
    {
      int hr = 0;
      try
      {
        // To stop the capture filter before stopping the media control
        // seems to solve the problem described in the next comment.
        // sancta simplicitas...
        if (this.captureFilter != null)
        {
          hr = this.captureFilter.Stop();
          this.frameTimer.Stop();
          if (hr != 0)
          {
            ErrorLogger.WriteLine("Error while stopping capture filter. Message: " + DsError.GetErrorText(hr));
          }
        }

      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }

      base.Stop();
    }

    /// <summary>
    /// Shut down capture.
    /// This is used to release all resources needed by the capture graph.
    /// </summary>
    public override void Dispose()
    {
      this.Stop();

      if (this.captureFilter != null)
      {
        Marshal.ReleaseComObject(this.captureFilter);
        this.captureFilter = null;
        this.videoControl = null;
        this.videoStreamConfig = null;
      }

      base.Dispose();
    }

    public override void Revert()
    {
      base.Revert();
      this.frameTimer.Reset();
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

    protected override void OnVideoFrameChanged()
    {
      this.UpdateFrameNumberAndMediatime();
      base.OnVideoFrameChanged();
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

    private void UpdateFrameNumberAndMediatime()
    {
      this.MediaPositionFrameIndex = this.frameCounter;
      this.MediaPositionInNanoSeconds = this.frameTimer.ElapsedMilliseconds * 10000;
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

      this.captureFilter = null;

      // Get the graphbuilder object
      this.filterGraph = (IFilterGraph2)new FilterGraph();

      // Create the ICaptureGraphBuilder2
      ICaptureGraphBuilder2 captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

      this.mediaControl = this.filterGraph as IMediaControl;

      try
      {

        // Create the SampleGrabber interface
        this.sampleGrabber = (ISampleGrabber)new SampleGrabber();

        // Start building the graph
        hr = captureGraphBuilder.SetFiltergraph(this.filterGraph);
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error in capGraph.SetFiltergraph. Could not build graph. Message: " + DsError.GetErrorText(hr));
        }

//#if DEBUG
        this.rotEntry = new DsROTEntry(this.filterGraph);
//#endif

        // Add the video device
        hr = this.filterGraph.AddSourceFilterForMoniker(dev.Mon, null, "Video input", out this.captureFilter);
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error in m_graphBuilder.AddSourceFilterForMoniker(). Could not add source filter. Message: " + DsError.GetErrorText(hr));
        }

        IBaseFilter baseGrabFlt = (IBaseFilter)this.sampleGrabber;

        this.ConfigureSampleGrabber(this.sampleGrabber);

        // Add the frame grabber to the graph
        hr = this.filterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error in m_graphBuilder.AddFilter(). Could not add filter. Message: " + DsError.GetErrorText(hr));
        }

        // If any of the default config items are set
        if (frameRate + height + width > 0)
        {
          this.SetConfigParms(captureGraphBuilder, this.captureFilter, frameRate, width, height);
        }

        hr = captureGraphBuilder.RenderStream(null, null, this.captureFilter, null, baseGrabFlt);
        string error = DsError.GetErrorText(hr);
        // Check for succesful rendering, if this failed the class cannot be used, so dispose the resources and return false.
        if (hr < 0)
        {
          this.Dispose();
          return false;
        }
        else
        {
          // Otherwise update the SampleGrabber.
          this.SaveSizeInfo(this.sampleGrabber);

          hr = this.sampleGrabber.SetBufferSamples(false);

          if (hr == 0)
          {
            hr = this.sampleGrabber.SetOneShot(false);
          }

          if (hr == 0)
          {
            hr = this.sampleGrabber.SetCallback(this, 1);
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
      finally
      {
        if (captureGraphBuilder != null)
        {
          Marshal.ReleaseComObject(captureGraphBuilder);
          captureGraphBuilder = null;
        }
      }

      return true;
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
      hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out o);

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

      // Get the new format
      hr = this.videoStreamConfig.GetFormat(out media);
      if (hr != 0)
      {
        ErrorLogger.WriteLine("Error while setting new camera format (videoStreamConfig) in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      // copy out the videoinfoheader
      Marshal.PtrToStructure(media.formatPtr, v);
      this.FrameTimeInNanoSeconds = v.AvgTimePerFrame;

      DsUtils.FreeAMMediaType(media);
      media = null;
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER
    #endregion //HELPER
  }
}