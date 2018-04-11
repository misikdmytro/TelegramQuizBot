using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Materialise.FrontendDays.Bot.Api.Commands;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Commands.Predicates.Contracts;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Filters;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Mediator;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Materialise.FrontendDays.Bot.Api.Services;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Materialise.FrontendDays.Bot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IContainer ApplicationContainer { get; private set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
            });

            services.AddDbContext<BotContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "basic";
                options.DefaultChallengeScheme = "basic";
            });

            var builder = new ContainerBuilder();

            builder.Register(c => new BotInfo(Configuration["botToken"], Configuration["hostUrl"]))
                .AsSelf();

            builder.RegisterType<TelegramBot>()
                .As<ITelegramBot>();

            builder.Register(context =>
            {
                var bot = context.Resolve<ITelegramBot>();
                return bot.InitializeAsync().Result;
            }).As<ITelegramBotClient>().SingleInstance();

            RegisterPredicates(builder);
            RegisterCommands(builder);

            builder.RegisterType<CommandsFactory>()
                .As<ICommandsFactory>()
                .SingleInstance();

            RegisterRepositories(builder);

            builder.RegisterType<UserRegistrationService>()
                .As<IUserRegistrationService>();

            builder.RegisterType<MessageSender>()
                .As<IMessageSender>();

            builder.Register(context => Configuration.GetSection("questions").Get<Question[]>())
                .AsSelf();

            builder.Register(context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                return (DbContextOptions<BotContext>)optionsBuilder.Options;
            }).As<DbContextOptions<BotContext>>();

            builder.RegisterType<IMessageSender>().AsSelf();

            var admins = Configuration.GetSection("admins").Get<Admin[]>();

            // mediator itself
            builder.RegisterType<MediatR.Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request handlers
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.TryResolve(t, out var o) ? o : null;
            })
                .InstancePerLifetimeScope();

            // notification handlers
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            })
                .InstancePerLifetimeScope();

            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()
            builder.RegisterAssemblyTypes(typeof(CheckStatusRequest).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            BasicAuthenticationFilter.Admins = admins;

            builder.Populate(services);

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseAuthentication();
        }

        private void RegisterPredicates(ContainerBuilder builder)
        {
            builder.RegisterTypes(GetType().Assembly
                .GetTypes()
                .Where(t => typeof(ICommandPredicate).IsAssignableFrom(t))
                .ToArray());
        }

        private void RegisterCommands(ContainerBuilder builder)
        {
            builder.RegisterTypes(GetType().Assembly
                .GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t))
                .ToArray());
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<DbRepository<User>>()
                .As<IDbRepository<User>>();

            builder.RegisterType<DbRepository<Answer>>()
                .As<IDbRepository<Answer>>();

            builder.RegisterType<QuestionsRepository>()
                .As<IDbRepository<Question>>();

            builder.RegisterType<UserAnswerRepository>()
                .As<IUserAnswerRepository>();

            builder.RegisterType<CategoryRepository>()
                .As<ICategoryRepository>();
        }
    }
}
