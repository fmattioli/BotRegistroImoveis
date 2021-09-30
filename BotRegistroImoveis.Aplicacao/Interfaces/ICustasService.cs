using BotRegistroImoveis.Aplicacao.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.Interfaces
{
    public interface ICustasService
    {
        Task<CustasProtocolo> ObterCustas(string TipoProtocolo, string Protocolo);
    }
}
