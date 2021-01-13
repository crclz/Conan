using Conan.Domain;
using Conan.Domain.Models;
using Ardalis.GuardClauses;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conan.API;

namespace Con.API
{
    public class AuthMiddleware : IMiddleware
    {
        public static readonly string SECRET = "e07df1bf8d9bb02b564bb6dfb80ed122";
        private const string AppName = "Conan";


        private readonly Auth _auth;
        private readonly IRepository<User> userRepository;

        public AuthMiddleware(IAuth auth, IRepository<User> userRepository)
        {
            Guard.Against.Null(auth, nameof(auth));

            _auth = (Auth)auth;
            this.userRepository = userRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Cookies["LoginInfo"];
            if (token == null)
            {
                goto fail;
            }

            try
            {
                var info = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(SECRET)
                    .MustVerifySignature()
                    .Decode<Dictionary<string, string>>(token);

                var email = info["email"];
                var password = info["password"];

                if (email == null || password == null)
                    goto fail;

                var user = await userRepository.SingleAsync(p => p.Email == email);
                if (user == null)
                    goto fail;

                if (user.Password != password)
                    goto fail;

                // Login info check ok

                _auth.RealUserId = user.Id;

                goto success;
            }
            catch (TokenExpiredException)
            {
                goto fail;
            }
            catch (SignatureVerificationException)
            {
                goto fail;
            }

        fail:
            context.Response.Cookies.Delete("LoginInfo");

        success:
            await next(context);
        }
    }
}
