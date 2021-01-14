using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain.Models
{
    public class VideoView : RootEntity
    {
        public string VideoId { get; private set; }
        public string UserId { get; private set; }

        private VideoView()
        {

        }

        public VideoView(string videoId, string userId)
        {
            VideoId = videoId ?? throw new ArgumentNullException(nameof(videoId));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public void RefreshViewTime()
        {
            UpdatedAtNow();
            CreatedAt = UpdatedAt;
        }
    }
}
