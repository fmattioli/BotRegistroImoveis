using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs.Titulo
{
    public class ContraditorioDialog : CancelAndHelpDialog
    {

        private readonly GerenciarCards _gerenciadorCards;
        private readonly ITituloServico _tituloServico;
        public ContraditorioDialog(GerenciarCards gerenciadorCards, ITituloServico utilitario)
            : base(nameof(ContraditorioDialog))
        {
            _tituloServico = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirContraditorioTitulo,
                ExibirOpcoesVoltar,
                VoltarAoMenuPrincipalDeTitulos

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> ExibirContraditorioTitulo(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var tituloViewModel = stepContext.Options as TituloViewModel;
            if (tituloViewModel != null)
                tituloViewModel = (TituloViewModel)stepContext.Options;

            stepContext.Values.Add("TituloViewModel", tituloViewModel);
            var listaJsons = new List<string>();
            //cardResumoContraditorio
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardResumoContraditorio");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(tituloViewModel, templateJson));

            await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBindingMesclarDados(listaJsons), cancellationToken);

            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Caso já tenha finalizado, basta escolher a opção desejado abaixo:");
            return await stepContext.ContinueDialogAsync(cancellationToken);

        }

        private async Task<DialogTurnResult> ExibirOpcoesVoltar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarAdaptiveCardBindingSemMesclagem(_gerenciadorCards.RetornarConteudoJson("cardOpcoesVoltar")), cancellationToken);
        }


        private async Task<DialogTurnResult> VoltarAoMenuPrincipalDeTitulos(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var consultas = _tituloServico.DesserializarClasse(stepContext.Result.ToString());
            var tituloViewModel = (TituloViewModel)stepContext.Values["TituloViewModel"];
            var consultaViewModel = new ConsultaViewModel
            {
                Protocolo = tituloViewModel.Protocolo,
                TipoPrenotacao = tituloViewModel.TipoPrenotacao
            };

            switch (consultas.Opcao.Trim())
            {
                case "Anterior":
                    await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Certo, vou te redirecionar para o menu de opções de consultas de títulos");
                    return await stepContext.BeginDialogAsync(nameof(TituloDialog), consultaViewModel, cancellationToken);
                default:
                    await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Certo, vou te redirecionar para o menu principal");
                    return await stepContext.BeginDialogAsync(nameof(ConsultaDialog), consultaViewModel, cancellationToken);

            }

        }


    }
}
