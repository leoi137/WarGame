using System.Collections.Generic;
using UnityEngine;

public static class AfricaFactions
{
    static readonly Color AfricaSkin = new(0.45f, 0.30f, 0.20f);

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
        u.visualConfig.skinTint = AfricaSkin;
        u.visualConfig.hasCape = false;
        return u;
    }

    static UnitTypeDefinition U(string factionId, string id, string displayName, UnitCategory cat, float hp, float atk, float armor, float spd, float range, float cd, string abilityId,
        WeaponStyle w)
    {
        return U(factionId, id, displayName, cat, hp, atk, armor, spd, range, cd, abilityId, w, ArmorStyle.Light, HelmetStyle.Wrapped, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood);
    }

    public static List<FactionDefinition> Create()
    {
        var list = new List<FactionDefinition>();

        var ghana = new FactionDefinition
        {
            id = "ghana",
            displayName = "Ghana Empire",
            region = Region.Africa,
            capitalCityId = "gha_koumbi",
            primaryColor = new Color(0.70f, 0.55f, 0.10f),
            secondaryColor = new Color(0.80f, 0.65f, 0.25f),
            estimatedMilitary = 31000,
            rulerName = "Tunka Manin",
            rulerBonus = "",
            factionTrait = "Gold Caravans",
            factionTraitDescription = "+25% income from desert provinces",
            strategicAsset = "Koumbi Saleh Markets",
            strategicAssetDescription = "+40% trade revenue",
            cities = new List<CityDefinition>
            {
                C("gha_koumbi", "Koumbi Saleh", 10000, 0.468f, 0.445f, TerrainType.Desert, TerrainType.Steppe, true),
                C("gha_awdaghost", "Awdaghost", 4500, 0.463f, 0.450f, TerrainType.Desert, TerrainType.Steppe, false),
                C("gha_walata", "Walata", 3500, 0.465f, 0.448f, TerrainType.Desert, TerrainType.Steppe, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("ghana", "gha_cavalry", "Iron Cavalry", UnitCategory.HeavyCavalry, 95, 16, 4, 4.5f, 2.5f, 1.0f, null, WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Wrapped, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel),
                U("ghana", "gha_poison_archer", "Poisoned Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, "poison_arrow", WeaponStyle.Bow, ArmorStyle.None, HelmetStyle.Wrapped, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood),
                U("ghana", "gha_spearman", "Ghanaian Spearman", UnitCategory.HeavyInfantry, 90, 10, 3, 3.4f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Wrapped, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Wood),
                U("ghana", "gha_levy", "Levy Infantry", UnitCategory.LightInfantry, 70, 9, 1, 3.8f, 2.5f, 1.0f, null, WeaponStyle.Club, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Bone, MaterialPreset.Wood),
                U("ghana", "gha_guard", "Royal Guard", UnitCategory.HeavyInfantry, 105, 14, 5, 3.5f, 2.5f, 1.0f, "inspire", WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Wrapped, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel)
            }
        };
        ghana.terrainDistribution[TerrainType.Desert] = 26;
        ghana.terrainDistribution[TerrainType.Steppe] = 18;
        ghana.terrainDistribution[TerrainType.Plains] = 14;
        ghana.terrainDistribution[TerrainType.Forest] = 10;
        ghana.terrainDistribution[TerrainType.RiverValley] = 8;
        ghana.terrainDistribution[TerrainType.Hills] = 6;
        ghana.terrainDistribution[TerrainType.Wetlands] = 18;
        list.Add(ghana);

        var makuria = new FactionDefinition
        {
            id = "makuria",
            displayName = "Makuria",
            region = Region.Africa,
            capitalCityId = "mak_dongola",
            primaryColor = new Color(0.70f, 0.60f, 0.40f),
            secondaryColor = new Color(0.80f, 0.70f, 0.55f),
            estimatedMilitary = 21000,
            rulerName = "Raphael",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("mak_dongola", "Dongola", 8000, 0.545f, 0.465f, TerrainType.Desert, TerrainType.RiverValley, true),
                C("mak_faras", "Faras", 3500, 0.544f, 0.480f, TerrainType.Desert, TerrainType.RiverValley, false),
                C("mak_qasr", "Qasr Ibrim", 3000, 0.545f, 0.485f, TerrainType.Desert, TerrainType.RiverValley, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("makuria", "mak_archers", "Nubian Archers", UnitCategory.Ranged, 55, 12, 0, 4.0f, 14.0f, 1.3f, "poison_arrow", WeaponStyle.Bow),
                U("makuria", "mak_spearman", "Makurian Spearman", UnitCategory.HeavyInfantry, 90, 10, 4, 3.4f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear),
                U("makuria", "mak_cavalry", "Makurian Cavalry", UnitCategory.HeavyCavalry, 90, 14, 3, 4.4f, 2.5f, 1.0f, null, WeaponStyle.Spear),
                U("makuria", "mak_guards", "Royal Guards", UnitCategory.HeavyInfantry, 100, 12, 5, 3.5f, 2.5f, 1.0f, "inspire", WeaponStyle.Sword),
                U("makuria", "mak_river", "River Troops", UnitCategory.Naval, 70, 10, 2, 4.0f, 2.5f, 1.0f, "naval_boarding", WeaponStyle.Spear)
            }
        };
        makuria.terrainDistribution[TerrainType.Desert] = 38;
        makuria.terrainDistribution[TerrainType.RiverValley] = 24;
        makuria.terrainDistribution[TerrainType.Plains] = 10;
        makuria.terrainDistribution[TerrainType.Hills] = 8;
        makuria.terrainDistribution[TerrainType.Mountains] = 6;
        makuria.terrainDistribution[TerrainType.Wetlands] = 14;
        list.Add(makuria);

        var ethiopia = new FactionDefinition
        {
            id = "ethiopia",
            displayName = "Ethiopian Highlands",
            region = Region.Africa,
            capitalCityId = "eth_aksum",
            primaryColor = new Color(0.15f, 0.45f, 0.15f),
            secondaryColor = new Color(0.25f, 0.55f, 0.25f),
            estimatedMilitary = 26000,
            rulerName = "Zagwe Dynasty",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("eth_aksum", "Aksum", 5000, 0.558f, 0.440f, TerrainType.Mountains, TerrainType.Hills, true),
                C("eth_lalibela", "Lalibela", 6000, 0.560f, 0.435f, TerrainType.Mountains, TerrainType.Hills, false),
                C("eth_tigray", "Tigray", 4000, 0.559f, 0.442f, TerrainType.Mountains, TerrainType.Hills, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("ethiopia", "eth_spearman", "Ethiopian Spearman", UnitCategory.HeavyInfantry, 95, 10, 4, 3.4f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear),
                U("ethiopia", "eth_archers", "Ethiopian Archers", UnitCategory.Ranged, 50, 11, 0, 3.9f, 12.0f, 1.4f, "mark", WeaponStyle.Bow),
                U("ethiopia", "eth_retainer", "Noble Retainers", UnitCategory.HeavyCavalry, 95, 15, 4, 4.3f, 2.5f, 1.0f, "inspire", WeaponStyle.Spear),
                U("ethiopia", "eth_hill", "Hill Troops", UnitCategory.LightInfantry, 75, 13, 2, 4.2f, 2.5f, 0.8f, "ambush", WeaponStyle.Spear),
                U("ethiopia", "eth_guards", "Royal Guards", UnitCategory.HeavyInfantry, 105, 13, 5, 3.5f, 2.5f, 1.0f, "parry", WeaponStyle.Sword)
            }
        };
        ethiopia.terrainDistribution[TerrainType.Mountains] = 34;
        ethiopia.terrainDistribution[TerrainType.Hills] = 26;
        ethiopia.terrainDistribution[TerrainType.Forest] = 14;
        ethiopia.terrainDistribution[TerrainType.Plains] = 8;
        ethiopia.terrainDistribution[TerrainType.RiverValley] = 8;
        ethiopia.terrainDistribution[TerrainType.Desert] = 4;
        ethiopia.terrainDistribution[TerrainType.Coast] = 6;
        list.Add(ethiopia);

        var kanem = new FactionDefinition
        {
            id = "kanem",
            displayName = "Kanem",
            region = Region.Africa,
            capitalCityId = "kan_njimi",
            primaryColor = new Color(0.75f, 0.45f, 0.10f),
            secondaryColor = new Color(0.85f, 0.55f, 0.25f),
            estimatedMilitary = 23000,
            rulerName = "Mai Dunama",
            rulerBonus = "",
            factionTrait = "",
            factionTraitDescription = "",
            strategicAsset = "",
            strategicAssetDescription = "",
            cities = new List<CityDefinition>
            {
                C("kan_njimi", "Njimi", 8000, 0.518f, 0.440f, TerrainType.Steppe, TerrainType.Desert, true),
                C("kan_lakechad", "Lake Chad Forts", 4000, 0.520f, 0.438f, TerrainType.Steppe, TerrainType.Desert, false),
                C("kan_caravan", "Caravan Nodes", 3000, 0.515f, 0.445f, TerrainType.Steppe, TerrainType.Desert, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("kanem", "kan_cavalry", "Kanem Cavalry", UnitCategory.HeavyCavalry, 95, 15, 4, 4.5f, 2.5f, 1.0f, null, WeaponStyle.Spear),
                U("kanem", "kan_spearman", "Kanem Spearman", UnitCategory.HeavyInfantry, 90, 10, 3, 3.4f, 3.0f, 1.2f, "formation_discipline", WeaponStyle.Spear),
                U("kanem", "kan_archers", "Kanem Archers", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null, WeaponStyle.Bow),
                U("kanem", "kan_guards", "Royal Guards", UnitCategory.HeavyInfantry, 100, 12, 5, 3.5f, 2.5f, 1.0f, "inspire", WeaponStyle.Sword),
                U("kanem", "kan_tribal", "Tribal Levies", UnitCategory.LightInfantry, 65, 9, 1, 4.0f, 2.5f, 1.0f, "war_cry", WeaponStyle.Club)
            }
        };
        kanem.terrainDistribution[TerrainType.Steppe] = 26;
        kanem.terrainDistribution[TerrainType.Desert] = 22;
        kanem.terrainDistribution[TerrainType.Plains] = 14;
        kanem.terrainDistribution[TerrainType.Forest] = 8;
        kanem.terrainDistribution[TerrainType.Wetlands] = 18;
        kanem.terrainDistribution[TerrainType.RiverValley] = 8;
        kanem.terrainDistribution[TerrainType.Hills] = 4;
        list.Add(kanem);

        return list;
    }
}
