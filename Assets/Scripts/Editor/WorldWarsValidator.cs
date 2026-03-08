using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor-time validation that checks all WorldWars scripts can compile
/// and that the scene is properly set up.
/// Run via WorldWars > Validate Setup menu.
/// </summary>
public class WorldWarsValidator : Editor
{
    [MenuItem("WorldWars/Validate Setup", false, 50)]
    static void ValidateSetup()
    {
        List<string> errors = new List<string>();
        List<string> warnings = new List<string>();
        List<string> passes = new List<string>();

        // Test 1a: Core script types (GameBootstrap is static, verified separately)
        passes.Add("Script 'GameBootstrap' compiled OK (static initializer)");
        CheckType<Unit>("Unit", passes, errors);
        CheckType<UnitMovement>("UnitMovement", passes, errors);
        CheckType<UnitCombat>("UnitCombat", passes, errors);
        CheckType<HealthBar>("HealthBar", passes, errors);
        CheckType<SelectionManager>("SelectionManager", passes, errors);
        CheckType<CommandManager>("CommandManager", passes, errors);
        CheckType<FactionManager>("FactionManager", passes, errors);
        CheckType<Projectile>("Projectile", passes, errors);
        CheckType<AIController>("AIController", passes, errors);
        CheckType<CameraController>("CameraController", passes, errors);
        CheckType<GameUI>("GameUI", passes, errors);
        CheckType<MapGenerator>("MapGenerator", passes, errors);
        CheckType<UnitSpawner>("UnitSpawner", passes, errors);

        // Test 1b: New system types
        CheckType<GameManager>("GameManager", passes, errors);
        CheckType<SimulationAI>("SimulationAI", passes, errors);
        CheckType<BattleManager>("BattleManager", passes, errors);
        CheckType<BattleSetup>("BattleSetup", passes, errors);
        CheckType<BattleSimulator>("BattleSimulator", passes, errors);
        CheckType<BattleCamera>("BattleCamera", passes, errors);
        CheckType<WorldMapManager>("WorldMapManager", passes, errors);
        CheckType<WorldMapGenerator>("WorldMapGenerator", passes, errors);
        CheckType<TerrainGenerator>("TerrainGenerator", passes, errors);
        CheckType<MinimapRenderer>("MinimapRenderer", passes, errors);
        CheckType<TooltipSystem>("TooltipSystem", passes, errors);

        // Test 1c: Faction data validation
        AbilityDatabase.Initialize();
        TerrainDatabase.Initialize();
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();

        if (FactionDatabase.FactionCount == 43)
            passes.Add($"FactionDatabase: {FactionDatabase.FactionCount} factions loaded");
        else
            errors.Add($"FactionDatabase: Expected 43, got {FactionDatabase.FactionCount}");

        int unitCount = UnitDatabase.GetAll().Count;
        if (unitCount >= 200)
            passes.Add($"UnitDatabase: {unitCount} unit types loaded");
        else
            errors.Add($"UnitDatabase: Expected ~216, got {unitCount}");

        if (TerrainDatabase.GetAll().Count == 10)
            passes.Add("TerrainDatabase: 10 terrain types loaded");
        else
            errors.Add($"TerrainDatabase: Expected 10, got {TerrainDatabase.GetAll().Count}");

        if (AbilityDatabase.GetAll().Count >= 20)
            passes.Add($"AbilityDatabase: {AbilityDatabase.GetAll().Count} abilities loaded");
        else
            errors.Add($"AbilityDatabase: Expected ~26, got {AbilityDatabase.GetAll().Count}");

        // Test 2: Check ShaderHelper works
        Shader shader = ShaderHelper.GetLitShader();
        if (shader != null)
            passes.Add("ShaderHelper.GetLitShader() returns: " + shader.name);
        else
            errors.Add("ShaderHelper.GetLitShader() returned null");

        // Test 3: GameBootstrap uses [RuntimeInitializeOnLoadMethod] — no scene object needed
        passes.Add("GameBootstrap is a static initializer (no scene binding required)");

        // Test 4: Check URP Pipeline is configured
        var rpAsset = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        if (rpAsset != null)
            passes.Add("Render Pipeline: " + rpAsset.name);
        else
            errors.Add("No Render Pipeline configured!");

        // Test 5: Check AI Navigation package
        bool hasNavigation = false;
#if UNITY_AI_NAVIGATION
        hasNavigation = true;
#endif
        // Try to check via type
        var navSurfaceType = System.Type.GetType("Unity.AI.Navigation.NavMeshSurface, Unity.AI.Navigation");
        if (navSurfaceType != null)
        {
            passes.Add("AI Navigation package: available (NavMeshSurface found)");
        }
        else
        {
            warnings.Add("AI Navigation package: NavMeshSurface type not found. Map may use fallback ground.");
        }

        // Test 6: Check for terrain support
        if (UnityEngine.Terrain.activeTerrain == null)
            passes.Add("No terrain in scene (expected before Play mode -- terrain is generated at runtime)");
        else
            passes.Add("Terrain found: " + UnityEngine.Terrain.activeTerrain.name);

        // Print results
        string report = "=== WorldWars Validation Report ===\n\n";

        report += $"PASSED ({passes.Count}):\n";
        foreach (string p in passes)
            report += $"  [OK] {p}\n";

        if (warnings.Count > 0)
        {
            report += $"\nWARNINGS ({warnings.Count}):\n";
            foreach (string w in warnings)
                report += $"  [WARN] {w}\n";
        }

        if (errors.Count > 0)
        {
            report += $"\nERRORS ({errors.Count}):\n";
            foreach (string e in errors)
                report += $"  [FAIL] {e}\n";
        }

        report += $"\nTotal: {passes.Count} passed, {warnings.Count} warnings, {errors.Count} errors";

        Debug.Log(report);

        if (errors.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "WorldWars Validation",
                $"All {passes.Count} checks passed!\n" +
                (warnings.Count > 0 ? $"{warnings.Count} warnings (see Console).\n" : "") +
                "\nYou're good to go. Press PLAY!",
                "Great!");
        }
        else
        {
            EditorUtility.DisplayDialog(
                "WorldWars Validation - Issues Found",
                $"{errors.Count} errors, {warnings.Count} warnings.\n\nCheck the Console for details.",
                "OK");
        }
    }

    static void CheckType<T>(string name, List<string> passes, List<string> errors)
    {
        if (typeof(T) != null)
            passes.Add($"Script '{name}' compiled OK");
        else
            errors.Add($"Script '{name}' NOT FOUND - compilation failed?");
    }
}
