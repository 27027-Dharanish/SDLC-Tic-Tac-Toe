using System;
using System.Text.Json.Serialization;

namespace TicTacToe.Core.Model
{
    /// <summary>
    /// Serves as the foundational entity for all user records.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class for JSON deserialization.
        /// </summary>
        [JsonConstructor]
        public User()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with specified user details.
        /// </summary>
        /// <param name="name">The username for the account.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="dateOfBirth">The user's date of birth.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="password"/> is <see langword="null"/>.</exception>
        public User(string name, string password, DateOnly dateOfBirth)
        {
            this.UserId = Guid.NewGuid();
            this.UserName = name ?? throw new ArgumentNullException(nameof(name));
            this.Password = password ?? throw new ArgumentNullException(nameof(password));
            this.Age = dateOfBirth;
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <value>
        /// User unique id.
        /// </value>
        public Guid UserId { get; init; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>
        /// The unique display or login name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        /// <value>
        /// The user's account password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's date of birth.
        /// </summary>
        /// <value>
        /// The calendar date on which the user was born.
        /// </value>
        public DateOnly Age { get; set; }
    }
}
