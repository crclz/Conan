using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain.Models
{
    public class Video : RootEntity
    {
        public string Title { get; set; }
        public bool IsTV { get; set; }
        public int SeqId { get; set; }
        public int Publish { get; set; }
        public string BiliPlayId { get; set; }
    }
}
