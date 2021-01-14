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
        public static readonly string JwtSecret = "e07df1bf8d9bb02b564bb6dfb80ed122";

        private static readonly string AppName = "Conan";
        public static readonly string LoginInfoKey = $"{AppName}-LoginInfo";

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
            var token = context.Request.Headers[LoginInfoKey].FirstOrDefault();

            if (token == null)
                token = context.Request.Cookies[LoginInfoKey];

            if (token == null)
            {
                goto fail;
            }

            try
            {
                var info = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(JwtSecret)
                    .MustVerifySignature()
                    .Decode<Dictionary<string, string>>(token);

                var username = info["username"];
                var password = info["password"];

                if (username == null || password == null)
                    goto fail;

                var user = await userRepository.SingleAsync(p => p.Username == username);
                if (user == null)
                    goto fail;

                if (user.Password != password)
                    goto fail;

                // Login info check ok, modify auth object

                _auth.RealUserId = user.Id;
                _auth.IsAdmin = user.Username == "admin";

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
            context.Response.Cookies.Delete(LoginInfoKey);

        success:
            await next(context);
        }
    }
}
