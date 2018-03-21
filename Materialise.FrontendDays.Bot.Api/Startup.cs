using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Materialise.FrontendDays.Bot.Api.Builders;
using Materialise.FrontendDays.Bot.Api.Commands;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Filters;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Materialise.FrontendDays.Bot.Api.Services;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
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

            builder.Register(context => new CommandsStrategy(context.Resolve<IComponentContext>(), new[]
                {
                    new KeyValuePair<string, Type>("/start", typeof(StartCommand)),
                    new KeyValuePair<string, Type>("/play", typeof(PlayGameCommand)),
                    new KeyValuePair<string, Type>("default", typeof(AnswerCommand))
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

            builder.RegisterType<DbRepository<User>>()
                .As<IDbRepository<User>>();

            builder.RegisterType<DbRepository<Answer>>()
                .As<IDbRepository<Answer>>();

            builder.RegisterType<QuestionsRepository>()
                .As<IDbRepository<Question>>();

            builder.RegisterType<UserAnswerRepository>()
                .As<IUserAnswerRepository>();

            builder.RegisterType<UserService>()
                .As<IUserService>();

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

            builder.RegisterType<KeyboardBuilder>().As<IKeyboardBuilder>();

            var admins = Configuration.GetSection("admins").Get<Admin[]>();

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
        }
    }
}
