using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conan.Domain.Models
{
    public class User : RootEntity
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        private User()
        {
            // Required by mongodb driver
        }

        public User(string id, string username, string password)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.BsonObjectId(id, nameof(id));

            Guard.Against.Null(username, nameof(username));
            Guard.Against.Null(password, nameof(password));

            if (!IsLowerOrDigit(username))
                throw new ArgumentException("char in username should be lower letter or digit");

            Guard.Against.OutOfRange(username.Length, nameof(username) + ".Length", 1, 16);
            Guard.Against.OutOfRange(password.Length, nameof(password) + ".Length", 6, 32);

            Id = id;
            Username = username;
            Password = password;
        }

        public static bool IsLowerOrDigit(string s)
        {
            return s.All(p => char.IsLower(p) || char.IsDigit(p));
        }
    }
}
