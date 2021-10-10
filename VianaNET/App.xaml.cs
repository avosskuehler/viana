namespace VianaNET
{
  using System;
  using System.Globalization;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Markup;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Threading;
  using VianaNET.Application;
  using VianaNET.Logging;
  using VianaNET.Properties;
  using WPFLocalizeExtension.Engine;

  public partial class App
  {
    /// <summary>
    ///   The static member, that holds the project.
    /// </summary>
    private static Project project;

    public App()
    {
      InitialiseCultures();
    }

    /// <summary>
    /// Gets or sets the static member, that holds the project.
    /// </summary>
    public static Project Project
    {
      get => project ?? (project = new Project());

      set => project = value;
    }

    #region Public Methods and Operators
    private static void InitialiseCultures()
    {
      if (!string.IsNullOrEmpty(Settings.Default.Culture))
      {
        LocalizeDictionary.Instance.Culture
          = Thread.CurrentThread.CurrentCulture
          = new CultureInfo(Settings.Default.Culture);
      }

      if (!string.IsNullOrEmpty(Settings.Default.UICulture))
      {
        LocalizeDictionary.Instance.Culture
          = Thread.CurrentThread.CurrentUICulture
          = new CultureInfo(Settings.Default.UICulture);
      }

      LocalizeDictionary.Instance.Culture
          = Thread.CurrentThread.CurrentCulture
          = new CultureInfo("de");
      FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));
    }

    /// <summary>
    ///   Is a replacement for the winform Application.DoEvents function
    ///   for WPF. Process the current dispatcher queue.
    /// </summary>
    public static void DoEvents()
    {
      if (Current != null)
      {
        Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      }
    }

    /// <summary>
    /// This static mehtod returns an <see cref="Image"/>
    /// for the given filename string, if the image is in the Images
    /// subfolder of the solution.
    /// </summary>
    /// <param name="imageName">A <see cref="String"/> with the images file name</param>
    /// <returns>The <see cref="Image"/> that can be used as a source for
    /// an icon property.</returns>
    public static Image GetImage(string imageName)
    {
      Image terminMenuentryIcon = new Image();
      BitmapImage terminMenuentryIconImage = new BitmapImage();
      terminMenuentryIconImage.BeginInit();
      terminMenuentryIconImage.UriSource = new Uri("pack://application:,,,/Images/" + imageName);
      terminMenuentryIconImage.EndInit();
      terminMenuentryIcon.Source = terminMenuentryIconImage;
      return terminMenuentryIcon;
    }

    /// <summary>
    /// This static mehtod returns an <see cref="ImageSource"/>
    /// for the given filename string, if the image is in the Images
    /// subfolder of the solution.
    /// </summary>
    /// <param name="imageName">A <see cref="String"/> with the images file name</param>
    /// <returns>The <see cref="ImageSource"/> that can be used as a source for
    /// an imagesource property.</returns>
    public static ImageSource GetImageSource(string imageName)
    {
      BitmapImage terminMenuentryIconImage = new BitmapImage();
      terminMenuentryIconImage.BeginInit();
      terminMenuentryIconImage.UriSource = new Uri("pack://application:,,,/Images/" + imageName);
      terminMenuentryIconImage.EndInit();
      return terminMenuentryIconImage;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The <see cref="System.Windows.Application.DispatcherUnhandledException"/> event handler.
    ///   Displays a message for each otherwise unhandled exception.
    /// </summary>
    /// <param name="sender">
    /// Source of the event 
    /// </param>
    /// <param name="e">
    /// The <see cref="StartupEventArgs"/> with the event data. 
    /// </param>
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      // Raise message box and log to file.
      ErrorLogger.ProcessException(e.Exception, true);
    }

    /// <summary>
    /// The <see cref="Application.Startup"/> event handler.
    ///   Defaults localization to german locale and registers
    ///   to unhandled exception event.
    /// </summary>
    /// <param name="sender">
    /// Source of the event 
    /// </param>
    /// <param name="args">
    /// The <see cref="StartupEventArgs"/> with the event data. 
    /// </param>
    private void VianaNetApplicationStartup(object sender, StartupEventArgs args)
    {
      this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
      this.MainWindow = new MainWindow.MainWindow();
    }

    #endregion
  }
}
