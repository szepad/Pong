namespace Pong.Core
{
    public class Animation(int frameCount, int frameWidth, int frameHeight, int y, float frameTime, bool isLooped = true)
    {
        public int FrameCount { get; set; } = frameCount;
        public int FrameWidth { get; set; } = frameWidth;
        public int FrameHeight { get; set; } = frameHeight;
        public int Y { get; set; } = y;
        public float FrameTime { get; set; } = frameTime;
        public bool IsLooped { get; set; } = isLooped;
    }
}
