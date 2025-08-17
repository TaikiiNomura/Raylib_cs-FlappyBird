using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

public static class Program
{
    private enum GameState { READY, PLAYING, GAMEOVER, GAMECLEAR }

    // --- ゲーム全体の設定 ---
    private static class GameConfig
    {
        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;
        public const int Fps = 60;

        public const float Gravity = 0.5f;
        public const float JumpPower = -8.0f;

        public const float PipeSpeed = 3;
        public const float PipeGap = 150;
        public const float PipeWidth = 50;
        public const float PipeSpawnInterval = 120;
        public const float PlayerRadius = 15;
    }

    // --- プレイヤー ---
    private class Player
    {
        public Vector2 Position;
        private float velocity;

        public Player() => Reset();

        public void Reset()
        {
            Position = new Vector2(100, GameConfig.ScreenHeight / 2);
            velocity = 0;
        }

        public void Update()
        {
            velocity += GameConfig.Gravity;
            Position.Y += velocity;

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                velocity = GameConfig.JumpPower;
            }
        }

        public void Draw() =>
            Raylib.DrawCircleV(Position, GameConfig.PlayerRadius, Color.Black);

        public bool IsOutOfBounds() =>
            Position.Y + GameConfig.PlayerRadius > GameConfig.ScreenHeight ||
            Position.Y - GameConfig.PlayerRadius < 0;
    }

    // --- パイプ管理 ---
    private class PipeManager
    {
        private readonly List<Rectangle> pipes = new();
        private float spawnTimer = 0;
        private bool scored = false;

        public void Reset()
        {
            pipes.Clear();
            spawnTimer = 0;
            scored = false;
        }

        public void Update()
        {
            spawnTimer++;
            if (spawnTimer >= GameConfig.PipeSpawnInterval)
            {
                SpawnPipes();
                spawnTimer = 0;
                scored = false;
            }

            for (int i = pipes.Count - 1; i >= 0; i--)
            {
                pipes[i] = new Rectangle(
                    pipes[i].X - GameConfig.PipeSpeed,
                    pipes[i].Y,
                    pipes[i].Width,
                    pipes[i].Height
                );
                if (pipes[i].X < -GameConfig.PipeWidth)
                {
                    pipes.RemoveAt(i);
                }
            }
        }

        private void SpawnPipes()
        {
            float gapCenterY = Raylib.GetRandomValue(GameConfig.ScreenHeight / 4, GameConfig.ScreenHeight * 3 / 4);
            float topHeight = gapCenterY - GameConfig.PipeGap / 2;
            float bottomY = gapCenterY + GameConfig.PipeGap / 2;
            float bottomHeight = GameConfig.ScreenHeight - bottomY;

            pipes.Add(new Rectangle(GameConfig.ScreenWidth, 0, GameConfig.PipeWidth, topHeight));
            pipes.Add(new Rectangle(GameConfig.ScreenWidth, bottomY, GameConfig.PipeWidth, bottomHeight));
        }

        public bool CheckCollision(Player player)
        {
            foreach (var pipe in pipes)
            {
                if (Raylib.CheckCollisionCircleRec(player.Position, GameConfig.PlayerRadius, pipe))
                    return true;
            }
            return false;
        }

        public bool TryScore(Player player, ref int score)
        {
            if (!scored && pipes.Count > 0 && pipes[0].X + pipes[0].Width < player.Position.X)
            {
                score++;
                scored = true;
                return true;
            }
            return false;
        }

        public void Draw()
        {
            foreach (var pipe in pipes)
                Raylib.DrawRectangleRec(pipe, Color.Green);
        }
    }

    // --- メイン処理 ---
    public static void Main()
    {
        Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, "Flappy Bird Clone");
        Raylib.SetTargetFPS(GameConfig.Fps);

        GameState state = GameState.READY;
        bool isFirstTime = true;

        Player player = new Player();
        PipeManager pipeManager = new PipeManager();
        int score = 0;

        while (!Raylib.WindowShouldClose())
        {
            // --- 更新 ---
            switch (state)
            {
                case GameState.READY:
                    if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                    {
                        state = GameState.PLAYING;
                        isFirstTime = false;
                    }
                    break;

                case GameState.PLAYING:
                    player.Update();
                    pipeManager.Update();

                    if (pipeManager.CheckCollision(player) || player.IsOutOfBounds())
                        state = GameState.GAMEOVER;

                    pipeManager.TryScore(player, ref score);

                    // ===== ここでゲームクリア条件を管理 =====
                    if (score >= 5)
                        state = GameState.GAMECLEAR;

                    break;

                case GameState.GAMEOVER:
                case GameState.GAMECLEAR:
                    if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                    {
                        player.Reset();
                        pipeManager.Reset();
                        score = 0;
                        state = GameState.PLAYING;
                    }
                    break;
            }

            // --- 描画 ---
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            switch (state)
            {
                case GameState.PLAYING:
                    player.Draw();
                    pipeManager.Draw();
                    Raylib.DrawText(score.ToString(), 10, 10, 30, Color.Black);
                    break;

                case GameState.READY:
                    if (isFirstTime)
                    {
                        Raylib.DrawText("FLAPPY BIRD", GameConfig.ScreenWidth / 2 - 200, GameConfig.ScreenHeight / 2 - 100, 50, Color.Black);
                        Raylib.DrawText("Press 'ENTER' to start", GameConfig.ScreenWidth / 2 - 150, GameConfig.ScreenHeight / 2, 20, Color.Black);
                    }
                    break;

                case GameState.GAMEOVER:
                    Raylib.DrawText("GAME OVER", GameConfig.ScreenWidth / 2 - 150, GameConfig.ScreenHeight / 2 - 50, 50, Color.Black);
                    Raylib.DrawText($"Score: {score}", GameConfig.ScreenWidth / 2 - 50, GameConfig.ScreenHeight / 2, 25, Color.Black);
                    Raylib.DrawText("Press 'ENTER' to Restart", GameConfig.ScreenWidth / 2 - 150, GameConfig.ScreenHeight / 2 + 50, 20, Color.Black);
                    break;

                case GameState.GAMECLEAR:
                    Raylib.DrawText("GAME CLEAR!!", GameConfig.ScreenWidth / 2 - 200, GameConfig.ScreenHeight / 2 - 50, 50, Color.Gold);
                    Raylib.DrawText($"Final Score: {score}", GameConfig.ScreenWidth / 2 - 80, GameConfig.ScreenHeight / 2, 25, Color.Black);
                    Raylib.DrawText("Press 'ENTER' to Play Again", GameConfig.ScreenWidth / 2 - 180, GameConfig.ScreenHeight / 2 + 50, 20, Color.Black);
                    break;
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}