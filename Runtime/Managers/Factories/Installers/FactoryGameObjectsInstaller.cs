using Game.Factories.Managers;

namespace Game.Factories
{
public static class FactoryGameObjectsInstaller
{
    public static IFactoryGameObjects Standart() => new StandardObjectsFactory();
}
}