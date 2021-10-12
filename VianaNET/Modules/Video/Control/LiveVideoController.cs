// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveVideoController.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   The live video controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Control
{
  using System;
  using System.Runtime.InteropServices;
  using System.Windows.Forms;

  using DirectShowLib;

  using VianaNET.Logging;
  using VianaNET.Modules.Video.Filter;

  /// <summary>
  ///   The live video controller.
  /// </summary>
  internal class LiveVideoController : IDisposable
  {


    /// <summary>
    ///   The never.
    /// </summary>
    private const long NEVER = long.MaxValue;





    /// <summary>
    ///   The capture graph source filter.
    /// </summary>
    private IBaseFilter captureGraphSourceFilter;

    // The file name to capture to
    /// <summary>
    ///   The filename.
    /// </summary>
    private string filename;

    // Running object table classes to show the graphs in GraphEdt

    // The graph that contains the file writer
    /// <summary>
    ///   The filter graph.
    /// </summary>
    private IGraphBuilder filterGraph;

    // The bridge controller that stands between the two graphs
    /// <summary>
    ///   The gmf bridge controller.
    /// </summary>
    private IGMFBridgeController gmfBridgeController;

    /// <summary>
    ///   The m_ capturing.
    /// </summary>
    private bool m_Capturing;

    /// <summary>
    ///   The m_ device selected.
    /// </summary>
    private bool m_DeviceSelected;

    // The capture pin of the capture device (or the smarttee if
    // the capture device has no preview pin)
    /// <summary>
    ///   The m_p cap output.
    /// </summary>
    private IPin m_pCapOutput;

    // The source side of the bridge
    /// <summary>
    ///   The m_p source graph sink filter.
    /// </summary>
    private IBaseFilter m_pSourceGraphSinkFilter;

    /// <summary>
    ///   The m_rot 1.
    /// </summary>
    private DsROTEntry m_rot1;

    /// <summary>
    ///   The m_rot 2.
    /// </summary>
    private DsROTEntry m_rot2;

    /// <summary>
    ///   The source filter graph.
    /// </summary>
    private IGraphBuilder sourceFilterGraph;

    // The other side of the bridge

    /// <summary>
    ///   The video compressor filter.
    /// </summary>
    private IBaseFilter videoCompressorFilter;

    /// <summary>
    ///   The video compressor filter name.
    /// </summary>
    private string videoCompressorFilterName;

    /// <summary>
    ///   The video panel.
    /// </summary>
    private Panel videoPanel;



    // Property to return the current filename

    // Property to report whether we are capturing


    /// <summary>
    ///   Gets a value indicating whether capturing.
    /// </summary>
    public bool Capturing => this.m_Capturing;

    /// <summary>
    ///   Gets the file name.
    /// </summary>
    public string FileName => this.filename;

    // Property to report whether we have a device
    /// <summary>
    ///   Gets a value indicating whether selected.
    /// </summary>
    public bool Selected => this.m_DeviceSelected;

    /// <summary>
    ///   Gets or sets the video compressor name.
    /// </summary>
    public string VideoCompressorName
    {
      get => this.videoCompressorFilterName;

      set
      {
        if (this.videoCompressorFilter != null)
        {
          Marshal.ReleaseComObject(this.videoCompressorFilter);
          this.videoCompressorFilter = null;
        }

        this.videoCompressorFilterName = value;

        // Create the filter for the selected video compressor
        this.videoCompressorFilter = DShowUtils.CreateFilter(
          FilterCategory.VideoCompressorCategory, this.videoCompressorFilterName);
      }
    }





    /// <summary>
    ///   The dispose.
    /// </summary>
    public void Dispose()
    {
      this.StopCapture();

      if (this.sourceFilterGraph is IMediaControl pMC)
      {
        pMC.Stop(); // Ignore any error
      }

      this.ReleaseFilenameMembers();
      this.ReleaseSelectMembers();
    }

    // Specify a device, and a window to draw the preview in
    /// <summary>
    /// The select device.
    /// </summary>
    /// <param name="videoInputDevice">
    /// The video input device. 
    /// </param>
    /// <param name="panel">
    /// The panel. 
    /// </param>
    public void SelectDevice(DsDevice videoInputDevice, Panel panel)
    {
      int hr;
      IBaseFilter inputDeviceFilter = null;
      ICaptureGraphBuilder2 sourceCaptureGraphBuilder = null;

      this.ReleaseSelectMembers();

      this.videoPanel = panel;
      this.videoPanel.Resize += this.videoPanel_Resize;

      // release any leftovers
      try
      {
        // create source graph and add sink filter
        this.sourceFilterGraph = (IGraphBuilder)new FilterGraph();
        this.m_rot1 = new DsROTEntry(this.sourceFilterGraph);

        this.gmfBridgeController = (IGMFBridgeController)new GMFBridgeController();

        // init to video-only, in discard mode (ie when source graph
        // is running but not connected, buffers are discarded at the bridge)
        hr = this.gmfBridgeController.AddStream(true, eFormatType.MuxInputs, true);
        DsError.ThrowExceptionForHR(hr);

        // Add the requested device
        hr = ((IFilterGraph2)this.sourceFilterGraph).AddSourceFilterForMoniker(
          videoInputDevice.Mon, null, videoInputDevice.Name, out inputDeviceFilter);
        DsError.ThrowExceptionForHR(hr);

        // Add the sink filter to the source graph
        hr = this.gmfBridgeController.InsertSinkFilter(this.sourceFilterGraph, out this.m_pSourceGraphSinkFilter);
        DsError.ThrowExceptionForHR(hr);

        // use capture graph builder to render preview
        sourceCaptureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        // Init the CaptureGraphBuilder2
        hr = sourceCaptureGraphBuilder.SetFiltergraph(this.sourceFilterGraph);
        DsError.ThrowExceptionForHR(hr);

        // Connect the filters together to allow preview
        hr = sourceCaptureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, inputDeviceFilter, null, null);
        DsError.ThrowExceptionForHR(hr);

        // connect capture output to the pseudo-sink filter,
        // where it will be discarded until required
        hr = sourceCaptureGraphBuilder.RenderStream(
          PinCategory.Capture, MediaType.Video, inputDeviceFilter, null, this.m_pSourceGraphSinkFilter);
        DsError.ThrowExceptionForHR(hr);

        // turn off capture stream if possible except when capturing
        hr = sourceCaptureGraphBuilder.FindPin(
          inputDeviceFilter, PinDirection.Output, PinCategory.Capture, MediaType.Video, false, 0, out this.m_pCapOutput);
        if (hr >= 0)
        {
          IAMStreamControl pSC = (IAMStreamControl)this.m_pCapOutput;
          pSC.StartAt(NEVER, 0); // Ignore any error
        }

        this.ConfigureVideo();

        IMediaControl pMC = (IMediaControl)this.sourceFilterGraph;

        hr = pMC.Run();
        DsError.ThrowExceptionForHR(hr);

        // If we made it here, the device is selected
        this.m_DeviceSelected = true;
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, true);
        this.ReleaseSelectMembers();
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

    // Given a file name, build the output graph
    /// <summary>
    /// The set next filename.
    /// </summary>
    /// <param name="localFilename">
    /// The local filename. 
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public void SetNextFilename(string localFilename)
    {
      int hr;

      ICaptureGraphBuilder2 captureGraphBuilder = null;
      IBaseFilter muxFilter = null;
      IFileSinkFilter fileSinkFilter = null;

      if (localFilename != null)
      {
        if (this.m_DeviceSelected)
        {
          this.ReleaseFilenameMembers();

          this.filterGraph = (IGraphBuilder)new FilterGraph();
          try
          {
            this.m_rot2 = new DsROTEntry(this.filterGraph);

            // Use the bridge to add the sourcefilter to the graph
            hr = this.gmfBridgeController.InsertSourceFilter(
              this.m_pSourceGraphSinkFilter, this.filterGraph, out this.captureGraphSourceFilter);
            DsError.ThrowExceptionForHR(hr);

            // use capture graph builder to create mux/writer stage
            captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

            hr = captureGraphBuilder.SetFiltergraph(this.filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // create the mux/writer
            hr = captureGraphBuilder.SetOutputFileName(
              MediaSubType.Avi, localFilename, out muxFilter, out fileSinkFilter);
            DsError.ThrowExceptionForHR(hr);

            // Add video compressor filter
            if (this.videoCompressorFilter != null)
            {
              hr = this.filterGraph.AddFilter(this.videoCompressorFilter, "Video Compressor");
              DsError.ThrowExceptionForHR(hr);
            }

            // render source output to mux
            hr = captureGraphBuilder.RenderStream(
              null, null, this.captureGraphSourceFilter, this.videoCompressorFilter, muxFilter);
            DsError.ThrowExceptionForHR(hr);

            // Store the file name for later use
            this.filename = localFilename;
          }
          catch
          {
            this.ReleaseFilenameMembers();
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

    /// <summary>
    ///   The show video compressor options dialog.
    /// </summary>
    public void ShowVideoCompressorOptionsDialog()
    {
      if (this.videoCompressorFilter != null)
      {
        DShowUtils.DisplayPropertyPage(IntPtr.Zero, this.videoCompressorFilter);
      }
    }

    // Release all the members of the capture graph

    // Start capturing to the file specified to SetNextFilename
    /// <summary>
    ///   The start capture.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void StartCapture()
    {
      int hr;

      if (this.m_DeviceSelected)
      {
        if (this.filename != null)
        {
          // re-enable capture stream
          IAMStreamControl pSC = (IAMStreamControl)this.m_pCapOutput;

          // immediately!
          pSC.StartAt(null, 0); // Ignore any error

          // start capture graph
          IMediaControl pMC = (IMediaControl)this.filterGraph;
          hr = pMC.Run();
          DsError.ThrowExceptionForHR(hr);

          hr = this.gmfBridgeController.BridgeGraphs(this.m_pSourceGraphSinkFilter, this.captureGraphSourceFilter);
          DsError.ThrowExceptionForHR(hr);

          // If we make it here, we are capturing
          this.m_Capturing = true;
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
    /// <summary>
    ///   The stop capture.
    /// </summary>
    public void StopCapture()
    {
      int hr;

      // Are we capturing?
      if (this.m_Capturing)
      {
        // disconnect segments
        hr = this.gmfBridgeController.BridgeGraphs(null, null);
        DsError.ThrowExceptionForHR(hr);

        // stop capture graph
        IMediaControl pMC = (IMediaControl)this.filterGraph;

        hr = pMC.Stop();
        DsError.ThrowExceptionForHR(hr);

        // disable capture stream (to save resources)
        IAMStreamControl pSC = (IAMStreamControl)this.m_pCapOutput;

        pSC.StartAt(NEVER, 0); // Ignore any error

        this.m_Capturing = false;
      }
    }



    // Configure the video to fit in the provided window (if any)


    /// <summary>
    ///   The configure video.
    /// </summary>
    private void ConfigureVideo()
    {
      int hr;

      if (this.videoPanel.Handle != IntPtr.Zero)
      {
        int cx, cy;
        IBasicVideo pBV = (IBasicVideo)this.sourceFilterGraph;

        hr = pBV.GetVideoSize(out cx, out cy);
        DsError.ThrowExceptionForHR(hr);

        // reparent playback window
        IVideoWindow pVW = (IVideoWindow)this.sourceFilterGraph;

        hr = pVW.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings | WindowStyle.ClipChildren);
        DsError.ThrowExceptionForHR(hr);

        hr = pVW.put_Owner(this.videoPanel.Handle);
        DsError.ThrowExceptionForHR(hr);

        hr = pVW.SetWindowPosition(0, 0, this.videoPanel.Width, this.videoPanel.Height);
        DsError.ThrowExceptionForHR(hr);
      }
    }

    /// <summary>
    ///   The release filename members.
    /// </summary>
    private void ReleaseFilenameMembers()
    {
      if (this.m_rot2 != null)
      {
        this.m_rot2.Dispose();
        this.m_rot2 = null;
      }

      if (this.captureGraphSourceFilter != null)
      {
        Marshal.ReleaseComObject(this.captureGraphSourceFilter);
        this.captureGraphSourceFilter = null;
      }

      if (this.filterGraph != null)
      {
        Marshal.ReleaseComObject(this.filterGraph);
        this.filterGraph = null;
      }

      if (this.videoCompressorFilter != null)
      {
        Marshal.ReleaseComObject(this.videoCompressorFilter);
        this.videoCompressorFilter = null;
      }
    }

    /// <summary>
    ///   The release select members.
    /// </summary>
    private void ReleaseSelectMembers()
    {
      if (this.m_rot1 != null)
      {
        this.m_rot1.Dispose();
        this.m_rot1 = null;
      }

      if (this.videoPanel != null)
      {
        this.videoPanel.Resize -= this.videoPanel_Resize;
      }

      if (this.gmfBridgeController != null)
      {
        Marshal.ReleaseComObject(this.gmfBridgeController);
        this.gmfBridgeController = null;
      }

      if (this.m_pSourceGraphSinkFilter != null)
      {
        Marshal.ReleaseComObject(this.m_pSourceGraphSinkFilter);
        this.m_pSourceGraphSinkFilter = null;
      }

      if (this.m_pSourceGraphSinkFilter != null)
      {
        Marshal.ReleaseComObject(this.m_pSourceGraphSinkFilter);
        this.m_pSourceGraphSinkFilter = null;
      }

      if (this.m_pCapOutput != null)
      {
        Marshal.ReleaseComObject(this.m_pCapOutput);
        this.m_pCapOutput = null;
      }

      if (this.sourceFilterGraph != null)
      {
        Marshal.ReleaseComObject(this.sourceFilterGraph);
        this.sourceFilterGraph = null;
      }
    }

    /// <summary>
    /// The video panel_ resize.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void videoPanel_Resize(object sender, EventArgs e)
    {
      // reparent playback window
      IVideoWindow pVW = (IVideoWindow)this.sourceFilterGraph;
      int hr = pVW.SetWindowPosition(0, 0, this.videoPanel.Width, this.videoPanel.Height);
      DsError.ThrowExceptionForHR(hr);
    }


  }
}