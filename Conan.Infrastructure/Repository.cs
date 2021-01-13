using Conan.Domain;
using Ardalis.GuardClauses;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Conan.Infrastructure
{
    // TODO: auto commit support, when in production

    public abstract class Repository<T> : IRepository<T> where T : RootEntity
    {
        private readonly IMediator mediator;
        private OneContext context { get; }
        private IMongoCollection<T> Collection { get; }

        public Repository(OneContext context, IMediator mediator)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            // Get mongo collection
            var collectionName = typeof(T).Name.ToLower() + "s";
            Collection = context.Database.GetCollection<T>(collectionName);
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await Collection.AsQueryable().Where(predicate).SingleOrDefaultAsync();
        }

        public async Task<T> ByIdAsync(string id)
        {
            return await Collection.AsQueryable().Where(p => p.Id == id).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            // Upsert entity
            var options = new ReplaceOptions
            {
                IsUpsert = true
            };
            await Collection.ReplaceOneAsync(p => p.Id == entity.Id, entity, options);

            // Dispatch events
            foreach (var @event in entity.DomainEvents)
            {
                await mediator.Publish(@event);
            }
            entity.ClearDomainEvents();
        }

        public async Task DeleteAsync(T entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            if (entity.DomainEvents.Any())
                throw new ArgumentException("Entity to delete should not have events", nameof(entity));

            var events = entity.GetDeleteEvents();

            // Delete entity
            await Collection.DeleteOneAsync(p => p.Id == entity.Id);

            // Dispatch events
            foreach (var @event in events)
            {
                await mediator.Publish(@event);
            }
        }

        public IQueryable<T> Query()
        {
            return Collection.AsQueryable();
        }
    }
}
