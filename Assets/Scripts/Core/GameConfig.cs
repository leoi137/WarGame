/// <summary>
/// Central tuning constants for battle/world/campaign systems.
/// Keep values static for deterministic access and easy balancing.
/// </summary>
public static class GameConfig
{
    public static int DefaultMapSize = 300;
    public static float SimulationTickRate = 0.05f;
    public static float DefaultBattleSpeed = 1.0f;
    public static float MaxBattleSpeed = 4.0f;
    public static float PlacementZoneDepth = 0.35f;
    public static int MaxUnitsPerSide = 800;
    public static float BaseDetectionRange = 18f;
    public static float WorldMapWidth = 200f;
    public static float WorldMapHeight = 100f;
    public static int UnitBudgetScaleFactor = 250;
    public static float TerrainHeightScale = 4f;
    public static float CountdownDuration = 3f;

    public static int FlockingThreshold = 200;
    public static int MaxFullLODUnits = 80;
    public static float LODFullDistance = 60f;
    public static float LODSimplifiedDistance = 120f;
    public static float LODBillboardDistance = 200f;
    public static float SpatialGridCellSize = 10f;
}
