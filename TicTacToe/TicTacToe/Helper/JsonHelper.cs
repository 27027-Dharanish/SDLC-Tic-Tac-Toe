using System;
using System.IO;
using System.Text.Json;

namespace TicTacToe.Core.Helpers
{
    /// <summary>
    /// Provides helper methods for serializing objects to and deserializing objects from JSON files.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Configuration options used for JSON serialization and deserialization.
        /// </summary>
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        /// <summary>
        /// Serializes an object to JSON and saves it to the specified file path.
        /// </summary>
        /// <typeparam name="T">Type of the data to be written.</typeparam>
        /// <param name="filePath">Actual file path.</param>
        /// <param name="data">The content to be written in file.</param>
        public static void SerializeToFile<T>(string filePath, T data)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty.", nameof(filePath));
            }

            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonString = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// Reads a JSON file and deserializes its content into the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the data to be written.</typeparam>
        /// <param name="filePath">Actual file path.</param>
        /// <returns>The content from the file.</returns>
        public static T DeserializeFromFile<T>(string filePath)
            where T : new()
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                return new T();
            }

            string jsonString = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return new T();
            }

            return JsonSerializer.Deserialize<T>(jsonString, _options) ?? new T();
        }
    }
}
