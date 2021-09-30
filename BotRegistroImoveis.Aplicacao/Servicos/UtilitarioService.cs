using BotRegistroImoveis.Aplicacao.Interfaces;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.Servicos
{
    public class UtilitarioService : IUtilitarioService
    {
        public async Task<bool> JsonValido(string Json)
        {
            bool jsonValido = false;

            await Task.Run(() =>
            {
                Json = Json.Trim();
                jsonValido = Json.StartsWith("{") && Json.EndsWith("}")
                       || Json.StartsWith("[") && Json.EndsWith("]");
            });

            return jsonValido;
        }
    }
}
