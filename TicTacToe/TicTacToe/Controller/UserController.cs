using TicTacToe.Core.Model;
using TicTacToe.Core.Services;
using TicTacToe.View;

namespace TicTacToe.Controller
{
    /// <summary>
    /// Handles console user interactions for authentication and routes valid sessions to the game flow.
    /// </summary>
    public class UserController
    {
        private const string OptionSignUp = "Sign Up";
        private const string OptionLogin = "Login";
        private const string OptionExit = "Exit";
        private readonly IUserManagementService _userService;
        private readonly GameController _gameController;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The user management service handling user business logic.</param>
        /// <param name="gameController">The user management handling user business logic.</param>
        public UserController(IUserManagementService userService, GameController gameController)
        {
            this._userService = userService;
            this._gameController = gameController;
        }

        /// <summary>
        /// Starts the authentication menu..
        /// </summary>
        public void Start()
        {
            while (true)
            {
                ConsoleActivity.ShowHeader("AUTHENTICATION", "TIC TAC TOE");
                string choice = ConsoleActivity.ShowSelectionMenu(
                    "[bold cyan]Select an option using Up/Down arrow keys:[/]",
                    new[] { OptionSignUp, OptionLogin, OptionExit });
                switch (choice)
                {
                    case OptionSignUp:
                        this.HandleSignUp();
                        break;

                    case OptionLogin:
                        this.HandleLogin();
                        break;

                    case OptionExit:
                        ConsoleActivity.ExitApplication();
                        return;
                }
            }
        }

        /// <summary>
        /// Collects and validates registration inputs immediately.
        /// </summary>
        private void HandleSignUp()
        {
            ConsoleActivity.ShowHeader("SIGN UP");

            try
            {
                string userName = InputValidator.GetValidatedUserName();
                string password = InputValidator.GetValidatedPassword();
                InputValidator.GetConfirmedPassword(password);
                DateOnly dateOfBirth = InputValidator.GetValidatedDateOfBirth();

                this._userService.AddNewUser(userName, password, dateOfBirth);
                ConsoleActivity.PrintSuccessMessage("Sign up successful! You can now log in.");
            }
            catch (InvalidOperationException ex)
            {
                ConsoleActivity.PrintInvalidMessage(ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleActivity.PrintInvalidMessage($"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Collects login credentials and delegates authentication to <see cref="IUserManagementService"/>.
        /// </summary>
        private void HandleLogin()
        {
            ConsoleActivity.ShowHeader("LOGIN");

            try
            {
                string userName = ConsoleActivity.GetStringInput("[cyan]Enter username:[/]");
                string password = ConsoleActivity.GetSecretInput("[cyan]Enter password:[/]");

                User? userData = this._userService.Login(userName, password);
                if (userData == null)
                {
                    ConsoleActivity.PrintInvalidMessage("Invalid username or password!");
                    return;
                }

                ConsoleActivity.PrintSuccessMessage($"Welcome back, {userData.UserName}!");
                this._gameController.SetCurrentUser(userData);
                this._gameController.Start();
            }
            catch (Exception ex)
            {
                ConsoleActivity.PrintInvalidMessage($"Login failed: {ex.Message}");
            }
        }
    }
}
