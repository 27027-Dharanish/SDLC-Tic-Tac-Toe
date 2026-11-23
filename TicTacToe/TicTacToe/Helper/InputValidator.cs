using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace TicTacToe.View
{
    /// <summary>
    /// Provides immediate console input validation helpers using Spectre.Console prompts.
    /// Ensures input format and basic field constraints are valid at entry time.
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Prompts for and validates a username (3–20 characters, letters, numbers, and underscores only).
        /// </summary>
        /// <param name="prompt">The prompt message displayed to the user.</param>
        /// <returns>A validated, trimmed username string.</returns>
        public static string GetValidatedUserName(string prompt = "[cyan]Enter username:[/]")
        {
            ConsoleActivity.PrintEmptyLine();
            return AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .Validate(input =>
                    {
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            return ValidationResult.Error("[red]Username cannot be empty.[/]");
                        }

                        string trimmed = input.Trim();
                        if (trimmed.Length < 3 || trimmed.Length > 20)
                        {
                            return ValidationResult.Error("[red]Username must be between 3 and 20 characters.[/]");
                        }

                        if (!Regex.IsMatch(trimmed, @"^[a-zA-Z0-9_]+$"))
                        {
                            return ValidationResult.Error("[red]Only letters, numbers, and underscores are allowed.[/]");
                        }

                        return ValidationResult.Success();
                    }))
                .Trim();
        }

        /// <summary>
        /// Prompts for and validates a password (minimum 4 characters, no spaces).
        /// </summary>
        /// <param name="prompt">The prompt message displayed to the user.</param>
        /// <returns>A validated password string.</returns>
        public static string GetValidatedPassword(string prompt = "[cyan]Enter password:[/]")
        {
            ConsoleActivity.PrintEmptyLine();
            return AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .Secret('*')
                    .Validate(input =>
                    {
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            return ValidationResult.Error("[red]Password cannot be empty.[/]");
                        }

                        if (input.Length < 4)
                        {
                            return ValidationResult.Error("[red]Password must be at least 4 characters long.[/]");
                        }

                        if (input.Contains(' '))
                        {
                            return ValidationResult.Error("[red]Password cannot contain spaces.[/]");
                        }

                        return ValidationResult.Success();
                    }));
        }

        /// <summary>
        /// Prompts the user to confirm their password and validates immediately that it matches <paramref name="expectedPassword"/>.
        /// </summary>
        /// <param name="expectedPassword">The original password to match against.</param>
        /// <param name="prompt">The prompt message displayed to the user.</param>
        /// <returns>The confirmed password string.</returns>
        public static string GetConfirmedPassword(string expectedPassword, string prompt = "[cyan]Confirm password:[/]")
        {
            ConsoleActivity.PrintEmptyLine();
            return AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .Secret('*')
                    .Validate(input =>
                    {
                        if (input != expectedPassword)
                        {
                            return ValidationResult.Error("[red]Passwords do not match. Please try again.[/]");
                        }

                        return ValidationResult.Success();
                    }));
        }

        /// <summary>
        /// Prompts for and validates a date of birth in <c>yyyy-MM-dd</c> format, ensuring it is a valid past date.
        /// </summary>
        /// <param name="prompt">The prompt message displayed to the user.</param>
        /// <returns>A validated <see cref="DateOnly"/> value.</returns>
        public static DateOnly GetValidatedDateOfBirth(string prompt = "[cyan]Enter date of birth ([yellow]yyyy-MM-dd[/]):[/]")
        {
            ConsoleActivity.PrintEmptyLine();
            string rawDate = AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .Validate(input =>
                    {
                        if (!DateOnly.TryParseExact(
                            input.Trim(),
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateOnly parsedDate))
                        {
                            return ValidationResult.Error("[red]Invalid format. Use yyyy-MM-dd (e.g., 2002-08-15).[/]");
                        }

                        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                        if (parsedDate >= today)
                        {
                            return ValidationResult.Error("[red]Date of birth must be in the past.[/]");
                        }

                        if (parsedDate < today.AddYears(-120))
                        {
                            return ValidationResult.Error("[red]Please enter a realistic date of birth.[/]");
                        }

                        return ValidationResult.Success();
                    }));

            return DateOnly.ParseExact(rawDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
