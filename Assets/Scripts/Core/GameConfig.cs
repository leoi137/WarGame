/// <summary>
/// Central tuning constants for battle/world/campaign systems.
/// Keep values static for deterministic access and easy balancing.
/// </summary>
public static class GameConfig
{
    public static int DefaultMapSize = 120;
    public static float SimulationTickRate = 0.05f;
    public static float DefaultBattleSpeed = 1.0f;
    public static float MaxBattleSpeed = 4.0f;
    public static float PlacementZoneDepth = 0.35f;
    public static int MaxUnitsPerSide = 40;
    public static float BaseDetectionRange = 12f;
    public static float WorldMapWidth = 200f;
    public static float WorldMapHeight = 100f;
    public static int UnitBudgetScaleFactor = 5000;
    public static float TerrainHeightScale = 4f;
    public static float CountdownDuration = 3f;
}
