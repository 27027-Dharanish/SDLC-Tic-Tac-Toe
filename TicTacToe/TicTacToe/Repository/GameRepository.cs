using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Helpers;
using TicTacToe.Core.Model;

namespace TicTacToe.Core.Repository
{
    /// <summary>
    /// Handles persistence and retrieval of <see cref="GameRecord"/> match history data using JSON storage.
    /// </summary>
    public class GameRepository
    {
        /// <summary>
        /// The file path where game match records are stored.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRepository"/> class with the default storage path.
        /// </summary>
        public GameRepository()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "games.json"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRepository"/> class with a custom JSON file path.
        /// </summary>
        /// <param name="filePath">The path to the JSON file used for storing game records.</param>
        public GameRepository(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty.", nameof(filePath));
            }

            this._filePath = filePath;
        }

        /// <summary>
        /// Creates and persists a new match record to the JSON file.
        /// </summary>
        /// <param name="record">The <see cref="GameRecord"/> to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is <see langword="null"/>.</exception>
        public void AddGameRecord(GameRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            List<GameRecord> records = this.GetAllGames();
            records.Add(record);
            JsonHelper.SerializeToFile(this._filePath, records);
        }

        /// <summary>
        /// Creates a new <see cref="GameRecord"/> from match parameters and persists it to the JSON file.
        /// </summary>
        /// <param name="userId">The unique identifier of the logged-in user.</param>
        /// <param name="player1Name">Player 1's display name.</param>
        /// <param name="player2Name">Player 2's display name.</param>
        /// <param name="mode">The game mode in which the match was played.</param>
        /// <param name="winnerName">The winner's name or <c>"Draw"</c>.</param>
        /// <param name="moves">The sequential array of board positions (1 to 9) played.</param>
        /// <returns>The newly created and saved <see cref="GameRecord"/>.</returns>
        public GameRecord CreateAndSaveGame(Guid userId, string player1Name, string player2Name, GameMode mode, string winnerName, int[] moves)
        {
            var record = new GameRecord(userId, player1Name, player2Name, mode, winnerName, DateTime.UtcNow, moves);
            this.AddGameRecord(record);
            return record;
        }

        /// <summary>
        /// Retrieves all saved match records from storage.
        /// </summary>
        /// <returns>A list of all <see cref="GameRecord"/> entries.</returns>
        public List<GameRecord> GetAllGames()
        {
            return JsonHelper.DeserializeFromFile<List<GameRecord>>(this._filePath);
        }

        /// <summary>
        /// Retrieves a specific match record by its unique game identifier.
        /// </summary>
        /// <param name="gameId">The unique <see cref="Guid"/> of the match.</param>
        /// <returns>The matching <see cref="GameRecord"/> if found; otherwise, <see langword="null"/>.</returns>
        public GameRecord? GetGameById(Guid gameId)
        {
            return this.GetAllGames().FirstOrDefault(g => g.GameId == gameId);
        }

        /// <summary>
        /// Retrieves all match records played by a specific user, ordered from newest to oldest.
        /// </summary>
        /// <param name="userId">The unique <see cref="Guid"/> of the user.</param>
        /// <returns>A list of <see cref="GameRecord"/> entries belonging to the user.</returns>
        public List<GameRecord> GetGamesByUserId(Guid userId)
        {
            return this.GetAllGames()
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.PlayedAt)
                .ToList();
        }

        /// <summary>
        /// Retrieves all match records for a specific user filtered by game mode, ordered from newest to oldest.
        /// </summary>
        /// <param name="userId">The unique <see cref="Guid"/> of the user.</param>
        /// <param name="mode">The <see cref="GameMode"/> to filter by.</param>
        /// <returns>A filtered list of <see cref="GameRecord"/> entries.</returns>
        public List<GameRecord> GetGamesByUserIdAndMode(Guid userId, GameMode mode)
        {
            return this.GetAllGames()
                .Where(g => g.UserId == userId && g.Mode == mode)
                .OrderByDescending(g => g.PlayedAt)
                .ToList();
        }
    }
}
