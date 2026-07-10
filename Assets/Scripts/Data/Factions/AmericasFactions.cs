using System.Collections.Generic;
using UnityEngine;

public static class AmericasFactions
{
    static readonly Color AmericasSkin = new(0.70f, 0.50f, 0.30f);

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

    static UnitTypeDefinition U(string factionId, string id, string displayName, UnitCategory cat, float hp, float atk, float armor, float spd, float range, float cd, string abilityId,
        WeaponStyle w, ArmorStyle a, HelmetStyle h, ShieldStyle sh, MaterialPreset am, MaterialPreset wm)
    {
        var u = new UnitTypeDefinition
        {
            id = id,
            factionId = factionId,
            displayName = displayName,
            category = cat,
            maxHP = hp,
            attackDamage = atk,
            armor = armor,
            moveSpeed = spd,
            attackRange = range,
            attackCooldown = cd,
            abilityId = abilityId
        };
        u.visualConfig.primaryWeapon = w;
        u.visualConfig.armorStyle = a;
        u.visualConfig.helmetStyle = h;
        u.visualConfig.shieldStyle = sh;
        u.visualConfig.armorMaterial = am;
        u.visualConfig.weaponMaterial = wm;
        u.visualConfig.skinTint = AmericasSkin;
        u.visualConfig.hasCape = false;
        return u;
    }

    static UnitTypeDefinition U(string factionId, string id, string displayName, UnitCategory cat, float hp, float atk, float armor, float spd, float range, float cd, string abilityId,
        WeaponStyle w)
    {
        return U(factionId, id, displayName, cat, hp, atk, armor, spd, range, cd, abilityId, w, ArmorStyle.Light, HelmetStyle.Feathered, ShieldStyle.None, MaterialPreset.Cotton, MaterialPreset.Wood);
    }

    public static List<FactionDefinition> Create()
    {
        var list = new List<FactionDefinition>();

        var toltec = new FactionDefinition
        {
            id = "toltec",
            displayName = "Toltec Sphere",
            region = Region.Americas,
            capitalCityId = "tol_tula",
            primaryColor = new Color(0.20f, 0.60f, 0.60f),
            secondaryColor = new Color(0.30f, 0.70f, 0.70f),
            estimatedMilitary = 33000,
            rulerName = "Ce Acatl Topiltzin",
            rulerBonus = "",
            factionTrait = "Warrior Orders",
            factionTraitDescription = "Elite units gain +15% damage after a kill",
            strategicAsset = "Atlantean Warrior Statues",
            strategicAssetDescription = "+30% shock infantry strength at Tula",
            cities = new List<CityDefinition>
            {
                C("tol_tula", "Tula", 12000, 0.185f, 0.480f, TerrainType.Hills, TerrainType.Plains, true),
                C("tol_cholula", "Cholula", 5000, 0.186f, 0.475f, TerrainType.Hills, TerrainType.Plains, false),
                C("tol_xochicalco", "Xochicalco", 3500, 0.184f, 0.472f, TerrainType.Hills, TerrainType.Plains, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("toltec", "tol_jaguar", "Jaguar Warrior", UnitCategory.LightInfantry, 80, 20, 3, 4.2f, 2.5f, 0.6f, "zealot_charge", WeaponStyle.Club, ArmorStyle.Light, HelmetStyle.Feathered, ShieldStyle.None, MaterialPreset.Cotton, MaterialPreset.Obsidian),
                U("toltec", "tol_eagle", "Eagle Warrior", UnitCategory.LightInfantry, 75, 15, 2, 4.5f, 5.0f, 1.1f, "atlatl_volley", WeaponStyle.Atlatl, ArmorStyle.Light, HelmetStyle.Feathered, ShieldStyle.None, MaterialPreset.Cotton, MaterialPreset.Obsidian),
                U("toltec", "tol_coyote", "Coyote Scout", UnitCategory.LightInfantry, 60, 12, 1, 5.0f, 2.5f, 0.7f, "skirmish", WeaponStyle.Club, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Cotton, MaterialPreset.Wood),
                U("toltec", "tol_macuahuitl", "Macuahuitl Swordsman", UnitCategory.HeavyInfantry, 95, 14, 4, 3.4f, 2.5f, 1.0f, null, WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Feathered, ShieldStyle.Round, MaterialPreset.Cotton, MaterialPreset.Obsidian),
                U("toltec", "tol_atlatl", "Atlatl Thrower", UnitCategory.Ranged, 50, 11, 0, 3.9f, 10.0f, 1.3f, "atlatl_volley", WeaponStyle.Atlatl, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Cotton, MaterialPreset.Wood)
            }
        };
        toltec.terrainDistribution[TerrainType.Hills] = 22;
        toltec.terrainDistribution[TerrainType.Plains] = 16;
        toltec.terrainDistribution[TerrainType.Mountains] = 16;
        toltec.terrainDistribution[TerrainType.Forest] = 10;
        toltec.terrainDistribution[TerrainType.Jungle] = 8;
        toltec.terrainDistribution[TerrainType.RiverValley] = 8;
        toltec.terrainDistribution[TerrainType.Desert] = 12;
        toltec.terrainDistribution[TerrainType.Coast] = 8;
        list.Add(toltec);

        var maya = new FactionDefinition
        {
            id = "maya",
            displayName = "Maya City-States",
            region = Region.Americas,
            capitalCityId = "may_chichen",
            primaryColor = new Color(0.30f, 0.55f, 0.25f),
            secondaryColor = new Color(0.40f, 0.65f, 0.35f),
            estimatedMilitary = 52000,
            rulerName = "K'inich K'ak'mo",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("may_chichen", "Chichén Itzá", 14000, 0.175f, 0.465f, TerrainType.Jungle, TerrainType.Forest, true),
                C("may_mayapan", "Mayapán", 7000, 0.174f, 0.462f, TerrainType.Jungle, TerrainType.Forest, false),
                C("may_uxmal", "Uxmal", 5000, 0.173f, 0.463f, TerrainType.Jungle, TerrainType.Forest, false),
                C("may_tikal", "Tikal", 8000, 0.180f, 0.455f, TerrainType.Jungle, TerrainType.Forest, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("maya", "may_spearman", "Maya Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 3.0f, 1.2f, "monsoon_tactics", WeaponStyle.Spear),
                U("maya", "may_archers", "Maya Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 11.0f, 1.4f, null, WeaponStyle.Bow),
                U("maya", "may_atlatl", "Atlatl Skirmisher", UnitCategory.Ranged, 55, 12, 0, 4.0f, 8.0f, 1.2f, "atlatl_volley", WeaponStyle.Atlatl),
                U("maya", "may_noble", "Noble Guards", UnitCategory.HeavyInfantry, 105, 14, 5, 3.5f, 2.5f, 1.0f, "zealot_charge", WeaponStyle.Sword),
                U("maya", "may_assault", "Assault Infantry", UnitCategory.LightInfantry, 75, 16, 2, 4.2f, 2.5f, 0.7f, "war_cry", WeaponStyle.Club)
            }
        };
        maya.terrainDistribution[TerrainType.Jungle] = 34;
        maya.terrainDistribution[TerrainType.Forest] = 18;
        maya.terrainDistribution[TerrainType.Wetlands] = 14;
        maya.terrainDistribution[TerrainType.Coast] = 12;
        maya.terrainDistribution[TerrainType.Plains] = 10;
        maya.terrainDistribution[TerrainType.Hills] = 8;
        maya.terrainDistribution[TerrainType.RiverValley] = 4;
        list.Add(maya);

        var oaxaca = new FactionDefinition
        {
            id = "oaxaca",
            displayName = "Oaxaca States",
            region = Region.Americas,
            capitalCityId = "oax_montealban",
            primaryColor = new Color(0.65f, 0.35f, 0.20f),
            secondaryColor = new Color(0.75f, 0.45f, 0.30f),
            estimatedMilitary = 26000,
            rulerName = "Mixtec/Zapotec Council",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("oax_montealban", "Monte Albán", 8000, 0.182f, 0.470f, TerrainType.Mountains, TerrainType.Hills, true),
                C("oax_mitla", "Mitla", 5000, 0.183f, 0.468f, TerrainType.Mountains, TerrainType.Hills, false),
                C("oax_mixtec", "Mixtec Highlands", 4000, 0.181f, 0.472f, TerrainType.Mountains, TerrainType.Hills, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("oaxaca", "oax_spearman", "Zapotec Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear),
                U("oaxaca", "oax_archers", "Mixtec Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 11.0f, 1.4f, null, WeaponStyle.Bow),
                U("oaxaca", "oax_skirmisher", "Skirmisher", UnitCategory.LightInfantry, 65, 12, 1, 4.3f, 5.0f, 1.1f, "skirmish", WeaponStyle.Javelin),
                U("oaxaca", "oax_elite", "Elite Guards", UnitCategory.HeavyInfantry, 100, 14, 5, 3.5f, 2.5f, 1.0f, "zealot_charge", WeaponStyle.Sword),
                U("oaxaca", "oax_hill", "Hill Troops", UnitCategory.LightInfantry, 70, 13, 2, 4.2f, 2.5f, 0.8f, "ambush", WeaponStyle.Spear)
            }
        };
        oaxaca.terrainDistribution[TerrainType.Mountains] = 26;
        oaxaca.terrainDistribution[TerrainType.Hills] = 26;
        oaxaca.terrainDistribution[TerrainType.Forest] = 14;
        oaxaca.terrainDistribution[TerrainType.Plains] = 10;
        oaxaca.terrainDistribution[TerrainType.Jungle] = 8;
        oaxaca.terrainDistribution[TerrainType.RiverValley] = 6;
        oaxaca.terrainDistribution[TerrainType.Coast] = 10;
        list.Add(oaxaca);

        var tiwanaku = new FactionDefinition
        {
            id = "tiwanaku",
            displayName = "Tiwanaku Sphere",
            region = Region.Americas,
            capitalCityId = "tiw_tiwanaku",
            primaryColor = new Color(0.50f, 0.50f, 0.45f),
            secondaryColor = new Color(0.60f, 0.60f, 0.55f),
            estimatedMilitary = 19000,
            rulerName = "Tiwanaku Council",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("tiw_tiwanaku", "Tiwanaku", 7000, 0.240f, 0.255f, TerrainType.Mountains, TerrainType.Hills, true),
                C("tiw_titicaca", "Lake Titicaca", 4000, 0.242f, 0.258f, TerrainType.Mountains, TerrainType.Hills, false),
                C("tiw_road", "Road Garrisons", 2500, 0.238f, 0.250f, TerrainType.Mountains, TerrainType.Hills, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("tiwanaku", "tiw_spearman", "Tiwanaku Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear),
                U("tiwanaku", "tiw_slinger", "Slingers", UnitCategory.Ranged, 45, 9, 0, 3.8f, 11.0f, 1.5f, "sling_barrage", WeaponStyle.Sling),
                U("tiwanaku", "tiw_clubman", "Club Infantry", UnitCategory.LightInfantry, 75, 14, 2, 3.8f, 2.0f, 0.8f, "war_cry", WeaponStyle.Club),
                U("tiwanaku", "tiw_dart", "Dart Troops", UnitCategory.Ranged, 50, 10, 0, 4.0f, 8.0f, 1.2f, null, WeaponStyle.Atlatl),
                U("tiwanaku", "tiw_guards", "Royal Guards", UnitCategory.HeavyInfantry, 100, 12, 5, 3.4f, 2.5f, 1.1f, "inspire", WeaponStyle.Sword)
            }
        };
        tiwanaku.terrainDistribution[TerrainType.Mountains] = 36;
        tiwanaku.terrainDistribution[TerrainType.Hills] = 28;
        tiwanaku.terrainDistribution[TerrainType.Plains] = 8;
        tiwanaku.terrainDistribution[TerrainType.Forest] = 2;
        tiwanaku.terrainDistribution[TerrainType.RiverValley] = 6;
        tiwanaku.terrainDistribution[TerrainType.Wetlands] = 10;
        tiwanaku.terrainDistribution[TerrainType.Desert] = 10;
        list.Add(tiwanaku);

        var wari = new FactionDefinition
        {
            id = "wari",
            displayName = "Wari Successor",
            region = Region.Americas,
            capitalCityId = "war_ayacucho",
            primaryColor = new Color(0.60f, 0.35f, 0.25f),
            secondaryColor = new Color(0.70f, 0.45f, 0.35f),
            estimatedMilitary = 24000,
            rulerName = "Wari Confederation",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("war_ayacucho", "Ayacucho", 8000, 0.235f, 0.265f, TerrainType.Mountains, TerrainType.Hills, true),
                C("war_highland", "Highland Nodes", 5000, 0.237f, 0.270f, TerrainType.Mountains, TerrainType.Hills, false),
                C("war_coastal", "Coastal Valleys", 3500, 0.230f, 0.260f, TerrainType.Mountains, TerrainType.Desert, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("wari", "war_spearman", "Wari Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.3f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear),
                U("wari", "war_slinger", "Wari Slingers", UnitCategory.Ranged, 45, 9, 0, 3.8f, 11.0f, 1.5f, "sling_barrage", WeaponStyle.Sling),
                U("wari", "war_shock", "Shock Infantry", UnitCategory.LightInfantry, 80, 16, 2, 4.0f, 2.5f, 0.7f, "zealot_charge", WeaponStyle.Club),
                U("wari", "war_garrison", "Garrison Troops", UnitCategory.HeavyInfantry, 105, 9, 6, 2.8f, 2.5f, 1.4f, "fortify", WeaponStyle.Spear),
                U("wari", "war_road", "Road-Logistics Troops", UnitCategory.LightInfantry, 70, 10, 2, 4.5f, 2.5f, 1.0f, "skirmish", WeaponStyle.Spear)
            }
        };
        wari.terrainDistribution[TerrainType.Mountains] = 34;
        wari.terrainDistribution[TerrainType.Hills] = 22;
        wari.terrainDistribution[TerrainType.Desert] = 16;
        wari.terrainDistribution[TerrainType.Plains] = 8;
        wari.terrainDistribution[TerrainType.RiverValley] = 8;
        wari.terrainDistribution[TerrainType.Forest] = 2;
        wari.terrainDistribution[TerrainType.Coast] = 10;
        list.Add(wari);

        return list;
    }
}
