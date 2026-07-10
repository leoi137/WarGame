using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    readonly float cellSize;
    readonly int gridWidth;
    readonly int gridHeight;
    readonly List<Unit>[,] cells;
    readonly List<Unit> resultBuffer = new List<Unit>(64);

    public SpatialGrid(float cellSize, int gridWidth, int gridHeight)
    {
        this.cellSize = cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        cells = new List<Unit>[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
                cells[x, z] = new List<Unit>(32);
    }

    public void Clear()
    {
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
                cells[x, z].Clear();
    }

    public void Insert(Unit unit)
    {
        if (unit == null) return;
        Vector3 pos = unit.transform.position;
        int cx = Mathf.Clamp(CellIndex(pos.x), 0, gridWidth - 1);
        int cz = Mathf.Clamp(CellIndex(pos.z), 0, gridHeight - 1);
        cells[cx, cz].Add(unit);
    }

    public List<Unit> GetNearby(Vector3 position, float radius)
    {
        resultBuffer.Clear();
        float radiusSq = radius * radius;
        int minX = CellIndex(position.x - radius);
        int maxX = CellIndex(position.x + radius);
        int minZ = CellIndex(position.z - radius);
        int maxZ = CellIndex(position.z + radius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                if (!InBounds(x, z)) continue;
                var cell = cells[x, z];
                for (int i = 0; i < cell.Count; i++)
                {
                    Unit u = cell[i];
                    if (u == null || u.isDead) continue;
                    float sqDist = (u.transform.position - position).sqrMagnitude;
                    if (sqDist <= radiusSq)
                        resultBuffer.Add(u);
                }
            }
        }
        return resultBuffer;
    }

    public Unit GetNearest(Vector3 position, Faction targetFaction, float maxRange)
    {
        var nearby = GetNearby(position, maxRange);
        Unit nearest = null;
        float nearestSq = maxRange * maxRange;

        for (int i = 0; i < nearby.Count; i++)
        {
            Unit u = nearby[i];
            if (u.faction != targetFaction) continue;
            float sqDist = (u.transform.position - position).sqrMagnitude;
            if (sqDist < nearestSq)
            {
                nearestSq = sqDist;
                nearest = u;
            }
        }
        return nearest;
    }

    public int CountInRadius(Vector3 position, float radius, Faction? factionFilter = null)
    {
        int count = 0;
        float radiusSq = radius * radius;
        int minX = CellIndex(position.x - radius);
        int maxX = CellIndex(position.x + radius);
        int minZ = CellIndex(position.z - radius);
        int maxZ = CellIndex(position.z + radius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                if (!InBounds(x, z)) continue;
                var cell = cells[x, z];
                for (int i = 0; i < cell.Count; i++)
                {
                    Unit u = cell[i];
                    if (u == null || u.isDead) continue;
                    if (factionFilter.HasValue && u.faction != factionFilter.Value) continue;
                    if ((u.transform.position - position).sqrMagnitude <= radiusSq)
                        count++;
                }
            }
        }
        return count;
    }

    int CellIndex(float worldCoord)
    {
        return Mathf.FloorToInt(worldCoord / cellSize);
    }

    bool InBounds(int x, int z)
    {
        return x >= 0 && x < gridWidth && z >= 0 && z < gridHeight;
    }
}
