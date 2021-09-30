using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs
{
    public static class DialogoComum
    {
        public static async Task AcaoDigitando(WaterfallStepContext stepContext)
        {
            ITypingActivity replyActivity = Activity.CreateTypingActivity();
            await stepContext.Context.SendActivityAsync((Activity)replyActivity);
            await Task.Delay(2000);
        }

        public static async Task<DialogTurnResult> ExibirMensagemDevidoAMalUsoPorParteDoUsuario(WaterfallStepContext stepContext, string msg, CancellationToken cancellationToken, string InitialDialogId)
        {
            return await stepContext.ReplaceDialogAsync(InitialDialogId, msg, cancellationToken);
        }


    }
}
