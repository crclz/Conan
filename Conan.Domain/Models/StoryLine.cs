using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conan.Domain.Models
{
    public class StoryLine : RootEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Videos { get; set; } = new List<string>();

        private StoryLine()
        {
            // ef
        }

        public StoryLine(string name, string description, List<string> videos)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Videos = videos ?? throw new ArgumentNullException(nameof(videos));
            Videos = videos.ToList();
        }
    }
}
