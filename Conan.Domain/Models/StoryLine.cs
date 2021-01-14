using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain.Models
{
    public class StoryLine : RootEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Videos { get; set; } = new List<string>();
    }
}
