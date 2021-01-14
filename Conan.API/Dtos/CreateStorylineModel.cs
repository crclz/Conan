using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Dtos
{
    public class CreateStorylineModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(10)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public List<string> Videos { get; set; } = new List<string>();
    }
}
