# Character Building Guide

Technical documentation for the procedural unit construction system in WorldWars - Viking Conquest. This guide covers the skeleton hierarchy, material system, model assembly, weapon positioning, and animation integration.

---

## Architecture Overview

Every unit in WorldWars is generated entirely at runtime using Unity primitives (`Cube`, `Sphere`, `Cylinder`). There are **no prefabs**, no imported 3D models, and no external textures. The system consists of:

1. **Skeleton** -- A hierarchy of 14 empty `Transform` pivots that enable rotation-based animation
2. **Geometry** -- Unity primitives parented to pivots, shaped and scaled to form body parts
3. **Materials** -- PBR materials from `ShaderHelper` presets (skin, chainmail, steel, etc.)
4. **Weapons** -- Forward-held implements parented to hand pivots
5. **Animation** -- `UnitAnimator` rotates the pivots to produce walk cycles, attacks, idle, and death

All construction happens in `Unit.cs`, specifically in the `BuildBlockModel()` method which calls `BuildSkeleton()` followed by a type-specific builder (`BuildSwordsmanModel`, `BuildArcherModel`, etc.).

---

## The Skeleton System

### Pivot Hierarchy

The skeleton is a tree of empty GameObjects (pivots). Rotating a parent pivot moves all children naturally, just like real joints:

```
Unit Root (transform)
  +-- Pivot_Hips (Y=0.65)              -- pelvis, root of skeleton
  |     +-- Pivot_Waist (Y=+0.35)      -- upper body root
  |     |     +-- Pivot_Neck (Y=+0.55) -- head mount
  |     |     +-- Pivot_LShoulder (X=-0.28, Y=+0.45) -- left arm root
  |     |     |     +-- Pivot_LElbow (Y=-0.32)
  |     |     |           +-- Pivot_LHand (Y=-0.28)   -- weapon/shield mount
  |     |     +-- Pivot_RShoulder (X=+0.28, Y=+0.45) -- right arm root
  |     |     |     +-- Pivot_RElbow (Y=-0.32)
  |     |     |           +-- Pivot_RHand (Y=-0.28)   -- weapon mount
  |     |     +-- Pivot_Cape (Y=+0.4, Z=-0.18) -- cape/cloak mount
  |     +-- Pivot_LHip (X=-0.12)       -- left leg root
  |     |     +-- Pivot_LKnee (Y=-0.38)
  |     +-- Pivot_RHip (X=+0.12)       -- right leg root
  |           +-- Pivot_RKnee (Y=-0.38)
```

### Pivot Fields on Unit.cs

These are declared as `[HideInInspector] public Transform` on the `Unit` component:

| Field | Parent | Default Local Position | Purpose |
|-------|--------|----------------------|---------|
| `pivotHips` | Unit root | (0, 0.65, 0) | Root of skeleton, pelvis height |
| `pivotWaist` | Hips | (0, 0.35, 0) | Upper body, torso attachment |
| `pivotNeck` | Waist | (0, 0.55, 0) | Head and helmet |
| `pivotLeftShoulder` | Waist | (-0.28, 0.45, 0) | Left arm chain root |
| `pivotRightShoulder` | Waist | (0.28, 0.45, 0) | Right arm chain root |
| `pivotLeftElbow` | L Shoulder | (0, -0.32, 0) | Left forearm |
| `pivotRightElbow` | R Shoulder | (0, -0.32, 0) | Right forearm |
| `pivotLeftHand` | L Elbow | (0, -0.28, 0) | Left hand + shield/bow |
| `pivotRightHand` | R Elbow | (0, -0.28, 0) | Right hand + sword/axe/spear |
| `pivotLeftHip` | Hips | (-0.12, 0, 0) | Left upper leg |
| `pivotRightHip` | Hips | (0.12, 0, 0) | Right upper leg |
| `pivotLeftKnee` | L Hip | (0, -0.38, 0) | Left lower leg + boot |
| `pivotRightKnee` | R Hip | (0, -0.38, 0) | Right lower leg + boot |
| `pivotCape` | Waist | (0, 0.4, -0.18) | Cape/cloak attachment |

### Skeleton Customization Per Unit Type

Some unit types override default pivot positions to change proportions:

- **Berserker** -- Wider shoulders (`LShoulder X=-0.33`, `RShoulder X=0.33`) and wider hips (`LHip X=-0.14`, `RHip X=0.14`) for a bulkier frame
- **Shieldbearer** -- Similar wider shoulders (`X=+-0.30`) and hips (`X=+-0.14`) for a stocky tank build

---

## Part Creation Helpers

Three methods in `Unit.cs` create geometry:

### `CreatePart(PrimitiveType type, string partName, Transform parent, Vector3 localPos, Vector3 scale, Material mat)`

The base method. Creates a Unity primitive, parents it, positions it, applies material, removes its collider (to avoid physics interference), and sets the layer.

### `CreateSphere(string partName, Transform parent, Vector3 localPos, Vector3 scale, Material mat)`

Shorthand for `CreatePart(PrimitiveType.Sphere, ...)`. Used for heads, hands, helmet domes, belt skulls.

### `CreateCylinder(string partName, Transform parent, Vector3 localPos, Vector3 scale, Material mat)`

Shorthand for `CreatePart(PrimitiveType.Cylinder, ...)`. Used for arms, legs, shields, spear shafts.

### `CreatePivot(string name, Transform parent, Vector3 localPos)`

Creates an empty `GameObject` with just a `Transform`. Used for skeleton joints and weapon tip markers.

---

## Material System (ShaderHelper.cs)

All materials are PBR (Physically Based Rendering) with metallic workflow, created through `ShaderHelper`. The system uses URP's Lit shader with fallbacks.

### Base Factory

```csharp
ShaderHelper.CreateMaterial(Color color, float metallic, float smoothness, Color? emission)
```

| Parameter | Range | Description |
|-----------|-------|-------------|
| `color` | Color | Base albedo color |
| `metallic` | 0-1 | 0 = dielectric (cloth, skin), 1 = pure metal |
| `smoothness` | 0-1 | 0 = rough/matte, 1 = mirror-like |
| `emission` | Color? | Optional glow color (null = no glow) |

### Material Presets

| Preset | Metallic | Smoothness | Emission | Typical Use |
|--------|----------|------------|----------|-------------|
| `SkinMaterial` | 0.0 | 0.25 | color * 0.03 | Faces, hands, bare skin |
| `ChainmailMaterial` | 0.7 | 0.45 | (0.05, 0.05, 0.08) | Armor mesh |
| `SteelMaterial` | 0.85 | 0.6 | none | Sword blades, axe heads, helm |
| `GoldMaterial` | 0.9 | 0.7 | color * 0.15 | Buckles, bands, accents |
| `LeatherMaterial` | 0.0 | 0.15 | none | Belts, boots, bracers |
| `FurMaterial` | 0.0 | 0.05 | none | Fur cloaks, trousers |
| `WoodMaterial` | 0.0 | 0.12 | none | Weapon handles, shields |
| `BoneMaterial` | 0.0 | 0.2 | color * 0.04 | Teeth, skulls, bowstring |
| `ClothMaterial` | 0.0 | 0.08 | none | Tunics, capes, surcoats |

### VFX Materials

| Method | Description |
|--------|-------------|
| `CreateUnlitMaterial(color)` | Flat color, no lighting (UI, markers) |
| `CreateAdditiveMaterial(color)` | Additive blend, no depth write (glows, trails) |

---

## Color Palette

Colors are defined as `static readonly Color` constants in `Unit.cs`:

| Constant | RGB | Description |
|----------|-----|-------------|
| `NorthPrimary` | (0.15, 0.35, 0.7) | Blue faction main |
| `NorthSecondary` | (0.2, 0.45, 0.85) | Blue faction accent |
| `SouthPrimary` | (0.7, 0.15, 0.12) | Red faction main |
| `SouthSecondary` | (0.85, 0.2, 0.15) | Red faction accent |
| `SkinColor` | (0.85, 0.7, 0.55) | All unit skin tone |
| `FurColor` | (0.45, 0.35, 0.25) | Bear/wolf fur |
| `DarkFur` | (0.3, 0.22, 0.15) | Dark trim fur |
| `IronColor` | (0.55, 0.55, 0.58) | Iron fittings |
| `SteelColor` | (0.7, 0.72, 0.75) | Polished steel |
| `WoodColor` | (0.5, 0.33, 0.15) | Weapon handles, shields |
| `GoldAccent` | (0.85, 0.7, 0.2) | Gold details |
| `LeatherColor` | (0.4, 0.28, 0.15) | Leather gear |
| `BoneWhite` | (0.9, 0.88, 0.82) | Bone, eyes |
| `DarkRed` | (0.5, 0.08, 0.05) | War paint |
| `ChainmailColor` | (0.48, 0.5, 0.52) | Chainmail armor |
| `HairBlond` | (0.82, 0.7, 0.4) | Berserker hair |

Faction colors are selected dynamically: `primary = faction == Faction.North ? NorthPrimary : SouthPrimary`.

---

## Weapon Positioning Rules

**Critical convention:** Weapons are parented to hand pivots (`pivotLeftHand`, `pivotRightHand`). Because arms hang vertically downward in rest pose, the coordinate system at the hand is:

- **Y axis** = along the arm (up toward shoulder, down toward ground)
- **Z axis** = forward (away from the unit's front)
- **X axis** = sideways (left/right)

### Held Weapons Must Extend Along Z (Forward)

Weapons should extend **forward (Z+)** from the grip, NOT upward along the arm (Y+). The grip sits at or near the hand origin, and the blade/head extends in Z.

| Weapon | Grip Position | Blade/Head Direction | Example |
|--------|--------------|---------------------|---------|
| Sword | Y=-0.02 to Y=0.04 (around hand) | Z+ (forward) | Blade at Z=0.32, tip at Z=0.6 |
| Axe | At hand origin | Z+ (forward) | Handle along Z, head at Z=0.5 |
| Spear | At hand origin | Z+ (forward) | Shaft along Z, head at Z=1.05 |
| Bow | Z=0.12 (forward of hand) | Staves extend Y+/Y- (up/down) | Vertical weapon, held forward |
| Shield | X=-0.15 to -0.22 (outward) | Flat face outward (X-) | Rotated 90 on Z to face sideways |

### Sword (Swordsman, right hand)

```csharp
// Grip wraps around hand
CreatePart(Cube, "SwordGrip", pivotRightHand,
    new Vector3(0, -0.02f, 0), new Vector3(0.04f, 0.12f, 0.04f), ...);
// Pommel below grip
CreateSphere("SwordPommel", pivotRightHand,
    new Vector3(0, -0.09f, 0), new Vector3(0.07f, 0.06f, 0.07f), ...);
// Guard at top of grip
CreatePart(Cube, "SwordGuard", pivotRightHand,
    new Vector3(0, 0.04f, 0), new Vector3(0.18f, 0.035f, 0.05f), ...);
// Blade extends FORWARD (Z+) from guard
CreatePart(Cube, "SwordBlade", pivotRightHand,
    new Vector3(0, 0.04f, 0.32f), new Vector3(0.05f, 0.1f, 0.55f), ...);
// Tip marker at blade end
weaponTip = CreatePivot("WeaponTip", pivotRightHand, new Vector3(0, 0.04f, 0.6f));
```

### Axe (Berserker, both hands)

```csharp
// Handle extends FORWARD (Z+) from grip
CreatePart(Cube, "AxeHandle", pivotRightHand,
    new Vector3(0, 0, 0.28f), new Vector3(0.04f, 0.04f, 0.52f), ...);
// Axe head at end of handle
CreatePart(Cube, "AxeHead", pivotRightHand,
    new Vector3(0, 0.08f, 0.5f), new Vector3(0.03f, 0.24f, 0.14f), ...);
// Sharp edge
CreatePart(Cube, "AxeEdge", pivotRightHand,
    new Vector3(0, 0.2f, 0.5f), new Vector3(0.02f, 0.04f, 0.1f), ...);
weaponTip = CreatePivot("AxeTip", pivotRightHand, new Vector3(0, 0.2f, 0.56f));
```

### Spear (Shieldbearer, right hand)

```csharp
// Shaft extends FORWARD (Z+) -- long reach
CreateCylinder("SpearShaft", pivotRightHand,
    new Vector3(0, 0, 0.55f), new Vector3(0.035f, 0.035f, 0.6f), ...);
// Spearhead at far end
CreatePart(Cube, "SpearHead", pivotRightHand,
    new Vector3(0, 0, 1.05f), new Vector3(0.035f, 0.08f, 0.15f), ...);
weaponTip = CreatePivot("SpearTip", pivotRightHand, new Vector3(0, 0, 1.18f));
```

### Bow (Archer, left hand)

The bow is a special case -- the grip is held forward (Z=0.12) and the staves extend vertically (Y+/Y-):

```csharp
// Upper stave (Y+)
CreatePart(Cube, "BowUpper", pivotLeftHand,
    new Vector3(0, 0.25f, 0.12f), new Vector3(0.03f, 0.32f, 0.08f), ...);
// Lower stave (Y-)
CreatePart(Cube, "BowLower", pivotLeftHand,
    new Vector3(0, -0.22f, 0.12f), new Vector3(0.03f, 0.3f, 0.07f), ...);
// String pulled back toward archer (Z=0.04, behind the staves)
CreatePart(Cube, "BowString", pivotLeftHand,
    new Vector3(0, 0f, 0.04f), new Vector3(0.015f, 0.55f, 0.015f), ...);
```

### Shield (Swordsman / Shieldbearer, left hand)

Shields face outward to the left (X-) and are rotated 90 degrees on Z:

```csharp
// Disc faces outward (X-)
CreateCylinder("Shield", pivotLeftHand,
    new Vector3(-0.15f, 0.05f, 0.05f), new Vector3(0.5f, 0.025f, 0.5f), ...);
partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
// Boss and cross on shield face
CreateCylinder("ShieldBoss", pivotLeftHand,
    new Vector3(-0.19f, 0.05f, 0.05f), ...);
```

---

## Weapon Tips and Trail Effects

Each weapon has a `weaponTip` (and optionally `weaponLeftTip`) -- an empty transform at the weapon's striking end. These are used by `TrailEffect.cs` to attach `TrailRenderer` components during attacks.

| Trail Type | Width | Colors | Used By |
|------------|-------|--------|---------|
| `AttachSwordTrail` | 0.25 start | White to pale blue | Swordsman |
| `AttachAxeTrail` | 0.3 start | Orange to red | Berserker |
| `AttachSpearTrail` | 0.2 start | Yellow to amber | Shieldbearer |
| `AttachArrowTrail` | 0.08 start | Cyan to white | Projectile (arrows) |

---

## Building a New Unit Type

Follow these steps to add a new unit type to the game:

### Step 1: Add to the Enum

In `Unit.cs`, add your type to the `UnitType` enum:

```csharp
public enum UnitType { Swordsman, Archer, Berserker, Shieldbearer, Raider }
```

### Step 2: Define Stats

In `Unit.ApplyStats()`, add a case:

```csharp
case UnitType.Raider:
    maxHealth = 70f;
    attackDamage = 18f;
    attackRange = 2.5f;
    attackCooldown = 0.6f;
    moveSpeed = 5.0f;
    armor = 1f;
    break;
```

### Step 3: Create the Model Builder

Add a new method in `Unit.cs` and register it in `BuildBlockModel()`:

```csharp
void BuildBlockModel()
{
    // ... existing code ...
    switch (unitType)
    {
        // ... existing cases ...
        case UnitType.Raider: BuildRaiderModel(primary, secondary); break;
    }
}

void BuildRaiderModel(Color primary, Color secondary)
{
    // Optionally override skeleton proportions
    pivotLeftShoulder.localPosition = new Vector3(-0.26f, 0.45f, 0);
    pivotRightShoulder.localPosition = new Vector3(0.26f, 0.45f, 0);

    // --- TORSO ---
    partBody = CreatePart(PrimitiveType.Cube, "Chest", pivotWaist,
        new Vector3(0, 0.2f, 0), new Vector3(0.44f, 0.38f, 0.26f),
        ShaderHelper.LeatherMaterial(LeatherColor)).transform;

    // --- HEAD ---
    partHead = CreateSphere("Head", pivotNeck,
        new Vector3(0, 0.18f, 0), new Vector3(0.36f, 0.38f, 0.36f),
        ShaderHelper.SkinMaterial(SkinColor)).transform;
    // Eyes, helmet, etc...

    // --- ARMS ---
    partLeftArm = CreateCylinder("LUpperArm", pivotLeftShoulder,
        new Vector3(0, -0.15f, 0), new Vector3(0.12f, 0.16f, 0.12f),
        ShaderHelper.LeatherMaterial(LeatherColor)).transform;
    // Elbow, hand...

    // --- WEAPON (extends FORWARD from hand in Z+) ---
    partWeapon = CreatePart(PrimitiveType.Cube, "Dagger", pivotRightHand,
        new Vector3(0, 0, 0.15f), new Vector3(0.04f, 0.06f, 0.28f),
        ShaderHelper.SteelMaterial(SteelColor)).transform;
    weaponTip = CreatePivot("WeaponTip", pivotRightHand,
        new Vector3(0, 0, 0.3f));

    // --- LEGS ---
    partLeftLeg = CreateCylinder("LUpperLeg", pivotLeftHip,
        new Vector3(0, -0.18f, 0), new Vector3(0.13f, 0.19f, 0.13f),
        ShaderHelper.LeatherMaterial(LeatherColor)).transform;
    // Lower leg, boots...

    // --- CAPE (optional) ---
    CreatePart(PrimitiveType.Cube, "Cape", pivotCape,
        new Vector3(0, -0.3f, -0.02f), new Vector3(0.4f, 0.5f, 0.03f),
        ShaderHelper.ClothMaterial(primary * 0.5f));
}
```

### Step 4: Add Animations

In `UnitAnimator.cs`, add cases for the new type in each animation method:

- `AnimateIdle()` -- subtle idle movements (weapon fidget, breathing)
- `AnimateWalk()` -- walk cycle (arm swing, leg stride, body bob)
- `AnimateAttack()` -- attack choreography (wind-up, strike, follow-through)

### Step 5: Add Combat Behavior

In `UnitCombat.cs`, create an `AttackAsRaider()` method and add it to the `Attack()` switch.

### Step 6: Add Weapon Trail

In `TrailEffect.cs`, add a preset:

```csharp
public static void AttachDaggerTrail(Transform weaponTip)
{
    AttachTrail(weaponTip,
        new Color(0.7f, 0.7f, 0.7f), new Color(0.3f, 0.3f, 0.3f),
        0.15f, 0.06f);
}
```

### Step 7: Wire Up Spawner

In `UnitSpawner.cs`:
1. Add a `raiderCount` field
2. Add spawning logic in the formation method
3. Wire all pivot references in `WireAnimatorPivots()`

### Step 8: Update AI and UI

- `AIController.cs` -- Add `HandleRaiderAI()` behavior
- `HealthBar.cs` -- Add name and color for the health bar label
- `GameUI.cs` -- Add to the faction breakdown count

---

## Body Part Conventions

### Primitives by Body Area

| Body Part | Primitive | Typical Scale | Notes |
|-----------|-----------|---------------|-------|
| Head | Sphere | 0.36-0.40 | Slightly taller than wide |
| Eyes | Cube | 0.055 x 0.04 x 0.03 | White sclera + colored pupil |
| Torso (chest) | Cube | 0.46-0.60 x 0.38-0.44 x 0.26-0.34 | Widest for Berserker |
| Upper arm | Cylinder | 0.12-0.16 diameter, 0.16 height | Parented to shoulder pivot |
| Forearm | Cylinder | 0.11-0.14 diameter, 0.14 height | Parented to elbow pivot |
| Hand | Sphere | 0.09-0.11 | Parented to hand pivot |
| Upper leg | Cylinder | 0.13-0.17 diameter, 0.19 height | Parented to hip pivot |
| Lower leg | Cylinder | 0.11-0.15 diameter, 0.17 height | Parented to knee pivot |
| Boot | Cube | 0.14-0.18 x 0.10-0.12 x 0.20-0.24 | Slightly forward (Z=0.02) |
| Cape | Cube | 0.40-0.50 x 0.55-0.70 x 0.03-0.035 | Thin slab on cape pivot |
| Helmet | Sphere | 0.42-0.46 | Various types (nasal, spectacle, hood) |

### Positioning Convention

All positions are **local** to the parent pivot:
- **Y+** = up (toward head)
- **Y-** = down (toward feet)
- **Z+** = forward (toward enemy)
- **Z-** = backward (behind unit)
- **X+** = right side of the unit
- **X-** = left side of the unit

---

## Animation Integration

After building the model, `UnitSpawner.cs` calls `WireAnimatorPivots()` to pass all 14 pivot references from the `Unit` component to the `UnitAnimator` component. The animator also receives legacy part references (`partHead`, `partBody`, etc.) for backward compatibility.

### Key Animation Concepts

- **Rotation-based**: All animation is done by setting `localRotation` on pivots (not position)
- **Easing curves**: `EaseInOut`, `EaseOutBack`, `EaseOutElastic`, `EaseInQuad`, `EaseOutQuad` for natural motion
- **Speed-dependent walk**: Walk cycle speed scales with NavMeshAgent velocity
- **Body bob**: Subtle Y-position oscillation on hips during walking
- **Head stabilization**: Head counter-rotates against body lean
- **Cape cloth physics**: Damped-spring simulation on `pivotCape` based on unit velocity
- **Weapon trails**: `TrailRenderer` attached/detached during attack animations
- **Death ragdoll**: Multi-phase fall with hit reaction, weapon detachment, settling, and fade-out

---

## Unit Type Reference Cards

### Huscarl Swordsman

- **Build**: Standard skeleton proportions
- **Torso**: Chainmail chest + faction cloth tunic + belly
- **Head**: Skin sphere + nasal helm (sphere dome + cube rim + cube nasal guard + gold band)
- **Arms**: Chainmail cylinders
- **Left hand**: Round wooden shield (cylinder rotated 90, iron boss, cross brace)
- **Right hand**: Viking sword (grip + pommel + gold guard + steel blade extending Z+)
- **Legs**: Faction cloth trousers + leather boots
- **Extras**: Leather belt with gold buckle, faction cape

### Norse Hunter (Archer)

- **Build**: Standard skeleton, slimmer proportions
- **Torso**: Leather chest + dark fur guard + leather belly
- **Head**: Skin sphere + faction-colored hood (sphere + brim)
- **Arms**: Skin with leather bracers
- **Left hand**: Wooden bow (upper/lower staves at Z=0.12, string at Z=0.04)
- **Right hand**: Empty (draw hand for bowstring)
- **Back**: Leather quiver with arrow tips
- **Legs**: Leather trousers + dark fur boots
- **Extras**: Leather belt, short faction cape

### Berserker

- **Build**: Widened skeleton (shoulders X=+-0.33, hips X=+-0.14)
- **Torso**: Bare skin chest + fur cloak + dark fur collar + belly
- **Head**: Skin sphere + wild blond hair (3 overlapping spheres) + thick beard
- **Face**: Red glowing eyes, war paint (dark red cross on chest), scars
- **Arms**: Bare skin with gold armbands
- **Both hands**: Dual iron axes (wooden handles extending Z+, iron heads at end)
- **Legs**: Fur trousers + dark leather boots
- **Extras**: Leather belt with bone skull buckle

### Shieldbearer

- **Build**: Widened skeleton (shoulders X=+-0.30, hips X=+-0.14)
- **Torso**: Chainmail chest + faction surcoat with gold cross emblem + chainmail belly
- **Head**: Skin sphere + spectacle helm (dome + face plate + crest)
- **Arms**: Chainmail cylinders + iron shoulder pauldrons
- **Left hand**: Massive round shield (cylinder rotated 90, iron rim top/bottom, boss, faction emblem cross)
- **Right hand**: Long spear (wooden shaft extending Z+, steel head at Z=1.05)
- **Legs**: Chainmail trousers + leather boots
- **Extras**: Leather belt, faction cape

---

## Performance at 20× Unit Scale

With army sizes ranging from 48 to 800 units per side (up to 1600 total), the procedural unit system must scale efficiently. Key optimizations:

### LOD System (UnitLODSystem.cs)

Four detail levels based on distance from camera:

| LOD Level | Distance | Meshes | Features | Tri Budget |
|-----------|----------|--------|----------|------------|
| **Full** | < 60m | All parts (14+ meshes) | Health bar, trails, normal maps, face details | ~150 tris |
| **Simplified** | 60-120m | 3 merged meshes (body+weapon+head) | No health bar, no trails, no normal maps | ~30 tris |
| **Billboard** | 120-200m | 1 quad | Faction-colored textured quad facing camera | 2 tris |
| **Culled** | > 200m | 0 | Renderers disabled, logic only | 0 tris |

Maximum 80 units at Full LOD regardless of distance (closest 80 get priority).

### Mesh Caching

`Unit.InitMeshCache()` generates each body-part mesh once and reuses it across all units of the same type. With 800 units of 5 types, this means only 5× mesh generation instead of 800×.

### GPU Instancing

For Simplified and Billboard LOD levels, identical meshes + materials use `Graphics.DrawMeshInstanced` to batch-render hundreds of units in a single draw call.

### Spatial Partitioning (SpatialGrid.cs)

A cell-based spatial grid (cell size = 10 units) replaces O(n²) nearest-enemy lookups with O(1) amortized queries. Essential for AI decision-making and combat targeting at 1600-unit battles.

### Flocking Movement

For battles > 200 units/side, `UnitMovement` switches from NavMeshAgent (expensive per-agent pathfinding) to lightweight Boids-style flocking: seek target, avoid allies (separation), maintain formation (cohesion), align facing (alignment).

### VFX Budgets at Scale

- Health bars: hidden beyond 40m from camera
- Weapon trails: only on 20 nearest attacking units
- Damage popups: pooled, max 30 active simultaneously
- Combat VFX (sparks, dust): only for Full LOD units

---

## Version History

- **v0.4** -- 20× unit scale: LOD system, spatial partitioning, flocking movement, GPU instancing. Army sizes 48-800 per side. Interactive placement system with formation presets.
- **v0.3** -- Premium graphics overhaul: 14-joint skeleton, PBR materials, forward-held weapons, trail effects, floating damage, camera shake, death ragdoll, cape physics
- **v0.2** -- Original cube-only Minecraft-style block models
- **v0.1** -- Basic colored cubes, no skeleton
