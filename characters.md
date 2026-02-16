# WorldWars - Characters & Units

This document describes all unit types in WorldWars, their stats, abilities, and tactical roles. Use this as a reference when balancing or adding new units.

---

## Factions

### Kingdom of the North (Player)
- **Color**: Blue (#3366CC)
- **Helmet accent**: Dark blue
- **Spawn side**: Left side of map
- **Controlled by**: Player (mouse input)

### Southern Empire (AI)
- **Color**: Red (#CC3333)
- **Helmet accent**: Dark red
- **Spawn side**: Right side of map
- **Controlled by**: AI opponent

---

## Unit Types

### Swordsman

> *Heavy melee fighter. The backbone of any army. Slow but durable, they absorb damage and deal punishment up close.*

| Stat | Value |
|------|-------|
| **HP** | 100 |
| **Attack Damage** | 15 |
| **Attack Range** | 2.5 units |
| **Attack Cooldown** | 1.0 seconds |
| **Move Speed** | 3.5 units/sec |
| **DPS** | 15.0 |
| **Role** | Front-line melee tank |

**Visual**: Blue/red torso, skin-tone head and arms, silver sword block held in right hand, faction-colored helmet.

**Combat behavior**:
- Deals instant melee damage on each attack
- Spawns a yellow flash effect on hit
- Must be within 2.5 units of target to attack
- Auto-engages enemies within detection range (8 units minimum)

**AI behavior** (when AI-controlled):
- Charges directly at the nearest enemy unit
- Always prioritized for front-line assault

**Tactical tips**:
- Use swordsmen as your front line to absorb damage
- Send them ahead of archers to protect the squishier units
- Focus-fire by selecting all swordsmen and right-clicking one enemy

---

### Archer

> *Fragile ranged attacker. Deadly at distance but vulnerable in close combat. Keep them behind your swordsmen.*

| Stat | Value |
|------|-------|
| **HP** | 60 |
| **Attack Damage** | 10 |
| **Attack Range** | 12.0 units |
| **Attack Cooldown** | 1.5 seconds |
| **Move Speed** | 4.0 units/sec |
| **DPS** | 6.67 |
| **Role** | Ranged damage dealer |

**Visual**: Blue/red torso, skin-tone head and arms, brown bow block held in right hand, faction-colored helmet.

**Combat behavior**:
- Fires arrow projectiles that arc toward the target
- Arrows travel at 15 units/sec with a parabolic arc
- Arrow deals damage on impact, then self-destructs
- Can engage from 12 units away -- nearly 5x the swordsman's range
- Auto-engages enemies within detection range (24 units for archers)

**AI behavior** (when AI-controlled):
- Stays behind swordsmen (offsets 6 units back from target)
- Retreats if an enemy gets within 60% of attack range (7.2 units)
- Repositions to maintain optimal firing distance

**Tactical tips**:
- Position archers behind your swordsmen
- Their long range lets them deal damage before enemies close in
- They're faster than swordsmen (4.0 vs 3.5) so they can kite
- Fragile at only 60 HP -- protect them from enemy swordsmen

---

## Unit Comparison

| Stat | Swordsman | Archer |
|------|-----------|--------|
| HP | **100** | 60 |
| Attack | **15** | 10 |
| Range | 2.5 | **12.0** |
| Cooldown | **1.0s** | 1.5s |
| Speed | 3.5 | **4.0** |
| DPS | **15.0** | 6.67 |
| Attack Type | Melee (instant) | Ranged (projectile) |
| Role | Tank / Front-line | DPS / Back-line |
| Survivability | High | Low |

---

## Spawn Configuration

Each faction spawns with:
- **3 Swordsmen** -- positioned in the front line
- **3 Archers** -- positioned in the back line

Total: **6 units per faction**, **12 units on the battlefield**.

Spacing: 3 units between each unit in formation.

---

## Common Stats (All Units)

| Property | Value |
|----------|-------|
| Detection range | 2x attack range (minimum 8 units) |
| NavMesh pathfinding | Yes (agent-based) |
| Collision radius | 0.4 units |
| Collision height | 2.2 units |
| Death delay | 0.5 seconds (before removal) |

---

## Planned Future Units

These units are not yet implemented but are planned for future updates:

### Cavalry (Planned)
- Mounted horseman with high speed and charge damage
- Expected stats: HP 80, Attack 20, Range 3, Speed 7.0, Cooldown 1.2s
- Role: Flanking, hit-and-run

### Siege Engine (Planned)
- Slow catapult with massive range and area damage
- Expected stats: HP 150, Attack 40, Range 25, Speed 1.5, Cooldown 4.0s
- Role: Structure destroyer, area denial

### Mage (Planned)
- Ranged magic user with area-of-effect spells
- Expected stats: HP 50, Attack 25 (AoE), Range 15, Speed 3.0, Cooldown 3.0s
- Role: AoE damage, support

### Shield Bearer (Planned)
- Defensive unit that reduces incoming damage for nearby allies
- Expected stats: HP 120, Attack 8, Range 2, Speed 3.0, Cooldown 1.5s
- Role: Tank, damage reduction aura

---

## Modifying Unit Stats

Unit stats are defined in `Assets/Scripts/Unit.cs` in the `ApplyStats()` method:

```csharp
public void ApplyStats()
{
    if (unitType == UnitType.Swordsman)
    {
        maxHealth = 100f;
        attackDamage = 15f;
        attackRange = 2.5f;
        attackCooldown = 1.0f;
        moveSpeed = 3.5f;
    }
    else // Archer
    {
        maxHealth = 60f;
        attackDamage = 10f;
        attackRange = 12f;
        attackCooldown = 1.5f;
        moveSpeed = 4.0f;
    }
}
```

To add a new unit type:
1. Add the type to the `UnitType` enum in `Unit.cs`
2. Add a new stats block in `ApplyStats()`
3. Add visual building logic in `BuildBlockModel()`
4. Update `UnitSpawner.cs` to spawn the new type
5. Update AI behavior in `AIController.cs` if needed
6. Update this document
