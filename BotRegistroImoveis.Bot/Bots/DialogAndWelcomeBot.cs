using BotRegistroImoveis.Bot.Cards;
using BotRegistroImoveis.Bot.Cards.Gerenciador;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Bot.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T>
        where T : Dialog
    {
        private readonly GerenciarCards gerenciarCards;
        public DialogAndWelcomeBot(ConversationState conversationState, GerenciarCards gerenciarCards, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
            this.gerenciarCards = gerenciarCards;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    ITypingActivity replyActivity = Activity.CreateTypingActivity();
                    await turnContext.SendActivityAsync((Activity)replyActivity);
                    await Task.Delay(2000);
                    var response = MessageFactory.Text("Olá, seja bem vindo! Sou o assistente virtual do RI e estou pronto pra te ajudar! \U0001F601");
                    await turnContext.SendActivityAsync(response, cancellationToken);
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }

        
    }
}

