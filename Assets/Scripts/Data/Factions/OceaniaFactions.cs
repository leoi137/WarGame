using System.Collections.Generic;
using UnityEngine;

public static class OceaniaFactions
{
    static readonly Color OceaniaSkin = new(0.55f, 0.38f, 0.25f);

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
        u.visualConfig.skinTint = OceaniaSkin;
        u.visualConfig.hasCape = false;
        return u;
    }

    public static List<FactionDefinition> Create()
    {
        var list = new List<FactionDefinition>();

        var tuiTonga = new FactionDefinition
        {
            id = "tui_tonga",
            displayName = "Tu'i Tonga Empire",
            region = Region.Oceania,
            capitalCityId = "ton_mua",
            primaryColor = new Color(0.10f, 0.45f, 0.65f),
            secondaryColor = new Color(0.20f, 0.55f, 0.75f),
            estimatedMilitary = 12000,
            rulerName = "Tu'i Tonga",
            rulerBonus = "",
            factionTrait = "Double-Hulled Canoes",
            factionTraitDescription = "Ocean crossing without penalty",
            strategicAsset = "Lapaha Royal Tombs",
            strategicAssetDescription = "+25% naval unit spawn",
            cities = new List<CityDefinition>
            {
                C("ton_mua", "Mu'a", 5000, 0.895f, 0.195f, TerrainType.Coast, TerrainType.Jungle, true),
                C("ton_samoa", "Samoa", 3500, 0.910f, 0.200f, TerrainType.Coast, TerrainType.Jungle, false),
                C("ton_fiji", "Fiji", 2500, 0.885f, 0.190f, TerrainType.Coast, TerrainType.Jungle, false)
            },
            unitTypes = new List<UnitTypeDefinition>
            {
                U("tui_tonga", "ton_marine", "Canoe Marine", UnitCategory.Naval, 75, 13, 2, 4.2f, 2.5f, 0.9f, "naval_boarding", WeaponStyle.Spear, ArmorStyle.None, HelmetStyle.Wrapped, ShieldStyle.None, MaterialPreset.Wood, MaterialPreset.Wood),
                U("tui_tonga", "ton_clubman", "Club Warrior", UnitCategory.LightInfantry, 80, 16, 1, 4.0f, 2.0f, 0.7f, "war_cry", WeaponStyle.Club, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Bone, MaterialPreset.Wood),
                U("tui_tonga", "ton_slinger", "Sling Thrower", UnitCategory.Ranged, 45, 9, 0, 3.8f, 11.0f, 1.5f, "sling_barrage", WeaponStyle.Sling, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Bone, MaterialPreset.Wood),
                U("tui_tonga", "ton_shock", "Tattooed Shock Troops", UnitCategory.LightInfantry, 70, 19, 1, 4.3f, 2.2f, 0.6f, "zealot_charge", WeaponStyle.Club, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Bone, MaterialPreset.Wood),
                U("tui_tonga", "ton_guard", "Chiefly Guard", UnitCategory.HeavyInfantry, 100, 11, 4, 3.5f, 2.5f, 1.1f, "inspire", WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.Wrapped, ShieldStyle.Round, MaterialPreset.Bone, MaterialPreset.Wood)
            }
        };
        tuiTonga.terrainDistribution[TerrainType.Coast] = 50;
        tuiTonga.terrainDistribution[TerrainType.Jungle] = 20;
        tuiTonga.terrainDistribution[TerrainType.Mountains] = 15;
        tuiTonga.terrainDistribution[TerrainType.Plains] = 10;
        tuiTonga.terrainDistribution[TerrainType.Hills] = 5;
        list.Add(tuiTonga);

        return list;
    }
}
