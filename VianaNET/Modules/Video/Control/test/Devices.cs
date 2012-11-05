namespace VianaNET
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;
  using DirectShowLib;

  /// <summary>
  /// This singleton class provides the available video capture devices
  /// of the system.
  /// </summary>
  public class Devices
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
    /// The static instance of the singleton class.
    /// </summary>
    private static Devices current = new Devices();

    private List<CameraInfo> cameraDevices;

    private ICaptureGraphBuilder2 capGraph;
    private IBaseFilter capFilter;
    private IFilterGraph2 m_graphBuilder = null;
    private IntPtr pscc;

    private int minHeight = 0;
    private int maxHeight = 10000;
    private int minWidth = 0;
    private int maxWidth = 10000;
    private int minFps = 1;
    private int maxFps = 10000;

    #endregion //FIELDS

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region CONSTRUCTION

    /// <summary>
    /// Prevents a default instance of the Devices class from being created.
    /// </summary>
    private Devices()
    {
      cameraDevices = getDevices();
      clean();
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
    /// Gets the singleton instance of the devices class
    /// that contains the current devices.
    /// </summary>
    public static Devices Current
    {
      get { return current; }
    }

    /// <summary>
    /// Gets the list of available capture devices
    /// </summary>
    public List<CameraInfo> Cameras
    {
      get { return this.cameraDevices; }
    }

    #endregion //PROPERTIES

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////
    #region PUBLICMETHODS
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
    /// Iterates through the list of available VideoInputDevice
    /// and fills CameraInfo if the device is not in use by another application.
    /// </summary>
    /// <returns>A <see cref="List{CameraInfo}"/> with the usable video capture devices.</returns>
    private List<CameraInfo> getDevices()
    {
      this.cameraDevices = new List<CameraInfo>();

      foreach (DsDevice device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
      {
        CameraInfo deviceInfo = Caps(device);
        if (deviceInfo != null)
        {
          deviceInfo.DirectshowDevice = device;
          cameraDevices.Add(deviceInfo);
        }
      }

      return cameraDevices;
    }

    /// <summary>
    /// Returns the <see cref="CameraInfo"/> for the given <see cref="DsDevice"/>.
    /// </summary>
    /// <param name="dev">A <see cref="DsDevice"/> to parse name and capabilities for.</param>
    /// <returns>The <see cref="CameraInfo"/> for the given device.</returns>
    private CameraInfo Caps(DsDevice dev)
    {
      CameraInfo camerainfo = new CameraInfo();

      try
      {
        int hr;

        // Get the graphbuilder object
        m_graphBuilder = (IFilterGraph2)new FilterGraph();

        // Get the ICaptureGraphBuilder2
        capGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        // Add the video device
        hr = m_graphBuilder.AddSourceFilterForMoniker(dev.Mon, null, "Video input", out capFilter);
        //DsError.ThrowExceptionForHR(hr);

        if (hr != 0)
        {
          return null;
        }


        hr = capGraph.SetFiltergraph(m_graphBuilder);
        DsError.ThrowExceptionForHR(hr);

        hr = m_graphBuilder.AddFilter(capFilter, "Ds.NET Video Capture Device");
        DsError.ThrowExceptionForHR(hr);


        object o = null;
        DsGuid cat = PinCategory.Capture;
        DsGuid type = MediaType.Interleaved;
        DsGuid iid = typeof(IAMStreamConfig).GUID;

        // Check if Video capture filter is in use
        hr = capGraph.RenderStream(cat, MediaType.Video, capFilter, null, null);
        if (hr != 0)
        {
          return null;
        }

        //hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Interleaved, capFilter, typeof(IAMStreamConfig).GUID, out o);
        //if (hr != 0)
        //{
        hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out o);
        DsError.ThrowExceptionForHR(hr);
        //}

        IAMStreamConfig videoStreamConfig = o as IAMStreamConfig;

        int iCount = 0;
        int iSize = 0;

        try
        {
          videoStreamConfig.GetNumberOfCapabilities(out iCount, out iSize);
        }
        catch (Exception ex)
        {
          ErrorLogger.ProcessException(ex, false);
          return null;
        }

        pscc = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(VideoStreamConfigCaps)));

        camerainfo.Name = dev.Name;
        camerainfo.DirectshowDevice = dev;

        for (int i = 0; i < iCount; i++)
        {
          AMMediaType curMedType;
          VideoStreamConfigCaps scc;

          try
          {

            hr = videoStreamConfig.GetStreamCaps(i, out curMedType, pscc);
            Marshal.ThrowExceptionForHR(hr);
            scc = (VideoStreamConfigCaps)Marshal.PtrToStructure(pscc, typeof(VideoStreamConfigCaps));


            CamSizeFPS CSF = new CamSizeFPS();
            CSF.FPS = (int)(10000000 / scc.MinFrameInterval);
            CSF.Height = scc.InputSize.Height;
            CSF.Width = scc.InputSize.Width;

            if (!inSizeFpsList(camerainfo.SupportedSizesAndFPS, CSF))
              if (ParametersOK(CSF))
                camerainfo.SupportedSizesAndFPS.Add(CSF);
          }
          catch (Exception ex)
          {
            ErrorLogger.ProcessException(ex, false);
          }

        }

      }
      finally
      {
      }

      return camerainfo;
    }

    /// <summary>
    /// Cleans up the test graph.
    /// </summary>
    private void clean()
    {
      if (m_graphBuilder != null)
      {
        Marshal.ReleaseComObject(m_graphBuilder);
        m_graphBuilder = null;
      }
      if (capFilter != null)
      {
        Marshal.ReleaseComObject(capFilter);
        capFilter = null;
      }
      if (capGraph != null)
      {
        Marshal.ReleaseComObject(capGraph);
        capGraph = null;
      }

      Marshal.FreeCoTaskMem(pscc);
    }

    #endregion //PRIVATEMETHODS

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region HELPER

    /// <summary>
    /// loop through the list of the differnet sizes and FPS
    /// </summary>
    /// <param name="lst"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    private bool inSizeFpsList(List<CamSizeFPS> lst, CamSizeFPS instance)
    {
      foreach (CamSizeFPS c in lst)
        if (c.CompareTo(instance) == 0)
          return true;
      return false;
    }

    private bool ParametersOK(CamSizeFPS instance)
    {
      if (instance.FPS < minFps || instance.FPS > maxFps)
        return false;

      if (instance.Width < minWidth || instance.Width > maxWidth)
        return false;

      if (instance.Height < minHeight || instance.Height > maxHeight)
        return false;

      return true;
    }

    private int SupportedResolution(int deviceNumber, int width, int height)
    {
      int highestfps = 0;
      foreach (CamSizeFPS csf in cameraDevices[deviceNumber].SupportedSizesAndFPS)
      {
        if (csf.Height == height && csf.Width == width)
        {
          if (csf.FPS > highestfps)
            highestfps = csf.FPS;
        }
      }
      return highestfps;
    }

    private CameraInfo getFirstAvailableCamera()
    {
      foreach (CameraInfo ci in cameraDevices)
      {
        if (ci.DirectshowDevice != null)
          return ci;
      }
      return null;
    }

    #endregion //HELPER
  }
}