using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using BotRegistroImoveis.Bot.Dialogs.Titulo;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs
{
    public class ConsultaDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IConsultaServico _consultaServico;
        public ConsultaDialog(GerenciarCards gerenciadorCards, TituloDialog tituloCertidaoDialog, CertidaoDialog certidaoDialog, MatriculaDialog matriculaDialog, IConsultaServico utilitario)
            : base(nameof(ConsultaDialog))
        {
            _consultaServico = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(tituloCertidaoDialog);
            AddDialog(certidaoDialog);
            AddDialog(matriculaDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirOpcoesConsultas,
                ProcessarOpcaoSelecionada
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> ExibirOpcoesConsultas(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Escolha abaixo qual opção você deseja utilizar! \U0001F609");
            var welcomeCard = _gerenciadorCards.RetornarAdaptiveCard
            (
                new List<string>()
                {
                    "cardConsultarTitulo",
                    "cardConsultarCertidao",
                    "cardConsultasMatricula"
                }
            );

            return await stepContext.PromptAsync(nameof(TextPrompt), welcomeCard, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessarOpcaoSelecionada(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            var msgErro = "Infelizmente não consegui entender o que você disse \U0001F629 selecione abaixo opção que você deseja utilizar, combinado? \U0001F609";
            var consultaViewModel = _consultaServico.DesserializarClasse(respostaCard);
            if (consultaViewModel is not null)
            {
                switch (consultaViewModel.OpcaoSelecionada)
                {
                    case "Titulo":
                        return await stepContext.BeginDialogAsync(nameof(TituloDialog), consultaViewModel, cancellationToken);
                    case "Certidao":
                        return await stepContext.BeginDialogAsync(nameof(CertidaoDialog), consultaViewModel, cancellationToken);
                    case "Matricula":
                        return await stepContext.BeginDialogAsync(nameof(MatriculaDialog), consultaViewModel, cancellationToken);
                    default:
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, msgErro, cancellationToken);
                }
            }
            return await stepContext.ReplaceDialogAsync(InitialDialogId, msgErro, cancellationToken);
        }

       
    }
}

