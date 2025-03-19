using Game.Factories.Managers;

namespace Game.Factories
{
public static class FactoryInstaller
{
    public static IFactoryGameObjects Standart() => new StandardObjectsFactory();
}
}