// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoBase.cs" company="Freie Universität Berlin">
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
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Threading;

  using AForge.Imaging;
  using OpenCvSharp;
  using OpenCvSharp.WpfExtensions;
  using VianaNET.CustomStyles.Types;
  using VianaNET.Logging;

  /// <summary>
  ///   This is the main class for the DirectShow interop.
  ///   It creates a graph that pushes video frames from a Video Input Device
  ///   through the filter chain to a SampleGrabber, from which the
  ///   frames can be catched and send into the processing tree of
  ///   the application.
  /// </summary>
  public abstract class VideoBase : DependencyObject, IDisposable
  {
    /// <summary>
    ///   The current state property.
    /// </summary>
    public static readonly DependencyProperty CurrentStateProperty = DependencyProperty.Register(
      "CurrentState", typeof(PlayState), typeof(VideoBase), new UIPropertyMetadata(PlayState.Stopped));

    /// <summary>
    ///   The frame count property.
    /// </summary>
    public static readonly DependencyProperty FrameCountProperty = DependencyProperty.Register(
      "FrameCount", typeof(int), typeof(VideoBase), new UIPropertyMetadata(default(int)));

    /// <summary>
    ///   The frame time in milli seconds property.
    /// </summary>
    public static readonly DependencyProperty FrameTimeInMSProperty =
      DependencyProperty.Register(
        "FrameTimeInMS", typeof(double), typeof(VideoBase), new UIPropertyMetadata(default(double)));

    /// <summary>
    ///   The media position frame index property.
    /// </summary>
    public static readonly DependencyProperty MediaPositionFrameIndexProperty =
      DependencyProperty.Register(
        "MediaPositionFrameIndex", typeof(int), typeof(VideoBase), new UIPropertyMetadata(default(int)));

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Saves the bufferLength of the video stream
    /// </summary>
    protected int bufferLength;

    /// <summary>
    ///   The frame counter.
    /// </summary>
    protected int frameCounter;

    /// <summary>
    ///   This points to the starting address of the mapped view of the video frame.
    /// </summary>
    private IntPtr originalMapping;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr originalSection;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr colorProcessingSection;

    /// <summary>
    ///   This points to a file mapping of the video frames.
    /// </summary>
    private IntPtr motionProcessingSection;

    /// <summary>
    ///   This indicates if the frames of the capture callback should be skipped.
    /// </summary>
    protected bool skipFrameMode = false;
    private RotateFlags? rotation;

    /// <summary>
    /// The OpenCV VideoCapture Device Control
    /// </summary>
    public VideoCapture OpenCVObject { get; private set; }

    /// <summary>
    /// Frame Available Background worker process
    /// </summary>
    protected readonly BackgroundWorker bkgWorker;

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////

    public VideoBase()
    {
      this.bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
      this.bkgWorker.DoWork += this.Worker_DoWork;

      this.OpenCVObject = new VideoCapture();
      this.OpenCVObject.SetExceptionMode(true);
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="VideoBase" /> class.
    /// </summary>
    ~VideoBase()
    {
      this.Dispose();

      if (this.OpenCVObject != null)
      {
        this.OpenCVObject.Dispose();
      }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   The video available.
    /// </summary>
    public event EventHandler VideoAvailable;

    /// <summary>
    ///   The video frame changed.
    /// </summary>
    public event EventHandler VideoFrameChanged;

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

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///   Gets or sets the current state.
    /// </summary>
    public PlayState CurrentState
    {
      get => (PlayState)this.GetValue(CurrentStateProperty);

      set => this.SetValue(CurrentStateProperty, value);
    }

    /// <summary>
    ///   Gets or sets the frame count.
    /// </summary>
    public int FrameCount
    {
      get => (int)this.GetValue(FrameCountProperty);

      set => this.SetValue(FrameCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the time between frames in milliseconds.
    /// </summary>
    public double FrameTimeInMS
    {
      get => (double)this.GetValue(FrameTimeInMSProperty);

      set => this.SetValue(FrameTimeInMSProperty, value);
    }

    /// <summary>
    ///   Gets or sets the media position frame index.
    /// </summary>
    public int MediaPositionFrameIndex
    {
      get => (int)this.GetValue(MediaPositionFrameIndexProperty);

      set => this.SetValue(MediaPositionFrameIndexProperty, value);
    }

    /// <summary>
    ///   Gets or sets the media position in milli seconds.
    /// </summary>
    public virtual double MediaPositionInMS { get; set; }

    /// <summary>
    ///   Gets or sets the rotation of the video.
    /// </summary>
    public RotateFlags? Rotation
    {
      get => rotation;
      set
      {
        rotation = value;
        this.ReleaseMappings();
        this.SaveSizeInfo(this.OpenCVObject);
        App.Project.ProcessingData.InitializeImageFilters();
        this.GrabCurrentFrame();
      }
    }

    /// <summary>
    ///   Gets or sets the natural video height.
    /// </summary>
    public double NaturalVideoHeight { get; set; }

    /// <summary>
    ///   Gets or sets the natural video width.
    /// </summary>
    public double NaturalVideoWidth { get; set; }

    /// <summary>
    ///  Gets or sets the pixel size of the video stream.
    ///   RGB24 = 3 bytes
    /// </summary>
    public int PixelSize { get; set; }

    /// <summary>
    ///  Gets or sets the starting address of the mapped view of the video frame.
    /// Used for the motion processing.
    /// </summary>
    public IntPtr MotionProcessingMapping { get; set; }

    /// <summary>
    ///  Gets or sets the starting address of the mapped view of the video frame.
    /// Used for the color processing.
    /// </summary>
    public IntPtr ColorProcessingMapping { get; set; }

    /// <summary>
    ///   Gets or sets the stride of the video stream
    /// </summary>
    public int Stride { get; set; }

    /// <summary>
    ///   Gets or sets the unmanaged image needed for the aforge motion detecton
    /// </summary>
    public UnmanagedImage UnmanagedImage { get; set; }


    /// <summary>
    ///   Shut down capture.
    ///   This is used to release all resources needed by the capture graph.
    /// </summary>
    public virtual void Dispose()
    {
      this.Stop();

      if (this.bkgWorker != null && this.bkgWorker.IsBusy)
      {
        this.bkgWorker.CancelAsync();
      }

      while (this.bkgWorker.IsBusy)
      {
        UiServices.WaitUntilReady();
      }


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

        if (!this.OpenCVObject.IsDisposed && this.OpenCVObject.IsOpened())
        {
          this.OpenCVObject.Release();
        }

        ReleaseMappings();
      }

      // Double check to make sure we aren't releasing something
      // important.
      GC.Collect();
    }

    private void ReleaseMappings()
    {
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
    }

    /// <summary>
    ///   Pause the capture graph. Running the graph takes up a lot of resources.  
    ///   Pause it when it isn't needed.
    /// </summary>
    public virtual void Pause()
    {
      if (this.bkgWorker.IsBusy)
      {
        this.bkgWorker.CancelAsync();
      }
      else
      {
        //this.bkgWorker.RunWorkerAsync();
      }
      this.CurrentState = PlayState.Paused;
    }

    /// <summary>
    ///   Start the capture graph
    /// </summary>
    public virtual void Play()
    {
      if (!this.bkgWorker.IsBusy)
      {
        this.bkgWorker.RunWorkerAsync();
      }
      this.CurrentState = PlayState.Running;
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
    /// Stops the video capturing or playing without a rewind.
    /// </summary>
    public virtual void Stop()
    {
      try
      {
        this.bkgWorker.CancelAsync();
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render,
          (SendOrPostCallback)delegate { this.CurrentState = PlayState.Stopped; },
          null);
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
    }

    /// <summary>
    ///   The refresh processing map.
    /// </summary>
    public void RefreshProcessingMap()
    {
      CopyMemory(this.ColorProcessingMapping, this.originalMapping, this.bufferLength);
    }

    public void CopyProcessedDataToProcessingMap()
    {
      CopyMemory(this.MotionProcessingMapping, this.UnmanagedImage.ImageData, this.bufferLength);
    }

    public void UpdateProcessedImageSource()
    {
      if (App.Project.ProcessingData.IsUsingColorDetection && Video.Instance.ColorProcessedImageSource != null)
      {
        // Update ColorProcessedVideoSource
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render,
          (SendOrPostCallback)delegate { ((InteropBitmap)Video.Instance.ColorProcessedImageSource).Invalidate(); },
          null);
      }

      if (App.Project.ProcessingData.IsUsingMotionDetection && Video.Instance.MotionProcessedImageSource != null)
      {
        // Update MotionProcessedVideoSource
        this.Dispatcher.BeginInvoke(
          DispatcherPriority.Render,
          (SendOrPostCallback)delegate { ((InteropBitmap)Video.Instance.MotionProcessedImageSource).Invalidate(); },
          null);
      }
    }

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

    public void CopyProcessingMapToUnmanagedImage()
    {
      this.UnmanagedImage = new UnmanagedImage(this.ColorProcessingMapping, (int)this.NaturalVideoWidth, (int)this.NaturalVideoHeight, this.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      CopyMemory(this.UnmanagedImage.ImageData, this.ColorProcessingMapping, this.bufferLength);
    }



    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   Gets called whenever a new frame arrives
    ///   Updates the memory mapping of the OpenCV image and raises the 
    ///   <see cref="FrameCaptureComplete"/> event.
    /// </summary>
    public void NewFrameCallback(WriteableBitmap newFrame)
    {
      this.frameCounter++;

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
            CopyMemory(this.originalMapping, newFrame.BackBuffer, this.bufferLength);

            this.Dispatcher.BeginInvoke(
              DispatcherPriority.Render,
              (SendOrPostCallback)delegate
              {
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

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////

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
    /// A pointer to a SECURITY_ATTRIBUTES structure that determines whether a returned handle can be inherited by child processes. 
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
    /// If the function succeeds, the return value is a handle to the file mapping object. If the object exists before the function call, the function returns a handle to the existing object (with its current size, not the specified size), and GetLastError returns ERROR_ALREADY_EXISTS. If the function fails, the return value is NULL. To get extended error information, call GetLastError. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr CreateFileMapping(
      IntPtr file, IntPtr fileMappingAttributes, uint protect, uint maximumSizeHigh, uint maximumSizeLow, string name);

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
    /// A low-order DWORD of the file offset where the view is to begin. The combination of the high and low offsets must specify an offset within the file mapping. They must also match the memory allocation granularity of the system. That is, the offset must be a multiple of the allocation granularity. 
    /// </param>
    /// <param name="numberOfBytesToMap">
    /// The number of bytes of a file mapping to map to the view. 
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is the starting address of the mapped view. If the function fails, the return value is NULL. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr MapViewOfFile(
      IntPtr fileMappingObject, uint desiredAccess, uint fileOffsetHigh, uint fileOffsetLow, uint numberOfBytesToMap);

    /// <summary>
    /// Unmaps a mapped view of a file from the calling process's address space.
    /// </summary>
    /// <param name="map">
    /// A pointer to the base address of the mapped view of a file that is to be unmapped. 
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero, and all dirty pages within the specified range are written "lazily" to disk. If the function fails, the return value is zero. 
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern bool UnmapViewOfFile(IntPtr map);

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
      this.colorProcessingSection = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);
      this.motionProcessingSection = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)this.bufferLength, null);

      this.originalMapping = MapViewOfFile(this.originalSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.ColorProcessingMapping = MapViewOfFile(this.colorProcessingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.MotionProcessingMapping = MapViewOfFile(this.motionProcessingSection, 0xF001F, 0, 0, (uint)this.bufferLength);
      this.UnmanagedImage = new UnmanagedImage(this.originalMapping, (int)this.NaturalVideoWidth, (int)this.NaturalVideoHeight, this.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
    }

    /// <summary>
    ///   The on video available.
    /// </summary>
    protected void OnVideoAvailable()
    {
      this.VideoAvailable?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///   The on video frame changed.
    /// </summary>
    protected virtual void OnVideoFrameChanged()
    {
      this.VideoFrameChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Saves the video properties of the SampleGrabber into member fields
    ///   and creates a file mapping for the captured frames.
    /// </summary>
    /// <param name="opencvCapture">
    /// The <see cref="VideoCapture"/> from which to retreive the sample information. 
    /// </param>
    public void SaveSizeInfo(VideoCapture opencvCapture)
    {
      // Grab the size info
      this.NaturalVideoWidth = opencvCapture.FrameWidth;
      this.NaturalVideoHeight = opencvCapture.FrameHeight;
      if (this.rotation.HasValue && (this.rotation.Value == RotateFlags.Rotate90Clockwise || this.rotation.Value == RotateFlags.Rotate90Counterclockwise))
      {
        // Grab the size info
        this.NaturalVideoWidth = opencvCapture.FrameHeight;
        this.NaturalVideoHeight = opencvCapture.FrameWidth;
      }

      // Grab the size info
      this.FrameTimeInMS = 1000d / opencvCapture.Fps;
      this.PixelSize = 3;
      this.Stride = (int)this.NaturalVideoWidth * this.PixelSize;

      this.CreateMemoryMapping(3);
      Video.Instance.OriginalImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.originalSection,
          (int)this.NaturalVideoWidth,
          (int)this.NaturalVideoHeight,
          PixelFormats.Bgr24,
          this.Stride,
          0) as InteropBitmap;

      Video.Instance.ColorProcessedImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.colorProcessingSection,
          (int)this.NaturalVideoWidth,
          (int)this.NaturalVideoHeight,
          PixelFormats.Bgr24,
          this.Stride,
          0) as InteropBitmap;

      Video.Instance.MotionProcessedImageSource =
        Imaging.CreateBitmapSourceFromMemorySection(
          this.motionProcessingSection,
          (int)this.NaturalVideoWidth,
          (int)this.NaturalVideoHeight,
          PixelFormats.Bgr24,
          this.Stride,
          0) as InteropBitmap;
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = (BackgroundWorker)sender;
      Stopwatch watch = new Stopwatch();
      //Stopwatch fpswatch = new Stopwatch();
      watch.Start();
      //fpswatch.Start();

      double frametimeInMS = 41;
      double framerateFactor = 1;
      double selectionEnd = 1;
      this.Dispatcher.Invoke(() =>
      {
        frametimeInMS = this.FrameTimeInMS;
        framerateFactor = App.Project.VideoData.FramerateFactor;
        selectionEnd = App.Project.VideoData.SelectionEnd;
      });

      try
      {
        while (!worker.CancellationPending)
        {
          var currentPosInMS = this.OpenCVObject.Get(VideoCaptureProperties.PosMsec);
          //var currentPosInMS = this.OpenCVObject.Get(VideoCaptureProperties.PosFrames) * frametimeInMS;

          var scaledCurrentPosInMS = currentPosInMS * framerateFactor;

          if (scaledCurrentPosInMS >= selectionEnd)
          {
            break;
          }

          if (Video.Instance.VideoMode == VideoMode.File)
          {
            // Get the time it took to process the last frame
            var total = watch.ElapsedMilliseconds;

            if (total < frametimeInMS)
            {
              // Processing time is shorter that default frametime, so wait till frametime is over, to have correct FPS
              var wait = new TimeSpan((long)((frametimeInMS - total) * 10000));
              Thread.Sleep(wait);
              //Console.WriteLine("In Time, wait: {0}", wait.Milliseconds);
            }
            else
            {
              // Processing has taken more time, than a frame should last, so skip the next frame to be in time again.
              var skipCount = Math.Floor(total / frametimeInMS);
              //Console.WriteLine("Skip: {0}", skipCount);
              for (int i = 0; i < skipCount; i++)
              {
                this.OpenCVObject.Grab();
                this.frameCounter++;
              }

              watch.Restart();
              continue;
            }

            watch.Restart();
          }

          // Output FPS
          //Console.WriteLine("FPS: {0}", 1f / fpswatch.ElapsedMilliseconds * 1000);
          //fpswatch.Restart();

          // Bildverarbeitung grab, retreive, analyze, send to processing chain
          var frameMat = new Mat();
          //using (var frameMat = new Mat())
          {
            this.OpenCVObject.Read(frameMat);
            if (frameMat.Empty())
            {
              // Letztes Bild erreicht
              this.Dispatcher.Invoke(() =>
              {
                this.Stop();
                if (Video.Instance.VideoMode == VideoMode.File)
                {
                  var lastFrameIndex = this.OpenCVObject.Get(VideoCaptureProperties.FrameCount);
                  this.OpenCVObject.Set(VideoCaptureProperties.PosFrames, lastFrameIndex);
                  Video.Instance.VideoPlayerElement.RaiseFileComplete();
                  this.OpenCVObject.Grab();
                  this.GrabCurrentFrame();
                }
              });

              break;
            }

            if (this.rotation.HasValue)
            {
              var rotMat = new Mat();
              //using (Mat rotMat = new Mat())
              {
                Cv2.Rotate(frameMat, rotMat, this.rotation.Value);

                // Must create and use WriteableBitmap in the same thread(UI Thread).
                this.Dispatcher.Invoke(() =>
                {
                  WriteableBitmap newFrame = rotMat.ToWriteableBitmap();
                  Video.Instance.OriginalImageSource = newFrame;
                  Video.Instance.VideoElement.NewFrameCallback(newFrame);
                });
              }

              rotMat.Release();
            }
            else
            {
              // Must create and use WriteableBitmap in the same thread(UI Thread).
              this.Dispatcher.Invoke(() =>
              {
                WriteableBitmap newFrame = frameMat.ToWriteableBitmap();
                Video.Instance.OriginalImageSource = newFrame;
                Video.Instance.VideoElement.NewFrameCallback(newFrame);
              });
            }
          }

          frameMat.Release();
          GC.Collect();
        }
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
    }

    /// <summary>
    ///  Retrieves the current frame from the opencv capture object and
    ///  sends it to the processing pipe
    /// </summary>
    protected void GrabCurrentFrame()
    {
      try
      {

        var frameMat = new Mat();
        //using (Mat frameMat = new Mat())
        {
          this.OpenCVObject.Retrieve(frameMat);
          if (frameMat.Empty())
          {
            return;
          }

          if (this.rotation.HasValue)
          {
            var rotMat = new Mat();
            //using (Mat rotMat = new Mat())
            {
              Cv2.Rotate(frameMat, rotMat, this.rotation.Value);

              // Must create and use WriteableBitmap in the same thread(UI Thread).
              this.Dispatcher.Invoke(() =>
              {
                WriteableBitmap newFrame = rotMat.ToWriteableBitmap();
                Video.Instance.OriginalImageSource = newFrame;
                Video.Instance.VideoElement.NewFrameCallback(newFrame);
              });
            }

            rotMat.Release();
          }
          else
          {
            // Must create and use WriteableBitmap in the same thread(UI Thread).
            this.Dispatcher.Invoke(() =>
            {
              WriteableBitmap newFrame = frameMat.ToWriteableBitmap();
              Video.Instance.OriginalImageSource = newFrame;
              Video.Instance.VideoElement.NewFrameCallback(newFrame);
            });
          }
        }

        frameMat.Release();
        GC.Collect();

        UiServices.WaitUntilReady();
      }
      catch (Exception ex)
      {
        ErrorLogger.ProcessException(ex, false);
      }
    }
  }
}