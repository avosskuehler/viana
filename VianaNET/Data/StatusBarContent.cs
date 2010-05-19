using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLocalizeExtension.Extensions;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace VianaNET
{
  public class StatusBarContent : DependencyObject
  {
    /// <summary>
    /// Holds the instance of singleton
    /// </summary>
    private static StatusBarContent instance;

    /// <summary>
    /// Gets the <see cref="Calibration"/> singleton.
    /// If the underlying instance is null, a instance will be created.
    /// </summary>
    public static StatusBarContent Instance
    {
      get
      {
        // check again, if the underlying instance is null
        if (instance == null)
        {
          // create a new instance
          instance = new StatusBarContent();
        }

        // return the existing/new instance
        return instance;
      }
    }

    private StatusBarContent()
    {
      //LocTextExtension ready = new LocTextExtension("VianaNET:Labels:StatusBarReady");
      //ready.SetBinding(this, StatusBarContent.StatusLabelProperty);
    }

    public string StatusLabel
    {
      get { return (string)this.GetValue(StatusLabelProperty); }
      set { this.SetValue(StatusLabelProperty, value); }
    }

    public static readonly DependencyProperty StatusLabelProperty = DependencyProperty.Register(
      "StatusLabel",
      typeof(string),
      typeof(StatusBarContent),
      new FrameworkPropertyMetadata("Ready"));

    public string MessagesLabel
    {
      get { return (string)this.GetValue(MessagesLabelProperty); }
      set { this.SetValue(MessagesLabelProperty, value); }
    }

    public static readonly DependencyProperty MessagesLabelProperty = DependencyProperty.Register(
      "MessagesLabel",
      typeof(string),
      typeof(StatusBarContent),
      new FrameworkPropertyMetadata(string.Empty));

    public double ProgressBarValue
    {
      get { return (double)this.GetValue(ProgressBarValueProperty); }
      set { this.SetValue(ProgressBarValueProperty, value); }
    }

    public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register(
      "ProgressBarValue",
      typeof(double),
      typeof(StatusBarContent),
      new FrameworkPropertyMetadata(default(double)));
  }
}
