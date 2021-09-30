using BotRegistroImoveis.Dominio.Entidades;
using BotRegistroImoveis.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Infra.Data.Repositorios
{
    public class CustasRepositorio : ICustasRepositorio
    {
        public async Task<CustasProtocolo> ObterCustas(string TipoProtocolo, string Protocolo)
        {
            var custas = new CustasProtocolo();
            await Task.Run(() =>
            {
                custas.Protocolo = Protocolo;
                custas.Natureza = "ESCRITURA PUBLICA";
                custas.Data_Expira = "01/01/2021";
                custas.Data_Recepcao = "10/01/2021";
            });

            return custas;

        }
    }
}
