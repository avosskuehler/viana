// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineOptionsDialog.xaml.cs" company="Freie Universität Berlin">
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
//   The line options dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.Data.Linefit
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  /// <summary>
  /// The line options dialog.
  /// </summary>
  public partial class LineOptionsDialog : Window
  {
    ///////////////////////////////////////////////////////////////////////////////
    // Defining Constants                                                        //
    ///////////////////////////////////////////////////////////////////////////////
    #region Fields

    /// <summary>
    /// The _line color regress.
    /// </summary>
    private SolidColorBrush _lineColorRegress;

    /// <summary>
    /// The _line color theorie.
    /// </summary>
    private SolidColorBrush _lineColorTheorie;

    /// <summary>
    /// The _line thickness regress.
    /// </summary>
    private double _lineThicknessRegress;

    /// <summary>
    /// The _line thickness theorie.
    /// </summary>
    private double _lineThicknessTheorie;

    /// <summary>
    /// The beispiel zahl.
    /// </summary>
    private double beispielZahl = 0.00123456789;

    /// <summary>
    /// The old border regress.
    /// </summary>
    private Border oldBorderRegress;

    /// <summary>
    /// The old border theorie.
    /// </summary>
    private Border oldBorderTheorie;

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Construction and Initializing methods                                     //
    ///////////////////////////////////////////////////////////////////////////////
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the LineOptionsDialog class.
    /// </summary>
    /// <param name="breiteRegress">
    /// The breite Regress.
    /// </param>
    /// <param name="regressLineColor">
    /// The regress Line Color.
    /// </param>
    /// <param name="breiteTheorie">
    /// The breite Theorie.
    /// </param>
    /// <param name="theorieLineColor">
    /// The theorie Line Color.
    /// </param>
    /// <param name="stellenAnzahl">
    /// The stellen Anzahl.
    /// </param>
    public LineOptionsDialog(
      double breiteRegress, 
      SolidColorBrush regressLineColor, 
      double breiteTheorie, 
      SolidColorBrush theorieLineColor, 
      int stellenAnzahl)
    {
      this.InitializeComponent();
      this.stellenZahl = 0;
      this.lineThicknessRegress = breiteRegress;
      this.lineThicknessTheorie = breiteTheorie;
      this.lineColorRegress = regressLineColor;
      this.lineColorTheorie = theorieLineColor;
      this.stellenZahl = stellenAnzahl;

      // regressLine.StrokeThickness = breiteRegress;
      // regressLine.Stroke = regressLineColor;
      // theorieLine.StrokeThickness = breiteTheorie;
      // theorieLine.Stroke = theorieLineColor;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Defining events, enums, delegates                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Defining Properties                                                       //
    ///////////////////////////////////////////////////////////////////////////////
    #region Public Properties

    /// <summary>
    /// Gets or sets the line color regress.
    /// </summary>
    public SolidColorBrush lineColorRegress
    {
      get
      {
        return this._lineColorRegress;
      }

      set
      {
        this._lineColorRegress = value;
        this.regressLine.Stroke = value;
        this.ShowSelectedRegressColor(value);
      }
    }

    /// <summary>
    /// Gets or sets the line color theorie.
    /// </summary>
    public SolidColorBrush lineColorTheorie
    {
      get
      {
        return this._lineColorTheorie;
      }

      set
      {
        this._lineColorTheorie = value;
        this.theorieLine.Stroke = value;
        this.ShowSelectedTheorieColor(value);
      }
    }

    /// <summary>
    /// Gets or sets the line thickness regress.
    /// </summary>
    public double lineThicknessRegress
    {
      get
      {
        return this._lineThicknessRegress;
      }

      set
      {
        this._lineThicknessRegress = value;
        this.regressLine.StrokeThickness = value;
        if (value != this.sliderLinienDickeA.Value)
        {
          this.sliderLinienDickeA.Value = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the line thickness theorie.
    /// </summary>
    public double lineThicknessTheorie
    {
      get
      {
        return this._lineThicknessTheorie;
      }

      set
      {
        this._lineThicknessTheorie = value;
        this.theorieLine.StrokeThickness = value;
        if (value != this.sliderLinienDickeT.Value)
        {
          this.sliderLinienDickeT.Value = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the stellen zahl.
    /// </summary>
    public int stellenZahl
    {
      get
      {
        return (int)this.sliderGenauigkeit.Value;
      }

      set
      {
        this.sliderGenauigkeit.Value = value;
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Public methods                                                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Inherited methods                                                         //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Eventhandler                                                              //
    ///////////////////////////////////////////////////////////////////////////////
    #region Methods

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// The o k_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Methods and Eventhandling for Background tasks                            //
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // Methods for doing main class job                                          //
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The show selected regress color.
    /// </summary>
    /// <param name="aktColor">
    /// The akt color.
    /// </param>
    private void ShowSelectedRegressColor(SolidColorBrush aktColor)
    {
      if (aktColor == this.borderR1.Background)
      {
        this.borderR_MouseDown(this.borderR1, null);
        return;
      }

      if (aktColor == this.borderR2.Background)
      {
        this.borderR_MouseDown(this.borderR2, null);
        return;
      }

      if (aktColor == this.borderR3.Background)
      {
        this.borderR_MouseDown(this.borderR3, null);
        return;
      }

      if (aktColor == this.borderR4.Background)
      {
        this.borderR_MouseDown(this.borderR4, null);
        return;
      }

      if (aktColor == this.borderR5.Background)
      {
        this.borderR_MouseDown(this.borderR5, null);
        return;
      }

      if (aktColor == this.borderR6.Background)
      {
        this.borderR_MouseDown(this.borderR6, null);
        return;
      }

      if (aktColor == this.borderR7.Background)
      {
        this.borderR_MouseDown(this.borderR7, null);
        return;
      }

      if (aktColor == this.borderR8.Background)
      {
        this.borderR_MouseDown(this.borderR8, null);
        return;
      }

      if (aktColor == this.borderR9.Background)
      {
        this.borderR_MouseDown(this.borderR9, null);
        return;
      }

      if (aktColor == this.borderR10.Background)
      {
        this.borderR_MouseDown(this.borderR10, null);
        return;
      }

      if (aktColor == this.borderR11.Background)
      {
        this.borderR_MouseDown(this.borderR11, null);
        return;
      }

      /*        if (aktColor == borderR12.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR13.Background) { border_MouseDown(borderR10, null); return;}
                    if (aktColor == borderR14.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR15.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR16.Background) { border_MouseDown(borderR10, null); return; }
                    if (aktColor == borderR17.Background) { border_MouseDown(borderR10, null); return; }  */
    }

    /// <summary>
    /// The show selected theorie color.
    /// </summary>
    /// <param name="aktColor">
    /// The akt color.
    /// </param>
    private void ShowSelectedTheorieColor(SolidColorBrush aktColor)
    {
      if (aktColor == this.borderT1.Background)
      {
        this.borderT_MouseDown(this.borderT1, null);
        return;
      }

      if (aktColor == this.borderT2.Background)
      {
        this.borderT_MouseDown(this.borderT2, null);
        return;
      }

      if (aktColor == this.borderT3.Background)
      {
        this.borderT_MouseDown(this.borderT3, null);
        return;
      }

      if (aktColor == this.borderT4.Background)
      {
        this.borderT_MouseDown(this.borderT4, null);
        return;
      }

      if (aktColor == this.borderT5.Background)
      {
        this.borderT_MouseDown(this.borderT5, null);
        return;
      }

      if (aktColor == this.borderT6.Background)
      {
        this.borderT_MouseDown(this.borderT6, null);
        return;
      }

      if (aktColor == this.borderT7.Background)
      {
        this.borderT_MouseDown(this.borderT7, null);
        return;
      }

      if (aktColor == this.borderT8.Background)
      {
        this.borderT_MouseDown(this.borderT8, null);
        return;
      }

      if (aktColor == this.borderT9.Background)
      {
        this.borderT_MouseDown(this.borderT9, null);
        return;
      }

      if (aktColor == this.borderT10.Background)
      {
        this.borderT_MouseDown(this.borderT10, null);
        return;
      }

      if (aktColor == this.borderT11.Background)
      {
        this.borderT_MouseDown(this.borderT11, null);
        return;
      }

      /*       if (aktColor == borderT12.Background) { border_MouseDown(borderT12, null); return; }
                   if (aktColor == borderT13.Background) { border_MouseDown(borderT13, null); return;}
                   if (aktColor == borderT14.Background) { border_MouseDown(borderT14, null); return; }
                   if (aktColor == borderT15.Background) { border_MouseDown(borderT15, null); return; }
                   if (aktColor == borderT16.Background) { border_MouseDown(borderT16, null); return; }
                   if (aktColor == borderT17.Background) { border_MouseDown(borderT17, null); return; }  */
    }

    /// <summary>
    /// The border r_ mouse down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void borderR_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var aktBorder = sender as Border;
      if ((this.oldBorderRegress != null) && (this.oldBorderRegress == aktBorder))
      {
        return;
      }

      if (this.oldBorderRegress != null)
      {
        this.oldBorderRegress.BorderBrush = null;
      }

      aktBorder.BorderBrush = Brushes.Black;
      this.oldBorderRegress = aktBorder;
      this.lineColorRegress = (SolidColorBrush)aktBorder.Background;
    }

    /// <summary>
    /// The border t_ mouse down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void borderT_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var aktBorder = sender as Border;
      if ((this.oldBorderTheorie != null) && (this.oldBorderTheorie == aktBorder))
      {
        return;
      }

      if (this.oldBorderTheorie != null)
      {
        this.oldBorderTheorie.BorderBrush = null;
      }

      aktBorder.BorderBrush = Brushes.Black;
      this.oldBorderTheorie = aktBorder;
      this.lineColorTheorie = (SolidColorBrush)aktBorder.Background;
    }

    /// <summary>
    /// The slider genauigkeit_ value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void sliderGenauigkeit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      string gx;
      if (this.labelBeispiel == null)
      {
        return;
      }

      this.stellenZahl = (int)this.sliderGenauigkeit.Value;
      gx = "G" + this.stellenZahl.ToString();
      this.labelBeispiel.Content = this.beispielZahl.ToString(gx);
    }

    /// <summary>
    /// The slider linien dicke a_ value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void sliderLinienDickeA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this._lineThicknessRegress = this.sliderLinienDickeA.Value;
      if (this.regressLine != null)
      {
        this.regressLine.StrokeThickness = this._lineThicknessRegress;
      }
    }

    /// <summary>
    /// The slider linien dicke t_ value changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void sliderLinienDickeT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this._lineThicknessTheorie = this.sliderLinienDickeT.Value;
      if (this.theorieLine != null)
      {
        this.theorieLine.StrokeThickness = this._lineThicknessTheorie;
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////
    // Small helping Methods                                                     //
    ///////////////////////////////////////////////////////////////////////////////
  }
}