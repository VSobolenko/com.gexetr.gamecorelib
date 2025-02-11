using System;
using UnityEngine;

namespace Game.Tests.Runtime.TestingElements
{
internal class TestClassWithUnityVectorAndQuaternion
{
    public int id;
    public string name;
    public Vector3 position;
    public Quaternion rotation;

    public override bool Equals(object obj)
    {
        if (obj is not TestClassWithUnityVectorAndQuaternion other)
            return false;

        return id == other.id && name == other.name && position == other.position && rotation == other.rotation;
    }

    public override int GetHashCode() => HashCode.Combine(id, name, position, rotation);
}
}