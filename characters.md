# World Wars - Viking Characters

A detailed breakdown of all unit types in the Viking warband system, including stats, abilities, visual identity, combat behavior, and tactical roles.

---

## Unit Roster

| Stat | Huscarl Swordsman | Norse Hunter (Archer) | Berserker | Shieldbearer |
|------|-------------------|-----------------------|-----------|--------------|
| HP | 110 | 55 | 85 | 140 |
| Attack | 14 | 11 | 22 | 8 |
| Range | 2.5 (melee) | 14.0 (ranged) | 2.8 (melee) | 3.0 (melee) |
| Cooldown | 0.9s | 1.4s | 0.7s | 1.6s |
| Move Speed | 3.8 | 4.2 | 4.0 | 3.0 |
| Armor | 4 | 0 | 2 | 8 (+15 in wall) |
| Role | Balanced fighter | Ranged DPS / Scout | Glass cannon | Tank / Protector |

---

## Huscarl Swordsman

**Role:** Balanced melee fighter
**Playstyle:** Reliable, versatile, the backbone of any Viking crew

### Visual Design
- Chainmail armor with faction-colored tunic
- Classic Viking helmet with nasal guard and gold band
- Sword + round wooden shield with iron boss
- Leather boots and belt
- Exposed hands for dexterity

### Stats Rationale
The Huscarl is the all-rounder. Decent HP (110), balanced attack (14), and light armor (4) make him effective in any situation without excelling at extremes. His fast attack speed (0.9s) gives consistent damage output.

### Special Ability: Parry
- **Type:** Passive / Auto-activate
- **Mechanic:** Every 8 seconds during combat, the Swordsman automatically enters a 1.5s parry stance
- **Effect:** Blocks 60% of incoming damage during the parry window
- **Cooldown:** 8 seconds
- **Visual:** None (internal state)

### Combat Behavior
- Engages the nearest enemy directly
- AI swordsmen charge when in aggro range
- Good in formation with other melee units
- Can adapt to various threats

### Tactical Tips
- Use as your main engagement force
- Position between shieldbearers and archers
- Their parry makes them surprisingly tanky in sustained fights
- Pair with Berserkers for burst + sustain

---

## Norse Hunter (Archer)

**Role:** Ranged DPS / Scout / Debuffer
**Playstyle:** Fast, precise, lethal from a distance

### Visual Design
- Light leather armor -- slimmer build than other units
- Hood in faction colors with brim for shade
- Bow with visible string held in left hand
- Quiver with arrows visible on back
- Leather bracers on both arms
- Dark fur boots and belt

### Stats Rationale
Low HP (55) and zero armor make the Hunter fragile, but 14-range and 4.2 move speed mean she can stay safe. The 11 damage per shot becomes devastating with the Mark ability amplifying all incoming damage.

### Special Ability: Mark Target
- **Type:** Passive / On-hit
- **Mechanic:** Every 6 seconds, the next arrow hit applies a Mark to the target
- **Effect:** Marked targets take 40% bonus damage from ALL sources for 5 seconds
- **Visual:** Orange diamond floating above the marked unit's head
- **Cooldown:** 6 seconds per mark

### Combat Behavior
- Stays at maximum range
- AI archers retreat if enemies get within 50% of their range
- Prioritizes marked targets, low-HP enemies, and high-value units (other archers, enraged berserkers)
- De-prioritizes shieldbearers in shield wall
- Carries arrows that arc toward targets

### Tactical Tips
- Keep behind your melee line at all times
- Mark priority targets before your Berserkers charge in
- Marked + Berserker Rage = devastating burst damage
- Run immediately if a melee unit closes distance
- Best synergy with Shieldbearers who hold the line

---

## Berserker

**Role:** Heavy melee / Glass cannon
**Playstyle:** Aggressive, high risk--high reward

### Visual Design
- Bare-chested with war paint (dark red stripe across chest)
- Bear/wolf fur cloak draped over shoulders with dark fur collar
- Wild blonde hair flowing back, thick beard
- Gold armbands on both arms
- DUAL AXES -- iron axe heads on wooden handles
- Skull on belt buckle
- Dark fur trousers and boots
- Largest build of any unit

### Stats Rationale
High attack (22) and fast cooldown (0.7s) deliver the highest burst DPS in the game. 85 HP and only 2 armor means they can't sustain prolonged combat. The Rage ability pushes damage even further at the cost of durability.

### Special Ability: Berserker Rage
- **Type:** Passive / Auto-trigger at low HP, or on AI engagement
- **Mechanic:** Activates automatically when HP drops below 40%
- **Effect:**
  - +60% attack damage (22 -> 35.2 while enraged)
  - +30% movement speed (4.0 -> 5.2 while enraged)
  - -40% armor (already low, becomes nearly zero)
- **Duration:** 6 seconds
- **Visual:** Red glowing aura around the Berserker
- **Cooldown:** Once per life (auto-rages have a single trigger; AI berserkers rage on engagement)

### Secondary Ability: Dual Axe Swing
- **Type:** Passive / Chance-based
- **Mechanic:** 30% chance each attack to swing the second axe
- **Effect:** Extra hit dealing 50% of normal attack damage
- **Visual:** Double hit effect (orange burst + sparks)

### Secondary Ability: Fear
- **Type:** Passive / On-hit
- **Mechanic:** Enemies below 30% HP are slowed by 30% on hit
- **Effect:** Target moves slower, making it hard to escape

### Combat Behavior
- AI berserkers activate Rage as soon as they close to 12 units of an enemy
- Prioritize weak (low HP) enemies for the kill
- Charge directly at targets -- no kiting or retreating
- In-combat rage trigger if HP drops below 60% (AI only)

### Tactical Tips
- Don't send Berserkers in first -- they'll die fast
- Wait for Shieldbearers to engage, then unleash Berserkers
- Berserker + Archer Mark on the same target = maximum burst
- They're expendable but devastating -- treat them as shock troops
- A single Berserker can clean up 2-3 weakened enemies in a rage

---

## Shieldbearer

**Role:** Tank / Team protector
**Playstyle:** Defensive, tactical, the immovable wall

### Visual Design
- Full chainmail armor with faction-colored surcoat
- Gold cross emblem on surcoat (front)
- Viking spectacle helm with crest and face plate
- Iron shoulder pauldrons on both arms
- LARGE round shield (biggest in the game) -- wooden with iron rim, boss, and faction emblem
- Spear in right hand (extends above head height)
- Stout, wide stance
- Heaviest-armored unit

### Stats Rationale
Highest HP (140) and armor (8, or 23 in shield wall) make this the tankiest unit. Low attack (8) and slow speed (3.0) balance this out. The Shield Wall ability makes them nearly immovable.

### Special Ability: Shield Wall
- **Type:** Passive / Auto-activate near enemies
- **Mechanic:** Automatically activates when within double attack range of enemies
- **Effect:**
  - +15 bonus armor (total: 23 armor in shield wall)
  - -70% movement speed (nearly immobile)
- **Visual:** Golden shield aura in front of the Shieldbearer
- **Toggle:** Can be toggled on/off; AI activates when 2+ enemies are nearby
- **Notes:** Total damage reduction with 23 armor significantly reduces incoming damage

### Secondary Ability: Shield Bash
- **Type:** Passive / On-hit
- **Mechanic:** Every attack pushes the enemy back slightly
- **Effect:** Target is pushed 0.5 units in the direction away from the Shieldbearer
- **Visual:** White/gold impact ring at the bash point

### Combat Behavior
- AI shieldbearers advance to the front line first
- Activate Shield Wall when near enemy units
- Act as a wall that other units fight behind
- Push back enemies trying to break through
- Will hold position rather than chase

### Tactical Tips
- Always position Shieldbearers in front of your formation
- Their Shield Wall absorbs enormous amounts of damage
- Pair with Archers behind them for the safest ranged damage
- Don't expect them to get kills -- their job is to absorb and delay
- A Shieldbearer + Shield Wall can tank 3-4 enemies simultaneously

---

## Team Synergy

### Optimal Formation

```
[BACK]    Archer    Archer
[MID]   Swordsman  Swordsman  Berserker  Berserker
[FRONT] Shieldbearer  Shieldbearer
```

### Combo Strategies

| Combo | Description |
|-------|-------------|
| Shield + Archer | Shieldbearer holds the line, Archer fires safely from behind |
| Mark + Rage | Archer marks a target, then Berserker rages and destroys it |
| Shield Wall + Swordsmen | Shieldbearers absorb, Swordsmen parry and deal steady damage |
| Full Push | Shieldbearers advance, everyone follows in formation |

### Why This Squad Works
- **Shieldbearer** holds enemies in place with Shield Wall
- **Berserker** smashes through front lines with Rage + Dual Axes
- **Swordsman** adapts to any threat with Parry + balanced stats
- **Archer** picks off targets safely and amplifies team damage with Mark

---

## Adding New Unit Types

To add a new Viking character:

1. **Add to enum** in `Unit.cs`:
   ```csharp
   public enum UnitType { Swordsman, Archer, Berserker, Shieldbearer, NewType }
   ```

2. **Add stats** in `Unit.ApplyStats()`:
   ```csharp
   case UnitType.NewType:
       maxHealth = 100f;
       attackDamage = 12f;
       // ... etc
       break;
   ```

3. **Add visual model** -- create `BuildNewTypeModel()` in `Unit.cs` and add to the switch in `BuildBlockModel()`

4. **Add combat behavior** -- create `AttackAsNewType()` in `UnitCombat.cs` and add to the switch in `Attack()`

5. **Add AI behavior** -- create `HandleNewTypeAI()` in `AIController.cs`

6. **Update spawner** -- add `newTypeCount` field in `UnitSpawner.cs` and spawn in formation

7. **Update UI** -- add name/color in `HealthBar.cs` and breakdown in `GameUI.cs`

---

## Planned Future Units

| Unit | Role | Signature Ability |
|------|------|-------------------|
| Skald (Bard) | Support / Buffer | War Chant -- buffs nearby allies' attack speed |
| Jarl (Chieftain) | Leader / Aura | Command Presence -- all nearby allies get +10% damage |
| Shaman (Volva) | Healer / Debuffer | Rune of Healing -- slowly heals nearby allies |
| Raider | Flanker / Assassin | Ambush -- bonus damage from behind, fast movement |
| Huscarl Elite | Heavy Infantry | Shield + Axe combo, stronger than base Swordsman |
| War Dog | Companion | Fast scout, low HP, causes fear |

---

## Version History

- **v0.2** -- Viking overhaul: 4 distinct characters with unique abilities, visual identities, and AI behaviors
- **v0.1** -- Initial MVP: 2 generic unit types (Swordsman, Archer) with basic block models
