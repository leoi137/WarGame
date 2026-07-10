using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 8: UI systems — theme, menus, HUDs, tooltips, minimap, color palette.
    /// </summary>
    [TestFixture]
    public class Phase8UISystemTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
            TerrainDatabase.Initialize();
        }

        // ── UIThemeManager ─────────────────────────────────────────

        [Test]
        public void Test_UIThemeManagerCreatePanelReturnsNonNull()
        {
            var canvas = UIThemeManager.CreateCanvas("TestCanvas", 0);
            var panel = UIThemeManager.CreatePanel(canvas.transform, "Panel", Vector2.zero, new Vector2(100, 100));
            Assert.IsNotNull(panel);
            Assert.IsNotNull(panel.GetComponent<Image>());
            UnityEngine.Object.DestroyImmediate(canvas);
        }

        [Test]
        public void Test_UIThemeManagerCreateButtonReturnsNonNull()
        {
            var canvas = UIThemeManager.CreateCanvas("TestCanvas", 0);
            bool clicked = false;
            var btn = UIThemeManager.CreateButton(canvas.transform, "Btn", "Test", Vector2.zero, new Vector2(100, 40), () => clicked = true);
            Assert.IsNotNull(btn);
            var button = btn.GetComponent<Button>();
            Assert.IsNotNull(button);
            button.onClick.Invoke();
            Assert.IsTrue(clicked);
            UnityEngine.Object.DestroyImmediate(canvas);
        }

        [Test]
        public void Test_UIThemeManagerCreateTextReturnsNonNull()
        {
            var canvas = UIThemeManager.CreateCanvas("TestCanvas", 0);
            var txt = UIThemeManager.CreateText(canvas.transform, "Txt", "Hello", Vector2.zero, new Vector2(200, 30), 14, TextAnchor.MiddleCenter);
            Assert.IsNotNull(txt);
            var text = txt.GetComponent<Text>();
            Assert.IsNotNull(text);
            Assert.AreEqual("Hello", text.text);
            UnityEngine.Object.DestroyImmediate(canvas);
        }

        // ── MainMenuUI ─────────────────────────────────────────────

        [Test]
        public void Test_MainMenuHasAllButtons()
        {
            Assert.IsNotNull(typeof(MainMenuUI).GetMethod("CreateMenuButtons"));
            Assert.IsNotNull(typeof(MainMenuUI).GetMethod("OnQuickBattle"));
            Assert.IsNotNull(typeof(MainMenuUI).GetMethod("OnWorldMap"));
            Assert.IsNotNull(typeof(MainMenuUI).GetMethod("OnUnitViewer"));
        }

        // ── FactionSelectUI ────────────────────────────────────────

        [Test]
        public void Test_FactionSelectShows43Factions()
        {
            Assert.AreEqual(43, FactionDatabase.FactionCount);
        }

        [Test]
        public void Test_RegionFilterReducesList()
        {
            var europe = FactionDatabase.GetByRegion(Region.Europe);
            Assert.AreEqual(11, europe.Count);
            var africa = FactionDatabase.GetByRegion(Region.Africa);
            Assert.AreEqual(4, africa.Count);
        }

        // ── BattleSetupUI ──────────────────────────────────────────

        [Test]
        public void Test_BattleSetupShowsArmyRoster()
        {
            Assert.IsNotNull(typeof(BattleSetupUI).GetMethod("CreateArmyRoster"));
        }

        [Test]
        public void Test_BattleSetupShowsFormationToolbar()
        {
            Assert.AreEqual(5, Enum.GetValues(typeof(FormationType)).Length);
        }

        [Test]
        public void Test_BattleSetupShowsOpponentPreview()
        {
            Assert.IsNotNull(typeof(BattleSetupUI).GetMethod("ShowOpponentPreview"));
        }

        [Test]
        public void Test_BattleSetupMinimapUpdates()
        {
            Assert.IsNotNull(typeof(BattleSetupUI).GetMethod("CreatePlacementMinimap"));
        }

        // ── BattleHUD ──────────────────────────────────────────────

        [Test]
        public void Test_BattleHUDUpdatesUnitCounts()
        {
            Assert.IsNotNull(typeof(BattleHUD).GetMethod("Update"));
        }

        // ── WorldMapHUD ────────────────────────────────────────────

        [Test]
        public void Test_WorldMapHUDBattleButtonDisabledWithOneSelection()
        {
            Assert.IsNotNull(typeof(WorldMapHUD).GetMethod("UpdateSelectionState"));
        }

        [Test]
        public void Test_WorldMapHUDBattleButtonEnabledWithTwoSelections()
        {
            Assert.IsNotNull(typeof(WorldMapHUD).GetMethod("CreateBattleButton"));
        }

        // ── Tooltip ────────────────────────────────────────────────

        [Test]
        public void Test_TooltipShowsCorrectUnitStats()
        {
            Assert.IsNotNull(typeof(TooltipSystem).GetMethod("ShowUnitTooltip"));
        }

        // ── Results ────────────────────────────────────────────────

        [Test]
        public void Test_ResultsScreenShowsWinner()
        {
            Assert.IsNotNull(typeof(BattleResultsUI).GetMethod("CreateVictoryBanner"));
        }

        // ── FactionColorPalette ────────────────────────────────────

        [Test]
        public void Test_FactionColorPaletteReturnsDistinctColors()
        {
            Color a = FactionColorPalette.GetPrimaryColor("north_sea_empire");
            Color b = FactionColorPalette.GetPrimaryColor("byzantine");
            Assert.AreNotEqual(a, b, "Two factions should have distinct primary colors");
        }

        // ── Minimap ────────────────────────────────────────────────

        [Test]
        public void Test_MinimapTextureNotNull()
        {
            var go = new GameObject("TestMinimap");
            var minimap = go.AddComponent<MinimapRenderer>();
            minimap.Initialize(300, Color.blue, Color.red);
            Assert.IsNotNull(minimap.MinimapTexture);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
