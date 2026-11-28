using System;
using System.Text.Json.Serialization;
using TicTacToe.Core.Constants;

namespace TicTacToe.Core.Model
{
    /// <summary>
    /// Represents a completed Tic-Tac-Toe match persisted for match history and replay playback.
    /// </summary>
    public class GameRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameRecord"/> class for JSON deserialization.
        /// </summary>
        [JsonConstructor]
        public GameRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRecord"/> class for JSON deserialization.
        /// </summary>
        /// <param name="userId">User id of the login user.</param>
        /// <param name="player1Name">Player 1 name.</param>
        /// <param name="player2Name">Player 2 name.</param>
        /// <param name="mode">Type of mode the gameplay played.</param>
        /// <param name="winnerName">Winner name.</param>
        /// <param name="playedAt">Time of gameplay recorded.</param>
        /// <param name="moves">List of move happen in the board.</param>
        public GameRecord(Guid userId, string player1Name, string player2Name, GameMode mode, string winnerName, DateTime playedAt, int[] moves)
        {
            this.GameId = Guid.NewGuid();
            this.UserId = userId;
            this.Player1Name = player1Name;
            this.Player2Name = player2Name;
            this.Mode = mode;
            this.WinnerName = winnerName;
            this.PlayedAt = playedAt;
            this.Moves = moves;
        }

        /// <summary>
        /// Gets or sets the unique match identifier.
        /// </summary>
        /// <value>
        /// A <see cref="Guid"/> uniquely identifying the match.
        /// </value>
        public Guid GameId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique identifier of the user who played the match.
        /// </summary>
        /// <value>
        /// The <see cref="Guid"/> associated with the user account.
        /// </value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets Player 1's username.
        /// </summary>
        /// <value>
        /// The username of the primary player.
        /// </value>
        public string Player1Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Player 2's display name (e.g., <c>"Computer"</c>).
        /// </summary>
        /// <value>
        /// The display name of the opponent.
        /// </value>
        public string Player2Name { get; set; } = "Computer";

        /// <summary>
        /// Gets or sets the gameplay mode used in this match.
        /// </summary>
        /// <value>
        /// A <see cref="GameMode"/> value indicating the match type or difficulty.
        /// </value>
        public GameMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the winner's name, or <c>"Draw"</c> if the match ended in a tie.
        /// </summary>
        /// <value>
        /// The winning player's name or <c>"Draw"</c>.
        /// </value>
        public string WinnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the UTC timestamp when the match was completed.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime"/> in UTC when the game finished.
        /// </value>
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the sequential board positions (1 to 9) chosen during the match.
        /// Even indices (0, 2, 4...) represent Player X moves, and odd indices (1, 3, 5...) represent Player O moves.
        /// </summary>
        /// <value>
        /// An array of integers representing the ordered board positions played.
        /// </value>
        public int[] Moves { get; set; } = Array.Empty<int>();
    }
}
