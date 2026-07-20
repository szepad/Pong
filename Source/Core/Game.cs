global using System.Numerics;
global using Raylib_cs;
global using Pong.Core;

using Pong.Scenes;

namespace Pong.Core
{
    public class Game
    {
        #region Singleton
        private Game() { }

        private static Game _instance;
        public static Game Instance
        {
            get
            {
                _instance ??= new Game();
                return _instance;
            }
        }
        #endregion

        public const int Width = 1280;
        public const int Height = 720;

        public StretchMode StretchMode { get; set; }
        public bool IsFullscreen { get; private set; }

        private bool _exited;
        private Scene _activeScene;

        private RenderTexture2D _virtualScreen;

        public void Run()
        {
            Init();
            Load();

            while (!Raylib.WindowShouldClose() && !_exited)
            {
                Update();
                Draw();
            }

            Close();
        }

        public void Exit()
        {
            _exited = true;
        }

        public void LoadScene(Scene scene)
        {
            _activeScene = scene;
        }

        public void PlaySound(string name)
        {
            if (!Assets.Sounds.ContainsKey(name))
            {
                Raylib.TraceLog(TraceLogLevel.Error, $"Couldn't play sound '{name}'");
                return;
            }

            Sound sound = Raylib.LoadSoundAlias(Assets.Sounds[name]);
            Raylib.PlaySound(sound);
        }

        public void SetFullscreen(bool fullscreen)
        {
            IsFullscreen = fullscreen;

            int monitor = Raylib.GetCurrentMonitor();
            int monitorWidth = Raylib.GetMonitorWidth(monitor);
            int monitorHeight = Raylib.GetMonitorHeight(monitor);

            if (fullscreen)
            {
                Raylib.SetWindowState(ConfigFlags.UndecoratedWindow);
                Raylib.SetWindowSize(monitorWidth, monitorHeight);
                Raylib.SetWindowPosition(0, 0);
            }
            else
            {
                Raylib.ClearWindowState(ConfigFlags.UndecoratedWindow);
                Raylib.SetWindowSize(Width, Height);
                Raylib.SetWindowPosition((monitorWidth - Width) / 2, (monitorHeight - Height) / 2);
            }
        }

        private void Init()
        {
            Raylib.SetTraceLogLevel(TraceLogLevel.Warning);

            Raylib.InitWindow(Width, Height, "Pong");
            Raylib.InitAudioDevice();

            Raylib.SetTargetFPS(0);
            Raylib.SetWindowState(ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);

            Raylib.SetExitKey(KeyboardKey.Null);
            Raylib.SetTextLineSpacing(40);
            Raylib.HideCursor();

            Raylib.SetWindowIcon(Raylib.LoadImage("res/icon.png"));

            StretchMode = StretchMode.LetterBox;
        }

        private void Load()
        {
            _virtualScreen = Raylib.LoadRenderTexture(Width, Height);

            Assets.Load("res");

            LoadScene(new TitleScene());
        }

        private void Update()
        {
            float dt = Raylib.GetFrameTime();
            _activeScene.Update(dt);

            if (Raylib.IsKeyPressed(KeyboardKey.F11))
            {
                SetFullscreen(!IsFullscreen);
            }
        }

        private void Draw()
        {
            Raylib.BeginTextureMode(_virtualScreen);
            Raylib.ClearBackground(Color.Black);

            _activeScene?.Draw();

            Raylib.EndTextureMode();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Flip height for opengl reasons
            Rectangle src = new(0, 0, Width, -Height);
            Rectangle dst = StretchMode switch
            {
                StretchMode.Stretch => GetStretchView(),
                StretchMode.LetterBox => GetLetterBoxView(),
                _ => GetNormalView(),
            };

            Raylib.DrawTexturePro(_virtualScreen.Texture, src, dst, Vector2.Zero, 0f, Color.White);
            Raylib.EndDrawing();
        }

        private static Rectangle GetNormalView()
        {
            return new(0, 0, Width, Height);
        }

        private static Rectangle GetStretchView()
        {
            return new(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }

        private static Rectangle GetLetterBoxView()
        {
            int x = 0, y = 0;
            int winWidth = Raylib.GetScreenWidth();
            int winHeight = Raylib.GetScreenHeight();
            int actualWidth = winWidth, actualHeight = winHeight;

            const float Aspect = Width / (float)Height;
            float ratio = winWidth / (float)winHeight;

            if (ratio > Aspect)
            {
                float lambda = winHeight / (float)Height;
                actualWidth = (int)(Width * lambda);
                x = (winWidth - actualWidth) / 2;
            }
            else
            {
                float lambda = winWidth / (float)Width;
                actualHeight = (int)(Height * lambda);
                y = (winHeight - actualHeight) / 2;
            }

            return new(x, y, actualWidth, actualHeight);
        }

        private void Close()
        {
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}
