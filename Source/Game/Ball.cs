namespace Pong
{
    public class Ball
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Radius { get; set; } = 10;

        private const float StartSpeed = 850f;
        private const float MaxSpeed = 2000f;
        private const float IncrementSpeedPerHit = 100f;

        private float _currentSpeed = StartSpeed;

        public Ball(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public void Update(float dt)
        {
            Position += Direction * _currentSpeed * dt;
            HandleBorderCollisions();
        }

        public bool HandlePaddleCollision(Paddle paddle)
        {
            if (Raylib.CheckCollisionCircleRec(Position, Radius, paddle.Rect))
            {
                Direction.Y = Raymath.Clamp(((Position.Y - paddle.Position.Y) / paddle.Size.Y * 2f) - 1f, -1f, 1f);

                if (Position.X > Game.Width / 2f)
                {
                    Position.X = paddle.Position.X - Radius;
                    Direction.X = -MathF.Abs(Direction.X);
                }
                else
                {
                    Position.X = paddle.Position.X + paddle.Size.X + Radius;
                    Direction.X = MathF.Abs(Direction.X);
                }

                Direction = Raymath.Vector2Normalize(Direction);

                _currentSpeed += IncrementSpeedPerHit;
                if (_currentSpeed > MaxSpeed)
                {
                    _currentSpeed = MaxSpeed;
                }

                Game.Instance.PlaySound("paddle_hit");

                return true;
            }

            return false;
        }

        private void HandleBorderCollisions()
        {
            if (Position.Y - Radius < 0f)
            {
                Position.Y = Radius;
                Direction.Y = MathF.Abs(Direction.Y);
                Game.Instance.PlaySound("wall_hit");
            }
            else if (Position.Y + Radius > Game.Height)
            {
                Position.Y = Game.Height - Radius;
                Direction.Y = -MathF.Abs(Direction.Y);
                Game.Instance.PlaySound("wall_hit");
            }
        }

        public void Draw()
        {
            Raylib.DrawCircleV(Position, Radius, Color.White);
        }
    }
}
