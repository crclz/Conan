using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API
{
    public static class QueryExecuteHelper
    {
        public static async Task<int> HelperCountAsync<T>(this IQueryable<T> query)
        {
            if (query is IMongoQueryable<T>)
            {
                return await (query as IMongoQueryable<T>).CountAsync();
            }
            else
            {
                // Memory
                // WARN: do not use EF as context
                return query.Count();
            }
        }

        public static async Task<bool> HelperAnyAsync<T>(this IQueryable<T> query)
        {
            if (query is IMongoQueryable<T>)
            {
                return await (query as IMongoQueryable<T>).AnyAsync();
            }
            else
            {
                // Memory
                // WARN: do not use EF as context
                return query.Any();
            }
        }

        public static async Task<List<T>> HelperToListAsync<T>(this IQueryable<T> query)
        {
            if (query is IMongoQueryable<T>)
            {
                return await (query as IMongoQueryable<T>).ToListAsync();
            }
            else
            {
                // Memory
                // WARN: do not use EF as context
                return query.ToList();
            }
        }
    }
}
