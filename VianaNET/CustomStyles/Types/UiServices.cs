namespace VianaNET.CustomStyles.Types
{
  using System;
  using System.Windows.Input;
  using System.Windows.Threading;

  /// <summary>
  ///   Contains helper methods for UI, so far just one for showing a waitcursor
  /// </summary>
  public static class UiServices
  {

    /// <summary>
    ///   A value indicating whether the UI is currently busy
    /// </summary>
    private static bool IsBusy;

    /// <summary>
    /// Sets the busystate as busy.
    /// </summary>
    public static void SetBusyState()
    {
      SetBusyState(true);
    }

    /// <summary>
    /// Sets the busystate to busy or not busy.
    /// </summary>
    /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
    private static void SetBusyState(bool busy)
    {
      if (busy != IsBusy)
      {
        IsBusy = busy;
        Mouse.OverrideCursor = busy ? Cursors.Wait : null;

        if (IsBusy)
        {
          new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, DispatcherTimer_Tick, App.Current.Dispatcher);
        }
      }
    }

    /// <summary>
    /// Arbeitet die momentane Arbeitsschleife ab.
    /// </summary>
    public static void WaitUntilReady()
    {
      App.Current.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
    }

    /// <summary>
    /// Handles the Tick event of the dispatcherTimer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private static void DispatcherTimer_Tick(object sender, EventArgs e)
    {
      if (sender is DispatcherTimer dispatcherTimer)
      {
        SetBusyState(false);
        dispatcherTimer.Stop();
      }
    }
  }
}
