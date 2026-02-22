using System.Collections.Generic;
using UnityEngine;

public static class FormationController
{
    const float DefaultSpacing = 2f;

    public static void ArrangeFormation(List<Unit> units, Vector3 center, Vector3 facing, float spacing)
    {
        if (units == null || units.Count == 0) return;
        Vector3[] positions = GetFormationPositions(units.Count, center, facing, spacing);
        for (int i = 0; i < units.Count && i < positions.Length; i++)
        {
            if (units[i] == null || units[i].isDead) continue;
            units[i].GetComponent<UnitMovement>()?.MoveTo(positions[i]);
        }
    }

    public static void ArrangeByCategory(List<Unit> units, Vector3 center, Vector3 facing)
    {
        if (units == null || units.Count == 0) return;
        var front = new List<Unit>();
        var mid = new List<Unit>();
        var flanks = new List<Unit>();
        var back = new List<Unit>();

        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            UnitCategory cat = GetCategory(u);
            if (cat == UnitCategory.HeavyInfantry) front.Add(u);
            else if (cat == UnitCategory.LightInfantry) mid.Add(u);
            else if (cat == UnitCategory.HeavyCavalry || cat == UnitCategory.LightCavalry) flanks.Add(u);
            else if (cat == UnitCategory.Ranged || cat == UnitCategory.Siege) back.Add(u);
            else mid.Add(u);
        }

        float spacing = DefaultSpacing;
        Vector3 right = Vector3.Cross(Vector3.up, facing.normalized);
        float depth = spacing * 2f;

        int row = 0;
        PlaceRow(front, center - facing * (row * depth), facing, spacing);
        row++;
        PlaceRow(mid, center - facing * (row * depth), facing, spacing);
        int halfFlank = flanks.Count / 2;
        if (halfFlank > 0)
            PlaceRow(flanks.GetRange(0, halfFlank), center - facing * (row * depth) + right * 8f, facing, spacing);
        if (flanks.Count - halfFlank > 0)
            PlaceRow(flanks.GetRange(halfFlank, flanks.Count - halfFlank), center - facing * (row * depth) - right * 8f, facing, spacing);
        row++;
        PlaceRow(back, center - facing * (row * depth), facing, spacing);
    }

    public static void ArrangeInFormation(List<Unit> units, Vector3 center, Vector3 facing, FormationType type, float spacing = 2f)
    {
        if (units == null || units.Count == 0) return;
        Vector3[] positions = GetFormationPositions(units.Count, center, facing, type, spacing);
        for (int i = 0; i < units.Count && i < positions.Length; i++)
        {
            if (units[i] == null || units[i].isDead) continue;
            units[i].GetComponent<UnitMovement>()?.MoveTo(positions[i]);
        }
    }

    public static Vector3[] GetFormationPositions(int count, Vector3 center, Vector3 facing, float spacing)
    {
        return GetFormationPositions(count, center, facing, FormationType.Line, spacing);
    }

    public static Vector3[] GetFormationPositions(int count, Vector3 center, Vector3 facing, FormationType type, float spacing)
    {
        if (count <= 0) return new Vector3[0];
        facing.y = 0;
        facing.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, facing);

        switch (type)
        {
            case FormationType.Column:
                return GetColumnPositions(count, center, facing, spacing);
            case FormationType.Wedge:
                return GetWedgePositions(count, center, facing, spacing);
            case FormationType.Square:
                return GetSquarePositions(count, center, facing, spacing);
            case FormationType.Spread:
                return GetSpreadPositions(count, center, facing, spacing);
            default:
                return GetLinePositions(count, center, facing, spacing);
        }
    }

    public static bool IsFormationIntact(List<Unit> units, float maxDeviation)
    {
        if (units == null || units.Count < 2) return true;
        Vector3 center = GetFormationCenter(units);
        float maxSq = maxDeviation * maxDeviation;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            if ((u.transform.position - center).sqrMagnitude > maxSq) return false;
        }
        return true;
    }

    public static void ReformFormation(List<Unit> units, Vector3 center)
    {
        if (units == null || units.Count == 0) return;
        Vector3 facing = GetAverageFacing(units);
        ArrangeFormation(units, center, facing, DefaultSpacing);
    }

    public static void RotateFormation(List<Unit> units, Vector3 center, float degrees)
    {
        if (units == null || units.Count == 0) return;
        Vector3 currentCenter = GetFormationCenter(units);
        Quaternion rot = Quaternion.Euler(0, degrees, 0);
        Vector3 newFacing = rot * (currentCenter - center).normalized;
        newFacing.y = 0;
        newFacing.Normalize();
        Vector3[] positions = GetFormationPositions(units.Count, center, newFacing, DefaultSpacing);
        for (int i = 0; i < units.Count && i < positions.Length; i++)
        {
            if (units[i] == null || units[i].isDead) continue;
            units[i].GetComponent<UnitMovement>()?.MoveTo(positions[i]);
        }
    }

    public static Vector3[] GetGhostPositions(List<Unit> units, Vector3 newCenter)
    {
        if (units == null || units.Count == 0) return new Vector3[0];
        Vector3 currentCenter = GetFormationCenter(units);
        Vector3 offset = newCenter - currentCenter;
        var positions = new Vector3[units.Count];
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null || units[i].isDead) continue;
            positions[i] = units[i].transform.position + offset;
        }
        return positions;
    }

    static void PlaceRow(List<Unit> row, Vector3 rowCenter, Vector3 facing, float spacing)
    {
        if (row == null || row.Count == 0) return;
        Vector3 right = Vector3.Cross(Vector3.up, facing.normalized);
        int n = row.Count;
        float totalWidth = (n - 1) * spacing;
        for (int i = 0; i < n; i++)
        {
            Unit u = row[i];
            if (u == null || u.isDead) continue;
            Vector3 pos = rowCenter + right * ((i - (n - 1) * 0.5f) * spacing);
            u.GetComponent<UnitMovement>()?.MoveTo(pos);
        }
    }

    static Vector3[] GetLinePositions(int count, Vector3 center, Vector3 facing, float spacing)
    {
        Vector3 right = Vector3.Cross(Vector3.up, facing);
        var positions = new Vector3[count];
        float totalWidth = (count - 1) * spacing;
        for (int i = 0; i < count; i++)
            positions[i] = center + right * ((i - (count - 1) * 0.5f) * spacing);
        return positions;
    }

    static Vector3[] GetColumnPositions(int count, Vector3 center, Vector3 facing, float spacing)
    {
        var positions = new Vector3[count];
        for (int i = 0; i < count; i++)
            positions[i] = center - facing * (i * spacing);
        return positions;
    }

    static Vector3[] GetWedgePositions(int count, Vector3 center, Vector3 facing, float spacing)
    {
        var positions = new Vector3[count];
        Vector3 right = Vector3.Cross(Vector3.up, facing);
        int row = 0;
        int idx = 0;
        while (idx < count)
        {
            int inRow = row + 1;
            for (int j = 0; j < inRow && idx < count; j++, idx++)
            {
                float x = (j - row * 0.5f) * spacing;
                float z = -row * spacing;
                positions[idx] = center + right * x + facing * z;
            }
            row++;
        }
        return positions;
    }

    static Vector3[] GetSquarePositions(int count, Vector3 center, Vector3 facing, float spacing)
    {
        int side = Mathf.CeilToInt(Mathf.Sqrt(count));
        Vector3 right = Vector3.Cross(Vector3.up, facing);
        var positions = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            int row = i / side;
            int col = i % side;
            positions[i] = center + right * ((col - side * 0.5f) * spacing) - facing * (row * spacing);
        }
        return positions;
    }

    static Vector3[] GetSpreadPositions(int count, Vector3 center, Vector3 facing, float spacing)
    {
        var positions = new Vector3[count];
        float radius = Mathf.Sqrt(count) * spacing * 0.5f;
        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * Mathf.PI * 2f;
            positions[i] = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
        return positions;
    }

    static Vector3 GetFormationCenter(List<Unit> units)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            sum += u.transform.position;
            count++;
        }
        return count > 0 ? sum / count : Vector3.zero;
    }

    static Vector3 GetAverageFacing(List<Unit> units)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            sum += u.transform.forward;
            count++;
        }
        sum.y = 0;
        return count > 0 ? sum.normalized : Vector3.forward;
    }

    static UnitCategory GetCategory(Unit unit)
    {
        if (unit?.typeDefinition != null) return unit.typeDefinition.category;
        if (unit == null) return UnitCategory.HeavyInfantry;
        switch (unit.unitType)
        {
            case UnitType.Archer: return UnitCategory.Ranged;
            case UnitType.Berserker: return UnitCategory.LightInfantry;
            default: return UnitCategory.HeavyInfantry;
        }
    }
}
