namespace Pong.Scenes
{
    public class GameScene : Scene
    {
        private enum Winner
        {
            None,
            Left,
            Right
        }

        private Paddle _leftPaddle;
        private Paddle _rightPaddle;
        private Ball _ball;

        private int _leftScore;
        private int _rightScore;

        private Winner _prevWinner = Winner.None;

        private const float StartTime = 1f;
        private float _startTimer;

        private readonly bool _aiMode;

        private bool _isPaused;
        private readonly ButtonHandler _pauseButtons;

        private const float AiReactionTime = 0.1f;
        private float _aiReactionTimer;
        private float _aiTarget;

        public GameScene(bool aiMode = false)
        {
            _aiMode = aiMode;

            _pauseButtons = new ButtonHandler(Game.Width / 2f, Game.Height * 0.4f);

            _pauseButtons.Add(new Button("Resume", () =>
            {
                Resume();
            }));

            _pauseButtons.Add(new Button("Exit", () =>
            {
                Game.Instance.LoadScene(new TitleScene());
            }));

            _pauseButtons.PlaceButtons();

            Reset();
        }

        public void Reset()
        {
            MakePaddles();
            MakeBall();

            _startTimer = StartTime - 0.01f;
            CalculateAiPrediction();
        }

        private void MakePaddles()
        {
            const int Gap = 10;
            const int Width = 20, Height = 120;

            _leftPaddle = new Paddle(Gap, Game.Height / 2 - Height / 2, Width, Height);
            _rightPaddle = new Paddle(Game.Width - Width - Gap, Game.Height / 2 - Height / 2, Width, Height);

            _leftPaddle.Color = Color.Blue;
            _rightPaddle.Color = Color.Red;
        }

        private void MakeBall()
        {
            _ball = new Ball(Game.Width / 2f, Game.Height / 2f);

            if (_prevWinner == Winner.Left) _ball.Direction.X = 1f;
            else if (_prevWinner == Winner.Right) _ball.Direction.X = -1f;
            else _ball.Direction.X = Helper.Rand(0, 2) == 0 ? -1f : 1f;

            _ball.Direction.Y = Helper.RandF(-0.05f, 0.05f);
        }

        private void Pause()
        {
            _pauseButtons.SetSeleced(0);
            _isPaused = true;
        }

        private void Resume()
        {
            _isPaused = false;
        }

        public override void Update(float dt)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                if (_isPaused) Resume();
                else Pause();
            }

            if (!_isPaused)
            {
                if (_startTimer <= 0f)
                {
                    HandlePaddles(dt);
                    HandleBall(dt);
                    HandleScoring();
                }
                else
                {
                    _startTimer -= dt;
                }
            }
            else
            {
                _pauseButtons.Update(dt);
            }
        }

        private void HandlePaddles(float dt)
        {
            if (_aiMode)
            {
                if (Raylib.IsKeyDown(KeyboardKey.W)) _leftPaddle.Position.Y -= Paddle.Speed * dt;
                else if (Raylib.IsKeyDown(KeyboardKey.Up)) _leftPaddle.Position.Y -= Paddle.Speed * dt;
                if (Raylib.IsKeyDown(KeyboardKey.S)) _leftPaddle.Position.Y += Paddle.Speed * dt;
                else if (Raylib.IsKeyDown(KeyboardKey.Down)) _leftPaddle.Position.Y += Paddle.Speed * dt;

                HandleAI(dt);
            }
            else
            {
                if (Raylib.IsKeyDown(KeyboardKey.W)) _leftPaddle.Position.Y -= Paddle.Speed * dt;
                if (Raylib.IsKeyDown(KeyboardKey.S)) _leftPaddle.Position.Y += Paddle.Speed * dt;
                if (Raylib.IsKeyDown(KeyboardKey.Up)) _rightPaddle.Position.Y -= Paddle.Speed * dt;
                if (Raylib.IsKeyDown(KeyboardKey.Down)) _rightPaddle.Position.Y += Paddle.Speed * dt;
            }

            _leftPaddle.Update(dt);
            _rightPaddle.Update(dt);
        }

        private void HandleAI(float dt)
        {
            if (_aiReactionTimer > 0f)
            {
                _aiReactionTimer -= dt;
                if (_aiReactionTimer <= 0f && _ball.Direction.X != 0f)
                {
                    CalculateAiPrediction();
                }
            }

            if (_aiTarget < _rightPaddle.Position.Y + _rightPaddle.Size.Y / 2f - _ball.Radius)
            {
                _rightPaddle.Position.Y -= Paddle.Speed * dt;
            }
            else if (_aiTarget > _rightPaddle.Position.Y + _rightPaddle.Size.Y / 2f + _ball.Radius)
            {
                _rightPaddle.Position.Y += Paddle.Speed * dt;
            }
        }

        private void CalculateAiPrediction()
        {
            _aiTarget = _ball.Position.Y + (Game.Width - _ball.Position.X) * (_ball.Direction.Y / _ball.Direction.X);
            _aiTarget = Game.Height - MathF.Abs(Math.Abs(_aiTarget) % (Game.Height * 2f) - Game.Height);
        }

        private void HandleBall(float dt)
        {
            _ball.Update(dt);
            _ball.HandlePaddleCollision(_rightPaddle);
            
            if (_ball.HandlePaddleCollision(_leftPaddle))
            {
                _aiReactionTimer = AiReactionTime;
            }
        }

        private void HandleScoring()
        {
            if (_ball.Position.X < 0f)
            {
                _rightScore++;
                _prevWinner = Winner.Right;
                Game.Instance.PlaySound("score");

                Reset();
            }
            else if (_ball.Position.X > Game.Width)
            {
                _leftScore++;
                _prevWinner = Winner.Left;
                Game.Instance.PlaySound("score");

                Reset();
            }
        }

        public override void Draw()
        {
            DrawCourt();
            DrawScores();

            _leftPaddle.Draw();
            _rightPaddle.Draw();
            _ball.Draw();

            DrawTimerOverlay();
            DrawPauseOverlay();
        }

        private void DrawCourt()
        {
            const int Lines = 20;
            const int Gap = 20;
            const int LineLength = 21;
            for (int i = 0; i < Lines; i++)
            {
                int y = i * Gap + i * LineLength;
                Raylib.DrawLineEx(new Vector2(Game.Width / 2, y), new Vector2(Game.Width / 2, y + LineLength), 2, Color.DarkGray);
            }
        }

        private void DrawScores()
        {
            Text.Draw(_leftScore.ToString(), Game.Width / 4f, Game.Height * 0.25f, 120, Color.Gray, Pivot.Center);
            Text.Draw(_rightScore.ToString(), Game.Width / 4f * 3f, Game.Height * 0.25f, 120, Color.Gray, Pivot.Center);
        }

        private void DrawTimerOverlay()
        {
            if (_startTimer <= 0f)
                return;

            Raylib.DrawRectangle(0, 0, Game.Width, Game.Height, new Color(0, 0, 0, 150));
            Text.Draw("Ready", Game.Width / 2f, Game.Height * 0.35f, 70, Color.White, Pivot.Center);
        }

        private void DrawPauseOverlay()
        {
            if (!_isPaused)
                return;

            Raylib.DrawRectangle(0, 0, Game.Width, Game.Height, new Color(0, 0, 0, 150));
            Text.Draw("Paused", Game.Width / 2f, Game.Height * 0.3f, 70, Color.White, Pivot.Center);
            _pauseButtons.Draw();
        }
    }
}
