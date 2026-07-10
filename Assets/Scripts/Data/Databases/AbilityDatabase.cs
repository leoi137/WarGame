using System.Collections.Generic;

/// <summary>
/// Static registry of all combat abilities shared across factions.
/// Values sourced from faction-data.md COMPLETE ABILITY REFERENCE table.
/// </summary>
public static class AbilityDatabase
{
    private static readonly Dictionary<string, AbilityDefinition> _byId = new();
    private static readonly List<AbilityDefinition> _all = new();
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        _byId.Clear();
        _all.Clear();

        Register(new AbilityDefinition {
            id = "parry", displayName = "Parry",
            trigger = AbilityTrigger.OnCooldown, cooldown = 8f, duration = 1.5f,
            damageModifier = 1f, speedModifier = 1f, armorModifier = 0f,
            incomingDamageModifier = 0.4f,
            description = "Block 60% of incoming damage for 1.5s every 8s."
        });

        Register(new AbilityDefinition {
            id = "mark", displayName = "Mark Target",
            trigger = AbilityTrigger.OnCooldown, cooldown = 6f, duration = 5f,
            damageModifier = 1f, speedModifier = 1f, armorModifier = 0f,
            incomingDamageModifier = 1.4f,
            description = "Mark target for +40% damage from all sources for 5s."
        });

        Register(new AbilityDefinition {
            id = "berserker_rage", displayName = "Berserker Rage",
            trigger = AbilityTrigger.OnLowHP, triggerThreshold = 0.4f,
            cooldown = 0f, duration = 6f,
            damageModifier = 1.6f, speedModifier = 1.3f, armorModifier = -6f,
            incomingDamageModifier = 1f,
            description = "At 40% HP: +60% damage, +30% speed, -6 armor for 6s."
        });

        Register(new AbilityDefinition {
            id = "shield_wall", displayName = "Shield Wall",
            trigger = AbilityTrigger.OnNearEnemy, triggerThreshold = 10f,
            cooldown = 0f, duration = 0f,
            damageModifier = 1f, speedModifier = 0.3f, armorModifier = 15f,
            incomingDamageModifier = 1f,
            description = "Near enemies: +15 armor, -70% speed."
        });

        Register(new AbilityDefinition {
            id = "dual_strike", displayName = "Dual Strike",
            trigger = AbilityTrigger.OnAttack, triggerThreshold = 0.3f,
            cooldown = 0f, duration = 0f,
            damageModifier = 0.5f, speedModifier = 1f,
            description = "30% chance of bonus attack at 50% damage."
        });

        Register(new AbilityDefinition {
            id = "shield_bash", displayName = "Shield Bash",
            trigger = AbilityTrigger.OnAttack, triggerThreshold = 0.25f,
            cooldown = 3f, duration = 0f,
            description = "Push enemies back on hit."
        });

        Register(new AbilityDefinition {
            id = "fear_aura", displayName = "Fear Aura",
            trigger = AbilityTrigger.Passive, triggerThreshold = 0.3f,
            speedModifier = 0.7f,
            description = "Slow enemies below 30% HP."
        });

        Register(new AbilityDefinition {
            id = "elephant_charge", displayName = "Elephant Charge",
            trigger = AbilityTrigger.OnAttack, cooldown = 10f, duration = 2f,
            damageModifier = 2f, speedModifier = 1.5f,
            description = "Massive knockback and area damage on first contact."
        });

        Register(new AbilityDefinition {
            id = "horse_archer_kite", displayName = "Horse Archer Kite",
            trigger = AbilityTrigger.OnNearEnemy, triggerThreshold = 6f,
            speedModifier = 1.4f,
            description = "Auto-retreat while attacking when enemies close in."
        });

        Register(new AbilityDefinition {
            id = "pike_brace", displayName = "Pike Brace",
            trigger = AbilityTrigger.Passive,
            damageModifier = 2f,
            targetCategoryCondition = new[] { UnitCategory.HeavyCavalry, UnitCategory.LightCavalry },
            description = "+100% damage vs cavalry charges."
        });

        Register(new AbilityDefinition {
            id = "volley_fire", displayName = "Volley Fire",
            trigger = AbilityTrigger.OnCooldown, cooldown = 12f,
            damageModifier = 1.5f,
            description = "Mass volley: area damage."
        });

        Register(new AbilityDefinition {
            id = "fire_lance", displayName = "Fire Lance",
            trigger = AbilityTrigger.OnCooldown, cooldown = 8f,
            damageModifier = 2f,
            description = "Short-range gunpowder fire damage."
        });

        Register(new AbilityDefinition {
            id = "naval_boarding", displayName = "Naval Boarding",
            trigger = AbilityTrigger.Passive,
            damageModifier = 1.3f,
            terrainCondition = new[] { TerrainType.Coast },
            description = "Bonus damage on coast terrain."
        });

        Register(new AbilityDefinition {
            id = "war_cry", displayName = "War Cry",
            trigger = AbilityTrigger.OnCooldown, cooldown = 15f, duration = 4f,
            damageModifier = 1.2f, speedModifier = 1.15f,
            description = "Temporary speed + damage boost for nearby allies."
        });

        Register(new AbilityDefinition {
            id = "feigned_retreat", displayName = "Feigned Retreat",
            trigger = AbilityTrigger.OnCooldown, cooldown = 20f, duration = 3f,
            damageModifier = 1.5f, speedModifier = 1.4f, armorModifier = -3f,
            description = "Temporarily flee then turn and attack with bonus damage."
        });

        Register(new AbilityDefinition {
            id = "fortify", displayName = "Fortify",
            trigger = AbilityTrigger.OnNearEnemy, triggerThreshold = 8f,
            speedModifier = 0f, armorModifier = 8f,
            description = "Stand ground: +8 defense but cannot move."
        });

        Register(new AbilityDefinition {
            id = "skirmish", displayName = "Skirmish",
            trigger = AbilityTrigger.OnAttack, cooldown = 4f, duration = 1.5f,
            speedModifier = 1.5f,
            description = "Hit-and-run: attack then auto-disengage."
        });

        Register(new AbilityDefinition {
            id = "inspire", displayName = "Inspire",
            trigger = AbilityTrigger.OnCooldown, cooldown = 20f, duration = 6f,
            damageModifier = 1.2f,
            description = "Nearby allies gain +20% damage for a duration."
        });

        Register(new AbilityDefinition {
            id = "javelin_throw", displayName = "Javelin Throw",
            trigger = AbilityTrigger.OnCooldown, cooldown = 8f,
            damageModifier = 1.8f,
            description = "Ranged attack before melee engagement."
        });

        Register(new AbilityDefinition {
            id = "sling_barrage", displayName = "Sling Barrage",
            trigger = AbilityTrigger.OnCooldown, cooldown = 10f,
            damageModifier = 1.3f,
            description = "Long-range area suppression."
        });

        Register(new AbilityDefinition {
            id = "atlatl_volley", displayName = "Atlatl Volley",
            trigger = AbilityTrigger.OnCooldown, cooldown = 8f,
            damageModifier = 1.5f,
            description = "Medium-range area attack."
        });

        Register(new AbilityDefinition {
            id = "poison_arrow", displayName = "Poison Arrow",
            trigger = AbilityTrigger.OnAttack, triggerThreshold = 0.3f,
            cooldown = 6f, duration = 4f,
            speedModifier = 0.85f,
            description = "Damage-over-time and slow on hit."
        });

        Register(new AbilityDefinition {
            id = "zealot_charge", displayName = "Zealot Charge",
            trigger = AbilityTrigger.OnAttack, cooldown = 12f,
            damageModifier = 2f, speedModifier = 1.3f, armorModifier = -2f,
            description = "Ignores armor on first attack."
        });

        Register(new AbilityDefinition {
            id = "formation_discipline", displayName = "Formation Discipline",
            trigger = AbilityTrigger.Passive,
            armorModifier = 3f,
            specialCondition = "AllyWithin5m",
            description = "Reduced damage when near allied units."
        });

        Register(new AbilityDefinition {
            id = "ambush", displayName = "Ambush",
            trigger = AbilityTrigger.OnAttack,
            damageModifier = 2f,
            terrainCondition = new[] { TerrainType.Forest, TerrainType.Jungle },
            description = "First attack from forest/jungle terrain does double damage."
        });

        Register(new AbilityDefinition {
            id = "monsoon_tactics", displayName = "Monsoon Tactics",
            trigger = AbilityTrigger.Passive,
            damageModifier = 1.1f, speedModifier = 1.1f, armorModifier = 2f,
            terrainCondition = new[] { TerrainType.Wetlands, TerrainType.Jungle },
            description = "Bonus in wetlands/jungle terrain."
        });
    }

    private static void Register(AbilityDefinition def)
    {
        _byId[def.id] = def;
        _all.Add(def);
    }

    public static AbilityDefinition Get(string id)
    {
        if (!_initialized) Initialize();
        return _byId.TryGetValue(id, out var def) ? def : null;
    }

    public static List<AbilityDefinition> GetAll()
    {
        if (!_initialized) Initialize();
        return _all;
    }
}
