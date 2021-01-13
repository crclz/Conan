using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain
{
    public abstract class RootEntity
    {
        public string Id { get; protected set; }
        public long CreatedAt { get; protected set; }
        public long UpdatedAt { get; protected set; }

        protected RootEntity()
        {
            // Designed for children's empty ctor
            // Safe design / convension first: Id and CreatedAt can be freely overwitten if value provided
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        protected RootEntity(string id)
        {
            Id = id;
            CreatedAt = UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        protected void UpdatedAtNow()
        {
            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        [BsonIgnore]
        private List<INotification> _domainEvents { get; set; } = new List<INotification>();

        [BsonIgnore]
        public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();


        internal void AddDomainEvent(INotification notification)
        {
            _domainEvents.Add(notification);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public virtual IEnumerable<INotification> GetDeleteEvents()
        {
            return new List<INotification>();
        }
    }
}
