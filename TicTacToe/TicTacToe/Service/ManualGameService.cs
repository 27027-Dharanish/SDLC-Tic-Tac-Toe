using System.Collections.Generic;

namespace TicTacToe.Core.Service
{
    /// <summary>
    /// Provides board rule evaluation and available position lookups for 2-player Manual mode.
    /// </summary>
    public class ManualGameService
    {
        /// <summary>
        /// All possible winning combinations on a 3x3 board.
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
        /// Returns all unoccupied 1-based positions (1 to 9) on the current board.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <returns>A list of available positions.</returns>
        public List<int> GetAvailablePositions(char[] board)
        {
            var available = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                char upper = char.ToUpperInvariant(board[i]);
                if (upper != 'X' && upper != 'O')
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
        /// <param name="symbol">The player symbol (<c>'X'</c> or <c>'O'</c>) to check.</param>
        /// <returns><c>true</c> if the symbol has won; otherwise, <c>false</c>.</returns>
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
        /// <returns><c>true</c> if the board is full; otherwise, <c>false</c>.</returns>
        public bool IsBoardFull(char[] board)
        {
            return this.GetAvailablePositions(board).Count == 0;
        }
    }
}
