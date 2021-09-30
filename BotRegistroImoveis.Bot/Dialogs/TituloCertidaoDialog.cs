using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs
{
    public class TituloCertidaoDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IUtilitarioService _utilitario;
        public TituloCertidaoDialog(GerenciarCards gerenciadorCards, CustasDialog custasDialog, IUtilitarioService utilitario)
            : base(nameof(TituloCertidaoDialog))
        {
            _utilitario = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(custasDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirOpcoesConsultasTituloCertidao,
                ProcessarOpcaoSelecionada
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> ExibirOpcoesConsultasTituloCertidao(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var mensagemInicio = stepContext.Options?.ToString() ?? "Entendido! abaixo você encontra as consultas disponíveis para título e certidões! \U0001F609";
            await DialogoComum.AcaoDigitando(stepContext);
            var response = MessageFactory.Text(mensagemInicio);
            await stepContext.Context.SendActivityAsync(response, cancellationToken);
            var welcomeCard = _gerenciadorCards.RetornarAdaptiveCard
            (
                new List<string>()
                {
                    "cardConsultarCustas",
                    "cardConsultarParticipantes"
                }
            );

            return await stepContext.PromptAsync(nameof(TextPrompt), welcomeCard, cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessarOpcaoSelecionada(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            if (await _utilitario.JsonValido(respostaCard))
            {
                ConsultaViewModel consulta = JsonConvert.DeserializeObject<ConsultaViewModel>(respostaCard);
                switch (consulta.Opcao)
                {
                    case "Custas":
                        return await stepContext.BeginDialogAsync(nameof(CustasDialog), null, cancellationToken);
                    default:
                        break;
                }
            }

            var msg = "Infelizmente não consegui entender o que você disse \U0001F629. Vamos começar novamente, selecione abaixo opção que você deseja utilizar, combinado? \U0001F609";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, msg, cancellationToken);

        }
    }
}
