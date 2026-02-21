# WorldWars 2.0 — Complete Faction Data Reference

**Every value in this file is implementation-ready. No interpretation required.**

The executing AI must copy these values directly into C# data classes. Every faction has: complete header, all cities with coordinates, all units with full stat blocks + visual configs, terrain distribution, ruler, trait, and asset.

**Legend:**
- **Cat** = UnitCategory (HI=HeavyInfantry, LI=LightInfantry, HC=HeavyCavalry, LC=LightCavalry, R=Ranged, S=Siege, E=Elephant, N=Naval, SP=Special)
- **W** = WeaponStyle, **A** = ArmorStyle, **H** = HelmetStyle, **Sh** = ShieldStyle
- **AM** = ArmorMaterial (MaterialPreset), **WM** = WeaponMaterial
- **Cape** = hasCape, **Back** = hasBackItem
- Stats: HP / ATK / DEF / SPD / Range / CD
- Position format: (x, y) normalized 0-1, x=longitude, y=latitude

---

# EUROPE (11 Factions)

---

## 1. North Sea Empire

| Field | Value |
|-------|-------|
| **ID** | `north_sea_empire` |
| **Display Name** | North Sea Empire |
| **Region** | Europe |
| **Capital** | `nse_london` |
| **Primary Color** | (0.15, 0.35, 0.70) |
| **Secondary Color** | (0.20, 0.45, 0.85) |
| **Military** | 48,000 |
| **Ruler** | Cnut the Great |
| **Ruler Bonus** | +20% attack on coastal terrain battles |
| **Faction Trait** | Viking Longships |
| **Trait Desc** | Coastal terrain movement penalty removed for all units |
| **Strategic Asset** | Roskilde Fleet |
| **Asset Desc** | +15% unit effectiveness on Coast terrain |
| **Terrain** | PL 28 / FO 18 / HI 12 / CO 22 / RV 10 / SW 10 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| nse_london | London | 7,000 | (0.488, 0.670) | Plains | RiverValley | Yes |
| nse_winchester | Winchester | 4,000 | (0.486, 0.665) | Plains | Hills | No |
| nse_york | York | 5,500 | (0.487, 0.690) | Hills | Plains | No |
| nse_roskilde | Roskilde | 4,500 | (0.510, 0.710) | Coast | Plains | No |
| nse_trondheim | Trondheim | 3,500 | (0.510, 0.760) | Coast | Mountains | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| nse_huscarl | Huscarl | HI | 110 | 14 | 4 | 3.8 | 2.5 | 0.9 | parry | Sword | Heavy | Nasal | Round | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| nse_hunter | Norse Hunter | R | 55 | 11 | 0 | 4.2 | 14.0 | 1.4 | mark | Bow | Light | Hood | None | Leather | Wood | Yes | Yes | (0.85,0.70,0.55) | 0.95 | 0.26 | 0.12 |
| nse_berserker | Berserker | LI | 85 | 22 | 2 | 4.0 | 2.8 | 0.7 | berserker_rage | DualAxe | None | None | None | Fur | Steel | No | No | (0.85,0.70,0.55) | 1.15 | 0.33 | 0.14 |
| nse_shieldbearer | Shieldbearer | HI | 140 | 8 | 8 | 3.0 | 3.0 | 1.6 | shield_wall | Spear | Heavy | Spectacle | Round | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.1 | 0.30 | 0.14 |
| nse_shipcrew | Ship Crew | N | 70 | 12 | 1 | 4.5 | 2.2 | 0.8 | naval_boarding | Axe | Light | None | None | Leather | Steel | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |

**Description strings:**
- nse_huscarl: "Elite Norse household warrior armed with sword and round shield"
- nse_hunter: "Swift ranged scout with longbow and marking arrows"
- nse_berserker: "Frenzied dual-axe fighter who rages at low health"
- nse_shieldbearer: "Heavily armored spear-and-shield wall specialist"
- nse_shipcrew: "Versatile axe-wielding marine from longship crews"

---

## 2. Kingdom of Norway

| Field | Value |
|-------|-------|
| **ID** | `norway` |
| **Display Name** | Kingdom of Norway |
| **Region** | Europe |
| **Capital** | `nor_trondheim` |
| **Primary Color** | (0.20, 0.30, 0.60) |
| **Secondary Color** | (0.30, 0.40, 0.75) |
| **Military** | 22,000 |
| **Ruler** | Olaf II Haraldsson |
| **Ruler Bonus** | +10% morale (religious zeal) |
| **Faction Trait** | Leidang Fleet Levy |
| **Trait Desc** | Faster unit mobilization from coastal cities |
| **Strategic Asset** | Nidaros Cathedral |
| **Asset Desc** | +15% levy speed from all cities |
| **Terrain** | MT 35 / FO 20 / CO 30 / HI 10 / SW 5 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| nor_trondheim | Trondheim | 4,000 | (0.510, 0.760) | Coast | Mountains | Yes |
| nor_bergen | Bergen | 3,500 | (0.505, 0.740) | Coast | Mountains | No |
| nor_oslo | Oslo | 3,000 | (0.510, 0.730) | Forest | Coast | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| nor_leidang | Leidang Levy | LI | 75 | 13 | 2 | 4.0 | 2.5 | 0.8 | war_cry | Axe | Light | None | Round | Leather | Steel | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| nor_spearman | Norse Spearman | HI | 100 | 10 | 5 | 3.5 | 3.0 | 1.2 | pike_brace | Spear | Medium | Nasal | Round | Chainmail | Steel | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| nor_axeman | Norse Axeman | LI | 80 | 18 | 2 | 3.8 | 2.5 | 0.7 | dual_strike | DualAxe | None | None | None | Fur | Steel | No | No | (0.85,0.70,0.55) | 1.1 | 0.30 | 0.13 |
| nor_archer | Norse Archer | R | 50 | 10 | 0 | 4.0 | 13.0 | 1.4 | mark | Bow | Light | Hood | None | Leather | Wood | Yes | Yes | (0.85,0.70,0.55) | 0.95 | 0.26 | 0.12 |
| nor_retainer | Noble Retainer | HC | 95 | 16 | 5 | 4.2 | 2.5 | 1.0 | inspire | Spear | Heavy | Nasal | Kite | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.05 | 0.28 | 0.12 |

---

## 3. Kingdom of Sweden

| Field | Value |
|-------|-------|
| **ID** | `sweden` |
| **Display Name** | Kingdom of Sweden |
| **Region** | Europe |
| **Capital** | `swe_sigtuna` |
| **Primary Color** | (0.65, 0.60, 0.15) |
| **Secondary Color** | (0.20, 0.35, 0.70) |
| **Military** | 24,000 |
| **Ruler** | Olof Skötkonung |
| **Ruler Bonus** | +15% trade income |
| **Faction Trait** | Varangian Routes |
| **Trait Desc** | Units gain experience faster via eastern trade routes |
| **Strategic Asset** | Uppsala Temple |
| **Asset Desc** | +10% recruitment speed |
| **Terrain** | FO 35 / PL 18 / CO 20 / HI 12 / SW 10 / MT 5 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| swe_sigtuna | Sigtuna | 4,000 | (0.520, 0.735) | Forest | Coast | Yes |
| swe_uppsala | Uppsala | 3,500 | (0.520, 0.738) | Forest | Plains | No |
| swe_birka | Birka | 3,000 | (0.518, 0.733) | Coast | Forest | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| swe_retainer | Swedish Retainer | HI | 105 | 13 | 5 | 3.6 | 2.5 | 1.0 | parry | Sword | Heavy | Nasal | Kite | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| swe_levy | Levy Spearman | HI | 95 | 9 | 4 | 3.3 | 3.0 | 1.3 | formation_discipline | Spear | Medium | None | Round | Chainmail | Wood | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| swe_archer | Swedish Archer | R | 50 | 10 | 0 | 4.0 | 13.0 | 1.4 | null | Bow | Light | Hood | None | Leather | Wood | No | Yes | (0.85,0.70,0.55) | 0.95 | 0.26 | 0.12 |
| swe_axeman | Swedish Axeman | LI | 80 | 17 | 2 | 3.9 | 2.5 | 0.7 | null | Axe | Light | None | None | Leather | Steel | No | No | (0.85,0.70,0.55) | 1.05 | 0.29 | 0.12 |
| swe_shipcrew | Ship Crew | N | 65 | 11 | 1 | 4.3 | 2.2 | 0.9 | naval_boarding | Axe | Light | None | None | Leather | Steel | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |

---

## 4. Kievan Rus'

| Field | Value |
|-------|-------|
| **ID** | `kievan_rus` |
| **Display Name** | Kievan Rus' |
| **Region** | Europe |
| **Capital** | `rus_kyiv` |
| **Primary Color** | (0.70, 0.55, 0.15) |
| **Secondary Color** | (0.85, 0.70, 0.20) |
| **Military** | 62,000 |
| **Ruler** | Yaroslav the Wise |
| **Ruler Bonus** | +20% movement on river terrain |
| **Faction Trait** | Druzhina Elite |
| **Trait Desc** | Noble retinue units have +10% all stats |
| **Strategic Asset** | Saint Sophia Cathedral |
| **Asset Desc** | +20% defensive bonus in capital battles |
| **Terrain** | FO 28 / PL 20 / ST 18 / RV 20 / SW 10 / CO 4 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| rus_kyiv | Kyiv | 10,000 | (0.545, 0.680) | RiverValley | Plains | Yes |
| rus_novgorod | Novgorod | 8,000 | (0.545, 0.720) | Forest | Wetlands | No |
| rus_smolensk | Smolensk | 5,000 | (0.540, 0.700) | Forest | RiverValley | No |
| rus_chernihiv | Chernihiv | 5,000 | (0.548, 0.685) | Plains | Forest | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| rus_druzhina | Druzhina Cavalry | HC | 100 | 18 | 5 | 4.5 | 2.5 | 0.9 | inspire | Spear | Heavy | Conical | Kite | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.05 | 0.28 | 0.12 |
| rus_militia | Town Militia | HI | 90 | 10 | 4 | 3.4 | 2.5 | 1.2 | formation_discipline | Spear | Medium | None | Round | Chainmail | Wood | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| rus_spearman | Rus' Spearman | HI | 100 | 11 | 5 | 3.3 | 3.0 | 1.1 | pike_brace | Spear | Medium | Conical | Round | Chainmail | Steel | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| rus_archer | Rus' Archer | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null | Bow | Light | None | None | Leather | Wood | No | Yes | (0.85,0.70,0.55) | 0.95 | 0.26 | 0.12 |
| rus_flotilla | River Flotilla | N | 75 | 12 | 2 | 4.0 | 2.5 | 0.9 | naval_boarding | Axe | Light | None | Round | Leather | Steel | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |

---

## 5. Kingdom of Poland

| Field | Value |
|-------|-------|
| **ID** | `poland` |
| **Display Name** | Kingdom of Poland |
| **Region** | Europe |
| **Capital** | `pol_gniezno` |
| **Primary Color** | (0.80, 0.20, 0.20) |
| **Secondary Color** | (0.90, 0.85, 0.85) |
| **Military** | 36,000 |
| **Ruler** | Bolesław I the Brave |
| **Ruler Bonus** | +15% fortress defense |
| **Faction Trait** | Piast Fortifications |
| **Trait Desc** | Fortified cities take 25% less damage during siege |
| **Strategic Asset** | Gniezno Cathedral |
| **Asset Desc** | +15% levy recruitment speed |
| **Terrain** | PL 28 / FO 27 / HI 12 / RV 15 / SW 8 / MT 10 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| pol_gniezno | Gniezno | 6,000 | (0.525, 0.695) | Plains | Forest | Yes |
| pol_poznan | Poznań | 5,000 | (0.522, 0.693) | Plains | RiverValley | No |
| pol_krakow | Kraków | 5,500 | (0.530, 0.680) | Hills | Forest | No |
| pol_wroclaw | Wrocław | 4,000 | (0.525, 0.685) | Plains | RiverValley | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| pol_druzhyna | Drużyna Cavalry | HC | 105 | 17 | 6 | 4.3 | 2.5 | 0.9 | inspire | Spear | Heavy | Conical | Kite | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.05 | 0.28 | 0.12 |
| pol_spearman | Polish Spearman | HI | 100 | 10 | 5 | 3.4 | 3.0 | 1.2 | pike_brace | Spear | Medium | Conical | Round | Chainmail | Wood | No | No | (0.85,0.70,0.55) | 1.0 | 0.28 | 0.12 |
| pol_archer | Polish Archer | R | 50 | 10 | 0 | 3.9 | 12.0 | 1.4 | null | Bow | Light | None | None | Leather | Wood | No | Yes | (0.85,0.70,0.55) | 0.95 | 0.26 | 0.12 |
| pol_garrison | Fort Garrison | HI | 120 | 9 | 7 | 2.8 | 2.5 | 1.4 | fortify | Spear | Heavy | Conical | Tower | Chainmail | Steel | No | No | (0.85,0.70,0.55) | 1.05 | 0.28 | 0.13 |
| pol_cavalry | Polish Light Cavalry | LC | 65 | 12 | 2 | 5.0 | 5.0 | 1.2 | skirmish | Javelin | Light | Conical | None | Leather | Steel | Yes | No | (0.85,0.70,0.55) | 0.95 | 0.27 | 0.12 |

---

## 6. Kingdom of Hungary

| Field | Value |
|-------|-------|
| **ID** | `hungary` |
| **Display Name** | Kingdom of Hungary |
| **Region** | Europe |
| **Capital** | `hun_esztergom` |
| **Primary Color** | (0.55, 0.75, 0.30) |
| **Secondary Color** | (0.70, 0.85, 0.45) |
| **Military** | 42,000 |
| **Ruler** | Stephen I |
| **Ruler Bonus** | +15% cavalry speed on Steppe terrain |
| **Faction Trait** | Steppe Heritage |
| **Trait Desc** | Light cavalry units cost 20% less to field |
| **Strategic Asset** | Esztergom Basilica |
| **Asset Desc** | +20% defense in capital province |
| **Terrain** | PL 38 / ST 20 / FO 12 / HI 10 / MT 10 / RV 10 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| hun_esztergom | Esztergom | 6,500 | (0.530, 0.670) | Plains | RiverValley | Yes |
| hun_szekesfehervar | Székesfehérvár | 5,000 | (0.530, 0.668) | Plains | Steppe | No |
| hun_pecs | Pécs | 3,500 | (0.528, 0.662) | Hills | Plains | No |
| hun_transylvania | Transylvanian Forts | 7,000 | (0.540, 0.665) | Mountains | Forest | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| hun_horsearcher | Horse Archer | LC | 60 | 11 | 1 | 5.2 | 8.0 | 1.3 | horse_archer_kite | Bow | Light | Conical | None | Leather | Wood | No | Yes | (0.82,0.68,0.52) | 0.95 | 0.27 | 0.12 |
| hun_lancer | Hungarian Lancer | HC | 110 | 19 | 6 | 4.5 | 2.5 | 1.0 | null | Spear | Heavy | Conical | Kite | Chainmail | Steel | Yes | No | (0.82,0.68,0.52) | 1.05 | 0.29 | 0.12 |
| hun_levy | Levy Infantry | LI | 70 | 11 | 2 | 3.8 | 2.5 | 0.9 | null | Spear | Light | None | Round | Leather | Wood | No | No | (0.82,0.68,0.52) | 1.0 | 0.28 | 0.12 |
| hun_border | Border Guard | HI | 105 | 10 | 6 | 3.2 | 2.5 | 1.3 | fortify | Sword | Medium | Conical | Kite | Chainmail | Steel | No | No | (0.82,0.68,0.52) | 1.0 | 0.28 | 0.12 |
| hun_noble | Noble Retinue | HC | 100 | 16 | 5 | 4.6 | 2.5 | 0.9 | inspire | Mace | Heavy | Conical | Kite | Chainmail | Steel | Yes | No | (0.82,0.68,0.52) | 1.05 | 0.28 | 0.12 |

---

## 7. Holy Roman Empire

| Field | Value |
|-------|-------|
| **ID** | `hre` |
| **Display Name** | Holy Roman Empire |
| **Region** | Europe |
| **Capital** | `hre_aachen` |
| **Primary Color** | (0.50, 0.50, 0.55) |
| **Secondary Color** | (0.70, 0.65, 0.20) |
| **Military** | 96,000 |
| **Ruler** | Henry II |
| **Ruler Bonus** | Diplomacy bonus with Church factions |
| **Faction Trait** | Feudal Call |
| **Trait Desc** | Vassal provinces grant temporary knight levies each campaign turn |
| **Strategic Asset** | Aachen Cathedral |
| **Asset Desc** | Core provinces: +25% levy speed |
| **Terrain** | PL 22 / FO 28 / HI 18 / MT 12 / RV 12 / CO 8 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| hre_aachen | Aachen | 7,000 | (0.507, 0.680) | Plains | Forest | Yes |
| hre_mainz | Mainz | 8,000 | (0.510, 0.675) | RiverValley | Hills | No |
| hre_regensburg | Regensburg | 7,000 | (0.515, 0.670) | Hills | Forest | No |
| hre_cologne | Cologne | 7,000 | (0.508, 0.680) | RiverValley | Plains | No |
| hre_magdeburg | Magdeburg | 5,000 | (0.515, 0.690) | Plains | Forest | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| hre_knight | Feudal Knight | HC | 120 | 20 | 7 | 4.3 | 2.5 | 1.0 | null | Spear | Heavy | Nasal | Kite | Chainmail | Steel | Yes | No | (0.85,0.72,0.58) | 1.1 | 0.29 | 0.13 |
| hre_menatarms | Man-at-Arms | HI | 110 | 13 | 6 | 3.5 | 2.5 | 1.0 | parry | Sword | Heavy | Nasal | Kite | Chainmail | Steel | No | No | (0.85,0.72,0.58) | 1.05 | 0.28 | 0.12 |
| hre_spearlevy | Spear Levy | HI | 90 | 9 | 4 | 3.3 | 3.0 | 1.3 | formation_discipline | Spear | Medium | None | Round | Chainmail | Wood | No | No | (0.85,0.72,0.58) | 1.0 | 0.28 | 0.12 |
| hre_crossbow | Crossbowman | R | 55 | 14 | 1 | 3.5 | 12.0 | 1.8 | volley_fire | Crossbow | Medium | Nasal | None | Chainmail | Steel | No | No | (0.85,0.72,0.58) | 1.0 | 0.28 | 0.12 |
| hre_siege | Siege Crew | S | 180 | 30 | 3 | 2.0 | 14.0 | 3.0 | null | None | Light | None | None | Leather | Wood | No | No | (0.85,0.72,0.58) | 1.0 | 0.28 | 0.12 |

---

## 8. Kingdom of France

| Field | Value |
|-------|-------|
| **ID** | `france` |
| **Display Name** | Kingdom of France |
| **Region** | Europe |
| **Capital** | `fra_paris` |
| **Primary Color** | (0.25, 0.25, 0.80) |
| **Secondary Color** | (0.85, 0.80, 0.20) |
| **Military** | 58,000 |
| **Ruler** | Robert II the Pious |
| **Ruler Bonus** | +15% defense in fortified cities |
| **Faction Trait** | Castle Network |
| **Trait Desc** | Controlled provinces with cities gain +10% defense |
| **Strategic Asset** | Paris Cathedral |
| **Asset Desc** | +20% income from capital province |
| **Terrain** | PL 30 / FO 22 / HI 16 / RV 14 / CO 10 / MT 8 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| fra_paris | Paris | 8,000 | (0.498, 0.670) | Plains | RiverValley | Yes |
| fra_orleans | Orléans | 5,000 | (0.497, 0.668) | Plains | Forest | No |
| fra_reims | Reims | 4,500 | (0.502, 0.672) | Plains | Hills | No |
| fra_rouen | Rouen | 5,000 | (0.495, 0.672) | Coast | Plains | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| fra_knight | French Knight | HC | 115 | 19 | 6 | 4.4 | 2.5 | 1.0 | null | Spear | Heavy | Nasal | Kite | Chainmail | Steel | Yes | No | (0.85,0.72,0.58) | 1.08 | 0.29 | 0.13 |
| fra_levy | Infantry Levy | HI | 85 | 9 | 3 | 3.4 | 2.5 | 1.2 | formation_discipline | Spear | Medium | None | Round | Chainmail | Wood | No | No | (0.85,0.72,0.58) | 1.0 | 0.28 | 0.12 |
| fra_archer | French Archer | R | 50 | 10 | 0 | 4.0 | 13.0 | 1.4 | mark | Bow | Light | Hood | None | Leather | Wood | No | Yes | (0.85,0.72,0.58) | 0.95 | 0.26 | 0.12 |
| fra_garrison | Castle Garrison | HI | 115 | 10 | 7 | 2.8 | 2.5 | 1.4 | fortify | Sword | Heavy | Nasal | Kite | Chainmail | Steel | No | No | (0.85,0.72,0.58) | 1.05 | 0.28 | 0.13 |
| fra_mercenary | Mercenary | LI | 75 | 16 | 2 | 4.2 | 2.5 | 0.7 | skirmish | Sword | Light | None | Buckler | Leather | Steel | No | No | (0.85,0.72,0.58) | 1.0 | 0.28 | 0.12 |

---

## 9. Byzantine Empire

| Field | Value |
|-------|-------|
| **ID** | `byzantine` |
| **Display Name** | Byzantine Empire |
| **Region** | Europe |
| **Capital** | `byz_constantinople` |
| **Primary Color** | (0.55, 0.15, 0.60) |
| **Secondary Color** | (0.80, 0.70, 0.20) |
| **Military** | 112,000 |
| **Ruler** | Basil II Bulgar-Slayer |
| **Ruler Bonus** | Massive anti-Balkan campaign bonus + Varangian loyalty |
| **Faction Trait** | Theme System |
| **Trait Desc** | Controlled provinces auto-recruit +15% extra thematic troops per campaign turn |
| **Strategic Asset** | Theodosian Walls & Hagia Sophia |
| **Asset Desc** | Constantinople: +40% siege defense; global Orthodox morale boost |
| **Terrain** | PL 16 / FO 14 / HI 22 / MT 24 / CO 16 / RV 8 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| byz_constantinople | Constantinople | 18,000 | (0.545, 0.640) | Coast | Plains | Yes |
| byz_thessaloniki | Thessaloniki | 10,000 | (0.538, 0.640) | Coast | Hills | No |
| byz_adrianople | Adrianople | 8,000 | (0.542, 0.645) | Plains | Hills | No |
| byz_antioch | Antioch | 9,000 | (0.555, 0.625) | Hills | Mountains | No |
| byz_nicaea | Nicaea | 6,000 | (0.548, 0.638) | Hills | Coast | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| byz_cataphract | Cataphract | HC | 130 | 22 | 8 | 4.0 | 3.0 | 1.1 | null | Spear | Heavy | Conical | None | Chainmail | Steel | Yes | No | (0.80,0.65,0.50) | 1.15 | 0.30 | 0.14 |
| byz_varangian | Varangian Guard | HI | 125 | 20 | 6 | 3.6 | 2.8 | 0.8 | berserker_rage | DualAxe | Heavy | Spectacle | None | Chainmail | Steel | Yes | No | (0.85,0.70,0.55) | 1.15 | 0.32 | 0.14 |
| byz_thematic | Thematic Infantry | HI | 100 | 11 | 5 | 3.3 | 3.0 | 1.2 | shield_wall | Spear | Medium | Conical | Round | Chainmail | Steel | No | No | (0.80,0.65,0.50) | 1.0 | 0.28 | 0.12 |
| byz_archer | Byzantine Archer | R | 55 | 12 | 1 | 4.0 | 13.0 | 1.3 | mark | Bow | Light | Conical | None | Leather | Wood | No | Yes | (0.80,0.65,0.50) | 0.95 | 0.26 | 0.12 |
| byz_tagmata | Tagmata Cavalry | HC | 110 | 17 | 6 | 4.5 | 2.5 | 0.9 | inspire | Sword | Heavy | Conical | Kite | Chainmail | Steel | Yes | No | (0.80,0.65,0.50) | 1.08 | 0.29 | 0.13 |

---

## 10. Christian Iberia

| Field | Value |
|-------|-------|
| **ID** | `iberia_north` |
| **Display Name** | Christian Iberia |
| **Region** | Europe |
| **Capital** | `ibe_leon` |
| **Primary Color** | (0.75, 0.60, 0.20) |
| **Secondary Color** | (0.85, 0.15, 0.15) |
| **Military** | 39,000 |
| **Ruler** | Alfonso V of León |
| **Ruler Bonus** | +15% defense in mountain terrain |
| **Faction Trait** | Reconquista Zeal |
| **Trait Desc** | +10% attack when battling Muslim Iberia faction |
| **Strategic Asset** | Santiago de Compostela |
| **Asset Desc** | +20% morale for all units |
| **Terrain** | MT 28 / HI 22 / PL 15 / FO 15 / RV 10 / CO 10 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| ibe_leon | León | 6,000 | (0.478, 0.645) | Mountains | Plains | Yes |
| ibe_pamplona | Pamplona | 4,500 | (0.484, 0.648) | Mountains | Hills | No |
| ibe_burgos | Burgos | 4,500 | (0.480, 0.647) | Hills | Plains | No |
| ibe_jaca | Jaca | 5,500 | (0.484, 0.650) | Mountains | Forest | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| ibe_knight | Iberian Knight | HC | 110 | 18 | 6 | 4.3 | 2.5 | 1.0 | null | Spear | Heavy | Nasal | Kite | Chainmail | Steel | Yes | No | (0.82,0.68,0.52) | 1.08 | 0.29 | 0.13 |
| ibe_jinete | Jinete | LC | 65 | 13 | 2 | 5.3 | 5.0 | 1.1 | skirmish | Javelin | Light | Conical | Buckler | Leather | Steel | No | No | (0.82,0.68,0.52) | 0.95 | 0.27 | 0.12 |
| ibe_levy | Infantry Levy | HI | 90 | 9 | 4 | 3.3 | 2.5 | 1.3 | formation_discipline | Spear | Medium | Conical | Round | Chainmail | Wood | No | No | (0.82,0.68,0.52) | 1.0 | 0.28 | 0.12 |
| ibe_archer | Iberian Archer | R | 50 | 10 | 0 | 3.9 | 12.0 | 1.4 | null | Bow | Light | None | None | Leather | Wood | No | Yes | (0.82,0.68,0.52) | 0.95 | 0.26 | 0.12 |
| ibe_fortress | Fortress Troops | HI | 120 | 10 | 7 | 2.8 | 2.5 | 1.4 | fortify | Sword | Heavy | Nasal | Tower | Chainmail | Steel | No | No | (0.82,0.68,0.52) | 1.05 | 0.28 | 0.13 |

---

## 11. Córdoba / Muslim Iberia

| Field | Value |
|-------|-------|
| **ID** | `cordoba` |
| **Display Name** | Córdoba |
| **Region** | Europe |
| **Capital** | `cor_cordoba` |
| **Primary Color** | (0.15, 0.55, 0.30) |
| **Secondary Color** | (0.85, 0.75, 0.20) |
| **Military** | 46,000 |
| **Ruler** | Hisham II (nominal) |
| **Ruler Bonus** | +15% income from trade cities |
| **Faction Trait** | Taifa Fragmentation |
| **Trait Desc** | Provinces lost have 20% chance to rebel and become neutral |
| **Strategic Asset** | Great Mosque of Córdoba |
| **Asset Desc** | +25% research and morale in capital |
| **Terrain** | PL 22 / HI 20 / MT 15 / DE 10 / RV 13 / CO 10 / FO 10 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| cor_cordoba | Córdoba | 9,000 | (0.479, 0.630) | Plains | RiverValley | Yes |
| cor_seville | Seville | 6,000 | (0.476, 0.626) | Plains | Coast | No |
| cor_toledo | Toledo | 6,500 | (0.480, 0.636) | Hills | Plains | No |
| cor_zaragoza | Zaragoza | 5,000 | (0.484, 0.642) | RiverValley | Hills | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| cor_cavalry | Andalusian Cavalry | HC | 105 | 17 | 5 | 4.5 | 2.5 | 0.9 | null | Sword | Heavy | Turban | Round | Chainmail | Steel | Yes | No | (0.75,0.60,0.45) | 1.05 | 0.28 | 0.12 |
| cor_berber | Berber Light Cavalry | LC | 60 | 14 | 2 | 5.2 | 5.0 | 1.1 | skirmish | Javelin | Light | Turban | Buckler | Leather | Steel | No | No | (0.65,0.50,0.35) | 0.95 | 0.27 | 0.12 |
| cor_spearman | Moorish Spearman | HI | 100 | 10 | 5 | 3.3 | 3.0 | 1.2 | shield_wall | Spear | Medium | Turban | Round | Chainmail | Steel | No | No | (0.75,0.60,0.45) | 1.0 | 0.28 | 0.12 |
| cor_archer | Moorish Archer | R | 50 | 11 | 0 | 4.0 | 13.0 | 1.4 | mark | Bow | Light | Turban | None | Leather | Wood | No | Yes | (0.75,0.60,0.45) | 0.95 | 0.26 | 0.12 |
| cor_guard | Guard Infantry | HI | 115 | 13 | 6 | 3.4 | 2.5 | 1.0 | parry | Sword | Heavy | Turban | Kite | Chainmail | Steel | Yes | No | (0.75,0.60,0.45) | 1.05 | 0.28 | 0.12 |

---

# MIDDLE EAST, NORTH AFRICA & CENTRAL ASIA (8 Factions)

---

## 12. Fatimid Caliphate

| Field | Value |
|-------|-------|
| **ID** | `fatimid` |
| **Capital** | `fat_cairo` |
| **Primary Color** | (0.10, 0.60, 0.25) |
| **Secondary Color** | (0.85, 0.80, 0.20) |
| **Military** | 72,000 |
| **Ruler** | al-Hakim bi-Amr Allah |
| **Ruler Bonus** | Religious zeal: +15% morale |
| **Trait** | Ismaili Propaganda — Convert enemy provinces 20% faster |
| **Asset** | Al-Azhar Mosque — Cairo: +30% research, unique Mamluk recruitment |
| **Terrain** | DE 34 / RV 18 / CO 14 / PL 10 / HI 8 / MT 6 / SW 10 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| fat_cairo | Cairo | 15,000 | (0.545, 0.555) | RiverValley | Desert | Yes |
| fat_alexandria | Alexandria | 6,000 | (0.543, 0.560) | Coast | Desert | No |
| fat_jerusalem | Jerusalem | 4,500 | (0.555, 0.575) | Hills | Desert | No |
| fat_damascus | Damascus | 5,500 | (0.558, 0.585) | Plains | Desert | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| fat_berber_cav | Berber Cavalry | HC | 100 | 17 | 5 | 4.6 | 2.5 | 0.9 | null | Sword | Medium | Turban | Round | Chainmail | Steel | Yes | No | (0.65,0.50,0.35) | 1.05 | 0.28 | 0.12 |
| fat_turkish | Turkish Guard | HC | 105 | 18 | 6 | 4.4 | 2.5 | 1.0 | inspire | Mace | Heavy | Turban | Kite | Chainmail | Steel | Yes | No | (0.75,0.60,0.45) | 1.08 | 0.29 | 0.13 |
| fat_sudanese | Sudanese Infantry | HI | 95 | 12 | 4 | 3.5 | 2.5 | 1.1 | war_cry | Spear | Light | Wrapped | Round | Leather | Steel | No | No | (0.45,0.30,0.20) | 1.05 | 0.29 | 0.13 |
| fat_daylamite | Daylamite Infantry | HI | 110 | 14 | 5 | 3.3 | 2.5 | 1.0 | shield_wall | Spear | Medium | Conical | Round | Chainmail | Steel | No | No | (0.75,0.60,0.45) | 1.0 | 0.28 | 0.12 |
| fat_marines | Fatimid Marines | N | 75 | 12 | 2 | 4.2 | 2.5 | 0.9 | naval_boarding | Sword | Light | Turban | Buckler | Leather | Steel | No | No | (0.70,0.55,0.40) | 1.0 | 0.28 | 0.12 |

---

## 13. Abbasid Caliphate

| Field | Value |
|-------|-------|
| **ID** | `abbasid` |
| **Capital** | `abb_baghdad` |
| **Primary Color** | (0.10, 0.10, 0.10) |
| **Secondary Color** | (0.85, 0.75, 0.20) |
| **Military** | 28,000 |
| **Ruler** | al-Qadir |
| **Ruler Bonus** | +20% legitimacy in controlled provinces |
| **Trait** | Caliphal Legitimacy — Allied factions have +10% morale |
| **Asset** | House of Wisdom — +25% research speed |
| **Terrain** | DE 28 / RV 26 / PL 14 / HI 10 / MT 8 / SW 8 / CO 6 |

**Cities:**

| ID | Name | Garrison | Position | Primary Terrain | Secondary Terrain | Capital |
|----|------|----------|----------|-----------------|-------------------|---------|
| abb_baghdad | Baghdad | 9,000 | (0.568, 0.590) | RiverValley | Desert | Yes |
| abb_basra | Basra | 4,000 | (0.572, 0.580) | Wetlands | Desert | No |
| abb_wasit | Wasit | 3,500 | (0.570, 0.585) | RiverValley | Plains | No |

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM | Cape | Back | Skin | bodyScale | shoulderW | hipW |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|----|----|------|------|-----------|-----------|------|
| abb_guard_cav | Guard Cavalry | HC | 100 | 16 | 5 | 4.4 | 2.5 | 1.0 | inspire | Sword | Heavy | Turban | Kite | Chainmail | Steel | Yes | No | (0.75,0.60,0.45) | 1.05 | 0.28 | 0.12 |
| abb_ghilman | Ghilmān | HI | 105 | 14 | 5 | 3.5 | 2.5 | 1.0 | parry | Sword | Heavy | Turban | Round | Chainmail | Steel | No | No | (0.72,0.58,0.42) | 1.05 | 0.28 | 0.12 |
| abb_infantry | City Infantry | HI | 90 | 10 | 4 | 3.3 | 2.5 | 1.2 | formation_discipline | Spear | Medium | Turban | Round | Chainmail | Wood | No | No | (0.75,0.60,0.45) | 1.0 | 0.28 | 0.12 |
| abb_garrison | City Garrison | HI | 110 | 9 | 6 | 2.8 | 2.5 | 1.4 | fortify | Spear | Medium | Turban | Tower | Chainmail | Steel | No | No | (0.75,0.60,0.45) | 1.0 | 0.28 | 0.12 |
| abb_auxiliary | Tribal Auxiliary | LC | 60 | 11 | 1 | 5.0 | 5.0 | 1.2 | skirmish | Javelin | Light | Wrapped | None | Leather | Steel | No | No | (0.70,0.55,0.40) | 0.95 | 0.27 | 0.12 |

---

## 14. Buyid Emirates

| Field | Value |
|-------|-------|
| **ID** | `buyid` |
| **Capital** | `buy_shiraz` |
| **Primary Color** | (0.50, 0.20, 0.55) |
| **Secondary Color** | (0.70, 0.35, 0.70) |
| **Military** | 56,000 |
| **Ruler** | Sultan al-Dawla |
| **Ruler Bonus** | +15% mountain defense |
| **Trait** | Daylamite Warriors — Infantry units gain +10% HP |
| **Asset** | Shiraz Palace — +20% income from core provinces |
| **Terrain** | MT 24 / HI 22 / DE 18 / PL 14 / RV 10 / CO 6 / FO 6 |

**Cities:** Shiraz (9,000, 0.580, 0.565), Rayy (7,000, 0.575, 0.595), Isfahan (7,000, 0.576, 0.580), Baghdad contingent (6,000, 0.568, 0.590)

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| buy_daylamite | Daylamite Infantry | HI | 115 | 14 | 6 | 3.4 | 2.5 | 1.0 | shield_wall | Spear | Medium | Conical | Round | Chainmail | Steel |
| buy_turkish_cav | Turkish Cavalry | HC | 100 | 17 | 5 | 4.5 | 2.5 | 0.9 | null | Sword | Heavy | Turban | Kite | Chainmail | Steel |
| buy_guards | Buyid Guards | HI | 105 | 13 | 5 | 3.5 | 2.5 | 1.0 | parry | Sword | Heavy | Turban | Round | Chainmail | Steel |
| buy_archers | Persian Archers | R | 50 | 11 | 0 | 4.0 | 13.0 | 1.4 | mark | Bow | Light | Turban | None | Leather | Wood |
| buy_provincial | Provincial Troops | LI | 75 | 11 | 2 | 3.8 | 2.5 | 1.0 | null | Spear | Light | None | Round | Leather | Wood |

Skin: (0.75, 0.60, 0.45). Standard build proportions. Turban helms. Cape: Guards only.

---

## 15. Ghaznavid Empire

| Field | Value |
|-------|-------|
| **ID** | `ghaznavid` |
| **Capital** | `ghz_ghazni` |
| **Primary Color** | (0.65, 0.15, 0.15) |
| **Secondary Color** | (0.85, 0.70, 0.20) |
| **Military** | 88,000 |
| **Ruler** | Mahmud of Ghazni |
| **Ruler Bonus** | Raid income bonus: +30% plunder from conquered provinces |
| **Trait** | Elephant Terror — Elephant units cause 20% enemy rout chance |
| **Asset** | Ghazni Palace Library — +25% plunder from enemy provinces |
| **Terrain** | MT 26 / HI 20 / ST 18 / DE 12 / PL 10 / RV 8 / FO 6 |

**Cities:** Ghazni (14,000, 0.610, 0.575, capital), Lahore (9,000, 0.625, 0.555), Balkh (7,000, 0.608, 0.590), Herat (7,000, 0.600, 0.585)

**Units:**

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| ghz_ghulam | Ghulam Cavalry | HC | 110 | 19 | 6 | 4.5 | 2.5 | 0.9 | inspire | Sword | Heavy | Turban | Kite | Chainmail | Steel |
| ghz_horsearcher | Horse Archer | LC | 60 | 11 | 1 | 5.3 | 8.0 | 1.3 | horse_archer_kite | Bow | Light | Turban | None | Leather | Wood |
| ghz_infantry | Afghan Infantry | HI | 95 | 11 | 4 | 3.4 | 2.5 | 1.1 | formation_discipline | Spear | Medium | Turban | Round | Chainmail | Steel |
| ghz_elephant | War Elephant | E | 250 | 25 | 8 | 2.2 | 3.5 | 1.8 | elephant_charge | Elephant | Heavy | None | None | Leather | Bronze |
| ghz_siege | Siege Corps | S | 160 | 28 | 2 | 1.8 | 15.0 | 3.2 | null | None | Light | None | None | Leather | Wood |

Skin: (0.70, 0.55, 0.40). Elephant: bodyScale 2.5, shoulderW 0.50, hipW 0.30. Cape: Ghulam only.

---

## 16-19: Remaining Middle East (Condensed Format)

### 16. Kara-Khanid Khanate
**ID:** `karakhanid` | Capital: `kk_balasaghun` | Color: (0.20, 0.60, 0.65) | Military: 74,000
Ruler: Yusuf Qadr Khan | Trait: Steppe Mastery — Cavalry +20% speed on Steppe
Cities: Balasaghun (10K, 0.630, 0.615), Kashgar (9K, 0.640, 0.600), Samarkand (11K, 0.618, 0.600)
Terrain: ST 40 / DE 18 / MT 16 / HI 10 / RV 8 / PL 8

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| kk_horsearcher | Turkic Horse Archer | LC | 60 | 12 | 1 | 5.4 | 8.0 | 1.2 | horse_archer_kite |
| kk_heavy_cav | Heavy Cavalry | HC | 110 | 18 | 6 | 4.4 | 2.5 | 1.0 | null |
| kk_lancer | Tribal Lancer | HC | 100 | 16 | 5 | 4.6 | 3.0 | 1.0 | null |
| kk_guards | Khanate Guards | HI | 105 | 13 | 5 | 3.5 | 2.5 | 1.1 | parry |
| kk_garrison | Town Garrison | HI | 95 | 9 | 5 | 3.0 | 2.5 | 1.3 | fortify |

Skin: (0.75, 0.60, 0.45). Turban helms. Fur armor. Steppe visual style.

### 17. Khwarazm
**ID:** `khwarazm` | Capital: `khw_gurganj` | Color: (0.55, 0.40, 0.20) | Military: 19,000
Ruler: Ma'mun I | Trait: Oasis Fortifications — +20% defense in Desert terrain
Cities: Gurganj (7K, 0.600, 0.610), Kath (3K, 0.598, 0.612), Khiva (2.5K, 0.599, 0.608)
Terrain: DE 34 / ST 24 / RV 14 / PL 8 / HI 8 / CO 6 / SW 6

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| khw_cavalry | Khwarazmian Cavalry | HC | 95 | 15 | 4 | 4.5 | 2.5 | 1.0 | null |
| khw_infantry | Khwarazmian Infantry | HI | 90 | 10 | 4 | 3.3 | 2.5 | 1.2 | formation_discipline |
| khw_archers | Khwarazmian Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| khw_guards | Palace Guards | HI | 105 | 13 | 5 | 3.4 | 2.5 | 1.1 | parry |
| khw_fortress | Fortress Troops | HI | 115 | 9 | 7 | 2.8 | 2.5 | 1.4 | fortify |

Skin: (0.75, 0.60, 0.45). Turban helms. Medium armor.

### 18. Georgia
**ID:** `georgia` | Capital: `geo_kutaisi` | Color: (0.60, 0.15, 0.30) | Military: 29,000
Ruler: Bagrat III | Trait: Mountain Fortress — +25% defense in Mountains
Cities: Kutaisi (5.5K, 0.565, 0.640), Tbilisi (5K, 0.568, 0.637), Mtskheta (3.5K, 0.567, 0.638)
Terrain: MT 40 / HI 22 / FO 14 / PL 8 / RV 8 / CO 8

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| geo_noble_cav | Noble Cavalry | HC | 105 | 17 | 6 | 4.3 | 2.5 | 1.0 | inspire |
| geo_infantry | Georgian Infantry | HI | 100 | 11 | 5 | 3.4 | 2.5 | 1.1 | formation_discipline |
| geo_archers | Georgian Archers | R | 50 | 10 | 0 | 3.9 | 12.0 | 1.4 | mark |
| geo_mountain | Mountain Troops | LI | 75 | 14 | 3 | 4.2 | 2.5 | 0.8 | ambush |
| geo_fortress | Fortress Garrison | HI | 115 | 9 | 7 | 2.8 | 2.5 | 1.4 | fortify |

Skin: (0.80, 0.65, 0.50). Conical helms. Medium-Heavy armor.

### 19. Armenian Kingdoms
**ID:** `armenia` | Capital: `arm_ani` | Color: (0.85, 0.45, 0.15) | Military: 23,000
Ruler: Gagik I | Trait: Highland Resilience — Units take 15% less attrition in Mountains
Cities: Ani (5K, 0.565, 0.635), Van (4.5K, 0.567, 0.630), Dvin (3.5K, 0.566, 0.633)
Terrain: MT 38 / HI 26 / PL 10 / FO 8 / RV 8 / DE 10

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| arm_heavy_cav | Armenian Heavy Cavalry | HC | 105 | 17 | 6 | 4.2 | 2.5 | 1.0 | null |
| arm_infantry | Armenian Infantry | HI | 95 | 10 | 5 | 3.3 | 2.5 | 1.2 | formation_discipline |
| arm_archers | Armenian Archers | R | 50 | 11 | 0 | 3.9 | 12.0 | 1.4 | mark |
| arm_hill | Hill Troops | LI | 75 | 14 | 3 | 4.1 | 2.5 | 0.8 | ambush |
| arm_fortress | Fortress Garrison | HI | 115 | 9 | 7 | 2.8 | 2.5 | 1.4 | fortify |

Skin: (0.80, 0.65, 0.50). Conical helms. Medium-Heavy armor.

---

# SOUTH ASIA (4 Factions)

---

## 20. Chola Empire

**ID:** `chola` | Capital: `cho_thanjavur` | Color: (0.85, 0.65, 0.10) | Military: 122,000
Ruler: Rajendra Chola I | Trait: Naval Supremacy — Naval units +25% combat bonus | Asset: Brihadeeswarar Temple — +20% morale
Cities: Thanjavur (18K, 0.665, 0.440, capital), Gangaikonda (12K, 0.667, 0.442), Kanchipuram (10K, 0.662, 0.445), Nagapattinam (8K, 0.668, 0.435)
Terrain: CO 20 / PL 18 / RV 16 / JN 10 / HI 12 / MT 10 / FO 8 / SW 6

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| cho_infantry | Chola Infantry | HI | 100 | 12 | 5 | 3.4 | 2.5 | 1.1 | formation_discipline | Sword | Medium | Conical | Round | Bronze | Steel |
| cho_cavalry | Chola Cavalry | HC | 95 | 16 | 4 | 4.5 | 2.5 | 1.0 | null | Spear | Medium | Conical | None | Bronze | Steel |
| cho_elephant | Chola Elephant Corps | E | 240 | 24 | 7 | 2.3 | 3.5 | 1.8 | elephant_charge | Elephant | Heavy | None | None | Leather | Bronze |
| cho_guard | Velaikkarar Guard | SP | 90 | 15 | 4 | 3.8 | 2.5 | 0.8 | inspire | Sword | Medium | Crown | Round | Bronze | Steel |
| cho_marines | Chola Navy Marines | N | 80 | 13 | 2 | 4.2 | 2.5 | 0.9 | naval_boarding | Sword | Light | None | Buckler | Leather | Bronze |

Skin: (0.65, 0.45, 0.30). Bronze/Gold armor. Conical helms.

---

## 21-23: Remaining South Asia (Condensed)

### 21. Western Chalukya
**ID:** `chalukya` | Capital: `cha_manyakheta` | Color: (0.15, 0.25, 0.65) | Military: 92,000
Ruler: Jayasimha II | Cities: Manyakheta (16K, 0.660, 0.460), Banavasi (7K, 0.658, 0.455), Lakkundi (6K, 0.659, 0.458)
Terrain: PL 16 / HI 22 / MT 18 / FO 12 / RV 10 / CO 8 / JN 14

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| cha_heavy_cav | Chalukya Heavy Cavalry | HC | 105 | 18 | 6 | 4.4 | 2.5 | 1.0 | null |
| cha_infantry | Chalukya Infantry | HI | 100 | 11 | 5 | 3.3 | 2.5 | 1.1 | formation_discipline |
| cha_elephant | Chalukya Elephant | E | 235 | 23 | 7 | 2.2 | 3.5 | 1.9 | elephant_charge |
| cha_archers | Chalukya Archers | R | 50 | 11 | 0 | 3.9 | 12.0 | 1.4 | mark |
| cha_feudatory | Feudatory Troops | LI | 75 | 12 | 2 | 3.8 | 2.5 | 1.0 | null |

### 22. Pala Empire
**ID:** `pala` | Capital: `pal_pataliputra` | Color: (0.55, 0.10, 0.20) | Military: 66,000
Ruler: Mahipala I | Cities: Pataliputra (8K, 0.660, 0.490), Gauda (14K, 0.668, 0.485), Vikrampur (6K, 0.672, 0.480)
Terrain: RV 28 / PL 18 / SW 18 / JN 12 / FO 10 / HI 8 / CO 6

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| pal_infantry | Pala Infantry | HI | 95 | 11 | 4 | 3.4 | 2.5 | 1.1 | formation_discipline |
| pal_cavalry | Pala Cavalry | HC | 95 | 16 | 4 | 4.4 | 2.5 | 1.0 | null |
| pal_elephant | Pala War Elephant | E | 245 | 24 | 7 | 2.2 | 3.5 | 1.8 | elephant_charge |
| pal_archers | Pala Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| pal_river | River Forces | N | 70 | 11 | 2 | 4.0 | 2.5 | 1.0 | naval_boarding |

### 23. Rajput States
**ID:** `rajput` | Capital: `raj_ajmer` | Color: (0.80, 0.15, 0.15) | Military: 98,000
Ruler: Rajput Confederation | Cities: Ajmer (10K, 0.635, 0.510), Kannauj (12K, 0.645, 0.510), Chittor (8K, 0.638, 0.505), Anhilwara (8K, 0.630, 0.500)
Terrain: PL 22 / DE 20 / HI 18 / MT 14 / FO 8 / RV 10 / CO 8

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| raj_lancer | Rajput Lancer | HC | 110 | 19 | 5 | 4.6 | 2.5 | 0.9 | null |
| raj_infantry | Rajput Infantry | HI | 95 | 11 | 4 | 3.4 | 2.5 | 1.1 | formation_discipline |
| raj_archers | Rajput Archers | R | 50 | 10 | 0 | 3.9 | 12.0 | 1.4 | mark |
| raj_elephant | Rajput War Elephant | E | 240 | 24 | 7 | 2.2 | 3.5 | 1.8 | elephant_charge |
| raj_garrison | Fort Garrison | HI | 120 | 9 | 7 | 2.8 | 2.5 | 1.4 | fortify |
| raj_elite | Rajput Elite Cavalry | HC | 100 | 20 | 5 | 4.8 | 2.5 | 0.8 | zealot_charge |

Skin: (0.65, 0.45, 0.30). Bronze/Silk materials. Conical helms.

---

# EAST ASIA (5 Factions)

---

## 24. Song Empire
**ID:** `song` | Capital: `son_kaifeng` | Color: (0.75, 0.20, 0.20) | Military: 900,000
Ruler: Emperor Zhenzong | Trait: Gunpowder Workshops — Fire lance units +25% damage | Asset: Grand Canal — Massive income + movement bonus
Cities: Kaifeng (90K, 0.755, 0.585, capital), Luoyang (45K, 0.748, 0.587), Taiyuan (55K, 0.752, 0.600), Chengdu (40K, 0.735, 0.565), Hangzhou (30K, 0.760, 0.570)
Terrain: PL 24 / RV 22 / FO 14 / HI 12 / MT 10 / CO 8 / SW 5 / JN 5

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| son_guard | Song Guard Infantry | HI | 105 | 12 | 5 | 3.4 | 2.5 | 1.1 | formation_discipline | Spear | Heavy | Conical | Tower | Lacquer | Steel |
| son_cavalry | Song Cavalry | HC | 95 | 15 | 4 | 4.4 | 2.5 | 1.0 | null | Spear | Heavy | Conical | None | Lacquer | Steel |
| son_crossbow | Crossbow Corps | R | 60 | 15 | 2 | 3.4 | 13.0 | 1.6 | volley_fire | Crossbow | Medium | Straw | None | Lacquer | Steel |
| son_riverfleet | River Fleet | N | 80 | 12 | 3 | 4.0 | 2.5 | 1.0 | naval_boarding | Spear | Medium | Straw | Round | Lacquer | Steel |
| son_firelance | Fire Lance Troops | R | 60 | 16 | 1 | 3.5 | 6.0 | 2.0 | fire_lance | None | Light | Straw | None | Lacquer | Bronze |

Skin: (0.90, 0.78, 0.60). Lacquered armor. Conical/Straw helms.

---

## 25-28: Remaining East Asia

### 25. Liao Dynasty
**ID:** `liao` | Capital: `lia_shangjing` | Color: (0.15, 0.20, 0.55) | Military: 185,000
Ruler: Shengzong | Cities: Shangjing (22K, 0.760, 0.620, capital), Nanjing (28K, 0.758, 0.612), Zhongjing (16K, 0.762, 0.618), Dongjing (14K, 0.765, 0.615)
Terrain: ST 36 / PL 16 / FO 16 / MT 12 / HI 10 / RV 6 / CO 4

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| lia_orda | Imperial Orda Cavalry | HC | 115 | 19 | 6 | 4.6 | 2.5 | 0.9 | null |
| lia_khitan | Khitan Cavalry | LC | 65 | 12 | 2 | 5.2 | 7.0 | 1.2 | horse_archer_kite |
| lia_auxiliary | Auxiliary Cavalry | HC | 95 | 15 | 4 | 4.4 | 2.5 | 1.0 | null |
| lia_han | Han Militia | HI | 85 | 9 | 3 | 3.3 | 2.5 | 1.3 | formation_discipline |
| lia_archers | Foot Archers | R | 50 | 11 | 0 | 3.8 | 12.0 | 1.4 | mark |

### 26. Goryeo
**ID:** `goryeo` | Capital: `gor_kaesong` | Color: (0.20, 0.55, 0.35) | Military: 72,000
Ruler: Hyeonjong | Cities: Kaesong (14K, 0.775, 0.600, capital), Pyongyang (7K, 0.773, 0.608), Gyeongju (5K, 0.780, 0.595)
Terrain: MT 30 / HI 20 / FO 20 / PL 10 / CO 10 / RV 5 / SW 5

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| gor_central | Central Army | HI | 105 | 12 | 5 | 3.4 | 2.5 | 1.1 | formation_discipline |
| gor_frontier | Frontier Troops | HI | 95 | 11 | 4 | 3.5 | 2.5 | 1.0 | fortify |
| gor_reserve | Reserve Infantry | LI | 75 | 10 | 2 | 3.8 | 2.5 | 1.0 | null |
| gor_archers | Goryeo Archers | R | 50 | 11 | 0 | 3.9 | 12.0 | 1.4 | mark |
| gor_cavalry | Goryeo Cavalry | HC | 95 | 15 | 4 | 4.4 | 2.5 | 1.0 | null |

### 27. Heian Japan
**ID:** `japan` | Capital: `jpn_kyoto` | Color: (0.90, 0.90, 0.90) | Military: 61,000
Ruler: Emperor Sanjō | Cities: Kyoto (11K, 0.790, 0.590, capital), Nara (4K, 0.790, 0.588), Dazaifu (5K, 0.785, 0.580), Kantō (12K, 0.795, 0.592)
Terrain: MT 34 / FO 26 / HI 16 / CO 14 / PL 6 / RV 4

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| jpn_samurai | Early Samurai | HC | 100 | 18 | 5 | 4.5 | 2.5 | 0.9 | parry |
| jpn_horsearcher | Mounted Archer | LC | 60 | 12 | 1 | 5.0 | 8.0 | 1.3 | horse_archer_kite |
| jpn_militia | Provincial Militia | HI | 85 | 9 | 3 | 3.3 | 2.5 | 1.3 | formation_discipline |
| jpn_spearman | Ashigaru Spearman | HI | 90 | 10 | 4 | 3.4 | 3.0 | 1.2 | pike_brace |
| jpn_palace | Palace Guards | HI | 110 | 14 | 6 | 3.5 | 2.5 | 1.0 | parry |

Skin: (0.90, 0.78, 0.60). Lacquer armor. Conical helms.

### 28. Dali Kingdom
**ID:** `dali` | Capital: `dal_dali` | Color: (0.25, 0.50, 0.20) | Military: 31,000
Ruler: Duan Sufeng | Cities: Dali (10K, 0.735, 0.540, capital), Kunming (6K, 0.738, 0.545), Yongchang (4K, 0.730, 0.535)
Terrain: MT 34 / HI 24 / JN 14 / FO 12 / RV 8 / PL 4 / CO 4

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| dal_spearman | Dali Spearman | HI | 90 | 10 | 4 | 3.4 | 3.0 | 1.2 | formation_discipline |
| dal_archers | Dali Archers | R | 50 | 10 | 0 | 3.9 | 12.0 | 1.4 | poison_arrow |
| dal_hill | Hill Troops | LI | 75 | 13 | 2 | 4.2 | 2.5 | 0.8 | ambush |
| dal_cavalry | Dali Cavalry | LC | 65 | 12 | 2 | 4.8 | 2.5 | 1.0 | skirmish |
| dal_elephant | Dali Elephant | E | 230 | 22 | 6 | 2.2 | 3.5 | 1.9 | elephant_charge |

---

# SOUTHEAST ASIA (5 Factions)

---

## 29-33: All Southeast Asia

### 29. Khmer Empire
**ID:** `khmer` | Capital: `khm_angkor` | Color: (0.75, 0.65, 0.15) | Military: 78,000
Ruler: Suryavarman I | Asset: Angkor Wat — +25% morale globally
Cities: Angkor (20K, 0.745, 0.400, capital), Siem Reap (8K, 0.744, 0.402), Mekong Staging (9K, 0.748, 0.395)
Terrain: JN 28 / PL 14 / RV 20 / SW 14 / FO 10 / HI 8 / MT 4 / CO 2

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| khm_spearman | Khmer Spearman | HI | 95 | 10 | 4 | 3.3 | 3.0 | 1.2 | formation_discipline |
| khm_shield | Shield Infantry | HI | 110 | 9 | 6 | 3.0 | 2.5 | 1.4 | shield_wall |
| khm_elephant | Khmer Elephant | E | 250 | 25 | 8 | 2.2 | 3.5 | 1.8 | elephant_charge |
| khm_archers | Khmer Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| khm_engineer | Engineer Corps | S | 150 | 25 | 2 | 2.0 | 14.0 | 3.0 | null |

### 30. Srivijaya
**ID:** `srivijaya` | Capital: `sri_palembang` | Color: (0.15, 0.55, 0.55) | Military: 47,000
Ruler: Sangrama Vijayottunggavarman | Trait: Maritime Empire — Naval units +20% in all waters
Cities: Palembang (12K, 0.745, 0.330, capital), Jambi (7K, 0.743, 0.335), Kedah (6K, 0.740, 0.360)
Terrain: CO 34 / JN 28 / SW 14 / RV 10 / PL 6 / HI 4 / MT 4

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| sri_marines | Naval Marines | N | 80 | 13 | 2 | 4.3 | 2.5 | 0.9 | naval_boarding |
| sri_shipcrew | Ship Crews | N | 70 | 11 | 1 | 4.5 | 2.2 | 0.8 | naval_boarding |
| sri_archers | Srivijayan Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| sri_spearman | Srivijayan Spearman | HI | 90 | 10 | 4 | 3.3 | 3.0 | 1.2 | monsoon_tactics |
| sri_tributary | Tributary Troops | LI | 70 | 10 | 2 | 3.8 | 2.5 | 1.0 | null |

### 31. Đại Cồ Việt
**ID:** `dai_viet` | Capital: `dv_hoalu` | Color: (0.80, 0.40, 0.10) | Military: 46,000
Ruler: Lê Hoàn | Cities: Hoa Lư (10K, 0.748, 0.420, capital), Đại La (9K, 0.749, 0.425), Thanh Hóa (5K, 0.747, 0.415)
Terrain: RV 24 / CO 14 / JN 18 / MT 18 / HI 12 / FO 8 / SW 6

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| dv_spearman | Viet Spearman | HI | 95 | 10 | 4 | 3.4 | 3.0 | 1.2 | pike_brace |
| dv_archers | Viet Archers | R | 50 | 11 | 0 | 3.9 | 12.0 | 1.4 | poison_arrow |
| dv_river | River Flotilla | N | 75 | 12 | 2 | 4.2 | 2.5 | 0.9 | naval_boarding |
| dv_guards | Imperial Guards | HI | 110 | 13 | 6 | 3.5 | 2.5 | 1.0 | parry |
| dv_militia | Levy Militia | LI | 70 | 9 | 2 | 3.7 | 2.5 | 1.1 | monsoon_tactics |

### 32. Champa
**ID:** `champa` | Capital: `cham_indrapura` | Color: (0.20, 0.65, 0.70) | Military: 36,000
Ruler: Harivarman IV | Cities: Indrapura (10K, 0.752, 0.405, capital), Kauthara (5K, 0.755, 0.395), Panduranga (4K, 0.756, 0.390)
Terrain: CO 24 / MT 22 / HI 16 / JN 16 / FO 8 / RV 8 / PL 6

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| cham_raiders | Maritime Raiders | N | 75 | 14 | 2 | 4.5 | 2.5 | 0.8 | naval_boarding |
| cham_infantry | Cham Infantry | HI | 90 | 10 | 4 | 3.3 | 2.5 | 1.1 | monsoon_tactics |
| cham_archers | Cham Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| cham_elephant | Cham Elephant | E | 230 | 22 | 6 | 2.2 | 3.5 | 1.9 | elephant_charge |
| cham_cavalry | Cham Cavalry | LC | 60 | 12 | 2 | 4.8 | 2.5 | 1.0 | skirmish |

### 33. Pagan
**ID:** `pagan` | Capital: `pag_bagan` | Color: (0.55, 0.40, 0.15) | Military: 56,000
Ruler: Kyiso | Cities: Bagan (14K, 0.738, 0.415, capital), Prome (6K, 0.736, 0.410), Irrawaddy Forts (8K, 0.737, 0.420)
Terrain: RV 22 / PL 16 / JN 18 / FO 14 / HI 12 / MT 10 / SW 8

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| pag_infantry | Pagan Infantry | HI | 90 | 10 | 4 | 3.4 | 2.5 | 1.1 | formation_discipline |
| pag_archers | Pagan Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| pag_elephant | Pagan Elephant | E | 240 | 23 | 7 | 2.2 | 3.5 | 1.8 | elephant_charge |
| pag_cavalry | Pagan Cavalry | LC | 60 | 12 | 2 | 4.8 | 2.5 | 1.0 | skirmish |
| pag_fort | Fort Troops | HI | 110 | 10 | 6 | 2.8 | 2.5 | 1.4 | fortify |

Skin for all SE Asia: (0.70, 0.50, 0.35). Bronze/Wood materials. Wrapped/Crown helms. Minimal armor.

---

# AFRICA (4 Factions)

---

## 34. Ghana Empire
**ID:** `ghana` | Capital: `gha_koumbi` | Color: (0.70, 0.55, 0.10) | Military: 31,000
Ruler: Tunka Manin | Trait: Gold Caravans — +25% income from desert provinces | Asset: Koumbi Saleh Markets — +40% trade revenue
Cities: Koumbi Saleh (10K, 0.468, 0.445, capital), Awdaghost (4.5K, 0.463, 0.450), Walata (3.5K, 0.465, 0.448)
Terrain: DE 26 / ST 18 / PL 14 / FO 10 / RV 8 / HI 6 / SW 18

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| gha_cavalry | Iron Cavalry | HC | 95 | 16 | 4 | 4.5 | 2.5 | 1.0 | null | Spear | Light | Wrapped | Round | Leather | Steel |
| gha_poison_archer | Poisoned Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | poison_arrow | Bow | None | Wrapped | None | Leather | Wood |
| gha_spearman | Ghanaian Spearman | HI | 90 | 10 | 3 | 3.4 | 3.0 | 1.2 | formation_discipline | Spear | Light | Wrapped | Round | Leather | Wood |
| gha_levy | Levy Infantry | LI | 70 | 9 | 1 | 3.8 | 2.5 | 1.0 | null | Club | None | None | None | Bone | Wood |
| gha_guard | Royal Guard | HI | 105 | 14 | 5 | 3.5 | 2.5 | 1.0 | inspire | Sword | Medium | Wrapped | Round | Leather | Steel |

Skin: (0.45, 0.30, 0.20). Leather/Bone/Wood materials. Wrapped helms.

---

## 35-37: Remaining Africa

### 35. Makuria
**ID:** `makuria` | Capital: `mak_dongola` | Color: (0.70, 0.60, 0.40) | Military: 21,000
Ruler: Raphael | Cities: Dongola (8K, 0.545, 0.465, capital), Faras (3.5K, 0.544, 0.480), Qasr Ibrim (3K, 0.545, 0.485)
Terrain: DE 38 / RV 24 / PL 10 / HI 8 / MT 6 / SW 14

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| mak_archers | Nubian Archers | R | 55 | 12 | 0 | 4.0 | 14.0 | 1.3 | poison_arrow |
| mak_spearman | Makurian Spearman | HI | 90 | 10 | 4 | 3.4 | 3.0 | 1.2 | formation_discipline |
| mak_cavalry | Makurian Cavalry | HC | 90 | 14 | 3 | 4.4 | 2.5 | 1.0 | null |
| mak_guards | Royal Guards | HI | 100 | 12 | 5 | 3.5 | 2.5 | 1.0 | inspire |
| mak_river | River Troops | N | 70 | 10 | 2 | 4.0 | 2.5 | 1.0 | naval_boarding |

### 36. Ethiopian Highlands
**ID:** `ethiopia` | Capital: `eth_aksum` | Color: (0.15, 0.45, 0.15) | Military: 26,000
Ruler: Zagwe Dynasty | Cities: Aksum (5K, 0.558, 0.440, capital), Lalibela (6K, 0.560, 0.435), Tigray (4K, 0.559, 0.442)
Terrain: MT 34 / HI 26 / FO 14 / PL 8 / RV 8 / DE 4 / CO 6

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| eth_spearman | Ethiopian Spearman | HI | 95 | 10 | 4 | 3.4 | 3.0 | 1.2 | formation_discipline |
| eth_archers | Ethiopian Archers | R | 50 | 11 | 0 | 3.9 | 12.0 | 1.4 | mark |
| eth_retainer | Noble Retainers | HC | 95 | 15 | 4 | 4.3 | 2.5 | 1.0 | inspire |
| eth_hill | Hill Troops | LI | 75 | 13 | 2 | 4.2 | 2.5 | 0.8 | ambush |
| eth_guards | Royal Guards | HI | 105 | 13 | 5 | 3.5 | 2.5 | 1.0 | parry |

### 37. Kanem
**ID:** `kanem` | Capital: `kan_njimi` | Color: (0.75, 0.45, 0.10) | Military: 23,000
Ruler: Mai Dunama | Cities: Njimi (8K, 0.518, 0.440, capital), Lake Chad Forts (4K, 0.520, 0.438), Caravan Nodes (3K, 0.515, 0.445)
Terrain: ST 26 / DE 22 / PL 14 / FO 8 / SW 18 / RV 8 / HI 4

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| kan_cavalry | Kanem Cavalry | HC | 95 | 15 | 4 | 4.5 | 2.5 | 1.0 | null |
| kan_spearman | Kanem Spearman | HI | 90 | 10 | 3 | 3.4 | 3.0 | 1.2 | formation_discipline |
| kan_archers | Kanem Archers | R | 50 | 10 | 0 | 3.8 | 12.0 | 1.4 | null |
| kan_guards | Royal Guards | HI | 100 | 12 | 5 | 3.5 | 2.5 | 1.0 | inspire |
| kan_tribal | Tribal Levies | LI | 65 | 9 | 1 | 4.0 | 2.5 | 1.0 | war_cry |

Skin for all Africa: (0.45, 0.30, 0.20). Leather/Bone/Wood materials. Wrapped helms. No capes.

---

# AMERICAS (5 Factions)

---

## 38. Toltec Sphere
**ID:** `toltec` | Capital: `tol_tula` | Color: (0.20, 0.60, 0.60) | Military: 33,000
Ruler: Ce Acatl Topiltzin | Trait: Warrior Orders — Elite units gain +15% damage after a kill | Asset: Atlantean Warrior Statues — +30% shock infantry strength at Tula
Cities: Tula (12K, 0.185, 0.480, capital), Cholula (5K, 0.186, 0.475), Xochicalco (3.5K, 0.184, 0.472)
Terrain: HI 22 / PL 16 / MT 16 / FO 10 / JN 8 / RV 8 / DE 12 / CO 8

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| tol_jaguar | Jaguar Warrior | LI | 80 | 20 | 3 | 4.2 | 2.5 | 0.6 | zealot_charge | Club | Light | Feathered | None | Cotton | Obsidian |
| tol_eagle | Eagle Warrior | LI | 75 | 15 | 2 | 4.5 | 5.0 | 1.1 | atlatl_volley | Atlatl | Light | Feathered | None | Cotton | Obsidian |
| tol_coyote | Coyote Scout | LI | 60 | 12 | 1 | 5.0 | 2.5 | 0.7 | skirmish | Club | None | None | None | Cotton | Wood |
| tol_macuahuitl | Macuahuitl Swordsman | HI | 95 | 14 | 4 | 3.4 | 2.5 | 1.0 | null | Sword | Medium | Feathered | Round | Cotton | Obsidian |
| tol_atlatl | Atlatl Thrower | R | 50 | 11 | 0 | 3.9 | 10.0 | 1.3 | atlatl_volley | Atlatl | None | None | None | Cotton | Wood |

Skin: (0.70, 0.50, 0.30). Obsidian/Cotton/Wood materials. Feathered helms. No metal.

---

## 39-42: Remaining Americas

### 39. Maya City-States
**ID:** `maya` | Capital: `may_chichen` | Color: (0.30, 0.55, 0.25) | Military: 52,000
Ruler: K'inich K'ak'mo | Cities: Chichén Itzá (14K, 0.175, 0.465, capital), Mayapán (7K, 0.174, 0.462), Uxmal (5K, 0.173, 0.463), Tikal (8K, 0.180, 0.455)
Terrain: JN 34 / FO 18 / SW 14 / CO 12 / PL 10 / HI 8 / RV 4

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| may_spearman | Maya Spearman | HI | 90 | 10 | 4 | 3.3 | 3.0 | 1.2 | monsoon_tactics |
| may_archers | Maya Archers | R | 50 | 10 | 0 | 3.8 | 11.0 | 1.4 | null |
| may_atlatl | Atlatl Skirmisher | R | 55 | 12 | 0 | 4.0 | 8.0 | 1.2 | atlatl_volley |
| may_noble | Noble Guards | HI | 105 | 14 | 5 | 3.5 | 2.5 | 1.0 | zealot_charge |
| may_assault | Assault Infantry | LI | 75 | 16 | 2 | 4.2 | 2.5 | 0.7 | war_cry |

### 40. Oaxaca States
**ID:** `oaxaca` | Capital: `oax_montealban` | Color: (0.65, 0.35, 0.20) | Military: 26,000
Ruler: Mixtec/Zapotec Council | Cities: Monte Albán (8K, 0.182, 0.470, capital), Mitla (5K, 0.183, 0.468), Mixtec Highlands (4K, 0.181, 0.472)
Terrain: MT 26 / HI 26 / FO 14 / PL 10 / JN 8 / RV 6 / CO 10

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| oax_spearman | Zapotec Spearman | HI | 90 | 10 | 4 | 3.3 | 3.0 | 1.2 | formation_discipline |
| oax_archers | Mixtec Archers | R | 50 | 10 | 0 | 3.8 | 11.0 | 1.4 | null |
| oax_skirmisher | Skirmisher | LI | 65 | 12 | 1 | 4.3 | 5.0 | 1.1 | skirmish |
| oax_elite | Elite Guards | HI | 100 | 14 | 5 | 3.5 | 2.5 | 1.0 | zealot_charge |
| oax_hill | Hill Troops | LI | 70 | 13 | 2 | 4.2 | 2.5 | 0.8 | ambush |

### 41. Tiwanaku Sphere
**ID:** `tiwanaku` | Capital: `tiw_tiwanaku` | Color: (0.50, 0.50, 0.45) | Military: 19,000
Ruler: Tiwanaku Council | Cities: Tiwanaku (7K, 0.240, 0.255, capital), Lake Titicaca (4K, 0.242, 0.258), Road Garrisons (2.5K, 0.238, 0.250)
Terrain: MT 36 / HI 28 / PL 8 / FO 2 / RV 6 / SW 10 / DE 10

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| tiw_spearman | Tiwanaku Spearman | HI | 90 | 10 | 4 | 3.3 | 3.0 | 1.2 | formation_discipline |
| tiw_slinger | Slingers | R | 45 | 9 | 0 | 3.8 | 11.0 | 1.5 | sling_barrage |
| tiw_clubman | Club Infantry | LI | 75 | 14 | 2 | 3.8 | 2.0 | 0.8 | war_cry |
| tiw_dart | Dart Troops | R | 50 | 10 | 0 | 4.0 | 8.0 | 1.2 | null |
| tiw_guards | Royal Guards | HI | 100 | 12 | 5 | 3.4 | 2.5 | 1.1 | inspire |

### 42. Wari Successor
**ID:** `wari` | Capital: `war_ayacucho` | Color: (0.60, 0.35, 0.25) | Military: 24,000
Ruler: Wari Confederation | Cities: Ayacucho (8K, 0.235, 0.265, capital), Highland Nodes (5K, 0.237, 0.270), Coastal Valleys (3.5K, 0.230, 0.260)
Terrain: MT 34 / HI 22 / DE 16 / PL 8 / RV 8 / FO 2 / CO 10

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability |
|----|------|-----|----|-----|-----|-----|-------|----|---------|
| war_spearman | Wari Spearman | HI | 90 | 10 | 4 | 3.3 | 3.0 | 1.2 | formation_discipline |
| war_slinger | Wari Slingers | R | 45 | 9 | 0 | 3.8 | 11.0 | 1.5 | sling_barrage |
| war_shock | Shock Infantry | LI | 80 | 16 | 2 | 4.0 | 2.5 | 0.7 | zealot_charge |
| war_garrison | Garrison Troops | HI | 105 | 9 | 6 | 2.8 | 2.5 | 1.4 | fortify |
| war_road | Road-Logistics Troops | LI | 70 | 10 | 2 | 4.5 | 2.5 | 1.0 | skirmish |

Skin for Americas: (0.70, 0.50, 0.30). Obsidian/Cotton/Wood/Bone materials. Feathered or no helm.

---

# OCEANIA (1 Faction)

---

## 43. Tu'i Tonga Empire
**ID:** `tui_tonga` | Capital: `ton_mua` | Color: (0.10, 0.45, 0.65) | Military: 12,000
Ruler: Tu'i Tonga | Trait: Double-Hulled Canoes — Ocean crossing without penalty | Asset: Lapaha Royal Tombs — +25% naval unit spawn
Cities: Mu'a (5K, 0.895, 0.195, capital), Samoa (3.5K, 0.910, 0.200), Fiji (2.5K, 0.885, 0.190)
Terrain: CO 50 / JN 20 / MT 15 / PL 10 / HI 5

| ID | Name | Cat | HP | ATK | DEF | SPD | Range | CD | Ability | W | A | H | Sh | AM | WM |
|----|------|-----|----|-----|-----|-----|-------|----|---------|---|---|---|----|----|-----|
| ton_marine | Canoe Marine | N | 75 | 13 | 2 | 4.2 | 2.5 | 0.9 | naval_boarding | Spear | None | Wrapped | None | Wood | Wood |
| ton_clubman | Club Warrior | LI | 80 | 16 | 1 | 4.0 | 2.0 | 0.7 | war_cry | Club | None | None | None | Bone | Wood |
| ton_slinger | Sling Thrower | R | 45 | 9 | 0 | 3.8 | 11.0 | 1.5 | sling_barrage | Sling | None | None | None | Bone | Wood |
| ton_shock | Tattooed Shock Troops | LI | 70 | 19 | 1 | 4.3 | 2.2 | 0.6 | zealot_charge | Club | None | None | None | Bone | Wood |
| ton_guard | Chiefly Guard | HI | 100 | 11 | 4 | 3.5 | 2.5 | 1.1 | inspire | Spear | Light | Wrapped | Round | Bone | Wood |

Skin: (0.55, 0.38, 0.25). Wood/Bone only. No metal. Minimal armor.

---

# COMPLETE ABILITY REFERENCE

| ID | Name | Trigger | Threshold | CD | Duration | DMG | SPD | ARM | IncomingDMG | Target Condition | Terrain Condition |
|----|------|---------|-----------|-----|----------|-----|-----|-----|-------------|------------------|-------------------|
| parry | Parry | OnCooldown | — | 8.0 | 1.5 | 1.0 | 1.0 | 0 | 0.4 | None | None |
| mark | Mark Target | OnCooldown | — | 6.0 | 5.0 | 1.0 | 1.0 | 0 | 1.4 | None | None |
| berserker_rage | Berserker Rage | OnLowHP | 0.4 | 0 | 6.0 | 1.6 | 1.3 | -6 | 1.0 | None | None |
| shield_wall | Shield Wall | OnNearEnemy | 10.0 | 0 | 0 | 1.0 | 0.3 | +15 | 1.0 | None | None |
| dual_strike | Dual Strike | OnAttack | 0.3 | 0 | 0 | 0.5 | 1.0 | 0 | 1.0 | None | None |
| shield_bash | Shield Bash | OnAttack | 0.25 | 3.0 | 0 | 1.0 | 1.0 | 0 | 1.0 | None | None |
| fear_aura | Fear Aura | Passive | 0.3 | 0 | 0 | 1.0 | 0.7 | 0 | 1.0 | None | None |
| elephant_charge | Elephant Charge | OnAttack | — | 10.0 | 2.0 | 2.0 | 1.5 | 0 | 1.0 | None | None |
| horse_archer_kite | Horse Archer Kite | OnNearEnemy | 6.0 | 0 | 0 | 1.0 | 1.4 | 0 | 1.0 | None | None |
| pike_brace | Pike Brace | Passive | — | 0 | 0 | 2.0 | 1.0 | 0 | 1.0 | HeavyCavalry,LightCavalry | None |
| volley_fire | Volley Fire | OnCooldown | — | 12.0 | 0 | 1.5 | 1.0 | 0 | 1.0 | None | None |
| fire_lance | Fire Lance | OnCooldown | — | 8.0 | 0 | 2.0 | 1.0 | 0 | 1.0 | None | None |
| naval_boarding | Naval Boarding | Passive | — | 0 | 0 | 1.3 | 1.0 | 0 | 1.0 | None | Coast |
| war_cry | War Cry | OnCooldown | — | 15.0 | 4.0 | 1.2 | 1.15 | 0 | 1.0 | None | None |
| feigned_retreat | Feigned Retreat | OnCooldown | — | 20.0 | 3.0 | 1.5 | 1.4 | -3 | 1.0 | None | None |
| fortify | Fortify | OnNearEnemy | 8.0 | 0 | 0 | 1.0 | 0.0 | +8 | 1.0 | None | None |
| skirmish | Skirmish | OnAttack | — | 4.0 | 1.5 | 1.0 | 1.5 | 0 | 1.0 | None | None |
| inspire | Inspire | OnCooldown | — | 20.0 | 6.0 | 1.2 | 1.0 | 0 | 1.0 | None | None |
| javelin_throw | Javelin Throw | OnCooldown | — | 8.0 | 0 | 1.8 | 1.0 | 0 | 1.0 | None | None |
| sling_barrage | Sling Barrage | OnCooldown | — | 10.0 | 0 | 1.3 | 1.0 | 0 | 1.0 | None | None |
| atlatl_volley | Atlatl Volley | OnCooldown | — | 8.0 | 0 | 1.5 | 1.0 | 0 | 1.0 | None | None |
| poison_arrow | Poison Arrow | OnAttack | 0.3 | 6.0 | 4.0 | 1.0 | 0.85 | 0 | 1.0 | None | None |
| zealot_charge | Zealot Charge | OnAttack | — | 12.0 | 0 | 2.0 | 1.3 | -2 | 1.0 | None | None |
| formation_discipline | Form. Discipline | Passive | — | 0 | 0 | 1.0 | 1.0 | +3 | 1.0 | AllyWithin5m | None |
| ambush | Ambush | OnAttack | — | 0 | 0 | 2.0 | 1.0 | 0 | 1.0 | None | Forest,Jungle |
| monsoon_tactics | Monsoon Tactics | Passive | — | 0 | 0 | 1.1 | 1.1 | +2 | 1.0 | None | Wetlands,Jungle |

**New fields added to AbilityDefinition:**
- `string targetCategoryCondition` — Comma-separated UnitCategory names. Null = applies to all. (e.g., "HeavyCavalry,LightCavalry")
- `string terrainCondition` — Comma-separated TerrainType names. Null = applies everywhere. (e.g., "Coast")
- `string specialCondition` — Free-form condition. (e.g., "AllyWithin5m", "FirstAttack")

---

# FACTION COLOR PALETTE (ALL 43)

| # | Faction | RGB |
|---|---------|-----|
| 1 | North Sea Empire | (0.15, 0.35, 0.70) |
| 2 | Norway | (0.20, 0.30, 0.60) |
| 3 | Sweden | (0.65, 0.60, 0.15) |
| 4 | Kievan Rus' | (0.70, 0.55, 0.15) |
| 5 | Poland | (0.80, 0.20, 0.20) |
| 6 | Hungary | (0.55, 0.75, 0.30) |
| 7 | Holy Roman Empire | (0.50, 0.50, 0.55) |
| 8 | France | (0.25, 0.25, 0.80) |
| 9 | Byzantine Empire | (0.55, 0.15, 0.60) |
| 10 | Christian Iberia | (0.75, 0.60, 0.20) |
| 11 | Córdoba | (0.15, 0.55, 0.30) |
| 12 | Fatimid | (0.10, 0.60, 0.25) |
| 13 | Abbasid | (0.10, 0.10, 0.10) |
| 14 | Buyid | (0.50, 0.20, 0.55) |
| 15 | Ghaznavid | (0.65, 0.15, 0.15) |
| 16 | Kara-Khanid | (0.20, 0.60, 0.65) |
| 17 | Khwarazm | (0.55, 0.40, 0.20) |
| 18 | Georgia | (0.60, 0.15, 0.30) |
| 19 | Armenia | (0.85, 0.45, 0.15) |
| 20 | Chola | (0.85, 0.65, 0.10) |
| 21 | Chalukya | (0.15, 0.25, 0.65) |
| 22 | Pala | (0.55, 0.10, 0.20) |
| 23 | Rajput | (0.80, 0.15, 0.15) |
| 24 | Song | (0.75, 0.20, 0.20) |
| 25 | Liao | (0.15, 0.20, 0.55) |
| 26 | Goryeo | (0.20, 0.55, 0.35) |
| 27 | Japan | (0.90, 0.90, 0.90) |
| 28 | Dali | (0.25, 0.50, 0.20) |
| 29 | Khmer | (0.75, 0.65, 0.15) |
| 30 | Srivijaya | (0.15, 0.55, 0.55) |
| 31 | Đại Cồ Việt | (0.80, 0.40, 0.10) |
| 32 | Champa | (0.20, 0.65, 0.70) |
| 33 | Pagan | (0.55, 0.40, 0.15) |
| 34 | Ghana | (0.70, 0.55, 0.10) |
| 35 | Makuria | (0.70, 0.60, 0.40) |
| 36 | Ethiopia | (0.15, 0.45, 0.15) |
| 37 | Kanem | (0.75, 0.45, 0.10) |
| 38 | Toltec | (0.20, 0.60, 0.60) |
| 39 | Maya | (0.30, 0.55, 0.25) |
| 40 | Oaxaca | (0.65, 0.35, 0.20) |
| 41 | Tiwanaku | (0.50, 0.50, 0.45) |
| 42 | Wari | (0.60, 0.35, 0.25) |
| 43 | Tu'i Tonga | (0.10, 0.45, 0.65) |

**Total: 43 factions, 216 units, ~155 cities — every value implementation-ready.**
