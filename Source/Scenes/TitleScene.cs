namespace Pong.Scenes
{
    public class TitleScene : Scene
    {
        private readonly ButtonHandler _buttons;
        private readonly ButtonHandler _connectButtons;
        private bool _showConnectPage;

        private readonly InputButton _ip;

        public TitleScene()
        {
            _buttons = new ButtonHandler(Game.Width / 2f, Game.Height * 0.375f, 15f);

            _buttons.Add(new Button("Singleplayer", () =>
            {
                Game.Instance.LoadScene(new GameScene(aiMode: true));
            }));

            _buttons.Add(new Button("Local Multiplayer", () =>
            {
                Game.Instance.LoadScene(new GameScene());
            }));

            // _buttons.Add(new Button("Online Multiplayer", () =>
            // {
            //     _showConnectPage = true;
            // }));

            _buttons.Add(new Button("Exit", () =>
            {
                Game.Instance.Exit();
            }));

            _buttons.PlaceButtons();

            _connectButtons = new ButtonHandler(Game.Width / 2f, Game.Height * 0.375f, 15f);
            _ip = new InputButton("ip:");
            _ip.Set("192.168.1.168");
            _connectButtons.Add(_ip);
            _connectButtons.Add(new Button("Connect", () =>
            {

            }));
            _connectButtons.Add(new Button("Host", () =>
            {

            }));
            _connectButtons.Add(new Button("Back", () =>
            {
                _showConnectPage = false;
            }));
            _connectButtons.PlaceButtons();
        }

        public override void Update(float dt)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                Game.Instance.Exit();
            }

            if (_showConnectPage) _connectButtons.Update(dt);
            else _buttons.Update(dt);
        }

        public override void Draw()
        {
            if (_showConnectPage)
            {
                _connectButtons.Draw();
            }
            else
            {
                Text.Draw("Pong", Game.Width / 2, Game.Height * 0.25f, 70, Color.White, Pivot.Center);
                _buttons.Draw();
            }
        }
    }
}
