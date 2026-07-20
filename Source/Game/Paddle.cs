namespace Pong
{
    public class Paddle
    {
        public Vector2 Position;
        public Vector2 Size;
        public Color Color = Color.White;

        public Rectangle Rect => new(Position, Size);

        public const float Speed = 800f;

        public Paddle(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public void Update(float dt)
        {
            if (Position.Y < 0) Position.Y = 0;
            else if (Position.Y + Size.Y > Game.Height) Position.Y = Game.Height - Size.Y;
        }

        public void Draw()
        {
            //Raylib.DrawRectangleV(Position, Size, Color);
            //Raylib.DrawTextureV(Assets.Tex["paddle"], Position, Color);

            var tex = Assets.Tex["paddle"];
            Raylib.DrawTexturePro(tex, new Rectangle(0, 0, tex.Width, tex.Height),
                new Rectangle(Position, Size), Vector2.Zero, 0f, Color);
        }
    }
}
