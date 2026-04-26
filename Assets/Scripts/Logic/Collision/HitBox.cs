namespace GaviShooting.Logic.Collision
{
    public struct HitBox
    {
        public float X;
        public float Y;
        public float Radius;

        public HitBox(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public bool Overlaps(in HitBox other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            float sumR = Radius + other.Radius;
            return dx * dx + dy * dy <= sumR * sumR;
        }
    }
}
