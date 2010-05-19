#region Using

using System;
using System.Windows.Media; 

#endregion

namespace VianaNET
{
    #region CheckBoxPallet

    public static class CheckBoxPallet
    {
        #region Declare

        public static Color Foreground;
        public static Color NormalBorder;
        public static Color Normalbackground;
        public static Color MouseOverBorder;

        public static Color InternalNormalBorder;
        
        public static Color HightLightInternalBorder;

        public static Color InternalNormalBackgroung1;
        public static Color InternalNormalBackgroung2;

        public static Color InternalHighLightBackgroung1;
        public static Color InternalHighLightBackgroung2;

        public static Color DisabledForeground;
        public static Color DisabledBorder;
        public static Color DisabledInternalBorder;
        
        #endregion

        #region Constructor

        static CheckBoxPallet()
        {
            CheckBoxPallet.Reset();
            OfficeColors.RegistersTypes.Add(typeof(CheckBoxPallet));
        }

        #endregion

        #region Reset

        public static void Reset()
        {
            Foreground = OfficeColors.Foreground.OfficeColor4;
            NormalBorder = OfficeColors.Background.OfficeColor6;
            Normalbackground = OfficeColors.Background.OfficeColor85;
            MouseOverBorder = OfficeColors.Background.OfficeColor6;

            InternalNormalBorder = OfficeColors.Background.OfficeColor83;
            
            HightLightInternalBorder = OfficeColors.HighLight.OfficeColor12;
                        
            InternalNormalBackgroung1 = OfficeColors.Background.OfficeColor84;
            InternalNormalBackgroung2 = OfficeColors.Background.OfficeColor85;
            
            InternalHighLightBackgroung1 = OfficeColors.HighLight.OfficeColor15;
            InternalHighLightBackgroung2 = OfficeColors.HighLight.OfficeColor15;
            InternalHighLightBackgroung2.A = 0;
            
            DisabledForeground = OfficeColors.Disabled.OfficeColor3;
            DisabledBorder = OfficeColors.Disabled.OfficeColor4;
            DisabledInternalBorder = OfficeColors.Disabled.OfficeColor1;
        }

        #endregion
    } 

    #endregion
}
