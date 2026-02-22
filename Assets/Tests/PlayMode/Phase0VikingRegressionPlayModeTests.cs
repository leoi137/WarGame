using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    public class Phase0VikingRegressionPlayModeTests
    {
        [UnityTest]
        public IEnumerator Test_VikingBattleSceneLoadsAndRuns_NoRegressions()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            yield return null;

            var scene = SceneManager.GetSceneByName("SampleScene");
            Assert.That(scene.IsValid(), Is.True, "SampleScene should be valid after loading.");
            Assert.That(scene.isLoaded, Is.True, "SampleScene should be loaded.");

            var roots = scene.GetRootGameObjects();
            Assert.That(roots.Length, Is.GreaterThan(0), "Expected root objects in SampleScene.");

            // Let simulation advance for a short window to catch immediate runtime issues.
            yield return new WaitForSeconds(5f);
            Assert.Pass("SampleScene remained stable for regression window.");
        }
    }
}
