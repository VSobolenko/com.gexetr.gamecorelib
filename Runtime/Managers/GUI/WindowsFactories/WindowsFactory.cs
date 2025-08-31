using System;
using System.Collections.Generic;
using System.Linq;
using Game.AssetContent;
using Game.Factories;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Factories
{
internal sealed class WindowsFactory : IWindowFactory
{
    private readonly IMediatorInstantiator _container;
    private readonly IResourceManager _resourceManager;
    private readonly IFactoryGameObjects _factory;

    private static readonly Dictionary<Type, Type> WindowMediatorMap = new(5);
    private string _uiRootPath = "UI/UIRoot";
    private string _uiTabSwitcher= "UI/TabSwitcher";

    public WindowsFactory(IMediatorInstantiator container, IResourceManager resourceManager, IFactoryGameObjects factory)
    {
        _resourceManager = resourceManager;
        _factory = factory;
        _container = container;

        MapMediatorTypes();
    }

    public WindowsFactory WithParameter(string uIRootPath, string uiTabSwitcherPath)
    {
        _uiRootPath = uIRootPath;
        _uiTabSwitcher = uiTabSwitcherPath;

        return this;
    }
    
    private static void MapMediatorTypes()
    {
        var mediators = AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(s => s.GetTypes())
                                 .Where(p => typeof(IMediator).IsAssignableFrom(p));

        foreach (var mediator in mediators)
        {
            Type windowType = null;
            foreach (var constructor in mediator.GetConstructors())
            {
                windowType = constructor.GetParameters()
                                        .FirstOrDefault(p => p.ParameterType.IsSubclassOf(typeof(WindowUI)))
                                        ?.ParameterType;

                if (windowType != null) 
                    break;
            }

            if (windowType != null)
                WindowMediatorMap.TryAdd(mediator, windowType);
        }
    }

    public bool TryCreateWindowsRoot(Transform root, out Transform uiRoot)
    {
        uiRoot = null;

        var rootPrefab = _resourceManager.LoadAsset<GameObject>(_uiRootPath);

        if (rootPrefab == null)
            throw new ArgumentNullException(_uiRootPath, $"Can't load UI prefab by path: {_uiRootPath}");

        uiRoot = _factory.InstantiatePrefab(rootPrefab, root).transform;

        return uiRoot != null;
    }

    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator,
        out WindowUI window) where TMediator : class, IMediator
    {
        mediator = null;
        window = null;
        var mediatorType = typeof(TMediator);

        if (WindowMediatorMap.TryGetValue(mediatorType, out var windowType) == false)
            throw new ArgumentNullException(mediatorType.Name, $"For {mediatorType} Window type not found");

        var prefabKey = $"UI/{mediatorType.Name.Replace("MediatorUI", "")}";
        var prefab = _resourceManager.LoadAsset<GameObject>(prefabKey);

        if (prefab == null)
            throw new ArgumentNullException(prefabKey, $"Can't load Prefab by path: {prefabKey}");

        if (prefab.GetComponent(windowType) == null)
            throw new ArgumentNullException(windowType.Name, $"Can't find \"{windowType}\" component in {prefab.gameObject}.");

        window = _factory.InstantiatePrefab(prefab, root).GetComponent<WindowUI>();
        SetTransformValuesFromPrefab(window, prefab);
        mediator = _container.Instantiate<TMediator>(window);

        return mediator != null;
    }

    public bool TryCreateWindow(Type mediatorType, Transform root, out IMediator mediator, out WindowUI window)
    {
        mediator = null;
        window = null;

        if (WindowMediatorMap.TryGetValue(mediatorType, out var windowType) == false)
            throw new ArgumentNullException(mediatorType.Name, $"For {mediatorType} Window type not found");

        var prefabKey = $"UI/{mediatorType.Name.Replace("MediatorUI", "")}";
        var prefab = _resourceManager.LoadAsset<GameObject>(prefabKey);

        if (prefab == null)
            throw new ArgumentNullException(prefabKey, $"Can't load UI prefab by path: {prefabKey}");

        if (prefab.GetComponent(windowType) == null)
            throw new ArgumentNullException(windowType.Name, $"Can't find \"{windowType}\" component in {prefab.gameObject}");

        window = _factory.InstantiatePrefab(prefab, root).GetComponent<WindowUI>();
        SetTransformValuesFromPrefab(window, prefab);
        mediator = _container.Instantiate(mediatorType, window);

        return mediator != null;
    }

    public bool TryCreateTabSwitcher<T>(Transform root, out ITabSwitcher<T> switcher) where T : struct, Enum
    {
        switcher = null;

        var prefab = _resourceManager.LoadAsset<GameObject>(_uiTabSwitcher);

        if (prefab == null)
            throw new ArgumentNullException(_uiTabSwitcher, $"Can't load UI prefab by path: {_uiTabSwitcher}");

        if (prefab.GetComponent<ITabSwitcher<T>>() == null)
            throw new ArgumentNullException(prefab.name, $"Can't find \"{typeof(ITabSwitcher<T>).Name}\" component in {prefab.gameObject}");
        
        switcher = _factory.InstantiatePrefab(prefab, root).GetComponent<ITabSwitcher<T>>();

        return switcher != null;
    }

    private static void SetTransformValuesFromPrefab(Component window, GameObject prefab)
    {
        var transform = window.transform;
        transform.SetLocalPositionAndRotation(prefab.transform.localPosition, prefab.transform.localRotation);
        transform.localScale = prefab.transform.localScale;
    }
}
}