using System;
using System.Collections.Generic;
using TicTacToe.Core.Model;

namespace TicTacToe.Core.Services
{
    /// <summary>
    /// Defines user management operations acting as an intermediary between the controller and <see cref="Repository.IUserRepository"/>.
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Validates user details and registers a new user account.
        /// </summary>
        /// <param name="username">The desired username.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="dateOfBirth">The user's date of birth.</param>
        void AddNewUser(string username, string password, DateOnly dateOfBirth);

        /// <summary>
        /// Authenticates a user using their username and password.
        /// </summary>
        /// <param name="name">The username entered by the user.</param>
        /// <param name="password">The password entered by the user.</param>
        /// <returns>The matching <see cref="User"/> if authentication succeeds; otherwise, <c>null</c>.</returns>
        User? Login(string name, string password);

        /// <summary>
        /// Checks whether a username already exists in the repository.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if the username is already taken; otherwise, <c>false</c>.</returns>
        bool IsUserNameTaken(string username);

        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        List<User> GetAllUsers();
    }
}
