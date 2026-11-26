using System;
using System.Collections.Generic;

namespace TicTacToe.Core.Service
{
    /// <summary>
    /// Provides the computer move selection strategy for Medium mode.
    /// </summary>
    public class MediumAiService
    {
        private readonly EasyAiService _easyAiService;
        private readonly HardAiService _hardAiService;
        private bool _useEasyNext = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediumAiService"/> class.
        /// </summary>
        /// <param name="easyAiService">The Easy AI service.</param>
        /// <param name="hardAiService">The Hard AI service.</param>
        public MediumAiService(EasyAiService easyAiService, HardAiService hardAiService)
        {
            this._easyAiService = easyAiService ?? throw new ArgumentNullException(nameof(easyAiService));
            this._hardAiService = hardAiService ?? throw new ArgumentNullException(nameof(hardAiService));
        }

        /// <summary>
        /// Resets the alternating move state at the start of a new round so the first computer move uses <see cref="EasyAiService"/>.
        /// </summary>
        public void ResetRoundState()
        {
            this._useEasyNext = true;
        }

        /// <summary>
        /// Computes the computer's next move by alternating between <see cref="EasyAiService"/> and <see cref="HardAiService"/>.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="humanSymbol">The symbol used by the human player.</param>
        /// <returns>A number between 1 and 9 denoting the computer's move.</returns>
        public int GetNextMove(char[] board, char humanSymbol)
        {
            int move = this._useEasyNext
                ? this._easyAiService.GetNextMove(board, humanSymbol)
                : this._hardAiService.GetNextMove(board, humanSymbol);
            this._useEasyNext = !this._useEasyNext;
            return move;
        }

        /// <summary>
        /// Returns all unoccupied 1-based positions (1 to 9) on the current board.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <returns>A list of available positions.</returns>
        public List<int> GetAvailablePositions(char[] board)
        {
            return this._hardAiService.GetAvailablePositions(board);
        }

        /// <summary>
        /// Checks whether the specified player symbol has achieved three in a row.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="symbol">The player symbol to check.</param>
        /// <returns><c>true</c> if the symbol has won; otherwise, <c>false</c>.</returns>
        public bool CheckWin(char[] board, char symbol)
        {
            return this._hardAiService.CheckWin(board, symbol);
        }

        /// <summary>
        /// Checks whether all 9 cells on the board are occupied without a winner.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <returns><c>true</c> if the board is full; otherwise, <c>false</c>.</returns>
        public bool IsBoardFull(char[] board)
        {
            return this._hardAiService.IsBoardFull(board);
        }
    }
}
