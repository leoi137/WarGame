using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleSetup : MonoBehaviour
{
    public Faction? PlayerSide { get; private set; }

    BattleConfiguration config;
    int mapSize;
    List<Unit> selectedUnits = new();
    List<Unit> playerUnits = new();
    Vector3 dragStart;
    bool isDragging;
    bool isDragMoving;
    Vector3 moveTarget;
    Unit hoveredUnit;
    GameObject placementZoneObj;
    List<GameObject> ghostObjects = new();
    FormationType currentFormation = FormationType.Line;
    float lastClickTime;
    const float DoubleClickThreshold = 0.3f;
    const float MinSpacing = 1.5f;

    void Update()
    {
        if (config == null || IsSpawning) return;
        HandleSelection();
        HandleDragMove();
        HandleRotation();
        HandleFormationHotkeys();
        HandleNumberKeys();
        HighlightHoveredUnit(GetUnitUnderCursor());
    }

    public bool IsSpawning { get; private set; }

    public void Initialize(BattleConfiguration cfg, Faction? playerSide)
    {
        config = cfg;
        PlayerSide = playerSide;
        mapSize = config.mapSize > 0 ? config.mapSize : GameConfig.DefaultMapSize;

        CreatePlacementZone(Faction.Attacker, mapSize);
        CreatePlacementZone(Faction.Defender, mapSize);

        StartCoroutine(SpawnUnitsInBatches(playerSide));
    }

    const int UnitsPerBatch = 30;

    System.Collections.IEnumerator SpawnUnitsInBatches(Faction? playerSide)
    {
        IsSpawning = true;

        if (playerSide == Faction.Attacker)
        {
            yield return StartCoroutine(SpawnRosterBatched(config.attackerFaction, Faction.Attacker, true));
            yield return StartCoroutine(SpawnRosterBatched(config.defenderFaction, Faction.Defender, false));
        }
        else if (playerSide == Faction.Defender)
        {
            yield return StartCoroutine(SpawnRosterBatched(config.defenderFaction, Faction.Defender, true));
            yield return StartCoroutine(SpawnRosterBatched(config.attackerFaction, Faction.Attacker, false));
        }
        else
        {
            yield return StartCoroutine(SpawnRosterBatched(config.attackerFaction, Faction.Attacker, false));
            yield return StartCoroutine(SpawnRosterBatched(config.defenderFaction, Faction.Defender, false));
        }

        IsSpawning = false;
    }

    System.Collections.IEnumerator SpawnRosterBatched(FactionDefinition faction, Faction side, bool isPlayer)
    {
        if (faction == null || faction.unitTypes == null || faction.unitTypes.Count == 0) yield break;

        int budget = side == Faction.Attacker ? config.attackerUnitBudget : config.defenderUnitBudget;
        var roster = BuildUnitRoster(faction, budget);
        int totalCount = 0;
        foreach (var (_, c) in roster) totalCount += c;

        Vector3 center = GetPlacementCenter(side);
        Vector3 facing = GetFacing(side);
        var positions = FormationController.GetFormationPositions(totalCount, center, facing, FormationType.Line, 2f);

        var spawnedUnits = new List<Unit>();
        int idx = 0;
        int spawnedThisBatch = 0;

        foreach (var (typeDef, count) in roster)
        {
            for (int k = 0; k < count && idx < positions.Length; k++, idx++)
            {
                Vector3 pos = positions[idx];
                if (Terrain.activeTerrain != null)
                    pos.y = Terrain.activeTerrain.SampleHeight(pos);

                Unit unit = UnitFactory.CreateUnit(typeDef, faction, side, pos, Quaternion.LookRotation(facing));
                spawnedUnits.Add(unit);
                if (isPlayer) playerUnits.Add(unit);

                if (side == Faction.Attacker)
                    BattleManager.Instance?.RegisterAttackerUnit(unit);
                else
                    BattleManager.Instance?.RegisterDefenderUnit(unit);

                spawnedThisBatch++;
                if (spawnedThisBatch >= UnitsPerBatch)
                {
                    spawnedThisBatch = 0;
                    yield return null;
                }
            }
        }

        FormationController.ArrangeByCategory(spawnedUnits, center, facing);
    }

    public void SpawnPlayerPlacementRoster(FactionDefinition faction, Faction side)
    {
        if (faction == null || faction.unitTypes == null || faction.unitTypes.Count == 0) return;

        int budget = side == Faction.Attacker ? config.attackerUnitBudget : config.defenderUnitBudget;
        var roster = BuildUnitRoster(faction, budget);
        int totalCount = 0;
        foreach (var (_, c) in roster) totalCount += c;

        var units = new List<Unit>();
        Vector3 center = GetPlacementCenter(side);
        Vector3 facing = GetFacing(side);
        var positions = FormationController.GetFormationPositions(totalCount, center, facing, FormationType.Line, 2f);

        int idx = 0;
        foreach (var (typeDef, count) in roster)
        {
            for (int k = 0; k < count && idx < positions.Length; k++, idx++)
            {
                Vector3 pos = positions[idx];
                if (Terrain.activeTerrain != null)
                    pos.y = Terrain.activeTerrain.SampleHeight(pos);

                Unit unit = UnitFactory.CreateUnit(typeDef, faction, side, pos, Quaternion.LookRotation(facing));
                units.Add(unit);
                playerUnits.Add(unit);

                if (side == Faction.Attacker)
                    BattleManager.Instance?.RegisterAttackerUnit(unit);
                else
                    BattleManager.Instance?.RegisterDefenderUnit(unit);
            }
        }

        FormationController.ArrangeByCategory(units, center, facing);
    }

    public void AutoPlaceOpponent(FactionDefinition faction, Faction side)
    {
        if (faction == null || faction.unitTypes == null || faction.unitTypes.Count == 0) return;

        int budget = side == Faction.Attacker ? config.attackerUnitBudget : config.defenderUnitBudget;
        var roster = BuildUnitRoster(faction, budget);
        var units = new List<Unit>();
        Vector3 center = GetPlacementCenter(side);
        Vector3 facing = GetFacing(side);

        foreach (var (typeDef, count) in roster)
        {
            for (int k = 0; k < count; k++)
            {
                Vector3 pos = center + Random.insideUnitSphere * 5f;
                pos.y = 0;
                if (Terrain.activeTerrain != null)
                    pos.y = Terrain.activeTerrain.SampleHeight(pos);

                Unit unit = UnitFactory.CreateUnit(typeDef, faction, side, pos, Quaternion.LookRotation(facing));
                units.Add(unit);

                if (side == Faction.Attacker)
                    BattleManager.Instance?.RegisterAttackerUnit(unit);
                else
                    BattleManager.Instance?.RegisterDefenderUnit(unit);
            }
        }

        FormationController.ArrangeByCategory(units, center, facing);
    }

    List<(UnitTypeDefinition, int)> BuildUnitRoster(FactionDefinition faction, int budget)
    {
        var roster = new List<(UnitTypeDefinition, int)>();
        if (faction.unitTypes == null || faction.unitTypes.Count == 0) return roster;

        int perType = budget / faction.unitTypes.Count;
        int remainder = budget % faction.unitTypes.Count;

        for (int i = 0; i < faction.unitTypes.Count; i++)
        {
            int count = perType + (i < remainder ? 1 : 0);
            if (count > 0 && faction.unitTypes[i] != null)
                roster.Add((faction.unitTypes[i], count));
        }
        return roster;
    }

    Vector3 GetPlacementCenter(Faction side)
    {
        float depth = GameConfig.PlacementZoneDepth * mapSize;
        float centerZ = mapSize * 0.5f;
        if (side == Faction.Attacker)
            return new Vector3(mapSize * 0.5f, 0, depth * 0.5f);
        return new Vector3(mapSize * 0.5f, 0, mapSize - depth * 0.5f);
    }

    Vector3 GetFacing(Faction side)
    {
        return side == Faction.Attacker ? Vector3.forward : Vector3.back;
    }

    public void CreatePlacementZone(Faction side, int size)
    {
        if (placementZoneObj != null) return;

        GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Plane);
        zone.name = "PlacementZone";
        zone.transform.localScale = new Vector3(size * 0.1f, 1f, GameConfig.PlacementZoneDepth * size * 0.1f);
        float z = side == Faction.Attacker ? GameConfig.PlacementZoneDepth * size * 0.5f : size - GameConfig.PlacementZoneDepth * size * 0.5f;
        zone.transform.position = new Vector3(size * 0.5f, 0, z);
        zone.transform.rotation = Quaternion.identity;

        var rend = zone.GetComponent<Renderer>();
        Color c = side == Faction.Attacker ? new Color(0.2f, 0.4f, 0.8f, 0.15f) : new Color(0.8f, 0.2f, 0.2f, 0.15f);
        rend.material = ShaderHelper.CreateMaterial(c);
        Object.Destroy(zone.GetComponent<Collider>());
        placementZoneObj = zone;
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragStart = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                Unit under = GetUnitUnderCursor();
                if (Vector3.Distance(dragStart, Input.mousePosition) < 10f)
                {
                    float now = Time.unscaledTime;
                    if (now - lastClickTime < DoubleClickThreshold && under != null && IsPlayerUnit(under))
                    {
                        SelectAllOfType(under.typeDefinition?.category ?? UnitCategory.HeavyInfantry);
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            AddToSelection(under);
                        else
                            SelectUnit(under);
                    }
                    lastClickTime = now;
                }
                else
                {
                    SelectUnitsInBox(dragStart, Input.mousePosition);
                }
                isDragging = false;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            SelectAllPlayerUnits();
        }
    }

    void HandleDragMove()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0 && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Vector3 target = hit.point;
                target.y = 0;
                if (Terrain.activeTerrain != null)
                    target.y = Terrain.activeTerrain.SampleHeight(target);

                if (IsValidPlacement(target, PlayerSide ?? Faction.Attacker))
                {
                    moveTarget = target;
                    Vector3 center = GetFormationCenter(selectedUnits);
                    Vector3 facing = (target - center).normalized;
                    facing.y = 0;
                    if (facing.sqrMagnitude < 0.01f) facing = Vector3.forward;
                    var positions = FormationController.GetFormationPositions(selectedUnits.Count, target, facing, currentFormation, MinSpacing);
                    EnforceSpacingAndMove(selectedUnits, positions);
                }
            }
        }
    }

    void HandleRotation()
    {
        if (selectedUnits.Count == 0) return;

        float rot = 0f;
        if (Input.GetKeyDown(KeyCode.R))
            rot = 45f;
        if (Input.GetMouseButton(2))
            rot = Input.GetAxis("Mouse X") * 3f;

        if (Mathf.Abs(rot) > 0.01f)
        {
            Vector3 center = GetFormationCenter(selectedUnits);
            FormationController.RotateFormation(selectedUnits, center, rot);
        }
    }

    void HandleFormationHotkeys()
    {
        if (selectedUnits.Count == 0) return;
        FormationType? next = null;
        if (Input.GetKeyDown(KeyCode.F1)) next = FormationType.Line;
        if (Input.GetKeyDown(KeyCode.F2)) next = FormationType.Column;
        if (Input.GetKeyDown(KeyCode.F3)) next = FormationType.Wedge;
        if (Input.GetKeyDown(KeyCode.F4)) next = FormationType.Square;
        if (Input.GetKeyDown(KeyCode.F5)) next = FormationType.Spread;
        if (next.HasValue)
            SetFormation(next.Value);
    }

    void HandleNumberKeys()
    {
        if (!Input.anyKeyDown) return;
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                var cat = (UnitCategory)(i - 1);
                if ((int)cat < System.Enum.GetValues(typeof(UnitCategory)).Length)
                    SelectAllOfType(cat);
                break;
            }
        }
    }

    Unit GetUnitUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            var unit = hit.collider.GetComponentInParent<Unit>();
            return unit;
        }
        return null;
    }

    bool IsPlayerUnit(Unit u)
    {
        return u != null && PlayerSide.HasValue && u.faction == PlayerSide.Value;
    }

    void SelectUnit(Unit u)
    {
        ClearSelection();
        if (u != null && IsPlayerUnit(u))
            AddToSelection(u);
    }

    void AddToSelection(Unit u)
    {
        if (u == null || !IsPlayerUnit(u)) return;
        if (!selectedUnits.Contains(u))
        {
            selectedUnits.Add(u);
            u.SetSelected(true);
        }
    }

    void ClearSelection()
    {
        foreach (Unit u in selectedUnits)
            if (u != null) u.SetSelected(false);
        selectedUnits.Clear();
    }

    void SelectUnitsInBox(Vector3 screenStart, Vector3 screenEnd)
    {
        ClearSelection();
        Rect rect = Rect.MinMaxRect(
            Mathf.Min(screenStart.x, screenEnd.x),
            Mathf.Min(screenStart.y, screenEnd.y),
            Mathf.Max(screenStart.x, screenEnd.x),
            Mathf.Max(screenStart.y, screenEnd.y));

        foreach (Unit u in playerUnits)
        {
            if (u == null || u.isDead) continue;
            Vector3 screen = Camera.main.WorldToScreenPoint(u.transform.position);
            if (rect.Contains(screen))
                AddToSelection(u);
        }
    }

    void SelectAllPlayerUnits()
    {
        ClearSelection();
        foreach (Unit u in playerUnits)
            if (u != null && !u.isDead) AddToSelection(u);
    }

    public void SelectAllOfType(UnitCategory category)
    {
        ClearSelection();
        foreach (Unit u in playerUnits)
        {
            if (u == null || u.isDead) continue;
            if (u.typeDefinition != null && u.typeDefinition.category == category)
                AddToSelection(u);
        }
    }

    public void SetFormation(FormationType formation)
    {
        currentFormation = formation;
        if (selectedUnits.Count == 0) return;

        Vector3 center = GetFormationCenter(selectedUnits);
        Vector3 facing = GetAverageFacing(selectedUnits);
        FormationController.ArrangeInFormation(selectedUnits, center, facing, formation, MinSpacing);
    }

    public bool IsValidPlacement(Vector3 position, Faction side)
    {
        float depth = GameConfig.PlacementZoneDepth * mapSize;
        if (side == Faction.Attacker)
            return position.z >= 0 && position.z <= depth && position.x >= 0 && position.x <= mapSize;
        return position.z >= mapSize - depth && position.z <= mapSize && position.x >= 0 && position.x <= mapSize;
    }

    public void ConfirmPlacement()
    {
        if (IsSpawning) return;
        ClearSelection();
        StartCoroutine(CountdownThenStart());
    }

    System.Collections.IEnumerator CountdownThenStart()
    {
        while (IsSpawning) yield return null;
        yield return new WaitForSecondsRealtime(GameConfig.CountdownDuration);
        if (BattleManager.Instance != null)
            BattleManager.Instance.StartSimulation();
    }

    public void AutoPlacePlayerUnits()
    {
        if (!PlayerSide.HasValue) return;
        var units = GetPlacedUnits(PlayerSide.Value);
        if (units.Count == 0) return;
        Vector3 center = GetPlacementCenter(PlayerSide.Value);
        Vector3 facing = GetFacing(PlayerSide.Value);
        FormationController.ArrangeByCategory(units, center, facing);
    }

    public List<Unit> GetPlacedUnits(Faction side)
    {
        var list = new List<Unit>();
        foreach (Unit u in playerUnits)
            if (u != null && !u.isDead && u.faction == side) list.Add(u);
        return list;
    }

    public void HighlightHoveredUnit(Unit unit)
    {
        if (hoveredUnit == unit) return;
        hoveredUnit = unit;
    }

    public void ShowFormationGhost(List<Unit> selected, Vector3 target)
    {
        foreach (var go in ghostObjects)
            if (go != null) Destroy(go);
        ghostObjects.Clear();

        if (selected == null || selected.Count == 0) return;

        Vector3 center = GetFormationCenter(selected);
        var positions = FormationController.GetGhostPositions(selected, target);
        foreach (Vector3 pos in positions)
        {
            GameObject ghost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ghost.name = "FormationGhost";
            ghost.transform.position = pos + Vector3.up * 0.5f;
            ghost.transform.localScale = new Vector3(0.8f, 0.02f, 0.8f);
            var rend = ghost.GetComponent<Renderer>();
            rend.material = ShaderHelper.CreateMaterial(new Color(0.2f, 1f, 0.3f, 0.4f));
            Object.Destroy(ghost.GetComponent<Collider>());
            ghostObjects.Add(ghost);
        }
    }

    void EnforceSpacingAndMove(List<Unit> units, Vector3[] positions)
    {
        for (int i = 0; i < units.Count && i < positions.Length; i++)
        {
            Unit u = units[i];
            if (u == null || u.isDead) continue;
            Vector3 pos = positions[i];
            for (int j = 0; j < i; j++)
            {
                if (Vector3.Distance(pos, positions[j]) < MinSpacing)
                    pos += (pos - positions[j]).normalized * MinSpacing;
            }
            u.GetComponent<UnitMovement>()?.MoveTo(pos);
        }
    }

    static Vector3 GetFormationCenter(List<Unit> units)
    {
        Vector3 sum = Vector3.zero;
        int n = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            sum += u.transform.position;
            n++;
        }
        return n > 0 ? sum / n : Vector3.zero;
    }

    static Vector3 GetAverageFacing(List<Unit> units)
    {
        Vector3 sum = Vector3.zero;
        int n = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            sum += u.transform.forward;
            n++;
        }
        sum.y = 0;
        return n > 0 ? sum.normalized : Vector3.forward;
    }
}
