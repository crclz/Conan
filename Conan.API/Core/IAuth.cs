using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API
{
    public interface IAuth
    {
        bool IsAuthenticated { get; }
        string UserId { get; }
    }
}
