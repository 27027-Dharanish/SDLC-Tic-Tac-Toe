using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Model;
using TicTacToe.Core.Service;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Handles local 2-player Manual mode gameplay between Player 1 (logged-in user) and Player 2.
    /// </summary>
    public class ManualModeController
    {
        private const string OptionPlayAgain = "Play Again";
        private const string OptionExitMode = "Exit Mode";
        private const string TimerOptionLimited = "Limited Time (10 Seconds per Move)";
        private const string TimerOptionUnlimited = "Unlimited Time";
        private readonly ManualGameService _manualGameService;
        private readonly GameService _gameService;
        private readonly Random _random = new Random();

        private int _player1Wins;
        private int _player2Wins;
        private int _draws;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualModeController"/> class.
        /// </summary>
        /// <param name="manualGameService">The board rule service for 2-player matches.</param>
        /// <param name="gameService">The game service used to persist completed matches.</param>
        public ManualModeController(ManualGameService manualGameService, GameService gameService)
        {
            this._manualGameService = manualGameService ?? throw new ArgumentNullException(nameof(manualGameService));
            this._gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        /// <summary>
        /// Starts the Manual mode 2-player match loop and resets the scoreboard upon entry and exit.
        /// </summary>
        /// <param name="currentUser">The logged-in user acting as Player 1.</param>
        public void StartMatch(User currentUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            string player1Name = currentUser.UserName;
            string player2Name = this.PromptPlayer2Name(player1Name);
            int? turnSeconds = this.PromptTurnTimeLimit();

            this.ResetScoreboard();

            try
            {
                string choice;
                do
                {
                    this.PlaySingleRound(currentUser, player1Name, player2Name, turnSeconds);
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
        /// Asks the players whether they want a 10-second turn timer or unlimited thinking time.
        /// </summary>
        /// <returns><c>10</c> for limited time, or <see langword="null"/> for unlimited time.</returns>
        private int? PromptTurnTimeLimit()
        {
            ConsoleActivity.ShowHeader("MANUAL MODE - TIMER SETTINGS");
            string choice = ConsoleActivity.ShowSelectionMenu(
                "[bold cyan]Select turn timer mode:[/]",
                new[] { TimerOptionLimited, TimerOptionUnlimited });

            return choice == TimerOptionLimited ? 10 : null;
        }

        /// <summary>
        /// Executes a single round of 2-player Tic-Tac-Toe and saves the match record.
        /// </summary>
        private void PlaySingleRound(User currentUser, string player1Name, string player2Name, int? turnSeconds)
        {
            char player1Symbol = this.AssignRandomSymbols(player1Name, player2Name);
            char player2Symbol = player1Symbol == 'X' ? 'O' : 'X';

            // 'X' is always saved as player1Name in match history so replay order matches turn order
            string xPlayerName = player1Symbol == 'X' ? player1Name : player2Name;
            string oPlayerName = player1Symbol == 'O' ? player1Name : player2Name;

            char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var moveHistory = new List<int>();
            char currentTurn = 'X';
            string winnerName;

            while (true)
            {
                string activePlayerName = currentTurn == player1Symbol ? player1Name : player2Name;

                this.RenderBoardWithScoreboard(
                    board,
                    player1Name,
                    player2Name,
                    player1Symbol,
                    player2Symbol,
                    activePlayerName,
                    currentTurn);

                int? playerMove = this.GetPlayerMove(board, activePlayerName, currentTurn, turnSeconds);
                if (playerMove == null)
                {
                    ConsoleActivity.PrintInvalidMessage($"{activePlayerName} quit the match.");
                    return;
                }

                if (playerMove == -1)
                {
                    bool isPlayer1TimedOut = currentTurn == player1Symbol;
                    winnerName = isPlayer1TimedOut ? player2Name : player1Name;

                    if (isPlayer1TimedOut)
                    {
                        this._player2Wins++;
                    }
                    else
                    {
                        this._player1Wins++;
                    }

                    this.RenderBoardWithScoreboard(
                        board,
                        player1Name,
                        player2Name,
                        player1Symbol,
                        player2Symbol,
                        activePlayerName,
                        currentTurn);

                    ConsoleActivity.PrintInvalidMessage($"Time's up for {activePlayerName}! {winnerName} wins the match by timeout!");

                    if (moveHistory.Count > 0)
                    {
                        this._gameService.SaveMatch(
                            currentUser.UserId,
                            xPlayerName,
                            oPlayerName,
                            GameMode.Manual,
                            winnerName,
                            moveHistory.ToArray());
                    }

                    return;
                }

                int chosenPosition = playerMove.Value;
                board[chosenPosition - 1] = currentTurn;
                moveHistory.Add(chosenPosition);

                if (this._manualGameService.CheckWin(board, currentTurn))
                {
                    bool isPlayer1Winner = currentTurn == player1Symbol;
                    winnerName = isPlayer1Winner ? player1Name : player2Name;

                    if (isPlayer1Winner)
                    {
                        this._player1Wins++;
                    }
                    else
                    {
                        this._player2Wins++;
                    }

                    this.RenderBoardWithScoreboard(
                        board,
                        player1Name,
                        player2Name,
                        player1Symbol,
                        player2Symbol,
                        activePlayerName,
                        currentTurn);

                    ConsoleActivity.PrintSuccessMessage($"Congratulations {winnerName} ({currentTurn})! You won the match!");
                    break;
                }

                if (this._manualGameService.IsBoardFull(board))
                {
                    winnerName = "Draw";
                    this._draws++;

                    this.RenderBoardWithScoreboard(
                        board,
                        player1Name,
                        player2Name,
                        player1Symbol,
                        player2Symbol,
                        activePlayerName,
                        currentTurn);

                    ConsoleActivity.PrintSuccessMessage("The match ended in a Draw!");
                    break;
                }

                currentTurn = currentTurn == 'X' ? 'O' : 'X';
            }

            this._gameService.SaveMatch(
                currentUser.UserId,
                xPlayerName,
                oPlayerName,
                GameMode.Manual,
                winnerName,
                moveHistory.ToArray());
        }

        /// <summary>
        /// Resets all session scoreboard counters to zero.
        /// </summary>
        private void ResetScoreboard() => this._player1Wins = this._player2Wins = this._draws = 0;

        /// <summary>
        /// Prompts for Player 2's display name and ensures it is valid and distinct from Player 1.
        /// </summary>
        private string PromptPlayer2Name(string player1Name)
        {
            ConsoleActivity.ShowHeader("MANUAL MODE - PLAYER 2 SETUP");

            while (true)
            {
                string name = ConsoleActivity.GetStringInput("[cyan]Enter Player 2 name:[/]");

                if (name.Length < 2 || name.Length > 20)
                {
                    ConsoleActivity.PrintInvalidMessage("Player 2 name must be between 2 and 20 characters.");
                    continue;
                }

                if (name.Equals(player1Name, StringComparison.OrdinalIgnoreCase))
                {
                    ConsoleActivity.PrintInvalidMessage("Player 2 name cannot be the same as Player 1.");
                    continue;
                }

                return name;
            }
        }

        /// <summary>
        /// Randomly assigns <c>'X'</c> and <c>'O'</c> between Player 1 and Player 2 for the round.
        /// </summary>
        private char AssignRandomSymbols(string player1Name, string player2Name)
        {
            ConsoleActivity.ShowHeader("MANUAL MODE - SYMBOL DRAW");
            char p1Symbol = this._random.Next(2) == 0 ? 'X' : 'O';
            char p2Symbol = p1Symbol == 'X' ? 'O' : 'X';
            string firstMover = p1Symbol == 'X' ? player1Name : player2Name;
            ConsoleActivity.PrintSuccessMessage(
            $"{player1Name} is '{p1Symbol}' | {player2Name} is '{p2Symbol}'. {firstMover} (X) moves first!");
            return p1Symbol;
        }

        private int? GetPlayerMove(char[] board, string activePlayerName, char activeSymbol, int? turnSeconds)
        {
            string[] choices = this._manualGameService
            .GetAvailablePositions(board)
            .Select(pos => $"Position {pos}")
            .ToArray();
            string color = activeSymbol == 'X' ? "cyan" : "yellow";
            string prompt = $"[bold {color}]{Markup.Escape(activePlayerName)} ({activeSymbol})[/] - Select an available cell:";
            string? selected = turnSeconds.HasValue
            ? ConsoleActivity.ShowTimedMoveSelectionMenu(prompt, choices, turnSeconds.Value)
            : ConsoleActivity.ShowMoveSelectionMenu(prompt, choices);
            if (selected == null)
            {
                return null;
            }
            if (selected == "TIMEOUT")
            {
                return -1;
            }
            return int.Parse(selected.Replace("Position ", string.Empty));
        }

        private void RenderBoardWithScoreboard(
        char[] board,
        string player1Name,
        string player2Name,
        char player1Symbol,
        char player2Symbol,
        string activePlayerName,
        char currentTurn)
        {
            ConsoleActivity.ShowHeader("MATCH - MANUAL MODE (2 PLAYERS)");
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
            scoreTable.AddRow($"[bold cyan]{Markup.Escape(player1Name)}[/]", $"[bold cyan]{player1Symbol}[/]", $"[bold green]{this._player1Wins}[/]");
            scoreTable.AddRow($"[bold yellow]{Markup.Escape(player2Name)}[/]", $"[bold yellow]{player2Symbol}[/]", $"[bold red]{this._player2Wins}[/]");
            scoreTable.AddRow("[grey]Draws[/]", "[grey]-[/]", $"[bold white]{this._draws}[/]");
            string turnColor = currentTurn == 'X' ? "cyan" : "yellow";
            string turnIndicator = $"[bold {turnColor}]▶ {Markup.Escape(activePlayerName)}'s Turn ({currentTurn})[/]";
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