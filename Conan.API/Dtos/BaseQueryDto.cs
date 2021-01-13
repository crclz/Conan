using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Dtos
{
    public class BaseQueryDto
    {
        public string Id { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
    }
}
