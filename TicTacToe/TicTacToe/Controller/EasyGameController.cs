using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Spectre.Console;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Model;
using TicTacToe.Core.Service;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Controls the Easy difficulty gameplay flow against the AI, featuring a side-by-side scoreboard,
    /// a play-again loop, and automatic score reset upon exiting the mode.
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
        /// Initializes a new instance of the <see cref="EasyGameController"/> class with default services.
        /// </summary>
        public EasyGameController()
            : this(new EasyAiService(), new GameService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyGameController"/> class with injected services.
        /// </summary>
        /// <param name="easyAiService">The service providing the Easy AI algorithm.</param>
        /// <param name="gameService">The service handling match persistence.</param>
        public EasyGameController(EasyAiService easyAiService, GameService gameService)
        {
            if (easyAiService == null)
            {
                throw new ArgumentNullException(nameof(easyAiService));
            }

            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }

            this._easyAiService = easyAiService;
            this._gameService = gameService;
        }

        /// <summary>
        /// Starts the Easy mode session, retaining scores across "Play Again" rounds and resetting
        /// the scoreboard when the user exits back to the menu.
        /// </summary>
        /// <param name="currentUser">The authenticated <see cref="User"/> playing the match.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="currentUser"/> is <see langword="null"/>.</exception>
        public void StartMatch(User currentUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            this.ResetScoreboard();

            try
            {
                bool keepPlaying = true;
                while (keepPlaying)
                {
                    this.PlaySingleRound(currentUser);

                    string postMatchChoice = ConsoleActivity.ShowSelectionMenu(
                        "[bold cyan]Match finished! What would you like to do next?[/]",
                        new[] { OptionPlayAgain, OptionExitMode });

                    if (postMatchChoice == OptionExitMode)
                    {
                        keepPlaying = false;
                    }
                }
            }
            finally
            {
                this.ResetScoreboard();
            }
        }

        /// <summary>
        /// Executes a single round of Easy mode Tic-Tac-Toe, updates the session scoreboard, and saves the match.
        /// </summary>
        private void PlaySingleRound(User currentUser)
        {
            char userSymbol = this.PromptUserSymbolChoice(currentUser.UserName);
            char aiSymbol = (userSymbol == 'X') ? 'O' : 'X';

            // Player 1 is always 'X' and Player 2 is always 'O' so replay indices match (even = X, odd = O)
            string player1Name = (userSymbol == 'X') ? currentUser.UserName : ComputerName;
            string player2Name = (userSymbol == 'O') ? currentUser.UserName : ComputerName;

            char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var moveHistory = new List<int>();
            char currentTurn = 'X'; // 'X' always moves first
            string winnerName;

            while (true)
            {
                this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);

                int chosenPosition;

                if (currentTurn == userSymbol)
                {
                    chosenPosition = this.GetUserMove(board);
                }
                else
                {
                    ConsoleActivity.PrintInConsole($"[yellow]{ComputerName} ({aiSymbol}) is thinking...[/]");
                    Thread.Sleep(500);
                    chosenPosition = this._easyAiService.GetNextMove(board, userSymbol);
                }

                board[chosenPosition - 1] = currentTurn;
                moveHistory.Add(chosenPosition);

                if (this._easyAiService.CheckWin(board, currentTurn))
                {
                    if (currentTurn == userSymbol)
                    {
                        winnerName = currentUser.UserName;
                        this._userWins++;
                        this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                        ConsoleActivity.PrintSuccessMessage($"Congratulations {currentUser.UserName}! You won the match!");
                    }
                    else
                    {
                        winnerName = ComputerName;
                        this._computerWins++;
                        this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                        ConsoleActivity.PrintInvalidMessage($"{ComputerName} ({aiSymbol}) won the match!");
                    }

                    break;
                }

                if (this._easyAiService.IsBoardFull(board))
                {
                    winnerName = "Draw";
                    this._draws++;
                    this.RenderBoardWithScoreboard(board, currentUser.UserName, userSymbol, aiSymbol, currentTurn);
                    ConsoleActivity.PrintSuccessMessage("The match ended in a Draw!");
                    break;
                }

                currentTurn = (currentTurn == 'X') ? 'O' : 'X';
            }

            this._gameService.SaveMatch(
                currentUser.UserId,
                player1Name,
                player2Name,
                GameMode.Easy,
                winnerName,
                moveHistory.ToArray());
        }

        /// <summary>
        /// Resets all session scoreboard counters back to zero.
        /// </summary>
        private void ResetScoreboard()
        {
            this._userWins = 0;
            this._computerWins = 0;
            this._draws = 0;
        }

        /// <summary>
        /// Prompts the user to select whether they want to play as 'X' or 'O'.
        /// </summary>
        private char PromptUserSymbolChoice(string userName)
        {
            ConsoleActivity.ShowHeader("EASY MODE - CHOOSE SYMBOL");
            string choice = ConsoleActivity.ShowSelectionMenu(
                $"[bold cyan]{Markup.Escape(userName)}, choose your symbol (X always moves first):[/]",
                new[] { SymbolOptionX, SymbolOptionO });

            if (choice == SymbolOptionX)
            {
                return 'X';
            }

            return 'O';
        }

        /// <summary>
        /// Prompts the user to select an available board slot (1 to 9) using arrow-key navigation.
        /// </summary>
        private int GetUserMove(char[] board)
        {
            string[] availableChoices = this._easyAiService
                .GetAvailablePositions(board)
                .Select(pos => $"Position {pos}")
                .ToArray();

            string selected = ConsoleActivity.ShowSelectionMenu(
                "[bold green]Your turn! Select an available cell:[/]",
                availableChoices);

            return int.Parse(selected.Replace("Position ", string.Empty));
        }

        /// <summary>
        /// Renders the 3x3 Tic-Tac-Toe board on the left and the live session scoreboard on the right.
        /// </summary>
        private void RenderBoardWithScoreboard(char[] board, string userName, char userSymbol, char aiSymbol, char currentTurn)
        {
            ConsoleActivity.ShowHeader("MATCH - EASY MODE");

            var boardGrid = new Table()
                .Border(TableBorder.Heavy)
                .BorderColor(Color.Cyan1)
                .ShowRowSeparators()
                .HideHeaders();

            boardGrid.AddColumn(new TableColumn(string.Empty).Centered().Width(7));
            boardGrid.AddColumn(new TableColumn(string.Empty).Centered().Width(7));
            boardGrid.AddColumn(new TableColumn(string.Empty).Centered().Width(7));

            for (int row = 0; row < 3; row++)
            {
                boardGrid.AddRow(
                    FormatCell(board[row * 3]),
                    FormatCell(board[row * 3 + 1]),
                    FormatCell(board[row * 3 + 2]));
            }

            var boardPanel = new Panel(Align.Center(boardGrid))
            {
                Header = new PanelHeader("[bold cyan] GAME BOARD [/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1),
                Padding = new Padding(2, 1)
            };

            var scoreTable = new Table()
                .Border(TableBorder.Simple)
                .BorderColor(Color.Grey);

            scoreTable.AddColumn(new TableColumn("[bold]Player[/]").LeftAligned());
            scoreTable.AddColumn(new TableColumn("[bold]Symbol[/]").Centered());
            scoreTable.AddColumn(new TableColumn("[bold]Score[/]").RightAligned());
            scoreTable.AddRow(
            $"[bold cyan]{Markup.Escape(userName)}[/]",
            $"[bold cyan]{userSymbol}[/]",
            $"[bold green]{this._userWins}[/]");
            scoreTable.AddRow(
            $"[bold yellow]{ComputerName}[/]",
            $"[bold yellow]{aiSymbol}[/]",
            $"[bold red]{this._computerWins}[/]");
            scoreTable.AddRow(
            "[grey]Draws[/]",
            "[grey]-[/]",
            $"[bold white]{this._draws}[/]");
            string turnIndicator = currentTurn == userSymbol
            ? $"[bold green]▶ Your Turn ({userSymbol})[/]"
            : $"[bold yellow]▶ {ComputerName}'s Turn ({aiSymbol})[/]";
            var rightContent = new Rows(
            scoreTable,
            new Text(string.Empty),
            new Markup(turnIndicator));
            var scoreboardPanel = new Panel(rightContent)
            {
                Header = new PanelHeader("[bold yellow] SESSION SCOREBOARD [/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow),
                Padding = new Padding(2, 1)
            };
            var sideBySideLayout = new Columns(boardPanel, scoreboardPanel)
            {
                Padding = new Padding(2, 0)
            };
            AnsiConsole.Write(Align.Center(sideBySideLayout));
            ConsoleActivity.PrintEmptyLine();
        }
        /// 
        /// Formats an individual cell with padded spacing and color-coded markup for 'X', 'O', or available numbers.
        /// 
        private static string FormatCell(char cell)
        {
            return cell switch
            {
                'X' => "\n[bold cyan]X[/]\n",
                'O' => "\n[bold yellow]O[/]\n",
                _ => $"\n[dim grey]{cell}[/]\n"
            };
        }
    }
}