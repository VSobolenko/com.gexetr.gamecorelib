using Game.IO.Managers;

namespace Game.IO.Installers
{
public static class SaveSystemInstaller
{
    public static ISaveFile FileSaver() => new BinarySave();
}
}