using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Infrastructure
{
    public class OneContext
    {
        public MongoClient Client { get; }
        public IMongoDatabase Database { get; }

        public const string DbName = "conan";

        public OneContext(string connectionString)
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(DbName);
        }
    }
}
