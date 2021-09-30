﻿using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.Servicos;
using BotRegistroImoveis.Dominio.Interfaces;
using BotRegistroImoveis.Infra.Data.Repositorios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BotRegistroImoveis.Infra.IoC
{
    public static class DependecyInjection
    {
        public static IServiceCollection AdicionarInfraEstrutura(this IServiceCollection services)
        {
            //Add injeção de classes
            services.AddTransient<IUtilitarioService, UtilitarioService>();
            services.AddTransient<ICustasService, CustasService>();
            services.AddTransient<ICustasRepositorio, CustasRepositorio>();


            return services;
        }
    }
}
