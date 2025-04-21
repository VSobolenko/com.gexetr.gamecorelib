using System;
using System.Collections.Generic;
using Game.Pools;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Editor.ObjectPoolTests
{ 
[TestFixture]
internal class PoolableObjectPoolCallSequenceTests
{
    [Test]
    public void Get_GetElementFromPoolInWorldSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = GetMockObject<IPoolable>();
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
        mockObject.Verify(x => x.SetPositionAndRotation(Moq.It.IsAny<Vector3>(), Moq.It.IsAny<Quaternion>()), Moq.Times.Once);
        mockObject.Verify(x => x.SetParent(Moq.It.IsAny<Transform>()), Moq.Times.Once);
        mockObject.Verify(x => x.SetActive(Moq.It.IsAny<bool>()), Moq.Times.Once);
        mockObject.Verify(x => x.OnUse(), Moq.Times.Once);
        mockObject.Verify(x => x.OnRelease(), Moq.Times.Never);
        Assert.AreEqual(callLog, callOrder);
    }

    [Test]
    public void Get_GetElementFromPoolInLocalSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var mockObject = GetMockObject<IPoolable>();
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
        mockObject.Verify(x => x.SetPositionAndRotation(Moq.It.IsAny<Vector3>(), Moq.It.IsAny<Quaternion>()), Moq.Times.Once);
        mockObject.Verify(x => x.SetParent(Moq.It.IsAny<Transform>()), Moq.Times.Once);
        mockObject.Verify(x => x.SetActive(Moq.It.IsAny<bool>()), Moq.Times.Once);
        mockObject.Verify(x => x.OnUse(), Moq.Times.Once);
        mockObject.Verify(x => x.OnRelease(), Moq.Times.Never);
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
        var mockObject = GetMockObject<IPoolable>();
        SetupMethodCall(mockObject, nameof(IPoolable.SetActive), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.SetParent), callOrder);
        SetupMethodCall(mockObject, nameof(IPoolable.OnRelease), callOrder);

        // Act
        pool.Release(mockObject.Object);

        // Assert
        mockObject.Verify(x => x.SetActive(Moq.It.IsAny<bool>()), Moq.Times.Once);
        mockObject.Verify(x => x.SetParent(Moq.It.IsAny<Transform>()), Moq.Times.Once);
        mockObject.Verify(x => x.OnRelease(), Moq.Times.Once);
        mockObject.Verify(x => x.OnUse(), Moq.Times.Never);
        Assert.AreEqual(callLog, callOrder);
    }

#if GCL_ENABLE_MOQ
    private static Moq.Mock<T> GetMockObject<T>() where T : class
    {
        return new Moq.Mock<T>();
    }
    
    private static void SetupMethodCall(Moq.Mock<IPoolable> mockObject, string methodName, ICollection<string> callOrder)
    {
        switch (methodName)
        {
            case nameof(IPoolable.SetPositionAndRotation):
                mockObject.Setup(x => x.SetPositionAndRotation(Moq.It.IsAny<Vector3>(), Moq.It.IsAny<Quaternion>()))
                    .Callback(() => callOrder.Add(nameof(IPoolable.SetPositionAndRotation)));
                break;
            case nameof(IPoolable.SetParent):
                mockObject.Setup(x => x.SetParent(Moq.It.IsAny<Transform>()))
                    .Callback(() => callOrder.Add(nameof(IPoolable.SetParent)));
                break;
            case nameof(IPoolable.SetActive):
                mockObject.Setup(x => x.SetActive(Moq.It.IsAny<bool>()))
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
#else
    private static string ErrorMessage = "Install Moq package and use GCL_ENABLE_MOQ define for tests!";

    private class Moq
    {
        internal class It
        {
            public static T IsAny<T>() => throw new NotSupportedException(ErrorMessage);
        }

        internal readonly struct Times
        {
            public static Times Once() => throw new NotSupportedException(ErrorMessage);
            public static Times Never() => throw new NotSupportedException(ErrorMessage);
        }
    }

    private class FakeMoq<T>
    {
        public IPoolable Object => throw new NotSupportedException(ErrorMessage);

        public void Verify(System.Linq.Expressions.Expression<Action<T>> expression, Func<Moq.Times> times) =>
            throw new NotSupportedException(ErrorMessage);
    }

    private static FakeMoq<T> GetMockObject<T>()
    {
        throw new NotSupportedException(ErrorMessage);
    }

    private static void SetupMethodCall<T>(FakeMoq<T> mockObject, string methodName, ICollection<string> callOrder)
    {
        throw new NotSupportedException(ErrorMessage);
    }
    
#endif

    private static IPoolable GetTestElement()
    {
        return new PoolableTestObject("Key", false);
    }
}
}