using Conan.Domain;
using Conan.UnitTest.Utils;
using Ardalis.GuardClauses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Conan.UnitTest.Dependency
{
    class InMemoryRepository<T> : IRepository<T> where T : RootEntity
    {
        internal Dictionary<string, T> objectStore = new Dictionary<string, T>();
        private readonly IMediator mediator;

        public InMemoryRepository(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<T> ByIdAsync(string id)
        {
            if (objectStore.TryGetValue(id, out T entity))
            {
                var entityCopy = entity.DeepClone();

                return Task.FromResult(entityCopy);
            }
            else
            {
                return Task.FromResult((T)null);
            }
        }

        public Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = objectStore.Values.AsQueryable().Where(predicate).SingleOrDefault();

            if (entity == null)
            {
                return Task.FromResult(entity);
            }
            else
            {
                var entityCopy = entity.DeepClone();
                return Task.FromResult(entityCopy);
            }
        }

        public async Task SaveAsync(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            entity = entity.DeepClone();

            // Upsert entity
            objectStore[entity.Id] = entity;

            // Dispatch events sync
            foreach (var @event in events)
            {
                await mediator.Publish(@event);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            if (entity.DomainEvents.Any())
                throw new ArgumentException("Entity to delete should not have events", nameof(entity));

            var events = entity.GetDeleteEvents();

            // Delete entity
            if (!objectStore.Remove(entity.Id))
            {
                throw new ArgumentException("Id not exist", nameof(entity));
            }

            // Dispatch events
            foreach (var @event in events)
            {
                await mediator.Publish(@event);
            }
        }

        public IQueryable<T> Query()
        {
            return objectStore.Values.Select(p => p.DeepClone()).AsQueryable();
        }

        public Task<List<U>> ToListAsync<U>(IQueryable<U> query)
        {
            var result = query.ToList();
            return Task.FromResult(result);
        }
    }
}
