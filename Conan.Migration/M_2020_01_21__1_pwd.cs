using Conan.Domain.Models;
using Conan.Infrastructure;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Conan.Migration
{
    class M_2020_01_21__1_pwd
    {
        public void DoMigration()
        {
            var password = Environment.GetEnvironmentVariable("CONAN_MONGO_PASSWORD");
            if (password == null)
                throw new Exception("env CONAN_MONGO_PASSWORD is null");

            var host = Environment.GetEnvironmentVariable("CONAN_MONGO_HOST");
            if (host == null)
                throw new Exception("env CONAN_MONGO_HOST is null");

            var context = new OneContext($"mongodb://root:{password}@{host}:27017");

            var userColl = context.Database.GetCollection<User>("users");

            var allUsers = userColl.AsQueryable().ToList();

            foreach (var u in allUsers)
            {
                var pwd = u.Password;

                u.SetPassword(pwd);

                u.Password = null;

                Debug.Assert(u.IsPasswordCorrect(pwd));

                //Console.WriteLine(JsonConvert.SerializeObject(u));

                userColl.ReplaceOne(p => p.Id == u.Id, u);
            }

            Console.WriteLine("End");
        }
    }
}
