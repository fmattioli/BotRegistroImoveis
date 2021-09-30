using BotRegistroImoveis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Dominio.Interfaces
{
    public interface ICustasRepositorio
    {
        Task<CustasProtocolo> ObterCustas(string TipoProtocolo, string Protocolo);
    }
}
