using System;
using System.Collections.Generic;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Model;
using TicTacToe.Core.Repository;

namespace TicTacToe.Core.Service
{
    /// <summary>
    /// Acts as an intermediate business logic layer between controllers and <see cref="GameRepository"/>
    /// for recording matches, retrieving history, and reconstructing replay board states.
    /// </summary>
    public class GameService
    {
        /// <summary>
        /// The repository instance used for match data persistence.
        /// </summary>
        private readonly GameRepository _gameRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class with a default repository.
        /// </summary>
        public GameService()
            : this(new GameRepository())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class with the specified repository.
        /// </summary>
        /// <param name="gameRepository">The <see cref="GameRepository"/> used for data access.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="gameRepository"/> is <see langword="null"/>.</exception>
        public GameService(GameRepository gameRepository)
        {
            this._gameRepository = gameRepository;
        }

        /// <summary>
        /// Validates match details and saves a completed match record via the repository.
        /// </summary>
        /// <param name="userId">The unique identifier of the logged-in user.</param>
        /// <param name="player1Name">Player 1's display name.</param>
        /// <param name="player2Name">Player 2's display name.</param>
        /// <param name="mode">The game mode in which the match was played.</param>
        /// <param name="winnerName">The winner's name or <c>"Draw"</c>.</param>
        /// <param name="moves">The sequential array of board positions (1 to 9) played.</param>
        /// <returns>The persisted <see cref="GameRecord"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when user ID, player names, winner name, or moves are invalid.</exception>
        public GameRecord SaveMatch(
            Guid userId,
            string player1Name,
            string player2Name,
            GameMode mode,
            string winnerName,
            int[] moves)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(player1Name))
            {
                throw new ArgumentException("Player 1 name cannot be empty.", nameof(player1Name));
            }

            if (string.IsNullOrWhiteSpace(player2Name))
            {
                throw new ArgumentException("Player 2 name cannot be empty.", nameof(player2Name));
            }

            if (string.IsNullOrWhiteSpace(winnerName))
            {
                throw new ArgumentException("Winner name cannot be empty.", nameof(winnerName));
            }

            if (moves == null || moves.Length == 0 || moves.Length > 9)
            {
                throw new ArgumentException("A valid match must contain between 1 and 9 moves.", nameof(moves));
            }

            foreach (int position in moves)
            {
                if (position < 1 || position > 9)
                {
                    throw new ArgumentOutOfRangeException(nameof(moves), $"Invalid board position '{position}'. Positions must be between 1 and 9.");
                }
            }

            return this._gameRepository.CreateAndSaveGame(
                userId,
                player1Name.Trim(),
                player2Name.Trim(),
                mode,
                winnerName.Trim(),
                moves);
        }

        /// <summary>
        /// Retrieves the match history for a specific user, ordered from newest to oldest.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of <see cref="GameRecord"/> items played by the user.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is empty.</exception>
        public List<GameRecord> GetUserMatchHistory(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return this._gameRepository.GetGamesByUserId(userId);
        }

        /// <summary>
        /// Retrieves the match history for a specific user filtered by game mode.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="mode">The <see cref="GameMode"/> to filter by.</param>
        /// <returns>A filtered list of <see cref="GameRecord"/> items.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is empty.</exception>
        public List<GameRecord> GetUserMatchHistoryByMode(Guid userId, GameMode mode)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return this._gameRepository.GetGamesByUserIdAndMode(userId, mode);
        }

        /// <summary>
        /// Retrieves a single match record by its unique identifier for replay or inspection.
        /// </summary>
        /// <param name="gameId">The unique identifier of the match.</param>
        /// <returns>The matching <see cref="GameRecord"/> if found; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="gameId"/> is empty.</exception>
        public GameRecord? GetMatchForReplay(Guid gameId)
        {
            if (gameId == Guid.Empty)
            {
                throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));
            }

            return this._gameRepository.GetGameById(gameId);
        }

        /// <summary>
        /// Generates step-by-step 9-cell board snapshots from a <see cref="GameRecord"/> for replay playback.
        /// Even move indices (0, 2, 4...) place <c>'X'</c> and odd indices (1, 3, 5...) place <c>'O'</c>.
        /// </summary>
        /// <param name="record">The <see cref="GameRecord"/> containing the sequential moves.</param>
        /// <returns>A list of 9-character board states representing the board after each turn.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is <see langword="null"/>.</exception>
        public List<char[]> BuildReplayFrames(GameRecord record)
        {
            if (record == null)
            { throw new ArgumentNullException(nameof(record)); }

            var frames = new List<char[]>();
            char[] currentBoard = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < record.Moves.Length; i++)
            {
                int position = record.Moves[i];
                if (position >= 1 && position <= 9)
                {
                    char symbol = (i % 2 == 0) ? 'X' : 'O';
                    currentBoard[position - 1] = symbol;
                }

                // Clone the current board state so each frame remains independent
                frames.Add((char[])currentBoard.Clone());
            }

            return frames;
        }
    }
}
