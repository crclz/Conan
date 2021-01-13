using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class ValueObjectAttribute : Attribute
    {

    }
}
