namespace Pong.Core
{
    public class Button
    {
        public event Action OnClick;
        public Vector2 Position;

        public string Text { get; set; } = string.Empty;
        public Pivot Pivot { get; set; }
        public int FontSize { get; set; } = 30;
        public Color Color { get; set; } = Color.White;

        public int SelectedFontSize { get; set; } = 35;
        public Color SelectedColor { get; set; } = Color.Yellow;

        /// <summary>
        /// Text is inserted at the place of '*'
        /// </summary>
        protected string CursorSelector { get; set; } = "> * <";

        public Button(string text, Action action, Pivot pivot = Pivot.Center)
        {
            Text = text;
            OnClick += action;
            Pivot = pivot;
        }

        public Button(string text, Pivot pivot = Pivot.Center)
        {
            Text = text;
            Pivot = pivot;
        }

        public void Click()
        {
            OnClick?.Invoke();
        }

        public virtual void Update(float dt)
        {

        }

        public virtual void UpdateSelected(float dt)
        {

        }

        public virtual void Draw()
        {
            Core.Text.Draw(Text, Position, FontSize, Color, Pivot);
        }

        public virtual void DrawSelected()
        {
            Core.Text.Draw(CursorSelector.Replace("*", Text), Position, SelectedFontSize, SelectedColor, Pivot);
        }
    }
}
