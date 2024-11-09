using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.FSMCore.States;
using Game.FSMCore.Transitions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace GameEditor.FSMVisualizers
{
public class FSMGraphView : GraphView
{
    public readonly List<FSMNode> _states = new();
    private readonly Vector2 _defaultNodeSize = new(150, 200);
    private readonly Vector2 _distance = new(10, 0);

    private Vector2 _activeDistance = Vector2.zero;

    public FSMGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        _distance = new Vector2(_distance.x + _defaultNodeSize.x, _distance.y);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compPorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compPorts.Add(port);
            }
        });

        return compPorts;
    }

    public FSMNode CreateStateNode<TIn, TOut>(State<TIn, TOut> state) =>
        CreateStateNode<TIn>(state).UpdateToOutput<TOut>();

    public FSMNode CreateStateNode<TIn>(DeadState<TIn> state) => CreateEmptyNode(state).UpdateToInput<TIn>();

    private FSMNode CreateEmptyNode(IState state, bool shift = true)
    {
        var stateNode = new FSMNode()
        {
            title = state.GetType().Name,
            sourceState = state,
            isOriginal = true,
        };
        _states.Add(stateNode);
        _activeDistance += shift ? _distance : Vector2.zero;
        stateNode.SetPosition(new Rect(_activeDistance, _defaultNodeSize));
        AddElement(stateNode);

        return stateNode;
    }

    public void CreateTransition<TDateTransfer>(AliveTransition<TDateTransfer> transition)
    {
        var sourceState = (IState) GetPrivateValue(transition, "_sourceState");
        var targetState = (IState) GetPrivateValue(transition, "_targetState");
        ConnectStates(sourceState, targetState);
    }

    public void CreateTransition<TDateTransfer>(Many2OneTransition<TDateTransfer> transition)
    {
        var sourceStates = (List<IEndState<IState>>) GetPrivateValue(transition, "_sourceStates");
        var targetState = (IState) GetPrivateValue(transition, "_targetState");

        foreach (var sourceState in sourceStates)
            ConnectStates(sourceState, targetState);
    }

    public void CreateTransition<TIn, TOut>(EntryTransition<TIn, TOut> transition)
    {
        var targetState = (DeadState<TIn>) GetPrivateValue(transition, "_targetState");
        var originalNode = GetNodeInstanceByState(targetState);
        var fakeNode = CreateEmptyNode(targetState, false).UpdateToOutput<TOut>();
        fakeNode.isOriginal = false;
        fakeNode.title = "Entry node";
        fakeNode.style.left = originalNode.style.left;
        fakeNode.style.top = originalNode.style.top.value.value + _distance.x;
        ConnectNodes(fakeNode, originalNode);
    }

    public void CreateTransition<TIn, TOut>(CircularTransition<TIn, TOut> transition)
    {
        var sourceState = (State<TIn, TOut>) GetPrivateValue(transition, "_targetState");
        var originalNode = GetNodeInstanceByState(sourceState);
        var fakeNode = CreateEmptyNode(sourceState, false).UpdateToInput<TIn>().UpdateToOutput<TOut>();
        fakeNode.isOriginal = false;
        fakeNode.title = "Circle node";
        fakeNode.style.left = originalNode.style.left;
        fakeNode.style.top = originalNode.style.top.value.value + _distance.x;
        ConnectNodesInput(originalNode, fakeNode);
        ConnectNodesOutput(fakeNode, originalNode);
    }

    private static object GetPrivateValue<T>(T instance, string field)
    {
        return typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(instance);
    }

    private void ConnectStates(IState sourceState, IState targetState)
    {
        var sourceNode = GetNodeInstanceByState(sourceState);
        var targetNode = GetNodeInstanceByState(targetState);
        ConnectNodes(sourceNode, targetNode);
    }

    private FSMNode GetNodeInstanceByState(IState state, bool isOriginal = true) =>
        _states.First(x => x.sourceState == state && (x.isOriginal || isOriginal));

    private static void ConnectNodes(FSMNode sourceNode, FSMNode targetNode)
    {
        var edge = new Edge()
        {
            output = sourceNode.outputPort,
            input = targetNode.inputPort,
        };
        SetupEdge(sourceNode, targetNode, edge);
    }

    private static void ConnectNodesOutput(FSMNode sourceNode, FSMNode targetNode)
    {
        var edge = new Edge()
        {
            output = sourceNode.outputPort,
            input = targetNode.outputPort,
        };

        SetupEdge(sourceNode, targetNode, edge);
    }

    private static void ConnectNodesInput(FSMNode sourceNode, FSMNode targetNode)
    {
        var edge = new Edge()
        {
            output = sourceNode.inputPort,
            input = targetNode.inputPort,
        };
        SetupEdge(sourceNode, targetNode, edge);
    }

    private static void SetupEdge(FSMNode sourceNode, FSMNode targetNode, Edge edge)
    {
        edge.input.Connect(edge);
        edge.output.Connect(edge);
        sourceNode.Add(edge);
        targetNode.Add(edge);
    }
}
}