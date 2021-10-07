﻿using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.Servicos;
using BotRegistroImoveis.Bot.Bots;
using BotRegistroImoveis.Bot.Cards;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using BotRegistroImoveis.Bot.Dialogs;
using BotRegistroImoveis.Bot.Dialogs.Certidao;
using BotRegistroImoveis.Bot.Dialogs.Matricula;
using BotRegistroImoveis.Bot.Dialogs.Titulo;
using BotRegistroImoveis.Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotRegistroImoveis.Bot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();
            services.AddSingleton<GerenciarCards>();
            services.AddSingleton<ConsultaDialog>();

            services.AddSingleton<TituloDialog>();
            services.AddSingleton<ContraditorioDialog>();
            services.AddSingleton<ConsultarSelosDialog>();
            services.AddSingleton<CustasTituloDialog>();

            services.AddSingleton<CertidaoDialog>();
            services.AddSingleton<ConsultarCustasCertidao>();
            services.AddSingleton<ConsultarSelosCertidaoDialog>();

            services.AddSingleton<MatriculaDialog>();
            services.AddSingleton<UltimosAtosDialog>();
            services.AddSingleton<UltimasCertidoesDialog>();
            services.AddSingleton<MatriculaDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
            services.AdicionarInfraEstrutura();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

        }
    }
}
