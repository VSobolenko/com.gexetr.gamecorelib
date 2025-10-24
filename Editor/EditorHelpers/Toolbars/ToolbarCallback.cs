using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

#if !UNITY_2019_1_OR_NEWER
using UnityEngine.Experimental.UIElements;
#endif

namespace GameEditor.Toolbars
{
public static class ToolbarCallback
{
    public static Type m_toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
    public static Type m_guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
    public static Type m_iWindowBackendType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");

    public static PropertyInfo m_windowBackend = m_guiViewType.GetProperty("windowBackend",
                                                                           BindingFlags.Public |
                                                                           BindingFlags.NonPublic |
                                                                           BindingFlags.Instance);

    public static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty("visualTree",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		public static PropertyInfo m_viewVisualTree = m_guiViewType.GetProperty("visualTree",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
    public static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    public static ScriptableObject m_currentToolbar;
    
    
    public static event Action OnToolbarGUI;
    public static event Action OnToolbarGUILeft;
    public static event Action OnToolbarGUIRight;

    static ToolbarCallback()
    {
        UnityEditor.EditorApplication.update -= OnUpdate;
        UnityEditor.EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
        if (m_currentToolbar == null)
        {
            // Find toolbar
            var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
            m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;

            if (m_currentToolbar != null)
            {
#if UNITY_2021_1_OR_NEWER
                var root = m_currentToolbar.GetType()
                                           .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                var rawRoot = root.GetValue(m_currentToolbar);
                var mRoot = rawRoot as VisualElement;
                RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

                void RegisterCallback(string root, Action cb)
                {
                    var toolbarZone = mRoot.Q(root);

                    var parent = new VisualElement()
                    {
                        style =
                        {
                            flexGrow = 1,
                            flexDirection = FlexDirection.Row,
                        }
                    };
                    var container = new IMGUIContainer();
                    container.onGUIHandler += () => { cb?.Invoke(); };
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
#else
#if UNITY_2020_1_OR_NEWER
					var windowBackend = m_windowBackend.GetValue(m_currentToolbar);

					// Get it's visual tree
					var visualTree = (VisualElement) m_viewVisualTree.GetValue(windowBackend, null);
#else
					// Get it's visual tree
					var visualTree = (VisualElement) m_viewVisualTree.GetValue(m_currentToolbar, null);
#endif
					// Get first child which 'happens' to be toolbar IMGUIContainer
					var container = (IMGUIContainer) visualTree[0];

					// (Re)attach handler
					var handler = (Action) m_imguiContainerOnGui.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					m_imguiContainerOnGui.SetValue(container, handler);
#endif
            }
        }
    }

    static void OnGUI()
    {
        var handler = OnToolbarGUI;
        if (handler != null) handler();
    }
}
}