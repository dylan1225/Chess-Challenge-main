using ChessChallenge.Chess;
using ChessChallenge.Example;
using Raylib_cs;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ChessChallenge.Application.Settings;
using static ChessChallenge.Application.ConsoleHelper;

namespace ChessChallenge.Application
{
    public class ChallengeController
    {
        public enum PlayerType
        {
            Human,
            MyBot,
            EvilBot
        }

        // Game state
        readonly Random rng;
        int gameID;
        bool isPlaying;
        Board board;
        // Bot match state
        readonly string[] botMatchStartFens;
        int botMatchGameIndex;
        // Bot task

        // Other

        public ChallengeController()
        {
            // Log($"Launching Chess-Challenge version {Settings.Version}");
            Warmer.Warm();
            // boardUI = new BoardUI();
            board = new Board();
            // pgns = new();

            // BotStatsA = new BotMatchStats("IBot");
            // BotStatsB = new BotMatchStats("IBot");
            botMatchStartFens = FileHelper.ReadResourceFile("Fens.txt").Split('\n').Where(fen => fen.Length > 0).ToArray();
            // botTaskWaitHandle = new AutoResetEvent(false);
        }

        public void start_engine(){
            isPlaying = true;
        }

        public bool check(){
            return isPlaying;
        }

        public void setup_basic(){
            board.LoadPosition(botMatchStartFens[0]);
        }

        public void setup_fen(string fen){
            board.LoadPosition(fen);
        }

        public void StartNewGame(PlayerType whiteType, PlayerType blackType)
        {
            // End any ongoing game
            // EndGame(GameResult.DrawByArbiter, log: false, autoStartNextBotMatch: false);
            // gameID = rng.Next();

            // // Stop prev task and create a new one
            // if (RunBotsOnSeparateThread)
            // {
            //     // Allow task to terminate
            //     botTaskWaitHandle.Set();
            //     // Create new task
            //     botTaskWaitHandle = new AutoResetEvent(false);
            //     Task.Factory.StartNew(BotThinkerThread, TaskCreationOptions.LongRunning);
            // }
            // // Board Setup
            // board = new Board();
            // bool isGameWithHuman = whiteType is PlayerType.Human || blackType is PlayerType.Human;
            // int fenIndex = isGameWithHuman ? 0 : botMatchGameIndex / 2;
            // board.LoadPosition(botMatchStartFens[fenIndex]);

            // // Player Setup
            // PlayerWhite = CreatePlayer(whiteType);
            // PlayerBlack = CreatePlayer(blackType);
            // PlayerWhite.SubscribeToMoveChosenEventIfHuman(OnMoveChosen);
            // PlayerBlack.SubscribeToMoveChosenEventIfHuman(OnMoveChosen);

            // // UI Setup
            // boardUI.UpdatePosition(board);
            // boardUI.ResetSquareColours();
            // SetBoardPerspective();

            // // Start
            // isPlaying = true;
            // NotifyTurnToMove();
        }



        public Move GetBotMove()
        {
            API.Board botBoard = new(board);
            API.Timer timer = new(int.MaxValue, int.MaxValue, 1, 0);
            API.Move move = MyBot.Think(botBoard, timer);
            Move nmove = new Move(move.RawValue);
            OnMoveChosen(nmove);
            string movestr = BoardHelper.SquareNameFromIndex(nmove.StartSquareIndex);
            movestr += BoardHelper.SquareNameFromIndex(nmove.TargetSquareIndex);
            if(nmove.IsPromotion){
                int type = nmove.PromotionPieceType;
                //2 = 
                if(type == PieceHelper.Knight) movestr += "N";
                if(type == PieceHelper.Bishop) movestr += "B";
                if(type == PieceHelper.Rook) movestr += "R";
                if(type == PieceHelper.Queen) movestr += "Q";
            }
            Console.WriteLine($"bestmove {movestr}");

            return Move.NullMove;
            // try
            // {
            //     API.Timer timer = new(PlayerToMove.TimeRemainingMs, PlayerNotOnMove.TimeRemainingMs, GameDurationMilliseconds, IncrementMilliseconds);
            //     API.Move move = PlayerToMove.Bot.Think(botBoard, timer);
            //     return new Move(move.RawValue);
            // }
            // catch (Exception e)
            // {
            //     Log("An error occurred while bot was thinking.\n" + e.ToString(), true, ConsoleColor.Red);
            //     hasBotTaskException = true;
            //     botExInfo = ExceptionDispatchInfo.Capture(e);
            // }
            // return Move.NullMove;
        }


        void OnMoveChosen(Move chosenMove)
        {
            PlayMove(chosenMove);
        }

        public void PlayMove(Move move)
        {
            board.MakeMove(move, false);
            // if (isPlaying)
            // {
            //     bool animate = PlayerToMove.IsBot;
            //     lastMoveMadeTime = (float)Raylib.GetTime();

            //     board.MakeMove(move, false);
            //     boardUI.UpdatePosition(board, move, animate);

            //     GameResult result = Arbiter.GetGameState(board);
            //     if (result == GameResult.InProgress)
            //     {
            //         NotifyTurnToMove();
            //     }
            //     else
            //     {
            //         EndGame(result);
            //     }
            // }
        }
        //missing GameResult result
        public void EndGame(bool log = true, bool autoStartNextBotMatch = true)
        {
            if (isPlaying)
            {
                isPlaying = false;
                gameID = -1;

                // if (log)
                // {
                //     Log("Game Over: " + result, false, ConsoleColor.Blue);
                // }

                // string pgn = PGNCreator.CreatePGN(board, result, GetPlayerName(PlayerWhite), GetPlayerName(PlayerBlack));
                // pgns.AppendLine(pgn);

                // // If 2 bots playing each other, start next game automatically.
                // if (PlayerWhite.IsBot && PlayerBlack.IsBot)
                // {
                //     UpdateBotMatchStats(result);
                //     botMatchGameIndex++;
                //     int numGamesToPlay = botMatchStartFens.Length * 2;

                //     if (botMatchGameIndex < numGamesToPlay && autoStartNextBotMatch)
                //     {
                //         botAPlaysWhite = !botAPlaysWhite;
                //         const int startNextGameDelayMs = 600;
                //         System.Timers.Timer autoNextTimer = new(startNextGameDelayMs);
                //         int originalGameID = gameID;
                //         autoNextTimer.Elapsed += (s, e) => AutoStartNextBotMatchGame(originalGameID, autoNextTimer);
                //         autoNextTimer.AutoReset = false;
                //         autoNextTimer.Start();

                //     }
                //     else if (autoStartNextBotMatch)
                //     {
                //         Log("Match finished", false, ConsoleColor.Blue);
                //     }
                // }
            }
        }

        public void Update()
        {
            // if (isPlaying)
            // {
            //     PlayerWhite.Update();
            //     PlayerBlack.Update();

            //     PlayerToMove.UpdateClock(Raylib.GetFrameTime());
            //     if (PlayerToMove.TimeRemainingMs <= 0)
            //     {
            //         EndGame(PlayerToMove == PlayerWhite ? GameResult.WhiteTimeout : GameResult.BlackTimeout);
            //     }
            //     else
            //     {
            //         if (isWaitingToPlayMove && Raylib.GetTime() > playMoveTime)
            //         {
            //             isWaitingToPlayMove = false;
            //             PlayMove(moveToPlay);
            //         }
            //     }
            // }
        }

        public class BotMatchStats
        {
            public string BotName;
            public int NumWins;
            public int NumLosses;
            public int NumDraws;
            public int NumTimeouts;
            public int NumIllegalMoves;

            public BotMatchStats(string name) => BotName = name;
        }
    }
}
