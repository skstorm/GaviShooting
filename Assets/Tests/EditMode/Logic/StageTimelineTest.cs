using System.Collections.Generic;
using NUnit.Framework;
using GaviShooting.Logic.Entity;
using GaviShooting.Logic.Stage;

namespace Tests.EditMode.Logic
{
    public class StageTimelineTest
    {
        private StageData createTestData()
        {
            return new StageData
            {
                StageName = "Test",
                TotalFrames = 120,
                ScrollSpeed = 2f,
                BossFrame = 100,
                SpawnEvents = new List<SpawnEvent>
                {
                    new() { Frame = 10, EnemyType = eEnemyType.Straight, X = 10f, Y = 3f },
                    new() { Frame = 10, EnemyType = eEnemyType.Straight, X = 10f, Y = -3f },
                    new() { Frame = 50, EnemyType = eEnemyType.Wave, X = 10f, Y = 0f },
                }
            };
        }

        [Test]
        public void Advance_ReturnsSpawnsAtFrame()
        {
            var timeline = new StageTimeline(createTestData());
            var spawns = timeline.Advance();
            Assert.AreEqual(0, spawns.Count);

            for (int i = 0; i < 9; i++) timeline.Advance();
            spawns = timeline.Advance();
            Assert.AreEqual(2, spawns.Count);
        }

        [Test]
        public void IsBossTime_TrueAtBossFrame()
        {
            var timeline = new StageTimeline(createTestData());
            for (int i = 0; i < 100; i++) timeline.Advance();
            Assert.IsTrue(timeline.IsBossTime);
        }

        [Test]
        public void IsFinished_TrueAtTotalFrames()
        {
            var timeline = new StageTimeline(createTestData());
            for (int i = 0; i < 120; i++) timeline.Advance();
            Assert.IsTrue(timeline.IsFinished);
        }
    }
}
