namespace VianaNET
{
    using System.Collections.Generic;
    using DirectShowLib;

    public class CameraInfo
    {
        public string Name { get; set; }
        public DsDevice DirectshowDevice { get; set; }
        public List<CamSizeFPS> SupportedSizesAndFPS { get; set; }

        public CameraInfo()
        {
            this.SupportedSizesAndFPS = new List<CamSizeFPS>();
        }

        public bool IsValidCamera
        {
            get
            {
                if (this.DirectshowDevice == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}