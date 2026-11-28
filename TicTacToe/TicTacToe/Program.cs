using TicTacToe.Controller;
using TicTacToe.Core.Repository;
using TicTacToe.Core.Service;
using TicTacToe.Core.Services;

namespace Assignments
{
    /// <summary>
    /// Represents the main entry point for the application and handles initial setup.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point of the program and start the controller
        /// </summary>
        public static void Main()
        {
            IUserRepository userRepository = new UserRepository();
            IUserManagementService userService = new UserService(userRepository);
            GameRepository gameRepository = new GameRepository();
            GameService gameService = new GameService(gameRepository);
            GameController gameController = new GameController(gameService);
            UserController controller = new UserController(userService, gameController);
            controller.Start();
        }
    }
}