using Game.AssetContent.Managers;

namespace Game.AssetContent.Installers
{
public static class ResourceManagerInstaller
{
    public static IResourceManager Addressable() => new AddressablesManager();
    public static IResourceManager Resources() => new ResourceManager();
}
}