// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Linefit.cs" company="Freie Universität Berlin">
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
namespace VianaNET.Data.Linefit
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;

  using VianaNET.Modules.Video.Control;

  /// <summary>
  ///   The line fit class.
  /// </summary>
  public class LineFitClass
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   The p.
    /// </summary>
    private static readonly Matrix p = new Matrix(10, 6); // Parameter der Ausgleichsfunktion

    /// <summary>
    ///   The genauigkeit.
    /// </summary>
    private static double genauigkeit = 1E-10;

    /// <summary>
    ///   The max iteration.
    /// </summary>
    private static int maxIteration = 50; // Begrenzungswerte für die internen Berechnungen

    /// <summary>
    ///   The min step.
    /// </summary>
    private static double minStep = 1E-6;

    /// <summary>
    ///   The param.
    /// </summary>
    private static double[] param; // Parameter einer Ausgleichsfunktionen

    /// <summary>
    ///   The start abw.
    /// </summary>
    private static double startAbw = 1E150;

    #endregion

    #region Fields

    /// <summary>
    ///   The line fit display sample.
    /// </summary>
    public DataCollection LineFitDisplaySample;

    // mit der Ausgleichsfunktion berechnete Punkte, die sich in der x-Koordinate um einen Pixelabstand unterscheiden

    /// <summary>
    ///   The theorie display sample.
    /// </summary>
    public DataCollection TheorieDisplaySample; // analog für theoretische Funktion

    /// <summary>
    ///   The akt func.
    /// </summary>
    public AusgleichFunction aktFunc;

    /// <summary>
    ///   The wert x.
    /// </summary>
    public List<double> wertX;

    // aus den Videodaten herausgelesene Messpaare - auf zwei Arrays aufgeteilt, Grunddaten der Berechnung der Ausgleichsfunktion 

    /// <summary>
    ///   The wert y.
    /// </summary>
    public List<double> wertY;

    // aus den Videodaten herausgelesene Messpaare - auf zwei Arrays aufgeteilt, Grunddaten der Berechnung der Ausgleichsfunktion 

    /// <summary>
    ///   The number of object.
    /// </summary>
    private int NumberOfObject; // Spaltennummer des 1. bzw. 2. Wertes; Nummer des betrachteten Objekts

    /// <summary>
    ///   The _ line fit abweichung.
    /// </summary>
    private double _LineFitAbweichung; // Wert der mittleren Abweichung der Messpunkte von der Ausgleichsfunktion

    /// <summary>
    ///   The _ line fit fkt str.
    /// </summary>
    private string _LineFitFktStr; // Ausgabestring für die Ausgleichsfunktion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Variables, Enumerations, Events                                  //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///   The anzahl.
    /// </summary>
    private int // Kennzahl für den Typ der Ausgleichsfunktion
      anzahl; // Anzahl der Wertepaare, die für die Berechnungen der Ausgleichsfunktion benutzt werden

    /// <summary>
    ///   The end pixel x.
    /// </summary>
    private double endPixelX;

    /// <summary>
    ///   The end x.
    /// </summary>
    private double endX;

    /// <summary>
    ///   The my g.
    /// </summary>
    private string myG = "G4"; // Formatstring für anzuzeigende Stellenzahl

    /// <summary>
    ///   The org data samples.
    /// </summary>
    private DataCollection orgDataSamples; // orginale Videodaten, wie sie bei Daten angezeigt werden

    /// <summary>
    ///   The reg typ.
    /// </summary>
    private int regTyp; // Anzahl der Wertepaare, die für die Berechnungen der Ausgleichsfunktion benutzt werden

    /// <summary>
    ///   The start pixel x.
    /// </summary>
    private double startPixelX;

    /// <summary>
    ///   The start time.
    /// </summary>
    private long startTime; // erster Zeitwert der Daten (in ms)

    /// <summary>
    ///   The start x.
    /// </summary>
    private double startX;

    /// <summary>
    ///   The step x.
    /// </summary>
    private double stepX;

    /// <summary>
    ///   The x nr.
    /// </summary>
    private int xNr; // Spaltennummer des 1. bzw. 2. Wertes; Nummer des betrachteten Objekts

    /// <summary>
    ///   The y nr.
    /// </summary>
    private int yNr; // Spaltennummer des 1. bzw. 2. Wertes; Nummer des betrachteten Objekts

    #endregion

    // Berechnet zu einem Wert den Funktionswert mit Hilfe der Ausgleichsfunktion
    // private Parser.TFktTerm userFkt;

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    /////////////////////////////////////////////////////////////////////////////// 
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LineFitClass"/> class.
    /// </summary>
    /// <param name="aktSamples">
    /// The akt samples. 
    /// </param>
    /// <param name="aktObjectNr">
    /// The akt object nr. 
    /// </param>
    /// <param name="aktxNr">
    /// The aktx nr. 
    /// </param>
    /// <param name="aktyNr">
    /// The akty nr. 
    /// </param>
    public LineFitClass(DataCollection aktSamples, int aktObjectNr, int aktxNr, int aktyNr)
    {
      this.regTyp = Constants.linReg;
      this.orgDataSamples = null;
      param = new double[3];
      this.wertX = new List<double>();
      this.wertY = new List<double>();
      this.ExtractDataColumnsFromVideoSamples(aktSamples, this.NumberOfObject, aktxNr, aktyNr);
      this.LineFitDisplaySample = null;
      this.TheorieDisplaySample = null;
    }

    #endregion

    #region Delegates

    /// <summary>
    ///   The ausgleich function.
    /// </summary>
    /// <param name="x"> The x. </param>
    public delegate double AusgleichFunction(double x);

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    ///   Gets the line fit abweichung.
    /// </summary>
    public double LineFitAbweichung
    {
      // mittlere Abweichung der Ausgleichsfunktion bzgl. der Messwerte; nur read-only
      get
      {
        return this._LineFitAbweichung;
      }
    }

    /// <summary>
    ///   Gets the line fit fkt str.
    /// </summary>
    public string LineFitFktStr
    {
      // Ausgabestring für die Ausgleichsfunktion; nur read-only
      get
      {
        return this._LineFitFktStr;
      }
    }

    /// <summary>
    ///   Gets or sets the gueltige stellen format string.
    /// </summary>
    public string gueltigeStellenFormatString
    {
      get
      {
        return this.myG;
      }

      set
      {
        this.myG = value;
        if (this.aktFunc != null)
        {
          this._LineFitFktStr = this.GetRegressionFunctionString(this.regTyp);
        }
      }
    }

    /// <summary>
    ///   Gets or sets the regression typ.
    /// </summary>
    public int regressionTyp
    {
      get
      {
        return this.regTyp;
      }

      set
      {
        if ((this.regTyp != value) && (value >= Constants.minRegWert) && (value <= Constants.maxRegWert))
        {
          this.regTyp = value;
          if (this.wertX.Count > 0)
          {
            // sind Datenreihen ausgewählt ?
            this.CalculateLineFitFunction(value); // neu berechnen !
          }
        }
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The ausgleichs exp.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsExp(double x)
    {
      return p[Constants.expReg, 0] * Math.Exp(x * p[Constants.expReg, 1]) + p[Constants.expReg, 2];
    }

    /// <summary>
    /// The ausgleichs exp spez.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsExpSpez(double x)
    {
      return p[Constants.expSpezReg, 0] * Math.Exp(x * p[Constants.expSpezReg, 1]);
    }

    /// <summary>
    /// The ausgleichs gerade.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsGerade(double x)
    {
      return p[Constants.linReg, 0] * x + p[Constants.linReg, 1];
    }

    /// <summary>
    /// The ausgleichs log.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsLog(double x)
    {
      return p[Constants.logReg, 0] * Math.Log(x * p[Constants.logReg, 1]);
    }

    /// <summary>
    /// The ausgleichs parabel.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsParabel(double x)
    {
      return (p[Constants.quadReg, 0] * x + p[Constants.quadReg, 1]) * x + p[Constants.quadReg, 2];
    }

    /// <summary>
    /// The ausgleichs pot.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsPot(double x)
    {
      return p[Constants.potReg, 0] * Math.Pow(x, p[Constants.potReg, 1]);
    }

    /// <summary>
    /// The ausgleichs reso.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsReso(double x)
    {
      return p[Constants.resoReg, 0]
             / Math.Pow(1 + p[Constants.resoReg, 1] * Math.Pow(x - p[Constants.resoReg, 2] / x, 2), 0.5);
    }

    /// <summary>
    /// The ausgleichs sin.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsSin(double x)
    {
      return p[Constants.sinReg, 0] * Math.Sin(x * p[Constants.sinReg, 1] + p[Constants.sinReg, 2])
             + p[Constants.sinReg, 3];
    }

    /// <summary>
    /// The ausgleichs sin exp.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsSinExp(double x)
    {
      return p[Constants.sinExpReg, 0] * Math.Sin(x * p[Constants.sinExpReg, 1])
             * Math.Exp(x * p[Constants.sinExpReg, 2]);
    }

    /// <summary>
    /// The null fkt.
    /// </summary>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public static double NullFkt(double x)
    {
      return 0;
    }

    /// <summary>
    /// The calculate line fit function.
    /// </summary>
    /// <param name="regressionTyp">
    /// The regression typ. 
    /// </param>
    public void CalculateLineFitFunction(int regressionTyp)
    {
      if (this.wertX.Count == 0)
      {
        this.CopySampleColumnsToArrays(0, this.xNr, this.yNr);
      }

      this.regTyp = regressionTyp;
      if (this.anzahl > 2)
      {
        switch (regressionTyp)
        {
          case Constants.linReg:
            this.BestimmeLinFkt();
            break;
          case Constants.expSpezReg:
            this.BestimmeExpSpezFkt();
            break;
          case Constants.logReg:
            this.BestimmeLogFkt();
            break;
          case Constants.potReg:
            this.BestimmePotFkt();
            break;
          case Constants.quadReg:
            this.BestimmeQuadratFkt();
            break;
          case Constants.expReg:
            this.BestimmeExpFkt();
            break;
          case Constants.sinReg:
            this.BestimmeSinFkt();
            break;
          case Constants.sinExpReg:
            this.BestimmeSinExpFkt();
            break;
          case Constants.resoReg:
            this.BestimmeResonanzFkt();
            break;
          default:
            this.BestimmeLinFkt();
            break;
        }

        this.GetRegressionFunctionStringAndAverageError(regressionTyp, -1);
      }
    }

    /// <summary>
    /// The calculate line fit theorie series.
    /// </summary>
    /// <param name="TheorieSamples">
    /// The theorie samples. 
    /// </param>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    public void CalculateLineFitTheorieSeries(DataCollection TheorieSamples, TFktTerm fx)
    {
      int k, anzahlPixel;
      double x;
      var p = new Point();
      List<Point> tempTheoriePoints;
      var tempParser = new Parse();

      if (fx == null)
      {
        return;
      }

      tempTheoriePoints = new List<Point>();
      if (tempParser.isLinearFunction(fx))
      {
        if (this.xNr == 2)
        {
          // zwei Punkte genügen bei x-y-Diagramm
          x = this.wertX[0]; // wertX[] - originale x-Werte der Wertepaare 
          p = new Point(x, tempParser.FreierFktWert(fx, x));
          tempTheoriePoints.Add(p);
          x = this.wertX[this.anzahl - 1];
          p = new Point(x, tempParser.FreierFktWert(fx, x));
          tempTheoriePoints.Add(p);
        }
        else
        {
          // Workaround beim t-?-Diagramm: gleichviele Punkte wie bei Originalwerten und gleiche x Werte. 
          for (k = 0; k < this.anzahl; k++)
          {
            x = this.wertX[k];
            p = new Point(x, tempParser.FreierFktWert(fx, x));
            tempTheoriePoints.Add(p);
          }
        }
      }
      else
      {
        // endPixelX und startPixelX
        // startX und endX wurden in aktualisiereTab(int aktObjectNr,int aktxNr, int aktyNr) bestimmt
        anzahlPixel = (int)(this.endPixelX - this.startPixelX);
        x = this.startX;

        for (k = 0; k < anzahlPixel; k++)
        {
          // Punkte im PixelAbstand (waagerecht) werden mit der theoretischen Funktion bestimmt.
          // führt bei t-?-Diagrammen zu falschen Darstellungen !!
          p = new Point(x, tempParser.FreierFktWert(fx, x));
          tempTheoriePoints.Add(p);
          x = x + this.stepX;
        }
      }

      this.CreateSampleFromCalculatedPoints(this.NumberOfObject, this.xNr, this.yNr, tempTheoriePoints, TheorieSamples);
      tempTheoriePoints.Clear();
    }

    /// <summary>
    /// The extract data columns from video samples.
    /// </summary>
    /// <param name="aktSamples">
    /// The akt samples. 
    /// </param>
    /// <param name="aktObjectNr">
    /// The akt object nr. 
    /// </param>
    /// <param name="aktxNr">
    /// The aktx nr. 
    /// </param>
    /// <param name="aktyNr">
    /// The akty nr. 
    /// </param>
    public void ExtractDataColumnsFromVideoSamples(DataCollection aktSamples, int aktObjectNr, int aktxNr, int aktyNr)
    {
      if ((this.orgDataSamples != aktSamples) || (this.NumberOfObject != aktObjectNr) || (this.xNr != aktxNr)
          || (this.yNr != aktyNr) || (this.wertX.Count == 0))
      {
        this.orgDataSamples = aktSamples;
        this.NumberOfObject = aktObjectNr;
        this.xNr = aktxNr;
        this.yNr = aktyNr;
        this.CopySampleColumnsToArrays(aktObjectNr, aktxNr, aktyNr);
      }
    }

    /// <summary>
    /// The get regression function string.
    /// </summary>
    /// <param name="regTyp">
    /// The reg typ. 
    /// </param>
    /// <returns>
    /// The <see cref="string"/> . 
    /// </returns>
    public string GetRegressionFunctionString(int regTyp)
    {
      double a = p[regTyp, 0];
      double b = p[regTyp, 1];
      double c = p[regTyp, 2];
      double d = p[regTyp, 3];
      string fktStr;
      if (this.aktFunc != null)
      {
        switch (regTyp)
        {
          case Constants.linReg:
            fktStr = string.Concat(a.ToString(this.myG), "*x + ", b.ToString(this.myG));
            break;
          case Constants.expSpezReg:
            fktStr = string.Concat(a.ToString(this.myG), "*exp(", b.ToString(this.myG), "*x)");
            break;
          case Constants.logReg:
            fktStr = string.Concat(a.ToString(this.myG), "*ln(", b.ToString(this.myG), "*x)");
            break;
          case Constants.potReg:
            fktStr = string.Concat(a.ToString(this.myG), "*x^", b.ToString(this.myG));
            break;
          case Constants.quadReg:
            fktStr = string.Concat(a.ToString(this.myG), "x² + ", b.ToString(this.myG), "x + ", c.ToString(this.myG));
            break;
          case Constants.expReg:
            fktStr = string.Concat(a.ToString(this.myG), "*exp(", b.ToString(this.myG), "*x) + ", c.ToString(this.myG));
            break;
          case Constants.sinReg:
            fktStr = string.Concat(
              a.ToString(this.myG), 
              "*Sin(", 
              b.ToString(this.myG), 
              "*x + ", 
              c.ToString(this.myG), 
              ") + ", 
              d.ToString(this.myG));
            this.aktFunc = AusgleichsSin;
            break;
          case Constants.sinExpReg:
            fktStr = string.Concat(
              a.ToString(this.myG), "*Sin(", b.ToString(this.myG), "*x )*exp( ", c.ToString(this.myG), "*x)");
            this.aktFunc = AusgleichsSinExp;
            break;
          case Constants.resoReg:
            fktStr = string.Concat(
              a.ToString(this.myG), "/Sqrt( 1 +", b.ToString(this.myG), "*( x - ", c.ToString(this.myG), "/x)² )");
            break;
          default:
            fktStr = " - ";
            this.aktFunc = NullFkt;
            break;
        }
      }
      else
      {
        fktStr = " - ";
      }

      return fktStr;
    }

    /// <summary>
    /// The get min max.
    /// </summary>
    /// <param name="werte">
    /// The werte. 
    /// </param>
    /// <param name="Min">
    /// The min. 
    /// </param>
    /// <param name="Max">
    /// The max. 
    /// </param>
    public void getMinMax(List<double> werte, out double Min, out double Max)
    {
      // int k;
      // double hilf;
      if (werte.Count == 0)
      {
        Min = 0;
        Max = 0;
        return;
      }

      // Falls noch keine Werte vorliegen
      Min = werte.Min();
      Max = werte.Max();

      /*          Min = werte[0];
            Max = Min;
            for (k = 1; k < anzahl; k++)
            {
                hilf = werte[k];
                if (Min > hilf) { Min = hilf; }
                else if (Max < hilf) { Max = hilf; }
            }
     */
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    ///   The berechne abc.
    /// </summary>
    private void BerechneABC()
    {
      double a, b, c;
      int k;
      a = 0;
      c = 0;
      b = this._AbschaetzungFuerB();
      for (k = 0; k < this.anzahl; k++)
      {
        a = a + (this.wertY[k] - this.wertY[k + 1]) / (Math.Exp(b * this.wertX[k]) - Math.Exp(b * this.wertX[k + 1]));
      }

      a = a / this.anzahl;
      for (k = 0; k < this.anzahl; k++)
      {
        c = c + this.wertY[0] - a * Math.Exp(b * this.wertX[0]);
      }

      c = c / this.anzahl;
    }

    /// <summary>
    ///   The bestimme exp fkt.
    /// </summary>
    private void BestimmeExpFkt()
    {
      double yMin, yMax;
      double yi, 
             fehler, 
             abw, 
             bestA, 
             bestB, 
             bestC, 
             schaetzWert, 
             schaetzStep, 
             steigungAmAnfang, 
             steigungAmEnde, 
             fehlergrenze;
      int k, iter, sign, z;
      List<double> tempWertY;
      bool weiter;

      // Schätzwert für Verschiebung in y-Richtung; 
      this.getMinMax(this.wertY, out yMin, out yMax);
      schaetzWert = 0;
      schaetzStep = 1;
      sign = 1;

      steigungAmAnfang = (this.wertY[0] - this.wertY[1]) / (this.wertX[0] - this.wertX[1]);
      steigungAmEnde = (this.wertY[this.anzahl - 1] - this.wertY[this.anzahl - 2])
                       / (this.wertX[this.anzahl - 1] - this.wertX[this.anzahl - 2]);
      if (((steigungAmAnfang < 0) && (steigungAmAnfang > -0.2)) || ((steigungAmEnde > 0) && (steigungAmEnde < 0.2)))
      {
        // Asymptote oben
        if (yMax < 0)
        {
          schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-yMax)));
          schaetzWert = 0;
        }
        else
        {
          if (yMax > 0)
          {
            schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(yMax)));
            schaetzWert = schaetzStep * 10;
          }
          else
          {
            schaetzWert = 1;
            schaetzStep = 0.1;
          }
        }

        sign = -1;
      }
      else
      {
        if (((steigungAmAnfang > 0) && (steigungAmAnfang < 0.2)) || ((steigungAmEnde < 0) && (steigungAmEnde > -0.2)))
        {
          // Asymptote unten
          if (yMin < 0)
          {
            schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-yMin)));
            schaetzWert = -schaetzStep * 10;
          }
          else
          {
            if (yMin > 0)
            {
              schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(yMin)));
              schaetzWert = Math.Floor(yMin * schaetzStep) / schaetzStep;
            }
            else
            {
              schaetzWert = -1;
              schaetzStep = 0.1;
            }
          }

          sign = 1;
        }
        else
        {
          // keine Aymptote erkennbar
          if (steigungAmAnfang < steigungAmEnde)
          {
            if (yMin > 0)
            {
              schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(yMin)));
              schaetzWert = -schaetzStep;
            }
            else
            {
              if (yMin < 0)
              {
                schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-yMin)));
                schaetzWert = Math.Floor(yMin * schaetzStep) / schaetzStep - schaetzStep;
              }
              else
              {
                schaetzWert = -1;
                schaetzStep = 0.1;
              }
            }

            sign = 1;
          }
          else
          {
            if (yMax > 0)
            {
              schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(yMax)));
              schaetzWert = 10 * schaetzStep;
            }
            else
            {
              if (yMax < 0)
              {
                schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-yMax)));
                schaetzWert = 0;
              }
              else
              {
                schaetzWert = 1;
                schaetzStep = 0.1;
              }
            }

            sign = -1;
          }
        }
      }

      tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertY.Add(0);
      }

      // iterieren über c; 
      bestA = 0;
      bestB = 0;
      bestC = 0;
      abw = startAbw;
      iter = 1;
      weiter = true;
      fehlergrenze = this.anzahl * genauigkeit;
      z = 0;
      do
      {
        for (k = 0; k < this.anzahl; k++)
        {
          tempWertY[k] = Math.Log(Math.Abs(this.wertY[k] - schaetzWert));
        }

        this._doRegress(this.anzahl, this.wertX, tempWertY, param);
        param[0] = Math.Exp(param[0]);
        if (schaetzWert > yMax)
        {
          param[0] = -param[0];
        }

        fehler = 0;
        for (k = 0; k < this.anzahl; k++)
        {
          yi = param[0] * Math.Exp(param[1] * this.wertX[k]) + schaetzWert;
          fehler = fehler + (yi - this.wertY[k]) * (yi - this.wertY[k]);
        }

        if (abw > fehler)
        {
          abw = fehler;
          bestA = param[0];
          bestB = param[1];
          bestC = schaetzWert;
          if ((abw < fehlergrenze) || (schaetzStep < minStep))
          {
            weiter = false;
          }
        }

        if (z < 9)
        {
          schaetzWert = schaetzWert + sign * schaetzStep;
          z = z + 1;
          if (((sign == 1) && (schaetzWert > yMin)) || ((sign == -1) && (schaetzWert < yMax)))
          {
            z = 10;
          }
        }

        if (z >= 9)
        {
          if (schaetzStep > minStep)
          {
            schaetzWert = bestC - sign * 0.9 * schaetzStep;
            schaetzStep = schaetzStep * 0.1;
            z = -10;
            iter = iter - 10;
          }
          else
          {
            weiter = false;
          }
        }

        iter++;
      }
      while (weiter && (iter < maxIteration));
      p[Constants.expReg, 0] = bestA;
      p[Constants.expReg, 1] = bestB;
      p[Constants.expReg, 2] = bestC;
      p[Constants.expReg, 5] = abw / this.anzahl;
    }

    /// <summary>
    ///   The bestimme exp spez fkt.
    /// </summary>
    private void BestimmeExpSpezFkt()
    {
      int k;
      var tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertY.Add(Math.Log(this.wertY[k]));
      }

      this._doRegress(this.anzahl, this.wertX, tempWertY, param);
      param[0] = Math.Exp(param[0]);
      p[Constants.expSpezReg, 0] = param[0];
      p[Constants.expSpezReg, 1] = param[1];
    }

    /// <summary>
    ///   The bestimme lin fkt.
    /// </summary>
    private void BestimmeLinFkt()
    {
      this._doRegress(this.anzahl, this.wertX, this.wertY, param);
      p[Constants.linReg, 0] = param[1];
      p[Constants.linReg, 1] = param[0];
    }

    /// <summary>
    ///   The bestimme log fkt.
    /// </summary>
    private void BestimmeLogFkt()
    {
      int k;
      double hilf;
      var tempWertX = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertX.Add(Math.Log(this.wertX[k]));
      }

      this._doRegress(this.anzahl, tempWertX, this.wertY, param);

      hilf = param[1];
      param[1] = Math.Exp(param[0] / hilf);
      param[0] = hilf;
      p[Constants.logReg, 0] = param[0];
      p[Constants.logReg, 1] = param[1];
    }

    /// <summary>
    ///   The bestimme pot fkt.
    /// </summary>
    private void BestimmePotFkt()
    {
      int k, start;
      var tempWertX = new List<double>();
      var tempWertY = new List<double>();

      if ((this.wertX[0] <= 0) || (this.wertY[0] <= 0))
      {
        start = 1;
      }
      else
      {
        start = 0;
      }

      for (k = start; k < this.anzahl; k++)
      {
        tempWertX.Add(Math.Log(this.wertX[k]));
        tempWertY.Add(Math.Log(this.wertY[k]));
      }

      this._doRegress(this.anzahl - start, tempWertX, tempWertY, param);
      param[0] = Math.Exp(param[0]);
      p[Constants.potReg, 0] = param[0];
      p[Constants.potReg, 1] = param[1];
    }

    /// <summary>
    ///   The bestimme quadrat fkt.
    /// </summary>
    private void BestimmeQuadratFkt()
    {
      int k;
      double sumX4, sumX3, sumX2, sumX, sumXY, sumX2Y, sumY, xi, yi, a, b, c;
      Matrix M, v, lsg;

      sumX4 = 0;
      sumX3 = 0;
      sumX2 = 0;
      sumX = 0;
      sumXY = 0;
      sumX2Y = 0;
      sumY = 0;

      for (k = 0; k < this.anzahl; k++)
      {
        xi = this.wertX[k];
        yi = this.wertY[k];
        sumX = sumX + xi;
        sumY = sumY + yi;
        sumXY = sumXY + yi * xi;
        xi = xi * xi;
        sumX2 = sumX2 + xi;
        sumX2Y = sumX2Y + xi * yi;
        sumX3 = sumX3 + xi * this.wertX[k];
        sumX4 = sumX4 + xi * xi;
      }

      // LGS:
      // a*sumX4 + b*sumX3 +c*sumX2 = sumX2Y
      // a*sumX3 + b*sumX2 +c*sumX  = sumXY
      // a*sumX2 + b*sumX  +c*k     = sumY
      M = new Matrix(3, 3);
      v = new Matrix(3, 1);
      lsg = new Matrix(3, 1);
      M[0, 0] = sumX4;
      M[0, 1] = sumX3;
      M[0, 2] = sumX2;
      v[0, 0] = sumX2Y;
      M[1, 0] = sumX3;
      M[1, 1] = sumX2;
      M[1, 2] = sumX;
      v[1, 0] = sumXY;
      M[2, 0] = sumX2;
      M[2, 1] = sumX;
      M[2, 2] = this.anzahl;
      v[2, 0] = sumY;
      lsg = Matrix.SolveLinear(M, v);
      a = lsg[0, 0];
      b = lsg[1, 0];
      c = lsg[2, 0];
      p[Constants.quadReg, 0] = a;
      p[Constants.quadReg, 1] = b;
      p[Constants.quadReg, 2] = c;
    }

    /*mechanische Schwingungen:
         *Gerthsen und andere
         *
         *                         K0                                   K0                 
         * A(w) = --------------------------------------  -->  ------------------------ 
         *        Sqrt( (w0^2 - w^2)^2 + b^2/m^2 *w^2 )         Sqrt( (R - x)^2 + S*x )  
         *
         * 
         * 
         * SchwingKreis:
         *                 U0                                      U0                                U0*L                                 a
         * I0 = -------------------------------  =  ------------------------------------ = ----------------------------------- --> ------------------------
         *      Sqrt( R^2 + (w*L - 1/(w*C) )^2 )    Sqrt( R^2 + L^2*(w - 1/(w*L*C) )^2 )   Sqrt( (R/L)^2 + (w - 1/(w*L*C) )^2 )    Sqrt( b + (x - c/x )^2 )
         *      
         * a= U0*L; b = (R/L)^2; c = 1/(L*C)
         */

    /// <summary>
    ///   The bestimme resonanz fkt.
    /// </summary>
    private void BestimmeResonanzFkt()
    {
      double maxSchaetz, schaetzWert, schaetzStep, hilf, maxY, offset, grenze;
      double xi, yi, hilfX, a, b, c, abw, fehler;
      double bestA, bestB, bestC;
      int anz, k, z, iter, maxPos;
      bool weiter;
      var tempWertX = new List<double>();
      var tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertX.Add(0);
        tempWertY.Add(0);
      }

      bestA = 0;
      bestB = 0;
      bestC = 0;
      maxSchaetz = 0;
      schaetzWert = 0;
      schaetzStep = 0;

      // Parameter abschätzen
      maxPos = -1;
      k = 0;
      maxY = -1E150;
      for (k = 0; k < this.anzahl; k++)
      {
        yi = this.wertY[k];
        if (yi > maxY)
        {
          maxY = yi;
          maxPos = k;
        }
      }

      offset = 10000000000.0;
      while (offset > maxY / 100)
      {
        offset = offset * 0.1;
      }

      // Quadrat des x-Wertes der ResonanzStelle ist ungefähr der Schaetzwert
      if (maxPos > 0)
      {
        schaetzWert = this.wertX[maxPos - 1];
      }
      else
      {
        schaetzWert = this.wertX[maxPos];
      }

      if (maxPos < this.anzahl - 11)
      {
        maxSchaetz = this.wertX[maxPos + 1];
      }
      else
      {
        maxSchaetz = this.wertX[maxPos];
      }

      maxSchaetz = maxSchaetz * maxSchaetz;
      schaetzWert = schaetzWert * schaetzWert;
      grenze = 1;
      while (grenze > (maxSchaetz - schaetzWert) / 1000)
      {
        grenze = grenze * 0.1;
      }

      schaetzStep = grenze * 1000;
      schaetzWert = Math.Floor(schaetzWert);

      if (maxPos == 0)
      {
        schaetzWert = schaetzWert - 2 * schaetzStep;
      }

      weiter = true;
      z = 9 - (int)Math.Floor((maxSchaetz - schaetzWert) / schaetzStep);

      abw = startAbw;
      iter = 0;
      while (weiter && (iter < maxIteration))
      {
        // Iteration über c
        // lin. Reg vorbereiten    1/y^2 = 1/a^2 + b/a^2 * (x - c/x) = a' + b' * hilfX
        anz = 0;
        for (k = 0; k < this.anzahl; k++)
        {
          xi = this.wertX[k];
          yi = this.wertY[k];
          if (yi != 0)
          {
            hilfX = xi - schaetzWert / xi;
            tempWertY[anz] = hilfX * hilfX;
            tempWertY[anz] = 1 / (yi * yi);
            anz = anz + 1;
          }
        }

        this._doRegress(anz, tempWertX, tempWertY, param);
        a = param[0]; // a' 
        b = param[1] / a; // b'/a'
        a = Math.Pow(a, -0.5);
        c = schaetzWert;
        fehler = 0;
        for (k = 0; k < this.anzahl; k++)
        {
          xi = this.wertX[k];
          hilfX = xi - c / xi;
          hilf = a / Math.Pow(1 + b * hilfX * hilfX, 0.5) - this.wertY[k];
          fehler = fehler + hilf * hilf;
        }

        iter++;
        if (abw > fehler)
        {
          abw = fehler;
          bestA = a;
          bestB = b;
          bestC = c;
        }

        if ((z < 9) && (schaetzWert <= maxSchaetz))
        {
          schaetzWert = schaetzWert + schaetzStep;
          z++;
        }
        else if (schaetzStep > grenze)
        {
          schaetzWert = bestC - 0.9 * schaetzStep;
          z = -10;
          schaetzStep = schaetzStep * 0.1;
        }
      }

      p[Constants.resoReg, 0] = bestA;
      p[Constants.resoReg, 1] = bestB;
      p[Constants.resoReg, 2] = bestC;
      p[Constants.resoReg, 5] = abw / this.anzahl;
    }

    /// <summary>
    ///   The bestimme sin exp fkt.
    /// </summary>
    private void BestimmeSinExpFkt()
    {
      double maxSchaetz, schaetzWert, schaetzStep, hilf;
      double xi, sinbxi, a, b, c, abw, fehler;
      double bestA, bestB, bestC;
      int anz, k, z, iter;
      bool weiter;
      List<double> tempWertX, tempWertY;

      // Schätzwert für b ermitteln:  y=a*sin(b*x)*exp(c*x);
      this._getPeriode(this.wertX, this.wertY, out schaetzWert, out maxSchaetz, out schaetzStep);

      weiter = true;
      z = 0;
      iter = 0;
      abw = startAbw;
      a = 0;
      b = schaetzWert;
      c = 0;
      bestA = 0;
      bestB = b;
      bestC = 0;
      tempWertX = new List<double>();
      tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertX.Add(0);
        tempWertY.Add(0);
      }

      while (weiter && (iter < maxIteration))
      {
        anz = 0;
        for (k = 0; k < this.anzahl; k++)
        {
          xi = this.wertX[k];
          sinbxi = Math.Sin(schaetzWert * xi);
          if (sinbxi != 0)
          {
            hilf = this.wertY[k] / sinbxi;
            if (hilf > 0)
            {
              tempWertX[anz] = xi;
              tempWertY[anz] = Math.Log(hilf);
              anz = anz + 1;
            }
          }
        }

        // for k
        if (anz >= 0.8 * this.anzahl)
        {
          // mehr als 80% der Wertepaare können bei dieser Schätzung für b berücksichtigt werden
          this._doRegress(anz, tempWertX, tempWertY, param);
          a = Math.Exp(param[0]);
          c = param[1];
          fehler = 0;
          for (k = 0; k < this.anzahl; k++)
          {
            xi = this.wertX[k];
            hilf = a * Math.Sin(schaetzWert * xi) * Math.Exp(c * xi) - this.wertY[k];
            fehler = fehler + hilf * hilf;
          }

          iter = iter + 1;
          if (abw > fehler)
          {
            abw = fehler;
            bestA = a;
            bestB = schaetzWert;
            bestC = c;
          }
        }
        else
        {
          z = z - 1;
        }

        if ((z < 9) && (schaetzWert < maxSchaetz))
        {
          schaetzWert = schaetzWert + schaetzStep;
          z = z + 1;
        }
        else
        {
          if (schaetzStep > minStep)
          {
            schaetzWert = bestB - 0.9 * schaetzStep;
            z = -10;
            schaetzStep = schaetzStep * 0.1;
          }
          else
          {
            weiter = false;
          }
        }
      }

      // while weiter
      p[Constants.sinExpReg, 0] = bestA;
      p[Constants.sinExpReg, 1] = bestB;
      p[Constants.sinExpReg, 2] = bestC;
      p[Constants.sinExpReg, 5] = abw / this.anzahl;
    }

    /// <summary>
    ///   The bestimme sin fkt.
    /// </summary>
    private void BestimmeSinFkt()
    {
      int n, k, iter, z;
      double yMin = 0;
      double yMax = 0;
      double maxSchaetz, schaetzWert, schaetzStep;
      double sumSin, 
             sumSin2, 
             sumSinY, 
             sumCosY, 
             sumSinCos, 
             sumCos, 
             sumCos2, 
             sumY, 
             xi, 
             xci, 
             yi, 
             fehler, 
             a1, 
             c1, 
             a, 
             b, 
             c, 
             d, 
             abw;
      List<double> tempWertY;
      Matrix M, v, lsg;
      double bestA, bestB, bestC, bestD;
      bool weiter;

      bestA = 0;
      bestB = 0;
      bestC = 0;
      bestD = 0;
      this.getMinMax(this.wertY, out yMin, out yMax);

      // Amplitude a:
      a = (yMax - yMin) * 1.02 / 2;

      // y-Verschiebung d:
      d = (yMax + yMin) / 2;

      // Periodenlänge:
      tempWertY = new List<double>();

      for (k = 0; k < this.anzahl; k++)
      {
        tempWertY.Add(this.wertY[k] - d);
      }

      this._getPeriode(this.wertX, tempWertY, out schaetzWert, out maxSchaetz, out schaetzStep);

      // a*sin(b*x+c)+d = a*cos(c)*sin(b*x) + a*sin(c)*cos(b*x)+d = a1*sin(b*x) + c1*cos(b*x) + d;
      // iteration über b:
      weiter = true;
      iter = 0;
      abw = startAbw;
      c = 0;
      b = schaetzWert;
      z = 0;
      while (weiter && (iter < maxIteration))
      {
        sumSin = 0;
        sumSin2 = 0;
        sumSinY = 0;
        sumSinCos = 0;
        sumCos = 0;
        sumCos2 = 0;
        sumCosY = 0;
        sumY = 0;
        fehler = 0;
        for (n = 0; n < this.anzahl; n++)
        {
          xi = this.wertX[n];
          yi = this.wertY[n];
          xci = Math.Cos(schaetzWert * xi);
          xi = Math.Sin(schaetzWert * xi);
          sumSin = sumSin + xi;
          sumY = sumY + yi;
          sumSinY = sumSinY + yi * xi;

          sumSin2 = sumSin2 + xi * xi;
          sumCosY = sumCosY + xci * yi;
          sumCos = sumCos + xci;
          sumCos2 = sumCos2 + xci * xci;
          sumSinCos = sumSinCos + xi * xci;
        }

        // LGS:
        // a1*sumSin2   + c1*sumSinCos + d*sumSin  = sumSinY
        // a1*sumSinCos + c1*sumCos2   + d*sumCos  = sumCosY
        // a1*sumSin    + c1*sumCos    + d*k       = sumY
        M = new Matrix(3, 3);
        v = new Matrix(3, 1);
        lsg = new Matrix(3, 1);
        M[0, 0] = sumSin2;
        M[0, 1] = sumSinCos;
        M[0, 2] = sumSin;
        v[0, 0] = sumSinY;
        M[1, 0] = sumSinCos;
        M[1, 1] = sumCos2;
        M[1, 2] = sumCos;
        v[1, 0] = sumCosY;
        M[2, 0] = sumSin;
        M[2, 1] = sumCos;
        M[2, 2] = this.anzahl;
        v[2, 0] = sumY;
        lsg = Matrix.SolveLinear(M, v);
        a1 = lsg[0, 0];
        c1 = lsg[1, 0];
        a = Math.Sqrt(a1 * a1 + c1 * c1);
        if (a1 < 0)
        {
          a = -a;
        }

        b = schaetzWert;
        c = Math.Asin(c1 / a);
        d = lsg[2, 0];

        fehler = 0;
        for (n = 0; n < this.anzahl; n++)
        {
          yi = a * Math.Sin(b * this.wertX[n] + c) + d - this.wertY[n];
          fehler = fehler + yi * yi;
        }

        iter = iter + 1;
        if (abw > fehler)
        {
          abw = fehler;
          bestA = a;
          bestB = schaetzWert;
          bestC = c;
          bestD = d;
          z = 5;
        }

        if ((z < 9) && (schaetzWert < maxSchaetz))
        {
          schaetzWert = schaetzWert + schaetzStep;
          z = z + 1;
        }
        else
        {
          if (schaetzStep > minStep)
          {
            schaetzWert = bestB - 0.9 * schaetzStep;
            z = -10;
            schaetzStep = schaetzStep * 0.1;
          }
          else
          {
            weiter = false;
          }
        }
      }

      // Ende While-Schleife
      if (bestA < 0)
      {
        bestA = -bestA;
        bestC = bestC + Math.PI;
      }

      p[Constants.sinReg, 0] = bestA;
      p[Constants.sinReg, 1] = bestB;
      p[Constants.sinReg, 2] = bestC;
      p[Constants.sinReg, 3] = bestD;
      p[Constants.sinReg, 5] = abw / this.anzahl;
    }

    /// <summary>
    /// The calculate line fit series.
    /// </summary>
    /// <param name="lineFitSamples">
    /// The line fit samples. 
    /// </param>
    private void CalculateLineFitSeries(DataCollection lineFitSamples)
    {
      int k, anzahlPixel;
      double x;
      Point p;
      List<Point> tempLineFitPoints;

      if (this.aktFunc == null)
      {
        return;
      }

      tempLineFitPoints = new List<Point>();
      if (this.regTyp == Constants.linReg)
      {
        // Sonderfall lineare Regression; Anzahl der Berechnungen wird drastisch reduziert,                                  
        // da Chart selbst Geraden zeichnen kann. 
        if (this.xNr == 2)
        {
          // zwei Punkte genügen bei x-y-Diagramm
          x = this.wertX[0]; // wertX[] - originale x-Werte der Wertepaare 
          p = new Point(x, this.aktFunc(x));
          tempLineFitPoints.Add(p);
          x = this.wertX[this.anzahl - 1];
          p = new Point(x, this.aktFunc(x));
          tempLineFitPoints.Add(p);
        }
        else
        {
          // Workaround beim t-?-Diagramm: gleichviele Punkte wie bei Originalwerten und gleiche x Werte. 
          for (k = 0; k < this.anzahl; k++)
          {
            x = this.wertX[k];
            p = new Point(x, this.aktFunc(x));
            tempLineFitPoints.Add(p);
          }
        }
      }
      else
      {
        // endPixelX und startPixelX
        // startX,endX und stepX wurden in aktualisiereTab(int aktObjectNr,int aktxNr, int aktyNr) bestimmt
        anzahlPixel = (int)(this.endPixelX - this.startPixelX); // Anzahl der Pixel im betrachtenen Bereich
        x = this.startX;
        for (k = 0; k < anzahlPixel; k++)
        {
          // Punkte im PixelAbstand (waagerecht) werden mit der Ausgleichsfunktion bestimmt.
          // führt bei t-?-Diagrammen zu falschen Darstellungen !!   
          p = new Point(x, this.aktFunc(x));
          tempLineFitPoints.Add(p);
          x = x + this.stepX;
        }

        /* 
        * Versuch, Zwischenpunkte zu erhalten, die die gleichen t-Koordinaten wie die Originalpunkte haben. Auch kein Erfolg 
        * 
                int nr = 1;
                anzahlPixel = (int)(endPixelX - startPixelX);  //Anzahl der Pixel im betrachtenen Bereich
                if (wertX[0] < wertX[anzahl-1]) 
                 {
                     if (stepX < 0) { stepX = -stepX; }
                     x = wertX[0];
                     for (k = 0; k < anzahlPixel; k++)  //Punkte im PixelAbstand (waagerecht) werden mit der Ausgleichsfunktion bestimmt.
                     {                                  //führt bei t-?-Diagrammen zu falschen Darstellungen !!   
                         p = new Point(x, aktFunc(x));
                         tempLineFitPoints.Add(p);
                         x = x + stepX;
                         if (nr<anzahl)
                         {
                             if (x > wertX[nr]) { x = wertX[nr]; nr = nr + 1; }
                         }
                     }
                }
                else
                {
                    if (stepX < 0) { stepX = -stepX; }
                    nr = anzahl - 2;
                    x = wertX[anzahl - 1];
                    for (k = 0; k < anzahlPixel; k++)  //Punkte im PixelAbstand (waagerecht) werden mit der Ausgleichsfunktion bestimmt.
                    {                                  //führt bei t-?-Diagrammen zu falschen Darstellungen !!   
                        p = new Point(x, aktFunc(x));
                        tempLineFitPoints.Add(p);
                        x = x - stepX;
                        if (nr > 0)
                        {
                            if (x < wertX[nr]) { x = wertX[nr]; nr = nr - 1; }
                        }

                    }
                }
        */
      }

      this.CreateSampleFromCalculatedPoints(this.NumberOfObject, this.xNr, this.yNr, tempLineFitPoints, lineFitSamples);
      tempLineFitPoints.Clear();
    }

    /// <summary>
    /// The copy sample columns to arrays.
    /// </summary>
    /// <param name="aktObjectNr">
    /// The akt object nr. 
    /// </param>
    /// <param name="aktxNr">
    /// The aktx nr. 
    /// </param>
    /// <param name="aktyNr">
    /// The akty nr. 
    /// </param>
    private void CopySampleColumnsToArrays(int aktObjectNr, int aktxNr, int aktyNr)
    {
      DataSample aktDataSample;
      int firstPossibleValueNr = 0;

      if (this.wertX != null)
      {
        this.wertX.Clear();
        this.wertY.Clear();
      }

      this.anzahl = 0;
      if ((aktxNr == 2) & (aktyNr == 3))
      {
        // x-y-Diagramm
        foreach (TimeSample sample in this.orgDataSamples)
        {
          aktDataSample = sample.Object[aktObjectNr];
          if (this.anzahl == 0)
          {
            this.startPixelX = aktDataSample.PixelX;
          }

          this.wertX.Add(aktDataSample.PositionX);
          this.wertY.Add(aktDataSample.PositionY);
          this.endPixelX = aktDataSample.PixelX;
          this.anzahl++;
        }

        if (this.anzahl > 0)
        {
          this.startX = this.wertX[firstPossibleValueNr];
          this.endX = this.wertX[this.anzahl - 1];
        }
        else
        {
          this.startX = 0;
          this.endX = 0;
        }
      }
      else
      {
        // t-?-Diagramm
        if (aktyNr >= 7)
        {
          // t-a-Diagramm,                                               
          firstPossibleValueNr = 2;
        }
          
          // erst der dritte Wert für die Beschleunigung ist definiert - Zählung startet bei 0
        else
        {
          if (aktyNr >= 4)
          {
            firstPossibleValueNr = 1;
          }

          // t-v-Diagramm, erst der zweite Wert für die Beschleunigung ist definiert
        }

        this.startTime = 0; // sonst meckert der Compiler
        foreach (TimeSample sample in this.orgDataSamples)
        {
          if (this.anzahl >= firstPossibleValueNr)
          {
            aktDataSample = sample.Object[aktObjectNr];
            if (this.anzahl == firstPossibleValueNr)
            {
              this.startPixelX = aktDataSample.PixelX;
              this.startTime = sample.Timestamp - 40;
            }

            // im Diagramm ist die kleinste Zeit bei 1 und ändert sich in 1er-Schritten, (realer) zeitlicher Abstand beträgt 40 ms
            this.wertX.Add(sample.Timestamp - this.startTime);
            this.endPixelX = aktDataSample.PixelX;
            switch (aktyNr)
            {
              case 2:
                this.wertY.Add(aktDataSample.PositionX);
                break;
              case 3:
                this.wertY.Add(aktDataSample.PositionY);
                break;
              case 4:
                this.wertY.Add(aktDataSample.Velocity.Value);
                break;
              case 5:
                this.wertY.Add(aktDataSample.VelocityX.Value);
                break;
              case 6:
                this.wertY.Add(aktDataSample.VelocityY.Value);
                break;
              case 7:
                this.wertY.Add(aktDataSample.Acceleration.Value);
                break;
              case 8:
                this.wertY.Add(aktDataSample.AccelerationX.Value);
                break;
              case 9:
                this.wertY.Add(aktDataSample.AccelerationY.Value);
                break;
            }
          }

          this.anzahl++;
        }

        this.anzahl = this.wertX.Count;
        if (this.anzahl > 0)
        {
          this.startX = this.wertX[0];
          this.endX = this.wertX[this.anzahl - 1];
        }
        else
        {
          this.startX = 0;
          this.endX = 0;
        }
      }

      if (this.startPixelX > this.endPixelX)
      {
        this.startX = this.wertX[this.anzahl - 1];
        this.endX = this.wertX[firstPossibleValueNr];
        double hilf = this.startPixelX;
        this.startPixelX = this.endPixelX;
        this.endPixelX = hilf;
      }

      this.stepX = (this.endX - this.startX) / (this.endPixelX - this.startPixelX);

      // Schrittweite zum Ausfüllen der Zwischenräume ( in x-Achsen-Richtung )
    }

    /// <summary>
    /// The create sample from calculated points.
    /// </summary>
    /// <param name="aktObjectNr">
    /// The akt object nr. 
    /// </param>
    /// <param name="aktxNr">
    /// The aktx nr. 
    /// </param>
    /// <param name="aktyNr">
    /// The akty nr. 
    /// </param>
    /// <param name="pointList">
    /// The point list. 
    /// </param>
    /// <param name="samples">
    /// The samples. 
    /// </param>
    private void CreateSampleFromCalculatedPoints(
      int aktObjectNr, int aktxNr, int aktyNr, List<Point> pointList, DataCollection samples)
    {
      DataSample aktDataSample;
      TimeSample timeHilf;
      Point p;
      int k, j;

      samples.Clear();

      if ((aktxNr == 2) & (aktyNr == 3))
      {
        // Ausgleichskurve für x-y-Diagramm vorbereiten
        for (k = 0; k < pointList.Count(); k++)
        {
          // Punkte im PixelAbstand (waagerecht) werden mit der Ausgleichsfunktion bestimmt.
          timeHilf = new TimeSample();
          timeHilf.Framenumber = k;
          for (j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
          {
            timeHilf.Object[j] = new DataSample();
          }

          aktDataSample = timeHilf.Object[aktObjectNr];
          p = pointList[k];
          aktDataSample.PositionX = p.X;
          aktDataSample.PositionY = p.Y;
          samples.Add(timeHilf);
        }
      }
      else
      {
        // Ausgleichskurve für t-?-Diagramm vorbereiten
        for (k = 0; k < pointList.Count(); k++)
        {
          // Punkte im PixelAbstand (waagerecht) werden mit der Ausgleichsfunktion bestimmt.
          timeHilf = new TimeSample();
          for (j = 0; j < Video.Instance.ImageProcessing.NumberOfTrackedObjects; j++)
          {
            timeHilf.Object[j] = new DataSample();
          }

          aktDataSample = timeHilf.Object[aktObjectNr];
          p = pointList[k];
          timeHilf.Timestamp = (long)(p.X + this.startTime);
          switch (aktyNr)
          {
            case 2:
              aktDataSample.PositionX = p.Y;
              break;
            case 3:
              aktDataSample.PositionY = p.Y;
              break;
            case 4:
              aktDataSample.VelocityI = p.Y;
              break;
            case 5:
              aktDataSample.VelocityXI = p.Y;
              break;
            case 6:
              aktDataSample.VelocityYI = p.Y;
              break;
            case 7:
              aktDataSample.AccelerationI = p.Y;
              break;
            case 8:
              aktDataSample.AccelerationXI = p.Y;
              break;
            case 9:
              aktDataSample.AccelerationYI = p.Y;
              break;
          }

          samples.Add(timeHilf);
        }
      }
    }

    /// <summary>
    /// The get regression function string and average error.
    /// </summary>
    /// <param name="regTyp">
    /// The reg typ. 
    /// </param>
    /// <param name="fehler">
    /// The fehler. 
    /// </param>
    private void GetRegressionFunctionStringAndAverageError(int regTyp, double fehler)
    {
      double yi;
      int k;

      switch (regTyp)
      {
        case Constants.linReg:
          this.aktFunc = AusgleichsGerade;
          break;
        case Constants.expSpezReg:
          this.aktFunc = AusgleichsExpSpez;
          break;
        case Constants.logReg:
          this.aktFunc = AusgleichsLog;
          break;
        case Constants.potReg:
          this.aktFunc = AusgleichsPot;
          break;
        case Constants.quadReg:
          this.aktFunc = AusgleichsParabel;
          break;
        case Constants.expReg:
          this.aktFunc = AusgleichsExp;
          break;
        case Constants.sinReg:
          this.aktFunc = AusgleichsSin;
          break;
        case Constants.sinExpReg:
          this.aktFunc = AusgleichsSinExp;
          break;
        case Constants.resoReg:
          this.aktFunc = AusgleichsReso;
          break;
        default:
          this.aktFunc = NullFkt;
          break;
      }

      this._LineFitFktStr = this.GetRegressionFunctionString(regTyp);
      if (this.aktFunc != NullFkt)
      {
        // Berechnung der Ausgleichsfunktion erfolgreich?
        if (this.LineFitDisplaySample == null)
        {
          this.LineFitDisplaySample = new DataCollection();
        }

        this.CalculateLineFitSeries(this.LineFitDisplaySample); // Punkte mit Ausgleichsfunktion bestimmen

        if (fehler < -1.5)
        {
          fehler = -2;
        }
        else
        {
          if (fehler < 0)
          {
            fehler = 0;
            for (k = 0; k < this.anzahl; k++)
            {
              yi = this.aktFunc(this.wertX[k]) - this.wertY[k];
              fehler = fehler + yi * yi;
            }

            fehler = fehler / this.anzahl;
          }
        }
      }
      else
      {
        fehler = -2;
      }

      this._LineFitAbweichung = fehler;
    }

    /// <summary>
    ///   The _ abschaetzung fuer b.
    /// </summary>
    /// <returns> The <see cref="double" /> . </returns>
    private double _AbschaetzungFuerB()
    {
      /* für f(x)=a*exp(b*x)+c bzw. f(x)= a*sin(b*x+c)+d gilt:
             * b=f''(x)/f'(x)  (bei der Sinusfunktion ergibt sich -b !!
             * mit der Annahme (f(x3)-f(x1))/(x3-x1) ungefähr gleich f'(x2)
             * ergibt sich nachfolgende Rechnung: )
            */
      return ((this.wertY[4] - this.wertY[2]) / (this.wertX[4] - this.wertX[2])
              - (this.wertY[2] - this.wertY[0]) / this.wertX[0] - this.wertX[0]) / (this.wertY[3] - this.wertY[1]);
    }

    /// <summary>
    /// The _do regress.
    /// </summary>
    /// <param name="anzahl">
    /// The anzahl. 
    /// </param>
    /// <param name="wertX">
    /// The wert x. 
    /// </param>
    /// <param name="wertY">
    /// The wert y. 
    /// </param>
    /// <param name="param">
    /// The param. 
    /// </param>
    private void _doRegress(int anzahl, List<double> wertX, List<double> wertY, double[] param)
    {
      double sumX, sumX2, sumXY, sumY, x, y;
      int k;
      sumX = 0;
      sumX2 = 0;
      sumXY = 0;
      sumY = 0;
      for (k = 0; k < anzahl; k++)
      {
        x = wertX[k];
        y = wertY[k];
        sumX = sumX + x;
        sumX2 = sumX2 + x * x;
        sumY = sumY + y;
        sumXY = sumXY + x * y;
      }

      double nenner = anzahl * sumX2 - sumX * sumX;

      // y = param[1]*x+param[0]
      if (nenner != 0)
      {
        param[1] = (anzahl * sumXY - sumX * sumY) / nenner; // Steigung
        param[0] = (sumY * sumX2 - sumX * sumXY) / nenner; // y-Achsenabschnitt
      }
    }

    /// <summary>
    /// The _get periode.
    /// </summary>
    /// <param name="dataX">
    /// The data x. 
    /// </param>
    /// <param name="dataY">
    /// The data y. 
    /// </param>
    /// <param name="schaetzWert">
    /// The schaetz wert. 
    /// </param>
    /// <param name="maxSchaetz">
    /// The max schaetz. 
    /// </param>
    /// <param name="schaetzStep">
    /// The schaetz step. 
    /// </param>
    private void _getPeriode(
      List<double> dataX, List<double> dataY, out double schaetzWert, out double maxSchaetz, out double schaetzStep)
    {
      double hilf, minX, maxX;
      int sign, anz, k;

      // Schätzwert für b ermitteln:  y=a*sin(b*x)*exp(c*x);
      this.getMinMax(dataX, out minX, out maxX);
      sign = Math.Sign(dataY[0]);
      if (sign == 0)
      {
        if (Math.Sign(dataY[1]) >= 0)
        {
          sign = 1;
        }
        else
        {
          sign = -1;
        }
      }

      anz = 1;
      for (k = 0; k < this.anzahl - 1; k++)
      {
        if (sign != Math.Sign(dataY[k + 1]))
        {
          anz = anz + 1;
          sign = -sign;
        }
      }

      maxSchaetz = anz * Math.PI / (maxX - minX);
      schaetzWert = (anz - 1) * Math.PI / maxX;
      schaetzStep = 10000;
      hilf = maxSchaetz - schaetzWert;
      while (schaetzStep >= hilf)
      {
        schaetzStep = schaetzStep * 0.1;
      }

      schaetzWert = Math.Floor(schaetzWert / schaetzStep) * schaetzStep;
      maxSchaetz = Math.Floor(maxSchaetz / schaetzStep + 1) * schaetzStep;
      if (schaetzWert == 0)
      {
        schaetzWert = schaetzStep;
      }
    }

    #endregion

    /*
        private void RegressAuswahl()
        {
            double minX, maxX, minY, maxY;

            getMinMax(wertX, anzahl, out minX, out maxX);
            getMinMax(wertY, anzahl, out minY, out maxY);
            LinefittingDialog RegAuswahlDlg = new LinefittingDialog(minX < 0, (minY < 0), regTyp);
            RegAuswahlDlg.ShowDialog();
            if ((bool)RegAuswahlDlg.DialogResult)
            {
                regTyp = (int)RegAuswahlDlg.GetAuswahl();
            }
        }
         */

    /*
        private void buttonCalcFkt_Click(object sender, RoutedEventArgs e)
        {
            CalculateLineFitFunction(regTyp);
        }
    */
  }
}