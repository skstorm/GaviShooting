using NUnit.Framework;
using GaviShooting.Core;

namespace Tests.EditMode.Core
{
    public class FrameManagerTest
    {
        [Test]
        public void CalcDeltaFrame_FirstCall_ReturnsOne()
        {
            var fm = new FrameManager(60);
            int delta = fm.CalcDeltaFrame(1f / 60f);
            Assert.AreEqual(1, delta);
        }

        [Test]
        public void CalcDeltaFrame_HalfFrame_ReturnsZero()
        {
            var fm = new FrameManager(60);
            int delta = fm.CalcDeltaFrame(1f / 120f);
            Assert.AreEqual(0, delta);
        }

        [Test]
        public void CalcDeltaFrame_AccumulatesTwoHalves_ReturnsOne()
        {
            var fm = new FrameManager(60);
            fm.CalcDeltaFrame(1f / 120f);
            int delta = fm.CalcDeltaFrame(1f / 120f);
            Assert.AreEqual(1, delta);
        }

        [Test]
        public void CalcDeltaFrame_LargeStep_ReturnsMultiple()
        {
            var fm = new FrameManager(60);
            int delta = fm.CalcDeltaFrame(3f / 60f);
            Assert.AreEqual(3, delta);
        }
    }
}
