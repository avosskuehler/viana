#region Using

using System;
using System.Windows.Media; 

#endregion

namespace VianaNET
{
    #region ScrollBarPallet

    public static class ScrollBarPallet
    {
        #region Declare

        public static Color ScrollBarNormalBorderBrush;

        public static Color ScrollBarBg1;
        public static Color ScrollBarBg2;

        public static Color ScrollBarBtnBgMouseOver1;
        public static Color ScrollBarBtnBgMouseOver2;
        public static Color ScrollBarBtnBgMouseOver3;
        public static Color ScrollBarBtnBgMouseOver4;

        public static Color ScrollBarBtnBgPressed1;
        public static Color ScrollBarBtnBgPressed2;
        public static Color ScrollBarBtnBgPressed3;
        public static Color ScrollBarBtnBgPressed4;

        public static Color  ScrollBarThumbBackground1;
        public static Color  ScrollBarThumbBackground2;
        public static Color  ScrollBarThumbBackground3;
        public static Color  ScrollBarThumbBackground4;
                        
        public static Color ScrollBarThumbOverBrush1;
        public static Color ScrollBarThumbOverBrush2;
        public static Color ScrollBarThumbOverBrush3;
        public static Color ScrollBarThumbOverBrush4;

        public static Color ScrollBarThumbPressedBrush1;
        public static Color ScrollBarThumbPressedBrush2;
        public static Color ScrollBarThumbPressedBrush3;
        public static Color ScrollBarThumbPressedBrush4;
    
        #endregion

        #region Constructor

        static ScrollBarPallet()
        {
            ScrollBarPallet.Reset();
            OfficeColors.RegistersTypes.Add(typeof(ScrollBarPallet));
        }

        #endregion

        #region Reset

        public static void Reset()
        {
            ScrollBarNormalBorderBrush = OfficeColors.Background.OfficeColor73;

            ScrollBarBg1 = OfficeColors.Background.OfficeColor58;
            ScrollBarBg2 = OfficeColors.Background.OfficeColor59;

            ScrollBarBtnBgMouseOver1 = OfficeColors.Background.OfficeColor60;
            ScrollBarBtnBgMouseOver2 = OfficeColors.Background.OfficeColor61;
            ScrollBarBtnBgMouseOver3 = OfficeColors.Background.OfficeColor62;
            ScrollBarBtnBgMouseOver4 = OfficeColors.Background.OfficeColor63;

            ScrollBarBtnBgPressed1 = OfficeColors.Background.OfficeColor64;
            ScrollBarBtnBgPressed2 = OfficeColors.Background.OfficeColor65;
            ScrollBarBtnBgPressed3 = OfficeColors.Background.OfficeColor66;

            ScrollBarBtnBgPressed4 = OfficeColors.Background.OfficeColor67;

            ScrollBarThumbBackground1 = OfficeColors.Background.OfficeColor1;
            ScrollBarThumbBackground2 = OfficeColors.Background.OfficeColor2;
            ScrollBarThumbBackground3 = OfficeColors.Background.OfficeColor3;
            ScrollBarThumbBackground4 = OfficeColors.Background.OfficeColor4;

            ScrollBarThumbOverBrush1 = OfficeColors.Background.OfficeColor74;
            ScrollBarThumbOverBrush2 = OfficeColors.Background.OfficeColor75;
            ScrollBarThumbOverBrush3 = OfficeColors.Background.OfficeColor76;
            ScrollBarThumbOverBrush4 = OfficeColors.Background.OfficeColor77;

            ScrollBarThumbPressedBrush1 = OfficeColors.Background.OfficeColor78;
            ScrollBarThumbPressedBrush2 = OfficeColors.Background.OfficeColor79;
            ScrollBarThumbPressedBrush3 = OfficeColors.Background.OfficeColor80;
            ScrollBarThumbPressedBrush4 = OfficeColors.Background.OfficeColor81;
        }

        #endregion
    } 

    #endregion
}
