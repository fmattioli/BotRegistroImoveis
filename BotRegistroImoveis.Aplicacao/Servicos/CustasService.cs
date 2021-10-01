using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Dominio.Interfaces;
using System;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.Servicos
{
    public class CustasService : ICustasService
    {
        private readonly ICustasRepositorio custasRepositorio;
        public CustasService(ICustasRepositorio custasRepositorio)
        {
            this.custasRepositorio = custasRepositorio;
        }
        public async Task<CustasProtocoloViewModel> ObterCustas(string TipoProtocolo, string Protocolo)
        {
            var custas = await custasRepositorio.ObterCustas(TipoProtocolo, Protocolo);
            return new CustasProtocoloViewModel()
            {
                Protocolo = custas.Protocolo,
                Data_Expira = custas.Data_Expira,
                Data_Recepcao = custas.Data_Recepcao,
                Natureza = custas.Natureza
            };
        }
    }
}
