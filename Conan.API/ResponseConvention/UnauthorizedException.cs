using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public class UnauthorizedException : StatusCodeException
    {
        public UnauthorizedException(string message) : base(401, message)
        {
        }
    }
}
