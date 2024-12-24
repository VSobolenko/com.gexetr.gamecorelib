using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Components.Utilities;
using Game.DynamicData;
using UnityEditor;
using UnityEngine;

namespace GameEditor.ProjectTools
{
    internal static class ProjectCheckForNullEditorTool
    {
        private static readonly List<string> Accessible = new() { "Assembly-CSharp", "GameCoreLib" };
        
        [MenuItem(GameData.EditorName + EditorToolsSubfolder.Project + EditorToolsSubfolder.NullValidator +
                  "/GameObjects Default Validate (Assembly-CSharp)")]
        private static void VerifyProjectGameObjectsToNull()
        {
            VerifyGameObjectsToNull(Accessible);
        }

        private static List<MonoBehaviour> VerifyGameObjectsToNull(List<string> accessible)
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(GameObject)}");
            var monoBehaviours = new List<MonoBehaviour>();
            
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                var verifyMonoBehToNull = VerifyGameObjectToNull(gameObject, accessible);
                monoBehaviours.AddRange(verifyMonoBehToNull.Where(monoBehaviour => monoBehaviour != null));
            }

            return monoBehaviours;
        }

        private static IEnumerable<MonoBehaviour> VerifyGameObjectToNull(GameObject gameObject, List<string> accessibleAssembly)
        {
            var monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>()
                .Where(x => x != null && accessibleAssembly.Contains(x.GetType().Assembly.GetName().Name));
            var healthy = true;
            foreach (var monoBehaviour in monoBehaviours)
                healthy &= SceneCheckForNullEditorProvider.CheckField(monoBehaviour, new List<object>(), monoBehaviour.gameObject,
                    monoBehaviour.GetType().Name, false, accessibleAssembly);

            if (healthy)
                Log.Info("All objects are healthy");

            return monoBehaviours;
        }
        
        [MenuItem(GameData.EditorName + EditorToolsSubfolder.Project + EditorToolsSubfolder.NullValidator +
                  "/GameObjects Custom Assembly")]
        public static void VerifyProjectGameObjectsToNullInCustomAssembly()
        {
            var provider = Object.FindFirstObjectByType<SceneCheckForNullEditorProvider>();
            if (provider == null)
            {
                var go = new GameObject("Project Null Validator", typeof(SceneCheckForNullEditorProvider));
                Undo.RegisterCreatedObjectUndo(go, "Create GameObject");
                Selection.activeGameObject = go;

                Log.Info($"\"Custom Assembly\" provider not found! " +
                         $"To use \"Custom Assembly\" firstly configure {go.name} and try again", go);
                return;
            }

            var verifyMonoBehToNull = VerifyGameObjectsToNull(provider._accessible);
            provider.MarkVerifiedGameObjects(verifyMonoBehToNull);
        }
        
        
        [MenuItem(GameData.EditorName + EditorToolsSubfolder.Project + EditorToolsSubfolder.NullValidator +
                  "/Scriptable Objects")]
        private static void VerifyProjectScriptableObjectsToNull()
        {
            VerifyScriptableObjectsToNull(Accessible);
        }
        
        private static List<ScriptableObject> VerifyScriptableObjectsToNull(List<string> accessible)
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");
            var monoBehaviours = new List<ScriptableObject>();
            
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                
                var serializedObject = new SerializedObject(scriptableObject);
                var property = serializedObject.GetIterator();
                while (property.NextVisible(true))
                    if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                        Log.Warning($"SO:{scriptableObject.name}; path:{assetPath}; Property={property.name}", scriptableObject);
            }

            return monoBehaviours;
        }
    }
}