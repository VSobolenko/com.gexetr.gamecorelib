using System;
using System.Collections.Generic;
using Game.Pools;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace GameTests.Tests.Runtime.ObjectPoolTests
{ 
[TestFixture]
public class PoolableObjectPoolCallSequenceTests
{
    [Test]
    public void Get_GetElementFromPoolInWorldSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = new Mock<PoolableTestObject>();
        var factory = new Func<PoolableTestObject>(() => mockObject.Object);
        IPoolableObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, factory);
        var callLog = new List<string>
        {
            nameof(PoolableTestObject.SetPositionAndRotation),
            nameof(PoolableTestObject.SetParent),
            nameof(PoolableTestObject.SetActive),
            nameof(PoolableTestObject.OnUse),
        };
        var callOrder = new List<string>();
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetPositionAndRotation), callOrder, It.IsAny<Vector3>(), It.IsAny<Quaternion>());
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetParent), callOrder, It.IsAny<Transform>());
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetActive), callOrder);
        SetupMethodCall(mockObject, nameof(PoolableTestObject.OnUse), callOrder);

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
        var mockObject = new Mock<PoolableTestObject>();
        var factory = new Func<PoolableTestObject>(() => mockObject.Object);
        IPoolableObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, factory);
        var callLog = new List<string>
        {
            nameof(PoolableTestObject.SetParent),
            nameof(PoolableTestObject.SetPositionAndRotation),
            nameof(PoolableTestObject.SetActive),
            nameof(PoolableTestObject.OnUse),
        };
        var callOrder = new List<string>();
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetParent), callOrder, It.IsAny<Transform>());
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetPositionAndRotation), callOrder, It.IsAny<Vector3>(), It.IsAny<Quaternion>());
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetActive), callOrder);
        SetupMethodCall(mockObject, nameof(PoolableTestObject.OnUse), callOrder);

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
        IPoolableObjectPool<PoolableTestObject> pool = new PoolableObjectPool<PoolableTestObject>(5, null, GetTestElement);
        var callLog = new List<string>
        {
            nameof(PoolableTestObject.SetActive),
            nameof(PoolableTestObject.SetParent),
            nameof(PoolableTestObject.OnRelease),
        };
        var callOrder = new List<string>();
        var mockObject = new Mock<PoolableTestObject>();
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetActive), callOrder, It.IsAny<bool>());
        SetupMethodCall(mockObject, nameof(PoolableTestObject.SetParent), callOrder, It.IsAny<Transform>());
        SetupMethodCall(mockObject, nameof(PoolableTestObject.OnRelease), callOrder);

        // Act
        pool.Release(mockObject.Object);

        // Assert
        mockObject.Verify(x => x.SetActive(It.IsAny<bool>()), Times.Once);
        mockObject.Verify(x => x.SetParent(It.IsAny<Transform>()), Times.Once);
        mockObject.Verify(x => x.OnRelease(), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }
    
    private static void SetupMethodCall<T>(Mock<T> mockService, string methodName, List<string> callOrder, params object[] parameters) where T : class
    {
        mockService.Setup(service => service.GetType().GetMethod(methodName).Invoke(service, parameters))
            .Callback(() => callOrder.Add(methodName));
    }

    private static PoolableTestObject GetTestElement()
    {
        return new PoolableTestObject("Key", false);
    }
}
}