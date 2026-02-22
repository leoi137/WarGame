using NUnit.Framework;
using System.IO;

namespace WorldWars.Tests.EditMode
{
    public class Phase0RestructureTests
    {
        private const string Root = "/home/leandro/Desktop/Resources/OtherApps/WorldWars";

        [Test]
        public void Test_ProjectCompilesAfterRestructure()
        {
            // This test is a placeholder assertion that runs only after compilation succeeds.
            // If scripts fail to compile, Unity will not execute this test suite.
            Assert.Pass("Project compiled; EditMode tests are running.");
        }

        [Test]
        public void Test_EditModeAssemblyExists()
        {
            var path = Path.Combine(Root, "Assets/Tests/EditMode/EditModeTests.asmdef");
            Assert.That(File.Exists(path), Is.True, $"Missing asmdef at: {path}");

            var text = File.ReadAllText(path);
            Assert.That(text.Contains("\"name\": \"EditModeTests\""), Is.True);
        }

        [Test]
        public void Test_PlayModeAssemblyExists()
        {
            var path = Path.Combine(Root, "Assets/Tests/PlayMode/PlayModeTests.asmdef");
            Assert.That(File.Exists(path), Is.True, $"Missing asmdef at: {path}");

            var text = File.ReadAllText(path);
            Assert.That(text.Contains("\"name\": \"PlayModeTests\""), Is.True);
        }
    }
}
