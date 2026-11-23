using TicTacToe.Core.Helpers;
using TicTacToe.Core.Model;

namespace TicTacToe.Core.Repository
{
    /// <summary>
    /// JSON file-based implementation of <see cref="IUserRepository"/> for storing and retrieving user data.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="filePath">Optional custom JSON file path. Defaults to <c>Data/users.json</c> in the application base directory.</param>
        public UserRepository(string? filePath = null)
        {
            this._filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
        }

        /// <summary>
        /// Retrieves all registered users from the JSON file.
        /// </summary>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        public List<User> GetAll()
        {
            return JsonHelper.DeserializeFromFile<List<User>>(this._filePath);
        }

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The matching <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        public User? GetById(Guid userId)
        {
            return this.GetAll().FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Retrieves a specific user by their username (case-insensitive).
        /// </summary>
        /// <param name="userName">The username to search for.</param>
        /// <returns>The matching <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        public User? GetByUserName(string userName)
        {
            return this.GetAll().FirstOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks whether a username is already registered in the JSON file (case-insensitive).
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns><c>true</c> if the username exists; otherwise, <c>false</c>.</returns>
        public bool UserNameExists(string userName)
        {
            return this.GetAll().Any(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds a new user and persists the updated list to the JSON file.
        /// </summary>
        /// <param name="user">The <see cref="User"/> entity to add.</param>
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

        /// <summary>
        /// Updates an existing user's details and persists the changes to the JSON file.
        /// </summary>
        /// <param name="updatedUser">The <see cref="User"/> entity containing updated information.</param>
        /// <returns><c>true</c> if the user was found and updated; otherwise, <c>false</c>.</returns>
        public bool Update(User updatedUser)
        {
            if (updatedUser == null)
            {
                throw new ArgumentNullException(nameof(updatedUser));
            }

            List<User> users = this.GetAll();
            int index = users.FindIndex(u => u.UserId == updatedUser.UserId);

            if (index == -1)
            {
                return false;
            }

            users[index] = updatedUser;
            JsonHelper.SerializeToFile(this._filePath, users);
            return true;
        }

        /// <summary>
        /// Deletes a user by their unique identifier and persists the updated list to the JSON file.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        /// <returns><c>true</c> if the user was found and deleted; otherwise, <c>false</c>.</returns>
        public bool Delete(Guid userId)
        {
            List<User> users = GetAll();
            User? existing = users.FirstOrDefault(u => u.UserId == userId);

            if (existing == null)
            {
                return false;
            }

            users.Remove(existing);
            JsonHelper.SerializeToFile(this._filePath, users);
            return true;
        }
    }
}
