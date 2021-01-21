using Ardalis.GuardClauses;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Conan.Domain.Models
{
    public class User : RootEntity
    {
        public string Username { get; private set; }
        public string Salt { get; private set; }
        public string PasswordHash { get; private set; }

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

            Id = id;
            Username = username;
            SetPassword(password);
        }

        public static bool IsLowerOrDigit(string s)
        {
            return s.All(p => char.IsLower(p) || char.IsDigit(p));
        }

        public bool IsPasswordCorrect(string password)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var passAndSalt = password + Salt;
            var sha256 = GetSHA256(passAndSalt);

            return sha256 == PasswordHash;
        }

        private static string GetSHA256(string s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            var data = Encoding.UTF8.GetBytes(s);

            using (var h = SHA256.Create())
            {
                var hashBytes = h.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }


        public void SetPassword(string password)
        {
            Guard.Against.OutOfRange(password.Length, nameof(password) + ".Length", 6, 32);

            using var rng = new RNGCryptoServiceProvider();
            var saltBytes = new byte[64];
            rng.GetBytes(saltBytes);

            Salt = Convert.ToBase64String(saltBytes);

            var passAndSalt = password + Salt;
            PasswordHash = GetSHA256(passAndSalt);
        }
    }
}
