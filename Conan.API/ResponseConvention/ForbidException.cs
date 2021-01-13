using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public class ForbidException : StatusCodeException
    {
        public ForbidException(string message) : base(403, message)
        {
        }
    }
}
