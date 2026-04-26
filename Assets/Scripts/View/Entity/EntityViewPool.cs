using System.Collections.Generic;
using UnityEngine;
using GaviShooting.Core;

namespace GaviShooting.View.Entity
{
    public class EntityViewPool<TView> where TView : MonoBehaviour, IEntityView
    {
        private readonly TView _prefab;
        private readonly Transform _parent;
        private readonly Stack<TView> _pool = new();

        public EntityViewPool(TView prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public TView Get()
        {
            var view = _pool.Count > 0 ? _pool.Pop() : Object.Instantiate(_prefab, _parent);
            view.gameObject.SetActive(true);
            Log.Info("EntityViewPool.Get type={0} poolRemain={1}", typeof(TView).Name, _pool.Count);
            return view;
        }

        public void Return(TView view)
        {
            view.Deactivate();
            _pool.Push(view);
            Log.Info("EntityViewPool.Return type={0} poolCount={1}", typeof(TView).Name, _pool.Count);
        }
    }
}
