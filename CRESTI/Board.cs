using System;

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


        public EventHandler GameFinished;
        private int countOfMarkedCells;

        public Cell[,] Cells { get; private set; }


        public GameState State { get; private set; }


        public PlayerType Turn { get; private set; }


        public PlayerType? Winner { get; private set; }


        public Board()
        {
            Restart();
        }


        public void Restart()
        {
            countOfMarkedCells = 0;
            Turn = PlayerType.X;
            Winner = null;
            Cells = new Cell[boardSize, boardSize];
            cleanBoard();
            State = GameState.InProgress;
        }


        public void Mark(int row, int col)
        {
            if (isCellValid(row, col) && (State == GameState.InProgress))
            {
                countOfMarkedCells += 1;
                Cells[row, col].Value = Turn;
                if (isPlayerWin(Turn, row, col))
                {
                    Winner = Turn;
                    State = GameState.Finished;
                    GameFinished?.Invoke(this, null);
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


        private bool isPlayerWin(PlayerType playerType, int curRow, int curCol)
        {
            return ((Cells[curRow, 0].Value == playerType)
                    && (Cells[curRow, 1].Value == playerType)
                    && (Cells[curRow, 2].Value == playerType))
                   || ((Cells[0, curCol].Value == playerType)
                       && (Cells[1, curCol].Value == playerType)
                       && (Cells[2, curCol].Value == playerType))
                   || ((curRow == curCol)
                       && (Cells[0, 0].Value == playerType)
                       && (Cells[1, 1].Value == playerType)
                       && (Cells[2, 2].Value == playerType))
                   || ((curRow + curCol == 2)
                       && (Cells[0, 2].Value == playerType)
                       && (Cells[1, 1].Value == playerType)
                       && (Cells[2, 0].Value == playerType));
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


        private void changeTurn()
        {
            if (Turn == PlayerType.O)
            {
                Turn = PlayerType.X;
            }
            else
            {
                Turn = PlayerType.O;
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
    }
}