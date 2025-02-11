using System;

namespace Game.Tests.Runtime.TestingElements
{
[Serializable]
internal class SimpleTestClass
{
    public int id;
    public string name;
    public DateTime time;
    
    public override bool Equals(object obj)
    {
        if (obj is not SimpleTestClass other)
            return false;

        return id == other.id && name == other.name && time == other.time;
    }

    public override int GetHashCode() => HashCode.Combine(id, name, time);
}
}