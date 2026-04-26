## Task 16: View — InputView + ScrollView + UiView

**Files:**
- Create: `Assets/Scripts/View/InputView.cs`
- Create: `Assets/Scripts/View/ScrollView.cs`
- Create: `Assets/Scripts/View/UiView.cs`

- [ ] **Step 1: InputView**

Create `Assets/Scripts/View/InputView.cs`:
```csharp
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
        private int _shootCooldown;

        private const int SHOOT_INTERVAL = 6;

        public void Init(ICommandQueue commandQueue, PlayerLogic player, EntityManager entityManager, StateMachine fsm, IBulletViewWriter bulletViewWriter)
        {
            _commandQueue = commandQueue;
            _player = player;
            _entityManager = entityManager;
            _fsm = fsm;
            _bulletViewWriter = bulletViewWriter;
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
                    _commandQueue.Enqueue(new GameStartCommand(_fsm));
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
```

- [ ] **Step 2: ScrollView**

Create `Assets/Scripts/View/ScrollView.cs`:
```csharp
using UnityEngine;

namespace GaviShooting.View
{
    public class ScrollView : MonoBehaviour
    {
        [SerializeField] private Transform _background;
        private float _speed;
        private bool _scrolling;

        public void Init(float speed)
        {
            _speed = speed;
        }

        public void SetScrolling(bool scrolling)
        {
            _scrolling = scrolling;
        }

        public void Render()
        {
            if (!_scrolling || _background == null) return;
            _background.position += Vector3.left * _speed * Time.deltaTime;
        }
    }
}
```

- [ ] **Step 3: UiView**

Create `Assets/Scripts/View/UiView.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;
using GaviShooting.Logic.Entity;

namespace GaviShooting.View
{
    public class UiView : MonoBehaviour
    {
        [SerializeField] private Image _hpBar;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _weaponLevelText;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _stageClearPanel;
        [SerializeField] private GameObject _pausePanel;

        public void UpdateHp(int current, int max)
        {
            if (_hpBar == null) return;
            float ratio = (float)current / max;
            _hpBar.fillAmount = ratio;
            _hpBar.color = ratio > 0.5f ? Color.green : ratio > 0.25f ? Color.yellow : Color.red;
        }

        public void UpdateScore(int score)
        {
            if (_scoreText != null) _scoreText.text = $"SCORE: {score}";
        }

        public void UpdateWeaponLevel(int level)
        {
            if (_weaponLevelText != null) _weaponLevelText.text = $"WEAPON LV.{level}";
        }

        public void ShowReady() { setPanel(_readyPanel); }
        public void ShowGameOver() { setPanel(_gameOverPanel); }
        public void ShowStageClear() { setPanel(_stageClearPanel); }
        public void ShowPause() { setPanel(_pausePanel); }
        public void HideAll() { setPanel(null); }

        private void setPanel(GameObject target)
        {
            if (_readyPanel != null) _readyPanel.SetActive(_readyPanel == target);
            if (_gameOverPanel != null) _gameOverPanel.SetActive(_gameOverPanel == target);
            if (_stageClearPanel != null) _stageClearPanel.SetActive(_stageClearPanel == target);
            if (_pausePanel != null) _pausePanel.SetActive(_pausePanel == target);
        }
    }
}
```

- [ ] **Step 4: 커밋**
```bash
git add Assets/Scripts/View/
git commit -m "add: InputView/ScrollView/UiView — 입력, 스크롤, UI"
```
