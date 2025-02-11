using System;
using Game.Extensions;
using Game.Factories;
using Game.Pools;
using Game.Pools.Managers;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Runtime.ObjectPoolTests
{ 
internal class ObjectPoolKeyManagerTests : ObjectPoolManagerGeneralTests
{
    protected override IObjectPoolManager
        CreatePoolManager(IFactoryGameObjects factory, Transform parent, int capacity) =>
        new ObjectPoolKeyManager(factory, parent, capacity);

    [Test]
    public void Release_UnknownPrefab_ShouldBeThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => poolManager.Release(prefab));

        // Assert
        StringAssert.StartsWith(
            $"Return unknown prefab to pool. Use {nameof(ObjectPoolKeyManager.Prepare)} first. Prefab={prefab.Key}",
            ex.Message);
    }

    [Test]
    public void Prepare_PrefabWithNullKey_ShouldThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled().With(x => x.Key = null);
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => poolManager.Prepare(prefab, 1));

        // Assert
        StringAssert.StartsWith($"Added Null or Empty key to Pool. Prefab name \"{prefab.name}\"", ex.Message);
    }
}
}