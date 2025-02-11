using System;
using Game.Factories;
using Game.Pools;
using Game.Pools.Managers;
using Game.Tests.Runtime.TestingElements;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Runtime.ObjectPoolTests
{
internal class ObjectPoolTypeManagerTests : ObjectPoolManagerGeneralTests
{
    protected override IObjectPoolManager
        CreatePoolManager(IFactoryGameObjects factory, Transform parent, int capacity) =>
        new ObjectPoolTypeManager(factory, parent, capacity);

    [Test]
    public void Release_UnknownPrefab_ShouldBeThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        IObjectPoolManager poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => poolManager.Release(prefab));

        // Assert
        StringAssert.StartsWith(
            $"Return unknown prefab to pool. Use {nameof(ObjectPoolKeyManager.Prepare)} first. Prefab={nameof(MonoPoolableTestObject)}",
            ex.Message);
    }
}
}