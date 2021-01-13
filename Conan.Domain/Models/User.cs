using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain.Models
{
    public class User : RootEntity
    {
        public string Email { get; private set; }

        public string Password { get; private set; }

        private User()
        {
            // Required by mongodb driver
        }

        public User(string id, string email, string password)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.BsonObjectId(id, nameof(id));
            Guard.Against.Null(email, nameof(email));

            Guard.Against.Null(password, nameof(password));
            Guard.Against.OutOfRange(password.Length, nameof(password) + ".Length", 8, 32);

            Id = id;
            Email = email;
            Password = password;
        }
    }
}
