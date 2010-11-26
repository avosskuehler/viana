/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Runtime.InteropServices;

using DirectShowLib;
using DirectShowLib.GDCL;

namespace VianaNET
{
  class LiveVideoController : IDisposable
  {
    #region Member variables

    const long NEVER = long.MaxValue;

    // The file name to capture to
    private string filename;

    // Running object table classes to show the graphs in GraphEdt
    private DsROTEntry m_rot1;
    private DsROTEntry m_rot2;

    // Has a device been selected yet?
    private bool m_DeviceSelected = false;

    // Are we actively capturing?
    private bool m_Capturing = false;

    // The graph that contains the source device
    private IGraphBuilder sourceFilterGraph;

    // The graph that contains the file writer
    private IGraphBuilder filterGraph;

    // The bridge controller that stands between the two graphs
    private IGMFBridgeController gmfBridgeController;

    // The capture pin of the capture device (or the smarttee if
    // the capture device has no preview pin)
    private IPin m_pCapOutput;

    // The source side of the bridge
    private IBaseFilter m_pSourceGraphSinkFilter;

    // The other side of the bridge
    private IBaseFilter captureGraphSourceFilter;

    private IBaseFilter videoCompressorFilter;
    private string videoCompressorFilterName;

    #endregion

    private System.Windows.Forms.Panel videoPanel;

    // Property to return the current filename
    public string FileName
    {
      get { return filename; }
    }

    // Property to report whether we are capturing
    public bool Capturing
    {
      get { return m_Capturing; }
    }

    // Property to report whether we have a device
    public bool Selected
    {
      get { return m_DeviceSelected; }
    }

    // Specify a device, and a window to draw the preview in
    public void SelectDevice(DsDevice videoInputDevice, string videoCompressorName, System.Windows.Forms.Panel panel)
    {
      int hr;
      IBaseFilter inputDeviceFilter = null;
      ICaptureGraphBuilder2 sourceCaptureGraphBuilder = null;

      ReleaseSelectMembers();

      this.videoPanel = panel;
      this.videoPanel.Resize += new EventHandler(videoPanel_Resize);
      this.videoCompressorFilterName = videoCompressorName;

      // release any leftovers

      try
      {
        // create source graph and add sink filter
        sourceFilterGraph = (IGraphBuilder)new FilterGraph();
        m_rot1 = new DsROTEntry(sourceFilterGraph);

        gmfBridgeController = (IGMFBridgeController)new GMFBridgeController();

        // init to video-only, in discard mode (ie when source graph
        // is running but not connected, buffers are discarded at the bridge)
        hr = gmfBridgeController.AddStream(true, eFormatType.MuxInputs, true);
        DsError.ThrowExceptionForHR(hr);

        // Add the requested device
        hr = ((IFilterGraph2)sourceFilterGraph).AddSourceFilterForMoniker(videoInputDevice.Mon, null, videoInputDevice.Name, out inputDeviceFilter);
        DsError.ThrowExceptionForHR(hr);

        // Add the sink filter to the source graph
        hr = gmfBridgeController.InsertSinkFilter(sourceFilterGraph, out m_pSourceGraphSinkFilter);
        DsError.ThrowExceptionForHR(hr);

        // use capture graph builder to render preview
        sourceCaptureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        // Init the CaptureGraphBuilder2
        hr = sourceCaptureGraphBuilder.SetFiltergraph(sourceFilterGraph);
        DsError.ThrowExceptionForHR(hr);

        // Connect the filters together to allow preview
        hr = sourceCaptureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, inputDeviceFilter, null, null);
        DsError.ThrowExceptionForHR(hr);

        // connect capture output to the pseudo-sink filter,
        // where it will be discarded until required
        hr = sourceCaptureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, inputDeviceFilter, null, m_pSourceGraphSinkFilter);
        DsError.ThrowExceptionForHR(hr);

        // turn off capture stream if possible except when capturing
        hr = sourceCaptureGraphBuilder.FindPin(inputDeviceFilter, PinDirection.Output, PinCategory.Capture, MediaType.Video, false, 0, out m_pCapOutput);
        if (hr >= 0)
        {
          IAMStreamControl pSC = (IAMStreamControl)m_pCapOutput;
          pSC.StartAt(NEVER, 0);  // Ignore any error
        }

        ConfigureVideo();

        IMediaControl pMC = (IMediaControl)sourceFilterGraph;

        hr = pMC.Run();
        DsError.ThrowExceptionForHR(hr);

        // If we made it here, the device is selected
        m_DeviceSelected = true;
      }
      catch
      {
        ReleaseSelectMembers();
        throw;
      }
      finally
      {
        if (sourceCaptureGraphBuilder != null)
        {
          Marshal.ReleaseComObject(sourceCaptureGraphBuilder);
        }

        if (inputDeviceFilter != null)
        {
          Marshal.ReleaseComObject(inputDeviceFilter);
        }
      }
    }

    void videoPanel_Resize(object sender, EventArgs e)
    {
      // reparent playback window
      IVideoWindow pVW = (IVideoWindow)sourceFilterGraph;
      int hr = pVW.SetWindowPosition(0, 0, this.videoPanel.Width, this.videoPanel.Height);
      DsError.ThrowExceptionForHR(hr);
    }

    // Release all the members of the source graph
    private void ReleaseSelectMembers()
    {
      if (m_rot1 != null)
      {
        m_rot1.Dispose();
        m_rot1 = null;
      }

      if (this.videoPanel != null)
      {
        this.videoPanel.Resize -= new EventHandler(videoPanel_Resize);
      }

      if (gmfBridgeController != null)
      {
        Marshal.ReleaseComObject(gmfBridgeController);
        gmfBridgeController = null;
      }

      if (m_pSourceGraphSinkFilter != null)
      {
        Marshal.ReleaseComObject(m_pSourceGraphSinkFilter);
        m_pSourceGraphSinkFilter = null;
      }

      if (m_pSourceGraphSinkFilter != null)
      {
        Marshal.ReleaseComObject(m_pSourceGraphSinkFilter);
        m_pSourceGraphSinkFilter = null;
      }

      if (m_pCapOutput != null)
      {
        Marshal.ReleaseComObject(m_pCapOutput);
        m_pCapOutput = null;
      }

      if (sourceFilterGraph != null)
      {
        Marshal.ReleaseComObject(sourceFilterGraph);
        sourceFilterGraph = null;
      }
    }

    // Given a file name, build the output graph
    public void SetNextFilename(string localFilename)
    {
      int hr;

      ICaptureGraphBuilder2 captureGraphBuilder = null;
      IBaseFilter muxFilter = null;
      IFileSinkFilter fileSinkFilter = null;

      if (localFilename != null)
      {
        if (m_DeviceSelected)
        {
          ReleaseFilenameMembers();

          filterGraph = (IGraphBuilder)new FilterGraph();
          try
          {
            m_rot2 = new DsROTEntry(filterGraph);

            // Use the bridge to add the sourcefilter to the graph
            hr = gmfBridgeController.InsertSourceFilter(m_pSourceGraphSinkFilter, filterGraph, out captureGraphSourceFilter);
            DsError.ThrowExceptionForHR(hr);

            // use capture graph builder to create mux/writer stage
            captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

            hr = captureGraphBuilder.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // create the mux/writer
            hr = captureGraphBuilder.SetOutputFileName(MediaSubType.Avi, localFilename, out muxFilter, out fileSinkFilter);
            DsError.ThrowExceptionForHR(hr);

            if (this.videoCompressorFilter == null)
            {
              // Get the video compressor and add it to the filter graph
              // Create the filter for the selected video compressor
              this.videoCompressorFilter = DShowUtils.CreateFilter(
                FilterCategory.VideoCompressorCategory,
                this.videoCompressorFilterName);
            }

            hr = filterGraph.AddFilter(this.videoCompressorFilter, "Video Compressor");
            DsError.ThrowExceptionForHR(hr);

            // render source output to mux
            hr = captureGraphBuilder.RenderStream(null, null, captureGraphSourceFilter, this.videoCompressorFilter, muxFilter);
            DsError.ThrowExceptionForHR(hr);

            // Store the file name for later use
            filename = localFilename;
          }
          catch
          {
            ReleaseFilenameMembers();
          }
          finally
          {
            if (captureGraphBuilder != null)
            {
              Marshal.ReleaseComObject(captureGraphBuilder);
            }

            if (muxFilter != null)
            {
              Marshal.ReleaseComObject(muxFilter);
            }

            if (fileSinkFilter != null)
            {
              Marshal.ReleaseComObject(fileSinkFilter);
            }
          }
        }
        else
        {
          throw new Exception("Device not selected");
        }
      }
      else
      {
        throw new Exception("Invalid parameter");
      }
    }

    // Release all the members of the capture graph
    private void ReleaseFilenameMembers()
    {
      if (m_rot2 != null)
      {
        m_rot2.Dispose();
        m_rot2 = null;
      }

      if (captureGraphSourceFilter != null)
      {
        Marshal.ReleaseComObject(captureGraphSourceFilter);
        captureGraphSourceFilter = null;
      }

      if (filterGraph != null)
      {
        Marshal.ReleaseComObject(filterGraph);
        filterGraph = null;
      }

      if (videoCompressorFilter != null)
      {
        Marshal.ReleaseComObject(videoCompressorFilter);
        videoCompressorFilter = null;
      }
    }

    // Start capturing to the file specified to SetNextFilename
    public void StartCapture()
    {
      int hr;

      if (m_DeviceSelected)
      {
        if (filename != null)
        {
          // re-enable capture stream
          IAMStreamControl pSC = (IAMStreamControl)m_pCapOutput;

          // immediately!
          pSC.StartAt(null, 0); // Ignore any error

          // start capture graph
          IMediaControl pMC = (IMediaControl)filterGraph;
          hr = pMC.Run();
          DsError.ThrowExceptionForHR(hr);

          hr = gmfBridgeController.BridgeGraphs(m_pSourceGraphSinkFilter, captureGraphSourceFilter);
          DsError.ThrowExceptionForHR(hr);

          // If we make it here, we are capturing
          m_Capturing = true;
        }
        else
        {
          throw new Exception("File name not specified");
        }
      }
      else
      {
        throw new Exception("Device not selected");
      }
    }

    // Stop the file capture (leave the preview running)
    public void StopCapture()
    {
      int hr;

      // Are we capturing?
      if (m_Capturing)
      {
        // disconnect segments
        hr = gmfBridgeController.BridgeGraphs(null, null);
        DsError.ThrowExceptionForHR(hr);

        // stop capture graph
        IMediaControl pMC = (IMediaControl)filterGraph;

        hr = pMC.Stop();
        DsError.ThrowExceptionForHR(hr);

        // disable capture stream (to save resources)
        IAMStreamControl pSC = (IAMStreamControl)m_pCapOutput;

        pSC.StartAt(NEVER, 0); // Ignore any error

        m_Capturing = false;
      }
    }

    // Configure the video to fit in the provided window (if any)
    private void ConfigureVideo()
    {
      int hr;

      if (this.videoPanel.Handle != IntPtr.Zero)
      {
        int cx, cy;
        IBasicVideo pBV = (IBasicVideo)sourceFilterGraph;

        hr = pBV.GetVideoSize(out cx, out cy);
        DsError.ThrowExceptionForHR(hr);

        // reparent playback window
        IVideoWindow pVW = (IVideoWindow)sourceFilterGraph;

        hr = pVW.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings | WindowStyle.ClipChildren);
        DsError.ThrowExceptionForHR(hr);

        hr = pVW.put_Owner(this.videoPanel.Handle);
        DsError.ThrowExceptionForHR(hr);

        hr = pVW.SetWindowPosition(0, 0, this.videoPanel.Width, this.videoPanel.Height);
        DsError.ThrowExceptionForHR(hr);
      }
    }

    public void Dispose()
    {
      StopCapture();

      IMediaControl pMC = sourceFilterGraph as IMediaControl;
      if (pMC != null)
      {
        pMC.Stop();  // Ignore any error
      }

      ReleaseFilenameMembers();
      ReleaseSelectMembers();
    }
  }
}
