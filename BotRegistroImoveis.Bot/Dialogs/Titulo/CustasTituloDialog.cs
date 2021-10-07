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
    public class CustasTituloDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly ITituloServico _tituloServico;
        public CustasTituloDialog(GerenciarCards gerenciadorCards, ITituloServico utilitario)
            : base(nameof(CustasTituloDialog))
        {
            _tituloServico = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirContraditorioTitulo,
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
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardResumoCustas");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(tituloViewModel, templateJson));

            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);

        }

        private async Task<DialogTurnResult> VoltarAoMenuPrincipalDeTitulos(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await RetornarParaMenuAnteriorOpcaoTitulo(stepContext, cancellationToken);

        }

        private async Task<DialogTurnResult> RetornarParaMenuAnteriorOpcaoTitulo(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
