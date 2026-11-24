namespace TicTacToe.Core.Constants
{
    /// <summary>
    /// Defines the available gameplay modes for a Tic-Tac-Toe match.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Single-player mode against a casual AI that makes random valid moves.
        /// </summary>
        Easy,

        /// <summary>
        /// Single-player mode against a balanced AI that blocks immediate threats and takes winning moves.
        /// </summary>
        Medium,

        /// <summary>
        /// Single-player mode against an unbeatable AI powered by the Minimax algorithm.
        /// </summary>
        Impossible,

        /// <summary>
        /// Local two-player mode where two human players take turns on the same console.
        /// </summary>
        Manual,
    }
}
