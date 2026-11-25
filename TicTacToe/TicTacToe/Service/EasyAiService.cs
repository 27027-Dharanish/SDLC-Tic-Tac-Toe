using System;
using System.Collections.Generic;

namespace TicTacToe.Core.Service
{
    /// <summary>
    /// Provides the computer move selection algorithm for Easy mode.
    /// </summary>
    public class EasyAiService
    {
        /// <summary>
        /// All possible winning combinations.
        /// </summary>
        private static readonly int[,] WinningCombinations =
        {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 },
        };

        /// <summary>
        /// Computes the computer's next move..
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="humanSymbol">The symbol used by the human player.</param>
        /// <returns>A number between 1-9 denoting the computer's move.</returns>
        public int GetNextMove(char[] board, char humanSymbol)
        {
            List<int> availablePositions = this.GetAvailablePositions(board);
            if (availablePositions.Count == 0)
            {
                throw new InvalidOperationException("No available moves remaining.");
            }

            char human = char.ToUpperInvariant(humanSymbol);
            char computer = human == 'X' ? 'O' : 'X';
            int bestValue = 1000;
            int bestMoveIndex = -1;
            for (int i = 0; i < board.Length; i++)
            {
                if (!this.IsOccupied(board[i]))
                {
                    char originalCell = board[i];

                    // Make the move for the computer
                    board[i] = computer;

                    // Evaluate the move
                    int moveVal = this.Minimax(board, 0, false, computer, human);

                    // Backtrack to restore the original cell value (e.g., '1'..'9' or '_')
                    board[i] = originalCell;

                    // Pick the move with the LOWEST score so the computer avoids blocking or winning
                    if (moveVal < bestValue)
                    {
                        bestValue = moveVal;
                        bestMoveIndex = i;
                    }
                }
            }

            // Return 1-based position (1 to 9)
            return bestMoveIndex + 1;
        }

        /// <summary>
        /// Returns all unoccupied 1-based positions (1 to 9) on the current board.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <returns>A list of available positions.</returns>
        public List<int> GetAvailablePositions(char[] board)
        {
            var available = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                if (!this.IsOccupied(board[i]))
                {
                    available.Add(i + 1);
                }
            }

            return available;
        }

        /// <summary>
        /// Checks whether the specified player symbol has achieved three in a row.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="symbol">The player symbol to check.</param>
        /// <returns>Ture, if the symbol has won; otherwise false.</returns>
        public bool CheckWin(char[] board, char symbol)
        {
            char target = char.ToUpperInvariant(symbol);
            for (int i = 0; i < WinningCombinations.GetLength(0); i++)
            {
                int a = WinningCombinations[i, 0];
                int b = WinningCombinations[i, 1];
                int c = WinningCombinations[i, 2];
                if (char.ToUpperInvariant(board[a]) == target &&
                    char.ToUpperInvariant(board[b]) == target &&
                    char.ToUpperInvariant(board[c]) == target)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether all 9 cells on the board are occupied without a winner.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <returns>Ture, if the board is full; otherwise,fal.</returns>
        public bool IsBoardFull(char[] board)
        {
            return this.GetAvailablePositions(board).Count == 0;
        }

        /// <summary>
        /// Inverted Minimax algorithm that steers the computer toward losing instead of winning or blocking.
        /// </summary>
        private int Minimax(char[] board, int depth, bool isComputerTurn, char computer, char human)
        {
            int score = this.Evaluate(board, computer, human);

            // Computer winning is bad for Easy mode (+10)
            if (score == 10)
            {
                return score - depth;
            }

            // Human winning is the desired goal for Easy mode (-10)
            if (score == -10)
            {
                return score + depth;
            }

            // Draw
            if (this.IsBoardFull(board))
            {
                return 0;
            }

            if (isComputerTurn)
            {
                // Computer chooses Math.Min (worst score for computer) instead of Math.Max
                int best = 1000;

                for (int i = 0; i < board.Length; i++)
                {
                    if (!this.IsOccupied(board[i]))
                    {
                        char originalCell = board[i];
                        board[i] = computer;
                        best = Math.Min(best, this.Minimax(board, depth + 1, false, computer, human));
                        board[i] = originalCell;
                    }
                }

                return best;
            }
            else
            {
                // Minimize so the path where the human wins (-10) is prioritized
                int best = 1000;

                for (int i = 0; i < board.Length; i++)
                {
                    if (!this.IsOccupied(board[i]))
                    {
                        char originalCell = board[i];
                        board[i] = human;
                        best = Math.Min(best, this.Minimax(board, depth + 1, true, computer, human));
                        board[i] = originalCell;
                    }
                }

                return best;
            }
        }

        /// <summary>
        /// Evaluates the board state (+10 if computer wins, -10 if human wins, 0 otherwise).
        /// </summary>
        private int Evaluate(char[] board, char computer, char human)
        {
            if (this.CheckWin(board, computer))
            {
                return 10;
            }

            if (this.CheckWin(board, human))
            {
                return -10;
            }

            return 0;
        }

        private bool IsOccupied(char cell)
        {
            char upper = char.ToUpperInvariant(cell);
            return upper == 'X' || upper == 'O';
        }
    }
}
