using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using BotRegistroImoveis.Bot.Dialogs.Certidao;
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
        public CertidaoDialog(GerenciarCards gerenciadorCards, ConsultarSelosCertidaoDialog consultarSelosCertidaoDialog, ConsultarCustasCertidao consultarCustasCertidao, ICertidaoServico utilitario)
            : base(nameof(CertidaoDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _certidaoServico = utilitario;
            AddDialog(consultarSelosCertidaoDialog);
            AddDialog(consultarCustasCertidao);
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

            stepContext.Values.Add("ConsultaViewModel", consulta);

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
            var consulta = (ConsultaViewModel)stepContext.Values["ConsultaViewModel"];
            string respostaCard = stepContext.Result?.ToString();
            var certidaoViewModel = _certidaoServico.DesserializarClasse(respostaCard);
            certidaoViewModel.PedidoCertidao = consulta.PedidoCertidao;

            if (!certidaoViewModel.EscolhaInvalida)
            {
                switch (certidaoViewModel.Opcao)
                {
                    case "ConsultarSeloCertidao":
                        return await stepContext.BeginDialogAsync(nameof(ConsultarSelosCertidaoDialog), certidaoViewModel, cancellationToken);
                    default:
                        return await stepContext.BeginDialogAsync(nameof(ConsultarCustasCertidao), certidaoViewModel, cancellationToken);
                }
            }

            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Infelizmente não consegui entender o que você disse \U0001F629. Parece que você tentou voltar em um outro contexto da conversa. \U0001F609");
            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Vou te redirecionar para o menu principal novamente...");
            return await DialogoComum.RetornarAoFluxoPrincipalDevidoAErroDoUsuario(stepContext, "", cancellationToken, InitialDialogId);

        }

    }
}
