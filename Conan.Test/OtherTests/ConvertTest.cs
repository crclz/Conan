using Conan.Domain.Models;
using Conan.UnitTest.Dependency;
using Autofac;
using AutoMapper;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Conan.API.Controllers.AccessController;

namespace Conan.Test.OtherTests
{
    public class ConvertTest
    {
        [Fact]
        void QUser_Convert_test()
        {
            var scope = ContainerFactory.CreateScope();
            var mapper = scope.Resolve<IMapper>();
            var user = new User(ObjectId.GenerateNewId().ToString(), "user123", "asdaasdad");
            var q = QUser.Convert(user, mapper);

            Assert.Equal(user.Id, q.Id);
            Assert.Equal(user.Username, q.Username);
            Assert.Equal(user.CreatedAt, q.CreatedAt);
            Assert.Equal(user.UpdatedAt, q.UpdatedAt);
        }
    }
}
