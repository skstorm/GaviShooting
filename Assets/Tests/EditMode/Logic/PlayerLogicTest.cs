using NUnit.Framework;
using GaviShooting.Logic.Entity;

namespace Tests.EditMode.Logic
{
    public class PlayerLogicTest
    {
        private class NullPlayerViewWriter : IPlayerViewWriter
        {
            public void PlayDamageEffect() { }
            public void PlayDeathEffect() { }
        }

        [Test]
        public void InitialHp_Is100()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            Assert.AreEqual(100, p.Hp);
        }

        [Test]
        public void TakeDamage_ReducesHp()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.TakeDamage(20);
            Assert.AreEqual(80, p.Hp);
        }

        [Test]
        public void TakeDamage_ToZero_IsNotAlive()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.TakeDamage(100);
            Assert.IsFalse(p.IsAlive);
        }

        [Test]
        public void SetDirection_Move_UpdatesPosition()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.SetDirection(1f, 0f);
            p.Move();
            Assert.Greater(p.X, 0f);
        }

        [Test]
        public void UpgradeWeapon_MaxLevel3()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.UpgradeWeapon();
            p.UpgradeWeapon();
            p.UpgradeWeapon();
            Assert.AreEqual(3, p.WeaponLevel);
        }

        [Test]
        public void Heal_ClampsToMaxHp()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.TakeDamage(10);
            p.Heal(50);
            Assert.AreEqual(100, p.Hp);
        }
    }
}
