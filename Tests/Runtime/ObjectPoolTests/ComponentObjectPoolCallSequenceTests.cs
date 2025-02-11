using System;
using Game.Extensions;
using Game.Pools;
using Game.Tests.Runtime.TestingElements;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Runtime.ObjectPoolTests
{
[TestFixture]
internal class ComponentObjectPoolCallSequenceTests
{
    private static Vector3 NegativeOne => Vector3.one * -1;

    [Test]
    public void Get_GetElementFromPoolInWorldSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var parent = GetTestObject().With(x => x.transform.position = NegativeOne).transform;
        var factory = new Func<TestMonoBeh>(() =>
        {
            var fakeParent = GetTestObject().With(x => x.transform.position = NegativeOne).transform;
            var testObject = GetTestObject()
                             .With(x => x.transform.SetParent(fakeParent))
                             .AddComponent<TestMonoBeh>();

            return SetupWorldObject(testObject, "new GO", Vector3.zero, Quaternion.identity);
        });
        var pool = new ComponentObjectPool<TestMonoBeh>(5, null, factory);

        // Act
        var pooledObject = pool.Get(Vector3.one, Quaternion.Euler(Vector3.one), parent, inWorldSpace: true);

        // Assert
        var angle = Quaternion.Angle(pooledObject.transform.rotation, Quaternion.Euler(Vector3.one));
        Assert.AreSame(parent, pooledObject.transform.parent);
        Assert.AreEqual(true, pooledObject.gameObject.activeSelf);
        Assert.AreEqual(Vector3.one, pooledObject.transform.position);
        Assert.AreEqual(Vector3.one - NegativeOne, pooledObject.transform.localPosition);
        Assert.IsTrue(angle < 0.01f, $"Quaternions are not equal. Angle: {angle}");
    }

    [Test]
    public void Get_GetElementFromPoolInLocalSpace_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var parent = GetTestObject().With(x => x.transform.position = NegativeOne).transform;
        var factory = new Func<TestMonoBeh>(() =>
        {
            var fakeParent = GetTestObject().With(x => x.transform.position = NegativeOne).transform;
            var testObject = GetTestObject()
                             .With(x => x.transform.SetParent(fakeParent))
                             .AddComponent<TestMonoBeh>();

            return SetupWorldObject(testObject, "new GO", Vector3.zero, Quaternion.identity);
        });
        var pool = new ComponentObjectPool<TestMonoBeh>(5, null, factory);

        // Act
        var pooledObject = pool.Get(Vector3.one, Quaternion.Euler(Vector3.one), parent, inWorldSpace: false);

        // Assert
        var angle = Quaternion.Angle(pooledObject.transform.rotation, Quaternion.Euler(Vector3.one));
        Assert.AreSame(parent, pooledObject.transform.parent);
        Assert.AreEqual(true, pooledObject.gameObject.activeSelf);
        Assert.AreEqual(Vector3.one + NegativeOne, pooledObject.transform.position);
        Assert.AreEqual(Vector3.one, pooledObject.transform.localPosition);
        Assert.IsTrue(angle < 0.01f, $"Quaternions are not equal. Angle: {angle}");
    }

    [Test]
    public void Release_AddElementToPool_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var pooledObject = GetTestObject().AddComponent<TestMonoBeh>();
        var parent = GetTestObject().With(x => x.transform.position = NegativeOne).transform;
        var factory = new Func<TestMonoBeh>(() =>
        {
            var fakeParent = GetTestObject().With(x => x.transform.position = NegativeOne).transform;
            var testObject = GetTestObject()
                             .With(x => x.transform.SetParent(fakeParent))
                             .AddComponent<TestMonoBeh>();

            return SetupWorldObject(testObject, "new GO", Vector3.zero, Quaternion.identity);
        });
        var pool = new ComponentObjectPool<TestMonoBeh>(5, parent, factory);

        // Act
        pool.Release(pooledObject);

        // Assert
        Assert.AreSame(parent, pooledObject.transform.parent);
        Assert.AreEqual(false, pooledObject.gameObject.activeSelf);
    }

    private static T SetupWorldObject<T>(T component, string name, Vector3 position, Quaternion rotation)
        where T : Component
    {
        component.gameObject.name = name;
        component.transform.position = position;
        component.transform.rotation = rotation;

        return component;
    }

    private static GameObject GetTestObject() => new();
}
}