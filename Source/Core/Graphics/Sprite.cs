namespace Pong.Core
{
    public class Sprite
    {
        public Texture2D SpriteSheet { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; } = Color.White;
        public float Scale { get; set; } = 1f;
        public float Rotation { get; set; }
        public bool IsLocked { get; set; }
        public bool FlipX { get; set; }

        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationName { get; private set; }

        private readonly Dictionary<string, Animation> _animations = [];
        private int _frame;
        private float _timer;

        public Sprite(Texture2D spriteSheet)
        {
            SpriteSheet = spriteSheet;
        }

        public void Add(string name, Animation animation)
        {
            _animations[name] = animation;

            if (_animations.Count == 1)
            {
                Play(name);
            }
        }

        public void Play(string name)
        {
            if (IsLocked || !_animations.ContainsKey(name) || CurrentAnimationName == name)
                return;

            CurrentAnimationName = name;
            CurrentAnimation = _animations[name];

            _frame = 0;
            _timer = CurrentAnimation.FrameTime;
        }

        public void Update(float dt)
        {
            _timer -= dt;
            if (_timer <= 0f)
            {
                _timer = CurrentAnimation.FrameTime;
                _frame++;

                if (_frame >= CurrentAnimation.FrameCount)
                {
                    if (CurrentAnimation.IsLooped) _frame = 0;
                    else _frame--;
                }
            }
        }

        public void Draw(float x, float y)
        {
            Draw(new Vector2(x, y));
        }

        public void Draw(Vector2 position)
        {
            var size = new Vector2(CurrentAnimation.FrameWidth, CurrentAnimation.FrameHeight);

            Raylib.DrawTexturePro(
                SpriteSheet, GetSourceRect(),
                new Rectangle(position, size * Scale),
                Origin, Rotation, Color);
        }

        public Rectangle GetSourceRect()
        {
            return new(CurrentAnimation.FrameWidth * _frame, CurrentAnimation.Y,
                CurrentAnimation.FrameWidth * (FlipX ? -1f : 1f), CurrentAnimation.FrameHeight);
        }
    }
}
