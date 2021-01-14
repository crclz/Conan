using AutoMapper;
using Con.API;
using Conan.API.Dtos;
using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Domain.Models;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IRepository<User> userRepository;
        private readonly IAuth auth;
        private readonly IMapper mapper;
        private readonly Guardian guardian;

        public AccessController(IRepository<User> userRepository, IAuth auth, IMapper mapper, Guardian guardian)
        {
            this.userRepository = userRepository;
            this.auth = auth;
            this.mapper = mapper;
            this.guardian = guardian;
        }


        public class LoginModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [MaxLength(100)]
            public string Password { get; set; }

        }

        [HttpPost("login")]
        public async Task<string> Login([FromBody] LoginModel model)
        {
            var user = await userRepository.SingleAsync(p => p.Username == model.Username);

            if (user == null)
                throw new BadRequestException(BadCode.UserNotFound, "用户不存在");

            if (user.Password != model.Password)
                throw new BadRequestException(BadCode.WrongPassword, "密码错误");

            // login ok, attach login info to token

            var nowAdd120Days = DateTimeOffset.UtcNow.AddDays(120).ToUnixTimeSeconds();

            var token = new JwtBuilder()
                 .WithAlgorithm(new HMACSHA256Algorithm())
                 .WithSecret(AuthMiddleware.JwtSecret)
                 .AddClaim("exp", nowAdd120Days)
                 .AddClaim("username", user.Username)
                 .AddClaim("password", user.Password)
                 .Encode();

            var option = new CookieOptions()
            {
                Expires = DateTime.Now.AddYears(10)
            };
            Response.Cookies.Append(AuthMiddleware.LoginInfoKey, token, option);

            return token;
        }

        [AutoMap(typeof(User))]
        public class QUser : BaseQueryDto
        {
            public string Username { get; set; }

            public static QUser Convert(User user, IMapper mapper)
            {
                if (user == null)
                    return null;

                return mapper.Map<User, QUser>(user);
            }
        }

        [HttpGet("me")]
        public async Task<QUser> GetMe()
        {
            if (!auth.IsAuthenticated)
            {
                return null;
            }
            else
            {
                var user = await userRepository.ByIdAsync(auth.UserId);
                var q = QUser.Convert(user, mapper);

                return q;
            }
        }

        [HttpPost("cookie-logout")]
        public void CookieLogout()
        {
            Response.Cookies.Delete(AuthMiddleware.LoginInfoKey);
        }
    }
}
