namespace Pong.Core
{
    public static class Text
    {
        private static Font _font = Raylib.GetFontDefault();
        public static void UseFont(Font font)
        {
            _font = font;
        }

        public static void Draw(string text, Vector2 pos, int fontSize, Color color, Pivot pivot = Pivot.TopLeft, int spacing = -1)
        {
            if (spacing < 0)
            {
                spacing = fontSize / 10;
            }

            Vector2 size = Raylib.MeasureTextEx(_font, text, fontSize, spacing);

            // X-axis
            if (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom) pos.X -= size.X / 2;
            else if (pivot == Pivot.TopRight || pivot == Pivot.Right || pivot == Pivot.BottomRight) pos.X -= size.X;

            // Y-axis
            if (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right) pos.Y -= size.Y / 2;
            else if (pivot == Pivot.BottomLeft || pivot == Pivot.Bottom || pivot == Pivot.BottomRight) pos.Y -= size.Y;

            Raylib.DrawTextPro(_font, text, pos, Vector2.Zero, 0f, fontSize, spacing, color);
        }

        public static void Draw(string text, float x, float y, int fontSize, Color color, Pivot pivot = Pivot.TopLeft, int spacing = -1)
        {
            Draw(text, new Vector2(x, y), fontSize, color, pivot, spacing);
        }
    }
}
