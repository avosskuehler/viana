using System;
using System.Windows.Media;
using System.Windows;

namespace VianaNET
{
  public static class Gradients
  {
    public enum Name
    {
      TrafficLight,
      Rainbow,
      Spectrum,
      Mixed,
      Red,
    }

    public static LinearGradientBrush GetByName(Name gradient)
    {
      switch (gradient)
      {
        case Name.TrafficLight:
          // Create a horizontal linear gradient with four stops.   
          LinearGradientBrush newTrafficLight = new LinearGradientBrush();
          newTrafficLight.StartPoint = new Point(0, 0.5);
          newTrafficLight.EndPoint = new Point(1, 0.5);
          newTrafficLight.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
          newTrafficLight.GradientStops.Add(new GradientStop(Colors.Yellow, 0.5));
          newTrafficLight.GradientStops.Add(new GradientStop(Colors.Green, 1));
          return newTrafficLight;
        case Name.Rainbow:
          // Create a horizontal linear gradient with four stops.   
          LinearGradientBrush newRainbow = new LinearGradientBrush();
          newRainbow.StartPoint = new Point(0, 0.5);
          newRainbow.EndPoint = new Point(1, 0.5);
          newRainbow.GradientStops.Add(new GradientStop(Color.FromArgb(0,255,0,0), 0.0));
          newRainbow.GradientStops.Add(new GradientStop(Colors.Red, 0.1));
          newRainbow.GradientStops.Add(new GradientStop(Colors.Yellow, 0.28));
          newRainbow.GradientStops.Add(new GradientStop(Color.FromRgb(1,180,57), 0.45));
          newRainbow.GradientStops.Add(new GradientStop(Color.FromRgb(0, 234, 255), 0.6));
          newRainbow.GradientStops.Add(new GradientStop(Color.FromRgb(0, 3, 144), 0.75));
          newRainbow.GradientStops.Add(new GradientStop(Color.FromRgb(255, 0, 198), 0.88));
          newRainbow.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 0, 0), 1.0));

          return newRainbow;
        case Name.Mixed:
          // Create a horizontal linear gradient with four stops.   
          LinearGradientBrush newMixed = new LinearGradientBrush();
          newMixed.StartPoint = new Point(0, 0.5);
          newMixed.EndPoint = new Point(1, 0.5);
          newMixed.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
          newMixed.GradientStops.Add(new GradientStop(Colors.Red, 0.2));
          newMixed.GradientStops.Add(new GradientStop(Colors.Blue, 0.35));
          newMixed.GradientStops.Add(new GradientStop(Colors.LimeGreen, 0.50));
          newMixed.GradientStops.Add(new GradientStop(Colors.Blue, 0.65));
          newMixed.GradientStops.Add(new GradientStop(Colors.Red, 0.80));
          newMixed.GradientStops.Add(new GradientStop(Colors.Yellow, 1.0));

          return newMixed;
        case Name.Spectrum:
          // Create a horizontal linear gradient with four stops.   
          LinearGradientBrush newSpectrum = new LinearGradientBrush();
          newSpectrum.StartPoint = new Point(0, 0.5);
          newSpectrum.EndPoint = new Point(1, 0.5);
          newSpectrum.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
          newSpectrum.GradientStops.Add(new GradientStop(Colors.Magenta, 0.15));
          newSpectrum.GradientStops.Add(new GradientStop(Colors.Blue, 0.33));
          newSpectrum.GradientStops.Add(new GradientStop(Colors.Cyan, 0.49));
          newSpectrum.GradientStops.Add(new GradientStop(Colors.Yellow, 0.84));
          newSpectrum.GradientStops.Add(new GradientStop(Colors.Red, 1.0));

          return newSpectrum;
        case Name.Red:
          // Create a horizontal linear gradient with four stops.   
          LinearGradientBrush newRed = new LinearGradientBrush();
          newRed.StartPoint = new Point(0, 0.5);
          newRed.EndPoint = new Point(1, 0.5);
          newRed.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
          newRed.GradientStops.Add(new GradientStop(Color.FromRgb(255, 131, 131), 0.7));
          newRed.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 131, 131), 1));

          return newRed;
      }

      return new LinearGradientBrush(Colors.Red, Colors.Blue, 0);
    }
  }
}
