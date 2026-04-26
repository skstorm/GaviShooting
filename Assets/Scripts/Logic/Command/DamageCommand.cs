using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class DamageCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly int _amount;

        public DamageCommand(PlayerLogic player, int amount)
        {
            _player = player;
            _amount = amount;
        }

        public void Execute()
        {
            _player.TakeDamage(_amount);
        }
    }
}
