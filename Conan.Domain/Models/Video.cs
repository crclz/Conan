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

        /// <summary> null if dont have a link </summary>
        public string BiliPlayId { get; set; }

        private Video()
        {
            // required by driver
        }

        public Video(string id, string title, bool isTV, int seqId, int publish, string biliPlayId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            IsTV = isTV;
            SeqId = seqId;
            Publish = publish;
            BiliPlayId = biliPlayId;
        }
    }
}
