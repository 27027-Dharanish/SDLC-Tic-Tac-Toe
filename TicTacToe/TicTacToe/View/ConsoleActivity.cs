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

        /// <summary>
        /// Allows the player to navigate directly inside the 3x3 board using Arrow keys (<c>↑ ↓ ← →</c>)
        /// and press <c>Enter</c> to mark the highlighted cell (or <c>Q</c> to quit).
        /// Supports an optional turn countdown timer.
        /// </summary>
        /// <param name="board">The current 9-cell board state.</param>
        /// <param name="renderBoardCallback">Callback that renders the header, 3x3 board (highlighting the cursor index), and scoreboard.</param>
        /// <param name="prompt">The turn prompt shown below the board.</param>
        /// <param name="turnSeconds">Optional countdown timer in seconds (<see langword="null"/> for unlimited time).</param>
        /// <param name="allowUndo">Optional in seconds (<see langword="null"/> for unlimited time).</param>
        /// <returns>The chosen 1-based position (1 to 9), <c>-1</c> on timeout, or <see langword="null"/> if the player quit.</returns>
        public static int? SelectCellOnBoard(
            char[] board,
            Action<int> renderBoardCallback,
            string prompt,
            int? turnSeconds = null,
            bool allowUndo = false)
        {
            // Start cursor on the first available cell
            int cursorIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != 'X' && board[i] != 'O')
                {
                    cursorIndex = i;
                    break;
                }
            }

            string undoHint = allowUndo ? " | U to undo" : string.Empty;
            AnsiConsole.MarkupLine($"[dim grey](Use arrows to move on board | Enter to mark{undoHint} | Q to quit | u to undo)[/]");

            DateTime? deadline = turnSeconds.HasValue
                ? DateTime.UtcNow.AddSeconds(turnSeconds.Value)
                : null;

            bool needsRedraw = true;
            int lastShownSeconds = -1;

            while (true)
            {
                int remaining = deadline.HasValue
                    ? Math.Max(0, (int)Math.Ceiling((deadline.Value - DateTime.UtcNow).TotalSeconds))
                    : 0;

                if (needsRedraw || (deadline.HasValue && remaining != lastShownSeconds))
                {
                    renderBoardCallback(cursorIndex);

                    if (deadline.HasValue)
                    {
                        string timerColor = remaining <= 3 ? "red" : "yellow";
                        AnsiConsole.MarkupLine($"{prompt}  [bold {timerColor}][[Time Left: {remaining}s]][/]");
                        lastShownSeconds = remaining;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine(prompt);
                    }

                    AnsiConsole.MarkupLine("[dim grey](Use arrows to move on board | Enter to mark | Q to quit | u to undo)[/]");
                    needsRedraw = false;
                }

                if (deadline.HasValue && DateTime.UtcNow >= deadline.Value)
                {
                    return -1; // Timeout
                }

                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(30);
                    continue;
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                int row = cursorIndex / 3;
                int col = cursorIndex % 3;

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        row = (row + 2) % 3;
                        cursorIndex = (row * 3) + col;
                        needsRedraw = true;
                        break;

                    case ConsoleKey.DownArrow:
                        row = (row + 1) % 3;
                        cursorIndex = (row * 3) + col;
                        needsRedraw = true;
                        break;

                    case ConsoleKey.LeftArrow:
                        col = (col + 2) % 3;
                        cursorIndex = (row * 3) + col;
                        needsRedraw = true;
                        break;

                    case ConsoleKey.RightArrow:
                        col = (col + 1) % 3;
                        cursorIndex = (row * 3) + col;
                        needsRedraw = true;
                        break;
                    case ConsoleKey.U:
                        if (allowUndo)
                        {
                            return -2;
                        }

                        break;

                    case ConsoleKey.Q:
                        if (ConfirmAction("[bold red]Are you sure you want to quit this match?[/]"))
                        {
                            return null;
                        }

                        if (deadline.HasValue)
                        {
                            deadline = DateTime.UtcNow.AddSeconds(remaining);
                        }

                        needsRedraw = true;
                        break;

                    case ConsoleKey.Enter:
                        if (board[cursorIndex] != 'X' && board[cursorIndex] != 'O')
                        {
                            return cursorIndex + 1; // Valid empty cell chosen
                        }

                        AnsiConsole.MarkupLine("[red]That cell is already occupied! Move to an empty cell.[/]");
                        Thread.Sleep(500);
                        needsRedraw = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Displays an interactive arrow-key move menu with a live turn countdown timer.
        /// Returns <c>"TIMEOUT"</c> if time expires, or <see langword="null"/> if the player confirms quitting (<c>Q</c>).
        /// </summary>
        /// <param name="title">The prompt title displayed above the choices.</param>
        /// <param name="moveChoices">The available cell choices.</param>
        /// <param name="turnSeconds">The fixed countdown duration in seconds per turn.</param>
        /// <returns>The chosen move string, or <see langword="null"/> if the player confirmed quitting.</returns>
        public static string? ShowTimedMoveSelectionMenu(string title, string[] moveChoices, int turnSeconds = 10)
        {
            const string quitOption = "[red][[Q]] Quit Match[/]";
            var allChoices = moveChoices.Append(quitOption).ToList();
            DateTime deadline = DateTime.UtcNow.AddSeconds(turnSeconds);

            while (true)
            {
                int selectedIndex = 0;

                while (true)
                {
                    int remaining = Math.Max(0, (int)Math.Ceiling((deadline - DateTime.UtcNow).TotalSeconds));
                    string timerColor = remaining <= 3 ? "red" : "yellow";

                    AnsiConsole.MarkupLine($"{title}  [bold {timerColor}][[Time Left: {remaining}s]][/]");
                    AnsiConsole.MarkupLine("[dim grey](Up/Down + Enter to place | Press Q to quit)[/]");

                    for (int i = 0; i < allChoices.Count; i++)
                    {
                        AnsiConsole.MarkupLine(i == selectedIndex
                            ? $"  [bold cyan]> {allChoices[i]}[/]"
                            : $"    {allChoices[i]}");
                    }

                    int linesToClear = allChoices.Count + 2;

                    // Wait up to 200ms for a keypress so the timer updates live every second
                    DateTime pollUntil = DateTime.UtcNow.AddMilliseconds(200);
                    while (!Console.KeyAvailable && DateTime.UtcNow < pollUntil)
                    {
                        Thread.Sleep(20);
                    }

                    Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - linesToClear));

                    if (DateTime.UtcNow >= deadline)
                    {
                        ClearMenuLines(linesToClear);
                        return "TIMEOUT";
                    }

                    if (!Console.KeyAvailable)
                    {
                        continue; // Redraw updated countdown
                    }

                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

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
                        ClearMenuLines(linesToClear);
                        if (ConfirmAction("[bold red]Are you sure you want to quit this match?[/]"))
                        {
                            return null;
                        }

                        deadline = DateTime.UtcNow.AddSeconds(remaining); // Resume remaining time
                        break;
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

                            deadline = DateTime.UtcNow.AddSeconds(remaining);
                            break;
                        }

                        return chosen;
                    }
                }
            }
        }
        /// <summary>
        /// Prompts the player to press a number key (<c>1-9</c>) for an available cell, or <c>Q</c> to quit.
        /// </summary>
        /// <param name="prompt">The prompt message displayed below the board.</param>
        /// <param name="availablePositions">The list of unoccupied cell numbers (1 to 9).</param>
        /// <returns>The chosen cell number (1 to 9), or <see langword="null"/> if the player confirmed quitting.</returns>
        public static int? GetDirectCellInput(string prompt, IReadOnlyCollection<int> availablePositions)
        {
            while (true)
            {
                AnsiConsole.MarkupLine(prompt);
                AnsiConsole.MarkupLine("[dim grey](Press 1-9 to place mark | Press Q to quit)[/]");

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                ClearMenuLines(2);

                if (keyInfo.Key == ConsoleKey.Q)
                {
                    if (ConfirmAction("[bold red]Are you sure you want to quit this match?[/]"))
                    {
                        return null;
                    }

                    continue;
                }

                int? number = ParseNumberKey(keyInfo);
                if (number.HasValue && availablePositions.Contains(number.Value))
                {
                    return number.Value;
                }

                AnsiConsole.MarkupLine("[red]Invalid or occupied cell! Press an available number (1-9).[/]");
                Thread.Sleep(600);
                ClearMenuLines(1);
            }
        }

        /// <summary>
        /// Prompts the player to press a number key (<c>1-9</c>) within a fixed countdown timer, or <c>Q</c> to quit.
        /// Returns <c>-1</c> on timeout, or <see langword="null"/> if the player confirmed quitting.
        /// </summary>
        /// <param name="prompt">The prompt message displayed below the board.</param>
        /// <param name="availablePositions">The list of unoccupied cell numbers (1 to 9).</param>
        /// <param name="turnSeconds">The fixed countdown duration in seconds.</param>
        /// <returns>The chosen cell number (1 to 9), or <see langword="null"/> if the player confirmed quitting.</returns>
        public static int? GetTimedDirectCellInput(string prompt, IReadOnlyCollection<int> availablePositions, int turnSeconds = 10)
        {
            DateTime deadline = DateTime.UtcNow.AddSeconds(turnSeconds);

            while (true)
            {
                int remaining = Math.Max(0, (int)Math.Ceiling((deadline - DateTime.UtcNow).TotalSeconds));
                string timerColor = remaining <= 3 ? "red" : "yellow";

                AnsiConsole.MarkupLine($"{prompt}  [bold {timerColor}][[Time Left: {remaining}s]][/]");
                AnsiConsole.MarkupLine("[dim grey](Press 1-9 to place mark | Press Q to quit)[/]");

                DateTime pollUntil = DateTime.UtcNow.AddMilliseconds(200);
                while (!Console.KeyAvailable && DateTime.UtcNow < pollUntil)
                {
                    Thread.Sleep(20);
                }

                ClearMenuLines(2);

                if (DateTime.UtcNow >= deadline)
                {
                    return -1; // Timeout
                }

                if (!Console.KeyAvailable)
                {
                    continue; // Update countdown
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Q)
                {
                    if (ConfirmAction("[bold red]Are you sure you want to quit this match?[/]"))
                    {
                        return null;
                    }

                    deadline = DateTime.UtcNow.AddSeconds(remaining);
                    continue;
                }

                int? number = ParseNumberKey(keyInfo);
                if (number.HasValue && availablePositions.Contains(number.Value))
                {
                    return number.Value;
                }

                AnsiConsole.MarkupLine("[red]Invalid or occupied cell! Press an available number (1-9).[/]");
                Thread.Sleep(500);
                ClearMenuLines(1);
            }
        }

        private static int? ParseNumberKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key >= ConsoleKey.D1 && keyInfo.Key <= ConsoleKey.D9)
            {
                return (keyInfo.Key - ConsoleKey.D1) + 1;
            }

            if (keyInfo.Key >= ConsoleKey.NumPad1 && keyInfo.Key <= ConsoleKey.NumPad9)
            {
                return (keyInfo.Key - ConsoleKey.NumPad1) + 1;
            }

            return null;
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
