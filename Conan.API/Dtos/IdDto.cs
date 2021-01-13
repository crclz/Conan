using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.Dtos
{
    public class IdDto
    {
        public string Id { get; set; }

        public IdDto(string id)
        {
            Guard.Against.NullOrWhiteSpace(id, nameof(id));
            Id = id;
        }
    }
}
