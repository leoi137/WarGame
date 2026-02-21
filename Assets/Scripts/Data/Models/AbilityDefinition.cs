using System;

[Serializable]
public class AbilityDefinition
{
    public string id;
    public string displayName;
    public AbilityTrigger trigger;
    public float triggerThreshold;
    public float cooldown;
    public float duration;
    public float damageModifier = 1f;
    public float speedModifier = 1f;
    public float armorModifier;
    public float incomingDamageModifier = 1f;
    public UnitCategory[] targetCategoryCondition;
    public TerrainType[] terrainCondition;
    public string specialCondition;
    public string description;
}
