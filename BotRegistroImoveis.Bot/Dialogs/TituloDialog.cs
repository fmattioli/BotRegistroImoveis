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
    public class TituloDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly ITituloServico _tituloServico;
        public TituloDialog(GerenciarCards gerenciadorCards, ITituloServico utilitario)
            : base(nameof(TituloDialog))
        {
            _tituloServico = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirOpcoesConsultasTitulo,
                ProcessarOpcaoSelecionada
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> ExibirOpcoesConsultasTitulo(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var listaJsons = new List<string>();
            var consulta = (ConsultaViewModel)stepContext.Options;
            await DialogoComum.CriarEEnviarMensagem(stepContext, cancellationToken, $"Entendido! abaixo você encontra as consultas disponíveis para o título: {consulta.Protocolo}! \U0001F609");

            //cardConsultarContraditorio
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardConsultarContraditorio");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            //cardConsultarSelosTitulos
            templateJson = _gerenciadorCards.RetornarConteudoJson("cardConsultarSelosTitulos");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));

            ////cardConsultarCustasProtocolo
            templateJson = _gerenciadorCards.RetornarConteudoJson("cardConsultarCustasProtocolo");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(consulta, templateJson));



            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBinding(listaJsons), cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessarOpcaoSelecionada(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            var tituloViewModel = _tituloServico.DesserializarClasse(respostaCard);
            if (tituloViewModel is not null)
            {
                switch (tituloViewModel.OpcaoSelecionada)
                {
                    case "Custas":
                        return await stepContext.BeginDialogAsync(nameof(ConsultaDialog), null, cancellationToken);
                    default:
                        break;
                }
            }

            var msg = "Infelizmente não consegui entender o que você disse \U0001F629. Vamos começar novamente, selecione abaixo opção que você deseja utilizar, combinado? \U0001F609";
            return await DialogoComum.ExibirMensagemDevidoAMalUsoPorParteDoUsuario(stepContext, msg, cancellationToken, InitialDialogId);

        }

        
    }
}
