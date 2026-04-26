using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class ItemPickupCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly ItemLogic _item;

        private const int HEAL_AMOUNT = 30;

        public ItemPickupCommand(PlayerLogic player, ItemLogic item)
        {
            _player = player;
            _item = item;
        }

        public void Execute()
        {
            switch (_item.ItemType)
            {
                case eItemType.WeaponUpgrade:
                    _player.UpgradeWeapon();
                    break;
                case eItemType.HpRecover:
                    _player.Heal(HEAL_AMOUNT);
                    break;
            }
            _item.Pickup();
        }
    }
}
