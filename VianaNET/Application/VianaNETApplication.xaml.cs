using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Globalization;
using WPFLocalizeExtension.Engine;
using System.Threading;

namespace VianaNET
{
  public partial class VianaNETApplication : Application
  {
    private void VianaNETApplication_Startup(object sender, StartupEventArgs args)
    {
      this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("de");
      this.MainWindow = new MainWindow();
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      Exception ex = e.Exception;
      string message = ex.Message;

      while (ex.InnerException != null)
      {
        ex = ex.InnerException;
        message = ex.Message + Environment.NewLine + message;
      }

      MessageBox.Show(message);
    }
    
    public static void DoEvents()
    {
      if (Application.Current != null)
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
    }
  }
}