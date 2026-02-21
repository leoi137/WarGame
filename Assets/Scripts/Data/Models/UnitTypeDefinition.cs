using System;

[Serializable]
public class UnitTypeDefinition
{
    public string id;
    public string factionId;
    public string displayName;
    public UnitCategory category;
    public float maxHP;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float moveSpeed;
    public float armor;
    public string abilityId;
    public UnitVisualConfig visualConfig = new();
    public string description;
}
