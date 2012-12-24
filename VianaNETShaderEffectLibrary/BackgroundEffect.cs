using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VianaNETShaderEffectLibrary
{
  public class BackgroundEffect : ShaderEffect
  {
    private static PixelShader _pixelShader = new PixelShader();

    #region Constructors

    static BackgroundEffect()
    {
      _pixelShader.UriSource = Global.MakePackUri("BackgroundEffect.ps");
    }

    public BackgroundEffect()
    {
      this.PixelShader = _pixelShader;

      // Update each DependencyProperty that's registered with a shader register.  This
      // is needed to ensure the shader gets sent the proper default value.
      UpdateShaderValue(InputProperty);
      UpdateShaderValue(ThresholdProperty);
    }

    #endregion

    public Brush Input
    {
      get { return (Brush)GetValue(InputProperty); }
      set { SetValue(InputProperty, value); }
    }

    public static readonly DependencyProperty InputProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BackgroundEffect), 0);

    public double Threshold
    {
      get { return (double)GetValue(ThresholdProperty); }
      set { SetValue(ThresholdProperty, value); }
    }

    public static readonly DependencyProperty ThresholdProperty =
        DependencyProperty.Register("Threshold", typeof(double), typeof(BackgroundEffect),
                new UIPropertyMetadata(0.5, PixelShaderConstantCallback(0)));
  }
}
