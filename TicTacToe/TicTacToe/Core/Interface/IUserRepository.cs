using TicTacToe.Core.Model;

namespace TicTacToe.Core.Repository
{
    /// <summary>
    /// Defines data access operations for managing <see cref="User"/> entities.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all registered users from the data store.
        /// </summary>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        List<User> GetAll();

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The matching <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        User? GetById(Guid userId);

        /// <summary>
        /// Retrieves a specific user by their username.
        /// </summary>
        /// <param name="userName">The username to search for.</param>
        /// <returns>The matching <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        User? GetByUserName(string userName);

        /// <summary>
        /// Checks whether a username is already registered in the data store.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns><c>true</c> if the username exists; otherwise, <c>false</c>.</returns>
        bool UserNameExists(string userName);

        /// <summary>
        /// Adds a new user to the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> entity to add.</param>
        void Add(User user);

        /// <summary>
        /// Updates an existing user's details in the data store.
        /// </summary>
        /// <param name="updatedUser">The <see cref="User"/> entity containing updated information.</param>
        /// <returns><c>true</c> if the user was found and updated; otherwise, <c>false</c>.</returns>
        bool Update(User updatedUser);

        /// <summary>
        /// Deletes a user from the data store by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        /// <returns><c>true</c> if the user was found and deleted; otherwise, <c>false</c>.</returns>
        bool Delete(Guid userId);
    }
}
