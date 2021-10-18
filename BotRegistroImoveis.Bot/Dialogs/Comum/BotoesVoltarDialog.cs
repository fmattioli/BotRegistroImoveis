using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Dialogs.Comum
{
    public class BotoesVoltarDialog : CancelAndHelpDialog
    {
        private readonly GerenciarCards _gerenciadorCards;
        private readonly IMatriculaServico _matriculaServico;
        public BotoesVoltarDialog(GerenciarCards gerenciadorCards, IMatriculaServico utilitario)
            : base(nameof(BotoesVoltarDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _matriculaServico = utilitario;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                Retornar1,
                Retornar
                
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> Retornar1(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(BotoesVoltarDialog), _gerenciadorCards.CriarAdaptiveCardBindingSemMesclagem(_gerenciadorCards.RetornarConteudoJson("cardOpcoesVoltar")), cancellationToken);
        } 
        
        private async Task<DialogTurnResult> Retornar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var consultas = _matriculaServico.DesserializarClasse(stepContext.Result.ToString());
            var certidaoViewModel = (CertidaoViewModel)stepContext.Values["MatriculaViewModel"];
            var consultaViewModel = new ConsultaViewModel
            {
                PedidoCertidao = certidaoViewModel.PedidoCertidao
            };

            switch (consultas.Opcao.Trim())
            {
                case "Anterior":
                    await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Certo, vou te redirecionar para o menu de opções de consultas de pedido certidão");
                    return await stepContext.BeginDialogAsync(nameof(MatriculaDialog), consultaViewModel, cancellationToken);
                default:
                    await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Certo, vou te redirecionar para o menu principal");
                    return await stepContext.BeginDialogAsync(nameof(ConsultaDialog), consultaViewModel, cancellationToken);

            }
        }
    }
}
