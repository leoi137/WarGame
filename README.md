# WorldWars - Medieval Conquest

A real-time strategy game built in Unity 6 where two medieval factions clash on a procedurally generated battlefield. Command your army of swordsmen and archers to destroy the enemy forces.

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

You command the **Kingdom of the North** (blue units) on the left side of the map. The **Southern Empire** (red units) on the right is controlled by AI.

Select your units, right-click to move or attack, and destroy all enemy units to win. If you lose all your units, the AI wins.

## Game Features

### Procedural Map
- 100x100 terrain with Perlin noise hills
- Grass and dirt terrain painting
- River dividing the two territories
- Minecraft-style block trees scattered across the map
- NavMesh baked at runtime for pathfinding

### Factions
- **Kingdom of the North** (Blue) -- player-controlled
- **Southern Empire** (Red) -- AI-controlled

### Units
Two unit types per faction. See [characters.md](characters.md) for full stats and details.

- **Swordsman** -- melee tank, high HP, short range
- **Archer** -- ranged glass cannon, low HP, long range

### Combat System
- Units auto-attack enemies within detection range
- Swordsmen deal instant melee damage
- Archers fire arrow projectiles that arc toward the target
- Health bars float above each unit (green to red gradient)
- Units die and are removed when HP reaches 0

### AI Opponent
- Swordsmen charge the nearest player unit
- Archers maintain distance and fire from behind
- Decisions made every 2 seconds
- Aggro range of 20 units
- Archers retreat if enemies get too close

### UI
- HUD showing unit counts for both factions
- Selected unit info panel (type, HP, attack)
- Victory/Defeat overlay with Restart button

## Architecture

### Scripts

| Script | Purpose |
|--------|---------|
| `GameBootstrap.cs` | Master initializer -- orchestrates all setup in correct order |
| `MapGenerator.cs` | Procedural terrain, river, trees, lighting, NavMesh bake |
| `Unit.cs` | Unit data, stats, Minecraft-style block model builder |
| `UnitMovement.cs` | NavMeshAgent-based pathfinding and movement |
| `UnitCombat.cs` | Auto-targeting, attack cooldowns, arrow spawning |
| `HealthBar.cs` | World-space billboard HP bar per unit |
| `SelectionManager.cs` | Click, shift-click, and drag-box unit selection |
| `CommandManager.cs` | Right-click move/attack orders with formation spreading |
| `FactionManager.cs` | Tracks living units per faction, win/lose detection |
| `Projectile.cs` | Arrow flight arc and damage on hit |
| `AIController.cs` | Enemy AI decision-making |
| `UnitSpawner.cs` | Spawns initial armies for both factions |
| `CameraController.cs` | WASD pan, scroll zoom, map bounds clamping |
| `GameUI.cs` | HUD, selected info, victory/defeat overlay, restart |
| `ShaderHelper.cs` | Shader fallback utility for cross-pipeline compatibility |

### Editor Tools

| Script | Purpose |
|--------|---------|
| `Editor/WorldWarsSetup.cs` | One-click scene setup, 3D renderer configuration, build settings |
| `Editor/WorldWarsValidator.cs` | Validates all scripts compile and configuration is correct |

### Key Design Decisions

- **Fully procedural**: The entire game bootstraps from a single `GameBootstrap` component. Map, units, UI, lighting, and camera are all created at runtime.
- **No prefabs required**: Unit models are built from Unity primitive cubes at runtime (Minecraft aesthetic).
- **New Input System**: Uses `Mouse.current` and `Keyboard.current` from Unity's Input System package.
- **Mobile-ready architecture**: Resolution-independent UI (Canvas Scaler), input abstraction, URP rendering, no hardcoded screen positions.

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
  Assets/
    Scenes/
      SampleScene.unity
    Scripts/
      GameBootstrap.cs
      MapGenerator.cs
      Unit.cs
      UnitMovement.cs
      UnitCombat.cs
      HealthBar.cs
      SelectionManager.cs
      CommandManager.cs
      FactionManager.cs
      Projectile.cs
      AIController.cs
      UnitSpawner.cs
      CameraController.cs
      GameUI.cs
      ShaderHelper.cs
      Editor/
        WorldWarsSetup.cs
        WorldWarsValidator.cs
    Settings/
      UniversalRP.asset
      Renderer2D.asset
      ForwardRenderer.asset
  Packages/
    manifest.json
  ProjectSettings/
```

## Future Roadmap

- [ ] More unit types (cavalry, siege, mage)
- [ ] Resource gathering (gold, wood, food)
- [ ] Base building (barracks, archery range, castle walls)
- [ ] Multiple maps with different terrain
- [ ] Country/nation system with territories
- [ ] Invasion and defense mechanics
- [ ] Multiplayer support
- [ ] Free low-poly asset pack integration
- [ ] Mobile port (Flutter/Swift wrapper or direct Unity build)
- [ ] Sound effects and music

## License

Personal project. Unity Personal license (free).
# WarGame
