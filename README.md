# WorldWars - Viking Conquest

A real-time strategy game built in Unity 6 where two Viking warbands clash on a procedurally generated battlefield. Command your Norse warriors -- Huscarls, Hunters, Berserkers, and Shieldbearers -- to destroy the rival clan.

## Quick Start

1. Open the project in **Unity 6** (6000.3.8f1 or compatible)
2. Go to menu **WorldWars > Setup Scene (One Click!)**
3. Press **Play**

## Controls

| Action | Input |
|--------|-------|
| Select unit | Left-click on a blue unit |
| Select multiple | Left-click and drag a box around units |
| Add to selection | Shift + left-click |
| Move units | Right-click on the ground |
| Attack enemy | Right-click on a red unit |
| Pan camera | WASD or Arrow keys |
| Zoom | Mouse scroll wheel |

## How to Play

The game opens with a **Main Menu** offering two options:
- **START BATTLE** -- Jump into a full battle against the AI
- **VIEW UNITS** -- Browse all unit types up close with an orbit camera, cycle through them, and preview their animations

In battle, you command the **Norse Warband** (blue units) on the north side of the map. The **Rival Clan** (red units) on the south side is controlled by AI.

Select your units, right-click to move or attack, and destroy all enemy warriors to win. If you lose all your units, the AI wins.

### Your Army

| Unit | Role | Signature |
|------|------|-----------|
| **Huscarl Swordsman** | Balanced fighter | Parries attacks every 8s (-60% damage) |
| **Norse Hunter** | Ranged DPS | Marks enemies (+40% damage from all sources) |
| **Berserker** | Glass cannon | Rages at low HP (+60% DMG, +30% SPD) |
| **Shieldbearer** | Tank / Protector | Shield Wall (+15 armor, -70% speed) |

For full stats, abilities, and tactical tips, see **[characters.md](Docs/characters.md)**. For the technical guide on how the procedural unit system works (skeleton, pivots, materials, animations), see **[character-building.md](Docs/character-building.md)**.

## Game Features

### Procedural Map
- 100x100 terrain with Perlin noise hills
- Grass and dirt terrain painting
- River dividing the two territories
- Minecraft-style block trees scattered across the map
- NavMesh baked at runtime for pathfinding

### Factions
- **Norse Warband** (Blue) -- player-controlled
- **Rival Clan** (Red) -- AI-controlled with tactical decision-making

### 4 Viking Unit Types
Each unit is procedurally built at runtime using a premium low-poly style. Models use a hierarchical skeleton with 14 pivot joints for rotation-based animation, PBR materials (metallic, smoothness, emission), and a mix of spheres, cylinders, and cubes for organic proportions:
- **Huscarl Swordsman** -- chainmail + faction tunic, Viking nasal helm with gold band, forward-held sword + round shield with iron boss
- **Norse Hunter** -- leather armor with hood, bow + quiver on back, bracers, slim build
- **Berserker** -- bare chest with war paint + scars, fur cloak, wild hair, dual forward-held axes, gold armbands, skull belt buckle
- **Shieldbearer** -- full chainmail with surcoat and cross emblem, spectacle helm, massive shield + forward-held spear, shoulder pauldrons

### Abilities System
- **Berserker Rage** -- auto-triggers at 40% HP, +60% damage, +30% speed, -40% armor for 6s
- **Shield Wall** -- auto-activates near enemies, +15 armor but nearly immobile
- **Archer Mark** -- every 6s marks an enemy for +40% damage from all sources for 5s
- **Swordsman Parry** -- every 8s, blocks 60% damage for 1.5s
- **Dual Axe** -- Berserker has 30% chance of bonus swing (50% extra damage)
- **Shield Bash** -- Shieldbearer pushes enemies back on hit
- **Fear** -- Berserker slows enemies below 30% HP

### Combat System
- Units auto-attack enemies within detection range
- Armor system: flat damage reduction on each hit
- Mark debuff amplifies all incoming damage by 40%
- Health bars with unit type labels, color-coded by faction
- Damage-type specific visual effects (slash arcs, axe burst rings, shield bash waves)
- Weapon trail effects during attacks (sword gleam, axe arc, spear thrust, arrow trail)
- Floating damage numbers with color-coding (marked, armor-reduced, critical)
- Camera shake on heavy impacts (shield bash, deaths)
- Arrows stick into targets on hit with splinter particles
- Multi-phase death animations with ragdoll-style fall and weapon detachment

### Smart AI Opponent
- **Shieldbearers** advance to front line and activate Shield Wall
- **Berserkers** rage on engagement and charge weak enemies
- **Swordsmen** fight the nearest threat adaptively
- **Archers** maintain distance, target marked/low-HP enemies, retreat if flanked
- Evaluates army strength to decide aggression level
- Decisions every 1.5 seconds with 25-unit aggro range

### Unit Viewer
- Pre-battle unit showcase accessible from the main menu
- Orbit camera to inspect each unit type up close
- Cycle through all unit types
- Play idle, walk, and attack animations on demand

### UI
- Main menu with Start Battle and View Units options
- HUD showing unit counts with type breakdown per faction
- Selected unit info with stats (HP, ATK, ARM, SPD) and active status effects
- Ability hints for selected unit type
- Viking-themed Victory/Defeat overlay with "FIGHT AGAIN" button

## Architecture

### Scripts

| Script | Purpose |
|--------|---------|
| `GameBootstrap.cs` | Master initializer -- main menu (Start Battle / View Units), orchestrates all setup |
| `MapGenerator.cs` | Procedural terrain, river, trees, lighting, NavMesh bake |
| `Unit.cs` | Unit data, stats, abilities (rage/wall/mark), skeletal model builder with 14-joint pivot hierarchy for all 4 types |
| `UnitMovement.cs` | NavMeshAgent-based pathfinding and movement |
| `UnitCombat.cs` | Type-specific attacks, parry, dual axe, shield bash, fear, marking |
| `HealthBar.cs` | World-space billboard HP bar with unit type label and armor indicator |
| `SelectionManager.cs` | Click, shift-click, and drag-box unit selection |
| `CommandManager.cs` | Right-click move/attack orders with formation spreading |
| `FactionManager.cs` | Tracks living units per faction, win/lose detection |
| `Projectile.cs` | Arrow flight arc, trail effect, stick-in-target on hit with splinter VFX |
| `AIController.cs` | Tactical AI with per-type behaviors and army strength evaluation |
| `UnitSpawner.cs` | Spawns 4-type armies in formation (shield front, melee mid, archers back) |
| `CameraController.cs` | WASD pan, scroll zoom, map bounds clamping |
| `GameUI.cs` | HUD with type breakdown, ability hints, Viking-themed game over |
| `ShaderHelper.cs` | PBR material factory with presets (skin, chainmail, steel, gold, leather, fur, wood, bone, cloth), shader fallback utility |
| `UnitAnimator.cs` | Rotation-based skeletal animation: idle, walk, attack choreography per unit type, death ragdoll, cape physics |
| `TrailEffect.cs` | Weapon trail renderer attachment for swords, axes, spears, and arrows |
| `DamagePopup.cs` | World-space floating damage numbers with color-coding and scale animation |
| `UnitViewer.cs` | Pre-battle unit viewer with orbit camera, unit cycling, and animation preview |
| `CameraShake.cs` | Perlin noise-based camera shake for combat impacts |

### Editor Tools

| Script | Purpose |
|--------|---------|
| `Editor/WorldWarsSetup.cs` | One-click scene setup, 3D renderer configuration, build settings |
| `Editor/WorldWarsValidator.cs` | Validates all scripts compile and configuration is correct |

### Key Design Decisions

- **Fully procedural**: The entire game bootstraps from a single `GameBootstrap` component. Map, units, UI, lighting, and camera are all created at runtime.
- **No prefabs required**: Unit models are built from Unity primitives (cubes, spheres, cylinders) at runtime with a hierarchical pivot skeleton for premium low-poly aesthetic.
- **New Input System**: Uses `Mouse.current` and `Keyboard.current` from Unity's Input System package.
- **Mobile-ready architecture**: Resolution-independent UI (Canvas Scaler), input abstraction, URP rendering, no hardcoded screen positions.
- **Data-driven stats**: All unit stats are defined in `ApplyStats()` -- easy to tweak and balance.

## Tech Stack

- **Engine**: Unity 6 (6000.3.8f1)
- **Render Pipeline**: Universal Render Pipeline (URP) 17.3.0
- **Input**: Unity Input System 1.18.0
- **Navigation**: AI Navigation 2.0.10 (NavMeshSurface)
- **Language**: C#
- **Cost**: $0 (Unity Personal license)

## Project Structure

```
WorldWars/
  Docs/
    characters.md
    character-building.md
  Assets/
    Scenes/
      SampleScene.unity
    Scripts/
      GameBootstrap.cs
      MapGenerator.cs
      Unit.cs
      UnitMovement.cs
      UnitCombat.cs
      UnitAnimator.cs
      UnitSpawner.cs
      UnitViewer.cs
      HealthBar.cs
      SelectionManager.cs
      CommandManager.cs
      FactionManager.cs
      Projectile.cs
      AIController.cs
      CameraController.cs
      CameraShake.cs
      GameUI.cs
      ShaderHelper.cs
      TrailEffect.cs
      DamagePopup.cs
      Editor/
        WorldWarsSetup.cs
        WorldWarsValidator.cs
    Settings/
      UniversalRP.asset
      Renderer2D.asset
  Packages/
    manifest.json
  ProjectSettings/
```

## Version History

- **v0.3** -- Premium Graphics Overhaul: 14-joint skeletal animation system, PBR materials, weapon trail effects, floating damage numbers, camera shake, forward-held weapons, death ragdoll animations, cape cloth physics, multi-element combat VFX
- **v0.2** -- Viking Overhaul: 4 unique character classes with abilities, tactical AI, formation spawning, Viking-themed UI
- **v0.1** -- MVP: 2 unit types, basic combat, procedural map, simple AI

## License

Personal project. Unity Personal license (free).
