using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.ViewModels
{
    public class ConsultaViewModel
    {
        public string Protocolo { get; set; }
        public string TipoPrenotacao { get; set; }
        public string PedidoCertidao { get; set; }
        public string NumeroLivro { get; set; }
        public string TipoLivro { get; set; }
        public string BuscarNumeroPedidoDeCertidao { get; set; }


        public string OpcaoSelecionada
        {
            get 
            {
                if (!string.IsNullOrEmpty(TipoPrenotacao))
                    return "Titulo";

                if (!string.IsNullOrEmpty(BuscarNumeroPedidoDeCertidao))
                    return "Certidao";

                if (!string.IsNullOrEmpty(TipoLivro))
                    return "Matricula";

                return "";
            }
        }
        

    }
}
