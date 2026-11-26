using System;
using System.Globalization;
using Spectre.Console;

namespace TicTacToe.View
{
    /// <summary>
    /// Provides Spectre.Console-based UI helper methods for rendering menus, prompts, banners, and status panels.
    /// </summary>
    public static class ConsoleActivity
    {
        /// <summary>
        /// Writes a formatted Spectre.Console markup line to the console.
        /// </summary>
        /// <param name="content">The Spectre markup text to print.</param>
        public static void PrintInConsole(string content)
        {
            AnsiConsole.MarkupLine(content);
        }

        /// <summary>
        /// Prompts the user for a non-empty text input.
        /// </summary>
        /// <param name="prompt">The prompt label displayed to the user.</param>
        /// <returns>The trimmed string entered by the user.</returns>
        public static string GetStringInput(string prompt)
        {
            PrintEmptyLine();
            return AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Input cannot be empty.[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)))
                .Trim();
        }

        /// <summary>
        /// Prompts the user for a masked secret input (such as a password).
        /// </summary>
        /// <param name="prompt">The prompt label displayed to the user.</param>
        /// <returns>The secret string entered by the user.</returns>
        public static string GetSecretInput(string prompt)
        {
            PrintEmptyLine();
            return AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .Secret('*')
                    .ValidationErrorMessage("[red]Password cannot be empty.[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));
        }

        /// <summary>
        /// Prompts the user for a valid date in <c>yyyy-MM-dd</c> format.
        /// </summary>
        /// <param name="prompt">The prompt label displayed to the user.</param>
        /// <returns>A parsed <see cref="DateOnly"/> value.</returns>
        public static DateOnly GetDateOnlyInput(string prompt)
        {
            PrintEmptyLine();
            string rawDate = AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Invalid date format. Please use yyyy-MM-dd (e.g., 2002-08-15).[/]")
                    .Validate(input =>
                        DateOnly.TryParseExact(
                            input.Trim(),
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out _)));

            return DateOnly.ParseExact(rawDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Displays an interactive arrow-key selection menu and returns the chosen option.
        /// </summary>
        /// <param name="title">The title shown above the menu choices.</param>
        /// <param name="options">The list of selectable menu options.</param>
        /// <returns>The selected option string.</returns>
        public static string ShowSelectionMenu(string title, string[] options)
        {
            PrintEmptyLine();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(options));
        }

        /// <summary>
        /// Displays a Yes/No confirmation prompt using arrow keys.
        /// </summary>
        /// <param name="prompt">The confirmation question.</param>
        /// <returns><c>true</c> if the user selects Yes; otherwise, <c>false</c>.</returns>
        public static bool ConfirmAction(string prompt)
        {
            PrintEmptyLine();
            return AnsiConsole.Confirm(prompt);
        }

        /// <summary>
        /// Pauses execution until the user presses any key.
        /// </summary>
        public static void WaitInConsole()
        {
            PrintEmptyLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Displays a red error panel and waits for user acknowledgment.
        /// </summary>
        /// <param name="content">The error message to display.</param>
        public static void PrintInvalidMessage(string content)
        {
            PrintEmptyLine();
            var panel = new Panel($"[bold red] {Markup.Escape(content)}[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
            };
            AnsiConsole.Write(panel);
            WaitInConsole();
        }

        /// <summary>
        /// Displays a green success panel and waits for user acknowledgment.
        /// </summary>
        /// <param name="content">The success message to display.</param>
        public static void PrintSuccessMessage(string content)
        {
            PrintEmptyLine();
            var panel = new Panel($"[bold green] {Markup.Escape(content)}[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green),
            };
            AnsiConsole.Write(panel);
            WaitInConsole();
        }

        /// <summary>
        /// Writes a blank line to the console.
        /// </summary>
        public static void PrintEmptyLine()
        {
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Clears the console and renders an optional ASCII Fillet banner along with a styled rule header.
        /// </summary>
        /// <param name="header">The section header text.</param>
        /// <param name="bannerTitle">Optional large banner text rendered above the header.</param>
        public static void ShowHeader(string header, string? bannerTitle = null)
        {
            ClearConsole();

            if (!string.IsNullOrWhiteSpace(bannerTitle))
            {
                AnsiConsole.Write(
                    new FigletText(bannerTitle)
                        .Centered()
                        .Color(Color.Cyan1));
            }

            var rule = new Rule($"[bold yellow]{Markup.Escape(header)}[/]")
            {
                Justification = Justify.Center,
                Style = Style.Parse("cyan"),
            };
            AnsiConsole.Write(rule);
            PrintEmptyLine();
        }

        /// <summary>
        /// Clears all content from the console screen.
        /// </summary>
        public static void ClearConsole()
        {
            AnsiConsole.Clear();
        }

        /// <summary>
        /// Displays a goodbye panel before exiting the application.
        /// </summary>
        public static void ExitApplication()
        {
            ClearConsole();
            var panel = new Panel("[bold cyan]Thank you for playing Tic Tac Toe! Goodbye.[/]")
            {
                Border = BoxBorder.Double,
                Padding = new Padding(2, 1),
                BorderStyle = new Style(Color.Cyan1),
            };
            AnsiConsole.Write(Align.Center(panel));
            WaitInConsole();
        }

        /// <summary>
        /// Displays an arrow-key move selection menu that also supports pressing <c>Q</c> to quit the match
        /// after user confirmation.
        /// </summary>
        /// <param name="title">The prompt title displayed above the choices.</param>
        /// <param name="moveChoices">The available position choices (e.g., <c>"Position 1"</c>).</param>
        /// <returns>The chosen move string, or <see langword="null"/> if the player confirmed quitting.</returns>
        public static string? ShowMoveSelectionMenu(string title, string[] moveChoices)
        {
            const string quitOption = "[red][[Q]] Quit Match[/]";
            var allChoices = moveChoices.Append(quitOption).ToList();

            while (true)
            {
                int selectedIndex = 0;

                // Render interactive menu with live Q-key detection
                while (true)
                {
                    AnsiConsole.MarkupLine(title);
                    AnsiConsole.MarkupLine("[dim grey](Use Up/Down arrows + Enter to place, or press Q to quit)[/]");

                    for (int i = 0; i < allChoices.Count; i++)
                    {
                        if (i == selectedIndex)
                        {
                            AnsiConsole.MarkupLine($"  [bold cyan]> {allChoices[i]}[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"    {allChoices[i]}");
                        }
                    }

                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                    // Move cursor back up to redraw cleanly
                    int linesToClear = allChoices.Count + 2;
                    Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - linesToClear));

                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        selectedIndex = (selectedIndex - 1 + allChoices.Count) % allChoices.Count;
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        selectedIndex = (selectedIndex + 1) % allChoices.Count;
                    }
                    else if (keyInfo.Key == ConsoleKey.Q)
                    {
                        // Clear menu lines before showing confirmation
                        ClearMenuLines(linesToClear);
                        if (ConfirmAction("[bold red]Are you sure you want to quit this match?[/]"))
                        {
                            return null;
                        }

                        break; // Re-render menu if user chose No
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        ClearMenuLines(linesToClear);
                        string chosen = allChoices[selectedIndex];

                        if (chosen == quitOption)
                        {
                            if (ConfirmAction("[bold red]Are you sure you want to quit this match?[/]"))
                            {
                                return null;
                            }

                            break; // Re-render menu if user chose No
                        }

                        return chosen;
                    }
                }
            }
        }

        private static void ClearMenuLines(int lineCount)
        {
            int startTop = Console.CursorTop;
            string blankLine = new string(' ', Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 80);
            for (int i = 0; i < lineCount; i++)
            {
                Console.SetCursorPosition(0, startTop + i);
                Console.Write(blankLine);
            }

            Console.SetCursorPosition(0, startTop);
        }

    }
}
