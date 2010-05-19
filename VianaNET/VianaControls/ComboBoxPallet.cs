#region Using

using System;
using System.Windows.Media; 

#endregion

namespace VianaNET
{
    #region ComboBoxPallet

    public static class ComboBoxPallet
    {
        #region Declare

        public static Color Foreground;
        public static Color NormalBorder;
        public static Color Glyph;
        public static Color LightBorder;
        public static Color PlusLightBorder;
        public static Color DisabledBorder;
        public static Color DisabledForeground;
        public static Color EditableControlBackground;

        public static Color NormalBackGround1;
        public static Color NormalBackGround2;
        public static Color NormalBackGround3;
        public static Color NormalBackGround4;

        public static Color DefaultControlMouseOver1;
        public static Color DefaultControlMouseOver2;
        public static Color DefaultControlMouseOver3;
        public static Color DefaultControlMouseOver4;

        public static Color DefaultControlPressed1;
        public static Color DefaultControlPressed2;
        public static Color DefaultControlPressed3;
        public static Color DefaultControlPressed4;
        public static Color DefaultControlPressed5;

        public static Color DisableBackGround1;
        public static Color DisableBackGround2;

        #endregion

        #region Constructor

        static ComboBoxPallet()
        {
            ComboBoxPallet.Reset();
            OfficeColors.RegistersTypes.Add(typeof(ComboBoxPallet));
        }

        #endregion

        #region Reset

        public static void Reset()
        {
            Foreground = OfficeColors.Foreground.OfficeColor1;
            NormalBorder = OfficeColors.Background.OfficeColor82;
            Glyph = OfficeColors.Foreground.OfficeColor3;
            EditableControlBackground = OfficeColors.EditableControlsBackground.OfficeColor1;

            LightBorder = OfficeColors.HighLight.OfficeColor20;
            PlusLightBorder = OfficeColors.HighLight.OfficeColor21;
            DisabledBorder = OfficeColors.Disabled.OfficeColor3;
            DisabledForeground = OfficeColors.Disabled.OfficeColor4;
            
		    NormalBackGround1 = OfficeColors.Background.OfficeColor1;
            NormalBackGround2 = OfficeColors.Background.OfficeColor2;
            NormalBackGround3 = OfficeColors.Background.OfficeColor3;
            NormalBackGround4 = OfficeColors.Background.OfficeColor4;
        
		    DefaultControlMouseOver1 = OfficeColors.HighLight.OfficeColor3;
            DefaultControlMouseOver2 = OfficeColors.HighLight.OfficeColor4;
            DefaultControlMouseOver3 = OfficeColors.HighLight.OfficeColor5;
            DefaultControlMouseOver4 = OfficeColors.HighLight.OfficeColor6;

            DefaultControlPressed1 = OfficeColors.HighLight.OfficeColor8;
            DefaultControlPressed2 = OfficeColors.HighLight.OfficeColor9;
            DefaultControlPressed3 = OfficeColors.HighLight.OfficeColor10;
            DefaultControlPressed4 = OfficeColors.HighLight.OfficeColor11;
            DefaultControlPressed5 = OfficeColors.HighLight.OfficeColor12;
        
            DisableBackGround1 = OfficeColors.Disabled.OfficeColor1;
            DisableBackGround2 = OfficeColors.Disabled.OfficeColor2;
        }

        #endregion
    } 

    #endregion
}
