using Game.IO.Managers;

namespace Game.IO
{
public static class SaveSystemInstaller
{
    public static ISaveFile FileSaver() => new BinarySave();
}
}