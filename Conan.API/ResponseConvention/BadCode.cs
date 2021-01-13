using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public enum BadCode
    {
        UniqueViolation,
        UserNotFound,
        WrongPassword,
        ReferenceNotFound,
        InvalidModel,
        InvalidOperation,
    }
}
