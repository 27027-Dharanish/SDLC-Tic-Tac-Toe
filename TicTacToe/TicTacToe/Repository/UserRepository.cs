using TicTacToe.Core.Helpers;
using TicTacToe.Core.Model;

namespace TicTacToe.Core.Repository
{
    /// <summary>
    /// Repository for storing and retrieving user data.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="filePath">Optional custom JSON file path.param>
        public UserRepository(string? filePath = null)
        {
            this._filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
        }

        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        public List<User> GetAll()
        {
            return JsonHelper.DeserializeFromFile<List<User>>(this._filePath);
        }

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The matching user if found; otherwise, null</returns>
        public User? GetById(Guid userId)
        {
            return this.GetAll().FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Retrieves a specific user by their username.
        /// </summary>
        /// <param name="userName">The username to search.</param>
        /// <returns>The matching user if found; otherwise, null.</returns>
        public User? GetByUserName(string userName)
        {
            return this.GetAll().FirstOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks whether a username is already present.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if the username exists; otherwise, false.</returns>
        public bool UserNameExists(string userName)
        {
            return this.GetAll().Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        public void Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            List<User> users = this.GetAll();
            users.Add(user);
            JsonHelper.SerializeToFile(this._filePath, users);
        }
    }
}
