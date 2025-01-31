using System;
using Game.Extensions;
using Game.Pools;
using NUnit.Framework;
using UnityEngine;

namespace GameTests.Tests.Runtime.ObjectPoolTests
{
[TestFixture]
public class GameObjectObjectPoolTests
{
    private const string NewGameObjectName = "GO";

    [Test]
    public void Get_GetElementFromPool_ShouldDecreasePoolSize()
    {
        // Arrange
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, GetTestElement);
        const int prepareCount = 3;
        pool.WithFor(prepareCount, x => x.Release(GetTestElement()));

        // Act
        pool.Get();

        // Assert
        Assert.AreNotEqual(pool.Count, prepareCount - 1);
    }

    [Test]
    public void Get_GetElementFromEmptyPool_ShouldCreateNewElementAndSizeNotChange()
    {
        // Arrange
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, GetTestElement);

        // Act
        var element = pool.Get();

        // Assert
        Assert.IsNotNull(element);
        Assert.AreEqual(element.name, NewGameObjectName);
        Assert.AreEqual(pool.Count, 0);
    }

    [Test]
    public void Get_ReturnAndGetSameElementWhenEmptyPool_ShouldReturnSameObject()
    {
        // Arrange
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, GetTestElement);
        var testElement = GetTestElement();

        // Act
        pool.Release(testElement);
        var returnedFromPool = pool.Get();

        // Assert
        Assert.AreEqual(testElement, returnedFromPool);
        Assert.ReferenceEquals(testElement, returnedFromPool);
    }

    [Test]
    public void Release_AddElementToEmptyPool_ShouldIncreasePoolSize()
    {
        // Arrange
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, GetTestElement);
        var testElement = GetTestElement();

        // Act
        pool.Release(testElement);

        // Assert
        Assert.AreEqual(pool.Count, 1);
    }

    [Test]
    public void Release_ReturnSameElementTwice_ShouldThrowException()
    {
        // Arrange
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, GetTestElement);
        var testElement = GetTestElement();

        // Act
        pool.Release(testElement);
        var ex = Assert.Throws<InvalidOperationException>(() => pool.Release(testElement));

        // Assert
        Assert.AreEqual($"The element \"{testElement.GetType().Name}\" is already in the pool!", ex.Message);
    }

    private static Transform GetTestElement() => new GameObject(NewGameObjectName).transform;
}
}