namespace Pong.Core
{
    public static class Helper
    {
        public static readonly Random Rng = new();

        public static int Rand(int min, int max)
        {
            return Rng.Next(min, max);
        }

        public static float RandF(float min, float max)
        {
            return Rng.NextSingle() * (max - min) + min;
        }

        public static float Clamp01(float value)
        {
            return Raymath.Clamp(value, 0f, 1f);
        }
    }
}
