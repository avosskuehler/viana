namespace VianaNET.CustomStyles.Types
{
  public class CameraDevice
  {
    public CameraDevice(int index, string name)
    {
      this.Name = name;
      this.Index = index;
    }

    public string Name { get; private set; }

    public int Index { get; private set; }
  }
}
