#region Using

using System;
using System.Windows.Media;

#endregion

namespace VianaNET
{
  #region ScrollViewerPallet

  public static class ScrollViewerPallet
  {
    #region Declare

    public static Color NormalBorderBrush;

    #endregion

    #region Constructor

    static ScrollViewerPallet()
    {
      ScrollViewerPallet.Reset();
      OfficeColors.RegistersTypes.Add(typeof(ScrollViewerPallet));
    }

    #endregion

    #region Reset

    public static void Reset()
    {
      NormalBorderBrush = OfficeColors.Background.OfficeColor82;
    }

    #endregion
  }

  #endregion
}
