using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VianaNETShaderEffectLibrary
{
  public class CropEffect : ShaderEffect
  {
    private static PixelShader _pixelShader = new PixelShader();

    #region Constructors

    static CropEffect()
    {
      _pixelShader.UriSource = Global.MakePackUri("CropEffect.ps");
    }

    public CropEffect()
    {
      this.PixelShader = _pixelShader;

      // Update each DependencyProperty that's registered with a shader register.  This
      // is needed to ensure the shader gets sent the proper default value.
      UpdateShaderValue(InputProperty);
      UpdateShaderValue(BlankColorProperty);
      UpdateShaderValue(MinXProperty);
      UpdateShaderValue(MaxXProperty);
      UpdateShaderValue(MinYProperty);
      UpdateShaderValue(MaxYProperty);
    }

    #endregion

    public Brush Input
    {
      get { return (Brush)GetValue(InputProperty); }
      set { SetValue(InputProperty, value); }
    }

    public static readonly DependencyProperty InputProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(CropEffect), 0);

    public Color BlankColor
    {
      get { return (Color)GetValue(BlankColorProperty); }
      set { SetValue(BlankColorProperty, value); }
    }

    public static readonly DependencyProperty BlankColorProperty =
        DependencyProperty.Register("BlankColor", typeof(Color), typeof(CropEffect),
                new UIPropertyMetadata(Colors.Black, PixelShaderConstantCallback(0)));

    public float MinX
    {
      get { return (float)GetValue(MinXProperty); }
      set { SetValue(MinXProperty, value); }
    }

    public static readonly DependencyProperty MinXProperty =
        DependencyProperty.Register("MinX", typeof(float), typeof(CropEffect),
                new UIPropertyMetadata(0, PixelShaderConstantCallback(1)));

    public float MaxX
    {
      get { return (float)GetValue(MaxXProperty); }
      set { SetValue(MaxXProperty, value); }
    }

    public static readonly DependencyProperty MaxXProperty =
        DependencyProperty.Register("MaxX", typeof(float), typeof(CropEffect),
                new UIPropertyMetadata(0, PixelShaderConstantCallback(2)));

    public float MinY
    {
      get { return (float)GetValue(MinYProperty); }
      set { SetValue(MinYProperty, value); }
    }

    public static readonly DependencyProperty MinYProperty =
        DependencyProperty.Register("MinY", typeof(float), typeof(CropEffect),
                new UIPropertyMetadata(0, PixelShaderConstantCallback(3)));

    public float MaxY
    {
      get { return (float)GetValue(MaxYProperty); }
      set { SetValue(MaxYProperty, value); }
    }

    public static readonly DependencyProperty MaxYProperty =
        DependencyProperty.Register("MaxY", typeof(float), typeof(CropEffect),
                new UIPropertyMetadata(0, PixelShaderConstantCallback(4)));
  }
}
