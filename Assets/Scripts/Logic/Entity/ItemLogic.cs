using GaviShooting.Core;
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public class ItemLogic : IEntity, IItemReadOnly
    {
        private const float MOVE_SPEED = 1.5f / 60f;
        private const float HIT_RADIUS = 0.4f;
        private const float SCREEN_LEFT = -12f;

        private readonly int _id;
        private readonly eItemType _itemType;
        private readonly IItemViewWriter _viewWriter;
        private float _x;
        private float _y;
        private bool _dead;

        public ItemLogic(int id, eItemType itemType, float x, float y, IItemViewWriter viewWriter)
        {
            _id = id;
            _itemType = itemType;
            _x = x;
            _y = y;
            _viewWriter = viewWriter;
            Log.Info("ItemLogic created id={0} type={1} pos=({2},{3})", id, itemType, x, y);
        }

        public int Id => _id;
        public float X => _x;
        public float Y => _y;
        public eItemType ItemType => _itemType;
        public bool IsAlive => !_dead;
        public bool ShouldRemove => _dead;
        public HitBox HitBox => new(_x, _y, HIT_RADIUS);
        public eCollisionLayer Layer => eCollisionLayer.Item;

        public void Move()
        {
            _x -= MOVE_SPEED;
        }

        public void Update()
        {
            if (_x < SCREEN_LEFT) _dead = true;
        }

        public void Pickup()
        {
            _dead = true;
            Log.Info("ItemLogic Pickup id={0} type={1}", _id, _itemType);
            _viewWriter.PlayPickupEffect();
        }
    }
}
