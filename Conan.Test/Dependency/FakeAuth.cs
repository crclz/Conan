using Conan.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Test.Dependency
{
    public class FakeAuth : IAuth
    {
        public bool IsAuthenticated => RealUserId != null;

        public string UserId => RealUserId;

        public string RealUserId { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
