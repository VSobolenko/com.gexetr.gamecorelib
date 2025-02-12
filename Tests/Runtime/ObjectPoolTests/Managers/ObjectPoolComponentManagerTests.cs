using System;
using System.Linq;
using Game.Extensions;
using Game.Factories;
using Game.Pools;
using Game.Pools.Managers;
using Game.Tests.Runtime.TestingElements;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Tests.Runtime.ObjectPoolTests
{
[TestFixture]
internal class ObjectPoolComponentManagerTests
{
    protected virtual IComponentObjectPoolManager CreatePoolManager(IFactoryGameObjects factory, Transform parent,
        int capacity) =>
        new ObjectPoolComponentManager(factory, parent, capacity);

    [Test]
    public void Constructor_CreatePoolManager_ShouldBeSetUpCorrectComponents()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var parent = CreateEmpty().transform;
        var expectedComponentsInRootParent = new[] { typeof(Transform), typeof(ObjectPoolProfiler), };

        // Act
        CreatePoolManager(mockFactory.Object, parent, 0);

        // Assert
        var componentsInRootParent = parent.GetComponents<Component>().Select(x => x.GetType());
        Assert.IsTrue(expectedComponentsInRootParent.SequenceEqual(componentsInRootParent));
    }

    [Test]
    public void Constructor_CreatePoolManager_ShouldBeSetUpCorrectParentHierarchy()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var parent = CreateEmpty().transform;

        // Act
        CreatePoolManager(mockFactory.Object, parent, 0);

        // Assert
        var totalChild = GetTotalChildren(parent);
        Assert.AreEqual(0, totalChild);
    }

    [Test]
    public void Get_NewElementFromEmptyPool_ShouldCreateAndReturnElementWithSameKey()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var pool = poolManager.Prepare(prefab);
        var instance = poolManager.Get(prefab);

        // Assert
        Assert.AreEqual(prefab.Key, instance.Key);
        Assert.AreNotSame(prefab, instance);
        Assert.AreEqual(pool.Count, 0);
    }

    [Test]
    public void Get_NullPrefab_ShouldThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => poolManager.Get<MonoPoolableTestObject>(null));

        // Assert
        StringAssert.StartsWith(
            $"Can't execute {nameof(ObjectPoolKeyManager.Get)} with null {nameof(MonoPoolableTestObject)}",
            ex.Message);
    }

    [Test]
    public void Get_UnknownPoolableObject_ShouldThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => poolManager.Get(prefab));

        // Assert
        StringAssert.StartsWith(
            $"An unknown object was requested. Use {nameof(ObjectPoolKeyManager.Prepare)} first",
            ex.Message);
    }

    [Test]
    public void Prepare_DoublePreparePrefabsForce_ShouldBeCreateSumOfNumbers()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var pool = poolManager.Prepare(prefab, 3);
        poolManager.Prepare(prefab, 2, true);

        // Assert
        Assert.AreEqual(5, pool.Count);
    }

    [Test]
    public void Prepare_DoublePreparePrefabsNotForce_ShouldBeCreateCountByMaximumNumber()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var pool = poolManager.Prepare(prefab, 3);
        poolManager.Prepare(prefab, 2);

        // Assert
        Assert.AreEqual(3, pool.Count);
    }

    [Test]
    public void Prepare_PreparePrefabsNotForce_ShouldBeCreateNewElementInsideManager()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var pool = poolManager.Prepare(prefab, 3);

        // Assert
        Assert.AreEqual(3, pool.Count);
    }

    [Test]
    public void Prepare_TryingPrepareNegativeCount_ShouldBeThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 0);

        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => poolManager.Prepare(prefab, -1));

        // Assert
        StringAssert.StartsWith(
            $"Expected count to be prepared can't be negative for {nameof(MonoPoolableTestObject)}",
            ex.Message);
    }

    [Test]
    public void Prepare_TryingPrepareNullPrefab_ShouldBeThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 0);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => poolManager.Prepare<MonoPoolableTestObject>(null, 1));

        // Assert
        StringAssert.StartsWith(
            $"Can't execute {nameof(ObjectPoolKeyManager.Prepare)} with null {nameof(MonoPoolableTestObject)}",
            ex.Message);
    }

    [Test]
    public void Release_NullPrefab_ShouldBeThrowException()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => poolManager.Release<MonoPoolableTestObject>(null));

        // Assert
        StringAssert.StartsWith(
            $"Can't execute {nameof(ObjectPoolKeyManager.Release)} with null {nameof(MonoPoolableTestObject)}",
            ex.Message);
    }

    [Test]
    public void Release_PrefabToPool_ShouldBeReturnToPool()
    {
        // Arrange
        var mockFactory = CreateMockFactory();
        var prefab = CreateUniqueMonoPooled();
        var poolManager = CreatePoolManager(mockFactory.Object, CreateEmpty().transform, 1);

        // Act
        var pool = poolManager.Prepare(prefab);
        poolManager.Release(prefab);

        // Assert
        Assert.AreEqual(1, pool.Count);
    }

    private static Mock<IFactoryGameObjects> CreateMockFactory()
    {
        var mockFactory = new Mock<IFactoryGameObjects>();
        mockFactory.Setup(x => x.InstantiateEmpty(It.IsAny<Transform>()))
            .Returns((Transform parent) =>
                CreateEmpty()
                    .With(x => x.transform.SetParent(parent)));

        mockFactory.Setup(x => x.InstantiateEmpty(It.IsAny<Transform>(), It.IsAny<Type[]>())).Returns(
            (Transform parent, Type[] components) =>
            {
                return CreateEmpty()
                    .With(components.Length, (i, x) => x.AddComponent(components[i]))
                    .With(x => x.transform.SetParent(parent));
            });

        mockFactory.Setup(x => x.Instantiate(It.IsAny<MonoPoolableTestObject>(), It.IsAny<Transform>())).Returns(
            (MonoPoolableTestObject pooled, Transform parent) => Object.Instantiate(pooled, parent));
        return mockFactory;
    }

    private static MonoPoolableTestObject CreateUniqueMonoPooled()
    {
        return CreateEmpty()
            .AddComponent<MonoPoolableTestObject>()
            .With(x => x.Key = Guid.NewGuid().ToString());
    }

    private static int GetTotalChildren(Transform parent) =>
        parent.childCount + parent.Cast<Transform>().Sum(GetTotalChildren);

    private static GameObject CreateEmpty() => new();
}
}