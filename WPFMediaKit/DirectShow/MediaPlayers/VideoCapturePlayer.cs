using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DirectShowLib;
using System.Windows;
using System.Diagnostics;

namespace WPFMediaKit.DirectShow.MediaPlayers
{
  public class VideoSampleArgs : EventArgs
  {
    public Bitmap VideoFrame { get; internal set; }
  }

  /// <summary>
  /// A Player that plays video from a video capture device.
  /// </summary>
  public class VideoCapturePlayer : MediaPlayerBase, ISampleGrabberCB
  {
    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(IntPtr destination, IntPtr source, [MarshalAs(UnmanagedType.U4)] int length);

    #region Locals
    /// <summary>
    /// The video capture pixel height
    /// </summary>
    private int m_desiredHeight = 240;

    /// <summary>
    /// The video capture pixel width
    /// </summary>
    private int m_desiredWidth = 320;

    /// <summary>
    /// The video capture's frames per second
    /// </summary>
    private int m_fps = 30;

    /// <summary>
    /// Our DirectShow filter graph
    /// </summary>
    private IGraphBuilder m_graph;

    /// <summary>
    /// The DirectShow video renderer
    /// </summary>
    private IBaseFilter m_renderer;

    /// <summary>
    /// The capture device filter
    /// </summary>
    private IBaseFilter m_captureDevice;

    /// <summary>
    /// The name of the video capture source device
    /// </summary>
    private string m_videoCaptureSource;

    /// <summary>
    /// Flag to detect if the capture source has changed
    /// </summary>
    private bool m_videoCaptureSourceChanged;

    /// <summary>
    /// The video capture device
    /// </summary>
    private DsDevice m_videoCaptureDevice;

    /// <summary>
    /// Flag to detect if the capture source device has changed
    /// </summary>
    private bool m_videoCaptureDeviceChanged;

    /// <summary>
    /// The sample grabber interface used for getting samples in a callback
    /// </summary>
    private ISampleGrabber m_sampleGrabber;

    /// <summary>
    /// Saves the bufferLength of the video stream
    /// </summary>
    private int bufferLength;

    private string m_fileName;

#if DEBUG
    private DsROTEntry m_rotEntry;
#endif
    #endregion

    /// <summary>
    /// Gets or sets if the instance fires an event for each of the samples
    /// </summary>
    public bool EnableSampleGrabbing { get; set; }

    /// <summary>
    /// Fires when a new video sample is ready
    /// </summary>
    public event EventHandler<VideoSampleArgs> NewVideoSample;

    /// <summary>
    /// The name of the video capture source to use
    /// </summary>
    public string VideoCaptureSource
    {
      get
      {
        VerifyAccess();
        return m_videoCaptureSource;
      }
      set
      {
        VerifyAccess();
        m_videoCaptureSource = value;
        m_videoCaptureSourceChanged = true;

        /* Free our unmanaged resources when
         * the source changes */
        FreeResources();
      }
    }

    public DsDevice VideoCaptureDevice
    {
      get
      {
        VerifyAccess();
        return m_videoCaptureDevice;
      }
      set
      {
        VerifyAccess();
        m_videoCaptureDevice = value;
        m_videoCaptureDeviceChanged = true;

        /* Free our unmanaged resources when
         * the source changes */
        FreeResources();
      }
    }

    /// <summary>
    /// The frames per-second to play
    /// the capture device back at
    /// </summary>
    public int FPS
    {
      get
      {
        VerifyAccess();
        return m_fps;
      }
      set
      {
        VerifyAccess();

        /* We support only a minimum of
         * one frame per second */
        if (value < 1)
          value = 1;

        m_fps = value;
      }
    }

    /// <summary>
    /// Gets or sets if Yuv is the prefered color space
    /// </summary>
    public bool UseYuv { get; set; }

    /// <summary>
    /// The desired pixel width of the video
    /// </summary>
    public int DesiredWidth
    {
      get
      {
        VerifyAccess();
        return m_desiredWidth;
      }
      set
      {
        VerifyAccess();
        m_desiredWidth = value;
      }
    }

    /// <summary>
    /// The desired pixel height of the video
    /// </summary>
    public int DesiredHeight
    {
      get
      {
        VerifyAccess();
        return m_desiredHeight;
      }
      set
      {
        VerifyAccess();
        m_desiredHeight = value;
      }
    }

    public string FileName
    {
      get
      {
        //VerifyAccess();
        return m_fileName;
      }
      set
      {
        //VerifyAccess();
        m_fileName = value;
      }
    }

    /// <summary>
    /// Plays the video capture device
    /// </summary>
    public override void Play()
    {
      VerifyAccess();

      if (m_graph == null)
        SetupGraph();

      base.Play();
    }

    /// <summary>
    /// Pauses the video capture device
    /// </summary>
    public override void Pause()
    {
      VerifyAccess();

      if (m_graph == null)
        SetupGraph();

      base.Pause();
    }

    public void ShowCapturePropertyPages(IntPtr hwndOwner)
    {
      VerifyAccess();

      if (m_captureDevice == null)
        return;

      using (var dialog = new PropertyPageHelper(m_captureDevice))
      {
        dialog.Show(hwndOwner);
      }
    }

    /// <summary>
    /// Configures the DirectShow graph to play the selected video capture
    /// device with the selected parameters
    /// </summary>
    private void SetupGraph()
    {
      /* Clean up any messes left behind */
      FreeResources();

      try
      {
        /* Create a new graph */
        m_graph = (IGraphBuilder)new FilterGraphNoThread();

#if DEBUG
        m_rotEntry = new DsROTEntry(m_graph);
#endif

        /* Create a capture graph builder to help 
                 * with rendering a capture graph */
        var graphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        /* Set our filter graph to the capture graph */
        int hr = graphBuilder.SetFiltergraph(m_graph);
        DsError.ThrowExceptionForHR(hr);

        /* Add our capture device source to the graph */
        if (m_videoCaptureSourceChanged)
        {
          m_captureDevice = AddFilterByName(m_graph,
                                            FilterCategory.VideoInputDevice,
                                            VideoCaptureSource);

          m_videoCaptureSourceChanged = false;
        }
        else if (m_videoCaptureDeviceChanged)
        {
          m_captureDevice = AddFilterByDevicePath(m_graph,
                                                  FilterCategory.VideoInputDevice,
                                                  VideoCaptureDevice.DevicePath);

          m_videoCaptureDeviceChanged = false;
        }

        /* If we have a null capture device, we have an issue */
        if (m_captureDevice == null)
          throw new Exception(string.Format("Capture device {0} not found or could not be created", VideoCaptureSource));

        if (UseYuv && !EnableSampleGrabbing)
        {
          /* Configure the video output pin with our parameters and if it fails
           * then just use the default media subtype*/
          if (!SetVideoCaptureParameters(graphBuilder, m_captureDevice, MediaSubType.YUY2))
            SetVideoCaptureParameters(graphBuilder, m_captureDevice, Guid.Empty);
        }
        else
          /* Configure the video output pin with our parameters */
          SetVideoCaptureParameters(graphBuilder, m_captureDevice, Guid.Empty);

        var rendererType = VideoRendererType.VideoMixingRenderer9;

        /* Creates a video renderer and register the allocator with the base class */
        m_renderer = CreateVideoRenderer(rendererType, m_graph, 1);

        if (rendererType == VideoRendererType.VideoMixingRenderer9)
        {
          var mixer = m_renderer as IVMRMixerControl9;

          if (mixer != null && !EnableSampleGrabbing && UseYuv)
          {
            VMR9MixerPrefs dwPrefs;
            mixer.GetMixingPrefs(out dwPrefs);
            dwPrefs &= ~VMR9MixerPrefs.RenderTargetMask;
            dwPrefs |= VMR9MixerPrefs.RenderTargetYUV;
            /* Prefer YUV */
            mixer.SetMixingPrefs(dwPrefs);
          }
        }

        if (EnableSampleGrabbing)
        {
          m_sampleGrabber = (ISampleGrabber)new SampleGrabber();
          this.ConfigureSampleGrabber(m_sampleGrabber);
          //SetupSampleGrabber(m_sampleGrabber);
          hr = m_graph.AddFilter(m_sampleGrabber as IBaseFilter, "SampleGrabber");
          DsError.ThrowExceptionForHR(hr);
        }

        IBaseFilter mux = null;
        IFileSinkFilter sink = null;
        if (!string.IsNullOrEmpty(this.m_fileName))
        {
          hr = graphBuilder.SetOutputFileName(MediaSubType.Asf, this.m_fileName, out mux, out sink);
          DsError.ThrowExceptionForHR(hr);

          hr = graphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, m_captureDevice, null, mux);
          DsError.ThrowExceptionForHR(hr);

          // use the first audio device
          var audioDevices = DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);

          if (audioDevices.Length > 0)
          {
            var audioDevice = AddFilterByDevicePath(m_graph,
                                                FilterCategory.AudioInputDevice,
                                                audioDevices[0].DevicePath);

            hr = graphBuilder.RenderStream(PinCategory.Capture, MediaType.Audio, audioDevice, null, mux);
            DsError.ThrowExceptionForHR(hr);
          }
        }

        hr = graphBuilder.RenderStream(PinCategory.Preview,
                                       MediaType.Video,
                                       m_captureDevice,
                                       null,
                                       m_renderer);

        DsError.ThrowExceptionForHR(hr);

        /* Register the filter graph 
         * with the base classes */
        SetupFilterGraph(m_graph);

        /* Sets the NaturalVideoWidth/Height */
        SetNativePixelSizes(m_renderer);

        this.SaveSizeInfo(m_sampleGrabber);

        HasVideo = true;

        /* Make sure we Release() this COM reference */
        if (mux != null)
        {
          Marshal.ReleaseComObject(mux);
        }
        if (sink != null)
        {
          Marshal.ReleaseComObject(sink);
        }

        Marshal.ReleaseComObject(graphBuilder);
      }
      catch (Exception ex)
      {
        /* Something got fuct up */
        FreeResources();
        InvokeMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
      }

      /* Success */
      InvokeMediaOpened();
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

      hr = sampGrabber.SetMediaType(media);

      if (hr != 0)
      {
        //ErrorLogger.WriteLine("Could not ConfigureSampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }

      DsUtils.FreeAMMediaType(media);
      media = null;

      // Configure the samplegrabber
      hr = sampGrabber.SetCallback(this, 1);

      if (hr != 0)
      {
        //ErrorLogger.WriteLine("Could not set callback method for sampleGrabber in Camera.Capture. Message: " + DsError.GetErrorText(hr));
      }
    }


    /// <summary>
    /// Sets the capture parameters for the video capture device
    /// </summary>
    private bool SetVideoCaptureParameters(ICaptureGraphBuilder2 capGraph, IBaseFilter captureFilter, Guid mediaSubType)
    {
      /* The stream config interface */
      object streamConfig;

      /* Get the stream's configuration interface */
      int hr = capGraph.FindInterface(PinCategory.Capture,
                                      MediaType.Video,
                                      captureFilter,
                                      typeof(IAMStreamConfig).GUID,
                                      out streamConfig);

      DsError.ThrowExceptionForHR(hr);

      var videoStreamConfig = streamConfig as IAMStreamConfig;

      /* If QueryInterface fails... */
      if (videoStreamConfig == null)
      {
        throw new Exception("Failed to get IAMStreamConfig");
      }

      /* The media type of the video */
      AMMediaType media;

      /* Get the AMMediaType for the video out pin */
      hr = videoStreamConfig.GetFormat(out media);
      DsError.ThrowExceptionForHR(hr);

      /* Make the VIDEOINFOHEADER 'readable' */
      var videoInfo = new VideoInfoHeader();
      Marshal.PtrToStructure(media.formatPtr, videoInfo);

      /* Setup the VIDEOINFOHEADER with the parameters we want */
      videoInfo.AvgTimePerFrame = DSHOW_ONE_SECOND_UNIT / FPS;
      videoInfo.BmiHeader.Width = DesiredWidth;
      videoInfo.BmiHeader.Height = DesiredHeight;

      if (mediaSubType != Guid.Empty)
      {
        int fourCC = 0;
        byte[] b = mediaSubType.ToByteArray();
        fourCC = b[0];
        fourCC |= b[1] << 8;
        fourCC |= b[2] << 16;
        fourCC |= b[3] << 24;

        videoInfo.BmiHeader.Compression = fourCC;
        media.subType = mediaSubType;
      }

      /* Copy the data back to unmanaged memory */
      Marshal.StructureToPtr(videoInfo, media.formatPtr, false);

      /* Set the format */
      hr = videoStreamConfig.SetFormat(media);
      DsError.ThrowExceptionForHR(hr);

      /* We don't want any memory leaks, do we? */
      DsUtils.FreeAMMediaType(media);

      videoWidth = videoInfo.BmiHeader.Width;
      videoHeight = videoInfo.BmiHeader.Height;
      stride = videoWidth * (videoInfo.BmiHeader.BitCount / 8);

      ////m_bitmapDataArray = new byte[videoInfoHeader.BmiHeader.ImageSize];
      //m_handle = Marshal.AllocCoTaskMem(stride * videoHeight);

      if (hr < 0)
        return false;

      return true;
    }

    #region ISampleGrabberCB Members

    int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample pSample)
    {
      Marshal.ReleaseComObject(pSample);
      return 0;
    }

    private int videoWidth;
    private int videoHeight;
    private int stride;

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
    private Bitmap videoImage;

    public Bitmap VideoImage
    {
      get { return this.videoImage; }
    }

    private const long pushTime = 500;
    private double lastTime = 0;
    private Stopwatch watch = new Stopwatch();

    int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
    {
      long time = this.watch.ElapsedMilliseconds;
      if (time - lastTime < pushTime)
      {
        return 0;
      }

      CopyMemory(this.map, pBuffer, bufferLen);
      lastTime = time;


      //// The next operation flips the video 
      //int handle = (int)m_handle;
      //handle += (videoHeight - 1) * stride;
      //m_videoFrame = new Bitmap(videoWidth, videoHeight, -stride, PixelFormat.Format24bppRgb, (IntPtr)handle);

      try
      {
        // Send new image to processing thread
        if (this.NewVideoSample != null)
        {
          VideoSampleArgs args = new VideoSampleArgs();
          args.VideoFrame = this.videoImage;
          this.NewVideoSample(this, args);
        }
      }
      catch (Exception we)
      {
        MessageBox.Show("Error in BufferCB:", we.ToString());
      }

      return 0;
    }

    #endregion

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

      // Grab the size info
      VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
      this.videoWidth = videoInfoHeader.BmiHeader.Width;
      this.videoHeight = videoInfoHeader.BmiHeader.Height;
      this.stride = this.videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

      this.bufferLength = this.videoWidth * this.videoHeight * 3; // RGB24 = 3 bytes

      hr = sampGrabber.SetCallback(this, 1);
      DsError.ThrowExceptionForHR(hr);

      // create memory section and map for the OpenCV Image.
      this.section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);
      this.map = MapViewOfFile(this.section, 0xF001F, 0, 0, (uint)this.bufferLength);

      this.videoImage = new System.Drawing.Bitmap(
  this.videoWidth,
  this.videoHeight,
  this.stride,
  System.Drawing.Imaging.PixelFormat.Format24bppRgb,
  this.map);

      DsUtils.FreeAMMediaType(media);
      media = null;

      watch.Start();
    }

    //private void SetupSampleGrabber(ISampleGrabber sampleGrabber)
    //{
    //  var mediaType = new DirectShowLib.AMMediaType
    //  {
    //    majorType = MediaType.Video,
    //    subType = MediaSubType.RGB24,
    //    formatType = FormatType.VideoInfo
    //  };

    //  int hr = sampleGrabber.SetMediaType(mediaType);

    //  DsUtils.FreeAMMediaType(mediaType);
    //  DsError.ThrowExceptionForHR(hr);

    //  hr = sampleGrabber.SetCallback(this, 1);
    //  DsError.ThrowExceptionForHR(hr);
    //}

    protected override void FreeResources()
    {
      /* We run the StopInternal() to avoid any 
       * Dispatcher VeryifyAccess() issues */
      StopInternal();

      /* Let's clean up the base 
       * class's stuff first */
      base.FreeResources();

#if DEBUG
      if (m_rotEntry != null)
        m_rotEntry.Dispose();

      m_rotEntry = null;
#endif
      if (this.videoImage != null)
      {
        videoImage.Dispose();
        videoImage = null;
      }
      if (m_renderer != null)
      {
        Marshal.FinalReleaseComObject(m_renderer);
        m_renderer = null;
      }
      if (m_captureDevice != null)
      {
        Marshal.FinalReleaseComObject(m_captureDevice);
        m_captureDevice = null;
      }
      if (m_sampleGrabber != null)
      {
        Marshal.FinalReleaseComObject(m_sampleGrabber);
        m_sampleGrabber = null;
      }
      if (m_graph != null)
      {
        Marshal.FinalReleaseComObject(m_graph);
        m_graph = null;

        InvokeMediaClosed(new EventArgs());
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

    }

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

  }
}