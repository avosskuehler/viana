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
  public class VideoCapturer : VideoBase, ISampleGrabberCB
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
    /// This indicates if the frames of the capture callback should be skipped.
    /// </summary>
    private bool skipFrameMode = false;

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
    /// Saves the framerate of the video stream
    /// </summary>
    private int fps = 0;

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

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS

    public void ShowPropertyPageOfVideoDevice()
    {
      if (this.capFilter != null)
      {
        DShowUtils.DisplayPropertyPage(IntPtr.Zero, this.capFilter);
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

      try
      {
        // Set up the capture graph
        if (this.SetupGraph(device, frameRate, width, height))
        {
          this.IsRunning = false;
        }
        else
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

      this.HasVideo = true;
      this.OnVideoAvailable();
    }

    /// <summary>
    /// Start the capture graph
    /// </summary>
    public override void Play()
    {
      if (!this.IsRunning && this.mediaControl != null)
      {
        int hr = this.mediaControl.Run();

        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error while starting camera. Message: " + DsError.GetErrorText(hr));
          this.IsRunning = false;
        }
        else
        {
          this.IsRunning = true;
        }
      }
    }

    /// <summary>
    /// Pause the capture graph. Running the graph takes up a lot of resources.  
    /// Pause it when it isn't needed.
    /// </summary>
    public override void Pause()
    {
      if (this.IsRunning)
      {
        int hr = this.mediaControl.Pause();
        if (hr != 0)
        {
          ErrorLogger.WriteLine("Error while pausing camera. Message: " + DsError.GetErrorText(hr));
        }

        this.IsRunning = false;
      }
    }

    public override void Stop()
    {
      if (this.IsRunning)
      {
        try
        {
          // To stop the capture filter before stopping the media control
          // seems to solve the problem described in the next comment.
          // sancta simplicitas...
          if (this.capFilter != null)
          {
            int hr = this.capFilter.Stop();
            if (hr != 0)
            {
              ErrorLogger.WriteLine("Error while stopping capture filter. Message: " + DsError.GetErrorText(hr));
            }
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
            if (hr != 0)
            {
              ErrorLogger.WriteLine("Error while stopping camera. Message: " + DsError.GetErrorText(hr));
            }
          }

          this.IsRunning = false;
        }
        catch (Exception ex)
        {
          ErrorLogger.ProcessException(ex, false);
        }

        base.Stop();
      }
    }

    public override void Revert()
    {
      this.Stop();
    }

    /// <summary>
    /// Shut down capture.
    /// This is used to release all resources needed by the capture graph.
    /// </summary>
    public override void Dispose()
    {
      this.Stop();

      if (this.capFilter != null)
      {
        Marshal.ReleaseComObject(this.capFilter);
        this.capFilter = null;
        this.videoControl = null;
        this.videoStreamConfig = null;
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
        this.HasVideo = false;
      }

      if (this.capGraph != null)
      {
        Marshal.ReleaseComObject(this.capGraph);
        this.capGraph = null;
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
      this.MediaPositionFrameIndex++;
      this.MediaPositionInMS = (long)(sampleTime * 1000);
      this.FrameCount++;

      if (buffer != IntPtr.Zero)
      {
        // Do not skip frames here by default.
        // We skip the processing in the Tracker class if it is not fast enough
        if (!this.skipFrameMode)
        {
          // Check mapping if it is not already released and the buffer is running
          if (this.Map != IntPtr.Zero)
          {
            // This is fast and lasts less than 1 millisecond.
            CopyMemory(this.Map, buffer, bufferLength);

            try
            {
              Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
              {
                ((InteropBitmap)this.ImageSource).Invalidate();
              }, null);
              //// Send new image to processing thread
              //if (this.FrameCaptureComplete != null)
              //{
              //    this.FrameCaptureComplete(this.videoImage);
              //}
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

        // If any of the default config items are set
        if (frameRate + height + width > 0)
        {
          this.SetConfigParms(this.capGraph, this.capFilter, frameRate, width, height);
        }

        hr = this.capGraph.RenderStream(PinCategory.Capture, MediaType.Video, this.capFilter, null, baseGrabFlt);
        // Check for succesful rendering, if this failed the class cannot be used, so dispose the resources and return false.
        if (hr < 0)
        {
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
      media.subType = MediaSubType.RGB24;
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

      // Get the new format
      hr = this.videoStreamConfig.GetFormat(out media);
      if (hr != 0)
      {
        ErrorLogger.WriteLine("Error while setting new camera format (videoStreamConfig) in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      // copy out the videoinfoheader
      Marshal.PtrToStructure(media.formatPtr, v);
      this.FrameTime = v.AvgTimePerFrame * 10000000;

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
      this.NaturalVideoWidth = videoInfoHeader.BmiHeader.Width;
      this.NaturalVideoHeight = videoInfoHeader.BmiHeader.Height;

      this.CreateMemoryMapping(3);
      this.ImageSource = Imaging.CreateBitmapSourceFromMemorySection(
        this.section,
        (int)this.NaturalVideoWidth,
        (int)this.NaturalVideoHeight,
        PixelFormats.Bgr32,
        this.Stride,
        0) as InteropBitmap;

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