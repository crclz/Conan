using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Conan;
using Conan.Test.Dependency;
using static Conan.API.Controllers.UsersController;
using Conan.UnitTest.Utils;
using Conan.API;

namespace Conan.Test.Controllers
{
    public class ClientErrorTest
    {
        private readonly CustomWebApplicationFactory<Startup> _factory = new CustomWebApplicationFactory<Startup>();


        [Fact]
        public async Task ModelValidationTest()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var model = new CreateUserModel
            {
                Username = "a",
                Password = "b"
            };
            var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/users", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var responseEntity = (await response.Content.ReadAsStringAsync()).FromJson<dynamic>();
            var errorMessage = (string)responseEntity.message;
            Assert.NotNull(errorMessage);
            Assert.Contains("Username", errorMessage);
            Assert.Contains("Password", errorMessage);
        }

        [Fact]
        public async Task ClientError_400()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users/client-error?status=400");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var responseEntity = (await response.Content.ReadAsStringAsync()).FromJson<dynamic>();
            var errorMessage = (string)responseEntity.message;
            Assert.NotNull(errorMessage);
            Assert.Equal("a400", errorMessage);
        }

        [Fact]
        public async Task ClientError_401()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users/client-error?status=401");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            var responseEntity = (await response.Content.ReadAsStringAsync()).FromJson<dynamic>();
            var errorMessage = (string)responseEntity.message;
            Assert.NotNull(errorMessage);
            Assert.Equal("a401", errorMessage);
        }

        [Fact]
        public async Task ClientError_403()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users/client-error?status=403");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
            var responseEntity = (await response.Content.ReadAsStringAsync()).FromJson<dynamic>();
            var errorMessage = (string)responseEntity.message;
            Assert.NotNull(errorMessage);
            Assert.Equal("a403", errorMessage);
        }

        [Fact]
        public async Task ClientError_404()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users/client-error?status=404");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var responseEntity = (await response.Content.ReadAsStringAsync()).FromJson<dynamic>();
            var errorMessage = (string)responseEntity.message;
            Assert.NotNull(errorMessage);
            Assert.Equal("a404", errorMessage);
        }
    }
}
