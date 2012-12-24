namespace VianaNET
{
    using System;

    public class CamSizeFPS : IComparable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int FPS { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is CamSizeFPS)
            {
                CamSizeFPS csf = (CamSizeFPS)obj;
                return (csf.Height + csf.Width + csf.FPS).CompareTo(this.Height + this.Width + this.FPS);
            }
            else
            {
                throw new ArgumentException("Object is not a CamSizeFPS");
            }
        }
    }
}