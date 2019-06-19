using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.Model.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        private readonly IConfiguration _configuration;
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog,
        ILogger<DialogBot<T>> logger, IConfiguration configuration)
            : base(conversationState, userState, dialog, logger)
        {
            _configuration = configuration;
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
            var response = turnContext.Activity.CreateReply();

            var card = new ThumbnailCard();
            card.Title = "Olá, eu sou o BenicioBot!";
            card.Text = @"Seja bem-vindo, eu sou o novo Assistente Virtual da Casa do Crédito.";
            var imageUrl = $"{_configuration.GetValue<string>("DefaultRootUrl")}/images/avatar_chatbot.png";
            card.Images = new List<CardImage>() { new CardImage(imageUrl) };

            response.Attachments = new List<Attachment>() { card.ToAttachment() };

            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        private async Task SendBotDutiesAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var response = turnContext.Activity.CreateReply();
            response.Text = "A minha função é ajudá-lo a obter o seu microcrédito de modo interativo.";

            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        private async Task SendBotDetailsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var response = turnContext.Activity.CreateReply();
            response.Text = "Para ser mais claro, eu consigo coletar o seu pedido através da nossa conversa e, enviar diretamente para a nossa equipe, que fará a analise do seu caso.";

            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        private async Task SendHelpSuggestionsCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var response = turnContext.Activity.CreateReply();

            var card = new ThumbnailCard();
            card.Text = "Como eu posso ajudá-lo?";
            card.Buttons =
                UserPreference.ChatbotOptions().Select(option =>
                    new CardAction(
                        ActionTypes.ImBack,
                        title: option,
                        value: option
                )).ToList();

            response.Attachments = new List<Attachment>() { card.ToAttachment() };

            await turnContext.SendActivityAsync(response, cancellationToken);
        }

    }
}