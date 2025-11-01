#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;

public static class ProjectFoldersGenerator
{
    private const string Root = "Assets/_Project";

    [MenuItem("Tools/Project/Generate Folders")]
    public static void Generate()
    {
        string[] folders =
        {
            // Root
            $"{Root}",

            // Art
            $"{Root}/Art",
            $"{Root}/Art/Shared",
            $"{Root}/Art/Shared/MasterMaterials",
            $"{Root}/Art/Shared/Tex_Tilables",
            $"{Root}/Art/Shared/Decals",
            $"{Root}/Art/Shared/Atlases",
            $"{Root}/Art/Shared/HDRI",
            $"{Root}/Art/Shared/LUTs_Color",
            $"{Root}/Art/Shared/VFX_Shared",

            $"{Root}/Art/Characters",
            $"{Root}/Art/Props",
            $"{Root}/Art/Environment",

            $"{Root}/Art/UI_Art",
            $"{Root}/Art/UI_Art/_Source",
            $"{Root}/Art/UI_Art/_Export",
            $"{Root}/Art/UI_Art/Sprites",
            $"{Root}/Art/UI_Art/Icons",
            $"{Root}/Art/UI_Art/Materials",

            $"{Root}/Art/VFX",
            $"{Root}/Art/VFX/Graphs",
            $"{Root}/Art/VFX/Textures",
            $"{Root}/Art/VFX/Materials",
            $"{Root}/Art/VFX/Prefabs",

            // Audio
            $"{Root}/Audio",
            $"{Root}/Audio/Music",
            $"{Root}/Audio/SFX",
            $"{Root}/Audio/SFX/Player",
            $"{Root}/Audio/SFX/Weapons",
            $"{Root}/Audio/SFX/UI",
            $"{Root}/Audio/SFX/Ambience",
            $"{Root}/Audio/Mixers",

            // Code
            $"{Root}/Code",
            $"{Root}/Code/Runtime",
            $"{Root}/Code/Runtime/Core",
            $"{Root}/Code/Runtime/Input",
            $"{Root}/Code/Runtime/UI",
            $"{Root}/Code/Runtime/Systems",
            $"{Root}/Code/Runtime/Gameplay",
            $"{Root}/Code/Runtime/Networking",
            $"{Root}/Code/Runtime/Platform",
            $"{Root}/Code/Editor",
            $"{Root}/Code/Tests",

            // Data
            $"{Root}/Data",
            $"{Root}/Data/Characters",
            $"{Root}/Data/Weapons",
            $"{Root}/Data/Items",
            $"{Root}/Data/Balancing",
            $"{Root}/Data/Localization",
            $"{Root}/Data/Settings",

            // FX
            $"{Root}/FX",
            $"{Root}/FX/Shaders",
            $"{Root}/FX/ShaderGraphs",
            $"{Root}/FX/Particles",
            $"{Root}/FX/Profiles",

            // Levels
            $"{Root}/Levels",
            $"{Root}/Levels/Scenes",
            $"{Root}/Levels/Scenes/Boot",
            $"{Root}/Levels/Scenes/MainMenu",
            $"{Root}/Levels/Scenes/Lobby",
            $"{Root}/Levels/Scenes/Gameplay",
            $"{Root}/Levels/Additive",
            $"{Root}/Levels/SceneData",

            // Prefabs
            $"{Root}/Prefabs",
            $"{Root}/Prefabs/Characters",
            $"{Root}/Prefabs/Environment",
            $"{Root}/Prefabs/Props",
            $"{Root}/Prefabs/FX",
            $"{Root}/Prefabs/UI",

            // UI (runtime)
            $"{Root}/UI",
            $"{Root}/UI/Canvases",
            $"{Root}/UI/Prefabs",
            $"{Root}/UI/Fonts",
            $"{Root}/UI/Sprites",
            $"{Root}/UI/Materials",
            $"{Root}/UI/Atlases",

            // Settings
            $"{Root}/Settings",
            $"{Root}/Settings/InputSystem",
            $"{Root}/Settings/Physics",
            $"{Root}/Settings/Rendering",
            $"{Root}/Settings/BuildProfiles",
            $"{Root}/Settings/Addressables",

            // Misc
            $"{Root}/Docs",
            $"{Root}/Prototypes",
            $"{Root}/Sandbox",
            $"{Root}/Tools",

            // Outside _Project
            $"Assets/_Addressables",
            $"Assets/_Tools",
            $"Assets/Vendors",
            $"Assets/Gizmos",
            $"Assets/Resources",      // keep minimal; prefer Addressables
            $"Assets/StreamingAssets" // platform files (e.g., MR maps)
        };

        foreach (var path in folders.Where(p => !Directory.Exists(p)))
            Directory.CreateDirectory(path);

        // add .gitkeep to ensure empty folders get committed
        AddGitkeepRecursively("Assets");

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Generate Folders",
            "Project folders generated successfully.", "OK");
    }

    private static void AddGitkeepRecursively(string root)
    {
        foreach (var dir in Directory.GetDirectories(root, "*", SearchOption.AllDirectories))
        {
            // skip Library/Packages/etc if ever called above Assets
            if (!dir.Replace("\\", "/").StartsWith("Assets")) continue;

            var hasFiles = Directory.EnumerateFileSystemEntries(dir)
                .Any(p => File.Exists(p) && Path.GetFileName(p) != ".gitkeep");
            var gitkeepPath = Path.Combine(dir, ".gitkeep");

            if (!hasFiles && !File.Exists(gitkeepPath))
                File.WriteAllText(gitkeepPath, string.Empty);
        }
    }
}
#endif
