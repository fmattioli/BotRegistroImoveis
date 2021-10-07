using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs.Matricula
{
    public class UltimosAtosDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IMatriculaServico _matriculaServico;
        public UltimosAtosDialog(GerenciarCards gerenciadorCards, IMatriculaServico utilitario)
            : base(nameof(UltimasCertidoesDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _matriculaServico = utilitario;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirUltimosAtos,
                Retornar
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> ExibirUltimosAtos(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var certidaoViewModel = stepContext.Options as MatriculaViewModel;
            if (certidaoViewModel != null)
                certidaoViewModel = (MatriculaViewModel)stepContext.Options;

            stepContext.Values.Add("CertidaoViewModel", certidaoViewModel);
            var listaJsons = new List<string>();
            //cardResumoContraditorio
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardResumoCustasCertidao");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(certidaoViewModel, templateJson));

            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);

        }

        private async Task<DialogTurnResult> Retornar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var consultas = _matriculaServico.DesserializarClasse(stepContext.Result.ToString());
            var certidaoViewModel = (CertidaoViewModel)stepContext.Values["CertidaoViewModel"];
            var consultaViewModel = new ConsultaViewModel
            {
                PedidoCertidao = certidaoViewModel.PedidoCertidao
            };

            switch (consultas.Opcao.Trim())
            {
                case "Anterior":
                    await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Certo, vou te redirecionar para o menu de opções de consultas de pedido certidão");
                    return await stepContext.BeginDialogAsync(nameof(CertidaoDialog), consultaViewModel, cancellationToken);
                default:
                    await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Certo, vou te redirecionar para o menu principal");
                    return await stepContext.BeginDialogAsync(nameof(ConsultaDialog), consultaViewModel, cancellationToken);

            }
        }
    }
}
