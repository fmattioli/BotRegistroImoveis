using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.ViewModels
{
    public class MatriculaViewModel
    {
        public string Opcao { get; set; }
        public string TipoLivro { get; set; }
        public string TipoConsulta { get; set; }
        public string NumeroLivro { get; set; }

        public bool EscolhaInvalida
        {
            get
            {
                return string.IsNullOrEmpty(Opcao);
            }
        }
    }
}
