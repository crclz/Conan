using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Domain.Models;
using Conan.UnitTest.Dependency;
using Conan.API.Controllers;
using Autofac;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Conan.API.Controllers.UsersController;

namespace Conan.Test.Controllers
{
    public class UsersControllerTest
    {
        #region CreateUser
        [Fact]
        async Task CreateUser_returns_bad_request_when_email_exist()
        {
            // Arrange
            var scope = ContainerFactory.CreateScope();
            var controller = scope.Resolve<UsersController>();
            var userRepository = scope.Resolve<IRepository<User>>();

            var username = "user231";
            var user = new User(ObjectId.GenerateNewId().ToString(), username, "asdasdasdasd");
            await userRepository.SaveAsync(user);

            // Act and Assert
            var model = new CreateUserModel
            {
                Username = username,
                Password = "aaaaaaaa"
            };
            var result = await Assert.ThrowsAsync<BadRequestException>(
                () => controller.CreateUser(model));

            Assert.Equal(BadCode.UniqueViolation, result.ErrorCode);
        }

        [Fact]
        async Task CreateUser_saves_to_db_when_all_ok()
        {
            // Arrange
            var scope = ContainerFactory.CreateScope();
            var controller = scope.Resolve<UsersController>();
            var userRepository = scope.Resolve<IRepository<User>>();

            // Act
            var model = new CreateUserModel
            {
                Username = "u123123",
                Password = "aaaaaaAa"
            };
            var idDto = await controller.CreateUser(model);

            // Assert
            var user = await userRepository.ByIdAsync(idDto.Id);
            Assert.NotNull(user);
            Assert.Equal(model.Username, user.Username);

            Assert.True(user.IsPasswordCorrect(model.Password));
        }

        #endregion
    }
}
