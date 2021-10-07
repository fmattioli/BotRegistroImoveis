using AdaptiveCards.Templating;
using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using BotRegistroImoveis.Bot.Dialogs.Matricula;
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
    public class MatriculaDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IMatriculaServico _matriculaServico;
        public MatriculaDialog(GerenciarCards gerenciadorCards, UltimasCertidoesDialog ultimasCertidoesDialog, UltimosAtosDialog ultimosRegistrosDialog,  IMatriculaServico utilitario)
            : base(nameof(MatriculaDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _matriculaServico = utilitario;
            AddDialog(ultimasCertidoesDialog);
            AddDialog(ultimosRegistrosDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirOpcoesConsultasMatricula,
                ProcessarOpcaoSelecionada
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ExibirOpcoesConsultasMatricula(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var listaJsons = new List<string>();
            var consulta = (ConsultaViewModel)stepContext.Options;
            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, $"Entendido! abaixo você encontra as consultas disponíveis para ${consulta.TipoLivro}-{consulta.NumeroLivro}! \U0001F609");

            stepContext.Values.Add("ConsultaViewModel", consulta);

            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardUltimasCertidoes");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            templateJson = _gerenciadorCards.RetornarConteudoJson("cardUltimosRegistros");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessarOpcaoSelecionada(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var consulta = (ConsultaViewModel)stepContext.Values["ConsultaViewModel"];
            string respostaCard = stepContext.Result?.ToString();
            var matriculaViewModel = _matriculaServico.DesserializarClasse(respostaCard);
            matriculaViewModel.NumeroLivro = consulta.NumeroLivro;
            matriculaViewModel.TipoLivro = consulta.TipoLivro;

            if (!matriculaViewModel.EscolhaInvalida)
            {
                switch (matriculaViewModel.Opcao)
                {
                    case "UltimasCertidoes":
                        return await stepContext.BeginDialogAsync(nameof(UltimasCertidoesDialog), matriculaViewModel, cancellationToken);
                    default:
                        return await stepContext.BeginDialogAsync(nameof(UltimosAtosDialog), matriculaViewModel, cancellationToken);
                }
            }

            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Infelizmente não consegui entender o que você disse \U0001F629. Parece que você tentou voltar em um outro contexto da conversa. \U0001F609");
            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Vou te redirecionar para o menu principal novamente...");
            return await DialogoComum.RetornarAoFluxoPrincipalDevidoAErroDoUsuario(stepContext, "", cancellationToken, InitialDialogId);


        }



    }
}
