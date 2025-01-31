using System;
using System.Collections.Generic;
using Game.Pools;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace GameTests.Tests.Runtime.ObjectPoolTests
{
[TestFixture]
public class GameObjectObjectPoolCallSequenceTests
{
    [Test]
    public void Get_GetElementFromPoolInWorldSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = new Mock<Transform>();
        var factory = new Func<Transform>(() => mockObject.Object);
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, factory);
        var callLog = new List<string>
        {
            nameof(Transform.SetPositionAndRotation),
            nameof(Transform.SetParent),
            nameof(Transform.gameObject.SetActive),
        };
        var callOrder = new List<string>();
        SetupMethodCall(mockObject, nameof(Transform.SetPositionAndRotation), callOrder, It.IsAny<Vector3>(), It.IsAny<Quaternion>());
        SetupMethodCall(mockObject, nameof(Transform.SetParent), callOrder, It.IsAny<Transform>());
        SetupMethodCall(mockObject, nameof(Transform.gameObject.SetActive), callOrder, It.IsAny<bool>());

        // Act
        pool.Get();

        // Assert
        mockObject.Verify(x => x.SetPositionAndRotation(It.IsAny<Vector3>(), It.IsAny<Quaternion>()), Times.Once);
        mockObject.Verify(x => x.SetParent(It.IsAny<Transform>()), Times.Once);
        mockObject.Verify(x => x.gameObject.SetActive(It.IsAny<bool>()), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }

    [Test]
    public void Get_GetElementFromPoolInLocalSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = new Mock<Transform>();
        var factory = new Func<Transform>(() => mockObject.Object);
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, factory);
        var callLog = new List<string>
        {
            nameof(Transform.SetParent),
            nameof(Transform.SetLocalPositionAndRotation),
            nameof(Transform.gameObject.SetActive),
        };
        var callOrder = new List<string>();
        SetupMethodCall(mockObject, nameof(Transform.SetParent), callOrder, It.IsAny<Transform>());
        SetupMethodCall(mockObject, nameof(Transform.SetLocalPositionAndRotation), callOrder, It.IsAny<Vector3>(), It.IsAny<Quaternion>());
        SetupMethodCall(mockObject, nameof(Transform.gameObject.SetActive), callOrder, It.IsAny<bool>());

        // Act
        pool.Get();

        // Assert
        mockObject.Verify(x => x.SetParent(It.IsAny<Transform>()), Times.Once);
        mockObject.Verify(x => x.SetLocalPositionAndRotation(It.IsAny<Vector3>(), It.IsAny<Quaternion>()), Times.Once);
        mockObject.Verify(x => x.gameObject.SetActive(It.IsAny<bool>()), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }

    [Test]
    public void Release_AddElementToPool_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        IGameObjectObjectPool<Transform> pool = new GameObjectObjectPool<Transform>(5, null, GetTestElement);
        var callLog = new List<string>
        {
            nameof(Transform.gameObject.SetActive),
            nameof(Transform.transform.SetParent),
        };
        var callOrder = new List<string>();
        var mockObject = new Mock<Transform>();
        SetupMethodCall(mockObject, nameof(Transform.gameObject.SetActive), callOrder, It.IsAny<bool>());
        SetupMethodCall(mockObject, nameof(Transform.transform.SetParent), callOrder, It.IsAny<Transform>());

        // Act
        pool.Release(mockObject.Object);

        // Assert
        mockObject.Verify(x => x.gameObject.SetActive(It.IsAny<bool>()), Times.Once);
        mockObject.Verify(x => x.transform.SetParent(It.IsAny<Transform>()), Times.Once);
        Assert.AreEqual(callLog, callOrder);
    }
    
    private static void SetupMethodCall<T>(Mock<T> mockService, string methodName, List<string> callOrder, params object[] parameters) where T : class
    {
        mockService.Setup(service => service.GetType().GetMethod(methodName).Invoke(service, parameters))
            .Callback(() => callOrder.Add(methodName));
    }

    private static Transform GetTestElement() => new GameObject("GO").transform;
}
}