﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TopFrame.xaml.cs" company="Freie Universität Berlin">
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
//   Interaction logic for TopFrame.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.MainWindow
{
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  ///   Interaction logic for TopFrame.xaml
  /// </summary>
  public partial class TopFrame : UserControl
  {


    /// <summary>
    ///   Initializes a new instance of the <see cref="TopFrame" /> class.
    /// </summary>
    public TopFrame()
    {
      this.InitializeComponent();
    }





    /// <summary>
    ///   Sets the icon.
    /// </summary>
    public ImageSource Icon
    {
      get => this.LeftIcon.Source;
      set => this.LeftIcon.Source = value;
    }

    /// <summary>
    ///   Gets or sets the title.
    /// </summary>
    public string Title
    {
      get => this.Header.Text;

      set => this.Header.Text = value;
    }

    /// <summary>
    ///   Gets or sets the titles fontsize.
    /// </summary>
    public double TitleSize
    {
      get => this.Header.FontSize;

      set => this.Header.FontSize = value;
    }


  }
}