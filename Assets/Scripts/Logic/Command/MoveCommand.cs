using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class MoveCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly float _dx;
        private readonly float _dy;

        public MoveCommand(PlayerLogic player, float dx, float dy)
        {
            _player = player;
            _dx = dx;
            _dy = dy;
        }

        public void Execute()
        {
            _player.SetDirection(_dx, _dy);
        }
    }
}
