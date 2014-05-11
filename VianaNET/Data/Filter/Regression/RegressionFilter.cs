// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegressionFilter.cs" company="Freie Universität Berlin">
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

namespace VianaNET.Data.Filter.Regression
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Serialization;
  using Application;
  using CustomStyles.Types;
  using Collections;
  using Interpolation;

  using VianaNET.Resources;

  using WPFMath;

  /// <summary>
  /// The line fit class.
  /// </summary>
  public class RegressionFilter : FilterBase
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Static Fields

    /// <summary>
    ///   (angestrebte) Genauigkeit der Regressionsparameter
    /// </summary>
    private const double Genauigkeit = 1E-10;

    /// <summary>
    /// Begrenzungswerte für die internen Berechnungen
    /// </summary>
    private const int MaxIteration = 50;

    /// <summary>
    ///   The min step.
    /// </summary>
    private const double MinStep = 1E-6;

    /// <summary>
    ///   Startabweichung / maximaler absoluter Fehler
    /// </summary>
    private const double StartAbw = 1E150;

    /// <summary>
    /// Parameter und Abweichungen der 9 Ausgleichsfunktionen; 
    /// ein weiterer Parametersatz, wenn man die beste aller Regressionen herausfinden will. 
    /// </summary>
    private static readonly Matrix FitParameterMatrix = new Matrix(10, 6);

    /// <summary>
    /// Parameter (maximal 4) einer Ausgleichsfunktionen
    /// </summary>
    private static double[] param;

    #endregion

    #region Fields

    /// <summary>
    /// Provides a formula parser which reads tex formulas
    /// </summary>
    private readonly TexFormulaParser formulaParser;

    /// <summary>
    /// Art der Regressionsberechnung
    /// </summary>
    private RegressionType regressionType;

    private char achsName = 'x';
    private char funcName = 'y';

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    /////////////////////////////////////////////////////////////////////////////// 
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RegressionFilter"/> class.
    /// </summary>
    public RegressionFilter()
    {
      this.regressionType = RegressionType.Linear;
      param = new double[3];
      TexFormulaParser.Initialize();
      this.formulaParser = new TexFormulaParser();
    }

    #endregion

    #region Delegates

    /// <summary>
    ///   The ausgleich function.
    /// </summary>
    /// <param name="x"> argument </param>
    /// <returns>A double</returns>
    public delegate double AusgleichFunction(double x);

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    /// Gets or sets the ausgleichs funktion.
    /// </summary>
    [XmlIgnore]
    public AusgleichFunction AusgleichsFunktion { get; set; }

    /// <summary>
    /// Gets or sets the aberration for the regression
    /// </summary>
    public double Aberration { get; set; }

    /// <summary>
    /// Gets or sets the regression type
    /// </summary>
    public RegressionType RegressionType
    {
      get
      {
        return this.regressionType;
      }

      set
      {
        this.regressionType = value;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Updates axis and function naming
    /// </summary>
    /// <param name="achs">The axis character.</param>
    /// <param name="func"> The function character.</param>
    public void SetBezeichnungen(char achs, char func)
    {
      this.achsName = achs;
      this.funcName = func;
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is linear.
    /// </summary>
    /// <param name="x"> argument is x  </param>
    /// <returns> The calculated value <see cref="double"/> .  </returns>
    public static double AusgleichsGerade(double x)
    {
      return FitParameterMatrix[(int)RegressionType.Linear, 0] * x + FitParameterMatrix[(int)RegressionType.Linear, 1];
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is exponential.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsExpSpez(double x)
    {
      return FitParameterMatrix[(int)RegressionType.ExponentiellMitKonstante, 0] * Math.Exp(x * FitParameterMatrix[(int)RegressionType.ExponentiellMitKonstante, 1]);
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is logarithic.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsLog(double x)
    {
      return FitParameterMatrix[(int)RegressionType.Logarithmisch, 0] * Math.Log(x * FitParameterMatrix[(int)RegressionType.Logarithmisch, 1]);
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is potential.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsPot(double x)
    {
      return FitParameterMatrix[(int)RegressionType.Potenz, 0] * Math.Pow(x, FitParameterMatrix[(int)RegressionType.Potenz, 1]);
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is quadratic.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsParabel(double x)
    {
      return (FitParameterMatrix[(int)RegressionType.Quadratisch, 0] * x + FitParameterMatrix[(int)RegressionType.Quadratisch, 1]) * x + FitParameterMatrix[(int)RegressionType.Quadratisch, 2];
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is exponential.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsExp(double x)
    {
      return FitParameterMatrix[(int)RegressionType.Exponentiell, 0] * Math.Exp(x * FitParameterMatrix[(int)RegressionType.Exponentiell, 1]) + FitParameterMatrix[(int)RegressionType.Exponentiell, 2];
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is sinus-function.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsSin(double x)
    {
      return FitParameterMatrix[(int)RegressionType.Sinus, 0] * Math.Sin(x * FitParameterMatrix[(int)RegressionType.Sinus, 1] + FitParameterMatrix[(int)RegressionType.Sinus, 2])
             + FitParameterMatrix[(int)RegressionType.Sinus, 3];
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is a sinus-function and its amplitude is going down.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsSinExp(double x)
    {
      return FitParameterMatrix[(int)RegressionType.SinusGedämpft, 0] * Math.Sin(x * FitParameterMatrix[(int)RegressionType.SinusGedämpft, 1])
             * Math.Exp(x * FitParameterMatrix[(int)RegressionType.SinusGedämpft, 2]);
    }

    /// <summary>
    /// Calculates the value of the Regression function at argument x, when the function is describing a resonanz.
    /// </summary>
    /// <param name="x">
    /// argument is x 
    /// </param>
    /// <returns>
    /// The calculated value <see cref="double"/> . 
    /// </returns>
    public static double AusgleichsReso(double x)
    {
      return FitParameterMatrix[(int)RegressionType.Resonanz, 0]
             / Math.Pow(1 + FitParameterMatrix[(int)RegressionType.Resonanz, 1] * Math.Pow(x - FitParameterMatrix[(int)RegressionType.Resonanz, 2] / x, 2), 0.5);
    }

    /// <summary>
    /// The null function.
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
    /// Calculates the regression samples.
    /// </summary>
    public override void CalculateFilterValues()
    {
      base.CalculateFilterValues();

      if (this.anzahl > 2)
      {
        this.CalcRegressionFunction(this.regressionType);
      }
    }

    /// <summary>
    /// Updates the function string and the abberation.
    /// </summary>
    /// <param name="neuBerechnen"></param>
    public void UpdateLinefitFunctionData(bool neuBerechnen)
    {
      // Darstellung des Funktionsterms übernehmen
      this.UpdateRegressionFunctionString();
      this.Aberration = FitParameterMatrix[(int)this.regressionType, 5];

      // Serie neu berechnen: Punkte mit Ausgleichsfunktion bestimmen
      if ((this.AusgleichsFunktion != NullFkt) && neuBerechnen)
      {
        this.CalculateRegressionSamples();
      }
    }

    /// <summary>
    /// The calc regression function.
    /// </summary>
    /// <param name="typeOfRegression">
    /// The type of regression.
    /// </param>
    private void CalcRegressionFunction(RegressionType typeOfRegression)
    {
      switch (typeOfRegression)
      {
        case RegressionType.Linear:
          this.BestimmeLinFkt();
          break;
        case RegressionType.ExponentiellMitKonstante:
          this.BestimmeExpSpezFkt();
          break;
        case RegressionType.Logarithmisch:
          this.BestimmeLogFkt();
          break;
        case RegressionType.Potenz:
          this.BestimmePotFkt();
          break;
        case RegressionType.Quadratisch:
          this.BestimmeQuadratFkt();
          break;
        case RegressionType.Exponentiell:
          this.BestimmeExpFkt();
          break;
        case RegressionType.Sinus:
          this.BestimmeSinFkt();
          break;
        case RegressionType.SinusGedämpft:
          this.BestimmeSinExpFkt();
          break;
        case RegressionType.Resonanz:
          this.BestimmeResonanzFkt();
          break;
        default:
          this.BestimmeLinFkt();
          break;
      }

      // Funktionsterm und mittlere Abweichung bestimmen
      this.GetRegressionFunctionAndAverageAberration(typeOfRegression, FitParameterMatrix[(int)typeOfRegression, 5]);
    }

    /// <summary>
    /// Updates the regressionFunctionString
    /// </summary>
    public void UpdateRegressionFunctionString()
    {
      Viana.Project.CurrentFilterData.RegressionFunctionTexFormula = this.GetRegressionFunctionTexFormula(this.regressionType);
    }

    /// <summary>
    /// The _Get _BestRegress
    /// </summary>
    /// <param name="bestRegTyp">
    /// type of best fit. 
    /// </param> 
    public void GetBestRegressData(out RegressionType bestRegTyp, int negFlag)
    {
      RegressionType aktRegTyp;
      double abw = StartAbw;
      FitParameterMatrix[0, 5] = -2;
      bestRegTyp = RegressionType.Linear;
      for (aktRegTyp = RegressionType.Linear; aktRegTyp <= RegressionType.Resonanz; aktRegTyp++)
      {
        bool weiter=true;
        if ( ((aktRegTyp==RegressionType.Potenz) & (negFlag>0)) |
           ((aktRegTyp==RegressionType.Logarithmisch) & (negFlag % 2 == 1) ) |
           ( (aktRegTyp==RegressionType.Exponentiell) & (negFlag>1) )){weiter=false;}
        if (weiter)
        {
          this.CalcRegressionFunction(aktRegTyp);
          FitParameterMatrix[(int)aktRegTyp, 5] = this.Aberration;
          if (this.Aberration > -2)
          {
            if (abw > this.Aberration)
            {
              abw = this.Aberration;
              bestRegTyp = aktRegTyp;
            }
          }
        }
      }

      this.GetRegressionFunctionAndAverageAberration(bestRegTyp, FitParameterMatrix[(int)bestRegTyp, 5]);
    }





    /// <summary>
    /// The get average aberration.
    /// </summary>
    /// <param name="tempAberration">
    /// The tempAberration: -2 no calculation; -1 do new calculation; >0  calulation was done already
    /// </param>
    private void GetAverageAberration(double tempAberration)
    {
      if (this.AusgleichsFunktion != NullFkt)
      {
        if (tempAberration < -1.5)
        {
          tempAberration = -2;
        }
        else
        {
          if (tempAberration < 0)
          {
            tempAberration = 0;
            for (var k = 0; k < this.anzahl; k++)
            {
              double yi = this.AusgleichsFunktion(this.WertX[k]) - this.WertY[k];
              tempAberration = tempAberration + yi * yi;
            }

            tempAberration = tempAberration / this.anzahl;
          }
        }
      }
      else
      {
        tempAberration = -2;
      }

      this.Aberration = tempAberration;
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
    private TexFormula GetRegressionFunctionTexFormula(RegressionType regTyp)
    {
      var a = FitParameterMatrix[(int)regTyp, 0];
      var b = FitParameterMatrix[(int)regTyp, 1];
      var c = FitParameterMatrix[(int)regTyp, 2];
      var d = FitParameterMatrix[(int)regTyp, 3];
      var aString = Viana.Project.CurrentFilterData.GetFormattedString(a);
      var bString = Viana.Project.CurrentFilterData.GetFormattedString(b);
      var cString = Viana.Project.CurrentFilterData.GetFormattedString(c);
      var dString = Viana.Project.CurrentFilterData.GetFormattedString(d);
      string fktStr;

      if (this.AusgleichsFunktion != null)
      {
        switch (regTyp)
        {
          case RegressionType.Linear:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}x+", bString);
            break;
          case RegressionType.ExponentiellMitKonstante:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}e^{", bString, "{\\cdot}x}");
            break;
          case RegressionType.Logarithmisch:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}ln(", bString, "{\\cdot}x)");
            break;
          case RegressionType.Potenz:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}x^{", bString + "}");
            break;
          case RegressionType.Quadratisch:
            fktStr = string.Concat("f(x)=", aString, "x^2 + ", bString, "x + ", cString);
            break;
          case RegressionType.Exponentiell:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}e^{", bString, "{\\cdot}x} + ", cString);
            break;
          case RegressionType.Sinus:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}sin(", bString, "{\\cdot}x + ", cString, ") + ", dString);
            break;
          case RegressionType.SinusGedämpft:
            fktStr = string.Concat("f(x)=", aString, "{\\cdot}sin(", bString, "{\\cdot}x){\\cdot}e^{", cString, "{\\cdot}x}");
            break;
          case RegressionType.Resonanz:
            fktStr = string.Concat("f(x)={\\frac{" + aString, "}{\\sqrt{1 +", bString, "{\\cdot}(x - {\\frac{", cString, "}{x}})^2}}}");
            break;
          default:
            fktStr = string.Empty;
            this.AusgleichsFunktion = NullFkt;
            break;
        }
      }
      else
      {
        fktStr = string.Empty;
      }

      TexFormula returnFormula = null;
      if (fktStr != string.Empty)
      {
        fktStr = fktStr.Replace('x', achsName);
        fktStr = fktStr.Replace('f', funcName);
        try
        {
          returnFormula = this.formulaParser.Parse(fktStr);
        }
        catch (Exception)
        {
        }
      }

      return returnFormula;
    }


    /// <summary>
    /// The method to calculate the regression aberation string.
    /// </summary>
    /// <param name="regTyp">The regression type. 
    /// </param>
    /// <returns>The <see cref="string"/> with the aberrationstring</returns>
    private string GetRegressionAberrationString(RegressionType regTyp)
    {
      var fktStr = " -/-";
      if (this.AusgleichsFunktion != null)
      {
        var a = FitParameterMatrix[(int)regTyp, 5];
        if (a >= 0)
        {
          // Fehlerquadrate mit Nachkommastellen wie die Funktionsgleichung
          // sind glaube ich übertrieben oder?
          fktStr = Labels.AberrationStringPrefix + a.ToString("N0"); // FittedData.Instance.GetFormattedString(a);
        }
      }

      return fktStr;
    }

    /// <summary>
    /// The get min max.
    /// </summary>
    /// <param name="werte">
    /// The werte. 
    /// </param>
    /// <param name="min">
    /// The min. 
    /// </param>
    /// <param name="max">
    /// The max. 
    /// </param>
    public void GetMinMax(List<double> werte, out double min, out double max)
    {
      if (werte.Count == 0)
      {          // Falls noch keine Werte vorliegen
        min = 0;
        max = 0;
        return;
      }

      min = werte.Min();
      max = werte.Max();
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods


    /// <summary>
    /// Calculate Regression parameters for those Regressions, that can be done by the least square method.
    /// </summary>
    /// <param name="anzahlRegress">
    /// The anzahl. 
    /// </param>
    /// <param name="wertX">
    /// List of all x-values. 
    /// </param>
    /// <param name="wertY">
    /// List of all y-values. 
    /// </param>
    /// <param name="paramRegress">
    /// The param. 
    /// </param>
    private void DoRegress(int anzahlRegress, List<double> wertX, List<double> wertY, double[] paramRegress)
    {
      if (wertX == null)
      {
        return;
      }

      double sumX = 0;
      double sumX2 = 0;
      double sumXY = 0;
      double sumY = 0;
      for (var k = 0; k < anzahlRegress; k++)
      {
        var x = wertX[k];
        var y = wertY[k];
        sumX = sumX + x;
        sumX2 = sumX2 + x * x;
        sumY = sumY + y;
        sumXY = sumXY + x * y;
      }

      double nenner = anzahlRegress * sumX2 - sumX * sumX;

      // y = param[1]*x+param[0]
      if (nenner != 0)
      {
        paramRegress[1] = (anzahlRegress * sumXY - sumX * sumY) / nenner; // Steigung
        paramRegress[0] = (sumY * sumX2 - sumX * sumXY) / nenner; // y-Achsenabschnitt
      }
    }


    /// <summary>
    /// The bestimme lin fkt.
    /// </summary>
    private void BestimmeLinFkt()
    {
      this.DoRegress(this.anzahl, this.WertX, this.WertY, param);
      FitParameterMatrix[(int)RegressionType.Linear, 0] = param[1];
      FitParameterMatrix[(int)RegressionType.Linear, 1] = param[0];
      FitParameterMatrix[(int)RegressionType.Linear, 5] = -1;
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
        tempWertY.Add(Math.Log(this.WertY[k]));
      }

      this.DoRegress(this.anzahl, this.WertX, tempWertY, param);
      param[0] = Math.Exp(param[0]);
      FitParameterMatrix[(int)RegressionType.ExponentiellMitKonstante, 0] = param[0];
      FitParameterMatrix[(int)RegressionType.ExponentiellMitKonstante, 1] = param[1];
      FitParameterMatrix[(int)RegressionType.ExponentiellMitKonstante, 5] = -1;  //Abweichung noch nicht berechnet
    }

    /// <summary>
    ///   The bestimme log fkt.
    /// </summary>
    private void BestimmeLogFkt()
    {
      int k;
      var tempWertX = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertX.Add(Math.Log(this.WertX[k]));
      }

      this.DoRegress(this.anzahl, tempWertX, this.WertY, param);

      double hilf = param[1];
      param[1] = Math.Exp(param[0] / hilf);
      param[0] = hilf;
      FitParameterMatrix[(int)RegressionType.Logarithmisch, 0] = param[0];
      FitParameterMatrix[(int)RegressionType.Logarithmisch, 1] = param[1];
      FitParameterMatrix[(int)RegressionType.Logarithmisch, 5] = -1;
    }

    /// <summary>
    ///   The bestimme pot fkt.
    /// </summary>
    private void BestimmePotFkt()
    {
      int k, start;
      var tempWertX = new List<double>();
      var tempWertY = new List<double>();

      if ((this.WertX[0] <= 0) || (this.WertY[0] <= 0))
      {
        start = 1;
      }
      else
      {
        start = 0;
      }

      for (k = start; k < this.anzahl; k++)
      {
        tempWertX.Add(Math.Log(this.WertX[k]));
        tempWertY.Add(Math.Log(this.WertY[k]));
      }

      this.DoRegress(this.anzahl - start, tempWertX, tempWertY, param);
      param[0] = Math.Exp(param[0]);
      FitParameterMatrix[(int)RegressionType.Potenz, 0] = param[0];
      FitParameterMatrix[(int)RegressionType.Potenz, 1] = param[1];
      FitParameterMatrix[(int)RegressionType.Potenz, 5] = -1;
    }

    /// <summary>
    ///   The bestimme quadrat fkt.
    /// </summary>
    private void BestimmeQuadratFkt()
    {
      int k;

      double sumX4 = 0;
      double sumX3 = 0;
      double sumX2 = 0;
      double sumX = 0;
      double sumXY = 0;
      double sumX2Y = 0;
      double sumY = 0;

      for (k = 0; k < this.anzahl; k++)
      {
        double xi = this.WertX[k];
        double yi = this.WertY[k];
        sumX = sumX + xi;
        sumY = sumY + yi;
        sumXY = sumXY + yi * xi;
        xi = xi * xi;
        sumX2 = sumX2 + xi;
        sumX2Y = sumX2Y + xi * yi;
        sumX3 = sumX3 + xi * this.WertX[k];
        sumX4 = sumX4 + xi * xi;
      }

      // LGS:
      // a*sumX4 + b*sumX3 +c*sumX2 = sumX2Y
      // a*sumX3 + b*sumX2 +c*sumX  = sumXY
      // a*sumX2 + b*sumX  +c*k     = sumY
      var m = new Matrix(3, 3);
      var v = new Matrix(3, 1);
      m[0, 0] = sumX4;
      m[0, 1] = sumX3;
      m[0, 2] = sumX2;
      v[0, 0] = sumX2Y;
      m[1, 0] = sumX3;
      m[1, 1] = sumX2;
      m[1, 2] = sumX;
      v[1, 0] = sumXY;
      m[2, 0] = sumX2;
      m[2, 1] = sumX;
      m[2, 2] = this.anzahl;
      v[2, 0] = sumY;
      var lsg = Matrix.SolveLinear(m, v);
      var a = lsg[0, 0];
      var b = lsg[1, 0];
      var c = lsg[2, 0];
      FitParameterMatrix[(int)RegressionType.Quadratisch, 0] = a;
      FitParameterMatrix[(int)RegressionType.Quadratisch, 1] = b;
      FitParameterMatrix[(int)RegressionType.Quadratisch, 2] = c;
      FitParameterMatrix[(int)RegressionType.Quadratisch, 5] = -1;
    }


    /// <summary>
    ///   The Abschaetzung fuer Parameter b.
    /// </summary>
    /// <returns> The <see cref="double" /> . </returns>
    private double AbschaetzungFuerB()
    {
      /* für f(x)=a*exp(b*x)+c bzw. f(x)= a*sin(b*x+c)+d gilt:
             * b=f''(x)/f'(x)  (bei der Sinusfunktion ergibt sich -b !!
             * mit der Annahme (f(x3)-f(x1))/(x3-x1) ungefähr gleich f'(x2)
             * ergibt sich nachfolgende Rechnung: )
            */
      return ((this.WertY[4] - this.WertY[2]) / (this.WertX[4] - this.WertX[2])
              - (this.WertY[2] - this.WertY[0]) / this.WertX[0] - this.WertX[0]) / (this.WertY[3] - this.WertY[1]);
    }

    /// <summary>
    ///   The bestimme exp fkt.
    /// </summary>
    private void BestimmeExpFkt()
    {
      double minY, maxY;
      int k;

      // Schätzwert für Verschiebung in y-Richtung; 
      this.GetMinMax(this.WertY, out minY, out maxY);
      double schaetzWert;
      double schaetzStep;
      int sign;

      double steigungAmAnfang = (this.WertY[0] - this.WertY[1]) / (this.WertX[0] - this.WertX[1]);
      double steigungAmEnde = (this.WertY[this.anzahl - 1] - this.WertY[this.anzahl - 2])
                              / (this.WertX[this.anzahl - 1] - this.WertX[this.anzahl - 2]);
      if (((steigungAmAnfang < 0) && (steigungAmAnfang > -0.2)) || ((steigungAmEnde > 0) && (steigungAmEnde < 0.2)))
      {
        // Asymptote oben
        if (maxY < 0)
        {
          schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-maxY)));
          schaetzWert = 0;
        }
        else
        {
          if (maxY > 0)
          {
            schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(maxY)));
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
          if (minY < 0)
          {
            schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-minY)));
            schaetzWert = -schaetzStep * 10;
          }
          else
          {
            if (minY > 0)
            {
              schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(minY)));
              schaetzWert = Math.Floor(minY * schaetzStep) / schaetzStep;
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
            if (minY > 0)
            {
              schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(minY)));
              schaetzWert = -schaetzStep;
            }
            else
            {
              if (minY < 0)
              {
                schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-minY)));
                schaetzWert = Math.Floor(minY * schaetzStep) / schaetzStep - schaetzStep;
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
            if (maxY > 0)
            {
              schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(maxY)));
              schaetzWert = 10 * schaetzStep;
            }
            else
            {
              if (maxY < 0)
              {
                schaetzStep = Math.Pow(10, Math.Floor(Math.Log10(-maxY)));
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

      var tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertY.Add(0);
      }

      // iterieren über c; 
      double bestA = 0;
      double bestB = 0;
      double bestC = 0;
      double abw = StartAbw;
      int iter = 1;
      bool weiter = true;
      double fehlergrenze = this.anzahl * Genauigkeit;
      int z = 0;
      do
      {
        for (k = 0; k < this.anzahl; k++)
        {
          tempWertY[k] = Math.Log(Math.Abs(this.WertY[k] - schaetzWert));
        }

        this.DoRegress(this.anzahl, this.WertX, tempWertY, param);
        param[0] = Math.Exp(param[0]);
        if (schaetzWert > maxY)
        {
          param[0] = -param[0];
        }

        double fehler = 0;
        for (k = 0; k < this.anzahl; k++)
        {
          double yi = param[0] * Math.Exp(param[1] * this.WertX[k]) + schaetzWert;
          fehler = fehler + (yi - this.WertY[k]) * (yi - this.WertY[k]);
        }

        if (abw > fehler)
        {
          abw = fehler;
          bestA = param[0];
          bestB = param[1];
          bestC = schaetzWert;
          if ((abw < fehlergrenze) || (schaetzStep < MinStep))
          {
            weiter = false;
          }
        }

        if (z < 9)
        {
          schaetzWert = schaetzWert + sign * schaetzStep;
          z = z + 1;
          if (((sign == 1) && (schaetzWert > minY)) || ((sign == -1) && (schaetzWert < maxY)))
          {
            z = 10;
          }
        }

        if (z >= 9)
        {
          if (schaetzStep > MinStep)
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
      while (weiter && (iter < MaxIteration));
      FitParameterMatrix[(int)RegressionType.Exponentiell, 0] = bestA;
      FitParameterMatrix[(int)RegressionType.Exponentiell, 1] = bestB;
      FitParameterMatrix[(int)RegressionType.Exponentiell, 2] = bestC;
      FitParameterMatrix[(int)RegressionType.Exponentiell, 5] = abw / this.anzahl;
    }


    /// <summary>
    /// The _get periode.
    /// </summary>
    /// <param name="dataX">
    /// List of all x-values. 
    /// </param>
    /// <param name="dataY">
    /// List of all y-values. 
    /// </param>
    /// <param name="schaetzWert">
    /// assumed value of factor determing the period 
    /// </param>
    /// <param name="maxSchaetz">
    /// max value of factor determing the period. 
    /// </param>
    /// <param name="schaetzStep">
    /// start step for changing that factor. 
    /// </param>
    private void GetPeriode(
      List<double> dataX, List<double> dataY, out double schaetzWert, out double maxSchaetz, out double schaetzStep)
    {
      double minX, maxX;

      // Schätzwert für b ermitteln:  y=a*sin(b*x)*exp(c*x);
      this.GetMinMax(dataX, out minX, out maxX);
      int sign = Math.Sign(dataY[0]);
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

      int anz = 1;
      for (var k = 0; k < this.anzahl - 1; k++)
      {
        if (sign != Math.Sign(dataY[k + 1]))
        {
          anz = anz + 1;
          sign = -sign;
        }
      }

      maxSchaetz = anz * Math.PI / (maxX - minX);
      //schaetzWert = (anz - 1) * Math.PI /maxX;   kann negativen Wert für hilf ergeben, wenn minX negativ: Endlosschleife!!
      schaetzWert = (anz - 1) * Math.PI / (maxX - minX);
      schaetzStep = 10000;
      var hilf = maxSchaetz - schaetzWert;
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

    /// <summary>
    ///   The bestimme sin fkt.
    /// </summary>
    private void BestimmeSinFkt()
    {
      int k;
      double minY;
      double maxY;
      double maxSchaetz, schaetzWert, schaetzStep;

      double bestA = 0;
      double bestB = 0;
      double bestC = 0;
      double bestD = 0;
      this.GetMinMax(this.WertY, out minY, out maxY);

      // Amplitude a:
      var a = (maxY - minY) * 1.02 / 2;

      // y-Verschiebung d:
      var d = (maxY + minY) / 2;

      // Periodenlänge:
      var tempWertY = new List<double>();

      for (k = 0; k < this.anzahl; k++)
      {
        tempWertY.Add(this.WertY[k] - d);
      }

      this.GetPeriode(this.WertX, tempWertY, out schaetzWert, out maxSchaetz, out schaetzStep);

      // a*sin(b*x+c)+d = a*cos(c)*sin(b*x) + a*sin(c)*cos(b*x)+d = a1*sin(b*x) + c1*cos(b*x) + d;
      // iteration über b:
      var weiter = true;
      int iter = 0;
      var abw = StartAbw;
      int z = 0;
      while (weiter && (iter < MaxIteration))
      {
        double sumSin = 0;
        double sumSin2 = 0;
        double sumSinY = 0;
        double sumSinCos = 0;
        double sumCos = 0;
        double sumCos2 = 0;
        double sumCosY = 0;
        double sumY = 0;
        double yi;
        int n;
        for (n = 0; n < this.anzahl; n++)
        {
          double xi = this.WertX[n];
          yi = this.WertY[n];
          double xci = Math.Cos(schaetzWert * xi);
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
        var m = new Matrix(3, 3);
        var v = new Matrix(3, 1);
        m[0, 0] = sumSin2;
        m[0, 1] = sumSinCos;
        m[0, 2] = sumSin;
        v[0, 0] = sumSinY;
        m[1, 0] = sumSinCos;
        m[1, 1] = sumCos2;
        m[1, 2] = sumCos;
        v[1, 0] = sumCosY;
        m[2, 0] = sumSin;
        m[2, 1] = sumCos;
        m[2, 2] = this.anzahl;
        v[2, 0] = sumY;
        var lsg = Matrix.SolveLinear(m, v);
        var a1 = lsg[0, 0];
        var c1 = lsg[1, 0];
        a = Math.Sqrt(a1 * a1 + c1 * c1);
        if (a1 < 0)
        {
          a = -a;
        }

        double b = schaetzWert;
        var c = Math.Asin(c1 / a);
        d = lsg[2, 0];

        double fehler = 0;
        for (n = 0; n < this.anzahl; n++)
        {
          yi = a * Math.Sin(b * this.WertX[n] + c) + d - this.WertY[n];
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
          if (schaetzStep > MinStep)
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

      FitParameterMatrix[(int)RegressionType.Sinus, 0] = bestA;
      FitParameterMatrix[(int)RegressionType.Sinus, 1] = bestB;
      FitParameterMatrix[(int)RegressionType.Sinus, 2] = bestC;
      FitParameterMatrix[(int)RegressionType.Sinus, 3] = bestD;
      FitParameterMatrix[(int)RegressionType.Sinus, 5] = abw / this.anzahl;
    }

    /// <summary>
    ///   The bestimme sin exp fkt.
    /// </summary>
    private void BestimmeSinExpFkt()
    {
      double maxSchaetz, schaetzWert, schaetzStep;
      int k;

      // Schätzwert für b ermitteln:  y=a*sin(b*x)*exp(c*x);
      this.GetPeriode(this.WertX, this.WertY, out schaetzWert, out maxSchaetz, out schaetzStep);

      var weiter = true;
      int z = 0;
      int iter = 0;
      double abw = StartAbw;
      double b = schaetzWert;
      double bestA = 0;
      double bestB = b;
      double bestC = 0;
      var tempWertX = new List<double>();
      var tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertX.Add(0);
        tempWertY.Add(0);
      }

      while (weiter && (iter < MaxIteration))
      {
        int anz = 0;
        double hilf;
        double xi;
        for (k = 0; k < this.anzahl; k++)
        {
          xi = this.WertX[k];
          double sinbxi = Math.Sin(schaetzWert * xi);
          if (sinbxi != 0)
          {
            hilf = this.WertY[k] / sinbxi;
            if (hilf > 0)
            {
              tempWertX[anz] = xi;
              tempWertY[anz] = Math.Log(hilf);
              anz = anz + 1;
            }
          }
        }

        // for k
        iter = iter + 1;
        if (anz >= 0.8 * this.anzahl)
        {
          // mehr als 80% der Wertepaare können bei dieser Schätzung für b berücksichtigt werden
          this.DoRegress(anz, tempWertX, tempWertY, param);
          double a = Math.Exp(param[0]);
          double c = param[1];
          double fehler = 0;
          for (k = 0; k < this.anzahl; k++)
          {
            xi = this.WertX[k];
            hilf = a * Math.Sin(schaetzWert * xi) * Math.Exp(c * xi) - this.WertY[k];
            fehler = fehler + hilf * hilf;
          }

          //iter = iter + 1;  wird evtl. nicht hochgezählt, wenn anz < 80% von Anzahl der Messpaare
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
          if (schaetzStep > MinStep)
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
      FitParameterMatrix[(int)RegressionType.SinusGedämpft, 0] = bestA;
      FitParameterMatrix[(int)RegressionType.SinusGedämpft, 1] = bestB;
      FitParameterMatrix[(int)RegressionType.SinusGedämpft, 2] = bestC;
      FitParameterMatrix[(int)RegressionType.SinusGedämpft, 5] = abw / this.anzahl;
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
      double yi;
      int k;
      var tempWertX = new List<double>();
      var tempWertY = new List<double>();
      for (k = 0; k < this.anzahl; k++)
      {
        tempWertX.Add(0);
        tempWertY.Add(0);
      }

      double bestA = 0;
      double bestB = 0;
      double bestC = 0;

      // Parameter abschätzen
      int maxPos = -1;
      double maxY = -1E150;
      for (k = 0; k < this.anzahl; k++)
      {
        yi = this.WertY[k];
        if (yi > maxY)
        {
          maxY = yi;
          maxPos = k;
        }
      }

      var offset = 10000000000.0;
      while (offset > maxY / 100)
      {
        offset = offset * 0.1;
      }

      // Quadrat des x-Wertes der ResonanzStelle ist ungefähr der Schaetzwert
      double schaetzWert = maxPos > 0 ? this.WertX[maxPos - 1] : this.WertX[maxPos];
      double maxSchaetz = maxPos < this.anzahl - 11 ? this.WertX[maxPos + 1] : this.WertX[maxPos];

      maxSchaetz = maxSchaetz * maxSchaetz;
      schaetzWert = schaetzWert * schaetzWert;
      if (maxSchaetz < schaetzWert)
      {
        double hilf = maxSchaetz;
        maxSchaetz = schaetzWert;
        schaetzWert = hilf;
      }
      double grenze = 1;
      while (grenze > (maxSchaetz - schaetzWert) / 1000)
      {
        grenze = grenze * 0.1;
      }

      double schaetzStep = grenze * 1000;
      schaetzWert = Math.Floor(schaetzWert);

      if (maxPos == 0)
      {
        schaetzWert = schaetzWert - 2 * schaetzStep;
      }

      var z = 9 - (int)Math.Floor((maxSchaetz - schaetzWert) / schaetzStep);

      var abw = StartAbw;
      int iter = 0;
      while (iter < MaxIteration)
      {
        // Iteration über c
        // lin. Reg vorbereiten    1/y^2 = 1/a^2 + b/a^2 * (x - c/x)² = a' + b' * hilfX
        int anz = 0;
        double xi;
        double hilfX;
        for (k = 0; k < this.anzahl; k++)
        {
          xi = this.WertX[k];
          yi = this.WertY[k];
          if ((yi != 0) && (xi != 0))
          {
            hilfX = xi - schaetzWert / xi;
            tempWertX[anz] = hilfX * hilfX;
            tempWertY[anz] = 1 / (yi * yi);
            anz = anz + 1;
          }
        }

        this.DoRegress(anz, tempWertX, tempWertY, param);
        var a = param[0];
        var b = param[1] / a;
        a = Math.Pow(a, -0.5);
        var c = schaetzWert;
        double fehler = 0;
        for (k = 0; k < this.anzahl; k++)
        {
          xi = this.WertX[k];
          hilfX = xi - c / xi;
          var hilf = a / Math.Pow(1 + b * hilfX * hilfX, 0.5) - this.WertY[k];
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

      FitParameterMatrix[(int)RegressionType.Resonanz, 0] = bestA;
      FitParameterMatrix[(int)RegressionType.Resonanz, 1] = bestB;
      FitParameterMatrix[(int)RegressionType.Resonanz, 2] = bestC;
      FitParameterMatrix[(int)RegressionType.Resonanz, 5] = abw / this.anzahl;
    }

    /// <summary>
    /// Calculates the regression samples.
    /// </summary>
    private void CalculateRegressionSamples()
    {
      var regressionSamples = new SortedObservableCollection<XYSample>();

      int k;
      double x;
      XYSample p;

      if (this.AusgleichsFunktion == null)
      {
        return;
      }

      if (this.regressionType == RegressionType.Linear)
      {
        // Sonderfall lineare Regression; Anzahl der Berechnungen wird drastisch reduziert,                                  
        // da Chart selbst Geraden zeichnen kann. 
        if (Viana.Project.CurrentFilterData.AxisX.Axis != AxisType.T)
        {
          // zwei Punkte genügen bei x-y-Diagramm (kleinster und größter)
          x = this.WertXMin;
          p = new XYSample(x, this.AusgleichsFunktion(x));
          regressionSamples.Add(p);
          x = this.WertXMax;
          p = new XYSample(x, this.AusgleichsFunktion(x));
          regressionSamples.Add(p);
        }
        else
        {
          // Workaround beim t-?-Diagramm: gleichviele Punkte wie bei Originalwerten und gleiche x Werte. 
          for (k = 0; k < this.anzahl; k++)
          {
            x = this.WertX[k];
            p = new XYSample(x, this.AusgleichsFunktion(x));
            regressionSamples.Add(p);
          }
        }
      }
      else
      {
        // endPixelX und startPixelX sowie startX,endX und stepX 
        // wurden in CopySampleColumnsToArrays(int aktObjectNr, DataCollection originalSamples) bestimmt
        var anzahlPixel = (int)(this.endPixelX - this.startPixelX);
        x = this.startX;
        for (k = 0; k <= anzahlPixel; k++)
        {
          // Punkte im PixelAbstand (waagerecht) werden mit der Ausgleichsfunktion bestimmt.
          // führt bei t-?-Diagrammen zu falschen Darstellungen !!   
          p = new XYSample(x, this.AusgleichsFunktion(x));
          regressionSamples.Add(p);
          x = x + this.stepX;
        }
      }

      Viana.Project.CurrentFilterData.RegressionSeries = regressionSamples;
    }

    /// <summary>
    /// The get regression function string and average error.
    /// </summary>
    /// <param name="regTyp">
    /// The reg typ. 
    /// </param>
    /// <param name="tempAberration">
    /// The aberration.
    /// </param>
    private void GetRegressionFunctionAndAverageAberration(RegressionType regTyp, double tempAberration)
    {
      // Ausgleichsfunktion zuweisen
      switch (regTyp)
      {
        case RegressionType.Linear:
          this.AusgleichsFunktion = AusgleichsGerade;
          break;
        case RegressionType.ExponentiellMitKonstante:
          this.AusgleichsFunktion = AusgleichsExpSpez;
          break;
        case RegressionType.Logarithmisch:
          this.AusgleichsFunktion = AusgleichsLog;
          break;
        case RegressionType.Potenz:
          this.AusgleichsFunktion = AusgleichsPot;
          break;
        case RegressionType.Quadratisch:
          this.AusgleichsFunktion = AusgleichsParabel;
          break;
        case RegressionType.Exponentiell:
          this.AusgleichsFunktion = AusgleichsExp;
          break;
        case RegressionType.Sinus:
          this.AusgleichsFunktion = AusgleichsSin;
          break;
        case RegressionType.SinusGedämpft:
          this.AusgleichsFunktion = AusgleichsSinExp;
          break;
        case RegressionType.Resonanz:
          this.AusgleichsFunktion = AusgleichsReso;
          break;
        default:
          this.AusgleichsFunktion = NullFkt;
          break;
      }

      // mittleres Abweichungsquadrat errechnen
      this.GetAverageAberration(tempAberration);
      Viana.Project.CurrentFilterData.RegressionAberration = this.Aberration;
      FitParameterMatrix[(int)regTyp, 5] = this.Aberration;
    }

    #endregion
  }
}