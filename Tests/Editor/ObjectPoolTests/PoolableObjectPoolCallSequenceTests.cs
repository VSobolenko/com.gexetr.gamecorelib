using System;
using System.Collections.Generic;
using Game.Pools;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Editor.ObjectPoolTests
{ 
[TestFixture]
public class PoolableObjectPoolCallSequenceTests
{
    [Test]
    public void Get_GetElementFromPoolInWorldSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = new Mock<IPoolable>();
        var factory = new Func<IPoolable>(() => mockObject.Object);
        IPoolableObjectPool<IPoolable> pool = new PoolableObjectPool<IPoolable>(5, null, factory);
        var callLog = new List<string>
        {
            nameof(IPoolable.SetPositionAndRotation),
            nameof(IPoolable.SetParent),
            nameof(IPoolable.SetActive),
            nameof(IPoolable.OnUse),
        };
        var callOrder = new List<string>();
        SetupMethodCall(mockObject, nameof(IPoolable.SetPositionAndRotation), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.SetParent), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.SetActive), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.OnUse), callOrder);
        
        // Act
        pool.Get();

        // Assert
        mockObject.Verify(x => x.SetPositionAndRotation(It.IsAny<Vector3>(), It.IsAny<Quaternion>()), Times.Once);
        mockObject.Verify(x => x.SetParent(It.IsAny<Transform>()), Times.Once);
        mockObject.Verify(x => x.SetActive(It.IsAny<bool>()), Times.Once);
        mockObject.Verify(x => x.OnUse(), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }

    [Test]
    public void Get_GetElementFromPoolInLocalSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = new Mock<IPoolable>();
        var factory = new Func<IPoolable>(() => mockObject.Object);
        IPoolableObjectPool<IPoolable> pool = new PoolableObjectPool<IPoolable>(5, null, factory);
        var callLog = new List<string>
        {
            nameof(IPoolable.SetParent),
            nameof(IPoolable.SetPositionAndRotation),
            nameof(IPoolable.SetActive),
            nameof(IPoolable.OnUse),
        };
        var callOrder = new List<string>();
        SetupMethodCall(mockObject, nameof(IPoolable.SetParent), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.SetPositionAndRotation), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.SetActive), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.OnUse), callOrder);

        // Act
        pool.Get(null, false);

        // Assert
        mockObject.Verify(x => x.SetPositionAndRotation(It.IsAny<Vector3>(), It.IsAny<Quaternion>()), Times.Once);
        mockObject.Verify(x => x.SetParent(It.IsAny<Transform>()), Times.Once);
        mockObject.Verify(x => x.SetActive(It.IsAny<bool>()), Times.Once);
        mockObject.Verify(x => x.OnUse(), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }

    [Test]
    public void Release_AddElementToPool_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        IPoolableObjectPool<IPoolable> pool = new PoolableObjectPool<IPoolable>(5, null, GetTestElement);
        var callLog = new List<string>
        {
            nameof(IPoolable.SetActive),
            nameof(IPoolable.SetParent),
            nameof(IPoolable.OnRelease),
        };
        var callOrder = new List<string>();
        var mockObject = new Mock<IPoolable>();
        SetupMethodCall(mockObject, nameof(IPoolable.SetActive), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.SetParent), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.OnRelease), callOrder);

        // Act
        pool.Release(mockObject.Object);

        // Assert
        mockObject.Verify(x => x.SetActive(It.IsAny<bool>()), Times.Once);
        mockObject.Verify(x => x.SetParent(It.IsAny<Transform>()), Times.Once);
        mockObject.Verify(x => x.OnRelease(), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }

    private static void SetupMethodCall(Mock<IPoolable> mockObject, string methodName, ICollection<string> callOrder)
    {
        switch (methodName)
        {
            case nameof(IPoolable.SetPositionAndRotation):
                mockObject.Setup(x => x.SetPositionAndRotation(It.IsAny<Vector3>(), It.IsAny<Quaternion>()))
                          .Callback(() => callOrder.Add(nameof(IPoolable.SetPositionAndRotation)));
                break;
            case nameof(IPoolable.SetParent):
                mockObject.Setup(x => x.SetParent(It.IsAny<Transform>()))
                          .Callback(() => callOrder.Add(nameof(IPoolable.SetParent)));
                break;
            case nameof(IPoolable.SetActive):
                mockObject.Setup(x => x.SetActive(It.IsAny<bool>()))
                          .Callback(() => callOrder.Add(nameof(IPoolable.SetActive)));
                break;
            case nameof(IPoolable.OnUse):
                mockObject.Setup(x => x.OnUse())
                          .Callback(() => callOrder.Add(nameof(IPoolable.OnUse)));
                break;
            case nameof(IPoolable.OnRelease):
                mockObject.Setup(x => x.OnRelease())
                          .Callback(() => callOrder.Add(nameof(IPoolable.OnRelease)));
                break;
            default:
                throw new ArgumentException("Unknown method");
        }
    }

    private static IPoolable GetTestElement()
    {
        return new PoolableTestObject("Key", false);
    }
}
}