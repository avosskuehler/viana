// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Freie Universität Berlin" file="DifferenceDetector.cs">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// 
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;
  using System.Drawing.Imaging;

  using AForge;
  using AForge.Imaging;
  using AForge.Imaging.Filters;
  using AForge.Vision.Motion;

  /// <summary>
  ///   Motion detector based on two continues frames difference. Is a clone of <see cref="TwoFramesDifferenceDetectorSpecial"/>
  /// with adaptions for viana.
  /// All credits to AForge.NET
  /// </summary>
  /// <seealso cref="MotionDetector" />
  public class TwoFramesDifferenceDetectorSpecial : IMotionDetector
  {
    #region Fields

    /// <summary>
    /// The erosion filter.
    /// </summary>
    private readonly BinaryErosion3x3 erosionFilter = new BinaryErosion3x3();

    /// <summary>
    /// dummy object to lock for synchronization
    /// </summary>
    private readonly object sync = new object();

    /// <summary>
    /// The difference threshold.
    /// </summary>
    private int differenceThreshold = 15;

    /// <summary>
    /// The difference threshold neg.
    /// </summary>
    private int differenceThresholdNeg = -15;

    /// <summary>
    /// The frame size.
    /// </summary>
    private int frameSize;

    /// <summary>
    /// The height.
    /// </summary>
    private int height;

    /// <summary>
    /// The current frame of video sream.
    /// </summary>
    private UnmanagedImage motionFrame;

    /// <summary>
    /// The number of pixels changed in the new frame of video stream
    /// </summary>
    private int pixelsChanged;

    /// <summary>
    /// The previous frame of video stream
    /// </summary>
    private UnmanagedImage previousFrame;

    /// <summary>
    /// The suppress noise.
    /// </summary>
    private bool suppressNoise = true;

    /// <summary>
    /// Temporary buffer used for suppressing noise
    /// </summary>
    private UnmanagedImage tempFrame;

    /// <summary>
    /// The width.
    /// </summary>
    private int width;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="TwoFramesDifferenceDetectorSpecial" /> class.
    /// </summary>
    public TwoFramesDifferenceDetectorSpecial()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwoFramesDifferenceDetectorSpecial"/> class.
    /// </summary>
    /// <param name="suppressNoise">
    /// Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).
    /// </param>
    public TwoFramesDifferenceDetectorSpecial(bool suppressNoise)
    {
      this.suppressNoise = suppressNoise;
    }

    #endregion

    // threshold values
    #region Public Properties

    /// <summary>
    ///  Gets or sets the difference threshold value, [1, 255].
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The value specifies the amount off difference between pixels, which is treated
    ///     as motion pixel.
    ///   </para>
    ///   <para>Default value is set to <b>15</b>.</para>
    /// </remarks>
    public int DifferenceThreshold
    {
      get
      {
        return this.differenceThreshold;
      }

      set
      {
        lock (this.sync)
        {
          this.differenceThreshold = Math.Max(1, Math.Min(255, value));
          this.differenceThresholdNeg = -this.differenceThreshold;
        }
      }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the positive threshold is used,
    ///   otherwise the negative threshold is used.
    /// </summary>
    public bool IsPositiveThreshold { get; set; }

    /// <summary>
    ///  Gets the motion frame containing detected areas of motion.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Motion frame is a grayscale image, which shows areas of detected motion.
    ///     All black pixels in the motion frame correspond to areas, where no motion is
    ///     detected. But white pixels correspond to areas, where motion is detected.
    ///   </para>
    ///   <para>
    ///     <note>
    ///       The property is set to <see langword="null" /> after processing of the first
    ///       video frame by the algorithm.
    ///     </note>
    ///   </para>
    /// </remarks>
    public UnmanagedImage MotionFrame
    {
      get
      {
        lock (this.sync)
        {
          return this.motionFrame;
        }
      }
    }

    /// <summary>
    ///   Gets the motion level value, [0, 1].
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Amount of changes in the last processed frame. For example, if value of
    ///     this property equals to 0.1, then it means that last processed frame has 10% difference
    ///     with previous frame.
    ///   </para>
    /// </remarks>
    public float MotionLevel
    {
      get
      {
        lock (this.sync)
        {
          return (float)this.pixelsChanged / (this.width * this.height);
        }
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether to suppress noise in video frames or not.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The value specifies if additional filtering should be
    ///     done to suppress standalone noisy pixels by applying 3x3 erosion image processing
    ///     filter.
    ///   </para>
    ///   <para>Default value is set to <see langword="true" />.</para>
    ///   <para>
    ///     <note>Turning the value on leads to more processing time of video frame.</note>
    ///   </para>
    /// </remarks>
    public bool SuppressNoise
    {
      get
      {
        return this.suppressNoise;
      }

      set
      {
        lock (this.sync)
        {
          this.suppressNoise = value;

          // allocate temporary frame if required
          if (this.suppressNoise && (this.tempFrame == null) && (this.motionFrame != null))
          {
            this.tempFrame = UnmanagedImage.Create(this.width, this.height, PixelFormat.Format8bppIndexed);
          }

          // check if temporary frame is not required
          if ((!this.suppressNoise) && (this.tempFrame != null))
          {
            this.tempFrame.Dispose();
            this.tempFrame = null;
          }
        }
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Process new video frame.
    /// </summary>
    /// <param name="videoFrame">
    /// Video frame to process (detect motion in).
    /// </param>
    /// <remarks>
    /// <para>
    /// Processes new frame from video source and detects motion in it.
    /// </para>
    /// <para>
    /// Check <see cref="MotionLevel"/> property to get information about amount of motion
    ///     (changes) in the processed frame.
    ///   </para>
    /// </remarks>
    public unsafe void ProcessFrame(UnmanagedImage videoFrame)
    {
      lock (this.sync)
      {
        // check previous frame
        if (this.previousFrame == null)
        {
          // save image dimension
          this.width = videoFrame.Width;
          this.height = videoFrame.Height;

          // alocate memory for previous and current frames
          this.previousFrame = UnmanagedImage.Create(this.width, this.height, PixelFormat.Format8bppIndexed);
          this.motionFrame = UnmanagedImage.Create(this.width, this.height, PixelFormat.Format8bppIndexed);

          this.frameSize = this.motionFrame.Stride * this.height;

          // temporary buffer
          if (this.suppressNoise)
          {
            this.tempFrame = UnmanagedImage.Create(this.width, this.height, PixelFormat.Format8bppIndexed);
          }

          // convert source frame to grayscale
          ConvertToGrayscale(videoFrame, this.previousFrame);

          return;
        }

        // check image dimension
        if ((videoFrame.Width != this.width) || (videoFrame.Height != this.height))
        {
          return;
        }

        // convert current image to grayscale
        ConvertToGrayscale(videoFrame, this.motionFrame);

        // pointers to previous and current frames
        var prevFrame = (byte*)this.previousFrame.ImageData.ToPointer();
        var currFrame = (byte*)this.motionFrame.ImageData.ToPointer();

        // 1 - get difference between frames
        // 2 - copy current frame to previous frame
        // 3 - threshold the difference
        for (int i = 0; i < this.frameSize; i++, prevFrame++, currFrame++)
        {
          // difference
          int diff = *currFrame - *prevFrame;

          // copy current frame to previous
          *prevFrame = *currFrame;

          // treshold
          if (this.IsPositiveThreshold)
          {
            *currFrame = (diff >= this.differenceThreshold) ? (byte)255 : (byte)0;
          }
          else
          {
            *currFrame = (diff <= this.differenceThresholdNeg) ? (byte)255 : (byte)0;
          }
        }

        if (this.suppressNoise)
        {
          // suppress noise and calculate motion amount
          SystemTools.CopyUnmanagedMemory(this.tempFrame.ImageData, this.motionFrame.ImageData, this.frameSize);
          this.erosionFilter.Apply(this.tempFrame, this.motionFrame);
        }

        // calculate amount of motion pixels
        this.pixelsChanged = 0;
        var motion = (byte*)this.motionFrame.ImageData.ToPointer();

        for (int i = 0; i < this.frameSize; i++, motion++)
        {
          this.pixelsChanged += *motion & 1;
        }
      }
    }

    /// <summary>
    /// Get grayscale image out of the specified one
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    private static void ConvertToGrayscale(UnmanagedImage source, UnmanagedImage destination)
    {
      if (source.PixelFormat != PixelFormat.Format8bppIndexed)
      {
        Grayscale.CommonAlgorithms.BT709.Apply(source, destination);
      }
      else
      {
        source.Copy(destination);
      }
    }

    /// <summary>
    ///   Reset motion detector to initial state.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Resets internal state and variables of motion detection algorithm.
    ///     Usually this is required to be done before processing new video source, but
    ///     may be also done at any time to restart motion detection algorithm.
    ///   </para>
    /// </remarks>
    public void Reset()
    {
      lock (this.sync)
      {
        if (this.previousFrame != null)
        {
          this.previousFrame.Dispose();
          this.previousFrame = null;
        }

        if (this.motionFrame != null)
        {
          this.motionFrame.Dispose();
          this.motionFrame = null;
        }

        if (this.tempFrame != null)
        {
          this.tempFrame.Dispose();
          this.tempFrame = null;
        }
      }
    }

    #endregion
  }
}