namespace Pong.Core
{
    public class ButtonHandler(float x, float y, float gap = 15f)
    {
        public float X { get; set; } = x;
        public float Y { get; set; } = y;
        public float Gap { get; set; } = gap;

        private readonly List<Button> _buttons = [];
        private int _selectedIndex = 0;

        public void Add(Button button)
        {
            _buttons.Add(button);
        }

        public void SetSeleced(int index)
        {
            _selectedIndex = index;
            if (_selectedIndex < 0 || _selectedIndex >= _buttons.Count)
            {
                _selectedIndex = 0;
            }
        }

        public void PlaceButtons()
        {
            int prevHeights = 0;

            for (int i = 0; i < _buttons.Count; i++)
            {
                Button button = _buttons[i];
                button.Position.X = X;

                if (i > 0) prevHeights += _buttons[i - 1].FontSize;

                button.Position.Y = Y + Gap * i + prevHeights;
            }
        }

        public void Update(float dt)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                _selectedIndex--;
                if (_selectedIndex < 0)
                {
                    _selectedIndex = _buttons.Count - 1;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                _selectedIndex++;
                if (_selectedIndex >= _buttons.Count)
                {
                    _selectedIndex = 0;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                if (_selectedIndex >= 0 && _selectedIndex < _buttons.Count)
                {
                    _buttons[_selectedIndex].Click();
                }
            }

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_selectedIndex == i) _buttons[i].UpdateSelected(dt);
                else _buttons[i].Update(dt);
            }
        }

        public void Draw()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_selectedIndex == i) _buttons[i].DrawSelected();
                else _buttons[i].Draw();
            }
        }
    }
}
