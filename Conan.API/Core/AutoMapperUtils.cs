using Conan.Domain.Models;
using Conan.Dtos;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API
{
    public static class AutoMapperUtils
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(IdDto).Assembly);
            });
            var mapper = config.CreateMapper();

            return mapper;
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            IMapper mapper = CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
