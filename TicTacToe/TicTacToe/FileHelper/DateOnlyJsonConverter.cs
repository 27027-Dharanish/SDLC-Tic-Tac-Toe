using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicTacToe.Helper
{
    /// <summary>
    /// Custom JSON converter for <see cref="DateOnly"/> serialization and deserialization in .NET 6.
    /// </summary>
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private static readonly string[] AcceptedFormats = { "dd-MM-yyyy", "yyyy-MM-dd" };

        /// <summary>
        /// Reads and converts a JSON date string (<c>dd-MM-yyyy</c> or <c>yyyy-MM-dd</c>) into a <see cref="DateOnly"/> value.
        /// </summary>
        /// <param name="reader">The UTF-8 JSON reader.</param>
        /// <param name="typeToConvert">The target type to convert.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>The parsed <see cref="DateOnly"/> value, or <c>default</c> if the JSON string is empty.</returns>
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            if (DateOnly.TryParseExact(value.Trim(), AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly result))
            {
                return result;
            }

            throw new JsonException($"Unable to convert '{value}' to DateOnly.");
        }

        /// <summary>
        /// Writes a <see cref="DateOnly"/> value as a <c>dd-MM-yyyy</c> formatted JSON string.
        /// </summary>
        /// <param name="writer">The UTF-8 JSON writer.</param>
        /// <param name="value">The <see cref="DateOnly"/> value to write.</param>
        /// <param name="options">The JSON serializer options.</param>
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
        }
    }
}
