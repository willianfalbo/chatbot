using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.Models;
using Chatbot.Common.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        private readonly IAppSettings _appSettings;
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog,
        ILogger<DialogBot<T>> logger, IAppSettings appSettings)
            : base(conversationState, userState, dialog, logger)
        {
            _appSettings = appSettings;
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendWelcomeCardAsync(turnContext, cancellationToken);
                    await SendBotDutiesAsync(turnContext, cancellationToken);
                    await SendBotDetailsAsync(turnContext, cancellationToken);
                    await SendHelpSuggestionsCardAsync(turnContext, cancellationToken);
                }
            }
        }

        private async Task SendWelcomeCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard();
            // card.Title = "Olá, eu sou o BenicioBot!";
            // card.Text = @"Seja bem-vindo, eu sou o novo Assistente Virtual da Casa do Crédito.";
            var imageUrl = $"{_appSettings.DefaultRootUrl}/images/avatar/greeting.png";
            card.Images = new List<CardImage>() { new CardImage(imageUrl) };

            var response = MessageFactory.Attachment(card.ToAttachment());

            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        private async Task SendBotDutiesAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Seja bem-vindo! Eu sou o novo Assistente Virtual da Casa do Crédito.");
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task SendBotDetailsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("A minha função é ajudá-lo a obter o seu microcrédito de modo interativo.");
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task SendHelpSuggestionsCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new ThumbnailCard();
            card.Text = "Como eu posso te ajudar?";
            card.Buttons =
                UserPreference.ChatbotOptions().Select(option =>
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