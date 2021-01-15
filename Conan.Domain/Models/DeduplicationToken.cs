using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain.Models
{
    public class DeduplicationToken : RootEntity
    {
        public string UserId { get; set; }
        public Guid ClientProvidedToken { get; set; }

        private DeduplicationToken()
        {
            // driver
        }

        public DeduplicationToken(string userId, Guid clientProvidedToken)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ClientProvidedToken = clientProvidedToken;
        }
    }
}
