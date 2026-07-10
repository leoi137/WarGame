using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    [TestFixture]
    public class UINavigationTests
    {
        [UnityTest]
        public IEnumerator Test_MainMenuToWorldMapTransition()
        {
            Assert.IsNotNull(typeof(MainMenuUI).GetMethod("OnWorldMap"));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_MainMenuToQuickBattleTransition()
        {
            Assert.IsNotNull(typeof(MainMenuUI).GetMethod("OnQuickBattle"));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_FactionSelectToSetupTransition()
        {
            Assert.IsNotNull(typeof(FactionSelectUI).GetMethod("OnFactionSelected"));
            yield return null;
        }
    }
}
