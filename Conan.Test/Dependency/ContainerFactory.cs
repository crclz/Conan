using Autofac;
using Autofac.Core;
using Conan.API;
using Conan.API.Controllers;
using MediatR;
using Conan.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using Conan.Test.Dependency;
using Conan.Domain.Models;
using MediatR.Extensions.Autofac.DependencyInjection;
using Autofac.Builder;
using Conan.Infrastructure;
using AutoMapper;
using Conan;
using System.Reflection;

namespace Conan.UnitTest.Dependency
{
    class ContainerFactory
    {
        public static ContainerBuilder Builder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();

            // Mediatr and Event handlers
            builder.RegisterMediatR(typeof(Startup).Assembly);

            // Infrastructure
            //builder.RegisterType<FakeTransactionControl>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<>)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(InMemoryRepository<>))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Services
            builder.RegisterType<FakeAuth>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<Guardian>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterInstance(AutoMapperUtils.CreateMapper()).As<IMapper>().SingleInstance();
            builder.RegisterType<Guardian>().AsSelf().InstancePerLifetimeScope();

            // Controllers
            builder.RegisterType<UsersController>().InstancePerLifetimeScope();


            return builder;
        }

        public static IContainer Create()
        {
            var container = Builder().Build();

            return container;
        }

        public static ILifetimeScope CreateScope()
        {
            var scope = Builder().Build().BeginLifetimeScope();

            return scope;
        }
    }
}
