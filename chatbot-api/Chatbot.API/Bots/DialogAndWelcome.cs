using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.Api.DTO;
using Chatbot.Api.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Chatbot.Api.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        public DialogAndWelcomeBot(IDialogHelper dialogHelper, T dialog,
        ILogger<DialogBot<T>> logger)
            : base(dialogHelper, dialog, logger)
        {

        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(turnContext, cancellationToken);

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendHelpSuggestionsCardAsync(turnContext, cancellationToken);
                }
            }
        }

        private async Task SendHelpSuggestionsCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new ThumbnailCard();
            card.Subtitle = "Seja bem-vindo! Eu sou o seu novo Assistente Virtual.\nA minha função é ajudá-lo a obter o seu microcrédito de modo interativo. Como eu posso te ajudar?";
            card.Buttons =
                UserPreferenceDTO.ChatbotOptions().Select(option =>
                    new CardAction(
                        ActionTypes.ImBack,
                        title: option,
                        value: option
                )).ToList();

            var response = MessageFactory.Attachment(card.ToAttachment());

            await turnContext.SendActivityAsync(response, cancellationToken);
        }

    }
}