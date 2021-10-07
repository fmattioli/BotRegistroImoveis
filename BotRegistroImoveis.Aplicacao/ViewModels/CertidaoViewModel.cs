using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.ViewModels
{
    public class CertidaoViewModel
    {
        public string Opcao { get; set; }
        public string TipoCertidao { get; set; }
        public string PedidoCertidao { get; set; }

        public bool EscolhaInvalida
        {
            get
            {
                return string.IsNullOrEmpty(Opcao);
            }
        }
    }
}
