﻿using AdaptiveCards.Templating;
using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
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
    public class CustasDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IUtilitarioService _utilitario;
        private readonly ICustasService _custasService;
        public CustasDialog(GerenciarCards gerenciadorCards, IUtilitarioService utilitario, ICustasService custasService)
            : base(nameof(CustasDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _utilitario = utilitario;
            _custasService = custasService;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirCardConsultarCustas,
                ProcessarCardCustas
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ExibirCardConsultarCustas(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var mensagemInicio = stepContext.Options?.ToString() ?? "Vai ser um prazer te ajudar! Clique em 'utilizar' e preencha as infos necessárias que eu irei retornar as custas para você! \U0001F609";
            await DialogoComum.AcaoDigitando(stepContext);
            var response = MessageFactory.Text(mensagemInicio);
            await stepContext.Context.SendActivityAsync(response, cancellationToken);

            var welcomeCard = _gerenciadorCards.RetornarAdaptiveCard(
                new List<string>()
                {
                    "cardObterCustas"
                });

            return await stepContext.PromptAsync(nameof(TextPrompt), welcomeCard, cancellationToken);
        }


        private async Task<DialogTurnResult> ProcessarCardCustas(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string respostaCard = stepContext.Result?.ToString();
            if (await _utilitario.JsonValido(respostaCard))
            {
                ProtocoloViewModel protocoloViewModel = JsonConvert.DeserializeObject<ProtocoloViewModel>(respostaCard);
                var templateJson = _gerenciadorCards.RetornarConteudoJson("cardResumoCustas");

                AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);
                var custasModel = await _custasService.ObterCustas(protocoloViewModel.Tipo, protocoloViewModel.Numero);
                var myData = new CustasProtocolo
                {
                    Protocolo = custasModel.Protocolo,
                    Data_Expira = custasModel.Data_Expira,
                    Data_Recepcao = custasModel.Data_Recepcao,
                    Natureza = custasModel.Natureza
                };

                // "Expand" the template - this generates the final Adaptive Card payload
                string cardJson = template.Expand(myData);
                return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarAdaptiveCardBinding(cardJson), cancellationToken);
            }

            var msg = "Infelizmente não consegui entender o que você disse \U0001F629 você selecionou por último que desejava consultar as custas de um determinado protocolo, então vamos seguir... \U0001F609";
            return await DialogoComum.ExibirMensagemDevidoAMalUsoPorParteDoUsuario(stepContext, msg, cancellationToken, InitialDialogId);
        }

        
        
    }
}
