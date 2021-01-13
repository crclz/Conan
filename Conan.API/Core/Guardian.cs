using Conan.API.ResponseConvention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API
{
    public class Guardian
    {
        private readonly IAuth _auth;

        public Guardian(IAuth auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        public void ThrowWhenNotAuthenticated()
        {
            if (!_auth.IsAuthenticated)
            {
                throw new UnauthorizedException("你还未登录");
            }
        }
    }
}
