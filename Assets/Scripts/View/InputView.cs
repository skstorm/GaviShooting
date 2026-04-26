using UnityEngine;
using UnityEngine.InputSystem;
using GaviShooting.Core;
using GaviShooting.Logic.Command;
using GaviShooting.Logic.Entity;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.View
{
    public class InputView : MonoBehaviour
    {
        private ICommandQueue _commandQueue;
        private PlayerLogic _player;
        private EntityManager _entityManager;
        private StateMachine _fsm;
        private IBulletViewWriter _bulletViewWriter;
        private System.Action _onRestart;
        private int _shootCooldown;

        private const int SHOOT_INTERVAL = 6;

        public void Init(ICommandQueue commandQueue, PlayerLogic player, EntityManager entityManager, StateMachine fsm, IBulletViewWriter bulletViewWriter, System.Action onRestart = null)
        {
            _commandQueue = commandQueue;
            _player = player;
            _entityManager = entityManager;
            _fsm = fsm;
            _bulletViewWriter = bulletViewWriter;
            _onRestart = onRestart;
            Log.Info("InputView.Init");
        }

        private void Update()
        {
            if (_commandQueue == null) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (_fsm.CurrentState == eGameState.Ready)
            {
                if (keyboard.spaceKey.wasPressedThisFrame)
                    _commandQueue.Enqueue(new GameStartCommand(_fsm));
                return;
            }

            if (_fsm.CurrentState == eGameState.GameOver || _fsm.CurrentState == eGameState.StageClear)
            {
                if (keyboard.spaceKey.wasPressedThisFrame)
                    _onRestart?.Invoke();
                return;
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
                _commandQueue.Enqueue(new PauseCommand(_fsm));

            if (_fsm.CurrentState != eGameState.Playing) return;

            float dx = 0f, dy = 0f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) dy = 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) dy = -1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) dx = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) dx = 1f;

            _commandQueue.Enqueue(new MoveCommand(_player, dx, dy));

            _shootCooldown--;
            if (keyboard.spaceKey.isPressed && _shootCooldown <= 0)
            {
                _commandQueue.Enqueue(new ShootCommand(_player, _entityManager, _bulletViewWriter));
                _shootCooldown = SHOOT_INTERVAL;
            }
        }
    }
}
