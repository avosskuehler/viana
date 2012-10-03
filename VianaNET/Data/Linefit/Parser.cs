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
namespace VianaNET.Data.Linefit
{
  using System;

  /// <summary>
  ///   The constants.
  /// </summary>
  public class Constants
  {
    // public const int MaxListSize = Int32.MaxValue / 16;

    // public const double _max_Real = 1.7e38;
    // public const int _Ln_max_Real = 88;
    #region Constants

    /// <summary>
    ///   The s list capacity error.
    /// </summary>
    public const string SListCapacityError = "Die Kapazität der Liste ist erschöpft (%d)";

    /// <summary>
    ///   The s list count error.
    /// </summary>
    public const string SListCountError = "Zu viele Einträge in der Liste (%d)";

    /// <summary>
    ///   The s list index error.
    /// </summary>
    public const string SListIndexError = "Der Index der Liste überschreitet das Maximum (%d)";

    /// <summary>
    ///   The _form aus.
    /// </summary>
    public const char _formAus = '!';

    /// <summary>
    ///   The _hoch.
    /// </summary>
    public const char _hoch = '\'';

    /// <summary>
    ///   The _normal.
    /// </summary>
    public const char _normal = '\"';

    /// <summary>
    ///   The _sym an.
    /// </summary>
    public const char _symAn = '#';

    // public const int do_Sym = 3;
    /// <summary>
    ///   The _sym aus.
    /// </summary>
    public const char _symAus = '§';

    /// <summary>
    ///   The _tief.
    /// </summary>
    public const char _tief = '_';

    /// <summary>
    ///   The euler zahl.
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


    #endregion

    #region Static Fields

    /// <summary>
    ///   The konstante.
    /// </summary>
    public static readonly KonstRec[] konstante = new[]
      {
        new KonstRec { titel = "PhysikKonstant_e", bez = "e§", wert = 1.6021E-19 }, 
        new KonstRec { titel = "PhysikKonstant_epsilon", bez = "#e§_0!", wert = 8.85419E-12 }, 
        new KonstRec { titel = "PhysikKonstant_c", bez = "c§", wert = 2.99792458E8 }, 
        new KonstRec { titel = "PhysikKonstant_mu", bez = "#m§_0!", wert = 1.256637E-6 }, 
        new KonstRec { titel = "PhysikKonstant_g", bez = "g§", wert = 9.80665 }, 
        new KonstRec { titel = "PhysikKonstant_h", bez = "h§", wert = 6.6256E-34 }, 
        new KonstRec { titel = "PhysikKonstant_me", bez = "m_#e§", wert = 9.1093897E-31 }, 
        new KonstRec { titel = "PhysikKonstant_f", bez = "#g§", wert = 6.67259E-11 }, 
        new KonstRec { titel = "PhysikKonstant_Lambda0", bez = "#l§_c!", wert = 2.43E-12 }
      };

    /// <summary>
    ///   The fnam.
    /// </summary>
    public static string[] fnam =
      {
        "(", ")", "+", "-", "*", "/", "^", "SIN", "COS", "TAN", "COTAN", "EXP", "LN", 
        "WURZEL", "SIGN", "ABS", "DELTA", "?"
      };

    #endregion

    /// <summary>
    ///   The konst rec.
    /// </summary>
    public struct KonstRec
    {
      #region Fields

      /// <summary>
      ///   The bez.
      /// </summary>
      public string bez;

      /// <summary>
      ///   The titel.
      /// </summary>
      public string titel;

      /// <summary>
      ///   The wert.
      /// </summary>
      public double wert;

      #endregion
    }
  }

  /*
    public struct KonstRec
    {
        public string titel;
        public string bez;
        public double wert;
    } // end KonstRec
*/
  /*   public struct TSpaltDefBuf
    {
        public string be;        // Bezeichnung
        public string di;        // Dimension
        public string ti;        // Titel
        public string fs;
        public int br;
        public int nk;
    } // end TSpaltDefBuf
*/

  /// <summary>
  ///   The t rechner art.
  /// </summary>
  public enum TRechnerArt
  {
    /// <summary>
    ///   The formel rechner.
    /// </summary>
    formelRechner,

    /// <summary>
    ///   The rechner.
    /// </summary>
    rechner
  }

  // end TRechnerArt

  /// <summary>
  ///   The sym typ.
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
    ///   The sinf.
    /// </summary>
    sinf,

    /// <summary>
    ///   The cosf.
    /// </summary>
    cosf,

    /// <summary>
    ///   The tanf.
    /// </summary>
    tanf,

    /// <summary>
    ///   The ctgf.
    /// </summary>
    ctgf,

    /// <summary>
    ///   The expf.
    /// </summary>
    expf,

    /// <summary>
    ///   The lnf.
    /// </summary>
    lnf,

    /// <summary>
    ///   The wurzf.
    /// </summary>
    wurzf,

    /// <summary>
    ///   The sigf.
    /// </summary>
    sigf,

    /// <summary>
    ///   The absf.
    /// </summary>
    absf,

    /// <summary>
    ///   The deltaf.
    /// </summary>
    deltaf,

    /// <summary>
    ///   The ffmax.
    /// </summary>
    ffmax,

    /// <summary>
    ///   The fstr end.
    /// </summary>
    fstrEnd
  }

  // end symTyp

  /// <summary>
  ///   The parse.
  /// </summary>
  public class Parse
  {
    #region Fields

    /// <summary>
    ///   The fkt term.
    /// </summary>
    public CalculatorFunctionTerm CalculatorFunctionTerm = new CalculatorFunctionTerm();

    /// <summary>
    ///   The fx.
    /// </summary>
    public CalculatorFunctionTerm fx;

    /// <summary>
    ///   The last err nr.
    /// </summary>
    public byte lastErrNr = 0;

    /// <summary>
    ///   The last err pos.
    /// </summary>
    public int lastErrPos = 0;

    /// <summary>
    ///   The calc err.
    /// </summary>
    private bool calcErr;

    /// <summary>
    ///   The ch.
    /// </summary>
    private char ch = ' ';

    /// <summary>
    ///   The ch pos.
    /// </summary>
    private short chPos = -1;

    /// <summary>
    ///   The ch_.
    /// </summary>
    private char ch_;

    /// <summary>
    ///   The erg 1.
    /// </summary>
    private double erg1;

    /// <summary>
    ///   The err nr.
    /// </summary>
    private int errNr;

    /// <summary>
    ///   The err pos.
    /// </summary>
    private int errPos;

    /// <summary>
    ///   The f formel.
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
    ///   The h str.
    /// </summary>
    private string hStr;

    /// <summary>
    ///   The k var.
    /// </summary>
    private CalculatorFunctionTerm kVar;

    /// <summary>
    ///   The max lang.
    /// </summary>
    private byte maxLang;

    /// <summary>
    ///   The no up case.
    /// </summary>
    private bool noUpCase = false;

    /// <summary>
    ///   The old ch pos.
    /// </summary>
    private short oldChPos = -1;

    /// <summary>
    ///   The result.
    /// </summary>
    private double result;

    /// <summary>
    ///   The sym.
    /// </summary>
    private symTyp sym;

    /// <summary>
    ///   The var xwert.
    /// </summary>
    private double varXwert;

    /// <summary>
    ///   The wert.
    /// </summary>
    private double wert;

    /// <summary>
    ///   The x var.
    /// </summary>
    private CalculatorFunctionTerm xVar;

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The err msg.
    /// </summary>
    /// <param name="nr">
    /// The nr. 
    /// </param>
    /// <param name="s">
    /// The s. 
    /// </param>
    public void ErrMsg(byte nr, ref string s)
    {
      switch (nr)
      {
        case 1:
          s = "( erwartet";
          break;
        case 2:
          s = ") erwartet";
          break;
        case 3:
          s = "Operator erwartet";
          break;
        case 4:
          s = "( oder Zahl oder Bezeichner erwartet";
          break;
        case 5:
          s = "unzulässiges Zeichen";
          break;
        case 6:
          s = "keine Formel definiert";
          break;
      }
    }

    /// <summary>
    /// The fkt wert.
    /// </summary>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    /// <param name="nr">
    /// The nr. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public double FktWert(CalculatorFunctionTerm fx, int nr)
    {
      double result;

      // bool fktWertError;
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
    /// The fkt wert_ berechne.
    /// </summary>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    /// <param name="nr">
    /// The nr. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public double FktWert_Berechne(CalculatorFunctionTerm fx, int nr)
    {
      // double result;
      // double erg1;
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
          this.result = fx.Zwert; // Constants.konstante[fx.nr].wert;
          break;
        case symTyp.ident:

          /*      if (nr >= 0)
                    {
                        switch (fx.vwert.WertAtOk((ushort)nr,ref erg1))
                        {
                            case Constants.is_FehlerZahl:
                                fktWertError = true;
                                calcErr = true;
                                break;
                            case Constants.is_LeerFeld:
                                fktWertError = true;
                                break;
                        }
                        result = erg1;
                    }
                    else
                    { */
          this.result = this.varXwert;

          // }
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
    ///   The fkt wert_ berechne_ fehler.
    /// </summary>
    public void FktWert_Berechne_Fehler()
    {
      this.fktWertError = true;
      this.calcErr = true;
      this.result = Constants.fehlerZahl;
      this.erg1 = Constants.fehlerZahl;
    }

    /// <summary>
    /// The fkt wert_ wert int pot.
    /// </summary>
    /// <param name="bas">
    /// The bas. 
    /// </param>
    /// <param name="ex">
    /// The ex. 
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
    /// The freier fkt wert.
    /// </summary>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    /// <param name="x">
    /// The x. 
    /// </param>
    /// <returns>
    /// The <see cref="double"/> . 
    /// </returns>
    public double FreierFktWert(CalculatorFunctionTerm fx, double x)
    {
      double result;
      this.varXwert = x;
      result = this.FktWert(fx, -1);
      return result;
    }

    /// <summary>
    ///   The get scan result.
    /// </summary>
    /// <returns> The <see cref="byte" /> . </returns>
    public byte GetScanResult()
    {
      byte result;
      result = this.lastErrNr;
      this.lastErrNr = 0;
      return result;
    }

    /// <summary>
    /// The loesch.
    /// </summary>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    public void Loesch(ref CalculatorFunctionTerm fx)
    {
      if (fx != null)
      {
        this.Loesch(ref fx.Li);
        this.Loesch(ref fx.Re);

        // fx.free           
        fx = null;

        // this.SpStat =(ushort)((this.SpStat & ~PhysTab.Constants.ZaS_berechnet) | PhysTab.Constants.ZaS_geaendert);
      }
    }

    /// <summary>
    /// The scan fkt.
    /// </summary>
    /// <param name="fx0">
    /// The fx 0. 
    /// </param>
    /// <param name="funcStr0">
    /// The func str 0. 
    /// </param>
    public void ScanFkt(ref CalculatorFunctionTerm fx0, string funcStr0)
    {
      if (funcStr0 != string.Empty)
      {
        this.maxLang = (byte)funcStr0.Length;
        funcStr0.Replace(',', '.');
        this.funcStr = funcStr0;
        this.fx = fx0;

        // while (funcStr.IndexOf(',') > 0)
        // {
        // funcStr[funcStr.IndexOf(',')] = '.';
        // }
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

      /*      if ((errNr == 0))
            {
                this.SpStat = (ushort)(PhysTab.Constants.ZaS_formel | PhysTab.Constants.ZaS_Scanned);
            }
            else
            {
                this.SpStat = (ushort)(PhysTab.Constants.ZaS_formel | PhysTab.Constants.ZaS_fehler);
            }
       */
      this.lastErrNr = (byte)this.errNr;
      this.lastErrPos = this.errPos;
      if (this.errNr == 0)
      {
        fx0 = this.fx;
      }
    }

    /// <summary>
    /// The scan fkt_ err.
    /// </summary>
    /// <param name="nr">
    /// The nr. 
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
    /// The scan fkt_ expression.
    /// </summary>
    /// <param name="expr">
    /// The expr. 
    /// </param>
    public void ScanFkt_Expression(ref CalculatorFunctionTerm expr)
    {
      symTyp addop;
      CalculatorFunctionTerm temp = null;
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
    /// The scan fkt_ expression_ term.
    /// </summary>
    /// <param name="prod">
    /// The prod. 
    /// </param>
    public void ScanFkt_Expression_Term(ref CalculatorFunctionTerm prod)
    {
      symTyp pSym;
      CalculatorFunctionTerm temp = null;
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
    /// The scan fkt_ expression_ term_ func term.
    /// </summary>
    /// <param name="fterm">
    /// The fterm. 
    /// </param>
    public void ScanFkt_Expression_Term_FuncTerm(ref CalculatorFunctionTerm fterm)
    {
      symTyp fsym;
      CalculatorFunctionTerm temp = null;
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
    /// The scan fkt_ expression_ term_ func term_ faktor.
    /// </summary>
    /// <param name="fakt">
    /// The fakt. 
    /// </param>
    public void ScanFkt_Expression_Term_FuncTerm_Faktor(ref CalculatorFunctionTerm fakt)
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
    ///   The scan fkt_ get ch.
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
    ///   The scan fkt_ get sym.
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
      if ((('A' <= this.ch) && (this.ch <= 'Z')) || (this.ch == '#'))
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
    ///   The scan fkt_ identifier.
    /// </summary>
    public void ScanFkt_Identifier()
    {
      string buf;
      string buf1;
      string Spezi = string.Concat(
        Constants._hoch, Constants._tief, Constants._symAn, Constants._symAus, Constants._formAus, Constants._normal);
      ushort i;

      // TZahlenSpalte sp;
      buf = string.Empty;
      buf1 = string.Empty;
      do
      {
        buf = buf + this.ch;
        buf1 = buf1 + this.ch_;
        this.ScanFkt_GetCh();
      }
      while (
        !(((this.ch < '0') || ((this.ch > '9') && (this.ch < 'A')) || (this.ch > 'Z')) && (Spezi.IndexOf(this.ch) == -1)));
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

      this.sym = symTyp.sinf;
      while ((this.sym < symTyp.ffmax) && (buf != Constants.fnam[(int)this.sym - 5]))
      {
        this.sym++;
      }

      /* 
            if ((sym == symTyp.ffmax))
            {
                i = 1;
                if ((this.fAnker != null))
                {
                    do
                    {
                        sp = this.fAnker[i];
                        if (sp != null)
                        {
                            if ((sp != this) && ((sp.Bez).ToLower().CompareTo((buf).ToLower()) == 0) && (!noUpCase || (sp.Bez.CompareTo(buf1) == 0)))
                            {
                                sym = symTyp.ident;
                                xVar = ScanFkt_MakeNode(null, symTyp.ident, null);
                                xVar.vwert = sp;
                                return;
                            }
                            else
                            {
                                i ++;
                            }
                        }
                        else
                        {
                            i = PhysTab.Constants.max_Anzahl_Spalten + 1;
                        }
                    } while (!((i > PhysTab.Constants.max_Anzahl_Spalten)));
                }
        */
      if (this.sym != symTyp.ident)
      {
        i = 0;
        do
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
          else
          {
            i++;
          }
        }
        while (!(i > Constants.konstante.Length-1));
      }

      if (this.sym == symTyp.ffmax)
      {
        this.sym = symTyp.ident;
        if (this.xVar == null)
        {
          this.xVar = this.ScanFkt_MakeNode(null, symTyp.ident, null);
          this.xVar.Zwert = 0;
          this.xVar.Name = buf1;
        }

        // ScanFkt_Err(5);
      }
    }

    /// <summary>
    /// The scan fkt_ make node.
    /// </summary>
    /// <param name="op1">
    /// The op 1. 
    /// </param>
    /// <param name="code">
    /// The code. 
    /// </param>
    /// <param name="op2">
    /// The op 2. 
    /// </param>
    /// <returns>
    /// The <see cref="CalculatorFunctionTerm"/> . 
    /// </returns>
    public CalculatorFunctionTerm ScanFkt_MakeNode(CalculatorFunctionTerm op1, symTyp code, CalculatorFunctionTerm op2)
    {
      CalculatorFunctionTerm result;
      result = new CalculatorFunctionTerm();
      result.Cwert = code;
      result.Li = op1;
      result.Re = op2;
      return result;
    }

    // Number
    /// <summary>
    /// The scan fkt_ make sym.
    /// </summary>
    /// <param name="s">
    /// The s. 
    /// </param>
    public void ScanFkt_MakeSym(symTyp s)
    {
      this.sym = s;
      this.ScanFkt_GetCh();
    }

    /// <summary>
    ///   The scan fkt_ number.
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
    /// The scan fkt_ zahl.
    /// </summary>
    /// <param name="r">
    /// The r. 
    /// </param>
    /// <returns>
    /// The <see cref="CalculatorFunctionTerm"/> . 
    /// </returns>
    public CalculatorFunctionTerm ScanFkt_Zahl(double r)
    {
      CalculatorFunctionTerm result;
      result = this.ScanFkt_MakeNode(null, symTyp.istZahl, null);
      result.Zwert = r;
      return result;
    }

    /// <summary>
    /// The is linear function.
    /// </summary>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool isLinearFunction(CalculatorFunctionTerm fx)
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

    #endregion

    #region Methods

    /// <summary>
    ///   The scan fkt_ read int.
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
    /// The does contain no var.
    /// </summary>
    /// <param name="fx">
    /// The fx. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    private bool doesContainNoVar(CalculatorFunctionTerm fx)
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

    #endregion
  }
}