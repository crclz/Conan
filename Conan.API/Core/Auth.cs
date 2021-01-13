using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API
{
    public class Auth : IAuth
    {
        public string RealUserId { get; set; } = null;
        public bool IsAuthenticated => RealUserId != null;
        public string UserId => RealUserId;
    }
}
