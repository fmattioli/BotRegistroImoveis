using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
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
        private readonly IUtilitarioService _utilitario;
        public ConsultaDialog(GerenciarCards gerenciadorCards, TituloDialog tituloCertidaoDialog, MatriculaDialog matriculaDialog, IUtilitarioService utilitario)
            : base(nameof(ConsultaDialog))
        {
            _utilitario = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(tituloCertidaoDialog);
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
            await DialogoComum.CriarEEnviarMensagem(stepContext, cancellationToken, "Escolha abaixo qual opção você deseja utilizar! \U0001F609");
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
            if (await _utilitario.JsonValido(respostaCard))
            {
                ConsultaViewModel consulta = JsonConvert.DeserializeObject<ConsultaViewModel>(respostaCard);
                consulta.OpcaoSelecionada = RetornarOpcaoBaseadaNoCardEscolhido(consulta);
                switch (consulta.OpcaoSelecionada)
                {
                    case "Titulo":
                        return await stepContext.BeginDialogAsync(nameof(TituloDialog), consulta, cancellationToken);
                    case "Certidao":
                        return await stepContext.BeginDialogAsync(nameof(MatriculaDialog), consulta, cancellationToken);
                    case "Matricula":
                        return await stepContext.BeginDialogAsync(nameof(MatriculaDialog), consulta, cancellationToken);
                    default:
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, msgErro, cancellationToken);
                }
            }
            return await stepContext.ReplaceDialogAsync(InitialDialogId, msgErro, cancellationToken);
        }

        private string RetornarOpcaoBaseadaNoCardEscolhido(ConsultaViewModel consulta)
        {
            if (!string.IsNullOrEmpty(consulta.TipoPrenotacao))
                return "Titulo";

            if (!string.IsNullOrEmpty(consulta.BuscarNumeroPedidoDeCertidao))
                return "Certidao";

            if (!string.IsNullOrEmpty(consulta.TipoBuscaLivroParticipante))
                return "Matricula";

            return "";

        }
    }
}

