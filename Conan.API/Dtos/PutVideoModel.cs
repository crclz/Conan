using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Dtos
{
    public class PutVideoModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Title { get; set; }

        public bool IsTV { get; set; }

        public int SeqId { get; set; }

        public int Publish { get; set; }

        public string BiliPlayId { get; set; }
    }
}
