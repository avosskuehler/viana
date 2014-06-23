// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GMFBridge.cs" company="Freie Universität Berlin">
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
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   The e format type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Modules.Video.Filter
{
  using System;
  using System.Runtime.InteropServices;

  using DirectShowLib;

  /// <summary>
  ///   The e format type.
  /// </summary>
  public enum eFormatType
  {
    /// <summary>
    ///   The uncompressed.
    /// </summary>
    Uncompressed, 

    /// <summary>
    ///   The mux inputs.
    /// </summary>
    MuxInputs, 

    /// <summary>
    ///   The any.
    /// </summary>
    Any, 
  }

  /// <summary>
  ///   The gmf bridge controller.
  /// </summary>
  [ComImport]
  [Guid("08E3287F-3A5C-47e9-8179-A9E9221A5CDE")]
  public class GMFBridgeController
  {
  }

  /// <summary>
  ///   The GMFBridgeController interface.
  /// </summary>
  [Guid("8C4D8054-FCBA-4783-865A-7E8B3C814011")]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IGMFBridgeController
  {
    /// <summary>
    /// The add stream.
    /// </summary>
    /// <param name="bVideo">
    /// The b video.
    /// </param>
    /// <param name="AllowedTypes">
    /// The allowed types.
    /// </param>
    /// <param name="bDiscardUnconnected">
    /// The b discard unconnected.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int AddStream(
      [In] [MarshalAs(UnmanagedType.Bool)] bool bVideo, 
      [In] eFormatType AllowedTypes, 
      [In] [MarshalAs(UnmanagedType.Bool)] bool bDiscardUnconnected);

    /// <summary>
    /// The insert sink filter.
    /// </summary>
    /// <param name="pGraph">
    /// The p graph.
    /// </param>
    /// <param name="ppFilter">
    /// The pp filter.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int InsertSinkFilter([In] IFilterGraph pGraph, out IBaseFilter ppFilter);

    /// <summary>
    /// The insert source filter.
    /// </summary>
    /// <param name="pUnkSourceGraphSinkFilter">
    /// The p unk source graph sink filter.
    /// </param>
    /// <param name="pRenderGraph">
    /// The p render graph.
    /// </param>
    /// <param name="ppFilter">
    /// The pp filter.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int InsertSourceFilter(
      [In] IBaseFilter pUnkSourceGraphSinkFilter, 
      [In] IGraphBuilder pRenderGraph, 
      out IBaseFilter ppFilter);

    /// <summary>
    /// The create source graph.
    /// </summary>
    /// <param name="strFile">
    /// The str file.
    /// </param>
    /// <param name="pGraph">
    /// The p graph.
    /// </param>
    /// <param name="pSinkFilter">
    /// The p sink filter.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int CreateSourceGraph(
      [In] [MarshalAs(UnmanagedType.BStr)] string strFile, 
      [In] IFilterGraph pGraph, 
      out IBaseFilter pSinkFilter);

    /// <summary>
    /// The create render graph.
    /// </summary>
    /// <param name="pSourceGraphSinkFilter">
    /// The p source graph sink filter.
    /// </param>
    /// <param name="pRenderGraph">
    /// The p render graph.
    /// </param>
    /// <param name="pRenderGraphSourceFilter">
    /// The p render graph source filter.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int CreateRenderGraph(
      [In] IBaseFilter pSourceGraphSinkFilter, 
      [In] IGraphBuilder pRenderGraph, 
      out IBaseFilter pRenderGraphSourceFilter);

    /// <summary>
    /// The bridge graphs.
    /// </summary>
    /// <param name="pSourceGraphSinkFilter">
    /// The p source graph sink filter.
    /// </param>
    /// <param name="pRenderGraphSourceFilter">
    /// The p render graph source filter.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int BridgeGraphs([In] IBaseFilter pSourceGraphSinkFilter, [In] IBaseFilter pRenderGraphSourceFilter);

    /// <summary>
    /// The set notify.
    /// </summary>
    /// <param name="hwnd">
    /// The hwnd.
    /// </param>
    /// <param name="msg">
    /// The msg.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int SetNotify([In] IntPtr hwnd, [In] int msg);

    /// <summary>
    /// The set buffer minimum.
    /// </summary>
    /// <param name="nMillisecs">
    /// The n millisecs.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int SetBufferMinimum([In] int nMillisecs);

    /// <summary>
    /// The get segment time.
    /// </summary>
    /// <param name="pdSeconds">
    /// The pd seconds.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int GetSegmentTime(out double pdSeconds);

    /// <summary>
    ///   The no more segments.
    /// </summary>
    /// <returns> The <see cref="int" /> . </returns>
    [PreserveSig]
    int NoMoreSegments();

    /// <summary>
    /// The get segment offset.
    /// </summary>
    /// <param name="pdOffset">
    /// The pd offset.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> .
    /// </returns>
    [PreserveSig]
    int GetSegmentOffset(out double pdOffset);
  }
}