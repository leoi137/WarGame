using System;

[Serializable]
public class BattleConfiguration
{
    public FactionDefinition attackerFaction;
    public FactionDefinition defenderFaction;
    public CityDefinition battleLocation;
    public TerrainType primaryTerrain;
    public TerrainType secondaryTerrain;
    public int attackerUnitBudget;
    public int defenderUnitBudget;
    public int mapSize;
    public int randomSeed;
    public Faction? playerSide;

    public static BattleConfiguration Create(FactionDefinition attacker, FactionDefinition defender, CityDefinition location = null, Faction? playerSide = null)
    {
        var config = new BattleConfiguration
        {
            attackerFaction = attacker,
            defenderFaction = defender,
            battleLocation = location ?? defender?.GetCapital(),
            mapSize = GameConfig.DefaultMapSize,
            randomSeed = Environment.TickCount,
            playerSide = playerSide
        };

        config.primaryTerrain = config.battleLocation?.primaryTerrain ?? attacker?.GetDominantTerrain() ?? TerrainType.Plains;
        config.secondaryTerrain = config.battleLocation?.secondaryTerrain ?? TerrainType.Plains;
        config.attackerUnitBudget = attacker?.GetBattleUnitBudget() ?? 48;
        config.defenderUnitBudget = defender?.GetBattleUnitBudget() ?? 48;

        return config;
    }
}
