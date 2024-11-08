using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.FSMCore;
using Game.FSMCore.Profiler;
using Game.InternalData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
            if (_fsmLinkers == null)
            {
                Log.Warning("This FSM was stopped or delete");

                return;
            }

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
        {
            _graph.CreateStateNode((dynamic) state);
        }

        foreach (var transition in stateMachine.Tree.GetTransitions())
            _graph.CreateTransition((dynamic) transition);
    }

    private void Update()
    {
        if (_stateMachine == null || _graph._states.Count == 0) 
            return;

        var nodeOld = _graph._states.FirstOrDefault(x => x.sourceState == _stateMachine.ActiveState);
        var node = _graph._states.FirstOrDefault(x => x.sourceState == _stateMachine.ActiveState);
        if (node == null || nodeOld == null)
            return;
        _graph.RemoveFromSelection(nodeOld);
        //_graph.AddToSelection(node);
        node.selected = true;
    }
}
}