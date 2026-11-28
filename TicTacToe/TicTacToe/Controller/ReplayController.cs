using Spectre.Console;
using TicTacToe.Core.Model;
using TicTacToe.Core.Service;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Handles match history browsing and replay playback in either Auto (1-second interval)
    /// or Manual (Next / Previous step-by-step) mode using <see cref="GameService"/>.
    /// </summary>
    public class ReplayController
    {
        private const string PlaybackAuto = "Auto play (1-second interval)";
        private const string PlaybackManual = "Manual Step-by-Step (Next / Previous)";
        private const string OptionReplayAgain = "Watch Replay Again";
        private const string OptionChooseAnotherMatch = "Choose Another Match";
        private const string OptionBackToMainMenu = "Back to Game Lobby";
        private readonly GameService _gameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplayController"/> class.
        /// </summary>
        /// <param name="gameService">The game service used to fetch match history and build replay frames.</param>
        public ReplayController(GameService gameService)
        {
            this._gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        /// <summary>
        /// Displays the logged-in user's saved matches using arrow-key navigation
        /// and opens the selected match in the chosen replay playback mode.
        /// </summary>
        /// <param name="currentUser">The currently logged-in user.</param>
        public void StartReplayMenu(User currentUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            while (true)
            {
                ConsoleActivity.ShowHeader("WATCH REPLAY - MATCH HISTORY");

                List<GameRecord> matches = this._gameService.GetUserMatchHistory(currentUser.UserId);
                if (matches == null || matches.Count == 0)
                {
                    ConsoleActivity.PrintInvalidMessage("No saved matches found yet. Play a game first!");
                    return;
                }

                var menuOptions = new List<string>();
                var recordLookup = new Dictionary<string, GameRecord>();

                for (int i = 0; i < matches.Count; i++)
                {
                    GameRecord match = matches[i];
                    string resultLabel = match.WinnerName.Equals("Draw", StringComparison.OrdinalIgnoreCase)
                        ? "Draw"
                        : $"Winner: {match.WinnerName}";

                    string label =
                        $"#{i + 1} | [[{match.Mode}]] {Markup.Escape(match.Player1Name)} (X) vs {Markup.Escape(match.Player2Name)} (O) | {Markup.Escape(resultLabel)} | {match.PlayedAt.ToLocalTime():yyyy-MM-dd HH:mm}";

                    menuOptions.Add(label);
                    recordLookup[label] = match;
                }

                menuOptions.Add(OptionBackToMainMenu);

                string selectedChoice = ConsoleActivity.ShowSelectionMenu(
                    "[bold cyan]Use Up/Down arrow keys to select a match (or go back to Game Lobby):[/]",
                    menuOptions.ToArray());

                if (selectedChoice == OptionBackToMainMenu)
                {
                    return;
                }

                GameRecord chosenMatch = recordLookup[selectedChoice];
                bool exitToLobby = this.RunReplayLoop(chosenMatch);
                if (exitToLobby)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Prompts the user to pick Auto (1s) or Manual (Next/Previous) playback mode, runs the replay,
        /// and handles post-replay navigation.
        /// </summary>
        /// <param name="match">The <see cref="GameRecord"/> selected for replay.</param>
        /// <returns><c>true</c> if the user chose to return directly to the Game Lobby; otherwise, <c>false</c>.</returns>
        private bool RunReplayLoop(GameRecord match)
        {
            while (true)
            {
                ConsoleActivity.ShowHeader("REPLAY SETTINGS");
                string playbackMode = ConsoleActivity.ShowSelectionMenu(
                    "[bold cyan]How would you like to watch this replay?[/]",
                    new[] { PlaybackAuto, PlaybackManual, OptionChooseAnotherMatch, OptionBackToMainMenu });

                if (playbackMode == OptionChooseAnotherMatch)
                {
                    return false;
                }

                if (playbackMode == OptionBackToMainMenu)
                {
                    return true;
                }

                if (playbackMode == PlaybackAuto)
                {
                    this.PlayAutoReplay(match);
                }
                else
                {
                    this.PlayManualStepReplay(match);
                }

                string postChoice = ConsoleActivity.ShowSelectionMenu(
                    "[bold cyan]Replay finished! What would you like to do next?[/]",
                    new[] { OptionReplayAgain, OptionChooseAnotherMatch, OptionBackToMainMenu });

                if (postChoice == OptionChooseAnotherMatch)
                {
                    return false;
                }

                if (postChoice == OptionBackToMainMenu)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Automatically plays all moves of the match with a 1-second interval between frames.
        /// </summary>
        /// <param name="match">The <see cref="GameRecord"/> to play back.</param>
        private void PlayAutoReplay(GameRecord match)
        {
            List<char[]> frames = this._gameService.BuildReplayFrames(match);
            char[] initialBoard = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            this.RenderReplayScreen(
                initialBoard,
                match,
                moveNumber: 0,
                totalMoves: frames.Count,
                statusText: "[grey]Starting auto replay (1s interval)...[/]");

            Thread.Sleep(1000);

            for (int i = 0; i < frames.Count; i++)
            {
                this.RenderReplayScreen(
                    frames[i],
                    match,
                    moveNumber: i + 1,
                    totalMoves: frames.Count,
                    statusText: BuildMoveStatusText(match, i, frames.Count));

                Thread.Sleep(1000);
            }

            this.ShowFinalOutcomeBanner(match);
        }

        /// <summary>
        /// Allows stepping forward and backward through the match moves using Next and Previous buttons/keys.
        /// </summary>
        /// <param name="match">The <see cref="GameRecord"/> to inspect step by step.</param>
        private void PlayManualStepReplay(GameRecord match)
        {
            List<char[]> moveFrames = this._gameService.BuildReplayFrames(match);

            // Timeline index 0 = initial empty board, indices 1..N = state after move 1..N
            var timeline = new List<char[]>
            {
                new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' }
            };
            timeline.AddRange(moveFrames);

            int currentStep = 0;
            int totalMoves = moveFrames.Count;

            while (true)
            {
                string statusText = currentStep == 0
                    ? "[grey]Initial Board (Move 0) - Press Next to start[/]"
                    : BuildMoveStatusText(match, currentStep - 1, totalMoves);

                this.RenderReplayScreen(
                    timeline[currentStep],
                    match,
                    moveNumber: currentStep,
                    totalMoves: totalMoves,
                    statusText: statusText);

                if (currentStep == totalMoves)
                {
                    this.ShowFinalOutcomeBanner(match);
                }

                // Build dynamic Next / Previous controls based on current step position
                var controls = new List<string>();
                if (currentStep < totalMoves)
                {
                    controls.Add("Next Move →");
                }

                if (currentStep > 0)
                {
                    controls.Add("← Previous Move");
                }

                controls.Add("Finish Replay");

                string action = ConsoleActivity.ShowSelectionMenu(
                    "[bold cyan]Step Controls:[/]",
                    controls.ToArray());

                if (action == "Next Move →" && currentStep < totalMoves)
                {
                    currentStep++;
                }
                else if (action == "← Previous Move" && currentStep > 0)
                {
                    currentStep--;
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Builds a formatted markup string describing the player, symbol, and cell position for a given move index.
        /// </summary>
        private static string BuildMoveStatusText(GameRecord match, int moveIndex, int totalMoves)
        {
            char symbol = (moveIndex % 2 == 0) ? 'X' : 'O';
            string moverName = symbol == 'X' ? match.Player1Name : match.Player2Name;
            int position = match.Moves[moveIndex];
            string color = symbol == 'X' ? "cyan" : "yellow";

            return $"[bold {color}]Move {moveIndex + 1}/{totalMoves}: {Markup.Escape(moverName)} ({symbol}) placed at Position {position}[/]";
        }

        /// <summary>
        /// Displays the final match winner or draw announcement banner.
        /// 
        private void ShowFinalOutcomeBanner(GameRecord match)
        {
            if (match.WinnerName.Equals("Draw", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleActivity.PrintSuccessMessage("Replay Complete: The match ended in a Draw!");
            }
            else
            {
                ConsoleActivity.PrintSuccessMessage($"Replay Complete: {match.WinnerName} won the match!");
            }
        }

        private void RenderReplayScreen(
        char[] board,
        GameRecord match,
        int moveNumber,
        int totalMoves,
        string statusText)
        {
            ConsoleActivity.ShowHeader($"REPLAY - {match.Mode.ToString().ToUpper()} MODE");
            var boardGrid = new Table()
            .Border(TableBorder.Heavy)
            .BorderColor(Color.Cyan1)
            .ShowRowSeparators()
            .HideHeaders();
            for (int i = 0; i < 3; i++)
            {
                boardGrid.AddColumn(new TableColumn(string.Empty).Centered().Width(7));
            }
            for (int row = 0; row < 3; row++)
            {
                boardGrid.AddRow(
                FormatCell(board[row * 3]),
                FormatCell(board[(row * 3) + 1]),
                FormatCell(board[(row * 3) + 2]));
            }
            var boardPanel = new Panel(Align.Center(boardGrid))
            {
                Header = new PanelHeader("[bold cyan] REPLAY BOARD [/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1),
                Padding = new Padding(2, 1),
            };
            var infoTable = new Table()
            .Border(TableBorder.Simple)
            .BorderColor(Color.Grey);
            infoTable.AddColumn(new TableColumn("[bold]Detail[/]").LeftAligned());
            infoTable.AddColumn(new TableColumn("[bold]Value[/]").LeftAligned());
            infoTable.AddRow("[grey]Mode[/]", $"[bold white]{match.Mode}[/]");
            infoTable.AddRow("[grey]Player 1 (X)[/]", $"[bold cyan]{Markup.Escape(match.Player1Name)}[/]");
            infoTable.AddRow("[grey]Player 2 (O)[/]", $"[bold yellow]{Markup.Escape(match.Player2Name)}[/]");
            infoTable.AddRow("[grey]Progress[/]", $"[bold green]{moveNumber} / {totalMoves} moves[/]");
            infoTable.AddRow("[grey]Played On[/]", $"[white]{match.PlayedAt.ToLocalTime():yyyy-MM-dd HH:mm}[/]");
            var infoPanel = new Panel(new Rows(infoTable, new Text(string.Empty), new Markup(statusText)))
            {
                Header = new PanelHeader("[bold yellow] MATCH INFO [/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow),
                Padding = new Padding(2, 1),
            };
            AnsiConsole.Write(Align.Center(new Columns(boardPanel, infoPanel) { Padding = new Padding(2, 0) }));
            ConsoleActivity.PrintEmptyLine();
        }
        private static string FormatCell(char cell) => cell switch
        {
            'X' => "\n[bold cyan]X[/]\n",
            'O' => "\n[bold yellow]O[/]\n",
            _ => $"\n[dim grey]{cell}[/]\n",
        };
    }
}