namespace VianaNET
{
  using System.Windows.Media;

  public static class SliderStyles
  {
    public static Color Glyph;
    public static Color NormalBorder;
    public static Color NormalBackground;

    public static Color SideButtonsExternalBorder;
    public static Color SideButtonsInternalBorder;

    public static Color SideButtonsBackground1;
    public static Color SideButtonsBackground2;
    public static Color SideButtonsBackground3;
    public static Color SideButtonsBackground4;

    public static Color ThumbBackground1;
    public static Color ThumbBackground2;
    public static Color ThumbBackground3;
    public static Color ThumbBackground4;

    public static Color SideButtonsLightBackground1;
    public static Color SideButtonsLightBackground2;
    public static Color SideButtonsLightBackground3;
    public static Color SideButtonsLightBackground4;

    public static Color SideButtonsPlusLightBackground1;
    public static Color SideButtonsPlusLightBackground2;
    public static Color SideButtonsPlusLightBackground3;
    public static Color SideButtonsPlusLightBackground4;

    static SliderStyles()
    {
      SliderStyles.Reset();
      OfficeColors.RegistersTypes.Add(typeof(SliderStyles));
    }

    public static void Reset()
    {
      Glyph = OfficeColors.Foreground.OfficeColor4;
      NormalBorder = OfficeColors.Background.OfficeColor6;
      NormalBackground = OfficeColors.Background.OfficeColor5;

      SideButtonsExternalBorder = OfficeColors.Background.OfficeColor42;
      SideButtonsInternalBorder = OfficeColors.Background.OfficeColor73;

      SideButtonsBackground1 = OfficeColors.Background.OfficeColor85;
      SideButtonsBackground2 = OfficeColors.Background.OfficeColor74;
      SideButtonsBackground3 = OfficeColors.Background.OfficeColor53;
      SideButtonsBackground4 = OfficeColors.Background.OfficeColor85;

      ThumbBackground1 = OfficeColors.Background.OfficeColor85;
      ThumbBackground2 = OfficeColors.Background.OfficeColor66;
      ThumbBackground3 = OfficeColors.Background.OfficeColor65;
      ThumbBackground4 = OfficeColors.Background.OfficeColor85;

      SideButtonsLightBackground1 = OfficeColors.Background.OfficeColor85;
      SideButtonsLightBackground2 = OfficeColors.HighLight.OfficeColor10;
      SideButtonsLightBackground3 = OfficeColors.HighLight.OfficeColor5;
      SideButtonsLightBackground4 = OfficeColors.Background.OfficeColor85;

      SideButtonsPlusLightBackground1 = OfficeColors.Background.OfficeColor85;
      SideButtonsPlusLightBackground2 = OfficeColors.HighLight.OfficeColor11;
      SideButtonsPlusLightBackground3 = OfficeColors.HighLight.OfficeColor10;
      SideButtonsPlusLightBackground4 = OfficeColors.Background.OfficeColor85;
    }

  }
}
