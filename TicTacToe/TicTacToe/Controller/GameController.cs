using Spectre.Console;
using TicTacToe.Core.Constants;
using TicTacToe.Core.Model;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Handles the main game lobby, game mode selection, and gameplay interactions for an authenticated user.
    /// Acts as the intermediary between <see cref="ConsoleActivity"/> and the game services.
    /// </summary>
    public class GameController
    {
        private const string OptionPlayNewGame = "Play New Game";
        private const string OptionWatchReplay = "Watch Replay";
        private const string OptionLogOut = "Log Out";
        private const string ModeEasy = "Easy (AI - Casual)";
        private const string ModeMedium = "Medium (AI - Balanced)";
        private const string ModeImpossible = "Impossible (AI - Unbeatable)";
        private const string ModeManual = "Manual (2 Players)";
        private const string ModeBack = "Back to Lobby";
        private User? _currentUser;

        /// <summary>
        /// Sets the currently authenticated user for the active game session.
        /// </summary>
        /// <param name="user">The logged-in <see cref="User"/> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> is <see langword="null"/>.</exception>
        public void SetCurrentUser(User user)
        {
            this._currentUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        /// <summary>
        /// Starts the main game menu loop with a personalized greeting and arrow-key navigation.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if started before setting a current user.</exception>
        public void Start()
        {
            if (this._currentUser == null)
            {
                throw new InvalidOperationException("Cannot start GameController without an authenticated user.");
            }

            while (true)
            {
                ConsoleActivity.ShowHeader("GAME LOBBY", "TIC TAC TOE");
                this.ShowUserWelcomeBanner();

                string choice = ConsoleActivity.ShowSelectionMenu(
                    "[bold cyan]Select an option using Up/Down arrow keys:[/]",
                    new[] { OptionPlayNewGame, OptionWatchReplay, OptionLogOut });

                switch (choice)
                {
                    case OptionPlayNewGame:
                        this.HandlePlayNewGame();
                        break;
                    case OptionWatchReplay:
                        this.HandleWatchReplay();
                        break;
                    case OptionLogOut:
                        this.HandleLogOut();
                        return;
                }
            }
        }

        /// <summary>
        /// Displays a styled Spectre.Console welcome message with the logged-in user's name.
        /// </summary>
        private void ShowUserWelcomeBanner()
        {
            string safeName = Markup.Escape(this._currentUser!.UserName);
            var panel = new Panel($"[bold white]Hello, [cyan]{safeName}[/]! Ready for a match?[/]")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(2, 0),
                BorderStyle = new Style(Color.Cyan1),
            };

            AnsiConsole.Write(Align.Center(panel));
            ConsoleActivity.PrintEmptyLine();
        }

        /// <summary>
        /// Displays the four game modes (Easy, Medium, Impossible, Manual) and starts the selected match.
        /// </summary>
        private void HandlePlayNewGame()
        {
            ConsoleActivity.ShowHeader("SELECT GAME MODE");
            this.ShowUserWelcomeBanner();

            string selectedMode = ConsoleActivity.ShowSelectionMenu(
                "[bold cyan]Choose your gameplay mode:[/]",
                new[] { ModeEasy, ModeMedium, ModeImpossible, ModeManual, ModeBack });

            switch (selectedMode)
            {
                case ModeEasy:
                    this.StartMatch(GameMode.Easy);
                    EasyGameController controller = new EasyGameController();
                    controller.StartMatch(this._currentUser!);
                    break;

                case ModeMedium:
                    this.StartMatch(GameMode.Medium);
                    break;

                case ModeImpossible:
                    this.StartMatch(GameMode.Impossible);
                    break;

                case ModeManual:
                    this.StartMatch(GameMode.Manual);
                    break;

                case ModeBack:
                    return;
            }
        }

        /// <summary>
        /// Launches a Tic-Tac-Toe match configured with the chosen <see cref="GameMode"/>.
        /// </summary>
        /// <param name="mode">The selected gameplay mode.</param>
        private void StartMatch(GameMode mode)
        {
            ConsoleActivity.ShowHeader($"MATCH - {mode.ToString().ToUpper()}");
            ConsoleActivity.PrintInConsole($"[green]Starting {mode} mode for [bold]{Markup.Escape(this._currentUser!.UserName)}[/]...[/]");
            // Next step: Initialize GameSession, render Spectre.Console board, and execute turns
            ConsoleActivity.WaitInConsole();
        }

        /// <summary>
        /// Displays saved match replays for the current user to watch.
        /// </summary>
        private void HandleWatchReplay()
        {
            ConsoleActivity.ShowHeader("WATCH REPLAY");
            ConsoleActivity.PrintInConsole("[yellow]Loading saved replays...[/]");
            ConsoleActivity.WaitInConsole();
        }

        /// <summary>
        /// Clears the active user session and returns to the authentication menu.
        /// </summary>
        private void HandleLogOut()
        {
            string userName = this._currentUser?.UserName ?? "Player";
            this._currentUser = null;
            ConsoleActivity.PrintSuccessMessage($"Logged out successfully. See you soon, {userName}!");
        }
    }
}
