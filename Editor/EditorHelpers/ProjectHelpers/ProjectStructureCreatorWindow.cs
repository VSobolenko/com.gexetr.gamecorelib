using System;
using System.IO;
using Game;
using Game.DynamicData;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace GameEditor.ProjectTools
{
internal sealed class ProjectStructureCreatorWindow : EditorWindow
{
    private readonly ProjectStructureCreator _structure = new();

    [MenuItem(GameData.EditorName + EditorSubfolder.Project + "/Create Project Structure")]
    private static void OpenWindow()
    {
        GetWindow<ProjectStructureCreatorWindow>("Create Project Structure");
    }

    private void OnEnable() => _structure.Name = Application.productName;

    private void OnGUI()
    {
        GUILayout.Label("Root Name", EditorStyles.boldLabel);
        _structure.Name = EditorGUILayout.TextField("Name:", _structure.Name);

        if (GUILayout.Button("Create"))
        {
            _structure.CreateFolders();
            _structure.CreateAsmdefs();
            _structure.CreateAssemblyInfo();
            Close();
        }
    }
}

public sealed class ProjectStructureCreator
{
    public string Name { get; set; } = "Default";

    private string CleanName => Name.Replace(" ", "");
    private string NameRoot => "_" + CleanName;
    private string PathRoot => Path.Combine("Assets", NameRoot);

    private static readonly string[] Folders =
    {
        "Code/Common",
        "Code/DI",
        "Code/Editor",
        "Code/Game",
        "Code/Tests/Editor",
        "Code/Tests/Runtime",
        "Code/UI",
        "DynamicAssets/Configs",
        "DynamicAssets/Prefabs",
        "DynamicAssets/Resources",
        "SandBox",
        "Scenes",
        "Shaders",
        "StaticAssets/Animations",
        "StaticAssets/Audio",
        "StaticAssets/Materials",
        "StaticAssets/Models",
        "StaticAssets/Sprites",
        "StaticAssets/Textures",
    };

    private static readonly AsmdefData[] Asmdefs =
    {
        new()
        {
            relativePath = ".",
            name = "{0}",
            rootNamespace = "{0}",
            references = new[]
            {
                "Unity.TextMeshPro", "Unity.Addressables", "Unity.ResourceManager",
                "Gexetr.GameCoreLib", "DOTween.Modules",
            },
            overrideReferences = true,
            precompiledReferences = new[] {"Newtonsoft.Json.dll", "DOTween.dll"},
        },
        new()
        {
            relativePath = "Code/Editor",
            name = "{0}.Editor",
            rootNamespace = "{0}Editor",
            includePlatforms = new[] {"Editor"},
            references = new[] {"Gexetr.GameCoreLib", "Gexetr.GameCoreLib.Editor", "Unity.Addressables.Editor"},
        },
        new()
        {
            relativePath = "Code/Tests/Runtime",
            name = "{0}.IntegrationTests",
            rootNamespace = "{0}.IntegrationTests",
            overrideReferences = true,
            precompiledReferences = new[] {"Newtonsoft.Json.dll", "nunit.framework.dll", "Moq.dll",},
            defineConstraints = new[] {"UNITY_INCLUDE_TESTS",},
        },
        new()
        {
            relativePath = "Code/Tests/Editor",
            name = "{0}.UnitTests",
            rootNamespace = "{0}.UnitTests",
            includePlatforms = new[] {"Editor"},
            overrideReferences = true,
            precompiledReferences = new[] {"Newtonsoft.Json.dll", "nunit.framework.dll", "Moq.dll",},
            defineConstraints = new[] {"UNITY_INCLUDE_TESTS",},
        },
    };

    public void CreateFolders()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Log.Errored("Folder name cannot be empty.");

            return;
        }

        var someDirectoryCreated = false;
        foreach (var folder in Folders)
        {
            var path = Path.Combine(PathRoot, folder);
            someDirectoryCreated |= SafeCreateDirectory(path);
        }

        AssetDatabase.Refresh();

        if (someDirectoryCreated)
            Log.Info($"Folder structure created at: {PathRoot}");
    }

    private static bool SafeCreateDirectory(string path)
    {
        if (Directory.Exists(path))
            return false; 

        Directory.CreateDirectory(path);

        return true;
    }

    public void CreateAsmdefs()
    {
        var cleanName = CleanName;

        foreach (var data in Asmdefs)
        {
            data.name = string.Format(data.name, cleanName);
            data.rootNamespace = string.Format(data.rootNamespace, cleanName);

            var path = Path.Combine(PathRoot, data.relativePath);
            var filePath = Path.Combine(path, data.name + ".asmdef");

            if (File.Exists(filePath))
            {
                Log.Warner($"Asmdef already exists: {filePath}");

                continue;
            }

            SafeCreateDirectory(path);

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Log.Info("Create .asmdef: " + filePath);
        }

        AssetDatabase.Refresh();
    }

    public void CreateAssemblyInfo()
    {
        const string fileName = "AssemblyInfo.cs";
        var assemblyInfoPath = Path.Combine(PathRoot, fileName);

        if (File.Exists(assemblyInfoPath))
        {
            Log.Warner($"{fileName} already exists: {assemblyInfoPath}");

            return;
        }

        var content = $@"
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(""{CleanName}"")]
[assembly: AssemblyVersion(""1.0.0"")]
[assembly: InternalsVisibleTo(""Assembly-CSharp"")]
[assembly: CLSCompliant(false)]
";

        File.WriteAllText(assemblyInfoPath, content);
        
        AssetDatabase.ImportAsset(assemblyInfoPath);
        AssetDatabase.Refresh();
        
        Log.Info($"{fileName} created at: {assemblyInfoPath}");
    }

    [Serializable]
    public class AsmdefData
    {
        public string name;
        [JsonProperty("rootNamespace")] public string rootNamespace;
        public string[] references = new string[0];
        public string[] includePlatforms = new string[0];
        public string[] excludePlatforms = new string[0];
        public bool allowUnsafeCode = false;
        public bool overrideReferences = false;
        public string[] precompiledReferences = new string[0];
        public bool autoReferenced = true;
        public string[] defineConstraints = new string[0];
        public VersionDefine[] versionDefines = new VersionDefine[0];
        public bool noEngineReferences = false;

        [JsonIgnore] public string relativePath;

        [System.Serializable]
        public class VersionDefine
        {
            public string name;
            public string expression;
            public string define;
        }
    }
}
}