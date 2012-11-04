// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaInfoDLL.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2012 Dr. Adrian Voßkühler  
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
//   The stream kind.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.MediaInfo
{
  using System;
  using System.Runtime.InteropServices;

  /// <summary>
  ///   The stream kind.
  /// </summary>
  public enum StreamKind
  {
    /// <summary>
    ///   The general.
    /// </summary>
    General, 

    /// <summary>
    ///   The video.
    /// </summary>
    Video, 

    /// <summary>
    ///   The audio.
    /// </summary>
    Audio, 

    /// <summary>
    ///   The text.
    /// </summary>
    Text, 

    /// <summary>
    ///   The chapters.
    /// </summary>
    Chapters, 

    /// <summary>
    ///   The image.
    /// </summary>
    Image
  }

  /// <summary>
  ///   The info kind.
  /// </summary>
  public enum InfoKind
  {
    /// <summary>
    ///   The name.
    /// </summary>
    Name, 

    /// <summary>
    ///   The text.
    /// </summary>
    Text, 

    /// <summary>
    ///   The measure.
    /// </summary>
    Measure, 

    /// <summary>
    ///   The options.
    /// </summary>
    Options, 

    /// <summary>
    ///   The name text.
    /// </summary>
    NameText, 

    /// <summary>
    ///   The measure text.
    /// </summary>
    MeasureText, 

    /// <summary>
    ///   The info.
    /// </summary>
    Info, 

    /// <summary>
    ///   The how to.
    /// </summary>
    HowTo
  }

  /// <summary>
  ///   The info options.
  /// </summary>
  public enum InfoOptions
  {
    /// <summary>
    ///   The show in inform.
    /// </summary>
    ShowInInform, 

    /// <summary>
    ///   The support.
    /// </summary>
    Support, 

    /// <summary>
    ///   The show in supported.
    /// </summary>
    ShowInSupported, 

    /// <summary>
    ///   The type of value.
    /// </summary>
    TypeOfValue
  }

  /// <summary>
  ///   The info file options.
  /// </summary>
  public enum InfoFileOptions
  {
    /// <summary>
    ///   The file option_ nothing.
    /// </summary>
    FileOption_Nothing = 0x00, 

    /// <summary>
    ///   The file option_ recursive.
    /// </summary>
    FileOption_Recursive = 0x01, 

    /// <summary>
    ///   The file option_ close all.
    /// </summary>
    FileOption_CloseAll = 0x02, 

    /// <summary>
    ///   The file option_ max.
    /// </summary>
    FileOption_Max = 0x04
  };

  /// <summary>
  ///   The media info.
  /// </summary>
  public class MediaInfo
  {
    // Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)  

    // MediaInfo class
    #region Fields

    /// <summary>
    ///   The handle.
    /// </summary>
    private readonly IntPtr Handle;

    /// <summary>
    ///   The must use ansi.
    /// </summary>
    private readonly bool MustUseAnsi;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MediaInfo" /> class.
    /// </summary>
    public MediaInfo()
    {
      this.Handle = MediaInfo_New();
      if (Environment.OSVersion.ToString().IndexOf("Windows") == -1)
      {
        this.MustUseAnsi = true;
      }
      else
      {
        this.MustUseAnsi = false;
      }
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="MediaInfo" /> class.
    /// </summary>
    ~MediaInfo()
    {
      MediaInfo_Delete(this.Handle);
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The close.
    /// </summary>
    public void Close()
    {
      MediaInfo_Close(this.Handle);
    }

    /// <summary>
    /// The count_ get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Count_Get(StreamKind StreamKind, int StreamNumber)
    {
      return (int)MediaInfo_Count_Get(this.Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber);
    }

    /// <summary>
    /// The count_ get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Count_Get(StreamKind StreamKind)
    {
      return this.Count_Get(StreamKind, -1);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <param name="KindOfSearch">
    /// The kind of search. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(
      StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
    {
      if (this.MustUseAnsi)
      {
        IntPtr Parameter_Ptr = Marshal.StringToHGlobalAnsi(Parameter);
        string ToReturn =
          Marshal.PtrToStringAnsi(
            MediaInfoA_Get(
              this.Handle, 
              (IntPtr)StreamKind, 
              (IntPtr)StreamNumber, 
              Parameter_Ptr, 
              (IntPtr)KindOfInfo, 
              (IntPtr)KindOfSearch));
        Marshal.FreeHGlobal(Parameter_Ptr);
        return ToReturn;
      }
      else
      {
        return
          Marshal.PtrToStringUni(
            MediaInfo_Get(
              this.Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
      }
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
    {
      if (this.MustUseAnsi)
      {
        return
          Marshal.PtrToStringAnsi(
            MediaInfoA_GetI(
              this.Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
      }
      else
      {
        return
          Marshal.PtrToStringUni(
            MediaInfo_GetI(this.Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
      }
    }

    // Default values, if you know how to set default values in C#, say me
    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo)
    {
      return this.Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(StreamKind StreamKind, int StreamNumber, string Parameter)
    {
      return this.Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(StreamKind StreamKind, int StreamNumber, int Parameter)
    {
      return this.Get(StreamKind, StreamNumber, Parameter, InfoKind.Text);
    }

    /// <summary>
    ///   The inform.
    /// </summary>
    /// <returns> The <see cref="string" /> . </returns>
    public string Inform()
    {
      if (this.MustUseAnsi)
      {
        return Marshal.PtrToStringAnsi(MediaInfoA_Inform(this.Handle, (IntPtr)0));
      }
      else
      {
        return Marshal.PtrToStringUni(MediaInfo_Inform(this.Handle, (IntPtr)0));
      }
    }

    /// <summary>
    /// The open.
    /// </summary>
    /// <param name="FileName">
    /// The file name. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Open(string FileName)
    {
      if (this.MustUseAnsi)
      {
        IntPtr FileName_Ptr = Marshal.StringToHGlobalAnsi(FileName);
        var ToReturn = (int)MediaInfoA_Open(this.Handle, FileName_Ptr);
        Marshal.FreeHGlobal(FileName_Ptr);
        return ToReturn;
      }
      else
      {
        return (int)MediaInfo_Open(this.Handle, FileName);
      }
    }

    /// <summary>
    /// The open_ buffer_ continue.
    /// </summary>
    /// <param name="Buffer">
    /// The buffer. 
    /// </param>
    /// <param name="Buffer_Size">
    /// The buffer_ size. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size)
    {
      return (int)MediaInfo_Open_Buffer_Continue(this.Handle, Buffer, Buffer_Size);
    }

    /// <summary>
    ///   The open_ buffer_ continue_ go to_ get.
    /// </summary>
    /// <returns> The <see cref="long" /> . </returns>
    public long Open_Buffer_Continue_GoTo_Get()
    {
      return (int)MediaInfo_Open_Buffer_Continue_GoTo_Get(this.Handle);
    }

    /// <summary>
    ///   The open_ buffer_ finalize.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    public int Open_Buffer_Finalize()
    {
      return (int)MediaInfo_Open_Buffer_Finalize(this.Handle);
    }

    /// <summary>
    /// The open_ buffer_ init.
    /// </summary>
    /// <param name="File_Size">
    /// The file_ size. 
    /// </param>
    /// <param name="File_Offset">
    /// The file_ offset. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Open_Buffer_Init(long File_Size, long File_Offset)
    {
      return (int)MediaInfo_Open_Buffer_Init(this.Handle, File_Size, File_Offset);
    }

    /// <summary>
    /// The option.
    /// </summary>
    /// <param name="Option">
    /// The option. 
    /// </param>
    /// <param name="Value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Option(string Option, string Value)
    {
      if (this.MustUseAnsi)
      {
        IntPtr Option_Ptr = Marshal.StringToHGlobalAnsi(Option);
        IntPtr Value_Ptr = Marshal.StringToHGlobalAnsi(Value);
        string ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Option(this.Handle, Option_Ptr, Value_Ptr));
        Marshal.FreeHGlobal(Option_Ptr);
        Marshal.FreeHGlobal(Value_Ptr);
        return ToReturn;
      }
      else
      {
        return Marshal.PtrToStringUni(MediaInfo_Option(this.Handle, Option, Value));
      }
    }

    /// <summary>
    /// The option.
    /// </summary>
    /// <param name="Option_">
    /// The option_. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Option(string Option_)
    {
      return this.Option(Option_, string.Empty);
    }

    /// <summary>
    ///   The state_ get.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    public int State_Get()
    {
      return (int)MediaInfo_State_Get(this.Handle);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The media info a_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <param name="KindOfSearch">
    /// The kind of search. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Get(
      IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);

    /// <summary>
    /// The media info a_ get i.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_GetI(
      IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);

    /// <summary>
    /// The media info a_ inform.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="Reserved">
    /// The reserved. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);

    /// <summary>
    /// The media info a_ open.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FileName">
    /// The file name. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);

    /// <summary>
    /// The media info a_ open.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="File_Size">
    /// The file_ size. 
    /// </param>
    /// <param name="File_Offset">
    /// The file_ offset. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);

    /// <summary>
    /// The media info a_ open_ buffer_ continue.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="File_Size">
    /// The file_ size. 
    /// </param>
    /// <param name="Buffer">
    /// The buffer. 
    /// </param>
    /// <param name="Buffer_Size">
    /// The buffer_ size. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Open_Buffer_Continue(
      IntPtr Handle, long File_Size, byte[] Buffer, IntPtr Buffer_Size);

    /// <summary>
    /// The media info a_ open_ buffer_ continue_ go to_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <returns>
    /// The <see cref="long"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

    /// <summary>
    /// The media info a_ open_ buffer_ finalize.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);

    /// <summary>
    /// The media info a_ option.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="Option">
    /// The option. 
    /// </param>
    /// <param name="Value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);

    /// <summary>
    /// The media info_ close.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    [DllImport("MediaInfo.dll")]
    private static extern void MediaInfo_Close(IntPtr Handle);

    /// <summary>
    /// The media info_ count_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);

    /// <summary>
    /// The media info_ delete.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    [DllImport("MediaInfo.dll")]
    private static extern void MediaInfo_Delete(IntPtr Handle);

    /// <summary>
    /// The media info_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <param name="KindOfSearch">
    /// The kind of search. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Get(
      IntPtr Handle, 
      IntPtr StreamKind, 
      IntPtr StreamNumber, 
      [MarshalAs(UnmanagedType.LPWStr)] string Parameter, 
      IntPtr KindOfInfo, 
      IntPtr KindOfSearch);

    /// <summary>
    /// The media info_ get i.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_GetI(
      IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);

    /// <summary>
    /// The media info_ inform.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="Reserved">
    /// The reserved. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);

    /// <summary>
    ///   The media info_ new.
    /// </summary>
    /// <returns> The <see cref="IntPtr" /> . </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_New();

    /// <summary>
    /// The media info_ open.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FileName">
    /// The file name. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

    /// <summary>
    /// The media info_ open_ buffer_ continue.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="Buffer">
    /// The buffer. 
    /// </param>
    /// <param name="Buffer_Size">
    /// The buffer_ size. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);

    /// <summary>
    /// The media info_ open_ buffer_ continue_ go to_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <returns>
    /// The <see cref="long"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

    /// <summary>
    /// The media info_ open_ buffer_ finalize.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);

    /// <summary>
    /// The media info_ open_ buffer_ init.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="File_Size">
    /// The file_ size. 
    /// </param>
    /// <param name="File_Offset">
    /// The file_ offset. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);

    /// <summary>
    /// The media info_ option.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="Option">
    /// The option. 
    /// </param>
    /// <param name="Value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_Option(
      IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);

    /// <summary>
    /// The media info_ state_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_State_Get(IntPtr Handle);

    #endregion
  }

  /// <summary>
  ///   The media info list.
  /// </summary>
  public class MediaInfoList
  {
    // Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)  

    // MediaInfo class
    #region Fields

    /// <summary>
    ///   The handle.
    /// </summary>
    private readonly IntPtr Handle;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MediaInfoList" /> class.
    /// </summary>
    public MediaInfoList()
    {
      this.Handle = MediaInfoList_New();
    }

    /// <summary>
    ///   Finalizes an instance of the <see cref="MediaInfoList" /> class.
    /// </summary>
    ~MediaInfoList()
    {
      MediaInfoList_Delete(this.Handle);
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The close.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    public void Close(int FilePos)
    {
      MediaInfoList_Close(this.Handle, (IntPtr)FilePos);
    }

    /// <summary>
    ///   The close.
    /// </summary>
    public void Close()
    {
      this.Close(-1);
    }

    /// <summary>
    /// The count_ get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Count_Get(int FilePos, StreamKind StreamKind, int StreamNumber)
    {
      return (int)MediaInfoList_Count_Get(this.Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber);
    }

    /// <summary>
    /// The count_ get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Count_Get(int FilePos, StreamKind StreamKind)
    {
      return this.Count_Get(FilePos, StreamKind, -1);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <param name="KindOfSearch">
    /// The kind of search. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(
      int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
    {
      return
        Marshal.PtrToStringUni(
          MediaInfoList_Get(
            this.Handle, 
            (IntPtr)FilePos, 
            (IntPtr)StreamKind, 
            (IntPtr)StreamNumber, 
            Parameter, 
            (IntPtr)KindOfInfo, 
            (IntPtr)KindOfSearch));
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
    {
      return
        Marshal.PtrToStringUni(
          MediaInfoList_GetI(
            this.Handle, 
            (IntPtr)FilePos, 
            (IntPtr)StreamKind, 
            (IntPtr)StreamNumber, 
            (IntPtr)Parameter, 
            (IntPtr)KindOfInfo));
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo)
    {
      return this.Get(FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter)
    {
      return this.Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter)
    {
      return this.Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text);
    }

    /// <summary>
    /// The inform.
    /// </summary>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Inform(int FilePos)
    {
      return Marshal.PtrToStringUni(MediaInfoList_Inform(this.Handle, (IntPtr)FilePos, (IntPtr)0));
    }

    /// <summary>
    /// The open.
    /// </summary>
    /// <param name="FileName">
    /// The file name. 
    /// </param>
    /// <param name="Options">
    /// The options. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    public int Open(string FileName, InfoFileOptions Options)
    {
      return (int)MediaInfoList_Open(this.Handle, FileName, (IntPtr)Options);
    }

    /// <summary>
    /// The open.
    /// </summary>
    /// <param name="FileName">
    /// The file name. 
    /// </param>
    public void Open(string FileName)
    {
      this.Open(FileName, 0);
    }

    /// <summary>
    /// The option.
    /// </summary>
    /// <param name="Option">
    /// The option. 
    /// </param>
    /// <param name="Value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Option(string Option, string Value)
    {
      return Marshal.PtrToStringUni(MediaInfoList_Option(this.Handle, Option, Value));
    }

    /// <summary>
    /// The option.
    /// </summary>
    /// <param name="Option_">
    /// The option_. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string Option(string Option_)
    {
      return this.Option(Option_, string.Empty);
    }

    /// <summary>
    ///   The state_ get.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    public int State_Get()
    {
      return (int)MediaInfoList_State_Get(this.Handle);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The media info list_ close.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    [DllImport("MediaInfo.dll")]
    private static extern void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos);

    /// <summary>
    /// The media info list_ count_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_Count_Get(
      IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber);

    /// <summary>
    /// The media info list_ delete.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    [DllImport("MediaInfo.dll")]
    private static extern void MediaInfoList_Delete(IntPtr Handle);

    /// <summary>
    /// The media info list_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <param name="KindOfSearch">
    /// The kind of search. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_Get(
      IntPtr Handle, 
      IntPtr FilePos, 
      IntPtr StreamKind, 
      IntPtr StreamNumber, 
      [MarshalAs(UnmanagedType.LPWStr)] string Parameter, 
      IntPtr KindOfInfo, 
      IntPtr KindOfSearch);

    /// <summary>
    /// The media info list_ get i.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="StreamKind">
    /// The stream kind. 
    /// </param>
    /// <param name="StreamNumber">
    /// The stream number. 
    /// </param>
    /// <param name="Parameter">
    /// The parameter. 
    /// </param>
    /// <param name="KindOfInfo">
    /// The kind of info. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_GetI(
      IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);

    /// <summary>
    /// The media info list_ inform.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FilePos">
    /// The file pos. 
    /// </param>
    /// <param name="Reserved">
    /// The reserved. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved);

    /// <summary>
    ///   The media info list_ new.
    /// </summary>
    /// <returns> The <see cref="IntPtr" /> . </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_New();

    /// <summary>
    /// The media info list_ open.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="FileName">
    /// The file name. 
    /// </param>
    /// <param name="Options">
    /// The options. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_Open(
      IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr Options);

    /// <summary>
    /// The media info list_ option.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <param name="Option">
    /// The option. 
    /// </param>
    /// <param name="Value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_Option(
      IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);

    /// <summary>
    /// The media info list_ state_ get.
    /// </summary>
    /// <param name="Handle">
    /// The handle. 
    /// </param>
    /// <returns>
    /// The <see cref="IntPtr"/> . 
    /// </returns>
    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfoList_State_Get(IntPtr Handle);

    #endregion
  }
}

//NameSpace