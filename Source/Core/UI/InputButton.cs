using System.Text;

namespace Pong.Core
{
    public class InputButton : Button
    {
        public int MaxLength { get; set; } = 15;
        public string Output => _text.ToString();

        private readonly StringBuilder _text = new();

        public InputButton(string text, Pivot pivot = Pivot.Center)
            : base(text, pivot)
        {
            CursorSelector = "*_";
        }

        public void Set(string text)
        {
            _text.Clear();
            _text.Append(text);
        }

        public override void UpdateSelected(float dt)
        {
            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if (key >= 32 && key <= 126 && _text.Length < MaxLength)
                {
                    _text.Append((char)key);
                }

                key = Raylib.GetCharPressed();
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace)
                || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace))
            {
                if (_text.Length > 0)
                {
                    _text.Remove(_text.Length - 1, 1);
                }
            }

            base.UpdateSelected(dt);
        }

        public override void Draw()
        {
            string temp = Text;
            Text += $" {Output}";

            base.Draw();

            Text = temp;
        }

        public override void DrawSelected()
        {
            string temp = Text;
            Text += $" {Output}";

            base.DrawSelected();

            Text = temp;
        }
    }
}
