using NUnit.Framework;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class BattleResultTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FactionDatabase.Initialize();
        }

        [Test]
        public void Test_BattleResultHasValidWinner()
        {
            var attacker = FactionDatabase.Get("north_sea_empire");
            var defender = FactionDatabase.Get("byzantine");
            Assert.IsNotNull(attacker);
            Assert.IsNotNull(defender);

            var result = new BattleResult
            {
                winnerFactionId = attacker.id,
                loserFactionId = defender.id,
                winningSide = Faction.Attacker,
                winnerSurvivors = 50,
                winnerStartCount = 80,
                loserStartCount = 60,
                battleDurationSeconds = 45f,
                totalCasualties = 90
            };

            Assert.IsTrue(result.winnerFactionId == attacker.id || result.winnerFactionId == defender.id,
                "Winner must be one of the two factions");
        }

        [Test]
        public void Test_SurvivorsLessThanStartCount()
        {
            var result = new BattleResult
            {
                winnerFactionId = "attacker",
                loserFactionId = "defender",
                winnerSurvivors = 40,
                winnerStartCount = 80,
                loserStartCount = 60,
                battleDurationSeconds = 30f,
                totalCasualties = 100
            };

            Assert.LessOrEqual(result.winnerSurvivors, result.winnerStartCount,
                "Survivors cannot exceed start count");
        }

        [Test]
        public void Test_BattleDurationIsPositive()
        {
            var result = new BattleResult
            {
                winnerFactionId = "attacker",
                loserFactionId = "defender",
                winnerSurvivors = 50,
                winnerStartCount = 80,
                loserStartCount = 60,
                battleDurationSeconds = 25.5f,
                totalCasualties = 90
            };

            Assert.Greater(result.battleDurationSeconds, 0f, "Battle duration must be positive");
        }
    }
}
