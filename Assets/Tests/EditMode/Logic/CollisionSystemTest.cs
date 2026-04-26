using NUnit.Framework;
using GaviShooting.Logic.Collision;

namespace Tests.EditMode.Logic
{
    public class CollisionSystemTest
    {
        [Test]
        public void HitBox_Overlapping_ReturnsTrue()
        {
            var a = new HitBox(0f, 0f, 1f);
            var b = new HitBox(1.5f, 0f, 1f);
            Assert.IsTrue(a.Overlaps(b));
        }

        [Test]
        public void HitBox_NotOverlapping_ReturnsFalse()
        {
            var a = new HitBox(0f, 0f, 1f);
            var b = new HitBox(3f, 0f, 1f);
            Assert.IsFalse(a.Overlaps(b));
        }

        [Test]
        public void HitBox_Touching_ReturnsTrue()
        {
            var a = new HitBox(0f, 0f, 1f);
            var b = new HitBox(2f, 0f, 1f);
            Assert.IsTrue(a.Overlaps(b));
        }
    }
}
