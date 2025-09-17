using UnityEngine;

namespace Game.Exceptions
{
internal class GCLInspectorReferenceException : GCLException
{
    public GCLInspectorReferenceException(
        string objectName, 
        string componentType, 
        string fieldName)
        : base($"Missing reference in inspector: {objectName} -> {componentType}.{fieldName}. " +
               $"Please assign the reference in the Unity inspector.")
    {
    }
    
    public GCLInspectorReferenceException(
        Object component,
        string fieldName)
        : this(component.name, component.GetType().Name, fieldName)
    {
    }
}
}