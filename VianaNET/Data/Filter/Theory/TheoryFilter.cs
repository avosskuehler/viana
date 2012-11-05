// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TheoryObject.cs" company="Freie Universität Berlin">
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
// <author>Herwig Niemeyer</author>
// <email>hn_muenster@web.de</email>
// <summary>
//   The line fit class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Theory
{
  using VianaNET.CustomStyles.Types;
  using VianaNET.Data.Collections;
  using VianaNET.Data.Filter.Interpolation;

  /// <summary>
  /// The line fit class.
  /// </summary>
  public class TheoryFilter : FilterBase
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    #endregion

    #region Fields
    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    /////////////////////////////////////////////////////////////////////////////// 
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryFilter"/> class.
    /// </summary>
    public TheoryFilter()
    {
    }

    #endregion

    #region Delegates
    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties
    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// calculate line fit function.
    /// </summary>
    /// <param name="originalSamples">
    /// The original Samples.
    /// </param>
    /// <param name="fittedSamples">
    /// The fitted Samples.
    /// </param>
    public override void CalculateFilterValues(DataCollection originalSamples, SortedObservableCollection<XYSample> fittedSamples)
    {
      base.CalculateFilterValues(originalSamples, fittedSamples);

      // Erase old values
      fittedSamples.Clear();
      var fx = FilterData.Instance.TheoreticalFunction;
      if (fx == null)
      {
        return;
      }

      double x;
      XYSample p;
      var tempParser = new Parse();

      /*     if (tempParser.isLinearFunction(fx))
           {
          
               if (this.axisX.Axis != AxisType.T)
             {
               // zwei Punkte genügen bei x-y-Diagramm
               x = this.WertX[0]; // wertX[] - originale x-Werte der Wertepaare 
               p = new XYSample(x, tempParser.FreierFktWert(fx, x));
               theorySamples.Add(p);;
               x = this.WertX[this.anzahl - 1];
               p = new XYSample(x, tempParser.FreierFktWert(fx, x));
               theorySamples.Add(p);
             }
             else 
             {
               // Workaround beim t-?-Diagramm: gleichviele Punkte wie bei Originalwerten und gleiche x Werte. 
               for (k = 0; k < this.anzahl; k++)
               {
                 x = this.WertX[k];
                 p = new XYSample(x, tempParser.FreierFktWert(fx, x));
                 theorySamples.Add(p);
               }
             }
           }
           else
           {
             // endPixelX und startPixelX
             // startX und endX wurden in aktualisiereTab(int aktObjectNr,int aktxNr, int aktyNr) bestimmt
             var anzahlPixel = (int)(this.endPixelX - this.startPixelX);
             x = this.startX;

             for (k = 0; k < anzahlPixel; k++)
             {
               // Punkte im PixelAbstand (waagerecht) werden mit der theoretischen Funktion bestimmt.
               // führt bei t-?-Diagrammen zu falschen Darstellungen !!
               p = new XYSample(x, tempParser.FreierFktWert(fx, x));
               theorySamples.Add(p);
               x = x + this.stepX;
             }
           } */
      if (tempParser.isLinearFunction(fx))
      {
        // zwei Punkte genügen bei x-y-Diagramm
        if (originalSamples.Count == 0)
        {
          // Nur wenn gar keine Daten da sind...
          p = new XYSample(-10, tempParser.FreierFktWert(fx, -10));
          fittedSamples.Add(p);
          p = new XYSample(10, tempParser.FreierFktWert(fx, 10));
          fittedSamples.Add(p);
        }
        else
        {
          x = this.WertX[0]; // wertX[] - originale x-Werte der Wertepaare 
          p = new XYSample(x, tempParser.FreierFktWert(fx, x));
          fittedSamples.Add(p);
          x = this.WertX[this.anzahl - 1];
          p = new XYSample(x, tempParser.FreierFktWert(fx, x));
          fittedSamples.Add(p);
        }
      }
      else
      {
        if (originalSamples.Count == 0)
        {
          // Nur wenn gar keine Daten da sind...
          for (int i = -10; i < 10; i++)
          {
            p = new XYSample(i, tempParser.FreierFktWert(fx, i));
            fittedSamples.Add(p);
          }
        }
        else
        {
          int k;
          if (FilterData.Instance.AxisX.Axis != AxisType.T)
          {
            // Workaround beim t-?-Diagramm: gleichviele Punkte wie bei Originalwerten und gleiche x Werte. 
            for (k = 0; k < this.anzahl; k++)
            {
              x = this.WertX[k];
              p = new XYSample(x, tempParser.FreierFktWert(fx, x));
              fittedSamples.Add(p);
            }
          }
          else
          {
            // endPixelX und startPixelX
            // startX und endX wurden in aktualisiereTab(int aktObjectNr,int aktxNr, int aktyNr) bestimmt
            var anzahlPixel = (int)(this.endPixelX - this.startPixelX);
            x = this.startX;

            for (k = 0; k < anzahlPixel; k++)
            {
              // Punkte im PixelAbstand (waagerecht) werden mit der theoretischen Funktion bestimmt.
              // führt bei t-?-Diagrammen zu falschen Darstellungen !!
              p = new XYSample(x, tempParser.FreierFktWert(fx, x));
              fittedSamples.Add(p);
              x = x + this.stepX;
            }
          }
        }
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods
    #endregion
  }
}