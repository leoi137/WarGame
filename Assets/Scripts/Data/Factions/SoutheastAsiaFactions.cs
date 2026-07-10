using System.Collections.Generic;
using UnityEngine;

public static class SoutheastAsiaFactions
{
    static readonly Color SeaSkin = new Color(0.70f, 0.50f, 0.35f);

    public static List<FactionDefinition> Create()
    {
        return new List<FactionDefinition>
        {
            Khmer(),
            Srivijaya(),
            DaiCoViet(),
            Champa(),
            Pagan()
        };
    }

    static FactionDefinition Khmer()
    {
        var f = new FactionDefinition
        {
            id = "khmer",
            displayName = "Khmer Empire",
            region = Region.SoutheastAsia,
            capitalCityId = "khm_angkor",
            primaryColor = new Color(0.75f, 0.65f, 0.15f),
            secondaryColor = new Color(0.85f, 0.75f, 0.25f),
            estimatedMilitary = 78000,
            rulerName = "Suryavarman I",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = "Angkor Wat",
            strategicAssetDescription = "+25% morale globally"
        };
        f.terrainDistribution[TerrainType.Jungle] = 28;
        f.terrainDistribution[TerrainType.Plains] = 14;
        f.terrainDistribution[TerrainType.RiverValley] = 20;
        f.terrainDistribution[TerrainType.Wetlands] = 14;
        f.terrainDistribution[TerrainType.Forest] = 10;
        f.terrainDistribution[TerrainType.Hills] = 8;
        f.terrainDistribution[TerrainType.Mountains] = 4;
        f.terrainDistribution[TerrainType.Coast] = 2;
        f.cities.Add(C("khm_angkor", "Angkor", 20000, 0.745f, 0.400f, TerrainType.Jungle, TerrainType.RiverValley, true));
        f.cities.Add(C("khm_siemreap", "Siem Reap", 8000, 0.744f, 0.402f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("khm_mekong", "Mekong Staging", 9000, 0.748f, 0.395f, TerrainType.RiverValley, TerrainType.Wetlands, false));
        f.unitTypes.Add(U("khm_spearman", "khmer", "Khmer Spearman", UnitCategory.HeavyInfantry, 95, 10, 4, 3.3f, 3.0f, 1.2f, "formation_discipline"));
        f.unitTypes.Add(U("khm_shield", "khmer", "Shield Infantry", UnitCategory.HeavyInfantry, 110, 9, 6, 3.0f, 2.5f, 1.4f, "shield_wall"));
        f.unitTypes.Add(U("khm_elephant", "khmer", "Khmer Elephant", UnitCategory.Elephant, 250, 25, 8, 2.2f, 3.5f, 1.8f, "elephant_charge"));
        f.unitTypes.Add(U("khm_archers", "khmer", "Khmer Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null));
        f.unitTypes.Add(U("khm_engineer", "khmer", "Engineer Corps", UnitCategory.Siege, 150, 25, 2, 2.0f, 14.0f, 3.0f, null));
        foreach (var u in f.unitTypes) u.visualConfig = CopySeaVisual(u.visualConfig);
        return f;
    }

    static FactionDefinition Srivijaya()
    {
        var f = new FactionDefinition
        {
            id = "srivijaya",
            displayName = "Srivijaya",
            region = Region.SoutheastAsia,
            capitalCityId = "sri_palembang",
            primaryColor = new Color(0.15f, 0.55f, 0.55f),
            secondaryColor = new Color(0.25f, 0.65f, 0.65f),
            estimatedMilitary = 47000,
            rulerName = "Sangrama Vijayottunggavarman",
            rulerBonus = null,
            factionTrait = "Maritime Empire",
            factionTraitDescription = "Naval units +20% in all waters",
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.Coast] = 34;
        f.terrainDistribution[TerrainType.Jungle] = 28;
        f.terrainDistribution[TerrainType.Wetlands] = 14;
        f.terrainDistribution[TerrainType.RiverValley] = 10;
        f.terrainDistribution[TerrainType.Plains] = 6;
        f.terrainDistribution[TerrainType.Hills] = 4;
        f.terrainDistribution[TerrainType.Mountains] = 4;
        f.cities.Add(C("sri_palembang", "Palembang", 12000, 0.745f, 0.330f, TerrainType.Coast, TerrainType.Jungle, true));
        f.cities.Add(C("sri_jambi", "Jambi", 7000, 0.743f, 0.335f, TerrainType.Jungle, TerrainType.RiverValley, false));
        f.cities.Add(C("sri_kedah", "Kedah", 6000, 0.740f, 0.360f, TerrainType.Coast, TerrainType.Plains, false));
        f.unitTypes.Add(U("sri_marines", "srivijaya", "Naval Marines", UnitCategory.Naval, 80, 13, 2, 4.3f, 2.5f, 0.9f, "naval_boarding"));
        f.unitTypes.Add(U("sri_shipcrew", "srivijaya", "Ship Crews", UnitCategory.Naval, 70, 11, 1, 4.5f, 2.2f, 0.8f, "naval_boarding"));
        f.unitTypes.Add(U("sri_archers", "srivijaya", "Srivijayan Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null));
        f.unitTypes.Add(U("sri_spearman", "srivijaya", "Srivijayan Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 3.0f, 1.2f, "monsoon_tactics"));
        f.unitTypes.Add(U("sri_tributary", "srivijaya", "Tributary Troops", UnitCategory.LightInfantry, 70, 10, 2, 3.8f, 2.5f, 1.0f, null));
        foreach (var u in f.unitTypes) u.visualConfig = CopySeaVisual(u.visualConfig);
        return f;
    }

    static FactionDefinition DaiCoViet()
    {
        var f = new FactionDefinition
        {
            id = "dai_viet",
            displayName = "Đại Cồ Việt",
            region = Region.SoutheastAsia,
            capitalCityId = "dv_hoalu",
            primaryColor = new Color(0.80f, 0.40f, 0.10f),
            secondaryColor = new Color(0.90f, 0.50f, 0.20f),
            estimatedMilitary = 46000,
            rulerName = "Lê Hoàn",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.RiverValley] = 24;
        f.terrainDistribution[TerrainType.Coast] = 14;
        f.terrainDistribution[TerrainType.Jungle] = 18;
        f.terrainDistribution[TerrainType.Mountains] = 18;
        f.terrainDistribution[TerrainType.Hills] = 12;
        f.terrainDistribution[TerrainType.Forest] = 8;
        f.terrainDistribution[TerrainType.Wetlands] = 6;
        f.cities.Add(C("dv_hoalu", "Hoa Lư", 10000, 0.748f, 0.420f, TerrainType.RiverValley, TerrainType.Mountains, true));
        f.cities.Add(C("dv_daila", "Đại La", 9000, 0.749f, 0.425f, TerrainType.RiverValley, TerrainType.Plains, false));
        f.cities.Add(C("dv_thanhhoa", "Thanh Hóa", 5000, 0.747f, 0.415f, TerrainType.Coast, TerrainType.Hills, false));
        f.unitTypes.Add(U("dv_spearman", "dai_viet", "Viet Spearman", UnitCategory.HeavyInfantry, 95, 10, 4, 3.4f, 3.0f, 1.2f, "pike_brace"));
        f.unitTypes.Add(U("dv_archers", "dai_viet", "Viet Archers", UnitCategory.Ranged, 50, 11, 0, 3.9f, 12.0f, 1.4f, "poison_arrow"));
        f.unitTypes.Add(U("dv_river", "dai_viet", "River Flotilla", UnitCategory.Naval, 75, 12, 2, 4.2f, 2.5f, 0.9f, "naval_boarding"));
        f.unitTypes.Add(U("dv_guards", "dai_viet", "Imperial Guards", UnitCategory.HeavyInfantry, 110, 13, 6, 3.5f, 2.5f, 1.0f, "parry"));
        f.unitTypes.Add(U("dv_militia", "dai_viet", "Levy Militia", UnitCategory.LightInfantry, 70, 9, 2, 3.7f, 2.5f, 1.1f, "monsoon_tactics"));
        foreach (var u in f.unitTypes) u.visualConfig = CopySeaVisual(u.visualConfig);
        return f;
    }

    static FactionDefinition Champa()
    {
        var f = new FactionDefinition
        {
            id = "champa",
            displayName = "Champa",
            region = Region.SoutheastAsia,
            capitalCityId = "cham_indrapura",
            primaryColor = new Color(0.20f, 0.65f, 0.70f),
            secondaryColor = new Color(0.30f, 0.75f, 0.80f),
            estimatedMilitary = 36000,
            rulerName = "Harivarman IV",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.Coast] = 24;
        f.terrainDistribution[TerrainType.Mountains] = 22;
        f.terrainDistribution[TerrainType.Hills] = 16;
        f.terrainDistribution[TerrainType.Jungle] = 16;
        f.terrainDistribution[TerrainType.Forest] = 8;
        f.terrainDistribution[TerrainType.RiverValley] = 8;
        f.terrainDistribution[TerrainType.Plains] = 6;
        f.cities.Add(C("cham_indrapura", "Indrapura", 10000, 0.752f, 0.405f, TerrainType.Coast, TerrainType.Hills, true));
        f.cities.Add(C("cham_kauthara", "Kauthara", 5000, 0.755f, 0.395f, TerrainType.Coast, TerrainType.Mountains, false));
        f.cities.Add(C("cham_panduranga", "Panduranga", 4000, 0.756f, 0.390f, TerrainType.Mountains, TerrainType.Jungle, false));
        f.unitTypes.Add(U("cham_raiders", "champa", "Maritime Raiders", UnitCategory.Naval, 75, 14, 2, 4.5f, 2.5f, 0.8f, "naval_boarding"));
        f.unitTypes.Add(U("cham_infantry", "champa", "Cham Infantry", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 2.5f, 1.1f, "monsoon_tactics"));
        f.unitTypes.Add(U("cham_archers", "champa", "Cham Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null));
        f.unitTypes.Add(U("cham_elephant", "champa", "Cham Elephant", UnitCategory.Elephant, 230, 22, 6, 2.2f, 3.5f, 1.9f, "elephant_charge"));
        f.unitTypes.Add(U("cham_cavalry", "champa", "Cham Cavalry", UnitCategory.LightCavalry, 60, 12, 2, 4.8f, 2.5f, 1.0f, "skirmish"));
        foreach (var u in f.unitTypes) u.visualConfig = CopySeaVisual(u.visualConfig);
        return f;
    }

    static FactionDefinition Pagan()
    {
        var f = new FactionDefinition
        {
            id = "pagan",
            displayName = "Pagan",
            region = Region.SoutheastAsia,
            capitalCityId = "pag_bagan",
            primaryColor = new Color(0.55f, 0.40f, 0.15f),
            secondaryColor = new Color(0.65f, 0.50f, 0.25f),
            estimatedMilitary = 56000,
            rulerName = "Kyiso",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.RiverValley] = 22;
        f.terrainDistribution[TerrainType.Plains] = 16;
        f.terrainDistribution[TerrainType.Jungle] = 18;
        f.terrainDistribution[TerrainType.Forest] = 14;
        f.terrainDistribution[TerrainType.Hills] = 12;
        f.terrainDistribution[TerrainType.Mountains] = 10;
        f.terrainDistribution[TerrainType.Wetlands] = 8;
        f.cities.Add(C("pag_bagan", "Bagan", 14000, 0.738f, 0.415f, TerrainType.RiverValley, TerrainType.Plains, true));
        f.cities.Add(C("pag_prome", "Prome", 6000, 0.736f, 0.410f, TerrainType.RiverValley, TerrainType.Hills, false));
        f.cities.Add(C("pag_irrawaddy", "Irrawaddy Forts", 8000, 0.737f, 0.420f, TerrainType.RiverValley, TerrainType.Coast, false));
        f.unitTypes.Add(U("pag_infantry", "pagan", "Pagan Infantry", UnitCategory.HeavyInfantry, 90, 10, 4, 3.4f, 2.5f, 1.1f, "formation_discipline"));
        f.unitTypes.Add(U("pag_archers", "pagan", "Pagan Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null));
        f.unitTypes.Add(U("pag_elephant", "pagan", "Pagan Elephant", UnitCategory.Elephant, 240, 23, 7, 2.2f, 3.5f, 1.8f, "elephant_charge"));
        f.unitTypes.Add(U("pag_cavalry", "pagan", "Pagan Cavalry", UnitCategory.LightCavalry, 60, 12, 2, 4.8f, 2.5f, 1.0f, "skirmish"));
        f.unitTypes.Add(U("pag_fort", "pagan", "Fort Troops", UnitCategory.HeavyInfantry, 110, 10, 6, 2.8f, 2.5f, 1.4f, "fortify"));
        foreach (var u in f.unitTypes) u.visualConfig = CopySeaVisual(u.visualConfig);
        return f;
    }

    static CityDefinition C(string id, string displayName, int garrison, float x, float y, TerrainType primary, TerrainType secondary, bool isCapital)
    {
        return new CityDefinition
        {
            id = id,
            displayName = displayName,
            garrison = garrison,
            normalizedPosition = new Vector2(x, y),
            primaryTerrain = primary,
            secondaryTerrain = secondary,
            isCapital = isCapital
        };
    }

    static UnitTypeDefinition U(string id, string factionId, string displayName, UnitCategory category, float hp, float atk, float armor, float speed, float range, float cd, string abilityId)
    {
        return new UnitTypeDefinition
        {
            id = id,
            factionId = factionId,
            displayName = displayName,
            category = category,
            maxHP = hp,
            attackDamage = atk,
            armor = armor,
            moveSpeed = speed,
            attackRange = range,
            attackCooldown = cd,
            abilityId = abilityId
        };
    }

    static UnitVisualConfig CopySeaVisual(UnitVisualConfig existing)
    {
        return new UnitVisualConfig
        {
            bodyScale = existing.bodyScale,
            shoulderWidth = existing.shoulderWidth,
            hipWidth = existing.hipWidth,
            skinTint = SeaSkin,
            armorStyle = ArmorStyle.Light,
            helmetStyle = HelmetStyle.Wrapped,
            armorMaterial = MaterialPreset.Bronze,
            weaponMaterial = MaterialPreset.Wood,
            armorTint = existing.armorTint,
            clothTint = existing.clothTint
        };
    }
}
