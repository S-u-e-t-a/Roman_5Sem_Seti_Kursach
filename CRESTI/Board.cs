namespace CRESTI
{
    public enum GameState
    {
        InProgress,
        Finished
    }

    public class Board
    {
        private const int boardSize = 3;
        private int countOfMarkedCells;


        public Board()
        {
            Restart();
        }


        public GameState State { get; private set; }


        public Player Turn { get; private set; }


        public Cell[,] Cells { get; private set; }


        public Player? Winner { get; private set; }


        public void Restart()
        {
            countOfMarkedCells = 0;
            Turn = Player.X;
            Winner = null;
            Cells = new Cell[boardSize, boardSize];
            cleanBoard();
            State = GameState.InProgress;
        }


        private void cleanBoard()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    Cells[i, j] = new Cell();
                }
            }
        }


        public void Mark(int row, int col)
        {
            //Trace.WriteLine(isCellValid(row, col));
            if (isCellValid(row, col) && (State == GameState.InProgress))
            {
                countOfMarkedCells += 1;
                Cells[row, col].Value = Turn;
                if (isPlayerWin(Turn, row, col))
                {
                    Winner = Turn;
                    State = GameState.Finished;
                }
                else
                {
                    if (countOfMarkedCells == boardSize * boardSize)
                    {
                        State = GameState.Finished;
                    }

                    changeTurn();
                }
            }
        }


        private void changeTurn()
        {
            if (Turn == Player.O)
            {
                Turn = Player.X;
            }
            else
            {
                Turn = Player.O;
            }
        }


        private bool isCellValid(int row, int col)
        {
            return !isCellMarked(row, col) && !isOutOfBounds(row, col);
        }


        private bool isOutOfBounds(int row, int col)
        {
            return (row < 0) || (row > boardSize - 1) || (col < 0) || (col > boardSize - 1);
        }


        private bool isCellMarked(int row, int col)
        {
            return Cells[row, col].Value != null;
        }


        private void printCell()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    //Trace.Write(Cells[i, j].Value);
                    //Trace.Write(" ");
                }

                //Trace.WriteLine(null);
            }
        }


        public bool isPlayerWin(Player player, int curRow, int curCol)
        {
            printCell();

            return ((Cells[curRow, 0].Value == player)
                    && (Cells[curRow, 1].Value == player)
                    && (Cells[curRow, 2].Value == player))
                   || ((Cells[0, curCol].Value == player)
                       && (Cells[1, curCol].Value == player)
                       && (Cells[2, curCol].Value == player))
                   || ((curRow == curCol)
                       && (Cells[0, 0].Value == player)
                       && (Cells[1, 1].Value == player)
                       && (Cells[2, 2].Value == player))
                   || ((curRow + curCol == 2)
                       && (Cells[0, 2].Value == player)
                       && (Cells[1, 1].Value == player)
                       && (Cells[2, 0].Value == player));
        }
    }
}