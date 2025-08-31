using System;
using Game.Extensions;
using Game.Pools;
using NUnit.Framework;

namespace Game.Tests.Editor.ObjectPoolTests
{
[TestFixture]
internal class PoolableObjectPoolTests
{
    [Test]
    public void Get_Get2UniqueElementFromPool_ShouldDecreasePoolSizeWith2NewElements()
    {
        // Arrange
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
        const int prepareCount = 2;
        pool.ForEach(prepareCount, x => x.Release(GetTestElement()));

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
        IObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
        const int prepareCount = 3;
        pool.ForEach(prepareCount, x => x.Release(GetTestElement()));

        // Act
        pool.Get();

        // Assert
        Assert.AreEqual(pool.Count, prepareCount - 1);
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
        Assert.AreEqual(element.Key, "Key");
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
        Assert.AreSame(testElement, returnedFromPool);
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