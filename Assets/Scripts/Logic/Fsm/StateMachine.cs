using System.Collections.Generic;
using GaviShooting.Core;

namespace GaviShooting.Logic.Fsm
{
    public class StateMachine
    {
        private readonly Dictionary<eGameState, IState> _states = new();
        private IState _current;
        private eGameState _currentKey;

        public eGameState CurrentState => _currentKey;

        public void AddState(eGameState key, IState state)
        {
            _states[key] = state;
            Log.Info("StateMachine.AddState {0}", key);
        }

        public void ChangeState(eGameState key)
        {
            Log.Info("StateMachine.ChangeState {0} → {1}", _currentKey, key);
            _current?.Exit();
            _currentKey = key;
            _current = _states[key];
            _current.Enter();
        }

        public void LogicUpdate()
        {
            _current?.LogicUpdate();
        }
    }
}
