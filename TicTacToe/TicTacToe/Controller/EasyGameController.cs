using Spectre.Console;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Model;
using TicTacToe.Core.Service;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Controls the Easy difficulty gameplay flow against the AI.
    /// </summary>
    public class EasyGameController
    {
        private const string SymbolOptionX = "Play as X (Moves First)";
        private const string SymbolOptionO = "Play as O (Computer Moves First)";
        private const string OptionPlayAgain = "Play Again";
        private const string OptionExitMode = "Back to Mode Menu";
        private const string ComputerName = "Computer";

        private readonly EasyAiService _easyAiService;
        private readonly GameService _gameService;

        private int _userWins;
        private int _computerWins;
        private int _draws;

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyGameController"/> class.
        /// </summary>
        /// <param name="easyAiService">The service providing the Easy computer move.</param>
        /// <param name="gameService">The service responsible for saving and managing match records.</param>
        public EasyGameController(EasyAiService easyAiService, GameService gameService)
        {
            this._easyAiService = easyAiService;
            this._gameService = gameService;
        }

        /// <summary>
        /// Starts the Easy mode session..
        /// </summary>
        /// <param name="currentUser">The current user information.</param>
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
        /// Executes a single round of Easy mode Tic-Tac-Toe and saves the match.
        /// </summary>
        private void PlaySingleRound(User currentUser)
        {
            char userSymbol = this.PromptUserSymbolChoice(currentUser.UserName);
            char computerSymbol = userSymbol == 'X' ? 'O' : 'X';
            string player1Name = userSymbol == 'X' ? currentUser.UserName : ComputerName;
            string player2Name = userSymbol == 'O' ? currentUser.UserName : ComputerName;
            char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var moveHistory = new List<int>();
            char currentTurn = 'X';
            string winnerName;

            while (true)
            {
                this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, computerSymbol, currentTurn);
                int chosenPosition;
                if (currentTurn == userSymbol)
                {
                    int? userMove = this.GetUserMove(board);
                    if (userMove == null)
                    {
                        ConsoleActivity.PrintInvalidMessage("Match quit. Returning to mode menu...");
                        return;
                    }

                    chosenPosition = userMove.Value;
                }
                else
                {
                    ConsoleActivity.PrintInConsole($"[yellow]{ComputerName} ({computerSymbol}) is thinking...[/]");
                    Thread.Sleep(500);
                    chosenPosition = this._easyAiService.GetNextMove(board, userSymbol);
                }

                board[chosenPosition - 1] = currentTurn;
                moveHistory.Add(chosenPosition);
                if (this._easyAiService.CheckWin(board, currentTurn))
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

                    this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, computerSymbol, currentTurn);
                    if (userWon)
                    {
                        ConsoleActivity.PrintSuccessMessage($"Congratulations {currentUser.UserName}! You won the match!");
                    }
                    else
                    {
                        ConsoleActivity.PrintInvalidMessage($"{ComputerName} ({computerSymbol}) won the match!");
                    }

                    break;
                }

                if (this._easyAiService.IsBoardFull(board))
                {
                    winnerName = "Draw";
                    this._draws++;
                    this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, computerSymbol, currentTurn);
                    ConsoleActivity.PrintSuccessMessage("The match ended in a Draw!");
                    break;
                }

                currentTurn = currentTurn == 'X' ? 'O' : 'X';
            }

            this._gameService.SaveMatch(
                currentUser.UserId,
                player1Name,
                player2Name,
                GameMode.Easy,
                winnerName,
                moveHistory.ToArray());
        }

        private void ResetScoreboard()
        {
            this._userWins = this._computerWins = this._draws = 0;
        }

        private char PromptUserSymbolChoice(string userName)
        {
            ConsoleActivity.ShowHeader("EASY MODE - CHOOSE SYMBOL");
            string choice = ConsoleActivity.ShowSelectionMenu(
                $"[bold cyan]{Markup.Escape(userName)}, choose your symbol (X always moves first):[/]",
                new[] { SymbolOptionX, SymbolOptionO });

            return choice == SymbolOptionX ? 'X' : 'O';
        }

        private int? GetUserMove(char[] board)
        {
            string[] choices = this._easyAiService
                .GetAvailablePositions(board)
                .Select(pos => $"Position {pos}")
                .ToArray();

            string? selected = ConsoleActivity.ShowMoveSelectionMenu(
                "[bold green]Your turn! Select an available cell:[/]",
                choices);

            if (selected == null)
            {
                return null;
            }

            return int.Parse(selected.Replace("Position ", string.Empty));
        }

        private void RenderBoardWithScoreboard(char[] board, string userName, char userSymbol, char aiSymbol, char currentTurn)
        {
            ConsoleActivity.ShowHeader("MATCH - EASY MODE");

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

        private static string FormatCell(char cell) => cell switch
        {
            'X' => "\n[bold cyan]X[/]\n",
            'O' => "\n[bold yellow]O[/]\n",
            _ => $"\n[dim grey]{cell}[/]\n",
        };
    }
}
