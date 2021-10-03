using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs
{
    public class CertidaoDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IUtilitarioService _utilitario;
        public CertidaoDialog(GerenciarCards gerenciadorCards, IUtilitarioService utilitario)
            : base(nameof(CertidaoDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _utilitario = utilitario;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirOpcoesConsultasCertidao
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ExibirOpcoesConsultasCertidao(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await DialogoComum.CriarEEnviarMensagem(stepContext, cancellationToken, "Escolha abaixo qual opção você deseja utilizar! \U0001F609");
            var welcomeCard = _gerenciadorCards.RetornarAdaptiveCard
            (
                new List<string>()
                {
                    "cardConsultarTitulo",
                    "cardConsultarCertidao",
                    "cardConsultasMatricula"
                }
            );

            return await stepContext.PromptAsync(nameof(TextPrompt), welcomeCard, cancellationToken);
        }

    }
}
