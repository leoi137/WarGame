using NUnit.Framework;
using System.IO;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 0: Verify project structure is intact after directory restructure.
    /// </summary>
    public class Phase0RestructureTests
    {
        private static readonly string Root = Path.GetFullPath(
            Path.Combine(UnityEngine.Application.dataPath, ".."));

        [Test]
        public void Test_ProjectCompilesAfterRestructure()
        {
            Assert.Pass("Project compiled; EditMode tests are running.");
        }

        [Test]
        public void Test_CoreDirectoryStructureExists()
        {
            string[] requiredDirs =
            {
                "Assets/Scripts/Core",
                "Assets/Scripts/Data/Models",
                "Assets/Scripts/Data/Databases",
                "Assets/Scripts/Data/Factions",
                "Assets/Tests/EditMode",
                "Assets/Tests/PlayMode"
            };

            foreach (string dir in requiredDirs)
            {
                string full = Path.Combine(Root, dir);
                Assert.That(Directory.Exists(full), Is.True, $"Missing directory: {dir}");
            }
        }

        [Test]
        public void Test_CoreScriptsExist()
        {
            string[] requiredFiles =
            {
                "Assets/Scripts/Core/Enums.cs",
                "Assets/Scripts/Core/EventBus.cs",
                "Assets/Scripts/Core/GameConfig.cs",
                "Assets/Scripts/Core/GameManager.cs",
                "Assets/Scripts/Core/BattleRandom.cs"
            };

            foreach (string file in requiredFiles)
            {
                string full = Path.Combine(Root, file);
                Assert.That(File.Exists(full), Is.True, $"Missing file: {file}");
            }
        }
    }
}
