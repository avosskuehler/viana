// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Freie Universität Berlin">
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
//   The constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Filter.Theory
{
  using System;

  /// <summary>
  ///   The constants.
  /// </summary>
  public class Constants
  {

    /// <summary>
    ///   The konst record.
    /// </summary>
    public struct KonstRec
      {
      

          /// <summary>
          ///   long name of constant.
          /// </summary>
          public string titel;

          /// <summary>
          ///   short name of constant.
          /// </summary>
          public string bez;

          /// <summary>
          ///   value of constant.
          /// </summary>
          public double wert;

      
      }
   


    /// <summary>
    ///   The euler number.
    /// </summary>
    public const double eulerZahl = 2.1782818;

    /// <summary>
    ///   The fehler zahl.
    /// </summary>
    public const double fehlerZahl = -1.0E32;

    /// <summary>
    ///   The leer feld.
    /// </summary>
    public const double leerFeld = -1.1E32;

    // public const double _max_Real = 1.7e38;
    // public const int _Ln_max_Real = 88;




    /// <summary>
    ///   spezial chars in shortnames of physical constants. 
    ///   # next char symbolic font; _ underscript
    /// </summary>
    public static readonly string spezialChars =  "#_0123456789";

    /// <summary>
    ///   The konstante.
    /// </summary>
    public static readonly KonstRec[] konstante = new[]
      {
        new KonstRec { titel = "PhysikKonstant_e", bez = "§e", wert = 1.6021E-19 }, 
        new KonstRec { titel = "PhysikKonstant_epsilon", bez = "§#e_0", wert = 8.85419E-12 }, 
        new KonstRec { titel = "PhysikKonstant_c", bez = "§c", wert = 2.99792458E8 }, 
        new KonstRec { titel = "PhysikKonstant_mu", bez = "§#m_0", wert = 1.256637E-6 }, 
        new KonstRec { titel = "PhysikKonstant_g", bez = "§g", wert = 9.80665 }, 
        new KonstRec { titel = "PhysikKonstant_h", bez = "§h", wert = 6.6256E-34 }, 
        new KonstRec { titel = "PhysikKonstant_me", bez = "§m_e", wert = 9.1093897E-31 }, 
        new KonstRec { titel = "PhysikKonstant_f", bez = "§#g", wert = 6.67259E-11 }, 
        new KonstRec { titel = "PhysikKonstant_Lambda0", bez = "§#l_c", wert = 2.43E-12 }
      };

    /// <summary>
    ///   operator- or function names.
    /// </summary>
    public static string[] fnam =
      {
        "(", ")", "+", "-", "*", "/", "^", "SIN", "COS", "TAN", "COTAN", "EXP", "LN", 
        "WURZEL", "SIGN", "ABS", "DELTA", "?"
      };

    public static string varName = "x";



  }



  /// <summary>
  ///   Calcalator or function editor.
  /// </summary>
  public enum TRechnerArt
  {
    /// <summary>
    ///   function editor.
    /// </summary>
    formelRechner,

    /// <summary>
    ///   calculator.
    /// </summary>
    rechner
  }
  // end TRechnerArt

  /// <summary>
  ///   symbol types.
  /// </summary>
  public enum symTyp
  {
    /// <summary>
    ///   The udef.
    /// </summary>
    udef,

    /// <summary>
    ///   The oldfx.
    /// </summary>
    oldfx,

    /// <summary>
    ///   The ist zahl.
    /// </summary>
    istZahl,

    /// <summary>
    ///   The ist konst.
    /// </summary>
    istKonst,

    /// <summary>
    ///   The ident.
    /// </summary>
    ident,

    /// <summary>
    ///   The lklammer.
    /// </summary>
    lklammer,

    /// <summary>
    ///   The rklammer.
    /// </summary>
    rklammer,

    /// <summary>
    ///   The plus.
    /// </summary>
    plus,

    /// <summary>
    ///   The minus.
    /// </summary>
    minus,

    /// <summary>
    ///   The mal.
    /// </summary>
    mal,

    /// <summary>
    ///   The durch.
    /// </summary>
    durch,

    /// <summary>
    ///   The pot.
    /// </summary>
    pot,

    /// <summary>
    ///   The sinus function.
    /// </summary>
    sinf,

    /// <summary>
    ///   The cosinus function.
    /// </summary>
    cosf,

    /// <summary>
    ///   The tangens function.
    /// </summary>
    tanf,

    /// <summary>
    ///   The ctgf.
    /// </summary>
    ctgf,

    /// <summary>
    ///   The exponential function.
    /// </summary>
    expf,

    /// <summary>
    ///   The logarithmic function.
    /// </summary>
    lnf,

    /// <summary>
    ///   The squareroot function.
    /// </summary>
    wurzf,

    /// <summary>
    ///   The signum function.
    /// </summary>
    sigf,

    /// <summary>
    ///   The absolute function.
    /// </summary>
    absf,

    /// <summary>
    ///   The delta function.
    /// </summary>
    deltaf,

    /// <summary>
    ///   The last function symbol - dummy function.
    /// </summary>
    ffmax,

    /// <summary>
    ///   The function string end.
    /// </summary>
    fstrEnd
  }
  // end symTyp

  /// <summary>
  ///   Parsing a string and building a function tree; calculate value of function for a given argument.
  /// </summary>
  public class Parse
  {


    /// <summary>
    ///   The function term/tree.
    /// </summary>
      public FunctionCalcTree CalculatorFunctionTerm = new FunctionCalcTree();

    /// <summary>
    ///   The fx.
    /// </summary>
      public FunctionCalcTree fx;

    /// <summary>
    ///   The last error number.
    /// </summary>
    public byte lastErrNr = 0;

    /// <summary>
    ///   The last error position.
    /// </summary>
    public int lastErrPos = 0;

    /// <summary>
    ///   The calculation error.
    /// </summary>
    private bool calcErr;

    /// <summary>
    ///   char read in function string.
    /// </summary>
    private char ch = ' ';

    /// <summary>
    ///   position of char in function string.
    /// </summary>
    private short chPos = -1;

    /// <summary>
    ///   The ch_ - same as ch, but is uppercase.
    /// </summary>
    private char ch_;

    /// <summary>
    ///   The erg1.
    /// </summary>
    private double erg1;

    /// <summary>
    ///   The error number.
    /// </summary>
    private int errNr;

    /// <summary>
    ///   The error position.
    /// </summary>
    private int errPos;

    /// <summary>
    ///   The functionname string.
    /// </summary>
    private string fFormel = string.Empty;

    /// <summary>
    ///   The fehler.
    /// </summary>
    private bool fehler;

    /// <summary>
    ///   The fkt wert error.
    /// </summary>
    private bool fktWertError;

    /// <summary>
    ///   The func str.
    /// </summary>
    private string funcStr;

    /// <summary>
    ///   The help string.
    /// </summary>
    private string hStr;

    /// <summary>
    ///   node containing a constant.
    /// </summary>
    private FunctionCalcTree kVar;

    /// <summary>
    ///   The maximum length.
    /// </summary>
    private byte maxLang;

    /// <summary>
    ///   flag for comparing strings case sensitive.
    /// </summary>
    private bool noUpCase = false;

    /// <summary>
    ///   The old char position.
    /// </summary>
    private short oldChPos = -1;

    /// <summary>
    ///   The result.
    /// </summary>
    private double result;

    /// <summary>
    ///   symbol
    /// </summary>
    private symTyp sym;

    /// <summary>
    ///   value of variable.
    /// </summary>
    private double varXwert;

    /// <summary>
    ///   value
    /// </summary>
    private double wert;

    /// <summary>
    ///   reference to node containing the varible.
    /// </summary>
    private FunctionCalcTree xVar;





    /// <summary>
    /// Error message.
    /// </summary>
    /// <param name="nr">
    /// error number. 
    /// </param>
    /// <param name="s">
    /// message string 
    /// </param>
    public void ErrMsg(byte nr, ref string s)
    {
      switch (nr)
      {
        case 1: 
          s = VianaNET.Localization.Labels.ParseErrorBracketOpen;
          break;
        case 2:
          s = VianaNET.Localization.Labels.ParseErrorBracketClose;
          break;
        case 3:
          s = VianaNET.Localization.Labels.ParseErrorOperatorMissing;
          break;
        case 4:
          s = VianaNET.Localization.Labels.ParseErrorTermMissing;
          break;
        case 5:
          s = VianaNET.Localization.Labels.ParseErrorIllegalCharacter;
          break;
        case 6:
          s = VianaNET.Localization.Labels.ParseErrorNoFormulaDefined;
          break;
        case 7:
          s = VianaNET.Localization.Labels.ParseErrorUnknownIdentifier;
          break;
      }
    }

    /// <summary>
    /// The fkt wert.
    /// </summary>
    /// <param name="fx">
    /// function. 
    /// </param>
    /// <param name="nr">
    /// number of variable - in this version unused
    /// </param>
    /// <returns>
    /// value <see cref="double"/> . 
    /// </returns>
    public double FktWert(FunctionCalcTree fx, int nr)
    {
      double result;

      if (fx != null)
      {
        this.calcErr = false;
        this.fktWertError = false;
        result = this.FktWert_Berechne(fx, nr);
        if (this.fktWertError)
        {
          if (this.calcErr)
          {
            result = Constants.fehlerZahl;
          }
          else
          {
            result = Constants.leerFeld;
          }
        }
      }
      else
      {
        result = Constants.fehlerZahl;
        this.calcErr = true;
      }

      return result;
    }

    /// <summary>
    /// Method of FktWert: calculate value.
    /// </summary>
    /// <param name="fx">
    /// function 
    /// </param>
    /// <param name="nr">
    /// The nr. 
    /// </param>
    /// <returns>
    /// value <see cref="double"/> . 
    /// </returns>
    public double FktWert_Berechne(FunctionCalcTree fx, int nr)
    {
      double erg2;
      if (this.fktWertError)
      {
        this.result = Constants.fehlerZahl;
        return this.result;
      }

      switch (fx.Cwert)
      {
        case symTyp.istZahl:
          this.result = fx.Zwert;
          break;
        case symTyp.istKonst:
          this.result = fx.Zwert; 
          break;
        case symTyp.ident:
          this.result = this.varXwert;
          break;
        default:
          erg2 = this.FktWert_Berechne(fx.Re, nr);
          if (this.fktWertError)
          {
            this.result = erg2;
            return this.result;
          }

          if (fx.Cwert <= symTyp.pot)
          {
            this.erg1 = this.FktWert_Berechne(fx.Li, nr);
            if (this.fktWertError)
            {
              this.result = this.erg1;
              return this.result;
            }

            switch (fx.Cwert)
            {
              case symTyp.plus:
                this.erg1 = this.erg1 + erg2;
                break;
              case symTyp.minus:
                this.erg1 = this.erg1 - erg2;
                break;
              case symTyp.mal:
                this.erg1 = this.erg1 * erg2;
                break;
              case symTyp.durch:
                if (erg2 != 0)
                {
                  this.erg1 = this.erg1 / erg2;
                }
                else
                {
                  this.FktWert_Berechne_Fehler();
                }

                break;
              case symTyp.pot:
                if (Convert.ToInt16(erg2) == erg2)
                {
                  this.erg1 = this.FktWert_WertIntPot(this.erg1, Convert.ToInt16(erg2));
                }
                else if (this.erg1 > 0)
                {
                  this.erg1 = Math.Exp(erg2 * Math.Log(this.erg1));
                }
                else if (this.erg1 < 0)
                {
                  this.FktWert_Berechne_Fehler();
                }

                break;
              default:
                this.FktWert_Berechne_Fehler();
                break;
            }
          }
          else
          {
            switch (fx.Cwert)
            {
              case symTyp.sinf:
                this.erg1 = Math.Sin(erg2);
                break;
              case symTyp.cosf:
                this.erg1 = Math.Cos(erg2);
                break;
              case symTyp.tanf:
                this.erg1 = Math.Cos(erg2);
                if (this.erg1 != 0)
                {
                  this.erg1 = Math.Sin(erg2) / this.erg1;
                }
                else
                {
                  this.FktWert_Berechne_Fehler();
                }

                break;
              case symTyp.ctgf:
                this.erg1 = Math.Sin(erg2);
                if (this.erg1 != 0)
                {
                  this.erg1 = Math.Cos(erg2) / this.erg1;
                }
                else
                {
                  this.FktWert_Berechne_Fehler();
                }

                break;
              case symTyp.expf:
                this.erg1 = Math.Exp(erg2);
                break;
              case symTyp.lnf:
                if (erg2 > 0)
                {
                  this.erg1 = Math.Log(erg2);
                }
                else
                {
                  this.FktWert_Berechne_Fehler();
                }

                break;
              case symTyp.wurzf:
                if (erg2 >= 0)
                {
                  this.erg1 = Math.Sqrt(erg2);
                }
                else
                {
                  this.FktWert_Berechne_Fehler();
                }

                break;
              case symTyp.sigf:
                if (erg2 > 0)
                {
                  this.erg1 = 1;
                }
                else if (erg2 < 0)
                {
                  this.erg1 = -1;
                }
                else
                {
                  this.erg1 = 0;
                }

                break;
              case symTyp.absf:
                this.erg1 = Math.Abs(erg2);
                break;
              case symTyp.deltaf:
                if (nr == 0)
                {
                  this.erg1 = Constants.leerFeld;
                }
                else
                {
                  this.erg1 = this.FktWert_Berechne(fx.Re, nr - 1);
                  if (this.fktWertError)
                  {
                    this.result = this.erg1;
                    return this.result;
                  }

                  this.erg1 = erg2 - this.erg1;
                }

                break;
              default:
                this.FktWert_Berechne_Fehler();
                break;
            }
          }

          this.result = this.erg1;
          break;
      }

      return this.result;
    }

    /// <summary>
    ///   Method of FktWert: set error values.
    /// </summary>
    public void FktWert_Berechne_Fehler()
    {
      this.fktWertError = true;
      this.calcErr = true;
      this.result = Constants.fehlerZahl;
      this.erg1 = Constants.fehlerZahl;
    }

    /// <summary>
    /// Method of FktWert: Calculates value of a potenz.
    /// </summary>
    /// <param name="bas">
    /// base 
    /// </param>
    /// <param name="ex">
    /// exponent - must be an integer. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public double FktWert_WertIntPot(double bas, int ex)
    {
      double result;
      int i;
      double r;
      r = 1.0;
      for (i = 1; i <= Math.Abs(ex); i++)
      {
        r = r * bas;
      }

      if (ex < 0)
      {
        if (r != 0)
        {
          r = 1 / r;
        }
        else
        {
          r = Constants.fehlerZahl;
          this.fktWertError = true;
        }
      }

      result = r;
      return result;
    }

    /// <summary>
    /// Calculates the value of a function for a given number.
    /// </summary>
    /// <param name="fx">
    /// function 
    /// </param>
    /// <param name="x">
    /// given number 
    /// </param>
    /// <returns>
    /// calculateted value <see cref="double"/> . 
    /// </returns>
    public double FreierFktWert(FunctionCalcTree fx, double x)
    {
      double result;
      this.varXwert = x;
      result = this.FktWert(fx, -1);
      return result;
    }

    /// <summary>
    ///   returns the scan result - 0 no error 
    /// </summary>
    /// <returns> error number <see cref="byte" /> . </returns>
    public byte GetScanResult()
    {
      byte result;
      result = this.lastErrNr;
      this.lastErrNr = 0;
      return result;
    }

    /// <summary>
    /// clears the function tree
    /// </summary>
    /// <param name="fx">
    /// function tree 
    /// </param>
    public void Loesch(ref FunctionCalcTree fx)
    {
      if (fx != null)
      {
        this.Loesch(ref fx.Li);
        this.Loesch(ref fx.Re);

        // fx.free           
        fx = null;
      }
    }

    /// <summary>
    /// ScanFkt - scans the function string and builds a function tree when successful.
    /// </summary>
    /// <param name="fx0">
    /// function tree
    /// </param>
    /// <param name="funcStr0">
    /// function string to scan 
    /// </param>
    public void ScanFkt(ref FunctionCalcTree fx0, string funcStr0)
    {
      if (funcStr0 != string.Empty)
      {
        this.maxLang = (byte)funcStr0.Length;
        this.funcStr = funcStr0;
        this.fx = fx0;
        this.fFormel = string.Empty;
        this.chPos = -1;
        this.oldChPos = -1;
        this.ch = ' ';
        this.fehler = false;
        this.errPos = 0;
        this.errNr = 0;
        this.ScanFkt_GetSym();
        this.ScanFkt_Expression(ref this.fx);
      }
      else
      {
        this.errNr = 6;
      }

      this.lastErrNr = (byte)this.errNr;
      this.lastErrPos = this.errPos;
      if (this.errNr == 0)
      {
        fx0 = this.fx;
      }
    }

    /// <summary>
    /// Method of the Scanfkt: set error number and its position in the string.
    /// </summary>
    /// <param name="nr">
    /// error number 
    /// </param>
    public void ScanFkt_Err(byte nr)
    {
      if (this.fehler)
      {
        return;
      }

      this.errPos = this.oldChPos;
      this.errNr = nr;
      this.fehler = true;
      this.chPos = 255;
      this.ch = '\0';
      this.sym = symTyp.fstrEnd;
      this.Loesch(ref this.fx);
    }


    /// <summary>
    /// Method of the Scanfkt: get expression.
    /// expression is a sum of terms or a term
    /// </summary>
    /// <param name="expr">
    /// The expression 
    /// </param>
    public void ScanFkt_Expression(ref FunctionCalcTree expr)
    {
      symTyp addop;
      FunctionCalcTree temp = null;
      if ((this.sym == symTyp.plus) || (this.sym == symTyp.minus))
      {
        addop = this.sym;
        this.ScanFkt_GetSym();
      }
      else
      {
        addop = symTyp.plus;
      }

      this.ScanFkt_Expression_Term(ref expr);
      if (addop == symTyp.minus)
      {
        if ((expr != null) && (expr.Cwert == symTyp.istZahl))
        {
          expr = this.ScanFkt_Zahl(-expr.Zwert);
        }
        else
        {
          expr = this.ScanFkt_MakeNode(this.ScanFkt_Zahl(-1), symTyp.mal, expr);
        }
      }

      while ((this.sym == symTyp.plus) || (this.sym == symTyp.minus))
      {
        do
        {
          addop = this.sym;
          this.ScanFkt_GetSym();
          if (this.sym == symTyp.plus)
          {
            this.sym = addop;
          }
          else if ((this.sym == symTyp.minus) && (addop == symTyp.minus))
          {
            this.sym = symTyp.plus;
          }
        }
        while (!((this.sym != symTyp.plus) && (this.sym != symTyp.minus)));
        this.ScanFkt_Expression_Term(ref temp);
        if (!this.fehler)
        {
          expr = this.ScanFkt_MakeNode(expr, addop, temp);
        }
      }
    }

    /// <summary>
    /// Method of the Scanfkt: get term.
    /// a term is a product or functionterm
    /// </summary>
    /// <param name="prod">
    /// prod - the term. 
    /// </param>
    public void ScanFkt_Expression_Term(ref FunctionCalcTree prod)
    {
      symTyp pSym;
      FunctionCalcTree temp = null;
      this.ScanFkt_Expression_Term_FuncTerm(ref prod);
      while ((this.sym == symTyp.mal) || (this.sym == symTyp.durch))
      {
        pSym = this.sym;
        this.ScanFkt_GetSym();
        this.ScanFkt_Expression_Term_FuncTerm(ref temp);
        if (!this.fehler)
        {
          prod = this.ScanFkt_MakeNode(prod, pSym, temp);
        }
      }
    }

    /// <summary>
    /// Method of the Scanfkt: get functionterm.
    /// a functionterm is function or a factor
    /// </summary>
    /// <param name="fterm">
    /// fterm - the functionterm. 
    /// </param>
    public void ScanFkt_Expression_Term_FuncTerm(ref FunctionCalcTree fterm)
    {
      symTyp fsym;
      FunctionCalcTree temp = null;
      if (this.sym < symTyp.sinf)
      {
        this.ScanFkt_Expression_Term_FuncTerm_Faktor(ref fterm);
      }
      else if (this.sym == symTyp.fstrEnd)
      {
        this.ScanFkt_Err(4);
      }
      else
      {
        fterm = null;
      }

      while ((symTyp.pot <= this.sym) && (this.sym < symTyp.ffmax))
      {
        fsym = this.sym;
        this.ScanFkt_GetSym();
        if (fsym > symTyp.pot)
        {
          if (this.sym != symTyp.lklammer)
          {
            this.ScanFkt_Err(1);
            return;
          }
        }

        if (this.sym < symTyp.sinf)
        {
          this.ScanFkt_Expression_Term_FuncTerm_Faktor(ref temp);
        }
        else
        {
          this.ScanFkt_Expression_Term_FuncTerm(ref temp);
        }

        if (!this.fehler)
        {
          fterm = this.ScanFkt_MakeNode(fterm, fsym, temp);
        }
      }
    }

    /// <summary>
    ///  Method of the Scanfkt:  get factor.
    ///  a factor is a variable, a constant, a number or an expression
    /// </summary>
    /// <param name="fakt">
    /// fakt - the factor. 
    /// </param>
    public void ScanFkt_Expression_Term_FuncTerm_Faktor(ref FunctionCalcTree fakt)
    {
      if (this.fehler)
      {
        return;
      }

      switch (this.sym)
      {
        case symTyp.ident:
          fakt = this.xVar;
          break;
        case symTyp.istKonst:
          fakt = this.kVar;
          break;
        case symTyp.istZahl:
          fakt = this.ScanFkt_Zahl(this.wert);
          break;
        case symTyp.lklammer:
          this.ScanFkt_GetSym();
          this.ScanFkt_Expression(ref fakt);
          if (this.sym != symTyp.rklammer)
          {
            this.ScanFkt_Err(2);
            return;
          }

          break;
        default:
          this.ScanFkt_Err(4);
          return;

        // break;
      }

      this.ScanFkt_GetSym();
      if (this.sym < symTyp.lklammer)
      {
        this.ScanFkt_Err(3);
      }
    }

    /// <summary>
    ///   Method of the Scanfkt: get next char.
    /// </summary>
    public void ScanFkt_GetCh()
    {
      if (this.chPos < this.maxLang - 1)
      {
        this.chPos++;
        this.ch_ = this.funcStr[this.chPos];
        this.ch = char.ToUpper(this.ch_);
      }
      else
      {
        this.ch = '\0';
        this.chPos = 255;
      }
    }

    /// <summary>
    ///   Method of the Scanfkt: get next symbol.
    /// </summary>
    public void ScanFkt_GetSym()
    {
      if (this.fehler)
      {
        return;
      }

      while (this.ch == ' ')
      {
        this.ScanFkt_GetCh();
      }

      this.oldChPos = this.chPos;
      if (('A' <= this.ch) && (this.ch <= 'Z')) 
      {
        this.ScanFkt_Identifier();
      }
      else if (('0' <= this.ch) && (this.ch <= '9'))
      {
        this.ScanFkt_Number();
      }
      else
      {
        switch (this.ch)
        {
          case '(':
            this.ScanFkt_MakeSym(symTyp.lklammer);
            break;
          case ')':
            this.ScanFkt_MakeSym(symTyp.rklammer);
            break;
          case '*':
            this.ScanFkt_MakeSym(symTyp.mal);
            break;
          case '+':
            this.ScanFkt_MakeSym(symTyp.plus);
            break;
          case '-':
            this.ScanFkt_MakeSym(symTyp.minus);
            break;
          case '/':
            this.ScanFkt_MakeSym(symTyp.durch);
            break;
          case '^':
            this.ScanFkt_MakeSym(symTyp.pot);
            break;
          case '§':
            this.ScanFkt_ConstantIdentifier();
            break;
          case '\0':
            this.sym = symTyp.fstrEnd;
            break;
          default:
            this.ScanFkt_Err(5);
            break;
        }
      }
    }

    /// <summary>
    ///   Method of the Scanfkt: reads an identifier.
    /// </summary>
    public void ScanFkt_Identifier()
    {
      string buf = string.Empty;
      string buf1 = string.Empty;
      do
      {
        buf = buf + this.ch;
        buf1 = buf1 + this.ch_;
        this.ScanFkt_GetCh();
      }
      while ( (this.ch >= 'A') && (this.ch <= 'Z') ); 

      if (buf == "PI")
      {
        this.sym = symTyp.istZahl;
        this.wert = Math.PI;
        return;
      }

      if (buf == "E")
      {
        if (this.ch == '^')
        {
          this.sym = symTyp.expf;
          this.ScanFkt_GetCh();
          return;
        }
        else
        {
          this.sym = symTyp.istZahl;
          this.wert = Constants.eulerZahl;
          return;
        }
      }
      // teste auf Funktionsbezeichner
      this.sym = symTyp.sinf;
      while ((this.sym < symTyp.ffmax) && (buf != Constants.fnam[(int)this.sym - 5]))
      {
        this.sym++;
      }

      if (this.sym == symTyp.ffmax)
      {
          if (buf.ToLower().CompareTo(Constants.varName.ToLower()) == 0)
          {
              this.sym = symTyp.ident;
              if (this.xVar == null)
              {
                  this.xVar = this.ScanFkt_MakeNode(null, symTyp.ident, null);
                  this.xVar.Zwert = 0;
                  this.xVar.Name = buf1;
              }
          }
          else
          {
          this.ScanFkt_Err(7);
          }
      }   
    }


    /// <summary>
    ///   Method of the Scanfkt: reads a constant_identifier.
    /// </summary>
    public void ScanFkt_ConstantIdentifier()
    {
        string buf = string.Empty;
        string buf1 = string.Empty;
        ushort i;

        do
        {
            buf = buf + this.ch;
            buf1 = buf1 + this.ch_;
            this.ScanFkt_GetCh();
        }
        while (( (this.ch >= 'A') && (this.ch <= 'Z') ) || (Constants.spezialChars.IndexOf(this.ch) >= 0) );
        
        i = 0;
        while (i < Constants.konstante.Length) 
        {
            if ((Constants.konstante[i].bez.ToLower().CompareTo(buf.ToLower()) == 0)
                    && (!this.noUpCase || (Constants.konstante[i].bez.CompareTo(buf1) == 0)))
            {
               this.sym = symTyp.istKonst;
               this.kVar = this.ScanFkt_MakeNode(null, symTyp.istKonst, null);
               this.kVar.Name = Constants.konstante[i].bez;
               this.kVar.Nr = i;
               this.kVar.Zwert = Constants.konstante[i].wert;
               return;
            }
            i++;
        }
      // keine Konstantenbezeichnung gefunden
      this.ScanFkt_Err(7);
    }


    /// <summary>
    /// Makes a node in the function tree.
    /// </summary>
    /// <param name="op1">
    ///  op1 - left tree/operand 
    /// </param>
    /// <param name="code">
    ///  code - operator 
    /// </param>
    /// <param name="op2">
    /// op2 right tree/operand 
    /// </param>
    /// <returns>
    /// The <see cref="CalculatorFunctionTerm"/> . 
    /// </returns>
    public FunctionCalcTree ScanFkt_MakeNode(FunctionCalcTree op1, symTyp code, FunctionCalcTree op2)
    {
        FunctionCalcTree result;
        result = new FunctionCalcTree();
      result.Cwert = code;
      result.Li = op1;
      result.Re = op2;
      return result;
    }

    
    /// <summary>
    /// Method of the Scanfkt: set sym and reads next char 
    /// </summary>
    /// <param name="s">
    /// s - symbol 
    /// </param>
    public void ScanFkt_MakeSym(symTyp s)
    {
      this.sym = s;
      this.ScanFkt_GetCh();
    }


    // Number
    /// <summary>
    ///   Method of the Scanfkt: reads a number.
    /// </summary>
    public void ScanFkt_Number()
    {
      this.hStr = string.Empty;
      this.sym = symTyp.istZahl;
      this.ScanFkt_ReadInt();
      if ((this.ch == '.') || (this.ch == ','))
      {
        this.hStr = this.hStr + this.ch;
        this.ScanFkt_GetCh();
        this.ScanFkt_ReadInt();
      }

      if (this.ch == 'E')
      {
        this.hStr = this.hStr + this.ch;
        this.ScanFkt_GetCh();
        if ((this.ch == '+') || (this.ch == '-'))
        {
          this.hStr = this.hStr + this.ch;
          this.ScanFkt_GetCh();
        }

        this.ScanFkt_ReadInt();
      }

      try
      {
        this.wert = Convert.ToDouble(this.hStr);
      }
      catch
      {
        this.ScanFkt_Err(4);
      }
      finally
      {
      }
    }

    /// <summary>
    /// Makes a node in the function tree, that contains a number
    /// </summary>
    /// <param name="r">
    /// r - value of number. 
    /// </param>
    /// <returns>
    /// The <see cref="CalculatorFunctionTerm"/> . 
    /// </returns>
    public FunctionCalcTree ScanFkt_Zahl(double r)
    {
        FunctionCalcTree result;
      result = this.ScanFkt_MakeNode(null, symTyp.istZahl, null);
      result.Zwert = r;
      return result;
    }

    /// <summary>
    /// Tests, if the given function is a linear function.
    /// </summary>
    /// <param name="fx">
    /// fx - the function term to test. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool isLinearFunction(FunctionCalcTree fx)
    {
      if ((fx == null) || (fx.Cwert <= symTyp.istKonst))
      {
        return true;
      }

      if (fx.Cwert > symTyp.pot)
      {
        return this.doesContainNoVar(fx.Re);
      }

      switch (fx.Cwert)
      {
        case symTyp.ident:
          return true;
        case symTyp.pot:
          return this.doesContainNoVar(fx.Li) && this.doesContainNoVar(fx.Re);
        case symTyp.plus:
          return this.isLinearFunction(fx.Li) && this.isLinearFunction(fx.Re);
        case symTyp.minus:
          return this.isLinearFunction(fx.Li) && this.isLinearFunction(fx.Re);
        case symTyp.mal:
          return (this.doesContainNoVar(fx.Li) && this.isLinearFunction(fx.Re))
                 || (this.isLinearFunction(fx.Li) && this.doesContainNoVar(fx.Re));
        case symTyp.durch:
          return this.isLinearFunction(fx.Li) && this.doesContainNoVar(fx.Re);
        default:
          return false;
      }
    }





    /// <summary>
    /// Method of the Scanfkt: reads an integer.
    /// </summary>
    private void ScanFkt_ReadInt()
    {
      while ((this.ch >= '0') && (this.ch <= '9'))
      {
        this.hStr = this.hStr + this.ch;
        this.ScanFkt_GetCh();
      }
    }

    /// <summary>
    /// Tests, if the function term contains no vars / only numbers and constants.
    /// </summary>
    /// <param name="fx">
    /// fx - the function term to test. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    private bool doesContainNoVar(FunctionCalcTree fx)
    {
      if ((fx == null) || (fx.Cwert <= symTyp.istKonst))
      {
        return true;
      }
      else
      {
        if (fx.Cwert == symTyp.ident)
        {
          return false;
        }
        else
        {
          return this.doesContainNoVar(fx.Li) && this.doesContainNoVar(fx.Re);
        }
      }
    }


  }
}