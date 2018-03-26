using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Materialise.FrontendDays.Bot.Api.Commands;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Commands.Predicates;
using Materialise.FrontendDays.Bot.Api.Commands.Predicates.Contracts;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Filters;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Mediator;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Materialise.FrontendDays.Bot.Api.Services;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Materialise.FrontendDays.Bot.Api.Validators;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            }).AsSelf().SingleInstance();

            builder.RegisterType<RequestEmailPredicate>()
                .AsSelf();

            builder.RegisterType<ResponseEmailPredicate>()
                .AsSelf();

            builder.RegisterType<StartPredicate>()
                .AsSelf();

            builder.RegisterType<PlayGamePredicate>()
                .AsSelf();

            builder.RegisterType<AnswerPredicate>()
                .AsSelf();

            builder.RegisterType<DefaultPredicate>()
                .AsSelf();

            builder.Register(context => new CommandsStrategy(context.Resolve<IComponentContext>(), new[]
                {
                    new KeyValuePair<ICommandPredicate, Type>(context.Resolve<RequestEmailPredicate>(), typeof(RequestEmailCommand)),
                    new KeyValuePair<ICommandPredicate, Type>(context.Resolve<ResponseEmailPredicate>(), typeof(ResponseEmailCommand)),
                    new KeyValuePair<ICommandPredicate, Type>(context.Resolve<StartPredicate>(), typeof(StartCommand)),
                    new KeyValuePair<ICommandPredicate, Type>(context.Resolve<PlayGamePredicate>(), typeof(PlayGameCommand)),
                    new KeyValuePair<ICommandPredicate, Type>(context.Resolve<AnswerPredicate>(), typeof(AnswerCommand)),
                    new KeyValuePair<ICommandPredicate, Type>(context.Resolve<DefaultPredicate>(), typeof(DefaultCommand))
                }))
                .As<ICommandsStrategy>()
                .SingleInstance();

            builder.RegisterType<StartCommand>()
                .AsSelf();

            builder.RegisterType<AnswerCommand>()
                .AsSelf();

            builder.RegisterType<PlayGameCommand>()
                .AsSelf();

            builder.RegisterType<NextQuestionCommand>()
                .AsSelf();

            builder.RegisterType<GameFinishedCommand>()
                .AsSelf();

            builder.RegisterType<RequestEmailCommand>()
                .AsSelf();

            builder.RegisterType<ResponseEmailCommand>()
                .AsSelf();

            builder.RegisterType<DefaultCommand>()
                .AsSelf();

            builder.RegisterType<DbRepository<User>>()
                .As<IDbRepository<User>>();

            builder.RegisterType<DbRepository<Answer>>()
                .As<IDbRepository<Answer>>();

            builder.RegisterType<QuestionsRepository>()
                .As<IDbRepository<Question>>();

            builder.RegisterType<UserAnswerRepository>()
                .As<IUserAnswerRepository>();

            builder.RegisterType<UserRegistrationService>()
                .As<IUserRegistrationService>();

            builder.Register(context => Configuration.GetSection("questions").Get<Question[]>())
                .AsSelf();

            builder.Register(context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                return (DbContextOptions<BotContext>)optionsBuilder.Options;
            }).As<DbContextOptions<BotContext>>();

            builder.RegisterType<Localization>()
                .WithParameter("filename", "Materialise.FrontendDays.Bot.Api.Resources.localization.json")
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<MessageSender>().AsSelf();
            builder.RegisterType<EmailValidator>().AsSelf();

            var admins = Configuration.GetSection("admins").Get<Admin[]>();

            // mediator itself
            builder
                .RegisterType<MediatR.Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request handlers
            builder
                .Register<SingleInstanceFactory>(ctx => {
                    var c = ctx.Resolve<IComponentContext>();
                    return t => c.TryResolve(t, out var o) ? o : null;
                })
                .InstancePerLifetimeScope();

            // notification handlers
            builder
                .Register<MultiInstanceFactory>(ctx => {
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

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            using (var context = new BotContext(serviceScope.ServiceProvider.GetRequiredService<DbContextOptions<BotContext>>()))
            {
                context.Database.Migrate();
            }

            app.UseMvc();

            app.UseAuthentication();
        }
    }
}
