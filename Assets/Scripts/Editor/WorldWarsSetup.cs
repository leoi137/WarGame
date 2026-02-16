using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

public class WorldWarsSetup : Editor
{
    [MenuItem("WorldWars/Setup Scene (One Click!)", false, 0)]
    static void SetupScene()
    {
        if (!EditorUtility.DisplayDialog(
            "WorldWars Setup",
            "This will:\n" +
            "1. Create a 3D Forward Renderer (if needed)\n" +
            "2. Clear the current scene\n" +
            "3. Add the GameBootstrap object\n\n" +
            "Continue?",
            "Yes, Set Up!",
            "Cancel"))
        {
            return;
        }

        // Step 1: Ensure we have a proper 3D renderer
        EnsureForwardRenderer();

        // Step 2: Clear the scene
        var allObjects = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var t in allObjects)
        {
            if (t != null && t.parent == null)
            {
                Object.DestroyImmediate(t.gameObject);
            }
        }

        // Step 3: Create GameBootstrap
        GameObject bootstrap = new GameObject("GameBootstrap");
        bootstrap.AddComponent<GameBootstrap>();

        // Save the scene so Restart works (SceneManager.LoadScene needs a saved scene)
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        // Add scene to Build Settings so SceneManager.LoadScene works
        AddSceneToBuildSettings(EditorSceneManager.GetActiveScene().path);

        Debug.Log("WorldWars: Scene setup complete! Press PLAY to start the game.");
        EditorUtility.DisplayDialog(
            "WorldWars - Ready!",
            "Scene is set up!\n\n" +
            "Now press the PLAY button (triangle at top) to start the game.\n\n" +
            "Controls:\n" +
            "- Left-click: Select blue units\n" +
            "- Right-click ground: Move\n" +
            "- Right-click enemy: Attack\n" +
            "- WASD: Pan camera\n" +
            "- Scroll: Zoom",
            "Got it!");

        Selection.activeGameObject = bootstrap;
    }

    static void EnsureForwardRenderer()
    {
        // Check if we already have a ForwardRenderer asset
        string rendererPath = "Assets/Settings/ForwardRenderer.asset";
        var existingRenderer = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(rendererPath);

        if (existingRenderer != null && existingRenderer is UniversalRendererData)
        {
            Debug.Log("WorldWars: Forward Renderer already exists.");
        }
        else
        {
            // Create a new UniversalRendererData (Forward Renderer)
            var forwardRenderer = ScriptableObject.CreateInstance<UniversalRendererData>();

            // Ensure directory exists
            if (!Directory.Exists("Assets/Settings"))
                Directory.CreateDirectory("Assets/Settings");

            AssetDatabase.CreateAsset(forwardRenderer, rendererPath);
            AssetDatabase.SaveAssets();
            Debug.Log("WorldWars: Created Forward Renderer at " + rendererPath);

            existingRenderer = forwardRenderer;
        }

        // Now assign it to the URP Pipeline asset
        string pipelinePath = "Assets/Settings/UniversalRP.asset";
        var pipelineAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);

        if (pipelineAsset != null)
        {
            // Use SerializedObject to modify the renderer list
            var serializedPipeline = new SerializedObject(pipelineAsset);
            var rendererListProp = serializedPipeline.FindProperty("m_RendererDataList");

            if (rendererListProp != null)
            {
                // Check if forward renderer is already in the list
                bool alreadyAssigned = false;
                for (int i = 0; i < rendererListProp.arraySize; i++)
                {
                    var elem = rendererListProp.GetArrayElementAtIndex(i);
                    if (elem.objectReferenceValue == existingRenderer)
                    {
                        alreadyAssigned = true;
                        // Set this as default
                        serializedPipeline.FindProperty("m_DefaultRendererIndex").intValue = i;
                        break;
                    }
                }

                if (!alreadyAssigned)
                {
                    // Add forward renderer to the list
                    int newIndex = rendererListProp.arraySize;
                    rendererListProp.InsertArrayElementAtIndex(newIndex);
                    rendererListProp.GetArrayElementAtIndex(newIndex).objectReferenceValue = existingRenderer;
                    serializedPipeline.FindProperty("m_DefaultRendererIndex").intValue = newIndex;
                }

                serializedPipeline.ApplyModifiedProperties();
                EditorUtility.SetDirty(pipelineAsset);
                AssetDatabase.SaveAssets();
                Debug.Log("WorldWars: URP Pipeline updated to use Forward Renderer.");
            }
        }
        else
        {
            Debug.LogWarning("WorldWars: Could not find URP Pipeline asset at " + pipelinePath);
        }
    }

    static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        bool found = false;
        foreach (var s in scenes)
        {
            if (s.path == scenePath)
            {
                s.enabled = true;
                found = true;
                break;
            }
        }
        if (!found)
        {
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("WorldWars: Scene added to Build Settings: " + scenePath);
    }

    [MenuItem("WorldWars/How to Play", false, 100)]
    static void HowToPlay()
    {
        EditorUtility.DisplayDialog(
            "WorldWars - How to Play",
            "1. Click 'WorldWars > Setup Scene' in the menu bar\n" +
            "2. Press PLAY\n\n" +
            "Controls:\n" +
            "- Left-click: Select your (blue) units\n" +
            "- Shift+click: Add to selection\n" +
            "- Drag box: Select multiple units\n" +
            "- Right-click ground: Move selected units\n" +
            "- Right-click enemy (red): Attack\n" +
            "- WASD / Arrows: Pan camera\n" +
            "- Mouse wheel: Zoom in/out\n\n" +
            "Goal: Destroy all enemy (red) units!",
            "OK");
    }
}
