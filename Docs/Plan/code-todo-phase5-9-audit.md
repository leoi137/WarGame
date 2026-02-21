# Phase 5–9 Audit Report

**Source:** `code-todo.md` lines 1030–1745  
**Date:** 2026-02-21

---

## PHASE 5: AI Simulation Engine

**Tests:** 15  
**Checklist items:** 7

### Tests
- `Test_SimulationAIIdenticalForBothSides` — Two SimulationAI instances with same config make same decisions
- `Test_AISelectsNearestTarget` — Unit with no special context targets nearest enemy
- `Test_AIRangedMaintainsDistance` — Ranged unit retreats when enemy closes in
- `Test_AICavalryChargesRanged` — Cavalry prioritizes ranged targets for charge
- `Test_AITankHoldsFormation` — Heavy infantry stays near formation center
- `Test_AIRetreatsAtLowHP` — Damaged unit retreats below HP threshold
- `Test_AIStrengthRatioAffectsStance` — Weaker army adopts defensive stance
- `Test_FormationPlacesShieldsFront` — Auto-formation puts heavy infantry in front row
- `Test_FormationPlacesRangedBack` — Auto-formation puts ranged in back row
- `Test_TargetScoringFavorsWeak` — Low-HP targets score higher than full-HP targets
- `Test_TypeMatchupBonusCorrect` — Cavalry vs ranged returns positive bonus
- `Test_TerrainAnalyzerFindsHighGround` — Finds elevated position within search radius
- `Test_DeterministicBattleOutcome` — Same seed, same setup produces identical result

### ISSUES FOUND
- **Test name unclear:** `Test_DeterministicBattleOutcome` — "outcome" is vague; could mean winner, casualties, or full replay. Prefer e.g. `Test_DeterministicBattleReplayIdentical`.
- **File without checklist:** All 5 Files (SimulationAI, TacticalDecisionMaker, TargetSelector, FormationController, TerrainAnalyzer) are covered by checklist items 1–5.
- **Missing test for TacticalDecisionMaker:** No test for `GetAdvanceTarget`, `GetFlankPosition`, `GetRetreatPosition`, or `ShouldFocusFire`.
- **Missing test for TerrainAnalyzer:** No test for `FindChokePoints`, `FindForestCover`, or `FindBestRangedPosition`.

### SUGGESTED ADDITIONS
- `Test_TacticalAdvanceTargetValid` — GetAdvanceTarget returns position within map bounds
- `Test_TerrainAnalyzerFindsChokePoints` — FindChokePoints returns non-empty list for map with narrow passages
- `Test_DeterministicBattleReplayIdentical` — Rename/clarify existing deterministic test

---

## PHASE 6: World Map System

**Tests:** 7  
**Checklist items:** 8

### ISSUES FOUND
- **File without checklist:** `WorldMapManager` — Checklist has "WorldMapManager created" ✓
- **Vague checklist item:** "ProvinceRenderer handles territory highlighting and selection" — "handles" is vague; specify e.g. "SetHighlighted/SetSelected change appearance".
- **Test not covered by Files:** `Test_AllCitiesPlacedOnMap` — CityMarker is in Files; CityMarker.Initialize creates markers. Covered ✓
- **Missing test for WorldMapInput:** No test for `RaycastToCity`, `HandleCityClick`, or `HandleHover`/tooltips.
- **Missing test for WorldMapCamera:** No test for `HandleInput`, `ClampToBounds`, or zoom limits.
- **Test name unclear:** `Test_CameraFocusOnFactionCenters` — "centers" ambiguous; could mean centroid of territory or capital city.

### SUGGESTED ADDITIONS
- `Test_RaycastReturnsCity` — Clicking city marker returns correct CityDefinition
- `Test_CameraClampsToMapBounds` — Panning past edge keeps camera within bounds
- `Test_HoverShowsTooltip` — Hovering territory shows faction tooltip

---

## PHASE 7: Battle Flow

**Tests:** 12  
**Checklist items:** 7

### ISSUES FOUND
- **Duplicate test (Phase 5):** `Test_DeterministicBattleWithSameSeed` (Phase 7) overlaps with `Test_DeterministicBattleOutcome` (Phase 5). Phase 7 version is full battle; Phase 5 is AI decisions. Consider renaming Phase 5 to `Test_AIDecisionsDeterministicWithSeed`.
- **File without checklist:** BattlefieldGenerator is referenced in architecture but not in Phase 7 Files. Phase 7 Files list BattleManager, BattleSetup, BattleSimulator, BattleCamera, BattleTimeController, BattleResultsScreen — all have checklist items.
- **Missing test for BattleSetup:** No test for `HandleDragAndDrop`, `IsValidPlacement` boundary cases, or `AutoPlaceUnits` using FormationController.
- **Missing test for BattleResultsScreen:** No test for `GetBattleSummary`, `CreateRematchButton`, or `CreateReturnButton` behavior.
- **Vague checklist item:** "BattleManager orchestrates full battle lifecycle" — "orchestrates" is vague; consider "BattleManager transitions through Placement → Simulating → Results".

### SUGGESTED ADDITIONS
- `Test_IsValidPlacementRejectsOutOfZone` — IsValidPlacement returns false for positions outside placement zone
- `Test_RematchButtonReusesConfig` — Rematch button restarts battle with same configuration

---

## PHASE 8: UI Systems

**Tests:** 9  
**Checklist items:** 11

### ISSUES FOUND
- **File without checklist:** All 10 Files have checklist items ✓
- **Missing test for WorldMapHUD:** No test for region buttons, search bar, battle button enable state, or `UpdateSelectionState`.
- **Missing test for FactionColorPalette:** No test for `GetPrimaryColor`, `GetSecondaryColor`, or `Brighten`/`Desaturate`.
- **Missing test for MinimapRenderer:** No test for `UpdateUnitPositions` or `MinimapTexture` validity.
- **Missing test for MainMenuUI:** No test for Settings button or `CreateBackgroundScene`.
- **Test name unclear:** `Test_UIThemeManagerCreatesValidUI` — "Valid" is vague; specify "non-null" or "with expected components".

### SUGGESTED ADDITIONS
- `Test_WorldMapHUDBattleButtonEnabledWhenTwoSelected` — Battle button enabled only when attacker and defender chosen
- `Test_FactionColorPaletteReturnsDistinctColors` — GetPrimaryColor returns different colors for different factions
- `Test_MinimapShowsUnitPositions` — Minimap dots update when units move

---

## PHASE 9: Comprehensive Testing Suite

**Tests:** 80 (18 DataValidation + 8 CombatMath + 8 TerrainEffect + 7 AIDecision + 8 Ability + 7 FactionBalance + 4 BattleResult + 7 BattleSimulation + 6 UnitSpawn + 3 TerrainGeneration + 4 Integration)  
**Checklist items:** 6

### ISSUES FOUND

#### DataValidationTests vs Phase 2
- **Phase 9 DROPS 3 Phase 2 tests:** `Test_ByzantineEmpireDataCorrect`, `Test_SongEmpireDataCorrect`, `Test_NorthSeaEmpirePreservesExistingUnits` are in Phase 2 but not in Phase 9 DataValidationTests. Phase 9 consolidation loses spot-check coverage.
- **Phase 9 ADDS:** `Test_CapitalCityExistsInCityList` — good addition.

#### TerrainEffectTests vs Phase 3
- **Different focus:** Phase 3 tests terrain generation (heightmaps, vegetation, rivers). Phase 9 TerrainEffectTests focus on TerrainEffects runtime modifiers. Not duplicates — complementary. Phase 9 TerrainEffectTests do not replace Phase 3 tests; both should exist.

#### AIDecisionTests vs Phase 5
- **Phase 9 DROPS 6 Phase 5 tests:** `Test_FormationPlacesShieldsFront`, `Test_FormationPlacesRangedBack`, `Test_TargetScoringFavorsWeak`, `Test_TypeMatchupBonusCorrect`, `Test_TerrainAnalyzerFindsHighGround`, `Test_DeterministicBattleOutcome` have no Phase 9 equivalent.
- **Phase 9 ADDS:** `Test_AIDoesNotCheat` — good addition.
- **Naming mismatch:** `Test_IdenticalAIProducesSameDecisions` ≈ `Test_SimulationAIIdenticalForBothSides`; `Test_RangedUnitsRetreatWhenFlanked` ≈ `Test_AIRangedMaintainsDistance`; `Test_CavalryTargetsRangedFirst` ≈ `Test_AICavalryChargesRanged`; `Test_StanceChangesWithStrengthRatio` ≈ `Test_AIStrengthRatioAffectsStance`.

#### CombatMathTests vs Phase 4
- Phase 4 has no explicit combat math tests (Test_GenericMeleeAttackDealsDamage is integration). Phase 9 CombatMathTests are new, not consolidation. ✓

#### AbilityTests vs Phase 4
- Phase 4 has `Test_AbilityActivatesOnLowHP`, `Test_AbilityAppliesDamageModifier`, etc. Phase 9 AbilityTests test specific abilities (Parry, Berserker Rage, etc.). Different focus — Phase 4 integration, Phase 9 unit-level. ✓

#### Test name unclear
- `Test_CriticalHitCalculation` — "If critical mechanic exists, verify multiplier" makes the test conditional; unclear what to implement.
- `Test_NoFactionHasOverwhelmingMilitaryAdvantage` — "Overwhelming" is subjective; "Largest faction < 20x smallest" in description helps but name could be clearer.
- `Test_AverageUnitStatsWithinRange` — "Reasonable" in description is vague.

#### Vague checklist items
- "Edge cases tested (empty armies, same faction vs self, etc.)" — "etc." is open-ended; no explicit test list.
- "Code coverage report generated (if available)" — "if available" makes completion criteria unclear.

#### Missing tests for Phase 9 Files
- **CampaignLogicTests** — Listed in file structure (line 243) but has no Phase 9 test file or tests. Phase 9 focuses on Quick Battle, not Campaign.
- **WorldMapTests** — Listed in file structure (line 251) but Phase 9 has no WorldMapTests.cs. World map coverage is in Phase 6 tests and IntegrationTests only.
- **UINavigationTests** — Listed in file structure (line 250) but Phase 9 has no UINavigationTests.cs.

### SUGGESTED ADDITIONS
- **DataValidationTests:** Restore `Test_ByzantineEmpireDataCorrect`, `Test_SongEmpireDataCorrect`, `Test_NorthSeaEmpirePreservesExistingUnits` from Phase 2.
- **AIDecisionTests:** Add Phase 5 equivalents: `Test_FormationPlacesShieldsFront`, `Test_FormationPlacesRangedBack`, `Test_TargetScoringFavorsWeak`, `Test_TerrainAnalyzerFindsHighGround`.
- **BattleResultTests:** Add `Test_BattleResultTracksAllCasualties` (Phase 7 has this; Phase 9 BattleResultTests should include it).
- **Checklist:** Replace "Edge cases tested (etc.)" with explicit list: "Empty army, same-faction-vs-self, single-unit army tested".
- **Checklist:** Replace "Code coverage report generated (if available)" with "Code coverage report generated, or document why unavailable".

---

## Summary Table

| Phase | Tests | Checklist | Critical Issues |
|-------|-------|-----------|-----------------|
| 5 | 15 | 7 | 1 unclear test name; 2 missing test areas |
| 6 | 7 | 8 | 1 vague checklist; 3 missing test areas |
| 7 | 12 | 7 | 1 duplicate with Phase 5; 2 missing tests |
| 8 | 9 | 11 | 4 missing test areas; 1 unclear test name |
| 9 | 80 | 6 | Consolidation drops Phase 2/5 tests; 3 conditional/vague tests; 3 missing test files from architecture |
