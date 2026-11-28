using Spectre.Console;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Model;
using TicTacToe.Core.Service;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Handles Hard (Impossible) mode gameplay with 2D board cursor navigation,
    /// turn timer, random symbol assignment, and match persistence.
    /// </summary>
    public class HardModeController
    {
        private const string ComputerName = "Computer";
        private const string OptionPlayAgain = "Play again";
        private const string OptionExitMode = "Exit mode";
        private readonly HardAiService _hardAiService;
        private readonly GameService _gameService;
        private readonly Random _random = new Random();
        private int _userWins;
        private int _computerWins;
        private int _draws;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardModeController"/> class.
        /// </summary>
        /// <param name="hardAiService">The unbeatable Minimax AI service for Hard mode.</param>
        /// <param name="gameService">The game service used to save completed matches.</param>
        public HardModeController(HardAiService hardAiService, GameService gameService)
        {
            this._hardAiService = hardAiService ?? throw new ArgumentNullException(nameof(hardAiService));
            this._gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        /// <summary>
        /// Starts the Hard mode match loop and resets the session scoreboard upon entry and exit.
        /// </summary>
        /// <param name="currentUser">The currently logged-in user.</param>
        public void StartMatch(User currentUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            this.ResetScoreboard();

            try
            {
                string choice;
                do
                {
                    this.PlaySingleRound(currentUser);
                    choice = ConsoleActivity.ShowSelectionMenu(
                        "[bold cyan]Match finished! What would you like to do next?[/]",
                        new[] { OptionPlayAgain, OptionExitMode });
                }
                while (choice != OptionExitMode);
            }
            finally
            {
                this.ResetScoreboard();
            }
        }

        /// <summary>
        /// Executes a single round of Hard mode Tic-Tac-Toe with randomly assigned symbols and saves the match.
        /// </summary>
        /// <param name="currentUser">The currently logged-in user.</param>
        private void PlaySingleRound(User currentUser)
        {
            char userSymbol = this.AssignRandomUserSymbol(currentUser.UserName);
            char aiSymbol = userSymbol == 'X' ? 'O' : 'X';
            string player1Name = userSymbol == 'X' ? currentUser.UserName : ComputerName;
            string player2Name = userSymbol == 'O' ? currentUser.UserName : ComputerName;
            char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var moveHistory = new List<int>();
            char currentTurn = 'X';
            string winnerName;
            while (true)
            {
                int chosenPosition;
                if (currentTurn == userSymbol)
                {
                    int? userMove = this.GetUserMove(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                    if (userMove == null)
                    {
                        ConsoleActivity.PrintInvalidMessage("Match quit.");
                        return;
                    }

                    if (userMove == -1)
                    {
                        winnerName = ComputerName;
                        this._computerWins++;

                        this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                        ConsoleActivity.PrintInvalidMessage($"Time's up! {ComputerName} wins the match by timeout!");
                        if (moveHistory.Count > 0)
                        {
                            this._gameService.SaveMatch(
                                currentUser.UserId,
                                player1Name,
                                player2Name,
                                GameMode.Impossible,
                                winnerName,
                                moveHistory.ToArray());
                        }

                        return;
                    }

                    chosenPosition = userMove.Value;
                }
                else
                {
                    this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                    ConsoleActivity.PrintInConsole($"[yellow]{ComputerName} ({aiSymbol}) is thinking...[/]");
                    Thread.Sleep(500);
                    chosenPosition = this._hardAiService.GetNextMove(board, userSymbol);
                }

                board[chosenPosition - 1] = currentTurn;
                moveHistory.Add(chosenPosition);

                if (this._hardAiService.CheckWin(board, currentTurn))
                {
                    bool userWon = currentTurn == userSymbol;
                    winnerName = userWon ? currentUser.UserName : ComputerName;

                    if (userWon)
                    {
                        this._userWins++;
                    }
                    else
                    {
                        this._computerWins++;
                    }

                    this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);

                    if (userWon)
                    {
                        ConsoleActivity.PrintSuccessMessage($"Incredible {currentUser.UserName}! You defeated hard AI!");
                    }
                    else
                    {
                        ConsoleActivity.PrintInvalidMessage($"{ComputerName} ({aiSymbol}) won the match!");
                    }

                    break;
                }

                if (this._hardAiService.IsBoardFull(board))
                {
                    winnerName = "Draw";
                    this._draws++;
                    this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                    ConsoleActivity.PrintSuccessMessage("The match ended in a Draw!");
                    break;
                }

                currentTurn = currentTurn == 'X' ? 'O' : 'X';
            }

            this._gameService.SaveMatch(
                currentUser.UserId,
                player1Name,
                player2Name,
                GameMode.Impossible,
                winnerName,
                moveHistory.ToArray());
        }

        /// <summary>
        /// Resets all session scoreboard counters to zero.
        /// </summary>
        private void ResetScoreboard() => this._userWins = this._computerWins = this._draws = 0;

        /// <summary>
        /// Randomly assigns <c>'X'</c> or <c>'O'</c> to the human player and displays the coin-toss assignment.
        /// </summary>
        /// <param name="userName">The human player's username.</param>
        /// <returns>The randomly assigned symbol (<c>'X'</c> or <c>'O'</c>).</returns>
        private char AssignRandomUserSymbol(string userName)
        {
            ConsoleActivity.ShowHeader("HARD MODE - RANDOM SYMBOL DRAW");
            char assignedSymbol = this._random.Next(2) == 0 ? 'X' : 'O';
            string turnOrderNote = assignedSymbol == 'X'
                ? "You play first!"
                : $"{ComputerName} (X) plays first!";

            ConsoleActivity.PrintSuccessMessage(
                $"{userName}, you have been randomly assigned '{assignedSymbol}'. {turnOrderNote}");

            return assignedSymbol;
        }

        /// <summary>
        /// Lets the user navigate directly inside the 3x3 board using arrow keys and press Enter to place their symbol.
        /// </summary>
        private int? GetUserMove(char[] board, string userName, char userSymbol, char aiSymbol, char currentTurn)
        {
            return ConsoleActivity.SelectCellOnBoard(
                board,
                cursorIndex => this.RenderBoardWithScoreboard(board, userName, userSymbol, aiSymbol, currentTurn, cursorIndex),
                "[bold green]Your turn! Move on the board and press Enter:[/]",
                turnSeconds: 10);
        }

        /// <summary>
        /// Renders the 3x3 game board panel (with optional cursor highlight) alongside the live session scoreboard panel.
        /// </summary>
        private void RenderBoardWithScoreboard(
            char[] board,
            string userName,
            char userSymbol,
            char aiSymbol,
            char currentTurn,
            int highlightedIndex = -1)
        {
            ConsoleActivity.ShowHeader("MATCH - HARD MODE");

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
                int i0 = row * 3;
                int i1 = (row * 3) + 1;
                int i2 = (row * 3) + 2;

                boardGrid.AddRow(
                    FormatCell(board[i0], i0 == highlightedIndex),
                    FormatCell(board[i1], i1 == highlightedIndex),
                    FormatCell(board[i2], i2 == highlightedIndex));
            }
            var boardPanel = new Panel(Align.Center(boardGrid))
            {
                Header = new PanelHeader("[bold cyan] GAME BOARD [/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1),
                Padding = new Padding(2, 1),
            };
            var scoreTable = new Table()
            .Border(TableBorder.Simple)
            .BorderColor(Color.Grey);
            scoreTable.AddColumn(new TableColumn("[bold]Player[/]").LeftAligned());
            scoreTable.AddColumn(new TableColumn("[bold]Symbol[/]").Centered());
            scoreTable.AddColumn(new TableColumn("[bold]Score[/]").RightAligned());
            scoreTable.AddRow($"[bold cyan]{Markup.Escape(userName)}[/]", $"[bold cyan]{userSymbol}[/]", $"[bold green]{this._userWins}[/]");
            scoreTable.AddRow($"[bold yellow]{ComputerName}[/]", $"[bold yellow]{aiSymbol}[/]", $"[bold red]{this._computerWins}[/]");
            scoreTable.AddRow("[grey]Draws[/]", "[grey]-[/]", $"[bold white]{this._draws}[/]");
            string turnIndicator = currentTurn == userSymbol
            ? $"[bold green]▶ Your Turn ({userSymbol})[/]"
            : $"[bold yellow]▶ {ComputerName}'s Turn ({aiSymbol})[/]";
            var scoreboardPanel = new Panel(new Rows(scoreTable, new Text(string.Empty), new Markup(turnIndicator)))
            {
                Header = new PanelHeader("[bold yellow] SESSION SCOREBOARD [/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow),
                Padding = new Padding(2, 1),
            };
            AnsiConsole.Write(Align.Center(new Columns(boardPanel, scoreboardPanel) { Padding = new Padding(2, 0) }));
            ConsoleActivity.PrintEmptyLine();
        }
        private static string FormatCell(char cell, bool isHighlighted = false)
        {
            if (isHighlighted)
            {
                return $"\n[black on green] {cell} [/]\n";
            }
            return cell switch
            {
                'X' => "\n[bold cyan]X[/]\n",
                'O' => "\n[bold yellow]O[/]\n",
                _ => $"\n[dim grey]{cell}[/]\n",
            };
        }
    }
}
