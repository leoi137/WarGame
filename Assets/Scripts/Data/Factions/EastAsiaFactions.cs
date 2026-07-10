using System.Collections.Generic;
using UnityEngine;

public static class EastAsiaFactions
{
    public static List<FactionDefinition> Create()
    {
        return new List<FactionDefinition>
        {
            Song(),
            Liao(),
            Goryeo(),
            HeianJapan(),
            Dali()
        };
    }

    static FactionDefinition Song()
    {
        var f = new FactionDefinition
        {
            id = "song",
            displayName = "Song Empire",
            region = Region.EastAsia,
            capitalCityId = "son_kaifeng",
            primaryColor = new Color(0.75f, 0.20f, 0.20f),
            secondaryColor = new Color(0.85f, 0.35f, 0.35f),
            estimatedMilitary = 900000,
            rulerName = "Emperor Zhenzong",
            rulerBonus = null,
            factionTrait = "Gunpowder Workshops",
            factionTraitDescription = "Fire lance units +25% damage",
            strategicAsset = "Grand Canal",
            strategicAssetDescription = "Massive income + movement bonus"
        };
        f.terrainDistribution[TerrainType.Plains] = 24;
        f.terrainDistribution[TerrainType.RiverValley] = 22;
        f.terrainDistribution[TerrainType.Forest] = 14;
        f.terrainDistribution[TerrainType.Hills] = 12;
        f.terrainDistribution[TerrainType.Mountains] = 10;
        f.terrainDistribution[TerrainType.Coast] = 8;
        f.terrainDistribution[TerrainType.Wetlands] = 5;
        f.terrainDistribution[TerrainType.Jungle] = 5;
        f.cities.Add(C("son_kaifeng", "Kaifeng", 90000, 0.755f, 0.585f, TerrainType.Plains, TerrainType.RiverValley, true));
        f.cities.Add(C("son_luoyang", "Luoyang", 45000, 0.748f, 0.587f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("son_taiyuan", "Taiyuan", 55000, 0.752f, 0.600f, TerrainType.Plains, TerrainType.Hills, false));
        f.cities.Add(C("son_chengdu", "Chengdu", 40000, 0.735f, 0.565f, TerrainType.RiverValley, TerrainType.Forest, false));
        f.cities.Add(C("son_hangzhou", "Hangzhou", 30000, 0.760f, 0.570f, TerrainType.Coast, TerrainType.RiverValley, false));
        var sonGuard = U("son_guard", "song", "Song Guard Infantry", UnitCategory.HeavyInfantry, 105, 12, 5, 3.4f, 2.5f, 1.1f, "formation_discipline");
        sonGuard.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Heavy, helmetStyle = HelmetStyle.Conical, shieldStyle = ShieldStyle.Tower, primaryWeapon = WeaponStyle.Spear, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Steel, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(sonGuard);
        var sonCav = U("son_cavalry", "song", "Song Cavalry", UnitCategory.HeavyCavalry, 95, 15, 4, 4.4f, 2.5f, 1.0f, null);
        sonCav.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Heavy, helmetStyle = HelmetStyle.Conical, primaryWeapon = WeaponStyle.Spear, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Steel, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(sonCav);
        var sonCrossbow = U("son_crossbow", "song", "Crossbow Corps", UnitCategory.Ranged, 60, 15, 2, 3.4f, 13.0f, 1.6f, "volley_fire");
        sonCrossbow.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Medium, helmetStyle = HelmetStyle.Straw, primaryWeapon = WeaponStyle.Crossbow, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Steel, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(sonCrossbow);
        var sonRiver = U("son_riverfleet", "song", "River Fleet", UnitCategory.Naval, 80, 12, 3, 4.0f, 2.5f, 1.0f, "naval_boarding");
        sonRiver.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Medium, helmetStyle = HelmetStyle.Straw, shieldStyle = ShieldStyle.Round, primaryWeapon = WeaponStyle.Spear, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Steel, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(sonRiver);
        var sonFire = U("son_firelance", "song", "Fire Lance Troops", UnitCategory.Ranged, 60, 16, 1, 3.5f, 6.0f, 2.0f, "fire_lance");
        sonFire.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Light, helmetStyle = HelmetStyle.Straw, primaryWeapon = WeaponStyle.None, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Bronze, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(sonFire);
        return f;
    }

    static FactionDefinition Liao()
    {
        var f = new FactionDefinition
        {
            id = "liao",
            displayName = "Liao Dynasty",
            region = Region.EastAsia,
            capitalCityId = "lia_shangjing",
            primaryColor = new Color(0.15f, 0.20f, 0.55f),
            secondaryColor = new Color(0.25f, 0.30f, 0.65f),
            estimatedMilitary = 185000,
            rulerName = "Shengzong",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.Steppe] = 36;
        f.terrainDistribution[TerrainType.Plains] = 16;
        f.terrainDistribution[TerrainType.Forest] = 16;
        f.terrainDistribution[TerrainType.Mountains] = 12;
        f.terrainDistribution[TerrainType.Hills] = 10;
        f.terrainDistribution[TerrainType.RiverValley] = 6;
        f.terrainDistribution[TerrainType.Coast] = 4;
        f.cities.Add(C("lia_shangjing", "Shangjing", 22000, 0.760f, 0.620f, TerrainType.Steppe, TerrainType.Plains, true));
        f.cities.Add(C("lia_nanjing", "Nanjing", 28000, 0.758f, 0.612f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("lia_zhongjing", "Zhongjing", 16000, 0.762f, 0.618f, TerrainType.Steppe, TerrainType.Plains, false));
        f.cities.Add(C("lia_dongjing", "Dongjing", 14000, 0.765f, 0.615f, TerrainType.Steppe, TerrainType.Forest, false));
        f.unitTypes.Add(U("lia_orda", "liao", "Imperial Orda Cavalry", UnitCategory.HeavyCavalry, 115, 19, 6, 4.6f, 2.5f, 0.9f, null));
        f.unitTypes.Add(U("lia_khitan", "liao", "Khitan Cavalry", UnitCategory.LightCavalry, 65, 12, 2, 5.2f, 7.0f, 1.2f, "horse_archer_kite"));
        f.unitTypes.Add(U("lia_auxiliary", "liao", "Auxiliary Cavalry", UnitCategory.HeavyCavalry, 95, 15, 4, 4.4f, 2.5f, 1.0f, null));
        f.unitTypes.Add(U("lia_han", "liao", "Han Militia", UnitCategory.HeavyInfantry, 85, 9, 3, 3.3f, 2.5f, 1.3f, "formation_discipline"));
        f.unitTypes.Add(U("lia_archers", "liao", "Foot Archers", UnitCategory.Ranged, 50, 11, 0, 3.8f, 12.0f, 1.4f, "mark"));
        return f;
    }

    static FactionDefinition Goryeo()
    {
        var f = new FactionDefinition
        {
            id = "goryeo",
            displayName = "Goryeo",
            region = Region.EastAsia,
            capitalCityId = "gor_kaesong",
            primaryColor = new Color(0.20f, 0.55f, 0.35f),
            secondaryColor = new Color(0.30f, 0.65f, 0.45f),
            estimatedMilitary = 72000,
            rulerName = "Hyeonjong",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.Mountains] = 30;
        f.terrainDistribution[TerrainType.Hills] = 20;
        f.terrainDistribution[TerrainType.Forest] = 20;
        f.terrainDistribution[TerrainType.Plains] = 10;
        f.terrainDistribution[TerrainType.Coast] = 10;
        f.terrainDistribution[TerrainType.RiverValley] = 5;
        f.terrainDistribution[TerrainType.Wetlands] = 5;
        f.cities.Add(C("gor_kaesong", "Kaesong", 14000, 0.775f, 0.600f, TerrainType.Hills, TerrainType.Plains, true));
        f.cities.Add(C("gor_pyongyang", "Pyongyang", 7000, 0.773f, 0.608f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("gor_gyeongju", "Gyeongju", 5000, 0.780f, 0.595f, TerrainType.Forest, TerrainType.Hills, false));
        f.unitTypes.Add(U("gor_central", "goryeo", "Central Army", UnitCategory.HeavyInfantry, 105, 12, 5, 3.4f, 2.5f, 1.1f, "formation_discipline"));
        f.unitTypes.Add(U("gor_frontier", "goryeo", "Frontier Troops", UnitCategory.HeavyInfantry, 95, 11, 4, 3.5f, 2.5f, 1.0f, "fortify"));
        f.unitTypes.Add(U("gor_reserve", "goryeo", "Reserve Infantry", UnitCategory.LightInfantry, 75, 10, 2, 3.8f, 2.5f, 1.0f, null));
        f.unitTypes.Add(U("gor_archers", "goryeo", "Goryeo Archers", UnitCategory.Ranged, 50, 11, 0, 3.9f, 12.0f, 1.4f, "mark"));
        f.unitTypes.Add(U("gor_cavalry", "goryeo", "Goryeo Cavalry", UnitCategory.HeavyCavalry, 95, 15, 4, 4.4f, 2.5f, 1.0f, null));
        return f;
    }

    static FactionDefinition HeianJapan()
    {
        var f = new FactionDefinition
        {
            id = "japan",
            displayName = "Heian Japan",
            region = Region.EastAsia,
            capitalCityId = "jpn_kyoto",
            primaryColor = new Color(0.90f, 0.90f, 0.90f),
            secondaryColor = new Color(0.95f, 0.95f, 0.95f),
            estimatedMilitary = 61000,
            rulerName = "Emperor Sanjō",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.Mountains] = 34;
        f.terrainDistribution[TerrainType.Forest] = 26;
        f.terrainDistribution[TerrainType.Hills] = 16;
        f.terrainDistribution[TerrainType.Coast] = 14;
        f.terrainDistribution[TerrainType.Plains] = 6;
        f.terrainDistribution[TerrainType.RiverValley] = 4;
        f.cities.Add(C("jpn_kyoto", "Kyoto", 11000, 0.790f, 0.590f, TerrainType.Plains, TerrainType.RiverValley, true));
        f.cities.Add(C("jpn_nara", "Nara", 4000, 0.790f, 0.588f, TerrainType.Plains, TerrainType.Forest, false));
        f.cities.Add(C("jpn_dazaifu", "Dazaifu", 5000, 0.785f, 0.580f, TerrainType.Coast, TerrainType.Hills, false));
        f.cities.Add(C("jpn_kanto", "Kantō", 12000, 0.795f, 0.592f, TerrainType.Hills, TerrainType.Mountains, false));
        var jpnSamurai = U("jpn_samurai", "japan", "Early Samurai", UnitCategory.HeavyCavalry, 100, 18, 5, 4.5f, 2.5f, 0.9f, "parry");
        jpnSamurai.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Heavy, helmetStyle = HelmetStyle.Conical, primaryWeapon = WeaponStyle.Sword, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Steel, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(jpnSamurai);
        f.unitTypes.Add(U("jpn_horsearcher", "japan", "Mounted Archer", UnitCategory.LightCavalry, 60, 12, 1, 5.0f, 8.0f, 1.3f, "horse_archer_kite"));
        f.unitTypes.Add(U("jpn_militia", "japan", "Provincial Militia", UnitCategory.HeavyInfantry, 85, 9, 3, 3.3f, 2.5f, 1.3f, "formation_discipline"));
        f.unitTypes.Add(U("jpn_spearman", "japan", "Ashigaru Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.4f, 3.0f, 1.2f, "pike_brace"));
        var jpnPalace = U("jpn_palace", "japan", "Palace Guards", UnitCategory.HeavyInfantry, 110, 14, 6, 3.5f, 2.5f, 1.0f, "parry");
        jpnPalace.visualConfig = new UnitVisualConfig { armorStyle = ArmorStyle.Heavy, helmetStyle = HelmetStyle.Conical, primaryWeapon = WeaponStyle.Sword, armorMaterial = MaterialPreset.Lacquer, weaponMaterial = MaterialPreset.Steel, skinTint = new Color(0.90f, 0.78f, 0.60f) };
        f.unitTypes.Add(jpnPalace);
        return f;
    }

    static FactionDefinition Dali()
    {
        var f = new FactionDefinition
        {
            id = "dali",
            displayName = "Dali Kingdom",
            region = Region.EastAsia,
            capitalCityId = "dal_dali",
            primaryColor = new Color(0.25f, 0.50f, 0.20f),
            secondaryColor = new Color(0.35f, 0.60f, 0.30f),
            estimatedMilitary = 31000,
            rulerName = "Duan Sufeng",
            rulerBonus = null,
            factionTrait = null,
            factionTraitDescription = null,
            strategicAsset = null,
            strategicAssetDescription = null
        };
        f.terrainDistribution[TerrainType.Mountains] = 34;
        f.terrainDistribution[TerrainType.Hills] = 24;
        f.terrainDistribution[TerrainType.Jungle] = 14;
        f.terrainDistribution[TerrainType.Forest] = 12;
        f.terrainDistribution[TerrainType.RiverValley] = 8;
        f.terrainDistribution[TerrainType.Plains] = 4;
        f.terrainDistribution[TerrainType.Coast] = 4;
        f.cities.Add(C("dal_dali", "Dali", 10000, 0.735f, 0.540f, TerrainType.Mountains, TerrainType.RiverValley, true));
        f.cities.Add(C("dal_kunming", "Kunming", 6000, 0.738f, 0.545f, TerrainType.Hills, TerrainType.Forest, false));
        f.cities.Add(C("dal_yongchang", "Yongchang", 4000, 0.730f, 0.535f, TerrainType.Mountains, TerrainType.Jungle, false));
        f.unitTypes.Add(U("dal_spearman", "dali", "Dali Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.4f, 3.0f, 1.2f, "formation_discipline"));
        f.unitTypes.Add(U("dal_archers", "dali", "Dali Archers", UnitCategory.Ranged, 50, 10, 0, 3.9f, 12.0f, 1.4f, "poison_arrow"));
        f.unitTypes.Add(U("dal_hill", "dali", "Hill Troops", UnitCategory.LightInfantry, 75, 13, 2, 4.2f, 2.5f, 0.8f, "ambush"));
        f.unitTypes.Add(U("dal_cavalry", "dali", "Dali Cavalry", UnitCategory.LightCavalry, 65, 12, 2, 4.8f, 2.5f, 1.0f, "skirmish"));
        f.unitTypes.Add(U("dal_elephant", "dali", "Dali Elephant", UnitCategory.Elephant, 230, 22, 6, 2.2f, 3.5f, 1.9f, "elephant_charge"));
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
}
