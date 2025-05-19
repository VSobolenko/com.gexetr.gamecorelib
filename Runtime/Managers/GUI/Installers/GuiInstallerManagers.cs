using Game.AssetContent;
using Game.Factories;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using Game.GUI.Windows.Managers;
using Game.GUI.Windows.Transitions;
using UnityEngine;

namespace Game.GUI
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
        ManagerAsync(windowFactory, transition, transition, rootUi);
    
    public static IWindowsManagerAsync ManagerAsync(IWindowFactory windowFactory,
        IWindowTransition openTransition,
        IWindowTransition closeTransition,
        Transform rootUi = null) =>
        new WindowsManagerAsync(windowFactory, rootUi, _settings, openTransition, closeTransition);

    public static IWindowFactory WindowFactory(IMediatorInstantiator mediatorBuilder,
        IResourceManager resourceManager,
        IFactoryGameObjects factory) =>
        new WindowsFactory(mediatorBuilder, resourceManager, factory);
    
    public static IWindowFactory WindowFactory(IMediatorInstantiator mediatorBuilder,
        IResourceManager resourceManager,
        IFactoryGameObjects factory,
        string uiRootPath,
        string uiTabSwitcherPath) =>
        new WindowsFactory(mediatorBuilder, resourceManager, factory)
            .WithParameter(uiRootPath, uiTabSwitcherPath);
}
}