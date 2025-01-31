using Game.Factories;
using Game.Factories.Managers;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Runtime
{
[TestFixture]
internal class FactoryGameObjectsTests
{
    [Test]
    public void InstantiateEmpty_CreateNewEmptyGameObject_ShouldReturnNewEmptyGameObject()
    {
        // Arrange
        IFactoryGameObjects factoryGameObjects = new StandardObjectsFactory();
        
        // Act
        var gameObject = factoryGameObjects.InstantiateEmpty();
        var gameObjectComponents = gameObject.GetComponents(typeof(Component));
        
        // Assert
        Assert.IsTrue(gameObject != null);
        Assert.IsTrue(gameObject is { });
        Assert.IsTrue(gameObject.transform.childCount == 0);
        Assert.IsTrue(gameObjectComponents.Length == 1); // Transform = 1
        Assert.IsTrue(gameObjectComponents[0].GetType() == typeof(Transform));
    }
}
}