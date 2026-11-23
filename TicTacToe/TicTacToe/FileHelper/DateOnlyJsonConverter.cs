using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TicTacToe.Helper
{
    /// <summary>
    /// Custom JSON converter to support <see cref="DateOnly"/> serialization and deserialization in .NET 6.
    /// </summary>
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        /// <summary>
        /// The standard ISO date format used for JSON persistence.
        /// </summary>
        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Reads and converts the JSON string representation into a <see cref="DateOnly"/> value.
        /// </summary>
        /// <param name="reader">The UTF-8 JSON reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>The parsed <see cref="DateOnly"/> value.</returns>
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            return DateOnly.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Writes a <see cref="DateOnly"/> value as a formatted JSON string.
        /// </summary>
        /// <param name="writer">The UTF-8 JSON writer.</param>
        /// <param name="value">The <see cref="DateOnly"/> value to write.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}
