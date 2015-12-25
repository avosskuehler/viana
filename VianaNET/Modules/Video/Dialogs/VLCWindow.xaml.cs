using System;
using System.Windows;

namespace VianaNET.Modules.Video.Dialogs
{
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Threading;

  using VianaNET.Application;
  using VianaNET.Modules.Video.Control;

  using Vlc.DotNet.Core;
  using Vlc.DotNet.Core.Interops;
  using Vlc.DotNet.Forms;

  using Path = System.IO.Path;

  /// <summary>
  /// Interaction logic for VLCWindow.xaml
  /// </summary>
  public partial class VLCWindow : Window, IDisposable
  {
    private string convertedFile;
    private string videoFile;
    private bool isPlaying;

    public VLCWindow()
    {
      this.InitializeComponent();
    }

    public void Dispose()
    {
      this.vlcConverterPlayer.Dispose();
      GC.Collect();
    }

    public string VideoFile
    {
      get
      {
        return this.videoFile;
      }

      set
      {
        this.videoFile = value;

        //var opts = new[] { "-I dummy", "-vvv", "--sout=#duplicate{dst=display,dst='transcode{vcodec=mp1v,acodec=mpga}:standard{access=file,mux=mpeg1,dst=C://Users//Adrian//Desktop//MyVid.mpg}'" };
        // % vlc -vvv input_stream --sout '#duplicate{dst=display,dst="transcode{vcodec=mp4v,acodec=mpga,vb=800,ab=128,deinterlace}:standard{access=udp,mux=ts,url=239.255.12.42,sap,name="TestStream"}"}'
        // "C://Program Files (x86)//Videolan//VLC//VLC.exe" "c:\Stahlfeder_JanMax.mp4" -I dummy --sout=#transcode{vcodec=mp1v,acodec=mpga}:standard{access=file,mux=mpeg1,dst=C://Users//Adrian//Desktop//MyVid.mpg}
        //this.Player.SetMedia(new Uri(this.videoFile), opts);

        //this.BtnPlayImage.Source = Viana.GetImageSource("Pause16.png");
        this.vlcConverterPlayer.Play(new Uri(this.videoFile));
        //this.isPlaying = true;
        Thread.Sleep(200);
        this.vlcConverterPlayer.Pause();
        //Process vlc;
        //vlc = Process.Start(
        //  "C://Program Files (x86)//Videolan//VLC//VLC.exe",
        //  "'c://Stahlfeder_JanMax.mp4' -I dummy --sout=#transcode{vcodec=mp1v,acodec=mpga}:standard{access=file,mux=mpeg1,dst=C://Users//Adrian//Desktop//MyVid.mpg}");
        //Thread.Sleep(9000);
        //vlc.Kill();


        ////vlc -I dummy -vvv "MyVid.mod"
        ////--sout=#transcode{vcodec=h264,vb=1024,acodec=mp4a,ab=192,channels=2,deinterlace}:standard{access=file,mux=ts,dst=MyVid.mp4
      }
    }

    private void VlcPlayer_LengthChanged(object sender, VlcMediaPlayerLengthChangedEventArgs e)
    {
      this.Dispatcher.InvokeAsync(() => { this.TimelineSlider.Maximum = new TimeSpan((long)e.NewLength).Duration().TotalMilliseconds; });
    }

    private void VlcControl_VlcLibDirectoryNeeded(object sender, VlcLibDirectoryNeededEventArgs e)
    {
      e.VlcLibDirectory = GetVlcLibDirectory();
    }

    private static DirectoryInfo GetVlcLibDirectory()
    {
      var currentAssembly = Assembly.GetEntryAssembly();
      var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
      if (currentDirectory == null)
      {
        return null;
      }
      var returnInfo = new DirectoryInfo(currentDirectory);

      if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
      {
        returnInfo = new DirectoryInfo(
             Path.Combine(currentDirectory, @"C:\Users\Adrian\VSProjects\Vlc.DotNet-master\Vlc.DotNet\lib\x86\"));
      }
      else
      {
        returnInfo = new DirectoryInfo(
            Path.Combine(currentDirectory, @"C:\Users\Adrian\VSProjects\Vlc.DotNet-master\Vlc.DotNet\lib\x64\"));
      }

      if (!returnInfo.Exists)
      {
        var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
        folderBrowserDialog.Description = "Select Vlc libraries folder.";
        folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
        folderBrowserDialog.ShowNewFolderButton = true;
        if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          returnInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
        }
      }
      return returnInfo;
    }

    private void VlcControl_PositionChanged(object sender, VlcMediaPlayerPositionChangedEventArgs e)
    {
      this.Dispatcher.InvokeAsync(
        () =>
        {
          this.TimelineSlider.Value = e.NewPosition * this.vlcConverterPlayer.Length;
          if (e.NewPosition * this.vlcConverterPlayer.Length > this.TimelineSlider.SelectionEnd)
          {
            this.vlcConverterPlayer.Pause();
          }

          this.ConverterProgressbar.Value = e.NewPosition * 100;
        });
    }

    private void BtnPlayClick(object sender, RoutedEventArgs e)
    {
      if (this.isPlaying)
      {
        this.BtnPlayImage.Source = Viana.GetImageSource("Start16.png");
        this.vlcConverterPlayer.Pause();
      }
      else
      {
        this.BtnPlayImage.Source = Viana.GetImageSource("Pause16.png");
        this.vlcConverterPlayer.Play();
      }

      this.isPlaying = !this.isPlaying;
    }

    private void BtnStopClick(object sender, RoutedEventArgs e)
    {
      this.vlcConverterPlayer.Stop();
      this.TimelineSlider.Value = this.TimelineSlider.SelectionStart;
      this.isPlaying = false;
      this.BtnPlayImage.Source = Viana.GetImageSource("Start16.png");
    }

    private void BtnSetCutoutLeftClick(object sender, RoutedEventArgs e)
    {
      this.TimelineSlider.SelectionStart = this.TimelineSlider.Value;
      this.TimelineSlider.UpdateSelectionTimes();
    }

    private void BtnSetCutoutRightClick(object sender, RoutedEventArgs e)
    {
      this.TimelineSlider.SelectionEnd = this.TimelineSlider.Value;
      this.TimelineSlider.UpdateSelectionTimes();
    }

    private void TimelineSliderDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {

    }

    private void TimelineSliderDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
      this.vlcConverterPlayer.Position = (float)(this.TimelineSlider.Value / this.vlcConverterPlayer.Length);
    }

    private void TimelineSliderDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {

    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      var path = Viana.Project.ProjectPath ?? Path.GetDirectoryName(this.videoFile);
      this.convertedFile = Path.Combine(path, Path.GetFileNameWithoutExtension(this.videoFile));
      this.convertedFile += ".mpg";
      var nfi = new NumberFormatInfo();
      nfi.NumberDecimalSeparator = ".";
      var startTimeOption = "start-time=" + (this.TimelineSlider.SelectionStart / 1000f).ToString("N3", nfi);
      var stopTimeOption = "stop-time=" + (this.TimelineSlider.SelectionEnd / 1000f).ToString("N3", nfi);
      var transcodeOption = @"sout=#transcode{vcodec=mp1v,vb=1024,acodec=mpga,ab=192}:standard{access=file,mux=mpeg1,dst=" + this.convertedFile + "}";
      this.ProgressPanel.Visibility = Visibility.Visible;
      this.VideoPanel.Visibility = Visibility.Hidden;
      var opts = new string[] { startTimeOption, stopTimeOption, transcodeOption };
      this.vlcConverterPlayer.Play(new Uri(this.videoFile), opts);
      this.vlcConverterPlayer.EndReached += this.VlcConverterPlayerEndReached;
      this.OK.IsEnabled = false;
      this.Cancel.IsEnabled = false;
    }

    void VlcConverterPlayerEndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
    {
      Viana.Project.VideoFile = this.convertedFile;

      this.Dispatcher.Invoke(
      () =>
      {
        this.TimelineSlider.ResetSelection();
        this.TimelineSlider.UpdateSelectionTimes();

        this.OK.IsEnabled = true;
        this.Close();
      });

    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
