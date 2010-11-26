#region Using

using System;
using System.Windows.Media; 

#endregion

namespace VianaNET
{
    #region RadioButtonPallet

    public static class RadioButtonPallet
    {
        #region Declare

        public static Color NormalBorder;
        public static Color Glyph;

        public static Color NormalBackground1;
        public static Color NormalBackground2;

        public static Color LightBackground1;
        public static Color LightBackground2;

        public static Color PlusLightBackground1;
        public static Color PlusLightBackground2; 
        
        public static Color DisabledBorder;
        public static Color DisabledBackground;
        public static Color DisabledForeground;

        #endregion

        #region Constructor

        static RadioButtonPallet()
        {
            RadioButtonPallet.Reset();
            OfficeColors.RegistersTypes.Add(typeof(RadioButtonPallet));
        }

        #endregion

        #region Reset

        public static void Reset()
        {
            NormalBorder = OfficeColors.Background.OfficeColor82;
            Glyph = OfficeColors.Foreground.OfficeColor3;

            NormalBackground1 = OfficeColors.Background.OfficeColor84;
            NormalBackground2 = OfficeColors.Background.OfficeColor85;

            LightBackground1 = OfficeColors.HighLight.OfficeColor12;
            LightBackground2 = OfficeColors.Background.OfficeColor85;

            PlusLightBackground1 = OfficeColors.HighLight.OfficeColor11;
            PlusLightBackground2 = OfficeColors.Background.OfficeColor85; 
        
            DisabledBorder = OfficeColors.Disabled.OfficeColor3;
            DisabledBackground = OfficeColors.Disabled.OfficeColor2;
            DisabledForeground = OfficeColors.Disabled.OfficeColor3;
        }

        #endregion
    } 

    #endregion
}
