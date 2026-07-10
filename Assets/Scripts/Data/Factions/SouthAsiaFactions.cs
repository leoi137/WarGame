using System.Collections.Generic;
using UnityEngine;

public static class SouthAsiaFactions
{
    public static List<FactionDefinition> Create()
    {
        return new List<FactionDefinition>
        {
            CreateChola(),
            CreateChalukya(),
            CreatePala(),
            CreateRajput()
        };
    }

    private static FactionDefinition CreateChola()
    {
        var f = new FactionDefinition
        {
            id = "chola",
            displayName = "Chola Empire",
            region = Region.SouthAsia,
            capitalCityId = "cho_thanjavur",
            primaryColor = new Color(0.85f, 0.65f, 0.10f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 122000,
            rulerName = "Rajendra Chola I",
            rulerBonus = "",
            factionTrait = "Naval Supremacy",
            factionTraitDescription = "Naval units +25% combat bonus",
            strategicAsset = "Brihadeeswarar Temple",
            strategicAssetDescription = "+20% morale"
        };
        f.terrainDistribution[TerrainType.Coast] = 20;
        f.terrainDistribution[TerrainType.Plains] = 18;
        f.terrainDistribution[TerrainType.RiverValley] = 16;
        f.terrainDistribution[TerrainType.Jungle] = 10;
        f.terrainDistribution[TerrainType.Hills] = 12;
        f.terrainDistribution[TerrainType.Mountains] = 10;
        f.terrainDistribution[TerrainType.Forest] = 8;
        f.terrainDistribution[TerrainType.Wetlands] = 6;
        f.cities.Add(C("cho_thanjavur", "Thanjavur", 18000, 0.665f, 0.440f, TerrainType.Plains, TerrainType.RiverValley, true));
        f.cities.Add(C("cho_gangaikonda", "Gangaikonda", 12000, 0.667f, 0.442f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("cho_kanchipuram", "Kanchipuram", 10000, 0.662f, 0.445f, TerrainType.Plains, TerrainType.Hills, false));
        f.cities.Add(C("cho_nagapattinam", "Nagapattinam", 8000, 0.668f, 0.435f, TerrainType.Coast, TerrainType.Plains, false));
        var skin = new Color(0.65f, 0.45f, 0.30f);
        var elephantVis = V(WeaponStyle.Elephant, ArmorStyle.Heavy, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin);
        elephantVis.bodyScale = 2.5f;
        elephantVis.shoulderWidth = 0.50f;
        elephantVis.hipWidth = 0.30f;
        f.unitTypes.Add(U("cho_infantry", "chola", "Chola Infantry", UnitCategory.HeavyInfantry, 100, 12, 5, 3.4f, 2.5f, 1.1f, "formation_discipline", V(WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("cho_cavalry", "chola", "Chola Cavalry", UnitCategory.HeavyCavalry, 95, 16, 4, 4.5f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("cho_elephant", "chola", "Chola Elephant Corps", UnitCategory.Elephant, 240, 24, 7, 2.3f, 3.5f, 1.8f, "elephant_charge", elephantVis, ""));
        f.unitTypes.Add(U("cho_guard", "chola", "Velaikkarar Guard", UnitCategory.Special, 90, 15, 4, 3.8f, 2.5f, 0.8f, "inspire", V(WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Crown, ShieldStyle.Round, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("cho_marines", "chola", "Chola Navy Marines", UnitCategory.Naval, 80, 13, 2, 4.2f, 2.5f, 0.9f, "naval_boarding", V(WeaponStyle.Sword, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.Buckler, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreateChalukya()
    {
        var f = new FactionDefinition
        {
            id = "chalukya",
            displayName = "Western Chalukya",
            region = Region.SouthAsia,
            capitalCityId = "cha_manyakheta",
            primaryColor = new Color(0.15f, 0.25f, 0.65f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 92000,
            rulerName = "Jayasimha II",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.Plains] = 16;
        f.terrainDistribution[TerrainType.Hills] = 22;
        f.terrainDistribution[TerrainType.Mountains] = 18;
        f.terrainDistribution[TerrainType.Forest] = 12;
        f.terrainDistribution[TerrainType.RiverValley] = 10;
        f.terrainDistribution[TerrainType.Coast] = 8;
        f.terrainDistribution[TerrainType.Jungle] = 14;
        f.cities.Add(C("cha_manyakheta", "Manyakheta", 16000, 0.660f, 0.460f, TerrainType.Plains, TerrainType.Hills, true));
        f.cities.Add(C("cha_banavasi", "Banavasi", 7000, 0.658f, 0.455f, TerrainType.Forest, TerrainType.Plains, false));
        f.cities.Add(C("cha_lakkundi", "Lakkundi", 6000, 0.659f, 0.458f, TerrainType.Plains, TerrainType.Hills, false));
        var skin = new Color(0.65f, 0.45f, 0.30f);
        var elephantVis = V(WeaponStyle.Elephant, ArmorStyle.Heavy, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin);
        elephantVis.bodyScale = 2.5f;
        elephantVis.shoulderWidth = 0.50f;
        elephantVis.hipWidth = 0.30f;
        f.unitTypes.Add(U("cha_heavy_cav", "chalukya", "Chalukya Heavy Cavalry", UnitCategory.HeavyCavalry, 105, 18, 6, 4.4f, 2.5f, 1.0f, null, V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("cha_infantry", "chalukya", "Chalukya Infantry", UnitCategory.HeavyInfantry, 100, 11, 5, 3.3f, 2.5f, 1.1f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("cha_elephant", "chalukya", "Chalukya Elephant", UnitCategory.Elephant, 235, 23, 7, 2.2f, 3.5f, 1.9f, "elephant_charge", elephantVis, ""));
        f.unitTypes.Add(U("cha_archers", "chalukya", "Chalukya Archers", UnitCategory.Ranged, 50, 11, 0, 3.9f, 12.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("cha_feudatory", "chalukya", "Feudatory Troops", UnitCategory.LightInfantry, 75, 12, 2, 3.8f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreatePala()
    {
        var f = new FactionDefinition
        {
            id = "pala",
            displayName = "Pala Empire",
            region = Region.SouthAsia,
            capitalCityId = "pal_pataliputra",
            primaryColor = new Color(0.55f, 0.10f, 0.20f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 66000,
            rulerName = "Mahipala I",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.RiverValley] = 28;
        f.terrainDistribution[TerrainType.Plains] = 18;
        f.terrainDistribution[TerrainType.Wetlands] = 18;
        f.terrainDistribution[TerrainType.Jungle] = 12;
        f.terrainDistribution[TerrainType.Forest] = 10;
        f.terrainDistribution[TerrainType.Hills] = 8;
        f.terrainDistribution[TerrainType.Coast] = 6;
        f.cities.Add(C("pal_pataliputra", "Pataliputra", 8000, 0.660f, 0.490f, TerrainType.RiverValley, TerrainType.Plains, true));
        f.cities.Add(C("pal_gauda", "Gauda", 14000, 0.668f, 0.485f, TerrainType.RiverValley, TerrainType.Wetlands, false));
        f.cities.Add(C("pal_vikrampur", "Vikrampur", 6000, 0.672f, 0.480f, TerrainType.Wetlands, TerrainType.RiverValley, false));
        var skin = new Color(0.65f, 0.45f, 0.30f);
        var elephantVis = V(WeaponStyle.Elephant, ArmorStyle.Heavy, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin);
        elephantVis.bodyScale = 2.5f;
        elephantVis.shoulderWidth = 0.50f;
        elephantVis.hipWidth = 0.30f;
        f.unitTypes.Add(U("pal_infantry", "pala", "Pala Infantry", UnitCategory.HeavyInfantry, 95, 11, 4, 3.4f, 2.5f, 1.1f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("pal_cavalry", "pala", "Pala Cavalry", UnitCategory.HeavyCavalry, 95, 16, 4, 4.4f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("pal_elephant", "pala", "Pala War Elephant", UnitCategory.Elephant, 245, 24, 7, 2.2f, 3.5f, 1.8f, "elephant_charge", elephantVis, ""));
        f.unitTypes.Add(U("pal_archers", "pala", "Pala Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null, V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("pal_river", "pala", "River Forces", UnitCategory.Naval, 70, 11, 2, 4.0f, 2.5f, 1.0f, "naval_boarding", V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreateRajput()
    {
        var f = new FactionDefinition
        {
            id = "rajput",
            displayName = "Rajput States",
            region = Region.SouthAsia,
            capitalCityId = "raj_ajmer",
            primaryColor = new Color(0.80f, 0.15f, 0.15f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 98000,
            rulerName = "Rajput Confederation",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.Plains] = 22;
        f.terrainDistribution[TerrainType.Desert] = 20;
        f.terrainDistribution[TerrainType.Hills] = 18;
        f.terrainDistribution[TerrainType.Mountains] = 14;
        f.terrainDistribution[TerrainType.Forest] = 8;
        f.terrainDistribution[TerrainType.RiverValley] = 10;
        f.terrainDistribution[TerrainType.Coast] = 8;
        f.cities.Add(C("raj_ajmer", "Ajmer", 10000, 0.635f, 0.510f, TerrainType.Plains, TerrainType.Desert, true));
        f.cities.Add(C("raj_kannauj", "Kannauj", 12000, 0.645f, 0.510f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("raj_chittor", "Chittor", 8000, 0.638f, 0.505f, TerrainType.Hills, TerrainType.Plains, false));
        f.cities.Add(C("raj_anhilwara", "Anhilwara", 8000, 0.630f, 0.500f, TerrainType.Plains, TerrainType.Desert, false));
        var skin = new Color(0.65f, 0.45f, 0.30f);
        var elephantVis = V(WeaponStyle.Elephant, ArmorStyle.Heavy, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin);
        elephantVis.bodyScale = 2.5f;
        elephantVis.shoulderWidth = 0.50f;
        elephantVis.hipWidth = 0.30f;
        f.unitTypes.Add(U("raj_lancer", "rajput", "Rajput Lancer", UnitCategory.HeavyCavalry, 110, 19, 5, 4.6f, 2.5f, 0.9f, null, V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("raj_infantry", "rajput", "Rajput Infantry", UnitCategory.HeavyInfantry, 95, 11, 4, 3.4f, 2.5f, 1.1f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("raj_archers", "rajput", "Rajput Archers", UnitCategory.Ranged, 50, 10, 0, 3.9f, 12.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("raj_elephant", "rajput", "Rajput War Elephant", UnitCategory.Elephant, 240, 24, 7, 2.2f, 3.5f, 1.8f, "elephant_charge", elephantVis, ""));
        f.unitTypes.Add(U("raj_garrison", "rajput", "Fort Garrison", UnitCategory.HeavyInfantry, 120, 9, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Tower, MaterialPreset.Bronze, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("raj_elite", "rajput", "Rajput Elite Cavalry", UnitCategory.HeavyCavalry, 100, 20, 5, 4.8f, 2.5f, 0.8f, "zealot_charge", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Bronze, MaterialPreset.Silk, false, false, skin), ""));
        return f;
    }

    private static CityDefinition C(string id, string name, int garrison, float x, float y, TerrainType primary, TerrainType secondary, bool capital)
    {
        return new CityDefinition
        {
            id = id,
            displayName = name,
            garrison = garrison,
            normalizedPosition = new Vector2(x, y),
            primaryTerrain = primary,
            secondaryTerrain = secondary,
            isCapital = capital
        };
    }

    private static UnitTypeDefinition U(string id, string factionId, string name, UnitCategory cat, float hp, float atk, float armor, float spd, float range, float cd, string ability, UnitVisualConfig vis, string desc)
    {
        return new UnitTypeDefinition
        {
            id = id,
            factionId = factionId,
            displayName = name,
            category = cat,
            maxHP = hp,
            attackDamage = atk,
            armor = armor,
            moveSpeed = spd,
            attackRange = range,
            attackCooldown = cd,
            abilityId = ability,
            visualConfig = vis,
            description = desc
        };
    }

    private static UnitVisualConfig V(WeaponStyle w, ArmorStyle a, HelmetStyle h, ShieldStyle sh, MaterialPreset am, MaterialPreset wm, bool cape, bool back, Color skin, float bodyScale = 1f, float shoulderW = 0.28f, float hipW = 0.12f)
    {
        return new UnitVisualConfig
        {
            primaryWeapon = w,
            armorStyle = a,
            helmetStyle = h,
            shieldStyle = sh,
            armorMaterial = am,
            weaponMaterial = wm,
            hasCape = cape,
            hasBackItem = back,
            skinTint = skin,
            bodyScale = bodyScale,
            shoulderWidth = shoulderW,
            hipWidth = hipW
        };
    }
}
