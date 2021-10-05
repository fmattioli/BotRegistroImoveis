using AdaptiveCards.Templating;
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
    public class MatriculaDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IMatriculaServico _matriculaServico;
        public MatriculaDialog(GerenciarCards gerenciadorCards, IMatriculaServico utilitario)
            : base(nameof(MatriculaDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _matriculaServico = utilitario;
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

            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardUltimasCertidoes");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            templateJson = _gerenciadorCards.RetornarConteudoJson("cardUltimosRegistros");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessarOpcaoSelecionada(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            var matriculaViewModel = _matriculaServico.DesserializarClasse(respostaCard);
            if (matriculaViewModel is not null)
            {
                var listaJsons = new List<string>();
                await DialogoComum.EnviarMensagem(stepContext, cancellationToken, $"Entendido! Aqui estão as opções de buscas para o tipo de livro {(matriculaViewModel.TipoLivro == "1" ? "Matrícula" : "Transcrição")}:  número: {matriculaViewModel.Numero}");
                
                //cardParticipantesMatricula
                var templateJson = _gerenciadorCards.RetornarConteudoJson("cardParticipantesMatricula");
                listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));

                //cardUltimasCertidoes
                templateJson = _gerenciadorCards.RetornarConteudoJson("cardUltimasCertidoes");
                listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));
                
                return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);
            }

            var msg = "Infelizmente não consegui entender o que você disse \U0001F629 você selecionou por último que desejava consultar as custas de um determinado protocolo, então vamos seguir... \U0001F609";
            return await DialogoComum.RetornarAoFluxoPrincipalDevidoAErroDoUsuario(stepContext, msg, cancellationToken, InitialDialogId);
        }

    }
}
