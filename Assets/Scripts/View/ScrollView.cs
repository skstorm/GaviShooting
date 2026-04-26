using UnityEngine;
using GaviShooting.Core;

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
            Log.Info("ScrollView.Init speed={0}", speed);
        }

        public void SetScrolling(bool scrolling)
        {
            _scrolling = scrolling;
            Log.Info("ScrollView.SetScrolling {0}", scrolling);
        }

        public void Render()
        {
            if (!_scrolling || _background == null) return;
            _background.position += Vector3.left * _speed * Time.deltaTime;
        }
    }
}
