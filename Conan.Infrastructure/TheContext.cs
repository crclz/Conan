using Conan.Domain;
using Conan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Infrastructure
{
    public class TheContext
    {
        public IRepository<User> Users { get; }
        public IRepository<Video> Videos { get; }
        public IRepository<DeduplicationToken> Dedups { get; }
        public IRepository<VideoView> VideoViews { get; }
        public IRepository<StoryLine> Storylines { get; }

        public TheContext(IRepository<User> users, IRepository<Video> videos, IRepository<DeduplicationToken> dedups, IRepository<VideoView> videoViews, IRepository<StoryLine> storylines)
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
            Videos = videos ?? throw new ArgumentNullException(nameof(videos));
            Dedups = dedups ?? throw new ArgumentNullException(nameof(dedups));
            VideoViews = videoViews ?? throw new ArgumentNullException(nameof(videoViews));
            Storylines = storylines ?? throw new ArgumentNullException(nameof(storylines));
        }
    }
}
