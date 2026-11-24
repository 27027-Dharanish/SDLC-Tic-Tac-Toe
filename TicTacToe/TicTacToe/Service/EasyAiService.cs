using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Core.Service
{
    /// <summary>
    /// Provides the AI move selection algorithm for Easy difficulty and core board evaluation rules.
    /// Deliberately avoids blocking the human player's winning moves whenever non-blocking cells exist.
    /// </summary>
    public class EasyAiService
    {
        /// <summary>
        /// Random number generator used to select casual AI moves.
        /// </summary>
        private readonly Random _random = new Random();

        /// <summary>
        /// All possible 3-in-a-row winning index combinations on a 0-based 9-cell board.
        /// </summary>
        private static readonly int[,] WinningCombinations =
        {
            { 0, 1, 2 }, // Top row
            { 3, 4, 5 }, // Middle row
            { 6, 7, 8 }, // Bottom row
            { 0, 3, 6 }, // Left column
            { 1, 4, 7 }, // Center column
            { 2, 5, 8 }, // Right column
            { 0, 4, 8 }, // Main diagonal
            { 2, 4, 6 }  // Anti-diagonal
        };

        /// <summary>
        /// Computes the computer's next move for Easy mode by avoiding any cell that would block
        /// the human player from completing three in a row.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="humanSymbol">The symbol (<c>'X'</c> or <c>'O'</c>) used by the human player.</param>
        /// <returns>A 1-based board position (1 to 9) for the computer's move.</returns>
        /// <exception cref="ArgumentException">Thrown when the board or human symbol is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no available moves remain on the board.</exception>
        public int GetNextMove(char[] board, char humanSymbol)
        {
            if (board == null || board.Length != 9)
            {
                throw new ArgumentException("Board must contain exactly 9 cells.", nameof(board));
            }

            if (humanSymbol != 'X' && humanSymbol != 'O')
            {
                throw new ArgumentException("Human symbol must be 'X' or 'O'.", nameof(humanSymbol));
            }

            List<int> availablePositions = GetAvailablePositions(board);

            if (availablePositions.Count == 0)
            {
                throw new InvalidOperationException("No available moves remaining on the board.");
            }

            HashSet<int> humanWinningPositions = GetThreateningPositions(board, humanSymbol);

            List<int> nonBlockingPositions = availablePositions
                .Where(pos => !humanWinningPositions.Contains(pos))
                .ToList();

            List<int> candidateMoves = nonBlockingPositions.Count > 0
                ? nonBlockingPositions
                : availablePositions;

            int randomIndex = this._random.Next(candidateMoves.Count);
            return candidateMoves[randomIndex];
        }

        /// <summary>
        /// Finds all 1-based open positions that would immediately complete three in a row for the given symbol.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="symbol">The player symbol (<c>'X'</c> or <c>'O'</c>) to evaluate threats for.</param>
        /// <returns>A set of 1-based board positions that complete a winning line.</returns>
        public HashSet<int> GetThreateningPositions(char[] board, char symbol)
        {
            var threats = new HashSet<int>();

            for (int i = 0; i < WinningCombinations.GetLength(0); i++)
            {
                int a = WinningCombinations[i, 0];
                int b = WinningCombinations[i, 1];
                int c = WinningCombinations[i, 2];

                int symbolCount = 0;
                int openIndex = -1;

                if (board[a] == symbol)
                {
                    symbolCount++;
                }
                else if (board[a] != 'X' && board[a] != 'O')
                {
                    openIndex = a;
                }

                if (board[b] == symbol)
                {
                    symbolCount++;
                }
                else if (board[b] != 'X' && board[b] != 'O')
                {
                    openIndex = b;
                }

                if (board[c] == symbol)
                {
                    symbolCount++;
                }
                else if (board[c] != 'X' && board[c] != 'O')
                {
                    openIndex = c;
                }

                if (symbolCount == 2 && openIndex != -1)
                {
                    threats.Add(openIndex + 1);
                }
            }

            return threats;
        }

        /// <summary>
        /// Returns all unoccupied 1-based positions (1 to 9) on the current board.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <returns>A list of available 1-based positions.</returns>
        public List<int> GetAvailablePositions(char[] board)
        {
            var available = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] != 'X' && board[i] != 'O')
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
        /// <returns><see langword="true"/> if the symbol has won; otherwise, <see langword="false"/>.</returns>
        public bool CheckWin(char[] board, char symbol)
        {
            for (int i = 0; i < WinningCombinations.GetLength(0); i++)
            {
                int a = WinningCombinations[i, 0];
                int b = WinningCombinations[i, 1];
                int c = WinningCombinations[i, 2];

                if (board[a] == symbol && board[b] == symbol && board[c] == symbol)
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
        /// <returns><see langword="true"/> if the board is full; otherwise, <see langword="false"/>.</returns>
        public bool IsBoardFull(char[] board)
        {
            return GetAvailablePositions(board).Count == 0;
        }
    }
}
