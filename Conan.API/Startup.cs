using Con.API;
using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;

namespace Conan.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            {
                var password = Environment.GetEnvironmentVariable("CONAN_MONGO_PASSWORD");
                if (password == null)
                    throw new Exception("env CONAN_MONGO_PASSWORD is null");

                var host = Environment.GetEnvironmentVariable("CONAN_MONGO_HOST");
                if (host == null)
                    throw new Exception("env CONAN_MONGO_HOST is null");

                services.AddSingleton(new OneContext($"mongodb://root:{password}@{host}:27017"));
            }



            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<TheContext>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<IAuth, Auth>();

            services.AddScoped<AuthMiddleware>();
            services.AddAutoMapper();
            services.AddScoped<Guardian>();

            services.AddSwaggerGen();

            services.AddControllers(c =>
            {
                c.Filters.Add<ExceptionHandlingAttribute>();
            })
                .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorList = context.ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();

                    var message = string.Join("; ", errorList);

                    var result = new BadRequestObjectResult(new
                    {
                        message = message
                    });

                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                    return result;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<AuthMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
