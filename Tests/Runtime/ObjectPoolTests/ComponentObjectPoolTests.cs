using System;
using Game.Extensions;
using Game.Pools;
using Game.Tests.Runtime.TestingElements;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Runtime.ObjectPoolTests
{
[TestFixture]
internal class ComponentObjectPoolTests
{
    private const string NewGameObjectName = "GO";

    [Test]
    public void Get_Get2UniqueElementFromPool_ShouldDecreasePoolSizeWith2NewElements()
    {
        // Arrange
        IComponentObjectPool<TestMonoBeh> pool = new ComponentObjectPool<TestMonoBeh>(5, null, GetTestElement);
        const int prepareCount = 2;
        pool.With(prepareCount, x => x.Release(GetTestElement()));

        // Act
        var element1 = pool.Get();
        var element2 = pool.Get();

        // Assert
        Assert.AreEqual(pool.Count, 0);
        Assert.AreNotSame(element1, element2);
    }

    [Test]
    public void Get_GetElementFromPool_ShouldDecreasePoolSize()
    {
        // Arrange
        IComponentObjectPool<TestMonoBeh> pool = new ComponentObjectPool<TestMonoBeh>(5, null, GetTestElement);
        const int prepareCount = 3;
        pool.With(prepareCount, x => x.Release(GetTestElement()));

        // Act
        pool.Get();

        // Assert
        Assert.AreEqual(pool.Count, prepareCount - 1);
    }

    [Test]
    public void Get_GetElementFromEmptyPool_ShouldCreateNewElementAndSizeNotChange()
    {
        // Arrange
        IComponentObjectPool<TestMonoBeh> pool = new ComponentObjectPool<TestMonoBeh>(5, null, GetTestElement);

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
        IComponentObjectPool<TestMonoBeh> pool = new ComponentObjectPool<TestMonoBeh>(5, null, GetTestElement);
        var testElement = GetTestElement();

        // Act
        pool.Release(testElement);
        var returnedFromPool = pool.Get();

        // Assert
        Assert.AreSame(testElement, returnedFromPool);
    }

    [Test]
    public void Release_AddElementToEmptyPool_ShouldIncreasePoolSize()
    {
        // Arrange
        IComponentObjectPool<TestMonoBeh> pool = new ComponentObjectPool<TestMonoBeh>(5, null, GetTestElement);
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
        IComponentObjectPool<TestMonoBeh> pool = new ComponentObjectPool<TestMonoBeh>(5, null, GetTestElement);
        var testElement = GetTestElement();

        // Act
        pool.Release(testElement);
        var ex = Assert.Throws<InvalidOperationException>(() => pool.Release(testElement));

        // Assert
        Assert.AreEqual($"The element \"{testElement.GetType().Name}\" is already in the pool!", ex.Message);
    }

    private static TestMonoBeh GetTestElement() => new GameObject(NewGameObjectName).AddComponent<TestMonoBeh>();
}
}