using BotRegistroImoveis.Aplicacao.Interfaces;
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
            services.AddTransient<IConsultaServico, ConsultaServico>();
            services.AddTransient<ITituloServico, TituloServico>();
            services.AddTransient<ICertidaoServico, CertidaoServico>();
            services.AddTransient<IMatriculaServico, MatriculaServico>();


            return services;
        }
    }
}
