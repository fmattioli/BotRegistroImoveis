using AdaptiveCards.Templating;
using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using BotRegistroImoveis.Bot.Dialogs.Titulo;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs.Titulo
{
    public class TituloDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly ITituloServico _tituloServico;
        public TituloDialog(GerenciarCards gerenciadorCards, ContraditorioDialog contraditorioDialog, ConsultarSelosDialog consultarSelos, CustasTituloDialog custasTituloDialog, ITituloServico utilitario)
            : base(nameof(TituloDialog))
        {
            _tituloServico = utilitario;
            _gerenciadorCards = gerenciadorCards;
            AddDialog(contraditorioDialog);
            AddDialog(consultarSelos);
            AddDialog(custasTituloDialog);
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
            
            var consulta = stepContext.Options as ConsultaViewModel;
            if (consulta != null)
                consulta = (ConsultaViewModel)stepContext.Options;

            stepContext.Values.Add("ConsultaViewModel", consulta);
            var listaJsons = new List<string>();

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
            var consulta = (ConsultaViewModel)stepContext.Values["ConsultaViewModel"];
            string respostaCard = stepContext.Result?.ToString();
            var tituloViewModel = _tituloServico.DesserializarClasse(respostaCard);
            tituloViewModel.Protocolo = consulta.Protocolo;
            tituloViewModel.TipoPrenotacao = consulta.TipoPrenotacao;
            
            if (!tituloViewModel.EscolhaInvalida)
            {
                switch (tituloViewModel.Opcao)
                {
                    case "Contraditorio":
                        return await stepContext.BeginDialogAsync(nameof(ContraditorioDialog), tituloViewModel, cancellationToken);
                    case "ConsultarSeloTitulo":
                        return await stepContext.BeginDialogAsync(nameof(ConsultarSelosDialog), tituloViewModel, cancellationToken);
                    case "CustasTitulo":
                        return await stepContext.BeginDialogAsync(nameof(CustasTituloDialog), tituloViewModel, cancellationToken);
                    default:
                        break;
                }
            }

            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Infelizmente não consegui entender o que você disse \U0001F629. Parece que você tentou voltar em um outro contexto da conversa. \U0001F609");
            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Vou te redirecionar para o menu principal novamente...");
            return await DialogoComum.RetornarAoFluxoPrincipalDevidoAErroDoUsuario(stepContext, "", cancellationToken, InitialDialogId);

        }


    }
}
