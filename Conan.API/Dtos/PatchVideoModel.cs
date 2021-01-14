using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Dtos
{
    public class PatchVideoModel
    {
        [MinLength(1)]
        [MaxLength(50)]
        public string Title { get; set; }
        public bool? IsTV { get; set; }
        public int? SeqId { get; set; }
        public int? Publish { get; set; }

        /// <summary>
        /// _ if want to delete
        /// </summary>
        public string BiliPlayId { get; set; }
    }
}
