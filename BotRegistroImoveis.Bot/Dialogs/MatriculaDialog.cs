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
        private readonly IUtilitarioService _utilitario;
        public MatriculaDialog(GerenciarCards gerenciadorCards, IUtilitarioService utilitario, ICustasService custasService)
            : base(nameof(MatriculaDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _utilitario = utilitario;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ObterInfomarmacoesSobreTipoBusca,
                ExibirOpcoesDeBusca
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ObterInfomarmacoesSobreTipoBusca(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await DialogoComum.CriarEEnviarMensagem(stepContext, cancellationToken, "Legal, me informa o tipo do livro e escolha uma opção de consulta. Combinado? \U0001F609");
            var welcomeCard = _gerenciadorCards.RetornarAdaptiveCard
            (
                new List<string>()
                {
                    "cardOpcoesMatricula"
                }
            );

            return await stepContext.PromptAsync(nameof(TextPrompt), welcomeCard, cancellationToken);

        }

        private async Task<DialogTurnResult> ExibirOpcoesDeBusca(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            if (await _utilitario.JsonValido(respostaCard))
            {
                var listaJsons = new List<string>();
                MatriculaViewModel matriculaViewModel = JsonConvert.DeserializeObject<MatriculaViewModel>(respostaCard);
                await DialogoComum.CriarEEnviarMensagem(stepContext, cancellationToken, $"Entendido! Aqui estão as opções de buscas para o tipo de livro {(matriculaViewModel.TipoLivro == "1" ? "Matrícula" : "Transcrição")}:  número: {matriculaViewModel.Numero}");
                
                //cardParticipantesMatricula
                var templateJson = _gerenciadorCards.RetornarConteudoJson("cardParticipantesMatricula");
                listaJsons.Add(MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));

                //cardUltimasCertidoes
                templateJson = _gerenciadorCards.RetornarConteudoJson("cardUltimasCertidoes");
                listaJsons.Add(MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));
                
                return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);
            }

            var msg = "Infelizmente não consegui entender o que você disse \U0001F629 você selecionou por último que desejava consultar as custas de um determinado protocolo, então vamos seguir... \U0001F609";
            return await DialogoComum.ExibirMensagemDevidoAMalUsoPorParteDoUsuario(stepContext, msg, cancellationToken, InitialDialogId);
        }

        private string MesclarDadosParaExibirNoCard(MatriculaViewModel matriculaViewModel, string templateJson)
        {
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);
            var dadosMesclar = matriculaViewModel;
            return template.Expand(dadosMesclar);
        }
    }
}
