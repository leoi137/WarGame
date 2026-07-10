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
- Chainmail armor (PBR metallic) with faction-colored cloth tunic
- Classic Viking nasal helmet with gold band accent and steel rim
- Forward-held Viking sword (steel blade extending from gold crossguard and leather grip)
- Round wooden shield with iron boss and cross brace, held outward on left arm
- Leather boots and belt with gold buckle
- Exposed skin hands at end of chainmail sleeves
- Faction-colored cape with cloth physics simulation

### Stats Rationale
The Huscarl is the all-rounder. Decent HP (110), balanced attack (14), and light armor (4) make him effective in any situation without excelling at extremes. His fast attack speed (0.9s) gives consistent damage output.

### Special Ability: Parry
- **Type:** Passive / Auto-activate
- **Mechanic:** Every 8 seconds during combat, the Swordsman automatically enters a 1.5s parry stance
- **Effect:** Blocks 60% of incoming damage during the parry window
- **Cooldown:** 8 seconds
- **Visual:** Weapon emits a brief glow during parry window; sparks on successful parry

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
- Bow held forward in left hand (staves vertical, string behind grip)
- Quiver with arrow tips visible on back
- Leather bracers on both forearms
- Dark fur boots and belt
- Short faction-colored cloak with cloth physics

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
- Bare-chested with war paint (dark red cross on chest) and visible scars
- Bear/wolf fur cloak draped over shoulders with dark fur collar
- Wild blonde hair (3 overlapping sphere tufts), thick beard
- Fierce red-glowing eyes
- Gold armbands on both upper arms
- DUAL AXES -- iron axe heads on wooden handles, held forward from each hand
- Bone skull on leather belt buckle
- Dark fur trousers and heavy leather boots
- Largest, widest build of any unit (widened skeleton shoulders and hips)

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
- Full chainmail armor (PBR metallic) with faction-colored surcoat and gold cross emblem
- Viking spectacle helm with dome, face plate, and raised crest
- Iron shoulder pauldrons on both arms
- LARGE round shield (biggest in the game) -- wooden disc with iron rim (top/bottom), center boss, and faction cross emblem, held outward on left arm
- Long spear in right hand extending forward with steel head and fine edge tip
- Stout, wide stance (widened skeleton shoulders and hips)
- Heaviest-armored unit
- Faction-colored cape with cloth physics

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

For the full technical guide on skeleton construction, material system, weapon positioning conventions, and step-by-step instructions for building a new character, see **[character-building.md](character-building.md)**.

Quick checklist:

1. **Add to enum** in `Unit.cs`
2. **Add stats** in `Unit.ApplyStats()`
3. **Create model builder** -- `BuildNewTypeModel()` in `Unit.cs` using the 14-joint pivot skeleton
4. **Add combat behavior** -- `AttackAsNewType()` in `UnitCombat.cs`
5. **Add animations** -- idle, walk, attack cases in `UnitAnimator.cs`
6. **Add weapon trail** -- preset in `TrailEffect.cs`
7. **Add AI behavior** -- `HandleNewTypeAI()` in `AIController.cs`
8. **Wire up spawner** -- pivot references in `UnitSpawner.cs`
9. **Update UI** -- name/color in `HealthBar.cs` and breakdown in `GameUI.cs`

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

## 20× Scale: Army Composition

With the unit budget scaling upgrade (`UnitBudgetScaleFactor = 250`, `MaxUnitsPerSide = 800`), each faction's battle army size is proportional to its historical military strength. The 4 Viking unit types are distributed across the North Sea Empire's 192-unit battle army as follows:

| Unit Type | Typical % | Count (192 total) | Formation Role |
|-----------|-----------|-------------------|----------------|
| Shieldbearer | 25% | ~48 | Front line, absorbs charges |
| Huscarl Swordsman | 30% | ~58 | Main battle line, versatile |
| Berserker | 20% | ~38 | Flanks and shock attacks |
| Norse Hunter | 25% | ~48 | Rear line, ranged support |

During the interactive **Battle Setup** phase, the player can:
- Drag-select groups of units and reposition them with right-click
- Use formation presets (F1=Line, F2=Column, F3=Wedge, F4=Square, F5=Spread)
- Rotate formations with R key
- Quick-select all units of a type with double-click or number keys

The opponent's army is auto-placed by AI on the other side of the battlefield.

---

## Version History

- **v0.4** -- 20× unit scale: armies of 48-800 units. Interactive placement system with drag-select, formation presets, rotation. LOD system for performance at scale.
- **v0.3** -- Premium graphics overhaul: PBR materials, forward-held weapons, skeletal animation, cloth physics, weapon trails, enhanced VFX
- **v0.2** -- Viking overhaul: 4 distinct characters with unique abilities, visual identities, and AI behaviors
- **v0.1** -- Initial MVP: 2 generic unit types (Swordsman, Archer) with basic block models
