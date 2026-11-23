using TicTacToe.Controller;
using TicTacToe.Core.Repository;
using TicTacToe.Core.Services;

namespace Assignments
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            IUserRepository userRepository = new UserRepository();
            IUserManagementService userService = new UserService(userRepository);
            UserController controller = new UserController(userService);
            controller.Start();
        }
    }
}