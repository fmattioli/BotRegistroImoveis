﻿using BotRegistroImoveis.Aplicacao.Interfaces;
using BotRegistroImoveis.Aplicacao.ViewModels;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using BotRegistroImoveis.Bot.Dialogs.Comum;
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
        public UltimosAtosDialog(GerenciarCards gerenciadorCards, BotoesVoltarDialog botoesVoltarDialog, IMatriculaServico utilitario)
            : base(nameof(UltimosAtosDialog))
        {
            _gerenciadorCards = gerenciadorCards;
            _matriculaServico = utilitario;
            AddDialog(botoesVoltarDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExibirUltimosAtos,
                ExibirOpcoesVoltar,
                Retornar
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> ExibirUltimosAtos(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var matriculaViewModel = stepContext.Options as MatriculaViewModel;
            if (matriculaViewModel != null)
                matriculaViewModel = (MatriculaViewModel)stepContext.Options;

            stepContext.Values.Add("MatriculaViewModel", matriculaViewModel);
            var listaJsons = new List<string>();
            //cardResumoContraditorio
            var templateJson = _gerenciadorCards.RetornarConteudoJson("cardResultadoUltimosAtos");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));

            templateJson = _gerenciadorCards.RetornarConteudoJson("cardResultadoUltimosAtos2");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));

            templateJson = _gerenciadorCards.RetornarConteudoJson("cardResultadoUltimosAtos3");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));

            templateJson = _gerenciadorCards.RetornarConteudoJson("cardResultadoUltimosAtos4");
            listaJsons.Add(DialogoComum.MesclarDadosParaExibirNoCard(matriculaViewModel, templateJson));

            await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarListaAdaptiveCardBindingMesclarDados(listaJsons), cancellationToken);

            await DialogoComum.EnviarMensagem(stepContext, cancellationToken, "Caso já tenha finalizado, basta escolher a opção desejado abaixo:");

            
            return await stepContext.ContinueDialogAsync(cancellationToken);

        }

        private async Task<DialogTurnResult> ExibirOpcoesVoltar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), _gerenciadorCards.CriarAdaptiveCardBindingSemMesclagem(_gerenciadorCards.RetornarConteudoJson("cardOpcoesVoltar")), cancellationToken);
        } 
        
        private async Task<DialogTurnResult> Retornar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var consultas = _matriculaServico.DesserializarClasse(stepContext.Result.ToString());
            var matriculaViewModel = (MatriculaViewModel)stepContext.Values["MatriculaViewModel"];
            var consultaViewModel = new ConsultaViewModel
            {
                NumeroLivro = matriculaViewModel.NumeroLivro,
                TipoLivro = matriculaViewModel.TipoLivro
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
