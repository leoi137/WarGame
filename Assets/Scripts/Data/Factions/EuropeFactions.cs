using System.Collections.Generic;
using UnityEngine;

public static class EuropeFactions
{
    public static List<FactionDefinition> Create()
    {
        return new List<FactionDefinition>
        {
            NorthSeaEmpire(),
            Norway(),
            Sweden(),
            KievanRus(),
            Poland(),
            Hungary(),
            HolyRomanEmpire(),
            France(),
            Byzantine(),
            ChristianIberia(),
            Cordoba()
        };
    }

    private static CityDefinition C(string id, string name, int garrison, float x, float y, TerrainType primary, TerrainType secondary, bool capital) => new()
    {
        id = id, displayName = name, garrison = garrison, normalizedPosition = new Vector2(x, y),
        primaryTerrain = primary, secondaryTerrain = secondary, isCapital = capital
    };

    private static UnitVisualConfig V(WeaponStyle w, ArmorStyle a, HelmetStyle h, ShieldStyle sh, MaterialPreset am, MaterialPreset wm, bool cape, bool back, float sr, float sg, float sb, float body, float shoulder, float hip) => new()
    {
        primaryWeapon = w, armorStyle = a, helmetStyle = h, shieldStyle = sh, armorMaterial = am, weaponMaterial = wm,
        hasCape = cape, hasBackItem = back, skinTint = new Color(sr, sg, sb), bodyScale = body, shoulderWidth = shoulder, hipWidth = hip
    };

    private static UnitTypeDefinition U(string id, string fid, string name, UnitCategory cat, float hp, float atk, float armor, float spd, float range, float cd, string ability, UnitVisualConfig vis, string desc) => new()
    {
        id = id, factionId = fid, displayName = name, category = cat, maxHP = hp, attackDamage = atk, armor = armor,
        moveSpeed = spd, attackRange = range, attackCooldown = cd, abilityId = ability, visualConfig = vis, description = desc
    };

    private static FactionDefinition NorthSeaEmpire()
    {
        var fid = "north_sea_empire";
        var f = new FactionDefinition
        {
            id = fid, displayName = "North Sea Empire", region = Region.Europe, capitalCityId = "nse_london",
            primaryColor = new Color(0.15f, 0.35f, 0.70f), secondaryColor = new Color(0.20f, 0.45f, 0.85f),
            estimatedMilitary = 48000, rulerName = "Cnut the Great", rulerBonus = "+20% attack on coastal terrain battles",
            factionTrait = "Viking Longships", factionTraitDescription = "Coastal terrain movement penalty removed for all units",
            strategicAsset = "Roskilde Fleet", strategicAssetDescription = "+15% unit effectiveness on Coast terrain"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.28f; f.terrainDistribution[TerrainType.Forest] = 0.18f;
        f.terrainDistribution[TerrainType.Hills] = 0.12f; f.terrainDistribution[TerrainType.Coast] = 0.22f;
        f.terrainDistribution[TerrainType.RiverValley] = 0.10f; f.terrainDistribution[TerrainType.Wetlands] = 0.10f;
        f.cities.AddRange(new[] { C("nse_london", "London", 7000, 0.488f, 0.670f, TerrainType.Plains, TerrainType.RiverValley, true),
            C("nse_winchester", "Winchester", 4000, 0.486f, 0.665f, TerrainType.Plains, TerrainType.Hills, false),
            C("nse_york", "York", 5500, 0.487f, 0.690f, TerrainType.Hills, TerrainType.Plains, false),
            C("nse_roskilde", "Roskilde", 4500, 0.510f, 0.710f, TerrainType.Coast, TerrainType.Plains, false),
            C("nse_trondheim", "Trondheim", 3500, 0.510f, 0.760f, TerrainType.Coast, TerrainType.Mountains, false) });
        f.unitTypes.AddRange(new[] {
            U("nse_huscarl", fid, "Huscarl", UnitCategory.HeavyInfantry, 110, 14, 4, 3.8f, 2.5f, 0.9f, "parry", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), "Elite Norse household warrior armed with sword and round shield"),
            U("nse_hunter", fid, "Norse Hunter", UnitCategory.Ranged, 55, 11, 0, 4.2f, 14.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Hood, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, true, true, 0.85f, 0.70f, 0.55f, 0.95f, 0.26f, 0.12f), "Swift ranged scout with longbow and marking arrows"),
            U("nse_berserker", fid, "Berserker", UnitCategory.LightInfantry, 85, 22, 2, 4.0f, 2.8f, 0.7f, "berserker_rage", V(WeaponStyle.DualAxe, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Fur, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.15f, 0.33f, 0.14f), "Frenzied dual-axe fighter who rages at low health"),
            U("nse_shieldbearer", fid, "Shieldbearer", UnitCategory.HeavyInfantry, 140, 8, 8, 3.0f, 3.0f, 1.6f, "shield_wall", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Spectacle, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.1f, 0.30f, 0.14f), "Heavily armored spear-and-shield wall specialist"),
            U("nse_shipcrew", fid, "Ship Crew", UnitCategory.Naval, 70, 12, 1, 4.5f, 2.2f, 0.8f, "naval_boarding", V(WeaponStyle.Axe, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), "Versatile axe-wielding marine from longship crews") });
        return f;
    }

    private static FactionDefinition Norway()
    {
        var fid = "norway";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Kingdom of Norway", region = Region.Europe, capitalCityId = "nor_trondheim",
            primaryColor = new Color(0.20f, 0.30f, 0.60f), secondaryColor = new Color(0.30f, 0.40f, 0.75f),
            estimatedMilitary = 22000, rulerName = "Olaf II Haraldsson", rulerBonus = "+10% morale (religious zeal)",
            factionTrait = "Leidang Fleet Levy", factionTraitDescription = "Faster unit mobilization from coastal cities",
            strategicAsset = "Nidaros Cathedral", strategicAssetDescription = "+15% levy speed from all cities"
        };
        f.terrainDistribution[TerrainType.Mountains] = 0.35f; f.terrainDistribution[TerrainType.Forest] = 0.20f;
        f.terrainDistribution[TerrainType.Coast] = 0.30f; f.terrainDistribution[TerrainType.Hills] = 0.10f;
        f.terrainDistribution[TerrainType.Wetlands] = 0.05f;
        f.cities.AddRange(new[] { C("nor_trondheim", "Trondheim", 4000, 0.510f, 0.760f, TerrainType.Coast, TerrainType.Mountains, true),
            C("nor_bergen", "Bergen", 3500, 0.505f, 0.740f, TerrainType.Coast, TerrainType.Mountains, false),
            C("nor_oslo", "Oslo", 3000, 0.510f, 0.730f, TerrainType.Forest, TerrainType.Coast, false) });
        f.unitTypes.AddRange(new[] {
            U("nor_leidang", fid, "Leidang Levy", UnitCategory.LightInfantry, 75, 13, 2, 4.0f, 2.5f, 0.8f, "war_cry", V(WeaponStyle.Axe, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("nor_spearman", fid, "Norse Spearman", UnitCategory.HeavyInfantry, 100, 10, 5, 3.5f, 3.0f, 1.2f, "pike_brace", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Nasal, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("nor_axeman", fid, "Norse Axeman", UnitCategory.LightInfantry, 80, 18, 2, 3.8f, 2.5f, 0.7f, "dual_strike", V(WeaponStyle.DualAxe, ArmorStyle.None, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Fur, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.1f, 0.30f, 0.13f), ""),
            U("nor_archer", fid, "Norse Archer", UnitCategory.Ranged, 50, 10, 0, 4.0f, 13.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Hood, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, true, true, 0.85f, 0.70f, 0.55f, 0.95f, 0.26f, 0.12f), ""),
            U("nor_retainer", fid, "Noble Retainer", UnitCategory.HeavyCavalry, 95, 16, 5, 4.2f, 2.5f, 1.0f, "inspire", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.05f, 0.28f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition Sweden()
    {
        var fid = "sweden";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Kingdom of Sweden", region = Region.Europe, capitalCityId = "swe_sigtuna",
            primaryColor = new Color(0.65f, 0.60f, 0.15f), secondaryColor = new Color(0.20f, 0.35f, 0.70f),
            estimatedMilitary = 24000, rulerName = "Olof Skötkonung", rulerBonus = "+15% trade income",
            factionTrait = "Varangian Routes", factionTraitDescription = "Units gain experience faster via eastern trade routes",
            strategicAsset = "Uppsala Temple", strategicAssetDescription = "+10% recruitment speed"
        };
        f.terrainDistribution[TerrainType.Forest] = 0.35f; f.terrainDistribution[TerrainType.Plains] = 0.18f;
        f.terrainDistribution[TerrainType.Coast] = 0.20f; f.terrainDistribution[TerrainType.Hills] = 0.12f;
        f.terrainDistribution[TerrainType.Wetlands] = 0.10f; f.terrainDistribution[TerrainType.Mountains] = 0.05f;
        f.cities.AddRange(new[] { C("swe_sigtuna", "Sigtuna", 4000, 0.520f, 0.735f, TerrainType.Forest, TerrainType.Coast, true),
            C("swe_uppsala", "Uppsala", 3500, 0.520f, 0.738f, TerrainType.Forest, TerrainType.Plains, false),
            C("swe_birka", "Birka", 3000, 0.518f, 0.733f, TerrainType.Coast, TerrainType.Forest, false) });
        f.unitTypes.AddRange(new[] {
            U("swe_retainer", fid, "Swedish Retainer", UnitCategory.HeavyInfantry, 105, 13, 5, 3.6f, 2.5f, 1.0f, "parry", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("swe_levy", fid, "Levy Spearman", UnitCategory.HeavyInfantry, 95, 9, 4, 3.3f, 3.0f, 1.3f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("swe_archer", fid, "Swedish Archer", UnitCategory.Ranged, 50, 10, 0, 4.0f, 13.0f, 1.4f, null, V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Hood, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.85f, 0.70f, 0.55f, 0.95f, 0.26f, 0.12f), ""),
            U("swe_axeman", fid, "Swedish Axeman", UnitCategory.LightInfantry, 80, 17, 2, 3.9f, 2.5f, 0.7f, null, V(WeaponStyle.Axe, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.05f, 0.29f, 0.12f), ""),
            U("swe_shipcrew", fid, "Ship Crew", UnitCategory.Naval, 65, 11, 1, 4.3f, 2.2f, 0.9f, "naval_boarding", V(WeaponStyle.Axe, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition KievanRus()
    {
        var fid = "kievan_rus";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Kievan Rus'", region = Region.Europe, capitalCityId = "rus_kyiv",
            primaryColor = new Color(0.70f, 0.55f, 0.15f), secondaryColor = new Color(0.85f, 0.70f, 0.20f),
            estimatedMilitary = 62000, rulerName = "Yaroslav the Wise", rulerBonus = "+20% movement on river terrain",
            factionTrait = "Druzhina Elite", factionTraitDescription = "Noble retinue units have +10% all stats",
            strategicAsset = "Saint Sophia Cathedral", strategicAssetDescription = "+20% defensive bonus in capital battles"
        };
        f.terrainDistribution[TerrainType.Forest] = 0.28f; f.terrainDistribution[TerrainType.Plains] = 0.20f;
        f.terrainDistribution[TerrainType.Steppe] = 0.18f; f.terrainDistribution[TerrainType.RiverValley] = 0.20f;
        f.terrainDistribution[TerrainType.Wetlands] = 0.10f; f.terrainDistribution[TerrainType.Coast] = 0.04f;
        f.cities.AddRange(new[] { C("rus_kyiv", "Kyiv", 10000, 0.545f, 0.680f, TerrainType.RiverValley, TerrainType.Plains, true),
            C("rus_novgorod", "Novgorod", 8000, 0.545f, 0.720f, TerrainType.Forest, TerrainType.Wetlands, false),
            C("rus_smolensk", "Smolensk", 5000, 0.540f, 0.700f, TerrainType.Forest, TerrainType.RiverValley, false),
            C("rus_chernihiv", "Chernihiv", 5000, 0.548f, 0.685f, TerrainType.Plains, TerrainType.Forest, false) });
        f.unitTypes.AddRange(new[] {
            U("rus_druzhina", fid, "Druzhina Cavalry", UnitCategory.HeavyCavalry, 100, 18, 5, 4.5f, 2.5f, 0.9f, "inspire", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.05f, 0.28f, 0.12f), ""),
            U("rus_militia", fid, "Town Militia", UnitCategory.HeavyInfantry, 90, 10, 4, 3.4f, 2.5f, 1.2f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("rus_spearman", fid, "Rus' Spearman", UnitCategory.HeavyInfantry, 100, 11, 5, 3.3f, 3.0f, 1.1f, "pike_brace", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("rus_archer", fid, "Rus' Archer", UnitCategory.Ranged, 50, 10, 0, 3.8f, 12.0f, 1.4f, null, V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.85f, 0.70f, 0.55f, 0.95f, 0.26f, 0.12f), ""),
            U("rus_flotilla", fid, "River Flotilla", UnitCategory.Naval, 75, 12, 2, 4.0f, 2.5f, 0.9f, "naval_boarding", V(WeaponStyle.Axe, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition Poland()
    {
        var fid = "poland";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Kingdom of Poland", region = Region.Europe, capitalCityId = "pol_gniezno",
            primaryColor = new Color(0.80f, 0.20f, 0.20f), secondaryColor = new Color(0.90f, 0.85f, 0.85f),
            estimatedMilitary = 36000, rulerName = "Bolesław I the Brave", rulerBonus = "+15% fortress defense",
            factionTrait = "Piast Fortifications", factionTraitDescription = "Fortified cities take 25% less damage during siege",
            strategicAsset = "Gniezno Cathedral", strategicAssetDescription = "+15% levy recruitment speed"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.28f; f.terrainDistribution[TerrainType.Forest] = 0.27f;
        f.terrainDistribution[TerrainType.Hills] = 0.12f; f.terrainDistribution[TerrainType.RiverValley] = 0.15f;
        f.terrainDistribution[TerrainType.Wetlands] = 0.08f; f.terrainDistribution[TerrainType.Mountains] = 0.10f;
        f.cities.AddRange(new[] { C("pol_gniezno", "Gniezno", 6000, 0.525f, 0.695f, TerrainType.Plains, TerrainType.Forest, true),
            C("pol_poznan", "Poznań", 5000, 0.522f, 0.693f, TerrainType.Plains, TerrainType.RiverValley, false),
            C("pol_krakow", "Kraków", 5500, 0.530f, 0.680f, TerrainType.Hills, TerrainType.Forest, false),
            C("pol_wroclaw", "Wrocław", 4000, 0.525f, 0.685f, TerrainType.Plains, TerrainType.RiverValley, false) });
        f.unitTypes.AddRange(new[] {
            U("pol_druzhyna", fid, "Drużyna Cavalry", UnitCategory.HeavyCavalry, 105, 17, 6, 4.3f, 2.5f, 0.9f, "inspire", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.05f, 0.28f, 0.12f), ""),
            U("pol_spearman", fid, "Polish Spearman", UnitCategory.HeavyInfantry, 100, 10, 5, 3.4f, 3.0f, 1.2f, "pike_brace", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, 0.85f, 0.70f, 0.55f, 1.0f, 0.28f, 0.12f), ""),
            U("pol_archer", fid, "Polish Archer", UnitCategory.Ranged, 50, 10, 0, 3.9f, 12.0f, 1.4f, null, V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.85f, 0.70f, 0.55f, 0.95f, 0.26f, 0.12f), ""),
            U("pol_garrison", fid, "Fort Garrison", UnitCategory.HeavyInfantry, 120, 9, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Tower, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.85f, 0.70f, 0.55f, 1.05f, 0.28f, 0.13f), ""),
            U("pol_cavalry", fid, "Polish Light Cavalry", UnitCategory.LightCavalry, 65, 12, 2, 5.0f, 5.0f, 1.2f, "skirmish", V(WeaponStyle.Javelin, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 0.95f, 0.27f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition Hungary()
    {
        var fid = "hungary";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Kingdom of Hungary", region = Region.Europe, capitalCityId = "hun_esztergom",
            primaryColor = new Color(0.55f, 0.75f, 0.30f), secondaryColor = new Color(0.70f, 0.85f, 0.45f),
            estimatedMilitary = 42000, rulerName = "Stephen I", rulerBonus = "+15% cavalry speed on Steppe terrain",
            factionTrait = "Steppe Heritage", factionTraitDescription = "Light cavalry units cost 20% less to field",
            strategicAsset = "Esztergom Basilica", strategicAssetDescription = "+20% defense in capital province"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.38f; f.terrainDistribution[TerrainType.Steppe] = 0.20f;
        f.terrainDistribution[TerrainType.Forest] = 0.12f; f.terrainDistribution[TerrainType.Hills] = 0.10f;
        f.terrainDistribution[TerrainType.Mountains] = 0.10f; f.terrainDistribution[TerrainType.RiverValley] = 0.10f;
        f.cities.AddRange(new[] { C("hun_esztergom", "Esztergom", 6500, 0.530f, 0.670f, TerrainType.Plains, TerrainType.RiverValley, true),
            C("hun_szekesfehervar", "Székesfehérvár", 5000, 0.530f, 0.668f, TerrainType.Plains, TerrainType.Steppe, false),
            C("hun_pecs", "Pécs", 3500, 0.528f, 0.662f, TerrainType.Hills, TerrainType.Plains, false),
            C("hun_transylvania", "Transylvanian Forts", 7000, 0.540f, 0.665f, TerrainType.Mountains, TerrainType.Forest, false) });
        f.unitTypes.AddRange(new[] {
            U("hun_horsearcher", fid, "Horse Archer", UnitCategory.LightCavalry, 60, 11, 1, 5.2f, 8.0f, 1.3f, "horse_archer_kite", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.82f, 0.68f, 0.52f, 0.95f, 0.27f, 0.12f), ""),
            U("hun_lancer", fid, "Hungarian Lancer", UnitCategory.HeavyCavalry, 110, 19, 6, 4.5f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.82f, 0.68f, 0.52f, 1.05f, 0.29f, 0.12f), ""),
            U("hun_levy", fid, "Levy Infantry", UnitCategory.LightInfantry, 70, 11, 2, 3.8f, 2.5f, 0.9f, null, V(WeaponStyle.Spear, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Leather, MaterialPreset.Wood, false, false, 0.82f, 0.68f, 0.52f, 1.0f, 0.28f, 0.12f), ""),
            U("hun_border", fid, "Border Guard", UnitCategory.HeavyInfantry, 105, 10, 6, 3.2f, 2.5f, 1.3f, "fortify", V(WeaponStyle.Sword, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.82f, 0.68f, 0.52f, 1.0f, 0.28f, 0.12f), ""),
            U("hun_noble", fid, "Noble Retinue", UnitCategory.HeavyCavalry, 100, 16, 5, 4.6f, 2.5f, 0.9f, "inspire", V(WeaponStyle.Mace, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.82f, 0.68f, 0.52f, 1.05f, 0.28f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition HolyRomanEmpire()
    {
        var fid = "hre";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Holy Roman Empire", region = Region.Europe, capitalCityId = "hre_aachen",
            primaryColor = new Color(0.50f, 0.50f, 0.55f), secondaryColor = new Color(0.70f, 0.65f, 0.20f),
            estimatedMilitary = 96000, rulerName = "Henry II", rulerBonus = "Diplomacy bonus with Church factions",
            factionTrait = "Feudal Call", factionTraitDescription = "Vassal provinces grant temporary knight levies each campaign turn",
            strategicAsset = "Aachen Cathedral", strategicAssetDescription = "Core provinces: +25% levy speed"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.22f; f.terrainDistribution[TerrainType.Forest] = 0.28f;
        f.terrainDistribution[TerrainType.Hills] = 0.18f; f.terrainDistribution[TerrainType.Mountains] = 0.12f;
        f.terrainDistribution[TerrainType.RiverValley] = 0.12f; f.terrainDistribution[TerrainType.Coast] = 0.08f;
        f.cities.AddRange(new[] { C("hre_aachen", "Aachen", 7000, 0.507f, 0.680f, TerrainType.Plains, TerrainType.Forest, true),
            C("hre_mainz", "Mainz", 8000, 0.510f, 0.675f, TerrainType.RiverValley, TerrainType.Hills, false),
            C("hre_regensburg", "Regensburg", 7000, 0.515f, 0.670f, TerrainType.Hills, TerrainType.Forest, false),
            C("hre_cologne", "Cologne", 7000, 0.508f, 0.680f, TerrainType.RiverValley, TerrainType.Plains, false),
            C("hre_magdeburg", "Magdeburg", 5000, 0.515f, 0.690f, TerrainType.Plains, TerrainType.Forest, false) });
        f.unitTypes.AddRange(new[] {
            U("hre_knight", fid, "Feudal Knight", UnitCategory.HeavyCavalry, 120, 20, 7, 4.3f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.72f, 0.58f, 1.1f, 0.29f, 0.13f), ""),
            U("hre_menatarms", fid, "Man-at-Arms", UnitCategory.HeavyInfantry, 110, 13, 6, 3.5f, 2.5f, 1.0f, "parry", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.85f, 0.72f, 0.58f, 1.05f, 0.28f, 0.12f), ""),
            U("hre_spearlevy", fid, "Spear Levy", UnitCategory.HeavyInfantry, 90, 9, 4, 3.3f, 3.0f, 1.3f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, 0.85f, 0.72f, 0.58f, 1.0f, 0.28f, 0.12f), ""),
            U("hre_crossbow", fid, "Crossbowman", UnitCategory.Ranged, 55, 14, 1, 3.5f, 12.0f, 1.8f, "volley_fire", V(WeaponStyle.Crossbow, ArmorStyle.Medium, HelmetStyle.Nasal, ShieldStyle.None, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.85f, 0.72f, 0.58f, 1.0f, 0.28f, 0.12f), ""),
            U("hre_siege", fid, "Siege Crew", UnitCategory.Siege, 180, 30, 3, 2.0f, 14.0f, 3.0f, null, V(WeaponStyle.None, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, false, 0.85f, 0.72f, 0.58f, 1.0f, 0.28f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition France()
    {
        var fid = "france";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Kingdom of France", region = Region.Europe, capitalCityId = "fra_paris",
            primaryColor = new Color(0.25f, 0.25f, 0.80f), secondaryColor = new Color(0.85f, 0.80f, 0.20f),
            estimatedMilitary = 58000, rulerName = "Robert II the Pious", rulerBonus = "+15% defense in fortified cities",
            factionTrait = "Castle Network", factionTraitDescription = "Controlled provinces with cities gain +10% defense",
            strategicAsset = "Paris Cathedral", strategicAssetDescription = "+20% income from capital province"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.30f; f.terrainDistribution[TerrainType.Forest] = 0.22f;
        f.terrainDistribution[TerrainType.Hills] = 0.16f; f.terrainDistribution[TerrainType.RiverValley] = 0.14f;
        f.terrainDistribution[TerrainType.Coast] = 0.10f; f.terrainDistribution[TerrainType.Mountains] = 0.08f;
        f.cities.AddRange(new[] { C("fra_paris", "Paris", 8000, 0.498f, 0.670f, TerrainType.Plains, TerrainType.RiverValley, true),
            C("fra_orleans", "Orléans", 5000, 0.497f, 0.668f, TerrainType.Plains, TerrainType.Forest, false),
            C("fra_reims", "Reims", 4500, 0.502f, 0.672f, TerrainType.Plains, TerrainType.Hills, false),
            C("fra_rouen", "Rouen", 5000, 0.495f, 0.672f, TerrainType.Coast, TerrainType.Plains, false) });
        f.unitTypes.AddRange(new[] {
            U("fra_knight", fid, "French Knight", UnitCategory.HeavyCavalry, 115, 19, 6, 4.4f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.72f, 0.58f, 1.08f, 0.29f, 0.13f), ""),
            U("fra_levy", fid, "Infantry Levy", UnitCategory.HeavyInfantry, 85, 9, 3, 3.4f, 2.5f, 1.2f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.None, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, 0.85f, 0.72f, 0.58f, 1.0f, 0.28f, 0.12f), ""),
            U("fra_archer", fid, "French Archer", UnitCategory.Ranged, 50, 10, 0, 4.0f, 13.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Hood, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.85f, 0.72f, 0.58f, 0.95f, 0.26f, 0.12f), ""),
            U("fra_garrison", fid, "Castle Garrison", UnitCategory.HeavyInfantry, 115, 10, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.85f, 0.72f, 0.58f, 1.05f, 0.28f, 0.13f), ""),
            U("fra_mercenary", fid, "Mercenary", UnitCategory.LightInfantry, 75, 16, 2, 4.2f, 2.5f, 0.7f, "skirmish", V(WeaponStyle.Sword, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.Buckler, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.85f, 0.72f, 0.58f, 1.0f, 0.28f, 0.12f), "") });
        return f;
    }

    private static FactionDefinition Byzantine()
    {
        var fid = "byzantine";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Byzantine Empire", region = Region.Europe, capitalCityId = "byz_constantinople",
            primaryColor = new Color(0.55f, 0.15f, 0.60f), secondaryColor = new Color(0.80f, 0.70f, 0.20f),
            estimatedMilitary = 112000, rulerName = "Basil II Bulgar-Slayer", rulerBonus = "Massive anti-Balkan campaign bonus + Varangian loyalty",
            factionTrait = "Theme System", factionTraitDescription = "Controlled provinces auto-recruit +15% extra thematic troops per campaign turn",
            strategicAsset = "Theodosian Walls & Hagia Sophia", strategicAssetDescription = "Constantinople: +40% siege defense; global Orthodox morale boost"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.16f; f.terrainDistribution[TerrainType.Forest] = 0.14f;
        f.terrainDistribution[TerrainType.Hills] = 0.22f; f.terrainDistribution[TerrainType.Mountains] = 0.24f;
        f.terrainDistribution[TerrainType.Coast] = 0.16f; f.terrainDistribution[TerrainType.RiverValley] = 0.08f;
        f.cities.AddRange(new[] { C("byz_constantinople", "Constantinople", 18000, 0.545f, 0.640f, TerrainType.Coast, TerrainType.Plains, true),
            C("byz_thessaloniki", "Thessaloniki", 10000, 0.538f, 0.640f, TerrainType.Coast, TerrainType.Hills, false),
            C("byz_adrianople", "Adrianople", 8000, 0.542f, 0.645f, TerrainType.Plains, TerrainType.Hills, false),
            C("byz_antioch", "Antioch", 9000, 0.555f, 0.625f, TerrainType.Hills, TerrainType.Mountains, false),
            C("byz_nicaea", "Nicaea", 6000, 0.548f, 0.638f, TerrainType.Hills, TerrainType.Coast, false) });
        f.unitTypes.AddRange(new[] {
            U("byz_cataphract", fid, "Cataphract", UnitCategory.HeavyCavalry, 130, 22, 8, 4.0f, 3.0f, 1.1f, null, V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.80f, 0.65f, 0.50f, 1.15f, 0.30f, 0.14f), ""),
            U("byz_varangian", fid, "Varangian Guard", UnitCategory.HeavyInfantry, 125, 20, 6, 3.6f, 2.8f, 0.8f, "berserker_rage", V(WeaponStyle.DualAxe, ArmorStyle.Heavy, HelmetStyle.Spectacle, ShieldStyle.None, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.85f, 0.70f, 0.55f, 1.15f, 0.32f, 0.14f), ""),
            U("byz_thematic", fid, "Thematic Infantry", UnitCategory.HeavyInfantry, 100, 11, 5, 3.3f, 3.0f, 1.2f, "shield_wall", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.80f, 0.65f, 0.50f, 1.0f, 0.28f, 0.12f), ""),
            U("byz_archer", fid, "Byzantine Archer", UnitCategory.Ranged, 55, 12, 1, 4.0f, 13.0f, 1.3f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.80f, 0.65f, 0.50f, 0.95f, 0.26f, 0.12f), ""),
            U("byz_tagmata", fid, "Tagmata Cavalry", UnitCategory.HeavyCavalry, 110, 17, 6, 4.5f, 2.5f, 0.9f, "inspire", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Conical, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.80f, 0.65f, 0.50f, 1.08f, 0.29f, 0.13f), "") });
        return f;
    }

    private static FactionDefinition ChristianIberia()
    {
        var fid = "iberia_north";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Christian Iberia", region = Region.Europe, capitalCityId = "ibe_leon",
            primaryColor = new Color(0.75f, 0.60f, 0.20f), secondaryColor = new Color(0.85f, 0.15f, 0.15f),
            estimatedMilitary = 39000, rulerName = "Alfonso V of León", rulerBonus = "+15% defense in mountain terrain",
            factionTrait = "Reconquista Zeal", factionTraitDescription = "+10% attack when battling Muslim Iberia faction",
            strategicAsset = "Santiago de Compostela", strategicAssetDescription = "+20% morale for all units"
        };
        f.terrainDistribution[TerrainType.Mountains] = 0.28f; f.terrainDistribution[TerrainType.Hills] = 0.22f;
        f.terrainDistribution[TerrainType.Plains] = 0.15f; f.terrainDistribution[TerrainType.Forest] = 0.15f;
        f.terrainDistribution[TerrainType.RiverValley] = 0.10f; f.terrainDistribution[TerrainType.Coast] = 0.10f;
        f.cities.AddRange(new[] { C("ibe_leon", "León", 6000, 0.478f, 0.645f, TerrainType.Mountains, TerrainType.Plains, true),
            C("ibe_pamplona", "Pamplona", 4500, 0.484f, 0.648f, TerrainType.Mountains, TerrainType.Hills, false),
            C("ibe_burgos", "Burgos", 4500, 0.480f, 0.647f, TerrainType.Hills, TerrainType.Plains, false),
            C("ibe_jaca", "Jaca", 5500, 0.484f, 0.650f, TerrainType.Mountains, TerrainType.Forest, false) });
        f.unitTypes.AddRange(new[] {
            U("ibe_knight", fid, "Iberian Knight", UnitCategory.HeavyCavalry, 110, 18, 6, 4.3f, 2.5f, 1.0f, null, V(WeaponStyle.Spear, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.82f, 0.68f, 0.52f, 1.08f, 0.29f, 0.13f), ""),
            U("ibe_jinete", fid, "Jinete", UnitCategory.LightCavalry, 65, 13, 2, 5.3f, 5.0f, 1.1f, "skirmish", V(WeaponStyle.Javelin, ArmorStyle.Light, HelmetStyle.Conical, ShieldStyle.Buckler, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.82f, 0.68f, 0.52f, 0.95f, 0.27f, 0.12f), ""),
            U("ibe_levy", fid, "Infantry Levy", UnitCategory.HeavyInfantry, 90, 9, 4, 3.3f, 2.5f, 1.3f, "formation_discipline", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Conical, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Wood, false, false, 0.82f, 0.68f, 0.52f, 1.0f, 0.28f, 0.12f), ""),
            U("ibe_archer", fid, "Iberian Archer", UnitCategory.Ranged, 50, 10, 0, 3.9f, 12.0f, 1.4f, null, V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.None, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.82f, 0.68f, 0.52f, 0.95f, 0.26f, 0.12f), ""),
            U("ibe_fortress", fid, "Fortress Troops", UnitCategory.HeavyInfantry, 120, 10, 7, 2.8f, 2.5f, 1.4f, "fortify", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Nasal, ShieldStyle.Tower, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.82f, 0.68f, 0.52f, 1.05f, 0.28f, 0.13f), "") });
        return f;
    }

    private static FactionDefinition Cordoba()
    {
        var fid = "cordoba";
        var f = new FactionDefinition
        {
            id = fid, displayName = "Córdoba", region = Region.Europe, capitalCityId = "cor_cordoba",
            primaryColor = new Color(0.15f, 0.55f, 0.30f), secondaryColor = new Color(0.85f, 0.75f, 0.20f),
            estimatedMilitary = 46000, rulerName = "Hisham II (nominal)", rulerBonus = "+15% income from trade cities",
            factionTrait = "Taifa Fragmentation", factionTraitDescription = "Provinces lost have 20% chance to rebel and become neutral",
            strategicAsset = "Great Mosque of Córdoba", strategicAssetDescription = "+25% research and morale in capital"
        };
        f.terrainDistribution[TerrainType.Plains] = 0.22f; f.terrainDistribution[TerrainType.Hills] = 0.20f;
        f.terrainDistribution[TerrainType.Mountains] = 0.15f; f.terrainDistribution[TerrainType.Desert] = 0.10f;
        f.terrainDistribution[TerrainType.RiverValley] = 0.13f; f.terrainDistribution[TerrainType.Coast] = 0.10f;
        f.terrainDistribution[TerrainType.Forest] = 0.10f;
        f.cities.AddRange(new[] { C("cor_cordoba", "Córdoba", 9000, 0.479f, 0.630f, TerrainType.Plains, TerrainType.RiverValley, true),
            C("cor_seville", "Seville", 6000, 0.476f, 0.626f, TerrainType.Plains, TerrainType.Coast, false),
            C("cor_toledo", "Toledo", 6500, 0.480f, 0.636f, TerrainType.Hills, TerrainType.Plains, false),
            C("cor_zaragoza", "Zaragoza", 5000, 0.484f, 0.642f, TerrainType.RiverValley, TerrainType.Hills, false) });
        f.unitTypes.AddRange(new[] {
            U("cor_cavalry", fid, "Andalusian Cavalry", UnitCategory.HeavyCavalry, 105, 17, 5, 4.5f, 2.5f, 0.9f, null, V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.75f, 0.60f, 0.45f, 1.05f, 0.28f, 0.12f), ""),
            U("cor_berber", fid, "Berber Light Cavalry", UnitCategory.LightCavalry, 60, 14, 2, 5.2f, 5.0f, 1.1f, "skirmish", V(WeaponStyle.Javelin, ArmorStyle.Light, HelmetStyle.Turban, ShieldStyle.Buckler, MaterialPreset.Leather, MaterialPreset.Steel, false, false, 0.65f, 0.50f, 0.35f, 0.95f, 0.27f, 0.12f), ""),
            U("cor_spearman", fid, "Moorish Spearman", UnitCategory.HeavyInfantry, 100, 10, 5, 3.3f, 3.0f, 1.2f, "shield_wall", V(WeaponStyle.Spear, ArmorStyle.Medium, HelmetStyle.Turban, ShieldStyle.Round, MaterialPreset.Chainmail, MaterialPreset.Steel, false, false, 0.75f, 0.60f, 0.45f, 1.0f, 0.28f, 0.12f), ""),
            U("cor_archer", fid, "Moorish Archer", UnitCategory.Ranged, 50, 11, 0, 4.0f, 13.0f, 1.4f, "mark", V(WeaponStyle.Bow, ArmorStyle.Light, HelmetStyle.Turban, ShieldStyle.None, MaterialPreset.Leather, MaterialPreset.Wood, false, true, 0.75f, 0.60f, 0.45f, 0.95f, 0.26f, 0.12f), ""),
            U("cor_guard", fid, "Guard Infantry", UnitCategory.HeavyInfantry, 115, 13, 6, 3.4f, 2.5f, 1.0f, "parry", V(WeaponStyle.Sword, ArmorStyle.Heavy, HelmetStyle.Turban, ShieldStyle.Kite, MaterialPreset.Chainmail, MaterialPreset.Steel, true, false, 0.75f, 0.60f, 0.45f, 1.05f, 0.28f, 0.12f), "") });
        return f;
    }
}
