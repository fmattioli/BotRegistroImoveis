using AdaptiveCards.Templating;
using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Dialogs.Titulo;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs
{
    /// <summary>
    /// Classe que contém métodos úteis que serão utilizados muitas vezes dentro de uma conversação.
    /// </summary>
    public static class DialogoComum
    {
       
        public static async Task AcaoDigitando(WaterfallStepContext stepContext)
        {
            ITypingActivity replyActivity = Activity.CreateTypingActivity();
            await stepContext.Context.SendActivityAsync((Activity)replyActivity);
            await Task.Delay(1500);
        }

        /// <summary>
        /// Quando o envio de mensagens está disponível, o usuário pode mandar uma mensagem sem contexto, este método retorna o último dialogo que foi entendido pela aplicação.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="msg"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="InitialDialogId"></param>
        /// <returns></returns>
        public static async Task<DialogTurnResult> RetornarAoFluxoPrincipalDevidoAErroDoUsuario(WaterfallStepContext stepContext, string msg, CancellationToken cancellationToken, string InitialDialogId)
        {
            return await stepContext.BeginDialogAsync(nameof(ConsultaDialog), null, cancellationToken);
        }

        /// <summary>
        /// Método responsável por criar e enviar uma mensagem para o usuário dentro da conversa.
        /// </summary>
        /// <param name="stepContext">stepContext stepContext stepContext</param>
        /// <param name="cancellationToken"></param>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public static async Task EnviarMensagem(WaterfallStepContext stepContext, CancellationToken cancellationToken, string mensagem)
        {
            await DialogoComum.AcaoDigitando(stepContext);
            var response = MessageFactory.Text(mensagem);
            await stepContext.Context.SendActivityAsync(response, cancellationToken);
        }

        public static string MesclarDadosParaExibirNoCard(object objeto, string templateJson)
        {
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);
            var dadosMesclar = objeto;
            return template.Expand(dadosMesclar);
        }




    }
}
