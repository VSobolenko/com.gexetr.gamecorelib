using System;
using System.Linq;
using Game.DynamicData;
using Game.FSMCore;
using Game.FSMCore.Machines;
using Game.FSMCore.Profilers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEditor.FSMVisualizers
{
public class FSMVisualizer : EditorWindow
{
    private FSMGraphView _graph;
    private FSMProfilerProvider[] _fsmLinkers = Array.Empty<FSMProfilerProvider>();
    private Toolbar _activeToolBar;

    [MenuItem(GameData.EditorName + "/FSM Visualizer")]
    private static void OpenWindow()
    {
        var window = GetWindow<FSMVisualizer>();
        window.titleContent = new GUIContent("FSM");
        window.Show();
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
    }

    private void OnDisable() => rootVisualElement.Remove(_graph);

    private void ConstructGraphView()
    {
        _graph?.ClearSelection();
        _graph?.ClearClassList();
        _graph?.Clear();
        _graph = new FSMGraphView() {name = "FSM Graph",};
        _graph.StretchToParentSize();
        rootVisualElement.Add(_graph);
    }

    private void GenerateToolBar()
    {
        if (_activeToolBar == null)
            _activeToolBar = new Toolbar();

        _activeToolBar.Clear();
        rootVisualElement.Add(_activeToolBar);

        var findButton = new Button
        {
            clickable = new Clickable(() =>
            {
                FindFSMLinkers();
                GenerateToolBar();
            }),
            text = "Find FSM's",
        };

        _activeToolBar.Add(findButton);
        foreach (var fsmLinker in _fsmLinkers)
        {
            if (fsmLinker == null)
                return;

            var fsmButton = new Button
            {
                clickable = new Clickable(() => DrawFSM(fsmLinker.stateMachine)),
                text = fsmLinker.gameObject.name,
            };
            _activeToolBar.Add(fsmButton);
        }
    }

    private void FindFSMLinkers() => _fsmLinkers = FindObjectsByType<FSMProfilerProvider>(FindObjectsSortMode.None);

    private FiniteStateMachine _stateMachine;

    private void DrawFSM(FiniteStateMachine stateMachine)
    {
        if (stateMachine == null)
            return;
        _stateMachine = stateMachine;
        OnEnable();
        foreach (var state in stateMachine.Tree.GetStates())
            _graph.CreateStateNode((dynamic) state);

        foreach (var transition in stateMachine.Tree.GetTransitions())
            _graph.CreateTransition((dynamic) transition);
    }

    private void Update()
    {
        if (_stateMachine == null || _graph.states.Count == 0)
            return;

        var nodeOld = _graph.states.FirstOrDefault(x => x.sourceState == _stateMachine.ActiveState);
        var node = _graph.states.FirstOrDefault(x => x.sourceState == _stateMachine.ActiveState);

        if (node == null || nodeOld == null)
            return;

        foreach (var graphState in _graph.states)
            graphState.selected = false;

        node.selected = true;
    }
}
}