using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public class NotFoundException : StatusCodeException
    {
        public NotFoundException(string message) : base(404, message)
        {
        }
    }
}
