using Game.AssetContent;
using Game.Factories;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using Game.GUI.Windows.Managers;
using Game.GUI.Windows.Transitions;
using UnityEngine;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    public static IWindowsManagerAsync Manager(IWindowFactory windowFactory,
                                               IWindowTransition transition,
                                               Transform rootUi = null) =>
        ManagerAsync(windowFactory, transition, rootUi);

    public static IWindowsManagerAsync ManagerAsync(IWindowFactory windowFactory,
                                                    IWindowTransition transition,
                                                    Transform rootUi = null) =>
        new WindowsManagerAsync(windowFactory, rootUi, WindowSettings, transition);

    public static IWindowFactory WindowFactory(IMediatorInstantiator mediatorBuilder,
                                               IResourceManagement resourceManagement,
                                               IFactoryGameObjects factory) =>
        new WindowsFactory(mediatorBuilder, resourceManagement, factory);
    
    public static IWindowFactory WindowFactory(IMediatorInstantiator mediatorBuilder,
                                               IResourceManagement resourceManagement,
                                               IFactoryGameObjects factory,
                                               string uiRootPath) =>
        new WindowsFactory(mediatorBuilder, resourceManagement, factory)
            .WithParameter(uiRootPath);
}
}