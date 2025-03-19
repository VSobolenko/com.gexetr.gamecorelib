using Game.Factories;
using Game.Pools.Managers;
using UnityEngine;

namespace Game.Pools
{
public static class ObjectPoolInstaller
{
    public static IObjectPoolManager Key(IFactoryGameObjects factory, Transform parent = null, int poolCapacity = 32)
    {
        parent = (parent == null ? new GameObject().transform : parent);

        return new ObjectPoolKeyManager(factory, parent, poolCapacity);
    }
    
    public static IObjectPoolManager KeyAutoEditor(IFactoryGameObjects factory, Transform parent = null, int poolCapacity = 32)
    {
        parent = (parent == null ? new GameObject().transform : parent);

        return Application.isEditor
            ? new ObjectPoolKeyEditorSeparateManager(factory, parent, poolCapacity)
            : Key(factory, parent, poolCapacity);
    }
    
    public static IObjectPoolManager Type(IFactoryGameObjects factory, Transform parent = null, int poolCapacity = 32)
    {
        parent = (parent == null ? new GameObject().transform : parent);

        return new ObjectPoolTypeManager(factory, parent, poolCapacity);
    }
    
    public static IObjectPoolManager TypeAutoEditor(IFactoryGameObjects factory, Transform parent = null, int poolCapacity = 32)
    {
        parent = (parent == null ? new GameObject().transform : parent);

        return Application.isEditor
            ? new ObjectPoolTypeEditorSeparateManager(factory, parent, poolCapacity)
            : Type(factory, parent, poolCapacity);
    }
    
    public static IComponentObjectPoolManager Component(IFactoryGameObjects factory, Transform parent = null, int poolCapacity = 32)
    {
        parent = (parent == null ? new GameObject().transform : parent);

        return new ObjectPoolComponentManager(factory, parent, poolCapacity);
    }
    
    public static IComponentObjectPoolManager ComponentAutoEditor(IFactoryGameObjects factory, Transform parent = null, int poolCapacity = 32)
    {
        parent = (parent == null ? new GameObject().transform : parent);

        return Application.isEditor
            ? new ObjectPoolComponentManager(factory, parent, poolCapacity)
            : new ObjectPoolComponentSeparateManager(factory, parent, poolCapacity);
    }
}
}