﻿using System;
using Game.Extensions;
using Game.Pools;
using NUnit.Framework;

namespace GameTests.Tests.Runtime.ObjectPoolTests
{
[TestFixture]
public class PoolableObjectPoolTests
{
    [Test]
    public void Get_GetElementFromPool_ShouldDecreasePoolSize()
    {
        // Arrange
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
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
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);

        // Act
        var element = pool.Get();

        // Assert
        Assert.IsNotNull(element);
        Assert.AreEqual(element.IsUiElement, "Key");
        Assert.AreEqual(pool.Count, 0);
    }

    [Test]
    public void Get_ReturnAndGetSameElementWhenEmptyPool_ShouldReturnSameObject()
    {
        // Arrange
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
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
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
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
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
        var testElement = GetTestElement();

        // Act
        pool.Release(testElement);
        var ex = Assert.Throws<InvalidOperationException>(() => pool.Release(testElement));

        // Assert
        Assert.AreEqual($"The element \"{testElement.GetType().Name}\" is already in the pool!", ex.Message);
    }

    private static PoolableTestObject GetTestElement() => new("Key", false);
}
}