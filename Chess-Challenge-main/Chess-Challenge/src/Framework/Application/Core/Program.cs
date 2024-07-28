using Raylib_cs;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ChessChallenge.Application
{
    public static class Program
    {
        const bool hideRaylibLogs = true;
        static Camera2D cam;
        static ChallengeController ghost = new();
        static async Task Main(string[] args)
        {
            //making this thing uci compatible ./
            ghost.start_engine();
            while(ghost.check()){
                string input = await Task.Run(Console.ReadLine);
                string[] tiny = input.Trim().Split();
                switch (tiny[0]){
                    case "uci":
                        Console.WriteLine("id name ghost");
                        Console.WriteLine("id author Dylan ChenZhen");
                        Console.WriteLine("uciok");
                        break;
                    case "isready":
                        Console.WriteLine("readyok");
                        break;
                    case "go":
                        go(token);
                        break;
                    case "position":
                        setup_position(tiny);
                        break;
                    case "ucinewgame":
                        break;
                    case "quit":
                        ghost.EndGame();
                        break;
                    default:
                        Console.WriteLine("UNKNOWN INPUT " + input);
                        return;
                    }
            }
            
            // Vector2 loadedWindowSize = GetSavedWindowSize();
            // int screenWidth = (int)loadedWindowSize.X;
            // int screenHeight = (int)loadedWindowSize.Y;

            // if (hideRaylibLogs)
            // {
            //     unsafe
            //     {
            //         Raylib.SetTraceLogCallback(&LogCustom);
            //     }
            // }

            // Raylib.InitWindow(screenWidth, screenHeight, "Chess Coding Challenge");
            // Raylib.SetTargetFPS(60);

            // UpdateCamera(screenWidth, screenHeight);

            // ChallengeController controller = new();

            // while (!Raylib.WindowShouldClose())
            // {
            //     Raylib.BeginDrawing();
            //     Raylib.ClearBackground(new Color(22, 22, 22, 255));
            //     Raylib.BeginMode2D(cam);

            //     controller.Update();
            //     controller.Draw();

            //     Raylib.EndMode2D();

            //     controller.DrawOverlay();

            //     Raylib.EndDrawing();
            // }

            // Raylib.CloseWindow();

            // controller.Release();
            // UIHelper.Release();
        }

        private static void setup_position(string [] tiny){
            if(tiny[1] == "startpos"){
                ghost.setup_basic();
            }
            if(tiny[1] == "fen"){
                string fen = string.Join(' ', tiny, 2, tiny.Length - 2);
                ghost.setup_fen(fen);
            } 

        }
        public static void SetWindowSize(Vector2 size)
        {
            Raylib.SetWindowSize((int)size.X, (int)size.Y);
            UpdateCamera((int)size.X, (int)size.Y);
            SaveWindowSize();
        }

        public static Vector2 ScreenToWorldPos(Vector2 screenPos) => Raylib.GetScreenToWorld2D(screenPos, cam);

        static void UpdateCamera(int screenWidth, int screenHeight)
        {
            cam = new Camera2D();
            cam.target = new Vector2(0, 15);
            cam.offset = new Vector2(screenWidth / 2f, screenHeight / 2f);
            cam.zoom = screenWidth / 1280f * 0.7f;
        }


        [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private static unsafe void LogCustom(int logLevel, sbyte* text, sbyte* args)
        {
        }

        static Vector2 GetSavedWindowSize()
        {
            if (File.Exists(FileHelper.PrefsFilePath))
            {
                string prefs = File.ReadAllText(FileHelper.PrefsFilePath);
                if (!string.IsNullOrEmpty(prefs))
                {
                    if (prefs[0] == '0')
                    {
                        return Settings.ScreenSizeSmall;
                    }
                    else if (prefs[0] == '1')
                    {
                        return Settings.ScreenSizeBig;
                    }
                }
            }
            return Settings.ScreenSizeSmall;
        }

        static void SaveWindowSize()
        {
            Directory.CreateDirectory(FileHelper.AppDataPath);
            bool isBigWindow = Raylib.GetScreenWidth() > Settings.ScreenSizeSmall.X;
            File.WriteAllText(FileHelper.PrefsFilePath, isBigWindow ? "1" : "0");
        }

      

    }


}