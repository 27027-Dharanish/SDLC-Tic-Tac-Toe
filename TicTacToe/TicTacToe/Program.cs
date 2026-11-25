using TicTacToe.Controller;
using TicTacToe.Core.Repository;
using TicTacToe.Core.Service;
using TicTacToe.Core.Services;

namespace Assignments
{
    public class Program
    {
        
        public static void Main(string[] args)
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