using Game.FSMCore.States;
using UnityEditor.Experimental.GraphView;

namespace GameEditor.FSMVisualizers
{
internal sealed class FSMNode : Node
{
    public IState sourceState;
    public Port inputPort;
    public Port outputPort;
    public bool isOriginal;

    public FSMNode UpdateToInput<TIn>()
    {
        var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(TIn));
        port.name = "Input";
        inputPort = port;
        inputContainer.Add(port);
        RefreshExpandedState();
        RefreshPorts();

        return this;
    }

    public FSMNode UpdateToOutput<TOut>()
    {
        var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(TOut));
        port.name = "Output";
        outputPort = port;
        outputContainer.Add(port);
        RefreshExpandedState();
        RefreshPorts();

        return this;
    }
}
}