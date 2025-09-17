using System;

namespace Game.Exceptions
{
internal class GCLException : Exception
{
    protected GCLException(string message) : base(message) { }
}
}