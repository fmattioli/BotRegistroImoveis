using BotRegistroImoveis.Aplicacao.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.Servicos
{
    public class UtilitarioService<T> : IUtilitarioServico<T> where T : class
    {
        public T DesserializarClasse(string Json)
        {
            return JsonSerializer.Deserialize<T>(Json);
        }

        
    }
}
