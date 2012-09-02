using System;
using System.Collections;


public class Constants
{
    public const int MaxListSize = Int32.MaxValue / 16;
    public const int _fixed_Rows = 2;
    public const double _max_Real = 1.7e38;
    public const int _Ln_max_Real = 88;
    public const double fehlerZahl = -1.0E32;
    public const int is_FehlerZahl = 2;
    public const double leerFeld = -1.1E32;
    public const int is_LeerFeld = 1;
    public const int is_RealZahl = 0;

    public const int linReg = 1;
    public const int expSpezReg = 2;
    public const int logReg = 3;
    public const int potReg = 4;
    public const int quadReg = 5;
    public const int expReg = 6;
    public const int sinReg = 7;
    public const int sinExpReg = 8;
    public const int resoReg = 9;


    public const char _hoch = '\'';
    public const int do_hoch = 1;
    public const char _tief = '_';
    public const int do_Tief = 2;
    public const char _symAn = '#';
    public const int do_Sym = 3;
    public const char _symAus = '§';
    public const int undo_Sym = 4;
    public const char _formAus = '!';
    public const int undo_HochTief = 5;
    public const char _normal = '\"';
    public const int do_normal = 6;
    public const int do_beenden = 7;
    public static readonly string _alle = string.Concat(_hoch, _tief, _symAn, _symAus, _formAus, _normal, '\0');
    public const int ss_ohne = 0;
    public const int ss_hochEin = 1;
    public const int ss_tiefEin = 2;
    public const int ss_symEin = 4;
    public static string formatZeichen = _alle;
    public const int max_FEdit_Len = 80;
    public const int hochTiefEin = ss_hochEin + ss_tiefEin;

    public struct KonstRec
    {
        public string titel;
        public string bez;
        public double wert;
    } // end KonstRec

    public const int max_Anz_Konst = 8;
    public static readonly KonstRec[] konstante = new KonstRec[]
      {
         new KonstRec(){titel="Elementarladung", bez="e§", wert=1.6021E-19},
         new KonstRec(){titel="elektr. Feldkonstante",bez="#e§_0!",wert=8.85419E-12} , 
         new KonstRec(){titel="Lichtgeschwindigkeit", bez="c§", wert=2.99792458E8} , 
         new KonstRec(){titel="magn. FeldKonstante", bez="#m§_0!", wert=1.256637E-6} , 
         new KonstRec(){titel="Ortsfaktor Erde", bez="g§", wert=9.80665} , 
         new KonstRec(){titel="Planck-Quantum", bez="h§", wert=6.6256E-34} , 
         new KonstRec(){titel="Ruhemasse Elektron", bez="m_#e§", wert=9.1093897E-31} , 
         new KonstRec(){titel="Gravitationskonstante", bez="#g§", wert=6.67259E-11} , 
         new KonstRec(){titel="Compton-Wellenlänge", bez="#l§_c!", wert=2.43E-12} 
      };

    public const int lin_regress = 1;
    public const int log_regress = 2;
    public const int exp_regress = 4;
    public const int pot_regress = 8;
    public const int wachs_regress = 16;
    public const int sinus_regress = 32;
    public const int expSin_regress = 64;
    public const int resonanz_regress = 128;
    public const int best_regress = lin_regress + log_regress + exp_regress + pot_regress + wachs_regress + sinus_regress + expSin_regress + resonanz_regress;

    public const int max_Anzahl_Spalten = 8;
    public const int max_Regress_Param = 3;   // 0 = guete; 1 = m; 2 = c; rest frei

    public const int ZaS_formel = 1;
    public const int ZaS_geaendert = 2;
    public const int ZaS_gesichert = 4;
    public const int ZaS_Scanned = 8;
    public const int ZaS_berechnet = 16;
    public const int ZaS_fehler = 32;
    public const int ZaS_upDate = 64;
    public const int ZaS_Zeile = 128;
    public const int ZaS_TempNeu = 256;

    public const int SpErr_noMem = 1;        // Spalte konnte nicht erzeugt werden
    public const int SpErr_fTerm = 2;        // fehlerhafter Funktionsterm
    public const int SpErr_noFkt = 3;        // Funktionsterm fehlt
    public const int SpErr_noBez = 4;        // doppelte oder fehlende Bezeichnung einer Größe
    public const int SpErr_Verweis = 5;      // Spalte konnte nicht gelöscht werden, da ein Verweis vorliegt

    public const int ZaST_normal = 0;        // z.B.: s in cm
    public const int ZaST_bruch = 1;         // z.B.: s/cm
    public const int ZaST_klammer = 2;       // z.B.: s[cm]

    public const int maxBezeichnungLen = 6;     // Bezeichnung
    public const int maxDimensionLen = 12;      // Dimension
    public const int maxSpaltTitelLen = maxBezeichnungLen + maxDimensionLen + 4;  // SpaltenTitel
    public const int maxFktStrLen = 80;        // Funktionen
    public const int maxTabTitelLen = 40;

    public const double eulerZahl = 2.1782818;
    public static string[] fnam = { "(", ")", "+", "-", "*", "/", "^", "SIN", "COS", "TAN", "COTAN", "EXP", "LN", "WURZEL", "SIGN", "ABS", "DELTA", "?" };

    public const string SListIndexError = "Der Index der Liste überschreitet das Maximum (%d)";
    public const string SListCapacityError = "Die Kapazität der Liste ist erschöpft (%d)";
    public const string SListCountError = "Zu viele Einträge in der Liste (%d)";
}


namespace Parser
{
    public struct KonstRec
    {
        public string titel;
        public string bez;
        public double wert;
    } // end KonstRec

    public struct TSpaltDefBuf
    {
        public string be;        // Bezeichnung
        public string di;        // Dimension
        public string ti;        // Titel
        public string fs;
        public int br;
        public int nk;
    } // end TSpaltDefBuf

    public class TFktTerm
    {
        public TFktTerm li;
        public TFktTerm re;
        public symTyp cwert;
        public double zwert;
        public string name;
        public ushort nr;
   //     public TZahlenSpalte vwert;
    } // end TFktTerm

    public enum TTabArt
    {
        physikTab,
        matheTab
    } // end TTabArt

    public enum TRechnerArt
    {
        formelRechner,
        rechner
    } // end TRechnerArt

    public enum symTyp
    {
        udef,
        oldfx,
        istZahl,
        istKonst,
        ident,
        lklammer,
        rklammer,
        plus,
        minus,
        mal,
        durch,
        pot,
        sinf,
        cosf,
        tanf,
        ctgf,
        expf,
        lnf,
        wurzf,
        sigf,
        absf,
        deltaf,
        ffmax,
        fstrEnd
    } // end symTyp

          

    public class Parse
    {
        private string fFormel = String.Empty;
        public TFktTerm fktTerm = new TFktTerm();
        private bool noUpCase = false;
        private bool calcErr = false;
        public byte lastErrNr = 0;
        public int lastErrPos = 0;
        private string funcStr;
        public TFktTerm fx;

        short chPos = -1;
        short oldChPos = -1;
        char ch = ' ';
        char ch_;
        symTyp sym;
        double wert;
        byte maxLang;
        TFktTerm xVar;
        TFktTerm kVar;
        bool fehler = false;
        int errPos = 0;
        int errNr = 0;

         

        public void ErrMsg(byte nr, ref string s)
        {
            switch(nr)
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

        public byte GetScanResult()
        {
            byte result;
            result = lastErrNr;
            lastErrNr = 0;
            return result;
        }

        public void Loesch(ref TFktTerm fx)
        {
            if ((fx != null))
            {
                Loesch(ref fx.li);
                Loesch(ref fx.re);
                //fx.free           
                fx = null;
              //  this.SpStat =(ushort)((this.SpStat & ~PhysTab.Constants.ZaS_berechnet) | PhysTab.Constants.ZaS_geaendert);
            }
        }

             

        public TFktTerm ScanFkt_MakeNode(TFktTerm op1, symTyp code, TFktTerm op2)
        {
            TFktTerm result;
            result = new TFktTerm();
            result.cwert = code;
            result.li = op1;
            result.re = op2;
            return result;
        }

        public TFktTerm ScanFkt_Zahl(double r)
        {
            TFktTerm result;
            result = ScanFkt_MakeNode(null, symTyp.istZahl, null);
            result.zwert = r;
            return result;
        }

        public void ScanFkt_Err(byte nr)
        {
            if (fehler)
            {
                return;
            }
            errPos = oldChPos;
            errNr = nr;
            fehler = true;
            chPos = 255;
            ch = '\0';
            sym = symTyp.fstrEnd;
            Loesch(ref fx);
        }

        public void ScanFkt_GetCh()
        {
            if (chPos < maxLang-1)
            {
                chPos ++;
                ch_ = funcStr[chPos];
                ch = Char.ToUpper(ch_);
            }
            else
            {
                ch = '\0';
                chPos = 255;
            }
        }

        public void ScanFkt_Identifier()
        {
            string buf;
            string buf1;
            string Spezi = string.Concat(Constants._hoch, Constants._tief, Constants._symAn, Constants._symAus, Constants._formAus, Constants._normal);
            ushort i;
          //  TZahlenSpalte sp;
            buf = "";
            buf1 = "";
            do
            {
                buf = buf + ch;
                buf1 = buf1 + ch_;
                ScanFkt_GetCh();
            } while (!(((ch < '0') || ((ch > '9') && (ch < 'A')) || (ch > 'Z')) && (Spezi.IndexOf(ch) == -1)));
            if (buf == "PI")
            {
                sym = symTyp.istZahl;
                wert = Math.PI;
                return;
            }
            if (buf == "E")
            {
                if (ch == '^')
                {
                    sym = symTyp.expf;
                    ScanFkt_GetCh();
                    return;
                }
                else
                {
                    sym = symTyp.istZahl;
                    wert = Constants.eulerZahl;
                    return;
                }
            }
            sym = symTyp.sinf;
            while ((sym < symTyp.ffmax) && (buf != Constants.fnam[(int)sym - 5]))
            {
                sym ++;
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
                if (sym != symTyp.ident)
                {
                    i = 0;
                    do
                    {
                        if (((Constants.konstante[i].bez).ToLower().CompareTo((buf).ToLower()) == 0) && (!noUpCase || (Constants.konstante[i].bez.CompareTo(buf1) == 0)))
                        {
                            sym = symTyp.istKonst;
                            kVar = ScanFkt_MakeNode(null, symTyp.istKonst, null);
                            kVar.name = Constants.konstante[i].bez;
                            kVar.nr = i;
                            kVar.zwert = Constants.konstante[i].wert;
                            return;
                        }
                        else
                        {
                            i ++;
                        }
                    } while (!(i > Constants.max_Anz_Konst));
                }
            if ((sym == symTyp.ffmax))
            {
                sym = symTyp.ident;
                if (xVar==null)
                {
                    xVar = ScanFkt_MakeNode(null, symTyp.ident, null);
                    xVar.zwert=0;
                    xVar.name=buf1;
                }
               // ScanFkt_Err(5);
            }
        }

        private string hStr;
        private void ScanFkt_ReadInt()
        {
            while ((ch >= '0') && (ch <= '9'))
            {
                hStr = hStr + ch;
                ScanFkt_GetCh();
            }
        }

        public void ScanFkt_Number()
        {
            hStr=string.Empty;
            sym = symTyp.istZahl;
            ScanFkt_ReadInt();
            if ((ch == '.') || (ch == ','))
            {
                hStr = hStr + ch;
                ScanFkt_GetCh();
                ScanFkt_ReadInt();
            }
            if (ch == 'E')
            {
                hStr = hStr + ch;
                ScanFkt_GetCh();
                if ((ch == '+') || (ch == '-'))
                {
                    hStr = hStr + ch;
                    ScanFkt_GetCh();
                }
                ScanFkt_ReadInt();
            }
            try
            {
                wert = Convert.ToDouble(hStr);
            }
            catch { ScanFkt_Err(4); }
            finally {}
        }

        // Number
        public void ScanFkt_MakeSym(symTyp s)
        {
            sym = s;
            ScanFkt_GetCh();
        }

        public void ScanFkt_GetSym()
        {
            if (fehler)
            {
                return;
            }
            while (ch == ' ')
            {
                ScanFkt_GetCh();
            }
            oldChPos = chPos;
            if ((('A' <= ch) && (ch <= 'Z')) || (ch == '#'))
            {
                ScanFkt_Identifier();
            }
            else if (('0' <= ch) && (ch <= '9'))
            {
                ScanFkt_Number();
            }
            else
            {
                switch(ch)
                {
                    case '(':
                        ScanFkt_MakeSym(symTyp.lklammer);
                        break;
                    case ')':
                        ScanFkt_MakeSym(symTyp.rklammer);
                        break;
                    case '*':
                        ScanFkt_MakeSym(symTyp.mal);
                        break;
                    case '+':
                        ScanFkt_MakeSym(symTyp.plus);
                        break;
                    case '-':
                        ScanFkt_MakeSym(symTyp.minus);
                        break;
                    case '/':
                        ScanFkt_MakeSym(symTyp.durch);
                        break;
                    case '^':
                        ScanFkt_MakeSym(symTyp.pot);
                        break;
                    case '\0':
                        sym = symTyp.fstrEnd;
                        break;
                    default:
                        ScanFkt_Err(5);
                        break;
                }
            }
        }
        

        public void ScanFkt_Expression_Term_FuncTerm_Faktor(ref TFktTerm fakt)
        {
            if (fehler)
            {
                return;
            }
            switch(sym)
            {
                case symTyp.ident:
                    fakt = xVar;
                    break;
                case symTyp.istKonst:
                    fakt = kVar;
                    break;
                case symTyp.istZahl:
                    fakt = ScanFkt_Zahl(wert);
                    break;
                case symTyp.lklammer:
                    ScanFkt_GetSym();
                    ScanFkt_Expression(ref fakt);
                    if (sym != symTyp.rklammer)
                    {
                        ScanFkt_Err(2);
                        return;
                    }
                    break;
                default:
                    ScanFkt_Err(4);
                    return;
                  //  break;
            }
            ScanFkt_GetSym();
            if (sym < symTyp.lklammer)
            {
                ScanFkt_Err(3);
            }
        }

        public void ScanFkt_Expression_Term_FuncTerm(ref TFktTerm fterm)
        {
            symTyp fsym;
            TFktTerm temp = null;
            if (sym < symTyp.sinf)
            {
                ScanFkt_Expression_Term_FuncTerm_Faktor(ref fterm);
            }
            else if (sym == symTyp.fstrEnd)
            {
                ScanFkt_Err(4);
            }
            else
            {
                fterm = null;
            }
            while ((symTyp.pot <= sym) && (sym < symTyp.ffmax))
            {
                fsym = sym;
                ScanFkt_GetSym();
                if (fsym > symTyp.pot)
                {
                    if (sym != symTyp.lklammer)
                    {
                        ScanFkt_Err(1);
                        return;
                    }
                }
                if (sym < symTyp.sinf)
                {
                    ScanFkt_Expression_Term_FuncTerm_Faktor(ref temp);
                }
                else
                {
                    ScanFkt_Expression_Term_FuncTerm(ref temp);
                }
                if (!fehler)
                {
                    fterm = ScanFkt_MakeNode(fterm, fsym, temp);
                }
            }
        }

        public void ScanFkt_Expression_Term(ref TFktTerm prod)
        {
            symTyp pSym;
            TFktTerm temp = null;
            ScanFkt_Expression_Term_FuncTerm(ref prod);
            while ((sym == symTyp.mal) || (sym == symTyp.durch))
            {
                pSym = sym;
                ScanFkt_GetSym();
                ScanFkt_Expression_Term_FuncTerm(ref temp);
                if (!fehler)
                {
                    prod = ScanFkt_MakeNode(prod, pSym, temp);
                }
            }
        }

        public void ScanFkt_Expression(ref TFktTerm expr)
        {
            symTyp addop;
            TFktTerm temp = null;
            if ((sym == symTyp.plus) || (sym == symTyp.minus))
            {
                addop = sym;
                ScanFkt_GetSym();
            }
            else
            {
                addop = symTyp.plus;
            }
            ScanFkt_Expression_Term(ref expr);
            if (addop == symTyp.minus)
            {
                if ((expr!=null)&&(expr.cwert == symTyp.istZahl))
                {
                    expr = ScanFkt_Zahl( -expr.zwert);
                }
                else
                {
                    expr = ScanFkt_MakeNode(ScanFkt_Zahl( -1), symTyp.mal, expr);
                }
            }
            while ((sym == symTyp.plus) || (sym == symTyp.minus))
            {
                do
                {
                    addop = sym;
                    ScanFkt_GetSym();
                    if ((sym == symTyp.plus))
                    {
                        sym = addop;
                    }
                    else if ((sym == symTyp.minus) && (addop == symTyp.minus))
                    {
                        sym = symTyp.plus;
                    }
                } while (!((sym != symTyp.plus) && (sym != symTyp.minus)));
                ScanFkt_Expression_Term(ref temp);
                if (!fehler)
                {
                    expr = ScanFkt_MakeNode(expr, addop, temp);
                }
            }
        }

        public void ScanFkt(ref TFktTerm fx0, string funcStr0)
        {
            if ((funcStr0 != ""))
            {
                maxLang = (byte)funcStr0.Length;
                funcStr0.Replace(',', '.');
                funcStr = funcStr0;
                fx = fx0;
            //   while (funcStr.IndexOf(',') > 0)
            //    {
            //        funcStr[funcStr.IndexOf(',')] = '.';
            //    }
                fFormel = String.Empty;
                chPos = -1;
                oldChPos = -1;
                ch = ' ';
                fehler = false;
                errPos = 0;
                errNr = 0;
                ScanFkt_GetSym();
                ScanFkt_Expression(ref fx);
                
            }
            else
            {
                errNr = 6;
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
            lastErrNr = (byte)(errNr);
            lastErrPos = errPos;
            if ((errNr == 0))
            {
                fx0=fx;
            }
        }

        bool fktWertError=false;
        double varXwert;
        public double FktWert_WertIntPot(double bas, int ex)
        {
            double result;
            int i;
            double r;
            r = 1.0;
            for (i = 1; i <= Math.Abs(ex); i ++ )
            {
                r = r * bas;
            }
            if ((ex < 0))
            {
                if ((r != 0))
                {
                    r = 1 / r;
                }
                else
                {
                    r = Constants.fehlerZahl;
                    fktWertError = true;
                }
            }
            result = r;
            return result;
        }

        double result;
        double erg1;
        public void FktWert_Berechne_Fehler()
        {
            fktWertError = true;
            calcErr = true;
            result = Constants.fehlerZahl;
            erg1 = Constants.fehlerZahl;
        }

        public double FktWert_Berechne(TFktTerm fx, int nr)
        {
          //  double result;
          //  double erg1;
            double erg2;
            if (fktWertError)
            {
                result = Constants.fehlerZahl;
                return result;
            }
            switch(fx.cwert)
            {
                case symTyp.istZahl:
                    result = fx.zwert;
                    break;
                case symTyp.istKonst:
                    result =fx.zwert;  // Constants.konstante[fx.nr].wert;
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
                        result = varXwert;
                   // }
                    break;
                default:
                    erg2 = FktWert_Berechne(fx.re, nr);
                    if (fktWertError)
                    {
                        result = erg2;
                        return result;
                    }
                   
                    if (fx.cwert <= symTyp.pot)
                    {
                        erg1 = FktWert_Berechne(fx.li, nr);
                        if (fktWertError)
                        {
                            result = erg1;
                            return result;
                        }
                        
                        switch(fx.cwert)
                        {
                            case symTyp.plus:
                                erg1 = erg1 + erg2;
                                break;
                            case symTyp.minus:
                                erg1 = erg1 - erg2;
                                break;
                            case symTyp.mal:
                                erg1 = erg1 * erg2;
                                break;
                            case symTyp.durch:
                                if (erg2 != 0)
                                {
                                    erg1 = erg1 / erg2;
                                }
                                else
                                {
                                    FktWert_Berechne_Fehler();
                                }
                                break;
                            case symTyp.pot:
                                if (Convert.ToInt16(erg2) == erg2)
                                {
                                    erg1 = FktWert_WertIntPot(erg1, Convert.ToInt16(erg2));
                                }
                                else if (erg1 > 0)
                                {
                                    erg1 = Math.Exp(erg2 * Math.Log(erg1));
                                }
                                else if (erg1 < 0)
                                {
                                    FktWert_Berechne_Fehler();
                                }
                                break;
                            default:
                                FktWert_Berechne_Fehler();
                                break;
                        }
                    }
                    else
                    {
                        switch(fx.cwert)
                        {
                            case symTyp.sinf:
                                erg1 = Math.Sin(erg2);
                                break;
                            case symTyp.cosf:
                                erg1 = Math.Cos(erg2);
                                break;
                            case symTyp.tanf:
                                erg1 = Math.Cos(erg2);
                                if (erg1 != 0)
                                {
                                    erg1 = Math.Sin(erg2) / erg1;
                                }
                                else
                                {
                                    FktWert_Berechne_Fehler();
                                }
                                break;
                            case symTyp.ctgf:
                                erg1 = Math.Sin(erg2);
                                if (erg1 != 0)
                                {
                                    erg1 = Math.Cos(erg2) / erg1;
                                }
                                else
                                {
                                    FktWert_Berechne_Fehler();
                                }
                                break;
                            case symTyp.expf:
                                erg1 = Math.Exp(erg2);
                                break;
                            case symTyp.lnf:
                                if (erg2 > 0)
                                {
                                    erg1 = Math.Log(erg2);
                                }
                                else
                                {
                                    FktWert_Berechne_Fehler();
                                }
                                break;
                            case symTyp.wurzf:
                                if (erg2 >= 0)
                                {
                                    erg1 = Math.Sqrt(erg2);
                                }
                                else
                                {
                                    FktWert_Berechne_Fehler();
                                }
                                break;
                            case symTyp.sigf:
                                if (erg2 > 0)
                                {
                                    erg1 = 1;
                                }
                                else if (erg2 < 0)
                                {
                                    erg1 =  -1;
                                }
                                else
                                {
                                    erg1 = 0;
                                }
                                break;
                            case symTyp.absf:
                                erg1 = Math.Abs(erg2);
                                break;
                            case symTyp.deltaf:
                                if (nr == 0)
                                {
                                    erg1 = Constants.leerFeld;
                                }
                                else
                                {
                                    erg1 = FktWert_Berechne(fx.re, nr - 1);
                                    if (fktWertError)
                                    {
                                        result = erg1;
                                        return result;
                                    }
                                    erg1 = erg2 - erg1;
                                }
                                break;
                            default:
                                FktWert_Berechne_Fehler();
                                break;
                        }
                    }
                    result = erg1;
                    break;
            }
            return result;
        }

        public double FktWert(TFktTerm fx, int nr)
        {
            double result;
          //  bool fktWertError;
            if (fx != null)
            {
                calcErr = false;
                fktWertError = false;
                result = FktWert_Berechne(fx, nr);
                if (fktWertError)
                {
                    if (calcErr)
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
                calcErr = true;
            }
            return result;
        }

        public double FreierFktWert(TFktTerm fx, double x)
        {
            double result;
            varXwert = x;
            result = FktWert(fx,  -1);
            return result;
        }

     /*   public bool Verweis_auf_Spalte(TFktTerm fx, ushort nr)
        {
            bool result;
            result = false;
            if (fx == null)
            {
                return result;
            }
            //@ Unsupported property or method(C): 'cwert'
            switch(fx.cwert)
            {
                case symTyp.istZahl:
                case symTyp.istKonst:
                    break;
                case symTyp.ident:
                    result = (fx.vwert == this.fAnker[nr]);
                    break;
                default:
                    result = Verweis_auf_Spalte(fx.re, nr) || Verweis_auf_Spalte(fx.li, nr);
                    break;
            }
            return result;
        }
  */

    }
}
