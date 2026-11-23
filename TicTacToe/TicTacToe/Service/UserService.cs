using System;
using System.Collections.Generic;
using TicTacToe.Core.Model;
using TicTacToe.Core.Repository;

namespace TicTacToe.Core.Services
{
    /// <summary>
    /// Provides business logic for user registration and authentication
    /// between the controller and <see cref="IUserRepository"/>.
    /// </summary>
    public class UserService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository instance.</param>
        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Enforces business rules (such as username uniqueness) and registers a new user in the repository.
        /// </summary>
        /// <param name="username">The validated username.</param>
        /// <param name="password">The validated password.</param>
        /// <param name="dateOfBirth">The validated date of birth.</param>
        /// <exception cref="InvalidOperationException">Thrown when the username is already registered.</exception>
        public void AddNewUser(string username, string password, DateOnly dateOfBirth)
        {
            string cleanUserName = username.Trim();

            if (this._userRepository.UserNameExists(cleanUserName))
            {
                throw new InvalidOperationException($"Username '{cleanUserName}' is already taken.");
            }

            User newUser = new User(cleanUserName, password, dateOfBirth);
            this._userRepository.Add(newUser);
        }

        /// <summary>
        /// Authenticates a user by matching the username and password against stored records.
        /// </summary>
        /// <param name="name">The username entered by the user.</param>
        /// <param name="password">The password entered by the user.</param>
        /// <returns>The authenticated <see cref="User"/> if credentials are valid; otherwise, <c>null</c>.</returns>
        public User? Login(string name, string password)
        {
            User? existingUser = this._userRepository.GetByUserName(name.Trim());
            if (existingUser == null)
            {
                return null;
            }

            if (existingUser.Password == password)
            {
                return existingUser;
            }

            return null;
        }

        /// <summary>
        /// Checks whether a username is already registered.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if the username exists; otherwise, <c>false</c>.</returns>
        public bool IsUserNameTaken(string username)
        {
            return this._userRepository.UserNameExists(username.Trim());
        }

        /// <summary>
        /// Retrieves all registered users from the repository.
        /// </summary>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        public List<User> GetAllUsers()
        {
            return this._userRepository.GetAll();
        }
    }
}
