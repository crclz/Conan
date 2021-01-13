using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public class BadRequestException : StatusCodeException
    {
        public BadCode ErrorCode { get; }

        public BadRequestException(BadCode errorCode, string message) : base(400, message)
        {
            ErrorCode = errorCode;
        }

    }
}
