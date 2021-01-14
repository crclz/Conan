using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Domain.Models;
using Conan.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> UserRepository;
        private readonly IAuth Auth;
        private readonly Guardian Guardian;

        public UsersController(
            IRepository<User> userRepository,
            IAuth auth,
            Guardian guardian)
        {
            UserRepository = userRepository;
            Auth = auth;
            Guardian = guardian;
        }

        public class CreateUserModel
        {
            [Required]
            [MinLength(5)]
            [MaxLength(16)]
            public string Username { get; set; }

            [Required]
            [MinLength(6)]
            [MaxLength(32)]
            public string Password { get; set; }
        }

        [HttpPost]
        public async Task<IdDto> CreateUser([FromBody] CreateUserModel model)
        {
            // check username is lower digital
            if (!Domain.Models.User.IsLowerOrDigit(model.Username))
                throw new BadRequestException(BadCode.InvalidModel, "用户名必须是小写字母或者数字");

            // check username not exist
            var user = await UserRepository.SingleAsync(p => p.Username == model.Username);
            if (user != null)
                throw new BadRequestException(BadCode.UniqueViolation, "用户名已经被注册");

            // save
            user = new User(ObjectId.GenerateNewId().ToString(), model.Username, model.Password);
            await UserRepository.SaveAsync(user);

            return new IdDto(user.Id);
        }

        [HttpGet("client-error")]
        public void ProduceClientError(int status)
        {
            switch (status)
            {
                case 400:
                    throw new BadRequestException(BadCode.UniqueViolation, "a400");
                case 401:
                    throw new UnauthorizedException("a401");
                case 403:
                    throw new ForbidException("a403");
                case 404:
                    throw new NotFoundException("a404");
                default:
                    throw new ArgumentException();
            }
        }

        [HttpGet("{userId}/video-views")]
        public IEnumerable<VideoView> GetUserVideoViews(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
