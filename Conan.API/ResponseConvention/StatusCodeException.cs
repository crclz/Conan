using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public abstract class StatusCodeException : Exception
    {
        public int Status { get; }

        public StatusCodeException(int status, string message) : base(message)
        {
            Status = status;
        }
    }
}
