using Game.AssetContent.Managers;
using Game.Factories;

namespace Game.AssetContent
{
public static class ResourceManagerInstaller
{
    public static IResourceManager Addressable() => new AddressablesManager();
    public static IResourceManager Resources() => new ResourceManager();
    public static IResourceFactoryManager Factory(
        IResourceManager resourceManager, 
        IFactoryGameObjects factoryGameObjects
        ) => new ResourceManagerFactory(resourceManager, factoryGameObjects);
}
}