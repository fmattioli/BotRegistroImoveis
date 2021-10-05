using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs
{
    public class CertidaoDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly ICertidaoServico _certidaoServico;
        public CertidaoDialog(GerenciarCards gerenciadorCards, ICertidaoServico utilitario)
            : base(nameof(CertidaoDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _certidaoServico = utilitario;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirOpcoesConsultasCertidao,
                ProcessarOpcaoSelecionada
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ExibirOpcoesConsultasCertidao(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var listaJsons = new List<string>();
            var consulta = (ConsultaViewModel)stepContext.Options;
            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, $"Entendido! abaixo você encontra as consultas disponíveis para a certidão: {consulta.PedidoCertidao}! \U0001F609");

            //cardConsultarSelosTitulos
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardConsultarSelosCertidao");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            ////cardConsultarCustasProtocolo
            templateJson = _gerenciadorCards.RetornarConteudoJson("cardConsultarCustasCertidao");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessarOpcaoSelecionada(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            var certidaoViewModel = _certidaoServico.DesserializarClasse(respostaCard);
            if (certidaoViewModel is not null)
            {
                switch (certidaoViewModel.OpcaoSelecionada)
                {
                    case "Custas":
                        return await stepContext.BeginDialogAsync(nameof(ConsultaDialog), null, cancellationToken);
                    default:
                        break;
                }
            }

            var msg = "Infelizmente não consegui entender o que você disse \U0001F629. Vamos começar novamente, selecione abaixo opção que você deseja utilizar, combinado? \U0001F609";
            return await DialogoComum.RetornarAoFluxoPrincipalDevidoAErroDoUsuario(stepContext, msg, cancellationToken, InitialDialogId);

        }

    }
}
