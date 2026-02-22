using System.Collections.Generic;
using UnityEngine;

public static class MiddleEastFactions
{
    public static List<FactionDefinition> Create()
    {
        return new List<FactionDefinition>
        {
            CreateFatimid(),
            CreateAbbasid(),
            CreateBuyid(),
            CreateGhaznavid(),
            CreateKaraKhanid(),
            CreateKhwarazm(),
            CreateGeorgia(),
            CreateArmenia()
        };
    }

    private static FactionDefinition CreateFatimid()
    {
        var f = new FactionDefinition
        {
            id = "fatimid",
            displayName = "Fatimid Caliphate",
            region = Region.MiddleEast,
            capitalCityId = "fat_cairo",
            primaryColor = new Color(0.10f, 0.60f, 0.25f),
            secondaryColor = new Color(0.85f, 0.80f, 0.20f),
            estimatedMilitary = 72000,
            rulerName = "al-Hakim bi-Amr Allah",
            rulerBonus = "Religious zeal: +15% morale",
            factionTrait = "Ismaili Propaganda",
            factionTraitDescription = "Convert enemy provinces 20% faster",
            strategicAsset = "Al-Azhar Mosque",
            strategicAssetDescription = "Cairo: +30% research, unique Mamluk recruitment"
        };
        f.terrainDistribution[TerrainType.Desert] = 34;
        f.terrainDistribution[TerrainType.RiverValley] = 18;
        f.terrainDistribution[TerrainType.Coast] = 14;
        f.terrainDistribution[TerrainType.Plains] = 10;
        f.terrainDistribution[TerrainType.Hills] = 8;
        f.terrainDistribution[TerrainType.Mountains] = 6;
        f.terrainDistribution[TerrainType.Wetlands] = 10;
        f.cities.Add(C("fat_cairo", "Cairo", 15000, 0.545f, 0.555f, TerrainType.RiverValley, TerrainType.Desert, true));
        f.cities.Add(C("fat_alexandria", "Alexandria", 6000, 0.543f, 0.560f, TerrainType.Coast, TerrainType.Desert, false));
        f.cities.Add(C("fat_jerusalem", "Jerusalem", 4500, 0.555f, 0.575f, TerrainType.Hills, TerrainType.Desert, false));
        f.cities.Add(C("fat_damascus", "Damascus", 5500, 0.558f, 0.585f, TerrainType.Plains, TerrainType.Desert, false));
        f.unitTypes.Add(U("fat_berber_cav", "fatimid", "Berber Cavalry", UnitCategory.HeavyCavalry, 100, 17, 5, 4.6f, 2.5f, 0.9f, null, V(WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, new Color(0.65f, 0.50f, 0.35f)), ""));
        f.unitTypes.Add(U("fat_turkish", "fatimid", "Turkish Guard", UnitCategory.HeavyCavalry, 105, 18, 6, 4.4f, 2.5f, 1.0f, "inspire", V(WeaponStyle.Mace, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, new Color(0.75f, 0.60f, 0.45f)), ""));
        f.unitTypes.Add(U("fat_sudanese", "fatimid", "Sudanese Infantry", UnitCategory.HeavyInfantry, 95, 12, 4, 3.5f, 2.5f, 1.1f, "war_cry", V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Wrapped, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel, false, false, new Color(0.45f, 0.30f, 0.20f)), ""));
        f.unitTypes.Add(U("fat_daylamite", "fatimid", "Daylamite Infantry", UnitCategory.HeavyInfantry, 110, 14, 5, 3.3f, 2.5f, 1.0f, "shield_wall", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, new Color(0.75f, 0.60f, 0.45f)), ""));
        f.unitTypes.Add(U("fat_marines", "fatimid", "Fatimid Marines", UnitCategory.Naval, 75, 12, 2, 4.2f, 2.5f, 0.9f, "naval_boarding", V(WeaponStyle.Sword, ArmorStyle.Light, HelmetStyle.Turban, ShieldStyle.Buckler, MaterialPreset.Leather, MaterialPreset.Steel, false, false, new Color(0.70f, 0.55f, 0.40f)), ""));
        return f;
    }

    private static FactionDefinition CreateAbbasid()
    {
        var f = new FactionDefinition
        {
            id = "abbasid",
            displayName = "Abbasid Caliphate",
            region = Region.MiddleEast,
            capitalCityId = "abb_baghdad",
            primaryColor = new Color(0.10f, 0.10f, 0.10f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 28000,
            rulerName = "al-Qadir",
            rulerBonus = "+20% legitimacy in controlled provinces",
            factionTrait = "Caliphal Legitimacy",
            factionTraitDescription = "Allied factions have +10% morale",
            strategicAsset = "House of Wisdom",
            strategicAssetDescription = "+25% research speed"
        };
        f.terrainDistribution[TerrainType.Desert] = 28;
        f.terrainDistribution[TerrainType.RiverValley] = 26;
        f.terrainDistribution[TerrainType.Plains] = 14;
        f.terrainDistribution[TerrainType.Hills] = 10;
        f.terrainDistribution[TerrainType.Mountains] = 8;
        f.terrainDistribution[TerrainType.Wetlands] = 8;
        f.terrainDistribution[TerrainType.Coast] = 6;
        f.cities.Add(C("abb_baghdad", "Baghdad", 9000, 0.568f, 0.590f, TerrainType.RiverValley, TerrainType.Desert, true));
        f.cities.Add(C("abb_basra", "Basra", 4000, 0.572f, 0.580f, TerrainType.Wetlands, TerrainType.Desert, false));
        f.cities.Add(C("abb_wasit", "Wasit", 3500, 0.570f, 0.585f, TerrainType.RiverValley, TerrainType.Plains, false));
        f.unitTypes.Add(U("abb_guard_cav", "abbasid", "Guard Cavalry", UnitCategory.HeavyCavalry, 100, 16, 5, 4.4f, 2.5f, 1.0f, "inspire", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, new Color(0.75f, 0.60f, 0.45f)), ""));
        f.unitTypes.Add(U("abb_ghilman", "abbasid", "Ghilman", UnitCategory.HeavyInfantry, 105, 14, 5, 3.5f, 2.5f, 1.0f, "parry", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, new Color(0.72f, 0.58f, 0.42f)), ""));
        f.unitTypes.Add(U("abb_infantry", "abbasid", "City Infantry", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 2.5f, 1.2f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, new Color(0.75f, 0.60f, 0.45f)), ""));
        f.unitTypes.Add(U("abb_garrison", "abbasid", "City Garrison", UnitCategory.HeavyInfantry, 110, 9, 6, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Tower, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, new Color(0.75f, 0.60f, 0.45f)), ""));
        f.unitTypes.Add(U("abb_auxiliary", "abbasid", "Tribal Auxiliary", UnitCategory.LightCavalry, 60, 11, 1, 5.0f, 5.0f, 1.2f, "skirmish", V(WeaponStyle.Javelin, ArmorStyle.Light, HelmetStyle.Wrapped, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Steel, false, false, new Color(0.70f, 0.55f, 0.40f)), ""));
        return f;
    }

    private static FactionDefinition CreateBuyid()
    {
        var f = new FactionDefinition
        {
            id = "buyid",
            displayName = "Buyid Emirates",
            region = Region.MiddleEast,
            capitalCityId = "buy_shiraz",
            primaryColor = new Color(0.50f, 0.20f, 0.55f),
            secondaryColor = new Color(0.70f, 0.35f, 0.70f),
            estimatedMilitary = 56000,
            rulerName = "Sultan al-Dawla",
            rulerBonus = "+15% mountain defense",
            factionTrait = "Daylamite Warriors",
            factionTraitDescription = "Infantry units gain +10% HP",
            strategicAsset = "Shiraz Palace",
            strategicAssetDescription = "+20% income from core provinces"
        };
        f.terrainDistribution[TerrainType.Mountains] = 24;
        f.terrainDistribution[TerrainType.Hills] = 22;
        f.terrainDistribution[TerrainType.Desert] = 18;
        f.terrainDistribution[TerrainType.Plains] = 14;
        f.terrainDistribution[TerrainType.RiverValley] = 10;
        f.terrainDistribution[TerrainType.Coast] = 6;
        f.terrainDistribution[TerrainType.Forest] = 6;
        f.cities.Add(C("buy_shiraz", "Shiraz", 9000, 0.580f, 0.565f, TerrainType.Plains, TerrainType.Hills, true));
        f.cities.Add(C("buy_rayy", "Rayy", 7000, 0.575f, 0.595f, TerrainType.Plains, TerrainType.Hills, false));
        f.cities.Add(C("buy_isfahan", "Isfahan", 7000, 0.576f, 0.580f, TerrainType.Plains, TerrainType.Hills, false));
        f.cities.Add(C("buy_baghdad", "Baghdad", 6000, 0.568f, 0.590f, TerrainType.RiverValley, TerrainType.Desert, false));
        var skin = new Color(0.75f, 0.60f, 0.45f);
        f.unitTypes.Add(U("buy_daylamite", "buyid", "Daylamite Infantry", UnitCategory.HeavyInfantry, 115, 14, 6, 3.4f, 2.5f, 1.0f, "shield_wall", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("buy_turkish_cav", "buyid", "Turkish Cavalry", UnitCategory.HeavyCavalry, 100, 17, 5, 4.5f, 2.5f, 0.9f, null, V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("buy_guards", "buyid", "Buyid Guards", UnitCategory.HeavyInfantry, 105, 13, 5, 3.5f, 2.5f, 1.0f, "parry", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, skin), ""));
        f.unitTypes.Add(U("buy_archers", "buyid", "Persian Archers", UnitCategory.Ranged, 50, 11, 0, 4.0f, 13.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Turban, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("buy_provincial", "buyid", "Provincial Troops", UnitCategory.LightInfantry, 75, 11, 2, 3.8f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreateGhaznavid()
    {
        var f = new FactionDefinition
        {
            id = "ghaznavid",
            displayName = "Ghaznavid Empire",
            region = Region.MiddleEast,
            capitalCityId = "ghz_ghazni",
            primaryColor = new Color(0.65f, 0.15f, 0.15f),
            secondaryColor = new Color(0.85f, 0.70f, 0.20f),
            estimatedMilitary = 88000,
            rulerName = "Mahmud of Ghazni",
            rulerBonus = "Raid income bonus: +30% plunder from conquered provinces",
            factionTrait = "Elephant Terror",
            factionTraitDescription = "Elephant units cause 20% enemy rout chance",
            strategicAsset = "Ghazni Palace Library",
            strategicAssetDescription = "+25% plunder from enemy provinces"
        };
        f.terrainDistribution[TerrainType.Mountains] = 26;
        f.terrainDistribution[TerrainType.Hills] = 20;
        f.terrainDistribution[TerrainType.Steppe] = 18;
        f.terrainDistribution[TerrainType.Desert] = 12;
        f.terrainDistribution[TerrainType.Plains] = 10;
        f.terrainDistribution[TerrainType.RiverValley] = 8;
        f.terrainDistribution[TerrainType.Forest] = 6;
        f.cities.Add(C("ghz_ghazni", "Ghazni", 14000, 0.610f, 0.575f, TerrainType.Mountains, TerrainType.Hills, true));
        f.cities.Add(C("ghz_lahore", "Lahore", 9000, 0.625f, 0.555f, TerrainType.Plains, TerrainType.RiverValley, false));
        f.cities.Add(C("ghz_balkh", "Balkh", 7000, 0.608f, 0.590f, TerrainType.Steppe, TerrainType.Desert, false));
        f.cities.Add(C("ghz_herat", "Herat", 7000, 0.600f, 0.585f, TerrainType.Plains, TerrainType.Desert, false));
        var skin = new Color(0.70f, 0.55f, 0.40f);
        var elephantVis = V(WeaponStyle.Elephant, ArmorStyle.Heavy, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Bronze, false, false, skin);
        elephantVis.bodyScale = 2.5f;
        elephantVis.shoulderWidth = 0.50f;
        elephantVis.hipWidth = 0.30f;
        f.unitTypes.Add(U("ghz_ghulam", "ghaznavid", "Ghulam Cavalry", UnitCategory.HeavyCavalry, 110, 19, 6, 4.5f, 2.5f, 0.9f, "inspire", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, skin), ""));
        f.unitTypes.Add(U("ghz_horsearcher", "ghaznavid", "Horse Archer", UnitCategory.LightCavalry, 60, 11, 1, 5.3f, 8.0f, 1.3f, "horse_archer_kite", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Turban, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("ghz_infantry", "ghaznavid", "Afghan Infantry", UnitCategory.HeavyInfantry, 95, 11, 4, 3.4f, 2.5f, 1.1f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("ghz_elephant", "ghaznavid", "War Elephant", UnitCategory.Elephant, 250, 25, 8, 2.2f, 3.5f, 1.8f, "elephant_charge", elephantVis, ""));
        f.unitTypes.Add(U("ghz_siege", "ghaznavid", "Siege Corps", UnitCategory.Siege, 160, 28, 2, 1.8f, 15.0f, 3.2f, null, V(WeaponStyle.None, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreateKaraKhanid()
    {
        var f = new FactionDefinition
        {
            id = "karakhanid",
            displayName = "Kara-Khanid Khanate",
            region = Region.MiddleEast,
            capitalCityId = "kk_balasaghun",
            primaryColor = new Color(0.20f, 0.60f, 0.65f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 74000,
            rulerName = "Yusuf Qadr Khan",
            rulerBonus = "",
            factionTrait = "Steppe Mastery",
            factionTraitDescription = "Cavalry +20% speed on Steppe",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.Steppe] = 40;
        f.terrainDistribution[TerrainType.Desert] = 18;
        f.terrainDistribution[TerrainType.Mountains] = 16;
        f.terrainDistribution[TerrainType.Hills] = 10;
        f.terrainDistribution[TerrainType.RiverValley] = 8;
        f.terrainDistribution[TerrainType.Plains] = 8;
        f.cities.Add(C("kk_balasaghun", "Balasaghun", 10000, 0.630f, 0.615f, TerrainType.Steppe, TerrainType.Mountains, true));
        f.cities.Add(C("kk_kashgar", "Kashgar", 9000, 0.640f, 0.600f, TerrainType.Desert, TerrainType.Mountains, false));
        f.cities.Add(C("kk_samarkand", "Samarkand", 11000, 0.618f, 0.600f, TerrainType.Steppe, TerrainType.Desert, false));
        var skin = new Color(0.75f, 0.60f, 0.45f);
        var visFur = V(WeaponStyle.Sword, ArmorStyle.Fur, HelmetStyle.Turban, ShieldStyle.None, MaterialPreset.Fur, MaterialPreset.Steel, false, false, skin);
        f.unitTypes.Add(U("kk_horsearcher", "karakhanid", "Turkic Horse Archer", UnitCategory.LightCavalry, 60, 12, 1, 5.4f, 8.0f, 1.2f, "horse_archer_kite", visFur, ""));
        f.unitTypes.Add(U("kk_heavy_cav", "karakhanid", "Heavy Cavalry", UnitCategory.HeavyCavalry, 110, 18, 6, 4.4f, 2.5f, 1.0f, null, visFur, ""));
        f.unitTypes.Add(U("kk_lancer", "karakhanid", "Tribal Lancer", UnitCategory.HeavyCavalry, 100, 16, 5, 4.6f, 3.0f, 1.0f, null, visFur, ""));
        f.unitTypes.Add(U("kk_guards", "karakhanid", "Khanate Guards", UnitCategory.HeavyInfantry, 105, 13, 5, 3.5f, 2.5f, 1.1f, "parry", visFur, ""));
        f.unitTypes.Add(U("kk_garrison", "karakhanid", "Town Garrison", UnitCategory.HeavyInfantry, 95, 9, 5, 3.0f, 2.5f, 1.3f, "fortify", visFur, ""));
        return f;
    }

    private static FactionDefinition CreateKhwarazm()
    {
        var f = new FactionDefinition
        {
            id = "khwarazm",
            displayName = "Khwarazm",
            region = Region.MiddleEast,
            capitalCityId = "khw_gurganj",
            primaryColor = new Color(0.55f, 0.40f, 0.20f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 19000,
            rulerName = "Ma'mun I",
            rulerBonus = "",
            factionTrait = "Oasis Fortifications",
            factionTraitDescription = "+20% defense in Desert terrain",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.Desert] = 34;
        f.terrainDistribution[TerrainType.Steppe] = 24;
        f.terrainDistribution[TerrainType.RiverValley] = 14;
        f.terrainDistribution[TerrainType.Plains] = 8;
        f.terrainDistribution[TerrainType.Hills] = 8;
        f.terrainDistribution[TerrainType.Coast] = 6;
        f.terrainDistribution[TerrainType.Wetlands] = 6;
        f.cities.Add(C("khw_gurganj", "Gurganj", 7000, 0.600f, 0.610f, TerrainType.Desert, TerrainType.RiverValley, true));
        f.cities.Add(C("khw_kath", "Kath", 3000, 0.598f, 0.612f, TerrainType.Desert, TerrainType.RiverValley, false));
        f.cities.Add(C("khw_khiva", "Khiva", 2500, 0.599f, 0.608f, TerrainType.Desert, TerrainType.RiverValley, false));
        var skin = new Color(0.75f, 0.60f, 0.45f);
        f.unitTypes.Add(U("khw_cavalry", "khwarazm", "Khwarazmian Cavalry", UnitCategory.HeavyCavalry, 95, 15, 4, 4.5f, 2.5f, 1.0f, null, V(WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("khw_infantry", "khwarazm", "Khwarazmian Infantry", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 2.5f, 1.2f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("khw_archers", "khwarazm", "Khwarazmian Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null, V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Turban, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("khw_guards", "khwarazm", "Palace Guards", UnitCategory.HeavyInfantry, 105, 13, 5, 3.4f, 2.5f, 1.1f, "parry", V(WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("khw_fortress", "khwarazm", "Fortress Troops", UnitCategory.HeavyInfantry, 115, 9, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Tower, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreateGeorgia()
    {
        var f = new FactionDefinition
        {
            id = "georgia",
            displayName = "Georgia",
            region = Region.MiddleEast,
            capitalCityId = "geo_kutaisi",
            primaryColor = new Color(0.60f, 0.15f, 0.30f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 29000,
            rulerName = "Bagrat III",
            rulerBonus = "",
            factionTrait = "Mountain Fortress",
            factionTraitDescription = "+25% defense in Mountains",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.Mountains] = 40;
        f.terrainDistribution[TerrainType.Hills] = 22;
        f.terrainDistribution[TerrainType.Forest] = 14;
        f.terrainDistribution[TerrainType.Plains] = 8;
        f.terrainDistribution[TerrainType.RiverValley] = 8;
        f.terrainDistribution[TerrainType.Coast] = 8;
        f.cities.Add(C("geo_kutaisi", "Kutaisi", 5500, 0.565f, 0.640f, TerrainType.Mountains, TerrainType.Forest, true));
        f.cities.Add(C("geo_tbilisi", "Tbilisi", 5000, 0.568f, 0.637f, TerrainType.Hills, TerrainType.Mountains, false));
        f.cities.Add(C("geo_mtskheta", "Mtskheta", 3500, 0.567f, 0.638f, TerrainType.Hills, TerrainType.Mountains, false));
        var skin = new Color(0.80f, 0.65f, 0.50f);
        f.unitTypes.Add(U("geo_noble_cav", "georgia", "Noble Cavalry", UnitCategory.HeavyCavalry, 105, 17, 6, 4.3f, 2.5f, 1.0f, "inspire", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("geo_infantry", "georgia", "Georgian Infantry", UnitCategory.HeavyInfantry, 100, 11, 5, 3.4f, 2.5f, 1.1f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("geo_archers", "georgia", "Georgian Archers", UnitCategory.Ranged, 50, 10, 0, 3.9f, 12.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("geo_mountain", "georgia", "Mountain Troops", UnitCategory.LightInfantry, 75, 14, 3, 4.2f, 2.5f, 0.8f, "ambush", V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("geo_fortress", "georgia", "Fortress Garrison", UnitCategory.HeavyInfantry, 115, 9, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Tower, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        return f;
    }

    private static FactionDefinition CreateArmenia()
    {
        var f = new FactionDefinition
        {
            id = "armenia",
            displayName = "Armenian Kingdoms",
            region = Region.MiddleEast,
            capitalCityId = "arm_ani",
            primaryColor = new Color(0.85f, 0.45f, 0.15f),
            secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 23000,
            rulerName = "Gagik I",
            rulerBonus = "",
            factionTrait = "Highland Resilience",
            factionTraitDescription = "Units take 15% less attrition in Mountains",
            strategicAsset = "",
            strategicAssetDescription = ""
        };
        f.terrainDistribution[TerrainType.Mountains] = 38;
        f.terrainDistribution[TerrainType.Hills] = 26;
        f.terrainDistribution[TerrainType.Plains] = 10;
        f.terrainDistribution[TerrainType.Forest] = 8;
        f.terrainDistribution[TerrainType.RiverValley] = 8;
        f.terrainDistribution[TerrainType.Desert] = 10;
        f.cities.Add(C("arm_ani", "Ani", 5000, 0.565f, 0.635f, TerrainType.Mountains, TerrainType.Hills, true));
        f.cities.Add(C("arm_van", "Van", 4500, 0.567f, 0.630f, TerrainType.Hills, TerrainType.Mountains, false));
        f.cities.Add(C("arm_dvin", "Dvin", 3500, 0.566f, 0.633f, TerrainType.Plains, TerrainType.RiverValley, false));
        var skin = new Color(0.80f, 0.65f, 0.50f);
        f.unitTypes.Add(U("arm_heavy_cav", "armenia", "Armenian Heavy Cavalry", UnitCategory.HeavyCavalry, 105, 17, 6, 4.2f, 2.5f, 1.0f, null, V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("arm_infantry", "armenia", "Armenian Infantry", UnitCategory.HeavyInfantry, 95, 10, 5, 3.3f, 2.5f, 1.2f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("arm_archers", "armenia", "Armenian Archers", UnitCategory.Ranged, 50, 11, 0, 3.9f, 12.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, skin), ""));
        f.unitTypes.Add(U("arm_hill", "armenia", "Hill Troops", UnitCategory.LightInfantry, 75, 14, 3, 4.1f, 2.5f, 0.8f, "ambush", V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel, false, false, skin), ""));
        f.unitTypes.Add(U("arm_fortress", "armenia", "Fortress Garrison", UnitCategory.HeavyInfantry, 115, 9, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Tower, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, skin), ""));
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
