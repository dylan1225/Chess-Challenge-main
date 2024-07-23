using ChessChallenge.API;

public class MyBot : IChessBot
{
	// chess piece values for evaluation, empty, pawn, bishop, horse, rook, queen, king 

	int[] piecevalue = {0, 100, 350, 300, 900, 1000000};

	public Move MinMax(Board board, int depth = 0, int max, int min){
		
	}

    public Move Think(Board board, Timer timer)
    {
        Move[] legal_move = board.GetLegalMoves();
		MinMax(board)
        return moves[0];
    }
}