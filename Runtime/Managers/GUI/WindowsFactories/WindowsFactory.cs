using System;
using System.Collections.Generic;
using System.Linq;
using Game.AssetContent;
using Game.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Factories
{
internal class WindowsFactory : IWindowFactory
{
    private readonly IMediatorInstantiator _container;
    private readonly IResourceManager _resourceManager;
    private readonly IFactoryGameObjects _factory;

    private static readonly Dictionary<Type, Type> WindowMediatorMap = new(5);
    private string _uiRootPath = "UI/UIRoot";

    public WindowsFactory(IMediatorInstantiator container, IResourceManager resourceManager, IFactoryGameObjects factory)
    {
        _resourceManager = resourceManager;
        _factory = factory;
        _container = container;

        MapMediatorTypes();
    }

    public WindowsFactory WithParameter(string uIRootPath)
    {
        _uiRootPath = uIRootPath;

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

        if (Application.isEditor && rootPrefab == null)
            throw new ArgumentNullException(_uiRootPath, $"Can't load UI Root by path: {_uiRootPath}");

        if (rootPrefab == null)
            return false;

        uiRoot = _factory.InstantiatePrefab(rootPrefab, root).transform;

        return uiRoot != null;
    }

    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out WindowUI window)
        where TMediator : class, IMediator
    {
        mediator = null;
        window = null;
        var mediatorType = typeof(TMediator);

        if (WindowMediatorMap.TryGetValue(mediatorType, out var windowType) == false)
        {
            var errorMessage = $"For {mediatorType} Window type not found";
            if (Application.isEditor)
                throw new ArgumentNullException(mediatorType.Name, errorMessage);
            
            Log.Errored(errorMessage);

            return false;
        }

        var prefabKey = $"UI/{mediatorType.Name.Replace("MediatorUI", "")}";
        var prefab = _resourceManager.LoadAsset<GameObject>(prefabKey);

        if (Application.isEditor && prefab == null)
            throw new ArgumentNullException(prefabKey, $"Can't load Prefab by path: {prefabKey}");
        
        if (prefab == null)
            return false;

        if (prefab.GetComponent(windowType) == null)
        {
            var errorMessage = $"Can't find \"{windowType}\" component in {prefab.gameObject}.";
            if (Application.isEditor)
                throw new ArgumentNullException(windowType.Name, errorMessage);
            Log.Errored(errorMessage);

            return false;
        }

        window = _factory.InstantiatePrefab(prefab, root).GetComponent<WindowUI>();
        SetTransformValuesFromPrefab(window, prefab);
        mediator = _container.Instantiate<TMediator>(window);

        return mediator != null;
    }

    private static void SetTransformValuesFromPrefab(Component window, GameObject prefab)
    {
        var transform = window.transform;
        transform.SetLocalPositionAndRotation(prefab.transform.localPosition, prefab.transform.localRotation);
        transform.localScale = prefab.transform.localScale;
    }
}
}