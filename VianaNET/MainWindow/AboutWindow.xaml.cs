// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutWindow.xaml.cs" company="Freie Universität Berlin">
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
//   The about window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.MainWindow
{
  using System.IO;
  using System.Reflection;
  using System.Windows;

  /// <summary>
  ///   The about window.
  /// </summary>
  public partial class AboutWindow : Window
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="AboutWindow" /> class.
    ///   Initializes a new instance of the MainWindow class.
    /// </summary>
    public AboutWindow()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets the company string from the assembly.
    /// </summary>
    public static string AssemblyCompany
    {
      get
      {
        // Get all Company attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly()
          .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

        // If there aren't any Company attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Company attribute, return its value
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }

    /// <summary>
    ///   Gets the copyright property from the assembly.
    /// </summary>
    public static string AssemblyCopyright
    {
      get
      {
        // Get all Copyright attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly()
          .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

        // If there aren't any Copyright attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Copyright attribute, return its value
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    /// <summary>
    ///   Gets assembly description.
    /// </summary>
    public static string AssemblyDescription
    {
      get
      {
        // Get all Description attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly()
          .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

        // If there aren't any Description attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Description attribute, return its value
        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    /// <summary>
    ///   Gets the product property from the assembly.
    /// </summary>
    public static string AssemblyProduct
    {
      get
      {
        // Get all Product attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly()
          .GetCustomAttributes(typeof(AssemblyProductAttribute), false);

        // If there aren't any Product attributes, return an empty string
        if (attributes.Length == 0)
        {
          return string.Empty;
        }

        // If there is a Product attribute, return its value
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    /// <summary>
    ///   Gets the assemblies title.
    /// </summary>
    public static string AssemblyTitle
    {
      get
      {
        // Get all Title attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

        // If there is at least one Title attribute
        if (attributes.Length > 0)
        {
          // Select the first one
          var titleAttribute = (AssemblyTitleAttribute)attributes[0];

          // If it is not an empty string, return it
          if (titleAttribute.Title != string.Empty)
          {
            return titleAttribute.Title;
          }
        }

        // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
        return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    /// <summary>
    ///   Gets OGAMAs current version.
    /// </summary>
    public static string AssemblyVersionLong
    {
      get
      {
        return "(" + Assembly.GetExecutingAssembly().GetName().Version + ")";
      }
    }

    /// <summary>
    ///   Gets OGAMAs current version.
    /// </summary>
    public static string AssemblyVersionShort
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The button_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    #endregion

  }
}