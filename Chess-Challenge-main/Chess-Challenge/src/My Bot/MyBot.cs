using System;
using System.ComponentModel;
using System.Linq;
using ChessChallenge.API;
using Microsoft.VisualBasic;

static public class MyBot
{
    /* 
     * Art by Donovan Bake
     .-.
    (o o) boo!
    | O \
     \   \
      `~~~'
     */
    //values obtained from peSTO's evaluation function, which perform much better than the simplied version :/
    //Null, pawn, horse, bishop, tower, queen and king
    //mg = mid game, eg = eg
    static int limit = 4500;
    static Timer t_time;
    static int[] mg_piece = { 0, 82, 337, 365, 477, 1025, 200000 };
    static int[] eg_piece = { 0, 94, 281, 297, 512, 936, 200000 };
    //correcting board index for white and black to make my life easier later 
    //correcting board index for white and black to make my life easier later 
    static int[] cor_white = {
        56, 57, 58, 59, 60, 61, 62, 63,
        48, 49, 50, 51, 52, 53, 54, 55,
        40, 41, 42, 43, 44, 45, 46, 47,
        32, 33, 34, 35, 36, 37, 38, 39,
        24, 25, 26, 27, 28, 29, 30, 31,
        16, 17, 18, 19, 20, 21, 22, 23,
        8, 9, 10, 11, 12, 13, 14, 15,
        0, 1, 2, 3, 4, 5, 6, 7
    };

    static int[] cor_black = {
        7, 6, 5, 4, 3, 2, 1, 0,
        15, 14, 13, 12, 11, 10, 9, 8,
        23, 22, 21, 20, 19, 18, 17, 16,
        31, 30, 29, 28, 27, 26, 25, 24,
        39, 38, 37, 36, 35, 34, 33, 32,
        47, 46, 45, 44, 43, 42, 41, 40,
        55, 54, 53, 52, 51, 50, 49, 48,
        63, 62, 61, 60, 59, 58, 57, 56
    };
    static int[] mg_pawn = {
        0,   0,   0,   0,   0,   0,  0,   0,
        98, 134,  61,  95,  68, 126, 34, -11,
        -6,   7,  26,  31,  65,  56, 25, -20,
        -14,  13,   6,  21,  23,  12, 17, -23,
        -27,  -2,  -5,  12,  17,   6, 10, -25,
        -26,  -4,  -4, -10,   3,   3, 33, -12,
        -35,  -1, -20, -23, -15,  24, 38, -22,
        0,   0,   0,   0,   0,   0,  0,   0
    };

    static int[] eg_pawn = {
        0,   0,   0,   0,   0,   0,   0,   0,
        178, 173, 158, 134, 147, 132, 165, 187,
        94, 100,  85,  67,  56,  53,  82,  84,
        32,  24,  13,   5,  -2,   4,  17,  17,
        13,   9,  -3,  -7,  -7,  -8,   3,  -1,
        4,   7,  -6,   1,   0,  -5,  -1,  -8,
        13,   8,   8,  10,  13,   0,   2,  -7,
        0,   0,   0,   0,   0,   0,   0,   0
    };

    static int[] mg_horse = {
        -167, -89, -34, -49,  61, -97, -15, -107,
        -73, -41,  72,  36,  23,  62,   7,  -17,
        -47,  60,  37,  65,  84, 129,  73,   44,
        -9,  17,  19,  53,  37,  69,  18,   22,
        -13,   4,  16,  13,  28,  19,  21,   -8,
        -23,  -9,  12,  10,  19,  17,  25,  -16,
        -29, -53, -12,  -3,  -1,  18, -14,  -19,
        -105, -21, -58, -33, -17, -28, -19,  -23
    };

    static int[] eg_horse = {
        -58, -38, -13, -28, -31, -27, -63, -99,
        -25,  -8, -25,  -2,  -9, -25, -24, -52,
        -24, -20,  10,   9,  -1,  -9, -19, -41,
        -17,   3,  22,  22,  22,  11,   8, -18,
        -18,  -6,  16,  25,  16,  17,   4, -18,
        -23,  -3,  -1,  15,  10,  -3, -20, -22,
        -42, -20, -10,  -5,  -2, -20, -23, -44,
        -29, -51, -23, -15, -22, -18, -50, -64
    };

    static int[] mg_bishop = {
        -29,   4, -82, -37, -25, -42,   7,  -8,
        -26,  16, -18, -13,  30,  59,  18, -47,
        -16,  37,  43,  40,  35,  50,  37,  -2,
        -4,   5,  19,  50,  37,  37,   7,  -2,
        -6,  13,  13,  26,  34,  12,  10,   4,
        0,  15,  15,  15,  14,  27,  18,  10,
        4,  15,  16,   0,   7,  21,  33,   1,
        -33,  -3, -14, -21, -13, -12, -39, -21
    };

    static int[] eg_bishop = {
        -14, -21, -11,  -8, -7,  -9, -17, -24,
        -8,  -4,   7, -12, -3, -13,  -4, -14,
        2,  -8,   0,  -1, -2,   6,   0,   4,
        -3,   9,  12,   9, 14,  10,   3,   2,
        -6,   3,  13,  19,  7,  10,  -3,  -9,
        -12,  -3,   8,  10, 13,   3,  -7, -15,
        -14, -18,  -7,  -1,  4,  -9, -15, -27,
        -23,  -9, -23,  -5, -9, -16,  -5, -17
    };

    static int[] mg_tower = {
        32,  42,  32,  51, 63,  9,  31,  43,
        27,  32,  58,  62, 80, 67,  26,  44,
        -5,  19,  26,  36, 17, 45,  61,  16,
        -24, -11,   7,  26, 24, 35,  -8, -20,
        -36, -26, -12,  -1,  9, -7,   6, -23,
        -45, -25, -16, -17,  3,  0,  -5, -33,
        -44, -16, -20,  -9, -1, 11,  -6, -71,
        -19, -13,   1,  17, 16,  7, -37, -26
    };

    static int[] eg_tower = {
        13, 10, 18, 15, 12,  12,   8,   5,
        11, 13, 13, 11, -3,   3,   8,   3,
        7,  7,  7,  5,  4,  -3,  -5,  -3,
        4,  3, 13,  1,  2,   1,  -1,   2,
        3,  5,  8,  4, -5,  -6,  -8, -11,
        -4,  0, -5, -1, -7, -12,  -8, -16,
        -6, -6,  0,  2, -9,  -9, -11,  -3,
        -9,  2,  3, -1, -5, -13,   4, -20
    };

    static int[] mg_queen = {
        -28,   0,  29,  12,  59,  44,  43,  45,
        -24, -39,  -5,   1, -16,  57,  28,  54,
        -13, -17,   7,   8,  29,  56,  47,  57,
        -27, -27, -16, -16,  -1,  17,  -2,   1,
        -9, -26,  -9, -10,  -2,  -4,   3,  -3,
        -14,   2, -11,  -2,  -5,   2,  14,   5,
        -35,  -8,  11,   2,   8,  15,  -3,   1,
        -1, -18,  -9,  10, -15, -25, -31, -50
    };

    static int[] eg_queen = {
        -9,  22,  22,  27,  27,  19,  10,  20,
        -17,  20,  32,  41,  58,  25,  30,   0,
        -20,   6,   9,  49,  47,  35,  19,   9,
        3,  22,  24,  45,  57,  40,  57,  36,
        -18,  28,  19,  47,  31,  34,  39,  23,
        -16, -27,  15,   6,   9,  17,  10,   5,
        -22, -23, -30, -16, -16, -23, -36, -32,
        -33, -28, -22, -43,  -5, -32, -20, -41
    };

    static int[] mg_king = {
        -65,  23,  16, -15, -56, -34,   2,  13,
        29,  -1, -20,  -7,  -8,  -4, -38, -29,
        -9,  24,   2, -16, -20,   6,  22, -22,
        -17, -20, -12, -27, -30, -25, -14, -36,
        -49,  -1, -27, -39, -46, -44, -33, -51,
        -14, -14, -22, -46, -44, -30, -15, -27,
        1,   7,  -8, -64, -43, -16,   9,   8,
        -15,  36,  12, -54,   8, -28,  24,  14
    };

    static int[] eg_king = {
        -74, -35, -18, -18, -11,  15,   4, -17,
        -12,  17,  14,  17,  17,  38,  23,  11,
        10,  17,  23,  15,  20,  45,  44,  13,
        -8,  22,  24,  27,  26,  33,  26,   3,
        -18,  -4,  21,  24,  27,  23,   9, -11,
        -19,  -3,  11,  21,  23,  16,   7,  -9,
        -27, -11,   4,  13,  14,   4,  -5, -17,
        -53, -34, -21, -11, -28, -14, -24, -43
    };

    //putting the array into table for easier access later using piecetype instead of manual check 
    static int[][] mg_table = {
        mg_pawn, mg_horse, mg_bishop, mg_tower, mg_queen, mg_king
    };

    static int[][] eg_table = {
        eg_pawn, eg_horse, eg_bishop, eg_tower, eg_queen, eg_king
    };

    static int[] phase_table = { 0, 0, 1, 1, 2, 4, 0 };
    //eval will calculate total piece value and their position square value for the given board position

    //struct for transposition table value
    struct ttv { public ulong Key; public float value; public int dep; public int bound; public Move move; }
    //transposition table
    static ttv[] ttable = new ttv[20000000];

    static int Cur_phase(Board board)
    {
        int phase = 0;
        foreach (PieceList list in board.GetAllPieceLists())
        {
            foreach (Piece piece in list)
            {
                phase += phase_table[(int)piece.PieceType];
            }
        }
        if (phase > 24) phase = 24;
        return phase;
    }
    static float Eval(Board board)
    {
        float mg_w = 0, eg_w = 0, mg_b = 0, eg_b = 0;
        int phase = Cur_phase(board);
        //iterating over each piece that is in the provided board
        //iterating over each piece that is in the provided board
        foreach (PieceList list in board.GetAllPieceLists())
        {
            foreach (Piece piece in list)
            {
                if (piece.IsWhite)
                {
                    mg_w += mg_table[((int)piece.PieceType) - 1][cor_black[piece.Square.Index]] + mg_piece[(int)piece.PieceType];
                    eg_w += eg_table[((int)piece.PieceType) - 1][cor_black[piece.Square.Index]] + eg_piece[(int)piece.PieceType];
                }
                else
                {
                    mg_b += mg_table[((int)piece.PieceType) - 1][cor_black[piece.Square.Index]] + mg_piece[(int)piece.PieceType];
                    eg_b += eg_table[((int)piece.PieceType) - 1][cor_black[piece.Square.Index]] + eg_piece[(int)piece.PieceType];
                }
            }
        }
        //distribute the point depend on what stage of the game we are in
        //distribute the point depend on what stage of the game we are in
        if (phase > 24) phase = 24;
        int eg_phrase = 24 - phase;
        //positve would mean white is winning, when negative would mean black is winning 
        //positve would mean white is winning, when negative would mean black is winning 
        float score = ((mg_w - mg_b) * phase + (eg_w - eg_b) * eg_phrase) / 24;
        if (!board.IsWhiteToMove) score = score * -1;
        return score;
    }

    //uh yea
    static float Quiesce(Board board, float alpha, float beta)
    {
        if (t_time.MillisecondsElapsedThisTurn > limit) return 0;
        float score = Eval(board);
        if (score >= beta) return beta;
        if (score > alpha) alpha = score;
        // sort so we capture bigger value first
        Move[] cap_move = board.GetLegalMoves(true).OrderByDescending(a => (int)a.CapturePieceType).ToArray<Move>();
        foreach (Move move in cap_move)
        {
            board.MakeMove(move);
            score = -Quiesce(board, -beta, -alpha);
            board.UndoMove(move);
            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }
        return alpha;
    }
    static float NegMax(Board board, float alpha, float beta, int dep)
    {
        if (t_time.MillisecondsElapsedThisTurn > limit || board.IsDraw()) return 0;
        if (board.IsInCheckmate() && board.GetLegalMoves().Length == 0) return float.MinValue + 3;
        if (dep == 0) return Quiesce(board, alpha, beta);
        //zobriskey
        ulong zkey = board.ZobristKey % 20000000;
        ttv value = ttable[zkey];
        if (value.Key == board.ZobristKey && value.dep >= dep)
        {
            if (value.bound == 0 || (value.bound == 1 && value.value <= alpha) || (value.bound == 2 && value.value >= beta))
            {
                return value.value;
            }
        }
        //simple ordering
        Move[] legal_move = board.GetLegalMoves().OrderByDescending(a => (int)a.CapturePieceType + (int)a.PromotionPieceType).ToArray<Move>();
        int bound = 1;
        foreach (Move move in legal_move)
        {
            board.MakeMove(move);
            float score = -NegMax(board, -beta, -alpha, dep - 1);
            board.UndoMove(move);
            if (score >= beta)
            {
                push_tvalue(zkey, board.ZobristKey, beta, dep, 2);
                return beta;
            }
            if (score > alpha)
            {
                bound = 0;
                alpha = score;
            }
        }
        push_tvalue(zkey, board.ZobristKey, alpha, dep, bound);
        return alpha;
    }

    static void push_tvalue(ulong zkey, ulong key, float value, int dep, int bound)
    {
        ttable[zkey].Key = key;
        ttable[zkey].value = value;
        ttable[zkey].dep = dep;
        ttable[zkey].bound = bound;
    }

    static public Move Think(Board board, Timer timer)
    {
        //simple ordering
        Move[] legal_move = board.GetLegalMoves().OrderByDescending(a => (int)a.CapturePieceType + (int)a.PromotionPieceType).ToArray<Move>();
        Move best_move = legal_move[0];
        t_time = timer;
        for (int dep = 1; ; dep++)
        {
            float alpha = float.MinValue + 2;
            float beta = float.MaxValue;
            Move cur_best_move = best_move;
            foreach (Move move in legal_move)
            {
                board.MakeMove(move);
                float score = -NegMax(board, -beta, -alpha, dep);
                board.UndoMove(move);
                if (score >= beta)
                {
                    break;
                }
                if (score > alpha)
                {
                    alpha = score;
                    cur_best_move = move;
                }
                if (timer.MillisecondsElapsedThisTurn > limit) break;
            }
            if (timer.MillisecondsElapsedThisTurn > limit)
            {
                Console.WriteLine($"dep: {dep}");
                break;
            }
            else best_move = cur_best_move;
        }
        return best_move;
    }
}