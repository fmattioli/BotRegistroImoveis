using FluentValidator;
using FluentValidator.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.ViewModels
{
    public class TituloViewModel 
    {
        public string Opcao { get; set; }
        public string TipoPrenotacao { get; set; }
        public string Protocolo { get; set; }

        public bool EscolhaInvalida
        {
            get
            {
                return string.IsNullOrEmpty(Opcao);
            }
        }
    }
}
