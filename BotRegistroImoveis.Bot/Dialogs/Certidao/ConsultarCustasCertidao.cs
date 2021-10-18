using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs.Certidao
{
    public class ConsultarCustasCertidao : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly ICertidaoServico certidaoServico;
        public ConsultarCustasCertidao(GerenciarCards gerenciadorCards, ICertidaoServico utilitario)
            : base(nameof(ConsultarCustasCertidao))
        {
            certidaoServico = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirCustasCertidao,
                ExibirOpcoesVoltar,
                VoltarAoMenuPrincipalDePedidoCertidao

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> ExibirCustasCertidao(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var certidaoViewModel = stepContext.Options as CertidaoViewModel;
            if (certidaoViewModel != null)
                certidaoViewModel = (CertidaoViewModel)stepContext.Options;

            stepContext.Values.Add("CertidaoViewModel", certidaoViewModel);
            var listaJsons = new List<string>();
            //cardResumoContraditorio
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardResumoCustasCertidao");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(certidaoViewModel, templateJson));

            await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBindingMesclarDados(listaJsons), cancellationToken);

            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Caso já tenha finalizado, basta escolher a opção desejado abaixo:");
            return await stepContext.ContinueDialogAsync(cancellationToken);

        }

        private async Task<DialogTurnResult> ExibirOpcoesVoltar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarAdaptiveCardBindingSemMesclagem(_gerenciadorCards.RetornarConteudoJson("cardOpcoesVoltar")), cancellationToken);
        }

        private async Task<DialogTurnResult> VoltarAoMenuPrincipalDePedidoCertidao(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var consultas = certidaoServico.DesserializarClasse(stepContext.Result.ToString());
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
